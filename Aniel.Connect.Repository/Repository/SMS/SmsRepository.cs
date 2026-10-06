using Aniel.Connect.Repository.Exceptions;
using Aniel.Connect.Repository.Model;
using RestSharp;
using System;
using System.Net;

namespace Aniel.Connect.Repository.SMS
{
    public class SmsRepository : ISmsRepository
    {
        private readonly string _usuarioApiAniel;
        private readonly string _senhaApiAniel;
        private readonly string _tokenApiAniel;

        private readonly RestClient _httpClient;

        public SmsRepository(string urlApiAniel, string usuarioApiAniel, string senhaApiAniel, string tokenApiAniel)
        {
            this._httpClient = new RestClient(urlApiAniel);

            this._usuarioApiAniel = usuarioApiAniel;
            this._senhaApiAniel = senhaApiAniel;
            this._tokenApiAniel = tokenApiAniel;
        }

        public void EnviarSms(string numero, string mensagem)
        {
            if (String.IsNullOrEmpty(numero))
            {
                throw new ArgumentNullException("numero");
            }

            if (String.IsNullOrEmpty(mensagem))
            {
                throw new ArgumentNullException("mensagem");
            }

            var request = new RestRequest("IntegracaoSMSApiController/EnviarSMS", Method.POST);
            request.AddHeader("accept", "application/json");

            var body = new IntegracaoEnviarSmsRequest
            {
                Num_Celular = numero,
                Mensagem = mensagem,
                settings = new IntegracaoEnviarSmsSettings
                {
                    user = this._usuarioApiAniel,
                    password = this._senhaApiAniel,
                    token = this._tokenApiAniel
                }
            };

            request.AddJsonBody(body);

            IRestResponse<IntegracaoEnviarSmsResponse> response = this._httpClient.Execute<IntegracaoEnviarSmsResponse>(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new IntegracaoSmsConnectionException();
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {                
                var retornoApi = Sinapse.Util.JsonService.DeserializeObject<IntegracaoEnviarSmsResponse>(response.Content);
                if(!retornoApi.ok)
                {
                    if(!string.IsNullOrEmpty(retornoApi.error))
                        throw new IntegracaoSmsException("Houve um erro ao enviar o SMS", new Exception(retornoApi.error));
                    else
                        throw new IntegracaoSmsException();
                }                
            }
        }
    }
}
