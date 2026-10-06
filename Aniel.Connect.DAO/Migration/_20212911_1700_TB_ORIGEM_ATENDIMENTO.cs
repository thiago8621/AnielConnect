using Sinapse.DAO;
using System.Text;

namespace Aniel.Connect.DAO
{
    public class _20212911_1700_TB_ORIGEM_ATENDIMENTO : AnielMigration
    {
        public _20212911_1700_TB_ORIGEM_ATENDIMENTO()
        {
            Codigo = "20212911_1700";
            Descricao = "Origens de Atendimento Padrão";
            Tipo = TipoAnielMigration.ProcedureTrigger;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"EXECUTE BLOCK as declare x int = 0;
                BEGIN

                

                UPDATE OR INSERT INTO TB_ORIGEM_ATENDIMENTO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CODIGO, DESCRICAO, STATUS)
                                     VALUES (-1, NULL, NULL, NULL, NULL, NULL, '0', 'Email', 1)
                                   MATCHING (ID);
UPDATE OR INSERT INTO TB_ORIGEM_ATENDIMENTO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CODIGO, DESCRICAO, STATUS)
                                     VALUES (-2, NULL, NULL, NULL, NULL, NULL, '0', 'Chat', 1)
                                   MATCHING (ID);
UPDATE OR INSERT INTO TB_ORIGEM_ATENDIMENTO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CODIGO, DESCRICAO, STATUS)
                                     VALUES (-3, NULL, NULL, NULL, NULL, NULL, '0', 'Whatsapp', 1)
                                   MATCHING (ID);
UPDATE OR INSERT INTO TB_ORIGEM_ATENDIMENTO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CODIGO, DESCRICAO, STATUS)
                                     VALUES (-4, NULL, NULL, NULL, NULL, NULL, '0', 'Telegram', 1)
                                   MATCHING (ID);
UPDATE OR INSERT INTO TB_ORIGEM_ATENDIMENTO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CODIGO, DESCRICAO, STATUS)
                                     VALUES (-5, NULL, NULL, NULL, NULL, NULL, '0', 'Messenger', 1)
                                   MATCHING (ID);
UPDATE OR INSERT INTO TB_ORIGEM_ATENDIMENTO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CODIGO, DESCRICAO, STATUS)
                                     VALUES (-6, NULL, NULL, NULL, NULL, NULL, '0', 'Instagram', 1)
                                   MATCHING (ID);
UPDATE OR INSERT INTO TB_ORIGEM_ATENDIMENTO (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CODIGO, DESCRICAO, STATUS)
                                     VALUES (-7, NULL, NULL, NULL, NULL, NULL, '0', 'Skype', 1)
                                   MATCHING (ID);

                


                END;

            ").ToString();
        }

    }
}
