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
    public class Log_IntegraFaturamentoMap : LogGenericoMap<Log_IntegraFaturamento, Int64>
    {
        public Log_IntegraFaturamentoMap() : base()
        {
            Table("TB_INTEGRA_FATURAMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_INTEGRA_FATURAMENTO");

            Map(p => p.Chave, "OBJETO").Index("IDX_INTE_ENDERECO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_INTE_ENDERECO_CAMPO");
        }
    }

}
