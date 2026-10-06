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
    public class Log_Config_GeraisMap : LogGenericoMap<LogConfigGeraisAniel, Int64>
    {
        public Log_Config_GeraisMap() : base()
        {
            Table("TB_LOG_CONFIG_GERAIS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_CONFIG_GERAIS");

            Map(p => p.Chave, "OBJETO").Index("IDX_ATENDIMENTO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_ATENDIMENTO_CAMPO");
            Map(p => p.RoteirizarSemLatLong, "ROTEIRIZAR_SEMLATLONG").Index("IDX_ROT_SEMlATLONG_CAMPO");
            Map(p => p.RoteirizarPorCx, "ROTEIRIZAR_PORCX").Index("IDX_ROT_PORCX_CAMPO");
        }
    }

}
