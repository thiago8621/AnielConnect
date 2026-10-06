using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
   public class Meio_Captacao_VendedorMap : ClassMapBase<Meio_Captacao_Vendedor>
    {
        public Meio_Captacao_VendedorMap() : base() {
            Table("TB_MEIO_CAPTACAO_VENDEDOR");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MEIO_CAPTACAO_VENDEDOR");
            Map(x => x.Meio_Captacao, "MEIO_CAPTACAO"); 
            References(m => m.Perfil_Vendedor, "ID_PERFIL_VENDEDOR").ForeignKey("FK_PERFIL_VENDEDOR");

        }
    }
}
