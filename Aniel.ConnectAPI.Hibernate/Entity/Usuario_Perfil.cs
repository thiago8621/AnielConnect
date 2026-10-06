using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Usuario_Perfil : BaseClass
    {
        public virtual Usuario Usuario_Aniel { get; set; }
        public virtual Perfil_Usuario Perfil_Usuario { get; set; }
    }
}
