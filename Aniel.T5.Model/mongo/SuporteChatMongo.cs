using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.mongo
{
    /// <summary>
    /// No momento sendo utilizada somente para Migrations, no futuro deverá ser refatorada 
    /// para ser a SuporteChatMongo que atualmente está localizada (de forma errada) no projeto Aniel.Connect.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class SuporteChatMongoMigration
    {
        [BsonId()]
        public ObjectId _id { get; set; }
        public string idConexaoChat { get; set; }
        public string idSuporte { get; set; }
    }
}
