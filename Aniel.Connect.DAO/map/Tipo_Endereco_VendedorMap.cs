using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
   public class Tipo_Endereco_VendedorMap : ClassMapBase<Tipo_Endereco_Vendedor>
    {
        public Tipo_Endereco_VendedorMap() : base() {
            Table("TB_TIPO_ENDERECO_VENDA");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TIPO_ENDERECO_VENDEDOR");
            Map(x => x.Cidade, "CIDADE"); 
            Map(x => x.Pais, "PAIS"); 
            Map(x => x.Uf, "UF"); 
            References(m => m.Perfil_Vendedor, "ID_PERFIL_VENDEDOR").ForeignKey("FK_PERFIL_VENDEDOR");

        }
    }
}
