using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class ProgramacaoRoteirizacaoProjetoMap : ClassMapBase<ProgramacaoRoteirizacaoProjeto>
    {
        public ProgramacaoRoteirizacaoProjetoMap() : base()
        {
            Table("TB_PROG_ROTEIRIZACAO_PROJETO");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PROG_ROTEIRIZACAO_PROJETO");
            References(x => x.ProgramacaoRoteirizacaoAutomatica, "ID_PROGRAMACAO");
            Map(x => x.CodCt, "CODCT");
            Map(x => x.Projeto, "PROJETO");
        }
    }
}
