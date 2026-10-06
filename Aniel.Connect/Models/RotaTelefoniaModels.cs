using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class RotaTelefoniaListaModel
    {
        public virtual string Id { get; set; }
        public virtual string Nome { get; set; }
        
        public virtual string Status { get; set; }
    }

    public class RotaTelefoniaCadastroModel : IViewModel<Int64>
    {
        //obrigatorio para implementar a interface IViewModel
        public virtual Int64 Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Status { get; set; }
    }

    public class RotaTelefonia_DataTableRequest : DataTableRequest
    {
    }

}