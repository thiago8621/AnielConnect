using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.mongo
{
    [BsonIgnoreExtraElements]
    public class DocumentoProducaoMongo : BaseModelMongo
    {
        public String codCt { get; set; }
        public String projeto { get; set; }
        public int contrato { get; set; }
        public String numObra { get; set; }
        public String numObraOriginal { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime dataAgendamento { get; set; }
        public List<StatusDocumentoProducao> status { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class StatusDocumentoProducao
    {
        public int? statusExecucao;
        public String equipe { get; set; }
        public String numFre { get; set; }
        public String nome { get; set; }
        public Double latitude { get; set; }
        public Double longitude { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime data { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime hora { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime dataHoraServidor { get; set; }
        public String justificativaPausaMongo { get; set; }
        public String justificativaReprova { get; set; }
        public String justificativaAprova { get; set; }
    }

}
