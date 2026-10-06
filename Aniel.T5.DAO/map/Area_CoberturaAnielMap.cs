using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class Area_CoberturaAnielMap: ClassMapBase<Area_Cobertura>
    {
        public Area_CoberturaAnielMap()
        {
            Table("TB_AREA_COBERTURA");

            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_AREA_COBERTURA");

         
            Map(p => p.Situacao, "SITUACAO").CustomType<Status_Default>();
            Map(p => p.Descricao, "DESCRICAO").Length(100);
            Map(p => p.Cor, "COR").Length(50);
            Map(p => p.Coordenadas, "COORDENADAS").Length(5000);
            Map(p => p.Tipo, "TIPO_POLIGONO").CustomType<Tipo_Poligono>();
             
        }
    }
}
