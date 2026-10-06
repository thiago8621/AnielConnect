using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
   public class MaterialOsMap : ClassMapBase<MaterialOs>
    {
        public MaterialOsMap() : base() {
            Table("TB_MATERIAL_OS");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MATERIAL_OS_PLANO_PRODUTO");
            Map(u => u.Codigo, "CODIGO");
            Map(u => u.Descricao, "DESCRICAO");
            References(m => m.Plano, "ID_PLANO").ForeignKey("FK_PLANO_MATERIALOS");
            References(m => m.Produto, "ID_PRODUTO").ForeignKey("FK_PRODUTO_MATERIALOS");
            
        }
    }
}
