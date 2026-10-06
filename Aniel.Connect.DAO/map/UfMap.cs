using Aniel.Connect.Model.enumerado;
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
    public class UfMap : ClassMapBase<Uf>
    {
        public UfMap() : base()
        {
            Table("TB_UF");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_UF");
            Map(x => x.Codigo_IBGE, "CODIGO_IBGE");
            Map(x => x.CodAuxiliar, "COD_AUXILIAR");

            Map(x => x.Descricao, "DESCRICAO").Length(30);
            Map(x => x.Sigla, "SIGLA").Length(2);
            References(x => x.Pais, "ID_PAIS").ForeignKey("FK_UF_PAIS");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
