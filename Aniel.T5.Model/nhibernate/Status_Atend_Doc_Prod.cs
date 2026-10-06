using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Status_Atend_Doc_Prod : BaseModel
    {
        public virtual Int16 Status { get; set; }
        public virtual String Descricao { get; set; }
    }
}
