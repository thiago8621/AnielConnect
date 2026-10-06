using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Status_Atend_Doc_Prod : BaseClass
    {
        public virtual Int16 Status { get; set; }
        public virtual String Descricao { get; set; }
    }
}
