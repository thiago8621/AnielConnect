using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class ProgramacaoRoteirizacaoLocalidadeMap : ClassMapBase<ProgramacaoRoteirizacaoLocalidade>
    {
        public ProgramacaoRoteirizacaoLocalidadeMap() : base()
        {
            Table("TB_PROG_ROTEIRIZACAO_LOCALIDADE");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PROG_ROTEIRIZACAO_LOCALIDADE");
            References(x => x.ProgramacaoRoteirizacaoAutomatica, "ID_PROGRAMACAO");
            Map(x => x.Localidade, "LOCALIDADE");
            Map(x => x.CodEscritorio, "COD_ESCRITORIO");
            Map(x => x.CodArea, "COD_AREA");
        }
    }
}
