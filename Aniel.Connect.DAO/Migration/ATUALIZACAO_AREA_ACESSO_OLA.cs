using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{   
    public class ATUALIZACAO_AREA_ACESSO_OLA : AnielMigration
    {
        public ATUALIZACAO_AREA_ACESSO_OLA()
        {
            Codigo = "20252401_1215";
            Descricao = "Areas de Acesso Olá";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@" UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (1, CURRENT_TIMESTAMP, '01', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 1), 1), 'Menu Comercial', 'Visualização do menu de recursos da área Comercial')
                            MATCHING (ID, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (2, CURRENT_TIMESTAMP, '02', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 2), 1), 'Menu Suporte', 'Visualização do menu de recursos da área de Suporte')
                            MATCHING (ID, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (3, CURRENT_TIMESTAMP, '03', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 3), 1), 'Menu Faturas', 'Visualização do menu de recursos da área de Financeiro/Faturas')
                            MATCHING (ID, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (4, CURRENT_TIMESTAMP, '04', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 4), 1), 'Detalhes da Solicitação', 'Visualizar os detalhes da Solicitação')
                            MATCHING (ID, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (5, CURRENT_TIMESTAMP, '05', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 5), 1), 'Acompanhar Técnico', 'Visualizar a rota do Técnico no mapa durante o deslocamento para o endereço do atendimento')
                            MATCHING (ID, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (6, CURRENT_TIMESTAMP, '06', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 6), 1), 'Cancelar Visita Técnica', 'Realizar o cancelamento de uma visita técnica agendada')
                            MATCHING (ID, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (7, CURRENT_TIMESTAMP, '07', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 7), 1), 'Chat de Atendimento', 'Acesso ao Chat para abertura de Atendimentos')
                            MATCHING (ID, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (8, CURRENT_TIMESTAMP, '08', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 8), 1), 'Lista de Solicitações', 'Visualizar a listagem de Solicitações')
                            MATCHING (ID, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (9, CURRENT_TIMESTAMP, '09', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 9), 1), 'Nova Solicitação', 'Solicitar a abertura de uma Solicitação')
                            MATCHING (ID, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_ACESSO_OLA (ID, DATAHORACRIACAO, COD_AREA, OCULTAR_ACESSO, DESCRICAO, OBSERVACAO)
                            VALUES (10, CURRENT_TIMESTAMP, '10', COALESCE((SELECT OCULTAR_ACESSO FROM TB_AREA_ACESSO_OLA WHERE ID = 10), 1), 'Reagendar Solicitação', 'Realizar o reagendamento de uma Solicitação em aberto')
                            MATCHING (ID, COD_AREA);
").ToString();


        }

    }


}
