using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Origem_Atendimento : BaseModel
    {
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
        
    }
}
