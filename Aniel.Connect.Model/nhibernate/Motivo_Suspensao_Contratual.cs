using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Motivo_Suspensao_Contratual : BaseModel
    {
        public virtual String Motivo { get; set; }
        public virtual Status_Motivo_Para Motivo_Para { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual String Observacao_Motivo_Suspensao_Plano_Cliente_Outros { get; set; }
    }
}
