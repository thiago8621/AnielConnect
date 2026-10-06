using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using log4net;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using S22.Imap;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Ude;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class EmailServiceApiController : BaseApiController
    {
        private Configuracoes_Gerais configuracao { get; set; }
        public ILog log4net { get; set; }
        public string caminhoFisico { get; set; }
        public string importarOsDigitalApenasPorData { get; set; }
        //[HttpPost()]
        [HttpGet()]
        [Route("api/EmailServiceApi/ImportarAtendimento")]
        public async Task ImportarAtendimento(String CaminhoFisicoConnect, String UrlPortalConnect)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoClienteAniel = new Conexao<ClienteAniel>(sessionT5))
            using (var repoConfGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repoContaEmail = new Conexao<Conta_Email>(session))
            using (var repoOrganizacao = new Conexao<Organizacao>(session))
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var repositoryOrigem = new Conexao<Origem_Atendimento>(session))
            using (var repoEmailAtendimento = new Conexao<Email_Atendimento>(session))
            using (var repoClienteContato = new Conexao<Cliente_Contato>(session))
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    this.configuracao = await repoConfGerais.selectFirst();
                    if (this.configuracao?.Criacao_Atendimento_Email == true || this.configuracao.Criacao_Atendimento_DominioEmail == true)
                    {
                        Origem_Atendimento origem = await repositoryOrigem.selectFirst(ori => ori.Id == -1);
                        IList<Cliente> clientes = await repoCliente.select(p => p.Email_Principal != null);
                        IEnumerable<string> emails = (await repoClienteContato.select()).Select(p => p.Email).Concat(clientes.Select(p => p.Email_Principal));
                        IList<Conta_Email> contas = await repoContaEmail.select(p => p.Status == Status_Default.Ativo);
                        List<Organizacao> organizacoes = this.configuracao?.Criacao_Atendimento_DominioEmail == true ? (await repoOrganizacao.select()).ToList() : new List<Organizacao>();
                        IList<string> dominios = organizacoes.Select(p => p.Dominios).SelectMany(p => p).Select(p => p.Dominio).ToList();

                        foreach (Conta_Email conta in contas)
                        {
                            DateTime? UltimaExecucao = DateTime.Today;
                            if (conta.UltimaData.HasValue)
                                UltimaExecucao = Convert.ToDateTime(conta.UltimaData.Value.ToString("dd/MM/yyyy HH:mm:ss"));

                            UltimaExecucao = UltimaExecucao.Value.Date;
                            using (ImapClient Client = new ImapClient(conta.Servidor_Imap, conta.Porta_Imap, conta.Email, conta.Senha, (AuthMethod)conta.AuthMethodImap, conta.Utilizar_Ssl_Imap))
                            {
                                IEnumerable<uint> uids = GetUids(Client);
                                IEnumerable<MailMessage> messages = GetMessages(Client, dominios.ToList(), emails.ToList(), uids.ToList(), UltimaExecucao.Value);
                                conta.UltimaData = DateTime.Now;
                                await repoContaEmail.Update(conta);

                                List<(Atendimento atendimento, MailMessage message)> atendimentos_messages = messages.Select(message => CriarAtendimentoPorEmail(organizacoes, message, clientes, repoCliente, repoAtendimento, repoClienteContato, origem, repoClienteAniel).Result).Where(p => p.atendimento != null).ToList();
                                List<(Atendimento_Acao atendimento_Acao, AttachmentCollection Attachments, AlternateViewCollection alternateViews)> acoes_anexos = atendimentos_messages.Select(p => GerarNovaAcao(p.atendimento, p.message, repoAtendimento).Result).Where(p => p.atendimento_Acao != null).ToList();
                                acoes_anexos.ForEach(acao => GravarAnexosAcao(acao.atendimento_Acao, CaminhoFisicoConnect, UrlPortalConnect, acao.Attachments, acao.alternateViews));
                                List<Atendimento> atendimentos = atendimentos_messages.Select(p => p.atendimento).ToList();
                                atendimentos = Gravar_Numero(atendimentos);
                                atendimentos.ForEach(atendimento =>
                                {
                                    Email_Atendimento email = new Email_Atendimento
                                    {
                                        Conta_Email = conta,
                                        Atendimento = atendimento,
                                        Enviado = false
                                    };
                                    repoEmailAtendimento.Save(email).Wait();
                                    atendimento.Id_Email_Abertura = email.Id;
                                });

                                await repoAtendimento.Save(atendimentos.ToList());
                                await ChecarRespostas(Client, uids, UltimaExecucao.Value, repoAtendimento, CaminhoFisicoConnect, UrlPortalConnect);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.log4net.Info("ERRO: " + ex.Message + " INNER: " + ex.InnerException?.Message + " STACK: " + ex.StackTrace);
                    transaction.Rollback();
                    return;
                }

                transaction.Commit();

            }
        }

        [HttpGet()]
        [Route("api/EmailServiceApi/ImportarOsDigital")]
        public void ImportarOsDigital(DateTime ultimaExecucao, string imapServer, string emailBuscar, string senhaEmail, bool usaSsl, short portaImap)
        {
            this.log4net.Info("ALCANCOU METODO ImportarOsDigital");
            try
            {
                using (ImapClient Client = new ImapClient(imapServer, portaImap, emailBuscar, senhaEmail, AuthMethod.Auto, usaSsl))
                {
                    IEnumerable<uint> uids = GetUidsOsDigital(Client, ultimaExecucao);
                    IEnumerable<MailMessage> messages = GetMessagesOsDigital(Client, uids.ToList(), ultimaExecucao);

                    List<(long? contrato, AttachmentCollection Attachments, AlternateViewCollection alternateViews)> anexosOs = messages.Select(p => BuscarArquivoOsDigital(p).Result).ToList();
                    anexosOs.ForEach(acao => GravarOsDigital(acao.contrato, acao.Attachments).Wait());
                }
            }
            catch (Exception ex)
            {
                this.log4net.Info("ERRO: " + ex.Message + " INNER: " + ex.InnerException?.Message + " STACK: " + ex.StackTrace);
                throw ex;
            }
        }

        [HttpGet()]
        [Route("api/EmailServiceApi/EnviarEmailAtendimento")]
        public async Task EnviarEmailAtendimento(String CaminhoFisicoConnect)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoEmailAtendimento = new Conexao<Email_Atendimento>(session))
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var repoAtendimentoAcao = new Conexao<Atendimento_Acao>(session))
            using (var repoConfGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var atendimentoApi = new AtendimentoApiController())
            {
                try
                {
                    this.configuracao = await repoConfGerais.selectFirst();
                    var lista_emails = (await repoEmailAtendimento.select(p => p.Enviado == false)).OrderBy(p => p.DataHoraCriacao).ToList();
                    foreach (var email in lista_emails)
                    {
                        try
                        {
                            long maxAcaoGravada = ((repoEmailAtendimento.Max(x => x.ReferenciaUltimoEnvioCLI).Result) + 1);

                            string messageID = String.Format("<{0}@{1}>", Guid.NewGuid().ToString(), $"smtp-worker-cli-{maxAcaoGravada}.kinghost.net");

                            GetApiRetorno retorno = null;
                            List<string> mensagensEnviadasAnteriores = new List<string>();
                            List<string> referenciasAnteriores = new List<string>();
                            if (email.Atendimento != null && email.Conta_Email != null)
                            {
                                retorno = Enviar_EmailResposta(email.Conta_Email, email.Atendimento, messageID);
                            }
                            else if (email.Atendimento_Acao != null)
                            {
                                long atendimentoId;

                                if (email.Atendimento is null)
                                    atendimentoId = email.Atendimento_Acao.Atendimento.Id;
                                else
                                    atendimentoId = email.Atendimento.Id;

                                if (atendimentoId != 0)
                                {
                                    var att = await repoAtendimento.selectFirst(x => x.Id == atendimentoId);

                                    var cliente = await repoCliente.selectFirst(x => x.Id == att.Cliente.Id);

                                    var primeiroEmailAtt = (await repoEmailAtendimento.select(x => x.Atendimento.Id == atendimentoId)).OrderBy(p => p.DataHoraCriacao).FirstOrDefault();

                                    string primeiraMessagem = primeiroEmailAtt?.ReferenciaMessageID;

                                    if (primeiraMessagem != null)
                                    {
                                        referenciasAnteriores.Add(primeiraMessagem);

                                        var acoesAnteriores = (await repoAtendimentoAcao.select(x => x.Atendimento.Id == atendimentoId && x.MessageID != primeiraMessagem))
                                                                                        .OrderBy(x => x.DataHoraCriacao)
                                                                                        .Select(x => x.MessageID)
                                                                                        .ToList();

                                        if (acoesAnteriores != null && acoesAnteriores.Count() > 0)
                                            acoesAnteriores.ForEach(x => referenciasAnteriores.Add(x));

                                        var atendimentoHistorico = await atendimentoApi.FindById_Historico(atendimentoId);

                                        if (atendimentoHistorico.Ok)
                                        {
                                            atendimentoHistorico.Objeto.historico_Atendimento = atendimentoHistorico.Objeto.historico_Atendimento.Where(p => p.id != email.Atendimento_Acao.Id &&
                                                                                                                                                             Convert.ToDateTime(p.dataCriacao) <= email.Atendimento_Acao.DataHoraCriacao);
                                            mensagensEnviadasAnteriores = await GerarHistoricoMensagens(String.Format(Resources.RsCadastros.ATENDIMENTO_ABERTO_EMAIL, att.Numero),
                                                                                                        atendimentoHistorico.Objeto.historico_Atendimento.ToList(), att.Numero,
                                                                                                            cliente, primeiroEmailAtt.Conta_Email, primeiroEmailAtt?.DataHoraEnvio);
                                        }
                                    }
                                }

                                retorno = Enviar_EmailAcao(session, CaminhoFisicoConnect, email.Atendimento_Acao, messageID, referenciasAnteriores, mensagensEnviadasAnteriores);
                            }
                            else if (email.Mudanca_Status.HasValue)
                            {
                                var primeiroEmailAtt = (await repoEmailAtendimento.select(x => x.Atendimento.Id == email.Atendimento.Id)).OrderBy(p => p.DataHoraCriacao).FirstOrDefault();
                                string primeiraMessagem = primeiroEmailAtt?.ReferenciaMessageID;
                                if (primeiraMessagem != null)
                                    referenciasAnteriores.Add(primeiraMessagem);

                                var atendimentoHistorico = await atendimentoApi.FindById_Historico(email.Atendimento.Id);

                                if (atendimentoHistorico.Ok)
                                {
                                    var mensagem_status = RetornoMensagem_MudancaStatus(email.Mudanca_Status.Value);

                                    var historico_atendimento = new List<AtendimentoHistoricoAcoesCadastroModel>();
                                    foreach (var item in atendimentoHistorico.Objeto.historico_Atendimento)
                                    {
                                        historico_atendimento.Add(item);
                                    }

                                    mensagensEnviadasAnteriores = await GerarHistoricoMensagens(String.Format(Resources.RsCadastros.ATENDIMENTO_ABERTO_EMAIL, email.Atendimento.Numero),
                                                                                                historico_atendimento, email.Atendimento.Numero,
                                                                                                    email.Atendimento.Cliente, primeiroEmailAtt.Conta_Email, primeiroEmailAtt?.DataHoraEnvio);

                                    retorno = EnviarEmailComHistoricoMensagens_StatusAlterado(session,
                                        email,
                                        mensagem_status,
                                        $"[Atendimento #{email.Atendimento.Numero}] Status do Atendimento foi alterado para Aguardando",
                                        messageID,
                                        mensagensEnviadasAnteriores,
                                        referenciasAnteriores);
                                }

                                //retorno = Enviar_EmailMudancaStatus(session, email.Atendimento, email.Mudanca_Status.Value);
                            }

                            if (retorno != null)
                            {
                                email.Enviado = retorno.Ok;
                                email.DataHoraEnvio = DateTime.Now;
                                email.ReferenciaMessageID = messageID;
                                email.ReferenciaUltimoEnvioCLI = maxAcaoGravada;

                                if (email.Atendimento_Acao != null)
                                    email.Atendimento_Acao.MessageID = messageID;

                                if (!String.IsNullOrEmpty(retorno.Mensagem) || !String.IsNullOrEmpty(retorno.InnerException))
                                {
                                    email.Mensagem = StringService.Truncate(String.Format("Erro: {0} | {1}", retorno.Mensagem, retorno.InnerException), 900);
                                }

                                await repoEmailAtendimento.Update(email);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.log4net.Info("ID EMAIL: " + email.Id + " ERRO: " + ex.Message + " INNER: " + ex.InnerException?.Message + " STACK: " + ex.StackTrace);
                            continue;
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.log4net.Info("ERRO: " + ex.Message + " INNER: " + ex.InnerException?.Message + " STACK: " + ex.StackTrace);

                    throw ex;
                }
            }


        }

        private async Task<List<string>> GerarHistoricoMensagens(string primeiraMensagemSistema, List<AtendimentoHistoricoAcoesCadastroModel> atendimentoHistorico,
                                                                string numeroAtt, Cliente cliente, Conta_Email contaEmail, DateTime? dataHoraEnvio)
        {
            List<string> response = new List<string>();
            atendimentoHistorico = atendimentoHistorico.Where(x => x.tipoAtividade == TipoAtividadeAtendimento.Publico)
                                                       .OrderByDescending(p => p.dataCriacaoDateTime)
                                                       .ToList();

            if (atendimentoHistorico != null && atendimentoHistorico.Count > 0)
            {
                string assunto = $"{String.Format(Resources.RsCadastros.ATENDIMENTO_ABERTO, numeroAtt)}";

                for (int i = 0; i < atendimentoHistorico.Count; i++)
                {
                    var att = atendimentoHistorico[i];

                    if (!string.IsNullOrEmpty(primeiraMensagemSistema) && i == atendimentoHistorico.Count - 1)
                    {
                        StringBuilder mensagem = new StringBuilder();

                        mensagem.AppendLine(i == 0 ? "<p>-----Mensagem Original-----</p>" : "<p>-----Resposta-----</p>");
                        mensagem.AppendLine(att.origem.ToLower() == "email" ? $"<p>De: {cliente.Email_Principal}</p>" : $"<p>De: {contaEmail.Email}</p>");
                        mensagem.AppendLine($"<p>Enviada em: {att.dataCriacaoDateTime.Value.ToString("dd 'de' MMMM 'de' yyyy 'às' HH:mm", new CultureInfo("pt-BR"))}</p>");
                        mensagem.AppendLine(att.origem.ToLower() == "email" ? $"<p>Para: {contaEmail.Email}</p>" : $"<p>Para: {cliente.Email_Principal}</p>");
                        mensagem.AppendLine($"<p>Assunto: RE: {assunto}</p>");
                        mensagem.AppendLine($"{att.html}<br><br>");

                        response.Add(mensagem.ToString());

                    }
                    else
                    {
                        //StringBuilder mensagem = new StringBuilder();

                        //mensagem.AppendLine("<p>-----Resposta-----</p>");
                        //mensagem.AppendLine($"<p>De: {contaEmail.Email}</p>");
                        //mensagem.AppendLine($"<p>Enviada em: {dataHoraEnvio.Value.ToString("dd 'de' MMMM 'de' yyyy 'às' HH:mm", new CultureInfo("pt-BR"))}</p>");
                        //mensagem.AppendLine($"<p>Para: {cliente.Email_Principal}</p>");
                        //mensagem.AppendLine($"<p>Assunto: {assunto}</p>");
                        //mensagem.AppendLine($"{primeiraMensagemSistema}<br><br>");
                        //response.Add(mensagem.ToString());

                        StringBuilder mensagem2 = new StringBuilder();

                        mensagem2.AppendLine("<p>-----Resposta-----</p>");
                        mensagem2.AppendLine(att.origem.ToLower() == "email" ? $"<p>De: {cliente.Email_Principal}</p>" : $"<p>De: {contaEmail.Email}</p>");
                        mensagem2.AppendLine($"<p>Enviada em: {att.dataCriacaoDateTime.Value.ToString("dd 'de' MMMM 'de' yyyy 'às' HH:mm", new CultureInfo("pt-BR"))}</p>");
                        mensagem2.AppendLine(att.origem.ToLower() == "email" ? $"<p>Para: {contaEmail.Email}</p>" : $"<p>Para: {cliente.Email_Principal}</p>");
                        mensagem2.AppendLine($"<p>Assunto: RE: {assunto}</p>");
                        mensagem2.AppendLine($"{att.html}<br><br>");
                        response.Add(mensagem2.ToString());
                    }
                }
            }

            response.Insert(0, "<blockquote style='color:#500050'>");
            response.Add("</blockquote>");

            return response;
        }



        private IEnumerable<uint> GetUids(ImapClient Client)
        {

            SearchCondition search_condition = SearchCondition.SentOn(DateTime.Now);
            return Client.Search(search_condition);


        }

        private IEnumerable<uint> GetUidsOsDigital(ImapClient Client, DateTime? data)
        {

            if (this.importarOsDigitalApenasPorData == "S" && data.HasValue)
            {
                SearchCondition search_condition = SearchCondition.SentOn(data.Value);
                return Client.Search(search_condition);
            }
            else
            {

                SearchCondition search_condition = SearchCondition.All();
                return Client.Search(search_condition);
            }
        }

        private IEnumerable<MailMessage> GetMessages(ImapClient Client, List<String> dominios, List<String> emails, List<uint> uids, DateTime UltimaExecucao)
        {
            if (this.configuracao.Criacao_Atendimento_DominioEmail && this.configuracao.Criacao_Atendimento_Email)
            {
                return Client.GetMessages(uids.Distinct()).Where(p => p.Date() > UltimaExecucao &&
                                                                    (emails.ToList().Contains(p.From.Address) || DominiosEmail(p.From.Address, dominios))
                                                                    && !p.Headers.AllKeys.Contains("In-Reply-To"));
            }
            else if (this.configuracao.Criacao_Atendimento_Email)
            {
                return Client.GetMessages(uids.Distinct()).Where(p => p.Date() > UltimaExecucao && emails.ToList().Contains(p.From.Address) && !p.Headers.AllKeys.Contains("In-Reply-To") && !p.Headers.AllKeys.Contains("References"));
            }
            else
            {
                return Client.GetMessages(uids.Distinct()).Where(p => p.Date() > UltimaExecucao && dominios.ToList().Contains(p.From.Address) && !p.Headers.AllKeys.Contains("In-Reply-To") && !p.Headers.AllKeys.Contains("References"));
            }
        }

        private IEnumerable<MailMessage> GetMessages(ImapClient Client, List<uint> uids, DateTime UltimaExecucao)
        {
            if (this.configuracao.Criacao_Atendimento_DominioEmail && this.configuracao.Criacao_Atendimento_Email)
            {
                return Client.GetMessages(uids.Distinct()).Where(p => p.Date() > UltimaExecucao
                                                                    && !p.Headers.AllKeys.Contains("In-Reply-To")
                                                                    && !p.Headers.AllKeys.Contains("References"));
            }
            else if (this.configuracao.Criacao_Atendimento_Email)
            {
                return Client.GetMessages(uids.Distinct()).Where(p => p.Date() > UltimaExecucao && !p.Headers.AllKeys.Contains("In-Reply-To") && !p.Headers.AllKeys.Contains("References"));
            }
            else
            {
                return Client.GetMessages(uids.Distinct()).Where(p => p.Date() > UltimaExecucao && !p.Headers.AllKeys.Contains("In-Reply-To") && !p.Headers.AllKeys.Contains("References"));
            }
        }

        private IEnumerable<MailMessage> GetMessagesOsDigital(ImapClient Client, List<uint> uids, DateTime? UltimaExecucao)
        {
            if (this.importarOsDigitalApenasPorData == "S")
                return Client.GetMessages(uids.Distinct()).Where(p => p.Date() >= UltimaExecucao);
            else
                return Client.GetMessages(uids.Distinct());
        }

        private bool DominiosEmail(String email, List<String> dominios)
        {

            foreach (var item in dominios)
            {
                if (email.Contains(item))
                {
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// Gera novo registro da entidade Cliente através de mensagem de email
        /// </summary>
        /// <param name="repoCliente"></param>
        /// <param name="message"></param>
        /// <param name="organizacoes"></param>
        /// <returns></returns>
        private async Task<Cliente> CriarClientePorEmail(Conexao<Cliente> repoCliente, MailMessage message, List<Organizacao> organizacoes, Conexao<ClienteAniel> repoClienteAniel)
        {
            Organizacao organizacao = null;
            var dominios = organizacoes.SelectMany(p => p.Dominios.Select(y => y.Dominio)).ToList();
            string dominio = dominios.Find(p => message.From.Address.Contains(p));

            if (!String.IsNullOrEmpty(dominio))
            {
                organizacao = organizacoes.Find(p => p.Dominios.ToList().Find(y => y.Dominio == dominio) != null);
            }

            ClienteAniel cliente_aniel = new ClienteAniel
            {
                Status = Status_Cliente.Ativo,
                Tipo_Pessoa = Tipo_Pessoa.Fisica,
                Tipo_Cliente = this.configuracao.Pessoa_Contato_Primeira_Vez == Enum_Pessoa_Contato_Primeira_Vez.Cliente ? Tipo_ClienteAniel.Cliente : Tipo_ClienteAniel.Prospecto,
                NomeFantasia = message.From.Address,
                Email_Principal = message.From.Address,
                Data_Cadastro = DateTime.Now.Date,
                Cod_Cliente = Convert.ToInt32(repoClienteAniel.Max(p => p.Cod_Cliente).Result) + 1
            };

            if (this.configuracao.Identificacao_Cliente == Identificacao_Cliente.Sequencial)
            {
                cliente_aniel.Numero_Contrato = Convert.ToString(repoClienteAniel.MaxString("NUMERO_CONTRATO", "TB_CLIENTE").Result + 1);
            }

            if (cliente_aniel.Tipo_Cliente == Tipo_ClienteAniel.Prospecto)
            {
                cliente_aniel.Status_Prospecto = Status_ProspectoAniel.Lead_Nao_Qualificado;
            }
            await repoClienteAniel.Save(cliente_aniel);

            Cliente cliente = new Cliente
            {
                Status = Status_Cliente.Ativo,
                Tipo_Cliente = this.configuracao.Pessoa_Contato_Primeira_Vez == Enum_Pessoa_Contato_Primeira_Vez.Cliente ? Tipo_Cliente.Cliente : Tipo_Cliente.Prospecto,
                Tipo_Pessoa = Tipo_Pessoa.Fisica,
                NomeFantasia = message.From.Address,
                Email_Principal = message.From.Address,
                Organizacao = organizacao,
                DataHoraCriacao = DateTime.Now.Date,
                Data_Criacao_Cliente = DateTime.Now.Date,
                Cod_Cliente = cliente_aniel?.Cod_Cliente
            };

            if (this.configuracao.Identificacao_Cliente == Identificacao_Cliente.Sequencial)
            {
                cliente.Numero_Contrato = Convert.ToString(repoCliente.MaxString("NUMERO_CONTRATO", "TB_CLIENTE").Result + 1);
            }

            if (cliente.Tipo_Cliente == Tipo_Cliente.Prospecto)
            {
                cliente.Status_Prospecto = Status_Prospecto.Lead_Nao_Qualificado;
            }

            await repoCliente.Save(cliente);
            return cliente;
        }


        /// <summary>
        /// Gera novo registro da entidade Atendimento através de mensagem de email
        /// </summary>
        /// <param name="organizacoes"></param>
        /// <param name="message"></param>
        /// <param name="clientes"></param>
        /// <param name="repoCliente"></param>
        /// <param name="repoAtendimento"></param>
        /// <param name="origem"></param>
        /// <returns></returns>
        private async Task<(Atendimento atendimento, MailMessage message)> CriarAtendimentoPorEmail(List<Organizacao> organizacoes, MailMessage message, IList<Cliente> clientes, Conexao<Cliente> repoCliente, Conexao<Atendimento> repoAtendimento, Conexao<Cliente_Contato> repoClienteContato, Origem_Atendimento origem, Conexao<ClienteAniel> repoClienteAniel)
        {
            var contatos = new List<string>();
            Cliente cliente = clientes.Where(cliente => cliente.Email_Principal == message.From.Address).FirstOrDefault();

            var messageID = message.Headers["Message-ID"];
            if (!String.IsNullOrEmpty(messageID))
            {
                var atendimentoReferenciado = await repoAtendimento.selectFirst(p => p.MessageID == messageID);
                if (atendimentoReferenciado != null)
                {
                    return (null, message);
                }
            }

            if (cliente == null)
            {
                cliente = await CriarClientePorEmail(repoCliente, message, organizacoes, repoClienteAniel);
                clientes.Add(cliente);
            }

            if (message.CC != null && message.CC.Count > 0)
            {
                foreach (var contato in message.CC)
                {
                    contatos.Add(contato.Address.Trim());
                }
            }

            var atendimento = new Atendimento
            {
                Assunto = StringService.Truncate(message.Subject, 100),
                Cliente = cliente,
                Problema_Relatado = message.IsBodyHtml ? String.Empty : StringService.Truncate(message.Body, 200),
                Status_Atendimento = Status_Atendimento.A_Iniciar,
                Status = Status_Default.Ativo,
                Origem_Atendimento = origem,
                DataHora_Indicador = DateTime.Now.AddMinutes(480),
                ContatosCopia = contatos.Count > 0 ? String.Join(";", contatos.Distinct().ToList()) : String.Empty,
                MessageID = messageID
            };

            await repoAtendimento.SaveOrUpdate(atendimento);

            //Notificação
            using (var api = new NotificacoesApiController())
            {
                var notificacao = api.EnviaNotificacoesAtendimento(0, atendimento, null);
            }

            return (atendimento, message);
        }

        /// <summary>
        /// Envia ao Cliente um email informando o sucesso da abertura de atendimento
        /// </summary>
        /// <param name="conta"></param>
        /// <param name="atendimento"></param>
        /// <param name="messageId">MessageID custom para respostas em um mesmo email ja criado</param>
        /// <returns></returns>
        private GetApiRetorno Enviar_EmailResposta(Conta_Email conta, Atendimento atendimento, string messageId)
        {
            GetApiRetorno retorno = new GetApiRetorno();
            retorno.Ok = true;
            try
            {
                EmailUtil emailService = new EmailUtil(conta.Email, conta.Senha, conta.Servidor_Smtp, conta.Porta_Smtp, (atendimento?.ContatosCopia != null ? atendimento?.ContatosCopia?.Split(';').ToList() : new List<string>()), new List<string>(';'));
                emailService.EnviarMensagem(atendimento.Cliente.Email_Principal, String.Format(Resources.RsCadastros.ATENDIMENTO_ABERTO, atendimento.Numero), String.Format(Resources.RsCadastros.ATENDIMENTO_ABERTO_EMAIL, atendimento.Numero), conta.Utilizar_Ssl_Smtp, messageId);
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;

        }

        /// <summary>
        /// Envia ao Cliente um email informando uma nova ação realizada pelo atendente no connect
        /// </summary>
        /// <param name="session"></param>
        /// <param name="CaminhoFisicoConnect"></param>
        /// <param name="acao"></param>
        /// <returns></returns>
        private GetApiRetorno Enviar_EmailAcao(ISession session, String CaminhoFisicoConnect,
                                               Atendimento_Acao acao, string messageID,
                                               List<string> referencesAnteriores = null,
                                               List<string> mensagensAnteriores = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                      | SecurityProtocolType.Tls11
                                      | SecurityProtocolType.Tls12;
            GetApiRetorno retorno = new GetApiRetorno();
            retorno.Ok = true;
            try
            {
                using (var repoAcao = new Conexao<Atendimento_Acao>(session))
                using (var repoEmail = new Conexao<Email_Atendimento>(session))
                using (var repoContaEmail = new Conexao<Conta_Email>(session))
                {
                    Conta_Email conta = null;
                    if (acao?.Atendimento?.Id_Email_Abertura != null)
                        conta = repoEmail.selectFirst(p => p.Id == acao.Atendimento.Id_Email_Abertura).Result?.Conta_Email;

                    if (conta == null)
                        conta = repoContaEmail.selectFirst(p => p.Principal == true).Result;

                    if (conta != null)
                    {
                        EmailUtil emailService = new EmailUtil(conta.Email, conta.Senha, conta.Servidor_Smtp, conta.Porta_Smtp, acao?.Atendimento?.ContatosCopia != null ? acao?.Atendimento?.ContatosCopia.Split(';').ToList() : null, new List<string>());
                        foreach (var arquivo in acao.Arquivos_AtendimentoAcao)
                        {
                            var _pasta = $@"{CaminhoFisicoConnect}\Arquivos\Atendimento";
                            var _arquivo = $@"{_pasta}\{arquivo.Id}{arquivo.Extensao}";
                            if (File.Exists(_arquivo))
                            {
                                emailService.Anexar(_arquivo);
                            }
                        }

                        //if (acao.Origem == Origem_AtendimentoAcao.CHAT)
                        //{

                        //    var mensagens = JsonService.DeserializeObject<List<Mensagem_ChatModels>>(acao.HTML);
                        //    if (acao.Status_Atendimento == Status_Atendimento.A_Iniciar || acao.Status_Atendimento == Status_Atendimento.Concluido)
                        //    {
                        //        emailService.EnviarMensagem(acao.Atendimento.Cliente.Email_Principal, $"[Atendimento #{acao.Atendimento.Numero}] Ação Adicionada", String.Join("\n", mensagens.Select(p => FormataMensagens(p)).ToList()), conta.Utilizar_Ssl_Smtp);
                        //    }
                        //}
                        //else
                        //{
                        //    emailService.EnviarMensagemReplys(acao.Atendimento.Cliente.Email_Principal, $"[Atendimento #{acao.Atendimento.Numero}] foi aberto", $"{acao.HTML}", conta.Utilizar_Ssl_Smtp, messageID, referencesAnteriores, mensagensAnteriores);
                        //}

                        emailService.EnviarMensagemReplys(acao.Atendimento.Cliente.Email_Principal, $"[Atendimento #{acao.Atendimento.Numero}] foi aberto", $"{acao.HTML}", conta.Utilizar_Ssl_Smtp, messageID, referencesAnteriores, mensagensAnteriores);
                    }
                    else
                    {
                        throw new Exception("Não foi encontrada conta de email para realizar o envio.");
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        private String FormataMensagens(Mensagem_ChatModels mensagem)
        {

            if (mensagem.fluxoChat == false)
            {
                return $@"
                       <div style='{(mensagem.enviadoPeloSuporte ? "margin-left:3%;" : "margin-right:3%;")}'>
                            <div style='border: solid 1px #b4bcc3; padding: 10px; margin-bottom: 10px; margin-top: 10px;background-color:{(mensagem.enviadoPeloSuporte ? "#e2e3e5" : "#d1ecf1")};'>
                               <strong>{mensagem.usuario}</strong>
                               <br/>
                               {mensagem.mensagem}
                               <br/>
                               {mensagem.dataHora.Split(' ')[1]}
                            </div>
                       </div>";
            }
            else
            {
                return $@"<div style='color:black'>{mensagem.mensagem}</div>";
            }
        }
        private GetApiRetorno Enviar_EmailMudancaStatus(ISession session, Atendimento atendimento, Status_Atendimento status)
        {
            GetApiRetorno retorno = new GetApiRetorno();
            retorno.Ok = true;
            try
            {
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoEmail = new Conexao<Email_Atendimento>(session))
                {
                    Conta_Email conta = null;
                    if (atendimento.Id_Email_Abertura != null)
                        conta = repoEmail.selectFirst(p => p.Id == atendimento.Id_Email_Abertura).Result?.Conta_Email;

                    if (conta != null)
                    {
                        EmailUtil emailService = new EmailUtil(conta.Email, conta.Senha, conta.Servidor_Smtp, conta.Porta_Smtp, atendimento?.ContatosCopia?.Split(';').ToList(), new List<string>());


                        switch (status)
                        {
                            case Status_Atendimento.Aguardando:
                                {
                                    emailService.EnviarMensagem(atendimento.Cliente.Email_Principal, $"[Atendimento #{atendimento.Numero}] Status do Atendimento foi alterado para Aguardando",
                                                                $"Status do Atendimento foi alterado para Aguardando", conta.Utilizar_Ssl_Smtp);
                                    break;
                                }
                            case Status_Atendimento.A_Iniciar:
                                {
                                    emailService.EnviarMensagem(atendimento.Cliente.Email_Principal, $"[Atendimento #{atendimento.Numero}] Status do Atendimento foi alterado para A Iniciar",
                                                                    $"Status do Atendimento foi alterado para A Iniciar", conta.Utilizar_Ssl_Smtp);
                                    break;
                                }
                            case Status_Atendimento.Cancelado:
                                {
                                    emailService.EnviarMensagem(atendimento.Cliente.Email_Principal, $"[Atendimento #{atendimento.Numero}] Status do Atendimento foi alterado para Cancelado",
                                                                    $"Status do Atendimento foi alterado para Cancelado", conta.Utilizar_Ssl_Smtp);
                                    break;
                                }
                            case Status_Atendimento.Concluido:
                                {
                                    emailService.EnviarMensagem(atendimento.Cliente.Email_Principal, $"[Atendimento #{atendimento.Numero}] Status do Atendimento foi alterado para Concluído",
                                                                    $"Status do Atendimento foi alterado para Concluído", conta.Utilizar_Ssl_Smtp);
                                    break;
                                }
                            case Status_Atendimento.Em_Atendimento:
                                {
                                    emailService.EnviarMensagem(atendimento.Cliente.Email_Principal, $"[Atendimento #{atendimento.Numero}] Status do Atendimento foi alterado para Em Atendimento",
                                                                    $"Status do Atendimento foi alterado para Em Atendimento", conta.Utilizar_Ssl_Smtp);
                                    break;
                                }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        private GetApiRetorno EnviarEmailComHistoricoMensagens_StatusAlterado(ISession session, Email_Atendimento email, String mensagem, String assunto, String messageID, List<string> mensagensAnteriores, List<string> referenciasAnteriores)
        {
            GetApiRetorno retorno = new GetApiRetorno();
            retorno.Ok = true;
            try
            {
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoEmail = new Conexao<Email_Atendimento>(session))
                {
                    Conta_Email conta = null;
                    if (email.Atendimento.Id_Email_Abertura != null)
                        conta = repoEmail.selectFirst(p => p.Id == email.Atendimento.Id_Email_Abertura).Result?.Conta_Email;

                    if (conta != null)
                    {
                        EmailUtil emailService = new EmailUtil(conta.Email, conta.Senha, conta.Servidor_Smtp, conta.Porta_Smtp, email?.Atendimento?.ContatosCopia?.Split(';').ToList(), new List<string>());
                        emailService.EnviarMensagemReplys(email.Atendimento.Cliente.Email_Principal,
                            assunto,
                            mensagem,
                            conta.Utilizar_Ssl_Smtp,
                            messageID,
                            referenciasAnteriores,
                            mensagensAnteriores);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        private String RetornoMensagem_MudancaStatus(Status_Atendimento status)
        {
            String retorno = String.Empty;

            switch (status)
            {
                case Status_Atendimento.Aguardando:
                    {
                        retorno = $"Status do Atendimento foi alterado para Aguardando";
                        break;
                    }
                case Status_Atendimento.A_Iniciar:
                    {
                        retorno = $"Status do Atendimento foi alterado para A Iniciar";
                        break;
                    }
                case Status_Atendimento.Cancelado:
                    {
                        retorno = $"Status do Atendimento foi alterado para Cancelado";
                        break;
                    }
                case Status_Atendimento.Concluido:
                    {
                        retorno = $"Status do Atendimento foi alterado para Concluído";
                        break;
                    }
                case Status_Atendimento.Em_Atendimento:
                    {
                        retorno = $"Status do Atendimento foi alterado para Em Atendimento";
                        break;
                    }
            }

            return retorno;
        }

        private List<Atendimento> Gravar_Numero(List<Atendimento> atendimentos)
        {
            return atendimentos.Select(atendimento =>
            {
                atendimento.Numero = this.configuracao.Formato_Protocolo_Atendimento == Formato_Protocolo_Atendimento.Ano_Mes_Dia_Hora_Sequencial ? String.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHH"), atendimento.Id) : atendimento.Id.ToString();
                return atendimento;
            }).ToList();
        }

        /// <summary>
        /// Busca por possiveis respostas do cliente ao email de abertura de atendimento
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="uids"></param>
        /// <param name="UltimaExecucao"></param>
        /// <param name="repoAtendimento"></param>
        /// <param name="CaminhoFisicoConnect"></param>
        /// <returns></returns>
        private async Task ChecarRespostas(ImapClient Client, IEnumerable<uint> uids, DateTime UltimaExecucao, Conexao<Atendimento> repoAtendimento, String CaminhoFisicoConnect, String UrlPortalConnect)
        {
            IEnumerable<MailMessage> replies = Client.GetMessages(uids.Distinct()).Where(p => p.Date() > UltimaExecucao && (p.Headers.AllKeys.Contains("In-Reply-To") || p.Headers.AllKeys.Contains("References")));
            var replies_content = replies.Select(reply =>
            {
                var reply_to = reply.Headers.GetValues("In-Reply-To");
                var subject = reply.Headers.GetValues("Subject")[0];
                try
                {
                    if (subject.Contains("#") && subject.Contains("]"))
                    {
                        var numero_atendimento = subject.Split('#')[1].Split(']')[0];
                        return (numero_atendimento, reply);
                    }
                    else return (null, null);
                }
                catch
                {
                    return (null, null);
                }

            }).Where(p => p.numero_atendimento != null && p.reply != null).ToList();

            var atendimentos_replied = await repoAtendimento.select(p => p.Numero.IsIn(replies_content.Select(reply => reply.numero_atendimento).ToList()));
            var atendimentos_acoes = replies_content.Select(p => GerarNovaAcao(atendimentos_replied.Where(y => y.Numero == p.numero_atendimento).FirstOrDefault(), p.reply, repoAtendimento, p.reply?.Headers != null ? p.reply?.Headers.Get("Message-ID") : "").Result).Where(p => p.atendimento_Acao != null).ToList();
            atendimentos_acoes.ForEach(acao => GravarAnexosAcao(acao.atendimento_Acao, CaminhoFisicoConnect, UrlPortalConnect, acao.Attachments, acao.alternateViews));


            foreach (var item in atendimentos_acoes)
            {
                using (var api = new NotificacoesApiController())
                {
                    var notificacao = api.EnviaNotificacoesAtendimento(47, item.atendimento_Acao?.Atendimento, null);
                }
            }

        }
        public static string CorrigirCodificacao(string textoOriginal)
        {
            // Verifica se o texto parece estar com encoding quebrado (caracteres estranhos)
            // Exemplo: “Ã©” no lugar de “é” indica UTF-8 lido como Latin1
            if (Regex.IsMatch(textoOriginal, @"Ã[a-zA-Z]|Â.|â.|ê.|õ.|ç|â€œ|â€"))
            {
                // Faz a "recodificação": trata como Latin1 e reconstrói como UTF-8
                byte[] isoBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(textoOriginal);
                return Encoding.UTF8.GetString(isoBytes);
            }

            // Se não detectou problema, retorna como está
            return textoOriginal;
        }

        /// <summary>
        /// Gera novo registro da entidade Atendimento_Acao através de mensagem de email
        /// </summary>
        /// <param name="atendimento"></param>
        /// <param name="message"></param>
        /// <param name="repoAtendimento"></param>
        /// <returns></returns>
        private async Task<(Atendimento_Acao atendimento_Acao, AttachmentCollection Attachments, AlternateViewCollection alternateViews)> GerarNovaAcao(Atendimento atendimento, MailMessage message, Conexao<Atendimento> repoAtendimento, string messageId = null)
        {
            var messageID = message.Headers["Message-ID"];
            if (atendimento == null)
            {
                return (null, message.Attachments, message.AlternateViews);
            }

            if (!String.IsNullOrEmpty(messageID))
            {
                var atendimentoAcaoReferenciado = atendimento?.Ultima_Acao != null ? atendimento.Ultima_Acao.Where(p => p.MessageID == messageID).FirstOrDefault() : null;
                if (atendimentoAcaoReferenciado != null)
                {
                    return (null, message.Attachments, message.AlternateViews);
                }
            }

            AlternateView view = message.AlternateViews.FirstOrDefault();
            string corpo = null;
            if (view != null)
            {
                corpo = GetCorpo(view);
            }
            else
            {
                corpo = message.Body; // cuidado: isso pode já estar decodificado errado
            }

            Atendimento_Acao nova_acao = new Atendimento_Acao
            {
                Atendimento = atendimento,
                Status_Atendimento = atendimento.Status_Atendimento,
                HTML = message.IsBodyHtml ? String.Empty : RemoverHistoricoRespostas(corpo),
                Responsavel = atendimento.Responsavel,
                Origem = Origem_AtendimentoAcao.EMAIL,
                MessageID = messageId,
                TipoAtividade = TipoAtividadeAtendimento.Publico
            };

            nova_acao.Arquivos_AtendimentoAcao = new List<Arquivo_AtendimentoAcao>();

            foreach (var arquivo in message.Attachments)
            {
                try
                {
                    var arquivo_AtendimentoAcao = new Arquivo_AtendimentoAcao();
                    arquivo_AtendimentoAcao.Arquivo = Path.GetFileNameWithoutExtension(DecodeMimeHeader(arquivo.Name));
                    arquivo_AtendimentoAcao.Extensao = Path.GetExtension(DecodeMimeHeader(arquivo.Name));
                    arquivo_AtendimentoAcao.Atendimento_Acao = nova_acao;
                    nova_acao.Arquivos_AtendimentoAcao.Add(arquivo_AtendimentoAcao);
                }
                catch (Exception ex)
                {
                    this.log4net.Info("ERRO: " + ex.Message + " INNER: " + ex.InnerException?.Message + " STACK: " + ex.StackTrace);
                }

            }


            var arquivos_inline = message.AlternateViews.Select(p =>
            {
                if (p.ContentType.MediaType == "image/jpeg")
                    return new Arquivo_AtendimentoAcao
                    {
                        Arquivo = p.ContentId,
                        Extensao = ".jpeg",
                        Atendimento_Acao = nova_acao
                    };
                else if (p.ContentType.MediaType == "image/png")
                    return new Arquivo_AtendimentoAcao
                    {
                        Arquivo = p.ContentId,
                        Extensao = ".png",
                        Atendimento_Acao = nova_acao
                    };
                else
                    return null;
            }).Where(p => p != null);


            string mensagemHtmlTratada = RemoverHistoricoRespostas(corpo);
            nova_acao.HTML = mensagemHtmlTratada;

            nova_acao.Arquivos_AtendimentoAcao = nova_acao.Arquivos_AtendimentoAcao.Concat(arquivos_inline).ToList();

            var contatosEmail = message.CC != null ? message.CC.Select(x => x.Address).ToList() : null;

            var contatosNovos = new List<String>();

            if (contatosEmail != null && contatosEmail.Count > 0)
            {
                var contatosJaAdd = atendimento.ContatosCopia != null ? atendimento.ContatosCopia.Split(';').ToList() : new List<string>();
                foreach (var ctt in contatosEmail)
                {
                    if (!contatosJaAdd.Any(x => x == ctt))
                        contatosNovos.Add(ctt);
                }
            }

            if (contatosNovos.Count > 0)
            {
                if (String.IsNullOrEmpty(atendimento.ContatosCopia))
                    atendimento.ContatosCopia = String.Join(";", contatosNovos);
                else
                    atendimento.ContatosCopia = String.Join(";", atendimento.ContatosCopia.Split(';').Concat(contatosNovos));
            }

            atendimento.Ultima_Acao.Add(nova_acao);
            await repoAtendimento.SaveOrUpdate(atendimento);


            return (nova_acao, message.Attachments, message.AlternateViews);
        }

        private async Task<(long? numOs, AttachmentCollection Attachments, AlternateViewCollection alternateViews)> BuscarArquivoOsDigital(MailMessage message)
        {
            try
            {
                string input = message.Subject;
                string output = input.Split('[', ']')[1];
                long? contrato = Convert.ToInt64(output.Split('/')[1]);

                return (contrato, message.Attachments, message.AlternateViews);
            }
            catch
            {
                return (null, message.Attachments, message.AlternateViews);
            }
        }

        /// <summary>
        /// Grava no diretorio do connect os arquivos que do atendimento que vieram por email
        /// </summary>
        /// <param name="nova_acao"></param>
        /// <param name="CaminhoFisicoConnect"></param>
        /// <param name="Attachments"></param>
        private void GravarAnexosAcao(Atendimento_Acao nova_acao, String CaminhoFisicoConnect, String UrlPortalConnect, AttachmentCollection Attachments, AlternateViewCollection alternateViews)
        {
            var _pasta = $@"{CaminhoFisicoConnect}\Arquivos\Atendimento";
            if (!Directory.Exists(_pasta))
            {
                Directory.CreateDirectory(_pasta);
            }

            foreach (var arquivo in nova_acao.Arquivos_AtendimentoAcao)
            {
                var _arquivo = $@"{_pasta}\{arquivo.Id}{arquivo.Extensao}";

                var attachment = Attachments.Where(p => DecodeMimeHeader(p.Name) == String.Concat(arquivo.Arquivo, arquivo.Extensao)).FirstOrDefault();
                var alternateview = alternateViews.Where(p => p.ContentId == arquivo.Arquivo).FirstOrDefault();

                if (attachment != null)
                {
                    byte[] allBytes = new byte[attachment.ContentStream.Length];
                    int bytesRead = attachment.ContentStream.Read(allBytes, 0, (int)attachment.ContentStream.Length);
                    File.WriteAllBytes($@"{_arquivo}", allBytes);
                }
                else if (alternateview != null)
                {
                    byte[] allBytes = new byte[alternateview.ContentStream.Length];
                    int bytesRead = alternateview.ContentStream.Read(allBytes, 0, (int)alternateview.ContentStream.Length);
                    File.WriteAllBytes($@"{_arquivo}", allBytes);
                }

            }

            if (alternateViews.Where(p => p.ContentType.MediaType == "text/html").Count() > 0)
            {
                foreach (var arquivo in nova_acao.Arquivos_AtendimentoAcao)
                {
                    if (nova_acao.HTML.Contains("cid:" + arquivo.Arquivo))
                    {
                        nova_acao.HTML = nova_acao.HTML.Replace("cid:" + arquivo.Arquivo, $@"{UrlPortalConnect}/Arquivos/Atendimento/{arquivo.Id}{arquivo.Extensao}");
                    }
                }

                nova_acao.HTML = RemoverHistoricoRespostas(nova_acao.HTML);
            }
        }

        private string GetCorpo(AlternateView view)
        {
            string corpo = null;

            if (view != null)
            {
                Encoding encoding;

                // 1. Tenta pegar do CharSet
                if (!string.IsNullOrWhiteSpace(view.ContentType.CharSet))
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(view.ContentType.CharSet);
                    }
                    catch
                    {
                        encoding = null; // Charset inválido
                    }
                }
                else
                {
                    encoding = null;
                }

                // 2. Se não conseguiu, tenta detectar com Ude
                if (encoding == null)
                {
                    view.ContentStream.Position = 0;
                    var detector = new CharsetDetector();
                    detector.Feed(view.ContentStream);
                    detector.DataEnd();

                    if (!string.IsNullOrWhiteSpace(detector.Charset))
                    {
                        try
                        {
                            encoding = Encoding.GetEncoding(detector.Charset);
                        }
                        catch
                        {
                            encoding = Encoding.UTF8; // fallback
                        }
                    }
                    else
                    {
                        encoding = Encoding.UTF8; // fallback
                    }
                }

                // 3. Agora lê o conteúdo com encoding detectado
                view.ContentStream.Position = 0;
                using (var reader = new StreamReader(view.ContentStream, encoding))
                {
                    corpo = reader.ReadToEnd();
                }
            }

            return corpo;
        }

        private string RemoverHistoricoRespostas(string mensagem)
        {
            string response = string.Empty;

            if (mensagem.Contains("-----Resposta-----"))
            {
                string procurarTexto = "-----Resposta-----";

                int index = mensagem.IndexOf(procurarTexto);

                if (index != -1)
                {
                    response = mensagem.Substring(0, index);
                }

                response = response.Replace("<br>", "");
            }
            else
            {
                response = mensagem;
            }

            return response;
        }

        private async Task GravarOsDigital(Int64? contrato, AttachmentCollection Attachments)
        {
            try
            {
                using (ISession sessiont5 = HibernateUtil.GetSessionT5())
                using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
                using (var repoArquivoDocProd = new Conexao<Arquivo_DocProd>(sessionFoto))
                using (var repoDocumentoProducao = new Conexao<Documento_ProducaoAniel>(sessiont5))
                {
                    if (contrato.HasValue)
                    {
                        var pageID = new int[] { 1, 2 };
                        this.log4net.Info($"buscando servicos com contrato {contrato}");
                        var documentos = await repoDocumentoProducao.select(p => p.Num_Doc == contrato);
                        documentos = documentos.Where(p => p.TemOsDigital != "S").ToList();

                        if (documentos.Count > 0)
                        {
                            this.log4net.Info($"quantidade de servicos encontrados: {documentos.Count} ");
                            string sourcePdf = @$"{this.caminhoFisico}\OSDIGITAL.pdf";
                            string targetHtml = @$"{this.caminhoFisico}\OSDIGITAL.html";

                            using (var fileStream = File.Create(sourcePdf))
                            {
                                Attachments.First(p => p.Name.ToUpper().Contains("DIGITAL")).ContentStream.Seek(0, SeekOrigin.Begin);
                                Attachments.First(p => p.Name.ToUpper().Contains("DIGITAL")).ContentStream.CopyTo(fileStream);
                            }

                            if (File.Exists(targetHtml))
                                File.Delete(targetHtml);

                            PdfToHtmlNet.Converter c = new PdfToHtmlNet.Converter();
                            c.Encoding = Encoding.UTF8;
                            c.ConvertToFile(sourcePdf, targetHtml, pageID);
                            string html = File.ReadAllText(targetHtml);


                            foreach (var documento in documentos)
                            {
                                bool numeroOsEncontrada = false;
                                if (html.Contains(documento.Numero_Obra_Original))
                                {
                                    numeroOsEncontrada = true;
                                }

                                if (documento != null && numeroOsEncontrada)
                                {
                                    if (documentos.Where(p => p.CodCt == documento.CodCt &&
                                                          p.Projeto == documento.Projeto &&
                                                          p.Num_Doc == documento.Num_Doc &&
                                                          p.Numero_Obra_Original == documento.Numero_Obra_Original).Max(p => p.Seq) == documento.Seq)
                                    {
                                        List<Arquivo_DocProd> temp = new List<Arquivo_DocProd>();
                                        temp = Attachments.Where(p => p.Name.ToUpper().Contains("DIGITAL")).Select(arquivo =>
                                        {
                                            byte[] allBytes = System.IO.File.ReadAllBytes(sourcePdf);
                                            return new Arquivo_DocProd
                                            {
                                                Id = (repoArquivoDocProd.Max(p => p.Id)).Result + 1,
                                                Chave = $"{documento.CodCt}&{documento.Projeto}&{documento.Num_Doc}",
                                                Descricao = Path.GetFileNameWithoutExtension(arquivo.Name),
                                                Tipo = Path.GetExtension(arquivo.Name).Replace(".", ""),
                                                Arquivo = allBytes,
                                                Data = DateTime.Now.Date,
                                                Hora = DateTime.Now,
                                                Usuario = User.Identity.GetUserId(),
                                                Num_Obra = documento.Num_Obra,
                                                OsDigital = "S"
                                            };
                                        }).ToList();
                                        await repoArquivoDocProd.SaveOrUpdate(temp);
                                    }

                                    documento.TemOsDigital = "S";
                                    await repoDocumentoProducao.Update(documento);
                                    this.log4net.Info($"OS DIGITAL vinculada ao documento {documento.Numero_Obra_Original}");
                                }
                            }

                            File.Delete(sourcePdf);
                            File.Delete(targetHtml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.log4net.Error($"erro durante integracaco de contrato: {contrato}; erro : {ex.Message}; innerException: {ex.InnerException?.Message}; stack: {ex.StackTrace}");
            }
        }

        public static string DecodeMimeHeader(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return Regex.Replace(input, @"=\?([^\?]+)\?([BbQq])\?([^\?]+)\?=", match =>
            {
                var charset = match.Groups[1].Value;
                var encoding = match.Groups[2].Value.ToUpperInvariant();
                var encodedText = match.Groups[3].Value;

                Encoding enc;
                try
                {
                    enc = Encoding.GetEncoding(charset);
                }
                catch
                {
                    enc = Encoding.UTF8; // fallback
                }

                if (encoding == "B") // Base64
                {
                    byte[] bytes = Convert.FromBase64String(encodedText);
                    return enc.GetString(bytes);
                }
                else if (encoding == "Q") // Quoted-printable
                {
                    string temp = encodedText.Replace('_', ' ');
                    byte[] bytes = DecodeQuotedPrintable(temp);
                    return enc.GetString(bytes);
                }

                return match.Value; // não reconhecido
            }, RegexOptions.IgnoreCase).Replace("\t", "").Replace("\r", "").Replace("\n", "").Trim();
        }

        private static byte[] DecodeQuotedPrintable(string input)
        {
            var output = new List<byte>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '=' && i + 2 < input.Length)
                {
                    string hex = input.Substring(i + 1, 2);
                    if (byte.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out byte b))
                    {
                        output.Add(b);
                        i += 2;
                    }
                    else
                    {
                        output.Add((byte)input[i]);
                    }
                }
                else
                {
                    output.Add((byte)input[i]);
                }
            }
            return output.ToArray();
        }

    }
}

