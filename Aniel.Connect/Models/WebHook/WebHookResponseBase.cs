using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.WebHook
{
    /// <summary>
    /// Sessão de Rsponse 
    /// </summary>
    public class WebHookResponseBase
    {
        /// <summary>
        /// Verifica se está OK
        /// </summary>
        public Boolean Ok { get; set; }
        /// <summary>
        /// No caso de erro, uma mensagem é gerada
        /// </summary>
        public String Mensagem { get; set; }
        /// <summary>
        /// No caso de erro, uma exceção é gerada
        /// </summary>
        public String InnerException { get; set; }
    }
}