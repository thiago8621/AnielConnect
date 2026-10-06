using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class _20230405_1936_TAMANHO_BAIRRO_DOC_PROD : AnielMigration
    {
        public _20230405_1936_TAMANHO_BAIRRO_DOC_PROD()
        {
            Codigo = "20230405_1936";
            Descricao = "Aumentando o tamanho do campo bairro da TB_DOCUMENTO_PRODUCAO";
            Tipo = TipoAnielMigration.ScriptUnico;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_DOCUMENTO_PRODUCAO ALTER COLUMN ENDERECO_BAIRRO TYPE VARCHAR(100);").ToString();
        }
    }
}
