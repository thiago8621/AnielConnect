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
    public class Motivo_EncerramentoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Select { get; set; }

        public virtual String Descricao { get; set; }

        public virtual Boolean Status { get; set; }
    }


    public class Motivo_EncerramentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
          ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        public virtual Boolean Status { get; set; }
    }

    public class Motivo_Encerramento_DataTableRequest : DataTableRequest
    {
        
    }

}