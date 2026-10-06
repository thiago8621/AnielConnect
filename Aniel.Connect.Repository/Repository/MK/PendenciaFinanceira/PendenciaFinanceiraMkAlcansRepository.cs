using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.MkAlcans;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aniel.Connect.Repository.MK.PendenciaFinanceira
{
    public class PendenciaFinanceiraMkAlcansRepository : IPendenciaFinanceiraIntegracaoRepository
    {
        private readonly string UsuarioApi, SenhaApi, UrlApi;

        public PendenciaFinanceiraMkAlcansRepository(string usuarioApi, string senhaApi, string urlApi)
        {
            UsuarioApi = usuarioApi;
            SenhaApi = senhaApi;
            UrlApi = urlApi;
        }

        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj)
        {
            var dados = new List<PendenciaFinanceiraListModel>();

            RestClient client = new RestClient(UrlApi);
            RestRequest request = new RestRequest("api-hub/contract/latterFeeByCustomer", Method.GET);

            request.AddHeader("username", UsuarioApi);
            request.AddHeader("password", SenhaApi);
            request.AddQueryParameter("document", FormatarCpfCnpj(cpfCnpj));

            var response = client.Execute<RetornoConsultaPendenciaCliente>(request);

            if (response.Data != null && response.Data.fees != null && response.Data.fees.Count > 0)
            {
                foreach (var pendenciaMk in response.Data.fees)
                {
                    dados.Add(new PendenciaFinanceiraListModel()
                    {
                        Cod_Cliente = pendenciaMk.customer.ToString(),
                        Cod_Contrato = pendenciaMk.contract.ToString(),
                        CodFatura = pendenciaMk.fee.ToString(),
                        CpfCnpj = pendenciaMk.document,
                        Data_Vencimento = pendenciaMk.due,
                        Valor_Total = pendenciaMk.amount,
                        Bairro = pendenciaMk.neighborhood,
                        Logradouro = pendenciaMk.street,
                        Numero = pendenciaMk.house_number,
                        Cidade = pendenciaMk.city,
                    });
                }
            }

            return dados;
        }

        static string FormatarCpfCnpj(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return documento;

            // Remove tudo que não for dígito
            string apenasDigitos = Regex.Replace(documento, @"\D", "");

            if (apenasDigitos.Length == 11)
                return FormatarCpf(apenasDigitos);
            else if (apenasDigitos.Length == 14)
                return FormatarCnpj(apenasDigitos);
            else
                return documento; // ou throw new ArgumentException("CPF ou CNPJ inválido");
        }

        private static string FormatarCpf(string cpf)
        {
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        private static string FormatarCnpj(string cnpj)
        {
            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }
    }
}