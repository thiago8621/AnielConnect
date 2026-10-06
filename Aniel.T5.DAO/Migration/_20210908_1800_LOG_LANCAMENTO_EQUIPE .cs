using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO
{
    public class _20210908_1800_LOG_LANCAMENTO_EQUIPE : AnielMigration
    {
        public _20210908_1800_LOG_LANCAMENTO_EQUIPE()
        {
            Codigo = "20210908_1800";
            Descricao = "Log lançamento equipe";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_log_lanc_servico_id for tb_log_lanc_servico
                                                    active before insert position 0
                                                    as
                                                    begin
                                                    if (new.ID is null) then
                                                    new.ID = gen_id(GEN_TB_LOG_LANC_SERVICO_ID,1);
                                                    IF(COALESCE(NEW.PROJETO,'') = '') THEN
                                                    NEW.PROJETO = (SELECT FIRST 1 PROJETO FROM TB_DOCUMENTO_PRODUCAO 
                                                    WHERE NUM_OBRA = NEW.NUM_OBRA AND CODCT = NEW.CODCT AND NUM_DOC = NEW.NUM_DOC);
                                                    end").ToString();
        }
    }
}
