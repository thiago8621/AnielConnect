using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAO.map;
using Sinapse.DAOLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    public class Log_DidMap : LogGenericoMap<Log_Did,Int64>
    {
        public Log_DidMap() : base()
        {
            Table("TB_DID");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_DID");

            Map(p => p.Chave, "OBJETO").Index("IDX_DID_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_DID_CAMPO");
        }
    }

}
