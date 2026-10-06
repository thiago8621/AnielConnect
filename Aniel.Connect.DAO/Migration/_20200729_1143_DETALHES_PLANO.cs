using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20200729_1143_DETALHES_PLANO : AnielMigration
    {
        public _20200729_1143_DETALHES_PLANO()
        {
            Codigo = "20200729_1143";
            Descricao = "Plano - Detalhes Plano";
            Tipo = TipoAnielMigration.DDL;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_PLANO ADD TEMP VARCHAR(255); 
                                                    UPDATE TB_PLANO A SET A.TEMP = A.DETALHESPLANO; 
                                                    ALTER TABLE TB_PLANO DROP DETALHESPLANO; 
                                                    ALTER TABLE TB_PLANO ADD DETALHESPLANO BLOB SUB_TYPE 1 SEGMENT SIZE 80; 
                                                    UPDATE TB_PLANO A SET A.DETALHESPLANO = A.TEMP; 
                                                    ALTER TABLE TB_PLANO DROP TEMP;").ToString();
        }
    }
}
