using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class ProgramacaoRoteirizacaoFilialMap : ClassMapBase<ProgramacaoRoteirizacaoFilial>
    {
        public ProgramacaoRoteirizacaoFilialMap() : base()
        {
            Table("TB_PROG_ROTEIRIZACAO_FILIAL");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PROG_ROTEIRIZACAO_FILIAL");
            References(x => x.ProgramacaoRoteirizacaoAutomatica, "ID_PROGRAMACAO");
            Map(x => x.CodEmpresa, "COD_EMPRESA");
            Map(x => x.CodFilial, "COD_FILIAL");
        }
    }
}
