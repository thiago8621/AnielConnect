using Aniel.Connect.Core;
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

    public class SLA_ExecucaoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Tipo_Atendimento { get; set; }
        public virtual String Categoria_Cliente { get; set; }
        public virtual String Minutos { get; set; }
    }
    
    public class SLA_ExecucaoCadastroModel : IViewModel<Int64>
    {

        public virtual Int64 Id { get; set; }

        
        public virtual Int64 Id_Tipo_Atendimento { get; set; }

        [Display(Name = "TIPO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual String Tipo_Atendimento_Desc { get; set; }
        
        public virtual Int64 Id_Categoria_Cliente { get; set; }

        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual String Categoria_Cliente_Desc{ get; set; }

        [Display(Name = "MINUTOS", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        public virtual String Horas { get; set; }

        public virtual Boolean Calendario_Padrao { get; set; }


        public virtual String Domingo_Entrada { get; set; }

        public virtual String Domingo_Saida { get; set; }


        public virtual String Segunda_Entrada { get; set; }

        public virtual String Segunda_Saida { get; set; }


        public virtual String Terca_Entrada { get; set; }

        public virtual String Terca_Saida { get; set; }


        public virtual String Quarta_Entrada { get; set; }

        public virtual String Quarta_Saida { get; set; }


        public virtual String Quinta_Entrada { get; set; }

        public virtual String Quinta_Saida { get; set; }


        public virtual String Sexta_Entrada { get; set; }

        public virtual String Sexta_Saida { get; set; }


        public virtual String Sabado_Entrada { get; set; }

        public virtual String Sabado_Saida { get; set; }
    }



    public class SLA_Execucao_DataTableRequest : DataTableRequest
    {

    }





}