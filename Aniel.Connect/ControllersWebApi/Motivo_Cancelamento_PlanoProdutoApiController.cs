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
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Motivo_Cancelamento_PlanoProdutoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/Motivo_Cancelamento_PlanoProdutoapi/GetAll")]
        public async Task<DataTableResponse<Motivo_Cancelamento_PlanoProdutoListaModel>> GetAll(Motivo_Cancelamento_PlanoProduto_DataTableRequest request)
        {
            List<Motivo_Cancelamento_PlanoProdutoListaModel> retorno = new List<Motivo_Cancelamento_PlanoProdutoListaModel>();
            IOrderedEnumerable<Motivo_Cancelamento_PlanoProdutoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MOTIVO_CANC_PLANOPRODUTO");

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

                    sql.AppendLine("SELECT A.ID, A.MOTIVO, A.STATUS ");
                    sql.AppendLine(" FROM TB_MOTIVO_CANC_PLANOPRODUTO A ");

                    //   sql.AppendLine(" WHERE A.STATUS = 1 ");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.MOTIVO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Motivo_Cancelamento_PlanoProdutoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Motivo = Convert.ToString(reader["MOTIVO"]),
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

            //Page e retornando DataTable
            return new DataTableResponse<Motivo_Cancelamento_PlanoProdutoListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        private Motivo_Cancelamento_PlanoProdutoCadastroModel GetViewObj(Motivo_Cancelamento_PlanoProduto obj)
        {
            return new Motivo_Cancelamento_PlanoProdutoCadastroModel
            {
                Id = obj.Id,
                Motivo = obj.Motivo,
                Status = obj.Status.Value == Status_Default.Ativo ? true : false

            };

        }

        private void BindViewToObj(ISession session, ref Motivo_Cancelamento_PlanoProduto objBanco, Motivo_Cancelamento_PlanoProdutoCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Motivo = view.Motivo;
        }


        [HttpPost()]
        [Route("api/Motivo_Cancelamento_PlanoProdutoapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Motivo_Cancelamento_PlanoProdutoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Motivo_Cancelamento_PlanoProduto modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Motivo_Cancelamento_PlanoProduto>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Motivo_Cancelamento_PlanoProduto
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

        [HttpPost()]
        [Route("api/Motivo_Cancelamento_PlanoProdutoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MOTIVO_CANC_PLANOPRODUTO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Motivo_Cancelamento_PlanoProdutoapi/FindById")]
        public async Task<GetApiRetorno<Motivo_Cancelamento_PlanoProdutoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Motivo_Cancelamento_PlanoProdutoCadastroModel> retorno = new GetApiRetorno<Motivo_Cancelamento_PlanoProdutoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Motivo_Cancelamento_PlanoProduto>(session))
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
        [Route("api/Motivo_Cancelamento_PlanoProdutoapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_CANC_PLANOPRODUTO A ");

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
                        condicoes.Add($"(UPPER(A.MOTIVO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.MOTIVO != '' AND ( {String.Join(" AND ", condicoes) } ) ");
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