using Sinapse.Model.nhibernate;

namespace Aniel.T5.Model.nhibernate
{
    public class PerfilAgendamentoNatureza : BaseModel
    {
        public virtual Natureza_ProducaoAniel Natureza { get; set; }
        public virtual PerfilAgendamento PerfilAgendamento { get; set; }
        public virtual FolhaDeptoAniel Departamento { get; set; }

    }
}
