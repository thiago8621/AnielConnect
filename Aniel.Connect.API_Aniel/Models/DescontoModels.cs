using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    public enum TipoValor_DescontoCadastroModel
    {
        Valor = 1,
        Percentual = 2
    }

    public class DescontoCadastroModel : IViewModel<Int64>
    {

        public virtual Int64 Id { get; set; }

        // [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }



        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]
        public virtual TipoValor_DescontoCadastroModel TipoValor { get; set; }
        
        public virtual Double? Percentual_Desconto { get; set; }


        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]
        
        public virtual Double? Valor_Desconto { get; set; }

        public virtual Int64? Id_Tipo_Desconto { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Desconto_Captacao? Desconto_Captacao { get; set; }

        public virtual Int64? Repeticoes_Desconto { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Boolean Desconto_Tempo_Indeterminado { get; set; }

        /// <summary> Temporário para alimentar a grid </summary>
        public virtual Int64? Id_Meio { get; set; }

        private List<Meio_CaptacaoListaModel> MeiosLista;

        public virtual List<Meio_CaptacaoListaModel> Meios

        {
            get
            {
                if (MeiosLista == null)
                    MeiosLista = new List<Meio_CaptacaoListaModel>();
                return MeiosLista;
            }
            set
            {
                MeiosLista = value;
            }
        }

    }



    public class Desconto_DataTableRequest : DataTableRequest
    {

    }
    public class Desconto_ApiRequest : ApiRequest
    {

    }

    public class Desconto_ComboModel : ObjComboResult<Int64?>
    {
        public virtual String Percentual_Desconto { get; set; }
        public virtual String Valor_Desconto { get; set; }
        public virtual String Tipo_Desconto { get; set; }
    }

    

}