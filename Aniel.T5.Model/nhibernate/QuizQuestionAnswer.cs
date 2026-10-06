namespace Aniel.T5.Model.nhibernate
{
    public class QuizQuestionAnswer
    {
        public virtual long Id { get; set; }
        public virtual QuizQuestion QuizQuestion { get; set; }
        public virtual string Resposta { get; set; }
    }
}
