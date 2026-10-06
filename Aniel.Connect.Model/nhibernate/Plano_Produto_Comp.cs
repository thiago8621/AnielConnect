using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Plano_Produto_Comp : BaseModel
    {
        public virtual Double? ValorFixo { get; set; }
        public virtual Double? ValorPercentual { get; set; }
        public virtual Double? ValorTotal { get; set; }
        public virtual ProdutosComp ProdutosComp { get; set; }
        public virtual Plano Plano { get; set; }
    }
}
