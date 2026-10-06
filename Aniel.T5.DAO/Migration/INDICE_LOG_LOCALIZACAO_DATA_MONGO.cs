using Aniel.T5.Model.mongo;
using MongoDB.Driver;
using Sinapse.DAO;
using Sinapse.DAO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.Migration
{
    public class INDICE_LOG_LOCALIZACAO_DATA_MONGO : MongoMigration
    {
        public INDICE_LOG_LOCALIZACAO_DATA_MONGO()
        {
            Codigo = "20240930_0920";
            Descricao = "Criar indices de data e equipe para collection LogLocalizacaoDataMongo";
            Tipo = TipoMongoMigration.Index;
        }

        public override void Execute()
        {
            using (var repository = new RepositoryMongo<LogLocalizacaoDataMongo>(this.Conexao))
            {
                IMongoCollection<LogLocalizacaoDataMongo> _collection = repository.GetCollection();

                IndexKeysDefinition<LogLocalizacaoDataMongo> indexDefinition = Builders<LogLocalizacaoDataMongo>.IndexKeys.Combine(
                                                                                         Builders<LogLocalizacaoDataMongo>.IndexKeys.Ascending(f => f.Equipe),
                                                                                         Builders<LogLocalizacaoDataMongo>.IndexKeys.Ascending(f => f.DataDocumento));

                CreateIndexModel<LogLocalizacaoDataMongo> indexModel = new CreateIndexModel<LogLocalizacaoDataMongo>(indexDefinition);

                _collection.Indexes.CreateOneAsync(indexModel);
            }
        }
    }
}
