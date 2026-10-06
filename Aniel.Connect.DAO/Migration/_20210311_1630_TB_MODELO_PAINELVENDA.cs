using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO
{
    public class _20210311_1630_TB_MODELO_PAINELVENDA : AnielMigration
    {
        public _20210311_1630_TB_MODELO_PAINELVENDA()
        {
            Codigo = "20210311_1630";
            Descricao = "Modelos de Painel de Venda";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"EXECUTE BLOCK as declare x int = 0;
                BEGIN

                UPDATE OR INSERT INTO TB_MODELO_PAINEL_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                      VALUES (1, 'ADMIN', '2020-09-17 16:54:58', 'ADMIN', NULL, NULL, 'Vendas aprovadas hoje', 1, 1, 0, 1)
                                    MATCHING (ID);
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                      VALUES (2, 'ADMIN', '2020-09-18 10:24:11', 'ADMIN', NULL, NULL, 'Vendas canceladas hoje', 1, 1, 0, 1)
                                                    MATCHING (ID);
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                      VALUES (3, 'ADMIN', '2020-09-30 09:14:51', NULL, NULL, NULL, 'Vendas abertas hoje', 1, 1, 0, 1)
                                                    MATCHING (ID);
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                      VALUES (4, 'ADMIN', '2020-09-30 09:15:32', NULL, NULL, NULL, ' Minhas Vendas ', 1, 1, 0, 1)
                                                    MATCHING (ID);
                UPDATE OR INSERT INTO TB_MODELO_PAINEL_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, DESCRICAO, VISIBILIDADE, STATUS, COLUNA_ORDENAR, PADRAO)
                                                      VALUES (5, 'ADMIN', '2020-09-30 09:16:10', NULL, NULL, NULL, 'Vendas dos meus vendedores', 1, 1, 0, 1)
                                                    MATCHING (ID);

                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO,DATA)
                                                       VALUES (22, NULL, '2020-09-30 09:10:09', NULL, NULL, NULL, '0', 28, 1,'0')
                                                     MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO,DATA)
                                                       VALUES (23, NULL, '2020-09-30 09:10:09', NULL, NULL, NULL, '6', 18, 1,NULL)
                                                     MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO,DATA)
                                                       VALUES (24, NULL, '2020-09-30 09:11:52', NULL, NULL, NULL, '0', 28, 2,'0')
                                                     MATCHING (ID);
                UPDATE OR INSERT INTO TB_RECURSO_PAINEL_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, VALOR, FILTRO, ID_MODELO,DATA)
                                                       VALUES (25, NULL, '2020-09-30 09:11:52', NULL, NULL, NULL, '4', 18, 2,NULL)
                                                     MATCHING (ID);

                

                SELECT COALESCE(MAX(ID), 0) FROM     TB_RECURSO_PAINEL_VENDA     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_RECURSO_PAINEL_VENDA     TO ' || X; 
                SELECT COALESCE(MAX(ID), 0) FROM     TB_COLUNA_PAINEL_VENDA     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_COLUNA_PAINEL_VENDA     TO ' || X; 
                SELECT COALESCE(MAX(ID), 0) FROM     TB_MODELO_PAINEL_VENDA     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_MODELO_PAINEL_VENDA     TO ' || X; 


                END;

            ").ToString();
        }

    }
}
