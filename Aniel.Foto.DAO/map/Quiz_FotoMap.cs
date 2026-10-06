using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class Quiz_FotoMap : ClassMap<Quiz_Foto>
    {
        public Quiz_FotoMap() : base()
        {
            Table("TB_QUIZ_FOTO");

            Id(u => u.Id, "ID").GeneratedBy.Identity();
            Map(x => x.Usuario, "USUARIO").Length(10);
            Map(x => x.Dia, "DIA").CustomSqlType("DATE");
            Map(x => x.Hora, "HORA").CustomSqlType("TIME");


            Map(x => x.Id_Answer_Capa, "ID_ANSWER_CAPA");
            Map(x => x.Id_Quiz_Answer_Item, "ID_QUIZ_ANSWER_ITEM");
            Map(x => x.Id_Question, "ID_QUESTION");
            Map(x => x.Id_Question_Answer, "ID_QUESTION_ANSWER");

            Map(x => x.Arquivo, "ARQUIVO");
        }
    }
}
