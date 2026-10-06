using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Perfil_Usuario_Acesso : BaseClass
    {
        public virtual Area_Modulo Area_Modulo { get; set; }
        public virtual Perfil_Usuario Perfil_Usuario { get; set; }
    }
}
