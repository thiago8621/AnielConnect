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
    public class INDICE_ATENDIMENTO_CHAT_MONGO : MongoMigration
    {
        public INDICE_ATENDIMENTO_CHAT_MONGO()
        {
            Codigo = "20240930_1049";
            Descricao = "Criar indices para collection AtendimentoChatMongo";
            Tipo = TipoMongoMigration.Index;
        }

        public override void Execute()
        {
            using (var repository = new RepositoryMongo<AtendimentoChatMongoMigration>(this.Conexao))
            {
                var db = this.Conexao.GetConnection();
                IMongoCollection<AtendimentoChatMongoMigration> _collection = db.GetCollection<AtendimentoChatMongoMigration>("AtendimentoChatMongo");                

                //Index Protocolo
                IndexKeysDefinition<AtendimentoChatMongoMigration> indexDefinitionProtocolo = Builders<AtendimentoChatMongoMigration>.IndexKeys.Ascending(f => f.protocolo);

                CreateIndexModel<AtendimentoChatMongoMigration> indexModelProtocolo = new CreateIndexModel<AtendimentoChatMongoMigration>(indexDefinitionProtocolo);

                //Index Data de Encerramento                
                IndexKeysDefinition<AtendimentoChatMongoMigration> indexDefinitionDataEncerramento = Builders<AtendimentoChatMongoMigration>.IndexKeys.Ascending(f => f.dataEncerramento);

                CreateIndexModel<AtendimentoChatMongoMigration> indexModelDataEncerramento = new CreateIndexModel<AtendimentoChatMongoMigration>(indexDefinitionDataEncerramento);

                List<CreateIndexModel<AtendimentoChatMongoMigration>> listaIndexes = new List<CreateIndexModel<AtendimentoChatMongoMigration>>();

                listaIndexes.Add(indexModelProtocolo);
                listaIndexes.Add(indexModelDataEncerramento);

                _collection.Indexes.CreateManyAsync(listaIndexes);               
            }
        }
    }
}
