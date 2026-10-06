using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class RetornoAPIMKModels : ApiRequest
    {
        public bool Success { get; set; }
        public int Length { get; set; }
        public string Message { get; set; }
        public List<object> Result { get; set; }
    }

    public class RetornoVinculaConexao : ApiRequest
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class VinculoConexaoRequest : ApiRequest
    {
        public string NomeCaixa { get; set; }
        public int NumPorta { get; set; }
        public string Usuario { get; set; }
        public string OnuSerial { get; set; }
        public RetornoVinculaConexao RetornoVinculaConexao { get; set; }
    }
}