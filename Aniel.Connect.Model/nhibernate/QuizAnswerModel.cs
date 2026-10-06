using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class QuizAnswerModel : BaseModel
    {
        public virtual long Id { get; set; }
        public virtual string Resposta { get; set; }
    }
}
