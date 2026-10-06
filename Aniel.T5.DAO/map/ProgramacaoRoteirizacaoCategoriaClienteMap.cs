using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class ProgramacaoRoteirizacaoCategoriaClienteMap : ClassMapBase<ProgramacaoRoteirizacaoCategoriaCliente>
    {
        public ProgramacaoRoteirizacaoCategoriaClienteMap() : base()
        {
            Table("TB_PROG_ROTEIRIZACAO_CATEGORIA");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PROG_ROTEIRIZACAO_CATEGORIA");
            References(x => x.ProgramacaoRoteirizacaoAutomatica, "ID_PROGRAMACAO");
            References(x => x.Categoria_ClienteAniel, "ID_CATEGORIA");
        }
    }
}
