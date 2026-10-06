
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Modelo_ContratoMap : ClassMapBase<Modelo_Contrato>
    {
        public Modelo_ContratoMap() : base()
        {
            Table("TB_MODELO_CONTRATO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MODELO_CONTRATO");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Arquivo_Doc, "ARQUIVO_DOC").Length(100);
            Map(x => x.Extensao_Arquivo, "EXTENSAO_ARQUIVO").Length(5);

            Map(x => x.Contrato_Automatico, "CONTRATO_AUTOMATICO");
            Map(x => x.Tem_Carencia, "TEM_CARENCIA");
            Map(x => x.Periodo_Carencia, "PERIODO_CARENCIA");
            Map(x => x.Multa_Carencia, "MULTA_CARENCIA");
            Map(x => x.Cancela_Contrato_Automatico_Final_Carencia, "CANCELA_PLANO_CARENCIA");
            Map(x => x.Tem_Reajuste, "TEM_REAJUSTE");
            Map(x => x.Tipo_Reajuste, "TIPO_REAJUSTE");
            Map(x => x.Renova_Carencia_Automaticamente, "RENOVAR_CARENCIA").CustomType<Renova_FidelidadeEnum>();

            Map(x => x.Frequencia_Reajuste, "FREQUENCIA_REAJUSTE").CustomType<Frequencia_Reajuste>();

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.Tem_Pro_Rata, "TEM_PRO_RATA");
            Map(x => x.MultaFidelidade, "MULTA_FIDELIDADE");
            Map(x => x.RenovarAutomaticamente, "RENOVAR_AUTOMATICAMENTE");
            Map(x => x.DisponibilizaTelefonia, "DISPONIBILIZA_TELEFONIA");
            Map(x => x.IntegracaoTelefonia, "INTEGRACAO_TELEFONIA").CustomType<IntegracaoTelefonia>();
            Map(x => x.DidFormatoCobrar, "DID_FORMATO_COBRAR").CustomType<DidFormatoCobrar>();
            Map(x => x.QuantidadeDid, "QUANTIDADE_DID");
            Map(x => x.FormatoCobrancaTelefonia, "FORMATO_COBRANCA_TELEFONIA").CustomType<Formato_Cobranca>();

            Map(x => x.ImpedirBloqueio_Automatico, "IMPEDIR_BLOQUEIO_AUTOMATICO");
            Map(x => x.ImpedirBloqueio_Sexta, "IMPEDIR_BLOQUEIO_SEXTA");
            Map(x => x.ImpedirBloqueio_Sabado, "IMPEDIR_BLOQUEIO_SABADO");
            Map(x => x.ImpedirBloqueio_Domingo, "IMPEDIR_BLOQUEIO_DOMINGO");
            Map(x => x.ImpedirBloqueio_Feriados, "IMPEDIR_BLOQUEIO_FERIADO");

            Map(x => x.ImpedirBloqueio_Segunda, "IMPEDIR_BLOQUEIO_SEGUNDA");
            Map(x => x.ImpedirBloqueio_Terca, "IMPEDIR_BLOQUEIO_TERCA");
            Map(x => x.ImpedirBloqueio_Quarta, "IMPEDIR_BLOQUEIO_QUARTA");
            Map(x => x.ImpedirBloqueio_Quinta, "IMPEDIR_BLOQUEIO_QUINTA");
            
            Map(x => x.Utiliza_Desconto, "UTILIZA_DESCONTO");
            References(p => p.Desconto, "ID_DESCONTO").ForeignKey("FK_ID_DESCONTO");

            Map(x => x.QuantidadeDiasBloqueio, "QUANTIDADE_DIAS_BLOQUEIO");
            Map(x => x.BloquearNumeroDias, "BLOQUEAR_NUMERO_DIAS");
            Map(x => x.BloquearAtrasarRenegociadas, "BLOQUEAR_ATRASAR_RENEGOCIADAS");
            Map(x => x.MaximoTitulosAberto, "MAXIMO_TITULOS_ABERTO");
            Map(x => x.FrequenciaPermitida, "ID_FREQUENCIA_PERMITIDA");

            Map(x => x.TesteGratis, "TESTEGRATIS");
            Map(x => x.DiasTeste, "DIASTESTE");
            Map(x => x.Percentual_Reajuste_Contratual, "PERCENTUAL_REAJUSTE");
            Map(x => x.Valor_Reajuste_Contratual, "VALOR_REAJUSTE");
            Map(x => x.UtilizaDesbloqueioConfianca, "UTILIZADESBLOQUEIOCONF");
            Map(x => x.BloqueioAutomaticoDebito, "BLOQUEIO_AUTOMATICO");
            Map(x => x.FrequenciaDesbloqueio, "FREQUENCIADESBLOQUEIO").CustomType<Frequencia_Desbloqueio>();
            Map(x => x.QtdDiasdesbloqueados, "QTDDIASDESBLOQUEADOS");
            Map(x => x.CobrarInstalacao, "COBRAR_INSTALACAO");
            Map(x => x.Nome_Combo, "NOME_COMBO").Length(100);
            Map(x => x.Especifico_Combo, "PLANO_ESPECIFICO_COMBO");
            Map(x => x.Item_selecionado_Instalacao, "ITEM_SELECIONADO_INSTALACAO");
            Map(x => x.CodigoIntegracao, "CODIGO_INTEGRACAO");
            Map(x => x.Observacao_ModeloContrato, "OBSERVACAO_MODELO_CONTRATO");
            //Fidelidade
            //Map(x => x.Tem_Fidelidade, "TEM_FIDELIDADE");
            //Map(x => x.Multa_Fidelidade, "MULTA_FIDELIDADE");
            //Map(x => x.Periodo_Fidelidade, "PERIODO_FINALIDADE");
        }
    }
}
