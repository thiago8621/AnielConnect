using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class TG_REINCIDENCIA_DOC_PRODUCAO : AnielMigration
    {
        public TG_REINCIDENCIA_DOC_PRODUCAO()
        {
            Codigo = "20231005_2111";
            Descricao = "Trigger para lançamento de reincidencia";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"CREATE OR ALTER trigger tg_documento_producao_rein for tb_documento_producao
active after insert or update position 0
AS
    /*
        Quantidade maxima registrada no sistema de dias apos a criacao da OS em
        que se configura reincidencia.

        Todas as quantidades de dias registradas nas outras naturezas estarao
        dentro da quantidade maxima.
    */
    DECLARE VARIABLE V_QTD_MAX_DIAS_REINC INTEGER;
    DECLARE VARIABLE V_QTD_DIAS_REINC INTEGER;

    DECLARE VARIABLE V_GERA_REINC VARCHAR(1);

    DECLARE VARIABLE V_CODCT_ANTERIOR VARCHAR(4);
    DECLARE VARIABLE V_PROJETO_ANTERIOR VARCHAR(20);
    DECLARE VARIABLE V_NUM_OBRA_ANTERIOR VARCHAR(50);
    DECLARE VARIABLE V_NUM_DOC_ANTERIOR INTEGER;
    
    DECLARE VARIABLE V_COD_TIPO_SERV SMALLINT;

    DECLARE VARIABLE V_CODCT VARCHAR(4);
    DECLARE VARIABLE V_PROJETO VARCHAR(20);
    DECLARE VARIABLE V_NUM_OBRA VARCHAR(50);
    DECLARE VARIABLE V_NUM_OBRA_ORIGINAL VARCHAR(50);
    DECLARE VARIABLE V_NUM_DOC INTEGER;

    DECLARE VARIABLE V_DIA_OS_REINC DATE;
    DECLARE VARIABLE V_DATA_PROD_OS_ANT DATE;
    DECLARE VARIABLE V_DIA_HORA_OS_REINC TIMESTAMP;
BEGIN
    V_COD_TIPO_SERV = NEW.COD_TIPO_SERV;

    V_CODCT = NEW.CODCT;
    V_PROJETO = NEW.PROJETO;
    V_NUM_OBRA = NEW.NUM_OBRA;
    V_NUM_OBRA_ORIGINAL = NEW.NUM_OBRA_ORIGINAL;
    V_NUM_DOC = NEW.NUM_DOC;

    V_DIA_OS_REINC = NEW.DIA;
    V_DIA_HORA_OS_REINC = NEW.DIA + NEW.HORA;

    SELECT FIRST 1 COALESCE(N.REINCIDENCIA, 'N')
    FROM TB_TIPO_SERVICO_EQUIPE T
    INNER JOIN TB_NATUREZA_PRODUCAO N
    ON N.ID = T.COD_NATUREZA
    WHERE T.COD_TIPO_SERV = :V_COD_TIPO_SERV
    INTO :V_GERA_REINC;

    IF (:V_GERA_REINC = 'S') THEN
    BEGIN

        SELECT MAX(N.QTDDIAS_GERAREINCIDENCIA)
        FROM TB_NATUREZA_PRODUCAO N
        INTO :V_QTD_MAX_DIAS_REINC;

        FOR
            SELECT D.CODCT, D.PROJETO, D.NUM_OBRA, D.NUM_DOC, D.DATA_PROD,
                   COALESCE(N.QTDDIAS_GERAREINCIDENCIA,0)
            FROM TB_DOCUMENTO_PRODUCAO D

            INNER JOIN TB_TIPO_SERVICO_EQUIPE T
            ON T.COD_TIPO_SERV = D.COD_TIPO_SERV

            INNER JOIN TB_NATUREZA_PRODUCAO N
            ON N.ID = T.COD_NATUREZA

            WHERE D.CODCT = :V_CODCT AND
                  D.PROJETO = :V_PROJETO AND
                  D.NUM_OBRA <> :V_NUM_OBRA AND
                  D.NUM_DOC = :V_NUM_DOC AND
                  (:V_DIA_OS_REINC BETWEEN
                   D.DATA_PROD AND
                   DATEADD(DAY, :V_QTD_MAX_DIAS_REINC, D.DATA_PROD)) AND
                  D.STATUS_EXECUCAO = 2 AND
                  (D.DATA_PROD + D.HORA_PROD) < (:V_DIA_HORA_OS_REINC) AND 
                  NOT EXISTS(SELECT FIRST 1 R.*
                             FROM TB_REINCIDENCIA_DOCPROD R
                             WHERE R.CODCT_ANTERIOR = D.CODCT AND
                                   R.PROJETO_ANTERIOR = D.PROJETO AND
                                   R.NUM_DOC_ANTERIOR = D.NUM_DOC AND
                                   R.NUM_OBRA_ANTERIOR = D.NUM_OBRA) AND

                   NOT EXISTS(SELECT R.*
                   FROM TB_REINCIDENCIA_DOCPROD R
                   WHERE R.CODCT = D.CODCT AND
                         R.PROJETO = D.PROJETO AND
                         R.NUM_DOC = D.NUM_DOC AND
                         R.NUM_OBRA IN (SELECT DP.NUM_OBRA FROM TB_DOCUMENTO_PRODUCAO DP WHERE DP.CODCT = :V_CODCT AND
                                        DP.PROJETO = :V_PROJETO AND
                                        DP.NUM_DOC = :V_NUM_DOC AND
                                        DP.NUM_OBRA_ORIGINAL = :V_NUM_OBRA_ORIGINAL))
            ORDER BY D.DATA_PROD DESC
            INTO :V_CODCT_ANTERIOR, :V_PROJETO_ANTERIOR, :V_NUM_OBRA_ANTERIOR,
                 :V_NUM_DOC_ANTERIOR, :V_DATA_PROD_OS_ANT, :V_QTD_DIAS_REINC
        DO
        BEGIN
            IF ((:V_QTD_DIAS_REINC > 0) AND (:V_DIA_OS_REINC - :V_DATA_PROD_OS_ANT) <= :V_QTD_DIAS_REINC) THEN
            BEGIN
                UPDATE OR INSERT INTO TB_REINCIDENCIA_DOCPROD
                (CODCT, PROJETO, NUM_OBRA, NUM_DOC, CODCT_ANTERIOR,
                 PROJETO_ANTERIOR, NUM_OBRA_ANTERIOR, NUM_DOC_ANTERIOR, DATA)
                VALUES
                (NEW.CODCT, NEW.PROJETO, NEW.NUM_OBRA, NEW.NUM_DOC,
                :V_CODCT_ANTERIOR, :V_PROJETO_ANTERIOR, :V_NUM_OBRA_ANTERIOR,
                :V_NUM_DOC_ANTERIOR, :V_DIA_OS_REINC)
                MATCHING (CODCT, PROJETO, NUM_OBRA, NUM_DOC);

                LEAVE;
            END

            V_CODCT_ANTERIOR = NULL;
            V_PROJETO_ANTERIOR = NULL;
            V_NUM_OBRA_ANTERIOR = NULL;
            V_NUM_DOC_ANTERIOR = NULL;

            V_DATA_PROD_OS_ANT = NULL;
            V_QTD_DIAS_REINC = NULL;
        END
    END
END
").ToString();
        }

    }
}
