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
    public class BairroListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Status_Default? Status_Filtro { get; set; }      
        public virtual String UF { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Pais { get; set; }

        public virtual String Status { get; set; }
    }


    public class BairroCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
                
        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(60, ErrorMessageResourceType = typeof(Resources.RsBase),
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
        
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual long? Cidade { get; set; }
        public virtual string CodigoAuxiliar { get; set; }
        public virtual Boolean ViaCep { get; set; }

    }

    public class Bairro_DataTableRequest : DataTableRequest
    {
        public String UF { get; set; }
        public String Status_Filtro { get; set; }
        public String Pais { get; set; }
        public String Cidade { get; set; }
    }

    [ExcelServiceKey("Descricao_Bairro")]
    [ExcelServiceSheetName("Bairro")]
    public class BairroImportacao : IExcelService
    {

        [MaxLength(60)]
        [ExcelService("Descrição do Bairro")]
        public virtual String Descricao_Bairro { get; set; }

        [MaxLength(30)]
        [ExcelService("País")]
        public virtual String Pais { get; set; }

        [MaxLength(2)]
        [ExcelService("UF")]
        public virtual String Sigla_UF { get; set; }
        
        [MaxLength(50)]
        [ExcelService("Cidade")]
        public virtual String Cidade { get; set; }

        public Boolean Importar { get; set; }
    }
}