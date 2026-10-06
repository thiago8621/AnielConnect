using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerado;
using Aniel.Connect.Model.nhibernate;

namespace Aniel.T5.Model.nhibernate
{
    public class Elemento_Rede_Porta : BaseModel
    {
        public virtual Elemento_Rede Caixa { get; set; }
        public virtual Int64? Cliente_Plano_Prod { get; set; }
        public virtual Int32? Porta { get; set; }
        public virtual DateTime? Data_Expiracao_Reserva { get; set; }
        public virtual String Login { get; set; }
        public virtual Int64? IdLogin { get; set; }
        public virtual Situacao_Porta Status { get; set; }
    }
}
