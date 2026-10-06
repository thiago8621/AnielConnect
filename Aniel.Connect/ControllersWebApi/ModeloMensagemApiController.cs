using Aniel.Connect.Controllers;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using NHibernate;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.MetaBusiness.Integracoes;
using Sinapse.Integracao.MetaBusiness.Models;
using Sinapse.Integracao.WhatsAppBusiness.Repositorio;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ModeloMensagem = Sinapse.Model.ModeloMensagem;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ModeloMensagemApiController : ApiController
    {
        [HttpPost]
        [Route("api/ModeloMensagemApi/GetAll")]
        public async Task<DataTableResponse<ModeloMensagemListaModel>> GetAll(DataTableRequest request)
        {
            var ModeloMensagems = new List<ModeloMensagemListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                var sql = new StringBuilder();
                var param = new Dictionary<string, object>();
                var filtros = new List<string>();

                var sqlFrom = new StringBuilder();
                sqlFrom.AppendLine("FROM TB_MODELO_MENSAGEM C");

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    filtros.Add($"UPPER(C.NOME) LIKE '%{request.search.value.ToUpperInvariant()}%'");
                }

                if (filtros.Count > 0)
                {
                    sqlFrom.AppendLine($"WHERE {String.Join(" AND ", filtros)}");
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(C.ID) CO ");
                    sqlCount.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, param, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                
                using (var com = session.Connection.CreateCommand())
                {
                    sql.AppendLine("SELECT C.ID, C.NOME, C.TEXTO, C.STATUS");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, param, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            ModeloMensagems.Add(new ModeloMensagemListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["NOME"]),
                                Status = BuscarStatusPtBr(Convert.ToString(reader["STATUS"])),
                                Texto = Convert.ToString(reader["TEXTO"]),
                            });
                        }
                    }
                }
            }

            return new DataTableResponse<ModeloMensagemListaModel>
            {
                data = ModeloMensagems,
                draw = request.draw,
                recordsFiltered = recordsTotal
            };
        }


        [HttpGet]
        [Route("api/ModeloMensagemApi/FindById")]
        public async Task<GetApiRetorno<ModeloMensagemCadastroModel>> FindById(long id)
        {
            var retorno = new GetApiRetorno<ModeloMensagemCadastroModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoModeloMensagem = new Conexao<ModeloMensagem>(session))
                {
                    ModeloMensagem ModeloMensagem = await repoModeloMensagem.selectFirst(p => p.Id == id);

                    retorno.Objeto = GetViewObject(ModeloMensagem);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }


        [HttpPost]
        [Route("api/ModeloMensagemApi/Gravar")]
        public async Task<PostApiRetorno<string>> Gravar([FromBody] ParamWebApi<ModeloMensagemCadastroModel> request)
        {
            var retorno = new PostApiRetorno<string>();

            try
            {
                using ISession session = HibernateUtil.GetSession();
                using var repositoryModelo = new Conexao<Sinapse.Model.ModeloMensagem>(session);
                using var repositoryCanal = new Conexao<Canal_Comunicacao>(session);

                var objeto = await repositoryCanal.selectFirst(p => p.IdNumeroWhatsAppBusiness == request.Objeto.IdWhatsAppBusinessIdentificacao);

                if (request.Objeto.Id == 0) // Novo Cadastro
                {
                    var existe = await repositoryModelo.select(p =>
                        p.Nome == request.Objeto.Descricao &&
                        p.IdWhatsAppBusinessAccount == request.Objeto.IdWhatsAppBusinessIdentificacao);

                    if (existe.Any())
                    {
                        throw new Exception("Já existe um objeto com o mesmo nome vinculado.");
                    }

                    var modelo = new ModeloMensagem
                    {
                        UsuarioCriacao = request.Usuario
                    };

                    modelo = await BindViewToObj(modelo, request.Objeto);

                    retorno = await VincularModeloMensagem(modelo);
                }
                else // Cadastro Existente
                {
                    var objetoExistente = (await repositoryModelo.select(p => p.Id == request.Objeto.Id)).FirstOrDefault();

                    if (objetoExistente == null)
                    {
                        throw new Exception("Objeto não encontrado.");
                    }

                    objetoExistente = await BindViewToObj(objetoExistente, request.Objeto);

                    retorno = await AtualizarModeloMensagem(objetoExistente, objeto?.Token);
                }


                //retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.IdObjeto = Convert.ToString(request.Objeto.Id);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }

        private async Task<ModeloMensagem> BindViewToObj(ModeloMensagem objBanco, ModeloMensagemCadastroModel view)
        {
            objBanco.Nome = view.Descricao;
            objBanco.Texto = view.Texto;
            objBanco.IdWhatsAppBusinessAccount = view.IdWhatsAppBusinessIdentificacao;
            objBanco.Status = BuscarStatusPtBr(objBanco.Status);

            return objBanco;
        }

        private ModeloMensagemCadastroModel GetViewObject(ModeloMensagem objBanco)
        {
            return new ModeloMensagemCadastroModel
            {
                Id = objBanco.Id,
                Codigo = Convert.ToString(objBanco?.Id),
                Descricao = objBanco.Nome,
                Texto = objBanco?.Texto,
                Status = BuscarStatusPtBr(objBanco.Status),
                IdWhatsAppBusinessIdentificacao = objBanco.IdWhatsAppBusinessAccount
            };
        }

        private string BuscarStatusPtBr(string status)
        {
            var statusDescricao = "";

            switch (status)
            {
                case "APPROVED":
                    statusDescricao = "Aprovado";
                    break;
                case "REJECT":
                    statusDescricao = "Rejeitado";
                    break;
                case "PENDING":
                    statusDescricao = "Pendente";
                    break;
                case "PAUSED":
                    statusDescricao = "Pausado";
                    break;
                default:
                    statusDescricao = "Indisponível";
                    break;
            }

            return statusDescricao;
        }

        [HttpPost]
        [Route("api/ModeloMensagemApi/VincularModeloMensagem")]
        public async Task<PostApiRetorno<string>> VincularModeloMensagem(ModeloMensagem modelo)
        {
            var retorno = new PostApiRetorno<string>();

            try
            {
                using ISession session = HibernateUtil.GetSession();
                using var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session);
                using var repoModelo = new Conexao<ModeloMensagem>(session);

                var canal = await repoCanalComunicacao.selectFirst(p =>
                    p.WhatsAppBusinessAccount == modelo.IdWhatsAppBusinessAccount);

                if (canal == null)
                {
                    throw new Exception("Canal de comunicação não encontrado.");
                }

                var retornoIntegracao = await new IntegracaoWhatsAppBusinessRepository().VincularModeloMensagem(
                    new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                    canal.TokenWhatsAppBusiness,
                    modelo.IdWhatsAppBusinessAccount,
                    modelo.Texto,
                    modelo.Nome
                );

                if (retornoIntegracao?.error != null)
                {
                    string errorMessage = retornoIntegracao.error.message ?? "Erro desconhecido na integração com WhatsApp Business.";
                    retorno.Ok = false;
                    throw new Exception($"Erro na integração WhatsApp Business: {errorMessage}");
                }

                modelo.Status = retornoIntegracao.Status;
                await repoModelo.Save(modelo);

                retorno.IdObjeto = retornoIntegracao.Id;
                retorno.Mensagem = retornoIntegracao.Status;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/ModeloMensagemApi/AtualizarModeloMensagem")]
        public async Task<PostApiRetorno<string>> AtualizarModeloMensagem(ModeloMensagem request, string token)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();

            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<ModeloMensagem>(session))
                {

                    var modelo = repo.selectFirst(p => p.Id == Convert.ToInt16(request.Id)).Result;

                    var retornoIntegracao = await new IntegracaoWhatsAppBusinessRepository().AtualizarModeloMensagem(new MetaWhatsAppBusiness(token),
                                                                                                                    Convert.ToString(modelo.Id),
                                                                                                                    request.Texto, request.Nome);
                    if (retornoIntegracao.success == "true")
                    {
                        modelo.Texto = request.Texto;
                        
                        await repo.SaveOrUpdate(modelo);
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
        [Route("api/ModeloMensagemApi/AtualizarStatusModeloMensagem")]
        public async Task<PostApiRetorno<string>> AtualizarStatusModeloMensagem(string idCanalComunicacao)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();

            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<ModeloMensagem>(session))
                using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                {

                    var canal = repoCanal.selectFirst(p => p.Id == Convert.ToInt32(idCanalComunicacao)).Result;
                    var modelos = repo.select().Result;

                    var retornoIntegracao = await new IntegracaoWhatsAppBusinessRepository().AtualizarStatusModeloMensagem(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                    canal.WhatsAppBusinessAccount);

                    if (retornoIntegracao.data?.Count > 0)
                    {

                        foreach (var modelo in modelos)
                        {

                            var modelo_padrao = retornoIntegracao.data
                                    .Where(p => p.name == modelo.Nome)
                                    .FirstOrDefault();

                            modelo.Status = modelo_padrao.status;

                            await repo.SaveOrUpdate(modelo);
                        }

                        retorno.Ok = true;
                    }
                    else
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = "Erro ao atualizar os modelos de mensagem.";
                    }
                }
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/ModeloMensagemApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MODELO_MENSAGEM A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 'APPROVED'");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME, A.TEXTO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboModeloMensagem<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["NOME"])}",
                                    TextoMensagem = $"{Convert.ToString(reader["TEXTO"])}"
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

    }
}
