using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAOLog;
using System;

namespace Aniel.Connect.DAOLog.map
{
    public class LogEtapaVendasMap : LogGenericoMap<LogEtapaVendas,Int64>
    {
        public LogEtapaVendasMap() : base()
        {
            Table("TB_ETAPA_VENDAS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ETAPA_VENDAS");

            Map(p => p.Chave, "OBJETO").Index("IDX_VENDAS_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_VENDAS_CAMPO");
            Map(p => p.Motivo, "MOTIVO");
        }
    }

}
