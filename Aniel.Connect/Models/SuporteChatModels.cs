using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    [BsonIgnoreExtraElements]
    public class SuporteChatModels
    {
        public StatusAtendimento status { get; set; }
        public String idConexaoChat { get; set; }
        public String idSuporte { get; set; }
        public List<Int64?> Categorias { get; set; }
        public List<Escritorio_Area> Localidades { get; set; }
        public String foto { get; set; }
        public String MotivoAusencia { get; set; }
        public long qtdMaximaAtendimentos { get; set; }
        public String nomeAgente { get; set; }

        private String _inicioAtendimento;
        public virtual String inicioAtendimento
        {
            get
            {
                return _inicioAtendimento;
            }
            set {
                _inicioAtendimento = value;
            }
        }
        public virtual String terminoAtendimento { get; set; }
        public virtual String datahoraAlive { get; set; }
    }

    public enum StatusAtendimento
    {
        DISPONIVEL = 1,
        AGUARD_CONFIRMACAO = 2,
        OCUPADO = 3,
        EM_ATENDIMENTO = 4,
        AUSENTE = 5

    }

    public class AlterarStatusModels
    {
        public String idSuporte { get; set; }
        public Boolean online { get; set; }
        public String motivoAusencia { get; set; }

    }

    [BsonIgnoreExtraElements]
    public class SuporteChatMongo : SuporteChatModels
    {
        [BsonId()]
        public ObjectId _id { get; set; }
    }

    public class GetStatusResponse
    {
        public StatusAtendimento status { get; set; }
        public String motivoAusencia { get; set; }
        public long qtdAtendimento { get; set; }

    }



}