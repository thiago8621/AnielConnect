using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class Notificacoes : BaseModel
    {

        public virtual String Notificacao { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Notificacoes_Variaveis VariaveisDisponiveis { get; set; }
        public virtual Notificacoes_Mom_Execucao MomentoExecucao { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual Notificacoes_Acao_Executada AcaoExecutada { get; set; }
        public virtual enumerado.SecaoConnectPerfilEnum SecaoConnect { get; set; }
        public virtual double? FrequenciaNotificaoMinutos { get; set; }

        /// <summary>
        /// Tempo em minutos para a notificação ser disparada de acordo com regras específicas a ela. 
        /// Ex: Notificação de vencimento de SLA da O.S no Touch, disparar 20 minutos antes do SLA vencer.
        /// </summary>
        public virtual int? TempoDisparoNotificacaoMinutos { get; set; }


        private IList<NotificacoesDestinatarios> _destinatarios;
        public virtual IList<NotificacoesDestinatarios> Destinatarios
        {
            get
            {
                if (_destinatarios == null)
                    _destinatarios = new List<NotificacoesDestinatarios>();
                return _destinatarios;
            }
            set
            {
                _destinatarios = value;
            }
        }
    }
}
