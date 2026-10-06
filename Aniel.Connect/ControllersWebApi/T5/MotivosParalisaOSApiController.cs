using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Util;
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
    public class MotivosParalisaOSApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/MotivosParalisaOSapi/GetAll")]
        #region [GetAll]
        public async Task<DataTableResponse<MotivosParalisaOS_ListModels>> GetAll(MotivosParalisaOS_DataTableRequest request)
        {
            List<MotivosParalisaOS_ListModels> retorno = new List<MotivosParalisaOS_ListModels>();

            Int32 recordsTotal = 0;
            try
            {
                var acesso = new AcessoApiController();
                var areas = await acesso.GetArea_ModuloUsuario(User.Identity.GetUserId());
                bool temAcesso356 = Core.Helpers.AcessoHelper.TemAcesso(areas.Objeto, "356");

                if (!temAcesso356)
                {
                    return new DataTableResponse<MotivosParalisaOS_ListModels>
                    {
                        draw = 0,
                        recordsFiltered = recordsTotal,
                        data = null,
                        error = "Você não possui acesso ao Visualizar os Motivos de Paralisação da OS, área 375"
                    };
                }

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MOTIVO_PARALISA_OS");
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

                        filtros.Add("UPPER(MOTIVO) LIKE UPPER(@SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", descricao));
                    }

                    if (filtros.Count > 0)
                    {
                        sqlFrom.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }
                    #endregion

                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine("SELECT COUNT(CODIGO) CO ");
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
                        sql.AppendLine("SELECT CODIGO, MOTIVO, STATUS");
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
                                            sql.AppendLine(" ORDER BY MOTIVO");
                                        else
                                            sql.AppendLine(" ORDER BY MOTIVO DESC");
                                        break;
                                    }
                                case "codigo":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY CODIGO");
                                        else
                                            sql.AppendLine(" ORDER BY CODIGO");
                                        break;
                                    }
                            }
                        }
                        #endregion

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new MotivosParalisaOS_ListModels
                                {
                                    Codigo = Convert.ToInt64(reader["CODIGO"]),
                                    Descricao = Convert.ToString(reader["MOTIVO"]),
                                    Status = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.ATIVO : RsCadastros.INATIVO
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new DataTableResponse<MotivosParalisaOS_ListModels>
                {
                    draw = 0,
                    recordsFiltered = recordsTotal,
                    data = null,
                    error = ex.Message
                };
            }

            return new DataTableResponse<MotivosParalisaOS_ListModels>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }
        #endregion

        [HttpPost()]
        [Route("api/MotivosParalisaOSapi/AlterarStatus")]
        #region [AlterarStatus]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarMotivosParalisaOSViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var _repoMotivosParalisaOS = new Conexao<MotivosParalisaOSAniel>(session))
                {
                    var listaMotivos = new List<MotivosParalisaOSAniel>();
                    foreach (var item in value.ids)
                    {
                        long codigo = Convert.ToInt64(item);

                        var obj = _repoMotivosParalisaOS.selectFirst(x => x.Codigo == codigo).Result;
                        obj.Status = Convert.ToBoolean(value.status);

                        listaMotivos.Add(obj);
                    }

                    await _repoMotivosParalisaOS.SaveOrUpdate(listaMotivos);
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
        [Route("api/MotivosParalisaOSapi/Gravar")]
        #region [Gravar]
        public async Task<PostApiRetorno<string>> Gravar([FromBody] ParamWebApi<MotivosParalisaOS_CadastroModels> value)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var _repoMotivosParalisaOS = new Conexao<MotivosParalisaOSAniel>(session))
                {
                    var obj = new MotivosParalisaOSAniel();

                    long codigo = !String.IsNullOrEmpty(value.Objeto.Id) ? Convert.ToInt64(value.Objeto.Id) : value.Objeto.Codigo;
                    obj = _repoMotivosParalisaOS.selectFirst(x => x.Codigo == codigo).Result;

                    if (obj != null)
                    {
                        obj.Descricao = value.Objeto.Descricao;
                        obj.Status = value.Objeto.Status;
                    }
                    else
                    {
                        obj = new MotivosParalisaOSAniel
                        {
                            Descricao = value.Objeto.Descricao,
                            Status = true
                        };
                    }

                    await _repoMotivosParalisaOS.SaveOrUpdate(obj);

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
        [Route("api/MotivosParalisaOSapi/FindById")]
        #region [FindById]
        public async Task<GetApiRetorno<MotivosParalisaOS_CadastroModels>> FindById(String Id)
        {
            GetApiRetorno<MotivosParalisaOS_CadastroModels> retorno = new GetApiRetorno<MotivosParalisaOS_CadastroModels>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var _repoMotivo = new Conexao<MotivosParalisaOSAniel>(session))
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
        private MotivosParalisaOS_CadastroModels GetObjView(MotivosParalisaOSAniel obj)
        {
            return new MotivosParalisaOS_CadastroModels
            {
                Id = obj.Codigo.ToString(),
                Codigo = obj.Codigo,
                Descricao = obj.Descricao,
                Status = obj.Status,
            };
        }
        #endregion

        [HttpPost()]
        [Route("api/MotivosParalisaOSapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MOTIVO_PARALISA_OS A  ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.CODIGO = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.MOTIVO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODIGO) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.CODIGO, A.MOTIVO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.MOTIVO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, sessionT5.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["CODIGO"]),
                                    Text = Convert.ToString(reader["MOTIVO"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }
    }
}