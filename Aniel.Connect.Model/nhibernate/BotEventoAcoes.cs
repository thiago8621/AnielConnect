

using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class BotEventoAcoes : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual bool UtilizaCondicao { get; set; }
        public virtual String CampoConsultar { get; set; }
        public virtual BotEvento EventoCondPositivo { get; set; }
        public virtual BotEvento EventoCondNegativo { get; set; }
        public virtual Status_Default Status { get; set; }

    }
}
