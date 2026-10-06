using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using MongoDB.Driver;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ISession = NHibernate.ISession;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class NotificacoesApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/NotificacoesApi/EnviaNotificacoesServico")]
        public async Task<PostApiRetorno<Int64>> EnviaNotificacoesServico(int idNotificacao, String CodCt, String Projeto, String NumObra, Int64? NumDoc, String parametro)
        {
            NotificacaoBusiness notiBuss = new NotificacaoBusiness();
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                Notificacoes modelo = null;

                using ISession session = HibernateUtil.GetSession();
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Notificacoes>(session))
                using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                using (var repositoryVinc = new Conexao<VincularAtendimento>(session))
                using (var repositoryDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repositoryNotificacoes = new Conexao<NotificacoesEnvio>(session))
                using (var repositoryDestinatarios = new Conexao<NotificacoesDestinatarios>(session))
                {
                    modelo = (await repository.selectFirst(p => p.Id == Convert.ToInt32(idNotificacao)));

                    Documento_ProducaoAniel documento = repositoryDocumento.selectFirst(p => p.CodCt == CodCt && p.Projeto == Projeto && p.Num_Obra == NumObra && p.Num_Doc == NumDoc).Result;
                    List<Atendimento> atendimentos = (await repositoryVinc.select(p => p.CodCT == CodCt && p.Projeto == Projeto && p.Num_Obra == NumObra)).Select(p => p.Atendimento).ToList();
                    if (documento.Id_Atendimento.HasValue)
                    {
                        atendimentos.Add(await repositoryAtendimento.selectFirst(p => p.Id == documento.Id_Atendimento));
                    }
                    if (modelo.Status == Status_Default.Ativo)
                    {
                        //1: Recupera a lsita de usuarios destinatários
                        var listaDestinatariosUsuario = repositoryDestinatarios.select(p => p.Notificacoes.Id == modelo.Id && p.Usuario_Destinatario != null).Result.Select(p => new NotificacoesDestinatarios
                        {
                            Id = p.Id,
                            Usuario_Destinatario = p.Usuario_Destinatario
                        }).ToList();

                        //2: Recupera a lista de perfis destinatários
                        var listaDestinatariosPerfil = repositoryDestinatarios.select(p => p.Notificacoes.Id == modelo.Id && p.Usuario_Destinatario == null).Result.Select(p => new NotificacoesDestinatarios
                        {
                            Id = p.Id,
                            Usuario_Destinatario = p.Usuario_Destinatario,
                            Tipo_Destinatario = p.Tipo_Destinatario
                        }).ToList();

                        var statusViaveisNotificacao = new List<Status_Atendimento>
                        {
                            Status_Atendimento.Em_Atendimento,
                            Status_Atendimento.Aguardando,
                        };
                        foreach (var atendimento in atendimentos.Where(p => statusViaveisNotificacao.Contains(p.Status_Atendimento) && p.Responsavel != null))
                        {
                            //3: Recupera os usuários da lista de perfis destinatários, já verificando se o usuário já estava na lista
                            List<NotificacoesDestinatarios> listaEnviar = await verificaListaUsuarioServico(listaDestinatariosUsuario, listaDestinatariosPerfil, modelo, documento);

                            //4: Aplica as variáveis
                            String textoNoti = notiBuss.AplicarVariaveis(modelo.Notificacao, documento, parametro, atendimento);
                            String chave = "";

                            //AÇÕES AO CLICAR // CHAVES 
                            if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Vincular_monitor_ao_tecnico)
                            {
                                chave = $"{parametro}";
                            }
                            else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Timeline_servicos)
                            {
                                chave = $"{documento.CodCt}|{documento.Cliente.NomeFantasia}|{documento.Num_Doc}|{documento.Num_Obra}|{documento.Projeto}";
                            }
                            else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Ordem_servico_os)
                            {
                                chave = $"{documento.CodCt}|{documento.Num_Doc}|{documento.Num_Obra}|{documento.Projeto}";
                            }
                            else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Acompanhamento_tecnicos)
                            {
                                chave = $"{documento.CodCt}|{documento.Num_Doc}|{documento.Num_Obra}|{documento.Projeto}";
                            }
                            else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Painel_roteirizacao)
                            {
                                chave = $"{documento.CodCt}|{documento.Cliente.NomeFantasia}|{documento.Num_Doc}|{documento.Num_Obra}|{documento.Projeto}|{documento.Usuario_Monitor}";
                            }
                            else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Painel_servicos_sem_agendamento)
                            {
                                chave = $"{documento.CodCt}|{documento.Cliente.NomeFantasia}|{documento.Num_Doc}|{documento.Num_Obra}|{documento.Projeto}|{documento.Usuario_Monitor}";
                            }
                            else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Baixa_OS)
                            {
                                chave = $"{documento.Numero_Obra_Original}|{documento.Status}|{atendimento.Protocolo}";
                            }

                            foreach (var envioLista in listaEnviar)
                            {
                                NotificacoesEnvio envio = new NotificacoesEnvio
                                {
                                    Data_Envio = DateTime.Now,
                                    Texto_Notificacao = textoNoti,
                                    Status_Notificacao = Model.enumerado.Status_Notificacao.NaoLido,
                                    Usuario_Destino = envioLista.Usuario_Destinatario,
                                    Notificacoes = modelo,
                                    DataHoraCriacao = DateTime.Now,
                                    Acao = (Notificacoes_Acao_Executada)Enum.Parse(typeof(Notificacoes_Acao_Executada), Convert.ToString(modelo.AcaoExecutada)),
                                    Chave = chave
                                };

                                repositoryNotificacoes.SaveOrUpdate(envio).Wait();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }
        private async Task<List<NotificacoesDestinatarios>> verificaListaUsuarioServico(List<NotificacoesDestinatarios> listaDestinatariosUsuario, List<NotificacoesDestinatarios> listaDestinatariosPerfil, Notificacoes value, Documento_ProducaoAniel documento)
        {
            List<NotificacoesDestinatarios> listaDestinatariosFinal = new List<NotificacoesDestinatarios>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repositoryDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repositoryAtendimento = new Conexao<Atendimento>(session))
            {
                foreach (var lista in listaDestinatariosPerfil)
                {
                    //Monitor Responsável
                    if (lista.Tipo_Destinatario == Model.enumerado.TipoUsuarioEnum.MONITOR_EQUIPE)
                    {
                        string usuarioMonitor = repositoryDocumento.selectFirst(p => p.Numero == documento.Numero).Result.Usuario_Monitor;

                        if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == usuarioMonitor).Count() == 0)
                        {
                            listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                            {
                                Notificacoes = value,
                                Usuario_Destinatario = usuarioMonitor
                            });
                        }
                    }
                    //Supervisor Equipe
                    else if (lista.Tipo_Destinatario == Model.enumerado.TipoUsuarioEnum.SUPERVISOR_EQUIPE)
                    {

                        string usuarioSupervisor = repositoryDocumento.selectFirst(p => p.Numero == documento.Numero).Result.Cod_Superv;

                        if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == usuarioSupervisor).Count() == 0)
                        {
                            listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                            {
                                Notificacoes = value,
                                Usuario_Destinatario = usuarioSupervisor
                            });
                        }
                    }
                    else if (lista.Tipo_Destinatario == Model.enumerado.TipoUsuarioEnum.ATENDENTE)
                    {
                        long? IdAtendimento = documento.Id_Atendimento;
                        string Responsavel = (await repositoryAtendimento.selectFirst(p => p.Id == IdAtendimento)).Responsavel.Funcionario.Num_Fre;

                        if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == Responsavel).Count() == 0)
                        {
                            listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                            {
                                Notificacoes = value,
                                Usuario_Destinatario = Responsavel
                            });
                        }
                    }
                }
            }

            return listaDestinatariosUsuario;
        }
    }
}
