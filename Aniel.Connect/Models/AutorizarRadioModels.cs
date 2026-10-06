using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class AutorizarRadioRequest
    {
        public string Id_Interface_Conexao {  get; set; }
        public string Id_Cliente_Servico { get; set; }
        public string Num_OS { get; set; }
        public string Usuario { get; set; }
        public string Descricao { get; set; }
    }
}