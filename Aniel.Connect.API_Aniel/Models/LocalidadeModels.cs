using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class LocalidadeResponseModel
    {
        public int? codLocalidade { get; set; }
        public String codArea { get; set; }
        public String codEscritorio { get; set; }
        public String descricaoLocalidade { get; set; }
        public String descricaoGrupoArea { get; set; }
        public String descricaoArea { get; set; }
    }

    public class LocalidadeRequestModel : ApiRequest
    {
        public String descricaoLocalidade { get; set; }
        public String descricaoGrupoArea { get; set; }
        public String descricaoArea { get; set; }
    }


}