using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Motivo_InviabilidadeListaModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Motivo { get; set; }

        public virtual String Status { get; set; }
    }


    public class Motivo_InviabilidadeCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "MOTIVO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Motivo { get; set; }
        public virtual Motivo_Principal_InviabilidadeEnum? Motivo_Principal { get; set; }
        public virtual Boolean Status { get; set; }
    }


    public class Motivo_InviabilidadeDataTableRequest : DataTableRequest
    {

    }
    public class Motivo_InviabilidadeComboResult<T> : ObjComboResult<T>
    {        
        public Motivo_Principal_InviabilidadeEnum? Motivo { get; set; }
    }

}