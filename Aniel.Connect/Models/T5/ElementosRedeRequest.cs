

using System.Collections.Generic;

namespace Aniel.Connect.Models.T5
{
    public class ElementosRedeRequest
    {
        public string isCaixa { get; set; }
        public long IdElemento { get; set; }
        public string Tipo { get; set; }
        public List<long> idsPortas { get; set; }
    }
}