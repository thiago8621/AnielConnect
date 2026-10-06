using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
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
    [Authorize]
    public class Mensagem_SistemaApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Mensagem_Sistemaapi/GetAll")]
        public async Task<DataTableResponse<Mensagem_SistemaListaModel>> GetAll(Mensagem_Sistema_DataTableRequest request)
        {
            List<Mensagem_SistemaListaModel> retorno = new List<Mensagem_SistemaListaModel>();
            IOrderedEnumerable<Mensagem_SistemaListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MENSAGEM_SISTEMA WHERE STATUS = 1");

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

                    sql.AppendLine("SELECT A.ID, A.TITULO, A.MENSAGEM"); 
                    sql.AppendLine("FROM TB_MENSAGEM_SISTEMA A");
                    
                    sql.AppendLine("WHERE A.STATUS = 1 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ( (UPPER(A.TITULO) LIKE UPPER(@SEARCH) ) ) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Mensagem_SistemaListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Titulo  = Convert.ToString(reader["TITULO"]),
                                Mensagem = Convert.ToString(reader["MENSAGEM"])
                                
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
                    case 1:     // ID
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                    case 2:     // TITULO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Titulo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Titulo);
                            break;
                        }
                    case 3:     // DESCRICAO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Mensagem);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Mensagem);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Mensagem_SistemaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private Mensagem_SistemaCadastroModel GetViewObj(Mensagem_Sistema obj)
        {
            return new Mensagem_SistemaCadastroModel
            {
                Id = obj.Id,
                Titulo = obj.Titulo,
                Mensagem = obj.Mensagem,
                Status = obj.Status == Status_Default.Ativo 
            };
        }

        private void BindViewToObj(ISession session, ref Mensagem_Sistema objBanco, Mensagem_SistemaCadastroModel view)
        {
            objBanco.Titulo = view.Titulo;
            objBanco.Mensagem = view.Mensagem;            
        }

        [HttpPost()]
        [Route("api/Mensagem_Sistemaapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MENSAGEM_SISTEMA SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Mensagem_Sistemaapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_MENSAGEM_SISTEMA WHERE ID IN {_ids}");

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
        [Route("api/Mensagem_SistemaApi/FindById")]
        public async Task<GetApiRetorno<Mensagem_SistemaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Mensagem_SistemaCadastroModel> retorno = new GetApiRetorno<Mensagem_SistemaCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Mensagem_Sistema>(session))
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
        [Route("api/Mensagem_SistemaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Mensagem_SistemaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Mensagem_Sistema modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Mensagem_Sistema>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Mensagem_Sistema
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };                            
                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
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

        [Route("api/Mensagem_SistemaApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Mensagem_Sistema>(session))
                    {

                        Mensagem_Sistema modelo = null;
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
