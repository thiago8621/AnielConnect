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
using System.Windows.Forms.VisualStyles;

namespace Aniel.T5.Model.nhibernate
{
    public class MunicipioAniel
    {
        public virtual short CodMunicipio { get; set; }
        public virtual String Municipio { get; set; }
        public virtual int CodIBGE { get; set; }
        public virtual String Uf { get; set; }
    
    }
}
