using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Localidade : BaseModel
    {
        public virtual String LocalidadeStr { get; set; }
        public virtual Int32 Cod_Localidade { get; set; }

        public virtual Area Area { get; set; }
    }
}
