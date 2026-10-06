using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20230926_1557_TG_CONNECT_DOCPROD_SLA : AnielMigration
    {
        public _20230926_1557_TG_CONNECT_DOCPROD_SLA()
        {
            Codigo = "20230926_1557";
            Descricao = "Trigger para atualizar dados do connect no documento produção";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_connect_docprod_sla for tb_documento_producao
active before insert position 0
AS
                  DECLARE VARIABLE VDATA_INDICADOR DATE;
                  DECLARE VARIABLE VHORA_INDICADOR TIME;
                  DECLARE VARIABLE VSLA_DATA_ABERTURA INTEGER;

                  DECLARE VARIABLE VDIA DATE;
                  DECLARE VARIABLE VHORA TIME;

                BEGIN

                   SELECT FIRST 1 SLA_DATA_ABERTURA FROM TB_MASTER INTO :VSLA_DATA_ABERTURA;
                   IF(:VSLA_DATA_ABERTURA = 0)THEN
                   BEGIN



                        SELECT FIRST 1 A.DIA, A.HORA
                        FROM TB_DOCUMENTO_PRODUCAO A 
                        WHERE A.CODCT = NEW.CODCT AND A.PROJETO = NEW.PROJETO AND 
                              A.NUM_DOC = NEW.NUM_DOC AND A.NUM_OBRA_ORIGINAL = NEW.NUM_OBRA_ORIGINAL
                        ORDER BY A.SEQ
                        INTO :VDIA, :VHORA;


                        if(:VDIA IS NULL or :VHORA IS NULL)THEN
                        BEGIN
                           :VDIA = NEW.DIA;
                           :VHORA = NEW.HORA;
                        END

                        SELECT A.RDATA_INDICADOR, A.RHORA_INDICADOR
                        FROM SP_CONNECT_DOCPROD_SLA(:VDIA, :VHORA, NEW.ID_CLIENTE,
                                               NEW.COD_TIPO_SERV, NEW.ENDERECO_UF, NEW.ENDERECO_CIDADE) A
                        INTO :VDATA_INDICADOR, :VHORA_INDICADOR;
                   END
                   ELSE if(:VSLA_DATA_ABERTURA = 1)THEN
                   BEGIN
                        SELECT A.RDATA_INDICADOR, A.RHORA_INDICADOR
                        FROM SP_CONNECT_DOCPROD_SLA(NEW.DATA_MAXIMA, COALESCE(NEW.HORA_MAXIMA,'00:00'), NEW.ID_CLIENTE,
                                               NEW.COD_TIPO_SERV, NEW.ENDERECO_UF, NEW.ENDERECO_CIDADE) A
                        INTO :VDATA_INDICADOR, :VHORA_INDICADOR;
                   END

                  
                   IF(NEW.ALTERAR_INDICADOR = 0)THEN  -- Permite que o usuario altere a hora indicador
                   BEGIN
                        NEW.DATA_INDICADOR = :VDATA_INDICADOR;
                        NEW.HORA_INDICADOR = :VHORA_INDICADOR;
                   END
                   ELSE
                        NEW.ALTERAR_INDICADOR = 0;


                   IF (INSERTING) THEN
                   BEGIN
                       IF (NEW.DATA_MAXIMA IS NULL) THEN
                          NEW.STATUS_EXECUCAO = 4;
                   END

                   IF (COALESCE(NEW.EQUIPE,'') = '') THEN
                   BEGIN
                     NEW.EQUIPE = NULL;
                   END

                   IF (NEW.STATUS_EXECUCAO IS NULL AND NEW.DATA_MAXIMA IS NULL) THEN
                   BEGIN
                     NEW.STATUS_EXECUCAO = 4;
                   END
                   ELSE IF (NEW.STATUS_EXECUCAO IS NULL AND (NOT NEW.DATA_MAXIMA IS NULL)) THEN
                   BEGIN
                     NEW.STATUS_EXECUCAO = 0;
                   END

                   IF (NEW.STATUS_EXECUCAO = 9) THEN
                   BEGIN
                     NEW.ID_AGENDAMENTO = NULL;
                     DELETE FROM TB_AGENDAMENTO_EQUIPEANIEL A
                     WHERE A.CODCT = NEW.CODCT AND A.PROJETO = NEW.PROJETO
                       AND A.NUM_OBRA = NEW.NUM_OBRA AND A.NUM_DOC = NEW.NUM_DOC;
                   END

                  IF (UPDATING) THEN
                  BEGIN
                   IF ( (NEW.ENDERECO <> OLD.ENDERECO) OR (NEW.ENDERECO_TIPO <> OLD.ENDERECO_TIPO) OR (NEW.ENDERECO_BAIRRO <> OLD.ENDERECO_BAIRRO) ) THEN
                       BEGIN
                        NEW.TEM_TESTE_ELETRICO = 'N';
                       END
                  END

                   WHEN ANY DO
                   BEGIN
                     EXIT;
                   END

                END").ToString();
        }

    }
}
