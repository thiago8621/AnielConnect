using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Projeto : BaseModel
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
