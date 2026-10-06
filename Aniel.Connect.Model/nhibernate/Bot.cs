using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class Bot : BaseModel
    {
        public virtual String Avatar { get; set; }
        public virtual String Nome { get; set; }
        public virtual String TextoInvalido { get; set; }
        public virtual Boolean? Transferencia { get; set; }
        public virtual int? Minutos { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual Habilitado_Bot Habilitado { get; set; }

        private IList<BotEvento> _eventos;
        public virtual IList<BotEvento> Eventos
        {
            get
            {
                if (_eventos == null)
                    _eventos = new List<BotEvento>();
                return _eventos;
            }
            set
            {
                _eventos = value;
            }
        }
    }
}
