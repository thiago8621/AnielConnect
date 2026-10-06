using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class MotivoReagendamentosAnielMap : ClassMap<MotivoReagendamentosAniel>
    {
        public MotivoReagendamentosAnielMap() : base()
        {
            Table("TB_MOTIVOS_REAGENDAMENTOS");

            Id(x => x.Codigo, "COD_MOTIVO_REAGENDAMENTOS").GeneratedBy.Sequence("GN_MOTIVOS_REAGENDAMENTOS");

            Map(x => x.Descricao, "DESCRICAO").Length(110);
            Map(x => x.Status, "STATUS");
            Map(x => x.Cod_Auxiliar, "COD_AUXILIAR");
        }
    }
}
