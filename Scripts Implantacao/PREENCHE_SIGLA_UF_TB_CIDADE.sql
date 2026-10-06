SET TERM ^;

EXECUTE BLOCK AS
DECLARE VARIABLE V_ID_CIDADE BIGINT;
DECLARE VARIABLE V_ID_UF BIGINT;
DECLARE VARIABLE V_SIGLA_UF VARCHAR(2);
BEGIN
    FOR
        SELECT C.ID, C.ID_UF
        FROM TB_CIDADE C
        INTO :V_ID_CIDADE, :V_ID_UF
    DO
    BEGIN
        :V_SIGLA_UF = NULL;

        IF (:V_ID_UF IS NOT NULL) THEN
        BEGIN
            SELECT U.SIGLA
            FROM TB_UF U
            WHERE U.ID = :V_ID_UF
            INTO :V_SIGLA_UF;

            IF (:V_SIGLA_UF IS NOT NULL) THEN
            BEGIN
                UPDATE TB_CIDADE C
                SET C.UF = :V_SIGLA_UF
                WHERE C.ID = :V_ID_CIDADE;
            END
        END
    END
END^

SET TERM ;^