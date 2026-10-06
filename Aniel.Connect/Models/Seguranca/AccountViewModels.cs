using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "LOGINREQUIRED")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "OLDPASSWORD", ResourceType = typeof(Resources.RsBase))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "NEWPASSWORDREQUIRED")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                           ErrorMessageResourceName = "PASSWORDLENGHT", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "NEWPASSWORD", ResourceType = typeof(Resources.RsBase))]
        public string NewPassword { get; set; }

        [Display(Name = "CONFIRMPASSWORD", ResourceType = typeof(Resources.RsBase))]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "CONFIRMACAOCOMPARE")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class PrimeiroAcessoViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "LOGINREQUIRED")]
        public string Login { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "NEWPASSWORDREQUIRED")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                           ErrorMessageResourceName = "PASSWORDLENGHT", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "CONFIRMACAOCOMPARE")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "LOGINREQUIRED")]
        [DataType(DataType.Password)]
        public string UserName { get; set; }

        [Display(Name = "PASSWORD", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "PASSWORDREQUIRED")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "LEMBREME", ResourceType = typeof(Resources.RsBase))]
        public bool RememberMe { get; set; }
    }

    public class SettingsViewModel
    {
        
    }

    public class MonitorAcesso
    {
        
        public string usuario { get; set; }
        public string hostName { get; set; }
        public long idAcesso { get; set; }
        public string ipLocal { get; set; }
    }
}
