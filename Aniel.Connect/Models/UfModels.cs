using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class UfListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo_IBGE { get; set; }
        public virtual String Sigla { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Status { get; set; }
        public virtual Status_Default? Status_Filtro { get; set; }
    }


    public class UfCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO_IBGE", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Resources.RsBase),
                       ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Codigo_IBGE { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(2, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Sigla { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
              ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "PAIS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual long? Pais { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual string CodigoAuxiliar { get; set; }

    }

    [ExcelServiceKey("Codigo_Oficial_IBGE")]
    [ExcelServiceSheetName("Uf")]
    public class UfImportacao : IExcelService
    {
        [MaxLength(50)]
        [ExcelService("Código Oficial (IBGE)")]
        public virtual String Codigo_Oficial_IBGE { get; set; }
               
        [ExcelService("Descrição da UF")]
        public virtual String Descricao_UF { get; set; }
                     
        [MaxLength(30)]
        [ExcelService("País")]
        public virtual String Pais { get; set; } 
        
        [MaxLength(2)]
        [ExcelService("Sigla UF")]
        public virtual String Sigla_UF { get; set; }
                
        public Boolean Importar { get; set; }
    }
    public class Uf_DataTableRequest : DataTableRequest
    {
        public String UF { get; set; }
        public String Status_Filtro { get; set; }
        public String Pais { get; set; }
    }

}