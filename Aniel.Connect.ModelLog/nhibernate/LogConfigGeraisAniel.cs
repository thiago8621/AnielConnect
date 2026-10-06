using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class LogConfigGeraisAniel : LogGenerico<Int64>
    {
        public LogConfigGeraisAniel()
        {
        }
        public virtual Boolean RoteirizarSemLatLong { get; set; }
        public virtual Boolean RoteirizarPorCx { get; set; }

    }
}
