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
    public class LogExclusaoOnuMap : ClassMapBase<LogExclusaoOnu>
    {
        public LogExclusaoOnuMap() : base()
        {
            Table("TB_LOG_EXCLUSAO_ONU");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_EXCLUSAO_ONU");

            Map(x => x.Data, "DATA");
            Map(x => x.Hora, "HORA");
            Map(x => x.Erro, "ERRO");
            Map(x => x.Info_Equipamento, "INFO_EQUIPAMENTO").Length(5000);
            Map(x => x.Usuario, "USUARIO");
        }
    }
}
