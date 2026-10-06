using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20250603_1114_TB_ATENDIMENTO_TRIGGER : AnielMigration
    {
        public _20250603_1114_TB_ATENDIMENTO_TRIGGER()
        {
            Codigo = "20250603_1114";
            Descricao = "Atendimento Trigger";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE TRIGGER TRG_UPD_ATT_DATAHORA FOR TB_ATENDIMENTO BEFORE UPDATE
                                                    AS
                                                    BEGIN
                                                        IF (NEW.DATAHORA_VINC_RESP IS NULL AND NEW.ID_RESPONSAVEL IS NOT NULL) THEN
                                                            NEW.DATAHORA_VINC_RESP = CURRENT_TIMESTAMP;
                                                    END").ToString();
        }
    }
}
