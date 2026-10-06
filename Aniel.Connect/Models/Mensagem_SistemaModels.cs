using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Mensagem_SistemaListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Titulo { get; set; }

        public virtual String Mensagem { get; set; }        
    }


    public class Mensagem_SistemaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "TITULO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Titulo { get; set; }

        [Display(Name = "MENSAGEM", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Mensagem { get; set; }


        public virtual Boolean Status { get; set; }

    }

    public class Mensagem_Sistema_DataTableRequest : DataTableRequest
    {
        
    }

}