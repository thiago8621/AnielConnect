using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20230309_2000_TG_CONNECT_DOCPROD_STATUS : AnielMigration
    {
        public _20230309_2000_TG_CONNECT_DOCPROD_STATUS()
        {
            Codigo = "20230309_2000";
            Descricao = "Trigger para atualizar Status no documento produção";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_connect_docprod_sla_status for tb_documento_producao
active before update position 0
AS
BEGIN
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
