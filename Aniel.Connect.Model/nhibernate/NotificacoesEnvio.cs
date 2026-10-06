using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class NotificacoesEnvio : BaseModel
    {

        public virtual String Texto_Notificacao { get; set; }

        public virtual Status_Notificacao? Status_Notificacao { get; set; }
        public virtual DateTime Data_Envio { get; set; }
        public virtual String Usuario_Destino { get; set; }
        public virtual Notificacoes_Acao_Executada? Acao { get; set; }
        public virtual Notificacoes Notificacoes { get; set; }
        public virtual String Chave { get; set; }
    }
}
