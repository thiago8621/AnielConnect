using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.MGTEC.Models
{
    public class EmpresaTelefoneViewModel : IViewModel<Int64>
    {
        public Int64 Id { get; set; }

        [Display(Name = "DDI", ResourceType = typeof(Resources.Resources))]
        [MaxLength(3, ErrorMessageResourceType= typeof(Resources.Resources),
                   ErrorMessageResourceName = "MAXLENGHT")]
        [DataType(DataType.Text)]
        public String DDI { get; set; }

        [Display(Name = "DDD", ResourceType = typeof(Resources.Resources))]
        [MaxLength(3, ErrorMessageResourceType = typeof(Resources.Resources),
                   ErrorMessageResourceName = "MAXLENGHT")]
        [DataType(DataType.Text)]
        public String DDD { get; set; }

        [Display(Name = "NUMERO", ResourceType = typeof(Resources.Resources))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
                  ErrorMessageResourceName = "NUMEROREQUIRED")]
        [MaxLength(10, ErrorMessageResourceType = typeof(Resources.Resources),
                   ErrorMessageResourceName = "MAXLENGHT")]
        [DataType(DataType.Text)]
        public String Numero { get; set; }

        [Display(Name = "RAMAL", ResourceType = typeof(Resources.Resources))]
        public String Ramal { get; set; }

        public String Operadora { get; set; }

        public Int64 ID_EMPRESA { get; set; }

    }

    

}