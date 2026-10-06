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
    public class Mensagem_Sistema : BaseModel
    {
        public virtual String Titulo { get; set; }
        public virtual String Mensagem { get; set; }
        public virtual Status_Default Status { get; set; }
        
    }
}
