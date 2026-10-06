using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20220215_1738_TG_STATUS_LOCALIDADE : AnielMigration
    {
        public _20220215_1738_TG_STATUS_LOCALIDADE()
        {
            Codigo = "20220215_1738";
            Descricao = "Trigger para preencher o status das localidades vindos do T5";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER TRIGGER TG_STATUS_LOCALIDADE FOR TB_LOCALIDADE
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
    IF (NEW.STATUS IS NULL) THEN
        NEW.STATUS = 1;
END").ToString();
        }

    }
}
