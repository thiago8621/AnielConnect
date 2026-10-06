using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{

    public class TipoServicoImpVinculoRequest
    {
        public List<TipoServicoImpVinculo> TiposNaoEncontrado { get; set; } = new List<TipoServicoImpVinculo>();
        public string DocumentosJson { get; set; }
        public HttpPostedFileBase FileField { get; set; }
    }

    public class TipoServicoImpVinculo
    {
        public string Descricao { get; set; }
        public string Acao { get; set; }
        public long TipoServicoVincular { get; set; }
        
    }
}