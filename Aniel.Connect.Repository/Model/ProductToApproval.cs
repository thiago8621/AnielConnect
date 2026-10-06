using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model
{
    public class ProductToApproval
    {
        public virtual String IdProduct { get; set; }
        public virtual String MAC { get; set; }
        public virtual String Serie { get; set; }
        public virtual String ComplementarAuxiliar { get; set; }
        public virtual Int64? Quantity { get; set; }
    }
}
