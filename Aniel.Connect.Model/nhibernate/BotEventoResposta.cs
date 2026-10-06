

using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class BotEventoResposta : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual BotEvento BotEvento { get; set; }

        public virtual BotEvento ProximoEvento { get; set; }
    }
}
