using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using Sinapse.Model.nhibernate;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Meio_CaptacaoListaModel
    {
        public virtual Int64 Id { get; set; }

        /// <summary> Utilizado na tela de descontos somente </summary>
        public virtual Int64 Id_Meio { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
    }


    public class Meio_CaptacaoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        // [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "DESCONTO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Status { get; set; }

    }

    public class Meio_Captacao_DataTableRequest : DataTableRequest
    {

    }
    public class Meio_Captacao_List_Model {
        /// <summary> Identificador Meio_captação </summary>
       public virtual Int64 Id { get; set; }

        /// <summary> Tipos Meio_captação </summary>
        public virtual String Descricao { get; set; }
        /// <summary> Disponibilidade_Meio_captação </summary>
        public virtual String Status { get; set; }
    }
    public class Meio_Captacao_ApiRequest: ApiRequest { 

    }

    public class Meio_CaptacaoDescontoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual DescontoListaModel Desconto { get; set; }
    }

}