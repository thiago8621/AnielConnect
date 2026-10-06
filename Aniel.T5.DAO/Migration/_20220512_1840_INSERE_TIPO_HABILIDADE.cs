using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Aniel.T5.DAO.Migration
{
    public class _20220512_1840_INSERE_TIPO_HABILIDADE : AnielMigration
    {

        public _20220512_1840_INSERE_TIPO_HABILIDADE()
        {
            Codigo = "20220512_1840";
            Descricao = "Registros Padroes para tipos de habilidade";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }
        public override string GetSql()
        {
            var Sql = new StringBuilder();
            Sql.AppendLine($@" EXECUTE BLOCK as declare x int = 0;
                                    BEGIN
                                        UPDATE OR INSERT INTO TB_TIPO_HABILIDADE_NATUREZA (VALOR, DESCRICAO) VALUES (0, 'Reparo');
                                        UPDATE OR INSERT INTO TB_TIPO_HABILIDADE_NATUREZA (VALOR, DESCRICAO) VALUES (1, 'Instalação');
                                        UPDATE OR INSERT INTO TB_TIPO_HABILIDADE_NATUREZA (VALOR, DESCRICAO) VALUES (2, 'Multiskill');
                                END

                            ");

            return Sql.ToString();
        }
    }
}
