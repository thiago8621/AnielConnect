using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class DESCRICAO_TB_PENDENCIA_ENDERECO_DADOS : AnielMigration
    {
        public DESCRICAO_TB_PENDENCIA_ENDERECO_DADOS()
        {
            Codigo = "20250710_1800";
            Descricao = "Aumento do tamanho da descricao na TB_PENDENCIA_ENDERECO_DADOS";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"
                                                    ALTER TABLE TB_PENDENCIA_ENDERECO_DADOS ADD TEMP BLOB SUB_TYPE 1 SEGMENT SIZE 80;
	                                                UPDATE TB_PENDENCIA_ENDERECO_DADOS SET TEMP = DESCRICAO;

	                                                ALTER TABLE TB_PENDENCIA_ENDERECO_DADOS DROP DESCRICAO;
	                                                ALTER TABLE TB_PENDENCIA_ENDERECO_DADOS ADD DESCRICAO BLOB SUB_TYPE 1 SEGMENT SIZE 80;
	                                                
	                                                UPDATE TB_PENDENCIA_ENDERECO_DADOS SET DESCRICAO = TEMP;	
	                                                ALTER TABLE TB_PENDENCIA_ENDERECO_DADOS DROP TEMP;
                                                    ")
                                                  .ToString();
        }
    }
}
