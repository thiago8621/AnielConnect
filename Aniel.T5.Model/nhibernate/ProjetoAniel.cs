using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class ProjetoAniel
    {
        public virtual String CodCt { get; set; }

        public virtual String Codigo { get; set; }
        public virtual String Localidade { get; set; }
        public virtual DateTime? IniPrev { get; set; }
        public virtual DateTime? TermPrev { get; set; }
        public virtual DateTime? IniReal { get; set; }
        public virtual DateTime? TermReal { get; set; }
        public virtual DateTime? TermReal2 { get; set; }


        public virtual Int16 CodEmpresa { get; set; }
        public virtual Int16 CodFilial { get; set; }

        public virtual String CodCt_Regulador { get; set; }
        public virtual String Situacao { get; set; }
        public virtual String Projeto_Regulador { get; set; }
        public virtual String CodEscritorio { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ProjetoAniel other = obj as ProjetoAniel;
            if (other == null)
                return false;
            if (CodCt == other.CodCt && Codigo == other.Codigo)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
