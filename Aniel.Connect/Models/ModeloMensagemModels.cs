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
    public class ModeloMensagemListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }        
        public virtual String Texto { get; set; }   
        public virtual String Status { get; set; }   
        public virtual String IdWhatsAppBusinessIdentificacao { get; set; }   
        public virtual String TipoModeloMensagem { get; set; }   

    }

    public class ModeloMensagemCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsCadastros))]

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Texto { get; set; }
        public virtual String Status { get; set; }
        public virtual String IdWhatsAppBusinessIdentificacao { get; set; }
        public virtual String TipoModeloMensagem { get; set; }
    }

    public class ModeloMensagem_DataTableRequest : DataTableRequest
    {

    }

    public class ObjComboModeloMensagem<T> : IObjComboResult<T>
    {
        public virtual T Id { get; set; }
        public virtual String Text { get; set; }
        public virtual String TextoMensagem { get; set; }

    }
}