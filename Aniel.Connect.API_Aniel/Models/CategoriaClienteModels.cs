using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Categoria_ClienteListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Status { get; set; }
    }
}