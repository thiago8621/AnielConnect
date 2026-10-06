using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20230309_2000_TG_CONNECT_DOCPROD_SLA_ALT : AnielMigration
    {
        public _20230309_2000_TG_CONNECT_DOCPROD_SLA_ALT()
        {
            Codigo = "20230309_2000";
            Descricao = "Trigger para atualizar SLA no documento produção";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_connect_docprod_sla_alt for tb_documento_producao
active before update position 0
AS
   DECLARE VARIABLE VDATA_INDICADOR DATE;
   DECLARE VARIABLE VHORA_INDICADOR TIME;
   DECLARE VARIABLE VSLA_DATA_ABERTURA INTEGER;
   DECLARE VARIABLE VDIA DATE;
   DECLARE VARIABLE VHORA TIME;
BEGIN

   SELECT FIRST 1 SLA_DATA_ABERTURA FROM TB_MASTER INTO :VSLA_DATA_ABERTURA;

   if ((:VSLA_DATA_ABERTURA = 1)AND((NEW.DATA_MAXIMA <> OLD.DATA_MAXIMA)OR(NEW.HORA_MAXIMA<>OLD.HORA_MAXIMA)))  THEN
   BEGIN
      SELECT A.RDATA_INDICADOR, A.RHORA_INDICADOR
      FROM SP_CONNECT_DOCPROD_SLA(NEW.DATA_MAXIMA, COALESCE(NEW.HORA_MAXIMA,'00:00'), NEW.ID_CLIENTE,
                                  NEW.COD_TIPO_SERV, NEW.ENDERECO_UF, NEW.ENDERECO_CIDADE) A
      INTO :VDATA_INDICADOR, :VHORA_INDICADOR;
      NEW.DATA_INDICADOR = :VDATA_INDICADOR;
      NEW.HORA_INDICADOR = :VHORA_INDICADOR;
   END

   WHEN ANY DO
   BEGIN
      EXIT;
   END
END").ToString();
        }

    }
}
