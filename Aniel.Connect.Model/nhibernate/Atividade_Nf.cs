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
    public class Atividade_Nf : BaseModel
    {
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Tipo_Atividade_Nf Tipo_Atividade { get; set; }
        public virtual Status_Default? Status { get; set; }
    }
}
