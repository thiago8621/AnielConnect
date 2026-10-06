using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20220704_1145_TG_EXCLUSAO_OS : AnielMigration
    {
        public _20220704_1145_TG_EXCLUSAO_OS()
        {
            Codigo = "20220704_1145";
            Descricao = "Trigger para preencher ID do log de exclusao";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER TRIGGER TG_LOG_EXCLUSAO_OS FOR TB_LOG_EXCLUSAO_OS
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
    NEW.ID = GEN_ID(GEN_LOG_EXCLUSAO_OS,1);
END").ToString();
        }

    }
}
