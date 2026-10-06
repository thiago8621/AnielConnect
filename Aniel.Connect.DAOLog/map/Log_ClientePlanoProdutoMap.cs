using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAOLog;
using System;

namespace Aniel.Connect.DAOLog.map
{
    public class Log_ClientePlanoProdutoMap : LogGenericoMap<Log_ClientePlanoProduto, Int64>
    {
        public Log_ClientePlanoProdutoMap() : base() 
        {
            Table("TB_LOG_CLIENTE_PLANPROD");

            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_LOG_CLIENTE_PLAN");

            Map(p => p.Chave, "OBJETO").Index("IDX_CLIENTE_PLANPROD_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_CLIENTE_PLANPROD_CAMPO");


        }
    }
}
