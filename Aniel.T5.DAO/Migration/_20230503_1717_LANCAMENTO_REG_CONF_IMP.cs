using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20230503_1717_LANCAMENTO_REG_CONF_IMP : AnielMigration
    {
        public _20230503_1717_LANCAMENTO_REG_CONF_IMP()
        {
            Codigo = "20230503_1717";
            Descricao = "Inclusão regs conf. Importação. O.S";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_NET (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO)
                                                   VALUES (65, 'DATA_CRIACAO', 'Data Criação', 'Data Criação', 1, 'N')
                                                   MATCHING (ID);

                                                   UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_NET (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO)
                                                   VALUES (66, 'HORA_CRIACAO', 'Hora Criação', 'Hora Criação', 1, 'N')
                                                   MATCHING (ID);").ToString();
        }

    }
}
