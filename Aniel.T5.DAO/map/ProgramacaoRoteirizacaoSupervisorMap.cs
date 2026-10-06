using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class ProgramacaoRoteirizacaoSupervisorMap : ClassMapBase<ProgramacaoRoteirizacaoSupervisor>
    {
        public ProgramacaoRoteirizacaoSupervisorMap() : base()
        {
            Table("TB_PROG_ROTEIRIZACAO_SUPERVISOR");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PROG_ROTEIRIZACAO_SUPERVISOR");
            References(x => x.ProgramacaoRoteirizacaoAutomatica, "ID_PROGRAMACAO");
            Map(x => x.Numfre, "NUM_FRE");
        }
    }
}
