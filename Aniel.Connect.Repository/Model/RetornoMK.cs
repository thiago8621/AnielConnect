using System.Collections.Generic;
using System;

namespace Aniel.Connect.Repository.Model
{
    public class RetornoMK
    {       
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class VinculoConexaoRequest
    {
        public string NomeCaixa { get; set; }
        public int NumPorta { get; set; }
        public string Usuario { get; set; }
        public string OnuSerial { get; set; }
    }
}
