using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Caderno_EncargosAniel
    {
        public virtual String Unidade_Custo { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Unidade { get; set; }
        public virtual String Classe_MO { get; set; }
        public virtual Double? Quantidade { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Caderno_EncargosAniel other = obj as Caderno_EncargosAniel;
            if (other == null)
                return false;
            if (Unidade_Custo == other.Unidade_Custo && Codigo == other.Codigo)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (Unidade_Custo + "|" + Codigo).GetHashCode();
        }
    }

   
}
