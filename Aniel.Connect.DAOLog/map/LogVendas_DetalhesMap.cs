using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAOLog;
using System;

namespace Aniel.Connect.DAOLog.map
{
    public class LogVendas_DetalhesMap : LogGenericoMap<LogVendas_Detalhes, Int64>
    {
        public LogVendas_DetalhesMap() : base()
        {
            Table("TB_CPP_EXCLUSAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CPP_EXCLUSAO");

            Map(p => p.Chave, "OBJETO").Index("IDX_VENDAS_DET_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_VENDAS_DET_CAMPO");
            Map(p => p.Log_Integracao, "LOG_INTEGRACAO").Length(5000);

            Map(x => x.Id_Cliente_Plano_Produto, "ID_CPP");
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

            Map(x => x.Id_ClienteContrato, "ID_CLIENTE_CONTRATO");

            Map(x => x.Multa_Carencia_Reais, "MULTA_CARENCIA");
            Map(x => x.CancelaContratoFimCarencia, "CANCEL_CONTRATO_CARENCIA");
            Map(x => x.Frequencia_Reajuste, "FREQUENCIA_REAJUSTE");
            Map(x => x.Renovar_CarenciaAuto, "RENOVAR_CARENCIA");

            Map(x => x.End_Instalacao_Igual_Principal, "END_INSTALACAO_PRINCIPAL");

            Map(x => x.Login_Radius, "LOGIN_RADIUS").Length(100);
            Map(x => x.Senha_Radius, "SENHA_RADIUS").Length(100);
            Map(x => x.VLAN_Uplink, "VLAN_UPLINK");

            Map(x => x.Data_Desbloqueio_Confianca, "DATA_DESBLOQUEIO_CONFIANCA");
            Map(x => x.Velocidade_Reduzida, "VELOCIDADE_REDUZIDA");

            Map(x => x.Nas, "NAS");
            Map(x => x.Data_Habilitado, "DATA_HABILITADO");
            Map(x => x.Data_Desabilitado, "DATE_DESABILITADO");

            Map(x => x.OrdemServicoIntegracao, "ORDEM_SERVICO_INTEGRACAO");

            Map(x => x.Cod_Escritorio, "COD_ESCRITORIO").Length(4);
            Map(x => x.Cod_Area, "COD_AREA").Length(4);
            Map(x => x.Cod_Localidade, "COD_LOCALIDADE");
            Map(x => x.Localidade, "LOCALIDADE").Length(50);
            Map(x => x.Id_Venda, "ID_VENDA").Index("IDX_CLIENTE_PLANO_PROD_VENDA");

            Map(x => x.Id_Cliente, "ID_CLIENTE");

            Map(x => x.Id_Plano, "ID_PLANO");
            Map(x => x.Id_Produto, "ID_PRODUTO");
            Map(x => x.Id_Endereco_Instalacao, "ID_ENDERECO_INSTALACAO");
            Map(x => x.Id_Endereco_Mudanca_Cliente, "ID_ENDERECO_MUDANCA_CLIENTE");
            Map(x => x.Id_Campanha, "ID_CAMPANHA");

            Map(x => x.Id_Pool_Ip, "ID_POOL_IP");

            Map(x => x.Status, "STATUS");
            Map(x => x.Motivo_Mudanca_PP, "MOTIVO_MUDANCA");

            Map(x => x.Atualiza_Ola, "ATUALIZA_OLA");

            Map(x => x.Id_Motivo_Cancelamento, "ID_MOTIVO_CANCELAMENTO");
            Map(x => x.Id_Motivo_Suspensao, "ID_MOTIVO_SUSPENSAO_CONTRATUAL");

            Map(x => x.Id_Forma_Pagamento_Cliente, "ID_FORMA_PAGTO_CLIENTE");
            Map(x => x.Id_Combo, "ID_COMBO");

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

            Map(x => x.FrequenciaDesbloqueio, "FREQUENCIADESBLOQUEIO");
            Map(x => x.QtdDiasbloqueados, "QTDDIASBLOQUEADOS");
            Map(x => x.QtdDiasdesbloqueados, "QTDDIASDESBLOQUEADOS");

            Map(x => x.Localidade_Anatel, "LOCALIDADE_ANATEL");

            Map(x => x.Id_TrocaPlano, "ID_TROCA_PLANO");
            Map(x => x.Caixa_Ftth_Reservado, "CAIXA_FTTH_RESERVA");
            Map(x => x.Porta_Ftth_Reservado, "PORTA_FTTH_RESERVA");
            Map(x => x.CodigoIntegracao, "CODIGO_INTEGRACAO");
            Map(x => x.ValorInstalacaoEntrada, "VALOR_INSTALACAO_ENTRADA");
            Map(x => x.Id_ClienteAnteriorTroca, "ID_CLI_ANTERIOR_TROCA_TITULAR");
        }
    }
}
