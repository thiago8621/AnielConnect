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
    public class _20230824_1500_INDICE_TB_MATERIAL_TAREFA : AnielMigration
    {
        
        public _20230824_1500_INDICE_TB_MATERIAL_TAREFA()
        {
            Codigo = "20230824_1500";
            Descricao = "Criar indice para TB_MATERIAL_TAREFA";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"CREATE INDEX TB_MATERIAL_TAREFA_REL
                                                    ON TB_MATERIAL_TAREFA (ID_CLIENTE,CODMAT_INST,CODCPL) ;").ToString();
        }

    }
}
