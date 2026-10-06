using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20201017_1551_TG_TB_PONTO_FOTO : AnielMigration
    {
        public _20201017_1551_TG_TB_PONTO_FOTO()
        {
            Codigo = "20201017_1551";
            Descricao = "Trigger";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_tb_ponto_foto for tb_pt_foto
                                                    active before insert position 0
                                                    AS 
                                                    BEGIN
                                                        NEW.ID = GEN_ID(GEN_ID_TB_PT_FOTO, 1);
                                                    END").ToString();
        }
    }
}
