using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using DocumentFormat.OpenXml.Drawing;
using NHibernate;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.MetaBusiness.Integracoes;
using Sinapse.Integracao.WhatsAppBusiness.Repositorio;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Path = System.IO.Path;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Canal_ComunicacaoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Canal_ComunicacaoApi/GetAll")]
        public async Task<DataTableResponse<Canal_ComunicacaoListModel>> GetAll(Canal_Comunicacao_DataTableRequest request)
        {

            List<Canal_ComunicacaoListModel> retorno = new List<Canal_ComunicacaoListModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_CANAL_COMUNICACAO ");

                    using var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);
                    if (await reader.ReadAsync())
                    {
                        recordsTotal = Convert.ToInt32(reader["CO"]);
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.IMAGEM, A.NOME, A.DESCRICAO,A.EMAIL,A.SITE, A.NUMERO, A.ID_BOT, B.NOME AS NOME_BOT, A.NUMERO, A.TIPO_CANAL,A.STATUS, ");
                    sql.AppendLine(" A.TELEGRAM_TOKEN,A.MESSENGER_TOKEN,INSTAGRAM_TOKEN,WHATSAPP_BUSINESS_ACCOUNT,WHATSAPP_BUSINESS_TOKEN,ID_NUM_WHATSAPP_BUSINESS,ID_MOD_PADRAO_WHATSAPP_BUS,SKYPE_TOKEN ");

                    sql.AppendLine(" FROM TB_CANAL_COMUNICACAO A");

                    sql.AppendLine("LEFT JOIN TB_BOT B ");
                    sql.AppendLine("ON(A.ID_BOT = B.ID)");

                    //Filtrando
                    List<String> filtros = new List<string>();
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        filtros.Add("UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.tipoCanal))
                    {
                        filtros.Add(" A.TIPO_CANAL = @TIPO_CANAL");
                        parametros.Add("@TIPO_CANAL", request.tipoCanal);
                    }


                    if (filtros.Count > 0)
                    {
                        sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }


                    ////Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;
                        string sortDirection = request.order[0].dir;
                        switch (coluna)
                        {
                            case "nome":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.NOME ");

                                    else
                                        sql.AppendLine(" ORDER BY A.NOME DESC ");
                                    break;
                                }
                        }
                    }


                    //Busca com paginacao
                    using var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start);
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new Canal_ComunicacaoListModel()
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            IdBot = reader["ID_BOT"] != DBNull.Value ? Convert.ToInt64(reader["ID_BOT"]) : (long?)null,
                            Imagem = Convert.ToString(reader["IMAGEM"]),
                            Nome = Convert.ToString(reader["NOME"]),
                            Descricao = Convert.ToString(reader["DESCRICAO"]),
                            Numero = Convert.ToString(reader["NUMERO"]),
                            StatusAPI = "CONECTADO",
                            StatusNumero = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),

                            BotVinculado = Convert.ToString(reader["NOME_BOT"]),
                            TipoCanalComunicacao = HtmlEnumExtensions.GetEnumDescription<Tipo_Canal_Comunicacao>(Convert.ToString(reader["TIPO_CANAL"])),
                            IdTipoCanalComunicacao = Convert.ToInt64(reader["TIPO_CANAL"]),
                            Email = Convert.ToString(reader["EMAIL"]),
                            Site = Convert.ToString(reader["SITE"]),
                            TokenWhatsAppBusiness = Convert.ToString(reader["WHATSAPP_BUSINESS_TOKEN"]),
                            IdNumeroWhatsAppBusiness = Convert.ToString(reader["ID_NUM_WHATSAPP_BUSINESS"]),
                            WhatsAppBusinessAccount = Convert.ToString(reader["WHATSAPP_BUSINESS_ACCOUNT"]),
                            IdModeloPadraoWhatsApp = Convert.ToString(reader["ID_MOD_PADRAO_WHATSAPP_BUS"]),
                            TokenTelegram = Convert.ToString(reader["TELEGRAM_TOKEN"]),
                            TokenMessenger = Convert.ToString(reader["MESSENGER_TOKEN"]),
                            TokenInstagram = Convert.ToString(reader["INSTAGRAM_TOKEN"]),
                            TokenSkype = Convert.ToString(reader["SKYPE_TOKEN"]),

                        });
                    }

                }
            }
            //Page e retornando DataTable
            return new DataTableResponse<Canal_ComunicacaoListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }
        [HttpPost()]
        [Route("api/Canal_ComunicacaoApi/GetAllFindById")]
        public async Task<DataTableResponse<Canal_ComunicacaoListModel>> GetAllFindById(Canal_Comunicacao_DataTableRequest request)
        {

            List<Canal_ComunicacaoListModel> retorno = new List<Canal_ComunicacaoListModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_CANAL_COMUNICACAO ");

                    using var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);
                    if (await reader.ReadAsync())
                    {
                        recordsTotal = Convert.ToInt32(reader["CO"]);
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.IMAGEM, A.NOME, A.DESCRICAO, A.EMAIL,A.SITE,A.NUMERO, A.ID_BOT, B.NOME AS NOME_BOT, A.NUMERO, A.TIPO_CANAL,A.STATUS,A.TELEGRAM_TOKEN,A.MESSENGER_TOKEN,INSTAGRAM_TOKEN, ");

                    sql.AppendLine(" WHATSAPP_BUSINESS_ACCOUNT,WHATSAPP_BUSINESS_TOKEN,ID_NUM_WHATSAPP_BUSINESS,ID_MOD_PADRAO_WHATSAPP_BUS,SKYPE_TOKEN ");

                    sql.AppendLine(" FROM TB_CANAL_COMUNICACAO A");

                    sql.AppendLine("LEFT JOIN TB_BOT B ");
                    sql.AppendLine("ON(A.ID_BOT = B.ID)");
                    sql.AppendLine("WHERE A.ID = @ID ");

                    //Filtrando
                    List<String> filtros = new List<string>();

                    filtros.Add(" A.ID = @ID");
                    parametros.Add("@ID", request.Id);



                    ////Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;
                        string sortDirection = request.order[0].dir;
                        switch (coluna)
                        {
                            case "nome":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.NOME ");

                                    else
                                        sql.AppendLine(" ORDER BY A.NOME DESC ");
                                    break;
                                }
                        }
                    }


                    //Busca com paginacao
                    using var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start);
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new Canal_ComunicacaoListModel()
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Imagem = Convert.ToString(reader["IMAGEM"]),
                            Nome = Convert.ToString(reader["NOME"]),
                            Descricao = Convert.ToString(reader["DESCRICAO"]),
                            Numero = Convert.ToString(reader["NUMERO"]),
                            Email = Convert.ToString(reader["EMAIL"]),
                            Site = Convert.ToString(reader["SITE"]),
                            TokenWhatsAppBusiness = Convert.ToString(reader["WHATSAPP_BUSINESS_TOKEN"]),
                            IdNumeroWhatsAppBusiness = Convert.ToString(reader["ID_NUM_WHATSAPP_BUSINESS"]),
                            WhatsAppBusinessAccount = Convert.ToString(reader["WHATSAPP_BUSINESS_ACCOUNT"]),
                            IdModeloPadraoWhatsApp = Convert.ToString(reader["ID_MOD_PADRAO_WHATSAPP_BUS"]),
                            TokenTelegram = Convert.ToString(reader["TELEGRAM_TOKEN"]),
                            TokenMessenger = Convert.ToString(reader["MESSENGER_TOKEN"]),
                            TokenInstagram = Convert.ToString(reader["INSTAGRAM_TOKEN"]),
                            TokenSkype = Convert.ToString(reader["SKYPE_TOKEN"]),
                            StatusAPI = "CONECTADO",
                            StatusNumero = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            BotVinculado = Convert.ToString(reader["NOME_BOT"]),
                            TipoCanalComunicacao = HtmlEnumExtensions.GetEnumDescription<Tipo_Canal_Comunicacao>(Convert.ToString(reader["TIPO_CANAL"])),
                        });
                    }

                }
            }
            //Page e retornando DataTable
            return new DataTableResponse<Canal_ComunicacaoListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }


        [HttpGet()]
        [Route("api/Canal_ComunicacaoApi/FindById")]
        public async Task<GetApiRetorno<Canal_Comunicacao_CadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Canal_Comunicacao_CadastroModel> retorno = new GetApiRetorno<Canal_Comunicacao_CadastroModel>();


            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Canal_Comunicacao>(session))
            {
                string TokenWhatsAppBusiness = null;
                try
                {
                    var modelo = await repository.selectFirst(c => c.Id == Id);
                    TokenWhatsAppBusiness = modelo?.TokenWhatsAppBusiness;

                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

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
        [Route("api/Canal_ComunicacaoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Canal_Comunicacao_CadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {

                Canal_Comunicacao modelo = null;

                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<Canal_Comunicacao>(session))
                {
                    if (value.Objeto.Id != 0)
                        modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                    if (modelo == null)
                    {
                        modelo = new Canal_Comunicacao
                        {
                            UsuarioCriacao = value.Usuario,
                            Status = Status_Default.Ativo
                        };
                    }
                    else
                    {
                        modelo.UsuarioAlteracao = value.Usuario;
                        modelo.DataHoraAlteracao = DateTime.Now;
                        modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                    }

                    if (value.Objeto.IdBot != null)
                    {
                        using var botApi = new Customizacao_BotApiController();
                        var botModel = await botApi.FindById(value.Objeto.IdBot.Value);
                        var bot = new Bot()
                        {
                            Id = botModel.Objeto.Id
                        };
                        value.Objeto.Bot = bot;
                    }


                    BindViewToObj(session, ref modelo, value.Objeto);

                    //envia a imagem
                    if (modelo.Imagem == null)
                    {
                        modelo.Imagem = $@"{CONSTANTES.URLPortal}/Arquivos/Bot/Default.jpg";
                    }

                    await repository.SaveOrUpdate(modelo);

                }
                retorno.IdObjeto = modelo.Id;
                retorno.Ok = true;

                //Caso tenha preenchido o modelo, salva o modelo
                if (retorno.Ok)
                {
                    var modeloMensagemRequest = new ModeloMensagemRequest()
                    {
                        idCanalComunicacao = Convert.ToString(modelo.Id),
                        whatsAppBusinessAccount = modelo.WhatsAppBusinessAccount,
                        mensagem = value.Objeto?.ModeloPadraoWhatsApp
                    };

                    if (modelo.ModeloPadrao != null && modelo.IdModeloPadrao == null)
                    {
                        await VincularModeloMensagem(modeloMensagemRequest);
                    }
                    else if (modelo.ModeloPadrao != value.Objeto.ModeloPadraoWhatsApp && modelo.IdModeloPadrao != null)
                    {
                        await AtualizarModeloMensagem(modeloMensagemRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        

        public string GravarImagem(Canal_Comunicacao_CadastroModel model)
        {
            string retornoURL = null;
            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\CanalComunicacao";
            for (var i = 0; i < model.Files.Count; i++)
            {
                if (!String.IsNullOrEmpty(model.Files[i].FileName))
                {

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var extensao = Path.GetExtension(model.Files[i].FileName);
                    byte[] allBytes = new byte[model.Files[i].InputStream.Length];
                    model.Files[i].InputStream.Read(allBytes, 0, (int)model.Files[i].InputStream.Length);
                    String _arquivo = $@"{_pasta}\{model.Id}{extensao}";
                    System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);
                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/Bot/{model.Id}{extensao}";
                }
            }
            return retornoURL;
        }

        [HttpPost()]
        [Route("api/Canal_ComunicacaoApi/AdicionarArquivo")]
        public async Task<GetApiRetorno> AdicionarArquivo(long Id)
        {
            try
            {
                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<Canal_Comunicacao>(session))
                {
                    var modelo = (await repository.select(p => p.Id == Id)).FirstOrDefault();
                    var retorno = new GetApiRetorno();
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    var provider = new MultipartMemoryStreamProvider();
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\CanalComunicacao";

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    await Request.Content.ReadAsMultipartAsync(provider);

                    foreach (var file in provider.Contents.Take(1))
                    {
                        string filename = StringService.RemoverAcentos(file.Headers.ContentDisposition.Parameters.Skip(1).ToList().FirstOrDefault().Value.Replace("\"", ""));
                        string ext = Path.GetExtension(filename).TrimStart('.');
                        var parameters = file.Headers.ContentDisposition.Parameters;
                        var buffer = await file.ReadAsByteArrayAsync();
                        File.WriteAllBytes($"{_pasta}/{Id}.{ext}", buffer);
                        modelo.Imagem = $"{CONSTANTES.URLPortal}/Arquivos/CanalComunicacao/{Id}.{ext}";
                        await repository.Update(modelo);
                    }

                    retorno.Ok = true;
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                return new GetApiRetorno<List<Mensagem_ChatModels>>
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    Objeto = new List<Mensagem_ChatModels>(),
                    InnerException = ex?.InnerException?.Message
                };
            }

        }



        private void BindViewToObj(ISession session, ref Canal_Comunicacao objBanco, Canal_Comunicacao_CadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Nome = StringService.Truncate(view.Nome, 100);
            objBanco.Descricao = StringService.Truncate(view.Descricao, 400);
            objBanco.Email = StringService.Truncate(view.Email, 100);
            objBanco.Site = StringService.Truncate(view.Site, 100);
            objBanco.Numero = StringService.Truncate(view.Numero, 20);
            objBanco.TwillioAccountId = StringService.Truncate(view.TwillioAccountId, 100);
            objBanco.TwilioAuthToken = StringService.Truncate(view.TwilioAuthToken, 100);
            objBanco.TwilioServiceSID = StringService.Truncate(view.TwilioServiceSID, 100);
            objBanco.TokenTelegram = StringService.Truncate(view.TokenTelegram, 250);
            objBanco.TokenMessenger = StringService.Truncate(view.TokenMessenger, 250);
            objBanco.TokenInstagram = StringService.Truncate(view.TokenInstagram, 250);
            objBanco.Tipo_Canal_Comunicacao = view.TipoCanalComunicacao;
            objBanco.WhatsAppBusinessAccount = StringService.Truncate(view.WhatsAppBusinessAccount, 250);
            objBanco.IdNumeroWhatsAppBusiness = StringService.Truncate(view.IdNumeroWhatsAppBusiness, 250);
            objBanco.TokenWhatsAppBusiness = StringService.Truncate(view.TokenWhatsAppBusiness, 250);
            objBanco.StatusModeloPadrao = view.StatusModeloPadraoWhatsApp;
            objBanco.IdModeloPadrao = view.IdModeloPadraoWhatsApp;
            objBanco.ModeloPadrao = view.ModeloPadraoWhatsApp;
            objBanco.TokenSkype = StringService.Truncate(view.TokenSkype, 250);

            objBanco.Bot = view.Bot;



            var arquivo = view.Files?.FirstOrDefault(p => p != null);
            if (arquivo != null && arquivo.ContentLength > 0)
            {
                objBanco.Imagem = GravarImagem(view);
            }
            else if (string.IsNullOrEmpty(objBanco.Imagem) && !string.IsNullOrEmpty(view.Imagem))
            {
                objBanco.Imagem = view.Imagem;
            }
        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/Canal_ComunicacaoApi/VerificaCanalExistente")]
        public async Task<Int32> VerificaCanalExistente(Canal_Comunicacao_CadastroModel canal)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();
            Int32 recordsTotal = 0;

            using ISession session = HibernateUtil.GetSession();
            using (var repository = new Conexao<Canal_Comunicacao>(session))
            {
                var numero = await repository.selectFirst(p => p.Numero == canal.Numero && p.Tipo_Canal_Comunicacao == canal.TipoCanalComunicacao);

                if (numero.Id > 0)
                {
                    recordsTotal = 1;
                }
            }

            return recordsTotal;
        }

        private Canal_Comunicacao_CadastroModel GetViewObj(Canal_Comunicacao obj)
        {
            return new Canal_Comunicacao_CadastroModel
            {
                Id = obj.Id,
                Imagem = obj.Imagem,
                Nome = obj.Nome,
                Descricao = obj.Descricao,
                Email = obj.Email,
                Site = obj.Site,
                IdBot = obj.Bot?.Id,
                Numero = obj.Numero,
                TwilioAuthToken = obj.TwilioAuthToken,
                TwillioAccountId = obj.TwillioAccountId,
                TwilioServiceSID = obj.TwilioServiceSID,
                TokenTelegram = obj.TokenTelegram,
                TokenMessenger = obj.TokenMessenger,
                TipoCanalComunicacao = obj.Tipo_Canal_Comunicacao,
                WhatsAppBusinessAccount = obj.WhatsAppBusinessAccount,
                IdModeloPadraoWhatsApp = obj.IdModeloPadrao,
                IdNumeroWhatsAppBusiness = obj.IdNumeroWhatsAppBusiness,
                ModeloPadraoWhatsApp = obj.ModeloPadrao,
                StatusModeloPadraoWhatsApp = obj.StatusModeloPadrao,
                TokenWhatsAppBusiness = obj.TokenWhatsAppBusiness
            };
        }




        [HttpPost()]
        [Route("api/Canal_ComunicacaoApi/BuscaStatusModeloMensagem")]
        public async Task<PostApiRetorno<string>> BuscaStatusModeloMensagem(string idCanalComunicacao, string nome)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
                {
                    var canalComunicacao = await repoCanalComunicacao.selectFirst(p => p.Id == Convert.ToInt64(idCanalComunicacao));

                    if (canalComunicacao != null)
                    {
                        var retornoIntegracao = await new IntegracaoWhatsAppBusinessRepository().BuscaStatusModeloMensagem(new MetaWhatsAppBusiness(canalComunicacao.TokenWhatsAppBusiness), canalComunicacao.WhatsAppBusinessAccount, nome);

                        if (retornoIntegracao != null)
                        {
                            canalComunicacao.StatusModeloPadrao = retornoIntegracao.status;
                            canalComunicacao.IdModeloPadrao = retornoIntegracao.id;
                            await repoCanalComunicacao.SaveOrUpdate(canalComunicacao);

                            retorno.Mensagem = retornoIntegracao.status;
                            retorno.Ok = true;
                        }
                        else
                        {
                            retorno.Ok = false;
                            retorno.Mensagem = "Ocorreu um erro ao buscar o status do modelo de mensagem";
                        }
                    }
                    else
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = "Canal de comunicação não encontrado";
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = "Ocorreu uma exceção: " + ex.Message;
            }

            return retorno;
        }



        [HttpPost()]
        [Route("api/Canal_ComunicacaoApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CANAL_COMUNICACAO A ");

                    List<String> condicoes = new List<String>();

                    condicoes.Add($"WHATSAPP_BUSINESS_ACCOUNT IS NOT NULL");

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.WHATSAPP_BUSINESS_ACCOUNT) = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.WHATSAPP_BUSINESS_ACCOUNT) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.WHATSAPP_BUSINESS_ACCOUNT, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["WHATSAPP_BUSINESS_ACCOUNT"]),
                                    Text = $"{Convert.ToString(reader["NOME"])}"
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

        [HttpPost()]
        [Route("api/Canal_ComunicacaoApi/VincularModeloMensagem")]
        public async Task<PostApiRetorno<string>> VincularModeloMensagem(ModeloMensagemRequest request)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();

            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var repositoryMensagemPerdida = new RepositoryMongo<MensagemPerdidaAtendimentoMongo>(conexaoMongo))
                using (var repositoryBot = new Conexao<Bot>(session))
                {

                    var canal_comunicao = await repoCanalComunicacao.selectFirst(p => p.Id == Convert.ToInt64(request.idCanalComunicacao));
                    //TODO: YAGO - Revisar essa chamada ao IntegracaoWhatsAppBusinessRepository
                    var retornoIntegracao = await new IntegracaoWhatsAppBusinessRepository().VincularModeloMensagem(new MetaWhatsAppBusiness(request.token),
                                                                                                                    request.token,
                                                                                                                    request.whatsAppBusinessAccount,
                                                                                                                    request.mensagem,
                                                                                                                    "modelo_padrao");

                    canal_comunicao.StatusModeloPadrao = retornoIntegracao.Status;
                    canal_comunicao.IdModeloPadrao = retornoIntegracao.Id;
                    canal_comunicao.ModeloPadrao = request.mensagem;

                    await repoCanalComunicacao.SaveOrUpdate(canal_comunicao);

                    retorno.IdObjeto = retornoIntegracao.Id;
                    retorno.Mensagem = retornoIntegracao.Status;
                    retorno.Ok = true;
                }

            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Canal_ComunicacaoApi/AtualizarModeloMensagem")]
        public async Task<PostApiRetorno<string>> AtualizarModeloMensagem(ModeloMensagemRequest request)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();

            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                {
                    var canal_comunicao = await repoCanalComunicacao.selectFirst(p => p.Id == Convert.ToInt64(request.idCanalComunicacao));

                    //TODO: YAGO - Revisar essa chamada ao IntegracaoWhatsAppBusinessRepository
                    var retornoIntegracao = await new IntegracaoWhatsAppBusinessRepository().AtualizarModeloMensagem(new MetaWhatsAppBusiness(request.token),
                                                                                                                    canal_comunicao.IdModeloPadrao,
                                                                                                                    request.mensagem, null);
                    if (retornoIntegracao.success == "true")
                    {
                        canal_comunicao.ModeloPadrao = request.mensagem;
                        await repoCanalComunicacao.SaveOrUpdate(canal_comunicao);
                        retorno.Ok = true;
                    }
                    else
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = retornoIntegracao.error?.error_user_msg;
                    }
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Canal_ComunicacaoApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_CANAL_COMUNICACAO SET STATUS = {value.status} WHERE ID IN {_ids}");

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





    }
}