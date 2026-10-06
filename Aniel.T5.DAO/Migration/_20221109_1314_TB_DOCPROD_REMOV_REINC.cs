using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class _20221109_1314_TB_DOCPROD_REMOV_REINC : AnielMigration
    {
        public _20221109_1314_TB_DOCPROD_REMOV_REINC()
        {
            Codigo = "20221109_1314";
            Descricao = "Remover registro de reincidencia para OS deletada";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"CREATE OR ALTER trigger tg_docprod_remov_reinc for tb_documento_producao
active before delete position 0
AS
BEGIN
    DELETE FROM TB_REINCIDENCIA_DOCPROD R
    WHERE (R.CODCT = OLD.CODCT AND
           R.PROJETO = OLD.PROJETO AND
           R.NUM_OBRA = OLD.NUM_OBRA AND
           R.NUM_DOC = OLD.NUM_DOC) OR
          (R.CODCT_ANTERIOR = OLD.CODCT AND
           R.PROJETO_ANTERIOR = OLD.PROJETO AND
           R.NUM_OBRA_ANTERIOR = OLD.NUM_OBRA AND
           R.NUM_DOC_ANTERIOR = OLD.NUM_DOC);
END").ToString();
        }

    }
}
