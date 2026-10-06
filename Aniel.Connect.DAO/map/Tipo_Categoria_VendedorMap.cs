using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
   public class Tipo_Categoria_VendedorMap : ClassMapBase<Tipo_Categoria_Vendedor>
    {
        public Tipo_Categoria_VendedorMap() : base() {
            Table("TB_Tipo_Categoria_Vendedor");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_Tipo_Categoria_Vendedor");
            Map(x => x.Tipo_Categoria, "TIPO_CATEGORIA"); 
            References(m => m.Perfil_Vendedor, "ID_PERFIL_VENDEDOR").ForeignKey("FK_PERFIL_VENDEDOR");

        }
    }
}
