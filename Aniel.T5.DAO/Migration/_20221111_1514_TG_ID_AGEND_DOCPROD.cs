using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class _20221111_1514_TG_ID_AGEND_DOCPROD : AnielMigration
    {
        public _20221111_1514_TG_ID_AGEND_DOCPROD()
        {
            Codigo = "20221111_1514";
            Descricao = "Limpa o ID_AGENDAMENTO da OS ao remover o agendamento do banco";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"CREATE OR ALTER TRIGGER TG_ID_AGEND_DOCPROD FOR TB_AGENDAMENTO_EQUIPEANIEL
ACTIVE BEFORE DELETE POSITION 0
AS
BEGIN
    UPDATE TB_DOCUMENTO_PRODUCAO D
    SET D.ID_AGENDAMENTO = NULL
    WHERE D.CODCT = OLD.CODCT AND
          D.PROJETO = OLD.PROJETO AND
          D.NUM_OBRA = OLD.NUM_OBRA AND
          D.NUM_DOC = OLD.NUM_DOC;
END").ToString();
        }

    }
}
