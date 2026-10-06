using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Escritorio : BaseClass
    {
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Descricao { get; set; }
    }
}
