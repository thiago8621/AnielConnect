using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Bot_ListModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Avatar { get; set; }

        public virtual string Nome { get; set; }

        public virtual string Status { get; set; }

        public virtual string Habilitado { get; set; }
    }

    public class Bot_CadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "AVATAR", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Avatar { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Nome { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Boolean Habilitado { get; set; }
    }

    public class Bot_ConfiguracaoGeralModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "AVATAR", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Avatar { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Nome { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Boolean Habilitado { get; set; }
    }

    public class Bot_CustomizacaoModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "AVATAR", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Avatar { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Nome { get; set; }

        [MaxLength(400, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "TEXTO_INVALIDO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string TextoInvalido { get; set; }
        public virtual Boolean Transferencia { get; set; }
        public virtual int? Minutos { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Boolean Habilitado { get; set; }

        public virtual List<HttpPostedFileBase> Files { get; set; }
    }

    public class Bot_DataTableRequest : DataTableRequest
    {
        public virtual string Nome { get; set; }
        public virtual string Status { get; set; }
        public virtual string Habilitado { get; set; }
    }
}
