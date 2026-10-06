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
    public class ProdutosComp_ListModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
        public virtual Status_Default? Status_Filtro { get; set; }

        public virtual String Codigo_NCM { get; set; }
        public virtual String Codigo_CEST { get; set; }

    }

    public class ProdutosComp_CadastroModel : IViewModel<String>
    {
        public virtual String Id { get; set; }
        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual String Codigo_NCM { get; set; }
        public virtual String Codigo_CEST { get; set; }

    }

    public class ProdutosComp_DataTableRequest : DataTableRequest
    {
        public virtual String Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
    }

    public class ProdutosCompComboResult : IObjComboResult<long>
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }


    public class ExcluirPlanoProdutosComp
    {
        public virtual Int64 value { get; set; }
        public virtual List<Int64> Ids { get; set; }
    }

    public class NCMComboResult : IObjComboResult<String>
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Codigo_CEST { get; set; }
    }
}