using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Motivo_Inviabilidade : BaseModel
    {
        public virtual String Motivo { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual Motivo_Principal_InviabilidadeEnum? Motivo_Principal { get; set; }
    }
}
