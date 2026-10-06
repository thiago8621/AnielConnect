using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO
{
    public class _20220805_1305_TB_MODELO_PAINELSERVICO_AGENDAMENTO : AnielMigration
    {
        public _20220805_1305_TB_MODELO_PAINELSERVICO_AGENDAMENTO()
        {
            Codigo = "20220805_1305";
            Descricao = "Modelos de Painel de Servico (Agendament/Reagendamento)";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"    EXECUTE BLOCK as declare x int = 0;


                BEGIN
                
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                        VALUES (100, 'ADMIN', '2022-08-04 19:14:00', NULL, NULL, NULL, 'Serviços para Primeiro Agendamento', 1, 1, 0, 1)
                                      MATCHING (ID);

                UPDATE OR INSERT INTO TB_MODELO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                        VALUES (101, 'ADMIN', '2022-08-04 19:14:00', NULL, NULL, NULL, 'Serviços para Reagendamento', 1, 1, 0, 1)
                                      MATCHING (ID);


                

                SELECT COALESCE(MAX(ID), 0) FROM     TB_RECURSO_PAINEL_SERVICO     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_RECURSO_PAINEL_SERVICO     TO ' || X; 
                SELECT COALESCE(MAX(ID), 0) FROM     TB_COLUNA_PAINEL_SERVICO     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_COLUNA_PAINEL_SERVICO     TO ' || X; 
                SELECT COALESCE(MAX(ID), 0) FROM     TB_MODELO_PAINEL_SERVICO     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_MODELO_PAINEL_SERVICO     TO ' || X; 

                END;
            ").ToString();
        }

    }
}
