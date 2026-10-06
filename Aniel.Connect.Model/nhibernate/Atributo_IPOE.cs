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
    public class Atributo_IPOE : BaseModel
    {
        public virtual String Nome { get; set; }
        public virtual String Atributo_Autenticacao { get; set; }
        public virtual String Atributo_Bloqueio { get; set; }
        public virtual String Atributo_ReducaoVelocidade { get; set; }
        public virtual String Atributo_VelocidadeNormal { get; set; }
    }
}
