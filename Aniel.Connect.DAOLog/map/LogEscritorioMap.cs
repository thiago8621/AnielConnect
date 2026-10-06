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
    public class LogEscritorioMap : LogGenericoMap<LogEscritorioAniel,Int64>
    {
        public LogEscritorioMap() : base()
        {
            Table("TB_ESCRITORIO");

            Id(u => u.Id, "COD_ESCRITORIO").GeneratedBy.Sequence("GN_ESCRITORIO");

            Map(p => p.Chave, "OBJETO").Index("IDX_ESCRITORIO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_ESCRITORIO_CAMPO");
        }
    }

}
