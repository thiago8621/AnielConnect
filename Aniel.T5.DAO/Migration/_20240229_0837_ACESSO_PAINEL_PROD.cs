using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{

    public class _20240229_0837_ACESSO_PAINEL_PROD : AnielMigration
    {
        public _20240229_0837_ACESSO_PAINEL_PROD()
        {
            Codigo = "20240229_0844";
            Descricao = "Areas de Acesso Connect";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '354', 'Visualizar Painel Produtividade', 'N', 'Visualizar Painel Produtividade')
                            MATCHING (SIGLA_MODULO, COD_AREA);
").ToString();


        }

    }
}

