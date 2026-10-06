using Aniel.Connect.Core;
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
    public class WebHookListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Url { get; set; }        
        public virtual String Type { get; set; }   
        public virtual String Status { get; set; }   

    }
    
    public class WebHookCadastroModel : IViewModel<Int64>
    { 
    /// <summary>
    /// Id
    /// </summary>
    public virtual Int64 Id { get; set; }
    /// <summary>
    /// Url do Webhook
    /// </summary>
    public virtual String Url { get; set; }
    /// <summary>
    /// Tipo de WebHook
    /// </summary>
    public virtual WebHook_Type Type { get; set; }
    /// <summary>
    /// Status da execução do WebHook
    /// </summary>
    public virtual Boolean Status { get; set; }
}

public class WebHook_DataTableRequest : DataTableRequest
    {
        
    }
}