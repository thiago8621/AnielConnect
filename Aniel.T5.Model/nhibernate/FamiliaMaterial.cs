using Sinapse.Model.nhibernate;

namespace Aniel.T5.Model.nhibernate
{
    public class FamiliaMaterial : BaseModel
    {
        public virtual string Familia { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Sintetica { get; set; }
    }
}
