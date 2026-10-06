using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.MkAlcans;
using Npgsql;
using RestSharp;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Aniel.Connect.Repository.MK.PendenciaEndereco
{
    public class PendenciaEnderecoMkAlcansRepository : IPendenciaEnderecoMkRepository
    {
        private readonly string UsuarioApi, SenhaApi, UrlApi;

        public PendenciaEnderecoMkAlcansRepository(string usuarioApi, string senhaApi, string urlApi)
        {
            UsuarioApi = usuarioApi;
            SenhaApi = senhaApi;
            UrlApi = urlApi;
        }

        public RetornoConsultaDescontos BuscarFormasPagamento(string codigoCombo)
        {
            RestClient client = new RestClient(UrlApi);
            RestRequest request = new RestRequest("api-hub/contract/paymentmethods", Method.GET);

            request.AddHeader("username", UsuarioApi);
            request.AddHeader("password", SenhaApi);

            request.AddQueryParameter("productCode", codigoCombo);

            var response = client.Execute<RetornoConsultaDescontos>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return response.Data;
            else
                throw new Exception($"Erro ao consultar descontos do produto CRM {codigoCombo}:  {response.Content}");
        }

        public IEnumerable<PendenciaEnderecoListModel> GetPendenciasEndereco(string logradouro, string numero, string cidade)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PendenciaEnderecoListModel> GetPendenciasEnderecoAlcans(string codigoUF, string codigoCidade, string codigoBairro, string codigoLogradouro, string numero)
        {
            var dados = new List<PendenciaEnderecoListModel>();

            RestClient client = new RestClient(UrlApi);
            RestRequest request = new RestRequest("api-hub/contract/latterFeesByAddress", Method.GET);

            request.AddHeader("username", UsuarioApi);
            request.AddHeader("password", SenhaApi);

            //request.AddQueryParameter("uf", codigoUF);
            //request.AddQueryParameter("city", codigoCidade);
            //request.AddQueryParameter("neighborhood", codigoBairro);
            request.AddQueryParameter("street", codigoLogradouro);
            request.AddQueryParameter("houseNumber", numero);

            var response = client.Execute<RetornoConsultaPendenciaEndereco>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Data != null && response.Data.latterFee != null && response.Data.latterFee.Count > 0)
            {
                foreach (var item in response.Data.latterFee)
                {
                    dados.Add(new PendenciaEnderecoListModel
                    {
                        Cod_Cliente = item.customer,
                        Cod_Contrato = item.contract,
                        CodFatura = item.fee,
                        Cpf = item.document,
                        Data_Vencimento = item.due,
                        Valor_Total = item.amount,
                        Cidade = item.city,
                        Bairro = item.neighborhood,
                        Logradouro = item.street,
                        Numero = item.number
                    });
                }
            }

            return dados;
        }
    }
}