using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class CobrancaAdicional : BaseModel
    {
        public virtual Faturamento_Cliente Faturamento_Cliente { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual ClienteContrato ClienteContrato { get; set; }
        public virtual double? Valor { get; set; }
        public virtual DateTime? DataCobranca { get; set; }
        public virtual MotivoCobrancaAdicional? Motivo { get; set; }
    }
}
