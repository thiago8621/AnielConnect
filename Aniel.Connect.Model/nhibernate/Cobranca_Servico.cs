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
    public class Cobranca_Servico : BaseModel 
    {
        public virtual string Descricao { get; set; }
        public virtual Escolha_Cobranca? Escolha_Cobranca { get; set; }       
    
        public virtual Status_Default Status { get; set; }
    }
}
