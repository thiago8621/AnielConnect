using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20230322_1600_AUMENTO_CAMPO_SCRIPT_AT : AnielMigration
    {
        public _20230322_1600_AUMENTO_CAMPO_SCRIPT_AT()
        {
            Codigo = "_20230322_1600";
            Descricao = "Aumento do tamanho do campo Pergunta na TB_ATEND_SCRIPT_ATEND";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {

            return new StringBuilder().AppendLine(@"ALTER TABLE TB_ATEND_SCRIPT_ATEND ADD TEMP VARCHAR(255); 
                                                    UPDATE TB_ATEND_SCRIPT_ATEND A SET A.TEMP = A.PERGUNTA; 
                                                    ALTER TABLE TB_ATEND_SCRIPT_ATEND DROP PERGUNTA; 
                                                    ALTER TABLE TB_ATEND_SCRIPT_ATEND ADD PERGUNTA BLOB SUB_TYPE 1 SEGMENT SIZE 80; 
                                                    UPDATE TB_ATEND_SCRIPT_ATEND A SET A.PERGUNTA = A.TEMP; 
                                                    ALTER TABLE TB_ATEND_SCRIPT_ATEND DROP TEMP;").ToString();


        }
    }
}
