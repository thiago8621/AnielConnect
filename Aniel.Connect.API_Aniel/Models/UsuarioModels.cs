using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class UsuarioModelsRequest : ApiRequest
    {
        public string NumFre { get; set; }
        public string Senha { get; set; }

    }
}