using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.Migration
{
    public class _20240722_1512_TB_CONFIG_PERFIL_ATIVACAO : AnielMigration
    {
        public _20240722_1512_TB_CONFIG_PERFIL_ATIVACAO()
        {
            Codigo = "20240722_1738";
            Descricao = "Adiciona colunas USUARIO_IMPORTACAO e DATAHORA_IMPORTACAO em TB_CONFIG_PERFIL_ATIVACAO";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"
                ALTER TABLE TB_CONFIG_PERFIL_ATIVACAO ADD USUARIO_IMPORTACAO VARCHAR(50);
                ALTER TABLE TB_CONFIG_PERFIL_ATIVACAO ADD DATAHORA_IMPORTACAO TIMESTAMP;
                ").ToString();
        }
    }
}
