using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
   public class Tipo_Pessoa_VendedorMap : ClassMapBase<Tipo_Pessoa_Vendedor>
    {
        public Tipo_Pessoa_VendedorMap() : base() {
            Table("TB_TIPO_PESSOA_VENDA");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PLANO_PRODUTO_COBRANCA");
            Map(x => x.Tipo_Pessoa, "TIPO_PESSOA"); 
            References(m => m.Perfil_Vendedor, "ID_PERFIL_VENDEDOR").ForeignKey("FK_PERFIL_VENDEDOR");

        }
    }
}
