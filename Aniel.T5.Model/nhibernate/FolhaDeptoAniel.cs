using Sinapse.Model.enumerados;

namespace Aniel.T5.Model.nhibernate
{
    public class FolhaDeptoAniel
    {
        public virtual short CodDepto { get; set; }
        public virtual string NumDepto { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
    }
}
