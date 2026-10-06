using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class BancoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Codigo { get; set; }

        public virtual String Descricao { get; set; }
        
    }


    public class BancoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsFinanceiro))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(5, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Codigo { get; set; }


        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsFinanceiro))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                      ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsFinanceiro))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                      ErrorMessageResourceName = "REQUIRED")]
        public virtual Status_Default Status { get; set; }
    }

    public class Banco_DataTableRequest : DataTableRequest
    {

    }
}