using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class Segmento_ListaModel
    {
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
    }


    public class Segmento_CadastroModel : IViewModel<Int64>
    {

        [Display(Name = "CÓDIGO")]
        [Required(ErrorMessage = "O CAMPO CÓDIGO É OBRIGATORIO")]
        [MaxLength(15, ErrorMessage = "O CAMPO ULTRAPASSOU O LIMITE DE {0} CARACTERES")]
        public virtual String Codigo { get; set; }

        [Display(Name = "DESCRIÇÃO")]
        [Required(ErrorMessage = "O CAMPO DESCRIÇÃO É OBRIGATORIO")]
        [MaxLength(100, ErrorMessage = "O CAMPO ULTRAPASSOU O LIMITE DE {0} CARACTERES")]
        public virtual String Descricao { get; set; }

        [Required(ErrorMessage = "O CAMPO STATUS É OBRIGATORIO")]
        [Display(Name = "STATUS")]
        public virtual Boolean Status { get; set; }
        public virtual long Id { get; set; }
    }


    public class Segmento_DataTableRequest : DataTableRequest
    {
         public virtual String Status { get; set; }
    }

    public class AlterarSegmentoStatusViewModel
    {
        public List<string> cods { get; set; }
        public Int16 status { get; set; }
        public String usuario { get; set; }
    }
}