using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Credito : BaseModel
    {
        public virtual String Motivo_Credito { get; set; }
        public virtual String Documento { get; set; }
        public virtual DateTime? Data_Fechamento { get; set; } // public virtual DateTime? Data_Pago { get; set; }
        public virtual Double? Valor { get; set; } // public virtual Double? Valor_Pago { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Faturamento_Cliente Faturamento_Cliente { get; set; }
        public virtual Status_Credito? Status { get; set; }
    }
}
