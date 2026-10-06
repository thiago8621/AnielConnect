set term ^;
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_AGENDAMENTO_EQUIPEANIEL     into x;  execute statement 'set generator     GN_AGENDAMENTO_EQUIPEANIEL     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_EQUIPE_LOCALIDADE     into x;  execute statement 'set generator     GN_EQUIPE_LOCALIDADE     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_EQUIPE_NATUREZA     into x;  execute statement 'set generator     GN_EQUIPE_NATUREZA     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_EQUIPE_PONTOPARTIDA     into x;  execute statement 'set generator     GN_EQUIPE_PONTOPARTIDA     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_EQUIPE_STATUS     into x;  execute statement 'set generator     GN_EQUIPE_STATUS     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_HORARIO_CONNECT     into x;  execute statement 'set generator     GN_HORARIO_CONNECT     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_HORARIO_EQUIPE_CONNECT     into x;  execute statement 'set generator     GN_HORARIO_EQUIPE_CONNECT     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_LOG_ROTEIRIZACAO     into x;  execute statement 'set generator     GN_LOG_ROTEIRIZACAO     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_LOG_ROTEIRIZACAO_ITEM     into x;  execute statement 'set generator     GN_LOG_ROTEIRIZACAO_ITEM     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_PONTO_PARTIDA     into x;  execute statement 'set generator     GN_PONTO_PARTIDA     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_SLA_SERVICO     into x;  execute statement 'set generator     GN_SLA_SERVICO     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_TIPO_SERVICO_EQUIPE_IMOVEL     into x;  execute statement 'set generator     GN_TIPO_SERVICO_EQUIPE_IMOVEL     to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from     TB_CAMPOS_OBRIG_DOC_PROD     into x;  execute statement 'set generator     GN_CAMPOS_OBRIG_DOC_PROD     to ' || x; end^

set term ;^

