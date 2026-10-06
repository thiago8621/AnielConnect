using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class MotivosParalisaOSAnielMap : ClassMap<MotivosParalisaOSAniel>
    {
        public MotivosParalisaOSAnielMap() : base()
        {
            Table("TB_MOTIVO_PARALISA_OS ");

            Id(x => x.Codigo, "CODIGO").GeneratedBy.Sequence("GEN_TB_MOTIVO_PARALISA_OS");

            Map(x => x.Descricao, "MOTIVO").Length(100);
            Map(x => x.Status, "STATUS");
        }
    }
}
