using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class _20230822_1400_TB_CATEGORIA_CLIENTE_CONNECT : AnielMigration
    {
        public _20230822_1400_TB_CATEGORIA_CLIENTE_CONNECT()
        {
            Codigo = "20230822_1400";
            Descricao = "Adiciona coluna TIPO_PRIORIDADE em TB_CATEGORIA_CLIENTE_CONNECT";
            Tipo = TipoAnielMigration.ScriptUnico;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_CATEGORIA_CLIENTE_CONNECT ADD TIPO_PRIORIDADE SMALLINT;").ToString();
        }
    }
}
