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
    public class LogPerfilAgendamentoAniel : LogGenerico<Int64>
    {
        public LogPerfilAgendamentoAniel()
        {
        }

        public virtual String Descricao { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Natureza { get; set; }
    }


}
 