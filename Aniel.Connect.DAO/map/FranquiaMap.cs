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
    public class FranquiaMap : ClassMapBase<Franquia>
    {
        public FranquiaMap() : base()
        {
            Table("TB_FRANQUIA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FRANQUIA");

            Map(m => m.Descricao, "DESCRICAO").Length(50);
            Map(m => m.Ldn, "LDN").Index("IDX_FRANQUIA_LDN");
            Map(m => m.Ldi, "LDI");
            Map(m => m.Lc, "LC");
            Map(m => m.Vc1, "VC1");
            Map(m => m.Vc2, "VC2");
            Map(m => m.Vc3, "VC3");
            Map(m => m.Minutos, "MINUTOS");
            Map(m => m.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
