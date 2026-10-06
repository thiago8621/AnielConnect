using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAOLog;
using System;

namespace Aniel.Connect.DAOLog.map
{
    public class Log_ClienteContratoMap : LogGenericoMap<Log_ClienteContrato, Int64>
    {
        public Log_ClienteContratoMap() : base()
        {
            Table("TB_LOG_CLIENTE_CTT");

            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_LOG_CLIENTE_CTT");

            Map(p => p.Chave, "OBJETO").Index("IDX_LOG_CLIENTE_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_LOG_CLIENTE_CAMPO");

        }
    }
}
