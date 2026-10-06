using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20220623_0945_UNQ_AGENDAMENTO : AnielMigration
    {
        public _20220623_0945_UNQ_AGENDAMENTO()
        {
            Codigo = "20220623_0945";
            Descricao = "criacao de unique constraint UNQ_AGENDAMENTO_OS";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_AGENDAMENTO_EQUIPEANIEL ADD CONSTRAINT UNQ_AGENDAMENTO_OS UNIQUE (CODCT, PROJETO, NUM_OBRA, NUM_DOC);").ToString();
        }

    }
}
