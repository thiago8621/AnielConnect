using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    public class LogImportacaoVinculoPerfilAtivacaoMap : ClassMapBase<LogImportacaoVinculoPerfilAtivacao>
    {
        public LogImportacaoVinculoPerfilAtivacaoMap() : base()
        {
            Table("TB_LOG_IMPORT_VINC_PERF_ATIV");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOG_IMPORT_VINC_PERF_ATIV");

            Map(x => x.Data, "DATA");
            Map(x => x.Hora, "HORA");
            Map(x => x.Erro, "ERRO");
            Map(x => x.Codmat, "CODMAT");
            Map(x => x.Id_OLT, "ID_OLT");
            Map(x => x.Usuario, "USUARIO");
        }
    }
}
