using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class VincularAtendimento : BaseModel
    {
        public virtual Atendimento Atendimento { get; set; }
        public virtual string Numero_Atendimento { get; set; }
        public virtual string CodCT { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string Num_Obra { get; set; }
        public virtual string Num_Doc { get; set; }
    }
}
