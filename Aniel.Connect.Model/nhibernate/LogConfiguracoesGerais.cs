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

namespace Aniel.Connect.Model.nhibernate
{
    public class LogConfiguracoesGerais : BaseModel
    {
        public virtual Boolean RoteirizarSemLatLong { get; set; }
        public virtual Boolean RoteirizarPorCx { get; set; }
    
        
    }
}
