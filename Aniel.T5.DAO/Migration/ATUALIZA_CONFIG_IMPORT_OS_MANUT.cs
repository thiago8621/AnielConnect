using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{


    public class _20241128_1650_ATUALIZA_CONFIG_IMPORT_OS_MANUT : AnielMigration
    {
        public _20241128_1650_ATUALIZA_CONFIG_IMPORT_OS_MANUT()
        {
            Codigo = "20241128_1650";
            Descricao = "Configurações de Importação de OS de Manutenção";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (24, 'ALTERAR_FAC', 'Portabilidade', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (14, 'COD_AREA', 'Área de Despacho', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (64, 'COD_ESCRITORIO', 'Escritório', '', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (16, 'COD_LOCALIDADE', 'Node', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (40, 'COD_SUB_TIPO_SERV', 'Sub. Tipo de Serviço', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (25, 'COD_TIPO_SERV', 'Tipo de Serviço', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (53, 'COD_TIPO_SERV_PRINC', 'Cod. Tipo de Serviço', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (6, 'COD_TURNO_SERV', 'Período', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (19, 'COM_SEM_CAPA', 'Vírtua', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (41, 'DATA_ABERTURA', 'Data/Hora Atendimento', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (65, 'DATA_CRIACAO', 'Data Criação', '', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (35, 'DATA_INDICADOR', 'Início Estimado', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (36, 'DATA_MAXIMA', 'Data Agendamento', ' ', 1, 'S', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (10, 'ENDERECO', 'Logradouro', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (11, 'ENDERECO_BAIRRO', 'Bairro', ' ', 2, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (48, 'ENDERECO_COMPL', 'Complemento do Endereco', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (12, 'ENDERECO_CPL', 'CEP', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (47, 'ENDERECO_NUMERO', 'Numero do Imovel', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (13, 'ENDERECO_TIPO', 'Cidade', ' ', 2, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (57, 'ENDERECO_UF', 'UF Cliente', '', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (21, 'END_ARM', 'Produto Para', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (20, 'END_CAIXA', 'Produto De', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (3, 'EQUIPE', 'Equipe', ' ', 2, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (66, 'HORA_CRIACAO', 'Hora Criação', '', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (54, 'HORA_MAXIMA', 'Hora Agendamento', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (61, 'INSCRICAO_ESTADUAL', 'Inscrição Estadual', '', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (32, 'INTEGRA_EQUIPAMENTO', 'Status', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (43, 'LATITUDE_STR', 'Latitude', ' ', 2, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (42, 'LONGITUDE_STR', 'Longitude', ' ', 2, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (1, 'NUM_OBRA', 'Número da OS', ' ', 1, 'S', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (33, 'OBSERVACAO_AUX', 'Observação', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (34, 'OBSERVACAO_TESTE', 'Ocorrências do Contrato', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (18, 'OK_PONTO', 'Emergencial', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (27, 'OUTROS_GASTOS', 'Produto', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (44, 'PARPRIM_PARA', 'Tipo de Imovel', ' ', 2, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (22, 'PINO_DE', 'Impresso', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (62, 'PPPOE', 'PPPOE', '', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (7, 'SEGMENTO', 'Segmento', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (63, 'SENHA_BAIXA', 'Senha Baixa', '', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (29, 'TAREFA', 'Telefone Fixo', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (28, 'TEL_CIENTE', 'Telefone Celular', ' ', 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (17, 'TEM_TESTE_ELETRICO', 'Imediata', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (5, 'TIPO_NOTA', 'Field/Fora Field', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (23, 'TIPO_TAREFA_REMEDY', 'Classe', ' ', 3, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (67, 'ID_POP', 'POP ( Código  ID )', NULL, 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (69, 'ID_ELEMENTO_REDE', 'Elemento de Rede (Código ID)', NULL, 1, 'N', NULL)
                                        MATCHING (ID);
UPDATE OR INSERT INTO TB_CONFIG_IMPORTACAO_MANUT (ID, NOME_CAMPO_ANIEL, APELIDO_CAMPO_ANIEL, CAMPO_PLANILHA, PLANILHA, INTERNO, CONFIG_PASTA)
                                          VALUES (68, 'ID_OLT', 'OLT (Código ID)', NULL, 1, 'N', NULL)
                                        MATCHING (ID);

").ToString();


        }

    }
}

