using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class VinculoAtendimentoCampos : BaseModel
    {
        public virtual Atendimento Atendimento { get; set; }
        public virtual AtendimentoCampos AtendimentoCampos { get; set; }
        public virtual AtendimentoCamposRespostas AtendimentoCamposRespostas { get; set; }
        public virtual string Resposta { get; set; }
    }
}
