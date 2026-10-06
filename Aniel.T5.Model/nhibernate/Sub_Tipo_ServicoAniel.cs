using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Sub_Tipo_ServicoAniel
    {
        public virtual Int16 Codigo_Tipo { get; set; }
        public virtual String Codigo_Sub_Tipo { get; set; }
        public virtual String Descricao { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Sub_Tipo_ServicoAniel other = obj as Sub_Tipo_ServicoAniel;
            if (other == null)
                return false;
            if (Codigo_Tipo == other.Codigo_Tipo && Codigo_Sub_Tipo == other.Codigo_Sub_Tipo)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
