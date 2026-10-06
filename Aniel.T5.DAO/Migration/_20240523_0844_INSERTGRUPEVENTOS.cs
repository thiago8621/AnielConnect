using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{

    public class _20240523_0844_INSERTGRUPEVENTOS : AnielMigration
    {
        public _20240523_0844_INSERTGRUPEVENTOS()
        {
            Codigo = "20240523_0844";
            Descricao = "InserirUpdate GrupoTiposGrupos_Eventos";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"

UPDATE OR INSERT INTO TB_GRUPO (ID, NOME)
VALUES ((SELECT COALESCE (MAX(ID)+1, 1) FROM TB_GRUPO), 'ENVIO_EMAILS');

UPDATE OR INSERT INTO TB_TIPO_EVENTOS (ID, NOME, PERIODICIDADE)
VALUES((SELECT COALESCE(MAX(ID)+1, 1) FROM TB_TIPO_EVENTOS), 'ENVIAR EMAIL LOG IMPORTACAO OS', 1);

UPDATE OR INSERT INTO TB_GRUPO_EVENTOS (DIA, HORA, USUARIO, ID_TIPO_EVENTO, ID_GRUPO)
VALUES(CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'ADMIN', (SELECT ID FROM TB_TIPO_EVENTOS WHERE NOME = 'ENVIAR EMAIL LOG IMPORTACAO OS'), (SELECT ID FROM TB_GRUPO WHERE NOME = 'ENVIO_EMAILS'));

").ToString();
        }
    }
}

