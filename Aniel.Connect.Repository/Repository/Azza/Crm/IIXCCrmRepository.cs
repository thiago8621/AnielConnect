using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Azza;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.Azza.Crm
{
    public interface IIXCCrmRepository
    {
        ResponsePostIXC InserirCliente(ClienteIXCResponse dadosCliente);
        ResponsePostIXC AtualizarCliente(ClienteIXCResponse dadosCliente);
        ResponsePostIXC InserirContrato(ClienteContratoIXCResponse dadosClienteContrato);
        ResponsePostIXC InserirLogin(LoginIXCResponse dadosLogin);
        ResponsePostIXC InserirAtendimento(AtendimentoIXCResponse dadosAtendimento);
        ResponseIXC<ClienteIXCResponse> FindByCliente(string cpfCnpj);
        ResponseIXC<LoginIXCResponse> FindByLogin_Contrato(string idContrato);
        ResponseIXC<RadGruposIXCResponse> FindByRadGrupo_Contrato(string idContrato);
        ResponseIXC<VdContratosProdutosIXCResponse> FindByVdContratosProdutos(string idContrato);
        ResponseIXC<CidadeIXCResponse> RequestCidade(string cod_ibge);
        ResponseIXC<SetorIXCResponse> FindBySetor(string id_processo);
        ResponseIXC<AssuntoIXCResponse> FindByAssunto(string id_assunto);
        ResponseIXC<TipoClienteIXCResponse> FindByTipoCliente(string id_tipo_cliente);
        ResponseIXC<ClienteContratoIXCResponse> FindByIdContratoCliente(string id_contrato_cliente);
        ResponseIXC<AtendimentoIXCResponse> FindAtendimentoByContratoCliente(string id_contrato_cliente);
    }
}
