using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Model.HubSoft.MovimentoCliente;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Office2010.Excel;
using NHibernate;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Twilio.TwiML.Voice;
using Atendimento = Aniel.Connect.Model.nhibernate.Atendimento;
using TipoUsuarioEnum = Aniel.Connect.Model.enumerado.TipoUsuarioEnum;
namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class NotificacoesApiController : BaseApiController
    {


        [AcceptVerbs("Get", "Post")]
        [Route("api/NotificacoesApi/GetAll")]
        public async Task<DataTableResponse<Notificacoes_ListModel>> GetAll(Notificacoes_DataTableRequest request)
        {

            List<Notificacoes_ListModel> retorno = new List<Notificacoes_ListModel>();
            IOrderedEnumerable<Notificacoes_ListModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_NOTIFICACOES");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.NOTIFICACAO, A.STATUS, A.SECAO_CONNECT, A.DESCRICAO ");
                    sql.AppendLine(" FROM TB_NOTIFICACOES A");
                    sql.AppendLine("WHERE A.ID > 0 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        sql.AppendLine("AND (A.STATUS = @STATUS)");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status));
                    }
                    sql.AppendLine(" ORDER BY A.ID");
                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Notificacoes_ListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),

                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.ATIVO : RsCadastros.INATIVO,
                                SecaoConnect = HtmlEnumExtensions.GetEnumDescription<Model.enumerado.SecaoConnectPerfilEnum>(Convert.ToString(reader["SECAO_CONNECT"])),
                                Mensagem = Convert.ToString(reader["NOTIFICACAO"]),
                                Notificacao = Convert.ToString(reader["NOTIFICACAO"]),
                            });
                        }
                    }
                }
            }

            //Ordenacao
            listaOrdenada = retorno.OrderBy(p => p.Id);

            //Page e retornando DataTable
            return new DataTableResponse<Notificacoes_ListModel>
            {
                draw = request.draw,
                //recordsTotal = listaOrdenada.Count(),
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/NotificacoesApi/GetNotificacoes")]
        public async Task<List<GetNotificacoesResponse>> GetNotificacoes(String usuario, String tipoStatus)
        {
            List<GetNotificacoesResponse> retorno = new List<GetNotificacoesResponse>();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(" SELECT A.ID,A.TEXTO_NOTIFICACAO, A.STATUS, A.DATA_ENVIO,A.CHAVE, A.ID_NOTIFICACOES, A.ACAO FROM TB_NOTIFICACOES_ENVIO A ");

                    List<String> filtros = new List<string>();

                    filtros.Add("(USUARIO_DESTINO = @USUARIO)");
                    parametros.Add("@USUARIO", usuario);

                    //Filtra somente não lidas
                    if (String.IsNullOrEmpty(tipoStatus) == false)
                    {
                        filtros.Add("(A.STATUS = @STATUS)");
                        parametros.Add("@STATUS", tipoStatus);
                    }

                    //Filtrar somente os últimos 31 dias
                    var dataFim = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                    var dataInicio = DateTime.Today.AddDays(-31);
                    filtros.Add("(A.DATA_ENVIO BETWEEN @INICIO AND @FIM)");
                    parametros.Add("@INICIO", dataInicio);
                    parametros.Add("@FIM", dataFim);

                    if (filtros.Count > 0)
                    {
                        sql.AppendLine(" WHERE ");
                        sql.AppendLine(String.Join(" AND ", filtros));
                    }

                    sql.AppendLine(" ORDER BY A.DATA_ENVIO DESC ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 20, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            var notificacao = new GetNotificacoesResponse
                            {
                                Id = Convert.ToString(reader["ID"]),
                                IdNotificacao = Convert.ToString(reader["ID_NOTIFICACOES"]),
                                Acao = Convert.ToString(reader["ACAO"]),

                                Texto_Notificacao = Convert.ToString(reader["TEXTO_NOTIFICACAO"]),
                                Status_Notificacao = (Status_Notificacao)Enum.Parse(typeof(Status_Notificacao), Convert.ToString(reader["STATUS"])),
                                Data_Envio = reader["DATA_ENVIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ENVIO"]).ToString("dd/MM/yyyy HH:mm") : null,
                                Chave = Convert.ToString(reader["CHAVE"])
                            };

                            retorno.Add(notificacao);
                        }
                    }
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/NotificacoesApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_NOTIFICACOES_ENVIO SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
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

        [HttpPost()]
        [Route("api/NotificacoesApi/AlterarStatusAtivoInativo")]
        public async Task<PostApiRetorno<Int64>> AlterarStatusAtivoInativo(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_NOTIFICACOES SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
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

        [HttpPost()]
        [Route("api/NotificacoesApi/EnviaNotificacaoNovaMensagem")]
        public async Task<PostApiRetorno<Int64>> EnviaNotificacaoNovaMensagem(List<ServicoUsuario> lista, Documento_ProducaoAniel documento)
        {
            NotificacaoBusiness notiBuss = new NotificacaoBusiness();
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using ISession session = HibernateUtil.GetSession();
                using (var repositoryNotificacoesEnvio = new Conexao<NotificacoesEnvio>(session))
                using (var repositoryNotificacoes = new Conexao<Notificacoes>(session))
                {
                    //ATENÇÃO: ID PERMANENTE UTILIZADO NO MIGRATION TB_NOTIFICACOES
                    var modelo = (await repositoryNotificacoes.selectFirst(p => p.Id == 37));

                    if (modelo.Status == Status_Default.Ativo)
                    {
                        String textoNoti = notiBuss.AplicarVariaveis(modelo.Notificacao, documento, null);

                        foreach (var envioLista in lista.Where(p => p.tipo_usuario != TipoUsuarioChatEnum.AGENTE).ToList())
                        {

                            NotificacoesEnvio envio = new NotificacoesEnvio
                            {
                                Data_Envio = DateTime.Now,
                                Texto_Notificacao = textoNoti,
                                Status_Notificacao = Model.enumerado.Status_Notificacao.NaoLido,
                                Usuario_Destino = Convert.ToString(envioLista.num_fre),
                                Notificacoes = modelo,
                                DataHoraCriacao = DateTime.Now,
                                Acao = (Notificacoes_Acao_Executada)Enum.Parse(typeof(Notificacoes_Acao_Executada), Convert.ToString(modelo.AcaoExecutada)),
                                Chave = Convert.ToString($"{documento.CodCt}|{documento.Projeto}|{documento.Num_Doc}|{documento.Num_Obra}")
                            };

                            repositoryNotificacoesEnvio.SaveOrUpdate(envio).Wait();
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
                    var atendimentosvinculadosOS = (await repositoryVinc.select(p =>
                       p.CodCT == documento.CodCt &&
                       p.Projeto == documento.Projeto &&
                       p.Num_Obra == documento.Num_Obra &&
                       p.Num_Doc == Convert.ToString(documento.Num_Doc)
                       )).ToList();
                    List<Atendimento> atendimentos = atendimentosvinculadosOS.Where(p => p.Atendimento != null).Select(p => p.Atendimento).Count() > 0 ?
                            atendimentosvinculadosOS.Select(p => p.Atendimento).ToList() : new List<Atendimento>();

                    if (atendimentos.Count == 0)
                    {
                        var resultado = session.QueryOver<Atendimento>()
                                               .WhereRestrictionOn(e => e.Protocolo)
                                               .IsIn(atendimentosvinculadosOS.Select(y => y.Numero_Atendimento).ToList())
                                               .List<Atendimento>();
                        atendimentos.AddRange(resultado);
                    }
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
                            List<NotificacoesDestinatarios> listaEnviar = await verificaListaUsuarioServico(listaDestinatariosUsuario, listaDestinatariosPerfil, modelo, documento, atendimento);


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

        [HttpPost()]
        [Route("api/NotificacoesApi/EnviaNotificacoesAtendimento")]
        public async Task<PostApiRetorno<Int64>> EnviaNotificacoesAtendimento(int idNotificacao, Atendimento atendimento, string parametro = null)
        {
            NotificacaoBusiness notiBuss = new NotificacaoBusiness();
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                Notificacoes modelo = null;

                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<Notificacoes>(session))
                using (var repositoryNotificacoes = new Conexao<NotificacoesEnvio>(session))
                using (var repositoryDestinatarios = new Conexao<NotificacoesDestinatarios>(session))
                using (var repositoryDocumento = new Conexao<Documento_ProducaoAniel>(session))
                using (var conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    modelo = (await repository.selectFirst(p => p.Id == Convert.ToInt32(idNotificacao)));

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

                        //3: Recupera os usuários da lista de perfis destinatários, já verificando se o usuário já estava na lista
                        List<NotificacoesDestinatarios> listaEnviar = await verificaListaUsuarioAtendimento(listaDestinatariosUsuario, listaDestinatariosPerfil, modelo, atendimento);

                        if (modelo.Id == 38)
                        {
                            //usuários convidados no chat
                            AtendimentoChatMongo chat = await repositoryMongo.Find(p => p.protocolo == atendimento.Protocolo);

                            var usuarios = chat.usuariosConvidados
                                .Where(p => p.aceite == null && p.dataAceite == null)
                                .ToList();

                            foreach (var usuario in usuarios)
                            {
                                listaEnviar.Add(new NotificacoesDestinatarios()
                                {
                                    Notificacoes = modelo,
                                    Usuario_Destinatario = usuario.usuarioAceite
                                });
                            }
                        }

                        //4: Aplica as variáveis
                        String textoNoti = notiBuss.AplicarVariaveisAtendimento(modelo.Notificacao, atendimento, parametro);

                        //5: Aplica as chaves de acordo com a ação
                        String chave = "";
                        if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Acompanhamento_atendimento ||
                            modelo.AcaoExecutada == Notificacoes_Acao_Executada.Aceitar_convite_chat ||
                            modelo.AcaoExecutada == Notificacoes_Acao_Executada.Painel_chat_atendimento)
                        {
                            chave = $"{atendimento.Id}|{atendimento.Protocolo}";
                        }
                        else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Ordem_servico_os)
                        {
                            Documento_ProducaoAniel documento = (await repositoryDocumento.selectFirst(p => p.Id_Atendimento == Convert.ToInt32(atendimento.Id)));
                            if (documento != null)
                            {
                                chave = $"{documento.CodCt}|{documento.Cliente.NomeFantasia}|{documento.Num_Doc}|{documento.Num_Obra}";
                            }
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
                                Acao = modelo.AcaoExecutada,
                                Chave = chave
                            };

                            await repositoryNotificacoes.SaveOrUpdate(envio);
                            retorno.IdObjeto = envio.Id;
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

        [HttpPost()]
        [Route("api/NotificacoesApi/EnviaNotificacoesComercial")]
        public async Task<PostApiRetorno<Int64>> EnviaNotificacoesComercial(int idNotificacao, Venda venda, string parametro)
        {
            NotificacaoBusiness notiBuss = new NotificacaoBusiness();
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                Notificacoes modelo = null;

                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<Notificacoes>(session))
                using (var repositoryNotificacoes = new Conexao<NotificacoesEnvio>(session))
                using (var repositoryDestinatarios = new Conexao<NotificacoesDestinatarios>(session))
                using (var repositoryVenda = new Conexao<Venda>(session))
                {
                    modelo = (await repository.selectFirst(p => p.Id == Convert.ToInt32(idNotificacao)));


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

                        //3: Recupera os usuários da lista de perfis destinatários, já verificando se o usuário já estava na lista
                        List<NotificacoesDestinatarios> listaEnviar = await verificaListaUsuarioComercial(listaDestinatariosUsuario, listaDestinatariosPerfil, modelo, venda);

                        //4: Aplica as variáveis
                        String textoNoti = notiBuss.AplicarVariaveisComercial(modelo.Notificacao, venda, parametro);

                        //5: Aplica as chaves de acordo com a ação
                        String chave = "";
                        if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Painel_vendas_reprovadas)
                        {
                            chave = $"{venda.Vendedor.Id}|{venda.Vendedor.Supervisor.Id}";
                        }
                        else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Painel_vendas_aprovadas)
                        {
                            chave = $"{venda.Vendedor.Id}|{venda.Vendedor.Supervisor.Id}";
                        }
                        else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Cadastro_campanha)
                        {
                            chave = $"{venda.Campanha.Id}";
                        }
                        else if (modelo.AcaoExecutada == Notificacoes_Acao_Executada.Cadastro_venda)
                        {
                            chave = $"{venda.Id}";
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
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        public async Task<List<NotificacoesDestinatarios>> verificaListaUsuarioAtendimento(List<NotificacoesDestinatarios> listaDestinatariosUsuario, List<NotificacoesDestinatarios> listaDestinatariosPerfil, Notificacoes value, Atendimento atendimento)
        {
            List<NotificacoesDestinatarios> listaDestinatariosFinal = new List<NotificacoesDestinatarios>();

            using ISession session = HibernateUtil.GetSession();
            using (var repository = new Conexao<AtendimentoChat>(session))
            using (var repositoryAtendimento = new Conexao<Atendimento>(session))
            using (var repoIdent = new Conexao<IdentityUser>(session))
            {
                foreach (var lista in listaDestinatariosPerfil)
                {
                    //Atendente responsável: troca de atendente
                    if (lista.Tipo_Destinatario == TipoUsuarioEnum.ATENDENTE && value.Id == 4)
                    {
                        IdentityUser atendenteResponsavel = repositoryAtendimento.selectFirst(p => p.Protocolo == atendimento.Protocolo).Result.Responsavel;

                        if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == atendimento.Responsavel.Id).Count() == 0)
                        {
                            listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                            {
                                Notificacoes = value,
                                Usuario_Destinatario = atendenteResponsavel.Id
                            });
                        }
                    }
                    //Atendente responsável
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.ATENDENTE && value.Id != 4)
                    {
                        IdentityUser atendenteResponsavel = repositoryAtendimento.selectFirst(p => p.Protocolo == atendimento.Protocolo).Result.Responsavel;

                        if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == atendenteResponsavel.Id).Count() == 0)
                        {
                            listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                            {
                                Notificacoes = value,
                                Usuario_Destinatario = atendenteResponsavel.Id
                            });
                        }
                    }
                    //Todos atendentes com o perfil de agente
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.TODOS_ATENDENTE_AGENTE)
                    {
                        using (var com1 = session.Connection.CreateCommand())
                        {

                            StringBuilder sql = new StringBuilder();
                            Dictionary<String, Object> parametros1 = new Dictionary<string, object>();

                            sql.AppendLine(" SELECT A.ID, C.NOME AS AGENTE, D.DESCRICAO AS SETOR, B.NUM_MAX_ATEND_SIMULT_AGENT, B.STATUS ");
                            sql.AppendLine(" FROM ASPNETUSERS A ");
                            sql.AppendLine(" INNER JOIN TB_AGENTE B ");
                            sql.AppendLine(" ON(B.ID = A.ID_PERFIL_AGENTE) ");
                            sql.AppendLine(" INNER JOIN TB_FUNCIONARIO C ");
                            sql.AppendLine(" ON(C.ID = A.ID_FUNCIONARIO) ");
                            sql.AppendLine(" LEFT JOIN TB_DEPARTAMENTO D ");
                            sql.AppendLine(" ON(D.ID = A.ID_DEPARTAMENTO) ");

                            sql.AppendLine(" WHERE B.PERFIL_ATENDE_CHAT = 1 AND STATUS = 1");

                            //Busca com paginacao
                            using (var reader = await CommandHelper.ExecuteReader(com1, sql, parametros1, session.Connection, null))
                            {

                                while (await reader.ReadAsync())
                                {
                                    var usuario = Convert.ToString(reader["ID"]);

                                    if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == usuario).Count() == 0)
                                    {
                                        listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                                        {
                                            Notificacoes = value,
                                            Usuario_Destinatario = usuario
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.TODOS_ATENDENTE)
                    {
                        using (var com1 = session.Connection.CreateCommand())
                        {

                            StringBuilder sql = new StringBuilder();
                            Dictionary<String, Object> parametros1 = new Dictionary<string, object>();

                            sql.AppendLine(" SELECT A.ID ");
                            sql.AppendLine(" FROM ASPNETUSERS A ");
                            sql.AppendLine(" WHERE A.TIPO_USUARIO = 0");

                            //Busca com paginacao
                            using (var reader = await CommandHelper.ExecuteReader(com1, sql, parametros1, session.Connection, null))
                            {

                                while (await reader.ReadAsync())
                                {
                                    var usuario = Convert.ToString(reader["ID"]);

                                    if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == usuario).Count() == 0)
                                    {
                                        listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                                        {
                                            Notificacoes = value,
                                            Usuario_Destinatario = usuario
                                        });
                                    }
                                }
                            }
                        }
                    }
                    //Supervisor Atendente Responsável
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.SUPERVISOR_ATENDENTE)
                    {
                        IdentityUser atendenteResponsavel = repositoryAtendimento.selectFirst(p => p.Protocolo == atendimento.Protocolo).Result.Responsavel;

                        using (var com2 = session.Connection.CreateCommand())
                        {

                            StringBuilder sql = new StringBuilder();
                            Dictionary<String, Object> parametros2 = new Dictionary<string, object>();

                            sql.AppendLine("SELECT C.ID FROM ASPNETUSERS A");
                            sql.AppendLine("INNER JOIN TB_FUNCIONARIO B ON(B.ID = A.ID_FUNCIONARIO)");
                            sql.AppendLine("INNER JOIN ASPNETUSERS C ON(C.ID_FUNCIONARIO = B.ID_SUPERVISOR)");
                            sql.AppendLine("WHERE A.ID = @USUARIO");

                            parametros2.Add("@USUARIO", atendenteResponsavel.Id);


                            //Busca com paginacao
                            using (var reader = await CommandHelper.ExecuteReader(com2, sql, parametros2, session.Connection, null))
                            {

                                while (await reader.ReadAsync())
                                {
                                    var usuario = Convert.ToString(reader["ID"]);

                                    if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == usuario).Count() == 0)
                                    {
                                        listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                                        {
                                            Notificacoes = value,
                                            Usuario_Destinatario = usuario
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.TODOS_SUPERVISOR_ATENDENTE)
                    {
                        IdentityUser atendenteResponsavel = repositoryAtendimento.selectFirst(p => p.Protocolo == atendimento.Protocolo).Result.Responsavel;

                        using (var com2 = session.Connection.CreateCommand())
                        {

                            StringBuilder sql = new StringBuilder();
                            Dictionary<String, Object> parametros2 = new Dictionary<string, object>();

                            sql.AppendLine(" SELECT A.ID ");
                            sql.AppendLine(" FROM ASPNETUSERS A ");
                            sql.AppendLine(" WHERE A.TIPO_USUARIO = 1");


                            parametros2.Add("@USUARIO", atendenteResponsavel.Id);


                            //Busca com paginacao
                            using (var reader = await CommandHelper.ExecuteReader(com2, sql, parametros2, session.Connection, null))
                            {

                                while (await reader.ReadAsync())
                                {
                                    var usuario = Convert.ToString(reader["ID"]);

                                    if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == usuario).Count() == 0)
                                    {
                                        listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                                        {
                                            Notificacoes = value,
                                            Usuario_Destinatario = usuario
                                        });
                                    }
                                }
                            }
                        }
                    }
                    //Responsável anterior
                    else if (value.Id == 5)
                    {
                        listaDestinatariosFinal.Add(new NotificacoesDestinatarios()
                        {
                            Notificacoes = value,
                            Usuario_Destinatario = atendimento.Responsavel.UserName
                        });

                    }

                    //Supervisores do setor responsável
                    else if (value.Id == 9)
                    {

                        var atendimentoSetor = repositoryAtendimento.selectFirst(p => p.Protocolo == atendimento.Protocolo).Result.Setor.Id;
                        var monitorSetor = repoIdent.selectFirst(p => p.Departamento.Id == atendimentoSetor && p.E_Monitor == true).Result.Id;

                        listaDestinatariosFinal.Add(new NotificacoesDestinatarios()
                        {
                            Notificacoes = value,
                            Usuario_Destinatario = monitorSetor
                        });
                    }
                }
            }

            return listaDestinatariosUsuario;
        }

        public async Task<List<NotificacoesDestinatarios>> verificaListaUsuarioServico(List<NotificacoesDestinatarios> listaDestinatariosUsuario, List<NotificacoesDestinatarios> listaDestinatariosPerfil, Notificacoes value, Documento_ProducaoAniel documento, Atendimento atendimento)
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
                    if (lista.Tipo_Destinatario == TipoUsuarioEnum.MONITOR_EQUIPE)
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
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.SUPERVISOR_EQUIPE)
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
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.ATENDENTE)
                    {                        
                        string Responsavel = atendimento?.Responsavel?.Funcionario?.Num_Fre;

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

        public async Task<List<NotificacoesDestinatarios>> verificaListaUsuarioComercial(List<NotificacoesDestinatarios> listaDestinatariosUsuario, List<NotificacoesDestinatarios> listaDestinatariosPerfil, Notificacoes value, Venda venda)
        {
            List<NotificacoesDestinatarios> listaDestinatariosFinal = new List<NotificacoesDestinatarios>();

            using ISession session = HibernateUtil.GetSession();
            using (var repository = new Conexao<Venda>(session))
            using (var repoIdent = new Conexao<IdentityUser>(session))
            {
                foreach (var lista in listaDestinatariosPerfil)
                {
                    //Atendente responsável
                    if (lista.Tipo_Destinatario == TipoUsuarioEnum.VENDEDOR)
                    {
                        var atendenteResponsavel = repository.selectFirst(p => p.Id == venda.Id).Result.Vendedor;

                        if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == atendenteResponsavel.Id.ToString()).Count() == 0)
                        {
                            listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                            {
                                Notificacoes = value,
                                Usuario_Destinatario = atendenteResponsavel.Num_Fre
                            });
                        }
                    }
                    //Todos atendentes com o perfil de agente
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.TODOS_VENDEDORES)
                    {
                        using (var com1 = session.Connection.CreateCommand())
                        {

                            StringBuilder sql = new StringBuilder();
                            Dictionary<String, Object> parametros1 = new Dictionary<string, object>();

                            sql.AppendLine(" SELECT A.ID, A.NUM_FRE, A.NOME ");
                            sql.AppendLine(" FROM TB_FUNCIONARIO A ");
                            sql.AppendLine(" WHERE A.VENDEDOR = 1 AND STATUS = 1");

                            //Busca com paginacao
                            using (var reader = await CommandHelper.ExecuteReader(com1, sql, parametros1, session.Connection, null))
                            {

                                while (await reader.ReadAsync())
                                {
                                    var usuario = Convert.ToString(reader["NUM_FRE"]);

                                    if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == usuario).Count() == 0)
                                    {
                                        listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                                        {
                                            Notificacoes = value,
                                            Usuario_Destinatario = usuario
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.TODOS_SUPERVISOR_COMERCIAL)
                    {
                        using (var com1 = session.Connection.CreateCommand())
                        {

                            StringBuilder sql = new StringBuilder();
                            Dictionary<String, Object> parametros1 = new Dictionary<string, object>();

                            sql.AppendLine(" SELECT A.ID, A.NUM_FRE, A.NOME ");
                            sql.AppendLine(" FROM TB_FUNCIONARIO A ");
                            sql.AppendLine(" WHERE A.VENDEDOR = 1 AND STATUS = 1 AND A.E_SUPERVISOR = 1 ");

                            //Busca com paginacao
                            using (var reader = await CommandHelper.ExecuteReader(com1, sql, parametros1, session.Connection, null))
                            {

                                while (await reader.ReadAsync())
                                {
                                    var usuario = Convert.ToString(reader["NUM_FRE"]);

                                    if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == usuario).Count() == 0)
                                    {
                                        listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                                        {
                                            Notificacoes = value,
                                            Usuario_Destinatario = usuario
                                        });
                                    }
                                }
                            }
                        }
                    }
                    //Supervisor Atendente Responsável
                    else if (lista.Tipo_Destinatario == TipoUsuarioEnum.SUPERVISOR_COMERCIAL)
                    {
                        var atendenteResponsavel = repository.selectFirst(p => p.Id == venda.Id).Result.Vendedor;

                        if (listaDestinatariosUsuario.FindAll(p => p.Usuario_Destinatario == atendenteResponsavel.Id.ToString()).Count() == 0)
                        {

                            var supervisor = repoIdent.selectFirst(p => p.Id == atendenteResponsavel.Supervisor.Id.ToString()).Result;

                            listaDestinatariosUsuario.Add(new NotificacoesDestinatarios()
                            {
                                Notificacoes = value,
                                Usuario_Destinatario = supervisor.UserName
                            });
                        }
                    }
                }
            }

            return listaDestinatariosUsuario;
        }


        [HttpPost()]
        [Route("api/NotificacoesApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Notificacoes_CadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Notificacoes modelo = null;

                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<Notificacoes>(session))
                using (var repositoryDestinatarios = new Conexao<NotificacoesDestinatarios>(session))
                {
                    if (value.Objeto.Id != null)
                        modelo = (await repository.selectFirst(p => p.Id == Convert.ToInt32(value.Objeto.Id)));


                    if (modelo == null)
                    {
                        modelo = new Notificacoes
                        {
                            UsuarioCriacao = value.Usuario,
                            Status = Status_Default.Ativo
                        };
                    }

                    else
                    {
                        modelo.UsuarioAlteracao = value.Usuario;
                        modelo.DataHoraAlteracao = DateTime.Now;
                    }

                    BindViewToObj(session, ref modelo, value.Objeto);

                    var destinatariosPadrao = new NotificacoesDestinatarios();
                    var listaDestinatariosPadrao = new List<NotificacoesDestinatarios>();

                    if (modelo.Id > 0) // só limpa se já existir notificação
                    {
                        var destinatariosExistentes = await repositoryDestinatarios.select(p => p.Notificacoes.Id == modelo.Id);
                        foreach (var dest in destinatariosExistentes)
                        {
                            await repositoryDestinatarios.Remove(dest);
                        }
                    }


                    //Gera lista de tipo usuário padrão
                    //SERVIÇOS
                    if (value.Objeto.IdSecaoConnect == Model.enumerado.SecaoConnectPerfilEnum.SERVICOS)
                    {
                        if (value.Objeto.MonitorEquipe == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.MONITOR_EQUIPE,
                            });
                        }
                        if (value.Objeto.SupervisorEquipe == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.SUPERVISOR_EQUIPE,
                            });
                        }
                    }

                    //COMERCIAL
                    else if (value.Objeto.IdSecaoConnect == Model.enumerado.SecaoConnectPerfilEnum.COMERCIAL)
                    {
                        if (value.Objeto.SupervisorComercialResp == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.SUPERVISOR_COMERCIAL,
                            });
                        }
                        if (value.Objeto.VendedorResp == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.VENDEDOR,
                            });
                        }
                        if (value.Objeto.TodosSupervisores == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_SUPERVISOR_COMERCIAL,
                            });
                        }
                        if (value.Objeto.TodosVendedores == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_VENDEDORES,
                            });
                        }
                    }

                    //ATENDIMENTO
                    else if (value.Objeto.IdSecaoConnect == Model.enumerado.SecaoConnectPerfilEnum.ATENDIMENTO)
                    {
                        if (value.Objeto.AtendenteResp == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.ATENDENTE,
                            });
                        }
                        if (value.Objeto.SupervisorResp == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.SUPERVISOR_ATENDENTE,
                            });
                        }
                        if (value.Objeto.AtendentesAgentes == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_ATENDENTE_AGENTE,
                            });
                        }
                        if (value.Objeto.TodosAtendentes == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_ATENDENTE,
                            });
                        }
                        if (value.Objeto.TodosSupervisoresAtendentes == true)
                        {
                            listaDestinatariosPadrao.Add(new NotificacoesDestinatarios
                            {
                                Notificacoes = modelo,
                                Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_SUPERVISOR_ATENDENTE,
                            });
                        }
                    }
                    //GESTÃO DE CONEXÕES
                    else if (value.Objeto.IdSecaoConnect == Model.enumerado.SecaoConnectPerfilEnum.GESTAO_CONEXOES)
                    {
                        if (value.Objeto.MonitorEquipe == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.MONITOR_EQUIPE;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.SupervisorEquipe == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.SUPERVISOR_EQUIPE;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.SupervisorComercialResp == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.SUPERVISOR_COMERCIAL;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.VendedorResp == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.VENDEDOR;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.TodosSupervisores == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_SUPERVISOR_COMERCIAL;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.TodosVendedores == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_VENDEDORES;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.AtendenteResp == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.ATENDENTE;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.SupervisorResp == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.SUPERVISOR_ATENDENTE;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.AtendentesAgentes == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_ATENDENTE_AGENTE;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.TodosAtendentes == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_ATENDENTE;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }

                        if (value.Objeto.TodosSupervisoresAtendentes == true)
                        {
                            destinatariosPadrao.Notificacoes = modelo;
                            destinatariosPadrao.Tipo_Destinatario = Model.enumerado.TipoUsuarioEnum.TODOS_SUPERVISOR_ATENDENTE;
                            listaDestinatariosPadrao.Add(destinatariosPadrao);
                        }
                    }

                    foreach (var destinatarios in value.Objeto.Destinatarios)
                    {
                        var destinatario = new NotificacoesDestinatarios
                        {
                            Usuario_Destinatario = destinatarios.Usuario,
                            Notificacoes = modelo,
                        };

                        await repositoryDestinatarios.SaveOrUpdate(destinatario);
                    }

                    await repository.SaveOrUpdate(modelo);
                    await repositoryDestinatarios.SaveOrUpdate(listaDestinatariosPadrao);
                }
                retorno.IdObjeto = modelo.Id;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = Convert.ToInt32(value.Objeto.Id);
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/NotificacoesApi/ExcluirDestGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirPlanoGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<NotificacoesDestinatarios>(session))
                    {

                        NotificacoesDestinatarios modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private void BindViewToObj(ISession session, ref Notificacoes objBanco, Notificacoes_CadastroModel view)
        {
            objBanco.Id = Convert.ToInt64(view.Id);
            objBanco.Status = view.Status == true ? Status_Default.Ativo : Status_Default.Inativo;
            objBanco.Descricao = view.Descricao;
            objBanco.Notificacao = view.Notificacao;
            objBanco.SecaoConnect = (Model.enumerado.SecaoConnectPerfilEnum)Enum.Parse(typeof(Model.enumerado.SecaoConnectPerfilEnum), Convert.ToString(view.IdSecaoConnect));
            objBanco.TempoDisparoNotificacaoMinutos = view.TempoDisparoNotificacaoMinutos;
        }


        [HttpGet()]
        [Route("api/NotificacoesApi/FindById")]
        public async Task<GetApiRetorno<Notificacoes_CadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Notificacoes_CadastroModel> retorno = new GetApiRetorno<Notificacoes_CadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Notificacoes>(session))
            using (var repositoryDest = new Conexao<NotificacoesDestinatarios>(session))
            using (var repositoryUser = new Conexao<IdentityUser>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

                    List<NotificacoesDestinatarios_ListModel> listaCheckBox = new List<NotificacoesDestinatarios_ListModel>();

                    listaCheckBox = repositoryDest.select(p => p.Notificacoes.Id == Id && p.Usuario_Destinatario == null).Result.Select(p => new NotificacoesDestinatarios_ListModel
                    {
                        Id = p.Id,
                        UsuarioEnum = (TipoUsuarioEnum)Enum.Parse(typeof(TipoUsuarioEnum), Convert.ToString(p.Tipo_Destinatario))
                    }).ToList();

                    foreach (var testeAux in listaCheckBox)
                    {
                        TipoUsuarioEnum foo = testeAux.UsuarioEnum;

                        if (foo == TipoUsuarioEnum.MONITOR_EQUIPE)
                        {
                            retorno.Objeto.MonitorEquipe = true;
                        }
                        if (foo == TipoUsuarioEnum.SUPERVISOR_EQUIPE)
                        {
                            retorno.Objeto.SupervisorEquipe = true;
                        }
                        if (foo == TipoUsuarioEnum.ATENDENTE)
                        {
                            retorno.Objeto.AtendenteResp = true;
                        }
                        if (foo == TipoUsuarioEnum.SUPERVISOR_ATENDENTE)
                        {
                            retorno.Objeto.SupervisorResp = true;
                        }
                        if (foo == TipoUsuarioEnum.SUPERVISOR_COMERCIAL)
                        {
                            retorno.Objeto.SupervisorComercialResp = true;
                        }
                        if (foo == TipoUsuarioEnum.VENDEDOR)
                        {
                            retorno.Objeto.VendedorResp = true;
                        }
                        if (foo == TipoUsuarioEnum.TODOS_VENDEDORES)
                        {
                            retorno.Objeto.TodosVendedores = true;
                        }
                        if (foo == TipoUsuarioEnum.TODOS_SUPERVISOR_COMERCIAL)
                        {
                            retorno.Objeto.TodosSupervisores = true;
                        }
                        if (foo == TipoUsuarioEnum.TODOS_ATENDENTE)
                        {
                            retorno.Objeto.TodosAtendentes = true;
                        }
                        if (foo == TipoUsuarioEnum.TODOS_SUPERVISOR_ATENDENTE)
                        {
                            retorno.Objeto.TodosSupervisoresAtendentes = true;
                        }
                        if (foo == TipoUsuarioEnum.TODOS_ATENDENTE_AGENTE)
                        {
                            retorno.Objeto.AtendentesAgentes = true;
                        }
                    }

                    retorno.Objeto.Destinatarios = repositoryDest.select(p => p.Notificacoes.Id == Id && p.Usuario_Destinatario != null).Result.Select(p =>
                    {

                        var user = repositoryUser.selectFirst(u => u.Id == p.Usuario_Destinatario).Result;
                        return new NotificacoesDestinatarios_ListModel
                        {
                            Id = p.Id,
                            Usuario = p.Usuario_Destinatario,
                            UsuarioNome = user?.Nome ?? user?.UserName
                        };
                    }).ToList();


                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/NotificacoesApi/EnviaNotificacaoImportacao")]
        public async Task<PostApiRetorno<Int64>> EnviaNotificacaoImportacao(Notificacoes_CadastroModel value)
        {
            NotificacaoBusiness notiBuss = new NotificacaoBusiness();
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using ISession session = HibernateUtil.GetSession();
                using (var repositoryNotificacoesEnvio = new Conexao<NotificacoesEnvio>(session))
                using (var repositoryNotificacoes = new Conexao<Notificacoes>(session))
                {
                    NotificacoesEnvio envio = new NotificacoesEnvio
                    {
                        Data_Envio = DateTime.Now,
                        Texto_Notificacao = value.Descricao,
                        Status_Notificacao = Model.enumerado.Status_Notificacao.NaoLido,
                        Usuario_Destino = Convert.ToString(value.Destinatarios.FirstOrDefault().Usuario),
                        //Notificacoes = null,
                        DataHoraCriacao = DateTime.Now,
                        Acao = value.Acao_Executada,
                    };

                    await repositoryNotificacoesEnvio.SaveOrUpdate(envio);
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }
        //[HttpPost]
        //[Route("api/NotificacoesApi/EnviaNotificacoesAtendimento")]
        //public async Task<PostApiRetorno> EnviaNotificacoesAtendimento(string protocolo)
        //{
        //    var retorno = new PostApiRetorno();
        //    NotificacaoPendenteAtualizacaoRequestModel notificacoesNaoLidas = null;
        //    Notificacoes obj = null;
        //    Model.nhibernate.Atendimento atendimento = null;
        //    try
        //    {
        //        using (var session = HibernateUtil.GetSession())
        //        using (var repo = new Conexao<Notificacoes>(session))
        //        using (var repoAtendimento = new Conexao<Model.nhibernate.Atendimento>(session))

        //        using (var api = new NotificacoesApiController())
        //        using (var comm = session.Connection.CreateCommand())
        //        {
        //            var sql = new StringBuilder();
        //            var condicoes = new List<string>();
        //            var parametros = new Dictionary<string, object>();

        //            if (!string.IsNullOrEmpty(protocolo))
        //            {
        //                condicoes.Add("A.CHAVE = @CHAVE");
        //                parametros.Add("@CHAVE", protocolo);
        //            }
        //            condicoes.Add("A.STATUS = 1");

        //            sql.AppendLine("SELECT A.ID,A.TEXTO_NOTIFICACAO, A.STATUS, A.DATA_CRIACAO, A.DATA_ENVIO, A.CHAVE, A.ID_NOTIFICACOES, A.ACAO FROM TB_NOTIFICACOES_ENVIO A")
        //               .AppendLine("WHERE" + string.Join("AND", condicoes))
        //               .AppendLine("ORDER BY A.ID DESC");
        //            using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, session.Connection, null))
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    notificacoesNaoLidas = new NotificacaoPendenteAtualizacaoRequestModel
        //                    {
        //                        Id = Convert.ToInt64(reader["ID"]),
        //                        Id_notificacoes = Convert.ToInt64(reader["ID_NOTIFICACOES"]),
        //                        Chave = Convert.ToString(reader["CHAVE"]),
        //                        Data_envio = Convert.ToDateTime(reader["DATA_ENVIO"]).ToString("dd/MM/yyyy"),
        //                        Acao = Convert.ToString(reader["ACAO"]),
        //                        Status = Convert.ToInt64(reader["STATUS"]),
        //                        Texto_notificacao = Convert.ToString(reader["TEXTO_NOTIFICACAO"])
        //                    };
        //                    obj = await repo.selectFirst(p => p.Id == notificacoesNaoLidas.Id_notificacoes);
        //                }
        //            }

        //            if (notificacoesNaoLidas == null)
        //            {
        //                obj = await repo.selectFirst(p => p.SecaoConnect == Model.enumerado.SecaoConnectPerfilEnum.ATENDIMENTO);
        //            }
        //            atendimento = await repoAtendimento.selectFirst(p => p.Protocolo == protocolo);

        //            if (obj == null && atendimento == null)
        //            {
        //                throw new Exception("Atendimento e Modelo de Notificação não encontrados");
        //            }
        //            await api.EnviaNotificacoesAtendimento(Convert.ToInt32(obj.Id), atendimento);
        //        }
        //        retorno.Ok = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.Ok = false;
        //        retorno.InnerException = ex?.InnerException?.Message;

        //        CONSTANTES.log4Net.Error(ex.Message, ex);
        //    }
        //    return retorno;
        //}

        [AllowAnonymous]
        [HttpGet()]
        [Route("api/NotificacoesApi/GetRegrasNotificacaoOSTecnico")]
        public async Task<GetApiRetorno<RegrasNotificacaoOSTecnicoReponse>> GetRegrasNotificacaoOSTecnico()
        {
            GetApiRetorno<RegrasNotificacaoOSTecnicoReponse> retorno = new GetApiRetorno<RegrasNotificacaoOSTecnicoReponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repositoryNotificacoesEnvio = new Conexao<NotificacoesEnvio>(session))
                using (var repositoryNotificacoes = new Conexao<Notificacoes>(session))
                using (var repositoryConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    var obj = new RegrasNotificacaoOSTecnicoReponse();
                    obj.TempoExecucaoPadrao = (await repositoryConfigGerais.selectFirst())?.Tempo_Execucao_Padrao;

                    var notificacaoAtendimentoOSProximo = await repositoryNotificacoes.selectFirst(p => p.Id == 42);
                    if (notificacaoAtendimentoOSProximo != null && notificacaoAtendimentoOSProximo.Status == Status_Default.Ativo)
                    {
                        obj.RegraAtendimentoOSProximo = true;
                        obj.MinutosAtendimentoOSProximo = notificacaoAtendimentoOSProximo.TempoDisparoNotificacaoMinutos;
                        obj.MsgAtendimentoOSProximo = notificacaoAtendimentoOSProximo.Notificacao;
                    }

                    var notificacaoPrazoEncerramentoOSVencido = await repositoryNotificacoes.selectFirst(p => p.Id == 43);
                    if (notificacaoPrazoEncerramentoOSVencido != null && notificacaoPrazoEncerramentoOSVencido.Status == Status_Default.Ativo)
                    {
                        obj.RegraPrazoEncerramentoOSVencido = true;
                        obj.MinutosPrazoEncerramentoOSVencido = notificacaoPrazoEncerramentoOSVencido.TempoDisparoNotificacaoMinutos;
                        obj.MsgPrazoEncerramentoOSVencido = notificacaoPrazoEncerramentoOSVencido.Notificacao;
                    }

                    var notificacaoPrazoEncerramentoOSAVencer = await repositoryNotificacoes.selectFirst(p => p.Id == 46);
                    if (notificacaoPrazoEncerramentoOSAVencer != null && notificacaoPrazoEncerramentoOSAVencer.Status == Status_Default.Ativo)
                    {
                        obj.RegraPrazoEncerramentoOSAVencer = true;
                        obj.MinutosPrazoEncerramentoOSAVencer = notificacaoPrazoEncerramentoOSAVencer.TempoDisparoNotificacaoMinutos;
                        obj.MsgPrazoEncerramentoOSAVencer = notificacaoPrazoEncerramentoOSAVencer.Notificacao;
                    }

                    var notificacaoSLAProximoVencimento = await repositoryNotificacoes.selectFirst(p => p.Id == 44);
                    if (notificacaoSLAProximoVencimento != null && notificacaoSLAProximoVencimento.Status == Status_Default.Ativo)
                    {
                        obj.RegraSLAProximoVencimento = true;
                        obj.MinutosSLAProximoVencimento = notificacaoSLAProximoVencimento.TempoDisparoNotificacaoMinutos;
                        obj.MsgSLAProximoVencimento = notificacaoSLAProximoVencimento.Notificacao;
                    }



                    retorno.Objeto = obj;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        private Notificacoes_CadastroModel GetViewObj(Notificacoes banco)
        {
            return new Notificacoes_CadastroModel
            {

                Id = Convert.ToString(banco.Id),
                IdCadastro = Convert.ToString(banco.Id),
                Descricao = banco.Descricao,
                Notificacao = banco.Notificacao,
                Status = banco.Status == Status_Default.Ativo,
                IdSecaoConnect = (banco.SecaoConnect),
                TempoDisparoNotificacaoMinutos = banco.TempoDisparoNotificacaoMinutos,
                Acao_Executada = banco.AcaoExecutada
            };
        }

        /// <summary>
        /// Endpoint consumido pelo Aniel Touch para notificação ao Back-Office sobre alertas de SOS ou de ausência de confirmação do técnico.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost()]
        [Route("api/NotificacoesApi/NotificarAlertaSOS")]
        public async Task<GetApiRetorno> NotificarAlertaSOS(NotificacaoAlertaSOSRequest value)
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<AtividadeMongo>(conexaoMongo))
                {
                    var atividadeMongo = await repositoryMongo.Find(p =>
                        p.Equipe == value.equipe &&
                        p.UuidNotificacao == value.uuidNotificacao);

                    if (atividadeMongo != null)
                    {
                        using (ISession session = HibernateUtil.GetSession())
                        using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                        using (var repositoryNotificacoesEnvio = new Conexao<NotificacoesEnvio>(session))
                        using (var repositoryNotificacoes = new Conexao<Notificacoes>(session))
                        using (var repositoryUser = new Conexao<IdentityUser>(session))
                        {
                            var repositoryEquipeAniel = new Repository.T5.Equipe.EquipeAnielRepository(sessionT5);
                            var repositoryUsuarioAniel = new Repository.T5.Usuario.UsuarioAnielRepository(sessionT5);
                            var repositoryFuncionario = new Repository.T5.Funcionario.FuncionarioAnielRepository(sessionT5);
                            var equipeAnielBusiness = new EquipeAnielBusiness(repositoryEquipeAniel, null, null);


                            var equipeMonitor = equipeAnielBusiness.GetByEquipe(value.monitor);

                            //ATENÇÃO: IDS PERMANENTES UTILIZADOS NO MIGRATION TB_NOTIFICACOES
                            Notificacoes modelo = null;

                            switch (atividadeMongo.Resposta)
                            {
                                case Aniel.T5.Model.enumerado.EnumRespostaSOS.Sem_Resposta:
                                    modelo = await repositoryNotificacoes.selectFirst(p => p.Id == 40);
                                    break;
                                case Aniel.T5.Model.enumerado.EnumRespostaSOS.SOS:
                                    modelo = await repositoryNotificacoes.selectFirst(p => p.Id == 41);
                                    break;
                                default:
                                    throw new Sinapse.Core.EAnielException($"Não existe modelo configurado para este tipo de notificação. EnumRespostaSOS = {atividadeMongo.Resposta}");
                            }


                            if (modelo.Status == Status_Default.Ativo)
                            {
                                var obj_equipe = equipeAnielBusiness.GetByEquipe(value.equipe);
                                var usuario = repositoryFuncionario.GetByNumFre(obj_equipe.Num_Fre);
                                String textoNoti = modelo.Notificacao
                                    .Replace("@NOME_TECNICO", usuario?.Nome ?? obj_equipe.Nome)
                                    .Replace("@EQUIPE", value.equipe);


                                var usuarioAniel = repositoryUsuarioAniel.GetAll().Where(p => p.NumFre == equipeMonitor.Num_Fre).FirstOrDefault();
                                var dataHoraEnvio = DateTime.Now;
                                var envio = new NotificacoesEnvio
                                {
                                    Data_Envio = dataHoraEnvio,
                                    Texto_Notificacao = textoNoti,
                                    Status_Notificacao = Model.enumerado.Status_Notificacao.NaoLido,
                                    Usuario_Destino = usuarioAniel.Usuario,
                                    Notificacoes = modelo,
                                    DataHoraCriacao = DateTime.Now,
                                    Acao = (Notificacoes_Acao_Executada)Enum.Parse(typeof(Notificacoes_Acao_Executada), Convert.ToString(modelo.AcaoExecutada)),
                                    Chave = value.uuidNotificacao
                                };
                                await repositoryNotificacoesEnvio.SaveOrUpdate(envio);

                                //atividadeMongo.DataHoraEnvioNotificacao = dataHoraEnvio;
                                //await repositoryMongo.Update(p => p.Equipe == value.equipe && p.UuidNotificacao == value.uuidNotificacao, atividadeMongo);
                            }
                        }
                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/NotificacoesApi/ConfirmarCienciaSOS")]
        public async Task<GetApiRetorno> ConfirmarCienciaSOS(ConfirmarCienciaSOSRequest value)
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<AtividadeMongo>(conexaoMongo))
                {
                    foreach (var item in value.uuidNotificacao)
                    {
                        var atividadeMongo = await repositoryMongo.Find(p => p.UuidNotificacao == item);

                        if (atividadeMongo == null)
                            throw new Sinapse.Core.EAnielException($"Alerta SOS não encontrado no Mongo. UUID da notificação: {value.uuidNotificacao}");

                        if (!atividadeMongo.ConfirmadoCiencia)
                        {
                            atividadeMongo.ConfirmadoCiencia = true;
                            atividadeMongo.DataHoraConfirmacao = DateTime.Now;
                            atividadeMongo.UsuarioConfirmacao = value.equipeMonitor;

                            await repositoryMongo.Update(p => p.UuidNotificacao == item, atividadeMongo);

                            retorno.Mensagem = "Realizada confirmação de ciência sobre o alerta!";
                        }
                        else
                        {
                            retorno.Mensagem = "Já foi realizada a confirmação de ciência deste alerta anteriormente.";
                        }
                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = "Ocorreu um erro ao confirmar ciência sobre o alerta!";
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }


        [HttpPost]
        [Route("api/NotificacoesApi/EnviaNotificacoesTransferenciaChat")]
        public async Task<PostApiRetorno> EnviaNotificacoesTransferenciaChat(NotificacaoTransferenciaChat request)
        {
            var retorno = new PostApiRetorno();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Atendimento>(session))
                {
                    var atendimento = await repository.selectFirst(p => p.Id == request.IdAtendimento);
                    var retornoNotificacao = await EnviaNotificacoesAtendimento(request.IdNotificacao, atendimento);
                    retorno.Ok = retornoNotificacao.Ok;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
    }

}