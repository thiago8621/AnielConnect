using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class Configuracao_Tributaria : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual bool Alterado { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }
}
