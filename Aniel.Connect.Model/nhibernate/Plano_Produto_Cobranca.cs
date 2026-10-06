using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
   public class Plano_Produto_Cobranca : BaseModel
    {
        public virtual Plano Plano { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Cobranca_Servico Cobranca_Servico { get; set; }

    }
}
