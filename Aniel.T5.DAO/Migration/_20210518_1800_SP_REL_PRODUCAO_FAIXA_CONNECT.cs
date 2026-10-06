using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20210518_1800_SP_REL_PRODUCAO_FAIXA_CONNECT : AnielMigration
    {
        public _20210518_1800_SP_REL_PRODUCAO_FAIXA_CONNECT()
        {
            Codigo = "20210518_1800";
            Descricao = "Proc relatório de produção analitico";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"create or alter procedure SP_REL_PRODUCAO_FAIXA_CONNECT (
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
    RCOD_SUPERV varchar(6),
    RSUPERVISOR varchar(50),
    RDATA date,
    RCONTRATO integer,
    ROS varchar(50),
    RMANIFESTO varchar(15),
    RSENHA_BX varchar(100),
    RBAIRRO varchar(30),
    RTIPO_IMOVEL varchar(50),
    RCOD_TIPO_SERV smallint,
    RDESC_TIPO_SERV varchar(150),
    RSEGMENTO varchar(15),
    RUNIDADE varchar(5),
    RCOD_ENCERRAMENTO varchar(6),
    RDESCRICAO_ENCERRAMENTO varchar(95),
    RDEVOLVIDO char(1),
    RCERTIFICADO char(1),
    RVEICULO varchar(20),
    RCODIGO varchar(20),
    RDESC_SERV varchar(500),
    RCOD_PRECO varchar(5),
    RUNIDADE_CUSTO DM_UND_CUSTO,
    RPONTO numeric(13,6),
    RVALOR_UNIDADE_CUSTO numeric(13,5),
    RVALOR_PROD numeric(13,5),
    RACUM_PONTO numeric(13,6),
    RID_FAIXA integer,
    RDESC_FAIXA varchar(100),
    RINI_FAIXA numeric(10,3),
    RFIM_FAIXA numeric(10,3),
    RCOEFICIENTE numeric(10,3),
    RSUBTOTAL numeric(14,5))
as
declare variable VCALC_PROD_APOS_FAIXA char(1);
declare variable VMESMA_FAIXA_TODOS_SERV char(1);
declare variable VID_FAIXA integer;
declare variable VDESC_FAIXA varchar(100);
declare variable VINI_FAIXA numeric(10,3);
declare variable VFIM_FAIXA numeric(10,3);
declare variable VCOEFICIENTE numeric(10,3);
declare variable VTOT_PONTO numeric(13,6);
declare variable VEQUIPE_LOOP varchar(10);
declare variable VEQUIPE_AUX varchar(10);
declare variable VNOME_AUX varchar(50);
declare variable VCONTRATO_ID_CONFIG_FAIXA integer;
BEGIN
    VEQUIPE_LOOP = '';

    RACUM_PONTO = 0;

    /* PEGA OS PARAMETROS DE CONFIGURACAO */
    SELECT COALESCE(CALCULAR_VALOR_PROD_APOS_FAIXA, 'N'),
        COALESCE(ATRIBUIR_MESMO_VALOR_FAIXA_TS, 'N'),
        ID_CONFIG_FAIXA
    FROM TB_CONTRATO
    WHERE CODCT = :PCODCT
    INTO :VCALC_PROD_APOS_FAIXA, :VMESMA_FAIXA_TODOS_SERV, :VCONTRATO_ID_CONFIG_FAIXA;

    /* A CONSULTA DAS PRODUCOES POR EQUIPE */
    FOR
        SELECT RCODCT, RPROJETO, REQUIPE, RNOME, RCOD_SUPERV, RSUPERVISOR, RDATA, RCONTRATO, ROS,
            RMANIFESTO, RSENHA_BX, RBAIRRO, RTIPO_IMOVEL, RCOD_TIPO_SERV, RDESC_TIPO_SERV, RSEGMENTO, RUNIDADE,
            RCOD_ENCERRAMENTO, RDESCRICAO_ENCERRAMENTO, RDEVOLVIDO,
            RCERTIFICADO, RVEICULO, RCODIGO, RUNIDADE_CUSTO, RDESC_SERV, RCOD_PRECO, RPONTO, RVALOR_PROD,
            REQUIPE_AUX, RNOME_AUX, RVALOR_UNIDADE_CUSTO
        FROM SP_LISTA_PRODUCAO_FAIXA_CONNE(:PCODCT, :PPROJETO, :PEQUIPE, :PCOD_SUPERV, :PCOD_TIPO_SERV, :PCOD_NATUREZA, :PDATAINI, :PDATAFIM,:PTIPODATA,
            :PCERTIFICADO, :PDEVOLVIDO, :PDATACRIACAO, :PQUANT_PREV, :PBAIXAMOBILE)
        ORDER BY RCODCT, RPROJETO, REQUIPE, RDATA
        INTO :RCODCT, :RPROJETO, :REQUIPE, :RNOME, :RCOD_SUPERV, :RSUPERVISOR, :RDATA, :RCONTRATO, :ROS,
            :RMANIFESTO, :RSENHA_BX, :RBAIRRO, :RTIPO_IMOVEL, :RCOD_TIPO_SERV, :RDESC_TIPO_SERV, :RSEGMENTO, :RUNIDADE,
            :RCOD_ENCERRAMENTO, :RDESCRICAO_ENCERRAMENTO, :RDEVOLVIDO,
            :RCERTIFICADO, :RVEICULO, :RCODIGO, :RUNIDADE_CUSTO, :RDESC_SERV, :RCOD_PRECO, :RPONTO, :RVALOR_PROD,
            :VEQUIPE_AUX, :VNOME_AUX, :RVALOR_UNIDADE_CUSTO
    DO
    BEGIN
        /* CASO A EQUIPE SEJA AUXILIAR */
        IF ((PEQUIPE <> '-') AND (PEQUIPE <> REQUIPE)) THEN
        BEGIN
            REQUIPE = :VEQUIPE_AUX;
            RNOME = :VNOME_AUX;
        END

        /* ENTRA AQUI CASO SEJA O PRIMEIRO REGISTRO DA EQUIPE */
        /* CALCULA DESDE O INICIO E ATRIBUI A ULTIMA FAIXA */
        IF ((VCALC_PROD_APOS_FAIXA = 'N' OR VMESMA_FAIXA_TODOS_SERV = 'S') AND
            (VEQUIPE_LOOP <> :REQUIPE)) THEN
        BEGIN
            SELECT COALESCE(SUM(RPONTO), 0) FROM SP_LISTA_PRODUCAO_FAIXA_CONNE(
                        :PCODCT, :PPROJETO, :REQUIPE, :PCOD_SUPERV, :PCOD_TIPO_SERV, :PCOD_NATUREZA, :PDATAINI, :PDATAFIM,:PTIPODATA,
                        :PCERTIFICADO, :PDEVOLVIDO, :PDATACRIACAO, :PQUANT_PREV,
                        :PBAIXAMOBILE)
            INTO :VTOT_PONTO;

            /* PEGA A ULTIMA FAIXA ALCANCADA PELA EQUIPE */
            SELECT FIRST 1 B.ID, B.DESCRICAO, A.FAIXA_INICIAL, A.FAIXA_FINAL, A.COEFICIENTE
            FROM TB_FAIXA_PRODUCAO_ITEM A
            INNER JOIN TB_FAIXA_PRODUCAO B ON (A.ID_CONFIG_FAIXA = B.ID
                                           AND B.ID = (SELECT COALESCE(ID_CONFIG_FAIXA, :VCONTRATO_ID_CONFIG_FAIXA)
                                                       FROM TB_EQUIPE Z
                                                       WHERE Z.EQUIPE = :REQUIPE))
            WHERE CAST(:VTOT_PONTO AS NUMERIC(10,6)) BETWEEN A.FAIXA_INICIAL AND A.FAIXA_FINAL
            INTO :VID_FAIXA, :VDESC_FAIXA, :VINI_FAIXA, :VFIM_FAIXA, :VCOEFICIENTE;
        END

        /* MUDOU A EQUIPE */
        IF (VEQUIPE_LOOP <> :REQUIPE) THEN
        BEGIN
            RACUM_PONTO = 0;
            VEQUIPE_LOOP = :REQUIPE;
        END

        /* ACUMULA O PONTO DE CADA ITEM */
        RACUM_PONTO = :RACUM_PONTO + :RPONTO;

        /* CALCULA ANTES DE ALCANCAR A FAIXA E ATRIBUI A ULTIMA FAIXA ALCANCADA */
        IF ((VCALC_PROD_APOS_FAIXA = 'N') AND (VMESMA_FAIXA_TODOS_SERV = 'S')) THEN
        BEGIN
            RID_FAIXA = :VID_FAIXA;
            RDESC_FAIXA = :VDESC_FAIXA;
            RINI_FAIXA = :VINI_FAIXA;
            RFIM_FAIXA = :VFIM_FAIXA;
            RCOEFICIENTE = :VCOEFICIENTE;

            IF (COALESCE(VDESC_FAIXA, '') = '') THEN
            BEGIN
                RSUBTOTAL = 0;
                --RVALOR_PROD = 0;
            END
            ELSE
            BEGIN
                RSUBTOTAL = RVALOR_PROD * RCOEFICIENTE;
            END
        END
        ELSE
        BEGIN
            RID_FAIXA = NULL; RDESC_FAIXA = NULL; RINI_FAIXA = NULL; RFIM_FAIXA = NULL; RCOEFICIENTE = NULL;

            /* CONSULTA A FAIXA ATUAL */
            SELECT FIRST 1 B.ID, B.DESCRICAO, A.FAIXA_INICIAL, A.FAIXA_FINAL, A.COEFICIENTE
            FROM TB_FAIXA_PRODUCAO_ITEM A
            INNER JOIN TB_FAIXA_PRODUCAO B ON (A.ID_CONFIG_FAIXA = B.ID
                                           AND B.ID = (SELECT COALESCE(ID_CONFIG_FAIXA, :VCONTRATO_ID_CONFIG_FAIXA)
                                                       FROM TB_EQUIPE Z
                                                       WHERE Z.EQUIPE = :REQUIPE))
            WHERE CAST(:RACUM_PONTO AS NUMERIC(13,6)) BETWEEN A.FAIXA_INICIAL AND A.FAIXA_FINAL
            INTO :RID_FAIXA, :RDESC_FAIXA, :RINI_FAIXA, :RFIM_FAIXA, :RCOEFICIENTE;

            /* CALCULA APOS ALCANCAR A FAIXA */
            IF (VCALC_PROD_APOS_FAIXA = 'S') THEN
            BEGIN
                IF (COALESCE(RDESC_FAIXA, '') = '') THEN
                BEGIN
                    /* NAO SE ENQUADROU EM NENHUMA FAIXA */

                  --RVALOR_PROD = 0;
                  RSUBTOTAL = 0;
                END
                ELSE IF ((VMESMA_FAIXA_TODOS_SERV = 'S') AND (COALESCE(VDESC_FAIXA, '') <> '')) THEN
                BEGIN
                  RID_FAIXA = :VID_FAIXA;
                  RDESC_FAIXA = :VDESC_FAIXA;
                  RINI_FAIXA = :VINI_FAIXA;
                  RFIM_FAIXA = :VFIM_FAIXA;
                  RCOEFICIENTE = :VCOEFICIENTE;
                  RSUBTOTAL = RVALOR_PROD * RCOEFICIENTE; /* SE ENQUADROU EM UMA FAIXA E ATRIBUI ELA */
                END

            END
            ELSE
            BEGIN
                /* SE AINDA NAO ALCANCOU NENHUM FAIXA MAS IRA ALCANCAR */
                IF ((COALESCE(RDESC_FAIXA, '') = '') AND (COALESCE(VDESC_FAIXA, '') <> '')) THEN
                BEGIN
                    RID_FAIXA = NULL; RDESC_FAIXA = NULL; RINI_FAIXA = NULL; RFIM_FAIXA = NULL; RCOEFICIENTE = NULL;

                    /* CONSULTA A FAIXA QUE SERA ALCANCADA */
                    SELECT FIRST 1 B.ID, B.DESCRICAO, A.FAIXA_INICIAL, A.FAIXA_FINAL, A.COEFICIENTE
                    FROM TB_FAIXA_PRODUCAO_ITEM A
                    INNER JOIN TB_FAIXA_PRODUCAO B ON (A.ID_CONFIG_FAIXA = B.ID
                                                   AND B.ID = (SELECT COALESCE(ID_CONFIG_FAIXA, :VCONTRATO_ID_CONFIG_FAIXA)
                                                               FROM TB_EQUIPE Z
                                                               WHERE Z.EQUIPE = :REQUIPE))
                    WHERE CAST(:RACUM_PONTO AS NUMERIC(13,6)) < A.FAIXA_INICIAL
                    ORDER BY A.FAIXA_INICIAL
                    INTO :RID_FAIXA, :RDESC_FAIXA, :RINI_FAIXA, :RFIM_FAIXA, :RCOEFICIENTE;
                END

                IF ((VMESMA_FAIXA_TODOS_SERV = 'S') AND (COALESCE(VDESC_FAIXA, '') <> '')) THEN
                BEGIN
                    /* MESMA FAIXA PARA TODOS PREENCHE COM A ULTIMA FAIXA */
                    RID_FAIXA = :VID_FAIXA;
                    RDESC_FAIXA = :VDESC_FAIXA;
                    RINI_FAIXA = :VINI_FAIXA;
                    RFIM_FAIXA = :VFIM_FAIXA;
                    RCOEFICIENTE = :VCOEFICIENTE;

                    RSUBTOTAL = RVALOR_PROD * VCOEFICIENTE;
                END
                ELSE IF (COALESCE(VDESC_FAIXA, '') <> '') THEN
                    RSUBTOTAL = RVALOR_PROD * RCOEFICIENTE; /* VAI ALCANCAR UMA FAIXA UTILIZA A PROXIMA FAIXA */
                ELSE
                BEGIN
                    /* NAO SE ENQUADROU E NEM VAI SE ENQUADRAR EM NENHUMA FAIXA */

                    --RVALOR_PROD = 0;
                    RSUBTOTAL = 0;
                END
            END
        END
        SUSPEND;
    END
END").ToString();
        }

    }
}
