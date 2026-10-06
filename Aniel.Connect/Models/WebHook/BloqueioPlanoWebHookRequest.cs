using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.WebHook
{
    public class BloqueioPlanoWebHookRequest : WebHookRequestBase
    {
        /// <summary>
        /// Id do Cliente
        /// </summary>
        public Int64 Id_Cliente { get; set; }
        /// <summary>
        /// ID do Plano
        /// </summary>
        public Int64 Id_Plano { get; set; }
        /// <summary>
        /// Data de Bloqueio do Plano
        /// </summary>
        public DateTime Data_Bloqueio { get; set; }
    }
}