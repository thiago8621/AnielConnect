using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Sub_Tipo_Servico : BaseClass
    {
        public virtual Tipo_Servico_Equipe Tipo_Servico_Equipe { get; set; }

        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }        
    }
}
