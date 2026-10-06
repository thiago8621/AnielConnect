using Sinapse.DAO;
using System;
using System.Text;

namespace Aniel.T5.DAO
{
    class ADD_CAMPO_OBRIGATORIO : AnielMigration
    {
        public ADD_CAMPO_OBRIGATORIO()
        {
            Codigo = "20240629_0947";
            Descricao = "Add campo obrigatorio";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(
            @"
               UPDATE OR INSERT INTO TB_CAMPOS_OBRIG_LEAD (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CAMPO, VISIVEL, OBRIGATORIO)
                                                                 VALUES (44, NULL, '2024-06-29 09:58:00', NULL, NULL, NULL, 43, 0, 0);
            ").ToString();            
        }
    }
}
