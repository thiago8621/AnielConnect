using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;

namespace Aniel.T5.DAO.map
{
    public class Unidade_CustoAnielMap : ClassMap<Unidade_CustoAniel>
    {
        public Unidade_CustoAnielMap()
        {
            Table("TB_UNIDADE_CUSTO_CLIENTE");

            Id(x => x.Unidade_Custo,"UNIDADE_CUSTO").Length(10);

            Map(x => x.Descricao, "DESCRICAO").Length(20);
            
        }
    }
}
