using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using DocumentFormat.OpenXml.Wordprocessing;
using Org.BouncyCastle.Asn1.Ocsp;
using Sinapse.Model.enumerados;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace Aniel.Connect.API_Aniel.Models
{
    public class PropostaVendaApiRequest : ApiRequest
    {
        public long? IdCliente { get; set; }
        public string CpfCnpj { get; set; }
        public long? IdVendedor { get; set; }
        public long? IdSupervisor { get; set; }
        public long? IdSetorDepartamento { get; set; }
        public string Representante { get; set; }
        public string Nacionalidade { get; set; }
        public Sexo? Sexo { get; set; }
        public string DataNascimento { get; set; }
        public Estado_Civil? EstadoCivil { get; set; }
        public string RG { get; set; }
        public string OrgaoEmissor { get; set; }
        public string DataEmissao { get; set; }
        public Grau_Instrucao? Instrucao { get; set; }
        public string Mae { get; set; }
        public string Pai { get; set; }

        public string Profissao { get; set; }
        public string Email { get; set; }

        public string TelefoneFixo { get; set; }
        public string TelefoneCelular { get; set; }
        public string TelefoneRecado { get; set; }

        public string NomeRecado { get; set; }
        public long? IdCategoriaCliente { get; set; }
        public long? IdQuemIndicou { get; set; }
        public Tipo_Pessoa? TipoPessoa { get; set; }

        public long? IdMeioCaptacao { get; set; }
        public string NumeroContrato { get; set; }

        public string NomeCompleto { get; set; }
        public string NomeFantasia { get; set; }

        public string EnderecosIgualLead { get; set; }

        public EnderecoPropostaVendaRequestModel EnderecoInstalacao { get; set; }
        public EnderecoPropostaVendaRequestModel EnderecoCliente { get; set; }

        /// <summary>
        /// Informar o Id da Campanha caso o usuário foi obtido através de uma.
        /// </summary>
        public virtual Int64? IdCampanha { get; set; }
    }

    public class EnderecoPropostaVendaRequestModel
    {
        public string Pais { get; set; }
        public string UF { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }

        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string PontoReferencia { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string CodEscritorio { get; set; }
        public string CodArea { get; set; }
        public string CodLocalidade { get; set; }

        public int? CodTipoImovel { get; set; }
    }

    public class VendaRequest : ApiRequest
    {
        public long Id { get; set; }
        public bool Pendencia { get; set; }
        public string Supervisor { get; set; }
        public string Vendedor { get; set; }
        public string Departamento { get; set; }
        public string Plano { get; set; }
        public string Status { get; set; }
        public string StatusAtutomatico { get; set; }
        public Status_Venda StatusManual { get; set; }
        public string Data_Encerramento { get; set; }
        public string DiasSemContato { get; set; }
        public string NomeFantasia { get; set; }
        public string CpfCnpj { get; set; }
        public string Categoria { get; set; }
        public string Login { get; set; }
        public string UF { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Tipo_Imovel { get; set; }
        public string Data_Expiracao { get; set; }
        public string Avaliacao_Expirada { get; set; }
        public string Categoria_Cliente { get; set; }
        public Perfil_Vendedor Perfil_Vendedor_Id { get; set; }
        public string Forma_Pagamento { get; set; }
        public string Identificacao_Cliente { get; set; }
        public string TelefonePrincipal { get; set; }
        public string TelefoneCelular { get; set; }
        public string Email { get; set; }
        public string DataVenda { get; set; }
        public string DataUltimoContato { get; set; }
        public string Campanha { get; set; }
        public string Meio_Captacao { get; set; }
        public string Quem_Indicou { get; set; }
        public string Data_Hora_Criacao { get; set; }
        public string Tipo_Cliente { get; set; }
        public string Tipo_Pessoa { get; set; }
        public long Id_Cliente { get; set; }
        public string Termo_Adesao_Assinado { get; set; }
        public string EquipeVenda { get; set; }
        public string Contrato { get; set; }
        public string StatusAuditoria { get; set; }
        public DateTime Data_Criacao { get; set; }
    }

    public class VendaListViewModel
    {
        public Int64 Id { get; set; }

        public Boolean Pendencia { get; set; }
        public String Supervisor { get; set; }
        public String Vendedor { get; set; }
        public String Departamento { get; set; }

        public String Plano { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status { get; set; }

        public String StatusAtutomatico { get; set; }
        public Status_Venda StatusManual { get; set; }

        public String Data_Encerramento { get; set; }
        public String DiasSemContato { get; set; }

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
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }

        [Display(Name = "TIPO_IMOVEL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Tipo_Imovel { get; set; }
        public string Data_Expiracao { get; set; }
        public string Avaliacao_Expirada { get; set; }

        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Categoria_Cliente { get; set; }
        public Perfil_Vendedor Perfil_Vendedor_Id { get; set; }
        [Display(Name = "FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Forma_Pagamento { get; set; }

        [Display(Name = "IDENTIFICACAO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Identificacao_Cliente { get; set; }

        [Display(Name = "TELEFONEPRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefonePrincipal { get; set; }
        public String TelefoneCelular { get; set; }
        public String Email { get; set; }

        [Display(Name = "DATA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String DataVenda { get; set; }
        public String DataUltimoContato { get; set; }

        [Display(Name = "CAMPANHA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Campanha { get; set; }
        public String Meio_Captacao { get; set; }

        public string Quem_Indicou { get; set; }
        public String Data_Hora_Criacao { get; set; }
        public String Tipo_Cliente { get; set; }
        public String Tipo_Pessoa { get; set; }

        public Int64 Id_Cliente { get; set; }

        [Display(Name = "TERMO_ADESAO_ASSINADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Termo_Adesao_Assinado { get; set; }
        public String EquipeVenda { get; set; }
    }
    public class Forma_PagamentoApiRequest : ApiRequest
    {
        public long? Id { get; set; }
        public short? Dia { get; set; }
    }
}
