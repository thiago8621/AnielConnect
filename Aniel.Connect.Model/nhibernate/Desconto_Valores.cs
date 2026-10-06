using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Desconto_Valores : BaseModel
    {
        public virtual Int16? MesInicial { get; set; }
        public virtual Int16? MesFinal { get; set; }
        public virtual Double? Valor { get; set; }
        public virtual Double? Percentual { get; set; }
        public virtual Desconto Desconto { get; set; }

    }
}









