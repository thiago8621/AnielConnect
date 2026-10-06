using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20201017_1532_TG_QUIZ_FOTO_IUD : AnielMigration
    {
        public _20201017_1532_TG_QUIZ_FOTO_IUD()
        {
            Codigo = "20201017_1532";
            Descricao = "Trigger";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER trigger tg_quiz_foto_iud for tb_quiz_foto
                                                    active before insert or update or delete position 0
                                                    AS
                                                    declare variable VUSUARIO VARCHAR(10);
                                                    declare variable VALTERACAO VARCHAR(15);
                                                    declare variable ARQUIVO BLOB SUB_TYPE 0 SEGMENT SIZE 8192;
                                                    BEGIN
                                                        if (INSERTING or UPDATING) then
                                                        BEGIN
                                                            if (INSERTING) then
                                                            begin
                                                                VALTERACAO = 'INCLUIU FOTO';
                                                                ARQUIVO = null;
                                                            end
                                                            ELSE
                                                            begin
                                                                VALTERACAO = 'ALTEROU FOTO';
                                                                ARQUIVO = old.arquivo;
                                                            end

                                                            VUSUARIO = RDB$GET_CONTEXT('USER_SESSION', 'USUARIO');
                                                            IF (VUSUARIO = '' OR VUSUARIO IS NULL) THEN
                                                                VUSUARIO = NEW.USUARIO;

                                                            insert into TB_QUIZ_FOTO_LOG (ID_QUIZ_FOTO, USUARIO, DIA, HORA, ID_ANSWER_CAPA, ID_QUIZ_ANSWER_ITEM, ID_QUESTION,
                                                                                            ID_QUESTION_ANSWER, ALTERACAO, ARQUIVO)
                                                            values (NEW.ID, :VUSUARIO, CURRENT_DATE, CURRENT_TIME, NEW.ID_ANSWER_CAPA, NEW.ID_QUIZ_ANSWER_ITEM, NEW.ID_QUESTION,
                                                                    NEW.ID_QUESTION_ANSWER, :VALTERACAO, :ARQUIVO);
                                                        END
                                                        ELSE if (deleting) then
                                                        BEGIN
                                                            VALTERACAO = 'EXCLUIU FOTO';
                                                            VUSUARIO = RDB$GET_CONTEXT('USER_SESSION', 'USUARIO');
                                                            IF (VUSUARIO = '' OR VUSUARIO IS NULL) THEN
                                                                VUSUARIO = OLD.USUARIO;

                                                            insert into TB_QUIZ_FOTO_LOG (ID_QUIZ_FOTO, USUARIO, DIA, HORA, ID_ANSWER_CAPA, ID_QUIZ_ANSWER_ITEM, ID_QUESTION,
                                                                                            ID_QUESTION_ANSWER, ALTERACAO, ARQUIVO)
                                                            values (OLD.ID, :VUSUARIO, CURRENT_DATE, CURRENT_TIME, OLD.ID_ANSWER_CAPA, OLD.ID_QUIZ_ANSWER_ITEM, OLD.ID_QUESTION,
                                                                    OLD.ID_QUESTION_ANSWER, :VALTERACAO, OLD.ARQUIVO);
                                                        END

                                                    END").ToString();
        }
    }
}
