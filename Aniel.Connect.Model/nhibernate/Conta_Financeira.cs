using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Conta_Financeira : BaseModel
    {
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
    }
}
