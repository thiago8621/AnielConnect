using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Aniel.Connect.Models
{


    public class VencimentoIndexModel
    {
        public Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Descricao { get; set; }

        public String Tipo { get; set; }

        public String Detalhes { get; set; }

        
    }


    public class VencimentoListModel
    {

        
        public Int64 Id { get; set; }
                
        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Descricao { get; set; }
                                           
    }


    public class VencimentoCadastroModel : IViewModel<Int64>
    {
        
        public Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public String Codigo { get; set; }
                                            
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                     ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "STATUSINICIAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public Status_PlanoProduto? StatusInicial { get; set; }

    }

 

    //public class VencimentoCadastroModel
    //{

    //    public Int64 Id { get; set; }
    //    public Int64? Id_Atividade { get; set; }
    //    public String Atividade { get; set; }
    //    public Double? Valor_Discriminado_NF { get; set; }
    //    public Double? Icms { get; set; }
    //    public Double? Pis { get; set; }
    //    public Double? Cofins { get; set; }
    //    public Double? Iss { get; set; }

    //}



}