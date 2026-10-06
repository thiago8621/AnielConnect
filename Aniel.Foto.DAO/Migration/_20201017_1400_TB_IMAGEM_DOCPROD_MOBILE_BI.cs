using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20201017_1400_TB_IMAGEM_DOCPROD_MOBILE_BI : AnielMigration
    {
        public _20201017_1400_TB_IMAGEM_DOCPROD_MOBILE_BI()
        {
            Codigo = "20201017_1400";
            Descricao = "Trigger";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tb_imagem_docprod_mobile_bi for tb_imagem_docprod_mobile
                                                    active before insert position 0
                                                    as
                                                    begin
                                                      if (new.id is null) then
                                                        new.id = gen_id(gen_tb_imagem_docprod_mobile_id, 1);
                                                    end ").ToString();
        }
    }
}
