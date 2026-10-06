using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Localidade : BaseClass
    {
        public virtual String LocalidadeStr { get; set; }
        public virtual Int32 Cod_Localidade { get; set; }

        public virtual Area Area { get; set; }
    }
}
