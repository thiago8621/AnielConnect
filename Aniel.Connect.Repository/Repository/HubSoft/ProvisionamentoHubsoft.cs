using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Hubsoft;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Sinapse.Util;
using static Aniel.Connect.Repository.Model.HubSoft.AtivarServico.AtivarServicoHubsoftModel;

namespace Aniel.Connect.Repository.Repository.HubSoft
{
    public static class ProvisionamentoHubsoft
    {
        public static ProvisionamentoHubsoftResponse ConfigurarAutenticacao(string urlBase, ConfigurarAutenticacaoHubsoft configurarAutenticacaoHubsoft, LoginHubsoftModel login, 
            string usuario, string detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            if (configurarAutenticacaoHubsoft == null)
                throw new ArgumentNullException("configurarAutenticacaoHubsoft");

            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/integracao/cliente/configurar_autenticacao", Method.POST);

            var token = AutenticacaoHubSoft.Login(urlBase, login);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(configurarAutenticacaoHubsoft);

            var json = JsonSerializer.Serialize(configurarAutenticacaoHubsoft, new JsonSerializerOptions { WriteIndented = true });
            var idLog = enviarLog($"Integração HubSoft [configurar_autenticacao] -> {json}", usuario, detalhe, null);

            var response = client.Execute<ProvisionamentoHubsoftResponse>(request);
            enviarLog(response.Content, usuario, detalhe, idLog.Result);

            ProvisionamentoHubsoftResponse responseBody = JsonService.DeserializeObject<ProvisionamentoHubsoftResponse>(response.Content);

            return responseBody;
        }

        public static ProvisionamentoHubsoftResponse LimpezaMac (string urlBase, LimpezaMacHubsoft limpezaMac, LoginHubsoftModel login,
          string usuario, string detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            if (limpezaMac == null)
                throw new ArgumentNullException("limpezaMac");

            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/integracao/cliente/reset_mac_addr", Method.POST);

            var token = AutenticacaoHubSoft.Login(urlBase, login);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(limpezaMac);

            var json = JsonSerializer.Serialize(limpezaMac, new JsonSerializerOptions { WriteIndented = true });
            var idLog = enviarLog($"Integração HubSoft [reset_mac_addr] -> {json}", usuario, detalhe, null);

            var response = client.Execute<ProvisionamentoHubsoftResponse>(request);
            enviarLog(response.Content, usuario, detalhe, idLog.Result);

            ProvisionamentoHubsoftResponse responseBody = JsonService.DeserializeObject<ProvisionamentoHubsoftResponse>(response.Content);

            return responseBody;
        }

        public static AtivarContratoHubsoftResponse AtivarContrato(string urlBase, LoginHubsoftModel login, string id_cliente_servico,
           string usuario, string detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            /*if (ativarContratoHubsoft == null)
                throw new ArgumentNullException("ativarContrratoHubsoft");*/

            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/integracao/cliente/cliente_servico/ativar/{id_cliente_servico}", Method.POST);

            var token = AutenticacaoHubSoft.Login(urlBase, login);
            request.AddHeader("Authorization", $"Bearer {token}");

            /*var json = JsonSerializer.Serialize(ativarContratoHubsoft, new JsonSerializerOptions { WriteIndented = true });*/
            var json = id_cliente_servico;
            var idLog = enviarLog($"Integração HubSoft [cliente_servico/ativar] -> {json}", usuario, detalhe, null);

            var response = client.Execute<AtivarContratoHubsoftResponse>(request);
            enviarLog(response.Content, usuario, detalhe, idLog.Result);

            AtivarContratoHubsoftResponse responseBody = JsonService.DeserializeObject<AtivarContratoHubsoftResponse>(response.Content);

            return responseBody;
        }

        public static ProvisionamentoHubsoftResponse HabilitarServico(string urlBase, LoginHubsoftModel login, HabilitarContratoHubsoft habilitarContrato, string id_cliente_servico,
         string usuario, string detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            if (habilitarContrato == null)
                throw new ArgumentNullException("ativarContratoHubsoft");

            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/cliente/servico/habilitar?id_cliente_servico={id_cliente_servico}", Method.POST);

            var token = AutenticacaoHubSoft.Login(urlBase, login);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(habilitarContrato);

            var json = JsonSerializer.Serialize(habilitarContrato, new JsonSerializerOptions { WriteIndented = true });
            var idLog = enviarLog($"Integração HubSoft [servico/habilitar?id_cliente_servico] -> {json}", usuario, detalhe, null);

            var response = client.Execute<ProvisionamentoHubsoftResponse>(request);
            enviarLog(response.Content, usuario, detalhe, idLog.Result);

            ProvisionamentoHubsoftResponse responseBody = JsonService.DeserializeObject<ProvisionamentoHubsoftResponse>(response.Content);

            return responseBody;
        }

        public static ProvisionamentoHubsoftResponse AtualizaCredenciaisWifi(string urlBase, LoginHubsoftModel login, AtualizaCredencialWifi atualizaCredencial,
        string usuario, string detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            if (atualizaCredencial == null)
                throw new ArgumentNullException("atualizaCredencial");

            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/integracao/cliente/cliente_servico/senhas", Method.POST);

            var token = AutenticacaoHubSoft.Login(urlBase, login);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(atualizaCredencial);

            var json = JsonSerializer.Serialize(atualizaCredencial, new JsonSerializerOptions { WriteIndented = true });
            var idLog = enviarLog($"Integração HubSoft [integracao/cliente/cliente_servico/senhas] -> {json}", usuario, detalhe, null);

            var response = client.Execute<ProvisionamentoHubsoftResponse>(request);
            enviarLog(response.Content, usuario, detalhe, idLog.Result);

            ProvisionamentoHubsoftResponse responseBody = JsonService.DeserializeObject<ProvisionamentoHubsoftResponse>(response.Content);

            return responseBody;
        }
    }


}
