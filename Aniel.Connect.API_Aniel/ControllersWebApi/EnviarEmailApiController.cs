using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class EnviarEmailApiController: BaseApiController
    {        
        [HttpPost]
        [Route("api/EnviarEmailApiController/EnviarEmail")]
        public async Task<ApiResponseObj<EnviarEmailResponseModel>> EnviarEmail(EnviarEmailRequestModel request)
        {
            EnviarEmailResponseModel retorno = new EnviarEmailResponseModel();
            
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                Model.nhibernate.Conta_Email conta_email = null;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();               

                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {                    
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.EMAIL, A.SENHA, A.UTILIZAR_SSL_SMTP, A.SERVIDOR_SMTP, A.PORTA_SMTP FROM TB_CONTA_EMAIL A ");
                    sql.AppendLine("WHERE A.PRINCIPAL = 1 AND A.STATUS = 1 ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            conta_email = new Model.nhibernate.Conta_Email
                            {
                                Email = Convert.ToString(reader["EMAIL"]),
                                Senha = Convert.ToString(reader["SENHA"]),
                                Utilizar_Ssl_Smtp = reader["UTILIZAR_SSL_SMTP"] != DBNull.Value ? Convert.ToBoolean(reader["UTILIZAR_SSL_SMTP"]) : false,
                                Servidor_Smtp = Convert.ToString(reader["SERVIDOR_SMTP"]),
                                Porta_Smtp = Convert.ToInt32(reader["PORTA_SMTP"])
                            };
                            
                        }
                    }  
                    if(conta_email == null)
                    {
                        throw new Exception("Não há nenhuma conta de E-mail configurada para a empresa.");
                    }

                    EmailUtil email_service = new EmailUtil(conta_email.Email, conta_email.Senha, conta_email.Servidor_Smtp, conta_email.Porta_Smtp);

                    email_service.EnviarMensagem(request.email_destinatario, request.assunto, request.mensagem, conta_email.Utilizar_Ssl_Smtp);
                }             
                retorno.enviado = true;

                return new ApiResponseObj<EnviarEmailResponseModel> { data = retorno, ok = true };
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<EnviarEmailResponseModel> { error = ex.Message };
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("api/EnviarEmailApiController/EnviarEmailConfigCustomizada")]
        public async Task<ApiResponseObj<EnviarEmailResponseModel>> EnviarEmailConfigCustomizada(EnviarEmailConfigCustomizadaRequestModel request)
        {
            EnviarEmailResponseModel retorno = new EnviarEmailResponseModel();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                Model.nhibernate.Conta_Email conta_email = new Model.nhibernate.Conta_Email
                {
                    Email = request.email_remetente,
                    Senha = request.senha_remetente,
                    Utilizar_Ssl_Smtp = request.utiliza_ssl_smtp,
                    Porta_Smtp = request.porta_smtp ?? 0,
                    Servidor_Smtp = request.servidor_smtp
                };

                ValidarContaEmail(conta_email);                

                EmailUtil email_service = new EmailUtil(conta_email.Email, conta_email.Senha, conta_email.Servidor_Smtp, conta_email.Porta_Smtp);

                email_service.EnviarMensagem(request.email_destinatario, request.assunto, request.mensagem, conta_email.Utilizar_Ssl_Smtp);

                retorno.enviado = true;

                return new ApiResponseObj<EnviarEmailResponseModel> { data = retorno, ok = true };
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<EnviarEmailResponseModel> { error = ex.Message };
            }
        }

        private void ValidarContaEmail(Model.nhibernate.Conta_Email conta)
        {
            if(String.IsNullOrEmpty(conta.Email))
            {
                throw new Exception("Endereço E-mail do remente não informado.");
            }

            if(conta.Porta_Smtp == 0 || String.IsNullOrEmpty(conta.Servidor_Smtp))
            {
                throw new Exception("Parâmetros SMTP não informados.");
            }

        }
    }
}