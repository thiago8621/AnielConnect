using System.Text;
using Sinapse.DAO;

namespace Aniel.Connect.DAO
{
    public class _20240126_1555_TG_ALTERAR_ETAPA_ATUAL_VENDA : AnielMigration
    {
        public _20240126_1555_TG_ALTERAR_ETAPA_ATUAL_VENDA()
        {
            Codigo = "20240126_1555";
            Descricao = "Trigger para preencher o fk de etapa_venda na venda";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE TRIGGER TG_ALTERAR_ETAPA_ATUAL_VENDA FOR TB_ETAPA_VENDA
ACTIVE AFTER UPDATE
AS
BEGIN
    IF(OLD.STATUS <> NEW.STATUS) THEN
    BEGIN
       UPDATE TB_VENDA SET ID_ETAPA_VENDA = COALESCE((
            SELECT FIRST 1 B.ID
                FROM TB_ETAPA_VENDA B
                WHERE B.ID_VENDA = NEW.ID_VENDA AND
                      B.STATUS IN (0)
                ORDER BY B.ETAPA),
            (SELECT FIRST 1 B.ID
                FROM TB_ETAPA_VENDA B
                WHERE B.ID_VENDA = NEW.ID_VENDA
                ORDER BY B.ETAPA DESC)) 
        WHERE ID = NEW.ID_VENDA;
    END
END").ToString();
        }

    }
}
