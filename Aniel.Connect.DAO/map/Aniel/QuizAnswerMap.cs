using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map.Aniel
{
    public class QuizAnswerMap : ClassMapBase<QuizAnswerModel>
    {
        public QuizAnswerMap() : base()
        {
            Table("TB_QUIZ_ANSWER_ITEM");

            Id(u => u.Id, "ID");
            Map(x => x.Resposta, "RESPOSTA");
        }
    }
}
