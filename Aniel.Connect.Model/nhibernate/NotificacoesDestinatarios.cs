

using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class NotificacoesDestinatarios : BaseModel
    {
        public virtual enumerado.TipoUsuarioEnum Tipo_Destinatario { get; set; }
        public virtual Notificacoes Notificacoes { get; set; }
        public virtual string Usuario_Destinatario { get; set; }
    }
}
