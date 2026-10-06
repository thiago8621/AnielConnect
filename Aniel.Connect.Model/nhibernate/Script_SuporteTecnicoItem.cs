using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Script_SuporteTecnicoItem : BaseModel
    {
        public virtual String Informacao { get; set; }

        public virtual Int64 Ordem { get; set; }

        public virtual Script_SuporteTecnico Script { get; set; }
    }
}
