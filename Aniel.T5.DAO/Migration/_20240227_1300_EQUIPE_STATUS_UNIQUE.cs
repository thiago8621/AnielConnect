using Sinapse.DAO;
using System.Text;

public class _20240227_1300_PREENCHE_ULTIMO_SEQ : AnielMigration
{

    public _20240227_1300_PREENCHE_ULTIMO_SEQ()
    {
        Codigo = "20240227_1300";
        Descricao = "Adicionando constraint Unique na TB_EQUIPE_STATUS";
        Tipo = TipoAnielMigration.ProcedureTrigger;
    }
    public override string GetSql()
    {
        var Sql = new StringBuilder();
        Sql.AppendLine($@"ALTER TABLE tb_equipe_status ADD CONSTRAINT EQUIPE_UNQ UNIQUE (EQUIPE);");
        return Sql.ToString();
    }
}