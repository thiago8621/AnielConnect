using Aniel.Connect.Core;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class Tecnologia_ListModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }
        public virtual Status_Default? Status_Filtro { get; set; }
        public virtual String Status { get; set; }
    }

    public class Tecnologia_CadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        [MaxLength(4, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        public virtual string Codigo { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Descricao { get; set; }
        public virtual bool Status { get; set; }
    }

    public class Tecnologia_DataTableRequest : DataTableRequest
    {
        public virtual String Codigo { get; set; }
       
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
    }
}