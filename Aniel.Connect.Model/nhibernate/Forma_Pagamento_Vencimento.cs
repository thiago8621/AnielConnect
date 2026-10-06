using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Forma_Pagamento_Vencimento : BaseModel
    {

        public virtual Forma_Pagamento Forma_Pagamento { get; set; }
        public virtual Vencimento Vencimento { get; set; }
    }
}
