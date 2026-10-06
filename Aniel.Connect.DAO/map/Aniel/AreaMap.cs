using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class AreaMap : ClassMapBase<Area>
    {
        public AreaMap() : base()
        {
            Table("TB_AREA");

            Id(x => x.Id);

            Map(x => x.Cod_Area, "COD_AREA").Length(4);
            Map(x => x.Descricao, "DESCRICAO").Length(20);

            References(x => x.Escritorio, "ID_ESCRITORIO");
        }
    }
}
