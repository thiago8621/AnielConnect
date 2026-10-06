using Aniel.Connect.Integracoes.SERPRO.Models;
using Aniel.Connect.Services.Interfaces;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Aniel.Connect.Services.Services
{
    public class AutenticacaoService : BaseService, IAutenticacaoService
    {
        private string _urlBase;
        private string _consumerKey;
        private string _consumerSecret;

        public AutenticacaoService(string consumerKey, string consumerSecret)
        {          
            if (string.IsNullOrEmpty(CONSTANTES.URLProducaoSERPRO))
                throw new ArgumentException("Url de Acesso não pode estar vazio ou nulo.");

            if (string.IsNullOrEmpty(consumerKey))
                throw new ArgumentException("ConsumerKey não pode estar vazio ou nulo.");

            if (string.IsNullOrEmpty(consumerSecret))
                throw new ArgumentException("ConsumerSecret não pode estar vazio ou nulo.");

            _urlBase = CONSTANTES.URLProducaoSERPRO;
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        public async Task<string> ObterTokenAutorizacao()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string auth = await Base64Encode($"{_consumerKey}:{_consumerSecret}");

            RestClient _client = new RestClient(_urlBase);
            RestRequest _request = new RestRequest($"/token", Method.POST);
            _request.AddHeader("Authorization", $"Basic {auth}");
            _request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            _request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);
            
            var response = await _client.ExecuteAsync<AutenticacaoResponse>(_request);

            if (response != null && response.Data != null)
                return response.Data.access_token;

            return "";
        }

        public static async Task<string> Base64Encode(string textoCodificar)
        {
            var textoCodificado = System.Text.Encoding.UTF8.GetBytes(textoCodificar);
            return Convert.ToBase64String(textoCodificado);
        }
    }
}