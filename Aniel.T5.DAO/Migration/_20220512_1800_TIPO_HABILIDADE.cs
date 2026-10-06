using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20220512_1800_TIPO_HABILIDADE : AnielMigration
    {
        public _20220512_1800_TIPO_HABILIDADE()
        {
            Codigo = "20220512_1800";
            Descricao = "criacao de tabela TB_TIPO_HABILIDADE_NATUREZA";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE TABLE TB_TIPO_HABILIDADE_NATUREZA (
    VALOR      INTEGER PRIMARY KEY,
    DESCRICAO  VARCHAR(255)
);").ToString();
        }

    }
}
