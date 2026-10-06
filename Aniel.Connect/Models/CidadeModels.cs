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
    public class CidadeListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Codigo_IBGE { get; set; }
        public virtual String UF { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Pais { get; set; }

        public virtual String Status { get; set; }
        public virtual Status_Default? Status_Filtro { get; set; }
        public virtual Int64? Id_UF { get; set; }
        public virtual Int64? Id_Pais { get; set; }
    }


    public class CidadeCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO_IBGE", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(7, ErrorMessageResourceType = typeof(Resources.RsBase),
                       ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Codigo_IBGE { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        public virtual Boolean Status { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        public virtual long? UF { get; set; }

        [Display(Name = "PAIS", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        public virtual long? Pais { get; set; }
        public virtual string CodigoAuxiliar { get;  set; }
    }
        

    [ExcelServiceKey("Codigo_Oficial_IBGE")]
    [ExcelServiceSheetName("Cidade")]
    public class CidadeImportacao : IExcelService
    {
        [MaxLength(7)]
        [ExcelService("Código Oficial (IBGE)")]
        public virtual String Codigo_Oficial_IBGE { get; set; }

        [MaxLength(50)]
        [ExcelService("Descrição da Cidade")]
        public virtual String Descricao_Cidade { get; set; }

        [MaxLength(30)]
        [ExcelService("País")]
        public virtual String Pais { get; set; }

        [MaxLength(2)]
        [ExcelService("Sigla UF")]
        public virtual String Sigla_UF { get; set; }

        public Boolean Importar { get; set; }
    }

    public class Cidade_DataTableRequest : DataTableRequest
    {
        public String UF { get; set; }
        public String Status_Filtro { get; set; }
        public String Pais { get; set; }
    }

}