using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Acesso : BaseClass
    {
        public virtual Area_Modulo Area_Modulo { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
