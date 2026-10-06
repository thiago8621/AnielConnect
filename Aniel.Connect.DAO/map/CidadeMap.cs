using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class CidadeMap : ClassMapBase<Cidade>
    {
        public CidadeMap() : base()
        {
            Table("TB_CIDADE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CIDADE");

            Map(x => x.Codigo_IBGE, "CODIGO_IBGE").Length(50);
            Map(x => x.Descricao, "DESCRICAO").Length(80);
            Map(x => x.CodigoAuxiliar, "COD_AUX").Length(80);
                     
            Map(x => x.UF, "UF").Length(2);

            References(x => x.Id_UF, "ID_UF").ForeignKey("FK_CIDADE_UF");
            References(x => x.Pais, "ID_PAIS").ForeignKey("FK_CIDADE_PAIS");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.DDD, "DDD").Length(2);
        }
    }
}
