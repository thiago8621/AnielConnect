using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class EmpresaTreeViewModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Nome { get; set; }
    }

    public class EmpresaListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Nome { get; set; }
    }
}