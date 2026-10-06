using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20210518_1809_SP_LISTA_PRODUCAO_FAIXA_CONNE : AnielMigration
    {
        public _20210518_1809_SP_LISTA_PRODUCAO_FAIXA_CONNE()
        {
            Codigo = "20210518_1809";
            Descricao = "Proc relatório de produção analitico";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"create or alter procedure SP_LISTA_PRODUCAO_FAIXA_CONNE (
    PCODCT varchar(4),
    PPROJETO varchar(20),
    PEQUIPE varchar(10),
    PCOD_SUPERV varchar(6),
    PCOD_TIPO_SERV smallint,
    PCOD_NATUREZA integer,
    PDATAINI date,
    PDATAFIM date,
    PTIPODATA varchar(100),
    PCERTIFICADO char(1),
    PDEVOLVIDO char(1),
    PDATACRIACAO char(1),
    PQUANT_PREV char(1),
    PBAIXAMOBILE char(1))
returns (
    RCODCT varchar(4),
    RPROJETO varchar(20),
    REQUIPE varchar(10),
    RNOME varchar(50),
    REQUIPE_AUX varchar(10),
    RNOME_AUX varchar(50),
    RCOD_SUPERV varchar(6),
    RSUPERVISOR varchar(50),
    RDATA date,
    RCONTRATO integer,
    ROS varchar(50),
    RCOD_TIPO_SERV smallint,
    RDESC_TIPO_SERV varchar(150),
    RSEGMENTO varchar(15),
    RUNIDADE varchar(5),
    RMANIFESTO varchar(15),
    RSENHA_BX varchar(100),
    RBAIRRO varchar(30),
    RTIPO_IMOVEL varchar(50),
    RCOD_ENCERRAMENTO varchar(6),
    RDESCRICAO_ENCERRAMENTO varchar(95),
    RDEVOLVIDO char(1),
    RCERTIFICADO char(1),
    RVEICULO varchar(20),
    RCODIGO varchar(20),
    RUNIDADE_CUSTO DM_UND_CUSTO,
    RDESC_SERV varchar(500),
    RCOD_PRECO varchar(5),
    RPONTO numeric(13,6),
    RVALOR_UNIDADE_CUSTO numeric(13,5),
    RVALOR_PROD numeric(13,5))
as
declare variable VSQL varchar(3000);
declare variable VCLASSE_MO varchar(5);
declare variable VID_DEVOLVIDO integer;
declare variable VNUM_FRE varchar(6);
declare variable VCOD_TIPO_IMOVEL integer;
declare variable VCO integer;
declare variable VQTDEPREV numeric(15,6);
declare variable VQTDEPPROD numeric(15,6);
declare variable VPONTO double precision;
BEGIN
    VSQL = 'SELECT A.CODCT, A.PROJETO, A.EQUIPE, C.NOME, K.NOME AS SUPERVISOR, A.DATA_PROD, A.UNIDADE_CUSTO, '||
           '       A.NUM_DOC, A.NUM_OBRA, B.NUM_RVT, B.SENHA_BAIXA, B.ENDERECO_BAIRRO, '||
           '       D.DESCRICAO, B.COD_ENCERRAMENTO, L.DESCRICAO AS DESCRICAO_ENCERRAMENTO, G.SEQITEM, B.CERTIFICADO, '||
           '       A.CODIGO, E.DESCRICAO, CAST(E.QUANTIDADE AS NUMERIC(13,6)), '||
           '       A.COD_PRECO, E.CLASSE_MO, C.NUM_FRE, B.PARPRIM_PARA, '||
           '       A.COD_TIPO_SERV, J.DESCRICAO AS DESC_TIPO_SERV, B.SEGMENTO, E.UNIDADE,'||
           '       A.QUANT_CATAL, A.QUANT_PROD, ';
    /* SE FILTRA POR EQUIPE REPETE PARA AS AUXILIARES */
    IF (PEQUIPE <> '-') THEN
        VSQL = VSQL || ' H.EQUIPE, I.NOME ';
    ELSE
        VSQL = VSQL || ' ''-'', ''-'' ';

    VSQL = VSQL || 'FROM TB_CUSTOS_TAREFA A '||
           'INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND '||
           '                                       A.NUM_OBRA = B.NUM_OBRA AND A.NUM_DOC = B.NUM_DOC) '||
           'LEFT JOIN TB_EQUIPE C ON (A.EQUIPE = C.EQUIPE) '||
           'LEFT JOIN TB_TIPO_IMOVEL D ON (B.PARPRIM_PARA = D.COD_TIPO_IMOVEL) '||
           'LEFT JOIN TB_CADERNO_ENCARGOS E ON (A.UNIDADE_CUSTO = E.UNIDADE_CUSTO AND '||
           '                                    A.CODIGO = E.CODIGO) '||
           'LEFT JOIN TB_DEVOLUCAO_CONTRATO_ITEM G ON (B.DATA_MAXIMA = G.DATA_MAXIMA AND B.NUM_DOC = G.NUM_DOC) ' ||
           'LEFT JOIN TB_FUNCIONARIO K ON (K.NUM_FRE = C.COD_SUPERV) ' ||
           'LEFT JOIN TB_CODIGO_ENCERRAMENTO L ON (B.COD_ENCERRAMENTO = L.COD_ENCERRAMENTO AND B.CODCT = L.CODCT) ' ||
           'LEFT JOIN TB_TIPO_SERVICO_EQUIPE J ON (J.COD_TIPO_SERV = A.COD_TIPO_SERV) ';

    /* SE FILTRA POR EQUIPE REPETE PARA AS AUXILIARES */
    IF (PEQUIPE <> '-') THEN
        VSQL = VSQL || ' LEFT JOIN TB_MOBILE_PARTICIPANTES_RDO H ON (A.CODCT = H.CODCT AND A.PROJETO = H.PROJETO '||
                       '                                         AND A.NUM_OBRA = H.NUM_OBRA AND A.NUM_DOC = H.NUM_DOC) '||
                       ' LEFT JOIN TB_EQUIPE I ON (H.EQUIPE = I.EQUIPE) ';

    VSQL = VSQL || ' WHERE A.CODCT = '''||:PCODCT||'''';

    if (:PPROJETO <> '-') then
       VSQL = VSQL || ' AND A.PROJETO = '''||:PPROJETO||''' ';

    /* VERIFICA A DATA USADA NO FILTRO */
    IF (PTIPODATA = 'CRIACAO') THEN
       VSQL = VSQL || ' AND B.DIA BETWEEN '''||:PDATAINI||''' AND '''||:PDATAFIM||''' ';
    IF (PTIPODATA = 'AGENDAMENTO') THEN
       VSQL = VSQL || ' AND B.DATA_MAXIMA BETWEEN '''||:PDATAINI||''' AND '''||:PDATAFIM||''' ';
    IF(PTIPODATA = 'EXECUCAO') THEN
       VSQL = VSQL || ' AND A.DATA_PROD BETWEEN '''||:PDATAINI||''' AND '''||:PDATAFIM||''' ';

    /* VERIFICA SE E PREVISTO OU PRODUZIDO */
    IF (PQUANT_PREV = 'S') THEN
       VSQL = VSQL || ' AND A.QUANT_CATAL > 0 ';
    ELSE
       VSQL = VSQL || ' AND A.QUANT_PROD > 0 ';

    if (:PCOD_SUPERV <> '-') then
       VSQL = VSQL || ' AND C.COD_SUPERV = ''' || :PCOD_SUPERV || ''' ';

    if (:PCOD_TIPO_SERV > 0) then
       VSQL = VSQL || ' AND A.COD_TIPO_SERV = ' || :PCOD_TIPO_SERV;

    if (:PCOD_NATUREZA > 0) then
        VSQL = VSQL || '  AND J.COD_NATUREZA = ' || :PCOD_NATUREZA;

    /* VERIFICA SE FOI BAIXADA PELO MOBILE */
    IF (PBAIXAMOBILE = 'S') THEN
       VSQL = VSQL || ' AND UPPER(B.MODULO_BAIXA) = ''MOBTV'' ';

    /* VERIFICA SE FILTRA POR EQUIPE */
    IF (PEQUIPE <> '-') THEN
        VSQL = VSQL || ' AND (A.EQUIPE = '''||:PEQUIPE||''' OR  H.EQUIPE = '''||:PEQUIPE||''') ';

    /* VERIFICA SE FILTRA OS CERTIFICADA */
    IF (PCERTIFICADO = 'S') THEN
        VSQL = VSQL || ' AND B.CERTIFICADO = ''S'' ';

    VSQL = VSQL || ' ORDER BY A.CODCT, A.PROJETO, A.EQUIPE, A.DATA_PROD';

    FOR
        EXECUTE STATEMENT :VSQL
        INTO :RCODCT, :RPROJETO, :REQUIPE, :RNOME, :RSUPERVISOR, :RDATA, :RUNIDADE_CUSTO,
             :RCONTRATO, :ROS, :RMANIFESTO, :RSENHA_BX, :RBAIRRO, :RTIPO_IMOVEL,
             :RCOD_ENCERRAMENTO, :RDESCRICAO_ENCERRAMENTO, :VID_DEVOLVIDO, :RCERTIFICADO, :RCODIGO,
             :RDESC_SERV, :RPONTO, :RCOD_PRECO, :VCLASSE_MO, :VNUM_FRE,
             :VCOD_TIPO_IMOVEL, :RCOD_TIPO_SERV, :RDESC_TIPO_SERV, :RSEGMENTO, :RUNIDADE,
             :VQTDEPREV, :VQTDEPPROD, :REQUIPE_AUX, :RNOME_AUX
    DO
    BEGIN
        RVALOR_UNIDADE_CUSTO = 0;

        /* PEGAR VALOR PONTO */
        SELECT CAST(COALESCE(VALOR_PREMIO, 0) AS NUMERIC(13,5))
        FROM TB_UNIDADE_CUSTO_EQUIPE
        WHERE EQUIPE = :REQUIPE AND UNIDADE_CUSTO = :RUNIDADE_CUSTO
          AND CLASSE_MO = :VCLASSE_MO AND COD_PRECO = :RCOD_PRECO
        INTO :RVALOR_UNIDADE_CUSTO;

        IF (COALESCE(RVALOR_UNIDADE_CUSTO, 0) = 0) THEN
        BEGIN
            SELECT CAST(COALESCE(VALOR_PREMIO, 0) AS NUMERIC(13,5))
            FROM TB_UNIDADE_CUSTO_PROJETO
            WHERE CODCT = :RCODCT AND PROJETO = :RPROJETO
              AND UNIDADE_CUSTO = :RUNIDADE_CUSTO
              AND CLASSE_MO =:VCLASSE_MO AND COD_PRECO=:RCOD_PRECO
            INTO :RVALOR_UNIDADE_CUSTO;

            IF (COALESCE(RVALOR_UNIDADE_CUSTO, 0) = 0) THEN
            BEGIN
                SELECT CAST(COALESCE(VALOR_PREMIO, 0) AS NUMERIC(13,5))
                FROM TB_UNIDADE_CUSTO_CONTRATO
                WHERE CODCT = :RCODCT AND UNIDADE_CUSTO = :RUNIDADE_CUSTO
                  AND CLASSE_MO = :VCLASSE_MO AND COD_PRECO = :RCOD_PRECO
                INTO :RVALOR_UNIDADE_CUSTO;

                IF (COALESCE(RVALOR_UNIDADE_CUSTO, 0) = 0) THEN
                    RVALOR_UNIDADE_CUSTO = 0;
            END
        END

        IF (PQUANT_PREV = 'S') THEN
        BEGIN
          VPONTO = CAST(:VQTDEPREV AS DOUBLE PRECISION) * CAST(:RPONTO AS DOUBLE PRECISION);
        END
        ELSE 
        BEGIN
          VPONTO = CAST(:VQTDEPPROD AS DOUBLE PRECISION) * CAST(:RPONTO AS DOUBLE PRECISION);
        END

        RPONTO = :VPONTO;
        RVALOR_PROD = CAST(VPONTO * CAST(:RVALOR_UNIDADE_CUSTO AS DOUBLE PRECISION) AS NUMERIC(13,6));

        /* CASO O ITEM SEJA DE VARIAS OS, REMOVE O NUMERO DA OS */
        SELECT COUNT(*) FROM TB_IMP_CADERNO_TIPO_SERV A
        INNER JOIN TB_IMP_CADERNO_ENCARGO B ON (A.ID_IMP_CADERNO = B.ID)
        WHERE B.UNIDADE_CUSTO = :RUNIDADE_CUSTO AND B.CODIGO = :RCODIGO
          AND B.COD_TIPO_IMOVEL = :VCOD_TIPO_IMOVEL
        INTO :VCO;

        IF (VCO > 1 ) THEN
            ROS = 'Agrupado';

        /* PEGA A INFORMACAO DO VEICULO */
        SELECT
            CASE
                WHEN LOCADO_PROPRIO = 'P' THEN 'Empresa'
                WHEN LOCADO_PROPRIO = 'A' THEN 'Locadora'
                WHEN LOCADO_PROPRIO = 'E' THEN 'Empresa'
                WHEN LOCADO_PROPRIO = 'L' THEN 'Agregado'
                ELSE '-'
            END
        FROM TB_VEICULO
        WHERE NUM_FRE = :VNUM_FRE
        INTO :RVEICULO;

        IF (RVEICULO IS NULL) THEN
            RVEICULO = '-';

        /* PERGUNTA FOI DEVOLVIDO */
        IF (VID_DEVOLVIDO > 0) THEN
            RDEVOLVIDO = 'S';
        ELSE
            RDEVOLVIDO = 'N';

        IF ((PDEVOLVIDO = 'N') OR ((PDEVOLVIDO = 'S') AND (RDEVOLVIDO = 'S'))) THEN
            SUSPEND;

        /* IMPRIME OS MESMOS DADOS COM AS EQUIPES AUXILIARES */
        IF (PEQUIPE = '-') THEN
        BEGIN
            FOR
                SELECT A.EQUIPE, B.NOME
                FROM TB_MOBILE_PARTICIPANTES_RDO A
                LEFT JOIN TB_EQUIPE B ON (A.EQUIPE = B.EQUIPE)
                WHERE A.CODCT = :RCODCT AND A.PROJETO = :RPROJETO
                  AND A.NUM_OBRA = :ROS AND A.NUM_DOC = :RCONTRATO
                INTO :REQUIPE, :RNOME
            DO
            BEGIN
                IF ((PDEVOLVIDO = 'N') OR ((PDEVOLVIDO = 'S') AND (RDEVOLVIDO = 'S'))) THEN
                    SUSPEND;
            END
        END
    END
END").ToString();
        }

    }
}
