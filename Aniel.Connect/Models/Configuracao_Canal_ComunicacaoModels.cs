using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Configuracao_Canal_ComunicacaoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Nome_Contato NomePrincipal { get; set; }
        public virtual Nome_Contato NomeSecundario { get; set; }
        public virtual Boolean MostrarNomeAtendente { get; set; }
        public virtual Boolean MostrarNomeAtendenteTelegram { get; set; }
        public virtual Boolean MostrarNomeAtendenteMessenger { get; set; }
        public virtual Boolean MostrarNomeAtendenteInstagram { get; set; }
        public virtual string Imagem { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Email { get; set; }
        public virtual string Site { get; set; }
        public virtual string Numero { get; set; }
        public virtual Int64? IdBot { get; set; }
        public virtual Bot Bot { get; set; }
        public Tipo_Canal_Comunicacao TipoCanalComunicacao { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual List<HttpPostedFileBase> Files { get; set; }

        public virtual string TwillioAccountId { get; set; }
        public virtual string TwilioAuthToken { get; set; }
        public virtual string TokenTelegram { get; set; }
        public virtual string TokenMessenger { get; set; }
        public virtual string TokenInstagram { get; set; }
        public virtual string TokenSkype { get; set; }

        public virtual string TokenWhatsAppBusiness { get; set; }
        public virtual string IdNumeroWhatsAppBusiness { get; set; }
        public virtual string WhatsAppBusinessAccount { get; set; }
        public virtual string ModeloPadraoWhatsApp { get; set; }
        public virtual string IdModeloPadraoWhatsApp { get; set; }
        public virtual string StatusModeloPadraoWhatsApp { get; set; }
        public virtual string TwilioServiceSID { get; set; }
    }

}