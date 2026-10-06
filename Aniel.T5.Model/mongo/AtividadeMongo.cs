using Aniel.T5.Model.enumerado;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.mongo
{
    /// <summary>
    /// Collection responsável por armazenar alertas de SOS sobre o técnico no Aniel Touch.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class AtividadeMongo : BaseModelMongo
    {

        [BsonRequired()]
        public String Equipe { get; set; }
        public String EquipeSupervisor { get; set; }
        public String EquipeMonitor { get; set; }
        public String Nome { get; set; }
        public String NomeSupervisor { get; set; }
        public String NomeMonitor { get; set; }
        public String Data { get; set; }
        public bool Ativo { get; set; }
        public bool ConfirmadoCiencia { get; set; }
        public DateTime? DataHoraConfirmacao { get; set; }
        public String UsuarioConfirmacao { get; set; }
        public DateTime? DataHoraEnvioNotificacao { get; set; }
        public DateTime? DataHoraCriacao { get; set; }
        public DateTime? DataHoraEnvioServidor { get; set; }
        public bool SmsEnviado { get; set; }
        public EnumRespostaSOS? Resposta { get; set; }
        public string UuidNotificacao { get; set; }
    }
}
