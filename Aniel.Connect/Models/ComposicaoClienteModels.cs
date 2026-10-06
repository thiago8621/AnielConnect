using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class ComposicaoClienteModels
    {
        public Int64 IdCliente { get; set; }
        public List<ComposicaoNotaCliModels> ComposicaoNotasCliente { get; set; }

    }
}