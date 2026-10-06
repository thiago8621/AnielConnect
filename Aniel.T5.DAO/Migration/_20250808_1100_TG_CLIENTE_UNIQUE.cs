using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO
{
    public class _20250808_1100_TG_CLIENTE_UNIQUE : AnielMigration
    {
        public _20250808_1100_TG_CLIENTE_UNIQUE()
        {
            Codigo = "20250808_1100";
            Descricao = "Trigger para não duplicar CNPJ do Cliente";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"EXECUTE BLOCK
                                                    AS
                                                    BEGIN
                                                      -- Cria a exceção se ela não existir
                                                      IF (NOT EXISTS (SELECT 1 FROM RDB$EXCEPTIONS WHERE RDB$EXCEPTION_NAME = 'EX_VERIF_CLIENTE_DUPLICADO')) THEN
                                                      BEGIN
                                                        EXECUTE STATEMENT 'CREATE EXCEPTION EX_VERIF_CLIENTE_DUPLICADO ''Já existe um cliente cadastrado com este CNPJ.''';
                                                      END

                                                      -- Cria ou altera o trigger
                                                      EXECUTE STATEMENT '
                                                        CREATE OR ALTER TRIGGER TG_CLIENTE_UNIQUE FOR TB_CLIENTE
                                                        ACTIVE BEFORE INSERT POSITION 0
                                                        AS
                                                        BEGIN
                                                          IF (
                                                            NEW.CNPJ IS NOT NULL AND
                                                            TRIM(NEW.CNPJ) <> '''' AND
                                                            EXISTS (SELECT 1 FROM TB_CLIENTE WHERE CNPJ = NEW.CNPJ)
                                                          ) THEN
                                                          BEGIN
                                                            EXCEPTION EX_VERIF_CLIENTE_DUPLICADO;
                                                          END
                                                        END
                                                      ';
                                                    END").ToString();
        }

    }
}
