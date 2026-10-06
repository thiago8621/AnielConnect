using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class CadastroCartaoEmail : BaseModel
    {
        public virtual long IdCliente { get; set; }
        public virtual long? IdForma { get; set; }
        public virtual DateTime DataExpiracao { get; set; }
        public virtual string Link { get; set; }
    }
}
