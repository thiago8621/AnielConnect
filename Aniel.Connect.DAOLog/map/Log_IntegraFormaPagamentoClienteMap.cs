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
    public class Log_IntegraFormaPagamentoClienteMap : LogGenericoMap<Log_IntegraFormaPagamentoCliente, Int64>
    {
        public Log_IntegraFormaPagamentoClienteMap() : base()
        {
            Table("TB_INTEGRA_FORMA_PAGA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_INTEGRA_FORMA_PAGA");

            Map(p => p.Chave, "OBJETO").Index("IDX_INTE_FORMA_PAGA_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_INTE_FORMA_PAGA_CAMPO");
        }
    }

}
