using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Slot : BaseModel
    {
        public virtual Int64? NumSlot { get; set; }
        public virtual string LineProfile { get; set; }
        public virtual String Descricao { get; set; }
        public virtual OLT OLT { get; set; }
        public virtual Status_Default Status { get; set; }

        #region CAMPOS ANTIGOS (mantidos para compatibilidade temporaria)

        public virtual Int64 SlotID { get; set; }

        #endregion
    }
}
