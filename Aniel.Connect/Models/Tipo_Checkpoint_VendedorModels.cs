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
    public class Tipo_Checkpoint_VendedorListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Status { get; set; }
    }


    public class Tipo_Checkpoint_VendedorCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        
        public virtual Boolean Status { get; set; }
    }

    public class Tipo_Checkpoint_Vendedor_DataTableRequest : DataTableRequest
    {
        
    }

}