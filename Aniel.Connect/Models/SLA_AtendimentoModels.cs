using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class SLA_AtendimentoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Tipo_Atendimento { get; set; }
        public virtual String Prioridade_Atendimento { get; set; }
        public virtual String Categoria_Cliente { get; set; }
        public virtual String Horas { get; set; }
        public virtual String Status { get; set; }
        public virtual String Contagem_Prazo { get; set; }
        public virtual String Horario_Atendimento { get; set; }
    }
    
    public class SLA_AtendimentoCadastroModel : IViewModel<Int64>
    {       
        public virtual Int64 Id { get; set; }
        public virtual Int64? Id_Plano_Produto { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual Boolean Sla_PlanoProduto  { get; set; }

        public virtual Int64 Id_Tipo_Atendimento { get; set; }

        public virtual Int64? Id_Horario { get; set; }

        [Display(Name = "TIPO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Tipo_Atendimento_Desc { get; set; }

        [Display(Name = "PRIORIDADE_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Prioridade_Atendimento Prioridade_Atendimento { get; set; }

        [Display(Name = "CONTAGEM_PRAZO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Contagem_Prazo Contagem_Prazo { get; set; }

        public virtual Int64? Id_Categoria_Cliente { get; set; }

        //[Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]
        //[MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        //public virtual String Categoria_Cliente_Desc{ get; set; }

        [Display(Name = "HORAS", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        public virtual String Horas { get; set; }

        private List<PlanoProdutoListModel> PlanoProdutoLista;

        public virtual List<PlanoProdutoListModel> PlanosProdutos

        {
            get
            {
                if (PlanoProdutoLista == null)
                    PlanoProdutoLista = new List<PlanoProdutoListModel>();
                return PlanoProdutoLista;
            }
            set
            {
                PlanoProdutoLista = value;
            }
        }



    }

    public class SLA_Atendimento_DataTableRequest : DataTableRequest
    {
        public string PrioridadeAtendimento { get; set; }
        public string CategoriaCliente { get; set; }
        public string ContagemPrazo { get; set; }
        public string HorarioAtendimento { get; set; }
        public string FiltroStatus { get; set; }
    }
}