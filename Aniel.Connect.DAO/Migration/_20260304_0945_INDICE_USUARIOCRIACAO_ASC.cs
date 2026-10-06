using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public  class _20260304_0945_INDICE_USUARIOCRIACAO_ASC : AnielMigration
    {
        public _20260304_0945_INDICE_USUARIOCRIACAO_ASC()
        {
            Codigo = "20260304_0945";
            Descricao = "Criar indice para TB_CLIENTE";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE ASCENDING INDEX IDX_USUARIOCRIACAO ON TB_CLIENTE (USUARIOCRIACAO);").ToString();
        }
    }
}
