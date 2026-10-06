using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20201017_1540_GEN_PONTO_ARQUIVO_AJUSTE : AnielMigration
    {
        public _20201017_1540_GEN_PONTO_ARQUIVO_AJUSTE()
        {
            Codigo = "20201017_1540";
            Descricao = "Generator";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"EXECUTE BLOCK AS BEGIN
                                                        if (not exists(select 1 from rdb$generators a where a.rdb$generator_name = 'GEN_TB_PONTO_ARQUIVO_AJUSTE')) then
                                                    execute statement 'CREATE SEQUENCE GEN_TB_PONTO_ARQUIVO_AJUSTE START WITH 0 INCREMENT BY 1;';
                                                    END").ToString();
        }
    }
}
