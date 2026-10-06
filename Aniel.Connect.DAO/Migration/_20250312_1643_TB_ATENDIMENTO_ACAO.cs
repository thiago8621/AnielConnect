using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20250312_1643_TB_ATENDIMENTO_ACAO : AnielMigration
    {
        public _20250312_1643_TB_ATENDIMENTO_ACAO()
        {
            Codigo = "20250312_1643";
            Descricao = "Atendimento Acao Update";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_ATENDIMENTO_ACAO ADD TEMP BLOB SUB_TYPE TEXT; 
                                                    UPDATE TB_ATENDIMENTO_ACAO A SET A.TEMP = A.HTML; 
                                                    ALTER TABLE TB_ATENDIMENTO_ACAO DROP HTML; 
                                                    ALTER TABLE TB_ATENDIMENTO_ACAO ADD HTML BLOB_TEXT_NONE; 
                                                    UPDATE TB_ATENDIMENTO_ACAO A SET A.HTML = A.TEMP; 
                                                    ALTER TABLE TB_ATENDIMENTO_ACAO DROP TEMP;").ToString();
        }
    }
}
