using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Campanha_AreaCobertura : BaseModel
    {
        //public virtual Boolean Definir_Area_Cobertura { get; set; }
        public virtual Int64? Area_Cobertura { get; set; }
        public virtual Campanha Campanha { get; set; }

    }
}




