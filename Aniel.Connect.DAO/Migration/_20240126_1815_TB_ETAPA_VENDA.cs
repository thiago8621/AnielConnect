using System.Text;
using Sinapse.DAO;

namespace Aniel.Connect.DAO
{
    public class _20240126_1815_TB_ETAPA_VENDA : AnielMigration
    {
        public _20240126_1815_TB_ETAPA_VENDA()
        {
            Codigo = "20240126_1815";
            Descricao = "Script que atualiza a fk de etapa_venda nos registros antigos em TB_VENDA";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE TB_VENDA A
                    SET A.ID_ETAPA_VENDA = COALESCE(
                        (SELECT FIRST 1 B.ID
                         FROM TB_ETAPA_VENDA B
                         WHERE B.ID_VENDA = A.ID AND COALESCE(B.STATUS, 0) IN (0)
                         ORDER BY B.ETAPA),
                        (SELECT FIRST 1 B.ID
                         FROM TB_ETAPA_VENDA B
                         WHERE B.ID_VENDA = A.ID
                         ORDER BY B.ETAPA DESC)
                    );").ToString();
        }

    }
}
