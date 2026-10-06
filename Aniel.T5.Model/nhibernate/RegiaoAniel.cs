using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class RegiaoAniel
    {
        public virtual Int16 Cod_Regiao { get; set; }
        public virtual String Nome { get; set; }
    }
}
