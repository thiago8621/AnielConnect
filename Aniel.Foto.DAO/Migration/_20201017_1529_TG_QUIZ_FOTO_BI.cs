using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20201017_1529_TG_QUIZ_FOTO_BI : AnielMigration
    {
        public _20201017_1529_TG_QUIZ_FOTO_BI()
        {
            Codigo = "20201017_1529";
            Descricao = "Trigger";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_quiz_foto_bi for tb_quiz_foto
                                                    active before insert position 0
                                                    as
                                                    begin
                                                      if (new.id is null) then
                                                        new.id = gen_id(GEN_TB_QUIZ_FOTO_ID,1);
                                                    end").ToString();
        }
    }
}
