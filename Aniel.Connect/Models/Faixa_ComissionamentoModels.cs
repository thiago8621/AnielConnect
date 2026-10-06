using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Faixa_ComissionamentoListModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual Int64 Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }

        [Display(Name = "FILIAL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Filial { get; set; }

        public virtual String Status { get; set; }


    }

    public class Faixa_ComissionamentoCadastroModel : IViewModel<Int64>
    
    {
        private List<Faixa_Comissionamento_ItemModel> _itens;

        public virtual Int64 Id { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //            ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual String Codigo { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "MAXLENGHT")]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }


        public virtual Int64? Id_Filial { get; set; }

        [Display(Name = "FILIAL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Desc_Filial { get; set; }

        public virtual List<Faixa_Comissionamento_ItemModel> Itens
        {
            get
            {
                if (_itens == null)
                    _itens = new List<Faixa_Comissionamento_ItemModel>();
                return _itens;
            }
            set
            {
                _itens = value;
            }
        }

        public virtual Boolean Status { get; set; }





        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //            ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "FAIXA_INICIAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual Double? Faixa_Inicial { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //            ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "FAIXA_FINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual Double? Faixa_Final { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //            ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(80, ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "MAXLENGHT")]
        public virtual Double? Coeficiente { get; set; }

    }

    public class Faixa_Comissionamento_ItemModel : IViewModel<Int64>

    {
        public virtual Int64 Id { get; set; }

        public virtual Int64 Id_Faixa_Comissionamento { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "FAIXA_INICIAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual String Faixa_Inicial { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "FAIXA_FINAL", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual String Faixa_Final { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                    ErrorMessageResourceName = "REQUIRED")]
        public virtual String Coeficiente { get; set; }
    }


    public class Faixa_Comissionamento_DataTableRequest : DataTableRequest
    {

    }
}