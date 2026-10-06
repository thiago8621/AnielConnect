using Aniel.Connect.Model.enumerado;
using System;
using Sinapse.Model.nhibernate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Model.nhibernate
{
    public class Modelo_Contrato : BaseModel
    {
        public virtual String Arquivo_Doc { get; set; }
        public virtual String Extensao_Arquivo { get; set; }
        public virtual String Descricao { get; set; }

        public virtual Boolean Contrato_Automatico { get; set; }
        public virtual Boolean Tem_Carencia { get; set; }
        public virtual Int64? Periodo_Carencia { get; set; }
        public virtual Double? Multa_Carencia { get; set; }
        public virtual Boolean Tem_Reajuste { get; set; }
        public virtual Tipo_Reajuste_Contratual? Tipo_Reajuste { get; set; }
        public virtual Frequencia_Reajuste? Frequencia_Reajuste { get; set; }
        public virtual Boolean Cancela_Contrato_Automatico_Final_Carencia { get; set; }
        public virtual Renova_FidelidadeEnum? Renova_Carencia_Automaticamente { get; set; }


        public virtual Boolean Tem_Pro_Rata { get; set; }
        public virtual Boolean MultaFidelidade { get; set; }
        public virtual Boolean RenovarAutomaticamente { get; set; }

        public virtual Boolean DisponibilizaTelefonia { get; set; }

        public virtual Status_Default? Status { get; set; }
        public virtual Status_Contrato? Status_Contrato { get; set; }
        public virtual IntegracaoTelefonia? IntegracaoTelefonia { get; set; }
        public virtual DidFormatoCobrar? DidFormatoCobrar { get; set; }
        public virtual Int64? QuantidadeDid { get; set; }
        public virtual Formato_Cobranca? FormatoCobrancaTelefonia { get; set; }
        public virtual Boolean Especifico_Combo { get; set; }
        public virtual String Nome_Combo { get; set; }
        public virtual Int64? Item_selecionado_Instalacao { get; set; }

        //Impedir_Bloqueio
        public virtual Boolean ImpedirBloqueio_Automatico { get; set; }
        public virtual Boolean ImpedirBloqueio_Segunda { get; set; }
        public virtual Boolean ImpedirBloqueio_Terca { get; set; }
        public virtual Boolean ImpedirBloqueio_Quarta { get; set; }
        public virtual Boolean ImpedirBloqueio_Quinta { get; set; }
        public virtual Boolean ImpedirBloqueio_Sexta { get; set; }
        public virtual Boolean ImpedirBloqueio_Sabado { get; set; }
        public virtual Boolean ImpedirBloqueio_Domingo { get; set; }
        public virtual Boolean ImpedirBloqueio_Feriados { get; set; }
        //Desconto
        public virtual Boolean Utiliza_Desconto { get; set; }
        public virtual Desconto Desconto { get; set; }
        //Migrado Rcentemente
        //Teste Grátis
        public virtual Boolean TesteGratis { get; set; }
        public virtual Int16? DiasTeste { get; set; }
        //Desbloqueio_Confianca
        public virtual Boolean UtilizaDesbloqueioConfianca { get; set; }
        public virtual Boolean BloqueioAutomaticoDebito { get; set; }
        public virtual Frequencia_Desbloqueio? FrequenciaDesbloqueio { get; set; }
        public virtual Int16 QtdDiasdesbloqueados { get; set; }
        public virtual Boolean CobrarInstalacao { get; set; }
        public virtual Double Percentual_Reajuste_Contratual { get; set; }
        public virtual Double Valor_Reajuste_Contratual { get; set; }
        public virtual int QuantidadeDiasBloqueio { get; set; }
        public virtual int BloquearNumeroDias { get; set; }
        public virtual int BloquearAtrasarRenegociadas { get; set; }
        public virtual int MaximoTitulosAberto { get; set; }
        public virtual long FrequenciaPermitida { get; set; }
        public virtual String CodigoIntegracao { get; set; }
        public virtual String Observacao_ModeloContrato { get; set; }
    }
}
