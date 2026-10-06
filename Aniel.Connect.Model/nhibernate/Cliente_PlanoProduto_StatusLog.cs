using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cliente_PlanoProduto_StatusLog : BaseModel
    {
        public virtual Cliente_PlanoProduto PlanoProduto { get; set; }
        public virtual ClienteContrato ClienteContrato { get; set; }
        public virtual DateTime? DataHoraAcao { get; set; }
        public virtual Status_PlanoProduto? AntigoStatus { get; set; }
        public virtual Status_PlanoProduto? NovoStatus { get; set; }
    }
}
