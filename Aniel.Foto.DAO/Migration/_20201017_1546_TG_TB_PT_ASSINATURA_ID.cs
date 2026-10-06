using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20201017_1546_TG_TB_PT_ASSINATURA_ID : AnielMigration
    {
        public _20201017_1546_TG_TB_PT_ASSINATURA_ID()
        {
            Codigo = "20201017_1546";
            Descricao = "Trigger";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_tb_pt_assinatura_id for tb_pt_assinatura
                                                    active before insert position 0
                                                    AS
                                                    BEGIN
                                                        NEW.ID = GEN_ID(GEN_TB_PT_ASSINATURA, 1);
                                                    END").ToString();
        }
    }
}
