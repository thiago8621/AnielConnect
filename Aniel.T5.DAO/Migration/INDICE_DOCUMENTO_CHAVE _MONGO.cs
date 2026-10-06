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
    public class INDICE_DOCUMENTO_CHAVE_MONGO : MongoMigration
    {
        public INDICE_DOCUMENTO_CHAVE_MONGO()
        {
            Codigo = "20240929_1325";
            Descricao = "Criar indice de chave da O.S para collection DocumentoProducaoMongo";
            Tipo = TipoMongoMigration.Index;
        }

        public override void Execute()
        {
            using (var repository = new RepositoryMongo<DocumentoProducaoMongo>(this.Conexao))
            {
                IMongoCollection<DocumentoProducaoMongo> _collection = repository.GetCollection();

                IndexKeysDefinition<DocumentoProducaoMongo> indexDefinition = Builders<DocumentoProducaoMongo>.IndexKeys.Combine(
                                                                                         Builders<DocumentoProducaoMongo>.IndexKeys.Ascending(f => f.codCt),
                                                                                         Builders<DocumentoProducaoMongo>.IndexKeys.Ascending(f => f.projeto),
                                                                                         Builders<DocumentoProducaoMongo>.IndexKeys.Ascending(f => f.numObra),
                                                                                         Builders<DocumentoProducaoMongo>.IndexKeys.Ascending(f => f.contrato));

                CreateIndexModel<DocumentoProducaoMongo> indexModel = new CreateIndexModel<DocumentoProducaoMongo>(indexDefinition);

                _collection.Indexes.CreateOneAsync(indexModel);
            }
        }

    }
}
