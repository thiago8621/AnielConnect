using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class FolhaDeptoAnielMap : ClassMap<FolhaDeptoAniel>
    {
        public FolhaDeptoAnielMap() : base()
        {
            Table("TB_FOLHA_DEPTO");

            Id(x => x.CodDepto, "COD_DEPTO");
            Map(x => x.NumDepto, "NUM_DEPTO").Length(10);
            Map(x => x.Descricao, "DESCRICAO").Length(30);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
