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
    public class TipoTarefaListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }        
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
        public virtual String Formato { get; set; }

    }

    public class TipoTarefaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 Codigo { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        public virtual bool Status { get; set; }
    }

    public class TipoTarefaDataTableRequest : DataTableRequest
    {
        
    }

    public class TipoTarefaComboResult : IObjComboResult<long>
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string Formato { get; set; }
    }
}