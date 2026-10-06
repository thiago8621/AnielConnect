using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20250814_1100_BLOB_TEXT_NONE : AnielMigration
    {
        public _20250814_1100_BLOB_TEXT_NONE()
        {
            Codigo = "20250814_1100";
            Descricao = "Dominio BLOB_TEXT_NONE";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE DOMAIN BLOB_TEXT_NONE AS BLOB SUB_TYPE TEXT CHARACTER SET NONE;").ToString();
        }
    }
}
