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
    public class RotaTelefoniaMap : ClassMapBase<RotaTelefonia>
    {
        public RotaTelefoniaMap() : base()
        {
            Table("TB_ROTA_TELEFONIA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ROTA_TELEFONIA");
            Map(m => m.Nome, "NOME").Length(100);
            Map(m => m.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
