using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20221205_1139_LANCAMENTO_REG_CONF_IMP : AnielMigration
    {
        public _20221205_1139_LANCAMENTO_REG_CONF_IMP()
        {
            Codigo = "20221205_1139";
            Descricao = "Inclusão regs conf. Importação. O.S";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_NET (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO)
                                                   VALUES (62, 'PPPOE', 'PPPOE', 'PPPOE', 1, 'N')
                                      MATCHING (ID);
                                                   UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_NET (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO)
                                        VALUES (63, 'SENHA_BAIXA', 'Senha Baixa', 'SENHA_BAIXA', 1, 'N')
                                                 MATCHING (ID);
                                        UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_NET (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO)
                                        VALUES (64, 'COD_ESCRITORIO', 'Escritório', 'Escritório', 1, 'N')
                                        MATCHING (ID);").ToString();
        }

    }
}
