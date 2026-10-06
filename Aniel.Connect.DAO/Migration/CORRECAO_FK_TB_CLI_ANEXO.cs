using Sinapse.DAO;
using System;
using System.Text;

namespace Aniel.T5.DAO
{
    class CORRECAO_FK_TB_CLI_ANEXO : AnielMigration
    {
        public CORRECAO_FK_TB_CLI_ANEXO()
        {
            Codigo = "20240618_1044";
            Descricao = "Correcao da FK on Delete";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(
            @"
                ALTER TABLE TB_CLIENTE_CONTRATO DROP CONSTRAINT FK_CLI_CT_CLIENTE_ANX;
                ALTER TABLE TB_CLIENTE_CONTRATO ADD CONSTRAINT FK_CLI_CT_CLIENTE_ANX FOREIGN KEY (ID_CLIENTE_ANEXO) REFERENCES TB_CLIENTE_ANEXO(ID) ON DELETE SET NULL;
            ").ToString();            
        }
    }
}
