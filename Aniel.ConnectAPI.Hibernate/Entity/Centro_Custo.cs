using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Centro_Custo : BaseClass
    {
        public virtual Int32 Codigo { get; set; }

        public virtual String Cod_Aux { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Int32 Status { get; set; }
    }
}
