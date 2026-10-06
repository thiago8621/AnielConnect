using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class EnviarEmailRequestModel: ApiRequest
    {
        public virtual string assunto { get; set; }
        public virtual string mensagem { get; set; }
        public virtual string email_destinatario { get; set; }
    }

    public class EnviarEmailConfigCustomizadaRequestModel: EnviarEmailRequestModel
    {
        public virtual string email_remetente { get; set; }               
        public virtual string senha_remetente { get; set; }
        public virtual bool utiliza_ssl_smtp { get; set; }
        public virtual string servidor_smtp { get; set; }
        public virtual int? porta_smtp { get; set; }
    }

    public class EnviarEmailResponseModel
    {
        public virtual bool enviado { get; set; }
    }
}