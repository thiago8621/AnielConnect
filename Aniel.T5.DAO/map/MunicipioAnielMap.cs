using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class MunicipioAnielMap : ClassMap<MunicipioAniel>
    {
        public MunicipioAnielMap()
        {
            Table("TB_MUNICIPIO");
            Id(x => x.CodMunicipio, "COD_MUNICIPIO");
            Map(x => x.Municipio, "MUNICIPIO").Length(100);
            Map(x => x.Uf, "UF").Length(2);
            Map(x => x.CodIBGE, "COD_MUNICIPIO_OFICIAL");
            
        }
    }
}
