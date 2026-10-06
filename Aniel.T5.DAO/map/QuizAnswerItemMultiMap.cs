using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class QuizAnswerItemMultiMap : ClassMap<QuizAnswerItemMulti>
    {
        public QuizAnswerItemMultiMap() : base()
        {
            Table("TB_QUIZ_ANSWER_ITEM_MULTI");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GEN_TB_QUIZ_ANSWER_ITEM_MULTIID");

            References(x => x.QuizAnswerItem, "ID_QUIZ_ANSWER_ITEM");
            References(x => x.QuizQuestionAnswer, "ID_QUESTION_ANSWER");
            
            Map(x => x.Resposta, "RESPOSTA").Length(300);
            Map(x => x.RespostaFoto, "RESPOSTA_FOTO");
        }
    }
}
