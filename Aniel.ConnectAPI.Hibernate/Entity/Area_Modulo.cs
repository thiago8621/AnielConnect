using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    public class Area_Modulo : BaseClass
    {
        public virtual String Sigla_Modulo { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Sub_Acesso { get; set; }
    }
}
