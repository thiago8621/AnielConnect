using Aniel.Connect.Core;
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
    public class Motivo_Status_AtendimentoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
        public virtual String Status_Motivo { get; set; }
        public virtual Int64 Id_Motivo { get; set; }
    }

    public class Motivo_Status_Tipo_AtendimentoListModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Motivo_Status_Atendimento { get; set; }
        public virtual String Motivo_Status_Atendimento_Desc { get; set; }
        public virtual String Status_Motivo{ get; set; }
        public virtual String Status_Motivo_Desc { get; set; }
        public virtual Int64? Id_Tipo_Atendimento { get; set; }
    }

    public class Motivo_Status_AtendimentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Status { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Status_Motivo Status_Motivo { get; set; }
    }

    public class Motivo_Status_Atendimento_DataTableRequest : DataTableRequest
    {
        public string Status { get; set; }
        public string MotivoStatus { get; set; }
    }
}