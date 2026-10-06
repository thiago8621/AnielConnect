using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20260408_1048_UPDATE_DATA_ULTIMO_CONTATO : AnielMigration
    {
        public _20260408_1048_UPDATE_DATA_ULTIMO_CONTATO()
        {
            Codigo = "20260408_1048";
            Descricao = "Atualização de campo de data";
            Tipo = TipoAnielMigration.ScriptUnico;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(
            @"
               UPDATE TB_VENDA A
                SET A.DATA_ULTIMO_CONTATO = (
                    SELECT FIRST 1 B.DATA_ULTIMO_CONTATO
                    FROM TB_VENDA_HISTORICO_CONTATO B
                    WHERE B.ID_VENDA = A.ID
                    ORDER BY B.DATA_ULTIMO_CONTATO DESC
                )
                WHERE A.DATA_ULTIMO_CONTATO IS NULL
            ").ToString();
        }
    }
}
