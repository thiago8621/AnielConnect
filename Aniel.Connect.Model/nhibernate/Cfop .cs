using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cfop : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
    }
}
