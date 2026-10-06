using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
   public class Tipo_Pessoa_Vendedor : BaseModel
    {
        public virtual String Tipo_Pessoa { get; set; }
 
        public virtual Perfil_Vendedor Perfil_Vendedor { get; set; }

    }
}
