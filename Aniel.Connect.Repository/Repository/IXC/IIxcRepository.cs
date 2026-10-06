using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Azza;
using NHibernate.Util;
using Sinapse.DAO.repository;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.Voalle
{
    public interface IIxcRepository
    {
        Equipe GetByMatAux(String MatAux);
        Ordem_Servico GetOrdem_Servico(String Codct, String Projeto, String Num_Obra_Original, Int32 Num_Doc, DateTime DataMaxima);
        String GetUnidadeCusto(String CODCT);
        Tipo_Servico InsertTipoServ(String Descricao, String Codigo_Integrator, String CodCt);
        Tipo_Servico GetTipoServ(String DESCRICAO, String CODCT);
        SubTipo_Servico InsertSubTipoServ(String Descricao, Int32? Cod_Tipo_Serv);
        SubTipo_Servico GetSubTipoServ(String DESCRICAO, Int32? COD_TIPO_SERV);
        ProjetoIxc GetByContratoProjeto(string Contrato, string Projeto);
        LocalidadeIxc GetByLocalidade(String Cidade, String Localidade, string codEscritorio = null);
        EnderecoClienteIxc GetLocalidade(String numeroContrato, String cpf);
        AreaIxc GetByCidade(String Desc_Area, string codEscritorio = null);
        Boolean InsertOrdemServico(Ordem_Servico value);
        Boolean InsertAgendamento(Ordem_Servico servico);
        Boolean InsertCliente(ClienteIxc value);
        Boolean UpdateDadosCliente(ClienteIxc value);
        EnderecoClienteIxc GetLocalidadeEnderecoCliente(String numeroContrato, String cpf);
        long? GetIdContrato(String numeroContrato, String cpf);
        void AtualizaGeneratorClienteConnect();
        Boolean UpdateOrInsertClienteConnect(ClienteIxc value);
        long? UpdateOrInsertEndereco(InfoCliente value, Cliente_ContratoResponse cliente_ContratoResponse, List<CidadeResponse> responseCidade);
        Boolean UpdateOrInsertContrato(InfoCliente value, Cliente_ContratoResponse cliente_ContratoResponse, long? IdEndereco);
    }
}
