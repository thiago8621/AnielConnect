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
    public class CircuitoTelefoniaDidMap : ClassMapBase<CircuitoTelefoniaDid>
    {
        public CircuitoTelefoniaDidMap() : base()
        {
            Table("TB_CIRCUITO_TELEFONIA_DID");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CIRCUITO_TELEFONIA_DID");


            //References(x => x.Did, "ID_DID").ForeignKey("FK_CIRCUITODID_DID");
            References(x => x.CircuitoTelefonia, "ID_CIRCUITO_TELEFONIA").ForeignKey("FK_CIRCUITODID_CIRCUITO");
        }
    }
}
