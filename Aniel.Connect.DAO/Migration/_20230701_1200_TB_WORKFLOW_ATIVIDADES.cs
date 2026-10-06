using Sinapse.DAO;

namespace Aniel.Connect.DAO.Migration
{
    class _20230701_1200_TB_WORKFLOW_ATIVIDADES : AnielMigration
    {
        public _20230701_1200_TB_WORKFLOW_ATIVIDADES()
        {
            Codigo = "20230701_1200";
            Descricao = "Remoção de colunas em TB_WORKFLOW_ATIVIDADE";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }
        public override string GetSql()
        {
            return @"
            ALTER TABLE TB_WORKFLOW_ATIVIDADE DROP STATUS_PADRAO,
            ALTER TABLE TB_WORKFLOW_ATIVIDADE DROP ID_STATUS_PADRAO,
            ALTER TABLE TB_WORKFLOW_ATIVIDADE DROP ATIV_STATUSVENDA";
        }

    }
}
