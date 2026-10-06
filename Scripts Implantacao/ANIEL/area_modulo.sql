UPDATE OR INSERT INTO TB_MODULO (SIGLA_MODULO, DESCRICAO, ATALHO, CAMINHO_REDE, SIGLA_AUX, RESPONSAVEL, COMPUTADOR, DATA_ARQUIVO, HORA_ARQUIVO, NOME_ARQUIVO, CONFIG_BD, BD_CONFIG, COD_PRODUTO, SERVICO, ENVIA_EXE)
                         VALUES ('CONNECT', 'MÓDULO CONNECT', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'N', 'N', NULL, 'N', 'N')
                       MATCHING (SIGLA_MODULO);
UPDATE OR INSERT INTO TB_MODULO (SIGLA_MODULO, DESCRICAO, ATALHO, CAMINHO_REDE, SIGLA_AUX, RESPONSAVEL, COMPUTADOR, DATA_ARQUIVO, HORA_ARQUIVO, NOME_ARQUIVO, CONFIG_BD, BD_CONFIG, COD_PRODUTO, SERVICO, ENVIA_EXE)
                         VALUES ('EVA', 'MODULO FINANCEIRO', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'N', 'N', NULL, 'N', 'N')
                       MATCHING (SIGLA_MODULO);

COMMIT WORK;


UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '98', 'Visualizar Escalas de Todas as Equipes', 'N', 'Visualizar escalas de todas as Equipes no Connect.')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '02', 'Cadastrar / Alterar Atendimento', 'N', 'Cadastra e altera Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '03', 'Excluir Atendimento', 'N', 'Exclui Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '04', 'Visualizar Atendimento', 'N', 'Visualiza Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '05', 'Cadastrar / Alterar Atividade NF', 'N', 'Cadastra e altera Atividade NF')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '06', 'Excluir Atividade NF', 'N', 'Exclui Atividade NF')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '07', 'Visualizar Atividade NF', 'N', 'Visualiza Atividade NF')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '11', 'Cadastrar / Alterar Campanha', 'N', 'Cadastra e altera Campanha')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '12', 'Excluir Campanha', 'N', 'Exclui Campanha')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '13', 'Visualizar Campanha', 'N', 'Visualiza Campanha')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '14', 'Cadastrar / Alterar Categoria Atendimento', 'N', 'Cadastra e altera Categoria Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '15', 'Excluir Categoria Atendimento', 'N', 'Exclui Categoria Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '16', 'Visualizar Categoria Atendimento', 'N', 'Visualiza Categoria Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '17', 'Cadastrar / Alterar Categoria Cliente', 'N', 'Cadastra e altera Categoria Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '18', 'Excluir Categoria Cliente', 'N', 'Exclui Categoria Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '19', 'Visualizar Categoria Cliente', 'N', 'Visualiza Categoria Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '20', 'Cadastrar / Alterar Cliente', 'N', 'Cadastra e altera Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '21', 'Excluir Cliente', 'N', 'Exclui Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '22', 'Visualizar Cliente', 'N', 'Visualiza Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '23', 'Cadastrar / Alterar Cliente Prospecto', 'N', 'Cadastra e altera Cliente Prospecto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '24', 'Excluir Cliente Prospecto', 'N', 'Exclui Cliente Prospecto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '25', 'Visualizar Cliente Prospecto', 'N', 'Visualiza Cliente Prospecto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '26', 'Cadastrar / Alterar Commisionamento', 'N', 'Cadastra e altera Commisionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '27', 'Excluir Commisionamento', 'N', 'Exclui Commisionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '28', 'Visualizar Commisionamento', 'N', 'Visualiza Commisionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '29', 'Cadastrar / Alterar Configuracoes Gerais', 'N', 'Cadastra e altera Configuracoes Gerais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '30', 'Excluir Configuracoes Gerais', 'N', 'Exclui Configuracoes Gerais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '31', 'Visualizar Configuracoes Gerais', 'N', 'Visualiza Configuracoes Gerais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '32', 'Cadastrar / Alterar Desconto', 'N', 'Cadastra e altera Desconto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '33', 'Excluir Desconto', 'N', 'Exclui Desconto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '34', 'Visualizar Desconto', 'N', 'Visualiza Desconto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '35', 'Cadastrar / Alterar Faixa Comissionamento', 'N', 'Cadastra e altera Faixa Comissionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '36', 'Excluir Faixa Comissionamento', 'N', 'Exclui Faixa Comissionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '37', 'Visualizar Faixa Comissionamento', 'N', 'Visualiza Faixa Comissionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '38', 'Cadastrar / Alterar Feriado', 'N', 'Cadastra e altera Feriado')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '39', 'Excluir Feriado', 'N', 'Exclui Feriado')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '40', 'Visualizar Feriado', 'N', 'Visualiza Feriado')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '44', 'Cadastrar / Alterar Funcionario', 'N', 'Cadastra e altera Funcionario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '45', 'Excluir Funcionario', 'N', 'Exclui Funcionario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '46', 'Visualizar Funcionario', 'N', 'Visualiza Funcionario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '47', 'Cadastrar / Alterar Meio de Captacao', 'N', 'Cadastra e altera Meio de Captacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '48', 'Excluir Meio de Captacao', 'N', 'Exclui Meio de Captacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '49', 'Visualizar Meio de Captacao', 'N', 'Visualiza Meio de Captacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '50', 'Cadastrar / Alterar Meta', 'N', 'Cadastra e altera Meta')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '51', 'Excluir Meta', 'N', 'Exclui Meta')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '52', 'Visualizar Meta', 'N', 'Visualiza Meta')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '53', 'Cadastrar / Alterar Modelo Contrato', 'N', 'Cadastra e altera Modelo Contrato')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '54', 'Inativar / Ativar Modelo Contrato', 'N', 'Inativar / Ativar Modelo Contrato')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '55', 'Visualizar Modelo Contrato', 'N', 'Visualiza Modelo Contrato')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '56', 'Cadastrar / Alterar Modelo Recibo', 'N', 'Cadastra e altera Modelo Recibo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '57', 'Excluir Modelo Recibo', 'N', 'Exclui Modelo Recibo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '58', 'Visualizar Modelo Recibo', 'N', 'Visualiza Modelo Recibo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '62', 'Cadastrar / Alterar Motivo Status Atendimento', 'N', 'Cadastra e altera Motivo Status Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '63', 'Excluir Motivo Status Atendimento', 'N', 'Exclui Motivo Status Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '64', 'Visualizar Motivo Status Atendimento', 'N', 'Visualiza Motivo Status Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '65', 'Cadastrar / Alterar NAS', 'N', 'Cadastra e altera NAS')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '66', 'Excluir NAS', 'N', 'Exclui NAS')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '67', 'Visualizar NAS', 'N', 'Visualiza NAS')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '68', 'Cadastrar / Alterar Origem Atendimento', 'N', 'Cadastra e altera Origem Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '69', 'Excluir Origem Atendimento', 'N', 'Exclui Origem Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '70', 'Visualizar Origem Atendimento', 'N', 'Visualiza Origem Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '71', 'Cadastrar / Alterar Pesquisa Satisfacao', 'N', 'Cadastra e altera Pesquisa Satisfacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '72', 'Excluir Pesquisa Satisfacao', 'N', 'Exclui Pesquisa Satisfacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '73', 'Visualizar Pesquisa Satisfacao', 'N', 'Visualiza Pesquisa Satisfacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '74', 'Cadastrar / Alterar Plano', 'N', 'Cadastra e altera Plano')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '75', 'Excluir Plano', 'N', 'Exclui Plano')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '76', 'Visualizar Plano', 'N', 'Visualiza Plano')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '77', 'Cadastrar / Alterar Produto', 'N', 'Cadastra e altera Produto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '78', 'Excluir Produto', 'N', 'Exclui Produto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '79', 'Visualizar Produto', 'N', 'Visualiza Produto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '80', 'Cadastrar / Alterar Ponto de Venda', 'N', 'Cadastra e altera Ponto de Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '81', 'Excluir Ponto de Venda', 'N', 'Exclui Ponto de Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '82', 'Visualizar Ponto de Venda', 'N', 'Visualiza Ponto de Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '83', 'Cadastrar / Alterar Script Atendimento', 'N', 'Cadastra e altera Script Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '84', 'Excluir Script Atendimento', 'N', 'Exclui Script Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '85', 'Visualizar Script Atendimento', 'N', 'Visualiza Script Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '86', 'Cadastrar / Alterar Sla Atendimento', 'N', 'Cadastra e altera Sla Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '87', 'Excluir Sla Atendimento', 'N', 'Exclui Sla Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '88', 'Visualizar Sla Atendimento', 'N', 'Visualiza Sla Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '89', 'Cadastrar / Alterar Sla Execucao', 'N', 'Cadastra e altera Sla Execucao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '90', 'Excluir Sla Execucao', 'N', 'Exclui Sla Execucao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '91', 'Visualizar Sla Execucao', 'N', 'Visualiza Sla Execucao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '92', 'Cadastrar / Alterar Tipo Atendimento', 'N', 'Cadastra e altera Tipo Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '93', 'Excluir Tipo Atendimento', 'N', 'Exclui Tipo Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '94', 'Visualizar Tipo Atendimento', 'N', 'Visualiza Tipo Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '95', 'Cadastrar / Alterar Tipo Checkpoint Vendedor', 'N', 'Cadastra e altera Tipo Checkpoint Vendedor')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '96', 'Excluir Tipo Checkpoint Vendedor', 'N', 'Exclui Tipo Checkpoint Vendedor')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '97', 'Visualizar Tipo Checkpoint Vendedor', 'N', 'Visualiza Tipo Checkpoint Vendedor')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '01', 'ACESSO AO SISTEMA', 'S', 'Acesso ao Sistema')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '100', 'Cadastrar / Alterar Campos Obrigatorios', 'N', 'Cadastra e altera Campos Obrigatorios')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '101', 'Cadastrar / Alterar Radius', 'N', 'Cadastrar e alterar Radius')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '102', 'Cadastra / Alterar OLT', 'N', 'Cadastrar e Alterar OLT')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '103', 'Visualizar IdentityUser Monitor Equipe', 'N', 'Visualizar IdentityUser (Monitor Equipes)')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '104', 'Visualizar Ponto Partida', 'N', 'Visualizar Ponto Partida')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '105', 'Visualizar Habilitar Servicos Equipe', 'N', 'Visualizar Habilitar Servicos Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '106', 'Visualizar Habilitar Area Equipe', 'N', 'Visualizar Habilitar Area Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '99', 'Cadastrar / Alterar Horario', 'N', 'Cadastra e Alter Horario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '107', 'Ativar / Inativar Horario', 'N', 'Ativar ou Inativar Horario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '108', 'Visualizar Criterio Roteirizacao', 'N', 'Visualizar Criterio Roteirizacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '109', 'Consultar Pesquisa Satisfacao CadCliente', 'N', 'Consultar Pesquisa Satisfacao CadCliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '110', 'Visualizar Radius', 'N', 'Visualizar Radius')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '111', 'Inativar / Ativar Radius', 'N', 'Inativar ou Ativar Radius')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '112', 'Visualizar OLT', 'N', 'Visualizar OLT')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '113', 'Inativar / Ativar OLT', 'N', 'Inativar ou Ativar OLT')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '114', 'Cadastro Escala de Trabalho', 'N', 'Cadastro Escala de Trabalho')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '115', 'Adicionar IdentityUser Monitor Equipe', 'N', 'Adicionar IdentityUser Monitor Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '116', 'Excluir IdentityUser Monitor Equipe', 'N', 'Excluir IdentityUser Monitor Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '117', 'Cadastrar Ponto Partida', 'N', 'Cadastrar Ponto Partida')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '118', 'Inativar / Ativar Ponto Partida', 'N', 'Inativar ou Ativar Ponto Partida')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '119', 'Adicionar Servico', 'N', 'Adicionar Servico')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '120', 'Adicionar Area Equipe', 'N', 'Adicionar Area Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '135', 'Cadastrar / Alterar Venda', 'N', 'Cadastrar / Alterar Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '136', 'Excluir Venda', 'N', 'Excluir Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '137', 'Habilitar Usuário a cadastrar sem acesso RF', 'N', 'Habilitar Usuário a cadastrar sem acesso RF')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '138', 'Alterar Endereço do Cliente', 'N', 'Alterar Endereço do Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '139', 'Definir Data/Hora Execução do Serviço', 'S', 'Definir Data/Hora Execução do Serviço')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '140', 'Cadastrar / Alterar Script Suporte Técnico', 'N', 'Cadastrar / Alterar Script Suporte Técnico')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '141', 'Excluir Script Suporte Técnico', 'N', 'Excluir Script Suporte Técnico')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '142', 'Visualizar Script Suporte Técnico', 'N', 'Visualizar Script Suporte Técnico')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '143', 'Cadastrar / Alterar Perfil de Dashboard', 'N', 'Cadastrar / Alterar Perfil de Dashboard')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '144', 'Excluir Perfil de Dashboard', 'N', 'Excluir Perfil de Dashboard')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '145', 'Visualizar Perfil de Dashboard', 'N', 'Visualizar Perfil de Dashboard')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '146', 'Cadastrar / Alterar Vincular Usuario ao Perfil', 'N', 'Cadastrar / Alterar Vincular Usuario ao Perfil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '147', 'Excluir Vincular Usuario ao Perfil', 'N', 'Excluir Vincular Usuario ao Perfil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '148', 'Visualizar Vincular Usuario ao Perfil', 'N', 'Visualizar Vincular Usuario ao Perfil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '150', 'Reabrir Atendimento', 'N', 'Reabrir Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '151', 'ESTORNO DE LANCAMENTO DE FATURAMENTO', 'N', NULL)
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '152', 'APROVAR/REPROVAR A AVALIAÇÃO COMERCIAL', 'N', 'aprovar a avaliação comercial ou reprovar a avalia')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '153', 'Alterar Vendedor/Supervisor', 'N', 'Alterar Vendedor/Supervisor')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '01', 'Acesso ao Sistema', 'N', 'Acesso ao Sistema')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '02', 'Cadastrar / Alterar Contas a Pagar', 'N', 'Cadastrar / Alterar Contas a Pagar')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '03', 'Inativar/Ativar Contas a Pagar', 'N', 'Inativar/Ativar Contas a Pagar')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '04', 'Visualizar Contas a Pagar', 'N', 'Visualizar Contas a Pagar')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '05', 'Cadastrar / Alterar Contas a Receber', 'N', 'Cadastrar / Alterar Contas a Receber')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '06', 'Inativar/Ativar Contas a Receber', 'N', 'Inativar/Ativar Contas a Receber')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '121', 'Acesso Menu Comercial', 'N', 'Acesso Menu Comercial')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '122', 'Visualizar Venda', 'N', 'Visualizar Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '123', 'Visualizar DashBoard Venda', 'N', 'Visualizar DashBoard Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '124', 'Visualizar Gestão de Equipe', 'N', 'Visualizar Gestão de Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '07', 'Visualizar Contas a Receber', 'N', 'Visualizar Contas a Receber')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '08', 'Cadastrar / Alterar Transferência', 'N', 'Cadastrar / Alterar Transferência')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '09', 'Inativar/Ativar Transferência', 'N', 'Inativar/Ativar Transferência')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '10', 'Visualizar Transferência', 'N', 'Visualizar Transferência')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '11', 'CadastrarFaturamento', 'N', 'CadastrarFaturamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '12', 'Calcular / Processar Faturamento', 'N', 'Calcular / Processar Faturamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '13', 'Visualizar Faturamento', 'N', 'Visualizar Faturamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '14', 'Gerar Notas Fiscais', 'N', 'Gerar Notas Fiscais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '15', 'Imprimir Notas Fiscais', 'N', 'Imprimir Notas Fiscais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '16', 'Visualizar Emitir Nota Fiscal', 'N', 'Visualizar Emitir Nota Fiscal')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '17', 'Cadastrar / Alterar Banco', 'N', 'Cadastrar / Alterar Banco')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '18', 'Inativar/Ativar Banco', 'N', 'Inativar/Ativar Banco')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '19', 'Visualizar Banco', 'N', 'Visualizar Banco')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '20', 'Cadastrar / Alterar Caixa Interno', 'N', 'Cadastrar / Alterar Caixa Interno')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '21', 'Inativar/Ativar Caixa Interno', 'N', 'Inativar/Ativar Caixa Interno')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '22', 'Visualizar Caixa Interno', 'N', 'Visualizar Caixa Interno')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '23', 'Cadastrar / Alterar Cartão de Crédito', 'N', 'Cadastrar / Alterar Cartão de Crédito')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '24', 'Inativar/Ativar Cartão de Crédito', 'N', 'Inativar/Ativar Cartão de Crédito')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '25', 'Visualizar Cartão de Crédito', 'N', 'Visualizar Cartão de Crédito')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '26', 'Cadastrar / Alterar Centro de Custo', 'N', 'Cadastrar / Alterar Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '27', 'Inativar/Ativar Centro de Custo', 'N', 'Inativar/Ativar Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '28', 'Visualizar Centro de Custo', 'N', 'Visualizar Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '29', 'Cadastrar / Alterar Conta Bancária', 'N', 'Cadastrar / Alterar Conta Bancária')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '30', 'Inativar/Ativar Conta Bancária', 'N', 'Inativar/Ativar Conta Bancária')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '31', 'Visualizar Conta Bancária', 'N', 'Visualizar Conta Bancária')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '32', 'Cadastrar / Alterar Conta Contábil', 'N', 'Cadastrar / Alterar Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '33', 'Inativar/Ativar Conta Contábil', 'N', 'Inativar/Ativar Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '34', 'Visualizar Conta Contábil', 'N', 'Visualizar Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '125', 'Visualizar Configurações', 'N', 'Visualizar Configurações')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '126', 'Visualizar Relatórios', 'N', 'Visualizar Relatórios')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '127', 'Visualizar Indicadores', 'N', 'Visualizar Indicadores')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '128', 'Visualizar Roteirização', 'N', 'Visualizar Roteirização')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '129', 'Visualizar Ajuda', 'N', 'Visualizar Ajuda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '130', 'Acompanhamento de Equipes', 'N', 'Acompanhamento de Equipes')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '131', 'Equipes no Mapa', 'N', 'Equipes no Mapa')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '132', 'Criar Ordem de Serviço', 'N', 'Criar Ordem de Serviço')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '133', 'Painel de Serviços', 'N', 'Painel de Serviços')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '134', 'Timeline de Serviços', 'N', 'Timeline de Serviços')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '35', 'Cadastrar / Alterar Conta Financeira', 'N', 'Cadastrar / Alterar Conta Financeira')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '36', 'Inativar/Ativar Conta Financeira', 'N', 'Inativar/Ativar Conta Financeira')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '37', 'Visualizar Conta Financeira', 'N', 'Visualizar Conta Financeira')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '38', 'Cadastrar / Alterar Grupo de Centro de Custo', 'N', 'Cadastrar / Alterar Grupo de Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '39', 'Inativar/Ativar Grupo de Centro de Custo', 'N', 'Inativar/Ativar Grupo de Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '40', 'Visualizar Grupo de Centro de Custo', 'N', 'Visualizar Grupo de Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '41', 'Cadastrar / Alterar Subgrupo Centro de Custo', 'N', 'Cadastrar / Alterar Subgrupo Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '42', 'Inativar/Ativar Subgrupo Centro de Custo', 'N', 'Inativar/Ativar Subgrupo Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '43', 'Visualizar Subgrupo Centro de Custo', 'N', 'Visualizar Subgrupo Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '44', 'Cadastrar / Alterar Grupo Conta Contábil', 'N', 'Cadastrar / Alterar Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '45', 'Inativar/Ativar Grupo Conta Contábil', 'N', 'Inativar/Ativar Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '46', 'Visualizar Grupo Conta Contábil', 'N', 'Visualizar Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '47', 'Cadastrar / Alterar Sub Grupo Conta Contábil', 'N', 'Cadastrar / Alterar Sub Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '48', 'Inativar/Ativar Sub Grupo Conta Contábil', 'N', 'Inativar/Ativar Sub Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '49', 'Visualizar Sub Grupo Conta Contábil', 'N', 'Visualizar Sub Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '50', 'Cadastrar / Alterar Grupo Conta Financeira', 'N', 'Cadastrar / Alterar Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '51', 'Inativar/Ativar Grupo Conta Financeira', 'N', 'Inativar/Ativar Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '52', 'Visualizar Grupo Conta Financeira', 'N', 'Visualizar Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '53', 'Cadastrar / Alterar Sub Grupo Conta Financeira', 'N', 'Cadastrar / Alterar Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '54', 'Inativar/Ativar Sub Grupo Conta Financeira', 'N', 'Inativar/Ativar Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '55', 'Visualizar Sub Grupo Conta Financeira', 'N', 'Visualizar Grupo Conta Contábil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '56', 'Cadastrar / Alterar Grupo de Rateio Entre Filiais', 'N', 'Cadastrar / Alterar Subgrupo Conta Financeira')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '57', 'Inativar/Ativar Grupo de Rateio Entre Filiais', 'N', 'Inativar/Ativar Subgrupo Conta Financeira')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '58', 'Visualizar Grupo de Rateio Entre Filiais', 'N', 'Visualizar Subgrupo Conta Financeira')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '59', 'Cadastrar / Alterar Estorno de Pagamento', 'N', 'Cadastrar / Alterar Grupo de Rateio Entre Filiais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '60', 'Inativar/Ativar Estorno de Pagamento', 'N', 'Inativar/Ativar Grupo de Rateio Entre Filiais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '61', 'Visualizar Estorno de Pagamento', 'N', 'Visualizar Grupo de Rateio Entre Filiais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '62', 'Cadastrar / Alterar Forma de Pagamento', 'N', 'Cadastrar / Alterar Estorno de Pagamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '63', 'Inativar/Ativar Forma de Pagamento', 'N', 'Inativar/Ativar Estorno de Pagamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '64', 'Visualizar Forma de Pagamento', 'N', 'Visualizar Estorno de Pagamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '65', 'Cadastrar / Alterar Orçamento de Centro de Custo', 'N', 'Cadastrar / Alterar Forma de Pagamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '66', 'Inativar/Ativar Orçamento de Centro de Custo', 'N', 'Inativar/Ativar Forma de Pagamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '67', 'Visualizar Orçamento de Centro de Custo', 'N', 'Visualizar Forma de Pagamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '68', 'Cadastrar / Alterar Vencimento', 'N', 'Cadastrar / Alterar Orçamento de Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '73', 'Visualizar Perfil de Dashboard', 'N', 'Visualizar Perfil de Dashboard')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '69', 'Inativar/Ativar Vencimento', 'N', 'Inativar/Ativar Orçamento de Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '74', 'Cadastrar / Alterar Perfil de Dashboard', 'N', 'Cadastrar / Alterar Perfil de Dashboard')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '75', 'Inativar/Ativar Perfil de Dashboard', 'N', 'Inativar/Ativar Perfil de Dashboard')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '76', 'Cadastrar / Alterar Vincular Usuario ao Perfil', 'N', 'Cadastrar / Alterar Vincular Usuario ao Perfil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '77', 'Inativar/Ativar Vincular Usuario ao Perfil', 'N', 'Inativar/Ativar Vincular Usuario ao Perfil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '78', 'Visualizar Vincular Usuario ao Perfil', 'N', 'Visualizar Vincular Usuario ao Perfil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '79', 'Cadastrar / Alterar Modelo de Recibo', 'N', 'Cadastrar / Alterar Modelo de Recibo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '80', 'Inativar / Ativar Modelo de Recibo', 'N', 'Inativar/Ativar Modelo de Recibo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '81', 'Visualizar Modelo de Recibo', 'N', 'Visualizar Modelo de Recibo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '82', 'Visualizar Conciliação Financeira', 'N', 'Visualizar Conciliação Financeira')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '84', 'Visualizar Faturamento', 'N', 'Visualizar Faturamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '85', 'Cadastrar / Alterar Crédito Faturamento', 'N', 'Cadastrar / Alterar Crédito Faturamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '83', 'Visualizar Movimentações Financeiras', 'N', 'Visualizar Movimentações Financeiras')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '86', 'Aprovar / Cancelar Crédito Faturamento', 'N', 'Aprovar / Cancelar Crédito Faturamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '87', 'Visualizar Crédito Faturamento', 'N', 'Visualizar Crédito Faturamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '70', 'Visualizar Vencimento', 'N', 'Visualizar Orçamento de Centro de Custo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '71', 'Visualizar Relatórios', 'N', 'Visualizar Relatórios')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('EVA', '72', 'Visualizar Configurações', 'N', 'Visualizar Configurações')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '149', 'Visualizar Gerencia de Rede', 'N', 'Visualizar Gerencia de Rede')
                            MATCHING (SIGLA_MODULO, COD_AREA);

COMMIT WORK;
