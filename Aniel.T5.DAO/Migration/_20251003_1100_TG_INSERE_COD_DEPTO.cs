using System.Text;
using Sinapse.DAO;

namespace Aniel.T5.DAO
{
    public class _20251003_1100_TG_INSERE_COD_DEPTO : AnielMigration
    {
        public _20251003_1100_TG_INSERE_COD_DEPTO()
        {
            Codigo = "20251003_1100";
            Descricao = "Trigger para adicionar um Código de Departamento em uma OS na sua criação";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE OR ALTER TRIGGER TG_INSERE_COD_DEPTO FOR TB_DOCUMENTO_PRODUCAO
                                                    ACTIVE BEFORE INSERT POSITION 0
                                                    AS
                                                    declare variable VCOD_NATUREZA SMALLINT;
                                                    declare variable VID_DEPARTAMENTO_PADRAO SMALLINT;
                                                    begin
                                                        if (COALESCE(NEW.COD_DEPTO,'') = '') then
                                                        BEGIN
                                                            SELECT A.COD_NATUREZA FROM TB_TIPO_SERVICO_EQUIPE A
                                                            WHERE A.COD_TIPO_SERV = NEW.COD_TIPO_SERV
                                                            INTO :VCOD_NATUREZA;

                                                            SELECT FIRST 1 B.ID_DEPARTAMENTO_PADRAO
                                                            FROM TB_NATUREZA_PRODUCAO B
                                                            WHERE B.ID = :VCOD_NATUREZA
                                                            INTO :VID_DEPARTAMENTO_PADRAO;

                                                            NEW.COD_DEPTO = :VID_DEPARTAMENTO_PADRAO;
                                                        END
                                                    END")
            .ToString();
        }

    }
}
