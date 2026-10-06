using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ProdutosCompMap : ClassMap<ProdutosComp>
    {
        public ProdutosCompMap()
        {
            Table("TB_PRODUTOS_COMP");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PRODUTOS_COMP");

            Map(x => x.Descricao, "DESCRICAO").Length(150);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.Codigo_NCM, "CODIGO_NCM").Length(10);
            Map(x => x.Codigo_CEST, "CEST").Length(7);

        }
    }
}
