using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Newtonsoft.Json;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    #region Enumerado Plano Produto
    //public enum PlanoProdutoEnum
    //{
    //    Plano = 1,
    //    Produto = 2
    //} 
    #endregion

    #region PlanoComboResult
    public class PlanoComboResult : IObjComboResult<Int64>
    {
        public Int64 Id { get; set; }
        public String Text { get; set; }
        public String Upload { get; set; }
        public String Download { get; set; }
    } 
    #endregion

    #region PlanoProdutoIndexModel
    public class PlanoProdutoIndexModel
    {
        public Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Descricao { get; set; }               
        public string Tipo { get; set; }               
        public string Situacao { get; set; }               
        public string Detalhes { get; set; }
        public string ModeloNF { get; set; }
        public string ValorDiscriminadoNF { get; set; }
        public string ValorDiscriminado { get; set; }
        public string PctImpostoValor { get; set; }
        public string Icms { get; set; }
        public string Pis { get; set; }
        public string Confins { get; set; }
        public string Iss { get; set; }


    } 
    #endregion

    #region PlanoProdutoApiRequest
    public class PlanoProdutoApiRequest : ApiRequest
    {
        public String ValorPlanoProduto { get; set; }
        public Int64? Id_Modelo_Contrato { get; set; }
        public Boolean AutenticacaoRadius { get; set; }
        public String ValorInstalacao { get; set; }
        public String ValorPlanoProduto_Final { get; set; }
        public String ValorInstalacao_Final { get; set; }
        public PlanoProdutoEnum Tipo { get; set; }
        public Status_PlanoProduto? Status_PlanoProduto { get; set; }
        public String Status_PlanoProduto_Str { get; set; }
        public Boolean Teste_Gratis { get; set; }
        public Int64? Qtde_Dias { get; set; }
        public Boolean Periodo_Carencia { get; set; }
        public Int64? Meses_Carencia { get; set; }
        public Int64? Numero_Parcelas { get; set; }
        public Int64? Id_Campanha { get; set; }

        public String Detalhes { get; set; }
        public String VelocidadeDownload { get; set; }
        public String VelocidadeDownloadReduzida { get; set; }
        public String VelocidadeUploadReduzida { get; set; }
        public String VelocidadeUpload { get; set; }
        public String VelocidadeNominal { get; set; }
        public Int64? Id_Radius { get; set; }
    }
    #endregion

    #region PlanoCadastroModel
    public class PlanoCadastroModel : IViewModel<Int64>
    {
        private List<PlanoMensalidadeCadastroModel> _mensalidades;

        private List<PlanoProdutoConfigNfCadastroModel> _configuracaoNF;
        private List<PlanoProdutoConfigNfCadastroModel> _configuracaoNFInstalacao;

        public Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Codigo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "DESCRICAOPLANO", ResourceType = typeof(Resources.RsConfiguracao))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Descricao { get; set; }

        [Display(Name = "DETALHES", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Detalhes { get; set; }


        [Display(Name = "AUTENTICACAORADIUS", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean AutenticacaoRadius { get; set; }

        public virtual Boolean AtributoIPOE { get; set; }





        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "VELOCIDADENOMINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeNominal { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "VELOCIDADEDOWNLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeDownload { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "VELOCIDADEUPLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeUpload { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "VALORPLANO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String ValorPlano { get; set; }

        [Display(Name = "COBRANCAINSTALACAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean CobrancaInstalacao { get; set; }

        [Display(Name = "VALORINSTALACAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String ValorInstalacao { get; set; }

        [Display(Name = "MODOCOBRANCAINSTALACAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Modo_Cobranca_Instalacao? ModoCobrancaInstalacao { get; set; }

        //[Display(Name = "QTDPARCELAS", ResourceType = typeof(Resources.RsConfiguracao))]
        public Int16? QtdParcelas { get; set; }

        //Novo Incluido em 02/04/2019
        [Display(Name = "OSTROCAENDERECO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean OSTrocaEndereco { get; set; }

        [Display(Name = "REDUCAOVELOCIDADE", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean ReducaoVelocidade { get; set; }


        [Display(Name = "DESBLOQUEIO_CONFIANCA", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean DesbLoqueio_Conf { get; set; }

        [Display(Name = "BLOQUEIO_AUTO_DEBITO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean BLOQUEIO_AUTO_DEBITO { get; set; }

        public Int16 QtdDiasbloqueados { get; set; }

        public Int16 dias_desbloq { get; set; }

        [Display(Name = "QTDDIASATRASO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Int16 QtdDiasAtraso { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeDownloadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeUploadReduzida { get; set; }

        public Int64? Id_Tecnologia { get; set; }

        [Display(Name = "SITUACAOPLANO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean SituacaoPlano { get; set; }

        [Display(Name = "ATIVARVALIDADE", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean AtivarValidade { get; set; }

        [Display(Name = "VALIDADE", ResourceType = typeof(Resources.RsConfiguracao))]
        public DateTime? Validade { get; set; }

        [Display(Name = "TESTEGRATIS", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean TesteGratis { get; set; }

        [Display(Name = "DIASTESTE", ResourceType = typeof(Resources.RsConfiguracao))]
        public Int16? DiasTeste { get; set; }

        [Display(Name = "PRORATA", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean ProRata { get; set; }

        public Boolean Pro_Rata_Habilitacao { get; set; }
        public Boolean Pro_Rata_Interrupcao { get; set; }
        public Boolean Pro_Rata_Cancelamento { get; set; }

        public Int64? Id_ContaFinanceiraInstalacao { get; set; }

        [Display(Name = "CONTAFINANCEIRA", ResourceType = typeof(Resources.RsConfiguracao))]
        public String DescContaFinanceiraInstalacao { get; set; }

        public Int64? Id_ContaFinanceiraPlano { get; set; }

        [Display(Name = "CONTAFINANCEIRA", ResourceType = typeof(Resources.RsConfiguracao))]
        public String DescContaFinanceiraPlano { get; set; }

        [Display(Name = "FREQUENCIACOBRANCA", ResourceType = typeof(Resources.RsConfiguracao))]
        public Frequencia_Cobranca? FrequenciaCobranca { get; set; }

        [Display(Name = "FREQUENCIA_DESBLOQUEIO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Frequencia_Desbloqueio? Frequencia_Desbloqueio { get; set; }

        public Int64? Id_Recibo { get; set; }
        public Int64? Id_ReciboInstalacaco { get; set; }

        public String Desc_Recibo { get; set; }


        [Display(Name = "COMPROVANTEPAGAMENTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Comprovante_Pagamento? ComprovantePagamento { get; set; }

        [Display(Name = "COMPROVANTEPAGAMENTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Comprovante_Pagamento? ComprovantePagamentoFinanceiro { get; set; }

        [Display(Name = "MODELONOTAFISCAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public Modelo_NotaFiscal? ModeloNotaFiscal { get; set; }
        public Modelo_NotaFiscal? ModeloNotaFiscalInstalacao { get; set; }

        public Int64? Id_Radius { get; set; }
        public Int64? NomeIPOE { get; set; }
        public Int64? Id_Cfop { get; set; }
        public Int64? Id_Cfop_Instalacao { get; set; }

        [Display(Name = "CODIGOCFOP", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Codigo_Cfop { get; set; }

        [Display(Name = "DESCRICAOCFOP", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Descricao_Cfop { get; set; }

        [Display(Name = "FORMATOCOBRANCA", ResourceType = typeof(Resources.RsConfiguracao))]
        public Formato_Cobranca? FormatoCobranca { get; set; }

        //Novo Incluido em 14/03/2019
        [Display(Name = "ATRIBUIRVALORINTEGRAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean AtribuirValorIntegral { get; set; }

        //[Display(Name = "DESCONTOANTECIPADO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public Boolean DescontoAntecipado { get; set; }

        //[Display(Name = "QTDDIASVENCTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public Int16? QtdDiasVencto { get; set; }

        //[Display(Name = "PERCENTUALDESCONTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public String PercentualDescontoAntecipado { get; set; }

        //[Display(Name = "VALORDESCONTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public String ValorDescontoAntecipado { get; set; }

        [Display(Name = "DESCONTOPROGRESSIVO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean DescontoProgressivo { get; set; }

        //Novo Incluido em 02/04/2019
        [Display(Name = "RETENCAOIMPOSTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean RetecaoImposto { get; set; }
        public Boolean RetecaoImposto_Instalacao { get; set; }

        [Display(Name = "PERCENTUALDESCONTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String PercentualDescontoProgressivo { get; set; }

        [Display(Name = "VALORDESCONTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String ValorDescontoProgressivo { get; set; }

        //Novo Incluido em 14/03/2019
        [Display(Name = "MENSALIDADEINICIAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public String MensalidadeInicial { get; set; }

        [Display(Name = "MENSALIDADEFINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public String MensalidadeFinal { get; set; }

        [Display(Name = "EMITIRCONTRATO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean EmitirContrato { get; set; }

        public Int64? IdModeloContrato { get; set; }

        [Display(Name = "MODELOCONTRATO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String DescModeloContrato { get; set; }


        [Display(Name = "IMPEDIRBLOQUEIO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean ImpedirBloqueio_Automatico { get; set; }
        public Boolean ImpedirBloqueio_Sexta { get; set; }
        public Boolean ImpedirBloqueio_Sabado { get; set; }
        public Boolean ImpedirBloqueio_Domingo { get; set; }
        public Boolean ImpedirBloqueio_Feriados { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                     ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "STATUSINICIAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public Status_PlanoProduto? StatusInicial { get; set; }

        // Fim Configuracao Plano Model

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


        #region Propiedades Temporarias de Mensalidade e ConfigNf
        public Int64 Id_Mensalidade { get; set; }
        public Int16? Parcela_Ini { get; set; }
        public Int16? Parcela_Fin { get; set; }
        public Double? Percentual_Desconto { get; set; }
        public Double? Valor_Desconto { get; set; }

        public Int64 Id_ConfigNf { get; set; }
        public Int64? Id_Atividade { get; set; }
        public Int64? Id_Atividade_Instalacao { get; set; }
        public String Atividade { get; set; }
        public String Descricao_Plano { get; set; }
        public String Descricao_Produto { get; set; }
        public Double? Percentual_Imposto { get; set; }
        public Double? Valor_Discriminado { get; set; }
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
    #endregion

    #region PlanoMensalidadeCadastroModel
    public class PlanoMensalidadeCadastroModel
    {
        public Int64? Id { get; set; }
        public String Parcela_Ini { get; set; }
        public String Parcela_Fin { get; set; }
        public String Percentual_Desconto { get; set; }
        public String Valor_Desconto { get; set; }
    }
    #endregion

    #region PlanoProdutoConfigNfCadastroModel
    public class PlanoProdutoConfigNfCadastroModel
    {
        public Int64 Id { get; set; }

        public Int64? IdClassificacaoTributaria { get; set; }
        public String DescClassificacaoTributaria { get; set; }

        public Modelo_NotaFiscal? ModeloNotaFiscal { get; set; }
        public String DescModeloNotaFiscal { get; set; }

        public String ValorDiscriminado { get; set; }
    }
    #endregion

    #region ProdutoCadastroModel
    public class ProdutoCadastroModel : IViewModel<Int64>
    {
        private List<PlanoMensalidadeCadastroModel> _mensalidadesProd;
        private List<PlanoProdutoConfigNfCadastroModel> _configuracaoProdNF;
        public Int64 Id { get; set; }
        /*********************
        * Configuração Produto
        *********************/

        //1º Bloco - Dados do Produto
        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Codigo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "DESCRICAOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Descricao { get; set; }

        [Display(Name = "DETALHES", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Detalhes { get; set; }


        [Display(Name = "SITUACAOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean SituacaoProduto { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "STATUSINICIALPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Status_PlanoProduto StatusInicialProduto { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "VALORPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String ValorProduto { get; set; }

        //2º Bloco - Configurações Adicionais

        [Display(Name = "ATIVARVALIDADE", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean AtivarValidade { get; set; }

        public DateTime? Data_Validade { get; set; }

        [Display(Name = "DIASTESTEPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Int64 DiasTesteProduto { get; set; }

        [Display(Name = "TESTEGRATISPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean TesteGratisProduto { get; set; }

        [Display(Name = "PRORATAPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean ProRataProduto { get; set; }

        public Boolean Pro_Rata_Habilitacao { get; set; }
        public Boolean Pro_Rata_Interrupcao { get; set; }
        public Boolean Pro_Rata_Cancelamento { get; set; }

        //3º Bloco - Financeiro
        [Display(Name = "EMITIRCONTRATOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean EmitirContratoProduto { get; set; }

        public Int64? IdModeloContrato { get; set; }

        [Display(Name = "MODELOCONTRATOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String ModeloContratoProduto { get; set; }

        [Display(Name = "CARENCIACONTRATOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String CarenciaContratoProduto { get; set; }

        public Int64? Id_Conta_Financeira { get; set; }

        [Display(Name = "CONTAFINANCEIRAPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Desc_Conta_Financeira { get; set; }

        [Display(Name = "FREQUENCIACOBRANCAPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Frequencia_Cobranca? FrequenciaCobrancaProduto { get; set; }

        [Display(Name = "DIAINCLUSAOCLIENTEPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public DateTime? DiaInclusaoClienteProduto { get; set; }


        [Display(Name = "COMPROVANTEPAGAMENTOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Comprovante_Pagamento? ComprovantePagamentoProduto { get; set; }

        [Display(Name = "MODELONOTAFISCALPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Modelo_NotaFiscal? ModeloNotaFiscalProduto { get; set; }

        public Int64? Id_Cfop { get; set; }

        [Display(Name = "CODIGOCFOP", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Codigo_Cfop { get; set; }

        [Display(Name = "DESCRICAOCFOP", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Descricao_Cfop { get; set; }

        [Display(Name = "FORMATOCOBRANCAPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Formato_Cobranca? FormatoCobrancaProduto { get; set; }

        public Int64? Id_Recibo { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]
        public String Desc_Recibo { get; set; }


        //4º Bloco - Descontos

        public Boolean DescontoValorIntegral { get; set; }

        //[Display(Name = "PERCENTUALDESCONTOPRODURO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public String PercentualDescontoAntecipadoProduto { get; set; }

        //[Display(Name = "QTDDIASVENCTOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public Int32 QtdeDiasVencimento { get; set; }

        //[Display(Name = "DESCONTOANTECIPADOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public Boolean DescontoAntecipadoProduto { get; set; }

        [Display(Name = "DESCONTOPROGRESSIVOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean DescontoProgressivoProduto { get; set; }

        [Display(Name = "MENSALIDADEINICIALPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String MensalidadeInicialProduto { get; set; }

        [Display(Name = "MENSALIDADEFINALPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String MensalidadeFinalProduto { get; set; }

        [Display(Name = "PERCENTUALDESCONTOPROGRESSIVO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String PercentualDescontoProgressivo { get; set; }

        [Display(Name = "VALORDESCONTOPROGRESSIVO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String ValorDescontoProgressivo { get; set; }

        public String PercentualDesconto { get; set; }

        [Display(Name = "VALORDESCONTOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String ValorDescontoProduto { get; set; }

        //[Display(Name = "VALORDESCONTOANTECIPADOPRODUTO", ResourceType = typeof(Resources.RsConfiguracao))]
        //public String ValorDescontoAntecipadoProduto { get; set; }

        [Display(Name = "IMPEDIRBLOQUEIO", ResourceType = typeof(Resources.RsConfiguracao))]
        public Boolean ImpedirBloqueio_Automatico { get; set; }
        public Boolean ImpedirBloqueio_Sexta { get; set; }
        public Boolean ImpedirBloqueio_Sabado { get; set; }
        public Boolean ImpedirBloqueio_Domingo { get; set; }
        public Boolean ImpedirBloqueio_Feriados { get; set; }

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

        #region Propiedades Temporarias de Mensalidade e ConfigNf
        public Int64 Id_Mensalidade { get; set; }
        public Int16? Parcela_Ini { get; set; }
        public Int16? Parcela_Fin { get; set; }
        public Double? Percentual_Desconto { get; set; }
        public Double? Valor_Desconto { get; set; }


        public Int64? Id_ConfigNf { get; set; }
        public Int64? Id_Atividade { get; set; }
        public String Atividade { get; set; }
        public String Descricao_Plano { get; set; }
        public String Descricao_Produto { get; set; }
        public Double? Percentual_Imposto { get; set; }
        public Double? Valor_Discriminado { get; set; }

        public Boolean RetencaoImposto { get; set; }
        public Double? Icms { get; set; }
        public Double? Pis { get; set; }
        public Double? Cofins { get; set; }
        public Double? Iss { get; set; }
        #endregion

    }
    #endregion

    #region ExcluirPlanosProdutosObjViewModel
    public class ExcluirPlanosProdutosObjViewModel
    {
        public String tipo { get; set; }
    } 
    #endregion

    #region PlanoProdutoComboAntendimentoViewModel
    public class PlanoProdutoComboAntendimentoViewModel
    {
        public virtual Status_PlanoProduto? Status_PlanoProduto { get; set; }

        [JsonProperty(PropertyName = "value")]
        public virtual Int64 Value { get; set; }
        [JsonProperty(PropertyName = "label")]
        public virtual String Label { get; set; }

        [JsonProperty(PropertyName = "login_Radius")]
        public String Login_Radius { get; set; }
        [JsonProperty(PropertyName = "endereco")]
        public String Endereco { get; set; }

        [JsonProperty(PropertyName = "cep")]
        public String Cep { get; set; }
        [JsonProperty(PropertyName = "complemento")]
        public String Complemento { get; set; }

        [JsonProperty(PropertyName = "ponto_Referencia_Cliente")]
        public String Ponto_Referencia_Cliente { get; set; }
        [JsonProperty(PropertyName = "valor_Final")]
        public String Valor_Final { get; set; }
        [JsonProperty(PropertyName = "tipo")]
        public PlanoProdutoEnum Tipo { get; set; }
        [JsonProperty(PropertyName = "tipo_Imovel")]
        public String Tipo_Imovel { get; set; }
        [JsonProperty(PropertyName = "cod_Escritorio")]
        public String Cod_Escritorio { get; set; }
        [JsonProperty(PropertyName = "desc_Escritorio")]
        public String Desc_Escritorio { get; set; }
        [JsonProperty(PropertyName = "cod_Area")]
        public String Cod_Area { get; set; }
        [JsonProperty(PropertyName = "desc_Area")]
        public String Desc_Area { get; set; }
        [JsonProperty(PropertyName = "cod_Localidade")]
        public Int32? Cod_Localidade { get; set; }
        [JsonProperty(PropertyName = "localidade")]
        public String Localidade { get; set; }
        [JsonProperty(PropertyName = "codCt")]
        public String CodCt { get; set; }
    }
    #endregion

    #region ExcluirPlanosProdutosListaViewModel
    public class ExcluirPlanosProdutosListaViewModel
    {
        public List<ExcluirPlanosProdutosObjViewModel> objs { get; set; }
    } 
    #endregion

    #region PlanoDetalhesModels
    public class PlanoDetalhesModels
    {
        [Display(Name = "PLANO_VALOR_REAL", ResourceType = typeof(Resources.RsCadastros))]
        public String ValorPlano { get; set; }

        [Display(Name = "VALOR_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        public String ValorPlanoProdutoFinal { get; set; }

        [Display(Name = "DIASTESTE", ResourceType = typeof(Resources.RsConfiguracao))]
        public Int64? Qtde_Dias { get; set; }

        [Display(Name = "VALOR_INSTALACAO", ResourceType = typeof(Resources.RsCadastros))]
        public String ValorInstalacao { get; set; }
        public String ValorInstalacaoFinal { get; set; }

        [Display(Name = "VELOCIDADENOMINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeNominal { get; set; }

        [Display(Name = "PERIODO_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public String Periodo_Carencia { get; set; }
        public Int64? Numero_Parcelas { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeDownload { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeDownloadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeUploadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeUpload { get; set; }
        public Int64? Id_Tecnologia { get; set; }
        public String Nome_NAS { get; set; }
        public String IP { get; set; }

        [Display(Name = "AUTENTICACAORADIUS", ResourceType = typeof(Resources.RsConfiguracao))]
        public String AutenticacaoRadius { get; set; }

        [Display(Name = "DETALHES", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Detalhes { get; set; }
    } 
    #endregion

    #region ProdutoDetalhesModels
    public class ProdutoDetalhesModels
    {
        [Display(Name = "PLANO_VALOR_REAL", ResourceType = typeof(Resources.RsCadastros))]
        public String ValorPlano { get; set; }

        [Display(Name = "VALOR_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        public String ValorPlanoProdutoFinal { get; set; }

        [Display(Name = "DIASTESTE", ResourceType = typeof(Resources.RsConfiguracao))]
        public Int64? Qtde_Dias { get; set; }

        public String Bloqueio_Automatico { get; set; }

        [Display(Name = "VALOR_INSTALACAO", ResourceType = typeof(Resources.RsCadastros))]
        public String ValorInstalacao { get; set; }
        public String ValorInstalacaoFinal { get; set; }

        [Display(Name = "VELOCIDADENOMINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeNominal { get; set; }

        public String Data_Validade { get; set; }

        [Display(Name = "PERIODO_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public String Periodo_Carencia { get; set; }
        public Int64? Numero_Parcelas { get; set; }
        public String Nome_NAS { get; set; }
        public String IP { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeDownload { get; set; }

        [Display(Name = "VELOCIDADEDOWNLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeDownloadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOADREDUZIDA", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeUploadReduzida { get; set; }

        [Display(Name = "VELOCIDADEUPLOAD", ResourceType = typeof(Resources.RsConfiguracao))]
        public String VelocidadeUpload { get; set; }

        [Display(Name = "AUTENTICACAORADIUS", ResourceType = typeof(Resources.RsConfiguracao))]
        public String AutenticacaoRadius { get; set; }

        [Display(Name = "DETALHES", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Detalhes { get; set; }
    }
    #endregion

    #region PlanoProdutoComboModels
    public class PlanoProdutoComboModels : ObjComboResult<Int64>
    {
        public String ValorPlanoProduto { get; set; }
        public Int64? Id_Modelo_Contrato { get; set; }
        public Boolean AutenticacaoRadius { get; set; }
        public String ValorInstalacao { get; set; }
        public String ValorPlanoProduto_Final { get; set; }
        public String ValorInstalacao_Final { get; set; }
        public PlanoProdutoEnum Tipo { get; set; }
        public Status_PlanoProduto? Status_PlanoProduto { get; set; }
        public String Status_PlanoProduto_Str { get; set; }
        public Boolean Teste_Gratis { get; set; }
        public Int64? Qtde_Dias { get; set; }
        public Boolean Periodo_Carencia { get; set; }
        public Int64? Meses_Carencia { get; set; }
        public Int64? Numero_Parcelas { get; set; }
        public Int64? Id_Campanha { get; set; }

        public String Detalhes { get; set; }
        public String VelocidadeDownload { get; set; }
        public String VelocidadeDownloadReduzida { get; set; }
        public String VelocidadeUploadReduzida { get; set; }
        public String VelocidadeUpload { get; set; }
        public String VelocidadeNominal { get; set; }
        public Int64? Id_Radius { get; set; }

    } 
    #endregion
    
    public class Desbloqueio_Confianca_ApiRequest : ApiRequest
    {
        public virtual String Plano { get; set; }
        public virtual String Cpf_Cnpj { get; set; }
    }

    public class getCodigoIntegracao
    {
        public virtual string CodigoIntegracao { get; set; }

        
    }

    public class getCodigoIntegracaoRequest : ApiRequest
    {
       
        public virtual long? Id { get; set; }
    }

    public class UpdateCodigoIntegracaoRequest : ApiRequest
    {

        public virtual long? Id { get; set; }
        public virtual string CodigoIntegracao { get; set; }
    }



}