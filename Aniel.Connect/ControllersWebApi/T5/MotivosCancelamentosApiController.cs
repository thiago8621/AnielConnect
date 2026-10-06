using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class MotivosCancelamentosApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/MotivosCancelamentosapi/GetAll")]
        #region [GetAll]
        public async Task<DataTableResponse<MotivosCancelamentos_ListModels>> GetAll(MotivosCancelamentos_DataTableRequest request)
        {
            List<MotivosCancelamentos_ListModels> retorno = new List<MotivosCancelamentos_ListModels>();

            Int32 recordsTotal = 0;
            try
            {
                var acesso = new AcessoApiController();
                var areas = await acesso.GetArea_ModuloUsuario(User.Identity.GetUserId());
                bool temAcesso356 = Core.Helpers.AcessoHelper.TemAcesso(areas.Objeto, "389");

                if (!temAcesso356)
                {
                    return new DataTableResponse<MotivosCancelamentos_ListModels>
                    {
                        draw = 0,
                        recordsFiltered = recordsTotal,
                        data = null,
                        error = "Você não possui acesso ao Visualizar Motivo de Cancelamento, área 389"
                    };
                }

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MOTIVOS_CANCELAMENTOS");
                    StringBuilder sql = new StringBuilder();

                    #region [FILTROS]
                    List<String> filtros = new List<string>();

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        filtros.Add("STATUS = @STATUS");
                        parametros.Add("@STATUS", request.Status);
                    }

                    if (!String.IsNullOrEmpty(request.search.value) || !String.IsNullOrEmpty(request.Descricao))
                    {
                        string descricao = !String.IsNullOrEmpty(request.search.value) ? request.search.value : request.Descricao;

                        filtros.Add("UPPER(DESCRICAO) LIKE UPPER(@SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", descricao));
                    }

                    if (filtros.Count > 0)
                    {
                        sqlFrom.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }
                    #endregion

                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine("SELECT COUNT(ID) CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        sql = new StringBuilder();
                        sql.AppendLine("SELECT ID, DESCRICAO, STATUS");
                        sql.AppendLine(sqlFrom.ToString());

                        #region [ORDENAÇÃO]
                        if (request.order != null && request.order.Any())
                        {
                            int sortColumnIndex = request.order[0].column;
                            var coluna = request.columns[sortColumnIndex].data;
                            string sortDirection = request.order[0].dir;

                            switch (coluna)
                            {
                                case "status":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY STATUS ");

                                        else
                                            sql.AppendLine(" ORDER BY STATUS DESC ");
                                        break;
                                    }
                                case "descricao":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY DESCRICAO DESC");
                                        break;
                                    }
                                case "codigo":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY ID");
                                        else
                                            sql.AppendLine(" ORDER BY ID DESC");
                                        break;
                                    }
                            }
                        }
                        #endregion

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new MotivosCancelamentos_ListModels
                                {
                                    Codigo = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Status = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.ATIVO : RsCadastros.INATIVO
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new DataTableResponse<MotivosCancelamentos_ListModels>
                {
                    draw = 0,
                    recordsFiltered = recordsTotal,
                    data = null,
                    error = ex.Message
                };
            }

            return new DataTableResponse<MotivosCancelamentos_ListModels>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }
        #endregion

        [HttpPost()]
        [Route("api/MotivosCancelamentosapi/AlterarStatus")]
        #region [AlterarStatus]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarMotivosCancelamentosStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var _repoMotivoCancelamentos = new Conexao<MotivoCancelamentoAniel>(session))
                {
                    var listaMotivos = new List<MotivoCancelamentoAniel>();
                    foreach (var item in value.ids)
                    {
                        long codigo = Convert.ToInt64(item);

                        var obj = _repoMotivoCancelamentos.selectFirst(x => x.Codigo == codigo).Result;
                        obj.Status = Convert.ToBoolean(value.status);

                        listaMotivos.Add(obj);
                    }

                    await _repoMotivoCancelamentos.SaveOrUpdate(listaMotivos);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/MotivosCancelamentosapi/Gravar")]
        #region [Gravar]
        public async Task<PostApiRetorno<string>> Gravar([FromBody] ParamWebApi<MotivosCancelamentos_CadastroModels> value)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var _repoMotivoReagendamento = new Conexao<MotivoCancelamentoAniel>(session))
                {
                    var obj = new MotivoCancelamentoAniel();

                    long codigo = !String.IsNullOrEmpty(value.Objeto.Id) ? Convert.ToInt64(value.Objeto.Id) : value.Objeto.Codigo;
                    obj = _repoMotivoReagendamento.selectFirst(x => x.Codigo == codigo).Result;

                    if (obj != null)
                    {
                        obj.Descricao = value.Objeto.Descricao;
                        obj.Cod_Auxiliar = value.Objeto.Cod_Auxiliar;
                        obj.Status = value.Objeto.Status;
                    }
              
                    else
                    {
                        obj = new MotivoCancelamentoAniel
                        {
                            Descricao = value.Objeto.Descricao,
                            Status = value.Objeto.Status,
                            Cod_Auxiliar = value.Objeto.Cod_Auxiliar
                        };
                    }

                    await _repoMotivoReagendamento.SaveOrUpdate(obj);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
        #endregion

        [HttpGet()]
        [Route("api/MotivosCancelamentosapi/FindById")]
        #region [FindById]
        public async Task<GetApiRetorno<MotivosCancelamentos_CadastroModels>> FindById(String Id)
        {
            GetApiRetorno<MotivosCancelamentos_CadastroModels> retorno = new GetApiRetorno<MotivosCancelamentos_CadastroModels>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var _repoMotivo = new Conexao<MotivoCancelamentoAniel>(session))
                {
                    long codigo = Convert.ToInt64(Id);

                    retorno.Objeto = GetObjView(await _repoMotivo.selectFirst(x => x.Codigo == codigo));

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }
        #endregion

        #region [GetObjView]
        private MotivosCancelamentos_CadastroModels GetObjView(MotivoCancelamentoAniel obj)
        {
            return new MotivosCancelamentos_CadastroModels
            {
                Id = obj.Codigo.ToString(),
                Codigo = obj.Codigo,
                Descricao = obj.Descricao,
                Status = obj.Status,
                Cod_Auxiliar= obj.Cod_Auxiliar,
            };
        }
        #endregion

        [HttpPost()]
        [Route("api/MotivosCancelamentosapi/GetCombo")]
        public async Task<ObjCombo<string>> GetCombo()
        {
            var retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };
            
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var _repo = new Conexao<MotivoCancelamentoAniel>(sessionT5))
                {
                    var lista = await _repo.select(x => x.Status == true);
                    var opcaoOutros = new ObjComboResult<string>() { Id = "0", Text = String.Format("{0}", "Outro") };
                    retorno.results.Add(opcaoOutros);

                    foreach (var item in lista)
                    {
                        var obj = new ObjComboMotivosCancelamentos<string>
                        {
                            Id = Convert.ToString(item.Codigo),
                            Text = item.Descricao,
                        };

                        retorno.results.Add(obj);
                    }
                    retorno.total_count = retorno.results.Count;
                }
            }
            catch (Exception ex)
            {
                retorno.results = null;
                retorno.total_count = 0;
            }

            return retorno;
        }
    }
}