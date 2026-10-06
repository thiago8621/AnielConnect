SET TERM ^ ;

create or alter procedure SP_CALCULA_CREDITO (
    DATA_INICIO_PERIODO date,
    DATA_FIM_PERIODO date,
    DEBITO double precision)
returns (
    CREDITO_REAL double precision)
as
declare variable PERCENTUAL_NOTURNAS double precision = 0;
declare variable HORAS_NOTURNAS double precision = 0;
declare variable HORAS_FINAIS double precision = 0;
declare variable HORAS double precision = 0;
declare variable PERCENTUAL double precision = 0;
declare variable ID_HORAS bigint;
declare variable BASE_CONVERSAO integer;
declare variable ID_DIA_FUNC bigint;
declare variable HORA_NOTURNA_ATUAL double precision = 0;
declare variable ACUM_CREDITO double precision = 0;
BEGIN
    CREDITO_REAL = 0;
    FOR
        SELECT A.BASE_CONVERSAO_HORAS, C.PERCENTUAL, C.HORAS, C.HORAS_FINAIS,
               C.ID, B.ID
        FROM TB_PT_JORNADA_FUNCIONARIO A
        INNER JOIN TB_PT_DIA_FUNCIONARIO B
        ON (B.ID_JORNADA = A.ID)
        INNER JOIN TB_PT_BD_HORAS C
        ON (C.ID_DIA_FUNCIONARIO = B.ID AND C.TIPO_MOVTO = 'B'
            AND C.TRANSACAO = 'C')
        WHERE B.DATA BETWEEN :DATA_INICIO_PERIODO AND :DATA_FIM_PERIODO
        ORDER BY C.ID
        INTO :BASE_CONVERSAO, :PERCENTUAL, :HORAS, :HORAS_FINAIS,
             :ID_HORAS, :ID_DIA_FUNC
    DO
    BEGIN
        IF (BASE_CONVERSAO = 0) THEN
            ACUM_CREDITO = ACUM_CREDITO + HORAS;
        ELSE IF (BASE_CONVERSAO = 1) THEN
            ACUM_CREDITO = ACUM_CREDITO + HORAS_FINAIS;

        IF (ACUM_CREDITO > DEBITO) THEN
        BEGIN
            SELECT FIRST 1 COALESCE(SUM(A.HORAS), 0), COALESCE(A.PERCENTUAL, 0)
            FROM TB_PT_BD_HORAS A
            WHERE A.ID_DIA_FUNCIONARIO = :ID_DIA_FUNC
              AND A.TIPO_HORA = 'N'
              AND A.HE_VINCULADA = :ID_HORAS
            GROUP BY PERCENTUAL
            INTO :HORAS_NOTURNAS, :PERCENTUAL_NOTURNAS;

            IF (BASE_CONVERSAO = 0) THEN
            BEGIN
                IF ((ACUM_CREDITO - DEBITO) < HORAS) THEN
                BEGIN
                    IF (PERCENTUAL > 0) THEN
                        CREDITO_REAL = CREDITO_REAL + ((HORAS - (ACUM_CREDITO - DEBITO)) * (PERCENTUAL / 100));
                    ELSE
                        CREDITO_REAL = CREDITO_REAL + (HORAS_FINAIS - (ACUM_CREDITO - DEBITO));

                    IF ((HORAS_NOTURNAS > 0) AND ((HORAS - ACUM_CREDITO - DEBITO) >= HORAS_NOTURNAS)) THEN
                        HORA_NOTURNA_ATUAL = HORAS_NOTURNAS;
                    ELSE IF ((HORAS_NOTURNAS > 0) AND ((HORAS - ACUM_CREDITO - DEBITO) < HORAS_NOTURNAS)) THEN
                        HORA_NOTURNA_ATUAL = HORAS - ACUM_CREDITO - DEBITO;

                    IF (HORA_NOTURNA_ATUAL > 0 AND PERCENTUAL_NOTURNAS > 0) THEN
                    BEGIN
                        IF (PERCENTUAL > 0) THEN
                            CREDITO_REAL = CREDITO_REAL + (((HORA_NOTURNA_ATUAL * (PERCENTUAL / 100)) + HORA_NOTURNA_ATUAL) * PERCENTUAL_NOTURNAS / 100);
                        ELSE
                            CREDITO_REAL = CREDITO_REAL + (HORA_NOTURNA_ATUAL * (PERCENTUAL_NOTURNAS / 100));
                    END
                END
                ELSE
                BEGIN
                    CREDITO_REAL = CREDITO_REAL + HORAS_FINAIS;

                    IF (PERCENTUAL > 0) THEN
                        HORAS_NOTURNAS = HORAS_NOTURNAS + (HORAS_NOTURNAS * (PERCENTUAL / 100));

                    IF ((HORAS_NOTURNAS > 0) AND (PERCENTUAL_NOTURNAS > 0)) THEN
                        CREDITO_REAL = CREDITO_REAL + (HORAS_NOTURNAS * (PERCENTUAL_NOTURNAS / 100));
                END
            END
            ELSE IF (BASE_CONVERSAO = 1) THEN
            BEGIN
                IF ((ACUM_CREDITO - DEBITO) < HORAS_FINAIS) THEN
                BEGIN
                    CREDITO_REAL = CREDITO_REAL + (HORAS_FINAIS - (ACUM_CREDITO - DEBITO));

                    IF (PERCENTUAL > 0) THEN
                        HORAS_NOTURNAS = HORAS_NOTURNAS * (PERCENTUAL / 100);

                    IF (HORAS_NOTURNAS > 0 AND ((HORAS_FINAIS - ACUM_CREDITO - DEBITO) >= HORAS_NOTURNAS)) THEN
                        HORA_NOTURNA_ATUAL = HORAS_NOTURNAS;
                    ELSE IF ((HORAS_NOTURNAS > 0) AND ((HORAS_FINAIS - ACUM_CREDITO - DEBITO) < HORAS_NOTURNAS)) THEN
                        HORA_NOTURNA_ATUAL = HORAS_FINAIS - ACUM_CREDITO - DEBITO;

                    IF ((HORA_NOTURNA_ATUAL > 0) AND (PERCENTUAL_NOTURNAS > 0)) THEN
                        CREDITO_REAL = CREDITO_REAL + (HORA_NOTURNA_ATUAL * (PERCENTUAL_NOTURNAS / 100));
                END
                ELSE
                BEGIN
                    CREDITO_REAL = CREDITO_REAL + HORAS_FINAIS;

                    IF (PERCENTUAL > 0) THEN
                        HORAS_NOTURNAS = HORAS_NOTURNAS + (HORAS_NOTURNAS * (PERCENTUAL / 100));

                    IF ((HORAS_NOTURNAS > 0) AND (PERCENTUAL_NOTURNAS > 0)) THEN
                        CREDITO_REAL = CREDITO_REAL + (HORAS_NOTURNAS * (PERCENTUAL_NOTURNAS / 100));
                END
            END

            HORAS_NOTURNAS = 0;
        END
    END
    SUSPEND;
END^

SET TERM ; ^

/* Following GRANT statements are generated automatically */

GRANT SELECT ON TB_PT_JORNADA_FUNCIONARIO TO PROCEDURE SP_CALCULA_CREDITO;
GRANT SELECT ON TB_PT_DIA_FUNCIONARIO TO PROCEDURE SP_CALCULA_CREDITO;
GRANT SELECT ON TB_PT_BD_HORAS TO PROCEDURE SP_CALCULA_CREDITO;

/* Existing privileges on this procedure */

GRANT EXECUTE ON PROCEDURE SP_CALCULA_CREDITO TO SYSDBA;