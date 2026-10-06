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
    public class Log_IntegraClientePlanoMap : LogGenericoMap<Log_IntegraClientePlano, Int64>
    {
        public Log_IntegraClientePlanoMap() : base()
        {
            Table("TB_INTEGRA_CLIENTE_PLANO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_INTEGRA_CLIENTE_PLANO");

            Map(p => p.Chave, "OBJETO").Index("IDX_INTE_CLIENTE_PLANO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_INTE_CLIENTE_PLANO_CAMPO");
        }
    }

}
