using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.Helper;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Azza;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.Azza.PendenciaEndereco
{
    public class PendenciaEnderecoIntegracaoRepository : HttpHelper, IDisposable, IPendenciaEnderecoIntegracaoRepository
    {
        private readonly string _url;
        private readonly string _token;
        private readonly ICidadeRepository _cidadeRepository;
        public PendenciaEnderecoIntegracaoRepository(string url, string token, ICidadeRepository cidadeRepository)
        {
            _url = url;
            _token = token;
            _cidadeRepository = cidadeRepository;
        }

        public async Task<IEnumerable<PendenciaEnderecoListModel>> GetPendenciasEndereco(string endereco, string numero, string id_cidade, string cep)
        {
            var dados = new List<PendenciaEnderecoListModel>();
            try
            {
                var linkUrl = string.Concat(_url, "cidade");
                var objCidade = _cidadeRepository.GetById(id_cidade);
                var cidade = RequestCidade(linkUrl, objCidade.Codigo_IBGE).Result.registros[0];

                linkUrl = string.Concat(_url, "cliente_contrato");
                var cliente_contrato = RequestClienteContrato(linkUrl, cidade.id, endereco, numero, cep).Result;
                if (cliente_contrato.registros == null)
                {
                    linkUrl = string.Concat(_url, "cliente");
                    var clientes = RequestCliente(linkUrl, cidade.id, endereco, numero, cep).Result;
                    if (clientes.registros != null)
                    {
                        cliente_contrato.registros = new List<ClienteContratoIXCResponse>();
                        clientes.registros.ToList().ForEach(x =>
                        {
                            linkUrl = string.Concat(_url, "cliente_contrato");
                            var cliente_contrato_tmp = RequestClienteContrato_Id(linkUrl, x.id)?.Result?.registros;

                            if (cliente_contrato_tmp != null)
                            {
                                if (x.flagCEP)
                                {
                                    cliente_contrato_tmp.ForEach(y =>
                                    {
                                        y.flagCEP = true;
                                    });
                                }
                                cliente_contrato.registros.AddRange(cliente_contrato_tmp);
                            }
                        });
                    }
                }

                if (cliente_contrato.registros != null)
                {
                    cliente_contrato.registros.ToList().ForEach(itemClienteContrato =>
                    {
                        linkUrl = string.Concat(_url, "cliente");
                        var cliente = ObjRequisicaoCliente(itemClienteContrato.id_cliente, linkUrl).Result.registros[0];

                        linkUrl = string.Concat(_url, "fn_areceber");
                        var boleto = RequestBoleto(linkUrl, itemClienteContrato.id).Result;
                        if (boleto.registros != null)
                        {
                            boleto.registros.ToList().ForEach(itemBoleto =>
                            {
                                dados.Add(new PendenciaEnderecoListModel
                                {
                                    Cod_Contrato = itemClienteContrato.contrato,
                                    //CodConexao = ,
                                    Cod_Cliente = cliente.id,
                                    Cpf = cliente.cnpj_cpf,
                                    //Contrato_Cancelado = ,
                                    //Conexao_Bloqueada = ,
                                    Logradouro = itemClienteContrato.endereco,
                                    Bairro = itemClienteContrato.bairro,
                                    Cidade = objCidade.Descricao,
                                    //CodFatura = ,
                                    Valor_Total = itemBoleto.valor,
                                    //Liquidado = ,
                                    //Descricao = ,
                                    //Fatura_Suspensa = ,
                                    Data_Vencimento = itemBoleto.data_vencimento,
                                    //Motivo_Suspensao = ,
                                    Numero = numero,
                                    Cliente = cliente.razao,
                                    FlagCEP = itemClienteContrato.flagCEP
                                });
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na comunicação com o IXC: {ex.Message}");
            }

            return await Task.FromResult(dados);
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

        #region [RequestClienteContrato]
        private async Task<ResponseIXC<ClienteContratoIXCResponse>> RequestClienteContrato(string url, string idCidade, string endereco, string numero, string cep)
        {
            var retorno = GetRequestClienteContrato(url, idCidade, endereco, numero, cep, false);
            if (retorno.registros != null)
            {
                retorno.registros.ToList().ForEach(x =>
                {
                    x.flagCEP = false;
                });

                return await Task.FromResult(retorno);
            }

            retorno = GetRequestClienteContrato(url, idCidade, endereco, numero, cep, true);
            if (retorno.registros != null)
            {
                retorno.registros.ToList().ForEach(x =>
                {
                    x.flagCEP = true;
                });

                return await Task.FromResult(retorno);
            }

            return await Task.FromResult(retorno);
        }
        #endregion

        #region [RequestClienteContrato - Id]
        private async Task<ResponseIXC<ClienteContratoIXCResponse>> RequestClienteContrato_Id(string url, string id_cliente)
        {
            var request = new
            {
                qtype = "cliente_contrato.id_cliente",
                query = $"{id_cliente}",
                oper = "=",
                page = "1",
                rp = "100",
                sortname = "cliente_contrato.id",
                sortorder = "desc",
                grid_param = JsonService.SerializeObject(new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente_contrato.endereco_padrao_cliente" },
                        { "OP", "=" },
                        { "P", "S" }
                    }
                })
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(url, _token, requestStr, "listar");

            return await Task.FromResult(JsonService.DeserializeObject<ResponseIXC<ClienteContratoIXCResponse>>(responseStr));
        }
        #endregion

        #region [Request Boleto]
        private async Task<ResponseIXC<BoletoIXCCliente>> RequestBoleto(string url, string id)
        {
            var request = new
            {
                qtype = "fn_areceber.id_contrato",
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

        #region [Request Cidade]
        private async Task<ResponseIXC<CidadeIXCResponse>> RequestCidade(string url, string cod_ibge)
        {
            var request = new
            {
                qtype = "cidade.cod_ibge",
                query = $"{cod_ibge}",
                oper = "=",
                rp = "1",
                sortname = "cidade.cod_ibge",
                sortorder = "asc"
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(url, _token, requestStr, "listar");

            return await Task.FromResult(JsonService.DeserializeObject<ResponseIXC<CidadeIXCResponse>>(responseStr));
        }
        #endregion

        #region [ObjRequisicaoCliente]
        private async Task<ResponseIXC<ClienteIXCResponse>> ObjRequisicaoCliente(string idCliente, string linkUrl)
        {
            var request = new
            {
                qtype = "cliente.id",
                query = $"{idCliente}",
                oper = "=",
                page = "1",
                rp = "1",
                sortname = "cliente.id",
                sortorder = "desc"
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return await Task.FromResult(JsonService.DeserializeObject<ResponseIXC<ClienteIXCResponse>>(responseStr));
        }
        #endregion

        #region [ObjRequisicaoCliente - Cidade]
        private async Task<ResponseIXC<ClienteIXCResponse>> ObjRequisicaoClienteCidade(string linkUrl, string idCidade, string endereco, string numero, string cep, bool flagCEP)
        {
            var grid_param = new List<Dictionary<string, string>>();

            if (!flagCEP)
            {
                grid_param = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente.endereco" },
                        { "OP", "=" },
                        { "P", $"{endereco}" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente.numero" },
                        { "OP", "=" },
                        { "P", $"{numero}" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente.cidade" },
                        { "OP", "=" },
                        { "P", $"{idCidade}" }
                    }
                };
            }
            else
            {
                grid_param = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente.cep" },
                        { "OP", "=" },
                        { "P", $"{cep}" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente.numero" },
                        { "OP", "=" },
                        { "P", $"{numero}" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente.cidade" },
                        { "OP", "=" },
                        { "P", $"{idCidade}" }
                    }
                };
            }

            var request = new
            {
                qtype = "cliente.id",
                query = "",
                oper = "=",
                page = "1",
                rp = "1",
                sortname = "cliente.id",
                sortorder = "desc",
                grid_param = JsonService.SerializeObject(grid_param)
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return await Task.FromResult(JsonService.DeserializeObject<ResponseIXC<ClienteIXCResponse>>(responseStr));
        }

        private async Task<ResponseIXC<ClienteIXCResponse>> RequestCliente(string url, string idCidade, string endereco, string numero, string cep)
        {
            var retorno = ObjRequisicaoClienteCidade(url, idCidade, endereco, numero, cep, false).Result;
            if (retorno.registros != null)
            {
                retorno.registros.ToList().ForEach(x =>
                {
                    x.flagCEP = false;
                });

                return await Task.FromResult(retorno);
            }

            retorno = ObjRequisicaoClienteCidade(url, idCidade, endereco, numero, cep, true).Result;
            if (retorno.registros != null)
            {
                retorno.registros.ToList().ForEach(x =>
                {
                    x.flagCEP = true;
                });

                return await Task.FromResult(retorno);
            }

            return await Task.FromResult(retorno);
        }

        #endregion

        #region [GetRequestClienteContrato]
        private ResponseIXC<ClienteContratoIXCResponse> GetRequestClienteContrato(string url, string idCidade, string endereco, string numero, string cep, bool flagCEP)
        {
            var grid_param = new List<Dictionary<string, string>>();
            if (!flagCEP)
            {
                grid_param = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente_contrato.endereco" },
                        { "OP", "=" },
                        { "P", $"{endereco}" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente_contrato.numero" },
                        { "OP", "=" },
                        { "P", $"{numero}" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente_contrato.cidade" },
                        { "OP", "=" },
                        { "P", $"{idCidade}" }
                    }
                };
            }
            else
            {
                var caracteres = new List<string>() { "-", "." };

                foreach (var item in caracteres)
                {
                    cep = cep.Replace(item, string.Empty);
                }
                cep = Convert.ToUInt64(cep).ToString(@"00000\-000");

                grid_param = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente_contrato.cep" },
                        { "OP", "=" },
                        { "P", $"{cep}" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente_contrato.numero" },
                        { "OP", "=" },
                        { "P", $"{numero}" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "cliente_contrato.cidade" },
                        { "OP", "=" },
                        { "P", $"{idCidade}" }
                    }
                };
            }

            var request = new
            {
                qtype = "cliente_contrato.id_cliente",
                query = "",
                oper = "=",
                page = "1",
                rp = "100",
                sortname = "cliente_contrato.id",
                sortorder = "desc",
                grid_param = JsonService.SerializeObject(grid_param)
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(url, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<ClienteContratoIXCResponse>>(responseStr);
        }

        #endregion
    }
}
