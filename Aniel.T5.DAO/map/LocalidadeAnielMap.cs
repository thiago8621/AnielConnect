using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class LocalidadeAnielMap : ClassMap<LocalidadeAniel>
    {
        public LocalidadeAnielMap()
        {
            Table("TB_LOCALIDADE");

            CompositeId()
            .KeyProperty(x => x.LocalidadeStr, set =>
            {
                set.ColumnName("LOCALIDADE");
                set.Length(30);
            })
            .KeyProperty(x => x.Cod_Area, set =>
            {
                set.ColumnName("COD_AREA");
                set.Length(4);
            })
            .KeyProperty(x => x.Cod_Escritorio, set =>
            {
                set.ColumnName("COD_ESCRITORIO");
                set.Length(4);
            })
            .KeyProperty(x => x.Cod_Cliente, set =>
            {
                set.ColumnName("COD_CLIENTE");
                set.Length(4);
            });

            Map(x => x.Cod_Localidade, "COD_LOCALIDADE");
            Map(x => x.PontoRoteamento, "PONTO_ROTEAMENTO").Length(100);
            Map(x => x.CodCt, "CODCT");
            Map(x => x.Projeto, "PROJETO");      
            Map(x => x.CodCt_Infra, "CODCT_INFRA").Length(4);
            Map(x => x.Projeto_Infra, "PROJETO_INFRA").Length(20);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONGITUDE");
            Map(x => x.Padrao, "PADRAO");
        }
    }
}
