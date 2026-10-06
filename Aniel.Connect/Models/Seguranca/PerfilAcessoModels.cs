using Sinapse.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.MGTEC.Models
{

    public class PerfilAcessoViewModel : IViewModel<Int64>
    {

        public Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.Resources))]
        public Int32 Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
                  ErrorMessageResourceName = "DESCRICAOREQUIRED")]
        [DataType(DataType.Text)]
        public string Descricao { get; set; }
    }

    public class PerfilAcessoListaModel : IViewModel<Int64>
    {
  
        public Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.Resources))]
        public Int32 Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.Resources))]
        public string Descricao { get; set; }

    }

    

}