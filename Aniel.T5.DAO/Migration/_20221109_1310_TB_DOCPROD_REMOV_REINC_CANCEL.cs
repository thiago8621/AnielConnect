using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class _20221109_1310_TB_DOCPROD_REMOV_REINC_CANCEL : AnielMigration
    {
        public _20221109_1310_TB_DOCPROD_REMOV_REINC_CANCEL()
        {
            Codigo = "20221109_1310";
            Descricao = "Remover registro de reincidencia para OS cancelada";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"CREATE OR ALTER trigger tg_docprod_remov_reinc_cancel for tb_documento_producao
active before update position 0
AS
BEGIN
    IF (NEW.STATUS_EXECUCAO = 9) THEN
    BEGIN
        DELETE FROM TB_REINCIDENCIA_DOCPROD R
        WHERE (R.CODCT = NEW.CODCT AND
               R.PROJETO = NEW.PROJETO AND
               R.NUM_OBRA = NEW.NUM_OBRA AND
               R.NUM_DOC = NEW.NUM_DOC) OR
              (R.CODCT_ANTERIOR = NEW.CODCT AND
               R.PROJETO_ANTERIOR = NEW.PROJETO AND
               R.NUM_OBRA_ANTERIOR = NEW.NUM_OBRA AND
               R.NUM_DOC_ANTERIOR = NEW.NUM_DOC);
    END
END").ToString();
        }

    }
}
