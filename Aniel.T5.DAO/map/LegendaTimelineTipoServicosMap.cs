using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class LegendaTimelineTipoServicosMap : ClassMapBase<LegendaTimelineTipoServicos>
    {
        public LegendaTimelineTipoServicosMap() : base()
        {
            Table("TB_LEGENDA_TIM_TIPO_SERV");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LEGENDA_TIM_TIPO_SERV");
            Map(x => x.Cor, "COR");
            References(x => x.Contrato, "CODCT").ForeignKey("FK_CODCT");
            References(x => x.Natureza_Producao, "ID_NATUREZA_PRODUCAO").ForeignKey("FK_NATUREZA_PRODUCAO");
            References(x => x.Tipo_Servico_Equipe, "ID_TP_SERV").ForeignKey("FK_TP_SERV");
        }
    }
}
