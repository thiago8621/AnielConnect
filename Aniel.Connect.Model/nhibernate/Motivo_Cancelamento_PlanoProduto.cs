using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Motivo_Cancelamento_PlanoProduto : BaseModel
    {
        public virtual String Motivo { get; set; }
        public virtual Status_Default? Status { get; set; }
    }
}
