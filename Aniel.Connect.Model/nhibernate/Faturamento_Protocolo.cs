using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Faturamento_Protocolo : BaseModel
    {
        public virtual String Protocolo { get; set; }
        //public virtual Fechamento Fechamento { get; set; }
        public virtual DateTime? Data_Fechamento { get; set; }
        //public virtual Status_Faturamento Status { get; set; }
        public virtual DateTime? Data_Inicial { get; set; }
        public virtual DateTime? Data_Final { get; set; }
        public virtual DateTime? Data_Vencimento { get; set; }
        public virtual Tipo_Faturamento Parcial { get; set; }
        public virtual Boolean Tem_Valor_Avulso { get; set; }
        public virtual Boolean Automatico { get; set; }
        public virtual Double? Valor_Avulso { get; set; }
        public virtual Int64? Recorrencia { get; set; }
        public virtual Int64? Quantidade_Recorrencia { get; set; }
    }
}

