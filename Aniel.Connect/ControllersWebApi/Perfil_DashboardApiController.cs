using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Perfil_DashboardApiController : BaseApiController
    {


        [HttpPost()]
        [Route("api/Perfil_Dashboardapi/GetAll")]
        public async Task<DataTableResponse<Perfil_DashboardListaModel>> GetAll(Perfil_Dashboard_DataTableRequest request)
        {

            List<Perfil_DashboardListaModel> retorno = new List<Perfil_DashboardListaModel>();
            IOrderedEnumerable<Perfil_DashboardListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_PERFIL_DASHBOARD_CONNECT"); //WHERE STATUS = 1  

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID,A.DESCRICAO, A.STATUS,A.Dashboard_Inicial, A.DASHBOARD_GERENCIAL");
                    sql.AppendLine(" FROM TB_PERFIL_DASHBOARD_CONNECT A");
                    //sql.AppendLine("LEFT OUTER JOIN TB_DETALHES_DASH_INICIAL B ");
                    //sql.AppendLine("ON(A.ID_DETALHES_DASH_INICIAL = B.ID) ");
                    //sql.AppendLine("LEFT OUTER JOIN TB_DETALHES_DASH_GERENCIAL C ");
                    //sql.AppendLine("ON(A.ID_DETALHES_DASH_GERENCIAL = C.ID) ");

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                    }

                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Perfil_DashboardListaModel
                            {

                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Perfil_Gerencial = HtmlEnumExtensions.GetEnumDescription<DashBoard_Gerencial>(Convert.ToString(reader["DASHBOARD_GERENCIAL"])),
                                Perfil_Inicial = HtmlEnumExtensions.GetEnumDescription<DashboardInicialConnect>(Convert.ToString(reader["Dashboard_Inicial"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),

                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Id);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<Perfil_DashboardListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Perfil_DashboardListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Perfil_DashboardListaModel>> retorno = new GetApiRetorno<IEnumerable<Perfil_DashboardListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Perfil_DashBoard_Connect>(session))
            {
                try
                {
                    var lista = await repository.select(p => p.Sistema == Perfil_DashBoard_Sistema.Connect);

                    retorno.Objeto = lista.Select(p => new Perfil_DashboardListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                    });

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

        private Perfil_DashboardCadastroModel GetViewObj(Perfil_DashBoard_Connect obj)
        {
            Perfil_DashboardCadastroModel retorno = new Perfil_DashboardCadastroModel();

            retorno.Id = obj.Id;
            retorno.Descricao = obj.Descricao;
            retorno.Status = obj.Status == Status_Default.Ativo ? true : false;
            //retorno.DashBoard_Gerencial = obj.DashBoard_Gerencial;
            retorno.Dashboard_Inicial = obj.Dashboard_Inicial.Value;
            retorno.VisualizacaoDashboard = obj.VisualizacaoDashboard;

            retorno.RespostasInicial = new List<Perfil_DashboardRecursoModel>();
            for (int i = 0; i < obj.Detalhes_Dash_Inicial.Count(); i++)
            {
                Perfil_DashboardRecursoModel recurso = new Perfil_DashboardRecursoModel
                {
                    Recurso = obj.Detalhes_Dash_Inicial.ToList()[i].Recurso
                };
                retorno.RespostasInicial.Add(recurso);
            }

            //retorno.RespostasGerencial = new List<Perfil_DashboardRecursoModel>();
            //for (int i = 0; i < obj.Detalhes_Dash_Gerencial.Count(); i++)
            //{
            //    Perfil_DashboardRecursoModel recurso = new Perfil_DashboardRecursoModel
            //    {
            //        Recurso = obj.Detalhes_Dash_Gerencial.ToList()[i].Recurso
            //    };
            //    retorno.RespostasGerencial.Add(recurso);
            //}

            return retorno;
        }

        private void BindViewToObj(ISession session, Perfil_DashBoard_Connect objBanco, Perfil_DashboardCadastroModel view)
        {

            objBanco.Id = view.Id;
            objBanco.Descricao = view.Descricao;
            objBanco.Dashboard_Inicial = view.Dashboard_Inicial;
            objBanco.Sistema = Perfil_DashBoard_Sistema.Connect;
            objBanco.VisualizacaoDashboard = view.VisualizacaoDashboard;
            //objBanco.DashBoard_Gerencial = view.DashBoard_Gerencial;

            //objBanco.Detalhes_Dash_Gerencial.Clear();
            //for (int i = 0; i < view.RespostasGerencial.Count(); i++) {
            //    Detalhes_Dash_Gerencial objgerencial = new Detalhes_Dash_Gerencial
            //    {
            //        Perfil_DashBoard_Connect = objBanco,
            //        Recurso = view.RespostasGerencial[i].Recurso
            //    };

            //    objBanco.Detalhes_Dash_Gerencial.Add(objgerencial);
            //}

            objBanco.Detalhes_Dash_Inicial.Clear();
            for (int i = 0; i < view.RespostasInicial.Count(); i++)
            {
                Detalhes_Dash_Inicial objinicial = new Detalhes_Dash_Inicial
                {
                    Perfil_DashBoard_Connect = objBanco,
                    Recurso = view.RespostasInicial[i].Recurso
                };
                objBanco.Detalhes_Dash_Inicial.Add(objinicial);
            }


        }

        [HttpPost()]
        [Route("api/Perfil_Dashboardapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PERFIL_DASHBOARD_CONNECT SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        [HttpPost()]
        [Route("api/Perfil_Dashboardapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_PERFIL_DASHBOARD_CONNECT WHERE ID IN {_ids}");

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


        [HttpGet()]
        [Route("api/Perfil_Dashboardapi/FindById")]
        public async Task<GetApiRetorno<Perfil_DashboardCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Perfil_DashboardCadastroModel> retorno = new GetApiRetorno<Perfil_DashboardCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Perfil_DashBoard_Connect>(session))
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

        [HttpPost()]
        [Route("api/Perfil_Dashboardapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {

            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PERFIL_DASHBOARD_CONNECT A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    condicoes.Add($" A.SISTEMA = 1 ");

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
                    }

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

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
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

        [HttpPost()]
        [Route("api/Perfil_Dashboardapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Perfil_DashboardCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Perfil_DashBoard_Connect modelo = null;
                Boolean perfil_novo = false;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repoDetalhesIni = new Conexao<Detalhes_Dash_Inicial>(session))
                    using (var repoDetalhesGer = new Conexao<Detalhes_Dash_Gerencial>(session))
                    using (var repositoryUser = new Conexao<IdentityUser>(session))
                    using (var repository = new Conexao<Perfil_DashBoard_Connect>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Perfil_DashBoard_Connect
                            {
                                Status = Status_Default.Ativo
                            };
                            perfil_novo = true;
                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        if (perfil_novo)
                        {
                            var modelo_usuario = await repositoryUser.selectFirst(p => p.Id == value.Objeto.Id_User);
                            modelo_usuario.Perfil_Dash_Connect = modelo;

                            await repositoryUser.Update(modelo_usuario);
                        }
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

        [Route("api/Perfil_Dashboardapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Perfil_DashBoard_Connect>(session))
                    {

                        Perfil_DashBoard_Connect modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
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


    }
}
