using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;

namespace Aniel.T5.Model.nhibernate
{
    public class Codigo_ComplementarAniel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String CodMat { get; set; }
        public virtual String CodCpl { get; set; }

        public virtual String Localizacao { get; set; }

        public virtual String Sinal_Ativo { get; set; }
        public virtual String Mensagem_Erro { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Codigo_ComplementarAniel other = obj as Codigo_ComplementarAniel;
            if (other == null)
                return false;
            if (CodCt == other.CodCt && Projeto == other.Projeto && CodMat == other.CodMat && CodCpl == other.CodCpl)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
