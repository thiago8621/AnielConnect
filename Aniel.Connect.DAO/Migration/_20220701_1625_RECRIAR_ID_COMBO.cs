using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO.Migration
{
    /// <summary>
    ///     O antigo campo ID_COMBO possuia uma FK que criava uma relação com a TB_CLIENTE_COMBO.
    ///     Porém como a estrutura do combo sofreu um retrabalho, o combo agora será gravado na TB_COMBO.
    ///     Esse script remove o campo antigo e gera um novo campo de mesmo nome mas com FK apontando para a nova tabela TB_COMBO.
    /// </summary>
    public class _20220701_1625_RECRIAR_ID_COMBO : AnielMigration
    {
        public _20220701_1625_RECRIAR_ID_COMBO()
        {
            Codigo = "20220701_1625";
            Descricao = "Recriando o campo ID_COMBO";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"
                ALTER TABLE TB_CLIENTE_PLANO_PRODUTO DROP ID_COMBO;
                ALTER TABLE TB_CLIENTE_PLANO_PRODUTO ADD ID_COMBO BIGINT;
                ALTER TABLE TB_CLIENTE_PLANO_PRODUTO ADD CONSTRAINT FK_CLIENTE_PP_COMBO FOREIGN KEY (ID_COMBO) REFERENCES TB_COMBO(ID);
            ").ToString();
        }
    }
}
