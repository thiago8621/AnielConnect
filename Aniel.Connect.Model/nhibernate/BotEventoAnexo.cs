

using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class BotEventoAnexo : BaseModel
    {
        public virtual String Nome { get; set; }
        public virtual String Extensao { get; set; }
        public virtual BotEvento BotEvento { get; set; }

        public virtual Tipo_Bot_Evento_Anexo Tipo { get; set; }

        public virtual string Url { get; set; }
    }
}
