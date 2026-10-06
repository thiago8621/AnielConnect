using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO
{
    public class _20201230_1034_SP_MIGRAR_FORMA_PAGTO_CLIENTE : AnielMigration
    {
        public _20201230_1034_SP_MIGRAR_FORMA_PAGTO_CLIENTE()
        {
            Codigo = "20201230_1034";
            Descricao = "Execução da procedure de Migração Forma de Pagamento do Cliente";
            Tipo = TipoAnielMigration.Select;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"SELECT * FROM SP_MIGRAR_FORMA_PAGTO_CLIENTE").ToString();
        }

    }
}
