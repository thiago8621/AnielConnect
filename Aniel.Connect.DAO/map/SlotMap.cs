using Aniel.Connect.Model.enumerado;
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
    public class SlotMap : ClassMapBase<Slot>
    {
        public SlotMap() : base()
        {
            Table("TB_SLOT");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_SLOT");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            
            Map(x => x.LineProfile, "LINE_PROFILE");
            
            References(x => x.OLT, "ID_OLT").NotFound.Ignore().Fetch.Join().ForeignKey("FK_SLOT_OLT");
            Map(x => x.NumSlot, "NUM_SLOT");

            #region CAMPOS ANTIGOS (mantidos para compatibilidade temporaria)

            Map(x => x.SlotID, "SLOTID");

            #endregion
        }
    }
}
