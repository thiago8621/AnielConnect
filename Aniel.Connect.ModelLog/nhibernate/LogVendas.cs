using Sinapse.Model;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class LogVendas : LogGenerico<Int64>
    {
        public virtual int StatusVenda { get; set; }
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }
        public virtual Int64? Id_Forma_Pagamento_Cliente { get; set; }
        public LogVendas()
        {
           
        }

    }
}
