using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class Blacklist : BaseModel
    {
        public virtual int Numero_Origem { get; set; }
        public virtual int Numero_Destino { get; set; }
        public virtual string Observacao { get; set; }
        public virtual Plano Plano { get; set; }
    }
}
