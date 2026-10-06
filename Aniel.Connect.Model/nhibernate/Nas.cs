using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class NAS : BaseModel
    {
        public virtual String Codigo { get; set; }
        public virtual String Codigo_Radius { get; set; }
        public virtual String Nome { get; set; }
        public virtual Tecnologia_Nas Tecnologia { get; set; }
        public virtual String Ip { get; set; }
        public virtual String Secret { get; set; }
        public virtual String Porta { get; set; }
        public virtual Status_Default? Status { get; set; }
    }
}
