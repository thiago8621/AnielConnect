using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class MotivoAusencia_ListModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "ID", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Codigo { get; set; }

        [Display(Name = "MOTIVO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Motivo { get; set; }

        public virtual String Status { get; set; }

    }
    public class MotivoAusencia_Model
    {
        public virtual int IdAusencia { get; set; }

        public virtual string Motivo { get; set; }

        public virtual Status_Default Status { get; set; }
    }

    public class MotivoAusencia_CadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(4, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        public virtual string Codigo { get; set; }
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "MOTIVO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Motivo { get; set; }   
        public virtual Boolean Status { get; set; }

    }

    public class MotivoAusencia_DataTableRequest : DataTableRequest
    {
        public virtual String Codigo { get; set; }

        public virtual string Motivo { get; set; }

        public virtual string Status { get; set; }
    }

    public class MotivoAusenciaComboResult : IObjComboResult<long>
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }
    }
}