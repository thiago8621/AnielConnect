using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class SLA_Atendimento_PlanoProduto : BaseModel
    {

        public virtual Plano Plano { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual SLA_Atendimento SLA_Atendimento { get; set; }
      

    }
}
