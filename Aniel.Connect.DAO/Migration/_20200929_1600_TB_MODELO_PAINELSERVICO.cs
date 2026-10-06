using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO
{
    public class _20200929_1600_TB_MODELO_PAINELSERVICO : AnielMigration
    {
        public _20200929_1600_TB_MODELO_PAINELSERVICO()
        {
            Codigo = "20200929_1600";
            Descricao = "Modelos de Painel de Servico";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"    EXECUTE BLOCK as declare x int = 0;


                BEGIN
                
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                        VALUES (4, 'ADMIN', '2020-09-29 16:55:18', NULL, NULL, NULL, 'Serviços sem Agendamento', 1, 1, 0, 1)
                                      MATCHING (ID);

                UPDATE OR INSERT INTO TB_MODELO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                        VALUES (5, 'ADMIN', '2020-09-29 16:58:11', NULL, NULL, NULL, 'Serviços sem Técnico', 1, 1, 0, 1)
                                                      MATCHING (ID);
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                        VALUES (6, 'ADMIN', '2020-09-29 16:59:00', NULL, NULL, NULL, 'Serviços Agendados Para Hoje', 1, 1, 0, 1)
                                                      MATCHING (ID);
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                        VALUES (7, 'ADMIN', '2020-09-29 17:00:06', NULL, NULL, NULL, 'Serviços Roteirizados para Hoje', 1, 1, 0, 1)
                                                      MATCHING (ID);
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                        VALUES (8, 'ADMIN', '2020-09-29 17:00:59', 'ADMIN', NULL, NULL, 'Serviços em Aberto dos Meus Técnicos', 1, 1, 0, 1)
                                                      MATCHING (ID);
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                        VALUES (9, 'ADMIN', '2020-09-29 17:02:08', 'ADMIN', NULL, NULL, 'Serviços Próximo a Vencer', 1, 1, 0, 1)
                                                      MATCHING (ID);
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                        VALUES (10, 'ADMIN', '2020-09-29 17:03:07', 'ADMIN', NULL, NULL, 'Serviços Vencidos', 1, 1, 0, 1)
                                                      MATCHING (ID);


                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (43, NULL, '2020-09-29 16:59:00', NULL, NULL, NULL, '0', 22, 6, NULL, NULL, 0)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (44, NULL, '2020-09-29 17:00:06', NULL, NULL, NULL, '0', 22, 7, NULL, NULL, 0)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (45, NULL, '2020-09-29 17:07:46', NULL, NULL, NULL, '0', 2, 10, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (46, NULL, '2020-09-29 17:07:46', NULL, NULL, NULL, '3', 2, 10, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (47, NULL, '2020-09-29 17:07:46', NULL, NULL, NULL, '4', 2, 10, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (48, NULL, '2020-09-29 17:07:46', NULL, NULL, NULL, '6', 2, 10, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (49, NULL, '2020-09-29 17:07:46', NULL, NULL, NULL, '10', 2, 10, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (50, NULL, '2020-09-29 17:07:46', NULL, NULL, NULL, '11', 2, 10, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (51, NULL, '2020-09-29 17:07:46', NULL, NULL, NULL, '12', 2, 10, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (52, NULL, '2020-09-29 17:07:46', NULL, NULL, NULL, '13', 2, 10, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (53, NULL, '2020-09-29 17:08:40', NULL, NULL, NULL, '0', 2, 9, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (54, NULL, '2020-09-29 17:08:40', NULL, NULL, NULL, '3', 2, 9, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (55, NULL, '2020-09-29 17:08:40', NULL, NULL, NULL, '4', 2, 9, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (56, NULL, '2020-09-29 17:08:40', NULL, NULL, NULL, '6', 2, 9, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (57, NULL, '2020-09-29 17:08:40', NULL, NULL, NULL, '10', 2, 9, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (58, NULL, '2020-09-29 17:08:40', NULL, NULL, NULL, '11', 2, 9, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (59, NULL, '2020-09-29 17:08:40', NULL, NULL, NULL, '12', 2, 9, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (60, NULL, '2020-09-29 17:08:40', NULL, NULL, NULL, '13', 2, 9, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (61, NULL, '2020-09-29 17:36:30', NULL, NULL, NULL, '4', 26, 10, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (62, NULL, '2020-09-29 17:52:13', NULL, NULL, NULL, '2', 26, 9, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (63, NULL, '2020-09-29 17:59:40', NULL, NULL, NULL, '0', 2, 8, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (64, NULL, '2020-09-29 17:59:40', NULL, NULL, NULL, '3', 2, 8, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (65, NULL, '2020-09-29 17:59:40', NULL, NULL, NULL, '4', 2, 8, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (66, NULL, '2020-09-29 17:59:40', NULL, NULL, NULL, '6', 2, 8, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (67, NULL, '2020-09-29 17:59:40', NULL, NULL, NULL, '10', 2, 8, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (68, NULL, '2020-09-29 17:59:40', NULL, NULL, NULL, '11', 2, 8, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (69, NULL, '2020-09-29 17:59:40', NULL, NULL, NULL, '12', 2, 8, NULL, NULL, NULL)
                                                       MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_SERVICO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO, VALOR_PAI, VALOR_AVO, DATA)
                                                         VALUES (70, NULL, '2020-09-29 17:59:40', NULL, NULL, NULL, '13', 2, 8, NULL, NULL, NULL)
                                                       MATCHING (ID);

                SELECT COALESCE(MAX(ID), 0) FROM     TB_RECURSO_PAINEL_SERVICO     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_RECURSO_PAINEL_SERVICO     TO ' || X; 
                SELECT COALESCE(MAX(ID), 0) FROM     TB_COLUNA_PAINEL_SERVICO     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_COLUNA_PAINEL_SERVICO     TO ' || X; 
                SELECT COALESCE(MAX(ID), 0) FROM     TB_MODELO_PAINEL_SERVICO     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_MODELO_PAINEL_SERVICO     TO ' || X; 

                END;
            ").ToString();
        }

    }
}
