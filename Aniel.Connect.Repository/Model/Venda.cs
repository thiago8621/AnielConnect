using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.Model
{
    public enum ContratoEnum
    {
        Novo = 0,
        Existente = 1
    }

    public class VendaDadosGeraisViewModel : IViewModel<Int64>
    {
        private IEnumerable<EtapaVendaViewModel> _etapasVenda = null;

        public VendaDadosGeraisViewModel()
        {
            EnderecoPrincipal = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Principal };
            EnderecoInstalacao = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Instalacao };
            EnderecoCobranca = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Cobranca };
            EnderecoProspecto = new EnderecoClienteViewModel { Tipo = Tipo_EnderecoCliente.Prospecto };
        }

        #region Atualizacoes projeto SaperX

        #region Plano Avulso
        public virtual Sinapse.Model.enumerados.Padrao Venda_Campanha { get; set; }
        public virtual Sinapse.Model.enumerados.Padrao TrocaPlanoEnum { get; set; }
        public virtual ContratoEnum Contrato { get; set; }
        public virtual String VendaConcluida { get; set; }
        public virtual String Formato_Desconto { get; set; }
        public virtual Formato_Venda Formato_Venda { get; set; }
        //public virtual Int64? Id_PlanoSelecionado { get; set;  }
        public virtual Int64? Multa_CarenciaPlano { get; set; }
        public virtual Int64? Renovacao_AutomaticaPlano { get; set; }
        #endregion

        public Int64? IdWorkFLowEscolhido { get; set; }
        public Int64? Id_Plano_Antigo { get; set; }
        public Int64? Id_Plano_Antigo_InsersaoDataTable { get; set; }
        public long? Campanha_Id_Uf { get; set; }

        #region Produto
        public virtual Int64? Multa_CarenciaProduto { get; set; }
        public virtual Int64? Renovacao_AutomaticaProduto { get; set; }
        #endregion

        #endregion

        public Int64 Id { get; set; }
        public Int64 Id_Cliente { get; set; }

        public Int64 Id_Forma_Pagamento_Cliente_Preferida { get; set; }

        //DADOS GERAIS****************************************************************************************************

        public String Tempo_Ultimo_Contato { get; set; }
        public String StatusVenda { get; set; }
        public virtual String Descricao_WF { get; set; }
        public virtual Int32? Prazo_Atividade { get; set; }
        public virtual Atividade_Decisoria? Atividade_Decisoria { get; set; }
        public virtual string Decorrido_Atividade { get; set; }
        public virtual Int64? Prazo_Workflow_Vendas { get; set; }
        public virtual string Decorrido_Workflow_Vendas { get; set; }
        public virtual Status_Venda? StatusAtividadeAprovada { get; set; }
        public virtual Status_Venda? StatusAtividadeReprovada { get; set; }
        public virtual EquipeVenda EquipeVendas { get; set; }
        public virtual string Porta_Reservada { get; set; }
        public virtual Int64? Id_Workflow_Vendas { get; set; }
        public virtual Workflow_VendasAtividade Workflow_VendasAtividade { get; set; }
        public virtual Workflow_Vendas Workflow_Vendas { get; set; }
        public Int64? Id_Workflow_Atividade { get; set; }
        public bool End_Insta_Mesmo_Princ {  get; set; }

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
        public Boolean OrigemMobile { get; set; }

        #region Fisico

        [Display(Name = "DATA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String DataVenda { get; set; }

        public String DataCriacao { get; set; }
        public String DataEncerramento { get; set; }
        public StatusAuditoria? StatusAuditoria { get; set; }

        [Display(Name = "ULTIMO_CONTATO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public DateTime? Ultimo_Contato { get; set; }

        [Display(Name = "TIPO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        //[DataType(DataType.Text)]
        public Tipo_Cliente? Tipo_Cliente { get; set; }


        [Display(Name = "SUPERVISOR", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Id_Supervisor { get; set; }

        [Display(Name = "VENDEDOR", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Id_Vendedor { get; set; }

        public Int64? Id_Departamento { get; set; }
        public String Id_Setor_Departamento { get; set; }
        public String Id_Responsavel { get; set; }
        public Int64? Id_Campanha_Venda { get; set; }

        [Display(Name = "SUPERVISOR", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Nome_Supervisor { get; set; }



        [Display(Name = "VENDEDOR", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Nome_Vendedor { get; set; }


        public Tipo_Pessoa? TipoPessoa { get; set; }

        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String CpfCnpj { get; set; }

        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Meio_Captacao { get; set; }

        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Meio_Captacao { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public Status_Venda Status { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Descricao_Status_Venda { get; set; }


        /// <summary>
        ///     Lista com as etapas da venda habilitadas de acordo com as configurações da empresa
        /// </summary>
        public IEnumerable<EtapaVendaViewModel> EtapasVenda
        {
            get
            {
                if (this._etapasVenda == null)
                    this._etapasVenda = new List<EtapaVendaViewModel>();

                return this._etapasVenda;
            }
            set
            {
                this._etapasVenda = value;
            }
        }

        /// <summary>
        ///     Última etapa da venda que ainda não foi aprovada
        /// </summary>
        public int EtapaAtualVenda { get; set; }

        public Status_Venda Status_Liberacao { get; set; }


        [Display(Name = "NOMECOMPLETO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String NomeCompleto { get; set; }



        //public Boolean End_Cobranca_Igual_Principal { get; set; }
        public Boolean End_Instalacao_Igual_Principal { get; set; }
        public Boolean End_Instalacao_Igual_Prospecto { get; set; }
        public Boolean End_Cobranca_Igual_Prospecto { get; set; }
        public Boolean End_CadastroCondominioPrincipal { get; set; }
        public Boolean End_CadastroCondominioCobranca { get; set; }
        public Boolean End_CadastroCondominioInstalacao { get; set; }
        public Boolean End_CadastroCondominioProspecto { get; set; }

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

        //
        //E PRODUTOS****************************************************************************************************
        #region

        public Int64 Id_Venda { get; set; }

        private List<VendaDadosGeraisViewModel> VendasLista;

        public virtual List<VendaDadosGeraisViewModel> Vendas

        {
            get
            {
                if (VendasLista == null)
                    VendasLista = new List<VendaDadosGeraisViewModel>();
                return VendasLista;
            }
            set
            {
                VendasLista = value;
            }
        }


        [Display(Name = "QUAL_MEIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Qual_Meio { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Quem_Indicou { get; set; }
        public string Encerra_Carencia { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Id_Quem_Indicou { get; set; }

        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Categoria_Cliente { get; set; }
        public String  DescCategoriaCliente { get; set; }

        [Display(Name = "VALOR_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Valor_Final { get; set; }

        public string Valor_Real { get; set; }
        public string Valor_Instalacao { get; set; }

        [Display(Name = "VALOR_INSTALACAO_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Valor_Instalacao_Final { get; set; }


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

        public Int64? Quantidade { get; set; }
        public String Valor_Total_Plano { get; set; }
        public Int64? Parcelas { get; set; }
        public String Valor_Entrada { get; set; }
        #endregion

        //MODAL RADIUS****************************************************************************************************
        #region

        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Login { get; set; }

        [Display(Name = "SENHA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Senha { get; set; }

        [Display(Name = "CONFIRMARSENHA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Confirmar_Senha { get; set; }
        public Localidade_Anatel? Localidade_Anatel { get; set; }

        #endregion

        //MODAL POOL IP****************************************************************************************************
        public Int64? Id_Pool_Ip { get; set; }
        public Int64? Id_Pool_Ip_Endereco { get; set; }

        //Combo(Telefonia)***********************************************************************************************
        #region Telefonia
        public virtual long? servico_Fixo { get; set; }
        public virtual long? servico_Movel { get; set; }
        public virtual long? servico_Adicional { get; set; }
        public virtual long? Vinculo_ServicoAdicional { get; set; }
        public virtual long? servico_Internet { get; set; }
        public virtual long? servico_TV { get; set; }
        public virtual String servico_Fixo_desc { get; set; }
        public virtual String servico_Movel_desc { get; set; }
        public virtual String servico_Adicional_desc { get; set; }
        public virtual String servico_Internet_desc { get; set; }
        public virtual String servico_TV_desc { get; set; }
        #endregion
        //FINCANCEIRO****************************************************************************************************
        #region  


        public Int64? Id_Motivo_Desistencia_Venda { get; set; }
        public Int64? Id_Motivo_Aprovacao_Venda { get; set; }
        public Int64? Id_Motivo_Aprovacao_Restricao { get; set; }


        public string Numero_ContratoVinculado { get; set; }
        public Int64? Id_Desconto { get; set; }

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

        public virtual Forma_Pagamento_ClienteModels FormaPagamentoVenda { get; set; }

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

        [Display(Name = "FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsFinanceiro))]
        public Int64? Id_Forma_Pagamento { get; set; }

        [Display(Name = "VENCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Vencimento { get; set; }

        [Display(Name = "BANCO", ResourceType = typeof(Resources.RsCadastros))]
        public String Desc_Banco { get; set; }

        [Display(Name = "BANCO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Banco { get; set; }

        [Display(Name = "CONTA_CORRENTE", ResourceType = typeof(Resources.RsCadastros))]
        public String Conta_Corrente { get; set; }

        [Display(Name = "AGENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public String Agencia { get; set; }

        [Display(Name = "DIGITO", ResourceType = typeof(Resources.RsCadastros)),
       MaxLength(1, ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "MAXLENGHT")]
        public String Digito { get; set; }

        public Cliente_FormaEnvio Forma_Envio { get; set; } = Cliente_FormaEnvio.Email;
        public  Boolean End_Cobranca_Igual_Principal { get; set; }

        #endregion

        /// <summary>
        /// Utilizado para preencimento da grid de planos
        /// </summary>
        public Int64? Id_Plano { get; set; }


        public Int64? Id_Combo { get; set; }

        public Int64? Id_Campanha { get; set; }
        public Int64? IdAtendimento { get; set; }
        public Int64? IdVenda { get; set; }
        public Connect.Model.nhibernate.Atendimento Data_Hora_Fechamento { get; set; }

        public Boolean Termo_Adesao_Assinado { get; set; }
        public Boolean Mudanca_Plano { get; set; }
        public Boolean PossuiChecklist { get; set; }
        public Perfil_Vendedor Perfil_Vendedor { get; set; }

        public Boolean Exige_Termo_Adesao { get; set; }
        public Boolean Exige_Termo_Contrato { get; set; }
        public Boolean Utiliza_Envio_Automatico_Contrato { get; set; }
        /// <summary>
        ///  Utilizado na consulta de viabilidade
        /// </summary>
        public Int64? Id_Caixa_Mais_Proxima { get; set; }
        public Int64? Id_Caixa { get; set; }
        public Int64? Id_Porta { get; set; }
        public Boolean Consulta_Viabilidade_Realizada { get; set; }

        public bool EtapasVendasConcluidas { get; set; }

        public bool AprovarEtapaWorkflowAprovaVenda { get; set; }
        public bool ReprovarEtapaWorkflowCancelaVenda { get; set; }

        public Boolean Exige_Foto_Termo { get; set; }
        public Boolean Permite_Work_Sem_Termo { get; set; }
        public Sinapse.Model.enumerados.StatusEtapaVenda StatusConsultaCredito { get; set; }
        public Sinapse.Model.enumerados.StatusEtapaVenda StatusConsultaViabilidade { get; set; }
        public Sinapse.Model.enumerados.StatusEtapaVenda StatusConsultaPendenciaFinanceira { get; set; }
        public Sinapse.Model.enumerados.StatusEtapaVenda StatusConsultaPendenciaEndereco { get; set; }

        public bool ConsultaCreditoRestringeVenda { get; set; }
        public bool ConsultaViabilidadeRestringeVenda { get; set; }
        public bool ConsultaPendenciaFinanceiraRestringeVenda { get; set; }
        public bool ConsultaPendenciaEnderecoRestringeVenda { get; set; }

        public bool ConsultaCreditoVendedorVisualiza { get; set; }
        public bool ConsultaViabilidadeVendedorVisualiza { get; set; }
        public bool ConsultaPendenciaFinanceiraVendedorVisualiza { get; set; }
        public bool ConsultaPendenciaEnderecoVendedorVisualiza { get; set; }

        public List<DocTermoAnexo> TermosAceite { get; set; }
        public virtual Tipo_Doc_Termo_Anexo TipoDocumento { get; set; }
        public virtual Status_Doc_Aceite Status_Doc_Aceite { get; set; }
        public virtual Lado_Doc_Aceite Lado { get; set; }
        public virtual string Url { get; set; }
        public virtual string InformacoesComplementares { get; set; }
        public virtual bool VinculosContratos { get; set; }
        public virtual bool ProximaEtapa { get; set; }
        public long? EtapaIntegracao { get; set; }
    }

    public class ClienteContratoListModel
    {
        public Int64 Id { get; set; }
        public String Descricao { get; set; }
        public String Numero_Contrato { get; set; }
    }

    public class PlanoProdutoListModel
    {
        public long? Id_PlanoMudanca { get; set; }
        public long? Id_MudancaPlano { get; set; }
        public long Id { get; set; }
        public long? Id_Plano_Produto { get; set; }
        public long? Id_Troca_Plano { get; set; }
        public Status_PlanoProduto? Status_PlanoProduto { get; set; }
        public Status_PlanoProduto? Status { get; set; } // ?
        public string Status_PlanoProduto_Str { get; set; }
        public virtual Formato_Venda Formato { get; set; }
        public virtual Boolean FlagPossuiValorAgrupado { get; set; }
        public virtual Boolean UtilizaVendaAgrupada { get; set; }
        public virtual String ValorVendaAgrupada { get; set; }
        public virtual String ValorPlanoOriginal { get; set; }
        public long? Id_Venda { get; set; }
        public long? Id_Combo { get; set; }
        public bool Combo { get; set; }
        public Status_Venda? Status_Venda { get; set; }
        public string Desc_Status_Venda { get; set; }
        public virtual bool Tipo_Tec { get; set; }
        public Tipo_Tecnologia? Tipo_Tecnologia { get; set; }

        public long? Id_Vendedor { get; set; }
        public string Nome_Vendedor { get; set; }

        public long? Id_Supervisor { get; set; }
        public string Nome_Supervisor { get; set; }
        public String ServicosAdicionais { get; set; }
        public long? Id_Onu { get; set; }

        public long? Id_Forma_Pagamento_Cliente { get; set; }
        public long? Id_Forma_De_Pagamento { get; set; }
        public string Desc_Forma_Pagto { get; set; }

        public string Produto_Plano { get; set; }
        public Regime_Anatel? Regime { get; set; }
        public Modelo_Anatel? Modelo_Anatel { get; set; }

        public string Cod_Area { get; set; }
        public string Desc_Area { get; set; }

        public string Cod_Escritorio { get; set; }
        public string Desc_Escritorio { get; set; }

        public int? Cod_Localidade { get; set; }
        public string Localidade { get; set; }
        public Localidade_Anatel? Localidade_Anatel { get; set; }

        /// <summary>
        ///     Id do modelo de contrato para novos planos
        /// </summary>
        public long? Id_Modelo_Contrato { get; set; }
        /// <summary>
        ///     Id da TB_CLIENTE_CONTRATO para planos já gravados
        /// </summary>
        public long? Id_Contrato { get; set; }
        public string Descricao_Contrato { get; set; }
        public string Numero_Contrato { get; set; }
        //public string SequenciaTMP { get; set; }

        public string Nas { get; set; }
        public string AutenticacaoRadius { get; set; }
        public string Detalhes { get; set; }

        public string VelocidadeDownload { get; set; }
        public string VelocidadeDownloadReduzida { get; set; }
        public string VelocidadeUploadReduzida { get; set; }
        public string VelocidadeUpload { get; set; }
        public string VelocidadeNominal { get; set; }

        public bool Velocidade_Reduzida { get; set; }

        public long? Id_Tecnologia { get; set; }
        public string Nome_NAS { get; set; }
        public string IP { get; set; }


        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string ValorTotal { get; set; }

        public long? Numero_Parcelas { get; set; }
        public bool CobrarInstalacaoEntrada { get; set; }
        public Momento_Cobra_Instalacao? Momento_Cobra_Instalacao { get; set; }
        public string Momento_Cobra_Instalacao_Str { get; set; }
        public long? Qtde_Dias { get; set; }

        public bool Periodo_Carencia { get; set; }
        public string Desc_Periodo_Carencia { get; set; }
        public long? Meses_Carencia { get; set; }

        //public bool Multa_Carencia { get; set; }
        public string Multa_Carencia_Reais { get; set; }
        public string CancelaContratoFimCarencia { get; set; }
        public string Renovar_CarenciaAuto { get; set; }
        public string Tem_Reajuste { get; set; }
        public string Frequencia_Reajuste { get; set; }

        public bool Teste_Gratis { get; set; }
        public long? Id_Campanha { get; set; }
        public string DataValidade { get; set; }
        public string BloqueioAutomatico { get; set; }

        public string ValorInstalacao { get; set; }
        public string ValorInstalacaoEntrada { get; set; }
        public string ValorInstalacao_Final { get; set; }
        public string ValorPlanoProduto { get; set; }
        public string ValorBina { get; set; }
        public virtual Boolean CobrarBina { get; set; }
        public virtual Boolean CobrarSigame { get; set; }
        public virtual Boolean CobrarBlacklist { get; set; }
        public virtual Int32? Quantidade { get; set; }
        public virtual String Valor_Total_Plano { get; set; }
        public virtual Int32? Parcela { get; set; }
        public virtual String Valor_Entrada { get; set; }
        public virtual String ValorBlacklist { get; set; }
        public virtual Boolean CobrarBloqueioChamadaCobrar { get; set; }
        public string ValorPlanoProduto_Final { get; set; }
        public virtual DidFormatoCobrar? BloqueioChamadaCobrar { get; set; }
        public virtual String ValorBloqueioChamadaCobrar { get; set; }


        //radius

        public string Login_Radius { get; set; }
        public string Senha_Radius { get; set; }
        public Int64? IdIpPool { get; set; }
        public Int64? Id_Pool_Ip_Endereco { get; set; }

        public bool End_Instalacao_Igual_Principal { get; set; }
        public bool End_Instalacao_Igual_Prospecto { get; set; }
        public PlanoProdutoEnum? Tipo { get; set; }
        public String JsonTelefonia { get; set; }
        public bool Utilizar_ServicoSelecionado { get; set; }
        public bool IsAlterado { get; set; }
        public string NecessitaCriarOS { get; set; }
        public string PossuiOSCriada { get; set; }

        public string NumOS { get; set; }
        public string DiasInadimplente { get; set; }
        public string DiasBloqueado { get; set; }

        public bool ReducaoVelocidade { get; set; }
        public bool DesbloqueioConfianca { get; set; }
        public bool PossuiDesbloqueioConfiancaRecente { get; set; }
        //plano adicional
        public bool isPlanoAdicional { get; set; }
        public bool possuiPlanosAdicionais { get; set; }
        public long? Id_Plano_Atrelado { get; set; }

        #region Endereco

        public long Id_Endereco { get; set; }

        public long? Id_Tipo_Imovel { get; set; }
        public long? Id_Pais { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string CEP { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public long? Id_Bairro { get; set; }
        public string Cidade { get; set; }
        public long? Id_Cidade { get; set; }
        public string UF { get; set; }
        public string Complemento { get; set; }
        public string Ponto_Referencia { get; set; }

        #endregion

        #region Informações Técnicas

        public string Login { get; set; }
        public string Senha { get; set; }
        public string IdConcentradora { get; set; }
        public string VLAN { get; set; }

        public string Olt { get; set; }
        public string Slot { get; set; }
        public string ONU { get; set; }
        public string PON { get; set; }
        public string Caixa_Ftth_Reservado { get; set; }
        public string Porta_Ftth_Reservado { get; set; }
        public string Data_Expiracao_Reserva { get; set; }


        #endregion

        #region campos adicionais  do plano
        public virtual Boolean BloqueioAutomatico_Debito { get; set; }
        public virtual Int16 QtdDiasbloqueados { get; set; }
        public Boolean ImpedirBloqueio_Automatico { get; set; }
        public Boolean ImpedirBloqueio_Sexta { get; set; }
        public Boolean ImpedirBloqueio_Sabado { get; set; }
        public Boolean ImpedirBloqueio_Domingo { get; set; }
        public Boolean ImpedirBloqueio_Feriados { get; set; }
        public Int16 QtdDiasAtraso { get; set; }


        public Boolean Reducao_Velocidade { get; set; }
        public String Velocidade_DownloadReduzida { get; set; }
        public String Velocidade_UploadReduzida { get; set; }
        public Frequencia_Desbloqueio? Frequencia_Desbloqueio { get; set; }
        public Int16 dias_desbloq { get; set; }
        public Boolean Desbloqueio_Conf { get; set; }
        public Boolean Disponibiliza_Telefonia { get; set; }
        public Boolean UtilizaVendaAgroup { get; set; }
        #endregion

        public bool FlagTrocaPlano { get; set; }
        public bool FlagVendaFormatoAvulso { get; set; }
    }
    public class AtividadeVendaRequestModel
    {
        public long? Id { get; set; }
        public string Descricao { get; set; }
        public long? AtividadeReprovada { get; set; }
        public short? Status { get; set; }
}
    public class ReprovacaoAutomaticaAtividadeModel
    {
        public long Venda { get; set; }
        public long Atividade { get; set; }
    }
    public class ReprovacaoAutomaticaComandos 
    {
        public IEnumerable<StringBuilder> ComandosSql { get; set; }
        public string  Usuario { get; set; }
        public string  Nome { get; set; }
        public long  Venda { get; set; }
        public Queue<long> Atividades { get; set; }
    }

    public class VerificaWorkflowRequestModel
    {
        public virtual String TipoPessoa { get; set; }
        public virtual String UF { get; set; }
        public virtual String Categoria { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Captacao { get; set; }


    }    
    public class PainelVendasFiltrosDashBoard
    {
        private List<Status_Venda> _StatusFiltrados;
        public long? Id { get; set; }
        public bool Supervisor { get; set; }       
        public bool ApenasInstalados { get; set; }
        public string DataInicial { get; set; }
        public string DataAtual { get; set; }
        public List<Status_Venda> StatusFiltrados { 
            get
            {
                if (_StatusFiltrados == null)
                    _StatusFiltrados = new List<Status_Venda>();
                return _StatusFiltrados;
            }
         
            set { _StatusFiltrados = value; } 
        }
    }
}
