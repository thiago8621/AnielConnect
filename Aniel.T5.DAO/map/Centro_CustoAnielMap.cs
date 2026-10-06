using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;

namespace Aniel.T5.DAO.map
{
    public class Centro_CustoAnielMap : ClassMap<Centro_CustoAniel>
    {
        public Centro_CustoAnielMap()
        {
            Table("TB_CENTRO_CUSTO");

            Id(x => x.Cod_Ccusto, "COD_CCUSTO");
            Map(x => x.Descricao, "DESCRICAO").Length(30);
            Map(x => x.Codigo_Auxiliar, "CODAUX");
            Map(x => x.Data_Criacao, "DATA_CRIACAO");
            Map(x => x.Hora_Criacao, "HORA_CRIACAO");
            Map(x => x.Ativo, "ATIVO");
        }
    }
}
