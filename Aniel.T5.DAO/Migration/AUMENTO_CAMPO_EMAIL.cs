using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    class AUMENTO_CAMPO_EMAIL : AnielMigration
    {
        public AUMENTO_CAMPO_EMAIL()
        {
            Codigo = "20231019_1559";
            Descricao = "Aumento dos campos de Email do cliente";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_CLIENTE ALTER COLUMN EMAIL TYPE VARCHAR(150);").ToString();
        }
    }
}
