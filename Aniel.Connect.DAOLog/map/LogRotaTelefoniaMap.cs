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
    public class LogRotaTelefoniaMap : LogGenericoMap<LogRotaTelefonia, Int64>
    {
        public LogRotaTelefoniaMap() : base()
        {
            Table("TB_ROTA_TELEFONIA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FRANQUIA");

            Map(p => p.Chave, "OBJETO").Index("IDX_FRANQUIA_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_FRANQUIA_CAMPO");
        }
    }

}
