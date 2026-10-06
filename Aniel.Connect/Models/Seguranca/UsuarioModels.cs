using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Models
{
    public class UsuarioListagemViewModel: IViewModel<String>
    {
        public String Id { get; set; }

        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsBase))]
        public string Login { get; set; }

        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsBase))]
        public string Email { get; set; }

        [Display(Name = "NOME", ResourceType = typeof(Resources.RsBase))]
        public string Nome { get; set; }

        //[Display(Name = "PERFIL", ResourceType = typeof(Resources.RsBase))]
        //public string Perfis { get; set; }
    }

    public class UsuarioViewModel : IViewModel<String>
    {
        public String Id { get; set; }

        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "LOGINREQUIRED")]
        public string Login { get; set; }

        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "EMAILREQUIRED")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.RsBase),
                      ErrorMessageResourceName = "EMAILINVALID")]
        [RegularExpression(".+@.+\\..+", ErrorMessageResourceType = typeof(Resources.RsBase),
                                         ErrorMessageResourceName = "EMAILINVALID")]
        public string Email { get; set; }

        [Display(Name = "NOME", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "NOMEREQUIRED")]
        public string Nome { get; set; }

    }

}