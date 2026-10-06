using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class ProgramacaoRoteirizacaoNaturezaProducaoMap : ClassMapBase<ProgramacaoRoteirizacaoNaturezaProducao>
    {
        public ProgramacaoRoteirizacaoNaturezaProducaoMap() : base()
        {
            Table("TB_PROG_ROTEIRIZACAO_NATUREZA");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PROG_ROTEIRIZACAO_NATUREZA");
            References(x => x.ProgramacaoRoteirizacaoAutomatica, "ID_PROGRAMACAO");
            References(x => x.NaturezaProducaoAniel, "ID_NATUREZA");
        }
    }
}
