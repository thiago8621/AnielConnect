using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.mongo
{
    public class GeoJsonModel
    {
        public virtual string type { get; set; }
        
        public virtual Object coordinates { get; set; }
    }
}
