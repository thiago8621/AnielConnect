using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cliente_Contrato_StatusLog : BaseModel
    {
        public virtual ClienteContrato ClienteContrato { get; set; }
        public virtual DateTime? DataHoraAcao { get; set; }
        public virtual Status_Contrato? AntigoStatus { get; set; }
        public virtual Status_Contrato? NovoStatus { get; set; }
    }
}
