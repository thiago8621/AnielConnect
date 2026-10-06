using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Repository.Model
{
    public class Forma_Pagamento_ClienteModels
    {
        public Int64 Id { get; set; }

        [Display(Name = "FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsFinanceiro))]
        public Int64? Id_Forma_Pagamento { get; set; }

        [Display(Name = "FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsFinanceiro))]
        public String Desc_Forma_Pagamento { get; set; }
        public Cliente_FormaEnvio? Forma_Envio { get; set; }

        public Status_Default? Status { get; set; }

        public String Desc_Forma_Envio { get; set; }

        [Display(Name = "VENCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Vencimento { get; set; }

        [Display(Name = "BANCO", ResourceType = typeof(Resources.RsCadastros))]
        public String Desc_Banco { get; set; }

        [Display(Name = "BANCO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Banco { get; set; }

        [Display(Name = "CONTA_CORRENTE", ResourceType = typeof(Resources.RsCadastros)),
        MaxLength(15, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Conta_Corrente { get; set; }

        [Display(Name = "AGENCIA", ResourceType = typeof(Resources.RsCadastros)),
        MaxLength(5, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Agencia { get; set; }

        [Display(Name = "DIGITO", ResourceType = typeof(Resources.RsCadastros)),
        MaxLength(1, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Digito { get; set; }

        public Int64? Id_Cliente { get; set; }

        public Int64? Id_PlanoProduto { get; set; }

        public String Dia_Vencimento { get; set; }

        public Boolean End_Cobranca_Igual_Principal { get; set; }

        #region EnderecoCobranca
        public Int64? Id_EnderecoCobranca { get; set; }

        public Int64? Id_Tipo_ImovelCobranca { get; set; }

        public Int64? Id_PaisCobranca { get; set; }

        public String LatitudeCobranca { get; set; }


        public String LongitudeCobranca { get; set; }


        public String CEPCobranca { get; set; }


        public String LogradouroCobranca { get; set; }


        public String NumeroCobranca { get; set; }


        public String BairroCobranca { get; set; }

        public Int64? Id_BairroCobranca { get; set; }

        public Int64? Id_CidadeCobranca { get; set; }

        public String CidadeCobranca { get; set; }

        public String UFCobranca { get; set; }

        public String UF_Tes { get; set; }

        public String ComplementoCobranca { get; set; }

        public String Ponto_ReferenciaCobranca { get; set; }

        public Tipo_Forma_Pagamento? Tipo_Forma_Pagamento { get; set; }
        public bool Selecionado { get; set; }

        public Boolean Origem_Mobile { get; set; }

        #endregion
    }
}
