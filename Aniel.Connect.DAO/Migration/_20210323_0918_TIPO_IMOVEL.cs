using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO.Migration
{
    public class _20210323_0918_TIPO_IMOVEL : AnielMigration
    {
        public _20210323_0918_TIPO_IMOVEL()
        {
            Codigo = "20210323_0918";
            Descricao = "Inclusão de Tipo de Imoveis";
            Tipo = TipoAnielMigration.ScriptUnico;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE OR INSERT INTO TB_TIPO_IMOVEL (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, COD_TIPO_IMOVEL, DESCRICAO)
                                                        VALUES (0, NULL, NULL, NULL, NULL, NULL, 0, 'INDIFERENTE')
                                                    MATCHING (ID);").ToString();
        }
    }
}
