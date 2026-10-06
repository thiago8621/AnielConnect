using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Agente_Localidade : BaseModel
    {
        public virtual Perfil_Agente Agente { get; set; }
        public virtual String Cod_Escritorio  { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Desc_Localidade { get; set; }
        public virtual Boolean Todas_Localidades { get; set; }
        public virtual Boolean Todas_Areas { get; set; }
    }
}
