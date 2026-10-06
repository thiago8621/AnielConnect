using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20200720_2007_TG_REMOVE_AGENDAMENTO : AnielMigration
    {
        public _20200720_2007_TG_REMOVE_AGENDAMENTO()
        {
            Codigo = "20200720_2007";
            Descricao = "Trigger para remover agendamento em casos da Os ser deletada";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER TRIGGER TG_REMOVE_AGENDAMENTO FOR TB_DOCUMENTO_PRODUCAO
                                                    ACTIVE BEFORE DELETE POSITION 0
                                                    AS
                                                    BEGIN
                                                        DELETE FROM TB_AGENDAMENTO_EQUIPEANIEL A WHERE A.NUM_OBRA = OLD.NUM_OBRA AND A.NUM_DOC = OLD.NUM_DOC AND A.CODCT = OLD.CODCT AND A.PROJETO = OLD.PROJETO;
                                                    END").ToString();
        }

    }
}
