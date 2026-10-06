using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Geosite.Models
{
    public class CaixaGeosite
    {
        public string tipoCodigo { get; set; }
        public string distancia { get; set; }
        public string qtdTotalDisponivel { get; set; }
        public string qtdSplitter { get; set; }
        public string qtdPortasSplitter { get; set; }
        public string qtdPortasSplitterDisp { get; set; }
        public string qtdPortasSplitterOcup { get; set; }
        public string qtdEquipamentosPacpon { get; set; }
        public string qtdPortasEthernet { get; set; }
        public string qtdPortasEthernetDisp { get; set; }
        public string qtdPortasEthernetOcup { get; set; }
    }
}