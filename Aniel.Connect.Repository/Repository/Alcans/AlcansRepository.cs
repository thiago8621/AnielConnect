using Aniel.Connect.Repository.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.Alcans
{
    public class AlcansRepository
    {
        private readonly string UsuarioApi, SenhaApi, UrlApi;

        public AlcansRepository(string usuarioApi, string senhaApi, string urlApi)
        {
            UsuarioApi = usuarioApi;
            SenhaApi = senhaApi;
            UrlApi = urlApi;
        }

        public AgruparContratosAlcansResponse AgruparContratos(AgruparContratosAlcansRequest model)
        {
            RestClient client = new RestClient(UrlApi);
            RestRequest request = new RestRequest("alcans-api/contract-groups/create", Method.POST);

            request.AddHeader("username", UsuarioApi);
            request.AddHeader("password", SenhaApi);

            request.AddJsonBody(model);

            var response = client.Execute<AgruparContratosAlcansResponse>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                return response.Data;
            else
            {
                if(!response.Content.Contains("The following contracts are already linked to an grouper"))
                    throw new Exception($"Erro ao agrupar contratos:  {response.Content}");

                return null;
            }
        }

        public async Task<List<ContratosDisponiveisResponse>> ContratosDisponiveisAllhub(ContratosDisponiveisRequest model)
        {
            RestClient client = new RestClient(UrlApi);
            RestRequest request = new RestRequest("api-hub/contract/contractsAvailableAlHub", Method.POST);

            request.AddHeader("username", UsuarioApi);
            request.AddHeader("password", SenhaApi);

            request.AddJsonBody(model);

            var response = client.Execute<List<ContratosDisponiveisResponse>>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                return response.Data;
            else
            {
                return null;
            }
        }
    }

    public class ContratosDisponiveisResponse
    {
        public int accessPlan { get; set; }
        public bool permit { get; set; }
    }


    public class ContratosDisponiveisRequest
    {
        public string document { get; set; }
        public List<int> accessPlans { get; set; }
    }

    public class AgruparContratosAlcansRequest
    {
        public long customer { get; set; }
        public string description { get; set; }
        public List<long> contracts { get; set; }
    }

    public class AgruparContratosAlcansResponse
    {
        public long contractGroupCode { get; set; }
    }

    public class AgruparContratosAlcansErroResponse
    {
        public string message { get; set; }
        public string error { get; set; }
        public int statusCode { get; set; }
    }
}