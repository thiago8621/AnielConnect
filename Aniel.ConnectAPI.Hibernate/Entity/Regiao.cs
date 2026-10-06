using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Regiao : BaseClass
    {
        public virtual Int16 Cod_Regiao { get; set; }
        public virtual String Nome { get; set; }
    }
}
