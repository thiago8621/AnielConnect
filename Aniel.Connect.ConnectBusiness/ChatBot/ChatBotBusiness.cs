using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness
{
    public class ChatBotBusiness
    {
        public Bot Bot { get; set; }
        public BotEvento EventoAtual { get; set; }

        public ChatBotBusiness(Bot bot, BotEvento evento)
        {
            this.Bot = bot;
            this.EventoAtual = evento;
        }

        public Response<List<String>> RunBot(string input)
        {
            Response<List<string>> retorno = new Response<List<string>>();
            List<string> mensagens = new List<string>();
            string retornoBot = null;
            int i = 0;
            do
            {
                var retornoSequencia = this.SequenciaBot(i == 0 ? input : "");
                retornoBot = retorno.message;

                if (!String.IsNullOrEmpty(retornoBot))
                    mensagens.Add(retornoBot);
            }
            while (retornoBot == null);

            return retorno;

        }



        public Response SequenciaBot(string input)
        {
            try
            {
                BotEvento evento = GetProximoEvento(input);
                MudarEventoAtual(evento);
                return new Response
                {
                    result = true,
                    message = GetTexto(evento),
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    exception = ex.Message,
                    result = false,
                    stacktrace = ex.StackTrace
                };
            }
        }
        private BotEvento GetProximoEvento(string input)
        {
            if (EventoAtual == null)
            {
                var eventoInicial = Bot.Eventos.FirstOrDefault(p => p.EventoInicial);
                if (eventoInicial == null)
                {
                    throw new Exception("Nenhum Evento Inicial Encontrado");
                }
                else
                {
                    return eventoInicial;
                }
            }
            if (EventoAtual.DisparoAutomatico)
            {
                return EventoAtual.AutoProximoEvento;
            }
            else if (EventoAtual.UtilizaAcao)
            {
                return ExecutarAcao();
            }
            else
            {
                if (!String.IsNullOrEmpty(input))
                {
                    foreach (var item in EventoAtual.Respostas)
                    {
                        List<string> respostasPosiveis = item.Descricao.Split(';').ToList();
                        foreach (var resposta in respostasPosiveis)
                        {
                            if (input.Contains(resposta))
                            {
                                return item.ProximoEvento;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private BotEvento ExecutarAcao()
        {
            throw new NotImplementedException();
        }

        private void MudarEventoAtual(BotEvento evento)
        {
            this.EventoAtual = evento;
        }

        private string GetTexto(BotEvento evento)
        {
            return evento?.TextoEnviado;
        }
    }
}
