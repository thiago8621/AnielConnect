using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Modelo_ReciboListaModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Nome_Arquivo { get; set; }

        public virtual String Extensao_Arquivo { get; set; }

        public virtual String Status { get; set; }
    }


    public class Modelo_ReciboCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        public virtual String Nome_Arquivo { get; set; }
        public virtual Boolean ArquivoAlterado { get; set; }
        public virtual String Extensao_Arquivo { get; set; }
        public virtual Boolean Status { get; set; }
    }

    public class Modelo_Recibo_DataTableRequest : DataTableRequest
    {
        
    }

}