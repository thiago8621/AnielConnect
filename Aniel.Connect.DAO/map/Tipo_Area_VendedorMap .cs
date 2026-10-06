using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
   public class Tipo_Area_VendedorMap : ClassMapBase<Tipo_Area_Vendedor>
    {
        public Tipo_Area_VendedorMap() : base() {
            Table("TB_TIPO_AREA_VENDEDOR");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_Tipo_Area_Vendedor");
            Map(x => x.Tipo_Area, "TIPO_AREA"); 
            References(m => m.Perfil_Vendedor, "ID_PERFIL_VENDEDOR").ForeignKey("FK_PERFIL_VENDEDOR");

        }
    }
}
