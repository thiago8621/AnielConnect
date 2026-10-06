using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20240425_0955_CAMPOSCONTRATO : AnielMigration
    {
        public _20240425_0955_CAMPOSCONTRATO()
        {
            Codigo = "20240425_0956";
            Descricao = "criacao de novos campos contrato";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"
                     ALTER TABLE TB_CONTRATO ADD ENCERRA_OS_AUTO DM_LOGICO;
                     ALTER TABLE TB_CONTRATO ADD DIAS_ENCERRAMENTO_AUTO SMALLINT;
                     ALTER TABLE TB_CONTRATO ADD COD_ENCERRAMENTO_AUTO VARCHAR(6) CHARACTER SET iSO8859_1 COLLATE iSO8859_1;").ToString();
        }

    }
}
