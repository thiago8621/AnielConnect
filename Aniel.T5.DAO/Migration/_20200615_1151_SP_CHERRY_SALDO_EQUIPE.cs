using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO
{
    public class _20200610_1651_SP_CHERRY_SALDO_EQUIPE : AnielMigration
    {
        public _20200610_1651_SP_CHERRY_SALDO_EQUIPE()
        {
            Codigo = "20200615_1151";
            Descricao = "Procedure de Extração de Saldo Equipe";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"create or alter procedure SP_CHERRY_SALDO_EQUIPE (
    CODCT varchar(4))
returns (
    RTIPOSALDO varchar(10),
    RCODCT varchar(4),
    RDESCCT varchar(30),
    RPROJETO varchar(20),
    REQUIPE varchar(10),
    REQUIPEAUX varchar(10),
    RNOME varchar(50),
    RCODPERFIL smallint,
    RDESCPERFIL varchar(30),
    RCODMAT varchar(12),
    RCODAUX varchar(30),
    RDESCMAT varchar(55),
    RUNID varchar(5),
    RCLASSIF char(1),
    RVALOR numeric(13,4),
    RFAMILIA varchar(5),
    RDESCFAMILIA varchar(30),
    REMPRESA varchar(30),
    RFILIAL varchar(40),
    RSUBCOD varchar(1),
    RSUBCOD_EXCLUSIVO varchar(1),
    RSALDO numeric(9,2))
as
BEGIN

    FOR
        SELECT B.TIPOSALDOAPLIC, A.CODCT, B.DESCRICAO, A.PROJETO, A.EQUIPE, E.EQUIPEAUX, E.NOME,
               F.COD_PERFIL_EQUIPE, F.NOME, A.CODMAT, A.CODAUX,
               A.MATERIAL, A.UNID, A.CLASSIFICACAO, A.VALOR,
               A.SUBCOD, A.SUBCOD_EXCLUSIVO, A.FAMILIA, G.DESCRICAO, D.EMPRESA, D.RAZAO_SOCIAL,
               SUM(A.VALOR) VALOR
          FROM(
               SELECT A.CODCT, A.PROJETO, A.CODMAT, B.CODAUX,
                      B.DESCRICAO AS MATERIAL, B.UNID, B.CLASSIFICACAO, B.VALOR,
                      B.SUBCOD, B.SUBCOD_EXCLUSIVO, B.FAMILIA, A.EQUIPE,
                      ((SUM(A.SAIDA) - SUM(A.ENTRADA)) * B.VALOR) AS SALDO
                 FROM TB_ESTOQUE A
                INNER JOIN TB_MATERIAL B ON(A.CODMAT = B.CODMAT AND B.CLASSIFICACAO = 'I')
                WHERE A.CODCT = :CODCT AND A.ID_INV_EQUIPE IS NULL
                  AND A.EQUIPE IS NOT NULL AND A.EQUIPE <> '-' AND A.EQUIPE <> ''
                GROUP BY A.CODCT, A.PROJETO, A.CODMAT, B.CODAUX,
                         B.DESCRICAO, B.UNID, B.CLASSIFICACAO, B.VALOR,
                         B.SUBCOD, B.SUBCOD_EXCLUSIVO, B.FAMILIA, A.EQUIPE
               HAVING (SUM(A.ENTRADA) - SUM(A.SAIDA)) <> 0
            
                UNION    
            
               SELECT A.CODCT, A.PROJETO, A.CODMAT, B.CODAUX,
                      B.DESCRICAO AS MATERIAL, B.UNID, B.CLASSIFICACAO, B.VALOR,
                      B.SUBCOD, B.SUBCOD_EXCLUSIVO, B.FAMILIA, A.EQUIPE,
                       ((SUM(A.QUANT_REMOV) - SUM(A.QUANTIDADE)) * B.VALOR) AS SALDO
                 FROM TB_MATERIAL_TAREFA A
                INNER JOIN TB_MATERIAL B ON(A.CODMAT = B.CODMAT AND B.CLASSIFICACAO = 'I')
                WHERE A.CODCT = :CODCT AND A.ID_INV_EQUIPE IS NULL
                GROUP BY A.CODCT, A.PROJETO, A.CODMAT, B.CODAUX,
                         B.DESCRICAO, B.UNID, B.CLASSIFICACAO, B.VALOR,
                         B.SUBCOD, B.SUBCOD_EXCLUSIVO, B.FAMILIA, A.EQUIPE
               HAVING (SUM(A.QUANT_REMOV) - SUM(A.QUANTIDADE)) <> 0) A
            
            LEFT JOIN TB_CONTRATO B ON(A.CODCT = B.CODCT)    
            LEFT JOIN TB_FILIAL C ON(B.CODEMPRESA = C.CODEMPRESA AND B.CODFILIAL = C.CODFILIAL)    
            LEFT JOIN TB_EMPRESA D ON(B.CODEMPRESA = D.CODEMPRESA)    
            LEFT JOIN TB_EQUIPE E ON (A.EQUIPE = E.EQUIPE)
            LEFT JOIN TB_PERFIL_EQUIPE F ON (E.COD_PERFIL_EQUIPE = F.COD_PERFIL_EQUIPE)
            LEFT JOIN TB_FAMILIA_MATERIAL G ON (A.FAMILIA = G.FAMILIA)
            GROUP BY B.TIPOSALDOAPLIC, A.CODCT, B.DESCRICAO, A.PROJETO, A.EQUIPE, E.EQUIPEAUX, E.NOME,
               F.COD_PERFIL_EQUIPE, F.NOME, A.CODMAT, A.CODAUX,
               A.MATERIAL, A.UNID, A.CLASSIFICACAO, A.VALOR,
               A.SUBCOD, A.SUBCOD_EXCLUSIVO, A.FAMILIA, G.DESCRICAO, D.EMPRESA, D.RAZAO_SOCIAL

        INTO :RTIPOSALDO, :RCODCT, :RDESCCT, :RPROJETO, :REQUIPE, :REQUIPEAUX, :RNOME,
            :RCODPERFIL, :RDESCPERFIL, :RCODMAT, :RCODAUX,
            :RDESCMAT, :RUNID, :RCLASSIF, :RVALOR,
            :RSUBCOD, :RSUBCOD_EXCLUSIVO, :RFAMILIA, :RDESCFAMILIA, :REMPRESA, :RFILIAL,
            :RSALDO
    DO
      BEGIN
        SUSPEND;
      END

END").ToString();
        }

    }
}
