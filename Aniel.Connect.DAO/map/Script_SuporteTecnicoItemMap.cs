using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Script_SuporteTecnicoItemMap : ClassMapBase<Script_SuporteTecnicoItem>
    {
        public Script_SuporteTecnicoItemMap() : base()
        {
            Table("TB_SCRIPT_SUPORTETECNICOITEM");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_SCRIPT_SUPORTETECNICOITEM");

            Map(x => x.Ordem, "ORDEM");

            Map(x => x.Informacao, "INFORMACAO").Length(1000);

            References(x => x.Script, "ID_SCRIPT").ForeignKey("FK_SCRIPT_SUPORTE_SCRIPT");
            
        }
    }
    
}
