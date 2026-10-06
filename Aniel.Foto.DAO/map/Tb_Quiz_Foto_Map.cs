using Aniel.Foto.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Foto.DAO.map
{
    public class Tb_Quiz_Foto_Map : ClassMap<Tb_Quiz_Foto>
    {
        public Tb_Quiz_Foto_Map()
        {
            Table("TB_QUIZ_FOTO");

            Id(x => x.Id).GeneratedBy.Sequence("GEN_TB_QUIZ_FOTO_ID");
            Map(x => x.IdAnswerCapa, "ID_ANSWER_CAPA");
            Map(x => x.IdAnswerItem, "ID_QUIZ_ANSWER_ITEM");
            Map(x => x.IdQuestion, "ID_QUESTION");
            Map(x => x.Arquivo, "ARQUIVO");
        }
    }
}
