using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAOLog;
using System;

namespace Aniel.Connect.DAOLog.map
{
    public class Log_Condominio_PredioMap : LogGenericoMap<Log_Condominio_Predio,Int64>
    {
        public Log_Condominio_PredioMap() : base()
        {
            Table("TB_CONDOMINIO_PREDIO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO");

            Map(p => p.Chave, "OBJETO").Index("IDX_ATENDIMENTO_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_ATENDIMENTO_CAMPO");
        }
    }

}
