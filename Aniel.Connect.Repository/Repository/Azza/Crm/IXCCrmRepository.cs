using Aniel.Connect.Repository.Helper;
using Aniel.Connect.Repository.Model;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using Sinapse.Util;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Repository.Azza.Crm
{
    public class IXCCrmRepository : HttpHelper, IDisposable, IIXCCrmRepository
    {
        private readonly string _url;
        private readonly string _token;

        public IXCCrmRepository(string url, string token)
        {
            _url = url;
            _token = token;
        }

        public ResponseIXC<ClienteIXCResponse> FindByCliente(string cpfCnpj)
        {
            var linkUrl = string.Concat(_url, "cliente");
            var responseStr = ObjRequisicaoCliente(cpfCnpj, linkUrl, false);
            var retorno = JsonService.DeserializeObject<ResponseIXC<ClienteIXCResponse>>(responseStr);

            if (retorno != null && retorno.registros != null) 
            {
                return retorno;
            }
            
            responseStr = ObjRequisicaoCliente(cpfCnpj, linkUrl, true);
            return JsonService.DeserializeObject<ResponseIXC<ClienteIXCResponse>>(responseStr);
        }

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
       
        public ResponseIXC<AtendimentoIXCResponse> FindAtendimentoByContratoCliente(string contrato)
        {
            var linkUrl = string.Concat(_url, "su_ticket");
            var request = new
            {
                qtype = "su_ticket.id_contrato",
                query = $"{contrato}",
                oper = "=",
                page = "1",
                rp = "1",
                sortname = "su_ticket.id_contrato",
                sortorder = "desc"
            };

            var requestStr = JsonService.SerializeObject(request);
            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<AtendimentoIXCResponse>>(responseStr);
        }

        public ResponsePostIXC InserirAtendimento(AtendimentoIXCResponse dadosAtendimento)
        {
            var linkUrl = string.Concat(_url, "su_ticket");

            var requestStr = JsonService.SerializeObject(dadosAtendimento);

            var responseStr = Post(linkUrl, _token, requestStr);

            return JsonService.DeserializeObject<ResponsePostIXC>(responseStr);
        }

        public ResponsePostIXC InserirCliente(ClienteIXCResponse dadosCliente)
        {
            var linkUrl = string.Concat(_url, "cliente");

            var requestStr = JsonService.SerializeObject(dadosCliente);

            var responseStr = Post(linkUrl, _token, requestStr);

            return JsonService.DeserializeObject<ResponsePostIXC>(responseStr);
        }

        public ResponsePostIXC AtualizarCliente(ClienteIXCResponse dadosCliente)
        {
            var linkUrl = string.Concat(_url, "cliente");

            var requestStr = JsonService.SerializeObject(dadosCliente);

            var responseStr = Put(linkUrl, _token, requestStr, dadosCliente.id);

            return JsonService.DeserializeObject<ResponsePostIXC>(responseStr);
        }

        public ResponsePostIXC InserirContrato(ClienteContratoIXCResponse dadosClienteContrato)
        {
            var linkUrl = string.Concat(_url, "cliente_contrato");

            var requestStr = JsonService.SerializeObject(dadosClienteContrato);

            var responseStr = Post(linkUrl, _token, requestStr);

            return JsonService.DeserializeObject<ResponsePostIXC>(responseStr);
        }

        public ResponsePostIXC InserirLogin(LoginIXCResponse dadosLogin)
        {
            var linkUrl = string.Concat(_url, "radusuarios");

            var requestStr = JsonService.SerializeObject(dadosLogin);

            var responseStr = Post(linkUrl, _token, requestStr);

            return JsonService.DeserializeObject<ResponsePostIXC>(responseStr);
        }

        public ResponseIXC<LoginIXCResponse> FindByLogin_Contrato(string idContrato)
        {
            var linkUrl = string.Concat(_url, "radusuarios");
            var request = new
            {
                qtype = "radusuarios.id_contrato",
                query = $"{idContrato}",
                oper = "=",
                page = "1",
                rp = "1",
                sortname = "radusuarios.id_contrato",
                sortorder = "desc"
            };

            var requestStr = JsonService.SerializeObject(request);
            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<LoginIXCResponse>>(responseStr);
        }

        public ResponseIXC<RadGruposIXCResponse> FindByRadGrupo_Contrato(string idContrato)
        {
            var linkUrl = string.Concat(_url, "view_radgrupos_vd_contratos_produtos");
            var request = new
            {
                qtype = "view_radgrupos_vd_contratos_produtos.cliente_contrato_id",
                query = $"{idContrato}",
                oper = "=",
                page = "1",
                rp = "1",
                sortname = "view_radgrupos_vd_contratos_produtos.cliente_contrato_id",
                sortorder = "desc"
            };

            var requestStr = JsonService.SerializeObject(request);
            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<RadGruposIXCResponse>>(responseStr);
        }

        public ResponseIXC<VdContratosProdutosIXCResponse> FindByVdContratosProdutos(string idContrato)
        {
            var linkUrl = string.Concat(_url, "vd_contratos_produtos");
            var request = new
            {
                qtype = "vd_contratos_produtos.id_vd_contrato",
                query = $"{idContrato}",
                oper = "=",
                page = "1",
                rp = "1",
                sortname = "vd_contratos_produtos.id_vd_contrato",
                sortorder = "desc",
                grid_param = JsonService.SerializeObject(new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "TB", "vd_contratos_produtos.tipo" },
                        { "OP", "=" },
                        { "P", "I" }
                    },
                    new Dictionary<string, string>
                    {
                        { "TB", "vd_contratos_produtos.id_plano" },
                        { "OP", "!=" },
                        { "P", "0" }
                    }
                })
            };

            var requestStr = JsonService.SerializeObject(request);
            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<VdContratosProdutosIXCResponse>>(responseStr);
        }

        public ResponseIXC<CidadeIXCResponse> RequestCidade(string cod_ibge)
        {
            var linkUrl = string.Concat(_url, "cidade");
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

            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<CidadeIXCResponse>>(responseStr);
        }

        public ResponseIXC<SetorIXCResponse> FindBySetor(string id_processo)
        {
            var linkUrl = string.Concat(_url, "wfl_processo");
            var request = new
            {
                qtype = "wfl_processo.id",
                query = $"{id_processo}",
                oper = "=",
                rp = "1",
                sortname = "wfl_processo.id",
                sortorder = "asc"
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<SetorIXCResponse>>(responseStr);
        }

        public ResponseIXC<AssuntoIXCResponse> FindByAssunto(string id_assunto)
        {
            var linkUrl = string.Concat(_url, "su_oss_assunto");
            var request = new
            {
                qtype = "su_oss_assunto.id",
                query = $"{id_assunto}",
                oper = "=",
                rp = "1",
                sortname = "su_oss_assunto.id",
                sortorder = "asc"
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<AssuntoIXCResponse>>(responseStr);
        }

        public ResponseIXC<TipoClienteIXCResponse> FindByTipoCliente(string id_tipo_cliente)
        {
            var linkUrl = string.Concat(_url, "tipo_cliente");
            var request = new
            {
                qtype = "tipo_cliente.id",
                query = $"{id_tipo_cliente}",
                oper = "=",
                rp = "1",
                sortname = "tipo_cliente.id",
                sortorder = "asc"
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<TipoClienteIXCResponse>>(responseStr);
        }

        public ResponseIXC<ClienteContratoIXCResponse> FindByIdContratoCliente(string id_contrato_cliente)
        {
            var linkUrl = string.Concat(_url, "cliente_contrato");
            var request = new
            {
                qtype = "cliente_contrato.id",
                query = $"{id_contrato_cliente}",
                oper = "=",
                rp = "1",
                sortname = "cliente_contrato.id",
                sortorder = "asc"
            };

            var requestStr = JsonService.SerializeObject(request);

            var responseStr = Get(linkUrl, _token, requestStr, "listar");

            return JsonService.DeserializeObject<ResponseIXC<ClienteContratoIXCResponse>>(responseStr);
        }
    }
}
