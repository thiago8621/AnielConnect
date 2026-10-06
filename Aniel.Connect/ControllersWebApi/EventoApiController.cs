using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class EventoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/EventoApi/GetAll")]
        public async Task<DataTableResponse<BotEvento_ListModel>> GetAll(BotEvento_DataTableRequest request)
        {

            List<BotEvento_ListModel> retorno = new List<BotEvento_ListModel>();
            IOrderedEnumerable<BotEvento_ListModel> listaOrdenada = null;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.STATUS, A.EVENTO_INICIAL, A.DISPARO_AUTOMATICO, A.ACAO_EXECUTADA, A.PESQUISA_SATISFACAO, ID_DEPTO_ATENDIMENTO");
                    sql.AppendLine(" FROM TB_BOT_EVENTO A");


                    List<String> filtros = new List<string>();
                    if (!String.IsNullOrEmpty(request.IdBot))
                    {
                        filtros.Add("A.ID_BOT = @ID_BOT");
                        parametros.Add("@ID_BOT", request.IdBot);
                    }

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        filtros.Add("A.STATUS = @STATUS");
                        parametros.Add("@STATUS", request.Status);
                    }

                    if (!String.IsNullOrEmpty(request.Disparo))
                    {
                        filtros.Add("A.DISPARO_AUTOMATICO = @DISPARO_AUTOMATICO");
                        parametros.Add("@DISPARO_AUTOMATICO", request.Disparo);
                    }

                    if (!String.IsNullOrEmpty(request.Inicial))
                    {
                        filtros.Add("A.EVENTO_INICIAL = @EVENTO_INICIAL");
                        parametros.Add("@EVENTO_INICIAL", request.Inicial);
                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        filtros.Add("UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (filtros.Count > 0)
                    {

                        sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new BotEvento_ListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.ATIVO : RsCadastros.INATIVO,
                                Disparo = Convert.ToString(reader["DISPARO_AUTOMATICO"]) == "1" ? RsCadastros.SIM : RsCadastros.NAO,
                                Inicial = Convert.ToString(reader["EVENTO_INICIAL"]) == "1" ? RsCadastros.SIM : RsCadastros.NAO,
                                AcaoExecutada = HtmlEnumExtensions.GetEnumDescription<Tipo_Bot_Evento_Acoes>(Convert.ToString(reader["ACAO_EXECUTADA"])),
                                PesquisaSatisfacao = Convert.ToString(reader["PESQUISA_SATISFACAO"]),
                                SetorAtendimento = Convert.ToString(reader["ID_DEPTO_ATENDIMENTO"]),
                            });
                        }
                    }

                }
            }

            //Ordenacao
            listaOrdenada = retorno.OrderBy(p => p.Id);

            //Page e retornando DataTable
            return new DataTableResponse<BotEvento_ListModel>
            {
                draw = request.draw,
                //recordsTotal = listaOrdenada.Count(),
                recordsFiltered = listaOrdenada.Count(),
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/EventoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<BotEvento_CadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                BotEvento modelo = null;

                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<BotEvento>(session))
                using (var repositoryAnexo = new Conexao<BotEventoAnexo>(session))
                using (var repositoryBot = new Conexao<Bot>(session))
                using (var repositoryResposta = new Conexao<BotEventoResposta>(session))
                {
                    if (value.Objeto.Id != 0)
                        modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                    if (modelo == null)
                    {
                        modelo = new BotEvento
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


                    var listaRespostas = value.Objeto.Respostas.Select(p => new BotEventoResposta
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        BotEvento = modelo,
                        ProximoEvento = repository.selectFirst(y => y.Id == p.IdProximoEvento).Result

                    }).ToList();

                    await repository.SaveOrUpdate(modelo);
                    await repositoryResposta.SaveOrUpdate(listaRespostas);
                    GravarAnexo(value.Objeto, modelo, repositoryAnexo);
                }
                retorno.IdObjeto = modelo.Id;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        public String GravarAnexo(BotEvento_CadastroModel model, BotEvento evento, Conexao<BotEventoAnexo> repository)
        {
            string retornoURL = null;
            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\BotEvento";

            //IMAGEM
            for (var i = 0; i < model.FilesAnexoImagem.Count; i++)
            {
                if (!String.IsNullOrEmpty(model.FilesAnexoImagem[i]?.FileName))
                {

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var anexo = new BotEventoAnexo();
                    var extensao = Path.GetExtension(model.FilesAnexoImagem[i].FileName);
                    var nome = Path.GetFileNameWithoutExtension(model.FilesAnexoImagem[i].FileName);


                    anexo.Nome = nome;
                    anexo.Extensao = extensao;
                    anexo.Tipo = Model.enumerado.Tipo_Bot_Evento_Anexo.Imagem;
                    anexo.BotEvento = evento;
                    repository.Save(anexo).Wait();

                    byte[] allBytes = new byte[model.FilesAnexoImagem[i].InputStream.Length];
                    model.FilesAnexoImagem[i].InputStream.Read(allBytes, 0, (int)model.FilesAnexoImagem[i].InputStream.Length);
                    String _arquivo = $@"{_pasta}\{anexo.Id}{extensao}";
                    System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);
                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/BotEvento/{anexo.Id}{extensao}";
                    anexo.Url = retornoURL;
                    repository.SaveOrUpdate(anexo).Wait();
                }
            }
            //VÍDEO
            for (var i = 0; i < model.FilesAnexoVideo.Count; i++)
            {
                if (!String.IsNullOrEmpty(model.FilesAnexoVideo[i]?.FileName))
                {

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var anexo = new BotEventoAnexo();
                    var extensao = Path.GetExtension(model.FilesAnexoVideo[i].FileName);
                    var nome = Path.GetFileNameWithoutExtension(model.FilesAnexoVideo[i].FileName);


                    anexo.Nome = nome;
                    anexo.Extensao = extensao;
                    anexo.Tipo = Model.enumerado.Tipo_Bot_Evento_Anexo.Video;
                    anexo.BotEvento = evento;
                    repository.Save(anexo).Wait();

                    byte[] allBytes = new byte[model.FilesAnexoVideo[i].InputStream.Length];
                    model.FilesAnexoVideo[i].InputStream.Read(allBytes, 0, (int)model.FilesAnexoVideo[i].InputStream.Length);
                    String _arquivo = $@"{_pasta}\{anexo.Id}{extensao}";
                    System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);
                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/BotEvento/{anexo.Id}{extensao}";
                    anexo.Url = retornoURL;
                    repository.SaveOrUpdate(anexo).Wait();
                }
            }
            //ÁUDIO
            for (var i = 0; i < model.FilesAnexoAudio.Count; i++)
            {
                if (!String.IsNullOrEmpty(model.FilesAnexoAudio[i]?.FileName))
                {

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var anexo = new BotEventoAnexo();
                    var extensao = Path.GetExtension(model.FilesAnexoAudio[i].FileName);
                    var nome = Path.GetFileNameWithoutExtension(model.FilesAnexoAudio[i].FileName);


                    anexo.Nome = nome;
                    anexo.Extensao = extensao;
                    anexo.Tipo = Model.enumerado.Tipo_Bot_Evento_Anexo.Audio;
                    anexo.BotEvento = evento;
                    repository.Save(anexo).Wait();

                    byte[] allBytes = new byte[model.FilesAnexoAudio[i].InputStream.Length];
                    model.FilesAnexoAudio[i].InputStream.Read(allBytes, 0, (int)model.FilesAnexoAudio[i].InputStream.Length);
                    String _arquivo = $@"{_pasta}\{anexo.Id}{extensao}";
                    System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);
                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/BotEvento/{anexo.Id}{extensao}";
                    anexo.Url = retornoURL;
                    repository.SaveOrUpdate(anexo).Wait();
                }
            }
            //ARQUIVO
            for (var i = 0; i < model.FilesAnexoArquivo.Count; i++)
            {
                if (!String.IsNullOrEmpty(model.FilesAnexoArquivo[i]?.FileName))
                {

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var anexo = new BotEventoAnexo();
                    var extensao = Path.GetExtension(model.FilesAnexoArquivo[i].FileName);
                    var nome = Path.GetFileNameWithoutExtension(model.FilesAnexoArquivo[i].FileName);


                    anexo.Nome = nome;
                    anexo.Extensao = extensao;
                    anexo.Tipo = Model.enumerado.Tipo_Bot_Evento_Anexo.Arquivo;
                    anexo.BotEvento = evento;
                    repository.Save(anexo).Wait();

                    byte[] allBytes = new byte[model.FilesAnexoArquivo[i].InputStream.Length];
                    model.FilesAnexoArquivo[i].InputStream.Read(allBytes, 0, (int)model.FilesAnexoArquivo[i].InputStream.Length);
                    String _arquivo = $@"{_pasta}\{anexo.Id}{extensao}";
                    System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);
                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/BotEvento/{anexo.Id}{extensao}";
                    anexo.Url = retornoURL;
                    repository.SaveOrUpdate(anexo).Wait();
                }
            }

            //LINK
            if (!String.IsNullOrEmpty(model.FilesAnexoUrl))
            {
                var anexo = new BotEventoAnexo();
                var nome = model.FilesAnexoUrl;

                anexo.Nome = nome;
                anexo.Tipo = Model.enumerado.Tipo_Bot_Evento_Anexo.Url;
                anexo.BotEvento = evento;
                anexo.Url = nome;
                repository.SaveOrUpdate(anexo).Wait();
            }


            return retornoURL;
        }

        [HttpGet()]
        [Route("api/EventoApi/FindById")]
        public async Task<GetApiRetorno<BotEvento_CadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<BotEvento_CadastroModel> retorno = new GetApiRetorno<BotEvento_CadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<BotEvento>(session))
            using (var repositoryAnexo = new Conexao<BotEventoAnexo>(session))
            using (var repositoryResposta = new Conexao<BotEventoResposta>(session))

            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

                    string variaveis = "";
                    foreach (var item in Enum.GetNames(typeof(EventoVariaveis)))
                    {
                        variaveis += " @" + item;
                    }
                    retorno.Objeto.VariaveisDisponiveis = variaveis;

                    retorno.Objeto.Anexos = repositoryAnexo.select(p => p.BotEvento.Id == Id).Result.Select(p => new BotEventoAnexo_ListModel
                    {
                        Id = p.Id,
                        Extensao = p.Extensao,
                        Nome = p.Nome,
                        Tipo = HtmlEnumExtensions.GetEnumDescription<Tipo_Bot_Evento_Anexo>(p.Tipo),
                        Url = p.Url
                    }).ToList();


                    retorno.Objeto.Respostas = repositoryResposta.select(p => p.BotEvento.Id == Id).Result.Select(p => new BotEventoResposta_ListModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        IdProximoEvento = p.ProximoEvento?.Id,

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
        [Route("api/EventoApi/Excluir")]
        public async Task<PostApiRetorno<Int64>> Excluir(AlterarStatusViewModel value)
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_BOT_EVENTO WHERE ID IN {_ids}");

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
        [Route("api/EventoApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                if (value.status == 1)
                {
                    var listagemInviavel = (await AlteracaoRegistroMesmaDescricao(value)).ToList();
                    if (listagemInviavel.Count > 0)
                    {
                        throw new Exception(String.Format("Alguns registros não podem estar ativos ao mesmo tempo: {0}", String.Join(",", listagemInviavel)));
                    }
                }

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_BOT_EVENTO SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }
        private async Task<IEnumerable<(long id, string descricao)>> AlteracaoRegistroMesmaDescricao(AlterarStatusViewModel value)
        {
            var idsInutilizaveis = new List<(long Id, string descricao)>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            using (ISession session = HibernateUtil.GetSession())
            {
                foreach (var _id in value.ids)
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.DESCRICAO FROM TB_BOT_EVENTO A")
                           .AppendLine("WHERE A.DESCRICAO IN (SELECT DESCRICAO FROM TB_BOT_EVENTO GROUP BY DESCRICAO HAVING COUNT(DESCRICAO) > 1)");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                long Id = Convert.ToInt64(reader["ID"]);
                                idsInutilizaveis.Add((Id, Convert.ToString(reader["DESCRICAO"])));
                            }
                        }
                    }
                }

                idsInutilizaveis = idsInutilizaveis.Where(p => value.ids.Contains(p.Id)).ToList();
            }

            return idsInutilizaveis;
        }

        private void BindViewToObj(ISession session, ref BotEvento objBanco, BotEvento_CadastroModel view)
        {
            using (var repoBotEvento = new Conexao<BotEvento>(session))
            using (var repoPesquisa = new Conexao<Pesquisa_Satisfacao>(session))
            using (var repoDepto = new Conexao<Departamento>(session))
            using (var repoBot = new Conexao<Bot>(session))
            {
                var descBot = repoBotEvento.selectFirst(p => p.Id != view.Id && p.Descricao == view.Descricao && p.Status == Status_Default.Ativo).Result;

                if (descBot != null)
                {
                    throw new Exception("Já existe um registro ativo com essa descrição");
                }

                objBanco.Id = view.Id;
                objBanco.Descricao = view.Descricao;
                objBanco.Status = view.Status == true ? Status_Default.Ativo : Status_Default.Inativo;
                objBanco.DisparoAutomatico = view.Disparo;
                objBanco.EventoInicial = view.Inicial;
                objBanco.TextoEnviado = view.TextoEnviado;
                objBanco.UtilizaAcao = view.AtivarAcao;
                objBanco.AcaoExecutada = view.AcaoExecutada;
                objBanco.CondicaoEvento = view.CondicaoEvento;
                objBanco.EventoArmazenamento = view.EventoArmazenamento;
                objBanco.VariaveisDisponiveis = view.VariaveisDisponiveis;


                if (view.Id == 0)
                {
                    objBanco.Bot = repoBot.selectFirst(p => p.Id == view.IdBot).Result;
                    view.NomeBot = objBanco.Bot.Nome;
                }
                if (objBanco.EventoInicial && objBanco.Status == Status_Default.Ativo)
                {
                    var validacao = repoBotEvento.selectFirst(p => p.EventoInicial == true && view.Id != p.Id && view.IdBot == p.Bot.Id && p.Status == Status_Default.Ativo).Result;
                    if (validacao != null)
                    {
                        throw new Exception("Já existe um registro ativo com Status Evento Inicial definido como 'Sim'");
                    }
                }
                objBanco.AutoProximoEvento = repoBotEvento.selectFirst(y => y.Id == view.AutoProximoEvento).Result;
                objBanco.EventoVerdadeiro = repoBotEvento.selectFirst(z => z.Id == view.EventoVerdadeiro).Result;
                objBanco.EventoFalso = repoBotEvento.selectFirst(c => c.Id == view.EventoFalso).Result;
                objBanco.PesquisaSatisfacao = repoPesquisa.selectFirst(x => x.Id == view.PesquisaSatisfacao).Result;
                objBanco.SetorAtendimento = repoDepto.selectFirst(g => g.Id == view.SetorAtendimento).Result;
            }
        }



        [HttpPost()]
        [Route("api/EventoApi/GetRespostas")]
        public async Task<DataTableResponse<BotEventoResposta_ListModel>> GetAll(BotEventoResposta_DataTableRequest request)
        {

            List<BotEventoResposta_ListModel> retorno = new List<BotEventoResposta_ListModel>();
            IOrderedEnumerable<BotEventoResposta_ListModel> listaOrdenada = null;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.ID_BOT_EVENTO");
                    sql.AppendLine(" FROM TB_BOT_EVENTO_RESP A");


                    //List<String> filtros = new List<string>();
                    //if (!String.IsNullOrEmpty(request.IdProximoEvento))
                    //{
                    //    filtros.Add("A.ID_BOT_EVENTO = @ID_BOT");
                    //    parametros.Add("@ID_BOT", request.IdEvento);
                    //}

                    //if (!String.IsNullOrEmpty(request.search.value))
                    //{
                    //    filtros.Add("UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                    //    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    //}

                    //if (filtros.Count > 0)
                    //{

                    //    sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    //}

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new BotEventoResposta_ListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                IdProximoEvento = Convert.ToInt64(reader["ID_BOT_EVENTO"]),
                            });
                        }
                    }

                }
            }

            //Ordenacao
            listaOrdenada = retorno.OrderBy(p => p.Id);

            //Page e retornando DataTable
            return new DataTableResponse<BotEventoResposta_ListModel>
            {
                draw = request.draw,
                //recordsTotal = listaOrdenada.Count(),
                recordsFiltered = listaOrdenada.Count(),
                data = listaOrdenada,
                error = ""
            };

        }


        [HttpPost()]
        [Route("api/EventoApi/GetAnexos")]
        public async Task<DataTableResponse<BotEventoAnexo_ListModel>> GetAll(BotEventoAnexo_DataTableRequest request)
        {

            List<BotEventoAnexo_ListModel> retorno = new List<BotEventoAnexo_ListModel>();
            IOrderedEnumerable<BotEventoAnexo_ListModel> listaOrdenada = null;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.NOME, A.TIPO, A.EXTENSAO, A.URL");
                    sql.AppendLine(" FROM TB_BOT_EVENTO_ANEXO A");


                    List<String> filtros = new List<string>();
                    if (!String.IsNullOrEmpty(request.IdEvento))
                    {
                        filtros.Add("A.ID_BOT_EVENTO = @ID_BOT");
                        parametros.Add("@ID_BOT", request.IdEvento);
                    }

                    //teste
                    if (!String.IsNullOrEmpty(request.IdAnexo))
                    {
                        filtros.Add("A.ID = @ID_ANEXO");
                        parametros.Add("@ID_ANEXO", request.IdAnexo);
                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        filtros.Add("UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (filtros.Count > 0)
                    {

                        sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new BotEventoAnexo_ListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME"]),
                                Tipo = HtmlEnumExtensions.GetEnumDescription<Tipo_Bot_Evento_Anexo>(Convert.ToString(reader["TIPO"])),
                                Extensao = Convert.ToString(reader["EXTENSAO"]),
                                Url = Convert.ToString(reader["URL"]),
                            });
                        }
                    }

                }
            }

            //Ordenacao
            listaOrdenada = retorno.OrderBy(p => p.Id);

            //Page e retornando DataTable
            return new DataTableResponse<BotEventoAnexo_ListModel>
            {
                draw = request.draw,
                //recordsTotal = listaOrdenada.Count(),
                recordsFiltered = listaOrdenada.Count(),
                data = listaOrdenada,
                error = ""
            };

        }

        [Route("api/EventoApi/ExcluirResposta")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirResposta(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<BotEventoResposta>(session))
                    {

                        BotEventoResposta modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();
                            await repository.Remove(modelo);

                            retorno.IdObjeto = Convert.ToInt64(modelo.Id);
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

        private BotEvento_CadastroModel GetViewObj(BotEvento obj)
        {
            return new BotEvento_CadastroModel
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                Disparo = obj.DisparoAutomatico,
                Status = obj.Status == Status_Default.Ativo,
                Inicial = obj.EventoInicial,
                TextoEnviado = obj.TextoEnviado,
                AtivarAcao = obj.UtilizaAcao,
                IdBot = obj.Bot?.Id,
                NomeBot = obj.Bot?.Nome,
                AutoProximoEvento = obj.AutoProximoEvento?.Id,
                EventoFalso = obj.EventoFalso?.Id,
                EventoVerdadeiro = obj.EventoVerdadeiro?.Id,
                AcaoExecutada = obj.AcaoExecutada,
                CondicaoEvento = obj.CondicaoEvento,
                PesquisaSatisfacao = obj.PesquisaSatisfacao?.Id,
                EventoArmazenamento = obj.EventoArmazenamento,
                VariaveisDisponiveis = obj.VariaveisDisponiveis,
                SetorAtendimento = obj.SetorAtendimento?.Id
            };
        }


        [HttpPost()]
        [Route("api/EventoApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPaiConfig<Int64?, Int64?, Dictionary<string, object>> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_BOT_EVENTO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_BOT = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (request.config != null && request.config.ContainsKey("IDIGNORAR"))
                    {
                        condicoes.Add($"A.ID <> {request.config["IDIGNORAR"]}");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }
        [HttpGet()]
        [Route("api/EventoApi/ExcluirAnexo")]
        public async Task<DeleteApiRetorno<long>> ExcluirAnexo(long value)
        {
            var retorno = new DeleteApiRetorno<long>();
            var parametros = new Dictionary<string, object>();
            var condicoes = new List<String>();

            using (ISession session = HibernateUtil.GetSession())
            using (var trans = session.Connection.BeginTransaction())
            {
                try
                {
                    string path = $"{CONSTANTES.CaminhoFisico}\\Arquivos\\BotEvento";
                    
                    var sqlFrom = new StringBuilder(" FROM TB_BOT_EVENTO_ANEXO ");
                    var sqlWhere = new StringBuilder();

                    if (value != 0)
                    {
                        condicoes.Add("ID = @ID");
                        parametros.Add("@ID", value);
                    }
                    if (condicoes.Count > 0)
                    {
                        sqlWhere.AppendLine($"WHERE {string.Join(" AND ", condicoes)}");
                    }
                    using (var comm = session.Connection.CreateCommand()) 
                    {
                        var sql = new StringBuilder("SELECT NOME, EXTENSAO")
                                        .AppendLine(sqlFrom.ToString())
                                        .AppendLine(sqlWhere.ToString());
                        using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, session.Connection, trans)) 
                        {
                            if (await reader.ReadAsync()) 
                            {
                                path += $"\\{Convert.ToString(reader["NOME"])}.{Convert.ToString(reader["EXTENSAO"])}";                                
                            }
                        }                        
                    }
                    using (var comm = session.Connection.CreateCommand())
                    {
                        var sql = new StringBuilder("DELETE")
                                        .AppendLine(sqlFrom.ToString())
                                        .AppendLine(sqlWhere.ToString());


                        await CommandHelper.ExecuteNonQuery(comm, sql, parametros, session.Connection, trans);
                    }

                    File.Delete(path);
                    retorno.IdObjeto = value;
                    retorno.Ok = true;
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    retorno.IdObjeto = 0;
                    retorno.Ok = false;
                    retorno.Mensagem = ex.InnerException != null ? $"{ex.Message} - {ex?.InnerException?.Message}" : ex.Message;
                    retorno.InnerException = ex?.InnerException?.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    trans.Rollback();
                }
            }
            return retorno;
        }
    }
}