using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20250730_1111_CORRECAO_CAMPO_BOT : AnielMigration
    {
        public _20250730_1111_CORRECAO_CAMPO_BOT()
        {
            Codigo = "20250730_1134";
            Descricao = "Correcao campo BOT EVENTO";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_BOT_EVENTO ADD TEMP_TEXTO VARCHAR(600) CHARACTER SET UTF8;

                                                    UPDATE TB_BOT_EVENTO SET TEMP_TEXTO = TEXTO_ENVIADO;

                                                    ALTER TABLE TB_BOT_EVENTO DROP TEXTO_ENVIADO;

                                                    ALTER TABLE TB_BOT_EVENTO ADD TEXTO_ENVIADO VARCHAR(600) CHARACTER SET UTF8;

                                                    UPDATE TB_BOT_EVENTO SET TEXTO_ENVIADO = TEMP_TEXTO;

                                                    ALTER TABLE TB_BOT_EVENTO DROP TEMP_TEXTO;
                                                    ").ToString();
        }
    }
}
