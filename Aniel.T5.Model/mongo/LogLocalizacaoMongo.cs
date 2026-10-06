using Aniel.T5.Model.enumerado;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.mongo
{
    [BsonIgnoreExtraElements]
    public class LogLocalizacaoMongo : BaseModelMongo
    {

        [BsonRequired()]
        public String Equipe { get; set; }
        [BsonRequired()]
        public String Usuario { get; set; }
        public String Nome { get; set; }
        public List<Localizacao> localizacoes { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Localizacao
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? DataHora { get; set; }
        public EquipeStatusTipo statusTecnico { get; set; }

        public Double longitude { get; set; }
        public Double latitude { get; set; }

        public String justificativa { get; set; }
        public String statusGps { get; set; }
        public Double precisao { get; set; }
        public Int32 idCheckpoint { get; set; }
        public ObjectId? DocumentoProducao { get; set; }
        public Aparelho Aparelho { get; set; }
        public String previsto { get; set; }
    }



    [BsonIgnoreExtraElements]
    public class DocumentoProducaoChave
    {
        public String Codct { get; set; }
        public Int64 Contrato { get; set; }
        public String Num_obra { get; set; }
        public String Projeto { get; set; }

    }




    [BsonIgnoreExtraElements]
    public class Aparelho
    {
        public String Modelo { get; set; }
        public String Imei { get; set; }
        public Double NivelBateria { get; set; }
        public int StatusWifi { get; set; }
        public String NivelRedeMovel { get; set; }
        public TipoCarregamentoBateria Carregando { get; set; }

    }
    
}
