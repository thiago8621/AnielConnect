using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO
{
    public class _20210427_1640_SP_SLA_DATA_ABERTURA : AnielMigration
    {
        public _20210427_1640_SP_SLA_DATA_ABERTURA()
        {
            Codigo = "20210427_1640";
            Descricao = "Procedure para configurar data do SLA por abertura/agendamento";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_CONFIGURACOES_GERAIS ADD SLA_DATA_ABERTURA INTEGER DEFAULT 0
                                       ; UPDATE tb_configuracoes_gerais set sla_data_abertura = 0 where sla_data_abertura is null
                                     ;").ToString();
        }

    }
}
