using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.HubDesenvolvedor;
using Aniel.Connect.Repository.Model;
using MongoDB.Bson.Serialization.Attributes;
using DocumentFormat.OpenXml.Drawing;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;

namespace Aniel.Connect.Models
{
    public class ClientePasso1ViewModel
    {
        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        public Tipo_Pessoa TipoPessoa { get; set; }

        [Display(Name = "CPF", ResourceType = typeof(Resources.RsCadastros))]
        public String CPF { get; set; }

        [Display(Name = "CNPJ", ResourceType = typeof(Resources.RsCadastros))]
        public String CNPJ { get; set; }

        [Display(Name = "DATANASCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public String DataNascimento { get; set; }

        public ResponseHub<ResponseCpf> retornoCPF { get; set; }
        public ResponseHub<ResponseCnpj> retornoCNPJ { get; set; }
    }
    public class ClienteListaBuscaAtendimento : ObjBusca<Int64>
    {
        public virtual String CPF_CNPJ_Cliente { get; set; }
        public virtual String RG_Cliente { get; set; }
        public virtual String Data_Nasc_Cliente { get; set; }

        public virtual String Id_Categoria_Cliente { get; set; }
        public virtual String Tipo_Prioridade { get; set; }
        public virtual String Categoria_Cliente { get; set; }
        public virtual Int64? Id_Organizacao { get; set; }

        public virtual String Nome_Fantasia { get; set; }
        public virtual String Telefone_Fixo { get; set; }
        public virtual String Telefone_Celular { get; set; }
        public virtual String Telefone_Recado { get; set; }
        public virtual String Nome_Recado { get; set; }
        public virtual String Email_Principal { get; set; }

        public virtual String Forma_Pagamento_Utilizada { get; set; }
        public virtual String Endereco_Instalacao { get; set; }
        public virtual String Login_Radius { get; set; }
        public virtual String Valor_Final_Total { get; set; }
        public virtual String Id_Endereco_Cliente { get; set; }
        public virtual String Id_Endereco_Prospecto { get; set; }
        public virtual Boolean Cliente_Prospecto { get; set; }
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
        public String TipoCliente { get; set; }
    }

    public class ClienteListViewModel
    {
        public Int64 Id { get; set; }
        public Boolean Pendencia { get; set; }


        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status { get; set; }

        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TipoPessoa { get; set; }

        [Display(Name = "ESTADO_CIVIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Estado_Civil { get; set; }
        public string Tipo_Prioridade { get; set; }

        [Display(Name = "SEXO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Sexo { get; set; }

        [Display(Name = "GRAU_INSTRUCAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Grau_Instrucao { get; set; }

        [Display(Name = "NOMEFANTASIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string NomeFantasia { get; set; }

        [Display(Name = "RAZAOSOCIAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string RazaoSocial { get; set; }
        public Perfil_Vendedor Perfil_Vendedor_ { get; set; }

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
        public Int64? Id_Motivo_Desistencia_Venda { get; set; }
    }

    public class UnificarClienteListViewModel
    {
        public Int64 Id { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status { get; set; }

        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TipoPessoa { get; set; }

        public string NomeFantasia { get; set; }

        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string CpfCnpj { get; set; }
        public string TipoCliente { get; set; }

    }

    public class ClientePlanoListViewModel
    {
        public Int64 Id { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status { get; set; }

        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TipoPessoa { get; set; }

        public string NomeFantasia { get; set; }

        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string CpfCnpj { get; set; }

        public string TipoCliente { get; set; }
        public string Plano { get; set; }
        public string NumeroContrato { get; set; }
        public string Login { get; set; }
        public string Endereco { get; set; }
    }

    public class ListaContatoListViewModel
    {
        public Int64 Id { get; set; }
        public String Tipo { get; set; }
        public string Telefone { get; set; }
        public string Nome { get; set; }
        public string InformacaoAdicional { get; set; }
        public string Email { get; set; }
        public String TipoValue { get; set; }
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
        public Int64 Id_Pesquisa_Satisfacao { get; set; }
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
        public string DataIni { get; set; }
        public string DataCriacao { get; set; }
        public string Email { get; set; }
        public string Descricao { get; set; }
        public string Obs { get; set; }
    }

    public class ClienteGetList
    {
        public virtual Int64 Id { get; set; }
        public virtual String NomeFantasia { get; set; }
    }
    public class setWorkflowStatus
    {
        public virtual Int64 idVenda { get; set; }
        public virtual Workflow_VendasAtividade proximaEtapaWorkflow { get; set; }
    }
    public class ClienteDadosGeraisViewModel : IViewModel<Int64>
    {
        public ClienteDadosGeraisViewModel()
        {
            EnderecoPrincipal = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Principal };
            EnderecoInstalacao = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Instalacao };
            EnderecoCobranca = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Cobranca };
        }

        public Int64 Id { get; set; }
        public string ValorReajusteContrato { get; set; }

        public Int64 Id_Cliente { get; set; }

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
        public Localidade_Anatel? Localidade_Anatel { get; set; }
        public Int64 Regime { get; set; }
        public Modelo_Anatel? Modelo_Anatel { get; set; }


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
        [MaxLength(150, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Email { get; set; }

        //public String Dia_Vencimento { get; set; }

        #endregion

        #region Juridico
        public String NomeFantasia { get; set; }
        public DateTime? Data_Abertura { get; set; }
        public String Razao_Social { get; set; }

        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Inscricao_Estadual { get; set; }
        public EnumInd_InscricaoEstadual Ind_Inscricao_Estadual { get; set; }

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

        private List<PlanoProdutoListModel> ListagemPlanosCancelados;
        public virtual List<PlanoProdutoListModel> PlanosCancelados
        {
            get
            {
                if (ListagemPlanosCancelados == null)
                    ListagemPlanosCancelados = new List<PlanoProdutoListModel>();
                return ListagemPlanosCancelados;
            }
            set
            {
                ListagemPlanosCancelados = value;
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
        public String Valor_Final { get; set; }

        [Display(Name = "VALOR_INSTALACAO_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Valor_Instalacao_Final { get; set; }


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

        public Boolean BloqueioAutomatico_Debito { get; set; }
        public Boolean Impedir_BloqueioAutomatico { get; set; }
        public Boolean Havera_Reducao_Velocidade { get; set; }

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
        public String Numero_ContratoVinculado { get; set; }

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

        public Int64? id_Forma_Pagamento_Cliente_Venda { get; set; } //Só para usar no Premier

        
        public Forma_Pagamento_ClienteModels Forma_PagamentoVenda { get; set; } //Remover

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

        public virtual Forma_Pagamento_ClienteModels Forma_Pagamento_Cliente { get; set; }

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

        /// <summary>
        /// Utilizado para preencimento da grid de planos
        /// </summary>
        public Int64? Id_Plano { get; set; }
        //public String Numero_Contrato { get; set; }

        public Int64? Id_Forma_Pagamento_Cliente_Preferida { get; set; }

        /// <summary>
        /// Utilizado na consulta de viabilidade
        /// </summary>
        public Int64? IdCaixaMaisProxima { get; set; }
        public Boolean ConsultaViabilidadeRealizada { get; set; }
        public string NumeroValidado { get; set; }
        public String Observacao_Motivo_Suspensao_Plano_Cliente_Outros { get; set; }
        public virtual String VinculosContratosClientes { get; set; }
        public bool Desbloqueio_Conf { get; set; }
        public int QuantidadeDiasBloqueio { get; set; }
        public Frequencia_Desbloqueio? Frequencia_Desbloqueio { get; set; }
        public bool BloqueioAutomaticoDebito { get; set; }
        public int BloquearNumeroDias { get; set; }
        public int BloquearAtrasarRenegociadas { get; set; }
        public int MaximoTitulosAberto { get; set; }
        public bool ImpedirBloqueio_Automatico { get; set; }
        public bool ImpedirBloqueio_Segunda { get; set; }
        public bool ImpedirBloqueio_Terca { get; set; }
        public bool ImpedirBloqueio_Quarta { get; set; }
        public bool ImpedirBloqueio_Quinta { get; set; }
        public bool ImpedirBloqueio_Sexta { get; set; }
        public bool ImpedirBloqueio_Sabado { get; set; }
        public bool ImpedirBloqueio_Domingo { get; set; }
        public bool ImpedirBloqueio_Feriados { get; set; }

        public virtual List<ComposicaoNotaCliModels> ComposicaoNotaClientes { get; set; }

        private List<ClienteContratoListModel> ClienteContratoLista;
        public virtual List<ClienteContratoListModel> ClienteContratosAtivos
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
    }

    public class AlterarEnderecoClienteViewModel
    {
        public EnderecoClienteViewModel EnderecoInstalacao { get; set; }
        public BuscarGeolocalizacaoModels Mapa { get; set; }
        public Int64? Id_Atendimento { get; set; }
        public Int64? Id_ClientePlanoProduto { get; set; }
        public bool ConsultaViabilidadeHabilitada { get; set; }
        public bool ConsultaPendenciaFinanceiraHabilitada { get; set; }
        public bool ConsultaPendenciaEnderecoHabilitada { get; set; }
        public bool ConsultaCreditoHabilitada { get; set; }

        public StatusEtapaVenda StatusConsultaViabilidade { get; set; }
        public StatusEtapaVenda StatusConsultaPendenciaFinanceira { get; set; }
        public StatusEtapaVenda StatusConsultaPendenciaEndereco { get; set; }
        public StatusEtapaVenda StatusConsultaCredito { get; set; }
        public bool ConsultaCreditoVendedorVisualiza { get; set; }
        public bool ConsultaViabilidadeVendedorVisualiza { get; set; }
        public bool ConsultaPendenciaFinanceiraVendedorVisualiza { get; set; }
        public bool ConsultaPendenciaEnderecoVendedorVisualiza { get; set; }
        public int? ConsultaPendenciaViabilidadeValidade { get; set; }
        public int? ConsultaPendenciaEnderecoValidade { get; set; }
    }
    
    public class AlterarEnderecoIdPlanoViewModel
    {
        public EnderecoClienteViewModel EnderecoInstalacao { get; set; }
        public Int64? IdPlano { get; set; }
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

        public long? IdConcentradora { get; set; }
        public string VLAN { get; set; }

        public Int64? IdPool { get; set; }
        public Int64? Id_Pool_Ip_Endereco { get; set; }
    }

    public class ListaContatosViewModel : IViewModel<Int64>
    {
        public long Id { get; set; }

        public string Telefone { get; set; }

        public Tipo_Contato_Cliente? Tipo_Contato_Cliente { get; set; }

        public string Email { get; set; }

        public string Nome { get; set; }




    }

    public class AlterarPontoAcesso : IViewModel<Int64>

    {
        public Int64 Id { get; set; }
        public Int64? Id_Ponto_Acesso { get; set; }
        public Int64? Id_Pool_Ip { get; set; }
    }



    public class Cliente_DataTableRequest : DataTableRequest
    {
        public String Tipo_Pessoa { get; set; }

        public String Status { get; set; }

        public String Estado { get; set; }

        public String Cidade { get; set; }

        public String Bairro { get; set; }

        public String Tipo_Imovel { get; set; }

        public String Login_Radius { get; set; }

        public String Categoria_Cliente { get; set; }

        public String Sexo { get; set; }

        public String Estado_Civil { get; set; }

        public String Grau_Instrucao { get; set; }

        public String Id_Plano_Produto { get; set; }

        public String Mes_Aniversario { get; set; }

    }
    public class UnificarCliente_DataTableRequest : DataTableRequest
    {
        public String TipoPessoa { get; set; }

        public String Status { get; set; }

        public String TipoCadastro { get; set; }
        public long? Organizacao { get; set; }

    }

    public class PesquisarClientePlano_DataTableRequest : DataTableRequest
    {
        public String Status { get; set; }

        public String IdCliente { get; set; }

    }

    public class ListaContatoCliente_DataTableRequest : DataTableRequest
    {
        public String IdCliente { get; set; }
        public string  EmailComparacao { get; set; }
        public string  TelefoneComparacao { get; set; }        
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
        public String Id_Plano_Produto { get; set; }
        public String Id_Produto { get; set; }

    }

    public class RequestContratoDataTable : DataTableRequest
    {
        public string Contrato { get; set; }
        public bool PlanosCancelados { get; set; }
        public string IdModeloContrato { get; set; }
        public string IdCliente { get; set; }
        public string IdPlanoProduto { get; set; }
        public string IdClienteContrato { get; set; }


    }

    public class AtualizacaoInformacaoContratoRequest
    {
        public long IdContrato { get; set; }
        public string ValorReajuste { get; set; }
        public string TipoReajuste { get; set; }
        public bool PermiteDesbloqueio { get; set; }
        public int QuantidadeDiasBloqueio { get; set; }
        public int FrequenciaDesbloqueio { get; set; }
        public bool BloqueioAutomaticoDebito { get; set; }
        public int BloqueioDiasAtrasar { get; set; }
        public int BloqueioDiasAtrasarRenegociadas { get; set; }
        public int MaximoTitulosAbertos { get; set; }
        public bool ImpedirBloqueioAutomatico { get; set; }

        public bool ImpedirBloqueioAutomaticoDomingo { get; set; }
        public bool ImpedirBloqueioAutomaticoSegunda { get; set; }
        public bool ImpedirBloqueioAutomaticoTerca { get; set; }
        public bool ImpedirBloqueioAutomaticoQuarta { get; set; }
        public bool ImpedirBloqueioAutomaticoQuinta { get; set; }
        public bool ImpedirBloqueioAutomaticoSexta { get; set; }
        public bool ImpedirBloqueioAutomaticoSabado { get; set; }
        public bool ImpedirBloqueioAutomaticoFeriados { get; set; }
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
        public Int64? Id_Venda { get; set; }
        public Int64? IdContrato { get; set; }
        public Boolean Foto_Perfil { get; set; }
        public String NumeroContrato { get; set; }
        public String DataHora { get; set; }
        public String Usuario { get; set; }
        public string StatusAceiteContrato { get; set; }
        public bool AceiteContrato { get; set; }
        public Int64? ModeloSelecionadoId { get; set; }
        public Boolean? IsArquivoContratoAditivo { get; set; }
    }

    /* ANEXO CLIENTE OLÁ */
    public class ClienteOlaAnexoModelView : IViewModel<Int64>
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
        public Int64 Id_Cliente { get; set; }
        public Int64? Id_Venda { get; set; }
        public Boolean? IsArquivoContratoAditivo { get; set; }
        public Boolean? IsArquivoTermoAdesao { get; set; }
    }

    public class ClienteAnexoOlaRequest
    {
        public Int64 Id_Cliente { get; set; }
        public List<Int64> list_id_Contrato { get; set; }
        public Int64 Id_Venda { get; set; }
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
        public bool Combo { get; set; }
        public bool ItemCombo { get; set; }
        public String Combo_Descricao { get; set; }
        public long? ComboId { get; set; }

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
        public Regime_Anatel? Regime { get; set; }
        public Int64? FormaEnvio { get; set; }
        public Int64? DiaVencimento { get; set; }
        public virtual List<Int64> OpcoesVencimento { get; set; }
        public virtual List<ClienteAnexo_Mobile> contratos { get; set; }
    }
    public class UnificacaoNovoContatoClienteRequestModel : UnificarClienteRequestModel 
    {
        private List<long> IdContatos;
        public List<long> _IdContatos { 
            get 
            {
                if (IdContatos == null) 
                {
                    IdContatos = new List<long>();
                }
                return IdContatos;
            }
            set { value = IdContatos; }
        }
    }
    public class UnificarClienteRequestModel
    {
        public Int64? IdClienteUnificar { get; set; }
        public Int64? IdClienteDestino { get; set; }
        public String Usuario { get; set; }
        public String nomeUsuario { get; set; }
    }
    public class AdicionarContatoRequestModel
    {
        public Int32? TipoContato { get; set; }
        public String Nome { get; set; }
        public String NomeContato { get; set; }
        public String Email { get; set; }
        public String Telefone { get; set; }
        public String Usuario { get; set; }
        public Int64 IdCliente { get; set; }
        public Int64? Id { get; set; }

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
        public Regime_Anatel? Regime { get; set; }
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

    #region Mobile
    public class ClienteAnexo_Mobile
    {
        public virtual Int64 ID { get; set; }
        public virtual String Titulo { get; set; }
        public virtual String nome_arquivo { get; set; }
        public virtual String extensao_arquivo { get; set; }
        public virtual String url_arquivo { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ClienteChat_Mobile
    {
        public Int64? idCliente { get; set; }
        public String cpf_cnpj { get; set; }
        public String email { get; set; }
        public String telefone { get; set; }
        public String nome { get; set; }
        public Int64 idTipoAtendimento { get; set; }
        public Int64 idplanoProduto { get; set; }
        public String idConexaoChat { get; set; }
        public Int64? idCategoriaCliente { get; set; }
        public String identificadorTelegram { get; set; }
        public Escritorio_Area Escritorio_Area { get; set; }
        public ClienteAnexo_Mobile anexos { get; set; }
    }

    public class ClienteToken
    {
        public Int64 idCliente { get; set; }
        public String Device_id { get; set; }
    }

    #endregion

    public class Cliente_Plano_ProdutoViewModel
    {
        public Int64 Id { get; set; }

        //public virtual Endereco_Cliente Endereco_Instalacao { get; set; }

        public virtual String VelocidadeDownlaod { get; set; }
        public virtual String VelocidadeNominal { get; set; }
        public virtual String Id_Plano_Produto { get; set; }
        public Int64? Id_Cliente { get; set; }

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
        public virtual Regime_Anatel? Regime { get; set; }
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

        private List<Cliente_Plano_ProdutoViewModel> _trocaPlano;

        public List<Cliente_Plano_ProdutoViewModel> Plano_Troca
        {
            get
            {
                if (_trocaPlano == null)
                    _trocaPlano = new List<Cliente_Plano_ProdutoViewModel>();
                return _trocaPlano;
            }
            set
            {
                _trocaPlano = value;
            }
        }

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
        public virtual String Temperatura { get; set; }
        public virtual String StatusTemperatura { get; set; }
        public virtual String Rx { get; set; }
        public virtual String Tx { get; set; }
        public virtual String Rx_OLT { get; set; }
        public virtual String Tx_OLT { get; set; }
        public virtual String Distancia { get; set; }
        public virtual string TempoOnline { get; set; }
        public virtual string Voltagem { get; set; }
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
        public virtual Status_Conta_Receber Status_Conta_Receber { get; set; }

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
        public virtual Status_Atendimento? Status_Atendimento { get; set; }
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
        public virtual Regime_Anatel? Regime { get; set; }

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
                if (String.IsNullOrEmpty(Valor_Final_Plano))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Valor Final do Plano é obrigatório pois foi preenchido o Código do plano." });
                }
                if (String.IsNullOrEmpty(Valor_Final_Instalacao))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Valor Final da Instalação é obrigatória pois foi preenchido o Código do plano." });
                }
                if (!Periodo_Carencia.HasValue)
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Período Carência é obrigatório pois foi preenchido o Código do plano." });
                }
                if (!Parcelas_Instalacao.HasValue)
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Parcelas da Instalação é obrigatório pois foi preenchido o Código do plano." });
                }
                if (String.IsNullOrEmpty(Area))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Área é obrigatória pois foi preenchido o Código do plano." });
                }
                if (String.IsNullOrEmpty(Grupo_Area))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Numero_Contrato, Erro = "Grupo Área é obrigatória pois foi preenchido o Código do plano." });
                }
                if (String.IsNullOrEmpty(Logradouro_Plano))
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
    public class Cliente_FisicaImportacao : Cliente_Importacao
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
        public virtual Int64? IdContrato { get; set; }
        public virtual Int64? IdVenda { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Text { get; set; }
        public virtual Int64? IdModeloContrato { get; set; }
        public virtual String Id_NumeroContrato { get; set; }
        public virtual String Status { get; set; }
        public virtual String Status_Contrato { get; set; }
    }

    public class ObjComboContratoCliente : ObjCombClientePlanoProduto
    {
        private List<ContratoPlanos> Lista_Planos { get; set; }
        public List<ContratoPlanos> Listagem_Planos
        {
            get
            {
                if (Lista_Planos == null)
                {
                    Lista_Planos = new List<ContratoPlanos>();
                }
                return Lista_Planos;
            }
            set { Lista_Planos = value; }
        }

    }

    public class ContratoPlanos
    {
        public Int64? Id { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public string NumOS { get; set; }
        public string NecessitaCriarOS { get; set; }
        public string PossuiOSCriada { get; set; }
        public Double? Valor_Final_Total { get; set; }
        public Double? Valor_Final { get; set; }
        public String sValor_Final { get; set; }
        public String sValor_Instalacao { get; set; }
        public Double? Valor_Instalacao { get; set; }
        public Int64? Id_Forma_Pagamento_Cliente { get; set; }
        public Int64? Id_Forma_Pagamento { get; set; }
        public string Forma_Pagamento { get; set; }
        public string Id_Cliente_Contrato { get; set; }
        public string Tem_Carencia { get; set; }
        public Int64? Periodo_Carencia { get; set; }
        public Double? Multa_Carencia { get; set; }
        public string Renova_Carencia_Automaticamente { get; set; }
        public string Tem_Reajuste { get; set; }
        public string Frequencia_Reajuste { get; set; }
        public string Cancela_Contrato_Automatico_Final_Carencia { get; set; }
        public string DiasTeste { get; set; }
        public string Encerra_Carencia { get; set; }
        public string Detalhes_Plano { get; set; }
        public Int64? Id_Modelo_Contrato { get; set; }
        public Int64? Id_Plano_Produto { get; set; }
        public string Autenticacao_Radius { get; set; }
        public string Id_Login_Radius { get; set; }
        public string Login_Radius { get; set; }
        public string Senha_PPPOE { get; set; }
        public string Id_ONU { get; set; }
        public string Descricao_ONU { get; set; }
        public string Id_OLT { get; set; }
        public string Descricao_OLT { get; set; }
        public string Id_PON { get; set; }
        public string Descricao_PON { get; set; }
        public string Descricao_SLOT { get; set; }
        public string Numero_Sip { get; set; }
        public string Senha_Circuito_Sip { get; set; }
        public string Franquia_Minutos { get; set; }
        public string Velocidade_Nominal { get; set; }
        public string Frequencia_Cobranca { get; set; }
        public string Formato_Cobranca { get; set; }
        public string Bloquei_Automatico_Debito { get; set; }
        public string Dias_Atraso { get; set; }
        public string Impedir_Bloqueio_Automatico { get; set; }
        public string Impedir_Bloqueio_Sexta { get; set; }
        public string Impedir_Bloqueio_Sabado { get; set; }
        public string Impedir_Bloqueio_Domingo { get; set; }
        public string Impedir_Bloqueio_Feriados { get; set; }
        public string Reducao_Velocidade { get; set; }
        public string Termo_Adesao_Assinado { get; set; }
        public String Dias_Desabilitado_Debito { get; set; }
        public String Dias_Inadimplente { get; set; }
        public String Velocidade_Reduzida { get; set; }
        public String Cod_Area { get; set; }
        public String Cod_Escritorio { get; set; }
        public String Cod_Localidade { get; set; }
        public String Id_Tipo_Servico { get; set; }
        public String Contrato { get; set; }
        public String Projeto { get; set; }
        public int? Tipo_Tecnologia { get; set; }
        public string Data_Aceite { get; set; }
        public bool? Desbloqueio_Conf { get; set; }
        public bool? BloqueioAutomaticoDebito { get; set; }
        public int? QuantidadeDiasBloqueio { get; set; }
        public int? QuantidadeProducaoVariavel { get; set; }
        public int? Frequencia_Desbloqueio { get; set; }
        public int? BloquearNumeroDias { get; set; }
        public int? BloquearAtrasarRenegociadas { get; set; }
        public int? MaximoTitulosAberto { get; set; }
        public bool? ImpedirBloqueio_Automatico { get; set; }
        public bool? ImpedirBloqueio_Segunda { get; set; }
        public bool? ImpedirBloqueio_Terca { get; set; }
        public bool? ImpedirBloqueio_Quarta { get; set; }
        public bool? ImpedirBloqueio_Quinta { get; set; }
        public bool? ImpedirBloqueio_Sexta { get; set; }
        public bool? ImpedirBloqueio_Sabado { get; set; }
        public bool? ImpedirBloqueio_Domingo { get; set; }
        public bool? ImpedirBloqueio_Feriados { get; set; }
        public double Valor_Reajuste { get; internal set; }
        public double Percentual_Reajuste { get; internal set; }
        public string Tipo_Reajuste { get; internal set; }
        public string sValor_Total_Plano { get; set; }
        public Double? Valor_Total_Plano { get; set; }

    }


    public class AcaoConsultaCliente : DataTableRequest
    {
        public virtual String Identificacao { get; set; }
        public virtual String Cpf_Cnpj { get; set; }
        public virtual String Rg { get; set; }
    }

    public class NegociacaoCliente
    {
        public virtual bool Possui_Negociacao { get; set; }
        public virtual String Valor_Negociado { get; set; }
    }
    public class Detalhes_Plano_Produto
    {
        public virtual Int64? Id { get; set; }
        public virtual Boolean ImpedirBloqueio_Feriados { get; set; }
        public virtual Boolean ImpedirBloqueio_Domingo { get; set; }
        public virtual Boolean ImpedirBloqueio_Sabado { get; set; }
        public virtual Boolean ImpedirBloqueio_Sexta { get; set; }
        public virtual Boolean ImpedirBloqueio_Automatico { get; set; }
        public virtual Boolean BloqueioAutomatico_Debito { get; set; }
        public virtual String Dias_Atraso { get; set; }



    }

    public class Faturamento_Cliente_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id_Forma_Pagamento { get; set; }
        public virtual String PeriodoIni { get; set; }
        public virtual String PeriodoFim { get; set; }
        public virtual String Data_Vencimento { get; set; }


        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_Contrato { get; set; }
        public virtual Int64? Id_Endereco { get; set; }
        public virtual Int32? Status_Faturamento { get; set; }
        public virtual String Data_Emissao_Fatura_Ini { get; set; }
        public virtual String Data_Emissao_Fatura_Fim { get; set; }

    }

    public class Faturamento_ClienteListaModel
    {

        public virtual Int64 Id { get; set; }

        public virtual String Cliente { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_Faturamento_Protocolo { get; set; }
        public virtual String Cpf { get; set; }

        public virtual String Documento { get; set; }

        public virtual String Data_Vencimento { get; set; }
        public virtual String Data_Recebimento { get; set; }

        public virtual String Valor_Recebido { get; set; }
        public virtual String Valor_Previsto { get; set; }
        public virtual String Forma_Pagamento { get; set; }
        public virtual String Status { get; set; }
        public virtual String Protocolo { get; set; }


        public virtual Int64 Id_Conta_Receber { get; set; }
        public virtual String Mensagem { get; set; }

    }

    public class Faturamento_Log : DataTableRequest
    {
        public virtual String DataHoraCriacao { get; set; }
        public virtual String NomeFantasia { get; set; }
        public virtual String Observacao { get; set; }
        public virtual String Usuario { get; set; }
    }

    public class Faturamento_Modal_Detalhes
    {
        public virtual String DataHoraCriacao { get; set; }
        public virtual String DataRegistro { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Recorrencia { get; set; }
        public virtual String Valor { get; set; }
        public virtual String PeriodoMedicao { get; set; }
        public virtual String QuantidadeDiasFaturar { get; set; }
        public virtual String NumeroContrato { get; set; }
    }

    public class BindEnderecoFromVendaRequestModel
    {
        public Cliente cliente { get; set; }
        public VendaDadosGeraisViewModel viewVenda { get; set; }
        public Tipo_EnderecoCliente tipoEndereco { get; set; }
        private IEnumerable<Forma_Pagamento_ClienteModels> FormasPrivate { get; set; }
        private IEnumerable<PlanoProdutoListModel> PlanosPrivate { get; set; }
        /*No caso do Endereco de Cobrança e de Instalação*/

        public Cliente_PlanoProduto planoProduto { get; set; }
        /*No caso do Endereco de Cobrança*/

        public IEnumerable<Forma_Pagamento_ClienteModels> Formas
        {
            get
            {

                return FormasPrivate.ToList().Count > 0 ? FormasPrivate : new List<Forma_Pagamento_ClienteModels>();
            }
            set { FormasPrivate = value; }
        }
        /*No caso do Endereco de Instalação*/
        public IEnumerable<PlanoProdutoListModel> Planos
        {
            get
            {
                return PlanosPrivate.ToList().Count > 0 ? PlanosPrivate : new List<PlanoProdutoListModel>();

            }
            set { PlanosPrivate = value; }
        }

    }
    public class BindDadosClienteRequestModel
    {
        public VendaDadosGeraisViewModel Venda { get; set; }
        public String Usuario { get; set; }
    }
    public class BindCPP_VendaRequestModel
    {
        public PlanoProdutoListModel Request_PlanoProduto { get; set; }
        public Cliente Request_Cliente { get; set; }
        public Modelo_Contrato Request_Modelo { get; set; }
        public List<PostApiMensagem> Request_Mensagens { get; set; }
    }
    public class ClienteEquipamentos
    {
        public virtual String codMat { get; set; }
        public virtual String descMat { get; set; }
        public virtual String codCpl { get; set; }
        public virtual String saldo { get; set; }

    }
    public class ConfigComboClientePlanoProdutoContrato
    {
        public bool ExibeCancelado { get; set; }
    }
    public class ConfigValidacaoContato 
    {
        public long Id { get; set; }
    }

    public class ContatosValidacaoListModel
    {
        public long Id { get; set; }
        public string NomeContato { get; set; }
        public string TipoContato { get; set; }
        public string Contato { get; set; }
    }
}