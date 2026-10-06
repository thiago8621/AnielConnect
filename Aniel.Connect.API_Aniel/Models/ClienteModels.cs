using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Newtonsoft.Json;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    public class ClienteListaBuscaAtendimento : ObjBusca<Int64>
    {
        public virtual String CPF_CNPJ_Cliente { get; set; }

        public virtual String Categoria_Cliente { get; set; }


        public virtual String Nome_Fantasia { get; set; }

        public virtual String Telefone_Fixo { get; set; }
        public virtual String Telefone_Celular { get; set; }
        public virtual String Telefone_Recado { get; set; }
        public virtual String Email_Principal { get; set; }

        public virtual String Forma_Pagamento_Utilizada { get; set; }

        public virtual String Endereco_Instalacao { get; set; }
        public virtual String Login_Radius { get; set; }
        public virtual String Valor_Final_Total { get; set; }
        public virtual String Id_Endereco_Cliente { get; set; }

        public virtual String Endereco_Cliente { get; set; }


        public virtual String Data_Cadastro { get; set; }
        public virtual String Data_Criacao_Cliente { get; set; }

        public virtual String Numero_Contrato { get; set; }

        public virtual Int32 Tipo_Cliente { get; set; }
        public virtual List<PlanoProdutoComboAntendimentoViewModel> PlanosProdutos { get; set; }

        public virtual Int32 Cliente_Ha { get; set; }
    }

    public class ClienteObjCombo : IObjComboResult<Int64>
    {
        public Int64 Id { get; set; }
        public String Text { get; set; }
        public String Cpf { get; set; }
    }

    public class ClienteListViewModel
    {
        /// <summary> Identificador </summary>
        public Int64 Id { get; set; }
        ///// <summary>
        ///// Se existe alguma pendência
        ///// </summary>
        //public Boolean Pendencia { get; set; }
        /// <summary> Status do Cliente </summary>

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status { get; set; }

        ///// <summary>
        ///// Tipo de Pessoa
        ///// </summary>
        //[Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        //[DataType(DataType.Text)]
        //public string TipoPessoa { get; set; }

        ///// <summary>
        ///// Estado Civil
        ///// </summary>
        //[Display(Name = "ESTADO_CIVIL", ResourceType = typeof(Resources.RsCadastros))]
        //[DataType(DataType.Text)]
        //public string Estado_Civil { get; set; }

        ///// <summary>
        ///// Sexo
        ///// </summary>

        //[Display(Name = "SEXO", ResourceType = typeof(Resources.RsCadastros))]
        //[DataType(DataType.Text)]
        //public string Sexo { get; set; }

        ///// <summary>
        ///// Grau de Instrução
        ///// </summary>
        //[Display(Name = "GRAU_INSTRUCAO", ResourceType = typeof(Resources.RsCadastros))]
        //[DataType(DataType.Text)]
        //public string Grau_Instrucao { get; set; }

        /// <summary> Nome Fantasia </summary>
        [Display(Name = "NOMEFANTASIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string NomeFantasia { get; set; }

        /// <summary> CPF/CNPJ </summary>
        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string CpfCnpj { get; set; }

        ///// <summary>
        ///// Categoria de Cliente
        ///// </summary>
        //[Display(Name = "CATEGORIA", ResourceType = typeof(Resources.RsCadastros))]
        //[DataType(DataType.Text)]
        //public string Categoria { get; set; }

        ///// <summary>
        ///// Login
        ///// </summary>
        //[Display(Name = "LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        //[DataType(DataType.Text)]
        //public string Login { get; set; }

        /// <summary> Estado </summary>
        [Display(Name = "ESTADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Estado { get; set; }

        /// <summary> Cidade </summary>
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Cidade { get; set; }

        /// <summary> Bairro </summary>
        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Bairro { get; set; }


        /// <summary> Telefone Principal </summary>
        [Display(Name = "TELEFONEPRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefonePrincipal { get; set; } 
        
       
        public string Data_nascimento { get; set; }
         
       
        public string TelefoneCelular { get; set; }
         
      
        public string TelefoneRecado { get; set; }
        public string Nome_Recado { get; set; }


        public string Email { get; set; }    
       
        public string Logradouro { get; set; }
        public string Cep { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Ponto_Ref { get; set; }
        public string Latitude { get; set; }
        public string Pais { get; set; }
        public string Longitude { get; set; }
        public string Tipo_Imovel { get; set; }
        public long? Cod_Cliente_Aniel { get; set; }

    }

    public class Cliente_ProvisionamentoListViewModel
    {
        public Int64 Id { get; set; }

        public String Nome { get; set; }

        public String Login { get; set; }

        public String PlanoProduto { get; set; }

        public String Pon { get; set; }
    }

    public class ClientePesquisaListViewModel
    {
        public Int64 Id { get; set; }

        public Boolean Pendencia { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status { get; set; }

        [Display(Name = "TIPO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Tipo_Cliente { get; set; }

        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TipoPessoa { get; set; }

        [Display(Name = "ESTADO_CIVIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Estado_Civil { get; set; }

        [Display(Name = "SEXO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Sexo { get; set; }

        [Display(Name = "GRAU_INSTRUCAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Grau_Instrucao { get; set; }

        [Display(Name = "NOMEFANTASIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string NomeFantasia { get; set; }

        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string CpfCnpj { get; set; }

        [Display(Name = "CATEGORIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Categoria { get; set; }

        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Login { get; set; }

        [Display(Name = "ESTADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Estado { get; set; }

        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Cidade { get; set; }

        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Bairro { get; set; }

        [Display(Name = "TELEFONEPRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefonePrincipal { get; set; }

    }
    public class ClienteOzMap
    {
        public virtual string DATAHORACRIACAO { get; set; }
        
        public virtual Int64 Id { get; set; }
        public bool Alterado { get; set; }
        public virtual string id_Integrador { get; set; }
        public virtual String NomeFantasia { get; set; }
    }

    public class ClienteGetList
    {
        public virtual Int64 Id { get; set; }
        public virtual String NomeFantasia { get; set; }
    }
    public class ClienteDadosGeraisViewModel : IViewModel<Int64>
    {   public ClienteDadosGeraisViewModel()
        {
            EnderecoPrincipal = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Principal };
            EnderecoInstalacao = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Instalacao };
            EnderecoCobranca = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Cobranca };
        }

        public Int64 Id { get; set; }
            
        public Int64 Id_Cliente { get; set; }
        public bool Alterado { get; set; }

        public Status_Prospecto Status_Prospecto { get; set; }
        public Int64? Id_Organizacao { get; set; }

        #region Fisico

        [Display(Name = "COD_AREA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cod_Area { get; set; }

        [Display(Name = "COD_ESCRITORIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cod_Escritorio { get; set; }

        [Display(Name = "COD_LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cod_Localidade { get; set; }

        [Display(Name = "COD_AREA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Desc_Cod_Area { get; set; }

        [Display(Name = "COD_ESCRITORIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Desc_Cod_Escritorio { get; set; }

        [Display(Name = "COD_LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Desc_Cod_Localidade { get; set; }

        public Status_Venda? Status_Venda { get; set; }

        public Tipo_Cliente? TipoCliente { get; set; }
        public Tipo_Pessoa? TipoPessoa { get; set; }

        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String CpfCnpj { get; set; }

        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Meio_Captacao { get; set; }

        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Meio_Captacao { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public Boolean Status { get; set; }

        [Display(Name = "NOMECOMPLETO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String NomeCompleto { get; set; }


        //public Boolean End_Cobranca_Igual_Principal { get; set; }
        public Boolean End_Instalacao_Igual_Principal { get; set; }

        public String End_Cobranca_Igual_Principal_Str { get; set; }
        public String End_Instalacao_Igual_Principal_Str { get; set; }

        public EnderecoClienteViewModel EnderecoPrincipal { get; set; }

        public Int64? Id_Pais { get; set; }

        public EnderecoClienteViewModel EnderecoInstalacao { get; set; }

        public EnderecoClienteViewModel EnderecoCobranca { get; set; }

        [Display(Name = "DATANASCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String DataNascimento { get; set; }


        [Display(Name = "NACIONALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(40, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Nacionalidade { get; set; }

        [Display(Name = "SEXO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Sexo? Sexo { get; set; }

        [Display(Name = "ESTADOCIVIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Estado_Civil? EstadoCivil { get; set; }

        [Display(Name = "RG", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string RG { get; set; }

        [Display(Name = "DATAEMISSAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string DataEmissao { get; set; }

        [Display(Name = "ORGAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Orgao { get; set; }

        [Display(Name = "PAI", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Pai { get; set; }

        [Display(Name = "MAE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Mae { get; set; }

        [Display(Name = "TELEFONEFIXO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefoneFixo { get; set; }

        [Display(Name = "TELEFONECELULAR", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Telefone_Celular { get; set; }

        [Display(Name = "TELEFONERECADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefoneRecado { get; set; }

        [Display(Name = "NOMERECADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string NomeRecado { get; set; }

        [Display(Name = "INSTRUCAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Grau_Instrucao? Instrucao { get; set; }

        [Display(Name = "PROFISSAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(40, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Profissao { get; set; }

        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(70, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Email { get; set; }

        //public String Dia_Vencimento { get; set; }

        #endregion
        
        public  String id_Integrador { get; set; }
     
        #region Juridico
        public String NomeFantasia { get; set; }
        public DateTime? Data_Abertura { get; set; }
        public String Razao_Social { get; set; }

        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Inscricao_Estadual { get; set; }

        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Inscricao_Municipal { get; set; }
        public Porte? Porte { get; set; }
        public String Representante { get; set; }

        [MaxLength(30, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Cargo_Responsavel { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Ramo_Atividade { get; set; }
        public String CPFResponsavel { get; set; }

        #endregion

        #region PLANOS E PRODUTOS

        private List<PlanoProdutoListModel> PlanosLista;
        public virtual List<PlanoProdutoListModel> Planos
        {
            get
            {
                if (PlanosLista == null)
                    PlanosLista = new List<PlanoProdutoListModel>();
                return PlanosLista;
            }
            set
            {
                PlanosLista = value;
            }
        }

        [Display(Name = "QUAL_MEIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Qual_Meio { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(60, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Quem_Indicou { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Id_Quem_Indicou { get; set; }

        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Categoria_Cliente { get; set; }

        [Display(Name = "VALOR_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Double? Valor_Final { get; set; }

        [Display(Name = "VALOR_INSTALACAO_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Double? Valor_Instalacao_Final { get; set; }


        [Display(Name = "TESTE_GRATIS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Boolean TesteGratis { get; set; }

        [Display(Name = "QTDE_DIAS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Qtde_Dias { get; set; }

        [Display(Name = "PERIODO_CARENCIA_MESES", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Boolean Periodo_Carencia { get; set; }

        [Display(Name = "MESES_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Meses_Carencia { get; set; }

        [Display(Name = "MESES_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Numero_Parcelas_Instalacao { get; set; }

        [Display(Name = "ENDERECO_INSTALACAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Boolean? Endereco_Instalacao { get; set; }

        #endregion

        #region MODAL RADIUS

        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Login { get; set; }

        [Display(Name = "SENHA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Senha { get; set; }

        [Display(Name = "CONFIRMARSENHA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Confirmar_Senha { get; set; }

        [Display(Name = "NAS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Nas { get; set; }

        public Int64? Id_Campanha { get; set; }

        #endregion

        #region MODAL MOTIVO CANCELAMENTO

        public Int64? Id_Motivo_Cancelamento { get; set; }

        public Int64? Id_PlanoProduto { get; set; }

        #endregion 

        #region  FINCANCEIRO

        public Int64? Id_Desconto { get; set; }

        private List<DescontoListaModel> DescontosLista;

        public virtual List<DescontoListaModel> Descontos

        {
            get
            {
                if (DescontosLista == null)
                    DescontosLista = new List<DescontoListaModel>();
                return DescontosLista;
            }
            set
            {
                DescontosLista = value;
            }
        }

        //private List<Cliente_Desconto> _descontos;
        //public virtual List<Cliente_Desconto> Descontos_Validacao
        //{
        //    get
        //    {
        //        if (_descontos == null)
        //            _descontos = new List<Cliente_Desconto>();
        //        return _descontos;
        //    }
        //    set
        //    {
        //        _descontos = value;
        //    }
        //}

        //public Cliente_FormaEnvio? Forma_Envio { get; set; }

        [Display(Name = "MODELO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Modelo_Contrato { get; set; }

        [Display(Name = "MODELO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public String Numero_Contrato { get; set; }

        [Display(Name = "IDENTIFICACAO_CLIENTE", ResourceType = typeof(Resources.RsCadastros)),
        MaxLength(9, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Identificacao_Cliente { get; set; }

        //[Display(Name = "FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsFinanceiro))]
        //public Int64? Id_Forma_Pagamento_Cobranca { get; set; }

        //public Int64? Id_Forma_Pagamento_PlanoProduto { get; set; }

        //public Forma_Pagamento Forma_Pagamento { get; set; } //Remover
         
        private List<Forma_Pagamento_ClienteModels> Formas_PagamentoLista;

        public virtual List<Forma_Pagamento_ClienteModels> Formas_Pagamento { 
            get 
            {
                if (Formas_PagamentoLista == null)
                    Formas_PagamentoLista = new List<Forma_Pagamento_ClienteModels>();
                return Formas_PagamentoLista;
            } 
            set
            {
                Formas_PagamentoLista = value;
            }
        }

        /*public virtual List<Forma_PagamentoListaModel> Formas_Pagamento
        {
            get
            {
                if (Formas_Pagamento == null)
                    Formas_Pagamento = new List<Forma_PagamentoListaModel>();
                return Formas_Pagamento;
            }
            set
            {
                Formas_Pagamento = value;
            }
        }*/

        /*[Display(Name = "VENCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Vencimento { get; set; }

        [Display(Name = "BANCO", ResourceType = typeof(Resources.RsCadastros))]
        public String Desc_Banco { get; set; }

        [Display(Name = "BANCO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Banco { get; set; }

        [Display(Name = "CONTA_CORRENTE", ResourceType = typeof(Resources.RsCadastros)),
        MaxLength(15, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Conta_Corrente { get; set; }

        [Display(Name = "AGENCIA", ResourceType = typeof(Resources.RsCadastros)),
        MaxLength(5, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Agencia { get; set; }

        [Display(Name = "DIGITO", ResourceType = typeof(Resources.RsCadastros)),
        MaxLength(1, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Digito { get; set; }*/






        #endregion

        /// <summary> Utilizado para preencimento da grid de planos </summary>
        public Int64? Id_Plano { get; set; }
    }
    #region [ClienteContratoViewModel]
    public class ClienteContratoViewModel
    {
        public long Id { get; set; }
        public long Id_Cliente { get; set; }
        public string Numero_Contrato { get; set; }
        public string Descricao { get; set; }
        public bool Contrato_Automatico { get; set; }
        public bool Tem_Carencia { get; set; }
        public long? Periodo_Carencia { get; set; }
        public double? Multa_Carencia { get; set; }
        public bool Tem_Reajuste { get; set; }
        public bool Renovar_Automaticamente { get; set; }
        public Renova_FidelidadeEnum? Renovar_Carencia { get; set; }
        public Tipo_Reajuste_Contratual? Tipo_Reajuste { get; set; }
        public Frequencia_Reajuste? Frequencia_Reajuste { get; set; }
        public bool Cancela_Plano_Carencia { get; set; }
        public bool Tem_Pro_Rata { get; set; }
        public bool Multa_Fidelidade { get; set; }
        public double Percentual_Reajuste { get; set; }
        public double Valor_Reajuste { get; set; }
        public bool Impedir_Bloqueio_Automatico { get; set; }
        public bool Impedir_Bloqueio_Segunda { get; set; }
        public bool Impedir_Bloqueio_Terca { get; set; }
        public bool Impedir_Bloqueio_Quarta { get; set; }
        public bool Impedir_Bloqueio_Quinta { get; set; }
        public bool Impedir_Bloqueio_Sexta { get; set; }
        public bool Impedir_Bloqueio_Sabado { get; set; }
        public bool Impedir_Bloqueio_Domingo { get; set; }
        public bool Impedir_Bloqueio_Feriado { get; set; }
        public bool Teste_Gratis { get; set; }
        public short? Dias_Teste { get; set; }
        public bool Utiliza_Desbloqueio_Conf { get; set; }
        public Frequencia_Desbloqueio? Frequencia_Desbloqueio { get; set; }
        public short Qtd_Dias_Desbloqueados { get; set; }
        public Status_Contrato? Status_Contrato { get; set; }
    }

    #endregion

    #region [ClientePlanoProdutoViewModel]
    public class ClientePlanoProdutoViewModel
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public double? Valor_Real { get; set; }
        public double? Valor_Instalacao_Real { get; set; }
        public double? Valor_Instalacao_Final { get; set; }
        public double? Valor_Final_Total { get; set; }
        public double? Valor_Final { get; set; }
        public long? Parcelas_Instalacao { get; set; }
        public long? Dias_Teste_Gratis { get; set; }
        public bool Periodo_Carencia { get; set; }
        public long? Meses_Carencia { get; set; }
        public bool End_Instalacao_Principal { get; set; }
        public string Data_Habilitado { get; set; }
        public string Data_Desabilitado { get; set; }
        public string Cod_Escritorio { get; set; }
        public string Desc_Escritorio { get; set; }
        public string Cod_Area { get; set; }
        public string Desc_Area { get; set; }
        public int? Cod_Localidade { get; set; }
        public string Localidade{ get; set; }
        public string Login_Radius{ get; set; }
        public string Senha_Radius { get; set; }
        public virtual Int32? Quantidade_Produto_Variavel { get; set; }
        public virtual Double? Valor_Total_Plano_Variavel { get; set; }
        public string Numero_Contrato { get; set; }
        public Status_PlanoProduto? Status_Plano_Produto { get; set; }
        public long Id_Cliente { get; set; }
        public long Id_Plano { get; set; }
        public long? Id_Produto { get; set; }
        public long Id_Endereco_Instalacao { get; set; }
        public long? Id_Campanha { get; set; }
        public long? Id_Motivo_Cancelamento { get; set; }
        public DateTime? Data_Desbloqueio_Confianca { get; set; }
        public bool Velocidade_Reduzida { get; set; }
        public bool ReducaoVelocidade { get; set; }
        public short QtdDiasAtraso { get; set; }
        public string VelocidadeDownloadReduzida { get; set; }
        public string VelocidadeUploadReduzida { get; set; }
        public bool ImpedirBloqueio_Automatico { get; set; }
        public bool ImpedirBloqueio_Sexta { get; set; }
        public bool ImpedirBloqueio_Sabado { get; set; }
        public bool ImpedirBloqueio_Domingo { get; set; }
        public bool ImpedirBloqueio_Feriados { get; set; }
        public bool UtilizaDesbloqueioConfianca { get; set; }
        public bool UtilizaBloqueioDebito { get; set; }
        public short QtdDiasbloqueados { get; set; }
        public short QtdDiasdesbloqueados { get; set; }
        public Frequencia_Desbloqueio? FrequenciaDesbloqueio { get; set; }
        public bool Telefonia { get; set; }
        public Localidade_Anatel? Localidade_Anatel { get; set; }
        public double? Multa_Carencia { get; set; }
        public bool Cancel_Contrato_Carencia { get; set; }
        public Frequencia_Reajuste? Frequencia_Reajuste { get; set; }
        public Renova_FidelidadeEnum? Renovar_Carencia { get; set; }
    }
    #endregion

    #region [ClienteViewModel]
    public class ClienteViewModel
    {
        public string NomeFantasia { get; set; }
        public string Data_Nascimento { get; set; }
        public string CPF_CNPJ { get; set; }
        public string Nacionalidade { get; set; }
        public string Profissao { get; set; }
        public Tipo_Cliente? Tipo_Cliente { get; set; }
        public Status_Cliente? Status_Cliente { get; set; }
        public Tipo_Pessoa? Tipo_Pessoa { get; set; }
        public Estado_Civil? Estado_Civil { get; set; }
        public Grau_Instrucao? Grau_Instrucao { get; set; }
        public string Outro_Meio { get; set; }
        public Porte? Porte { get; set; }
        public Status_Prospecto? Status_Prospecto { get; set; }

        [JsonProperty("ClienteContrato", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClienteContratoViewModel> ClienteContrato { get; set; }

        [JsonProperty("ClientePlanoProduto", NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientePlanoProdutoViewModel> ClientePlanoProduto { get; set; }
    }
    #endregion

    public class AlterarEnderecoClienteViewModel
    {
        public EnderecoClienteViewModel EnderecoInstalacao { get; set; }
        public Int64? Id_Atendimento { get; set; }
    }

    public class AlterarDetalhes_Plano_ProdutoViewModel
    {
        [Display(Name = "VALOR_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Double? Valor_Final { get; set; }

        [Display(Name = "VALOR_INSTALACAO_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Double? Valor_Instalacao_Final { get; set; }


        [Display(Name = "TESTE_GRATIS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Boolean TesteGratis { get; set; }

        [Display(Name = "QTDE_DIAS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Qtde_Dias { get; set; }

        [Display(Name = "PERIODO_CARENCIA_MESES", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Boolean Periodo_Carencia { get; set; }

        [Display(Name = "MESES_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Meses_Carencia { get; set; }
    }


    public class AlterarLoginRadiusViewModel : IViewModel<Int64>
    {
        public long Id { get; set; }
        
        public long IdPlanoProduto { get; set; }

        public string Usuario { get; set; }

        public string Login { get; set; }
        public string Senha { get; set; }

        public long IdConcentradora { get; set; }
    }

    public class AlterarPontoAcesso : IViewModel<Int64>

    {
        public Int64 Id { get; set; }
        public Int64? Id_Ponto_Acesso { get; set; }
        public Int64? Id_Pool_Ip { get; set; }
    }

    public class Cliente_ApiRequest : ApiRequest
    {
        #region Request
        ///// <summary>
        ///// Tipo de Pessoa
        ///// </summary>
        //public String Tipo_Pessoa { get; set; }
        ///// <summary>
        ///// Status do Cliente
        ///// </summary>
        //public String Status { get; set; }
        ///// <summary>
        ///// Estado do Cliente
        ///// </summary>
        //public String Estado { get; set; }
        ///// <summary>
        ///// Cidade do Cliente
        ///// </summary>
        //public String Cidade { get; set; }
        ///// <summary>
        ///// Bairro do Cliente
        ///// </summary>
        //public String Bairro { get; set; }
        ///// <summary>
        ///// Tipo de Imóvel do Cliente
        ///// </summary>
        //public String Tipo_Imovel { get; set; }
        ///// <summary>
        ///// Login Radius do Cliente
        ///// </summary>
        //public String Login_Radius { get; set; }
        ///// <summary>
        ///// Categoria do Cliente
        ///// </summary>
        //public String Categoria_Cliente { get; set; }
        ///// <summary>
        ///// Sexo do Cliente
        ///// </summary>
        //public String Sexo { get; set; }
        ///// <summary>
        ///// Estado Civil do Cliente
        ///// </summary>
        //public String Estado_Civil { get; set; }
        ///// <summary>
        ///// Grau de Instrução
        ///// </summary>
        //public String Grau_Instrucao { get; set; }
        ///// <summary>
        ///// Identificador do Plano/Produto
        ///// </summary>
        //public String Id_Plano_Produto { get; set; }
        ///// <summary>
        ///// Mês de Aniversário
        ///// </summary>
        //public String Mes_Aniversario { get; set; } 
        #endregion

    }

    public class FindByIdApiRequest : IApiValidate
    {
        /// <summary> Identificador do Objeto </summary>
        [Required]
        public Int64 id { get; set; }
        /// <summary> Usuário da API </summary>
        [Required]
        public string user { get; set; }
        /// <summary> Senha do Usuário </summary>
        [Required]
        public string password { get; set; }
        /// <summary> Token da API </summary>
        [Required]
        public string token { get; set; }
    }

    public class FindByPalavraChaveApiRequest : IApiValidate
    {
        /// <summary> Identificador do Objeto </summary>
        [Required]
        public string palavraChave { get; set; }
        /// <summary> Usuário da API </summary>
        [Required]
        public string user { get; set; }
        /// <summary> Senha do Usuário </summary>
        [Required]
        public string password { get; set; }
        /// <summary> Token da API </summary>
        [Required]
        public string token { get; set; }
    }

    public class Cliente_ProvisionamentoDataTableRequest : DataTableRequest
    {

    }
    public class ClientePesquisa_DataTableRequest : DataTableRequest
    {
        public String Ids_Pesquisa_Satisfacao { get; set; }

        public List<String> Descricoes_Pesquisa_Satisfacao { get; set; }

        public String Tipo_Cliente { get; set; }

        public String Tipo_Pessoa { get; set; }

        public String Status { get; set; }

        public String Categoria_Cliente { get; set; }

        public String Estado_Civil { get; set; }

        public String Estado { get; set; }

        public String Cidade { get; set; }

        public String Bairro { get; set; }


        public String Id_Plano { get; set; }
        public String Id_Produto { get; set; }

    }


    /* ANEXO CLIENTE */
    public class ClienteAnexoModelView : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        [Display(Name = "TITULO", ResourceType = typeof(Resources.RsCadastros))]
        public string Titulo { get; set; }

        [Display(Name = "ARQUIVO", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public string Nome_Arquivo { get; set; }
        public string caminho_Arquivo { get; set; }

        public string Extensao_Arquivo { get; set; }
        public string FotoWebCam { get; set; }
        public Int64 Id_Cliente { get; set; }
        public Boolean Foto_Perfil { get; set; }
    }

    public class ClienteAnexo_DataTableRequest : DataTableRequest
    {
        public Int64 Id_Cliente { get; set; }
    }

    public class Cliente_AnexoRequest
    {
        public String Nome_Arquivo { get; set; }
    }

    public class Cliente_ListaLoginRadius
    {
        public String Login { get; set; }
        public String Plano { get; set; }
    }

    public class Planos_HabilitadosModel : Cliente_ListaLoginRadius
    {
        public String Status { get; set; }
        public String Valor { get; set; }

    }

    public class Cliente_ValidacaoModel
    {
        public Int64 Id { get; set; }
        public Int32 TipoPessoa { get; set; }

        public String CpfCnpj { get; set; }

        public Boolean Status { get; set; }
    }

    public class Cliente_DadosPessoais
    {
        public Int64 Id { get; set; }
        public String Nome { get; set; }
        public String Email { get; set; }
        public String Telefone { get; set; }
        public String Plano { get; set; }
        public Int64 FormaEnvio { get; set; }
        public Int64? DiaVencimento { get; set; }
        public virtual List<Int64> OpcoesVencimento { get; set; }
    }

    public class Cliente_TreeViewFibra
    {
        public Int64 Id { get; set; }
        public Int64 Id_ClientePlanoProduto { get; set; }
        public Int64 Id_Cliente { get; set; }
        public String Equipamento { get; set; }
        public String Cliente { get; set; }
        public String Login { get; set; }
        public String Plano { get; set; }
    }

    public class GetTreeViewFibraExportModels
    {
        public Int64 Id { get; set; }
        public Int64 Id_ClientePlanoProduto { get; set; }
        public Int64 Id_Cliente { get; set; }

        public String Empresa { get; set; }
        public String Filial { get; set; }
        public String OLT { get; set; }
        public String SLOT { get; set; }
        public String PON { get; set; }
        public String Equipamento { get; set; }
        public String Cliente { get; set; }
        public String Plano { get; set; }
        public String Login { get; set; }

    }

    public class Cliente_Plano_ProdutoViewModel
    {
        public Int64 Id { get; set; }

        //public virtual Endereco_Cliente Endereco_Instalacao { get; set; }

        public virtual String VelocidadeDownlaod { get; set; }
        public virtual String VelocidadeNominal { get; set; }

        public virtual String Nas { get; set; }
        public virtual String Login_Radius { get; set; }
        public virtual String Senha_Radius { get; set; }
        public virtual DateTime? Data_Habilitado { get; set; }
        public virtual DateTime? Data_Desabilitado { get; set; }

        public virtual Modelo_Contrato Modelo_Contrato { get; set; }
        public virtual String Numero_Contrato { get; set; }

        public virtual Venda Venda { get; set; }
        public virtual Campanha Campanha { get; set; }
        public virtual Plano Plano { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Status_PlanoProduto? Status { get; set; }


        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual Endereco_Cliente Endereco_Instalacao { get; set; }
        public virtual Boolean End_Instalacao_Igual_Principal { get; set; }

        public virtual Double? Valor_Final_Total { get; set; }
        public virtual Double? Valor_Real { get; set; }
        public virtual Double? Valor_Final { get; set; }
        public virtual Double? Valor_Instalacao_Real { get; set; }
        public virtual Double? Valor_Instalacao_Final { get; set; }

        public virtual Int64? Parcelas_Instalacao { get; set; }
        public virtual Int64? Dias_Teste_Gratis { get; set; }
        public virtual Boolean Periodo_Carencia { get; set; }
        public virtual Int64? Meses_Carencia { get; set; }

        public virtual Boolean? Atualiza_Ola { get; set; }
    }

    public class ClienteVinculoONUCadastro : IViewModel<Int64>
    {
        public Int64 Id { get; set; }
        public String Cliente { get; set; }
        public Int64? Id_OLT { get; set; }
        public Int64? Id_Slot { get; set; }
        public Int64? Id_PON { get; set; }
        public Int64? Id_ONU { get; set; }

    }
    public class StatusOnuInformacoesBasicas
    {
        public virtual String Status { get; set; }
        public virtual String Onu_Ip { get; set; }
        public virtual Int64 Id_Onu { get; set; }
        public virtual String Auth { get; set; }
        public virtual String Auth_Info { get; set; }
        public virtual String LastOffTime { get; set; }
        public virtual String Onu_Temperature { get; set; }
        public virtual String Onu_TemperatureStatus { get; set; }
        public virtual String Onu_RxPower { get; set; }
        public virtual String Onu_TxPower { get; set; }

    }

    public class StatusOnuHardware
    {
        public virtual Int64 Id_Onu { get; set; }
        public virtual String Modelo { get; set; }
        public virtual String Versao_Software { get; set; }
        public virtual String Memoria { get; set; }
        public virtual String CPU { get; set; }
    }

    public class StatusOnuPing
    {
        public virtual Int64 Id_Onu { get; set; }
        public virtual List<String> Respostas_Ping { get; set; }

    }

    public class StatusOnuConfiguracoes
    {
        public virtual Int64 Id_Onu { get; set; }
        public virtual String PPPOE_Proxy { get; set; }
        public virtual String PPPOE_Usuario { get; set; }
        public virtual String PPPOEMODE { get; set; }
        public virtual String PPPOE_ServicoName { get; set; }

        public virtual String IP_Estatico { get; set; }
        public virtual String IP_Mascara { get; set; }
        public virtual String IP_Gateway { get; set; }
        public virtual String DNS_Master { get; set; }
        public virtual String DNS_Slave { get; set; }
        public virtual String Porta_ONU { get; set; }


        public virtual String QoS { get; set; }
        public virtual String Nome_Servico { get; set; }
        public virtual String Modo_Conexao { get; set; }
        public virtual String Tipo_Conexao { get; set; }
        public virtual String VLAN_ID { get; set; }
        public virtual String VLAN_COS { get; set; }
        public virtual String NAT { get; set; }
        public virtual String Tipo_IP { get; set; }




    }
    public class ClienteArquivosMobile
    {
        public String nome { get; set; }
        public String mimeType { get; set; }
        public Int64 idCliente { get; set; }
    }

    public class ClienteVinculoONUCombo : ObjComboResult<Int64>
    {
        public String Login { get; set; }
        public String Senha { get; set; }
    }

    public class HistoricoFaturaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Numero { get; set; }
        public virtual String Assunto { get; set; }

        public virtual String Tipo_Atendimento { get; set; }


        public virtual String Data_Limite { get; set; }

        public virtual String Setor { get; set; }

        public virtual String Extensao { get; set; }

        public virtual String Responsavel { get; set; }

        public virtual Status_Atendimento Status { get; set; }

        private IEnumerable<AtendimentoHistoricoAcoesCadastroModel> historico_atendimento;

        public virtual IEnumerable<AtendimentoHistoricoAcoesCadastroModel> Historico_Atendimento

        {
            get
            {
                if (historico_atendimento == null)
                    historico_atendimento = new List<AtendimentoHistoricoAcoesCadastroModel>();
                return historico_atendimento;
            }
            set
            {
                historico_atendimento = value;
            }
        }

        public virtual Int64 Id_Cliente { get; set; }

        public virtual String Dt_Inicial_Filtro { get; set; }
        public virtual String Dt_Final_Filtro { get; set; }
    }

    public class Historico_Nota_FiscalModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64 Id_Cliente { get; set; }

        public virtual String Dt_Inicial_Filtro { get; set; }

        public virtual String Dt_Final_Filtro { get; set; }
    }

    public class EnderecoViaCep
    {
        public virtual String Endereco { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual Int64 Id_Cidade { get; set; }
        public virtual Int64 Id_Bairro { get; set; }
    }

    public class UFMunicipioObj
    {
        public virtual String UF { get; set; }
        public virtual String Descricao { get; set; }

    }

    public class Cliente_Importacao : IExcelService
    {
        [ExcelService("ID_CLIENTE")]
        [Required]
        public virtual String Numero_Contrato { get; set; }

        [ExcelService("MEIO_CAPTACAO")]
        public virtual Int64? Meio_Captacao { get; set; }

        [Required]
        [ExcelService("CATEGORIA_CLIENTE")]
        public virtual Int64? Categoria_Cliente { get; set; }

        [MaxLength(14)]
        [ExcelService("TELEFONE_FIXO")]
        public virtual String Telefone_Fixo { get; set; }
        [MaxLength(15)]
        [ExcelService("TELEFONE_CELULAR")]
        public virtual String Telefone_Celular { get; set; }
        [MaxLength(15)]
        [ExcelService("TELEFONE_RECADO")]
        public virtual String Telefone_Recado { get; set; }
        [MaxLength(30)]
        [ExcelService("PESSOA_RECADO")]
        public virtual String Pessoa_Recado { get; set; }
        [MaxLength(50)]
        [ExcelService("EMAIL")]
        public virtual String Email_Principal { get; set; }

        [Required]
        [MaxLength(255)]
        [ExcelService("ENDERECO")]
        public virtual String Logradouro { get; set; }

        [Required]
        [MaxLength(10)]
        [ExcelService("NUMERO")]
        public virtual String Numero { get; set; }
        [MaxLength(100)]
        [ExcelService("COMPLEMENTO")]
        public virtual String Complemento { get; set; }

        [Required]
        [MaxLength(30)]
        [ExcelService("BAIRRO")]
        public virtual String Bairro { get; set; }

        [Required]
        [MaxLength(10)]
        [ExcelService("CEP")]
        public virtual String Cep { get; set; }
        [Required]
        [MaxLength(30)]
        [ExcelService("CIDADE")]
        public virtual String Cidade { get; set; }

        [Required]
        [ExcelService("UF")]
        public virtual String UF { get; set; }

        [Required]
        [MaxLength(50)]
        [ExcelService("PAÍS")]
        public virtual Int64? Pais { get; set; }

        [MaxLength(50)]
        [ExcelService("PONTO_REFERENCIA")]
        public virtual String Ponto_Referencia { get; set; }

        [ExcelService("ID_PLANO")]
        public virtual Int64? Cod_Plano { get; set; }
      
        [ExcelService("VALOR_FINAL_PLANO")]
        public virtual String Valor_Final_Plano { get; set; }

        [ExcelService("VALOR_FINAL_INSTALACAO")]
        public virtual String Valor_Final_Instalacao { get; set; }

        [ExcelService("PERIODO_CARENCIA")]
        public virtual Int32? Periodo_Carencia { get; set; }

        [ExcelService("PARCELAS_INSTALACAO")]
        public virtual Int64? Parcelas_Instalacao { get; set; }

        [ExcelService("AREA")]
        [MaxLength(4)]
        public virtual String Area { get; set; }

        [ExcelService("GRUPO_AREA")]
        [MaxLength(4)]
        public virtual String Grupo_Area { get; set; }

        [MaxLength(255)]
        [ExcelService("ENDERECO_PLANO")]
        public virtual String Logradouro_Plano { get; set; }
        [MaxLength(10)]
        [ExcelService("NUMERO_PLANO")]
        public virtual String Numero_Plano { get; set; }
        [MaxLength(100)]
        [ExcelService("COMPLEMENTO_PLANO")]
        public virtual String Complemento_Plano { get; set; }
        [MaxLength(30)]
        [ExcelService("BAIRRO_PLANO")]
        public virtual String Bairro_Plano { get; set; }
        [MaxLength(10)]
        [ExcelService("CEP_PLANO")]
        public virtual String Cep_Plano { get; set; }
        [MaxLength(30)]
        [ExcelService("CIDADE_PLANO")]
        public virtual String Cidade_Plano { get; set; }
        [ExcelService("UF_PLANO")]
        public virtual String UF_Plano { get; set; }
        [MaxLength(50)]
        [ExcelService("PAÍS_PLANO")]
        public virtual Int64? Pais_Plano { get; set; }

        [MaxLength(50)]
        [ExcelService("PONTO_REF_PLANO")]
        public virtual String Ponto_Referencia_Plano { get; set; }

        [MaxLength(50)]
        [ExcelService("ENDERECO_FINANCEIRO")]
        public virtual String Logradouro_Financeiro { get; set; }
        [MaxLength(10)]
        [ExcelService("NUMERO_FINANCEIRO")]
        public virtual String Numero_Financeiro { get; set; }
        [MaxLength(100)]
        [ExcelService("COMPLEMENTO_FINANCEIRO")]
        public virtual String Complemento_Financeiro { get; set; }
        [MaxLength(30)]
        [ExcelService("BAIRRO_FINANCEIRO")]
        public virtual String Bairro_Financeiro { get; set; }
        [MaxLength(10)]
        [ExcelService("CEP_FINANCEIRO")]
        public virtual String Cep_Financeiro { get; set; }
        [MaxLength(30)]
        [ExcelService("CIDADE_FINANCEIRO")]
        public virtual String Cidade_Financeiro { get; set; }
        [ExcelService("UF_FINANCEIRO")]
        public virtual String UF_Financeiro { get; set; }
        [MaxLength(50)]
        [ExcelService("PAÍS_FINANCEIRO")]
        public virtual Int64? Pais_Financeiro { get; set; }

        [MaxLength(50)]
        [ExcelService("PONTO_REF_FINANCEIRO")]
        public virtual String Ponto_Referencia_Financeiro { get; set; }

        [Required]
        [MaxLength(50)]
        [ExcelService("TIPO_IMOVEL")]      
        public virtual Int32? Tipo_Imovel { get; set; }    

        [Required]
        [ExcelService("ENVIO_FATURA")]
        public virtual String Forma_Envio { get; set; }

        [Required]
        [ExcelService("DIA_VENCIMENTO")]
        public virtual short? Dia_Vencimento { get; set; }

        [Required]
        [ExcelService("FORMA_PAGAMENTO")]       
        public virtual Int32? Forma_Pagamento { get; set; }

        [ExcelService("BANCO")]
        [MaxLength(255)]
        public virtual String Banco { get; set; }

        [ExcelService("AGENCIA")]
        [MaxLength(8)]
        public virtual String Agencia { get; set; }

        [ExcelService("CONTA")]
        [MaxLength(25)]
        public virtual String Conta_Corrente { get; set; }

        [ExcelService("DIGITO")]
        [MaxLength(1)]
        public virtual String Digito { get; set; }

        public Boolean Importar { get; set; }
        
        public List<ExcelServiceLogItem> validaCamposPlano()
        {
            List<ExcelServiceLogItem> validacoes = new List<ExcelServiceLogItem>();
            if (Cod_Plano.HasValue)
            {
                if(String.IsNullOrEmpty(Valor_Final_Plano))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Valor Final do Plano é obrigatório pois foi preenchido o Código do plano." });
                }
                if(String.IsNullOrEmpty(Valor_Final_Instalacao))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Valor Final da Instalação é obrigatória pois foi preenchido o Código do plano." });
                }
                if(!Periodo_Carencia.HasValue)
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Período Carência é obrigatório pois foi preenchido o Código do plano." });
                }
                if(!Parcelas_Instalacao.HasValue)
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Parcelas da Instalação é obrigatório pois foi preenchido o Código do plano." });
                }
                if(String.IsNullOrEmpty(Area))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Área é obrigatória pois foi preenchido o Código do plano." });
                }
                if(String.IsNullOrEmpty(Grupo_Area))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Grupo Área é obrigatória pois foi preenchido o Código do plano." });
                }
                if(String.IsNullOrEmpty(Logradouro_Plano))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Logradouro Plano é obrigatório pois foi preenchido o Código do plano." });
                }
                if (String.IsNullOrEmpty(Numero_Plano))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Número Plano é obrigatório pois foi preenchido o Código do plano." });
                }               
                if (String.IsNullOrEmpty(Bairro_Plano))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Bairro Plano é obrigatório pois foi preenchido o Código do plano." });
                }
                if (String.IsNullOrEmpty(Cep_Plano))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Cep Plano é obrigatório pois foi preenchido o Código do plano." });
                }
                if (String.IsNullOrEmpty(Cidade_Plano))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Cidade Plano é obrigatório pois foi preenchido o Código do plano." });
                }
                if (String.IsNullOrEmpty(UF_Plano))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "UF Plano é obrigatório pois foi preenchido o Código do plano." });
                }
                if (!Pais_Plano.HasValue)
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "País Plano é obrigatório pois foi preenchido o Código do plano." });
                }            
            }
            return validacoes;
        }
    }

    [ExcelServiceKey("Numero_Contrato")]
    [ExcelServiceSheetName("Pessoa Fisica")]
    public class Cliente_FisicaImportacao: Cliente_Importacao
    {
        [Required]
        [MaxLength(255)]
        [ExcelService("NOME_COMPLETO")]
        public virtual String Nome { get; set; }

        [Required]
        [MaxLength(18)]
        [ExcelService("CPF")]
        public virtual String Cpf { get; set; }

        [MaxLength(20)]
        [ExcelService("RG")]
        public virtual String RG { get; set; }

        [Required]
        [ExcelService("DATA_NASCIMENTO")]
        public virtual DateTime Data_Nascimento { get; set; }

        [ExcelService("DATA_EMISSAO_RG")]
        public virtual DateTime? Data_Emissao_RG { get; set; }

        [MaxLength(18)]
        [ExcelService("ORGAO_RG")]
        public virtual String Orgao_RG { get; set; }

        [Required]
        [MaxLength(18)]
        [ExcelService("SEXO")]
        public virtual String Sexo { get; set; }

        [MaxLength(18)]
        [ExcelService("ESTADO_CIVIL")]
        public virtual String Estado_Civil { get; set; }

        [MaxLength(18)]
        [ExcelService("GRAU_INSTRUCAO")]
        public virtual String Grau_Instrucao { get; set; }

        [MaxLength(50)]
        [ExcelService("PAI")]
        public virtual String Pai { get; set; }

        [MaxLength(50)]
        [ExcelService("MAE")]
        public virtual String Mae { get; set; }
        [MaxLength(25)]
        [ExcelService("PROFISSAO")]
        public virtual String Profissao { get; set; }

        [MaxLength(25)]
        [ExcelService("NACIONALIDADE")]
        public virtual String Nacionalidade { get; set; }
    }

    [ExcelServiceKey("Numero_Contrato")]
    [ExcelServiceSheetName("Pessoa Juridica")]
    public class Cliente_JuridicaImportacao : Cliente_Importacao
    {
        [Required]
        [MaxLength(255)]
        [ExcelService("NOME_FANTASIA")]
        public virtual String Nome { get; set; }

        [Required]
        [MaxLength(255)]
        [ExcelService("RAZAO_SOCIAL")]
        public virtual String Razao_Social { get; set; }

        [Required]
        [ExcelService("DATA_ABERTURA")]
        public virtual DateTime? Data_Abertura { get; set; }

        [Required]
        [MaxLength(18)]
        [ExcelService("CNPJ")]
        public virtual String CNPJ { get; set; }

        [MaxLength(15)]
        [ExcelService("INSCRICAO_ESTADUAL")]
        public virtual String Inscricao_Estadual { get; set; }

        [MaxLength(15)]
        [ExcelService("INSCRICAO_MUNICIPAL")]
        public virtual String Inscricao_Municipal { get; set; }

        [MaxLength(18)]
        [ExcelService("PORTE")]
        public virtual String Porte { get; set; }

        [MaxLength(100)]
        [ExcelService("RAMO_ATIVIDADE")]
        public virtual String Ramo_Atividade { get; set; }

        [MaxLength(18)]
        [ExcelService("RESPONSAVEL")]
        public virtual String Responsavel { get; set; }

        [MaxLength(18)]
        [ExcelService("CPF_RESPONSAVEL")]
        public virtual String Cpf_Responsavel { get; set; }

        [MaxLength(30)]
        [ExcelService("CARGO_RESPONSAVEL")]
        public virtual String Cargo_Responsavel { get; set; }
    }

    public class ObjCombClientePlanoProduto : IObjComboResult<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Text { get; set; }

    }
    public class AlteracaoStatusCliente
    {
        public Int32 Status { get; set; }
        public Int64 Id { get; set; }
    }

    public class ClienteCpfCnpj : IApiValidate
    {
        [Required]
        public string cpf_cnpj { get; set; }
        [Required]
        public string user { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string token { get; set; }
    }
}