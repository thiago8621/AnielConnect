using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO
{
    public class _20201230_1033_EXEC_MIGRAR_FORMA_PAGTO_CLIENTE : AnielMigration
    {
        public _20201230_1033_EXEC_MIGRAR_FORMA_PAGTO_CLIENTE()
        {
            Codigo = "20201230_1033";
            Descricao = "Procedure de Migração Forma de Pagamento do Cliente";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"create or alter procedure SP_MIGRAR_FORMA_PAGTO_CLIENTE
            RETURNS(
                RSTATUS VARCHAR(100),
                RID_CLIENTE BIGINT,
                RID_FORMA_PAGAMENTO BIGINT,
                RID_BANCO BIGINT,
                RCONTA_CORRENTE VARCHAR(25),
                RAGENCIA VARCHAR(8),
                RDIGITO VARCHAR(1),
                RID_VENCIMENTO BIGINT,
                RDIA_VENCIMENTO VARCHAR(255),
                RFORMA_ENVIO INTEGER,
                RID_ENDERECO_COBRANCA BIGINT,
                REND_COBRANCA_IGUAL_PRINCIPAL SMALLINT,
                RUSUARIO VARCHAR(10)
            )
            as
            declare variable VCO INTEGER;
            declare variable VID_FORMA_PAGTO_CLIENTE BIGINT;
            begin

                FOR
                    SELECT A.ID, A.ID_FORMA_PAGAMENTO, A.ID_BANCO, A.CONTA_CORRENTE,
                            A.AGENCIA, A.DIGITO, A.ID_VENCIMENTO, A.DIA_VENCIMENTO,
                            A.FORMA_ENVIO, A.ID_ENDERECO_COBRANCA, A.END_COBRANCA_IGUAL_PRINCIPAL,
                            A.USUARIOCRIACAO
                    FROM TB_CLIENTE A
                    INTO :RID_CLIENTE, :RID_FORMA_PAGAMENTO, :RID_BANCO, :RCONTA_CORRENTE,
                        :RAGENCIA, :RDIGITO, :RID_VENCIMENTO, :RDIA_VENCIMENTO,
                        :RFORMA_ENVIO, :RID_ENDERECO_COBRANCA, :REND_COBRANCA_IGUAL_PRINCIPAL,
                        :RUSUARIO
                DO
                BEGIN

                    SELECT COUNT(ID)
                    FROM TB_FORMA_PAGAMENTO_CLIENTE B
                    WHERE B.ID_CLIENTE = :RID_CLIENTE
                    INTO :VCO;

                    if (:VCO = 1) then
                    BEGIN
                    RSTATUS = 'OK';
                    END
                    ELSE
                    BEGIN

                    SELECT GEN_ID(GN_FORMA_PAGAMENTO_CLIENTE,1) FROM RDB$DATABASE
                    INTO :VID_FORMA_PAGTO_CLIENTE;

                    insert into TB_FORMA_PAGAMENTO_CLIENTE (ID, USUARIOCRIACAO, DATAHORACRIACAO, ID_FORMA_PAGAMENTO, CONTA_CORRENTE, AGENCIA, DIGITO, FORMA_ENVIO,
                                                                DIA_VENCIMENTO, END_COBRANCA_IGUAL_PRINCIPAL, STATUS, ID_VENCIMENTO, ID_BANCO,
                                                                ID_CLIENTE, ID_ENDERECO_COBRANCA)
                        values (:VID_FORMA_PAGTO_CLIENTE, :RUSUARIO, CURRENT_TIMESTAMP, :RID_FORMA_PAGAMENTO,
                                :RCONTA_CORRENTE, :RAGENCIA, :RDIGITO, :RFORMA_ENVIO, :RDIA_VENCIMENTO, :REND_COBRANCA_IGUAL_PRINCIPAL, 1,
                                :RID_VENCIMENTO, :RID_BANCO, :RID_CLIENTE, :RID_ENDERECO_COBRANCA);

                    UPDATE TB_CLIENTE_PLANO_PRODUTO C
                    SET C.ID_FORMA_PAGTO_CLIENTE = :VID_FORMA_PAGTO_CLIENTE
                    WHERE C.ID_CLIENTE = :RID_CLIENTE;

                    RSTATUS = 'FORMA_PAGAMENTO_CLIENTE ' || :VID_FORMA_PAGTO_CLIENTE || ' CRIADO';
                    END

                    suspend;
                END
            end").ToString();
        }

    }
}
