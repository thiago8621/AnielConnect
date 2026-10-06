using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO
{
    public class _20250212_1512_TB_LOG_IMPORTACAO : AnielMigration
    {
        public _20250212_1512_TB_LOG_IMPORTACAO()
        {
            Codigo = "20250213_1111";
            Descricao = "Ajuste de Tamanho do campo ERRO da tabela TB_LOG_IMPORTACAO";
            Tipo = TipoAnielMigration.ScriptUnico;
        }


        public override string GetSql()
        {
            return new StringBuilder(@" ALTER TABLE TB_LOG_IMPORTACAO ALTER COLUMN ERRO TYPE VARCHAR(255)").ToString();
        }

    }
}
