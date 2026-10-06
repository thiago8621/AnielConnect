using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{
    public class _20231027_0810_TB_HORARIO_CONNECT : AnielMigration
    {
        public _20231027_0810_TB_HORARIO_CONNECT()
        {
            Codigo = "20231027_0810";
            Descricao = "Adiciona coluna de Hora extra em TB_HORARIO_CONNECT";
            Tipo = TipoAnielMigration.ScriptUnico;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"ALTER TABLE TB_HORARIO_CONNECT 
                                                    ADD QTDE_HE_FERIADO TIMESTAMP,
                                                    ADD QTDE_HE_DOMINGO TIMESTAMP,
                                                    ADD QTDE_HE_SEGUNDA TIMESTAMP,
                                                    ADD QTDE_HE_TERCA TIMESTAMP,
                                                    ADD QTDE_HE_QUARTA TIMESTAMP,
                                                    ADD QTDE_HE_QUINTA TIMESTAMP,
                                                    ADD QTDE_HE_SEXTA TIMESTAMP,
                                                    ADD QTDE_HE_SABADO TIMESTAMP;
                                                    ").ToString();
        }
    }
}
