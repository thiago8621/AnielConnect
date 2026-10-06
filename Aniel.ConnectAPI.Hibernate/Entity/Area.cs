using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Area : BaseClass
    {
        public virtual String Cod_Area { get; set; }
        public virtual String Descricao { get; set; }

        public virtual Escritorio Escritorio { get; set; }
    }
}
