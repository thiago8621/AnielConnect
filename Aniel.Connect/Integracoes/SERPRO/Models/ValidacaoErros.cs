using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.SERPRO.Models
{
    public class ValidacaoErros
    {
        public bool Valido { get; set; }
        public string Mensagem { get; set; } 
        public string Token { get; set; }
    }
}