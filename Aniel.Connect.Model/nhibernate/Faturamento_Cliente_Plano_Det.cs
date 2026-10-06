using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Faturamento_Cliente_Plano_Det : BaseModel
    {
        public virtual Faturamento_Cliente_Plano Faturamento_Cliente_Plano { get; set; }
        public virtual DateTime? DataHora_Abertura { get; set; }
        public virtual DateTime? DataHora_Fechamento { get; set; }
        public virtual Double? Valor_Periodo { get; set; }
    }
}
