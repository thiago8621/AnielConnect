using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20220429_1055_PROCEDURE_SET_CAMPANHA_PLANO_PRODUTO_TYPE : AnielMigration
    {
        public _20220429_1055_PROCEDURE_SET_CAMPANHA_PLANO_PRODUTO_TYPE()
        {
            Codigo = "20220429_1055";
            Descricao = "Procedure para Realizar Update nos registros da Tb_Campanha_Plano_Produto";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }
        public override String GetSql()
        {
            return @"   
      CREATE OR ALTER PROCEDURE SP_SET_CAMP_PLAN_PROD_TYPE
        RETURNS(RNUM BIGINT)
        AS
        DECLARE PE_PLANOPRODUTO INT NOT NULL;

        BEGIN

        FOR
            SELECT (CASE
        WHEN EXISTS(SELECT A.ID ,B.ID_PLANO_PRODUTO FROM TB_CAMPANHA_PLANO_PRODUTO B  INNER JOIN TB_PLANO A ON A.ID = B.ID_PLANO_PRODUTO
        WHERE B.ID_PLANO_PRODUTO BETWEEN 1 AND(SELECT MAX(B.ID) FROM TB_CAMPANHA_PLANO_PRODUTO B))THEN 1

        WHEN EXISTS (SELECT C.ID ,B.ID_PLANO_PRODUTO  FROM TB_CAMPANHA_PLANO_PRODUTO B  INNER JOIN TB_PRODUTO C ON C.ID = B.ID_PLANO_PRODUTO
        WHERE B.ID_PLANO_PRODUTO BETWEEN 1 AND(SELECT MAX(B.ID) FROM TB_CAMPANHA_PLANO_PRODUTO B)) THEN 2
                END)
                AS PLANOPRODUTO
        FROM TB_CAMPANHA_PLANO_PRODUTO B INTO :PE_PLANOPRODUTO
        DO
            BEGIN
              UPDATE TB_CAMPANHA_PLANO_PRODUTO X SET X.E_PLANO_PRODUTO = :PE_PLANOPRODUTO;
              RNUM = :PE_PLANOPRODUTO;
              SUSPEND;
            END
        END
";
        }
    }
}
