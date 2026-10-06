using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Vencimento : BaseModel
    {

        public virtual Int16 DiaFechamento { get; set; }
        public virtual Int16 DiaVencimento { get; set; }
        public virtual Int16 DiaFaturamento { get; set; }
        public virtual Boolean GeraFaturamento_Auto { get; set; }
        public virtual Status_Default? Status { get; set; }

    }
}
