namespace Aniel.T5.Model.nhibernate
{
    public class QuizAnswerItemMulti
    {
        public virtual long Id { get; set; }

        public virtual QuizAnswerItem QuizAnswerItem { get; set; }
        public virtual QuizQuestionAnswer QuizQuestionAnswer { get; set; }
       
        public virtual string Resposta { get; set; }
        public virtual long? RespostaFoto { get; set; }
    }
}
