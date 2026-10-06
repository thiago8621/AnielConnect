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
    public class Log_IntegraAtendimentoMap : LogGenericoMap<Log_IntegraAtendimento,Int64>
    {
        public Log_IntegraAtendimentoMap() : base()
        {
            Table("TB_INTEGRA_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_INTEGRA_ATENDIMENTO");

            Map(p => p.Chave, "OBJETO").Index("IDX_INTE_ATEND_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_INTE_ATEND_CAMPO");
        }
    }

}
