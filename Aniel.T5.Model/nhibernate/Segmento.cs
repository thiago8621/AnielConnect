using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;

namespace Aniel.T5.Model.nhibernate
{
    public class Segmento : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
    }
}
