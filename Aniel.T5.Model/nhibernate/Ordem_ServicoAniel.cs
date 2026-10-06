using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Microsoft.Diagnostics.Runtime.DacInterface;
using NHibernate.Util;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Ordem_ServicoAniel
    {
        public virtual String CodOs { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Equipe { get; set; }
        public virtual DateTime? InicPrev { get; set; }
        public virtual DateTime? TermPrev { get; set; }
        public virtual DateTime? InicReal { get; set; }
        public virtual DateTime? TermReal { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Ordem_ServicoAniel other = obj as Ordem_ServicoAniel;
            if (other == null)
                return false;

            if (CodCt == other.CodCt && Projeto == other.Projeto && CodOs == other.CodOs)
                return true;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return $"{CodCt} | {Projeto} | {CodOs}".GetHashCode();
        }
    }
}
