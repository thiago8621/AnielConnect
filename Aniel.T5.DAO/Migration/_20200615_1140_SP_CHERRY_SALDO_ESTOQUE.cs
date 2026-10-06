using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO
{
    public class _20200610_1640_SP_CHERRY_SALDO_ESTOQUE : AnielMigration
    {
        public _20200610_1640_SP_CHERRY_SALDO_ESTOQUE()
        {
            Codigo = "20200615_1140";
            Descricao = "Procedure de Extração de Saldo Estoque";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"create or alter procedure SP_CHERRY_SALDO_ESTOQUE
returns (
    RTIPOSALDO varchar(10),
    RCODCT varchar(4),
    RDESCCT varchar(30),
    RPROJETO varchar(20),
    RCODMAT varchar(12),
    RCODAUX varchar(30),
    RDESCMAT varchar(55),
    RUNID varchar(5),
    RCLASSIF char(1),
    RTIPO char(1),
    RVALOR numeric(13,4),
    RTIPOFORNEC varchar(1),
    RFAMILIA varchar(5),
    RDESCFAMILIA varchar(30),
    RGRUPO smallint,
    RDESCGRUPO varchar(30),
    RCODCPL varchar(100),
    RCODCCUSTO integer,
    RCODCCUSTOAUX varchar(25),
    RDESCCCUSTO varchar(30),
    REMPRESA varchar(30),
    RFILIAL varchar(40),
    RSUBCOD varchar(1),
    RSUBCOD_EXCLUSIVO varchar(1),
    RSALDO numeric(10,2))
as
BEGIN
    FOR
        SELECT A.RTIPOSALDO, A.RCODCT, D.DESCRICAO, A.RPROJETO,
               B.RCOD_CCUSTO, B.RCOD_AUX, C.DESCRICAO, F.EMPRESA, G.NOME
          FROM SP_LISTA_TIPOSALDO_ESTOQUE('-','-') A
         INNER JOIN TB_CONTRATO D ON (A.RCODCT = D.CODCT AND D.TERMREAL IS NULL)
          LEFT JOIN TB_PROJETO E ON (A.RCODCT = E.CODCT AND A.rprojeto = E.PROJETO AND
               ((D.TIPOSALDO = 'PROJETO' AND E.SITUACAO = 'A' AND E.TERMREAL IS NULL) OR
                (D.TIPOSALDO = 'CONTRATO')))
          LEFT JOIN SP_BUSCAR_CENTRO_CUSTO_PADRAO (A.RCODCT, A.RPROJETO, '-') B
            ON (A.RCODCT = B.RCODCT AND A.RPROJETO = B.RPROJETO)
          LEFT JOIN TB_CENTRO_CUSTO C ON (B.RCOD_CCUSTO = C.COD_CCUSTO)
          LEFT JOIN TB_EMPRESA F ON ((CASE WHEN D.TIPOSALDO = 'PROJETO' THEN E.CODEMPRESA ELSE D.CODEMPRESA END) = F.CODEMPRESA)
          LEFT JOIN TB_FILIAL G ON (((CASE WHEN D.TIPOSALDO = 'PROJETO' THEN E.CODEMPRESA ELSE D.CODEMPRESA END) = G.CODEMPRESA) AND
                                    ((CASE WHEN D.TIPOSALDO = 'PROJETO' THEN E.CODFILIAL ELSE D.CODFILIAL END) = G.CODFILIAL))
          INTO :RTIPOSALDO, :RCODCT, :RDESCCT, :RPROJETO, :RCODCCUSTO,
               :RCODCCUSTOAUX, :RDESCCCUSTO, :REMPRESA, :RFILIAL
    DO
    BEGIN
        FOR
            SELECT A.RCODMAT, B.CODMAT_INTEGRA, B.DESCRICAO, B.UNID, B.VALOR,
                B.CLASSIFICACAO, B.TIPO, E.TIPO_FORNEC, B.FAMILIA, C.DESCRICAO,
                B.COD_SUBGRUPO_MAT, D.DESCRICAO, '-' AS CODCPL, B.SUBCOD, B.SUBCOD_EXCLUSIVO, SUM(A.RSALDO)
            FROM SP_CALCULA_SALDO_ESTOQUE(:RTIPOSALDO, :RCODCT, :RPROJETO,
                '-', '-', '[ALL]') A
            INNER JOIN TB_MATERIAL B ON (A.RCODMAT = B.CODMAT AND B.CLASSIFICACAO = 'I')
            LEFT JOIN TB_FAMILIA_MATERIAL C ON (B.FAMILIA = C.FAMILIA)
            LEFT JOIN TB_SUBGRUPO_MATERIAL D ON (B.COD_SUBGRUPO_MAT = D.COD_SUBGRUPO_MAT)
            LEFT JOIN TB_TIPO_FORNECIMENTO_MATERIAL E ON (E.CODCT = :RCODCT AND A.RCODMAT = E.CODMAT)
            WHERE A.RSALDO > 0
            GROUP BY A.RCODMAT, B.CODMAT_INTEGRA, B.DESCRICAO, B.UNID, B.VALOR,
                B.CLASSIFICACAO, B.TIPO, E.TIPO_FORNEC, B.FAMILIA, C.DESCRICAO,
                B.COD_SUBGRUPO_MAT, D.DESCRICAO, B.SUBCOD, B.SUBCOD_EXCLUSIVO
            INTO :RCODMAT, :RCODAUX, :RDESCMAT, :RUNID, :RVALOR, :RCLASSIF,
                :RTIPO, :RTIPOFORNEC, :RFAMILIA, :RDESCFAMILIA, :RGRUPO,
                :RDESCGRUPO, :RCODCPL, :RSUBCOD, :RSUBCOD_EXCLUSIVO, :RSALDO
        DO
            SUSPEND;
    END

END").ToString();
        }

    }
}
