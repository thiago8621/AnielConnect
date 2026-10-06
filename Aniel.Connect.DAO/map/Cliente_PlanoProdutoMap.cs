using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;


namespace Aniel.Connect.DAO.map
{
    public class Cliente_PlanoProdutoMap : ClassMapBase<Cliente_PlanoProduto>
    {

        public Cliente_PlanoProdutoMap() : base()
        {
            Table("TB_CLIENTE_PLANO_PRODUTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CLIENTE_PLANO_PRODUTO");

            Map(x => x.Valor_Real, "VALOR_REAL");
            Map(x => x.Valor_Instalacao_Real, "VALOR_INSTALACAO_REAL");
            Map(x => x.Valor_Instalacao_Final, "VALOR_INSTALACAO_FINAL");
            Map(x => x.Valor_Final_Total, "VALOR_FINAL_TOTAL");
            Map(x => x.Valor_Final, "VALOR_FINAL");
            Map(x => x.Parcelas_Instalacao, "PARCELAS_INSTALACAO");
            Map(x => x.Dias_Teste_Gratis, "DIAS_TESTE_GRATIS");
            Map(x => x.Periodo_Carencia, "PERIODO_CARENCIA");
            Map(x => x.Meses_Carencia, "MESES_CARENCIA");
            Map(x => x.CobrarInstalacaoEntrada, "COBRAR_ENTRADA_INSTALACAO");

            Map(x => x.CodigoAgrupamento, "CODIGO_AGRUPAMENTO_INTEGRACAO");

            References(x => x.ClienteContrato, "ID_CLIENTE_CONTRATO").ForeignKey("FK_CLIENTE_PP_CONTRATO");

            Map(x => x.Multa_Carencia_Reais, "MULTA_CARENCIA");
            Map(x => x.CancelaContratoFimCarencia, "CANCEL_CONTRATO_CARENCIA");
            Map(x => x.Frequencia_Reajuste, "FREQUENCIA_REAJUSTE").CustomType<Frequencia_Reajuste>();
            Map(x => x.Renovar_CarenciaAuto, "RENOVAR_CARENCIA").CustomType<Renova_FidelidadeEnum>();

            Map(x => x.End_Instalacao_Igual_Principal, "END_INSTALACAO_PRINCIPAL");

            Map(x => x.Login_Radius, "LOGIN_RADIUS").Length(100);
            Map(x => x.Senha_Radius, "SENHA_RADIUS").Length(100);
            Map(x => x.VLAN_Uplink, "VLAN_UPLINK");

            Map(x => x.Data_Desbloqueio_Confianca, "DATA_DESBLOQUEIO_CONFIANCA");
            Map(x => x.Velocidade_Reduzida, "VELOCIDADE_REDUZIDA");

            Map(x => x.Nas, "NAS");
            Map(x => x.Data_Habilitado, "DATA_HABILITADO");
            Map(x => x.Data_Desabilitado, "DATE_DESABILITADO");

            Map(x => x.CodContratoIntegracao, "COD_CONTRATO_INTEGRACAO");

            Map(x => x.OrdemServicoIntegracao, "ORDEM_SERVICO_INTEGRACAO");

            Map(x => x.Cod_Escritorio, "COD_ESCRITORIO").Length(4);
            Map(x => x.Cod_Area, "COD_AREA").Length(4); 
            Map(x => x.Cod_Localidade, "COD_LOCALIDADE");
            Map(x => x.Localidade,"LOCALIDADE").Length(50);
            Map(x => x.Id_Venda, "ID_VENDA").Index("IDX_CLIENTE_PLANO_PROD_VENDA");
            Map(x => x.IdFormaPagamentoVenda, "ID_FORMAPGTO_VENDA");

            References(x => x.Cliente, "ID_CLIENTE").NotFound.Ignore().Fetch.Join().ForeignKey("FK_CLIENTE_PP_CLIENTE");
            
            References(x => x.Plano, "ID_PLANO").ForeignKey("FK_CLIENTE_PP_PLANO");
            References(x => x.Produto, "ID_PRODUTO").ForeignKey("FK_CLIENTE_PP_PRODUTO");
            References(x => x.Endereco_Instalacao, "ID_ENDERECO_INSTALACAO").Cascade.SaveUpdate().ForeignKey("FK_CLIENTE_PP_END_INST");
            References(x => x.Endereco_Mudanca_Cliente, "ID_ENDERECO_MUDANCA_CLIENTE").Cascade.SaveUpdate().ForeignKey("FK_CLIENTE_PP_END_MUDANCA");
            References(x => x.Campanha, "ID_CAMPANHA").NotFound.Ignore().Fetch.Join().ForeignKey("FK_CLIENTE_PP_CAMPANHA");

            References(x => x.Pool_Ip, "ID_POOL_IP").NotFound.Ignore().Fetch.Join().ForeignKey("FK_CLIENTE_POOL_IP");

            Map(x => x.Status, "STATUS").CustomType<Status_PlanoProduto>();
            Map(x => x.Motivo_Mudanca_PP, "MOTIVO_MUDANCA").CustomType<Motivo_MudancaPlano_Produto>();

            Map(x => x.Atualiza_Ola, "ATUALIZA_OLA");

            References(x => x.Motivo_Cancelamento, "ID_MOTIVO_CANCELAMENTO").NotFound.Ignore().Fetch.Join().ForeignKey("FK_CLIENTE_MOTIVO_CANC");
            References(x => x.Motivo_Suspensao, "ID_MOTIVO_SUSPENSAO_CONTRATUAL").NotFound.Ignore().Fetch.Join().ForeignKey("FK_CLIENTE_MOTIVO_SUSPENSAO");
            
            References(x => x.Forma_Pagamento_Cliente, "ID_FORMA_PAGTO_CLIENTE").NotFound.Ignore().Fetch.Join().ForeignKey("FK_CLIENTE_PP_FORM_PAG");
            References(x => x.Combo, "ID_COMBO").NotFound.Ignore().Fetch.Join().ForeignKey("FK_CLIENTE_PP_COMBO");

            //campos adicionais 
            Map(x => x.ReducaoVelocidade, "REDUCAOVELOCIDADE");
            Map(x => x.QtdDiasAtraso, "QTDDIASATRASO");
            Map(x => x.VelocidadeDownloadReduzida, "VELOCIDADEDOWNLOADREDUZIDA");
            Map(x => x.VelocidadeUploadReduzida, "VELOCIDADEUPLOADREDUZIDA");
            Map(x => x.Telefonia, "TELEFONIA");

            Map(x => x.ImpedirBloqueio_Automatico, "IMPEDIRBLOQUEIO_AUTOMATICO");
            Map(x => x.ImpedirBloqueio_Sexta, "IMPEDIRBLOQUEIO_SEXTA");
            Map(x => x.ImpedirBloqueio_Sabado, "IMPEDIRBLOQUEIO_SABADO");
            Map(x => x.ImpedirBloqueio_Domingo, "IMPEDIRBLOQUEIO_DOMINGO");
            Map(x => x.ImpedirBloqueio_Feriados, "IMPEDIRBLOQUEIO_FERIADOS");

            Map(x => x.UtilizaDesbloqueioConfianca, "UTILIZADESBLOQUEIOCONF");
            Map(x => x.UtilizaBloqueioDebito, "UTILIZABLOQUEIODEBITO");

            Map(x => x.FrequenciaDesbloqueio, "FREQUENCIADESBLOQUEIO").CustomType<Frequencia_Desbloqueio>();
            Map(x => x.QtdDiasbloqueados, "QTDDIASBLOQUEADOS");
            Map(x => x.QtdDiasdesbloqueados, "QTDDIASDESBLOQUEADOS");

            Map(x => x.Localidade_Anatel, "LOCALIDADE_ANATEL").CustomType<Localidade_Anatel>();

            Map(x => x.Quantidade_Produto_Variavel, "QTD_PROD_VARIAVEL");
            Map(x => x.Valor_Total_Plano_Variavel, "VLR_TOTAL_PLANO_VARIAVEL");
            Map(x => x.Id_TrocaPlano, "ID_TROCA_PLANO");
            Map(x => x.Id_TrocaPlanoCancelados, "ID_TROCA_PLANO_CANCELADOS");
            Map(x => x.Caixa_Ftth_Reservado, "CAIXA_FTTH_RESERVA");
            Map(x => x.Porta_Ftth_Reservado, "PORTA_FTTH_RESERVA");
            Map(x => x.CodigoIntegracao, "CODIGO_INTEGRACAO");
            Map(x => x.ValorInstalacaoEntrada, "VALOR_INSTALACAO_ENTRADA");
            Map(x => x.ClienteAnteriorTroca, "ID_CLI_ANTERIOR_TROCA_TITULAR");
            Map(x => x.ContaReceberInstalacao, "GERADO_CONTARECEBER_INSTALACAO");
            Map(x => x.BaixadoInstalacao, "BAIXADO_INSTALACAO");
            Map(x => x.UtilizaVendaAgroup, "UTILIZA_VENDA_AGRUP");
            Map(x => x.isPlanoAdicional, "IS_PLANO_ADICIONAL");
            Map(x => x.possuiPlanosAdicionais, "POSSUI_PLANOS_ADICIONAIS");
            Map(x => x.Id_Plano_Atrelado, "ID_PLANO_ATRELADO");
            Map(x => x.Parcela_Entrada, "PARCELA_ENTRADA");
            Map(x => x.Valor_Entrada, "VALOR_ENTRADA");
        }
    }
}
