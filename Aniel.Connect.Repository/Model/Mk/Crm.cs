using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model.Mk
{
    public abstract class BaseCrmMkResponse
    {
        /// <summary>
        ///     "OK" ou "ERRO"
        /// </summary>
        public string status { get; set; }

        /// <summary>
        ///     Mensagem para descrição do erro na requisição
        /// </summary>
        public string Mensagem { get; set; }
    }

    public class AutenticacaoCrmResponse : BaseCrmMkResponse
    {
        /// <summary>
        ///     Token que deverá ser utilizado para autenticação nas APIs do CRM do MK Solutions
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     Data e hora de expiração do token
        /// </summary>
        public string Expire { get; set; }
    }

    /// <summary>
    ///     Informações do endereço conforme o MK Solutions retorna na listagem de endereços
    /// </summary>
    public class DadosEstruturaEndereco
    {
        public long? cd_uf { get; set; }
        public long? cd_cidade { get; set; }
        public long? cd_bairro { get; set; }
        public long? cd_logradouro { get; set; }

        public string siglaestado { get; set; }
        public string cidade { get; set; }
        public string bairro { get; set; }
        public string logradouro { get; set; }
    }

    /// <summary>
    ///     Objeto de resposta do endpoint da listagem de endereços do MK Solutions
    /// </summary>
    public class ListaEstruturaEnderecosResponse : BaseCrmMkResponse
    {
        private IEnumerable<DadosEstruturaEndereco> _listaEnderecos = null;

        public IEnumerable<DadosEstruturaEndereco> ListaEnderecos
        {
            get
            {
                if (this._listaEnderecos == null)
                    this._listaEnderecos = new List<DadosEstruturaEndereco>();

                return this._listaEnderecos;
            }
            set
            {
                this._listaEnderecos = value;
            }
        }
    }

    public abstract class BaseCrmInserirDadosEnderecoResponse
    {
        /// <summary>
        ///     Status HTTP da requisição
        /// </summary>
        public int statusCode { get; set; }

        /// <summary>
        ///     Mensagem descrevendo o erro, casa haja algum problema no processamento da requisição
        /// </summary>
        public string message { get; set; }
    }

    public class InserirCidadeRequest
    {
        /// <summary>
        ///     Nome da cidade
        /// </summary>
        public string nome { get; set; }

        /// <summary>
        ///     Código do IBGE para a cidade
        /// </summary>
        public string ibge { get; set; }

        /// <summary>
        ///     Código do estado
        /// </summary>
        public long estadoId { get; set; }
    }

    public class InserirCidadeResponse : BaseCrmInserirDadosEnderecoResponse
    {
        public long id { get; set; }
        public string nome { get; set; }
        public string ibge { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string dataCadastro { get; set; }
        public long estadoId { get; set; }
    }

    public class InserirBairroRequest
    {
        /// <summary>
        ///     Nome do bairro
        /// </summary>
        public string nome { get; set; }

        /// <summary>
        ///     Código da cidade onde o bairro se encontra
        /// </summary>
        public long cidadeId { get; set; }
    }

    public class InserirBairroResponse : BaseCrmInserirDadosEnderecoResponse
    {
        public long id { get; set; }
        public string nome { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string dataCadastro { get; set; }
        public long cidadeId { get; set; }
    }

    public class InserirLogradouroRequest
    {
        /// <summary>
        ///     Nome do logradouro
        /// </summary>
        public string nome { get; set; }

        /// <summary>
        ///     CEP do logradouro
        /// </summary>
        public string cep { get; set; }

        /// <summary>
        ///     Código da cidade onde o bairro se encontra
        /// </summary>
        public long cidadeId { get; set; }

        /// <summary>
        ///     Código do bairro onde o logradouro se encontra
        /// </summary>
        public long bairroId { get; set; }
    }

    public class InserirLogradouroResponse : BaseCrmInserirDadosEnderecoResponse
    {
        public long id { get; set; }
        public string nome { get; set; }
        public string cep { get; set; }
        public string dataCadastro { get; set; }
        public long cidadeId { get; set; }
        public long bairroId { get; set; }
    }

    public class GetDadosClienteByDocMkResponse : BaseCrmMkResponse
    {
        /// <summary>
        ///     ID do cliente no MK Solutions
        /// </summary>
        public long CodigoPessoa { get; set; }
        public string CEP { get; set; }
        public string Email { get; set; }
        public string Endereco { get; set; }
        public string Fone { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Nome { get; set; }
        public string Situacao { get; set; }
    }

    public class InserirClienteRequest
    {
        public int tipo_cadastro;
        public string cliente_com;
        public string revenda;
        public string grupo_atend;

        /// <summary>
        ///     CPF/CNPJ do cliente
        /// </summary>
        public string doc { get; set; }
        public string nome { get; set; }
        public string cep { get; set; }
        /// <summary>
        ///     Código da UF no MK Solutions
        /// </summary>
        public string cd_uf { get; set; }
        /// <summary>
        ///     Código da cidade no MK Solutions
        /// </summary>
        public string cd_cidade { get; set; }
        /// <summary>
        ///     Código do bairro no MK Solutions
        /// </summary>
        public string cd_bairro { get; set; }
        /// <summary>
        ///     Código do logradouro no MK Solutions
        /// </summary>
        public string cd_logradouro { get; set; }

        /// <summary>
        ///     Número do endereço
        /// </summary>
        public string numero { get; set; }

        public string complemento { get; set; }
        public string cd_empresa { get; set; }
        public string email { get; set; }
        /// <summary>
        ///     Dia, mês e ano de nascimento do cliente
        /// </summary>
        public string nascimento { get; set; }
        /// <summary>
        ///     Número de telefone do cliente
        /// </summary>
        public string fone { get; set; }
        /// <summary>
        ///     Latitude do endereço
        /// </summary>
        public string lat { get; set; }
        /// <summary>
        ///     Longitude do endereço
        /// </summary>
        public string lon { get; set; }
        public string nome_mae { get; set; }
        public string rg { get; set; }
        public string user_sac { get; set; }
        public string senha_sac { get; set; }
    }

    public class InserirClienteResponse : BaseCrmMkResponse
    {
        /// <summary>
        ///     Código do cliente que foi inserido no MK
        /// </summary>
        public string CodigoPessoa { get; set; }
    }

    public class InserirContratoRequest
    {
        public string CodigoCliente { get; set; }
        public string CodigoTipoPlano { get; set; }
        public string CodigoPlanoAcesso { get; set; }
        public string CodigoRegraVencimento { get; set; }
        /// <summary>
        ///     Campo definido pelo tipo de pessoa do cliente (física ou jurídica)
        ///     <para>
        ///         Valores aceitos: 1 para plano residencial, 2 para plano corporativo.
        ///     </para>
        /// </summary>
        public string CodigoSLA { get; set; }
        public string IgnorarRenovacaoAutomatica { get; set; }
        public string ValorSugeridoRenovacao { get; set; }
        public string CodigoRegraBloqueio { get; set; }
        public string CodigoFormaPagamento { get; set; }
        public string CodigoProfilePagamento { get; set; }
        public string CodigoMetodoFaturamento { get; set; }
        public string CodigoPlanoContas { get; set; }
        public string ConsultorCRM { get; set; }
        public string AguardaAtivacao { get; set; }
        public string CodigoDescontoAplicado { get; set; }
        public string cd_empresa { get; set; }
    }

    public class InserirContratoResponse : BaseCrmMkResponse
    {

    }

    public class InserirConexaoRequest
    {
        public string CodigoCliente { get; set; }
        public string CodigoPontoAcesso { get; set; }
        public string CodigoContrato { get; set; }
        public string CodigoServidor { get; set; }
        public string CodigoPlanoAcesso { get; set; }
        public string CodigoTipoIP { get; set; }
        public string Tecnologia { get; set; }
        public string AutoDesbloqueio { get; set; }
        public string MacAddress { get; set; }
        public string EnderecoCadastro { get; set; }
        public string TipoConexao { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CodigoEstado { get; set; }
        public string CodigoCidade { get; set; }
        public string CodigoBairro { get; set; }
        public string CodigoLogradouro { get; set; }
        public string EnderecoNumero { get; set; }
        public string Complemento { get; set; }
        public string CEP { get; set; }
        public string PoolMKCentral { get; set; }
        public string DataConnection { get; set; }
        public string TpAutenticacao { get; set; }
    }

    public class InserirAtendimentoRequest
    {
        public string cd_contrato { get; set; }
        public string cd_cliente { get; set; }
        public string cd_processo { get; set; }
        public string cd_classificacao_ate { get; set; }
        public string origem_contato { get; set; }
        public string cd_grupo_visualizadores { get; set; }
        public string info { get; set; }
        public string op_abertura { get; set; }
        public string conexaoId { get; set; }
    }

    public class InserirConexaoResponse : BaseCrmMkResponse
    {

    }

    public class InserirAtendimentoResponse : BaseCrmMkResponse
    {
        public string Cliente { get; set; }
        public int CodigoAtendimento { get; set; }
        public int CodigoCliente { get; set; }
        public string Protocolo { get; set; }
    }

    public class CriarOrdemServicoRequest
    {
        public string sys { get; set; }
        public string token { get; set; }
        public string CodigoCliente { get; set; }
        public string CodigoConexao { get; set; }
        public string CodigoContrato { get; set; }
        public string DescricaoProblema { get; set; }
        public string CodigoTipoOS { get; set; }
        public string CodigoTecnico { get; set; }        
        public string CodigoGrupoServico { get; set; }
        public string CodigoAtendimento { get; set; }
        public long Cidade { get; set; }
        public long Bairro { get; set; }
        public long Logradouro { get; set; }
        public string Numero { get; set; }
        public string dataConnection { get; set; }
    }

    public class Agenda
    {
        public string Cliente { get; set; }
        public string Descricao_os { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public int codCliente { get; set; }
        public int codOS { get; set; }
        public int codtecnico { get; set; }
        public string dataOS { get; set; }
        public string logradouro { get; set; }
        public string siglaestado { get; set; }
        public int status { get; set; }
    }

    public class InserirOrdemServicoResponse : BaseCrmMkResponse
    {
        public List<Agenda> Agendas { get; set; }
    }
}
