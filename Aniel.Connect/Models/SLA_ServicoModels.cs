using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class SLA_ServicoListaModel
    {
        public virtual Int64 Id { get; set; }
        
        public virtual String Contrato { get; set; }
        public virtual String Tipo_Servico { get; set; }
        public virtual String Categoria_Cliente { get; set; }
        public virtual String Horas { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Calendario { get; set; }
        public virtual String Calendario_Padrao { get; set; }

        public virtual String Horario_Trabalho { get; set; }

        public virtual String Status { get; set; }
    }
    
    public class SLA_ServicoCadastroModel : IViewModel<Int64>
    {

        public virtual Int64 Id { get; set; }

        public Boolean SLA_Retroativo { get; set; }

        public virtual Boolean Status { get; set; }

        [Display(Name = "CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
          ErrorMessageResourceName = "REQUIRED")]
        public virtual String CodCt { get; set; }

        [Display(Name = "TIPO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
          ErrorMessageResourceName = "REQUIRED")]
        public virtual Int32? Id_Tipo_Servico { get; set; }
        
        public virtual String Tipo_Servico { get; set; }

        public virtual Int64? Id_Categoria_Cliente { get; set; }
        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Categoria_Cliente { get; set; }
                        
        public virtual Tipo_SLA Tipo { get; set; }

        public virtual String Calendario { get; set; }

        
        public virtual Int64? Id_Horario_Connect { get; set; }

        public virtual Horario_Connect Horario_Trabalho { get; set; }


        [Display(Name = "HORAS", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        public virtual String Horas { get; set; }

        //public virtual Boolean Calendario_Padrao { get; set; }
        
        //public virtual String Domingo_Entrada { get; set; }

        //public virtual String Domingo_Saida { get; set; }


        //public virtual String Segunda_Entrada { get; set; }

        //public virtual String Segunda_Saida { get; set; }


        //public virtual String Terca_Entrada { get; set; }

        //public virtual String Terca_Saida { get; set; }


        //public virtual String Quarta_Entrada { get; set; }

        //public virtual String Quarta_Saida { get; set; }


        //public virtual String Quinta_Entrada { get; set; }

        //public virtual String Quinta_Saida { get; set; }


        //public virtual String Sexta_Entrada { get; set; }

        //public virtual String Sexta_Saida { get; set; }


        //public virtual String Sabado_Entrada { get; set; }

        //public virtual String Sabado_Saida { get; set; }
    }



    public class SLA_Servico_DataTableRequest : DataTableRequest
    {

    }

}