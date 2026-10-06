using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class QuizQuestionAnswerMap : ClassMap<QuizQuestionAnswer>
    {
        public QuizQuestionAnswerMap() : base()
        {
            Table("TB_QUIZ_QUESTION_ANSWER");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GEN_TB_QUIZ_QUESTION_ANSWER_ID");

            References(x => x.QuizQuestion, "ID_QUIZ_QUESTION");
            Map(x => x.Resposta, "RESPOSTA").Length(300);
        }
    }
}
