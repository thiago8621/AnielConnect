using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;
//using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class PlanoMap : ClassMap<Plano>
    {

        public PlanoMap()
        {
            Table("TB_PLANO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("Plano_Produto");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Nome_Venda, "NOME_VENDA").Length(100);
            Map(x => x.Codigo, "CODIGO").Length(20);
            Map(x => x.DetalhesPlano, "DETALHESPLANO").Length(5000);
            Map(x => x.AutenticacaoRadius, "AUTENTICACAORADIUS");
            Map(x => x.VelocidadeNominal, "VELOCIDADENOMINAL").Length(20);
            Map(x => x.VelocidadeDownload, "VELOCIDADEDOWNLOAD").Length(20);
            Map(x => x.VelocidadeUpload, "VELOCIDADEUPLOAD").Length(20);
            Map(x => x.ValorPlano, "VALORPLANO");
            Map(x => x.ValorPlanoFinal, "VALORPLANOFINAL");
            Map(x => x.CobrancaInstalacao, "COBRANCAINSTALACAO");
            Map(x => x.ValorInstalacao, "VALORINSTALACAO");
            Map(x => x.ValorInstalacaoFinal, "VALORINSTALACAOFINAL");
            Map(x => x.Periodo_Carencia, "PERIODO_CARENCIA");
            Map(x => x.Formato_do_Plano, "FORMATO_DO_PLANO").CustomType<Formato_Plano>();
            Map(x => x.Num_Dids, "NUMERO_DIDS_VINCULADOS");
            Map(x => x.Integracao, "INTEGRACAO_EXTERNA").CustomType<IntegracaoExterna>();
            Map(x => x.Cobrar_Por_Telefonia).CustomType<Sinapse.Model.enumerados.Circuito_Did_Cobranca>();
            Map(x => x.ModoCobrancaInstalacao, "MODOCOBRANCAINSTALACAO").CustomType<Modo_Cobranca_Instalacao>();

            Map(x => x.QtdParcelas, "QTDPARCELAS");
            Map(x => x.CobrancaInstalacaoEntrada, "COB_INSTALACAO_ENTRADA");
            Map(x => x.OSTrocaEndereco, "OSTROCAENDERECO");
            Map(x => x.ReducaoVelocidade, "REDUCAOVELOCIDADE");
            Map(x => x.QtdDiasAtraso, "QTDDIASATRASO");
            Map(x => x.VelocidadeDownloadReduzida, "VELOCIDADEDOWNLOADREDUZIDA");
            Map(x => x.VelocidadeUploadReduzida, "VELOCIDADEUPLOADREDUZIDA");
            Map(x => x.SituacaoPlano, "SITUACAOPLANO").CustomType<Situacao_plano>();
            Map(x => x.AtivarValidade, "ATIVARVALIDADE");
            Map(x => x.Validade, "VALIDADE");
            Map(x => x.UsaPlanoAdicional, "USA_PLANO_ADICIONAL");
            Map(x => x.ProRata, "PRORATA");
            Map(x => x.Pro_Rata_Habilitacao, "PRO_RATA_HABILITACAO");
            Map(x => x.Pro_Rata_Interrupcao, "PRO_RATA_INTERRUPCAO");
            Map(x => x.Pro_Rata_Cancelamento, "PRO_RATA_CANCELAMENTO");


            Map(x => x.UtilizaQoSProfile, "UTILIZA_QOS_PROFILE");
            Map(x => x.IdQoSProfile, "ID_QOS_PROFILE");
            Map(x => x.IdQoSProfileReduzido, "ID_QOS_PROFILE_VELOC_REDUZIDA");
            Map(x => x.BloqueioDomainName, "BLOQUEIO_POR_DOMAIN_NAME");
            Map(x => x.IdDomainName, "ID_DOMAIN_NAME");

            Map(x => x.IdPolicyNameDownload, "ID_POLICY_NAME_DOWNLOAD");
            Map(x => x.IdPolicyNameDownloadReduzido, "ID_POLICY_NAME_DOWN_REDUZIDO");
            Map(x => x.IdPolicyNameUpload, "ID_POLICY_NAME_UPLOAD");
            Map(x => x.IdPolicyNameUploadReduzido, "ID_POLICY_NAME_UP_REDUZIDO");
            Map(x => x.UtilizaPolicyNameIPv6, "UTILIZA_POLICY_NAME_IPV6");
            Map(x => x.BloqueioPoolIp, "BLOQUEIO_POR_POOL_IP");
            Map(x => x.UtilizaPoolReducao, "UTILIZA_POOL_IP_REDUCAO");
            Map(x => x.FrequenciaCobranca, "FREQUENCIACOBRANCA").CustomType<Frequencia_Cobranca>();

            Map(x => x.ComprovantePagamento, "COMPROVANTEPAGAMENTO").CustomType<Comprovante_Pagamento>();
            Map(x => x.ComprovantePagamentoFinanceiro, "COMPROVANTEPAGAMENTO_INSTALACAO").CustomType<Comprovante_Pagamento>();
            Map(x => x.Momento_Cobra_Instalacao, "MOMENTO_COBRANCA_INSTALACAO").CustomType<Momento_Cobra_Instalacao>();
            Map(x => x.FormatoCobranca, "FORMATOCOBRANCA").CustomType<Formato_Cobranca>();
            Map(x => x.RetencaoImposto, "RETENCAOIMPOSTO");

            Map(x => x.PercentualDescontoProgressivo, "PERCENTUALDESCONTOPROGRESSIVO");
            Map(x => x.ValorDescontoProgressivo, "VALORDESCONTOPRORESSIVO");
            Map(x => x.MensalidadeInicial, "MENSALIDADEINICIAL");
            Map(x => x.MensalidadeFinal, "MENSALIDADEFINAL");
            Map(x => x.EmitirContrato, "EMITIRCONTRATO");

            Map(x => x.UtilizaBloqueioDebito, "UTILIZABLOQUEIODEBITO");
            Map(x => x.UtilizaAtributoIPOE, "UTILIZAATRIBUTOIPOE");
            Map(x => x.QtdDiasbloqueados, "QTDDIASBLOQUEADOS");
            Map(x => x.StatusInicial, "STATUSINICIAL").CustomType<Sinapse.Model.enumerados.Status_PlanoProduto>();
            Map(x => x.Status, "STATUS").CustomType<Sinapse.Model.enumerados.Status_Default>();
            Map(x => x.Tipo_Tecnologia, "TIPO_TECNOLOGIA").CustomType<Tipo_Tecnologia>();
            Map(x => x.FormatoTarifaTelefonia, "FORMATO_TARIFA_TELEFONIA").CustomType<FormatoTarifaTelefonia>();

            Map(x => x.ConsumoMinimo, "CONSUMO_MINIMO");
            Map(x => x.PossuiConsumoMinimo, "POSSUI_CONSUMO_MINIMO");            
            Map(x => x.PossuiFranquia, "POSSUI_FRANQUIA");
            Map(x => x.Id_Tipo_Servico, "ID_TIPO_SERVICO");
            Map(x => x.Id_Subtipo_Servico, "ID_SUBTIPO_SERVICO");
            Map(x => x.Id_Tipo_ServicoOS, "ID_TIPO_SERVICO_MUDANCA_PLANO");
            Map(x => x.Id_Subtipo_ServicoOS, "ID_SUB_SERVICO_MUDANCA_PLANO");
            Map(x => x.DefinirTipo_SubTipoOS, "TIPO_SUBTIPO_MUDANCA_PLANO");

            Map(x => x.HabilitarLc, "HABILITAR_LC");
            Map(x => x.ValorLc, "VALOR_LC");
            Map(x => x.CadenciaLc, "CADENCIA_LC");

            Map(x => x.HabilitarLdn, "HABILITAR_LDN");
            Map(x => x.ValorLdn, "VALOR_LDN");
            Map(x => x.CadenciaLdn, "CADENCIA_LDN");
            Map(x => x.Disponibiliza_Telefonia, "DISPONIBILIZA_TELEFONIA");
            Map(x => x.HabilitarVc1, "HABILITAR_VC1");
            Map(x => x.ValorVc1, "VALOR_VC1");
            Map(x => x.CadenciaVc1, "CADENCIA_VC1");
            Map(x => x.HabilitarVc2, "HABILITAR_VC2");
            Map(x => x.ValorVc2, "VALOR_VC2");
            Map(x => x.CadenciaVc2, "CADENCIA_VC2");
            Map(x => x.HabilitarVc3, "HABILITAR_VC3");
            Map(x => x.ValorVc3, "VALOR_VC3");
            Map(x => x.CadenciaVc3, "CADENCIA_VC3");

            Map(x => x.HabilitarLdi, "HABILITAR_LDI");
            Map(x => x.ValorLdi, "VALOR_LDI");
            Map(x => x.CadenciaLdi, "CADENCIA_LDI");
            Map(x => x.CobrarBina, "COBRAR_BINA");
            Map(x => x.ValorBina, "VALOR_BINA");
            Map(x => x.Utiliza_Integracao_Externa, "UTILIZA_INTEGRACAO_EXTERNA");
            Map(x => x.UtilizaDesconto, "UTILIZA_DESCONTO");
            Map(x => x.CobrarSigame, "COBRAR_SIGAME");
            Map(x => x.ValorSigame, "VALOR_SIGAME");
            Map(x => x.CobrarBlacklist, "COBRAR_BLACKLIST");
            Map(x => x.ValorBlacklist, "VALOR_BLACKLIST");
            Map(x => x.IntragrupoGratuita, "INTRAGRUPO_GRATUITA");
            Map(x => x.IntragrupoLcGratuita, "INTRAGRUPO_LC_GRATUITA");

            Map(x => x.CobrarBloqueioChamadaCobrar, "COBRARBLOQUEIO_CHAMADACOBRAR");
            Map(x => x.ValorBloqueioChamadaCobrar, "VALORBLOQUEIO_CHAMADACOBRAR");
            Map(x => x.BloqueioChamadaCobrar, "BLOQUEIO_CHAMADA_COBRAR").CustomType<DidFormatoCobrar>();
            Map(x => x.Momento_Cobranca, "MOMENTO_COBRANCA").CustomType<Momento_Cobranca>();
            Map(x => x.Momento_Cobranca_Plano, "MOMENTO_COBRANCA_PLANO").CustomType<Momento_Cobranca_Plano>();
            Map(x => x.Cobranca_Recorrente, "COBRANCA_RECORRENTE").CustomType<Sinapse.Model.enumerados.Padrao>();
            Map(x => x.Cobranca_Realizada, "COBRANCA_REALIZADA").CustomType<Cobranca_Realizada>();
            References(x => x.FormaPagamentoCob, "ID_FORMAPAG_COB").ForeignKey("FK_PLANO_FORMA"); ;
            Map(x => x.Plano_Comercializavel, "PLANO_COMERCIALIZAVEL");
            Map(x => x.Dias_Vencimento, "DIAS_VENCIMENTO");
            Map(x => x.CodigoAuxiliar, "CODIGO_AUXILIAR");
            
            Map(x => x.Modelo_Anatel, "MODELO_ANATEL").CustomType<Modelo_Anatel>();
            Map(x => x.Tecnologia_Anatel, "TECNOLOGIA_ANATEL").CustomType<Tecnologia_Anatel>();
            Map(x => x.Tipo_Produto, "TIPO_PRODUTO_ANATEL").CustomType<Tipo_Produto_Anatel>();
            Map(x => x.Tipo_ProdutoScm, "TIPO_PRODUTO_ANATELSMP").CustomType<Tipo_Produto_AnatelSCM>();
            Map(x => x.Meio_Acesso_Anatel, "MEIO_ACESSO_ANATEL").CustomType<Meio_Acesso_Anatel>();
            Map(x => x.Regime, "REGIME").CustomType<Regime_Anatel>();
            Map(x => x.Modalidade_Anatel, "MODALIDADE_ANATEL").CustomType<Modalidade_Plano>();
            Map(x => x.Tarifa_Anatel, "TARIFA_ANATEL").CustomType<Tarifa_Anatel>();
            Map(x => x.CodigoAuxiliarFranquia, "CODIGO_AUXILIAR_FRANQUIA");
            Map(x => x.OS_Mudanca_Plano, "OS_MUDANCA_PLANO");
            Map(x => x.ProductId, "PRODUCT_ID");
            Map(x => x.CodigoTipoPlanoIntegracao, "CODIGO_TIPO_PLANO_INTEGRACAO");
            Map(x => x.CodigoIntegracao, "CODIGO_INTEGRACAO");
            Map(x => x.FormaPagamentoCobranca, "FORMA_PAG_COBRANCA_INSTALACAO");
            Map(x => x.DiasBoleto, "DIAS_VENC_BOLETO");
            Map(x => x.Tipo, "TIPO").CustomType<Tipo_Plano_Produto>();
            Map(x => x.CodigoIntegracaoAuxiliar, "CODIGO_INTEGRACAO_AUX");
            Map(x => x.UtilizaVendaAgroup, "UTILIZA_VENDA_AGRUPADA");
            Map(x => x.ValorVendaAgrupada, "VALOR_VENDA_AGRUPADA");
            Map(x => x.ProdutoQtdVariavel, "PRODUTO_QTD_VARIAVEL");
            Map(x => x.QtdParcelas_ProdutoVariavel, "QTD_PARC_PROD_VARIAVEL");
            Map(x => x.PossuiValorEntrada, "POSSUI_VALOR_ENTRADA");
            Map(x => x.ExclusivoCliente, "EXCLUSIVO_CLIENTE");

            References(x => x.Tecnologia, "ID_TECNOLOGIA").ForeignKey("FK_PLANO_TECNOLOGIA");
            References(x => x.Conta_FinanceiraInstalacao, "ID_CONTAFINANCEIRA_INSTALACAO").ForeignKey("FK_PLANO_CFINANINST");
            References(x => x.Conta_FinanceiraPlano, "IDCONTAFINANCEIRA").ForeignKey("FK_PLANO_CFINANPLANO");
            References(x => x.Radius, "ID_RADIUS").ForeignKey("FK_PLANO_RADIUS");
            References(x => x.Modelo_Recibo, "ID_MODELO_RECIBO").ForeignKey("FK_PLANO_MODELO_RECIBO");
            References(x => x.Modelo_Recibo_Instalacao, "ID_MODELO_RECIBO_INSTALACAO").ForeignKey("FK_PLANO_MODEL_REC_INST");
            References(x => x.Cfop, "ID_CFOP").ForeignKey("FK_PLANO_CFOP");
            References(x => x.Modelo_Contrato, "ID_MODELOCONTRATO").ForeignKey("FK_PLANO_MODELO_CONTRATO");
            References(x => x.Atributo_IPOE, "ID_ATRIBUTOIPOE").ForeignKey("FK_PLANO_IPOE");
            References(x => x.Franquia, "ID_FRANQUIA").ForeignKey("FK_PLANO_FRANQUIA");
            References(x => x.Blacklist, "ID_BLACKLIST").ForeignKey("FK_BLACKLIST");
            HasMany(x => x.Config_NF_PlanoProduto).KeyColumn("ID_PLANO").Cascade.All().Inverse();
            HasMany(x => x.Faixas_Mensalidades).KeyColumn("ID_PLANO").Cascade.All().Inverse();
            HasMany(x => x.PlanosAdicionais).KeyColumn("ID_PLANOS_ADICIONAIS").Cascade.All().Inverse();
            
            HasMany(x => x.Plano_Produto_Comp).KeyColumn("ID_PLANO").Cascade.All().Inverse();
            HasMany(x => x.MaterialOSPlano).KeyColumn("ID_PLANO").Cascade.All().Inverse();
        }
    }
}
