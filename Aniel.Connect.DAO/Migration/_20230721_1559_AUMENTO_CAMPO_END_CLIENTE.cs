using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO.Migration
{
    class _20230721_1559_AUMENTO_CAMPO_END_CLIENTE : AnielMigration
    {
        public _20230721_1559_AUMENTO_CAMPO_END_CLIENTE()
        {
            Codigo = "20230721_1559";
            Descricao = "Aumento dos campos do endereco do cliente";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {

            return new StringBuilder().AppendLine(@"ALTER TABLE TB_ENDERECO_CLIENTE ALTER COLUMN ENDERECO_LOGRADOURO TYPE VARCHAR(100);
ALTER TABLE TB_ENDERECO_CLIENTE ALTER COLUMN ENDERECO_BAIRRO TYPE VARCHAR(80);
ALTER TABLE TB_ENDERECO_CLIENTE ALTER COLUMN ENDERECO_CIDADE TYPE VARCHAR(80);").ToString();


        }
    }
}
