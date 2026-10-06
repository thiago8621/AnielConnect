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
    public class INDICE_SUPORTE_CHAT_MONGO : MongoMigration
    {
        public INDICE_SUPORTE_CHAT_MONGO()
        {
            Codigo = "20240930_1124";
            Descricao = "Criar indice para collection SuporteChatMongo";
            Tipo = TipoMongoMigration.Index;
        }

        public override void Execute()
        {
            using (var repository = new RepositoryMongo<SuporteChatMongoMigration>(this.Conexao))
            {
                var db = this.Conexao.GetConnection();
                IMongoCollection<SuporteChatMongoMigration> _collection = db.GetCollection<SuporteChatMongoMigration>("SuporteChatMongo");

                //Index idConexaoChat
                IndexKeysDefinition<SuporteChatMongoMigration> indexDefinitionIdConexaoChat = Builders<SuporteChatMongoMigration>.IndexKeys.Combine(
                                                                                         Builders<SuporteChatMongoMigration>.IndexKeys.Ascending(f => f.idConexaoChat));

                CreateIndexModel<SuporteChatMongoMigration> indexModelIdConexaoChat = new CreateIndexModel<SuporteChatMongoMigration>(indexDefinitionIdConexaoChat);

                //Index idSuporte
                IndexKeysDefinition<SuporteChatMongoMigration> indexDefinitionIdSuporte = Builders<SuporteChatMongoMigration>.IndexKeys.Combine(
                                                                                         Builders<SuporteChatMongoMigration>.IndexKeys.Ascending(f => f.idSuporte));

                CreateIndexModel<SuporteChatMongoMigration> indexModelIdSuporte = new CreateIndexModel<SuporteChatMongoMigration>(indexDefinitionIdSuporte);

                List<CreateIndexModel<SuporteChatMongoMigration>> listaIndexes = new List<CreateIndexModel<SuporteChatMongoMigration>>();

                listaIndexes.Add(indexModelIdConexaoChat);
                listaIndexes.Add(indexModelIdSuporte);

                _collection.Indexes.CreateManyAsync(listaIndexes);
            }
        }
    }
}
