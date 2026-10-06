using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Departamento : BaseClass
    {
        public virtual Int16 Cod_Depto { get; set; }
        public virtual String Num_Depto { get; set; }
        public virtual String Descricao { get; set; }
    }
}
