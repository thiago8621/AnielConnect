using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Desconto_Regioes : BaseModel
    {
        public virtual String Pais { get; set; }
        public virtual String Estado { get; set; }
        public virtual String Cidade { get; set; }
        //public virtual Boolean CampanhaRegioes { get; set; }
        public virtual Desconto Desconto { get; set; }

    }
}









