using Aniel.Connect.Integracoes.SERPRO.Models;
using Aniel.Connect.Services.Interfaces;
using Aniel.Connect.Services.Services;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Aniel.Connect.Services.SERPRO.Services
{
    public class ConsultaService : BaseService, IConsultaService
    {
        private string _urlBase;
        private string _token;

        public ConsultaService(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token não pode estar vazio ou nulo");

            if (string.IsNullOrEmpty(CONSTANTES.URLProducaoSERPRO))
                throw new ArgumentException("URL do ambiente de produção SERPRO não foi definida");

            _urlBase = CONSTANTES.URLProducaoSERPRO;
            _token = token;
        }

        public async Task<ConsultaCnpj> ObterCadastroCNPJ(string cnpj)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string uri = string.Empty;
            RestClient _client = new RestClient(_urlBase);

            if (CONSTANTES.AmbienteTestesSERPRO == "S")
                uri = $"/consulta-cnpj-df-trial/v2";
            else
                uri = $"/consulta-cnpj-df/v2";

            var isApiAtiva = await VerificarStatusAPI(uri);

            if (!isApiAtiva)
                throw new Exception("API Consulta CNPJ indisponível");

            RestRequest _request = new RestRequest(uri + $"/basica/{cnpj}", Method.GET);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Accept", "application/json");
            _request.AddHeader("Authorization", $"Bearer {_token}");

            var response = await _client.ExecuteAsync<ConsultaCnpj>(_request);

            var validaErros = ValidarErrosResponse((int)response.StatusCode);

            if (validaErros != null && !validaErros.Valido)
                throw new Exception(validaErros.Mensagem);

            return response.Data;
        }

        public async Task<ConsultaCpf> ObterCadastroCPF(string cpf)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string uri = string.Empty;
            RestClient _client = new RestClient(_urlBase);

            if (CONSTANTES.AmbienteTestesSERPRO == "S")
                uri = $"/consulta-cpf-df-trial/v1";
            else
                uri = $"/consulta-cpf/v1";

            var isApiAtiva = await VerificarStatusAPI(uri);

            if (!isApiAtiva)
                throw new Exception("API Consulta CPF indisponível");

            RestRequest _request = new RestRequest(uri + $"/cpf/{cpf}", Method.GET);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Accept", "application/json");
            _request.AddHeader("Authorization", $"Bearer {_token}");

            var response = await _client.ExecuteAsync<ConsultaCpf>(_request);

            var validaErros = ValidarErrosResponse((int)response.StatusCode);

            if (validaErros != null && !validaErros.Valido)
                throw new Exception(validaErros.Mensagem);

            return response.Data;
        }

        public async Task<bool> VerificarStatusAPI(string uri)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            RestClient _client = new RestClient(_urlBase);

            RestRequest _request = new RestRequest(uri + "/status", Method.GET);
            _request.AddHeader("Content-Type", "application/json");
            _request.AddHeader("Accept", "application/json");
            _request.AddHeader("Authorization", $"Bearer {_token}");

            var response = await _client.ExecuteAsync(_request);

            var validaErros = ValidarErrosResponse((int)response.StatusCode);

            if (validaErros != null && !validaErros.Valido)
                throw new Exception(validaErros.Mensagem);

            return true;
        }
    }
}