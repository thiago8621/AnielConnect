using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class MaterialOs : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Plano Plano { get; set; }
        public virtual Produto Produto { get; set; }    
    }
}
