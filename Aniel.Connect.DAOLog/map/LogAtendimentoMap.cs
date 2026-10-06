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
    public class LogAtendimentoMap : LogGenericoMap<LogAtendimento,Int64>
    {
        public LogAtendimentoMap() : base()
        {
            Table("TB_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO");

            Map(p => p.Chave, "OBJETO").Index("IDX_ATENDIMENTO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_ATENDIMENTO_CAMPO");
        }
    }

}
