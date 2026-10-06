using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.mongo
{
    [BsonIgnoreExtraElements]
    public class LogLocalizacaoDataMongo : BaseModelMongo
    {

        [BsonRequired()]
        public String Equipe { get; set; }
        [BsonRequired()]
        public String Usuario { get; set; }
        public String Nome { get; set; }
        public DateTime DataDocumento { get; set; }
        public List<Localizacao> localizacoes { get; set; }



    }
}
