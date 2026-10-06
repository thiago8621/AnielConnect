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
    public class Canal_ComunicacaoListModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64? IdBot { get; set; }
        public virtual string Imagem { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Email { get; set; }
        public virtual string Site { get; set; }
        public virtual string Numero { get; set; }
        public virtual string BotVinculado { get; set; }
        public virtual string StatusAPI { get; set; }
        public virtual string StatusNumero { get; set; }
        public virtual string TipoCanalComunicacao { get; set; }
        public virtual Int64 IdTipoCanalComunicacao { get; set; }

        public virtual string TwillioAccountId { get; set; }
        public virtual string TwilioAuthToken { get; set; }
        public virtual string TokenTelegram { get; set; }
        public virtual string TokenMessenger { get; set; }
        public virtual string TokenInstagram { get; set; }
        public virtual string TokenWhatsAppBusiness { get; set; }
        public virtual string IdNumeroWhatsAppBusiness { get; set; }
        public virtual string WhatsAppBusinessAccount { get; set; }
        public virtual string ModeloPadraoWhatsApp { get; set; }
        public virtual string IdModeloPadraoWhatsApp { get; set; }
        public virtual string StatusModeloPadraoWhatsApp { get; set; }
        public virtual string TwilioServiceSID { get; set; }
        public virtual string TokenSkype { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual List<HttpPostedFileBase> Files { get; set; }
    }

    public class Canal_Comunicacao_CadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "IMAGEM", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Imagem { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Nome { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Descricao { get; set; }
        public virtual string Email { get; set; }
        public virtual string Site { get; set; }
        public virtual string Numero { get; set; }
        public virtual Int64? IdBot { get; set; }
        public virtual Bot Bot { get; set; }
        public Tipo_Canal_Comunicacao TipoCanalComunicacao { get; set; }

        public virtual List<HttpPostedFileBase> Files { get; set; }

        public virtual string TwillioAccountId { get; set; }
        public virtual string TwilioAuthToken { get; set; }
        public virtual string TokenTelegram { get; set; }
        public virtual string TokenMessenger { get; set; }
        public virtual string TokenInstagram { get; set; }
        public virtual string TokenWhatsAppBusiness { get; set; }
        public virtual string IdNumeroWhatsAppBusiness { get; set; }
        public virtual string WhatsAppBusinessAccount { get; set; }
        public virtual string ModeloPadraoWhatsApp { get; set; }
        public virtual string IdModeloPadraoWhatsApp { get; set; }
        public virtual string StatusModeloPadraoWhatsApp { get; set; }
        public virtual string TwilioServiceSID { get; set; }
        public virtual string TokenSkype { get; set; }
        public virtual Boolean Status { get; set; }

    }

    public class ModeloMensagemRequest
    {
        public string idCanalComunicacao { get; set; }
        public string idModeloMensagem { get; set; }
        public string token { get; set; }
        public string whatsAppBusinessAccount { get; set; }
        public string mensagem { get; set; }
    }


    public class Canal_Comunicacao_DataTableRequest : DataTableRequest
    {
        public virtual string Nome { get; set; }
        public virtual Int64 Id { get; set; }

        public virtual String tipoCanal { get; set; }
    }
}