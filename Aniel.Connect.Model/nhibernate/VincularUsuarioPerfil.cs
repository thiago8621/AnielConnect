using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
   public class VincularUsuarioPerfil : BaseModel
    {
        public virtual String Secao { get; set; }
         public virtual String Perfil_Vinculado { get; set; }

        public virtual String Tipo_Usuario { get; set; }

 
        public virtual IdentityUser IdentityUser { get; set; }
        public virtual Perfil_DashBoard_Connect Perfil_DashBoard_Connect { get; set; }



    }
}
