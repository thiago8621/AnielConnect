using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class GeoGridModels
    {
        public class ReservaGeoGrid: ApiRequest
        {   
            
            public long IdPorta { get; set; }

        
            public long IdCliente { get; set; }
        
        }
        public class Dados
        {
            public string id { get; set; }
            public string porta { get; set; }
            public string tipo { get; set; }
            public object conector { get; set; }
            public string destino { get; set; }
            public string label { get; set; }
            public object dbm { get; set; }
            public object potencia { get; set; }
            public string comentario { get; set; }
            public string reserva { get; set; }
            public string idEquipamento { get; set; }
            public string idRazaoSocial { get; set; }
            public string defasadaCodigo { get; set; }
            public string defasadaPonta { get; set; }
            public string defasadaRecipiente { get; set; }
            public object defasadaFusao { get; set; }
            public object fusaoId { get; set; }
            public object fusaoEm { get; set; }
            public string cordaoOptico { get; set; }
            public object tipoLaser { get; set; }
            public string dataReserva { get; set; }
            public object defasadaCliente { get; set; }
            public string idCliente { get; set; }
            public string reservaAtendimento { get; set; }
            public string reservaAtendimentoConvertida { get; set; }
            public object codigoIntegracao { get; set; }
        }

        public class Reserva
        {
            public Dados dados { get; set; }
        }

        public class ResponseGeogrid
        {
            internal bool success;
            internal object message;

            public List<Reserva> reservas { get; set; }
        }
        public class VincluaPorta : ApiRequest
        {
            public long? Porta { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }

            public string Login{ get; set; }
        }
    }
}