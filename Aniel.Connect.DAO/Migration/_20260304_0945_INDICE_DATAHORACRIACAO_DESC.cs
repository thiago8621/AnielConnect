using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20260304_0945_INDICE_DATAHORACRIACAO_DESC : AnielMigration
    {
        public _20260304_0945_INDICE_DATAHORACRIACAO_DESC()
        {
            Codigo = "20260304_0945";
            Descricao = "Criar indice para TB_VENDA";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE ASCENDING INDEX IDX_VENDA_DT_CRIACAO ON TB_VENDA (DATAHORACRIACAO);").ToString();
        }
    }
}
