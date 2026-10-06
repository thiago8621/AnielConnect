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
    public class LogPortabilidadeMap : LogGenericoMap<LogFranquia,Int64>
    {
        public LogPortabilidadeMap() : base()
        {
            Table("TB_PORTABILIDADE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PORTABILIDADE");

            Map(p => p.Chave, "OBJETO").Index("IDX_PORTABILIDADE_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_PORTABILIDADE_CAMPO");
        }
    }

}
