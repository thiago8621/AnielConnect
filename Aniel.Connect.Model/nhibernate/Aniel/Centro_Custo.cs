using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Centro_Custo : BaseModel
    {
        public virtual Int32 Cod_Ccusto { get; set; }
        public virtual String Descricao { get; set; }
    }
}
