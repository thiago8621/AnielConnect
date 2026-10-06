using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class FilialAniel : BaseModel
    {
        public virtual Int64? Id_Filial_Connect { get; set; }
        public virtual Int16 CodFilial { get; set; }
        public virtual Int16 CodEmpresa { get; set; }
        public virtual String Nome { get; set; }
        public virtual string CodFilialAux { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            FilialAniel other = obj as FilialAniel;
            if (other == null)
                return false;
            if (CodFilial == other.CodFilial && CodEmpresa == other.CodEmpresa)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (CodFilial + "|" + CodEmpresa).GetHashCode();
        }
    }
}
