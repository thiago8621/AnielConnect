using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class TermoAdesao : BaseModel
    {
        public virtual string NomeArquivo { get; set; }
        public virtual string CaminhoArquivo { get; set; }
        public virtual string MensagemConcluido { get; set; }
    }
}
