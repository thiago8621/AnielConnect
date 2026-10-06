using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAOLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    class LogLoginRadiusMap : LogGenericoMap<LogLoginRadius, Int64>
    {
        public LogLoginRadiusMap() : base()
        {
            Table("TB_LOGIN_RADIUS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_LOGIN_RADIUS");

            Map(p => p.Chave, "OBJETO").Index("IDX_LOGIN_RADIUS_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_LOGIN_RADIUS_CAMPO");            
        }
    }
}
