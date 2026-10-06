using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Geosite.Models
{
    public class ResponseGeosite
    {
        public bool success { get; set; }
        public long? fid { get; set; }
        public String message { get; set; }

    }
    public class ResponseGeosite<T>:ResponseGeosite  where T : class
    {
        public string page { get; set; }
        public string total { get; set; }
        public List<T> records { get; set; }

    }

    public class ResponseGeositeReserva : ResponseGeosite
    {
        public String porta { get; set; }
        public String dataExpiracao { get; set; }

    }
    public class ResponseGeositeViabilidade : ResponseGeosite
    {
        private List<CaixaGeosite> caixas;
        public List<CaixaGeosite> caixasGeosite
        {
            get
            {
                if (caixas == null) { caixas = new List<CaixaGeosite>(); }
                return caixas;
            }
            set { caixas = value; }
        }

    }
    public class Request_ViabilidadeGeosite
    {
        public double Raio { get; set; }
        public String Endereco { get; set; }
        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }
        public String GooglePlaceId { get; set; }
        public viabilidade_GeositeParameter Mudanca_RequisicaoViabilidade { get; set; }
    }
}