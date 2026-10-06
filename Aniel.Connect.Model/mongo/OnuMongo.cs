using Aniel.T5.Model.mongo;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.mongo
{
    [BsonIgnoreExtraElements]
    public class OnuMongo : BaseModelMongo
    {
        public virtual long IdOnu { get; set; }
        public virtual DateTime DataDocumento { get; set; }
        public virtual List<DiagnosticoOnuMongo> Diagnosticos { get; set; }
    }
}
