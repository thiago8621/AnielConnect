using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.WebHook
{
    /// <summary>
    /// Sessão de request
    /// </summary>
    public class WebHookRequestBase
    {
        /// <summary>
        /// Request de Usuário
        /// </summary>
        public virtual String Usuario { get; set; }
        /// <summary>
        /// Request de Senha
        /// </summary>
        public virtual String Senha { get; set; }
        /// <summary>
        /// Request de Token
        /// </summary>
        public virtual String Token { get; set; }
    }
}

