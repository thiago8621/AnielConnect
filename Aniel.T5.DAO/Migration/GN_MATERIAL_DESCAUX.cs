using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class GN_MATERIAL_DESCAUX : AnielMigration
    {
        public GN_MATERIAL_DESCAUX()
        {
            Codigo = "20240503_0900";
            Descricao = "Criação do generator GN_MATERIAL_DESCAUX";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE GENERATOR GN_MATERIAL_DESCAUX;").ToString();
        }

    }
}
