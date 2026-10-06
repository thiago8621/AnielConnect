using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Locker_Retirada_Models
    {
        public virtual string Numero { get; set; }
        public virtual string dataretirada { get; set; }
        public virtual string horaRetirada { get; set; }
        public virtual string Boxretirados { get; set; }
        public virtual string observacao { get; set; }
    }
    public class Locker_Deposito_Models
    {
        public virtual string Numero { get; set; }
        public virtual string IdEntrega { get; set; }
        public virtual string codRetirada { get; set; }
        public virtual string Boxusados { get; set; }
        public virtual string datadeposito { get; set; }
        public virtual string horadeposito { get; set; }
        public virtual string observacao { get; set; }

    }

}