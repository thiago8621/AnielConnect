using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.T5.DAO.map
{
    public class LimiteAgendamentoAnielMap : ClassMapBase<LimiteAgendamentoAniel>
    {
        public LimiteAgendamentoAnielMap() : base()
        {
            Table("TB_LIMITE_AGENDAMENTO");

            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_ID_LIM_AGEND");

            Map(x => x.CodEscritorio, "COD_ESCRITORIO")
                .Index("IDX_LIM_PERC_ESC_AREA")
                .Index("IDX_LIM_PERC_ESC_AREA_NAT");
            
            Map(x => x.CodArea, "COD_AREA")
                .Index("IDX_LIM_PERC_ESC_AREA")
                .Index("IDX_LIM_PERC_ESC_AREA_NAT");
            
            Map(x => x.IdNatureza, "ID_NATUREZA").Index("IDX_LIM_PERC_ESC_AREA_NAT");

            Map(x => x.Percentual, "PERCENTUAL");

            Map(x => x.DataInicio, "DATA_INICIO");
            Map(x => x.DataFim, "DATA_FIM");
        }
    }
}
