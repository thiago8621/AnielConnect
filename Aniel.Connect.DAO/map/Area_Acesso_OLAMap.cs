using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Area_Acesso_OLAMap : ClassMapBase<Area_Acesso_OLA>
    {
        public Area_Acesso_OLAMap() : base()
        {
            Table("TB_AREA_ACESSO_OLA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_AREA_ACESSO_OLA");
            
            Map(x => x.Cod_Area_Acesso, "COD_AREA");
            Map(x => x.Ocultar_Acesso, "OCULTAR_ACESSO").CustomType<Boolean>();
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Observacao, "OBSERVACAO").Length(250);
        }
    }
}
