using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Helper;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Azza;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using MongoDB.Bson.IO;
using Npgsql;
using RestSharp;
using Sinapse.DAO.repository;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.Azza.PendenciaFinanceira
{
    public class PendenciaFinanceiraIXCRepository : HttpHelper, IDisposable, IPendenciaFinanceiraIntegracaoRepository
    {
        private readonly string _url;
        private readonly string _token;

        public PendenciaFinanceiraIXCRepository(string url, string token)
        {
            _url = url;
            _token = token;
        }

        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj)
        {
            var dados = new List<PendenciaFinanceiraListModel>();
            try
            {
                var linkUrl = string.Concat(_url, "cliente");

                var responseStr = ObjRequisicaoCliente(cpfCnpj, linkUrl, false);

                var cliente = JsonService.DeserializeObject<ResponseIXC<ClienteIXCResponse>>(responseStr);
                if (cliente.registros == null)
                {
                    responseStr = ObjRequisicaoCliente(cpfCnpj, linkUrl, true);
                    cliente = JsonService.DeserializeObject<ResponseIXC<ClienteIXCResponse>>(responseStr);
                }
                if (cliente.registros == null)
                {
                    return dados;
                }

                foreach (var item in cliente.registros)
                {
                    linkUrl = string.Concat(_url, "fn_areceber");
                    var registros = RequestBoleto(linkUrl, item.id).Result;
                    if (registros.registros != null)
                    {
                        foreach (var item01 in registros.registros)
                        {
                            bool faturasuspensa = Convert.ToDateTime(item01.data_vencimento) < DateTime.Now.Date;
                            if (faturasuspensa)
                            {
                                dados.Add(new PendenciaFinanceiraListModel
                                {
                                    Cod_Cliente = item01.id_cliente,
                                    Cliente = item.razao,
                                    CpfCnpj = item.cnpj_cpf,
                                    Logradouro = item.endereco,
                                    Numero = item.numero,
                                    Bairro = item.bairro,
                                    Cidade = item.cidade,
                                    Data_Vencimento = item01.data_vencimento,
                                    Fatura_Suspensa = $"{Convert.ToInt16(faturasuspensa)}"
                                });
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na comunicação com o IXC: {ex.Message}");
            }

            return dados;
        }

        #region [RequestCliente]
        private async Task<ResponseIXC<ClienteIXCResponse>> RequestCliente(string url, string cpfCnpj)
        {
            var request = new
            {
                qtype = "cliente.cnpj_cpf",
                query = $"{cpfCnpj}",
                oper = "=",
                page = "1",
                rp = "1",
                sortname = "cliente.id",
                sortorder = "desc"
            };
            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(url, _token, requestStr, "listar");

            return await Task.FromResult(JsonService.DeserializeObject<ResponseIXC<ClienteIXCResponse>>(responseStr));
        }
        #endregion

        #region [Request Boleto]
        private async Task<ResponseIXC<BoletoIXCCliente>> RequestBoleto(string url, string id)
        {
            var request = new
            {
                qtype = "fn_areceber.id_cliente",
                query = $"{id}",
                oper = "=",
                rp = "200000",
                sortname = "fn_areceber.data_vencimento",
                sortorder = "asc",
                grid_param = JsonService.SerializeObject(new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "TB", "fn_areceber.liberado" },
                        { "OP", "=" },
                        { "P", "S" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "fn_areceber.status" },
                        { "OP", "!=" },
                        { "P", "C" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "fn_areceber.status" },
                        { "OP", "!=" },
                        { "P", "R" }
                    }
                })
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(url, _token, requestStr, "listar");

            return await Task.FromResult(JsonService.DeserializeObject<ResponseIXC<BoletoIXCCliente>>(responseStr));
        }
        #endregion

        #region [ObjRequisicaoCliente]
        private string ObjRequisicaoCliente(string cpfCnpj, string linkUrl, bool flagFormatarCampo)
        {
            var caracteres = new List<string>() { "/", "-", "." };
            foreach (var item in caracteres)
            {
                cpfCnpj = cpfCnpj.Replace(item, string.Empty);
            }

            if (flagFormatarCampo)
            {
                cpfCnpj = cpfCnpj.Length <= 11 ? Convert.ToUInt64(cpfCnpj).ToString(@"000\.000\.000\-00") :
                    Convert.ToUInt64(cpfCnpj).ToString(@"00\.000\.000\/0000\-00");
            }

            var request = new
            {
                qtype = "cliente.cnpj_cpf",
                query = $"{cpfCnpj}",
                oper = "=",
                page = "1",
                rp = "1",
                sortname = "cliente.id",
                sortorder = "desc"
            };

            var requestStr = JsonService.SerializeObject(request);
            return Get(linkUrl, _token, requestStr, "listar");
        }
        #endregion
    }
}
