using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20201027_1720_SP_CONNECT_TOTAL_PONTOS : AnielMigration
    {
        public _20201027_1720_SP_CONNECT_TOTAL_PONTOS()
        {
            Codigo = "20201027_1720";
            Descricao = "Proc utilizada no acompanhamento de equipes";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"create or alter procedure SP_CONNECT_TOTAL_PONTOS(
    CODCT VARCHAR(4),
    PROJETO VARCHAR(20),
    NUM_OBRA VARCHAR(50),
    NUM_DOC INTEGER)
RETURNS(
    RCODCT VARCHAR(4),
    RPROJETO VARCHAR(20),
    RNUM_OBRA VARCHAR(50),
    RNUM_DOC INTEGER,
    RPONTOS_PREVISTOS NUMERIC(9,2),
    RPONTOS_PRODUZIDOS NUMERIC(9,2)
)
as
begin

  RCODCT = CODCT;
  RPROJETO = PROJETO;
  RNUM_OBRA = NUM_OBRA;
  RNUM_DOC = NUM_DOC;

  SELECT SUM(A.QUANT_PROD * B.QUANTIDADE), SUM(A.QUANT_CATAL * B.QUANTIDADE) FROM TB_CUSTOS_TAREFA A
    LEFT JOIN TB_CADERNO_ENCARGOS B
      ON(A.CODIGO = B.CODIGO AND A.UNIDADE_CUSTO = B.UNIDADE_CUSTO)
   WHERE A.CODCT = :CODCT AND A.PROJETO = :PROJETO AND A.NUM_OBRA = :NUM_OBRA AND A.NUM_DOC = :NUM_DOC
    INTO :RPONTOS_PRODUZIDOS, :RPONTOS_PREVISTOS;

  if (:RPONTOS_PRODUZIDOS IS NULL) then
    RPONTOS_PRODUZIDOS = 0;
  if (:RPONTOS_PREVISTOS IS NULL) then
    RPONTOS_PREVISTOS = 0;

  suspend;
end").ToString();
        }

    }
}
