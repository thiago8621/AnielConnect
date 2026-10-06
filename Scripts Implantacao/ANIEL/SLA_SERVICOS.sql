SET TERM ^ ;

CREATE OR ALTER procedure SP_CONNECT_DOCPROD_SLA_ITEM (
    DATA_MAXIMA date,
    HORA_MAXIMA time,
    COD_TIPO_SERV integer,
    ID_CATEGORIA bigint,
    ENDERECO_UF varchar(2),
    ENDERECO_CIDADE varchar(25),
    SLA_SERVICOS_PADRAO integer,
    ID_HORARIO_CONNECT_SERV integer)
returns (
    RDATA_INDICADOR date,
    RHORA_INDICADOR time)
as
declare variable VSLA_SERVICOS integer;
declare variable VMINUTOS_RESTANTES integer;
declare variable VDATA_CORRENTE date;
declare variable VHORA_CORRENTE time;
declare variable VDIA_DA_SEMANA smallint;
declare variable VDIA_DA_SEMANA_STR varchar(15);
declare variable VSQL varchar(2000);
declare variable VID_HORARIO_CONNECT bigint;
declare variable VINICIO timestamp;
declare variable VTERMINO timestamp;
declare variable VINICIO_INTERVALO timestamp;
declare variable VTERMINO_INTERVALO timestamp;
declare variable VTRABALHA_FERIADO smallint;
declare variable VINICIO_FERIADO timestamp;
declare variable VTERMINO_FERIADO timestamp;
declare variable VINICIO_INTERVALO_FERIADO timestamp;
declare variable VTERMINO_INTERVALO_FERIADO timestamp;
declare variable VE_FERIADO smallint;
declare variable VABRANGENCIA smallint;
declare variable VUF varchar(4);
declare variable VCIDADE varchar(4);
declare variable VTMP_MINUTOS integer;
declare variable VTMP_MINUTOS2 integer;
BEGIN

   IF (:DATA_MAXIMA IS NOT NULL) THEN
   BEGIN

       IF (:ID_CATEGORIA IS NOT NULL) THEN
       BEGIN

         VSQL = 'SELECT FIRST 1 B.MINUTOS, B.ID_HORARIO_CONNECT ' ||
                'FROM TB_SLA_SERVICO B ' ||
                'WHERE B.ID_TIPO_SERVICO = ' || :COD_TIPO_SERV ||
                 ' AND (B.ID_CATEGORIA_CLIENTE = ' || :ID_CATEGORIA || ' OR B.ID_CATEGORIA_CLIENTE IS NULL)' ||
                 ' AND B.STATUS = 1';

         EXECUTE STATEMENT :VSQL
         INTO :VSLA_SERVICOS, :VID_HORARIO_CONNECT;

         IF (COALESCE(:VSLA_SERVICOS,0) = 0) THEN
            VSLA_SERVICOS = :SLA_SERVICOS_PADRAO;

         VMINUTOS_RESTANTES = :VSLA_SERVICOS;

         VDATA_CORRENTE = :DATA_MAXIMA;
         VHORA_CORRENTE = :HORA_MAXIMA;

         if (COALESCE(:VID_HORARIO_CONNECT,0) = 0) then
         BEGIN
           VID_HORARIO_CONNECT = :ID_HORARIO_CONNECT_SERV;
         END

         if (COALESCE(:VID_HORARIO_CONNECT,0) = 0) then
         begin
           RDATA_INDICADOR = DATA_MAXIMA + 1;
           RHORA_INDICADOR = HORA_MAXIMA;
           SUSPEND;
         end
         else
         begin
             WHILE (:VMINUTOS_RESTANTES <> 0) DO
             BEGIN
    
                 SELECT EXTRACT(WEEKDAY FROM :VDATA_CORRENTE) FROM RDB$DATABASE
                 INTO :VDIA_DA_SEMANA;
        
                 VDIA_DA_SEMANA_STR = CASE WHEN VDIA_DA_SEMANA = 0 THEN 'DOMINGO'
                                           WHEN VDIA_DA_SEMANA = 1 THEN 'SEGUNDA'
                                           WHEN VDIA_DA_SEMANA = 2 THEN 'TERCA'
                                           WHEN VDIA_DA_SEMANA = 3 THEN 'QUARTA'
                                           WHEN VDIA_DA_SEMANA = 4 THEN 'QUINTA'
                                           WHEN VDIA_DA_SEMANA = 5 THEN 'SEXTA'
                                           WHEN VDIA_DA_SEMANA = 6 THEN 'SABADO'
                                      END;
    
                 VSQL = 'SELECT FIRST 1 D.INICIO_' || :VDIA_DA_SEMANA_STR || ',' ||
                              ' D.TERMINO_' || :VDIA_DA_SEMANA_STR || ',' ||
                              ' D.INICIO_INTERVALO_' || :VDIA_DA_SEMANA_STR || ',' ||
                              ' D.TERMINO_INTERVALO_' || :VDIA_DA_SEMANA_STR || ',' ||
                              ' D.INICIO_FERIADO,' ||
                              ' D.TERMINO_FERIADO,' ||
                              ' D.INICIO_INTERVALO_FERIADO,' ||
                              ' D.TERMINO_INTERVALO_FERIADO, ' ||
                              ' D.TABALHA_FERIADO ' ||
                        'FROM TB_HORARIO_CONNECT D ' ||
                        'WHERE D.ID = ' || :VID_HORARIO_CONNECT;
        
    
                EXECUTE STATEMENT :VSQL
                INTO :VINICIO, :VTERMINO, :VINICIO_INTERVALO, :VTERMINO_INTERVALO,
                     :VINICIO_FERIADO, :VTERMINO_FERIADO, : VINICIO_INTERVALO_FERIADO, :VTERMINO_INTERVALO_FERIADO,
                     :VTRABALHA_FERIADO;
    
                /* SE TRABALHA FERIADO */
                /*
                IF (:VTRABALHA_FERIADO = 1) THEN
                BEGIN
                   VE_FERIADO = 0;
        
                   FOR
                      SELECT E.ABRANGENCIA, E.UF, E.CIDADE
                        FROM TB_FERIADO E
                       WHERE E.DATA = :VDATA_CORRENTE
                        INTO :VABRANGENCIA, :VUF, :VCIDADE
                   DO
                     BEGIN
        
                       IF (:VABRANGENCIA IN (0,1)) THEN
                       BEGIN
                          VE_FERIADO = 1;
                          BREAK;
                       END
                       ELSE IF (:VABRANGENCIA = 3 AND :VUF = :ENDERECO_UF AND :VCIDADE = :ENDERECO_CIDADE) THEN
                       BEGIN
                          VE_FERIADO = 1;
                          BREAK;
                       END
                       ELSE IF (:VABRANGENCIA = 2 AND :VUF = :ENDERECO_UF) THEN
                       BEGIN
                          VE_FERIADO = 1;
                          BREAK;
                       END
                     END
        
                   IF (:VE_FERIADO = 1 AND :VINICIO IS NOT NULL) THEN
                   BEGIN
                       VINICIO = :VINICIO_FERIADO;
                       VTERMINO = :VTERMINO_FERIADO;
                       VINICIO_INTERVALO = :VINICIO_INTERVALO_FERIADO;
                       VTERMINO_INTERVALO = :VTERMINO_INTERVALO_FERIADO;
                   END
                 END
                 */
    
                 /* PASSA PARA PROXIMA DATA */
                 if ((:VINICIO IS NULL) OR (:VTERMINO IS NULL)) then
                 BEGIN
                   VDATA_CORRENTE = VDATA_CORRENTE + 1;
                 END
                 ELSE
                 BEGIN
    
                   if (HORA_MAXIMA IS NULL) then
                       VHORA_CORRENTE = VINICIO;
                   ELSE
                   BEGIN
                       VHORA_CORRENTE = HORA_MAXIMA;
                       HORA_MAXIMA = NULL;
                   END
    
                   VTMP_MINUTOS = (CAST(:VTERMINO AS TIME) - CAST(:VINICIO AS TIME)) / 60; /* DIFERENCA EM MINUTOS DO PERIODO CHEIO */
                   VTMP_MINUTOS2 = (CAST(:VTERMINO AS TIME) - CAST(:VHORA_CORRENTE AS TIME)) / 60; /* DIFERENCA ENTRE A HORA CORRENTE DO PERIODO QUEBRADO */
    
                   if (:VTMP_MINUTOS2 < :VMINUTOS_RESTANTES) then
                   BEGIN
                     if ((:VMINUTOS_RESTANTES - :VTMP_MINUTOS2) < 0) then
                     BEGIN
                        VMINUTOS_RESTANTES = 0;
                        SUSPEND;
                     END
                     ELSE
                        VMINUTOS_RESTANTES = :VMINUTOS_RESTANTES - :VTMP_MINUTOS2;
                     VDATA_CORRENTE = VDATA_CORRENTE + 1;
                   END
                   ELSE if (VTMP_MINUTOS <= VTMP_MINUTOS2) then
                   BEGIN
                     if ((:VMINUTOS_RESTANTES - :VTMP_MINUTOS2) <= 0) then
                     BEGIN
                       RDATA_INDICADOR = :VDATA_CORRENTE;
                       RHORA_INDICADOR = :VHORA_CORRENTE + (:VMINUTOS_RESTANTES * 60);
                       VMINUTOS_RESTANTES = 0;
                       SUSPEND;
                     END
                     ELSE
                     BEGIN
                       VMINUTOS_RESTANTES = :VMINUTOS_RESTANTES - (:VTMP_MINUTOS * 60);
                     END
    
                     VDATA_CORRENTE = VDATA_CORRENTE + 1;
                   END
                   ELSE
                   BEGIN
                     RDATA_INDICADOR = :VDATA_CORRENTE;
                     RHORA_INDICADOR = :VHORA_CORRENTE + (:VMINUTOS_RESTANTES * 60);
                     VMINUTOS_RESTANTES = 0;
                     SUSPEND;
                   END
                 END
            END
        END



       END
   END
END^

SET TERM ; ^

SET TERM ^ ;

CREATE OR ALTER procedure SP_CONNECT_DOCPROD_SLA (
    DATA_MAXIMA date,
    HORA_MAXIMA time,
    ID_CLIENTE bigint,
    COD_TIPO_SERV integer,
    ENDERECO_UF varchar(2),
    ENDERECO_CIDADE varchar(35))
returns (
    RDATA_INDICADOR date,
    RHORA_INDICADOR time)
as
declare variable VSLA_SERVICOS_PADRAO integer;
declare variable VSLA_SERVICOS integer;
declare variable VUSA_ROTEIRIZACAO varchar(1);
declare variable VID_CATEGORIA integer;
declare variable VSQL varchar(5000);
declare variable VTIPO_SLA smallint;
declare variable VDATA_HORA_INDICADOR timestamp;
declare variable VID_HORARIO_CONNECT_SERV bigint;
BEGIN

   IF (:DATA_MAXIMA IS NOT NULL) THEN
   BEGIN
       SELECT FIRST 1 A.SLA_SERVICOS_PADRAO, A.USA_ROTEIRIZACAO, A.ID_HORARIO_CONNECT_SERV
       FROM TB_MASTER A
       INTO :VSLA_SERVICOS_PADRAO, :VUSA_ROTEIRIZACAO, :VID_HORARIO_CONNECT_SERV;

       VUSA_ROTEIRIZACAO = 'S';

       IF (:VUSA_ROTEIRIZACAO = 'S' AND (COALESCE(:ID_CLIENTE,0) <> 0) AND (COALESCE(:COD_TIPO_SERV,0) <> 0)) THEN
       BEGIN
         SELECT C.ID_CATEGORIA FROM TB_CLIENTE C
         WHERE C.COD_CLIENTE = :ID_CLIENTE
         INTO :VID_CATEGORIA;
    
         VSQL = 'SELECT FIRST 1 B.MINUTOS, ' ||
                              ' B.TIPO_SLA ' ||
                'FROM TB_SLA_SERVICO B ' ||
                'WHERE B.ID_TIPO_SERVICO = ' || :COD_TIPO_SERV ||
                 ' AND B.ID_CATEGORIA_CLIENTE = ' || :VID_CATEGORIA ||
                 ' AND B.STATUS = 1';


         EXECUTE STATEMENT :VSQL
         INTO :VSLA_SERVICOS, :VTIPO_SLA;

         IF (COALESCE(:VSLA_SERVICOS,0) = 0) THEN
            VSLA_SERVICOS = VSLA_SERVICOS_PADRAO;

         IF (COALESCE(:VTIPO_SLA,0) = 0) THEN /* corridos */
         BEGIN
           SELECT DATEADD(:VSLA_SERVICOS MINUTE TO (:DATA_MAXIMA + :HORA_MAXIMA))
           FROM RDB$DATABASE
           INTO :VDATA_HORA_INDICADOR;

           RDATA_INDICADOR = CAST(:VDATA_HORA_INDICADOR AS DATE);
           RHORA_INDICADOR = CAST(:VDATA_HORA_INDICADOR AS TIME);
         END
         ELSE IF (COALESCE(:VTIPO_SLA,0) = 1) THEN /* uteis */
         BEGIN

           SELECT A.RDATA_INDICADOR, A.RHORA_INDICADOR
           FROM SP_CONNECT_DOCPROD_SLA_ITEM(:DATA_MAXIMA, :HORA_MAXIMA, :COD_TIPO_SERV, :VID_CATEGORIA, :ENDERECO_UF,
                                            :ENDERECO_CIDADE, :VSLA_SERVICOS_PADRAO, :VID_HORARIO_CONNECT_SERV) A
           INTO :RDATA_INDICADOR, :RHORA_INDICADOR;

         END
         SUSPEND;
       END
   END
END^

SET TERM ; ^

SET TERM ^ ;

CREATE OR ALTER trigger tg_connect_docprod_sla for tb_documento_producao
active before insert or update position 0
AS
  declare variable VDATA_INDICADOR DATE;
  declare variable VHORA_INDICADOR TIME;
BEGIN

   if (INSERTING) then
   BEGIN
       SELECT A.RDATA_INDICADOR, A.RHORA_INDICADOR
       FROM SP_CONNECT_DOCPROD_SLA(NEW.DATA_MAXIMA, NEW.HORA_MAXIMA, NEW.ID_CLIENTE,
                                   NEW.COD_TIPO_SERV, NEW.ENDERECO_UF, NEW.ENDERECO_CIDADE) A
       INTO :VDATA_INDICADOR, :VHORA_INDICADOR;
    
       NEW.DATA_INDICADOR = :VDATA_INDICADOR;
       NEW.HORA_INDICADOR = :VHORA_INDICADOR;
   END
   ELSE if (UPDATING AND (NEW.DATA_MAXIMA <> OLD.DATA_MAXIMA)) then
   BEGIN
     SELECT A.RDATA_INDICADOR, A.RHORA_INDICADOR
     FROM SP_CONNECT_DOCPROD_SLA(NEW.DATA_MAXIMA, NEW.HORA_MAXIMA, NEW.ID_CLIENTE,
                                 NEW.COD_TIPO_SERV, NEW.ENDERECO_UF, NEW.ENDERECO_CIDADE) A
     INTO :VDATA_INDICADOR, :VHORA_INDICADOR;
    
     NEW.DATA_INDICADOR = :VDATA_INDICADOR;
     NEW.HORA_INDICADOR = :VHORA_INDICADOR;
   END

   when any do
   begin
     exit;
   end

END^

SET TERM ; ^



