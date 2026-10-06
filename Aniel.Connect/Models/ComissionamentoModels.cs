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
    public class ComissionamentoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Filial { get; set; }
        public virtual String Status { get; set; }

    }


    public class ComissionamentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }


        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Codigo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        public virtual Int64? Id_Filial { get; set; }

        [Display(Name = "NOME_EMPRESA", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Nome_Filial { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]         
        public virtual Boolean Status { get; set; }

        public Boolean Carregar_Plano_Produto { get; set; }

        public virtual Int64? Id_Plano { get; set; }

        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Desc_Plano { get; set; }

        public virtual Double? Valor_Plano { get; set; }

        public virtual Int64? Id_Produto { get; set; }

        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Desc_Produto { get; set; }

        public virtual String Valor_Produto { get; set; }

        public virtual Int64 Qtd_Plano_Produto { get; set; }
      

    }

    public class Comissionamento_PlanoProdutoListaModel
    {
        public long Id { get; set; }

        public long IdComissionamento { get; set; }

        public string Descricao { get; set; }
              
        public string Unidade { get; set; }
               
        public string Quantidade { get; set; }

    }

    public class Comissionamento_PlanoProdutoDataTableRequest : DataTableRequest
    {
        public virtual Int64 Id_Comissionamento { get; set; }
    }

    public class Comissionamento_DataTableRequest : DataTableRequest
    {

    }

    public class ComissionamentoExtrato
    {
        public virtual String NomeCliente { get; set; }

        public virtual DateTime DataVenda { get; set; }

        public virtual String PlanoProduto { get; set; }

        public virtual Int64? IdComissionamento { get; set; }

        public virtual Double? Coeficiente { get; set; }

        public virtual Double? ValorFinal { get; set; }

        public virtual Double? Comissao { get; set; }

        public virtual Double? Pontos { get; set; }

        public virtual Int64? IdFaixaItem { get; set; }

        public virtual Int32? mesDaComissao { get; set; }

    }
}