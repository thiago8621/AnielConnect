using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO.Migration
{
    class _20250901_1627_CORRECAO_CAMPO_ATT : AnielMigration
    {
        public _20250901_1627_CORRECAO_CAMPO_ATT()
        {
            Codigo = "20250901_1627";
            Descricao = "Correcao campo HTML";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_ATENDIMENTO_ACAO ADD TEMP_TEXTO BLOB SUB_TYPE TEXT CHARACTER SET UTF8;
                                                    UPDATE TB_ATENDIMENTO_ACAO SET TEMP_TEXTO = CAST(CAST(HTML AS BLOB SUB_TYPE TEXT CHARACTER SET ISO8859_1) AS BLOB SUB_TYPE TEXT CHARACTER SET UTF8)
                                                    ALTER TABLE TB_ATENDIMENTO_ACAO DROP HTML;
                                                    ALTER TABLE TB_ATENDIMENTO_ACAO ADD HTML BLOB SUB_TYPE TEXT CHARACTER SET UTF8;
                                                    UPDATE TB_ATENDIMENTO_ACAO SET HTML = TEMP_TEXTO;
                                                    ALTER TABLE TB_ATENDIMENTO_ACAO DROP TEMP_TEXTO;").ToString();
        }
    }
}
