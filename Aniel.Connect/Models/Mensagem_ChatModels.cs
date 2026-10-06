using Aniel.Connect.Model.enumerado;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    [BsonIgnoreExtraElements]
    public class Mensagem_ChatModels
    {
        public string id { get; set; }
        public String mensagem { get; set; }
        public String dataHora { get; set; }
        public String dataHoraServidor { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");
        public Boolean enviadoPeloSuporte { get; set; }
        public StatusEnvioEnum statusEnvio { get; set; }
        public String usuario { get; set; }
        public String nome_usuario { get; set; }
        public String protocolo { get; set; }
        public Boolean fluxoChat { get; set; }
        public String conversasId { get; set; }
        //// public virtual FluxoAtendimento EtapaFluxo { get; set; }
        // public virtual DateTime? DataIndicador_EtapaFluxo { get; set; }
        // public virtual DateTime? DataInicio_EtapaFluxo { get; set; }
        public string idConexao { get; set; }

        private IList<ArquivoMensagem_ChatModels> _arquivos;        
        public long? idModeloMensagem { get; set; }
        public String matriculaUsuario { get; set; }
        public virtual IList<ArquivoMensagem_ChatModels> arquivos
        {
            get
            {
                if (_arquivos == null)
                    _arquivos = new List<ArquivoMensagem_ChatModels>();
                return _arquivos;
            }
            set
            {
                _arquivos = value;
            }
        }

    }

    public class Conversas_Chat
    {
        public String id { get; set; }
        public String data_inicio { get; set; }
        public String data_final { get; set; }
    }

    public class Mensagem_ChatBackOfficeModels
    {
        public string id { get; set; }
        public String mensagem { get; set; }
        public String dataHoraString { get; set; }
        public DateTime? dataHora { get; set; }
        public StatusEnvioEnum statusEnvio { get; set; }
        public String? num_os { get; set; }
        public ServicoUsuario usuario { get; set; }
        private IList<ArquivoMensagem_ChatBackOfficeModels> _arquivos;
        public virtual IList<ArquivoMensagem_ChatBackOfficeModels> arquivos
        {
            get
            {
                if (_arquivos == null)
                    _arquivos = new List<ArquivoMensagem_ChatBackOfficeModels>();
                return _arquivos;
            }
            set
            {
                _arquivos = value;
            }
        }
        public ArquivoMensagem_ChatBackOfficeModels anexo { get; set; }
    }

    public class ArquivoMensagem_ChatModels
    {
        public String id { get; set; }
        public String name { get; set; }
        public String url { get; set; }
        public Tipo_Bot_Evento_Anexo tipo { get; set; }
    }

    public class Request
    {
        public List<Entry> Entry { get; set; }
    }

    /*public class Entry
    {
        public List<Change> Changes { get; set; }
    }

    public class Change
    {
        public Value Value { get; set; }
    }

    public class Value
    {
        public List<Message> Messages { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }*/


    public class ArquivoMensagem_ChatBackOfficeModels
    {
        public String id { get; set; }
        public String name { get; set; }
        public String url { get; set; }
        public String mimeType { get; set; }
        public String fileExtension { get; set; }
        public byte[] arquivoBytes { get; set; }
        public TipoAnexoEnum tipo { get; set; }
        public bool isUrl { get; set; }
    }
}