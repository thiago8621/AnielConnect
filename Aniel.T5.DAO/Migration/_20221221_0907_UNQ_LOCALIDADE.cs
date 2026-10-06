using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20221221_0907_UNQ_AGENDAMENTO : AnielMigration
    {
        public _20221221_0907_UNQ_AGENDAMENTO()
        {
            Codigo = "20221221_0907";
            Descricao = "criacao de unique constraint UNQ_LOCALIDADE";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_LOCALIDADE ADD CONSTRAINT UNQ_LOCALIDADE UNIQUE (COD_ESCRITORIO, COD_AREA, COD_LOCALIDADE);").ToString();
        }

    }
}
