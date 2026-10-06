using Aniel.Connect.Model.enumerado;
using System;
using Sinapse.Model.nhibernate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Model.nhibernate
{
    public class Acoes_Prova_Vida_Termo : BaseModel
    {
        public virtual String Ordem { get; set; }
        public virtual String Valor { get; set; }
    }
}
