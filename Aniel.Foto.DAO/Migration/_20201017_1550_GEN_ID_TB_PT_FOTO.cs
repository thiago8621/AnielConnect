using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20201017_1550_GEN_ID_TB_PT_FOTO : AnielMigration
    {
        public _20201017_1550_GEN_ID_TB_PT_FOTO()
        {
            Codigo = "20201017_1550";
            Descricao = "Generator";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"EXECUTE BLOCK AS BEGIN
                                                        if (not exists(select 1 from rdb$generators a where a.rdb$generator_name = 'GEN_ID_TB_PT_FOTO')) then
                                                    execute statement 'CREATE SEQUENCE GEN_ID_TB_PT_FOTO START WITH 0 INCREMENT BY 1;';
                                                    END").ToString();
        }
    }
}
