using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20231729_1729_SP_CONNECT_DOCPROD_SLA : AnielMigration
    {
        public _20231729_1729_SP_CONNECT_DOCPROD_SLA()
        {
            Codigo = "_20231729_1729";
            Descricao = "Proc para atualizar SLA de Execução do Aniel";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"create or alter procedure SP_CONNECT_DOCPROD_SLA (
    DATA_CADASTRO date,
    HORA_CADASTRO time,
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
BEGIN

   IF (:DATA_CADASTRO IS NOT NULL) THEN
   BEGIN
       SELECT FIRST 1 A.SLA_SERVICOS_PADRAO, A.USA_ROTEIRIZACAO
       FROM TB_MASTER A
       INTO :VSLA_SERVICOS_PADRAO, :VUSA_ROTEIRIZACAO;

       VUSA_ROTEIRIZACAO = 'S';

       IF (:VUSA_ROTEIRIZACAO = 'S' AND (COALESCE(:COD_TIPO_SERV,0) <> 0)) THEN
       BEGIN
         SELECT C.ID_CATEGORIA FROM TB_CLIENTE C
         WHERE C.COD_CLIENTE = :ID_CLIENTE
         INTO :VID_CATEGORIA;

         IF(:VID_CATEGORIA IS NULL) THEN
         BEGIN
            :VID_CATEGORIA = 0;
         END
    
         VSQL = 'SELECT FIRST 1 B.MINUTOS, ' ||
                              ' B.TIPO_SLA ' ||
                'FROM TB_SLA_SERVICO B ' ||
                'WHERE B.ID_TIPO_SERVICO = ' || :COD_TIPO_SERV ||
                 ' AND (B.ID_CATEGORIA_CLIENTE = ' || :VID_CATEGORIA || ' OR B.ID_CATEGORIA_CLIENTE IS NULL) ' ||
                 ' AND B.STATUS = 1' ||
                 ' ORDER BY CASE WHEN B.ID_CATEGORIA_CLIENTE IS NULL THEN 1 ELSE 0 END, B.ID_CATEGORIA_CLIENTE';



         EXECUTE STATEMENT :VSQL
         INTO :VSLA_SERVICOS, :VTIPO_SLA;

         IF (COALESCE(:VSLA_SERVICOS,0) = 0) THEN
            VSLA_SERVICOS = VSLA_SERVICOS_PADRAO;

         IF (COALESCE(:VTIPO_SLA,0) = 0) THEN /* corridos */
         BEGIN
           SELECT DATEADD(:VSLA_SERVICOS MINUTE TO (:DATA_CADASTRO + :HORA_CADASTRO))
           FROM RDB$DATABASE
           INTO :VDATA_HORA_INDICADOR;

           RDATA_INDICADOR = CAST(:VDATA_HORA_INDICADOR AS DATE);
           RHORA_INDICADOR = CAST(:VDATA_HORA_INDICADOR AS TIME);
         END
         ELSE IF (COALESCE(:VTIPO_SLA,0) = 1) THEN /* uteis */
         BEGIN

           SELECT A.RDATA_INDICADOR, A.RHORA_INDICADOR
           FROM SP_CONNECT_DOCPROD_SLA_ITEM(:DATA_CADASTRO, :HORA_CADASTRO, :COD_TIPO_SERV, :VID_CATEGORIA, :ENDERECO_UF,
                                            :ENDERECO_CIDADE, :VSLA_SERVICOS_PADRAO) A
           INTO :RDATA_INDICADOR, :RHORA_INDICADOR;

         END
         SUSPEND;
       END
   END
END").ToString();
        }

    }
}
