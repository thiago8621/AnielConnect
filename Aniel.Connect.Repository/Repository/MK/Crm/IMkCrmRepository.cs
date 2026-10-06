using Aniel.Connect.Repository.Model.Mk;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.MK.Crm
{
    public interface IMkCrmRepository
    {
        /// <summary>
        ///     Traz uma listagem com os endereços cadastrados no MK
        /// </summary>
        IEnumerable<DadosEstruturaEndereco> GetEnderecos();

        /// <summary>
        ///     Executa uma requisição para inserir uma cidade no sistema do MK
        /// </summary>
        /// <param name="dadosCidade">Objeto com as informações da cidade que será inserida no MK</param>
        InserirCidadeResponse InserirCidade(InserirCidadeRequest dadosCidade);

        /// <summary>
        ///     Executa uma requisição para inserir um bairro no sistema do MK
        /// </summary>
        /// <param name="dadosBairro">Objeto com as informações do bairro que será inserido no MK</param>
        InserirBairroResponse InserirBairro(InserirBairroRequest dadosBairro);

        /// <summary>
        ///     Executa uma requisição para inserir um logradouro no sistema do MK
        /// </summary>
        /// <param name="dadosLogradouro">Objeto com as informações do logradouro que será inserido no MK</param>
        InserirLogradouroResponse InserirLogradouro(InserirLogradouroRequest dadosLogradouro);

        /// <summary>
        ///     Retorna os dados de um cliente caso ele esteja registrado no MK Solutions
        /// </summary>
        /// <param name="documento">CPF ou CNPJ do cliente</param>
        GetDadosClienteByDocMkResponse GetDadosClienteByDocumento(string documento);

        /// <summary>
        ///     Insere um cliente no MK
        /// </summary>
        /// <param name="dadosCliente">Dados do cliente que será inserido no sistema</param>
        InserirClienteResponse InserirCliente(InserirClienteRequest dadosCliente);

        /// <summary>
        ///     Insere um contrato no MK
        /// </summary>
        /// <param name="dadosContrato">Dados do contrato que será inserido no sistema</param>
        InserirContratoResponse InserirContrato(InserirContratoRequest dadosContrato);

        /// <summary>
        ///     Insere uma conexão no MK
        /// </summary>
        /// <param name="dadosConexao">Dados da conexão que será inserido no sistema</param>
        InserirConexaoResponse InserirConexao(InserirConexaoRequest dadosConexao);

        /// <summary>
        ///     Insere um Atendimento no Mk
        /// </summary>
        InserirAtendimentoResponse InserirAtendimento(InserirAtendimentoRequest dadosAtendimento);

        /// <summary>
        ///     Insere uma OS no MK
        /// </summary>
        InserirOrdemServicoResponse InserirOrdemServico(CriarOrdemServicoRequest parametros);

    }
}
