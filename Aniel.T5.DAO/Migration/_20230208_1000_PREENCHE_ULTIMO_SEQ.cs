using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Aniel.T5.DAO.Migration
{
    public class _20230208_1000_PREENCHE_ULTIMO_SEQ : AnielMigration
    {

        public _20230208_1000_PREENCHE_ULTIMO_SEQ()
        {
            Codigo = "20230208_1000";
            Descricao = "CorrigePassivo";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }
        public override string GetSql()
        {
            var Sql = new StringBuilder();
            Sql.AppendLine($@" EXECUTE BLOCK as declare x int = 0;
                                    BEGIN
                                        UPDATE TB_DOCUMENTO_PRODUCAO A SET A.ULTIMO_SEQ = 'S'
                                        WHERE COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ),1)
                                                                    FROM TB_DOCUMENTO_PRODUCAO M 
                                                                    WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO 
                                                                        AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL 
                                                                        AND A.NUM_DOC = M.NUM_DOC);

                                        UPDATE TB_DOCUMENTO_PRODUCAO A SET A.ULTIMO_SEQ = 'N'
                                        WHERE COALESCE(A.SEQ, 1) <> (SELECT FIRST 1 COALESCE(MAX(M.SEQ),1)
                                                                    FROM TB_DOCUMENTO_PRODUCAO M 
                                                                    WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO 
                                                                        AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL 
                                                                        AND A.NUM_DOC = M.NUM_DOC);
                                END

                            ");

            return Sql.ToString();
        }
    }
}
