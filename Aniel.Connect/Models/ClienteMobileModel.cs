using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class ClienteMobileModel
    {
        public Cliente_ProspectoDadosGeraisViewModel clienteProspectoDados { get; }
        public VendaDadosGeraisViewModel vendasDados { get; }

        public ClienteMobileModel()
        {
            clienteProspectoDados = new Cliente_ProspectoDadosGeraisViewModel();
            vendasDados = new VendaDadosGeraisViewModel();
        }


        #region Dados do Cliente 
        public Int64 idCliente
        {
            get { return clienteProspectoDados.Id; }
            set
            {
                clienteProspectoDados.Id = value;
                vendasDados.Id_Cliente = value;
            }
        }

        public string cargoResponsavel
        {
            get { return clienteProspectoDados.Cargo_Responsavel; }
            set
            {
                clienteProspectoDados.Cargo_Responsavel = value;
                vendasDados.Cargo_Responsavel = value;
            }
        }

        public string celular
        {
            get { return clienteProspectoDados.Telefone_Celular; }
            set
            {
                clienteProspectoDados.Telefone_Celular = value;
                vendasDados.TelefoneCelular = value;
            }
        }

        public string codArea
        {
            get { return clienteProspectoDados.Cod_Area; }
            set
            {
                clienteProspectoDados.Cod_Area = value;
                vendasDados.Cod_Area = value;
            }
        }

        public string descCodArea
        {
            get { return clienteProspectoDados.Desc_Cod_Area; }
            set
            {
                clienteProspectoDados.Desc_Cod_Area = value;
                vendasDados.Desc_Cod_Area = value;
            }
        }

        public string codEscritorio
        {
            get { return clienteProspectoDados.Cod_Escritorio; }
            set
            {
                clienteProspectoDados.Cod_Escritorio = value;
                vendasDados.Cod_Escritorio = value;
            }
        }

        public string regime
        {
            get { return clienteProspectoDados.Regime; }
            set
            {
                clienteProspectoDados.Regime = value;
                vendasDados.Regime = value;
            }
        }

        public string descCodEscritorio
        {
            get { return clienteProspectoDados.Desc_Cod_Escritorio; }
            set
            {
                clienteProspectoDados.Desc_Cod_Escritorio = value;
                vendasDados.Desc_Cod_Escritorio = value;
            }
        }    
       

        public string codLocalidade
        {
            get { return clienteProspectoDados.Cod_Localidade; }
            set
            {
                clienteProspectoDados.Cod_Localidade = value;
                vendasDados.Cod_Localidade = value != null ? Int32.Parse(value) : 0;
            }
        } 
      

        public string descCodLocalidade
        {
            get { return clienteProspectoDados.Desc_Cod_Localidade; }
            set
            {
                clienteProspectoDados.Desc_Cod_Localidade = value;
                vendasDados.Desc_Cod_Localidade = value;
            }
        }

        public string cpfCnpj
        {
            get { return clienteProspectoDados.Cpf ?? clienteProspectoDados.Cnpj; }
            set
            {
                
                if (clienteProspectoDados.TipoPessoa.Value == Tipo_Pessoa.Fisica)
                    clienteProspectoDados.Cpf = value;
                else
                    clienteProspectoDados.Cnpj = value;

                vendasDados.CpfCnpj = value;
            }
        }

        public string cpfResponsavel
        {
            get { return clienteProspectoDados.CPFResponsavel; }
            set
            {
                clienteProspectoDados.CPFResponsavel = value;
                vendasDados.CPFResponsavel = value;
            }
        }

        public string dataEmissao
        {
            get { return clienteProspectoDados.DataEmissao; }
            set
            {
                clienteProspectoDados.DataEmissao = value;
                vendasDados.DataEmissao = value;
            }
        }

        public string dataNascimento
        {
            get { return clienteProspectoDados.DataNascimento; }
            set
            {
                clienteProspectoDados.DataNascimento = value;
                vendasDados.DataNascimento = value;
            }
        }

        public DateTime? dataAbertura
        {
            get { return clienteProspectoDados.Data_Abertura; }
            set
            {
                clienteProspectoDados.Data_Abertura = value;
                vendasDados.Data_Abertura = value;
            }
        }

        public string email
        {
            get { return clienteProspectoDados.Email; }
            set
            {
                clienteProspectoDados.Email = value;
                vendasDados.Email = value;
            }
        }

        public String emailAux
        {
            get { return clienteProspectoDados.Email_Aux; }
            set
            {
                clienteProspectoDados.Email_Aux = value;
            }
        }

        public EnderecoClienteViewModel enderecoPrincipal
        {
            get { return clienteProspectoDados.EnderecoPrincipal; }
            set
            {
                clienteProspectoDados.EnderecoPrincipal = value;
                vendasDados.EnderecoPrincipal = value;
            }
        }

        public EnderecoClienteViewModel enderecoCobranca
        {
            get { return clienteProspectoDados.EnderecoCobranca; }
            set
            {
                clienteProspectoDados.EnderecoCobranca = value;
                vendasDados.EnderecoCobranca = value;
            }
        }

        public EnderecoClienteViewModel enderecoInstalacao
        {
            get { return clienteProspectoDados.EnderecoInstalacao; }
            set
            {
                clienteProspectoDados.EnderecoInstalacao = value;
                vendasDados.EnderecoInstalacao = value;
            }
        }

        /*public Boolean isEndCobrancaIgualPrincipal
        {
            get { return clienteProspectoDados.End_Cobranca_Igual_Principal; }
            set
            {
                clienteProspectoDados.End_Cobranca_Igual_Principal = value;
                vendasDados.End_Cobranca_Igual_Principal = value;
            }
        }*/

        public Boolean isEndInstalacaoIgualPrincipal
        {
            get { return clienteProspectoDados.End_Instalacao_Igual_Principal; }
            set
            {
                clienteProspectoDados.End_Instalacao_Igual_Principal = value;
                vendasDados.End_Instalacao_Igual_Principal = value;
            }
        }

        public Estado_Civil? estadoCivil
        {
            get { return clienteProspectoDados.EstadoCivil; }
            set
            {
                clienteProspectoDados.EstadoCivil = value;
                vendasDados.EstadoCivil = value;
            }
        }

        public Int64? idModeloContrato
        {
            get { return clienteProspectoDados.Id_Modelo_Contrato; }
            set
            {
                clienteProspectoDados.Id_Modelo_Contrato = value;
                vendasDados.Id_Modelo_Contrato = value;
            }

        }

        public Int64? idMeioCaptacao
        {
            get { return clienteProspectoDados.Id_Meio_Captacao; }
            set
            {
                clienteProspectoDados.Id_Meio_Captacao = value;
                vendasDados.Id_Meio_Captacao = value;
            }
        }

        public Grau_Instrucao? instrucao
        {
            get { return clienteProspectoDados.Instrucao; }
            set { clienteProspectoDados.Instrucao = value; vendasDados.Instrucao = value; }
        }

        public String inscricaoEstadual
        {
            get { return clienteProspectoDados.Inscricao_Estadual; }
            set { clienteProspectoDados.Inscricao_Estadual = value; vendasDados.Inscricao_Estadual = value; }
        }

        public String inscricaoMunicipal
        {
            get { return clienteProspectoDados.Inscricao_Municipal; }
            set
            {
                clienteProspectoDados.Inscricao_Municipal = value;
                vendasDados.Inscricao_Municipal = value;
            }
        }

        public Int64? idQuemIndicou
        {
            get { return clienteProspectoDados.Id_Quem_Indicou; }
            set
            {
                clienteProspectoDados.Id_Quem_Indicou = value;
                vendasDados.Id_Quem_Indicou = value;
            }
        }

        public Int64? idCategoriaCliente
        {
            get { return clienteProspectoDados.Id_Categoria_Cliente; }
            set
            {
                clienteProspectoDados.Id_Categoria_Cliente = value;
                vendasDados.Id_Categoria_Cliente = value;
            }
        }       

        public String meioCaptacao
        {
            get { return clienteProspectoDados.Meio_Captacao; }
            set
            {
                clienteProspectoDados.Meio_Captacao = value;
                vendasDados.Meio_Captacao = value;
            }
        }

        public string mae
        {
            get { return clienteProspectoDados.Mae; }
            set
            {
                clienteProspectoDados.Mae = value;
                vendasDados.Mae = value;
            }
        }

        public String numeroContrato
        {
            get { return clienteProspectoDados.Numero_Contrato; }
            set
            {
                clienteProspectoDados.Numero_Contrato = value;
                vendasDados.Numero_Contrato = value;
            }
        }

        public String nomeCompleto
        {
            get { return clienteProspectoDados.NomeCompleto; }
            set
            {
                clienteProspectoDados.NomeCompleto = value;
                vendasDados.NomeCompleto = value;
            }
        }

        public string nacionalidade
        {
            get { return clienteProspectoDados.Nacionalidade; }
            set
            {
                clienteProspectoDados.Nacionalidade = value;
                vendasDados.Nacionalidade = value;
            }
        }

        public string nomeRecado
        {
            get { return clienteProspectoDados.NomeRecado; }
            set
            {
                clienteProspectoDados.NomeRecado = value;
                vendasDados.NomeRecado = value;
            }
        }

        public String nomeFantasia
        {
            get { return clienteProspectoDados.NomeFantasia; }
            set
            {
                clienteProspectoDados.NomeFantasia = value;
                vendasDados.NomeFantasia = value;
            }
        }

        public string orgao
        {
            get { return clienteProspectoDados.Orgao; }
            set
            {
                clienteProspectoDados.Orgao = value;
                vendasDados.Orgao = value;
            }
        }

        public string pai
        {
            get { return clienteProspectoDados.Pai; }
            set
            {
                clienteProspectoDados.Pai = value;
                vendasDados.Pai = value;
            }
        }

        public Porte? porte
        {
            get { return clienteProspectoDados.Porte; }
            set
            {
                clienteProspectoDados.Porte = value;
                vendasDados.Porte = value;
            }
        }

        public string profissao
        {
            get { return clienteProspectoDados.Profissao; }
            set
            {
                clienteProspectoDados.Profissao = value;
                vendasDados.Profissao = value;
            }
        }

        public string qualMeio
        {
            get { return clienteProspectoDados.Qual_Meio; }
            set { clienteProspectoDados.Qual_Meio = value; vendasDados.Qual_Meio = value; }
        }

        public string quemIndicou
        {
            get { return clienteProspectoDados.Quem_Indicou; }
            set { clienteProspectoDados.Quem_Indicou = value; vendasDados.Quem_Indicou = value; }
        }

        public string rg
        {
            get { return clienteProspectoDados.RG; }
            set
            {
                clienteProspectoDados.RG = value;
                vendasDados.RG = value;
            }
        }

        public String razaoSocial
        {
            get { return clienteProspectoDados.Razao_Social; }
            set
            {
                clienteProspectoDados.Razao_Social = value;
                vendasDados.Razao_Social = value;
            }
        }

        public String representante
        {
            get { return clienteProspectoDados.Representante; }
            set
            {
                clienteProspectoDados.Representante = value;
                vendasDados.Representante = value;
            }
        }

        public String ramoAtividade
        {
            get { return clienteProspectoDados.Ramo_Atividade; }
            set
            {
                clienteProspectoDados.Ramo_Atividade = value;
                vendasDados.Ramo_Atividade = value;
            }
        }

        public String site
        {
            get { return clienteProspectoDados.Site; }
            set
            {
                clienteProspectoDados.Site = value;
            }
        }

        public Boolean statusCliente
        {
            get { return clienteProspectoDados.Status; }
            set
            {
                clienteProspectoDados.Status = value;
                //vendasDados.Status = value;
            }
        }

        public Status_Prospecto? statusProspecto
        {
            get { return clienteProspectoDados.Status_Prospecto; }
            set
            {
                clienteProspectoDados.Status_Prospecto = value;
                //vendasDados.Status_Prospecto = value;
            }
        }

        public Sexo? sexo
        {
            get { return clienteProspectoDados.Sexo; }
            set
            {
                clienteProspectoDados.Sexo = value;
                vendasDados.Sexo = value;
            }
        }

        public String telefoneFixo2
        {
            get { return clienteProspectoDados.Telefone_Fixo2; }
            set
            {
                clienteProspectoDados.Telefone_Fixo2 = value;
            }
        }

        public Tipo_Cliente? tipoCliente
        {
            get { return clienteProspectoDados.TipoCliente; }
            set
            {
                clienteProspectoDados.TipoCliente = value;
                vendasDados.Tipo_Cliente = value;
            }
        }

        public Tipo_Pessoa? tipoPessoa
        {
            get { return clienteProspectoDados.TipoPessoa; }
            set
            {
                clienteProspectoDados.TipoPessoa = value;
                vendasDados.TipoPessoa = value;
            }
        }

        public string telefoneFixo
        {
            get { return clienteProspectoDados.TelefoneFixo; }
            set
            {
                clienteProspectoDados.TelefoneFixo = value;
                vendasDados.TelefoneFixo = value;
            }
        }

        public string telefoneRecado
        {
            get { return clienteProspectoDados.TelefoneRecado; }
            set
            {
                clienteProspectoDados.TelefoneRecado = value;
                vendasDados.TelefoneRecado = value;
            }
        }

        #endregion

        #region Dados da Venda
        public Int64 idVenda
        {
            get { return vendasDados.Id; }
            set { vendasDados.Id = value; }
        }

        public List<VendaDadosGeraisViewModel> vendas
        {
            get { return vendasDados.Vendas; }
            set { vendasDados.Vendas = value; }
        }

        public List<DescontoListaModel> descontos
        {
            get { return vendasDados.Descontos; }
            set { vendasDados.Descontos = value; }
        }

        public virtual List<PlanoProdutoListModel> planos
        {
            get { return vendasDados.Planos; }
            set { vendasDados.Planos = value; }
        }

        public virtual Forma_Pagamento_ClienteModels formasPagamento
        {
            get { return vendasDados.FormaPagamentoVenda; }
            set { vendasDados.FormaPagamentoVenda = value; }
        }

        public Int64 id_Forma_Pagamento_Cliente_Preferida
        {
            get { return vendasDados.Id_Forma_Pagamento_Cliente_Preferida; }
            set
            {
                vendasDados.Id_Forma_Pagamento_Cliente_Preferida = value;
            }
        }

        public String tempoUltimoContato
        {
            get { return vendasDados.Tempo_Ultimo_Contato; }
            set { vendasDados.Tempo_Ultimo_Contato = value; }
        }

        public String localidade
        {
            get { return vendasDados.Localidade; }
            set { vendasDados.Localidade = value; }
        }

        public String dataVenda
        {
            get { return vendasDados.DataVenda; }
            set { vendasDados.DataVenda = value; }
        }

        public String dataCriacao
        {
            get { return vendasDados.DataCriacao; }
            set { vendasDados.DataCriacao = value; }
        }

        public String dataEncerramento
        {
            get { return vendasDados.DataEncerramento; }
            set { vendasDados.DataEncerramento = value; }
        }

        public DateTime? ultimoContato
        {
            get { return vendasDados.Ultimo_Contato; }
            set { vendasDados.Ultimo_Contato = value; }
        }

        public Int64? idSupervisor
        {
            get { return vendasDados.Id_Supervisor; }
            set { vendasDados.Id_Supervisor = value; }
        }

        public Int64? idVendedor
        {
            get { return vendasDados.Id_Vendedor; }
            set { vendasDados.Id_Vendedor = value; }
        }

        public String nomeSupervisor
        {
            get { return vendasDados.Nome_Supervisor; }
            set { vendasDados.Nome_Supervisor = value; }
        }

        public String nomeVendedor
        {
            get { return vendasDados.Nome_Vendedor; }
            set { vendasDados.Nome_Vendedor = value; }
        }

        public Status_Venda statusVenda
        {
            get { return vendasDados.Status; }
            set { vendasDados.Status = value; }
        }

        public String identificacaoCliente
        {
            get { return vendasDados.Identificacao_Cliente; }
            set { vendasDados.Identificacao_Cliente = value; }
        }

        public string valorFinal
        {
            get { return vendasDados.Valor_Final; }
            set { vendasDados.Valor_Final = value; }
        }

        public string valorInstalacaoFinal
        {
            get { return vendasDados.Valor_Instalacao_Final; }
            set { vendasDados.Valor_Instalacao_Final = value; }
        }

        public Boolean testeGratis
        {
            get { return vendasDados.TesteGratis; }
            set { vendasDados.TesteGratis = value; }
        }

        public Boolean periodoCarencia
        {
            get { return vendasDados.Periodo_Carencia; }
            set { vendasDados.Periodo_Carencia = value; }
        }

        public Int64? qtdeDias
        {
            get { return vendasDados.Qtde_Dias; }
            set { vendasDados.Qtde_Dias = value; }
        }

        public Int64? mesesCarencia
        {
            get { return vendasDados.Meses_Carencia; }
            set { vendasDados.Meses_Carencia = value; }
        }

        public Int64? numeroParcelasInstalacao
        {
            get { return vendasDados.Numero_Parcelas_Instalacao; }
            set { vendasDados.Numero_Parcelas_Instalacao = value; }
        }

        public String login
        {
            get { return vendasDados.Login; }
            set { vendasDados.Login = value; }
        }

        public String senha
        {
            get { return vendasDados.Senha; }
            set { vendasDados.Senha = value; }
        }

        public String confirmarSenha
        {
            get { return vendasDados.Confirmar_Senha; }
            set { vendasDados.Confirmar_Senha = value; }
        }

        public Int64? idDesconto
        {
            get { return vendasDados.Id_Desconto; }
            set { vendasDados.Id_Desconto = value; }
        }

        /*public Int64? idFormaPagamento
        {
            get { return vendasDados.Id_Forma_Pagamento; }
            set { vendasDados.Id_Forma_Pagamento = value; }
        }

        public Int64? idVencimento
        {
            get { return vendasDados.Id_Vencimento; }
            set { vendasDados.Id_Vencimento = value; }
        }

        public Int64? idBanco
        {
            get { return vendasDados.Id_Banco; }
            set { vendasDados.Id_Banco = value; }
        }

        public String descBanco
        {
            get { return vendasDados.Desc_Banco; }
            set { vendasDados.Desc_Banco = value; }
        }

        public String contaCorrente
        {
            get { return vendasDados.Conta_Corrente; }
            set { vendasDados.Conta_Corrente = value; }
        }

        public String digitoDaConta
        {
            get { return vendasDados.Digito; }
            set { vendasDados.Digito = value; }
        }

        public String agencia
        {
            get { return vendasDados.Agencia; }
            set { vendasDados.Agencia = value; }
        }*/

        public Int64? idPlano
        {
            get { return vendasDados.Id_Plano; }
            set { vendasDados.Id_Plano = value; }
        }  
        
        public Localidade_Anatel? Localidade_Anatel
        {
            get { return vendasDados.Localidade_Anatel; }
            set { vendasDados.Localidade_Anatel = value; }
        }

        public Int64? idCampanha
        {
            get { return vendasDados.Id_Campanha; }
            set { vendasDados.Id_Campanha = value; }
        }

        /*public Cliente_FormaEnvio? forma_envio
        {
            get { return vendasDados.Forma_Envio; }
            set { vendasDados.Forma_Envio = value ?? Cliente_FormaEnvio.Correio; }
        }*/

        public Int64? Id_Caixa_Mais_Proxima
        {
            get { return vendasDados.Id_Caixa_Mais_Proxima; }
            set { vendasDados.Id_Caixa_Mais_Proxima = value; }
        }

        public Boolean Consulta_Viabilidade_Realizada
        {
            get { return vendasDados.Consulta_Viabilidade_Realizada; }
            set { vendasDados.Consulta_Viabilidade_Realizada = value; }
        }

        #endregion
    }
}