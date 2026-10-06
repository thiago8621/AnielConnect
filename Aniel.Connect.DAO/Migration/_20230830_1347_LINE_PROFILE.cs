using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20230830_1347_LINE_PROFILE : AnielMigration
    {
        public _20230830_1347_LINE_PROFILE()
        {
            Codigo = "20230830_1347";
            Descricao = "Alteração dos campos LINE_PROFILE para VARCHAR";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_PERFIL_ATIVACAO ADD TEMP VARCHAR(100); 
                                                    UPDATE TB_PERFIL_ATIVACAO A SET A.TEMP = CAST(A.LINE_PROFILE AS VARCHAR(100)); 
                                                    ALTER TABLE TB_PERFIL_ATIVACAO DROP LINE_PROFILE; 
                                                    ALTER TABLE TB_PERFIL_ATIVACAO ADD LINE_PROFILE VARCHAR(100); 
                                                    UPDATE TB_PERFIL_ATIVACAO A SET A.LINE_PROFILE = A.TEMP; 
                                                    ALTER TABLE TB_PERFIL_ATIVACAO DROP TEMP;
                                                    ALTER TABLE TB_SLOT ADD TEMP VARCHAR(100); 
                                                    UPDATE TB_SLOT A SET A.TEMP = CAST(A.LINE_PROFILE AS VARCHAR(100)); 
                                                    ALTER TABLE TB_SLOT DROP LINE_PROFILE; 
                                                    ALTER TABLE TB_SLOT ADD LINE_PROFILE VARCHAR(100); 
                                                    UPDATE TB_SLOT A SET A.LINE_PROFILE = A.TEMP; 
                                                    ALTER TABLE TB_SLOT DROP TEMP;").ToString();
        }
    }
}
