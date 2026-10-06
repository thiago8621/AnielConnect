using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Newtonsoft.Json;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Aniel.Connect.Models
{

    public class PlanoComboResult : IObjComboResult<long>
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string Upload { get; set; }
        public string Download { get; set; }
    }

    public class PlanoProdutoExcluirModel
    {
        public string id { get; set; }
        public string excluirId { get; set;}
    }
    public class ClientePlanoProdutoComboResult : IObjComboResult<long>
    {
        public long Id { get; set; }
        public long Plano { get; set; }
        public string Text { get; set; }
        public string Upload { get; set; }
        public string Download { get; set; }
        public long? IdClienteContrato { get; set; }
    }

    public class PlanoProdutoIndexModel
    {
        public long Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Descricao { get; set; }

        public string Tipo { get; set; }

        public string Situacao { get; set; }

        public string Detalhes { get; set; }


    }

    public class PlanoCadastroModel : IViewModel<long>
    {

        private List<PlanoMensalidadeCadastroModel> _mensalidades;
        private List<MaterialosCadastroModel> _materialOs;
        private List<GetDadosPlanoProdutoModel> _contratoPlano;

        private List<PlanoProdutoConfigNfCadastroModel> _configuracaoNF;
        private List<PlanoProdutoConfigNfCadastroModel> _configuracaoNFInstalacao;
        private List<PlanoProdutosCompCadastroModel> _planoProdutosComp;

        public long Id { get; set; }
        public virtual String Objeto_In_Json { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Codigo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "DESCRICAOPLANO", ResourceType = typeof(Resources.RsConfiguracao))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Descricao { get; set; }
        public string Nome_Venda { get; set; }

        [Display(Name = "DETALHES", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Detalhes { get; set; }


        [Display(Name = "AUTENTICACAORADIUS", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool AutenticacaoRadius { get; set; }
        public long? IdFormaPagamentoCob { get; set; }
        public virtual bool AtributoIPOE { get; set; }

        #region Atributos Radius Huawei
        [Display(Name = "UTILIZA_QOS_PROFILE", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool UtilizaQoSProfile { get; set; }

        [Display(Name = "QOS_PROFILE_ID", ResourceType = typeof(Resources.RsCadastros))]
        public string IdQoSProfile { get; set; }

        [Display(Name = "QOS_PROFILE_REDUZIDO_ID", ResourceType = typeof(Resources.RsCadastros))]
        public string IdQoSProfile_Reduzido { get; set; }

        [Display(Name = "BLOQUEIO_DOMAIN_NAME", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool UtilizaBloqueioDomainName { get; set; }

        [Display(Name = "DOMAIN_NAME_ID", ResourceType = typeof(Resources.RsCadastros))]
        public string IdDomainName { get; set; }
        #endregion

        #region Atributos Radius Juniper
        //[Display(Name = "UTILIZA_POLICY_NAME", ResourceType = typeof(Resources.RsConfiguracao))]
        //public bool UtilizaPolicyName { get; set; }

        [Display(Name = "POLICY_NAME_DOWNLOAD_ID", ResourceType = typeof(Resources.RsCadastros))]
        public string IdPolicyNameDownload { get; set; }

        [Display(Name = "POLICY_NAME_DOWNLOAD_ID_REDUZIDO", ResourceType = typeof(Resources.RsCadastros))]
        public string IdPolicyNameDownload_Reduzido { get; set; }

        [Display(Name = "POLICY_NAME_UPLOAD_ID", ResourceType = typeof(Resources.RsCadastros))]
        public string IdPolicyNameUpload { get; set; }

        [Display(Name = "POLICY_NAME_UPLOAD_ID_REDUZIDO", ResourceType = typeof(Resources.RsCadastros))]
        public string IdPolicyNameUpload_Reduzido { get; set; }

        [Display(Name = "UTILIZA_POLICY_NAME_IPV6", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool UtilizaPolicyNameIPv6 { get; set; }
        #endregion


        [Display(Name = "BLOQUEIO_IP_POOL", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool UtilizaBloqueioPoolIp { get; set; }

        [Display(Name = "VELOCIDADENOMINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeNominal { get; set; }


        [Display(Name = "VELOCIDADEDOWNLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeDownload { get; set; }


        [Display(Name = "VELOCIDADEUPLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeUpload { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "VALORPLANO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string ValorPlano { get; set; }

        [Display(Name = "COBRANCAINSTALACAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool CobrancaInstalacao { get; set; }

        [Display(Name = "COBRANCAINSTALACAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool CobrancaInstalacaoEntrada { get; set; }

        [Display(Name = "VALORINSTALACAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string ValorInstalacao { get; set; }

        [Display(Name = "MODOCOBRANCAINSTALACAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Modo_Cobranca_Instalacao? ModoCobrancaInstalacao { get; set; }
        public Modo_Cobranca_Instalacao? ModoCobranca { get; set; }

        //[Display(Name = "QTDPARCELAS", ResourceType = typeof(Resources.RsConfiguracao))]
        public Int16? QtdParcelas { get; set; }

        //Novo Incluido em 02/04/2019
        [Display(Name = "OSTROCAENDERECO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool OSTrocaEndereco { get; set; }

        [Display(Name = "REDUCAOVELOCIDADE", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool ReducaoVelocidade { get; set; }

        //[Display(Name = "DESBLOQUEIO_CONFIANCA", ResourceType = typeof(Resources.RsConfiguracao))]
        //public bool Desbloqueio_Conf { get; set; }

        //public Int16 dias_desbloq { get; set; }

        //[Display(Name = "FREQUENCIA_DESBLOQUEIO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public Frequencia_Desbloqueio? Frequencia_Desbloqueio { get; set; }
        public bool Disponibiliza_Telefonia { get; set; }

        [Display(Name = "BLOQUEIO_AUTO_DEBITO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool BLOQUEIO_AUTO_DEBITO { get; set; }
        //public bool Cobranca_Recorrente { get; set; }

        public bool Plano_Comercializavel { get; set; }

        public string CodigoTipoPlanoIntegracao { get; set; }
        public string CodigoIntegracao { get; set; }
        public string CodigoIntegracaoAuxiliar { get; set; }

        public Momento_Cobranca? Momento_Cobranca { get; set; }

        public Cobranca_Realizada? Cobranca_Realizada { get; set; }

        public Int64? Dias_Vencimento { get; set; }

        public virtual Boolean ProdutoQtdVariavel { get; set; }
        public virtual Int32? QtdParcelas_ProdutoVariavel { get; set; }
        public virtual Boolean PossuiValorEntrada { get; set; }
        public Int16 QtdDiasbloqueados { get; set; }


        [Display(Name = "QTDDIASATRASO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Int16 QtdDiasAtraso { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeDownloadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeUploadReduzida { get; set; }

        public long? Id_Tecnologia { get; set; }

        [Display(Name = "TIPO_TECNOLOGIA", ResourceType = typeof(Resources.RsCadastros))]
        public Tipo_Tecnologia? Tipo_Tecnologia { get; set; }

        [Display(Name = "TIPO", ResourceType = typeof(Resources.RsCadastros))]
        public Tipo_Plano_Produto? Tipo { get; set; }

        [Display(Name = "SITUACAOPLANO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool SituacaoPlano { get; set; }

        [Display(Name = "ATIVARVALIDADE", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool AtivarValidade { get; set; }

        [Display(Name = "VALIDADE", ResourceType = typeof(Resources.RsConfiguracao))]
        public DateTime? Validade { get; set; }

        //[Display(Name = "TESTEGRATIS", ResourceType = typeof(Resources.RsConfiguracao))]
        //public bool TesteGratis { get; set; }

        //[Display(Name = "DIASTESTE", ResourceType = typeof(Resources.RsConfiguracao))]
        //public Int16? DiasTeste { get; set; }

        [Display(Name = "PRORATA", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool ProRata { get; set; }

        public bool Pro_Rata_Habilitacao { get; set; }
        public bool Pro_Rata_Interrupcao { get; set; }
        public bool Pro_Rata_Cancelamento { get; set; }

        public long? Id_ContaFinanceiraInstalacao { get; set; }

        [Display(Name = "CONTAFINANCEIRA", ResourceType = typeof(Resources.RsConfiguracao))]
        public string DescContaFinanceiraInstalacao { get; set; }

        public long? Id_ContaFinanceiraPlano { get; set; }

        [Display(Name = "CONTAFINANCEIRA", ResourceType = typeof(Resources.RsConfiguracao))]
        public string DescContaFinanceiraPlano { get; set; }

        [Display(Name = "FREQUENCIACOBRANCA", ResourceType = typeof(Resources.RsConfiguracao))]
        public Frequencia_Cobranca? FrequenciaCobranca { get; set; }
        public Sinapse.Model.enumerados.Padrao? Cobranca_Recorrente { get; set; }
        public Momento_Cobranca_Plano? Momento_Cobranca_Plano { get; set; }


        public long? Id_Recibo { get; set; }
        public long? Id_ReciboInstalacaco { get; set; }
        public string Desc_Recibo { get; set; }


        [Display(Name = "COMPROVANTEPAGAMENTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Comprovante_Pagamento? ComprovantePagamento { get; set; }

        [Display(Name = "COMPROVANTEPAGAMENTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Comprovante_Pagamento? ComprovantePagamentoFinanceiro { get; set; }
        public Momento_Cobra_Instalacao? Momento_Cobra_Instalacao { get; set; }

        public Modelo_Anatel? Modelo_Anatel { get; set; }

        public Tecnologia_Anatel? Tecnologia_Anatel { get; set; }
        public Tipo_Produto_Anatel? Tipo_Produto { get; set; }
        public Tipo_Produto_AnatelSCM? Tipo_ProdutoScm { get; set; }
        public Meio_Acesso_Anatel? Meio_Acesso_Anatel { get; set; }
        public Regime_Anatel? Regime { get; set; }
        public Tarifa_Anatel? Tarifa_Anatel { get; set; }
        public Modalidade_Plano? Modalidade_Anatel { get; set; }
        [Display(Name = "MODELONOTAFISCAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public Modelo_NotaFiscal? ModeloNotaFiscal { get; set; }
        public Modelo_NotaFiscal? ModeloNotaFiscalInstalacao { get; set; }

        public long? Id_Radius { get; set; }
        public long? NomeIPOE { get; set; }
        public long? Id_Cfop { get; set; }
        public long? Id_Cfop_Instalacao { get; set; }

        [Display(Name = "CODIGOCFOP", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Codigo_Cfop { get; set; }

        [Display(Name = "DESCRICAOCFOP", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Descricao_Cfop { get; set; }

        [Display(Name = "FORMATOCOBRANCA", ResourceType = typeof(Resources.RsConfiguracao))]
        public Formato_Cobranca? FormatoCobranca { get; set; }

        //Novo Incluido em 14/03/2019
        [Display(Name = "ATRIBUIRVALORINTEGRAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool AtribuirValorIntegral { get; set; }

        //[Display(Name = "DESCONTOANTECIPADO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public bool DescontoAntecipado { get; set; }

        //[Display(Name = "QTDDIASVENCTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public Int16? QtdDiasVencto { get; set; }

        //[Display(Name = "PERCENTUALDESCONTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public string PercentualDescontoAntecipado { get; set; }

        //[Display(Name = "VALORDESCONTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public string ValorDescontoAntecipado { get; set; }

        //[Display(Name = "DESCONTOPROGRESSIVO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public bool DescontoProgressivo { get; set; }

        public virtual bool Utiliza_Desconto { get; set; }
        public virtual long? Desconto { get; set; }


        //Novo Incluido em 02/04/2019
        [Display(Name = "RETENCAOIMPOSTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool RetecaoImposto { get; set; }
        public bool RetecaoImposto_Instalacao { get; set; }

        [Display(Name = "PERCENTUALDESCONTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string PercentualDescontoProgressivo { get; set; }

        [Display(Name = "VALORDESCONTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string ValorDescontoProgressivo { get; set; }

        //Novo Incluido em 14/03/2019
        [Display(Name = "MENSALIDADEINICIAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public string MensalidadeInicial { get; set; }

        [Display(Name = "MENSALIDADEFINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public string MensalidadeFinal { get; set; }

        [Display(Name = "EMITIRCONTRATO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool EmitirContrato { get; set; }
        public bool TemFidelidade { get; set; }

        public long? IdModeloContrato { get; set; }

        [Display(Name = "MODELOCONTRATO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string DescModeloContrato { get; set; }
        public bool Tem_Carencia { get; set; }

        public Int64? Periodo_Carencia { get; set; }


        //Migrar Bloqueio Automático para o Modelo de Contrato 


        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                     ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "STATUSINICIAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public Status_PlanoProduto? StatusInicial { get; set; }

        public Boolean Os_Mudanca_Plano { get; set; }
        public long? Id_Tipo_Servico { get; set; }
        public long? Id_Subtipo_Servico { get; set; }
        public Boolean DefinirTipo_SubTipoOS { get; set; }
        public long? Id_Tipo_ServicoOS { get; set; }
        public long? Id_Subtipo_ServicoOS { get; set; }
        public Boolean UsaPlanoAdicional { get; set; }
        public String Material { get; set; }
        public String PlanoProduto { get; set; }
        public virtual Boolean PossuiFranquia { get; set; }
        public virtual Boolean PossuiConsumoMinimo { get; set; }
        public virtual String ConsumoMinimo { get; set; }
        public virtual Int64? IdFranquia { get; set; }
        public virtual FormatoTarifaTelefonia? FormatoTarifaTelefonia { get; set; }
        public virtual Formato_Plano Formato_do_plano { get; set; }
        public virtual bool Utiliza_Integraca_Externa { get; set; }

        [Display(Name = "INTEGRACAO_EXTERNA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual IntegracaoExterna Integracao { get; set; }
        public virtual long Num_Dids { get; set; }
        public virtual Circuito_Did_Cobranca Cobrar_Por_Telefonia { get; set; }
        public virtual Boolean HabilitarLc { get; set; }
        public virtual String ValorLc { get; set; }
        public virtual String CadenciaLc { get; set; }
        public virtual Boolean HabilitarLdi { get; set; }
        public virtual String ValorLdi { get; set; }
        public virtual String CadenciaLdi { get; set; }
        public virtual Boolean HabilitarLdn { get; set; }
        public virtual String ValorLdn { get; set; }
        public virtual String CadenciaLdn { get; set; }
        public virtual Int64? MultiplicadorInternacional { get; set; }
        public virtual String CadenciaInternacional { get; set; }
        public virtual Boolean HabilitarVc1 { get; set; }
        public virtual String ValorVc1 { get; set; }
        public virtual String CadenciaVc1 { get; set; }
        public virtual Boolean HabilitarVc2 { get; set; }
        public virtual String ValorVc2 { get; set; }
        public virtual String CadenciaVc2 { get; set; }
        public virtual Boolean HabilitarVc3 { get; set; }
        public virtual String ValorVc3 { get; set; }
        public virtual String CadenciaVc3 { get; set; }
        public virtual Boolean IntragrupoLcGratuita { get; set; }
        public virtual Boolean IntragrupoGratuita { get; set; }
        public virtual Boolean CobrarBina { get; set; }
        public virtual String ValorBina { get; set; }
        public virtual Boolean CobrarSigame { get; set; }
        public virtual String ValorSigame { get; set; }
        public virtual Boolean CobrarBlacklist { get; set; }
        public virtual String ValorBlacklist { get; set; }
        public virtual Boolean CobrarBloqueioChamadaCobrar { get; set; }
        public virtual String ValorBloqueioChamadaCobrar { get; set; }
        public virtual DidFormatoCobrar? BloqueioChamadaCobrar { get; set; }
        public virtual Int64? FormaPagamentoCobranca { get; set; }
        public virtual int DiasBoleto { get; set; }
        public string ProductId { get; set; }
        public virtual Boolean UtilizaVendaAgroup { get; set; }
        public string ValorVendaAgrupada { get; set; }

        // Fim Configuracao Plano Model

        //Composição do Plano
        public virtual Int64? IdProdutosComp { get; set; }
        public virtual Double? Percentual_Discriminado_Comp { get; set; }
        public virtual Double? Valor_Discriminado_Comp { get; set; }
        public virtual Int64? Id_Empresa { get; set; }
        public virtual Int64? Id_Filial { get; set; }
        public virtual Int64? Id_Classificacao_Tributaria { get; set; }
        public virtual Boolean DuplicarPlano { get; set; }
        public virtual bool ExclusivoCliente { get; set; }

        public List<PlanoMensalidadeCadastroModel> Mensalidades
        {
            get
            {
                if (_mensalidades == null)
                    _mensalidades = new List<PlanoMensalidadeCadastroModel>();
                return _mensalidades;
            }
            set
            {
                _mensalidades = value;
            }
        }
        public List<MaterialosCadastroModel> Material_Os
        {
            get
            {
                if (_materialOs == null)
                    _materialOs = new List<MaterialosCadastroModel>();
                return _materialOs;
            }
            set
            {
                _materialOs = value;
            }
        }

        private List<PlanosProdutosAdicionaisCadastroModel> _planosProdutosAdicionais;
        public List<PlanosProdutosAdicionaisCadastroModel> PlanosProdutosAdicionais
        {
            get
            {
                if (_planosProdutosAdicionais == null)
                    _planosProdutosAdicionais = new List<PlanosProdutosAdicionaisCadastroModel>();
                return _planosProdutosAdicionais;
            }
            set
            {
                _planosProdutosAdicionais = value;
            }
        }
        public List<GetDadosPlanoProdutoModel> ContratoPlano
        {
            get
            {
                if (_contratoPlano == null)
                    _contratoPlano = new List<GetDadosPlanoProdutoModel>();
                return _contratoPlano;
            }
            set
            {
                _contratoPlano = value;
            }
        }


        public List<PlanoProdutoConfigNfCadastroModel> ConfiguracaoNF
        {
            get
            {
                if (_configuracaoNF == null)
                    _configuracaoNF = new List<PlanoProdutoConfigNfCadastroModel>();
                return _configuracaoNF;
            }
            set
            {
                _configuracaoNF = value;
            }
        }

        public List<PlanoProdutoConfigNfCadastroModel> ConfiguracaoNFInstalacao
        {
            get
            {
                if (_configuracaoNFInstalacao == null)
                    _configuracaoNFInstalacao = new List<PlanoProdutoConfigNfCadastroModel>();
                return _configuracaoNFInstalacao;
            }
            set
            {
                _configuracaoNFInstalacao = value;
            }
        }

        public List<PlanoProdutosCompCadastroModel> PlanoProdutosComp
        {
            get
            {
                if (_planoProdutosComp == null)
                    _planoProdutosComp = new List<PlanoProdutosCompCadastroModel>();
                return _planoProdutosComp;
            }
            set
            {
                _planoProdutosComp = value;
            }
        }

        #region Propiedades Temporarias de Mensalidade e ConfigNf
        public long Id_Mensalidade { get; set; }
        public Int16? Parcela_Ini { get; set; }
        public Int16? Parcela_Fin { get; set; }
        public Double? Percentual_Desconto { get; set; }
        public Double? Valor_Desconto { get; set; }

        public long Id_ConfigNf { get; set; }
        public long? Id_Atividade { get; set; }
        public long? Id_Atividade_Instalacao { get; set; }
        public string Atividade { get; set; }
        public string Descricao_Plano { get; set; }
        public string Descricao_Produto { get; set; }
        public Double? Percentual_Imposto { get; set; }
        public Double? Valor_Discriminado { get; set; }
        public Double? Percentual_Discriminado { get; set; }
        public Tipo_Reajuste_Contratual Tipo_Discriminado { get; set; }

        public Double? Valor_Discriminado_Instalacao { get; set; }
        public Double? Percentual_Imposto_Instalacao { get; set; }
        public Double? Icms { get; set; }
        public Double? Icms_Instalacao { get; set; }
        public Double? Pis { get; set; }
        public Double? Pis_Instalacao { get; set; }
        public Double? Cofins { get; set; }
        public Double? Cofins_Instalacao { get; set; }
        public Double? Iss { get; set; }
        public Double? Iss_Instalacao { get; set; }
        #endregion


    }


    public class PlanoMensalidadeCadastroModel
    {
        public long? Id { get; set; }
        public string Parcela_Ini { get; set; }
        public string Parcela_Fin { get; set; }
        public string Percentual_Desconto { get; set; }
        public string Valor_Desconto { get; set; }
    }
    public class MaterialosCadastroModel
    {
        public long? Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }   
    
    public class PlanosProdutosAdicionaisCadastroModel
    {
        public long? Id { get; set; }
        public string Descricao { get; set; }
        public long? Id_Plano_Atrelado { get; set; }
        public long? Tipo { get; set; }
    }

    public class PlanoProdutoConfigNfCadastroModel
    {
        public long PlanoId { get; set; }
        public long Id { get; set; }

        public long? IdClassificacaoTributaria { get; set; }
        public string DescClassificacaoTributaria { get; set; }

        public Modelo_NotaFiscal? ModeloNotaFiscal { get; set; }
        public string DescModeloNotaFiscal { get; set; }

        public string ValorDiscriminado { get; set; }
        public string PercentualDiscriminado { get; set; }
        public long? Id_Produtos_Comp { get; set; }
        public string DescProduto { get; set; }
        public string ValorDiscriminadoNf { get; set; }
        public long? Id_Empresa { get; set; }
        public string DescEmpresa { get; set; }
        public long? Id_Filial { get; set; }
        public string DescFilial { get; set; }

    }

    public class ComposicaoNotaCliModels
    {
        public string DescricaoContrato { get; set; }
        public string DescricaoPlano { get; set; }
        public string DescricaoProdutoComp { get; set; }
        public string Valor { get; set; }
        public string ValorDiscriminadoNf { get; set; }
        public string PercentualRateio { get; set; }
        public string ModeloNota { get; set; }
        public string DescricaoEmpresa { get; set; }
        public string DescricaoFilial { get; set; }
        public string DescricaoClassificacao { get; set; }
        public long? ModeloNotaNum { get; set; }
        public long? IdProdutoComp { get; set; }
        public long? IdPlano { get; set; }
        public long? IdContrato { get; set; }    
        public long? IdConfigTributaria { get; set; }    
        public long? IdEmpresa { get; set; }
        public long? IdFilial { get; set; }
        public long? Id { get; set; }
        public bool Instalacao { get; set; }
        public string ValorFinalTotal { get; set; }
    }


    public class PlanoProdutosCompCadastroModel
    {
        public long Id { get; set; }
        public long IdProdutosComp { get; set; }
        public string DescricaoProdutosComp { get; set; }
        public string ValorFixo { get; set; }
        public string ValorPercentual { get; set; }
        public string ValorTotal { get; set; }
    }

    public class ProdutoCadastroModel : IViewModel<long>
    {
        private List<PlanoMensalidadeCadastroModel> _mensalidadesProd;
        private List<PlanoProdutoConfigNfCadastroModel> _configuracaoProdNF;

        public long Id { get; set; }

        /*********************
        * Configuração Produto
        *********************/

        public bool Utiliza_Desconto { get; set; }
        public long? Desconto { get; set; }

        #region 1º Bloco - Dados do Produto

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Codigo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "DESCRICAOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Descricao { get; set; }

        [Display(Name = "DETALHES", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Detalhes { get; set; }


        [Display(Name = "SITUACAOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool SituacaoProduto { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "STATUSINICIALPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Status_PlanoProduto StatusInicialProduto { get; set; }

        public long? Id_Tipo_Servico { get; set; }
        public long? Id_Subtipo_Servico { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "VALORPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string ValorProduto { get; set; }

        #endregion

        #region 2º Bloco - Instalação

        public bool CobrancaInstalacao { get; set; }
        public string ValorInstalacao { get; set; }
        public long? IdContaFinanceiraInstalacao { get; set; }
        public Momento_Cobranca? MomentoCobrancaInstalacao { get; set; }
        public Comprovante_Pagamento? ComprovantePagamentoInstalacao { get; set; }
        public bool OSTrocaEndereco { get; set; }

        #endregion

        #region 3º Bloco - Financeiro

        [Display(Name = "EMITIRCONTRATOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool EmitirContratoProduto { get; set; }

        public long? IdModeloContrato { get; set; }

        [Display(Name = "MODELOCONTRATOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string ModeloContratoProduto { get; set; }
        public bool Tem_Carencia { get; set; }

        public Int64? Periodo_Carencia { get; set; }

        [Display(Name = "CARENCIACONTRATOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string CarenciaContratoProduto { get; set; }

        public long? Id_Conta_Financeira { get; set; }

        [Display(Name = "CONTAFINANCEIRAPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Desc_Conta_Financeira { get; set; }

        [Display(Name = "FREQUENCIACOBRANCAPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Frequencia_Cobranca? FrequenciaCobrancaProduto { get; set; }

        public Padrao? Cobranca_Recorrente { get; set; }
        public Momento_Cobranca_Plano? Momento_Cobranca_Plano { get; set; }

        [Display(Name = "DIAINCLUSAOCLIENTEPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public DateTime? DiaInclusaoClienteProduto { get; set; }


        [Display(Name = "COMPROVANTEPAGAMENTOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Comprovante_Pagamento? ComprovantePagamentoProduto { get; set; }

        [Display(Name = "MODELONOTAFISCALPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Modelo_NotaFiscal? ModeloNotaFiscalProduto { get; set; }

        public long? Id_Cfop { get; set; }

        [Display(Name = "CODIGOCFOP", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Codigo_Cfop { get; set; }

        [Display(Name = "DESCRICAOCFOP", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Descricao_Cfop { get; set; }

        [Display(Name = "FORMATOCOBRANCAPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Formato_Cobranca? FormatoCobrancaProduto { get; set; }

        public long? Id_Recibo { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]
        public string Desc_Recibo { get; set; }



        #endregion

        #region 4º Bloco - Descontos

        public bool DescontoValorIntegral { get; set; }

        //[Display(Name = "PERCENTUALDESCONTOPRODURO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public string PercentualDescontoAntecipadoProduto { get; set; }

        //[Display(Name = "QTDDIASVENCTOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public int QtdeDiasVencimento { get; set; }

        //[Display(Name = "DESCONTOANTECIPADOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public bool DescontoAntecipadoProduto { get; set; }

        [Display(Name = "DESCONTOPROGRESSIVOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool DescontoProgressivoProduto { get; set; }

        [Display(Name = "MENSALIDADEINICIALPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string MensalidadeInicialProduto { get; set; }

        [Display(Name = "MENSALIDADEFINALPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string MensalidadeFinalProduto { get; set; }

        [Display(Name = "PERCENTUALDESCONTOPROGRESSIVO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string PercentualDescontoProgressivo { get; set; }

        [Display(Name = "VALORDESCONTOPROGRESSIVO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string ValorDescontoProgressivo { get; set; }

        public string PercentualDesconto { get; set; }

        [Display(Name = "VALORDESCONTOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string ValorDescontoProduto { get; set; }

        public bool Comerciavel { get; set; }
        public Tipo_Produto? Tipo_Produto { get; set; }
        public String Nome_Venda { get; set; }

        public List<PlanoMensalidadeCadastroModel> MensalidadesProd
        {
            get
            {
                if (_mensalidadesProd == null)
                    _mensalidadesProd = new List<PlanoMensalidadeCadastroModel>();
                return _mensalidadesProd;
            }
            set
            {
                _mensalidadesProd = value;
            }
        }

        public List<PlanoProdutoConfigNfCadastroModel> ConfiguracaoProdNF
        {
            get
            {
                if (_configuracaoProdNF == null)
                    _configuracaoProdNF = new List<PlanoProdutoConfigNfCadastroModel>();
                return _configuracaoProdNF;
            }
            set
            {
                _configuracaoProdNF = value;
            }
        }



        #endregion

        #region 5º Bloco - Configurações Adicionais

        [Display(Name = "ATIVARVALIDADE", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool AtivarValidade { get; set; }

        public bool OsPremier { get; set; }
        public Boolean UsaPlanoAdicional { get; set; }
        public String PlanoProduto { get; set; }
        public DateTime? Data_Validade { get; set; }

        [Display(Name = "PRORATAPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool ProRataProduto { get; set; }

        public bool Pro_Rata_Habilitacao { get; set; }
        public bool Pro_Rata_Interrupcao { get; set; }

        public bool Plano_Comercializavel { get; set; }

        public Momento_Cobranca? Momento_Cobranca { get; set; }

        public Cobranca_Realizada? Cobranca_Realizada { get; set; }

        public Int64? Dias_Vencimento { get; set; }
        public bool Pro_Rata_Cancelamento { get; set; }
        public virtual Boolean UtilizaVendaAgroup { get; set; }
        public string ValorVendaAgrupada { get; set; }
        public long? IdFormaPagamentoCob { get; set; }
        
        private List<PlanosProdutosAdicionaisCadastroModel> _planosProdutosAdicionais;
        public List<PlanosProdutosAdicionaisCadastroModel> PlanosProdutosAdicionais
        {
            get
            {
                if (_planosProdutosAdicionais == null)
                    _planosProdutosAdicionais = new List<PlanosProdutosAdicionaisCadastroModel>();
                return _planosProdutosAdicionais;
            }
            set
            {
                _planosProdutosAdicionais = value;
            }
        }
        #endregion

        #region Propiedades Temporarias de Mensalidade e ConfigNf
        public long Id_Mensalidade { get; set; }
        public Int16? Parcela_Ini { get; set; }
        public Int16? Parcela_Fin { get; set; }
        public Double? Percentual_Desconto { get; set; }
        public Double? Valor_Desconto { get; set; }


        public long? Id_ConfigNf { get; set; }
        public long? Id_Atividade { get; set; }
        public string Atividade { get; set; }
        public string Descricao_Plano { get; set; }
        public string Descricao_Produto { get; set; }
        public Double? Percentual_Imposto { get; set; }
        public Double? Valor_Discriminado { get; set; }
        public long? Id_Cfop_Instalacao { get; set; }
        public string Percentual_Discriminado { get; set; }
        public Tipo_Reajuste_Contratual Tipo_Discriminado { get; set; }
        public bool RetencaoImposto { get; set; }
        public Double? Icms { get; set; }
        public Double? Pis { get; set; }
        public Double? Cofins { get; set; }
        public Double? Iss { get; set; }
        #endregion


    }

    public class ExcluirPlanosProdutosObjViewModel
    {
        public string tipo { get; set; }
    }

    public class PlanoProdutoComboAntendimentoViewModel
    {
        public virtual Status_PlanoProduto? Status_PlanoProduto { get; set; }

        [JsonProperty(PropertyName = "id_Cliente_PlanoProduto")]
        public virtual long Id_Cliente_PlanoProduto { get; set; }
        [JsonProperty(PropertyName = "value")]
        public virtual long Value { get; set; }
        [JsonProperty(PropertyName = "label")]
        public virtual string Label { get; set; }

        [JsonProperty(PropertyName = "login_Radius")]
        public string Login_Radius { get; set; }
        [JsonProperty(PropertyName = "endereco")]
        public string Endereco { get; set; }

        [JsonProperty(PropertyName = "cep")]
        public string Cep { get; set; }
        [JsonProperty(PropertyName = "complemento")]
        public string Complemento { get; set; }

        [JsonProperty(PropertyName = "ponto_Referencia_Cliente")]
        public string Ponto_Referencia_Cliente { get; set; }
        [JsonProperty(PropertyName = "valor_Final")]
        public string Valor_Final { get; set; }
        [JsonProperty(PropertyName = "tipo")]
        public PlanoProdutoEnum? Tipo { get; set; }
        [JsonProperty(PropertyName = "tipo_Imovel")]
        public string Tipo_Imovel { get; set; }
        [JsonProperty(PropertyName = "cod_Escritorio")]
        public string Cod_Escritorio { get; set; }
        [JsonProperty(PropertyName = "desc_Escritorio")]
        public string Desc_Escritorio { get; set; }
        [JsonProperty(PropertyName = "cod_Area")]
        public string Cod_Area { get; set; }
        [JsonProperty(PropertyName = "desc_Area")]
        public string Desc_Area { get; set; }
        [JsonProperty(PropertyName = "cod_Localidade")]
        public int? Cod_Localidade { get; set; }
        [JsonProperty(PropertyName = "localidade")]
        public string Localidade { get; set; }
        [JsonProperty(PropertyName = "codCt")]
        public string CodCt { get; set; }
        [JsonProperty(PropertyName = "projeto")]
        public string Projeto { get; set; }
        public int? IdTipoServico { get; set; }
        public string IdSubTipoServico { get; set; }
        public string Bloco { get; set; }
        public string Apartamento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Pais { get; set; }
        public string Numero { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Int64 TipoImovelId { get; set; }


    }

    public class GetDadosPlanoProdutoModel
    {

        // Plano/Produto        
        public virtual String Forma_Envio { get; set; }
        public virtual String Banco { get; set; }
        public virtual String Agencia { get; set; }
        public virtual String Conta_Corrente { get; set; }
        public virtual String Digito_Verificador { get; set; }
        public virtual String Dia_Vencimento { get; set; }
        public virtual String Dias_Teste_Gratis { get; set; }
        public virtual String Dias_Teste_Contrato { get; set; }
        public virtual String Periodo_Carencia { get; set; }
        public virtual String Periodo_Carencia_Contrato { get; set; }
        public virtual long? Meses_Carencia { get; set; }
        public virtual long? Meses_Carencia_Carencia { get; set; }
        public virtual long? Meses_Contrato { get; set; }
        public virtual String Data_Assinatura { get; set; }
        public virtual String Multa_Rescisao { get; set; }
        public virtual Double? Multa_Rescisao_Contrato { get; set; }

        // Plano/Produto - Informações Técnicas
        public virtual String Login_Radius { get; set; }
        public virtual String Senha_Radius { get; set; }
        public virtual String POP { get; set; }
        public virtual String ONU { get; set; }
        public virtual long? id_ONU { get; set; }
        public virtual String OLT { get; set; }
        public virtual String Slot { get; set; }
        public virtual String PON { get; set; }
        public virtual String Caixa_FTTH { get; set; }
        public virtual String Porta_FTTH { get; set; }
        public virtual String Velocidade_Download { get; set; }
        public virtual String Velocidade_Upload { get; set; }
        public virtual String Velocidade_Nominal { get; set; }
        public virtual String Velocidade_Download_Reduzida { get; set; }
        public virtual String Velocidade_Upload_Reduzida { get; set; }
        public virtual String Observacoes { get; set; }
        public virtual String Status_PlanoProdutoStr { get; set; }
        public virtual String Quantidade { get; set; }
        public virtual Status_PlanoProduto? Status_PlanoProduto { get; set; }
        public virtual String Status_ContratoStr { get; set; }
        public virtual Status_Contrato? Status_Contrato { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //         ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "ID", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }
        public virtual Int64? Id_Cliente_Contrato { get; set; }

        public virtual Int64? Id_Plano_Produto { get; set; }

        public virtual String Nome_Plano_Produto { get; set; }

        [Display(Name = "LOGIN_DO_RADIUS", ResourceType = typeof(Resources.RsCadastros))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]


        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Endereco_Instalacao { get; set; }
        public virtual String Valor_Instalacao { get; set; }
        public virtual String Valor_Final_Plano { get; set; }
        public virtual String Valor_Total_Plano { get; set; }
        public virtual String ValorSubTotal { get; set; }

        public virtual String Numero_Contrato { get; set; }

        public virtual String Forma_Pagamento_Utilizada { get; set; }
        public virtual String Forma_Pagamento_Contrato { get; set; }
        public virtual String Forma_Envio_Contrato { get; set; }
        public virtual String Data_Vencimento_Contrato { get; set; }

        public virtual String CepInstalacao { get; set; }
        public virtual String LogradouroInstalacao { get; set; }
        public virtual String NumeroInstalacao { get; set; }
        public virtual String UFInstalacao { get; set; }
        public virtual String CidadeInstalacao { get; set; }
        public virtual String BairroInstalacao { get; set; }
        public virtual String TipoImovelInstalacao { get; set; }
        public virtual String PaisInstalacao { get; set; }
        public virtual String ComplementoInstalacao { get; set; }
        public virtual String PontoReferenciaInstalacao { get; set; }
        public virtual Boolean VelocidadeReduzida { get; set; }
        public virtual String Login_Cliente { get; set; }
        public virtual String Pool_Cliente { get; set; }
        public virtual String Ip_Cliente { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Id_Tipo_Servico { get; set; }
        private List<PlanoContratoCadastroModel> _contratoPlano;
        public List<PlanoContratoCadastroModel> ContratoPlano
        {
            get
            {
                if (_contratoPlano == null)
                    _contratoPlano = new List<PlanoContratoCadastroModel>();
                return _contratoPlano;
            }
            set
            {
                _contratoPlano = value;
            }
        }

        public static implicit operator GetDadosPlanoProdutoModel(List<GetDadosPlanoProdutoModel> v)
        {
            throw new NotImplementedException();
        }
    }

    public class ExcluirPlanosProdutosListaViewModel
    {
        public List<ExcluirPlanosProdutosObjViewModel> objs { get; set; }
    }

    public class PlanoDetalhesModels
    {
        [Display(Name = "PLANO_VALOR_REAL", ResourceType = typeof(Resources.RsCadastros))]
        public string ValorPlano { get; set; }

        [Display(Name = "VALOR_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        public string ValorPlanoProdutoFinal { get; set; }

        [Display(Name = "DIASTESTE", ResourceType = typeof(Resources.RsConfiguracao))]
        public long? Qtde_Dias { get; set; }

        [Display(Name = "VALOR_INSTALACAO", ResourceType = typeof(Resources.RsCadastros))]
        public string ValorInstalacao { get; set; }
        public string ValorInstalacaoFinal { get; set; }

        [Display(Name = "VELOCIDADENOMINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeNominal { get; set; }

        [Display(Name = "PERIODO_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public string Periodo_Carencia { get; set; }
        public string Multa_Carencia_Reais { get; set; }
        public string Renovar_Carencia_Automaticamente { get; set; }
        public string Cancelar_Contrato_Automatico_Fim_Carencia { get; set; }
        public string Tem_Reajuste { get; set; }
        public string Frequencia_Reajuste { get; set; }
        public long? Numero_Parcelas { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeDownload { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeDownloadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeUploadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeUpload { get; set; }
        public long? Id_Tecnologia { get; set; }
        public string Nome_NAS { get; set; }
        public string IP { get; set; }

        [Display(Name = "AUTENTICACAORADIUS", ResourceType = typeof(Resources.RsConfiguracao))]
        public string AutenticacaoRadius { get; set; }

        [Display(Name = "DETALHES", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Detalhes { get; set; }

        public List<PlanosProdutosAdicionaisCadastroModel> PlanosProdutosAdicionais { get; set; }
        public string ValorTotalSemInstalacao { get; set; }
        public string ValorTotalComInstalacao { get; set; }
        public string Multa_Carencia_Periodo { get; set; }
    }

    public class ProdutoDetalhesModels
    {
        [Display(Name = "PLANO_VALOR_REAL", ResourceType = typeof(Resources.RsCadastros))]
        public string ValorPlano { get; set; }

        [Display(Name = "VALOR_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        public string ValorPlanoProdutoFinal { get; set; }

        [Display(Name = "DIASTESTE", ResourceType = typeof(Resources.RsConfiguracao))]
        public long? Qtde_Dias { get; set; }

        [Display(Name = "VALOR_INSTALACAO", ResourceType = typeof(Resources.RsCadastros))]
        public string ValorInstalacao { get; set; }
        public string ValorInstalacaoFinal { get; set; }

        [Display(Name = "VELOCIDADENOMINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeNominal { get; set; }

        public string Data_Validade { get; set; }

        [Display(Name = "PERIODO_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public string Periodo_Carencia { get; set; }
        public long? Numero_Parcelas { get; set; }
        public string Nome_NAS { get; set; }
        public string IP { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeDownload { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeDownloadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeUploadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public string VelocidadeUpload { get; set; }

        [Display(Name = "AUTENTICACAORADIUS", ResourceType = typeof(Resources.RsConfiguracao))]
        public string AutenticacaoRadius { get; set; }

        [Display(Name = "DETALHES", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Detalhes { get; set; }
        public List<PlanosProdutosAdicionaisCadastroModel> PlanosProdutosAdicionais { get; set; }
    }

    public class PlanoProdutoComboModels : ObjComboResult<long>
    {
        public string ValorPlanoProduto { get; set; }
        public string ValorVendaAgrupada { get; set; }
        public bool UtilizaVendaAgrupada { get; set; }
        public long? Id_Modelo_Contrato { get; set; }
        public bool AutenticacaoRadius { get; set; }
        public bool CobraInstalacao { get; set; }
        public string ValorInstalacao { get; set; }
        public string ValorPlanoProduto_Final { get; set; }
        public string ValorInstalacao_Final { get; set; }
        public PlanoProdutoEnum Tipo { get; set; }
        public Status_PlanoProduto? Status_PlanoProduto { get; set; }
        public string Status_PlanoProduto_Str { get; set; }
        public bool Teste_Gratis { get; set; }
        public long? Qtde_Dias { get; set; }
        public bool Periodo_Carencia { get; set; }
        public long? Meses_Carencia { get; set; }
        public long? Numero_Parcelas { get; set; }
        public long? Id_Campanha { get; set; }

        public string Detalhes { get; set; }
        public string VelocidadeDownload { get; set; }
        public string VelocidadeDownloadReduzida { get; set; }
        public string VelocidadeUploadReduzida { get; set; }
        public string VelocidadeUpload { get; set; }
        public string VelocidadeNominal { get; set; }
        public long? Id_Radius { get; set; }
        public int? Tipo_Tecnologia { get; set; }
        public String Tipo_TecnologiaStr { get; set; }
        public String Disponibiliza_Telefonia { get; set; }
        public long? Id_Tecnologia { get; set; }
        public bool ProdutoQtdVariavel { get; set; }
        public int? Parcela { get; set; }
        public bool PossuiEntrada { get; set; }
    }

    public class PlanoProdutoAreaCoberturaCombo : ObjComboResult<long>
    {

        #region clone
        public string ValorPlanoProduto { get; set; }
        public long? Id_Modelo_Contrato { get; set; }
        public bool? AutenticacaoRadius { get; set; }
        public string ValorInstalacao { get; set; }
        public string ValorPlanoProduto_Final { get; set; }
        public string ValorInstalacao_Final { get; set; }
        public Status_PlanoProduto? Status_PlanoProduto { get; set; }
        public string Status_PlanoProduto_Str { get; set; }
        public bool? Teste_Gratis { get; set; }
        public long? Qtde_Dias { get; set; }
        public bool? Periodo_Carencia { get; set; }
        public long? Meses_Carencia { get; set; }
        public long? Numero_Parcelas { get; set; }
        public long? Id_Campanha { get; set; }

        public string Detalhes { get; set; }
        public string VelocidadeDownload { get; set; }
        public string VelocidadeDownloadReduzida { get; set; }
        public string VelocidadeUploadReduzida { get; set; }
        public string VelocidadeUpload { get; set; }
        public string VelocidadeNominal { get; set; }
        public long? Id_Radius { get; set; }
        #endregion
        public string Id_area_cobertura { get; set; }
        public PlanoProdutoEnum Tipo { get; set; }

    }
    public class RequestObjComboPai_Obj_Combo_Montagem<T, Y> : RequestObjCombo<long?>
    {

        public T idpai { get; set; }
        public Y config { get; set; }
    }
    public class ObjComboResultIdContrato : ObjComboResult<long>
    {
        public virtual long Id_Contrato { get; set; }
        public virtual String DescricaoContrato { get; set; }
        public virtual List<String> Reference { get; set; }
    }

    public class PlanoContratoCadastroModel : IViewModel<Int64>
    {

        // Plano/Produto        
        public virtual Int64 Id { get; set; }
        public virtual String Forma_Envio { get; set; }
        public virtual String Banco { get; set; }
        public virtual String Agencia { get; set; }
        public virtual String Conta_Corrente { get; set; }
        public virtual String Digito_Verificador { get; set; }
        public virtual String Dia_Vencimento { get; set; }
        public virtual String Dias_Teste_Gratis { get; set; }
        public virtual String Dias_Teste_Contrato { get; set; }
        public virtual String Periodo_Carencia { get; set; }
        public virtual String Periodo_Carencia_Contrato { get; set; }
        public virtual long? Meses_Carencia { get; set; }
        public virtual long? Meses_Carencia_Carencia { get; set; }
        public virtual long? Meses_Contrato { get; set; }
        public virtual String Data_Assinatura { get; set; }
        public virtual String Multa_Rescisao { get; set; }
        public virtual Double? Multa_Rescisao_Contrato { get; set; }

        // Plano/Produto - Informações Técnicas
        public virtual String Login_Radius { get; set; }
        public virtual String Senha_Radius { get; set; }
        public virtual String POP { get; set; }
        public virtual String ONU { get; set; }
        public virtual long? id_ONU { get; set; }
        public virtual String OLT { get; set; }
        public virtual String Slot { get; set; }
        public virtual String PON { get; set; }
        public virtual String Caixa_FTTH { get; set; }
        public virtual String Porta_FTTH { get; set; }
        public virtual String Velocidade_Download { get; set; }
        public virtual String Velocidade_Upload { get; set; }
        public virtual String Velocidade_Nominal { get; set; }
        public virtual String Velocidade_Download_Reduzida { get; set; }
        public virtual String Velocidade_Upload_Reduzida { get; set; }
        public virtual String Status_PlanoProdutoStr { get; set; }
        public virtual Status_PlanoProduto? Status_PlanoProduto { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //         ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "ID", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }
        public virtual Int64? Id_Cliente_Contrato { get; set; }

        public virtual Int64? Id_Plano_Produto { get; set; }

        public virtual String Nome_Plano_Produto { get; set; }

        [Display(Name = "LOGIN_DO_RADIUS", ResourceType = typeof(Resources.RsCadastros))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]


        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Endereco_Instalacao { get; set; }

        public virtual String Valor_Final_Plano { get; set; }

        public virtual String Numero_Contrato { get; set; }

        public virtual String Forma_Pagamento_Utilizada { get; set; }
        public virtual String Forma_Pagamento_Contrato { get; set; }
        public virtual String Forma_Envio_Contrato { get; set; }
        public virtual String Data_Vencimento_Contrato { get; set; }

        public virtual String CepInstalacao { get; set; }
        public virtual String LogradouroInstalacao { get; set; }
        public virtual String NumeroInstalacao { get; set; }
        public virtual String UFInstalacao { get; set; }
        public virtual String CidadeInstalacao { get; set; }
        public virtual String BairroInstalacao { get; set; }
        public virtual String TipoImovelInstalacao { get; set; }
        public virtual String PaisInstalacao { get; set; }
        public virtual String ComplementoInstalacao { get; set; }
        public virtual String PontoReferenciaInstalacao { get; set; }
        public virtual Boolean VelocidadeReduzida { get; set; }

    }

    public class RelatorioAnatel : DataTableRequest
    {
        public virtual String CNPJ { get; set; }
        public Int64 Mes { get; set; }
        public Int64 Ano { get; set; }
        public String Numero_Fistel { get; set; }
        public bool SCM { get; set; }
        public bool SMP { get; set; }
        public bool STFCPublico { get; set; }
        public bool STFCPubPriv { get; set; }
        public bool TV_Assinatura { get; set; }
        public string Empresa { get; set; }
        public string Filial { get; set; }
        public string Codigo_IBGE { get; set; }
        public Tipo_Cliente? Tipo_Cliente { get; set; }
        public Tipo_Endereco_Logradouro? Tipo_Atendimento { get; set; }
        public Meio_Acesso_Anatel? Meio_Acesso_Anatel { get; set; }
        public Tipo_Produto_Anatel? Tipo_Produto_Anatel { get; set; }
        public Tecnologia_Anatel? Tecnologia_Anatel { get; set; }
        public Modelo_Anatel? Modelo_Anatel { get; set; }
        public string Velocidade { get; set; }
        public Int64 Qtd_Acesso { get; set; }
        public UF? Uf { get; set; }
        public Int64 Area { get; set; }
        public Tarifa_Anatel? Tarifa_Basica { get; set; }
        public Localidade_Anatel? Localidade_Anatel { get; set; }
        public Int64 Estacao_Padrao { get; set; }
        public Int64 Estacao_M2M { get; set; }


    }

    public class ConfigConexaoPlano_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
    }

    public class PlanoProduto_DataTableRequest : DataTableRequest
    {
        public virtual String Status { get; set; }
    }

    public class ConfigConexaoPlano
    {
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }
        public virtual string Nome_Plano_Produto { get; set; }
        public virtual string Nome_Cliente { get; set; }
        public virtual string Numero_Contrato { get; set; }
        public virtual string Desc_Login_Radius { get; set; }

        public virtual Int16? Vlan_Plano_Produto { get; set; }
    }


    public class ConsultaDetalhePlanoModels
    {
        public int id { get; set; }
        public int id_cliente { get; set; }
        public int status { get; set; }
        public int id_cliente_contrato { get; set; }
        public int id_combo { get; set; }
        public int id_troca_plano { get; set; }
        public int is_plano_adicional { get; set; }
        public int id_plano_atrelado { get; set; }
        public string descricao { get; set; }
        public string codigo { get; set; }
        public double valorplano { get; set; }
        public int situacaoplano { get; set; }
        public int id_tipo_servico { get; set; }
        public int integracao_externa { get; set; }
        public int utiliza_integracao_externa { get; set; }
        public string codigo_auxiliar { get; set; }
        public int os_mudanca_plano { get; set; }
        public string product_id { get; set; }
        public string codigo_tipo_plano_integracao { get; set; }
        public string codigo_integracao { get; set; }
        public int tipo { get; set; }
        public string codigo_integracao_aux { get; set; }
        public string detalhesplano { get; set; }
        public int usa_plano_adicional { get; set; }



    }

    public class PlanosListaModels
    {
        public ConsultaDetalhePlanoModels PlanoPrincipal { get; set; }
        public List<ConsultaDetalhePlanoModels> PlanosAdicionais { get; set; }
    }

    public class PlanosClientesRelatorio
    {
        public string CodCliente { get; set; }
        public string CodContrato { get; set; }
        public string CodPlanoProduto { get; set; }
        public string Venda { get; set; }
        public string Cliente { get; set; }
        public string DataAprovacaoVenda { get; set; }
        public string DataAtivacao { get; set; }
        public string DataFimCarencia { get; set; }
        public string DataCancelamento { get; set; }
        public string DataSuspensao { get; set; }
        public string StatusPlano { get; set; }
        public string ValorPlano { get; set; }
        public string NomePlano { get; set; }
        public string TipoPlano { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Vendedor { get; set; }
    }

    public class PlanosClientesRelatorioRequest : DataTableRequest
    {
        public string IdCliente { get; set; }
        public string Contrato { get; set; }
        public string TipoPlano { get; set; }
        public string IdPlano { get; set; }
        public string IdBairro { get; set; }
        public string IdCidade { get; set; } 
        public string IdLogin { get; set; }
        public string[] StatusSelecionados { get; set; }
        public bool? Aguardando { get; set; }
        public bool? Habilitado { get; set; }
        public bool? Suspenso { get; set; }
        public bool? Cancelado { get; set; }
        public bool? Todos { get; set; }

        public string TipoData { get; set; }
        public string Inicio { get; set; }
        public string Fim { get; set; }
    }

    public class DesbloqueioConfiancaRequest
    {
        public long IdClienteContrato { get; set; }
    }

    public class PlanoInfo
    {
        public long? PlanoId { get; set; }
        public long? PlanoProdutoId { get; set; }
    }

    public class GetObjetoDadosPlanoProduto
    {
        // Plano/Produto        
        public virtual String Forma_Envio { get; set; }
        public virtual String Banco { get; set; }
        public virtual String Agencia { get; set; }
        public virtual String Conta_Corrente { get; set; }
        public virtual String Digito_Verificador { get; set; }
        public virtual String Dia_Vencimento { get; set; }
        public virtual String Dias_Teste_Gratis { get; set; }
        public virtual String Dias_Teste_Contrato { get; set; }
        public virtual String Periodo_Carencia { get; set; }
        public virtual String Periodo_Carencia_Contrato { get; set; }
        public virtual long? Meses_Carencia { get; set; }
        public virtual long? Meses_Carencia_Carencia { get; set; }
        public virtual long? Meses_Contrato { get; set; }
        public virtual String Data_Assinatura { get; set; }
        public virtual String Multa_Rescisao { get; set; }
        public virtual Double? Multa_Rescisao_Contrato { get; set; }

        // Plano/Produto - Informações Técnicas
        public virtual String Login_Radius { get; set; }
        public virtual String Senha_Radius { get; set; }
        public virtual String POP { get; set; }
        public virtual String ONU { get; set; }
        public virtual long? id_ONU { get; set; }
        public virtual String OLT { get; set; }
        public virtual String Slot { get; set; }
        public virtual String PON { get; set; }
        public virtual String Caixa_FTTH { get; set; }
        public virtual String Porta_FTTH { get; set; }
        public virtual String Velocidade_Download { get; set; }
        public virtual String Velocidade_Upload { get; set; }
        public virtual String Velocidade_Nominal { get; set; }
        public virtual String Velocidade_Download_Reduzida { get; set; }
        public virtual String Velocidade_Upload_Reduzida { get; set; }
        public virtual String Observacoes { get; set; }
        public virtual String Status_PlanoProdutoStr { get; set; }
        public virtual Status_PlanoProduto? Status_PlanoProduto { get; set; }
        public virtual String Status_ContratoStr { get; set; }
        public virtual Status_Contrato? Status_Contrato { get; set; }
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }
        public virtual Int64? Id_Cliente_Contrato { get; set; }

        public virtual Int64? Id_Plano_Produto { get; set; }

        public virtual String Nome_Plano_Produto { get; set; }
        public virtual String Endereco_Instalacao { get; set; }
        public virtual String Valor_Instalacao { get; set; }
        public virtual String Valor_Final_Plano { get; set; }
        public virtual String ValorSubTotal { get; set; }

        public virtual String Numero_Contrato { get; set; }

        public virtual String Forma_Pagamento_Utilizada { get; set; }
        public virtual String Forma_Pagamento_Contrato { get; set; }
        public virtual String Forma_Envio_Contrato { get; set; }
        public virtual String Data_Vencimento_Contrato { get; set; }

        public virtual String CepInstalacao { get; set; }
        public virtual String LogradouroInstalacao { get; set; }
        public virtual String NumeroInstalacao { get; set; }
        public virtual String UFInstalacao { get; set; }
        public virtual String CidadeInstalacao { get; set; }
        public virtual String BairroInstalacao { get; set; }
        public virtual String TipoImovelInstalacao { get; set; }
        public virtual String PaisInstalacao { get; set; }
        public virtual String ComplementoInstalacao { get; set; }
        public virtual String PontoReferenciaInstalacao { get; set; }
        public virtual Boolean VelocidadeReduzida { get; set; }
        public virtual String Login_Cliente { get; set; }
        public virtual String Pool_Cliente { get; set; }
        public virtual String Ip_Cliente { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Id_Tipo_Servico { get; set; }
    }
}