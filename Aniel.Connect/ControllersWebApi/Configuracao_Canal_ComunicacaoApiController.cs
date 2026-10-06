using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Resources;
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
    public class Configuracao_Canal_ComunicacaoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Configuracao_Canal_ComunicacaoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Configuracao_Canal_ComunicacaoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Configuracao_Canal_Comunicacao modelo = null;

                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<Configuracao_Canal_Comunicacao>(session))
                {
                    if (value.Objeto.Id != 0)
                        modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                    if (modelo == null)
                    {
                        modelo = new Configuracao_Canal_Comunicacao
                        {
                            UsuarioCriacao = value.Usuario
                        };
                    }
                    else
                    {
                        modelo.UsuarioAlteracao = value.Usuario;
                        modelo.DataHoraAlteracao = DateTime.Now;
                    }

                    BindViewToObj(session, ref modelo, value.Objeto);

                    await repository.SaveOrUpdate(modelo);

                }
                retorno.IdObjeto = modelo.Id;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Configuracao_Canal_ComunicacaoApi/FindById")]
        public async Task<GetApiRetorno<Configuracao_Canal_ComunicacaoCadastroModel>> FindFirst()
        {
            GetApiRetorno<Configuracao_Canal_ComunicacaoCadastroModel> retorno = new GetApiRetorno<Configuracao_Canal_ComunicacaoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Configuracao_Canal_Comunicacao>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst());

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

        private void BindViewToObj(ISession session, ref Configuracao_Canal_Comunicacao objBanco, Configuracao_Canal_ComunicacaoCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.MostrarNomeAtendente = view.MostrarNomeAtendente;
            objBanco.MostrarNomeAtendenteTelegram = view.MostrarNomeAtendenteTelegram;
            objBanco.MostrarNomeAtendenteMessenger = view.MostrarNomeAtendenteMessenger;
            objBanco.NomePrincipal = view.NomePrincipal.ToString();
            objBanco.NomeSecundario = view.NomeSecundario.ToString();
        }

        private Configuracao_Canal_ComunicacaoCadastroModel GetViewObj(Configuracao_Canal_Comunicacao obj)
        {
            return new Configuracao_Canal_ComunicacaoCadastroModel
            {
                Id = obj.Id,
                NomePrincipal = (Nome_Contato)Enum.Parse(typeof(Nome_Contato), obj.NomePrincipal),
                NomeSecundario = (Nome_Contato)Enum.Parse(typeof(Nome_Contato), obj.NomeSecundario),
                MostrarNomeAtendente =  obj.MostrarNomeAtendente,
                MostrarNomeAtendenteTelegram = obj.MostrarNomeAtendenteTelegram,
                MostrarNomeAtendenteMessenger = obj.MostrarNomeAtendenteMessenger
            };
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}