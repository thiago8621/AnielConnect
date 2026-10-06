using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20260312_1700_TB_CLIENTE_PLANO_PRODUTO : AnielMigration
    {
        public _20260312_1700_TB_CLIENTE_PLANO_PRODUTO()
        {
            Codigo = "20260312_1700";
            Descricao = "Valor Plano Variavel Acao Update";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE TB_CLIENTE_PLANO_PRODUTO A
                                                    SET A.VLR_TOTAL_PLANO_VARIAVEL = A.VALOR_REAL
                                                    WHERE A.VLR_TOTAL_PLANO_VARIAVEL IS NULL;").ToString();
        }
    }
}
