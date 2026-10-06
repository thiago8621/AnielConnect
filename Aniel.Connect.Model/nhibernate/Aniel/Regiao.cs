using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Regiao : BaseModel
    {
        public virtual Int16 Cod_Regiao { get; set; }
        public virtual String Nome { get; set; }
    }
}
