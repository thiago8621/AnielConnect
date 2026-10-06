DELETE FROM TB_PERFIL_USUARIO_SUBACESSOS WHERE SIGLA_MODULO = 'PONTOELETR';
DELETE FROM TB_SUB_ACESSO WHERE SIGLA_MODULO = 'PONTOELETR';
DELETE FROM TB_PERFIL_DE_USUARIO_ACESSO WHERE SIGLA_MODULO = 'PONTOELETR';
DELETE FROM TB_ACESSO WHERE SIGLA_MODULO = 'PONTOELETR';
DELETE FROM TB_AREA_MODULO  WHERE SIGLA_MODULO = 'PONTOELETR';
DELETE FROM TB_MODULO WHERE SIGLA_MODULO = 'PONTOELETR';

COMMIT WORK;

UPDATE OR INSERT INTO TB_MODULO (SIGLA_MODULO, DESCRICAO, ATALHO, CAMINHO_REDE, SIGLA_AUX, RESPONSAVEL, COMPUTADOR, DATA_ARQUIVO, HORA_ARQUIVO, NOME_ARQUIVO, CONFIG_BD, BD_CONFIG, COD_PRODUTO, SERVICO, ENVIA_EXE)
                         VALUES ('POINT', 'PONTO ELETRÔNICO WEB', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'N', 'N', NULL, 'N', 'N')
                       MATCHING (SIGLA_MODULO);

COMMIT WORK;

UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '01', 'Visualizar informações de todos os funcionários', 'N', 'visualizar informações de todos os funcionários')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '02', 'Visualizar listagem de departamentos', 'N', 'visualizar listagem de departamentos')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '03', 'Visualizar listagem de funções', 'N', 'visualizar listagem de funções')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '04', 'Visualizar listagem de locais de marcação', 'N', 'visualizar listagem de locais de marcação')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '05', 'Visualizar listagem de motivos de ajuste', 'N', 'visualizar listagem de motivos de ajuste')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '06', 'Visualizar listagem de feriados', 'N', 'visualizar listagem de feriados')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '07', 'Visualizar listagem de eventos', 'N', 'visualizar listagem de eventos')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '08', 'Visualizar listagem de jornadas', 'N', 'visualizar listagem de jornadas')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '09', 'Duplicar jornada', 'N', 'duplicar jornada')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '10', 'Visualizar listagem de funcionários', 'S', 'visualizar listagem de funcionários')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '11', 'Importar funcionários', 'N', 'Importar funcionários')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '12', 'Cadastrar/alterar departamento', 'N', 'cadastrar/alterar departamento')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '13', 'Cadastrar/alterar função', 'N', 'cadastrar/alterar função')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '14', 'Cadastrar/alterar locais marcação', 'N', 'cadastrar/alterar locais marcação')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '15', 'Cadastrar/alterar motivos de ajuste', 'N', 'cadastrar/alterar motivos de ajuste')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '16', 'Cadastrar/alterar feriados', 'N', 'cadastrar/alterar feriados')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '17', 'Cadastrar/alterar eventos', 'N', 'cadastrar/alterar eventos')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '18', 'Cadastrar/alterar jornada', 'N', 'cadastrar/alterar jornada')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '19', 'Cadastrar/alterar funcionário', 'S', 'cadastrar/alterar funcionário')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '20', 'Criar e gerar QrCode', 'N', 'Acesso gerar QrCode')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '21', 'Trocar a jornada de trabalho', 'N', 'trocar a jornada de trabalho')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '22', 'Alterar o valor HxH do funcionário', 'N', 'alterar o valor HxH do funcionário')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '23', 'Habilitar locais de marcação de ponto', 'N', 'habilitar locais de marcação de ponto')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '24', 'Criar local de marcação para um funcionário', 'N', 'criar local específico de marcação para um funcionário')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '25', 'Logs do funcionário - Férias, Jornada e Situação', 'N', 'Acesso aos logs do funcionário (Férias, Jornada e Situação)')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '26', 'Visualizar escala de trabalho e Exportar Excel', 'S', 'visualizar escala de trabalho e Exportar Excel')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '27', 'Lançar ou alterar evento na escala de trabalho', 'S', 'lançar ou alterar evento na escala de trabalho')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '28', 'Log de trocas de Escala', 'S', 'Acesso ao log de trocas de jornada')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '29', 'Visualizar as configurações de banco de horas', 'N', 'visualizar as configurações de banco de horas')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '30', 'Alterar duração do período e teto do banco', 'N', 'alterar duração do período e teto do banco')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '31', 'Visualizar o log de períodos de banco fechados', 'N', 'visualizar o log de períodos de banco fechados')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '32', 'Importar saldo inicial de banco do horas', 'N', 'importar saldo inicial de banco do horas')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '33', 'Definir alertas com prioridades', 'N', 'definir alertas com prioridades')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '34', 'Visualizar a folha de ponto', 'S', 'visualizar a folha de ponto')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '35', 'Ajustar folha de ponto', 'N', 'ajustar folha de ponto')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '36', 'Visualizar o painel de avaliação de ajustes', 'S', 'visualizar o painel de avaliação de ajustes')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '37', 'Avaliar pedidos de ajuste', 'N', 'avaliar pedidos de ajuste')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '38', 'Processar fechamento de folhas de ponto', 'S', 'Acesso ao fechamento de folhas de ponto (apenas processar)')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '39', 'Quintar o banco de horas (zerar)', 'S', 'quintar o banco de horas (zerar)')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '40', 'Fechar Folhas de Ponto', 'N', 'fechar período')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '41', 'Visualizar períodos fechados', 'N', 'visualizar períodos fechados')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '42', 'Relatório Sintético - Saldo de Banco de Horas', 'S', 'Acesso Relatório Sintético de Saldo de Banco de Horas')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '43', 'Relatório Sintético - Horas Extras Realizadas', 'S', 'Acesso Relatório Sintético de Horas Extras Realizadas')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '44', 'Relatório Sintético -  Horas Efetivas', 'S', 'Acesso Relatório Sintético de Horas Efetivas')
                            ;
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('POINT', '45', 'Relatório de Marcações Originais', 'S', 'Acesso Relatório de Marcações Originais')
                            ;

COMMIT WORK;


