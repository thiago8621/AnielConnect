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
    public class BairroMap : ClassMapBase<Bairro>
    {
        public BairroMap() : base()
        {
            Table("TB_BAIRRO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_BAIRRO");

            Map(x => x.Descricao, "DESCRICAO").Length(80);
            //Map(x => x.Pais, "PAIS");
           
            References(x => x.UF, "ID_UF").ForeignKey("FK_BAIRRO_UF");
            References(x => x.Pais, "ID_PAIS").ForeignKey("FK_BAIRRO_PAIS");
            References(x => x.Cidade, "ID_CIDADE").ForeignKey("FK_BAIRRO_CIDADE"); 
            Map(x => x.CodigoAuxiliar, "COD_AUX").Length(80);

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
