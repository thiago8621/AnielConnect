namespace Aniel.T5.Model.nhibernate
{
    public class QuizAnswerVenda
    {
        public virtual long Id { get; set; }
        
        public virtual long IdVenda { get; set; }     
        public virtual long IdTrocaTitularidade { get; set; }     
        public virtual QuizAnswerCapa Capa { get; set; }
    }
}
