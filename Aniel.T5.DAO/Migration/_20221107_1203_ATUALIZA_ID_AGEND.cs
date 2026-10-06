using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class _20221107_1203_ATUALIZA_ID_AGEND : AnielMigration
    {
        public _20221107_1203_ATUALIZA_ID_AGEND()
        {
            Codigo = "20221107_1203";
            Descricao = "Script para atualizar o ID_AGENDAMENTO das OSs anteriores";
            Tipo = TipoAnielMigration.ScriptUnico;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"EXECUTE BLOCK AS
    DECLARE V_ID_AGENDAMENTO BIGINT;
    DECLARE V_CODCT VARCHAR(4);
    DECLARE V_PROJETO VARCHAR(20);
    DECLARE V_NUM_DOC INTEGER;
    DECLARE V_NUM_OBRA VARCHAR(50);
BEGIN
    FOR
        SELECT A.ID, A.CODCT, A.PROJETO, A.NUM_DOC, A.NUM_OBRA
        FROM TB_AGENDAMENTO_EQUIPEANIEL A
        INTO :V_ID_AGENDAMENTO, :V_CODCT, :V_PROJETO, :V_NUM_DOC, :V_NUM_OBRA
    DO
    BEGIN
        UPDATE TB_DOCUMENTO_PRODUCAO D
        SET D.ID_AGENDAMENTO = :V_ID_AGENDAMENTO
        WHERE D.CODCT = :V_CODCT AND
              D.PROJETO = :V_PROJETO AND
              D.NUM_DOC = :V_NUM_DOC AND
              D.NUM_OBRA = :V_NUM_OBRA;
    END
END").ToString();
        }

    }
}
