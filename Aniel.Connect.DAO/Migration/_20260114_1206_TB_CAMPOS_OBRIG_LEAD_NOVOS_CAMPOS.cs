using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    class _20260115_1340_TB_CAMPOS_OBRIG_LEAD_NOVOS_CAMPOS : AnielMigration
    {
        public _20260115_1340_TB_CAMPOS_OBRIG_LEAD_NOVOS_CAMPOS()
        {
            Codigo = "20260115_1340";
            Descricao = "Campos Obrigatorios de Lead";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }


        public override string GetSql()
        {
            return new StringBuilder(@"UPDATE OR INSERT INTO TB_CAMPOS_OBRIG_LEAD (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CAMPO, VISIVEL, OBRIGATORIO)
                                                                 VALUES (44, NULL, '2026-01-14 12:06:04', NULL, NULL, NULL, 43, 0, 0);
                                    UPDATE OR INSERT INTO TB_CAMPOS_OBRIG_LEAD (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CAMPO, VISIVEL, OBRIGATORIO)
                                                                 VALUES (45, NULL, '2026-01-14 12:06:04', NULL, NULL, NULL, 44, 0, 0);
                                    UPDATE OR  INSERT INTO TB_CAMPOS_OBRIG_LEAD (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CAMPO, VISIVEL, OBRIGATORIO)
                                                                 VALUES (46, NULL, '2026-01-14 12:06:04', NULL, NULL, NULL, 45, 0, 0);
                                    UPDATE OR  INSERT INTO TB_CAMPOS_OBRIG_LEAD (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CAMPO, VISIVEL, OBRIGATORIO)
                                                                 VALUES (47, NULL, '2026-01-14 12:06:04', NULL, NULL, NULL, 46, 0, 0);
                                    UPDATE OR  INSERT INTO TB_CAMPOS_OBRIG_LEAD (ID, USUARIOCRIACAO, DATAHORACRIACAO, USUARIOALTERACAO, DATAHORAALTERACAO, TOKEN, CAMPO, VISIVEL, OBRIGATORIO)
                                                                 VALUES (48, NULL, '2026-01-14 12:06:04', NULL, NULL, NULL, 47, 0, 0);").ToString();
        }

    }
}
