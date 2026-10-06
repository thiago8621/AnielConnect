using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Exceptions;
using Aniel.Connect.Repository.Exceptions.Voalle;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Repository.MK;
using log4net;
using MongoDB.Bson.IO;
using Npgsql;
using RestSharp;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace Aniel.Connect.Repository.MK
{
    /// <summary>
    ///     Classe para realizar requisições para a API de provisionamento do MK
    /// </summary>
    public class MKProvisionamentoApiRepository : IMKProvisionamentoRepository
    {
        /// <summary>
        ///     Client HTTP com a URL base da API de provisionamento do MK
        /// </summary>
        private readonly RestClient _httpClient;


        private readonly string _urlClient;
        private readonly string _token;

        /// <param name="apiKey">Chave necessária para autenticação das requisições</param>
        /// <param name="apiSecret">Secret necessário para autenticação das requisições</param>
        public MKProvisionamentoApiRepository(IntegracoesExternas dadosIntegracaoMK)
        {
            if (String.IsNullOrEmpty(dadosIntegracaoMK.TokenAcesso))
                throw new ArgumentNullException("TokenAcesso");

            if (String.IsNullOrEmpty(dadosIntegracaoMK.UrlApi))
                throw new ArgumentNullException("UrlApi");

            this._urlClient = dadosIntegracaoMK.UrlApi;
            this._httpClient = new RestClient(_urlClient);
            this._token = dadosIntegracaoMK.TokenAcesso;
        }


        public async Task<RetornoMK> VinculaConexao(string nomeCaixa, int? numPorta, string usuario, string onuSerial)
        {
            RetornoMK response = new RetornoMK();

            string urlCompleta = $"{_urlClient}/provisionamento/vincula-conexao";

            var client = new RestClient(urlCompleta)
            {
                Timeout = -1,
            };

            var token = this._token;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AlwaysMultipartFormData = true;

            request.AddParameter("nomeCaixa", nomeCaixa);
            request.AddParameter("numPorta", numPorta);
            request.AddParameter("usuario", usuario);
            request.AddParameter("onuSerial", onuSerial);

            IRestResponse restResponse = await client.ExecuteAsync(request);
            LogIntegracaoAtivacao($"Response Status: {restResponse.StatusCode}");
            LogIntegracaoAtivacao($"Response Content: {restResponse.Content}");
            LogIntegracaoAtivacao($"Response ErrorMessage: {restResponse.ErrorMessage}");

            if (restResponse.IsSuccessful)
            {
                response.success = true;               
            }
            else
            {
               
                response.success = false;
                response.message = $"Error: {restResponse.ErrorMessage}";
            }

            return response;
        }


        public async Task<RetornoMK> DesvinculaConexao(string usuario)
        {
            RetornoMK response = new RetornoMK();

            string urlCompleta = $"{_urlClient}/provisionamento/desvincula-conexao";

            var client = new RestClient(urlCompleta)
            {
                Timeout = -1,
            };

            var token = this._token;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "multipart/form-data");

            request.AlwaysMultipartFormData = true;

            request.AddParameter("usuario", usuario);

           
            IRestResponse restResponse = await client.ExecuteAsync(request);
            LogIntegracaoAtivacao($"Response Status: {restResponse.StatusCode}");
            LogIntegracaoAtivacao($"Response Content: {restResponse.Content}");
            LogIntegracaoAtivacao($"Response ErrorMessage: {restResponse.ErrorMessage}");

            if (restResponse.IsSuccessful)
            {
                response.success = true;
            }
            else
            {

                response.success = false;
                response.message = $"Error: {restResponse.ErrorMessage}";
            }

            return response;
        }

        private void LogIntegracaoAtivacao(string msg)
        {
            LogManager.GetLogger("IntegracaoOLT").Info($"[MKProvisionamentoApiRepository] - {msg}");
        }
    }
}
