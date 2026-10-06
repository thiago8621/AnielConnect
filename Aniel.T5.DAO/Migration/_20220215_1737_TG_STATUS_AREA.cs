using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20220215_1737_TG_STATUS_AREA : AnielMigration
    {
        public _20220215_1737_TG_STATUS_AREA()
        {
            Codigo = "20220215_1737";
            Descricao = "Trigger para preencher o status das areas vindos do T5";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER TRIGGER TG_STATUS_AREA FOR TB_AREA
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
    IF (NEW.STATUS IS NULL) THEN
        NEW.STATUS = 1;
END").ToString();
        }

    }
}
