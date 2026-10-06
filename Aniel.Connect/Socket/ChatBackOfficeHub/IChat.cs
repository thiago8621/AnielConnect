using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace Aniel.Connect.Socket.ChatBackOfficeHub
{
    public interface IChat
    {
        IHubContext HubContext { get; set; }
        Task StartChat(ServicoChat chat);
        Task EnviarNotificacaoSuporte(ServicoChat chat, Boolean reincidente = false, String SuporteReincidente = null);
        Task EnviarMensagem(String num_os, String m);
        Task EnviarMensagemAnexo(String num_os, String mensagemMongoId);
        Task SendFromClient(ServicoChat chat, String mensagem);
        Task InativaAtivaUsuario(String grupo, String num_os, FuncionarioAniel funcionario);
    }
}
