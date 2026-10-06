create or alter procedure EXTRACT_INTEGER (
    VALORSTR varchar(1000))
returns (
    RETORNO bigint)
as
declare variable I integer;
declare variable S varchar(1000);
declare variable C varchar(1);
declare variable R varchar(1000);
BEGIN 
  IF (VALORSTR is null) THEN EXIT;
  S = TRIM( VALORSTR );
  R = NULL;
  I = 1;
  WHILE (I <= CHAR_LENGTH(S)) DO
  BEGIN
    C = SUBSTRING( S FROM I FOR 1 );
    IF ((C >= '0') AND (C <= '9')) THEN LEAVE;
    I = I + 1;
  END
  /*
  WHILE (I <= CHAR_LENGTH(S)) DO
  BEGIN
    C = SUBSTRING( S FROM I FOR 1 );
    IF (C < '0') THEN LEAVE;
    IF (C > '9') THEN LEAVE;
    IF (C IS NULL) THEN LEAVE;
    IF (R IS NULL) THEN R=C; ELSE R = R || C;
    I = I + 1; 
  END*/

  WHILE (I <= CHAR_LENGTH(S)) DO
  BEGIN
    C = SUBSTRING( S FROM I FOR 1 );

    IF ((C >= '0') AND (C <= '9')) THEN
    BEGIN
      IF (R IS NULL) THEN R=C; ELSE R = R || C;
      I = I + 1; 
    END
    ELSE IF (C IS NULL) THEN
    BEGIN
      R = '0';
      I = CHAR_LENGTH(S) + 1;
      LEAVE;
    END
    ELSE
    BEGIN
      R = '0';
      I = CHAR_LENGTH(S) + 1;
      LEAVE;
    END
  END

  RETORNO = CAST(R AS BIGINT);

  SUSPEND;
END