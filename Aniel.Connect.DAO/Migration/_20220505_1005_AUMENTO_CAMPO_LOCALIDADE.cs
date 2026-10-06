using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20220505_1005_AUMENTO_CAMPO_LOCALIDADE : AnielMigration
    {
        public _20220505_1005_AUMENTO_CAMPO_LOCALIDADE()
        {
            Codigo = "_20220505_1005";
            Descricao = "Aumento do tamanho do campo de Localidade na TB_CLIENTE_PLANO_PRODUTO";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_CLIENTE_PLANO_PRODUTO ALTER COLUMN LOCALIDADE TYPE VARCHAR(50);").ToString();
        }
    }
}
