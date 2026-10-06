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
    public class Log_IntegraClienteContratoMap : LogGenericoMap<Log_IntegraClienteContrato, Int64>
    {
        public Log_IntegraClienteContratoMap() : base()
        {
            Table("TB_INTEGRA_CLIENTE_CONTRATO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_INTEGRA_CLIENTE_CONTRATO");

            Map(p => p.Chave, "OBJETO").Index("IDX_INTE_CLIENTE_CONTRATO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_INTE_CLIENTE_CONTRATO_CAMPO");
        }
    }

}
