using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;


namespace Aniel.T5.Model.nhibernate
{
    public class Motivo_Ausencia : BaseModel
    {
        public virtual string Motivo { get; set; }
        public virtual Status_Default Status { get; set; }
    }
}

