using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.mongo
{
    /// <summary>
    /// No momento sendo utilizada somente para Migrations, no futuro deverá ser refatorada 
    /// para ser a AtendimentoChatMongo que atualmente está localizada (de forma errada) no projeto Aniel.Connect.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class AtendimentoChatMongoMigration
    {
        [BsonId()]
        public ObjectId _id { get; set; }
        public string protocolo { get; set; }
        public string telegramChatId { get; set; }
        public string messengerChatId { get; set; }
        public string instagramChatId { get; set; }
        public string whatsAppChatId { get; set; }
        public string conversationSid { get; set; }
        public string telegramToken { get; set; }
        public string messengerToken { get; set; }
        public string backOfficeId { get; set; }
        public string instagramToken { get; set; }
        public string whatsAppToken { get; set; }
        public string motivo { get; set; }
        public string descricaoProblema { get; set; }
        public string dataAbertura { get; set; }
        public string dataEncerramento { get; set; }
        public bool bot { get; set; }
        public long? idUltimoEvento { get; set; }
        public long? idCanal { get; set; }
        public string dataHoraEnviadoAtendente { get; set; }
    }
}
