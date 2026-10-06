using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Area : BaseModel
    {
        public virtual String Cod_Area { get; set; }
        public virtual String Descricao { get; set; }

        public virtual Escritorio Escritorio { get; set; }
    }
}
