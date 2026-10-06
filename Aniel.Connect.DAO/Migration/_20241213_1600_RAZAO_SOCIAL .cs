using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20241213_1600_RAZAO_SOCIAL : AnielMigration
    {
        public _20241213_1600_RAZAO_SOCIAL()
        {
            Codigo = "20241213_1600";
            Descricao = "Alteração dos campos RAZAO_SOCIAL para VARCHAR";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_CLIENTE ALTER COLUMN RAZAO_SOCIAL TYPE VARCHAR(200);").ToString();
        }
    }
}
