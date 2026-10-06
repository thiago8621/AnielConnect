using Aniel.Connect.Model.mongo;
using Aniel.T5.Model.mongo;
using MongoDB.Driver;
using Sinapse.DAO;
using Sinapse.DAO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class INDICE_ONU_MONGO : MongoMigration
    {
        public INDICE_ONU_MONGO()
        {
            Codigo = "20251029_1820";
            Descricao = "Criar indice de IdOnu e Data para collection OnuMongo";
            Tipo = TipoMongoMigration.Index;
        }

        public override void Execute()
        {
            using (var repository = new RepositoryMongo<OnuMongo>(this.Conexao))
            {
                IMongoCollection<OnuMongo> _collection = repository.GetCollection();

                IndexKeysDefinition<OnuMongo> indexDefinition = Builders<OnuMongo>.IndexKeys.Combine(
                                                                                         Builders<OnuMongo>.IndexKeys.Ascending(f => f.IdOnu),
                                                                                         Builders<OnuMongo>.IndexKeys.Ascending(f => f.DataDocumento));

                CreateIndexModel<OnuMongo> indexModel = new CreateIndexModel<OnuMongo>(indexDefinition);

                _collection.Indexes.CreateOneAsync(indexModel);
            }
        }
    }
}
