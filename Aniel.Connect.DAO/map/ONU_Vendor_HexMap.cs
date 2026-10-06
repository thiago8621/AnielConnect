using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ONU_Vendor_HexMap : ClassMapBase<ONU_Vendor_Hex>
    {
        public ONU_Vendor_HexMap() : base()
        {
            Table("TB_ONU_VENDOR_HEX");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ONU_VENDOR_HEX");

            Map(x => x.Vendor, "VENDOR").Length(100);
            Map(x => x.Hexadecimal, "HEXADECIMAL").Length(100);
            Map(x => x.Descricao, "DESCRICAO").Length(100);

        }
    }
}
