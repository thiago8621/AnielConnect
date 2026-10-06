using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.mongo
{
    [BsonIgnoreExtraElements]
    public class UfMongo /*: BaseModelMongo*/
    {
        [BsonId]
        public virtual Int64 Id { get; set; }
        public virtual Int64 Id_Pais { get; set; } 
        public virtual string Descricao { get; set; }               
        public virtual string Sigla { get; set; }               
        public virtual string Codigo_IBGE { get; set; }                 
        public GeoJsonModel geoJson { get; set; }

    }

}
