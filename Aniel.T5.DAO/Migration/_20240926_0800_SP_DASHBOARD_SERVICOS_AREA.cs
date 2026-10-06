using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20240926_0800_SP_DASHBOARD_SERVICOS_AREA : AnielMigration
    {
        public _20240926_0800_SP_DASHBOARD_SERVICOS_AREA()
        {
            Codigo = "20240926_0800";
            Descricao = "Proc relatório Painel de Indicadores na DashBoard - Área";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"SET TERM ^ ;

create or alter procedure SP_DASHBOARD_SERVICOS_AREA (
    DATAINI date,
    DATAFIM date)
returns (
    COD_ESCRITORIO DM_COD_ESCRITORIO,
    ESCRITORIO varchar(70),
    COD_AREA DM_COD_AREA,
    DESCRICAO varchar(255),
    PRODUTIVO numeric(9,2),
    SLA numeric(9,2),
    AGE1 numeric(9,2),
    TEC1 numeric(9,2),
    QTD_REV3 numeric(9,2),
    QTD_REV1 numeric(9,2),
    QTD_RTI numeric(9,2))
as
declare variable VQTDE_OS_PERIODO float;
declare variable VQTDE_OS_BXD_PERIODO float;
declare variable VQTDE_OS_BXD_PROD float;
BEGIN
    FOR
            SELECT F.cod_escritorio, ESC.descricao, F.cod_area, F.DESCRICAO,

    CAST(COUNT(
            CASE
                WHEN D.STATUS_EXECUCAO = '2'
                THEN 1
                ELSE NULL
            END) AS DECIMAL(10, 2)) AS PRODUTIVO,

    CAST(COUNT(
            CASE
                WHEN D.STATUS_EXECUCAO IN (1, 2)
                    AND D.datahora_ini_atend IS NULL
                    AND D.datahora_fim_atend <= CAST(D.DATA_INDICADOR || ' ' || D.hora_indicador AS TIMESTAMP)
                THEN 1
                WHEN D.STATUS_EXECUCAO IN (1, 2)
                    AND D.datahora_ini_atend IS NOT NULL
                    AND D.datahora_ini_atend <= CAST(D.DATA_INDICADOR || ' ' || D.hora_indicador AS TIMESTAMP)
                THEN 1
                ELSE NULL
            END) AS DECIMAL(10, 2)) AS SLA,

    CAST(COUNT(
            CASE
                WHEN D.STATUS_EXECUCAO IN (1, 2)
                    AND D.datahora_ini_atend IS NULL
                    AND D.datahora_fim_atend <= AG.datahora_fim
                THEN 1
                WHEN
                    D.STATUS_EXECUCAO IN (1, 2)
                    AND D.datahora_ini_atend IS NOT NULL
                    AND D.datahora_ini_atend <= AG.datahora_fim
                THEN 1
                ELSE NULL
            END) AS DECIMAL(10, 2)) AS AGE1,

    CAST(COUNT(
            CASE
                WHEN D.STATUS_EXECUCAO = '2'
                    AND D.seq = '1'
                THEN 1
                ELSE NULL
            END) AS DECIMAL(10, 2)) AS TEC1,

    CAST(COUNT(
            CASE
                WHEN (
                    SELECT count(REINC2.num_doc||NP2.ID)
                    FROM tb_reincidencia_docprod REINC2
                    LEFT join tb_documento_producao D2
                        on (REINC2.codct_anterior = D2.codct AND reinc2.projeto_anterior = D2.projeto AND reinc2.num_doc_anterior = D2.NUM_DOC AND reinc2.num_obra_anterior = D2.NUM_OBRA)
                    LEFT Join tb_tipo_servico_equipe TS2
                        ON (TS2.codct = D2.CODCT AND TS2.cod_tipo_serv = D2.cod_tipo_serv)
                    LEFT JOIN tb_natureza_producao NP2
                        ON (NP2.id = TS2.cod_natureza)
                    WHERE REINC2.num_doc = D.num_doc
                      AND REINC2.data BETWEEN (D.dia - 30) AND D.dia
                      AND NP2.TIPO_HABILIDADE <> '1'
                    HAVING count(REINC2.num_doc||NP2.ID) >= 3) >= 1
                THEN 1
                ELSE NULL
            END) AS DECIMAL(10, 2)) AS QTD_REV3,

    CAST(COUNT(
            CASE
                WHEN (
                    SELECT COUNT(REINC3.num_obra)
                    FROM tb_reincidencia_docprod REINC3
                    LEFT join tb_documento_producao D3
                        on (REINC3.codct_anterior = D3.codct AND reinc3.projeto_anterior = D3.projeto AND reinc3.num_doc_anterior = D3.NUM_DOC AND reinc3.num_obra_anterior = D3.NUM_OBRA)
                    LEFT JOIN TB_TIPO_SERVICO_EQUIPE TS3
                        ON (TS3.codct = D3.codct
                        AND TS3.COD_TIPO_SERV = D3.COD_TIPO_SERV)
                    LEFT JOIN TB_NATUREZA_PRODUCAO NP3
                        ON (NP3.ID = TS3.COD_NATUREZA)
                    WHERE REINC3.codct = D.codct
                      AND REINC3.projeto = D.projeto
                      AND REINC3.num_doc = D.num_doc
                      AND REINC3.num_obra = D.num_obra
                      AND NP3.TIPO_HABILIDADE <> 1) >= 1
                THEN 1
                ELSE NULL
            END) AS DECIMAL(10, 2)) AS QTD_REV1,

    CAST(COUNT(
            CASE
                WHEN (
                    SELECT COUNT(REINC4.num_obra)
                    FROM tb_reincidencia_docprod REINC4
                    LEFT join tb_documento_producao D4
                        on (REINC4.codct_anterior = D4.codct AND reinc4.projeto_anterior = D4.projeto AND reinc4.num_doc_anterior = D4.NUM_DOC AND reinc4.num_obra_anterior = D4.NUM_OBRA)
                    LEFT JOIN TB_TIPO_SERVICO_EQUIPE TS4
                        ON (TS4.codct = D4.codct
                        AND TS4.COD_TIPO_SERV = D4.COD_TIPO_SERV)
                    LEFT JOIN TB_NATUREZA_PRODUCAO NP4
                        ON (NP4.ID = TS4.COD_NATUREZA)
                    WHERE REINC4.codct = D.codct
                      AND REINC4.projeto = D.projeto
                      AND REINC4.num_doc = D.num_doc
                      AND REINC4.num_obra = D.num_obra
                      AND NP4.TIPO_HABILIDADE = 1) >= 1
                THEN 1
                ELSE NULL
            END) AS DECIMAL(10, 2)) AS QTD_RTI

        FROM TB_DOCUMENTO_PRODUCAO D
        LEFT JOIN tb_agendamento_equipeaniel AG
            ON (AG.codct = D.codct
            AND ag.projeto = D.projeto
            AND ag.num_doc = D.num_doc
            AND ag.num_obra = D.num_obra)
        LEFT JOIN TB_TIPO_SERVICO_EQUIPE TS
            ON (TS.codct = D.codct
            AND TS.COD_TIPO_SERV = D.COD_TIPO_SERV)
        LEFT JOIN TB_NATUREZA_PRODUCAO NP
            ON (NP.ID = TS.COD_NATUREZA)
        LEFT JOIN TB_ESCRITORIO ESC
            ON (D.COD_ESCRITORIO = ESC.COD_ESCRITORIO)
        LEFT JOIN TB_AREA F
            ON (D.COD_ESCRITORIO = F.COD_ESCRITORIO
            AND D.COD_AREA = F.COD_AREA)
        LEFT JOIN TB_LOCALIDADE LOC
            ON (D.COD_ESCRITORIO = LOC.COD_ESCRITORIO
            AND D.COD_AREA = LOC.COD_AREA
            AND D.COD_LOCALIDADE = LOC.COD_LOCALIDADE)
        LEFT JOIN TB_EQUIPE EQ
            ON (EQ.NUM_FRE = D.EQUIPE)
        LEFT JOIN TB_REINCIDENCIA_DOCPROD REINC
            ON (REINC.codct = D.CODCT
            AND REINC.projeto = D.PROJETO
            AND REINC.num_doc = D.NUM_DOC
            AND REINC.num_obra = D.NUM_OBRA)
        WHERE D.DATA_MAXIMA BETWEEN :DATAINI AND :DATAFIM
        GROUP BY 
            F.cod_escritorio, ESC.descricao, F.cod_area, F.DESCRICAO
        INTO
            :cod_escritorio, :ESCRITORIO, :cod_area, :DESCRICAO, :PRODUTIVO, :SLA, :AGE1, :TEC1, :QTD_REV3, :QTD_REV1, :QTD_RTI
    DO
    begin
        
        SELECT FIRST 1 COUNT(*) FROM tb_documento_producao
        WHERE COD_ESCRITORIO = :cod_escritorio
            AND COD_AREA = :cod_area
            AND DATA_MAXIMA between :dataini AND :datafim
        into :vqtde_os_periodo;

        SELECT FIRST 1 COUNT(*) FROM tb_documento_producao
        WHERE COD_ESCRITORIO = :cod_escritorio
            AND COD_AREA = :cod_area
            AND DATA_MAXIMA between :dataini AND :datafim
            AND STATUS_EXECUCAO IN ('1','2')
        into :vqtde_os_bxd_periodo;

        SELECT FIRST 1 COUNT(*) FROM tb_documento_producao
        WHERE COD_ESCRITORIO = :cod_escritorio
            AND COD_AREA = :cod_area
            AND DATA_MAXIMA between :dataini AND :datafim
            AND STATUS_EXECUCAO = '2'
        into :vqtde_os_bxd_prod;

        PRODUTIVO = ROUND(((PRODUTIVO/NULLIF(vqtde_os_bxd_periodo,0))*100),2);
        SLA = ROUND(((SLA/NULLIF(vqtde_os_bxd_periodo,0))*100),2);
        AGE1 = ROUND(((AGE1/NULLIF(vqtde_os_bxd_periodo,0))*100),2);
        TEC1 = ROUND(((TEC1/NULLIF(vqtde_os_bxd_prod,0))*100),2);
        QTD_REV3 = ROUND(((QTD_REV3/NULLIF(vqtde_os_periodo,0))*100),2);
        QTD_REV1 = ROUND(((QTD_REV1/NULLIF(vqtde_os_periodo,0))*100),2);
        QTD_RTI = ROUND(((QTD_RTI/NULLIF(vqtde_os_periodo,0))*100),2);

        SUSPEND;

    END

END^

SET TERM ; ^

/* Following GRANT statetements are generated automatically */

GRANT SELECT ON TB_REINCIDENCIA_DOCPROD TO PROCEDURE SP_DASHBOARD_SERVICOS_AREA;
GRANT SELECT ON TB_DOCUMENTO_PRODUCAO TO PROCEDURE SP_DASHBOARD_SERVICOS_AREA;
GRANT SELECT ON TB_TIPO_SERVICO_EQUIPE TO PROCEDURE SP_DASHBOARD_SERVICOS_AREA;
GRANT SELECT ON TB_NATUREZA_PRODUCAO TO PROCEDURE SP_DASHBOARD_SERVICOS_AREA;
GRANT SELECT ON TB_AGENDAMENTO_EQUIPEANIEL TO PROCEDURE SP_DASHBOARD_SERVICOS_AREA;
GRANT SELECT ON TB_ESCRITORIO TO PROCEDURE SP_DASHBOARD_SERVICOS_AREA;
GRANT SELECT ON TB_AREA TO PROCEDURE SP_DASHBOARD_SERVICOS_AREA;
GRANT SELECT ON TB_LOCALIDADE TO PROCEDURE SP_DASHBOARD_SERVICOS_AREA;
GRANT SELECT ON TB_EQUIPE TO PROCEDURE SP_DASHBOARD_SERVICOS_AREA;

/* Existing privileges on this procedure */

GRANT EXECUTE ON PROCEDURE SP_DASHBOARD_SERVICOS_AREA TO SYSDBA;").ToString();
        }

    }
}
