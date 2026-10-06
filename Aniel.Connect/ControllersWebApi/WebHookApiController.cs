using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.WebHook;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class WebHookApiController : BaseApiController
    {
        #region WebHook
        public async Task<TResponse> ExecutaWebHook<TRequest, TResponse>(String Usuario, TRequest Request, WebHook_Type TipoWebHook)
            where TRequest : WebHookRequestBase
            where TResponse : WebHookResponseBase, new()
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<WebHook>(session))
            using (var repositoryExecute = new Conexao<WebHook_Execute>(session))
            {
                var _webHook = await repository.selectFirst(p => p.Type == TipoWebHook);

                if (_webHook == null)
                    return null;

                var _execucao = new WebHook_Execute
                {
                    UsuarioCriacao = Usuario,
                    DataHoraCriacao = DateTime.Now,
                    Type = TipoWebHook,
                    Attempts = 1,
                    State = WebHook_ExecuteState.Created,
                    WebHook = _webHook
                };

                using (var post = new PostHttp(CONSTANTES.log4Net))
                {
                    await FillRequest<TRequest>(session, Request);

                    var retorno = String.Empty;
                    try
                    {
                        _execucao.Request = JsonService.SerializeObject(Request);

                        retorno = post.ExecuteString<TResponse>(_webHook.Url, _execucao.Request);

                        _execucao.Response = retorno;
                        _execucao.State = WebHook_ExecuteState.Sucess;

                        await repositoryExecute.Save(_execucao);

                        return JsonService.DeserializeObject<TResponse>(retorno);
                    }
                    catch
                    {
                        _execucao.Response = retorno;
                        _execucao.State = WebHook_ExecuteState.Erro;
                        await repositoryExecute.Save(_execucao);

                        return null;
                    }
                }
            }
        }

        public async Task ExecutaBloqueioPlano(String Usuario, Int64 Id_Cliente, Int64 Id_Plano, DateTime Data_Bloqueio)
        {
            BloqueioPlanoWebHookRequest request = new BloqueioPlanoWebHookRequest
            {
                Id_Cliente = Id_Cliente,
                Id_Plano = Id_Plano,
                Data_Bloqueio = Data_Bloqueio
            };
            await ExecutaWebHook<BloqueioPlanoWebHookRequest, WebHookResponseBase>(Usuario, request, WebHook_Type.BloquearPlano);
        }

        public async Task ExecutaDesbloqueioPlano(String Usuario, Int64 Id_Cliente, Int64 Id_Plano, DateTime Data_Desbloqueio)
        {
            DesbloqueioPlanoWebHookRequest request = new DesbloqueioPlanoWebHookRequest
            {
                Id_Cliente = Id_Cliente,
                Id_Plano = Id_Plano,
                Data_Desbloqueio = Data_Desbloqueio
            };
            await ExecutaWebHook<DesbloqueioPlanoWebHookRequest, WebHookResponseBase>(Usuario, request, WebHook_Type.DesbloquearPlano);
        }

        public async Task ExecutaCancelamentoPlano(String Usuario, Int64 Id_Cliente,Int64 Id_Contrato, Int64 Id_Plano, DateTime Data_Cancelamento)
        {
            CancelamentoPlanoWebHookRequest request = new CancelamentoPlanoWebHookRequest
            {
                Id_Cliente = Id_Cliente,
                Id_Plano = Id_Plano,
                Id_Contrato = Id_Contrato,

                Data_Cancelamento = Data_Cancelamento
            };
            await ExecutaWebHook<CancelamentoPlanoWebHookRequest, WebHookResponseBase>(Usuario, request, WebHook_Type.CancelarPlano);
        }

        public async Task ExecutaAtivacaoPlano(String Usuario, Int64 Id_Cliente, Int64 Id_Plano, DateTime Data_Ativacao)
        {
            AtivacaoPlanoWebHookRequest request = new AtivacaoPlanoWebHookRequest
            {
                Id_Cliente = Id_Cliente,
                Id_Plano = Id_Plano,
                Data_Ativacao = Data_Ativacao
            };
            await ExecutaWebHook<AtivacaoPlanoWebHookRequest, WebHookResponseBase>(Usuario, request, WebHook_Type.AtivarPlano);
        }

        public async Task FillRequest<TRequest>(ISession sessionT5, TRequest obj)
            where TRequest : WebHookRequestBase
        {
            using (var repository = new Conexao<Configuracoes_Gerais>(sessionT5))
            {
                var config = await repository.selectFirst();

                if (config.Usuario_APIANIEL == null && config.Senha_APIANIEL == null && config.Token_APIANIEL == null)
                {
                    throw new Exception("API Aniel não configurado em Connect/Configurações Gerais");
                }

                obj.Usuario = config.Usuario_APIANIEL;
                obj.Senha = config.Senha_APIANIEL;
                obj.Token = config.Token_APIANIEL;
            }
        }


        #endregion

        [HttpPost()]
        [Route("api/WebHookApi/GetAll")]
        public async Task<DataTableResponse<WebHookListaModel>> GetAll(WebHook_DataTableRequest request)
        {

            List<WebHookListaModel> retorno = new List<WebHookListaModel>();
            IOrderedEnumerable<WebHookListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_WEBHOOK A");

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

                    sql.AppendLine("SELECT A.ID, A.URL, A.TYPE, A.STATUS");
                    sql.AppendLine(" FROM TB_WEBHOOK A");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.URL) LIKE UPPER(@SEARCH)) ) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new WebHookListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Url = Convert.ToString(reader["URL"]),
                                Type = HtmlEnumExtensions.GetEnumDescription<WebHook_Type>(Convert.ToString(reader["TYPE"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Id
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Type);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Type);
                            break;
                        }
                    //case 2:     // Type
                    //    {
                    //        if (sortDirection == "asc")
                    //            listaOrdenada = retorno.OrderBy(p => p.Type);
                    //        else
                    //            listaOrdenada = retorno.OrderByDescending(p => p.Type);
                    //        break;
                    //    }
                    //case 3:     // Url
                    //    {
                    //        if (sortDirection == "asc")
                    //            listaOrdenada = retorno.OrderBy(p => p.Url);
                    //        else
                    //            listaOrdenada = retorno.OrderByDescending(p => p.Url);
                    //        break;
                    //    }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Url);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<WebHookListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private WebHookCadastroModel GetViewObj(WebHook obj)
        {
            return new WebHookCadastroModel
            {
                Id = obj.Id,
                Url = obj.Url,
                Type = obj.Type,
                Status = obj.Status == Status_Default.Ativo ? true : false
            };

        }

        private void BindViewToObj(ISession session, WebHook objBanco, WebHookCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Url = view.Url;
            objBanco.Type = view.Type;
            if (objBanco.Id != 0)
            {
                objBanco.Status = view.Status == false ? Status_Default.Inativo : Status_Default.Ativo;
            }
        }

        [HttpPost()]
        [Route("api/WebHookApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_WebHook SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/WebHookApi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_WebHook WHERE ID IN {_ids}");

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

        [HttpGet()]
        [Route("api/WebHookApi/FindById")]
        public async Task<GetApiRetorno<WebHookCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<WebHookCadastroModel> retorno = new GetApiRetorno<WebHookCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<WebHook>(session))
            {
                try
                {
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
        [Route("api/WebHookApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<WebHookCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                WebHook modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<WebHook>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new WebHook
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

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
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


    }
}