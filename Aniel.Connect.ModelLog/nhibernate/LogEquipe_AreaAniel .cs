using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class LogEquipe_AreaAniel : BaseLog
    {
        public virtual string Escritorio { get; set; }
        public virtual string Area { get; set; }
        public virtual string Localidade { get; set; }
        public virtual Int16 Nivel { get; set; }
        public virtual Boolean Todas_Localidades { get; set; }
        public virtual Boolean Todas_Areas { get; set; }
    }
}
