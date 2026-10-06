using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using System.Linq;

namespace Aniel.Connect.Models
{
    public class Notificacoes_ListModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Descricao { get; set; }
        public virtual string Status { get; set; }
        public virtual string Mensagem { get; set; }

        public virtual string Notificacao { get; set; }
        public virtual string VariaveisDisponiveis { get; set; }
        public virtual string MomentoExecucao { get; set; }
        public virtual string AcaoExecutada { get; set; }

        public virtual string SecaoConnect { get; set; }

    }


    //elementos carregados na pagina cadastro / alteracao

    public class Notificacoes_CadastroModel : IViewModel<String>
    {
        public virtual String Id { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual String Notificacao { get; set; }
        public virtual Notificacoes_Mom_Execucao? MomentoExecucao { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Model.enumerado.SecaoConnectPerfilEnum? IdSecaoConnect { get; set; }
        public virtual String IdCadastro { get; set; }
        public virtual string Usuario { get; set; }

        //SERVIÇOS
        public virtual Boolean MonitorEquipe { get; set; }
        public virtual Boolean SupervisorEquipe { get; set; }

        //COMERCIAL
        public virtual Boolean SupervisorComercialResp { get; set; }
        public virtual Boolean VendedorResp { get; set; }
        public virtual Boolean TodosSupervisores { get; set; }
        public virtual Boolean TodosVendedores { get; set; }


        //ATENDIMENTO
        public virtual Boolean AtendenteResp { get; set; }
        public virtual Boolean SupervisorResp { get; set; }
        public virtual Boolean AtendentesAgentes { get; set; }
        public virtual Boolean TodosAtendentes { get; set; }
        public virtual Boolean TodosSupervisoresAtendentes { get; set; }

        public virtual int? TempoDisparoNotificacaoMinutos { get; set; }

        //IMPORTACAO
        public virtual Notificacoes_Acao_Executada? Acao_Executada { get; set; }

        private List<NotificacoesDestinatarios_ListModel> _destinatarios;
        public virtual List<NotificacoesDestinatarios_ListModel> Destinatarios
        {
            get
            {
                if (_destinatarios == null)
                    _destinatarios = new List<NotificacoesDestinatarios_ListModel>();
                return _destinatarios;
            }
            set
            {
                _destinatarios = value;
            }
        }


    }

    public class Notificacoes_DataTableRequest : DataTableRequest
    {
        public virtual string Notificacao { get; set; }
        public virtual String IdNotificacao { get; set; }
        public virtual String Status { get; set; }
    }


    public class NotificacoesDestinatarios_ListModel
    {
        public virtual Int64 Id { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string UsuarioNome { get; set; }
        public virtual string TipoUsuarioEnum { get; set; }
        public virtual Model.enumerado.TipoUsuarioEnum UsuarioEnum { get; set; }
    }



        


    public class NotificacoesDestinatarios_DataTableRequest : DataTableRequest
    {
        public virtual Int64 IdNotificacoes { get; set; }
        public string IdDestinatario { get; internal set; }
    }

    public class GetNotificacoesResponse
    {
        public virtual string Id { get; set; }

        public virtual string IdNotificacao { get; set; }
        public virtual string Acao { get; set; }

        public virtual String Texto_Notificacao { get; set; }
        public virtual Status_Notificacao Status_Notificacao { get; set; }
        public virtual string Data_Envio { get; set; }
        public virtual string Chave { get; set; }
    }


    public class RegrasNotificacaoOSTecnicoReponse
    {
        public bool RegraAtendimentoOSProximo { get; set; }
        public int? MinutosAtendimentoOSProximo { get; set; }
        public string MsgAtendimentoOSProximo { get; set; }
        public bool RegraSLAProximoVencimento { get; set; }
        public int? MinutosSLAProximoVencimento { get; set; }
        public string MsgSLAProximoVencimento { get; set; }
        public bool RegraPrazoEncerramentoOSVencido { get; set; }
        public int? MinutosPrazoEncerramentoOSVencido { get; set; }
        public string MsgPrazoEncerramentoOSVencido { get; set; }
        public bool RegraPrazoEncerramentoOSAVencer { get; set; }
        public int? MinutosPrazoEncerramentoOSAVencer { get; set; }
        public string MsgPrazoEncerramentoOSAVencer { get; set; }
        public DateTime? TempoExecucaoPadrao { get; set; }

        
    }

    public class NotificacaoAlertaSOSRequest
    {
        public virtual string mensagem { get; set; }
        public virtual string equipe { get; set; }
        public virtual string monitor { get; set; }
        public virtual string uuidNotificacao { get; set; }
    }

    public class ConfirmarCienciaSOSRequest
    {
        public virtual List<string> uuidNotificacao { get; set; }
        public virtual string equipeMonitor { get; set; }
    }
    public class NotificacaoTransferenciaChat
    {
        public int IdNotificacao { get; set; }
        public long IdAtendimento { get; set; }
    }
}
