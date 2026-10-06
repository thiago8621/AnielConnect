using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;


namespace Aniel.T5.DAO.map
{
    public class Sub_Tipo_ServicoAnielMap : ClassMap<Sub_Tipo_ServicoAniel>
    {
        public Sub_Tipo_ServicoAnielMap()
        {
            Table("TB_SUB_TIPO_SERVICO");

            CompositeId()
            .KeyProperty(x => x.Codigo_Sub_Tipo, "COD_SUB_TIPO_SERV")
            .KeyProperty(x => x.Codigo_Tipo, "COD_TIPO_SERV");

            Map(x => x.Descricao, "DESCRICAO").Length(70);

        }
    }
}
