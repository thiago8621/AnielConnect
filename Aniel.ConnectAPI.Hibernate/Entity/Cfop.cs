using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{    
    public class Cfop : BaseClass
    {
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
    }
}
