using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.mongo
{
    public class BaseModelMongo
    {
        [BsonId()]
        public ObjectId _id { get; set; }


        private DateTime _ultimaAlteracao;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UltimaAlteracao
        {
            get => _ultimaAlteracao;
            set => _ultimaAlteracao = DateTime.Now;
        }

    }
}
