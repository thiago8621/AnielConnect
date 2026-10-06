using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Motivo_Suspensao_ContratualApiController : BaseApiController
    {
        #region [GetAll]
        [HttpPost]
        [Route("api/Motivo_Suspensao_ContratualApi/GetAll")]
        public async Task<DataTableResponse<Motivo_Suspensao_ContratualListaModel>> GetAll(Motivo_Suspensao_Contratual_DataTableRequest request)
        {
            List<Motivo_Suspensao_ContratualListaModel> retorno = new List<Motivo_Suspensao_ContratualListaModel>();
            IOrderedEnumerable<Motivo_Suspensao_ContratualListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                #region [Count]
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MOTIVO_SUSPENSAO_CONTRATUAL");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }
                #endregion

                #region [Select]
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.MOTIVO, A.MOTIVO_PARA, A.STATUS FROM TB_MOTIVO_SUSPENSAO_CONTRATUAL A");

                    #region [Filtro]
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.MOTIVO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }
                    #endregion

                    #region [BUSCA POR PAGINAÇÃO]
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Motivo_Suspensao_ContratualListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Motivo = Convert.ToString(reader["MOTIVO"]),
                                Motivo_Para = HtmlEnumExtensions.GetEnumDescription<Status_Motivo_Para>(Convert.ToString(reader["MOTIVO_PARA"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            });
                        }
                    }
                    #endregion
                }
                #endregion
            }

            #region [Ordenação]
            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Motivo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Motivo);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Motivo);
                            break;
                        }
                }
            }
            #endregion

            return new DataTableResponse<Motivo_Suspensao_ContratualListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }
        #endregion

        #region [Gravar]
        [HttpPost()]
        [Route("api/Motivo_Suspensao_ContratualApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Motivo_Suspensao_ContratualCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Motivo_Suspensao_Contratual modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Motivo_Suspensao_Contratual>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Motivo_Suspensao_Contratual
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Status = Convert.ToBoolean(value.Objeto.Status) ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
        #endregion

        #region [GetCombo]
        [HttpPost()]
        [Route("api/Motivo_Suspensao_ContratualApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboAvo<Int64?, Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    #region [Filtros]
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_SUSPENSAO_CONTRATUAL A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        if (request.idpai == 2)
                        {
                            condicoes.Add($"A.MOTIVO_PARA IN (1, 2)");
                        }
                        else if (request.idpai == 3)
                        {
                            condicoes.Add($"A.MOTIVO_PARA IN (1, 3)");
                        }
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.MOTIVO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.MOTIVO IS NOT NULL AND ( {String.Join(" AND ", condicoes)} ) ");
                    }
                    #endregion

                    #region [Count]
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }
                    #endregion

                    #region [Select]
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.MOTIVO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.MOTIVO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(reader["MOTIVO"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["MOTIVO"])
                                    });

                                }
                            }

                            retorno.results.Add(new ObjComboResult<Int64>
                            {
                                Id = -1,
                                Text = String.Format("{0}", "Outro")
                            });
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }
        #endregion

        #region [FindById]
        [HttpGet()]
        [Route("api/Motivo_Suspensao_ContratualApi/FindById")]
        public async Task<GetApiRetorno<Motivo_Suspensao_ContratualCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Motivo_Suspensao_ContratualCadastroModel> retorno = new GetApiRetorno<Motivo_Suspensao_ContratualCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Motivo_Suspensao_Contratual>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }
        #endregion

        #region [Alterar Status]
        [HttpPost()]
        [Route("api/Motivo_Suspensao_ContratualApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MOTIVO_SUSPENSAO_CONTRATUAL SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
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

        #region [BindViewToObj]
        private void BindViewToObj(ISession session, ref Motivo_Suspensao_Contratual objBanco, Motivo_Suspensao_ContratualCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Motivo = view.Motivo;
            objBanco.Motivo_Para = view.Motivo_Para;
        }
        #endregion

        #region [GetViewObj]
        private Motivo_Suspensao_ContratualCadastroModel GetViewObj(Motivo_Suspensao_Contratual obj)
        {
            return new Motivo_Suspensao_ContratualCadastroModel
            {
                Id = obj.Id,
                Motivo = obj.Motivo,
                Motivo_Para = obj.Motivo_Para,
                Status = obj.Status.Value == Status_Default.Ativo ? true : false
            };

        }
        #endregion

    }
}