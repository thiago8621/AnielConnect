using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Aniel.Connect.Core;
using Sinapse.Model.enumerados;
using Aniel.Connect.Models.HubDesenvolvedor;
using Sinapse.Model.nhibernate;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Util.Excel;
using Sinapse.Util;
using Aniel.Connect.Repository.Model;

namespace Aniel.Connect.Models
{
    public class Mapa_ViabilidadeModel
    {
        public virtual long Id { get; set; }
        public virtual string Latitude { get; set; }
        public virtual string Longitude { get; set; }
        public virtual String Certificado { get; set; }
        public virtual bool Area_Cobertura { get; set; }
        public virtual bool Caixa { get; set; }
        public virtual bool Cliente { get; set; }
        public virtual bool Lead { get; set; }
        public virtual bool Prospect { get; set; }
    }
    public class Cliente_ProspectoPasso1ViewModel
    {

        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Tipo_Pessoa TipoPessoa { get; set; }

        [Display(Name = "CPF", ResourceType = typeof(Resources.RsCadastros))]
        public String CPF { get; set; }

        [Display(Name = "CNPJ", ResourceType = typeof(Resources.RsCadastros))]
        public String CNPJ { get; set; }

        [Display(Name = "DATANASCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public String DataNascimento { get; set; }
        public String Mensagem { get; set; }
        public ResponseHub<ResponseCpf> retornoCPF { get; set; }

        public ResponseHub<ResponseCnpj> retornoCNPJ { get; set; }
    }


    public class Cliente_ProspectoListViewModel
    {
        public Int64 Id { get; set; }
        public String Numero_Contrato { get; set; }

        public Boolean Pendencia { get; set; }


        public String Sexo { get; set; }

        public String Pais { get; set; }
        public String Estado_Civil { get; set; }
        public String Departamento { get; set; }


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
        public string TelefoneCelular { get; set; }
        public string Data { get; set; }
        public string MeioCaptcao { get; set; }
        public string NomeIndicacao { get; set; }
        public string NomeCampanha { get; set; }
        public string Id_QuemIndicou { get; set; }
        public string OutroMeio { get; set; }
        public string UsuarioCriacao { get; set; }

    }

    public class Cliente_Lead_ConsultaEndereco
    {
        public String logradouro { get; set; }
        public String numero { get; set; }
    }

    public class Cliente_Prospecto_DataTableRequest : DataTableRequest
    {
        public String AcessoUsuario { get; set; }
        public String AcessoProspect { get; set; }
        public String Tipo_Pessoa { get; set; }
        public String Status_Prospecto { get; set; }
        public String Departamento { get; set; }
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
        public Int64? Id_Modelo { get; set; }
        public bool Somente_Leads { get; set; }
        [Venda_Modelo(Filtro_PainelVendaEnum.EtapaVenda)]
        public String EtapaVenda { get; set; }
        public String StatusEtapa { get; set; }
        public String Data_Inicial { get; set; }
        public String Data_Final { get; set; }
        public string IdMeioCaptcao { get; set; }
        public string IdCampanha { get; set; }
        public string Id_QuemIndicou { get; set; }
        public string FiltroEndereco { get; set; }
        public string UsuarioCriacao { get; set; }
        public List<string> ConsultasRestricao { get; set; } = new List<string>();        

    }


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

    [ExcelServiceKey("Numero_Contrato")]
    [ExcelServiceSheetName("Pessoa Fisica")]
    public class Cliente_ProspectoFisicaImportacao : Cliente_ProspectoImportacao
    {

        [Required]
        [MaxLength(255)]
        [ExcelService("NOME_COMPLETO")]
        public virtual String Nome { get; set; }


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

    [ExcelServiceKey("Numero_Contrato")]
    [ExcelServiceSheetName("Pessoa Juridica")]
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


    public class Cliente_ProspectoMobile
    {
        public Int64 Id { get; set; }

        public Tipo_Cliente? TipoCliente { get; set; }
        public Tipo_Pessoa? TipoPessoa { get; set; }
        public Boolean Status { get; set; }
        public Status_Prospecto? Status_Prospecto { get; set; }
        public String CpfCnpj { get; set; }

        public String NomeCompleto { get; set; }
        public string RG { get; set; }
        public string DataEmissao { get; set; }
        public string Orgao { get; set; }
        public String DataNascimento { get; set; }
        public string Nacionalidade { get; set; }
        public Sexo? Sexo { get; set; }
        public Estado_Civil? EstadoCivil { get; set; }
        public Grau_Instrucao? Instrucao { get; set; }
        public Int64? Id_Categoria_Cliente { get; set; }
        public string Profissao { get; set; }
        public string Pai { get; set; }
        public string Mae { get; set; }

        public String NomeFantasia { get; set; }
        public DateTime? Data_Abertura { get; set; }
        public String Razao_Social { get; set; }
        public String Inscricao_Estadual { get; set; }
        public String Inscricao_Municipal { get; set; }
        public Porte? Porte { get; set; }
        public String Representante { get; set; }
        public String Cargo_Responsavel { get; set; }
        public String Ramo_Atividade { get; set; }
        public String CPFResponsavel { get; set; }

        public string TelefoneFixo { get; set; }
        public string Telefone_Celular { get; set; }
        public string TelefoneRecado { get; set; }
        public string NomeRecado { get; set; }
        public string Email { get; set; }

        private EnderecoClienteViewModel _enderecoProspecto { get; set; }
        public EnderecoClienteViewModel Endereco_Prospecto
        {
            get
            {
                if (_enderecoProspecto == null)
                {
                    _enderecoProspecto = new EnderecoClienteViewModel();
                }

                _enderecoProspecto.Tipo = Tipo_EnderecoCliente.Prospecto;
                return _enderecoProspecto;

            }
            set
            {
                _enderecoProspecto = value;
            }
        }

        public Int64? Id_Meio_Captacao { get; set; }
        public Int64? Id_Departamento { get; set; }
    }

    public class ConsultarCliente_ProspectoMobileRequest
    {
        public bool IsCpfCnpj { get; set; }
        public string Busca { get; set; }
    }

}