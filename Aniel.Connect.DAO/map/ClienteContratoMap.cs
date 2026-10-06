using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class ClienteContratoMap : ClassMapBase<ClienteContrato>
    {
        public ClienteContratoMap() : base()
        {
            Table("TB_CLIENTE_CONTRATO");

            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_CLIENTE_CONTRATO");

            References(p => p.Cliente, "ID_CLIENTE").ForeignKey("FK_CLI_CT_CLIENTE");
            References(p => p.Cliente_Anexo, "ID_CLIENTE_ANEXO").Nullable().Cascade.SaveUpdate().ForeignKey("FK_CLI_CT_CLIENTE_ANX");
            References(p => p.ModeloContrato, "ID_MODELO_CONTRATO").ForeignKey("FK_CLI_CT_MODELO_CT");
            References(p => p.Endereco_Cliente, "ID_ENDERECO").ForeignKey("FK_CLI_CT_ENDERECO");
            References(p => p.Desconto, "ID_DESCONTO").ForeignKey("FK_CLI_CT_DESCONTO");
            Map(p => p.NumeroContrato, "NUMERO_CONTRATO").Length(50);
            Map(p => p.Descricao, "DESCRICAO").Length(100);

            Map(p => p.Arquivo, "ARQUIVO");
            Map(p => p.NomeArquivo, "NOME_ARQUIVO");

            Map(x => x.ContratoAutomatico, "CONTRATO_AUTOMATICO");
            Map(x => x.TemCarencia, "TEM_CARENCIA");
            Map(x => x.PeriodoCarencia, "PERIODO_CARENCIA");
            Map(x => x.MultaCarencia, "MULTA_CARENCIA");
            Map(x => x.TemReajuste, "TEM_REAJUSTE");
            Map(x => x.RenovarAutomaticamente, "RENOVAR_AUTOMATICAMENTE");
            Map(x => x.RenovaCarenciaAutomaticamente, "RENOVAR_CARENCIA").CustomType<Renova_FidelidadeEnum>();
            Map(x => x.TipoReajuste, "TIPO_REAJUSTE");
            Map(x => x.FrequenciaReajuste, "FREQUENCIA_REAJUSTE").CustomType<Frequencia_Reajuste>();
            Map(x => x.CancelaContratoAutomaticoFinalCarencia, "CANCELA_PLANO_CARENCIA");

            Map(x => x.TemProRata, "TEM_PRO_RATA");
            Map(x => x.MultaFidelidade, "MULTA_FIDELIDADE");

            Map(x => x.PercentualReajusteContratual, "PERCENTUAL_REAJUSTE");
            Map(x => x.ValorReajusteContratual, "VALOR_REAJUSTE");

            Map(x => x.UtilizaDesconto, "UTILIZA_DESCONTO");
            
            Map(x => x.ImpedirBloqueioAutomatico, "IMPEDIR_BLOQUEIO_AUTOMATICO");
            Map(x => x.ImpedirBloqueioSegunda, "IMPEDIR_BLOQUEIO_SEGUNDA");
            Map(x => x.ImpedirBloqueioTerca, "IMPEDIR_BLOQUEIO_TERCA");
            Map(x => x.ImpedirBloqueioQuarta, "IMPEDIR_BLOQUEIO_QUARTA");
            Map(x => x.ImpedirBloqueioQuinta, "IMPEDIR_BLOQUEIO_QUINTA");
            Map(x => x.ImpedirBloqueioSexta, "IMPEDIR_BLOQUEIO_SEXTA");
            Map(x => x.ImpedirBloqueioSabado, "IMPEDIR_BLOQUEIO_SABADO");
            Map(x => x.ImpedirBloqueioDomingo, "IMPEDIR_BLOQUEIO_DOMINGO");
            Map(x => x.ImpedirBloqueioFeriados, "IMPEDIR_BLOQUEIO_FERIADO");

            Map(x => x.TesteGratis, "TESTE_GRATIS");
            Map(x => x.DiasTeste, "DIAS_TESTE");

            Map(x => x.UtilizaDesbloqueioConfianca, "UTILIZA_DESBLOQUEIO_CONF");
            Map(x => x.FrequenciaDesbloqueio, "FREQUENCIA_DESBLOQUEIO").CustomType<Frequencia_Desbloqueio>();
            Map(x => x.QtdDiasDesbloqueados, "QTD_DIAS_DESBLOQUEADOS");
            Map(x => x.Data_Desbloqueio_Confianca, "DATA_DESBLOQUEIO_CONFIANCA");

            Map(x => x.Status_Contrato, "STATUS").CustomType<Status_Contrato>();
            Map(x => x.StatusAceiteContrato, "STATUS_ACEITE_CONTRATO").CustomType<Status_Aceite_Contrato>();
            Map(x => x.DataAceiteContrato, "DATA_ACEITE_CONTRATO");

            Map(x => x.IpOrigemAceite, "IP_ORIGEM_ACEITE");
            Map(x => x.UserAgentContratoAceite, "USER_AGENT_CTT");
            Map(x => x.CodigoHashContratoAceite, "HASH_CTT_ACEITE");
            Map(x => x.AdicionadoAceiteDigital, "ADC_ACEITE_DIGITAL");
            Map(x => x.UrlAceiteContrato, "URL_ACEITE_CTT");
            Map(x => x.PortaOrigemAceite, "PORTA_ACEITE_DIGITAL");

            Map(x => x.Url_Assinar_Documento, "URL_CTT_ASSINATURA");
            Map(x => x.Url_Documento_Assinado, "URL_CTT_ASSINADO");
            Map(x => x.Tipo_Integracao_Assinatura, "TIPO_ASSINATURA").CustomType<TipoIntegracaoAssinaturaEnum>();
            Map(x => x.CodIntegracao, "COD_INTEGRACAO");
            Map(x => x.IsContratoAditivo, "IS_CONTRATO_ADITIVO");            
        }
    }
}
