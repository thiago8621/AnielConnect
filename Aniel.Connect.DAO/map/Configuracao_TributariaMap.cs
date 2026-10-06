using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Configuracao_TributariaMap : ClassMapBase<Configuracao_Tributaria>
    {
        public Configuracao_TributariaMap() : base()
        {
            Table("TB_CONFIGURACAO_TRIBUTARIA");

            Map(x => x.Codigo, "CODIGO");
            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Alterado, "ALTERADO");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
