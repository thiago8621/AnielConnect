using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Configuracao_PlanoProdutoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Codigo { get; set; }
    }


    public class PlanoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.Resources),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.Resources))]
        public virtual String Codigo { get; set; }

    }

    public class Configuracao_PlanoProduto_DataTableRequest : DataTableRequest
    {

    }

    public class Plano_Model
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }
    }

}