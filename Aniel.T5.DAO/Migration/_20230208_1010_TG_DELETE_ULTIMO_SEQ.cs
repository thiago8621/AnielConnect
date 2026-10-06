using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20230208_1010_TG_ULTIMO_SEQ : AnielMigration
    {
        public _20230208_1010_TG_ULTIMO_SEQ()
        {
            Codigo = "20230208_1010";
            Descricao = "Trigger para atualizar campo ULTIMO_SEQ em casos de delete";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger TG_DELETE_ULTIMO_SEQ for TB_DOCUMENTO_PRODUCAO
                                                    ACTIVE BEFORE DELETE POSITION 0
                                                    AS
                                                    BEGIN
                                                      UPDATE TB_DOCUMENTO_PRODUCAO A SET A.ULTIMO_SEQ = 'S'
                                                             WHERE A.CODCT = OLD.CODCT AND A.PROJETO = OLD.PROJETO AND A.NUM_DOC = OLD.NUM_DOC AND A.NUM_OBRA_ORIGINAL = OLD.NUM_OBRA_ORIGINAL AND A.SEQ = (OLD.SEQ - 1);
                                                    END").ToString();
        }

    }
}
