using Aniel.Connect.Acoes;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.Ativacao;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.Ativacao;
using Aniel.T5.Model.nhibernate;
using Microsoft.Ajax.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Aniel.Connect.Socket.ChatHub
{
    public class ChatBotBusiness
    {
        private AtendimentoChatMongo AtendimentoChatMongo;
        public Bot Bot { get; set; }
        public BotEvento EventoAtual { get; set; }
        public LogExecucao LogExecucao { get; set; }

        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        public ChatBotBusiness(Bot bot, BotEvento evento, LogExecucao logExecucao, AtendimentoChatMongo chat)
        {
            this.Bot = bot;
            this.EventoAtual = evento;
            this.LogExecucao = logExecucao;
            this.AtendimentoChatMongo = chat;
        }

        public async Task<Response<List<RetornoSequenciaBot>>> RunBot(string input)
        {
            Response<List<RetornoSequenciaBot>> retorno = new Response<List<RetornoSequenciaBot>>();
            List<RetornoSequenciaBot> mensagens = new List<RetornoSequenciaBot>();
            bool continuar;

            if (this.EventoAtual?.VariaveisDisponiveis == null)
            {
                string variaveis = string.Join(" ", Enum.GetNames(typeof(EventoVariaveis)).Select(item => "@" + item));

                if (this.EventoAtual != null)
                    this.EventoAtual.VariaveisDisponiveis = variaveis;
            }

            if (this.EventoAtual?.EventoArmazenamento != null)
            {
                ArmazenamentoEvento(this.EventoAtual, input);
            }

            int i = 0;
            do
            {
                continuar = false;

                var eventoAnterior = this.EventoAtual;
                var retornoSequencia = this.SequenciaBot(i == 0 ? input : "");

                if (!string.IsNullOrEmpty(retornoSequencia.texto))
                {
                    mensagens.Add(retornoSequencia);
                }

                if (this.EventoAtual.UtilizaAcao)
                {
                    var resultadoAcao = await ExecutarAcao(this.EventoAtual.AcaoExecutada, input);

                    mensagens.AddRange(resultadoAcao.mensagens.Select(p => new RetornoSequenciaBot
                    {
                        texto = p,
                        result = true,
                        anexos = this.EventoAtual.Anexos.Concat(resultadoAcao.anexos).ToList()
                    }));

                    if (resultadoAcao.evento != null)
                    {
                        MudarEventoAtual(resultadoAcao.evento);

                        string textoEventoAcao = GetTexto(resultadoAcao.evento);
                        mensagens.Add(new RetornoSequenciaBot
                        {
                            texto = textoEventoAcao,
                            anexos = resultadoAcao.evento.Anexos.ToList(),
                            result = true
                        });

                        if (this.EventoAtual.UtilizaAcao)
                        {
                            continuar = true;
                        }
                        else
                        {
                            continuar = this.EventoAtual.DisparoAutomatico;
                        }

                        //continuar = this.EventoAtual.DisparoAutomatico;
                        //continuar = false;
                    }
                }
                else if (this.EventoAtual.DisparoAutomatico)
                {
                    continuar = true;
                }

                i++;
            }
            while (continuar);

            retorno.valor = mensagens;
            return retorno;
        }


        public RetornoSequenciaBot SequenciaBot(string input)
        {
            try
            {

                BotEvento evento = GetProximoEvento(input);

                if (evento != null)
                {
                    MudarEventoAtual(evento);
                    return new RetornoSequenciaBot
                    {
                        result = true,
                        texto = GetTexto(evento),
                        anexos = evento.Anexos.ToList()
                    };
                }
                else
                {
                    if (this.EventoAtual.UtilizaAcao == false)
                    {
                        return new RetornoSequenciaBot
                        {
                            result = true,
                            texto = GetTextoInvalido(Bot),
                        };
                    }
                    else
                    {
                        return new RetornoSequenciaBot
                        {
                            result = true,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogExecucao.RegistraLog("Erro: " + ex.Message + "INNER EX:" + ex.InnerException?.Message + " STACK:" + ex.StackTrace);
                return new RetornoSequenciaBot
                {
                    exception = ex.Message,
                    result = false,
                    stackTrace = ex.StackTrace,
                };
            }
        }
        private BotEvento GetProximoEvento(string input)
        {
            var eventoInicial = Bot.Eventos.FirstOrDefault(p => p.EventoInicial);
            if (this.EventoAtual == null)
            {
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
            //else if (EventoAtual.UtilizaAcao)
            //{
            //}
            else
            {
                if (!String.IsNullOrEmpty(input))
                {
                    var retornoDefault = true;
                    foreach (var item in (EventoAtual.Respostas ?? new List<BotEventoResposta>()))
                    {
                        List<string> respostasPosiveis = item.Descricao.Split(';')
                                                                      .Select(r => r.Trim())
                                                                      .ToList();

                        foreach (var resposta in respostasPosiveis)
                        {
                            if (string.Equals(input, resposta, StringComparison.OrdinalIgnoreCase))
                            {
                                retornoDefault = false;
                                return item.ProximoEvento;
                            }
                        }
                    }

                    if (retornoDefault == true)
                    {
                        var resposta_possiveis = EventoAtual.Respostas == null ? new List<BotEventoResposta>() : EventoAtual.Respostas;
                        var respostas_any = resposta_possiveis.Where(x => x.Descricao == "(ANY)").FirstOrDefault();
                        if (respostas_any != null)
                        {
                            retornoDefault = false;
                            return respostas_any.ProximoEvento;
                        }
                    }

                    if (retornoDefault == true)
                    {
                        //caso associe uma variável a uma respota futuramente, só adicionar a verificação
                        foreach (var item in (EventoAtual.Respostas == null ? new List<BotEventoResposta>() : EventoAtual.Respostas))
                        {
                            List<string> respostasPosiveis = item.Descricao.Split(';').Select(r => r.Trim()).ToList();

                            foreach (var resposta in respostasPosiveis)
                            {
                                if (EventoAtual.VariaveisDisponiveis.Contains(resposta.ToUpper()))
                                {
                                    //Armazena a variável JSON
                                    var obj = new List<VariavelArmazenamento>();
                                    obj.Add(new VariavelArmazenamento
                                    {
                                        nomeVariavel = resposta.ToUpper(),
                                        valor = input.ToUpper()
                                    });

                                    this.AtendimentoChatMongo.JsonArmazenamento = JsonService.SerializeObject(obj);

                                    retornoDefault = false;
                                    return item.ProximoEvento;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private async Task<(BotEvento evento, List<string> mensagens, List<BotEventoAnexo> anexos)> ExecutarAcao(Tipo_Bot_Evento_Acoes? tipo_Bot, string valorEntrada)
        {

            BotEvento botEvento = null;
            List<string> mensagens = new List<string>();
            List<BotEventoAnexo> anexos = new List<BotEventoAnexo>();


            try
            {
                var acoesCliente = new ClienteAcoes();
                var clienteApi = new ClienteApiController();
                var pesquisa = new Pesquisa_SatisfacaoApiController();
                var acoes = new Acoes.AtendimentoAcoes();
                var consultaOs = new Documento_ProducaoApiController();
                var areaManutencao = new Area_ManutencaoApiController();
                var planoApi = new PlanoProdutoApiController();
                var boletoApi = new ContaFinanceiraApiController();
                var integrRadiusApi = new IntegracaoRadiusApiController();
                var integrAtivacaoApi = new IntegracaoAtivacaoApiController();
                var modeloContratoApi = new Modelo_ContratoApiController();
                var ONUApi = new ListagemONUApiController();
                var clienteContratoApi = new ClienteContratoApiController();

                ////VALIDA SE JÁ ESTÁ SALVO OS DADOS DO USUÁRIO
                //if (this.AtendimentoChatMongo.cliente.idCliente == null)
                //{
                //    using (var conexaoMongo = new ConexaoMongo())
                //    using (var session = HibernateUtil.GetSession())
                //    using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                //    using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                //    using (var repositoryCliente = new Conexao<Cliente>(session))
                //    {
                //        var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);

                //        if (atendimento?.Cliente?.Id != null)
                //        {
                //            var dadosCliente = await repositoryCliente.selectFirst(p => p.Id == atendimento.Cliente.Id);

                //            var chat = await repository.Find(p => p.protocolo == this.AtendimentoChatMongo.protocolo);

                //            AtendimentoChatMongo chatMongo = await repository.Find(x => x.protocolo == this.AtendimentoChatMongo.protocolo);

                //            if (chatMongo != null)
                //            {
                //                chatMongo.cliente.cpf_cnpj = dadosCliente.CpfCnpj;
                //                chatMongo.cliente.email = dadosCliente.Email_Principal;
                //                chatMongo.cliente.idCliente = dadosCliente.Id;
                //                chatMongo.cliente.idCategoriaCliente = dadosCliente.Categoria_Cliente.Id;
                //                chatMongo.cliente.nome = dadosCliente.NomeFantasia;
                //                await repository.Update(p => p.protocolo == this.AtendimentoChatMongo.protocolo, chatMongo);
                //            }
                //        }

                //    }
                //}

                switch (tipo_Bot)
                {

                    case Tipo_Bot_Evento_Acoes.EV_VALIDA_CPF:
                        if (this.AtendimentoChatMongo.cliente.cpf_cnpj == null)
                        {
                            var existeNull = clienteApi.Existecpf(valorEntrada).Result;
                            if (existeNull > 0)
                            {
                                ClienteChat_Mobile dadosCliente = clienteApi.ClienteChat(valorEntrada, this.AtendimentoChatMongo).Result;

                                using (var conexaoMongo = new ConexaoMongo())
                                using (var session = HibernateUtil.GetSession())
                                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                                using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                                using (var repositoryCliente = new Conexao<Cliente>(session))
                                {
                                    var chat = await repository.Find(p => p.protocolo == this.AtendimentoChatMongo.protocolo);

                                    AtendimentoChatMongo chatMongo = await repository.Find(x => x.protocolo == this.AtendimentoChatMongo.protocolo);

                                    if (chatMongo != null)
                                    {
                                        chatMongo.cliente.cpf_cnpj = dadosCliente.cpf_cnpj;
                                        chatMongo.cliente.email = dadosCliente.email;
                                        chatMongo.cliente.idCliente = dadosCliente.idCliente;
                                        chatMongo.cliente.idCategoriaCliente = dadosCliente.idCategoriaCliente;
                                        chatMongo.cliente.nome = dadosCliente.nome;
                                        await repository.Update(p => p.protocolo == this.AtendimentoChatMongo.protocolo, chatMongo);
                                    }

                                    Atendimento atendimento = await repositoryAtendimento.selectFirst(p => p.Numero == this.AtendimentoChatMongo.protocolo);
                                    if (atendimento != null)
                                    {
                                        atendimento.Cliente = await repositoryCliente.selectFirst(p => p.Id == dadosCliente.idCliente);
                                        atendimento.Protocolo = this.AtendimentoChatMongo.protocolo;
                                        await repositoryAtendimento.SaveOrUpdate(atendimento);
                                    }

                                }

                                botEvento = this.EventoAtual.EventoVerdadeiro;
                            }
                            else
                            {
                                using (var conexaoMongo = new ConexaoMongo())
                                using (var session = HibernateUtil.GetSession())
                                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                                using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                                using (var repositoryCliente = new Conexao<Cliente>(session))
                                {
                                    var chat = await repository.Find(p => p.protocolo == this.AtendimentoChatMongo.protocolo);

                                    if (chat != null)
                                    {
                                        chat.cliente.cpf_cnpj = valorEntrada;
                                        await repository.Update(p => p.protocolo == this.AtendimentoChatMongo.protocolo, chat);
                                    }
                                }
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                        }
                        else
                        {
                            var existe = clienteApi.Existecpf(this.AtendimentoChatMongo.cliente.cpf_cnpj).Result;
                            if (this.EventoAtual.CondicaoEvento)
                            {
                                if (existe > 0)
                                {
                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                }
                                else
                                {
                                    botEvento = this.EventoAtual.EventoFalso;
                                }
                            }
                        }
                        break;

                    //Verifica se numeração digitada é válida
                    case Tipo_Bot_Evento_Acoes.EV_EXISTE_CPF:

                        if (this.AtendimentoChatMongo.cliente.cpf_cnpj == null)
                        {
                            var validaCPF = acoesCliente.ValidaCpf(valorEntrada);
                            if (validaCPF == "Cpf Válido")
                            {
                                ClienteChat_Mobile dadosCliente = clienteApi.ClienteChat(valorEntrada, this.AtendimentoChatMongo).Result;

                                using (var conexaoMongo = new ConexaoMongo())
                                using (var session = HibernateUtil.GetSession())
                                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                                using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                                using (var repositoryCliente = new Conexao<Cliente>(session))
                                {
                                    var chat = await repository.Find(p => p.protocolo == this.AtendimentoChatMongo.protocolo);

                                    if (chat != null)
                                    {
                                        chat.cliente.cpf_cnpj = dadosCliente.cpf_cnpj;
                                        chat.cliente.email = dadosCliente.email;
                                        chat.cliente.idCliente = dadosCliente.idCliente;
                                        chat.cliente.idCategoriaCliente = dadosCliente.idCategoriaCliente;

                                        repository.Update(p => p.protocolo == this.AtendimentoChatMongo.protocolo, chat).Wait();
                                    }

                                    Atendimento atendimento = await repositoryAtendimento.selectFirst(p => p.Numero == this.AtendimentoChatMongo.protocolo);
                                    if (atendimento != null)
                                    {
                                        atendimento.Cliente = await repositoryCliente.selectFirst(p => p.Id == dadosCliente.idCliente);
                                        atendimento.Protocolo = this.AtendimentoChatMongo.protocolo;
                                        await repositoryAtendimento.SaveOrUpdate(atendimento);
                                    }
                                }

                                botEvento = this.EventoAtual.EventoVerdadeiro;
                            }
                            else
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                        }
                        else
                        {
                            var cpfCnpj = acoesCliente.ValidaCpf(this.AtendimentoChatMongo.cliente.cpf_cnpj);
                            if (this.EventoAtual.CondicaoEvento)
                            {
                                if (cpfCnpj == "Cpf Válido")
                                {
                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                }
                                else
                                {
                                    botEvento = this.EventoAtual.EventoFalso;
                                }
                            }
                        }

                        break;

                    //Valida se é PF ou PJ
                    case Tipo_Bot_Evento_Acoes.EV_VALIDA_TIPO_PESSOA:
                        if (this.AtendimentoChatMongo.cliente.idCliente == null)
                        {
                            botEvento = this.EventoAtual.EventoFalso;
                        }
                        else
                        {
                            var tipoPessoa = acoesCliente.FindTipoPessoa(this.AtendimentoChatMongo.cliente.idCliente.Value).Result;
                            if (this.EventoAtual.CondicaoEvento)
                            {
                                if (tipoPessoa == "Pessoa Jurídica")
                                {
                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                }
                                else
                                {
                                    botEvento = this.EventoAtual.EventoFalso;
                                }
                            }
                        }

                        break;

                    //Consultar vendas com situação diferente de cancelada e concluídas
                    case Tipo_Bot_Evento_Acoes.EV_CONSULTA_VENDA:
                        using (var session = HibernateUtil.GetSession())
                        using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                        {
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);

                            if (atendimento?.Cliente?.Id == null)
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                            else
                            {
                                mensagens = clienteApi.consultaVenda(atendimento?.Cliente?.Id.ToString()).Result;

                                if (this.EventoAtual.CondicaoEvento)
                                {
                                    if (mensagens.Count > 0)
                                    {
                                        botEvento = this.EventoAtual.EventoVerdadeiro;
                                    }
                                    else
                                    {
                                        botEvento = this.EventoAtual.EventoFalso;
                                    }
                                }

                            }

                        }
                        break;

                    //Enviar contrato
                    case Tipo_Bot_Evento_Acoes.EV_ENVIA_CONTRATO:

                        using (var session = HibernateUtil.GetSession())
                        using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                        {
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);

                            if (atendimento?.Cliente?.Id == null)
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                            else
                            {
                                RetornoSequenciaBot retornoBot = modeloContratoApi.GetContratoCliente(atendimento?.Cliente?.Id).Result;
                                mensagens = retornoBot.sequenciaMensagens.ToList();
                                anexos = retornoBot.anexos;

                                if (this.EventoAtual.CondicaoEvento)
                                {
                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                }

                            }
                        }
                        break;

                    //Transfere o atendimento para o setor selecionado
                    case Tipo_Bot_Evento_Acoes.EV_CANCELA_PLANO:
                        var value = new TransferirAtendimentoRequestModel
                        {
                            protocolo = this.AtendimentoChatMongo.protocolo,
                            usuario = "",
                            novoAtendente = "",
                            novoDepartamento = this.EventoAtual.SetorAtendimento.Id,
                        };
                        acoes.TransferenciaAtendente(value).Wait();

                        await ChatHub.EnviarNotificacaoSuporte(AtendimentoChatMongo, false, null);

                        if (this.EventoAtual.CondicaoEvento)
                        {
                            botEvento = this.EventoAtual.EventoVerdadeiro;
                        }
                        break;

                    //Consulta se cliente está bloqueado
                    case Tipo_Bot_Evento_Acoes.EV_CONSULTA_BLOQUEIO:
                        using (var session = HibernateUtil.GetSession())
                        using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                        {
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);

                            if (atendimento?.Cliente?.Id == null)
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                            else
                            {
                                var consultaBloqueio = clienteApi.ClienteBloqueado(atendimento?.Cliente?.Id.ToString()).Result;

                                if (this.EventoAtual.CondicaoEvento)
                                {
                                    if (consultaBloqueio > 0)
                                    {
                                        botEvento = this.EventoAtual.EventoVerdadeiro;
                                    }
                                    else
                                    {
                                        botEvento = this.EventoAtual.EventoFalso;
                                    }
                                }
                            }
                        }

                        break;

                    //Consulta as faturas com status diferente de baixado, cancelado ou suspenso e que não possuem data de recebimento
                    case Tipo_Bot_Evento_Acoes.EV_FATURAS_ABERTO:
                        if (this.AtendimentoChatMongo.cliente.idCliente == null)
                        {
                            botEvento = this.EventoAtual.EventoFalso;
                        }
                        else
                        {
                            mensagens = acoesCliente.GetAllFaturasAberto(Convert.ToString(this.AtendimentoChatMongo.cliente.idCliente)).Result;
                            if (this.EventoAtual.CondicaoEvento)
                            {
                                botEvento = this.EventoAtual.EventoVerdadeiro;
                            }
                        }
                        break;

                    //Envia Pesquisa selecionada
                    case Tipo_Bot_Evento_Acoes.EV_ENVIA_PESQUISA:

                        using (var session = HibernateUtil.GetSession())
                        using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                        {
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);
                            pesquisa.Enviar(Convert.ToInt64(atendimento?.Cliente?.Id), this.EventoAtual.PesquisaSatisfacao.Id, "ADMIN", atendimento.Id).Wait();
                            if (this.EventoAtual.CondicaoEvento)
                            {
                                botEvento = this.EventoAtual.EventoVerdadeiro;
                            }
                        }
                        break;

                    //Finalizar Antendimento
                    case Tipo_Bot_Evento_Acoes.EV_FINALIZA_ATENDIMENTO:
                        this.AtendimentoChatMongo.dataEncerramento = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        acoes.ConcluirAtendimento(this.AtendimentoChatMongo.protocolo).Wait();
                        break;

                    //Consultar Ordem de Serviço
                    case Tipo_Bot_Evento_Acoes.EV_CONSULTA_OS:
                        using (var session = HibernateUtil.GetSession())
                        using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                        {
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);

                            if (atendimento?.Cliente?.Id == null)
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                            else
                            {
                                mensagens = consultaOs.GetOSAbertas(atendimento?.Cliente?.Id.ToString()).Result;
                                if (this.EventoAtual.CondicaoEvento)
                                {
                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                }
                            }
                        }
                        break;

                    //Consulta a manutenção de rede na área
                    case Tipo_Bot_Evento_Acoes.EV_CONSULTA_MAN_REDE:
                        using (var session = HibernateUtil.GetSession())
                        using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                        {
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);


                            if (atendimento?.Cliente?.Id == null)
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                            else
                            {
                                //mensagens.Add = areaManutencao.GetManutencaoRede(atendimento?.Cliente?.Id.ToString()).Result;
                                if (this.EventoAtual.CondicaoEvento)
                                {
                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                }
                            }
                        }
                        break;

                    //Realiza um teste na conexão
                    case Tipo_Bot_Evento_Acoes.EV_TESTE_CONEXAO:
                        if (this.AtendimentoChatMongo.cliente.idCliente == null)
                        {
                            botEvento = this.EventoAtual.EventoFalso;
                        }
                        else
                        {
                            AtivaEquipamento dadosTesteConex = GetDadosONU();
                            integrAtivacaoApi.DiagnosticoONU(dadosTesteConex).Wait();
                            if (this.EventoAtual.CondicaoEvento)
                            {
                                botEvento = this.EventoAtual.EventoVerdadeiro;
                            }
                        }
                        break;

                    //Reinicia o sinal da ONU
                    case Tipo_Bot_Evento_Acoes.EV_REINICIA_SINAL:
                        if (this.AtendimentoChatMongo.cliente.idCliente == null)
                        {
                            botEvento = this.EventoAtual.EventoFalso;
                        }
                        else
                        {
                            AtivaEquipamento dadosReiniciaSinal = GetDadosONU();
                            integrAtivacaoApi.ReiniciarONU(dadosReiniciaSinal);
                            if (this.EventoAtual.CondicaoEvento)
                            {
                                botEvento = this.EventoAtual.EventoVerdadeiro;
                            }
                        }
                        break;

                    //Solicita o bloqueio em confiança
                    case Tipo_Bot_Evento_Acoes.EV_DESBL_CONFIANCA:
                        using (var session = HibernateUtil.GetSession())
                        using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                        {
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);

                            if (atendimento?.Cliente?.Id == null)
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                            else
                            {
                                using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                                {
                                    var idPlanoProduto = GetTipoVariavel(EventoArmazenamento.PLANO);

                                    var objPlanoProduto = !string.IsNullOrEmpty(idPlanoProduto) ? await repoCliente_PlanoProduto.selectFirst(p => p.Id == Convert.ToInt64(idPlanoProduto)) : null;
                                    var valueDesb = new DesbloqueioConfiancaRequest
                                    {
                                        IdClienteContrato = objPlanoProduto?.ClienteContrato?.Id ?? 0
                                    };
                                    var desbloqueio = await clienteContratoApi.DesbloqueioConfianca(valueDesb);

                                    if (this.EventoAtual.CondicaoEvento)
                                    {
                                        if (desbloqueio.result == true)
                                        {
                                            botEvento = this.EventoAtual.EventoVerdadeiro;
                                        }
                                        else
                                        {
                                            botEvento = this.EventoAtual.EventoFalso;
                                        }
                                    }
                                }                                
                            }
                        }
                        break;

                    //Lista numericamente os planos do cliente
                    case Tipo_Bot_Evento_Acoes.EV_CONSULTA_PLANOS:
                        using (var session = HibernateUtil.GetSession())
                        using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                        {
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);

                            if (atendimento?.Cliente?.Id == null)
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                            else
                            {
                                mensagens = planoApi.getPlanoAtivoCliente(atendimento?.Cliente?.Id).Result;
                                if (this.EventoAtual.CondicaoEvento)
                                {
                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                }
                            }
                        }
                        break;

                    //Lista todas as informações de todas as faturas dos últimos seis meses, independente de status
                    case Tipo_Bot_Evento_Acoes.EV_LISTA_BOLETOS:
                        using (var session = HibernateUtil.GetSession())
                        using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                        {
                            var atendimento = await repositoryAtendimento.selectFirst(p => p.Protocolo == this.AtendimentoChatMongo.protocolo);


                            if (atendimento?.Cliente?.Id == null)
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                            else
                            {
                                mensagens = acoesCliente.ListaFaturasSegundaVia(Convert.ToString(atendimento?.Cliente?.Id)).Result;
                                if (this.EventoAtual.CondicaoEvento)
                                {
                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                }
                            }
                        }
                        break;

                    //Transfere o chat para um atendente
                    case Tipo_Bot_Evento_Acoes.EV_TRANSFERE_CHAT:

                        var valueTransf = new TransferirAtendimentoRequestModel
                        {
                            protocolo = this.AtendimentoChatMongo.protocolo,
                            usuario = "",
                            novoAtendente = "",
                            novoDepartamento = this.EventoAtual.SetorAtendimento.Id
                        };
                        acoes.TransferenciaSetor(valueTransf).Wait();

                        await ChatHub.EnviarNotificacaoSuporte(AtendimentoChatMongo, false, null);

                        if (this.EventoAtual.CondicaoEvento)
                        {
                            botEvento = this.EventoAtual.EventoVerdadeiro;
                        }
                        break;

                    //Valida a resposta do armazenamento de variável
                    case Tipo_Bot_Evento_Acoes.EV_ARMAZENA:
                        if (this.EventoAtual.EventoArmazenamento.HasValue)
                        {
                            try
                            {
                                //adicionar case para verificar se a variável é valida ou não
                                switch (this.EventoAtual.EventoArmazenamento)
                                {
                                    case EventoArmazenamento.PLANO:
                                        {
                                            var variavelArmazena = GetVariavelArmazenamento(this.EventoAtual);
                                            var resultado = planoApi.FindById(Convert.ToInt64(variavelArmazena)).Result;
                                            if (resultado?.Objeto?.Id != null && resultado.Ok == true)
                                            {
                                                if (this.EventoAtual.CondicaoEvento)
                                                {
                                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                                }
                                            }
                                            else
                                            {
                                                if (this.EventoAtual.CondicaoEvento)
                                                {
                                                    botEvento = this.EventoAtual.EventoFalso;
                                                }
                                            }
                                            break;
                                        }

                                    case EventoArmazenamento.BOLETO:
                                        {
                                            var variavelArmazena = GetVariavelArmazenamento(this.EventoAtual);
                                            if (this.EventoAtual.CondicaoEvento)
                                            {
                                                botEvento = this.EventoAtual.EventoVerdadeiro;
                                            }
                                            break;
                                        }

                                    case EventoArmazenamento.CPF_CNPJ:
                                        {
                                            var variavelArmazena = GetVariavelArmazenamento(this.EventoAtual);
                                            var resultado = clienteApi.buscaCpf(variavelArmazena).Result;

                                            using (var conexaoMongo = new ConexaoMongo())
                                            using (var session = HibernateUtil.GetSession())
                                            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                                            {
                                                AtendimentoChatMongo chat = repository.Find(p => p.protocolo == this.AtendimentoChatMongo.protocolo).Result;

                                                //Se for cliente, já armazena os dados
                                                if (resultado.Ok == true)
                                                {
                                                    //atualiza dados no mondoDb
                                                    ClienteChat_Mobile dados = clienteApi.ClienteChat(valorEntrada, this.AtendimentoChatMongo).Result;

                                                    if (dados != null)
                                                    {
                                                        chat.cliente.cpf_cnpj = dados.cpf_cnpj;
                                                        chat.cliente.email = dados.email;
                                                        chat.cliente.idCliente = dados.idCliente;
                                                        chat.cliente.idCategoriaCliente = dados.idCategoriaCliente;

                                                        repository.Update(p => p.protocolo == this.AtendimentoChatMongo.protocolo, chat).Wait();
                                                    }

                                                    if (this.EventoAtual.CondicaoEvento)
                                                    {
                                                        botEvento = this.EventoAtual.EventoVerdadeiro;
                                                    }
                                                }

                                                //Se não for cliente, armazena o CPF
                                                else
                                                {
                                                    if (String.IsNullOrEmpty(variavelArmazena))
                                                    {
                                                        if (!string.IsNullOrEmpty(this.AtendimentoChatMongo.JsonArmazenamento))
                                                        {
                                                            var cpfCnpjList = JsonConvert.DeserializeObject<List<VariavelArmazenamento>>(this.AtendimentoChatMongo.JsonArmazenamento);

                                                            if (cpfCnpjList != null && cpfCnpjList.Any())
                                                            {
                                                                var valorArmazenar = cpfCnpjList.FirstOrDefault(p => p.nomeVariavel == "@CPF_CNPJ")?.valor ?? "";

                                                                chat.cliente.cpf_cnpj = valorArmazenar;
                                                                await repository.Update(p => p.protocolo == this.AtendimentoChatMongo.protocolo, chat);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        chat.cliente.cpf_cnpj = variavelArmazena;
                                                        await repository.Update(p => p.protocolo == this.AtendimentoChatMongo.protocolo, chat);
                                                    }

                                                    if (this.EventoAtual.CondicaoEvento)
                                                    {
                                                        botEvento = this.EventoAtual.EventoVerdadeiro;
                                                    }
                                                }

                                            }
                                            break;
                                        }
                                }
                            }
                            catch
                            {
                                if (this.EventoAtual.CondicaoEvento)
                                {
                                    botEvento = this.EventoAtual.EventoFalso;
                                }
                            }


                        }
                        break;

                    case Tipo_Bot_Evento_Acoes.EV_CADASTRO_CLIENTE:

                        var cpf_cnpj = acoesCliente.ValidaCpf(valorEntrada);
                        if (cpf_cnpj == "Cpf Válido")
                        {
                            using (var session = HibernateUtil.GetSession())
                            using (var sessionT5 = HibernateUtil.GetSessionT5())
                            using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                            using (var repositoryConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                            using (var repositoryCliente = new Conexao<Cliente>(session))
                            using (var repositoryClienteAniel = new Conexao<ClienteAniel>(sessionT5))
                            {
                                var config_empresa = await repositoryConfiguracoes_Gerais.selectFirst();
                                var atendimento = await repositoryAtendimento.selectFirst(x => x.Protocolo == this.AtendimentoChatMongo.protocolo);
                                var cliente = await repositoryCliente.selectFirst(x => x.CpfCnpj == valorEntrada);

                                if (cliente == null)
                                {
                                    String[] caracteres = new String[]
                                    {
                                        ".", "-", "/"
                                    };

                                    foreach (var item in caracteres)
                                    {
                                        valorEntrada = valorEntrada.Replace(item, "");
                                    }

                                    int tipo_cliente = Convert.ToInt32(config_empresa.Pessoa_Contato_Primeira_Vez);
                                    ClienteAniel cliente_aniel = new ClienteAniel
                                    {
                                        NomeFantasia = this.AtendimentoChatMongo.cliente.nome,
                                        CPF = valorEntrada,
                                        Tipo_Cliente = tipo_cliente == 0 ? Tipo_ClienteAniel.Cliente : Tipo_ClienteAniel.Prospecto,
                                        Tipo_Pessoa = valorEntrada.Length <= 11 ? Sinapse.Model.enumerados.Tipo_Pessoa.Fisica : Sinapse.Model.enumerados.Tipo_Pessoa.Juridica,
                                        Status = Status_Cliente.Ativo,
                                        Status_Prospecto = Status_ProspectoAniel.Lead_Qualificado
                                    };
                                    await repositoryClienteAniel.Save(cliente_aniel);

                                    cliente = new Cliente
                                    {
                                        NomeFantasia = this.AtendimentoChatMongo.cliente.nome,
                                        CpfCnpj = valorEntrada,
                                        Tipo_Cliente = tipo_cliente == 0 ? Tipo_Cliente.Prospecto : Tipo_Cliente.Cliente,
                                        Tipo_Pessoa = valorEntrada.Length <= 11 ? Sinapse.Model.enumerados.Tipo_Pessoa.Fisica : Sinapse.Model.enumerados.Tipo_Pessoa.Juridica,
                                        Status = Sinapse.Model.enumerados.Status_Cliente.Ativo,
                                        Status_Prospecto = Status_Prospecto.Lead_Qualificado,
                                        Cod_Cliente = cliente_aniel?.Cod_Cliente
                                    };

                                    await repositoryCliente.SaveOrUpdate(cliente);

                                    atendimento.Cliente = cliente;
                                }
                                else
                                {
                                    atendimento.Cliente = cliente;
                                }

                                if (this.EventoAtual.CondicaoEvento)
                                {
                                    botEvento = this.EventoAtual.EventoVerdadeiro;
                                }

                                await repositoryAtendimento.SaveOrUpdate(atendimento);
                            }
                        }
                        else
                        {
                            if (this.EventoAtual.CondicaoEvento)
                            {
                                botEvento = this.EventoAtual.EventoFalso;
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                if (this.EventoAtual.CondicaoEvento)
                {
                    botEvento = this.EventoAtual?.EventoFalso;

                }
                else
                {
                    mensagens.Add(this.Bot.TextoInvalido);
                    botEvento = this.EventoAtual;
                }
            }
            return (botEvento, mensagens, anexos);
        }

        private void MudarEventoAtual(BotEvento evento)
        {
            LogExecucao.RegistraLog("Eventual Anterior: " + this.EventoAtual?.Descricao);
            LogExecucao.RegistraLog("Eventual Novo: " + evento?.Descricao);

            //LogExecucao.RegistraLog("Ação Executada: " + this.EventoAtual.AcaoExecutada);
            this.EventoAtual = evento;
        }

        private AtivaEquipamento GetDadosONU()
        {
            var apiONU = new ListagemONUApiController();
            var idPlanoProduto = GetTipoVariavel(EventoArmazenamento.PLANO);
            ONUListModel dadosONU = apiONU.GetDadosONU(idPlanoProduto).Result;
            var valueONU = new AtivaEquipamento
            {
                IdONU = Convert.ToInt64(dadosONU.Id),
                IdOLT = Convert.ToInt64(dadosONU.IdOLT),
                IdPON = Convert.ToInt64(dadosONU.NumPON),
            };

            return valueONU;
        }

        private string GetVariavelArmazenamento(BotEvento evento)
        {

            var data = this.AtendimentoChatMongo.JsonArmazenamento;
            List<VariavelArmazenamento> resultado = JsonConvert.DeserializeObject<List<VariavelArmazenamento>>(data);
            var retorno = "";

            if (evento.EventoArmazenamento.HasValue)
            {
                var nomeEnum = evento.EventoArmazenamento.GetDisplayName();
                var nomeVariavel = resultado.Find(p => p.nomeVariavel == "@" + nomeEnum)?.valor;

                if (!string.IsNullOrEmpty(nomeVariavel))
                {
                    retorno = nomeVariavel;
                }

            }

            return retorno;
        }

        private string GetTipoVariavel(EventoArmazenamento variavelArm)
        {
            var data = this.AtendimentoChatMongo.JsonArmazenamento;
            List<ArmazendamentoEvento> resultado = JsonConvert.DeserializeObject<List<ArmazendamentoEvento>>(data);
            var retorno = resultado.Find(p => p.TipoArmazenamento == variavelArm).valor;

            return retorno;
        }

        private void ArmazenamentoEvento(BotEvento evento, String resposta)
        {

            var obj = new List<ArmazendamentoEvento>();
            obj.Add(new ArmazendamentoEvento
            {
                TipoArmazenamento = evento.EventoArmazenamento.Value,
                valor = resposta
            });

            if (this.AtendimentoChatMongo != null)
            {
                this.AtendimentoChatMongo.JsonArmazenamento = JsonService.SerializeObject(obj);
            }
        }

        private string GetTexto(BotEvento evento)
        {
            return evento?.TextoEnviado;
        }

        private string GetTextoInvalido(Bot bot)
        {
            return bot?.TextoInvalido;
        }

    }

    public class RetornoSequenciaBot
    {
        public bool result { get; set; }
        public string exception { get; set; }
        public string stackTrace { get; set; }
        public String texto { get; set; }
        public List<BotEventoAnexo> anexos { get; set; } = new List<BotEventoAnexo>();
        public List<string> sequenciaMensagens { get; set; }

    }

    public class RetornoTesteBot
    {
        public string eventoAtual { get; set; }
        public List<string> sequenciaMensagens { get; set; }
        public AtendimentoChatMongo atendimento { get; set; }

    }

    public class ArmazendamentoEvento
    {
        public EventoArmazenamento TipoArmazenamento { get; set; }
        public string valor { get; set; }

    }

    public class VariavelArmazenamento
    {
        public string nomeVariavel { get; set; }
        public string valor { get; set; }
    }

    public class SegundaViaResponseModel : PostApiRetorno<String>
    {
        public virtual Int64? id_conta_receber { get; set; }
        public virtual Int64? id_faturamento_cliente { get; set; }
        public virtual Int64? id_forma_pagamento { get; set; }
    }

}