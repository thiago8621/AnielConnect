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
    public class Categoria_ClienteListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }
        public virtual Tipo_Prioridade? Tipo_Prioridade { get; set; }

        public virtual String Status { get; set; }
    }


    public class Categoria_ClienteCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        public virtual Tipo_Prioridade? Tipo_Prioridade { get; set; }
        public long? Id_Cfop { get; set; }

        [Display(Name = "CODIGOCFOP", ResourceType = typeof(Resources.RsConfiguracao))]
        public string Codigo_Cfop { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Status { get; set; }

        public virtual Int64 Id_Categoria_Aniel { get; set; }

        [Display(Name = "CODIGO_TIPO_CLIENTE_INTEGRACAO", ResourceType = typeof(Resources.RsBase))]
        public virtual String CodigoTipoClienteIntegracao { get; set; }
    }

    public class Categoria_Cliente_DataTableRequest : DataTableRequest
    {
        
    }

}