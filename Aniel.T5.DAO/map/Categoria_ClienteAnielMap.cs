using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class Categoria_ClienteAnielMap : ClassMapBase<Categoria_ClienteAniel>
    {
        public Categoria_ClienteAnielMap()
        {
            Table("TB_CATEGORIA_CLIENTE_CONNECT");

            Id(u => u.Id, "ID").GeneratedBy.Assigned();

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(m => m.Tipo_Prioridade, "TIPO_PRIORIDADE").CustomType<Tipo_PrioridadeT5>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
