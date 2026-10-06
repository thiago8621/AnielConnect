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
    public class LogVincularMonitorTecnicoMap : LogGenericoMap<LogVincularMonitorTecnico, Int64>
    {
        public LogVincularMonitorTecnicoMap() : base()
        {
            Table("TB_LOG_VINCULAR_MONITOR_TECNICO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_VINCULAR_MONITOR_TECNICO");
            Map(u => u.Tecnico, "TECNICO").Length(150);
            Map(u => u.Monitor, "MONITOR").Length(150);

            Map(p => p.Chave, "OBJETO").Index("IDX_VINC_MONITORTECNICO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_VINC_MONITORTECNICO_CAMPO");

        }
    }

}
