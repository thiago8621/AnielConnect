using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class QuizAnswerVendaMap : ClassMap<QuizAnswerVenda>
    {
        public QuizAnswerVendaMap() : base()
        {
            Table("TB_QUIZ_ANSWER_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_QUIZ_ANSWER_VENDA");

            Map(x => x.IdVenda, "ID_VENDA").Index("IDX_QUIZ_ANSWER_VENDA");
            Map(x => x.IdTrocaTitularidade, "ID_TROCA_TITULAR").Index("IDX_QUIZ_ANSWER_TROCA");
            References(x => x.Capa, "ID_ANSWER_CAPA");
        }
    }
}
