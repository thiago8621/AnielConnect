using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.T5.DAO.map
{
    public class FamiliaMaterialMap : ClassMapBase<FamiliaMaterial>
    {
        public FamiliaMaterialMap()
        {
            Table("TB_FAMILIA_MATERIAL");

            Id(x => x.Familia,"FAMILIA");

            Map(x => x.Descricao,"DESCRICAO");
            Map(x => x.Sintetica, "SINTETICA");

        }
    }
}
