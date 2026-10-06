using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20260408_1000_TB_VENDA_HISTORICO_CONTATO_TRIGGER : AnielMigration
    {
        public _20260408_1000_TB_VENDA_HISTORICO_CONTATO_TRIGGER()
        {
            Codigo = "20260408_1000";
            Descricao = "Venda Historico Contato Trigger";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }
        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE TRIGGER TG_UPD_DATA_ULTIMO_CONTATO
                                                    FOR TB_VENDA_HISTORICO_CONTATO
                                                    ACTIVE AFTER INSERT OR UPDATE
                                                    POSITION 0
                                                    AS
                                                    BEGIN
                                                      UPDATE TB_VENDA V
                                                      SET V.DATA_ULTIMO_CONTATO = (
                                                          SELECT FIRST 1 H.DATA_ULTIMO_CONTATO
                                                          FROM TB_VENDA_HISTORICO_CONTATO H
                                                          WHERE H.ID_VENDA = NEW.ID_VENDA
                                                          ORDER BY H.DATA_ULTIMO_CONTATO DESC
                                                      )
                                                      WHERE V.ID = NEW.ID_VENDA;
                                                    END").ToString();
        }
    }
}
