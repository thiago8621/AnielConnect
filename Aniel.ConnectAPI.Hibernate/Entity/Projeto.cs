using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Projeto : BaseClass
    {
        public virtual Contrato Contrato { get; set; }

        public virtual String Codigo { get; set; }
        public virtual String Localidade { get; set; }
        public virtual DateTime? IniPrev { get; set; }
        public virtual DateTime? TermPrev { get; set; }
        public virtual DateTime? IniReal { get; set; }
        public virtual DateTime? TermReal { get; set; }
        public virtual DateTime? TermReal2 { get; set; }

        
        public virtual Filial Filial { get; set; }
        public virtual Projeto Projeto_Regulador { get; set; }
    }
}
