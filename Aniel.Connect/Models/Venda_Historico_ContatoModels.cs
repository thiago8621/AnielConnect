using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Venda_Historico_Contrato_View
    {
        public virtual String Data_Ultimo_Contato { get; set; }
        public virtual String Forma_Contato { get; set; }
        public virtual String Observacao { get; set; }
    }
}