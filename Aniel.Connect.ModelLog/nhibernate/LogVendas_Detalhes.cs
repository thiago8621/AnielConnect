using Sinapse.Model.enumerados;
using Sinapse.ModelLog;
using System;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class LogVendas_Detalhes : LogGenerico<Int64>
    {
        public LogVendas_Detalhes()
        {
            
        }
        public virtual Int64 Id_Cliente_Plano_Produto { get; set; }
        public virtual String VelocidadeDownlaod { get; set; }
        public virtual String Caixa_Ftth_Reservado { get; set; }
        public virtual String Porta_Ftth_Reservado { get; set; }
        public virtual String VelocidadeNominal { get; set; }

        public virtual String Nas { get; set; }
        public virtual String Login_Radius { get; set; }
        public virtual String Senha_Radius { get; set; }
        public virtual DateTime? Data_Habilitado { get; set; }
        public virtual DateTime? Data_Desabilitado { get; set; }

        public virtual Int64? Id_ClienteContrato { get; set; }

        public virtual Int64? Id_Venda { get; set; }
        public virtual Int64? Id_Campanha { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_Plano { get; set; }
        public virtual Int64? Id_Produto { get; set; }
        public virtual Int64? Id_Combo { get; set; }
        public virtual Status_PlanoProduto? Status { get; set; }
        public virtual Int32? Motivo_Mudanca_PP { get; set; }
        public virtual Int32? Momento_Cobra_Instalacao { get; set; }

        public virtual DateTime? Data_Desbloqueio_Confianca { get; set; }
        public virtual Boolean Velocidade_Reduzida { get; set; }

        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual Int32? Localidade_Anatel { get; set; }
        public virtual Int32? Regime { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual Int64? Id_Endereco_Instalacao { get; set; }
        public virtual Int64? Id_Endereco_Mudanca_Cliente { get; set; }
        public virtual Boolean End_Instalacao_Igual_Principal { get; set; }
        public virtual Boolean End_Instalacao_Igual_Prospecto { get; set; }

        public virtual Double? Valor_Final_Total { get; set; }
        public virtual Double? Valor_Real { get; set; }
        public virtual Double? Valor_Final { get; set; }
        public virtual Double? Valor_Instalacao_Real { get; set; }
        public virtual Double? Valor_Instalacao_Final { get; set; }

        public virtual Int64? Parcelas_Instalacao { get; set; }
        public virtual Int64? Dias_Teste_Gratis { get; set; }
        public virtual Boolean Periodo_Carencia { get; set; }
        public virtual Int64? Meses_Carencia { get; set; }
        public virtual Double? Multa_Carencia_Reais { get; set; }
        public virtual Boolean CancelaContratoFimCarencia { get; set; }
        public virtual Int32? Renovar_CarenciaAuto { get; set; }
        public virtual Boolean Tem_Reajuste { get; set; }
        public virtual Int32? Frequencia_Reajuste { get; set; }
        public virtual String Multa_Rescisao { get; set; }

        public virtual Boolean? Atualiza_Ola { get; set; }
        public virtual Boolean CobrarInstalacaoEntrada { get; set; }
        public virtual Int64? Id_Pool_Ip { get; set; }
        public virtual Int64? Id_NasObjeto { get; set; }
        public virtual Int64? Id_Motivo_Cancelamento { get; set; }
        public virtual Int64? Id_Motivo_Suspensao { get; set; }
        public virtual Int64? Id_Forma_Pagamento_Cliente { get; set; }

        public virtual Int64? Id_ElementoRede_Porta { get; set; }
        public virtual Boolean ReducaoVelocidade { get; set; }
        public virtual Int16 QtdDiasAtraso { get; set; }

        //campos adicionais 
        public virtual String VelocidadeDownloadReduzida { get; set; }
        public virtual String VelocidadeUploadReduzida { get; set; }
        public virtual String JsonTelefonia { get; set; }
        public virtual Boolean Telefonia { get; set; }
        public virtual Boolean ImpedirBloqueio_Automatico { get; set; }
        public virtual Boolean ImpedirBloqueio_Sexta { get; set; }
        public virtual Boolean ImpedirBloqueio_Sabado { get; set; }
        public virtual Boolean ImpedirBloqueio_Domingo { get; set; }
        public virtual Boolean ImpedirBloqueio_Feriados { get; set; }
        public virtual Boolean UtilizaDesbloqueioConfianca { get; set; }
        public virtual Boolean UtilizaBloqueioDebito { get; set; }
        public virtual Int32? FrequenciaDesbloqueio { get; set; }
        public virtual Int16 QtdDiasbloqueados { get; set; }
        public virtual Int16 QtdDiasdesbloqueados { get; set; }
        public virtual long? Id_TrocaPlano { get; set; }
        public virtual Int16? VLAN_Uplink { get; set; }
        public virtual String CodigoIntegracao { get; set; }
        public virtual bool FlagTrocaPlano { get; set; }
        public virtual bool FlagVendaFormatoAvulso { get; set; }
        public virtual Double? ValorInstalacaoEntrada { get; set; }
        public virtual Int64? Id_ClienteAnteriorTroca { get; set; }
        public virtual string OrdemServicoIntegracao { get; set; }
        public virtual string CodContratoIntegracao { get; set; }
    }
}
