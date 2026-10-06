using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;

namespace Aniel.T5.DAO.map
{
    public class Tipo_Servico_Equipe_ImovelAnielMap : ClassMapBase<Tipo_Servico_Equipe_ImovelAniel>
    {
        public Tipo_Servico_Equipe_ImovelAnielMap()
        {
            Table("TB_TIPO_SERVICO_EQUIPE_IMOVEL");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TIPO_SERVICO_EQUIPE_IMOVEL");

            References(x => x.Tipo_Imovel, "ID_TIPO_IMOVEL").ForeignKey("FK_TIPO_SERVI_TIPO_IMOV").Index("IDX_TSEI_SERVICO_IMOVEL");
            References(x => x.Tipo_Servico, "ID_TIPO_SERVICO").ForeignKey("FK_TIPO_SERV_TIPO_SERV").Index("IDX_TSEI_SERVICO_IMOVEL"); ;
            Map(x => x.Horas, "HORAS").CustomType("Time");
        }
    }
}
