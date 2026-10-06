UPDATE OR INSERT INTO TB_PT_MOTIVO_AJUSTE (ID, DESCRICAO, STATUS)
                                   VALUES (1, 'Esquecimento', 1)
                                 MATCHING (ID);
UPDATE OR INSERT INTO TB_PT_MOTIVO_AJUSTE (ID, DESCRICAO, STATUS)
                                   VALUES (2, 'Sistema Indisponível', 1)
                                 MATCHING (ID);
UPDATE OR INSERT INTO TB_PT_MOTIVO_AJUSTE (ID, DESCRICAO, STATUS)
                                   VALUES (3, 'Sem Celular', 1)
                                 MATCHING (ID);
UPDATE OR INSERT INTO TB_PT_MOTIVO_AJUSTE (ID, DESCRICAO, STATUS)
                                   VALUES (4, 'Em Área de Risco', 1)
                                 MATCHING (ID);
UPDATE OR INSERT INTO TB_PT_MOTIVO_AJUSTE (ID, DESCRICAO, STATUS)
                                   VALUES (5, 'Sem Bateria', 1)
                                 MATCHING (ID);

COMMIT WORK;
