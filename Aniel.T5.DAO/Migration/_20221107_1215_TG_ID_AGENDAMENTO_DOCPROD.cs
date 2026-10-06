using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class _20221107_1215_TG_ID_AGENDAMENTO_DOCPROD : AnielMigration
    {
        public _20221107_1215_TG_ID_AGENDAMENTO_DOCPROD()
        {
            Codigo = "20221107_1215";
            Descricao = "Trigger para preencher o ID_AGENDAMENTO após o agendamento da OS";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"CREATE trigger tg_id_agendamento_docprod for tb_agendamento_equipeaniel
active after insert position 0
AS
BEGIN
    IF (NEW.CODCT IS NOT NULL AND
        NEW.PROJETO IS NOT NULL AND
        NEW.NUM_DOC IS NOT NULL AND
        NEW.NUM_OBRA IS NOT NULL) THEN
    BEGIN
        UPDATE TB_DOCUMENTO_PRODUCAO D
        SET D.ID_AGENDAMENTO = NEW.ID
        WHERE D.CODCT = NEW.CODCT AND
              D.PROJETO = NEW.PROJETO AND
              D.NUM_DOC = NEW.NUM_DOC AND
              D.NUM_OBRA = NEW.NUM_OBRA;
    END
END").ToString();
        }

    }
}
