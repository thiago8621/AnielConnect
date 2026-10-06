using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Canal_ComunicacaoMap : ClassMapBase<Canal_Comunicacao>
    {
        public Canal_ComunicacaoMap() : base()
        {
            Table("TB_CANAL_COMUNICACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CANAL_COMUNICACAO");
            Map(x => x.AccountId, "ACCOUNT_ID").Length(100);
            Map(x => x.AuthToken, "AUTH_TOKEN").Length(100);
            Map(x => x.TwillioAccountId, "TWILLIO_ACCOUNTID ").Length(100);
            Map(x => x.TwilioAuthToken, "TWILLIO_AUTHTOKEN").Length(100);
            Map(x => x.TwilioServiceSID, "TWILIO_SERVICE_SID").Length(100);
            Map(x => x.TokenTelegram, "TELEGRAM_TOKEN").Length(250);
            Map(x => x.TokenMessenger, "MESSENGER_TOKEN").Length(250);
            Map(x => x.TokenInstagram, "INSTAGRAM_TOKEN").Length(250);
            Map(x => x.TokenSkype, "SKYPE_TOKEN").Length(250);


            Map(x => x.WhatsAppBusinessAccount, "WHATSAPP_BUSINESS_ACCOUNT").Length(250);
            Map(x => x.TokenWhatsAppBusiness, "WHATSAPP_BUSINESS_TOKEN").Length(250);
            Map(x => x.IdNumeroWhatsAppBusiness, "ID_NUM_WHATSAPP_BUSINESS").Length(250);
            Map(x => x.ModeloPadrao, "MODELO_PADRAO_WHATSAPP_BUS").Length(250);
            Map(x => x.IdModeloPadrao, "ID_MOD_PADRAO_WHATSAPP_BUS").Length(250);
            Map(x => x.StatusModeloPadrao, "STATUS_MOD_PADRAO_WHATSAPP_BUS").Length(250);

            Map(x => x.Imagem, "IMAGEM").Length(100);
            Map(x => x.Nome, "NOME").Length(100);
            Map(x => x.Descricao, "DESCRICAO").Length(400);
            Map(x => x.Email, "EMAIL").Length(100);
            Map(x => x.Site, "SITE").Length(100);
            Map(x => x.Numero, "NUMERO").Length(20);
            Map(x => x.Tipo_Canal_Comunicacao, "TIPO_CANAL").CustomType<Tipo_Canal_Comunicacao>();
            Map(m => m.Status, "STATUS").CustomType<Status_Default>();

            References(u => u.Bot, "ID_BOT").ForeignKey("FK_CANAL_COMUNICACAO");
        }
    }
}
