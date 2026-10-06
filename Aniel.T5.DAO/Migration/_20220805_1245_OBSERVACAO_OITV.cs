using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO
{
    public class _20220805_1245_OBSERVACAO_OITV : AnielMigration
    {
        public _20220805_1245_OBSERVACAO_OITV()
        {
            Codigo = "20220805_1245";
            Descricao = "Ajuste Coluna OBSERVACAO_OITV";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_DOCUMENTO_PRODUCAO DROP OBSERVACAO_OITV;
                                                    ALTER TABLE TB_DOCUMENTO_PRODUCAO ADD OBSERVACAO_OITV BLOB SUB_TYPE 1 SEGMENT SIZE 4096;").ToString();


        }

    }
}

