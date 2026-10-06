using System;
using System.Collections.Generic;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class Plano : BaseModel
    {
        private IList<Config_NF_PlanoProduto> config_NF_PlanoProduto;
        private IList<Faixas_Mensalidades> faixas_Mensalidades;
        private IList<MaterialOs> materialOs;
        private IList<PlanoProdutoAdicional> _planosAdicionais;
        private IList<Plano_Produto_Comp> plano_Produto_Comp;        

        public virtual String Codigo { get; set; }
        public virtual Radius Radius { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Nome_Venda { get; set; }
        public virtual String DetalhesPlano { get; set; }
        public virtual Boolean AutenticacaoRadius { get; set; }
        public virtual String VelocidadeNominal { get; set; }
        public virtual String VelocidadeDownload { get; set; }
        public virtual String VelocidadeUpload { get; set; }
        public virtual Tecnologia Tecnologia { get; set; }
        public virtual Tipo_Tecnologia? Tipo_Tecnologia { get; set; }
        public virtual Double ValorPlano { get; set; }
        public virtual Double ValorPlanoFinal { get; set; }
        public virtual Double ValorInstalacao { get; set; }
        public virtual Double ValorInstalacaoFinal { get; set; }
        public virtual Boolean Periodo_Carencia { get; set; }
        public virtual Boolean CobrancaInstalacao { get; set; }
        public virtual Boolean UsaPlanoAdicional { get; set; }

        public virtual Formato_Plano Formato_do_Plano { get; set; }
        public virtual long Num_Dids { get; set; }
        public virtual IntegracaoExterna Integracao { get; set; }

        public virtual Modo_Cobranca_Instalacao? ModoCobrancaInstalacao { get; set; }
        public virtual Circuito_Did_Cobranca Cobrar_Por_Telefonia { get; set; }

        public virtual Int16? QtdParcelas { get; set; }
        public virtual Boolean OSTrocaEndereco { get; set; }
        public virtual Boolean ReducaoVelocidade { get; set; }
        public virtual Int16 QtdDiasAtraso { get; set; }
        public virtual String VelocidadeDownloadReduzida { get; set; }
        public virtual String VelocidadeUploadReduzida { get; set; }
        public virtual Boolean Disponibiliza_Telefonia { get; set; }
        public virtual Situacao_plano? SituacaoPlano { get; set; }

        public virtual Boolean AtivarValidade { get; set; }
        public virtual DateTime? Validade { get; set; }
        //public virtual Boolean TesteGratis { get; set; }
        //public virtual Int16? DiasTeste { get; set; }
        public virtual Boolean ProRata { get; set; }
        public virtual Boolean Pro_Rata_Habilitacao { get; set; }
        public virtual Boolean Pro_Rata_Interrupcao { get; set; }
        public virtual Boolean Pro_Rata_Cancelamento { get; set; }


        public virtual Conta_Financeira Conta_FinanceiraInstalacao { get; set; }

        public virtual Conta_Financeira Conta_FinanceiraPlano { get; set; }
        public virtual Blacklist Blacklist { get; set; }


        public virtual Frequencia_Cobranca? FrequenciaCobranca { get; set; }

        public virtual Modelo_Recibo Modelo_Recibo { get; set; }
        public virtual Modelo_Recibo Modelo_Recibo_Instalacao { get; set; }

        public virtual Comprovante_Pagamento? ComprovantePagamento { get; set; }
        public virtual Comprovante_Pagamento? ComprovantePagamentoFinanceiro { get; set; }
        public virtual Momento_Cobra_Instalacao? Momento_Cobra_Instalacao { get; set; }

        public virtual Boolean ProdutoQtdVariavel { get; set; }
        public virtual Int32? QtdParcelas_ProdutoVariavel { get; set; }
        public virtual Boolean PossuiValorEntrada { get; set; }
        public virtual Cfop Cfop { get; set; }

        public virtual Formato_Cobranca? FormatoCobranca { get; set; }

        //Foi pedido para ser removido pelo renato #442 EVA
        //public virtual Boolean AtribuirValorIntegral { get; set; }
        //public virtual Boolean DescontoAntecipado { get; set; }
        //public virtual Int16? QtdDiasVencto { get; set; }
        //public virtual Double? PercentualDescontoAntecipado { get; set; }
        //public virtual Double? ValorDescontoAntecipado { get; set; }

        //public virtual Boolean DescontoProgressivo { get; set; }
        public virtual Boolean UtilizaDesconto { get; set; }
        public virtual Boolean Utiliza_Integracao_Externa { get; set; }
        public virtual long? Desconto { get; set; }
        public virtual Boolean RetencaoImposto { get; set; }
        //public virtual Boolean Cobranca_Recorrente { get; set; }
        public virtual Modo_Cobranca_Instalacao? Modo_Cobranca_Plano { get; set; }
        public virtual Double? PercentualDescontoProgressivo { get; set; }
        public virtual Double? ValorDescontoProgressivo { get; set; }
        public virtual String MensalidadeInicial { get; set; }
        public virtual String MensalidadeFinal { get; set; }
        public virtual Boolean EmitirContrato { get; set; }

        public virtual Modelo_Contrato Modelo_Contrato { get; set; }

        public virtual Boolean ImpedirBloqueio_Automatico { get; set; }
        public virtual Boolean ImpedirBloqueio_Sexta { get; set; }
        public virtual Boolean ImpedirBloqueio_Sabado { get; set; }
        public virtual Boolean ImpedirBloqueio_Domingo { get; set; }
        public virtual Boolean ImpedirBloqueio_Feriados { get; set; }

        public virtual Status_PlanoProduto? StatusInicial { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual long? Id_Tipo_Servico { get; set; }
        public virtual long? Id_Subtipo_Servico { get; set; }
        public virtual Boolean DefinirTipo_SubTipoOS { get; set; }
        public virtual long? Id_Tipo_ServicoOS { get; set; }
        public virtual long? Id_Subtipo_ServicoOS { get; set; }
        public virtual Boolean UtilizaAtributoIPOE { get; set; }
        public virtual Atributo_IPOE Atributo_IPOE { get; set; }
        public virtual Boolean UtilizaBloqueioDebito { get; set; }
        public virtual Boolean CobrancaInstalacaoEntrada { get; set; }
        public virtual Int16 QtdDiasbloqueados { get; set; }

        //public virtual Boolean UtilizaDesbloqueioConfianca { get; set; }
        //public virtual Frequencia_Desbloqueio? FrequenciaDesbloqueio { get; set; }
        //public virtual Int16 QtdDiasdesbloqueados { get; set; }
        public virtual Int16 Dias_bloqueio { get; set; }
        public virtual bool Plano_Comercializavel { get; set; }

        public virtual Momento_Cobranca? Momento_Cobranca { get; set; }
        public virtual Momento_Cobranca_Plano? Momento_Cobranca_Plano { get; set; }
        public virtual Sinapse.Model.enumerados.Padrao? Cobranca_Recorrente { get; set; }

        public virtual Cobranca_Realizada? Cobranca_Realizada { get; set; }
        public virtual Forma_Pagamento FormaPagamentoCob { get; set; }
        public virtual Int64? Dias_Vencimento { get; set; }

        public virtual NAS Servidor_NAS { get; set; }
        public virtual Boolean PossuiFranquia { get; set; }
        public virtual Boolean PossuiConsumoMinimo { get; set; }
        public virtual Double? ConsumoMinimo { get; set; }
        public virtual Franquia Franquia { get; set; }
        public virtual FormatoTarifaTelefonia? FormatoTarifaTelefonia { get; set; }

        public virtual Boolean HabilitarLc { get; set; }
        public virtual Double? ValorLc { get; set; }
        public virtual String CadenciaLc { get; set; }
        public virtual Boolean HabilitarLdn { get; set; }
        public virtual Double? ValorLdn { get; set; }
        public virtual String CadenciaLdn { get; set; }
        public virtual Boolean HabilitarLdi { get; set; }
        public virtual Double? ValorLdi { get; set; }
        public virtual String CadenciaLdi { get; set; }
        public virtual Boolean HabilitarVc1 { get; set; }
        public virtual Double? ValorVc1 { get; set; }
        public virtual String CadenciaVc1 { get; set; }
        public virtual Boolean HabilitarVc2 { get; set; }
        public virtual Double? ValorVc2 { get; set; }
        public virtual String CadenciaVc2 { get; set; }
        public virtual Boolean HabilitarVc3 { get; set; }
        public virtual Double? ValorVc3 { get; set; }
        public virtual String CadenciaVc3 { get; set; }
        public virtual Boolean IntragrupoLcGratuita { get; set; }
        public virtual Boolean IntragrupoGratuita { get; set; }

        public virtual Boolean CobrarBina { get; set; }
        public virtual Double? ValorBina { get; set; }
        public virtual Boolean CobrarSigame { get; set; }
        public virtual Double? ValorSigame { get; set; }
        public virtual Boolean CobrarBlacklist { get; set; }
        public virtual Double? ValorBlacklist { get; set; }
        public virtual Boolean CobrarBloqueioChamadaCobrar { get; set; }
        public virtual Double? ValorBloqueioChamadaCobrar { get; set; }
        public virtual DidFormatoCobrar? BloqueioChamadaCobrar { get; set; }
        public virtual Modelo_Anatel? Modelo_Anatel { get; set; }
        public virtual Tecnologia_Anatel? Tecnologia_Anatel { get; set; }
        public virtual Tipo_Produto_Anatel? Tipo_Produto { get; set; }
        public virtual Tipo_Produto_AnatelSCM? Tipo_ProdutoScm { get; set; }
        public virtual Meio_Acesso_Anatel? Meio_Acesso_Anatel { get; set; }
        public virtual Regime_Anatel? Regime { get; set; }
        public virtual Modalidade_Plano? Modalidade_Anatel { get; set; }
        public virtual Tarifa_Anatel? Tarifa_Anatel { get; set; }
        public virtual Int64? FormaPagamentoCobranca { get; set; }
        public virtual int DiasBoleto { get; set; }
        public virtual Tipo_Plano_Produto? Tipo { get; set; }
        #region Huawei
        public virtual bool UtilizaQoSProfile { get; set; }
        public virtual string IdQoSProfile { get; set; }
        public virtual string IdQoSProfileReduzido { get; set; }
        public virtual bool BloqueioDomainName { get; set; }
        public virtual string IdDomainName { get; set; }
        #endregion

        #region Juniper
        //public virtual bool UtilizaPolicyName { get; set; }
        public virtual string IdPolicyNameDownload { get; set; }
        public virtual string IdPolicyNameDownloadReduzido { get; set; }
        public virtual string IdPolicyNameUpload { get; set; }
        public virtual string IdPolicyNameUploadReduzido { get; set; }
        public virtual bool UtilizaPolicyNameIPv6 { get; set; }
        #endregion

        public virtual Boolean BloqueioPoolIp { get; set; }
        public virtual Boolean UtilizaPoolReducao { get; set; }
        public virtual IList<Config_NF_PlanoProduto> Config_NF_PlanoProduto
        {
            get
            {
                if (config_NF_PlanoProduto == null)
                    config_NF_PlanoProduto = new List<Config_NF_PlanoProduto>();
                return config_NF_PlanoProduto;
            }
            set
            {
                config_NF_PlanoProduto = value;
            }
        }

        public virtual IList<Faixas_Mensalidades> Faixas_Mensalidades
        {
            get
            {
                if (faixas_Mensalidades == null)
                    faixas_Mensalidades = new List<Faixas_Mensalidades>();
                return faixas_Mensalidades;
            }
            set
            {
                faixas_Mensalidades = value;
            }
        }
        public virtual IList<MaterialOs> MaterialOSPlano
        {
            get
            {
                if (materialOs == null)
                    materialOs = new List<MaterialOs>();
                return materialOs;
            }
            set
            {
                materialOs = value;
            }
        }

        public virtual IList<PlanoProdutoAdicional> PlanosAdicionais
        {
            get
            {
                if (_planosAdicionais == null)
                    _planosAdicionais = new List<PlanoProdutoAdicional>();
                return _planosAdicionais;
            }
            set
            {
                _planosAdicionais = value;
            }
        }

        public virtual IList<Plano_Produto_Comp> Plano_Produto_Comp
        {
            get
            {
                if (plano_Produto_Comp == null)
                    plano_Produto_Comp = new List<Plano_Produto_Comp>();
                return plano_Produto_Comp;
            }
            set
            {
                plano_Produto_Comp = value;
            }
        }

        public virtual double QtdDiasdesbloqueados { get; set; }
        public virtual String CodigoAuxiliar { get; set; }
        public virtual String CodigoAuxiliarFranquia { get; set; }
        public virtual Boolean OS_Mudanca_Plano { get; set; }

        public virtual string ProductId { get; set; }
        public virtual string CodigoTipoPlanoIntegracao { get; set; }
        public virtual string CodigoIntegracao { get; set; }
        public virtual string CodigoIntegracaoAuxiliar { get; set; }

        ///Venda Agrupada
        public virtual Boolean UtilizaVendaAgroup { get; set; }
        public virtual Double ValorVendaAgrupada { get; set; }

        public virtual bool ExclusivoCliente { get; set; }
    }
}
