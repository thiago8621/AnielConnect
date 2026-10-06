using Aniel.Connect.Core;
using DocumentFormat.OpenXml.Wordprocessing;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace Aniel.Connect.Models
{

    public class ChatListaModel
    {

    }

    public class MensagemListaModel
    {
        public virtual Int64? Id_Sala { get; set; }
        public virtual Int64 Id_Mensagem { get; set; }

        public virtual String Num_Identificador { get; set; }
        public virtual String Mensagem { get; set; }
        public virtual String Remetente { get; set; }
        public virtual String Data_Envio { get; set; }

    }

    public class QtdMensagensModel
    {
        public virtual Int64 Qtd_Mensagens { get; set; }
    }

    public class ChatModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Tabela { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Cod_ct { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_doc { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Avatar { get; set; }

    }

    public class ChatBackOfficeModel
    {
        public virtual bool bloqueiaMensagem { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Tipo_Usuario { get; set; }
        public virtual String Cod_ct { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_doc { get; set; }

    }

    public class User_CustomizacaoModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "AVATAR", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Avatar { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Nome { get; set; }

        [MaxLength(400, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "TEXTO_INVALIDO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string TextoInvalido { get; set; }
        public virtual Boolean Transferencia { get; set; }
        public virtual int? Minutos { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Boolean Habilitado { get; set; }

        public virtual List<HttpPostedFileBase> Files { get; set; }
    }

    public class Conversas_DataTableRequest : DataTableRequest
    {
        public virtual string numOS { get; set; }
        public virtual String dataConversa { get; set; }
        public virtual String dataInicio { get; set; }
        public virtual String dataFim { get; set; }
        public virtual String temAcesso { get; set; }
        public virtual String usuario { get; set; }

    }

    public class AgendamentoChat
    {
        public virtual string CodCt { get; set; }
        public virtual string num_obra { get; set; }
        public virtual string num_doc { get; set; }
        public virtual string projeto { get; set; }
        public virtual string data { get; set; }
        public virtual string hora { get; set; }
        public virtual string turno { get; set; }
    } 

    public class Conversas_ListModel
    {
        public virtual string numOS { get; set; }
        public virtual string codCt { get; set; }
        public virtual string projeto { get; set; }
        public virtual string numDoc { get; set; }
        public virtual String dataConversa { get; set; }
        public virtual int? totalDiasUltimaMensagem { get; set; }

    }

    public class DadosOS_ListModel
    {
        public virtual string numOS { get; set; }
        public virtual string dataAgendamento { get; set; }
        public virtual string horaInicio { get; set; }
        public virtual string horaFim { get; set; }
        public virtual string num_doc { get; set; }
        public virtual string tecnico { get; set; }
        public virtual string tipo_usuario { get; set; }
        public virtual string dataDesvinculacao { get; set; }
        public virtual bool usuario_ativo { get; set; }
        //public virtual ServicoUsuario? usuario { get; set; }
        public virtual string turno { get; set; }
        public virtual string status_execucao { get; set; }
        public virtual string status_auditoria { get; set; }
        public virtual string codct { get; set; }
        public virtual string projeto { get; set; }
    }

}