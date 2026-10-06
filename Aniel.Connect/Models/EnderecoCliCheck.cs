using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class EnderecoCliCheck
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CEP { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string Uf { get; set; }
        public string Pais { get; set; }
        public string Cidade { get; set; }
        public string Bloco { get; set; }
        public string Apartamento { get; set; }
        public string Numero { get; set; }
        public Int64? Id_Endereco_Logradouro { get; set; }
        public Int64? Id_Condominio_Predio { get; set; }
        public string BuscarPor { get; set; }
    }
}