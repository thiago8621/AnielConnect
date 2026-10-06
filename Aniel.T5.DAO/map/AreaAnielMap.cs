using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class AreaAnielMap : ClassMap<AreaAniel>
    {
        public AreaAnielMap()
        {
            Table("TB_AREA");

            CompositeId()
               .KeyProperty(x => x.Cod_Escritorio, "COD_ESCRITORIO")
               .KeyProperty(x => x.Cod_Area, "COD_AREA")
               .KeyProperty(x => x.Cod_Cliente, "COD_CLIENTE");

            References(x => x.Grupo_Area, "COD_ESCRITORIO").ReadOnly();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Descricao, "DESCRICAO").Length(20);
            Map(x => x.UF, "UF").Length(2);

            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONGITUDE");
            Map(x => x.Distancia, "DISTANCIA");
        }
    }
}
