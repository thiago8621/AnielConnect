using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20230208_0930_TG_ULTIMO_SEQ : AnielMigration
    {
        public _20230208_0930_TG_ULTIMO_SEQ()
        {
            Codigo = "20230208_0930";
            Descricao = "Trigger para atualizar campo ULTIMO_SEQ ";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger TG_ULTIMO_SEQ for TB_DOCUMENTO_PRODUCAO
                                                    ACTIVE BEFORE INSERT POSITION 0
                                                    AS
                                                    BEGIN
                                                      IF (INSERTING) THEN
                                                      BEGIN
                                                       NEW.ULTIMO_SEQ = 'S';
                                                       UPDATE TB_DOCUMENTO_PRODUCAO D SET D.ULTIMO_SEQ = 'N'
                                                       WHERE D.CODCT = NEW.CODCT AND D.PROJETO = NEW.PROJETO AND D.NUM_DOC = NEW.NUM_DOC AND D.NUM_OBRA_ORIGINAL = NEW.NUM_OBRA_ORIGINAL;
                                                      END
                                                    END").ToString();
        }

    }
}
