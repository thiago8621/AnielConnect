using Aniel.T5.Model.mongo;
using MongoDB.Driver;
using Sinapse.DAO;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO
{
    public class _20240322_0910_INDICE_NUM_OBRA_ORIGINAL_DESC : AnielMigration
    {
        
        public _20240322_0910_INDICE_NUM_OBRA_ORIGINAL_DESC()
        {
            Codigo = "20240322_0910";
            Descricao = "Criar indice para TB_DOCUMENTO_PRODUCAO";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE DESCENDING INDEX IDX_ORIGINAL_DESC ON TB_DOCUMENTO_PRODUCAO (NUM_OBRA_ORIGINAL);").ToString();
        }

    }
}
