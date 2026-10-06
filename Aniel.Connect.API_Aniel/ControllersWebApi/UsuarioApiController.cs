using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.API_Aniel.Repositorio;
using Aniel.Connect.Core;
using Google.Protobuf.WellKnownTypes;
using log4net;
using NHibernate;
using Org.BouncyCastle.Asn1.Ocsp;
using Sinapse.DAO.repository;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{

    public class UsuarioApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/UsuarioApiController/GetUsuarioValido")]
        public async Task<GetApiRetorno> GetUsuarioValido(IApiValidate apiValidate)
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new UsuarioRepository())
                {
                    retorno.Ok = await repository.UsuarioValido(session, apiValidate.user, apiValidate.password, apiValidate.token, true);
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;

        }       
        

        [HttpPost()]
        [Route("api/UsuarioApiController/AtualizarSenha")]
        public async Task<GetApiRetorno> AtualizarSenha(UsuarioModelsRequest value)
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {                
                if (!(await GetUsuarioValido(value.settings)).Ok)
                {
                    LogManager.GetLogger("Aniel.Connect.API_Aniel").Error("Credenciais do API Aniel inválidas ao atualizar senha do Connect via Touch.");
                    throw new Exception("Usuário e/ou Senha e/ou Token Inválidos.");
                }

                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new UsuarioRepository())
                {
                    retorno.Ok = await repository.AtualizarSenha(session, value.NumFre, value.Senha);
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;

        }

    }
}
