using Aniel.Connect.Core;
using Aniel.Connect.Models;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Aniel.Connect.Integracoes.FaturamentoEVA.Service
{
    public class FaturamentoService
    {
        /// <summary>
        /// Envio de Faturamento Parcial das Cobrancas 
        /// </summary>
        /// <param name="fatParcialModel"></param>
        /// <returns></returns>
        public async Task<PostApiRetorno<Int64>> GerarFaturamento(FaturamentoModels fatModels)
        {
            RestClient client = new RestClient(CONSTANTES.GetBaseUrlFinanceiro());
            RestRequest request = new RestRequest("/api/Faturamentoapi/CriarIntegral", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(fatModels);

            var response = await client.ExecuteAsync<PostApiRetorno<Int64>>(request);

            return response.Data;
        }

        /// <summary>
        /// Envio de Faturamento Parcial das Cobrancas 
        /// </summary>
        /// <param name="fatParcialModel"></param>
        /// <returns></returns>
        public async Task<PostApiRetorno<String>> GerarProcessamento(FaturamentoProcessarRequestModels fatProcessarModel)
        {
            RestClient client = new RestClient(CONSTANTES.GetBaseUrlFinanceiro());
            RestRequest request = new RestRequest("/api/Faturamentoapi/Processar", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(fatProcessarModel);

            var response = await client.ExecuteAsync<PostApiRetorno<String>>(request);

            return response.Data;
        }
    }
}