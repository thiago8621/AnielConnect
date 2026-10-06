using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20230822_0900_STATUS_VENDA : AnielMigration
    {
        public _20230822_0900_STATUS_VENDA()
        {
            Codigo = "20230822_0900";
            Descricao = "Status Vendas - Status Fixos";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder(@" EXECUTE BLOCK as declare x int = 0;


                BEGIN
                                       UPDATE OR INSERT INTO TB_ATIVSTATUS_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, STATUS, DESCRICAO, STATUSFIXO,STATUS_PADRAO)
                                                                 VALUES (1, NULL, NULL, NULL, NULL, NULL, 1, 'Análise Cliente', 'Sim',0)
                                                                    MATCHING (ID);
                                       UPDATE OR INSERT INTO TB_ATIVSTATUS_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, STATUS, DESCRICAO, STATUSFIXO,STATUS_PADRAO)
                                                                 VALUES (2, NULL, NULL, NULL, NULL, NULL, 1, 'Aguardando Avaliacao Comercial', 'Sim',5)
                                                                    MATCHING (ID);
                                       UPDATE OR INSERT INTO TB_ATIVSTATUS_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, STATUS, DESCRICAO, STATUSFIXO,STATUS_PADRAO)
                                                                 VALUES (3, NULL, NULL, NULL, NULL, NULL, 1, 'Avaliação Aprovada', 'Sim',2)
                                                                    MATCHING (ID);
                                       UPDATE OR INSERT INTO TB_ATIVSTATUS_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, STATUS, DESCRICAO, STATUSFIXO,STATUS_PADRAO)
                                                                 VALUES (4, NULL, NULL, NULL, NULL, NULL, 1, 'Venda Aprovada', 'Sim',6)
                                                                    MATCHING (ID);
                                       UPDATE OR INSERT INTO TB_ATIVSTATUS_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, STATUS, DESCRICAO, STATUSFIXO,STATUS_PADRAO)
                                                                 VALUES (5, NULL, NULL, NULL, NULL, NULL, 1, 'Avaliação Reprovada', 'Sim',3)
                                                                    MATCHING (ID);
                                       UPDATE OR INSERT INTO TB_ATIVSTATUS_VENDA (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, STATUS, DESCRICAO, STATUSFIXO,STATUS_PADRAO)
                                                                 VALUES (6, NULL, NULL, NULL, NULL, NULL, 1, 'Venda Cancelada', 'Sim',4)
                                                                    MATCHING (ID);

SELECT COALESCE(MAX(ID), 0) FROM     TB_ATIVSTATUS_VENDA     INTO X;  EXECUTE STATEMENT 'SET GENERATOR     GN_ATIVSTATUS_VENDA     TO ' || X;
END;
").ToString();


        }
    }
}



