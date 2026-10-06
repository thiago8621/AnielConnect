using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class ProgramacaoRoteirizacaoTecnicoMap : ClassMapBase<ProgramacaoRoteirizacaoTecnico>
    {
        public ProgramacaoRoteirizacaoTecnicoMap() : base()
        {
            Table("TB_PROG_ROTEIRIZACAO_EQUIPE");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PROG_ROTEIRIZACAO_EQUIPE");
            References(x => x.ProgramacaoRoteirizacaoAutomatica, "ID_PROGRAMACAO");
            Map(x => x.Equipe, "EQUIPE");
        }
    }
}
