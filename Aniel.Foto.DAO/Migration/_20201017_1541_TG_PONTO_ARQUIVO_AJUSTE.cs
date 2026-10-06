using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20201017_1541_TG_PONTO_ARQUIVO_AJUSTE : AnielMigration
    {
        public _20201017_1541_TG_PONTO_ARQUIVO_AJUSTE()
        {
            Codigo = "20201017_1541";
            Descricao = "Trigger";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_ponto_arquivo_ajuste for tb_ponto_arquivo_ajuste
                                                    active before insert position 0
                                                    as
                                                    begin
                                                      if (new.id is null) then
                                                        new.id = gen_id(gen_ponto_arquivo_ajuste,1);
                                                    end").ToString();
        }
    }
}
