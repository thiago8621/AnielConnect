using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20220223_1455_AUMENTO_CAMPO_EMAIL_CLIENTE : AnielMigration
    {
        public _20220223_1455_AUMENTO_CAMPO_EMAIL_CLIENTE()
        {
            Codigo = "20220223_1455";
            Descricao = "Aumento do tamanho dos campos de email na TB_CLIENTE";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_CLIENTE ALTER COLUMN EMAIL_PRINCIPAL TYPE VARCHAR(150);
                                                    ALTER TABLE TB_CLIENTE ALTER COLUMN EMAIL_AUX TYPE VARCHAR(150);").ToString();
        }
    }
}
