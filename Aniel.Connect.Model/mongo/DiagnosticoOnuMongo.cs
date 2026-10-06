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
    public class DiagnosticoOnuMongo : BaseModelMongo
    {
        public virtual DateTime DataHoraConsulta { get; set; }
        public virtual Double Potencia_Rx { get; set; }
        public virtual Double Potencia_Tx { get; set; }
        public virtual string Temperatura { get; set; }
        public virtual string Voltagem { get; set; }
    }
}
