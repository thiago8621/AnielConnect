using Aniel.Connect.Repository.Model.Hubsoft;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;



namespace Aniel.Connect.Repository.Repository.HubSoft
{
    public static class OrdemServicoHubSoftDao
    {
        public static OrdemServicoHubSoft GetOrdemServico(string urlBase, string numero, LoginHubsoftModel login)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/ordem_servico/consultar/paginado/600?page=1", Method.POST);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            var requestModel = new
            {
                data_inicio = "2024-01-01",
                data_fim = DateTime.Now.ToString("yyyy-MM-dd"),
                tipo_data = "data_cadastro",
                order_by = "data_cadastro",
                order_by_key = "DESC",
                status_ordem_servico = new List<string>
                {
                    "pendente",
                    "aguardando_agendamento"
                },
                busca = numero
            };

            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(requestModel);

            var response = client.Execute<ResponseConsultaOrdemServico>(request);
            var deserialized = JsonSerializer.Deserialize<ResponseConsultaOrdemServico>(response.Content);
            var retorno = deserialized.ordens_servico.data.FirstOrDefault();


            return retorno;
        }

        public static OrdemServicoHubSoft GetOrdemServicoById(string urlBase, int id, LoginHubsoftModel login, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/ordem_servico/{id}", Method.GET);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute<ConsultaOrdemServicoDetalhada>(request);
            var idLog = enviarLog($"Integração HubSoft [ordem_servico] -> consultando ordem de serviço de ID {id}", usuarioHubsoft, detalheHubsoft, null);
            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            return response?.Data?.ordem_servico;
        }

        public static OrdensServicoConsultar GetLoginRadiusByOS(string urlBase, string numero, LoginHubsoftModel login, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null,
            string numObra = null, string usuario = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/integracao/ordem_servico/consultar", Method.POST);
            var token = AutenticacaoHubSoft.Login(urlBase, login);

            var requestModel = new
            {
                consulta = numero
            };


            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(requestModel);

            var response = client.Execute<RetornoOrdemDeServicoConsultar>(request);

            var idLog = enviarLog($"Integração HubSoft [ordem_servico] -> consultando ordem de serviço de número {numero}", usuario, numObra, null);
            enviarLog(response.Content, usuario, numObra, idLog.Result);


            return response.Data.ordens_servico.FirstOrDefault();
        }
    }
}