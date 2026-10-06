using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Perfil_Equipe : BaseClass
    {
        public virtual Int32 Cod_Perfil_Equipe { get; set; }
        public virtual String Nome { get; set; }
    }
}
