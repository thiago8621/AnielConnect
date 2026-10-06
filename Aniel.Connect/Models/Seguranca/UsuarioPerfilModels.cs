using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class UsuarioPerfilListViewModel : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        [Display(Name = "PERFIL", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "PERFILREQUIRED")]
        public Int64 IdPerfil { get; set; }

        public String IdUsuario { get; set; }

        public Int32 Codigo { get; set; }

        public String Descricao { get; set; }
    }

}