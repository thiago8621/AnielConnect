using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20240229_1556_AREA_ACESSO_VENDA : AnielMigration
    {
        public _20240229_1556_AREA_ACESSO_VENDA()
        {
            Codigo = "_20240229_1556";
            Descricao = "Area de Acesso Venda 2";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '353', 'Permitir Cancelar Vendas individual e/ou em Massa', 'N', 'Permitir Cancelar Vendas individual e/ou em Massa')
                            MATCHING (SIGLA_MODULO, COD_AREA);").ToString();
        }
    }
}
