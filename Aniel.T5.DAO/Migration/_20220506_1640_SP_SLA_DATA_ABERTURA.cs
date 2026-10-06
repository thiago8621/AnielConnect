using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO
{
    public class _20220506_1640_SP_SLA_DATA_ABERTURA : AnielMigration
    {
        public _20220506_1640_SP_SLA_DATA_ABERTURA()
        {
            Codigo = "20220506_1640";
            Descricao = "Procedure para configurar data do SLA por abertura/agendamento";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_MASTER ADD SLA_DATA_ABERTURA INTEGER DEFAULT 0
                                       ; UPDATE TB_MASTER SET SLA_DATA_ABERTURA = 0 WHERE SLA_DATA_ABERTURA IS NULL
                                     ;").ToString();
        }

    }
}
