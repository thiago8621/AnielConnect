using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class VincularAtendimentoVenda : BaseModel
    {
        public virtual Int64? Venda { get; set; }
        public virtual Int64? Atendimento { get; set; }     

    }
}
