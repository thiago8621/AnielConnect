using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Atividade_NfListModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }

        public virtual String Status { get; set; }

        public virtual String Tipo_Atividade { get; set; }

    }

    public class Atividade_NfCadastroModel : IViewModel<Int64>
    
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }

        public virtual Tipo_Atividade_Nf Tipo_Atividade { get; set; }
        public virtual Boolean Status { get; set; }
    }
}