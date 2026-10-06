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
    public class LogImportacaoOnuMap : ClassMapBase<LogImportacaoOnu>
    {
        public LogImportacaoOnuMap() : base()
        {
            Table("TB_LOG_IMPORTACAO_ONU");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_IMPORTACAO_ONU");

            Map(x => x.Data, "DATA");
            Map(x => x.Hora, "HORA");
            Map(x => x.Erro, "ERRO");
            Map(x => x.Serial_Equipamento, "SERIAL_EQUIPAMENTO");
            Map(x => x.Usuario, "USUARIO");
        }
    }
}
