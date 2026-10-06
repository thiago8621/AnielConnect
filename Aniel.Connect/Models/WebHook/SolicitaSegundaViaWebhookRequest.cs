using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.WebHook
{
    public class SolicitaSegundaViaWebhookRequest : WebHookRequestBase
    {
        public Int64 Id_Cliente { get; set; }
        public Int64? Id_Fatura { get; set; }
        public DateTime? Data_Solicitacao_SV { get; set; }
    }
}