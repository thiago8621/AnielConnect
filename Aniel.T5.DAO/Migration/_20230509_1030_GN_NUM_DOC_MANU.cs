using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20230509_1030_GN_NUM_DOC_MANU : AnielMigration
    {
        public _20230509_1030_GN_NUM_DOC_MANU()
        {
            Codigo = "20230509_1030";
            Descricao = "Criação do generator GN_NUM_DOC_MANU";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE GENERATOR GN_NUM_DOC_MANU;").ToString();
        }

    }
}
