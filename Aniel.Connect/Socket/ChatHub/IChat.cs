using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Socket.ChatHub
{
    public interface IChat
    {
        IHubContext HubContext { get; set; }
        Task StartChat(AtendimentoChat chat);
        Task StartChatAvulso(AtendimentoChat chat);
        Task<ResponseEnviarNotificacaoSuporte> EnviarNotificacaoSuporte(AtendimentoChat chat, Boolean reincidente = false, String SuporteReincidente = null);
        Task<Response> EnviarMensagem(String protocolo, String m);
        Task SendFromClient(AtendimentoChat chat, String mensagem);
    }

    public class ResponseEnviarNotificacaoSuporte : Response 
    {
        public bool notificacaoEnviada { get; set; }
    }

}
