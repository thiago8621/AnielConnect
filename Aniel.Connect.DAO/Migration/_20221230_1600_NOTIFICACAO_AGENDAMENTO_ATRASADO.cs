using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO
{
    public class _20221230_1600_NOTIFICACAO_AGENDAMENTO_ATRASADO : AnielMigration
    {
        public _20221230_1600_NOTIFICACAO_AGENDAMENTO_ATRASADO()
        {
            Codigo = "20221230_1600";
            Descricao = "Registros de Notificação de atraso de agendamento";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder(@"    EXECUTE BLOCK as declare x int = 0;
                BEGIN
                
                UPDATE OR INSERT INTO TB_NOTIFICACOES (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VARIAVEIS_DISPONIVEIS, STATUS, MOMENTO_EXECUCAO, ACAO_EXECUTADA, SECAO_CONNECT, FREQUENCIA_NOTIFICAO_MINUTOS,NOTIFICACAO)
                               VALUES (1, 'ADMIN', NULL, NULL, NULL, NULL, 0, 1, 0, 0, 0, 15,'OS: @NUMERO_OS está atrasada. Técnico @TECNICO ainda não iniciou atendimento no horário previsto (@DATA_AGENDAMENTO)')
                             MATCHING (ID);

                UPDATE OR INSERT INTO TB_NOTIFICACOES_DESTINATARIOS (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, TIPO, ID_NOTIFICACOES)
                                             VALUES (1, NULL, NULL, NULL, NULL, NULL, 4, 1)
                                           MATCHING (ID);

                SELECT COALESCE(MAX(ID), 0) FROM     TB_NOTIFICACOES     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_NOTIFICACOES     TO ' || X; 
                SELECT COALESCE(MAX(ID), 0) FROM     TB_NOTIFICACOES_DESTINATARIOS     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_NOTIFICACOES_DESTINATARIOS     TO ' || X; 


                END;
            ").ToString();
        }

    }
}
