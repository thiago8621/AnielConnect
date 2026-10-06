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
    public class Log_IntegraLoginMap : LogGenericoMap<Log_IntegraLogin, Int64>
    {
        public Log_IntegraLoginMap() : base()
        {
            Table("TB_INTEGRA_LOGIN");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_INTEGRA_LOGIN");

            Map(p => p.Chave, "OBJETO").Index("IDX_INTE_LOGIN_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_INTE_LOGIN_CAMPO");
        }
    }

}
