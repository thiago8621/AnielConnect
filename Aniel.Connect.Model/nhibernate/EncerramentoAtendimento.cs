using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
   public class EncerramentoAtendimento: BaseModel
    {
      
       
        public virtual string Contrato { get; set; }
        public virtual string Cod_Encerramento { get; set; }
        public virtual long? IdFluxoEtapa { get; set; }
        public virtual FluxoAtendimento FluxoAtendimento { get; set; }

        
    }
}
