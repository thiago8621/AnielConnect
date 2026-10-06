using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
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
    public class URL_SistemaApiController : BaseApiController
    {



        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<URL_SistemaListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<URL_SistemaListaModel>> retorno = new GetApiRetorno<IEnumerable<URL_SistemaListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<URL_Sistema>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new URL_SistemaListaModel
                    {
                        Id = p.Id,
                        Sistema = HtmlEnumExtensions.GetEnumDescription<Sistema>(Convert.ToString(p.Sistema)),
                        URL = p.URL

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

        private URL_SistemaCadastroModel GetViewObj(URL_Sistema obj)
        {
            return new URL_SistemaCadastroModel
            {
                Id = obj.Id,
                Id_Configuracoes_Gerais = obj.Id_Configuracoes_Gerais,
                Sistema = obj.Sistema,
                URL = obj.URL


               
            };
        }

        private void BindViewToObj(ISession session, ref URL_Sistema objBanco, URL_SistemaCadastroModel view)
        {
            using (var repository = new Conexao<Configuracoes_Gerais>(session))
            {
                objBanco.Id = view.Id;
                objBanco.URL = view.URL;
                objBanco.Sistema = view.Sistema;
                objBanco.Id_Configuracoes_Gerais = view.Id_Configuracoes_Gerais;

            }
        }

        [HttpPost()]
        [Route("api/URL_Sistemaapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_URL_SISTEMA SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/URL_Sistemaapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_URL_SISTEMA WHERE ID IN {_ids}");

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
        [Route("api/URL_Sistemaapi/FindById")]
        public async Task<GetApiRetorno<URL_SistemaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<URL_SistemaCadastroModel> retorno = new GetApiRetorno<URL_SistemaCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<URL_Sistema>(session))
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
        [Route("api/URL_Sistemaapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<URL_SistemaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                URL_Sistema modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<URL_Sistema>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new URL_Sistema
                            {
                                //Status = Connect.Model.enumerado.Status_Default.Ativo
                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {
                            //modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
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

        [Route("api/URL_Sistemaapi/Excluir")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> Excluir(Int64 id) 
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<URL_Sistema>(session))
                    {

                        URL_Sistema modelo = null;
                        if (id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == id)).FirstOrDefault();

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
