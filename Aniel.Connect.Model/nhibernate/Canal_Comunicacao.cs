using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Canal_Comunicacao : BaseModel
    {
        public virtual String AccountId { get; set; }
        public virtual String AuthToken { get; set; }
        public virtual String TwillioAccountId { get; set; }
        public virtual String TwilioAuthToken { get; set; }
        public virtual String TwilioServiceSID { get; set; }
        public virtual String TokenTelegram { get; set; }
        public virtual String TokenSkype { get; set; }
        public virtual String TokenMessenger { get; set; }
        public virtual String TokenInstagram { get; set; }
        public virtual String Imagem { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Email { get; set; }
        public virtual String Site { get; set; }
        public virtual String Numero { get; set; }
        public virtual String WhatsAppBusinessAccount { get; set; }
        public virtual String TokenWhatsAppBusiness { get; set; }
        public virtual String IdNumeroWhatsAppBusiness { get; set; }
        public virtual String ModeloPadrao { get; set; }
        public virtual String IdModeloPadrao { get; set; }
        public virtual String StatusModeloPadrao { get; set; }
        public virtual Bot Bot { get; set; }
        public virtual Tipo_Canal_Comunicacao Tipo_Canal_Comunicacao { get; set; }
        public virtual Status_Default Status { get; set; }

    }
}
