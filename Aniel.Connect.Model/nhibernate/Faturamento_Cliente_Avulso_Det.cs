using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Faturamento_Cliente_Avulso_Det : BaseModel
    {
        public virtual DateTime? DataHora_Abertura { get; set; }
        public virtual DateTime? DataHora_Fechamento { get; set; }
        public virtual Double? Valor_Avulso { get; set; }
        public virtual Boolean? UtilizaRecorrencia { get; set; }
        public virtual Int64? NumeroSeqRecorrencia { get; set; }
        public virtual Faturamento_Protocolo Faturamento_Protocolo { get; set; }
        public virtual Faturamento_Cliente Faturamento_Cliente { get; set; }

    }
}
