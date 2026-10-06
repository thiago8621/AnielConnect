using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Perfil_Usuario : BaseClass
    {
        public virtual Int32 Cod_Perfil_Usuario { get; set; }
        public virtual String Descricao{ get; set; }
    }
}
