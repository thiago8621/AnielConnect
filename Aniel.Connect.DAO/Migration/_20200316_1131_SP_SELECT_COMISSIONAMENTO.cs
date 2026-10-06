using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO
{
    public class _20200316_1131_SP_SELECT_COMISSIONAMENTO : AnielMigration
    {
        public _20200316_1131_SP_SELECT_COMISSIONAMENTO()
        {
            Codigo = "20200316_1131";
            Descricao = "Venda - Select Comissão do Vendedor";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER PROCEDURE SP_SELECT_COMISSIONAMENTO (
                PID_VENDEDOR BIGINT, 
                PDATA_INICIO DATE, 
                PDATA_FIM DATE) 
            RETURNS ( 
                ID_VENDEDOR BIGINT, 
                NOME_VENDEDOR VARCHAR(50), 
                ID_SUPERVISOR BIGINT, 
                NOME_SUPERVISOR VARCHAR(50), 
                ID_FAIXA BIGINT, 
                ID_CLIENTE BIGINT, 
                NOME_CLIENTE VARCHAR(255), 
                ID_PLANO BIGINT, 
                ID_PRODUTO BIGINT, 
                PRODUTO_PLANO VARCHAR(255), 
                PONTOS DOUBLE PRECISION, 
                VALOR_FINAL DOUBLE PRECISION, 
                DATA_VENDA TIMESTAMP) 
            AS 
            BEGIN 
                FOR
                    SELECT A.NOME AS NOME_FUNCIONARIO, 
                   B.ID AS ID_SUPERVISOR, 
                   B.NOME AS NOME_SUPERVISOR, 
                   COALESCE(A.ID_FAIXA, B.ID_FAIXA, C.ID_FAIXA_COMISSIONAMENTO) AS ID_COMISSIONAMENTO, 
                   O.ID AS ID_CLIENTE, 
                   O.NOMEFANTASIA AS NOMEFANTASIA, 
                   E.ID_PLANO, 
                   E.ID_PRODUTO, 
                   COALESCE(I.QUANTIDADE,  0) AS PONTOS, 
                   COALESCE(E.VALOR_FINAL, 0) AS VALOR_FINAL, 
                   D.DATA_VENDA AS DATA_VENDA, 
                   COALESCE(L.DESCRICAO, M.DESCRICAO) AS PRODUTO_PLANO 
     
                FROM TB_FUNCIONARIO A 
                INNER JOIN TB_FUNCIONARIO B ON(A.ID_SUPERVISOR = B.ID) 
                LEFT JOIN TB_FILIAL C ON(A.ID_FILIAL = C.ID) 
                INNER JOIN TB_VENDA D ON(A.ID = D.ID_VENDEDOR) 
                INNER JOIN TB_CLIENTE_PLANO_PRODUTO E ON(E.ID_VENDA = D.ID) 
     
                LEFT JOIN TB_COMISSIONAMENTO G ON(G.ID_FILIAL = A.ID_FILIAL) -- FILIAL 
                LEFT JOIN TB_COMISSIONAMENTO P ON(P.ID = A.ID_COMISSIONAMENTO) -- VENDEDOR 
                LEFT JOIN TB_COMISSIONAMENTO Q ON(Q.ID = B.ID_COMISSIONAMENTO) -- SUPERVISOR 
     
                LEFT JOIN TB_COMISSAO_PLANO_PRODUTO I 
                ON(I.ID_COMISSIONAMENTO = COALESCE(P.ID, Q.ID, G.ID) AND (I.ID_PLANO = E.ID_PLANO OR I.ID_PRODUTO = E.ID_PRODUTO)) 
     
                LEFT JOIN TB_PRODUTO L ON (L.ID = E.ID_PRODUTO) 
                LEFT JOIN TB_PLANO M ON (M.ID = E.ID_PLANO) 
                INNER JOIN TB_CLIENTE O ON (O.ID = D.ID_CLIENTE) 
     
                WHERE D.STATUS = 6 AND (A.ID = :pID_VENDEDOR) AND (CAST(D.DATA_VENDA AS DATE) BETWEEN :pDATA_INICIO AND :pDATA_FIM) 
                INTO :NOME_VENDEDOR, 
                 :ID_SUPERVISOR, 
                 :NOME_SUPERVISOR, 
                 :ID_FAIXA, 
                 :ID_CLIENTE, 
                 :NOME_CLIENTE, 
                 :ID_PLANO, 
                 :ID_PRODUTO, 
                 :PONTOS, 
                 :VALOR_FINAL, 
                 :DATA_VENDA, 
                 :PRODUTO_PLANO 
                DO 
                BEGIN 
                  ID_VENDEDOR = :pID_VENDEDOR; 
                  SUSPEND; 
                END 
            END").ToString();
        }

    }
}
