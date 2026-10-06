using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Desconto_Meio_Captacao : BaseModel
    {
        public virtual Desconto Desconto { get; set; }
        public virtual Meio_Captacao Meio_Captacao { get; set; }

    }
}
