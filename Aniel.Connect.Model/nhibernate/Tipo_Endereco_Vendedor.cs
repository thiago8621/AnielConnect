using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
   public class Tipo_Endereco_Vendedor : BaseModel
    {
        public virtual String Pais { get; set; }
        public virtual String Uf { get; set; }
         public virtual String Cidade { get; set; }
 
        public virtual Perfil_Vendedor Perfil_Vendedor { get; set; }

    }
}
