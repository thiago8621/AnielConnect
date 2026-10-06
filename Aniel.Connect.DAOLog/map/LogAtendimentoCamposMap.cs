using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAOLog;
using System;

namespace Aniel.Connect.DAOLog.map
{
    public class LogAtendimentoCamposMap : LogGenericoMap<LogAtendimentoCampos, Int64>
    {
        public LogAtendimentoCamposMap() : base()
        {
            Table("TB_ATENDIMENTO_CAMPOS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO_CAMPOS");

            Map(p => p.Chave, "OBJETO").Index("IDX_ATENDIMENTO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_ATENDIMENTO_CAMPO");
        }
    }
}
