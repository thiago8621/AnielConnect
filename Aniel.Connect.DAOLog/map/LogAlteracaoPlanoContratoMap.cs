using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAOLog;
using System;

namespace Aniel.Connect.DAOLog.map
{
    public class LogAlteracaoPlanoContratoMap : LogGenericoMap<LogAlteracaoPlanoContrato, Int64>
    {
        public LogAlteracaoPlanoContratoMap() : base()
        {
            Table("TB_ALTERACAO_PLANO_CON");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ALT_PLAN_CON");

            Map(p => p.Chave, "OBJETO").Index("IDX_PLAN_CON_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_PLAN_CON_CAMPO");            
        }
    }

}
