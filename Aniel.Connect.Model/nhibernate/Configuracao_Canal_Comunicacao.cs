using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Configuracao_Canal_Comunicacao : BaseModel
    {
        public virtual bool MostrarNomeAtendente { get; set; }
        public virtual bool MostrarNomeAtendenteTelegram { get; set; }
        public virtual bool MostrarNomeAtendenteMessenger { get; set; }

        public virtual String NomePrincipal { get; set; }

        public virtual String NomeSecundario { get; set; }
    }
}
