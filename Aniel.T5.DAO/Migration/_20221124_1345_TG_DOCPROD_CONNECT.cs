using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20221124_1345_TG_DOCPROD_CONNECT : AnielMigration
    {
        public _20221124_1345_TG_DOCPROD_CONNECT()
        {
            Codigo = "20221124_1345";
            Descricao = "Trigger para atualizar supervisor e monitor no documento produção ao criar ou atualizar uma OS ";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_docprod_connect for tb_documento_producao
active before insert or update position 0
AS
declare variable VCOD_SUPERV VARCHAR(6);
declare variable VUSUARIO_MONITOR VARCHAR(10);
declare variable VINFORMACACOES_COMPLEMENTARES blob(4096);
declare variable VOBSERVACAO_AUX VARCHAR(900);
declare variable VGEOSEGMENTO VARCHAR(18);
begin
  if (INSERTING) then
  BEGIN
    if (NEW.SEQ > 1) then
    BEGIN
      SELECT FIRST 1 A.OBSERVACAO_OITV ,A.OBSERVACAO_AUX,A.GEOSEGMENTO
      FROM TB_DOCUMENTO_PRODUCAO A
      WHERE A.CODCT = NEW.CODCT AND A.PROJETO = NEW.PROJETO AND A.NUM_DOC= NEW.NUM_DOC
        AND A.NUM_OBRA_ORIGINAL = NEW.NUM_OBRA_ORIGINAL AND A.SEQ = NEW.SEQ - 1
      INTO :VINFORMACACOES_COMPLEMENTARES,:VOBSERVACAO_AUX,:VGEOSEGMENTO;

      if (:VINFORMACACOES_COMPLEMENTARES IS NOT NULL) then
      BEGIN
        NEW.OBSERVACAO_OITV = :VINFORMACACOES_COMPLEMENTARES;
      END
      if (:VOBSERVACAO_AUX IS NOT NULL) then
      BEGIN
        NEW.OBSERVACAO_AUX = :VOBSERVACAO_AUX;
      END
      if (:VGEOSEGMENTO IS NOT NULL) then
      BEGIN
        NEW.GEOSEGMENTO = :VGEOSEGMENTO;
      END
    END
    if (NEW.SEQ > 1 AND COALESCE(NEW.EQUIPE,'') = '') then
    BEGIN
      SELECT FIRST 1 A.COD_SUPERV, A.USUARIO_MONITOR
      FROM TB_DOCUMENTO_PRODUCAO A
      WHERE A.CODCT = NEW.CODCT AND A.PROJETO = NEW.PROJETO AND A.NUM_DOC= NEW.NUM_DOC
        AND A.NUM_OBRA_ORIGINAL = NEW.NUM_OBRA_ORIGINAL AND A.SEQ = NEW.SEQ - 1
      INTO :VCOD_SUPERV, :VUSUARIO_MONITOR;
   
        NEW.COD_SUPERV = :VCOD_SUPERV;
        NEW.USUARIO_MONITOR = :VUSUARIO_MONITOR;
      
    END
  END
  ELSE
  BEGIN

    if (COALESCE(NEW.EQUIPE,'') <> '') then
    BEGIN

      SELECT A.COD_SUPERV, A.USUARIO_MONITOR
      FROM TB_EQUIPE A
      WHERE A.EQUIPE = NEW.EQUIPE
      INTO :VCOD_SUPERV, :VUSUARIO_MONITOR;
      
        NEW.COD_SUPERV = :VCOD_SUPERV;
        NEW.USUARIO_MONITOR = :VUSUARIO_MONITOR;
      

    END

  END
END").ToString();
        }

    }
}
