using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class _20230518_1255_PREENCHE_TIPO_QUIZ : AnielMigration
    {
        public _20230518_1255_PREENCHE_TIPO_QUIZ()
        {
            Codigo = "20230518_1255";
            Descricao = "Preenchendo o campo TIPO nas checklists antigas";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            // Preenche o tipo das checklists antigas com o tipo 0, de checklist de serviço
            return new StringBuilder().AppendLine(@"
UPDATE TB_QUIZ 
SET TIPO = 0
WHERE TIPO IS NULL;").ToString();
        }

    }
}
