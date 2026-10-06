using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20231220_0914_AUMENTO_CAMPO_TOKEN_ACESSO : AnielMigration
    {
        public _20231220_0914_AUMENTO_CAMPO_TOKEN_ACESSO()
        {
            Codigo = "20231220_0914";
            Descricao = "Aumento do tamanho do campo de Token de Acesso na TB_INTEGRACOES_EXTERNAS";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"
                                                    ALTER TABLE TB_INTEGRACOES_EXTERNAS ADD TEMP BLOB SUB_TYPE 1 SEGMENT SIZE 80;
	                                                UPDATE TB_INTEGRACOES_EXTERNAS SET TEMP = TOKEN_ACESSO;

	                                                ALTER TABLE TB_INTEGRACOES_EXTERNAS DROP TOKEN_ACESSO;
	                                                ALTER TABLE TB_INTEGRACOES_EXTERNAS ADD TOKEN_ACESSO BLOB SUB_TYPE 1 SEGMENT SIZE 80;
	                                                
	                                                UPDATE TB_INTEGRACOES_EXTERNAS SET TOKEN_ACESSO = TEMP;	
	                                                ALTER TABLE TB_INTEGRACOES_EXTERNAS DROP TEMP;
                                                    ")
                                                  .ToString();
        }
    }
}
