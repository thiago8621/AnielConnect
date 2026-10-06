using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class HubSoftModels : ApiRequest
    {
        public class PortaHubSoftRequest
        {

            public long IdClienteServico { get; set; }
            public long IdPortaAtendimento { get; set; }
            public long? Porta { get; set; }
          
            public string Login { get; set; }
        }
    }

}