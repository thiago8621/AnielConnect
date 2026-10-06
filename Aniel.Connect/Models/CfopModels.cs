using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Cfop_ListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
    }

    public class Cfop_CadastroModel : IViewModel<Int64>
    
    {
        public virtual Int64 Id { get; set; }

        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
    }
}