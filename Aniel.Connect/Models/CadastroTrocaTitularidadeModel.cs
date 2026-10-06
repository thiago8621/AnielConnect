using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using DocumentFormat.OpenXml.Office2010.Excel;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class CadastroTrocaTitularidadeModel
    {
        public virtual Tipo_Doc_Termo_Anexo TipoDocumento { get; set; }
        public virtual Status_Doc_Aceite Status_Doc_Aceite { get; set; }
        public virtual Lado_Doc_Aceite Lado { get; set; }
        private IEnumerable<EtapaVendaViewModel> _etapasTroca = null;
        public long Id { get; set; }
        public long NumeroContrato { get; set; }
        public StatusTrocaTitularidade StatusTrocaTitularidade { get; set; }
        public ClienteCadastroTitularidade ClienteAtual { get; set; } 
        public ClienteCadastroTitularidade ClienteNovo { get; set; }
        public Boolean ProximaEtapa { get; set; }
        public int EtapaAtualVenda { get; set; }
        public string StatusDescricao { get; set; }
        public EnderecoClienteViewModel EnderecoCobranca { get; set; }
        public bool ConsultaCreditoRestringeVenda { get; set; }
        public bool ConsultaViabilidadeRestringeVenda { get; set; }
        public bool ConsultaPendenciaFinanceiraRestringeVenda { get; set; }
        public bool ConsultaPendenciaEnderecoRestringeVenda { get; set; }
        public IEnumerable<EtapaVendaViewModel> EtapasTroca
        {
            get
            {
                if (this._etapasTroca == null)
                    this._etapasTroca = new List<EtapaVendaViewModel>();

                return this._etapasTroca;
            }
            set
            {
                this._etapasTroca = value;
            }
        }

        private List<Forma_Pagamento_ClienteModels> Formas_PagamentoLista;

        public virtual List<Forma_Pagamento_ClienteModels> Formas_Pagamento
        {
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
    }

    public class ClienteCadastroTitularidade
    {
        public Int64 Id_Cliente { get; set; }

        public Int64 Id_Forma_Pagamento_Cliente_Preferida { get; set; }

        public String Justificativa_Avaliacao_Comercial { get; set; }

        [Display(Name = "COD_AREA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cod_Area { get; set; }

        [Display(Name = "COD_ESCRITORIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cod_Escritorio { get; set; }

        public String Regime { get; set; }

        [Display(Name = "COD_LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int32? Cod_Localidade { get; set; }

        public String Localidade { get; set; }

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

        public String DataCriacao { get; set; }
        public String DataEncerramento { get; set; }

        [Display(Name = "ULTIMO_CONTATO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public DateTime? Ultimo_Contato { get; set; }

        [Display(Name = "TIPO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        //[DataType(DataType.Text)]
        public Tipo_Cliente? Tipo_Cliente { get; set; }
        public Tipo_Pessoa? TipoPessoa { get; set; }

        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String CpfCnpj { get; set; }

        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Meio_Captacao { get; set; }

        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Meio_Captacao { get; set; }


        [Display(Name = "NOMECOMPLETO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String NomeCompleto { get; set; }

        public EnderecoClienteViewModel EnderecoPrincipal { get; set; }

        public EnderecoClienteViewModel EnderecoInstalacao { get; set; }

        public EnderecoClienteViewModel EnderecoCobranca { get; set; }
        public EnderecoClienteViewModel EnderecoProspecto { get; set; }

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
        public string RG { get; set; }

        [Display(Name = "DATAEMISSAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string DataEmissao { get; set; }

        [Display(Name = "ORGAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Orgao { get; set; }

        [Display(Name = "PAI", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Pai { get; set; }

        [Display(Name = "MAE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Mae { get; set; }

        [Display(Name = "TELEFONEFIXO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefoneFixo { get; set; }

        [Display(Name = "TELEFONECELULAR", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefoneCelular { get; set; }

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
        public string Profissao { get; set; }

        public String Identificacao_Cliente { get; set; }

        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(70, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Email { get; set; }

        //consulta_credito
        public String StatusAprovacaoConsultaCredito { get; set; }
        public Int64 RestricaoFaixaPontos_1 { get; set; }
        public Int64 RestricaoFaixaPontos_2 { get; set; }
        public Int64 RestricaoFaixaPontos_3 { get; set; }
        public Int64 CriterioRestricaoUltimaPendencia { get; set; }
        public String CriterioRestricaoScore { get; set; }
        public Int64 CriterioRestricaoTotalDebitos { get; set; }
        public Int64 TotaldePontos { get; set; }
        public String ResultadoCriteriosConsulta { get; set; }
        public String Categoria { get; set; }
        public long? Id_Categoria_Cliente{ get; set; }

        #endregion

        #region Juridico
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
        #endregion

        #region  

        private List<ClienteContratoListModel> ClienteContratoLista;

        public virtual List<ClienteContratoListModel> ClienteContratos
        {
            get
            {
                if (ClienteContratoLista == null)
                    ClienteContratoLista = new List<ClienteContratoListModel>();
                return ClienteContratoLista;
            }
            set
            {
                ClienteContratoLista = value;
            }
        }

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

        private List<Forma_Pagamento_ClienteModels> Formas_PagamentoLista;

        public virtual List<Forma_Pagamento_ClienteModels> Formas_Pagamento
        {
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


        [Display(Name = "MODELO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Modelo_Contrato { get; set; }

        [Display(Name = "NUMERO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public String Numero_Contrato { get; set; }

        #endregion
    }
}