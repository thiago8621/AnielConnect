set term ^;                
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_BANCO_HORAS_ITEM         into x;  execute statement 'set generator         GN_PT_BANCO_HORAS_ITEM         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_BANCO_HORAS_MOVTO         into x;  execute statement 'set generator         GN_PT_BANCO_HORAS_MOVTO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_BANCO_HORAS         into x;  execute statement 'set generator         GN_PT_BANCO_HORAS         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_CERCA_ELETRONICA         into x;  execute statement 'set generator         GN_PT_CERCA_ELETRONICA         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_CLASSIFICACAO_ESCALA         into x;  execute statement 'set generator         GN_PT_CLASSIFICACAO_ESCALA         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_COMUNICADO_USUARIO         into x;  execute statement 'set generator         GN_PT_COMUNICADO_USUARIO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_COMUNICADO         into x;  execute statement 'set generator         GN_PT_COMUNICADO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_FECHAMENTO_PERIODO         into x;  execute statement 'set generator         GN_PT_FECHAMENTO_PERIODO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_FECHAMENTO_PERIODO         into x;  execute statement 'set generator         GN_PT_FECHAMENTO_PERIODO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_FERIADO         into x;  execute statement 'set generator         GN_PT_FERIADO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_HISTORICOFUNCJORN         into x;  execute statement 'set generator         GN_PT_HISTORICOFUNCJORN         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_JORNADA_DIA         into x;  execute statement 'set generator         GN_PT_JORNADA_DIA         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_JORNADA_LOG         into x;  execute statement 'set generator         GN_PT_JORNADA_LOG         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_JORNADA         into x;  execute statement 'set generator         GN_PT_JORNADA         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_MUNICIPIO         into x;  execute statement 'set generator         GN_PT_MUNICIPIO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_PONTO_ABONO         into x;  execute statement 'set generator         GN_PT_PONTO_ABONO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_TIPO_ABONO         into x;  execute statement 'set generator         GN_PT_TIPO_ABONO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_PONTO         into x;  execute statement 'set generator         GN_PT_PONTO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_PONTO_HISTORICO_JANELA         into x;  execute statement 'set generator         GN_TB_PT_PONTO_HISTORICO         to ' || x; end^
execute block as declare variable x integer; begin SELECT COALESCE(max(id),0) from         TB_PT_PONTO_ABONO         into x;  execute statement 'set generator         GN_PT_PONTO_ABONO         to ' || x; end^
set term ;^                


