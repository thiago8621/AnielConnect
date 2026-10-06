using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Repository.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Cliente_ProspectoApi
    {
        public virtual String Cpf { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Status_Prospecto { get; set; }
        public virtual String Status { get; set; }
        public virtual String Numero_Contrato { get; set; }
        public virtual String TelefoneFixo { get; set; }
        public virtual String TelefoneCelular { get; set; }
        public virtual String TelefoneRecado { get; set; }
        public virtual String NomeRecado { get; set; }
        public virtual String Email { get; set; }
        public virtual String UF { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Tipo_Imovel { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual String Ponto_Referencia { get; set; }
        public virtual String Nome { get; set; }
        public virtual String DataNascimento { get; set; }
        public virtual String DataAbertura { get; set; }
        public virtual Int64? IdCampanha { get; set; }
    }
    /// <summary> Request do Cliente_Prospecto </summary>
    public class Cliente_ProspectoApiRequest : ApiRequest 
    {
        public virtual long? Id { get; set; }
        /// <summary> Cpf do Cliente_Prospecto </summary>
        public virtual String Cpf { get; set; }
        /// <summary> Categoria do Cliente_Prospecto </summary>
        public virtual Int32? Categoria { get; set; }
        /// <summary> Cep do Cliente_Prospecto </summary>
        public virtual String Cep { get; set; }
        /// <summary> Logradouro do Endereço do Cliente_Prospecto </summary>
        public virtual String Logradouro { get; set; }
        
        /// <summary> 
        ///Status de Cliente do Cliente_Prospecto 
        ///Inativo = 0,
        ///Ativo = 1
        /// </summary>
        public virtual Status_Cliente Status { get; set; }
        /// <summary> Número de Telefone Fixo do Cliente_Prospecto </summary>
        public virtual String TelefoneFixo { get; set; }
        /// <summary> Número de Telefone de Celular do Cliente_Prospecto </summary>
        public virtual String TelefoneCelular { get; set; }
        /// <summary> Número de Telefone de Recados do Cliente_Prospecto </summary>
        public virtual String TelefoneRecado { get; set; }
        /// <summary> Pessoa de Recado do Cliente_Prospecto </summary>
        public virtual String NomeRecado { get; set; }
        /// <summary> Email do Cliente_Prospecto </summary>
        public virtual String Email { get; set; }
        /// <summary> UF do Cliente_prospecto </summary>
        public virtual Int64 UF { get; set; }
        /// <summary> Bairro do Cliente_Prospecto </summary>
        public virtual String Bairro { get; set; }
        /// <summary> Numero da Cidade do Cliente_Prospecto </summary>
        public virtual Int64 Cidade { get; set; }
        /// <summary> Tipo de Imóvel do Cliente_Prospecto </summary>
        public virtual Int32? Tipo_Imovel { get; set; }      
        /// <summary> Complemento do Endereco do Cliente_Prospecto </summary>
        public virtual String Complemento { get; set; }
        /// <summary> Latitude do Cliente_Prospecto </summary>
        public virtual Double Latitude { get; set; }
        /// <summary> Longitude do Cliente_Prospecto </summary>
        public virtual Double Longitude { get; set; }
        /// <summary> Ponto de Referencia do Cliente_Prospecto </summary>
        public virtual String Ponto_Referencia { get; set; }
        /// <summary> Nome do Cliente_Prospecto </summary>
        [Required]
        public virtual String Nome { get; set; }
        public virtual String FiliacaoMae { get; set; }
        public virtual String FiliacaoPai { get; set; }
        /// <summary> Informações complemetares do Cliente_Prospecto </summary>
        public virtual String Informacoes_Complementares { get; set; }
        /// <summary> Numero do Contrato do Cliente_Prospecto </summary>
        public virtual String Numero_contrato { get; set; }
        /// <summary> Numero  do Endereco do Cliente_Prospecto </summary>
        public virtual String Numero { get; set; }
        /// <summary> Data de Nascimento do CLiente_Prospecto </summary>
        public virtual String DataNascimento { get; set; }
        /// <summary> Data de Abertura da Empresa do Cliente_Prospecto </summary>
        public virtual String DataAbertura { get; set; }
        /// <summary>
        /// Tipo de Cliente do Cliente_Prospecto
        ///Cliente = 0,        
        ///Prospecto = 1
        /// </summary>
        public virtual Tipo_Cliente TipoCliente { get; set; }
        /// <summary>
        ///Tipo de Pessoa do Cliente_Prospecto
        ///Fisica = 0,
        ///Juridica = 1,
        ///Fisica_Rural = 2,
        ///Juridica_Rural = 3
        ///</summary>
        public virtual Tipo_Pessoa TipoPessoa { get; set; }

        /// <summary>
        /// Informar o Id da Campanha caso o usuário foi obtido através de uma.
        /// </summary>
        public virtual Int64? IdCampanha { get; set; }
        public virtual Int64? Meio_Captacao { get; set; }

        public long? IdEnderecoLogradouro { get; set; }
        public long? IdCondominioPredio { get; set; }
    }



    #region ClienteProspectoListView
    public class Cliente_ProspectoListViewModel
    {
        public Int64 Id { get; set; }
        public String Numero_Contrato { get; set; }

        public Boolean Pendencia { get; set; }


        public String Sexo { get; set; }

        public String Pais { get; set; }
        public String Estado_Civil { get; set; }


        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status { get; set; }

        [Display(Name = "STATUS_PROSPECTO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status_Prospecto { get; set; }

        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TipoPessoa { get; set; }

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

        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
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
    #endregion

    #region ClienteProspecotDadosGeraisView
    public class Cliente_ProspectoDadosGeraisViewModel : IViewModel<Int64>
    {
        public Cliente_ProspectoDadosGeraisViewModel()
        {
            EnderecoPrincipal = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Principal };
            EnderecoInstalacao = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Instalacao };
            EnderecoCobranca = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Cobranca };
        }


        public Int64 Id { get; set; }



        //DADOS GERAIS****************************************************************************************************

        [Display(Name = "SITE", ResourceType = typeof(Resources.RsCadastros))]
        public String Site { get; set; }

        [Display(Name = "EMAIL_AUX", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(70, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Email_Aux { get; set; }

        [Display(Name = "TELEFONE_FIXO2", ResourceType = typeof(Resources.RsCadastros))]
        public String Telefone_Fixo2 { get; set; }

        [Display(Name = "RESPONSAVEL_FINANCEIRO", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Responsavel_Financeiro { get; set; }

        [Display(Name = "MODELO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Modelo_Contrato { get; set; }

        [Display(Name = "MODELO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public String Numero_Contrato { get; set; }

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
        public String Desc_Cod_Escritorio { get; set; }

        [Display(Name = "COD_LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Desc_Cod_Localidade { get; set; }

        #region Fisico

        public Tipo_Cliente? TipoCliente { get; set; }

        public Tipo_Pessoa? TipoPessoa { get; set; }

        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String CpfCnpj { get; set; }

        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Meio_Captacao { get; set; }

        [Display(Name = "COMO_FICOU_SABENDO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Meio_Captacao { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public Boolean Status { get; set; }

        [Display(Name = "STATUS_PROSPECTO", ResourceType = typeof(Resources.RsCadastros))]
        public Status_Prospecto? Status_Prospecto { get; set; }

        [Display(Name = "NOMECOMPLETO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String NomeCompleto { get; set; }
        public Boolean End_Cobranca_Igual_Principal { get; set; }
        public Boolean End_Instalacao_Igual_Principal { get; set; }

        public EnderecoClienteViewModel EnderecoPrincipal { get; set; }

        public EnderecoClienteViewModel EnderecoInstalacao { get; set; }

        public EnderecoClienteViewModel EnderecoCobranca { get; set; }

        [Display(Name = "DATANASCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String DataNascimento { get; set; }


        [Display(Name = "NACIONALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
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
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Pai { get; set; }

        [Display(Name = "MAE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
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

        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefonePrincipal { get; set; }

        [Display(Name = "INSTRUCAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Grau_Instrucao? Instrucao { get; set; }

        [Display(Name = "PROFISSAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(25, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Profissao { get; set; }

        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(70, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Email { get; set; }




        #endregion

        #region Juridico

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String NomeFantasia { get; set; }
        public DateTime? Data_Abertura { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
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

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Ramo_Atividade { get; set; }
        public String CPFResponsavel { get; set; }


        #endregion

        //PLANOS E PRODUTOS****************************************************************************************************
        #region
        [Display(Name = "QUAL_MEIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Qual_Meio { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Quem_Indicou { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Id_Quem_Indicou { get; set; }

        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Categoria_Cliente { get; set; }

        #endregion

    }
    #endregion


    #region ProspectoDataTable
    public class Cliente_Prospecto_DataTableRequest : DataTableRequest
    {
        public String Tipo_Pessoa { get; set; }
        public String Status_Prospecto { get; set; }
        public String Status { get; set; }

        public String Pais { get; set; }
        public String Estado { get; set; }

        public String Cidade { get; set; }

        public String Bairro { get; set; }

        public String Login_Radius { get; set; }

        public String Categoria_Cliente { get; set; }

        public String Sexo { get; set; }

        public String Estado_Civil { get; set; }

        public String Grau_Instrucao { get; set; }

        public String Id_Plano_Produto { get; set; }

    }
    #endregion


    #region ProspectoImportacao
    public class Cliente_ProspectoImportacao : IExcelService
    {
        [ExcelService("ID_CLIENTE")]
        [Required]
        public virtual String Numero_Contrato { get; set; }

        [ExcelService("MEIO_CAPTACAO")]
        public virtual Int64? Meio_Captacao { get; set; }

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
        [MaxLength(50)]
        [ExcelService("ENDERECO")]
        public virtual String Logradouro { get; set; }
        [MaxLength(10)]
        [ExcelService("NUMERO")]
        public virtual String Numero { get; set; }
        [MaxLength(100)]
        [ExcelService("Complemento")]
        public virtual String Complemento { get; set; }
        [MaxLength(30)]
        [ExcelService("Bairro")]
        public virtual String Bairro { get; set; }
        [MaxLength(10)]
        [ExcelService("CEP")]
        public virtual String Cep { get; set; }
        [MaxLength(30)]
        [ExcelService("CIDADE")]
        public virtual String Cidade { get; set; }
        [ExcelService("UF")]
        public virtual String UF { get; set; }
        [MaxLength(50)]
        [ExcelService("PAÍS")]
        public virtual String PAIS { get; set; }

        [MaxLength(50)]
        [ExcelService("PONTO_REFERENCIA")]
        public virtual String Ponto_Referencia { get; set; }

        [MaxLength(50)]
        [ExcelService("TIPO_IMOVEL")]
        public virtual Int32? Tipo_Imovel { get; set; }
        public Boolean Importar { get; set; }
    }
    #endregion

    [ExcelServiceKey("Numero_Contrato")]
    [ExcelServiceSheetName("Pessoa Fisica")]
    #region FisicaImportacao
    public class Cliente_ProspectoFisicaImportacao : Cliente_ProspectoImportacao
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
    #endregion

    [ExcelServiceKey("Numero_Contrato")]
    [ExcelServiceSheetName("Pessoa Juridica")]
    #region JuridicaImportacao
    public class Cliente_ProspectoJuridicaImportacao : Cliente_ProspectoImportacao
    {
        [Required]
        [MaxLength(255)]
        [ExcelService("NOME_FANTASIA")]
        public virtual String Nome { get; set; }
        [Required]
        [MaxLength(255)]
        [ExcelService("RAZAO_SOCIAL")]
        public virtual String Razao_Social { get; set; }


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
    #endregion




}