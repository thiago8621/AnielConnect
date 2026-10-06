using Aniel.T5.Model.enumerado;
using MongoDB.Bson.Serialization.Attributes;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;


namespace Aniel.T5.Model.nhibernate.mongo
{
    [BsonIgnoreExtraElements]
    public class Area_CoberturaMongo
    {
        [BsonId()]
        public virtual Int64 Id { get; set; }

        public virtual String UsuarioCriacao { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime? DataHoraCriacao { get; set; }

        public virtual String UsuarioAlteracao { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime? DataHoraAlteracao { get; set; }

        public virtual String Descricao { get; set; }

        public virtual Status_Default Situacao { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual List<List<Double?>> Coordenadas { get; set; } = new List<List<double?>>();
        public virtual String Cor { get; set; }
        public virtual Tipo_Poligono Tipo{get;set;}
    }

}
