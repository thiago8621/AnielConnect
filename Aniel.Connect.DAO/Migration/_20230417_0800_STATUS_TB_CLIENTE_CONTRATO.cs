using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20230417_0800_STATUS_TB_CLIENTE_CONTRATO : AnielMigration
    {
        public _20230417_0800_STATUS_TB_CLIENTE_CONTRATO()
        {
            Codigo = "20230417_0800";
            Descricao = "Realizar Update nos registros da tb_cliente_contrato";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }
        public override String GetSql()
        {
            return @"   
                -- Altera para Aguardando Ativação (1) todos os planos produtos que estão como 0, 1 ou 6
                UPDATE TB_CLIENTE_CONTRATO 
                SET STATUS = 1 
                WHERE ID IN (
                    SELECT ID_CLIENTE_CONTRATO
                    FROM TB_CLIENTE_PLANO_PRODUTO 
                    WHERE STATUS IN (0, 1, 6)
                );

                -- Altera para Ativo (2) caso todos os planos estejam habilitados
                UPDATE TB_CLIENTE_CONTRATO 
                SET STATUS = 2 
                WHERE ID IN (
                    SELECT DISTINCT ID 
                    FROM TB_CLIENTE_PLANO_PRODUTO 
                    WHERE ID_CLIENTE_CONTRATO IN (
                        SELECT ID_CLIENTE_CONTRATO
                        FROM TB_CLIENTE_PLANO_PRODUTO 
                        WHERE STATUS != 2
                    ) 
                    AND ID NOT IN (
                        SELECT ID 
                        FROM TB_CLIENTE_PLANO_PRODUTO 
                        WHERE STATUS != 2
                    )
                );


                -- Altera para Suspenso Empresa (3) caso todos os planos estejam suspensos pela empresa
                UPDATE TB_CLIENTE_CONTRATO 
                SET STATUS = 3
                WHERE ID IN (
                    SELECT DISTINCT ID 
                    FROM TB_CLIENTE_PLANO_PRODUTO 
                    WHERE ID_CLIENTE_CONTRATO IN (
                        SELECT ID_CLIENTE_CONTRATO
                        FROM TB_CLIENTE_PLANO_PRODUTO 
                        WHERE STATUS != 4
                    ) 
                    AND ID NOT IN (
                        SELECT ID 
                        FROM TB_CLIENTE_PLANO_PRODUTO 
                        WHERE STATUS != 4
                    )
                );

                -- Altera para Cancelado (4) caso todos os planos estejam cancelados
                UPDATE TB_CLIENTE_CONTRATO 
                SET STATUS = 4
                WHERE ID IN (
                    SELECT DISTINCT ID 
                    FROM TB_CLIENTE_PLANO_PRODUTO 
                    WHERE ID_CLIENTE_CONTRATO IN (
                        SELECT ID_CLIENTE_CONTRATO
                        FROM TB_CLIENTE_PLANO_PRODUTO 
                        WHERE STATUS != 5
                    ) 
                    AND ID NOT IN (
                        SELECT ID 
                        FROM TB_CLIENTE_PLANO_PRODUTO 
                        WHERE STATUS != 5
                    )
                );


                ";
        }
    }
}
