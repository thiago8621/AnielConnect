using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Model.Hubsoft.ClienteModel;
using RestSharp;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Repository.HubSoft
{
    public static class ClienteDao
    {
        public static ResponseConsultaCliente GetCliente(string urlBase, LoginHubsoftModel login, int codigoCliente)
        {
            ResponseConsultaCliente retorno = new ResponseConsultaCliente
            {
                clientes = new List<Model.Hubsoft.ClienteModel.Cliente>()
            };

            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/integracao/cliente?busca=codigo_cliente&termo_busca={codigoCliente}&inativo=todos", Method.GET);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            request.AddHeader("Authorization", $"Bearer {token}");

            retorno = client.Execute<ResponseConsultaCliente>(request).Data;

            return retorno;
        }
    }
}