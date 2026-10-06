using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class FilialListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String CodFilial { get; set; }
        public virtual String Nome { get; set; }
        
        
    }
    
    public class FilialCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String CodFilial { get; set; }
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Nome { get; set; }

        public virtual Int64? Id_Empresa { get; set; }
        [MaxLength(40, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Empresa { get; set; }
        //public virtual Empresa Empresa { get; set; }

        public virtual Int64? Id_Faixa_Comissionamento { get; set; }

        //public virtual Faixa_Comissionamento Faixa_Comissionamento { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        public virtual String Faixa_Comissionamento { get; set; }

    }

    public class FilialMotivoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
                
    }

    public class FilialTreeViewModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Nome { get; set; }
    }
    public class Filial_DataTableRequest : DataTableRequest
    {
        
    }

    
}

