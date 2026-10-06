using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    public class Criterio_RoteirizacaoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Criterio_Roteirizacaoapi/GetAll")]
        public async Task<DataTableResponse<Criterio_RoteirizacaoListaModel>> GetAll(Criterio_Roteirizacao_DataTableRequest request)
        {

            List<Criterio_RoteirizacaoListaModel> retorno = new List<Criterio_RoteirizacaoListaModel>();
            IOrderedEnumerable<Criterio_RoteirizacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_Criterio_Roteirizacao A"); //WHERE STATUS = 1  
                    sql.AppendLine(" WHERE A.STATUS = 1 ");

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

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO,A.VALOR_Criterio_Roteirizacao,A.PERCENTUAL_Criterio_Roteirizacao,REPETICOES_Criterio_Roteirizacao,A.Criterio_Roteirizacao_CAPTACAO");
                    sql.AppendLine(" FROM TB_Criterio_Roteirizacao A");
                    sql.AppendLine(" WHERE A.STATUS = 1 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Criterio_RoteirizacaoListaModel
                            {
                                //Plano = Convert.ToString(reader["PLANO_DESCRICAO"]),
                                //Produto = Convert.ToString(reader["PRODUTO_DESCRICAO"]),
                                //Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                                //Tem_Criterio_Roteirizacao = Convert.ToString(reader["TEM_Criterio_Roteirizacao"]),
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
                                listaOrdenada = retorno.OrderBy(p => p.Ordem);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Ordem);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Ordem);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<Criterio_RoteirizacaoListaModel>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

       

        private Criterio_RoteirizacaoCadastroModel GetViewObj(List<Criterio_RoteirizacaoAniel> obj)
        {
            return new Criterio_RoteirizacaoCadastroModel
            {

                Criterios = (obj.Count() == 0) ? new List<Criterio_RoteirizacaoListaModel>() :
                                                   obj.Select(p => new Criterio_RoteirizacaoListaModel
                                                   {

                                                       Criterio = HtmlEnumExtensions.GetEnumDescription<Criterio_RoteirizacaoTipoAniel>(p.Criterio),
                                                       Criterio_Val = Convert.ToInt16(p.Criterio),
                                                       Ordem = Convert.ToInt16(p.Ordem),
                                                       Id = p.Id

                                                   }).ToList()


            };

        }

        private void BindViewToObj(ISession session, Criterio_RoteirizacaoAniel objBanco, Criterio_RoteirizacaoListaModel view)
        {
            objBanco.Criterio = (Criterio_RoteirizacaoTipoAniel) Enum.Parse(typeof(Criterio_RoteirizacaoTipoAniel),view.Criterio_Val.ToString());
            objBanco.Ordem = view.Ordem + 1;
        }

        [HttpPost()]
        [Route("api/Criterio_Roteirizacaoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_Criterio_Roteirizacao SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Criterio_Roteirizacaoapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_Criterio_Roteirizacao WHERE ID IN {_ids}");

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
        [Route("api/Criterio_Roteirizacaoapi/FindById")]
        public async Task<GetApiRetorno<Criterio_RoteirizacaoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Criterio_RoteirizacaoCadastroModel> retorno = new GetApiRetorno<Criterio_RoteirizacaoCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    retorno.Objeto = GetViewObj((await new Conexao<Criterio_RoteirizacaoAniel>(session).select()).ToList());

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
        [Route("api/Criterio_Roteirizacaoapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Criterio_RoteirizacaoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Criterio_RoteirizacaoAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Criterio_RoteirizacaoAniel>(session))
                    {

                        if (value.Objeto.Criterios != null) {

                            await repository.Remove((await repository.select()).ToList());

                            foreach (Criterio_RoteirizacaoListaModel criterio in value.Objeto.Criterios) {

                                modelo = new Criterio_RoteirizacaoAniel();
                                
                                BindViewToObj(session, modelo, criterio);

                                await repository.SaveOrUpdate(modelo);

                            }

                        }
                        
                    }
                    retorno.IdObjeto = 1;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 1;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/Criterio_Roteirizacaoapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Criterio_RoteirizacaoAniel>(session))
                    {

                        Criterio_RoteirizacaoAniel modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.Resources.NAOENCONTRADO);
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

        [Route("api/Criterio_Roteirizacaoapi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Criterio_RoteirizacaoAniel>(session))
                    {

                        Criterio_RoteirizacaoAniel modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.Resources.NAOENCONTRADO);
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
