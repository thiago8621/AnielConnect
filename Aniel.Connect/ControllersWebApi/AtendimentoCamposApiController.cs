using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using DocumentFormat.OpenXml.Bibliography;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using Microsoft.AspNet.Identity;
using NHibernate;
using OpenXmlPowerTools;
using Org.BouncyCastle.Ocsp;
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
    public class AtendimentoCamposApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/AtendimentoCamposApi/GetAll")]
        public async Task<DataTableResponse<AtendimentoCamposCadastroModels>> GetAll(AtendimentoCamposDataTableRequest request)
        {
            List<AtendimentoCamposCadastroModels> retorno = new List<AtendimentoCamposCadastroModels>();
            Int32 recordsTotal = 0;

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            var condicoes = new List<string>();

            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlWhere = new StringBuilder();
                StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO_CAMPOS A  ");

                condicoes.Add("A.ID IS NOT NULL");

                if (request.IdsTipoAtendimento != null && request.IdsTipoAtendimento.Count() > 0)
                {
                    condicoes.Add($"(B.ID_TIPO_ATENDIMENTO IN({string.Join(", ", request.IdsTipoAtendimento)}) OR A.VISUALIZACAO_TOTAL = 1)");

                    sqlFrom.AppendLine(" LEFT JOIN TB_ATENDIMENTO_CAMPOS_TIPO B ");
                    sqlFrom.AppendLine(" ON (B.ID_ATENDIMENTO_CAMPO = A.ID) ");
                }

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    condicoes.Add("((UPPER(A.TITULO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                if (!String.IsNullOrEmpty(request.TipoCampo) && request.TipoCampo != "99")
                {
                    condicoes.Add("A.TIPO_CAMPO = @TIPOCAMPO ");
                    parametros.Add("@TIPOCAMPO", request.TipoCampo);
                }

                if (!String.IsNullOrEmpty(request.Status))
                {
                    condicoes.Add("A.STATUS = @STATUS ");
                    parametros.Add("@STATUS", request.Status);
                }

                if (!String.IsNullOrEmpty(request.CampoObrigatorio))
                {
                    condicoes.Add("A.CAMPO_OBRIGATORIO = @CAMPOBRIG ");
                    parametros.Add("@CAMPOBRIG", request.CampoObrigatorio);
                }

                sqlWhere.AppendLine($"WHERE {string.Join(" AND ", condicoes)}");
                sqlFrom.AppendLine(sqlWhere.ToString());

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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

                    sql.AppendLine("SELECT A.ID AS CODIGO, A.TITULO AS DESCRICAO, A.CAMPO_OBRIGATORIO AS OBRIGATORIO, A.STATUS AS STATUS, A.TIPO_CAMPO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new AtendimentoCamposCadastroModels
                            {
                                Id = Convert.ToInt64(reader["CODIGO"]),
                                CampoObrigatorio = reader["OBRIGATORIO"] != DBNull.Value ? Convert.ToBoolean(reader["OBRIGATORIO"]) : false,
                                TipoCampoGravarString = reader["TIPO_CAMPO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<TipoCampoGravar>(Convert.ToString(reader["TIPO_CAMPO"])) : "-",
                                Titulo = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,
                                Status = reader["STATUS"] != DBNull.Value ? (Convert.ToBoolean(reader["STATUS"])) : false,
                            });
                        }
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<AtendimentoCamposCadastroModels>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/AtendimentoCamposApi/GetCamposPersonalizados")]
        public async Task<PostApiRetorno<List<CamposPersonalizadosResponse>>> GetCamposPersonalizados(long TipoSelecionado)
        {
            List<CamposPersonalizadosResponse> retorno = new List<CamposPersonalizadosResponse>();
            Int32 recordsTotal = 0;

            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO_CAMPOS A  ");
                sqlFrom.AppendLine(" LEFT JOIN TB_ATENDIMENTO_CAMPOS_TIPO B ");
                sqlFrom.AppendLine(" ON (B.ID_ATENDIMENTO_CAMPO = A.ID) ");
                sqlFrom.AppendLine(" WHERE A.STATUS = 1 AND (B.ID_TIPO_ATENDIMENTO = @IDTIPO OR A.VISUALIZACAO_TOTAL = 1) ");
                parametros.Add("@IDTIPO", TipoSelecionado);

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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

                    sql.AppendLine("SELECT A.ID AS CODIGO, A.TITULO AS DESCRICAO, A.CAMPO_OBRIGATORIO AS OBRIGATORIO, A.STATUS AS STATUS, A.TIPO_CAMPO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new CamposPersonalizadosResponse
                            {
                                IdCampo = Convert.ToString(reader["CODIGO"]),
                                Obrigatorio = reader["OBRIGATORIO"] != DBNull.Value ? Convert.ToBoolean(reader["OBRIGATORIO"]) : false,
                                TipoCampo = reader["TIPO_CAMPO"] != DBNull.Value ? Convert.ToString(reader["TIPO_CAMPO"]) : "-",
                                Titulo = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,
                            });
                        }
                    }
                }


                foreach (var campo in retorno)
                {
                    if (campo.ListaRespostas == null)
                        campo.ListaRespostas = new List<RespostasPersonalizadasUnicas>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID AS CODIGO, A.RESPOSTA_PERSONALIZADA AS DESCRICAO, B.ID AS ID_ATT_CAMPO ");
                        sql.AppendLine(" FROM TB_ATENDIMENTO_CAMPOS_RESPOSTAS A ");
                        sql.AppendLine(" LEFT JOIN TB_ATENDIMENTO_CAMPOS B ON (B.ID = A.ID_ATENDIMENTO_CAMPO) ");
                        sql.AppendLine($" WHERE A.ID_ATENDIMENTO_CAMPO = {campo.IdCampo} ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                campo.ListaRespostas.Add(new RespostasPersonalizadasUnicas
                                {
                                    IdResposta = Convert.ToString(reader["CODIGO"]),
                                    IdAtendimentoCampo = Convert.ToString(reader["ID_ATT_CAMPO"]),
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty
                                });
                            }
                        }
                    }
                }
            }

            var response = new PostApiRetorno<List<CamposPersonalizadosResponse>>
            {
                IdObjeto = retorno,
                Mensagem = "Obtido os campos com sucesso!",
                Ok = true
            };

            return response;
        }


        [HttpPost()]
        [Route("api/AtendimentoCamposApi/ObterVinculoRespostas")]
        public async Task<PostApiRetorno<List<CamposPersonalizadosResponse>>> ObterVinculoRespostas(long IdAtendimento)
        {
            var response = new PostApiRetorno<List<CamposPersonalizadosResponse>>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoVinculo = new Conexao<VinculoAtendimentoCampos>(session))
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                {
                    var atendimento = await repoAtendimento.selectFirst(x => x.Id == IdAtendimento);

                    var todosVinculos = repoVinculo.select(x => x.Atendimento == atendimento).Result.ToList();

                    if (todosVinculos != null && todosVinculos.Count() > 0)
                    {
                        response.IdObjeto = new List<CamposPersonalizadosResponse>();

                        foreach (var vinc in todosVinculos)
                        {
                            if (!response.IdObjeto.Any(x => x.IdCampo == vinc.AtendimentoCampos.Id.ToString()))
                            {
                                var campoResposta = new CamposPersonalizadosResponse()
                                {
                                    IdCampo = vinc.AtendimentoCampos.Id.ToString(),
                                    Resposta = vinc.Resposta,
                                    ListaRespostas = new List<RespostasPersonalizadasUnicas>()
                                };

                                if (vinc.AtendimentoCamposRespostas != null)
                                    campoResposta.ListaRespostas.Add(new RespostasPersonalizadasUnicas { IdResposta = Convert.ToString(vinc.AtendimentoCamposRespostas.Id) });

                                response.IdObjeto.Add(campoResposta);
                            }
                            else
                            {
                                var campoJaAdicionado = response.IdObjeto.Where(x => x.IdCampo == vinc.Id.ToString()).FirstOrDefault();

                                if (vinc.AtendimentoCamposRespostas != null && !campoJaAdicionado.ListaRespostas.Any(x => x.IdResposta == Convert.ToString(vinc.AtendimentoCamposRespostas.Id)))
                                    campoJaAdicionado.ListaRespostas.Add(new RespostasPersonalizadasUnicas { IdResposta = Convert.ToString(vinc.AtendimentoCamposRespostas.Id) });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.IdObjeto = null;
                return response;
            }

            response.Ok = true;
            return response;
        }

        [HttpPost()]
        [Route("api/AtendimentoCamposApi/GetHistoricoAtendimentoCampos")]
        public async Task<DataTableResponse<AtendimentoCamposHistoricoResponse>> GetHistoricoAtendimentoCampos(AtendimentoCamposHistoricoRequest request)
        {
            List<AtendimentoCamposHistoricoResponse> retorno = new List<AtendimentoCamposHistoricoResponse>();
            Int32 recordsTotal = 0;

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO_CAMPOS A WHERE A.TIPOOPERACAO IS NOT NULL ");
            //Filtrando
            if (!String.IsNullOrEmpty(request.search.value))
            {
                sqlFrom.AppendLine(" AND ((UPPER(A.CAMPO) LIKE UPPER(@SEARCH))) ");
                parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
            }

            if (!String.IsNullOrEmpty(request.DataInicial) && !String.IsNullOrEmpty(request.DataFim))
            {
                string dataInicio = Convert.ToDateTime(request.DataInicial).ToString("dd/MM/yyyy HH:mm:ss");
                string dataFim = Convert.ToDateTime(request.DataFim).ToString("dd/MM/yyyy 23:59:59");

                sqlFrom.AppendLine(" AND A.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAFIM ");
                parametros.Add("@DATAINICIO", dataInicio);
                parametros.Add("@DATAFIM", dataFim);
            }

            if (!String.IsNullOrEmpty(request.Usuario))
            {
                sqlFrom.AppendLine(" AND (A.USUARIOCRIACAO = @USUARIO OR A.NOMEUSUARIOCRIACAO = @USUARIO) ");
                parametros.Add("@USUARIO", request.Usuario);
            }

            if (request.TipoOperacao != null && request.TipoOperacao.Length > 0)
            {
                sqlFrom.AppendLine($" AND A.TIPOOPERACAO IN ({String.Join(",", request.TipoOperacao)}) ");
            }

            if (request.IdCampoPersonalizado != null && request.IdCampoPersonalizado != 0)
            {
                sqlFrom.AppendLine($" AND A.IDLOG = @IDCAMPO ");
                parametros.Add("@IDCAMPO", request.IdCampoPersonalizado);
            }

            using (ISession session = HibernateUtil.GetSessionLog())
            {


                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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

                    sql.AppendLine(" SELECT A.IDLOG AS ID, A.USUARIOCRIACAO, A.NOMEUSUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS ");
                    sql.AppendLine(sqlFrom.ToString());



                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            string valorAnterior = reader["CAMPO_VALORANTES"] != DBNull.Value ? Convert.ToString(reader["CAMPO_VALORANTES"]) : "N/A";
                            string valorPosterior = reader["CAMPO_VALORDEPOIS"] != DBNull.Value ? Convert.ToString(reader["CAMPO_VALORDEPOIS"]) : "N/A";

                            if (valorAnterior == "True")
                                valorAnterior = "Ativo";
                            else if (valorAnterior == "False")
                                valorAnterior = "Inativo";

                            if (valorPosterior == "True")
                                valorPosterior = "Ativo";
                            else if (valorPosterior == "False")
                                valorPosterior = "Inativo";

                            retorno.Add(new AtendimentoCamposHistoricoResponse
                            {
                                CodigoCampo = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : "N/A",
                                Usuario = reader["USUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCRIACAO"]) : "N/A",
                                NomeUsuario = reader["NOMEUSUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["NOMEUSUARIOCRIACAO"]) : "N/A",
                                Operacao = reader["TIPOOPERACAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])) : "N/A",
                                Campo = reader["CAMPO"] != DBNull.Value ? Convert.ToString(reader["CAMPO"]) : "N/A",
                                ValorAntes =  !string.IsNullOrEmpty(valorAnterior) ? valorAnterior : "N/A",
                                ValorDepois = !string.IsNullOrEmpty(valorPosterior) ? valorPosterior : "N/A",
                                DataHora = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToString(reader["DATAHORACRIACAO"]) : "N/A",
                            });
                        }
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<AtendimentoCamposHistoricoResponse>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }


        [HttpPost()]
        [Route("api/AtendimentoCamposApi/GetRespostasCampos")]
        [AcceptVerbs("Get", "Post")]
        public async Task<DataTableResponse<AtendimentoCamposRespostasResponse>> GetRespostasCampos(AtendimentoCamposDataTableRequest request)
        {
            List<AtendimentoCamposRespostasResponse> retorno = new List<AtendimentoCamposRespostasResponse>();
            Int32 recordsTotal = 0;

            if (request.Id == null || request.Id == 0)
            {
                return new DataTableResponse<AtendimentoCamposRespostasResponse>
                {
                    draw = request.draw,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };
            }


            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO_CAMPOS_RESPOSTAS A ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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

                    sql.AppendLine("SELECT A.ID AS CODIGO, A.RESPOSTA_PERSONALIZADA AS RESPOSTA ");
                    sql.AppendLine(sqlFrom.ToString());


                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new AtendimentoCamposRespostasResponse
                            {
                                Id = Convert.ToInt64(reader["CODIGO"]),
                                RespostaPersonalizada = Convert.ToString(reader["RESPOSTA"])
                            });
                        }
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<AtendimentoCamposRespostasResponse>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/AtendimentoCamposApi/GetTiposCamposAtendimento")]
        public async Task<DataTableResponse<AtendimentoCamposTiposModels>> GetTiposCamposAtendimento(AtendimentoCamposDataTableRequest request)
        {
            List<AtendimentoCamposTiposModels> retorno = new List<AtendimentoCamposTiposModels>();
            Int32 recordsTotal = 0;

            if (request.Id == null || request.Id == 0)
            {
                return new DataTableResponse<AtendimentoCamposTiposModels>
                {
                    draw = request.draw,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };
            }

            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO_CAMPOS_TIPO A ");
                sqlFrom.AppendLine(" INNER JOIN TB_TIPO_ATENDIMENTO B ON (B.ID = A.ID_TIPO_ATENDIMENTO) ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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

                    sql.AppendLine("SELECT A.ID AS CODIGO, A.ID_TIPO_ATENDIMENTO AS TIPO_ATENDIMENTO, B.NOME AS NOME ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine(" WHERE A.ID_ATENDIMENTO_CAMPO = @IDCAMPO ");
                    parametros.Add("@IDCAMPO", request.Id);

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new AtendimentoCamposTiposModels
                            {
                                Id = Convert.ToInt64(reader["CODIGO"]),
                                Id_Tipo_Atendimento = Convert.ToString(reader["TIPO_ATENDIMENTO"]),
                                DescricaoTipo = Convert.ToString(reader["NOME"])
                            });
                        }
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<AtendimentoCamposTiposModels>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/AtendimentoCamposApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionlog = HibernateUtil.GetSessionLog())
                using (var repo = new Conexao<AtendimentoCampos>(session))
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        bool status = value.status == 1 ? true : false;

                        List<AtendimentoCampos> campos = new List<AtendimentoCampos>();

                        foreach (var id in value.ids)
                        {
                            var model = await repo.selectFirst(x => x.Id == id);

                            if (model != null)
                                campos.Add(model);
                        }

                        foreach (var obj in campos)
                        {
                            var objCopia = (AtendimentoCampos)obj.Clone();

                            obj.Status = status;
                            obj.DataHoraAlteracao = DateTime.Now;
                            obj.UsuarioAlteracao = User.Identity.GetUserId();

                            await repo.SaveOrUpdate(obj);

                            using (var repositoryLogs = new Conexao<LogAtendimentoCampos>(sessionlog))
                            {
                                var logs = obj.GeraLog(objCopia, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLogs.Save(logs);
                            }
                        }

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
        [Route("api/AtendimentoCamposApi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_ATENDIMENTO_ACAO WHERE ID IN {_ids}");

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
        [Route("api/AtendimentoCamposApi/FindById")]
        public async Task<GetApiRetorno<AtendimentoCamposCadastroModels>> FindById(Int64 Id)
        {
            GetApiRetorno<AtendimentoCamposCadastroModels> retorno = new GetApiRetorno<AtendimentoCamposCadastroModels>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtendimentoCampos = new Conexao<AtendimentoCampos>(session))
            {
                try
                {
                    if (Id == 0)
                        retorno.Objeto = new AtendimentoCamposCadastroModels { Id = 0 };
                    else
                        retorno.Objeto = GetViewObj(await repoAtendimentoCampos.selectFirst(p => p.Id == Id), session);

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
        [Route("api/AtendimentoCamposApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<AtendimentoCamposCadastroModels> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                AtendimentoCampos atendimentoCampo = null;
                AtendimentoCampos atendimentoCampoAnterior = null;

                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<AtendimentoCampos>(session))
                    using (var repoAtendimentoResp = new Conexao<AtendimentoCamposRespostas>(session))
                    using (var repoAtendimentoTipos = new Conexao<AtendimentoCamposTipo>(session))
                    {
                        if (value.Objeto.Id != 0)
                            atendimentoCampo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (atendimentoCampo == null)
                        {
                            if (value.Objeto == null)
                                throw new Exception("Erro em obter dados do formulário.");

                            if (string.IsNullOrEmpty(value.Objeto.Titulo))
                                throw new Exception("Informe o Título para o cadastro.");

                            if (value.Objeto.TipoCampoGravar == null)
                                throw new Exception("Informe o tipo do campo para o cadastro.");

                            atendimentoCampo = new AtendimentoCampos
                            {
                                Status = true,
                                UsuarioCriacao = value.Usuario,
                                DataHoraCriacao = DateTime.Now,
                                CampoObrigatorio = value.Objeto.CampoObrigatorio,
                                Titulo = value.Objeto.Titulo,
                                TipoCampoGravar = value.Objeto.TipoCampoGravar.Value,
                                VisualizacaoTotal = value.Objeto.ExibirTodosAtendimentos
                            };

                            if ((value.Objeto.AtendimentoCamposTiposModels is null || value.Objeto.AtendimentoCamposTiposModels.Count() == 0) && !value.Objeto.ExibirTodosAtendimentos)
                                throw new Exception("Informe os tipos de atendimentos referente a esses campos personalizados");

                            List<AtendimentoCamposTipo> atendimentosCamposTipos = new List<AtendimentoCamposTipo>();

                            foreach (var item in value?.Objeto?.AtendimentoCamposTiposModels)
                            {
                                if (string.IsNullOrEmpty(item.Id_Tipo_Atendimento))
                                    throw new Exception("Id do Tipo de Atendimento não informado.");

                                if (string.IsNullOrEmpty(item.DescricaoTipo))
                                    throw new Exception("Descricao do Tipo de Atendimento não informado.");

                                var tipoAtendimentoCampos = new AtendimentoCamposTipo
                                {
                                    TipoAtendimento = Convert.ToInt64(item.Id_Tipo_Atendimento),
                                    DescricaoTipo = item.DescricaoTipo,
                                    AtendimentoCampos = atendimentoCampo
                                };

                                atendimentosCamposTipos.Add(tipoAtendimentoCampos);
                            }

                            List<AtendimentoCamposRespostas> atendimentosRespostas = new List<AtendimentoCamposRespostas>();

                            if (value.Objeto.TipoCampoGravar == TipoCampoGravar.UnicaEscolha)
                            {
                                if (value.Objeto.AtendimentoCamposRespostaPerso is null ||
                                    value.Objeto.AtendimentoCamposRespostaPerso.Count() == 0)
                                    throw new Exception("Respostas personalizadas não informada para o tipo de campo selecionado");

                                foreach (var item in value.Objeto.AtendimentoCamposRespostaPerso)
                                {
                                    if (string.IsNullOrEmpty(item.RespostaPersonalizada))
                                        throw new Exception("Respostas personalizada não informada.");

                                    var atendimentoRespostas = new AtendimentoCamposRespostas
                                    {
                                        RespostaPersonalizada = item.RespostaPersonalizada,
                                        AtendimentoCampos = atendimentoCampo,
                                    };

                                    atendimentosRespostas.Add(atendimentoRespostas);
                                }
                            }

                            await repository.SaveOrUpdate(atendimentoCampo);

                            if (atendimentosCamposTipos.Count() > 0)
                                await repoAtendimentoTipos.SaveOrUpdate(atendimentosCamposTipos);

                            if (atendimentosRespostas.Count() > 0)
                                await repoAtendimentoResp.SaveOrUpdate(atendimentosRespostas);
                        }
                        else
                        {
                            atendimentoCampo.UsuarioAlteracao = value.Usuario;
                            atendimentoCampo.DataHoraAlteracao = DateTime.Now;
                            atendimentoCampoAnterior = (AtendimentoCampos)atendimentoCampo.Clone();
                            await BindViewToObj(session, sessionLog, atendimentoCampo, value.Objeto,
                                                repository, repoAtendimentoResp, repoAtendimentoTipos,
                                                value.Usuario, value.NomeUsuario);
                        }

                        if (atendimentoCampoAnterior != null)
                        {
                            using (var repositoryLogs = new Conexao<LogAtendimentoCampos>(sessionLog))
                            {
                                var logs = atendimentoCampo.GeraLog(atendimentoCampoAnterior, value.Usuario,
                                                                    value.NomeUsuario,
                                                                    Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLogs.Save(logs);
                            }
                        }
                        else
                        {
                            using (var repositoryLogs = new Conexao<LogAtendimentoCampos>(sessionLog))
                            {
                                var logs = atendimentoCampo.GeraLog(atendimentoCampoAnterior, value.Usuario,
                                                                    value.NomeUsuario,
                                                                    Sinapse.ModelLog.TipoOperLog.Insercao);
                                await repositoryLogs.Save(logs);
                            }
                        }

                    }

                    retorno.IdObjeto = atendimentoCampo.Id;
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

        [Route("api/AtendimentoCamposApi/ExcluirGridRespostas")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridRespostas(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<AtendimentoCamposRespostas>(session))
                    using (var repositoryVinculoAtendimentoCampos = new Conexao<VinculoAtendimentoCampos>(session))
                    {

                        AtendimentoCamposRespostas modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();
                            if (modelo == null)
                            {
                                throw new Exception(Resources.RsBase.NAOENCONTRADO);
                            }

                            var vinculo = await repositoryVinculoAtendimentoCampos.selectFirst(p => p.AtendimentoCamposRespostas.Id == modelo.Id);
                            if (vinculo != null)
                                throw new Exception("Não é possível excluir pois já existe vínculo com Atendimento.");

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

        [Route("api/AtendimentoCamposApi/ExcluirGridTipos")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridTipos(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<AtendimentoCamposTipo>(session))
                    {

                        AtendimentoCamposTipo modelo = null;
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

        [HttpPost()]
        [Route("api/AtendimentoCamposApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<String, Int64?> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ATENDIMENTO_CAMPOS A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1 ");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.TITULO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.TITULO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.TITULO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new IdentityUserComboModel
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["TITULO"]),
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



        private AtendimentoCamposCadastroModels GetViewObj(AtendimentoCampos obj, ISession session)
        {
            var atendimentoModel = new AtendimentoCamposCadastroModels();
            using (var repoAtendimentoTipos = new Conexao<AtendimentoCamposTipo>(session))
            using (var repoTipoAtendimento = new Conexao<Tipo_Atendimento>(session))
            using (var repoAtendimentoResp = new Conexao<AtendimentoCamposRespostas>(session))
            {
                atendimentoModel.Status = obj.Status;
                atendimentoModel.Titulo = obj.Titulo;
                atendimentoModel.TipoCampoGravar = obj.TipoCampoGravar;
                atendimentoModel.Id = obj.Id;
                atendimentoModel.CampoObrigatorio = obj.CampoObrigatorio.Value;
                atendimentoModel.ExibirTodosAtendimentos = obj.VisualizacaoTotal;
                atendimentoModel.AtendimentoCamposTiposModels = repoAtendimentoTipos.select(p => p.AtendimentoCampos.Id == obj.Id).Result.Select(p => new AtendimentoCamposTiposModels
                {
                    Id = p.Id,
                    DescricaoTipo = repoTipoAtendimento.selectFirst(y => y.Id == p.TipoAtendimento).Result?.Nome,
                    Id_Tipo_Atendimento = Convert.ToString(p.TipoAtendimento.Value)
                }).ToList();
                atendimentoModel.AtendimentoCamposRespostaPerso = repoAtendimentoResp.select(p => p.AtendimentoCampos.Id == obj.Id).Result.Select(p => new AtendimentoCamposRespostaPerso
                {
                    Id = p.Id,
                    RespostaPersonalizada = p.RespostaPersonalizada
                }).ToList();
            }

            return atendimentoModel;
        }

        private async Task<bool> BindViewToObj(ISession session, ISession sessionLog, AtendimentoCampos objBanco,
                                   AtendimentoCamposCadastroModels view,
                                   Conexao<AtendimentoCampos> repository,
                                   Conexao<AtendimentoCamposRespostas> repoAtendimentoResp,
                                   Conexao<AtendimentoCamposTipo> repoAtendimentoTipo, string usuarioId,
                                   string usuarioNome)
        {
            AtendimentoCamposTipo atendimentoCampoTipoAnterior = null;
            AtendimentoCamposRespostas atendimentoCampoRespostasAnterior = null;

            objBanco.Titulo = view.Titulo;
            objBanco.CampoObrigatorio = view.CampoObrigatorio;
            objBanco.Status = view.Status;
            objBanco.VisualizacaoTotal = view.ExibirTodosAtendimentos;

            List<AtendimentoCamposRespostas> atendimentosRespostas = new List<AtendimentoCamposRespostas>();

            if (objBanco.TipoCampoGravar == TipoCampoGravar.UnicaEscolha ||
                (view.AtendimentoCamposRespostaPerso != null && view.AtendimentoCamposRespostaPerso.Count() > 0))
            {
                if (view.AtendimentoCamposRespostaPerso is null ||
                    view.AtendimentoCamposRespostaPerso.Count() == 0)
                    throw new Exception("Respostas personalizadas não informada para o tipo de campo selecionado");

                foreach (var item in view.AtendimentoCamposRespostaPerso)
                {
                    if (item.Id == 0)
                    {
                        if (string.IsNullOrEmpty(item.RespostaPersonalizada))
                            continue;

                        var atendimentoRespostas = new AtendimentoCamposRespostas
                        {
                            RespostaPersonalizada = item.RespostaPersonalizada,
                            AtendimentoCampos = objBanco,
                            UsuarioCriacao = usuarioId,
                            DataHoraCriacao = DateTime.Now
                        };

                        using (var repositoryLogs = new Conexao<LogAtendimentoCampos>(sessionLog))
                        {
                            var logs = atendimentoRespostas.GeraLog(atendimentoCampoRespostasAnterior, usuarioId, usuarioNome, Sinapse.ModelLog.TipoOperLog.Insercao);
                            await repositoryLogs.Save(logs);
                        }

                        atendimentosRespostas.Add(atendimentoRespostas);
                    }
                    else
                    {
                        var objBancoResp = repoAtendimentoResp.selectFirst(x => x.Id == item.Id).Result;
                        atendimentoCampoRespostasAnterior = (AtendimentoCamposRespostas)objBancoResp.Clone();

                        objBancoResp.RespostaPersonalizada = item.RespostaPersonalizada;
                        objBancoResp.DataHoraAlteracao = DateTime.Now;
                        objBancoResp.UsuarioAlteracao = usuarioId;

                        await repoAtendimentoResp.SaveOrUpdate(objBancoResp);

                        using (var repositoryLogs = new Conexao<LogAtendimentoCampos>(sessionLog))
                        {
                            var logs = objBancoResp.GeraLog(atendimentoCampoRespostasAnterior, usuarioId, usuarioNome, Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogs.Save(logs);
                        }
                    }
                }
            }

            List<AtendimentoCamposTipo> atendimentosCamposTipos = new List<AtendimentoCamposTipo>();

            foreach (var item in view.AtendimentoCamposTiposModels)
            {
                if (item.Id == 0)
                {
                    if (string.IsNullOrEmpty(item.Id_Tipo_Atendimento))
                        continue;

                    if (string.IsNullOrEmpty(item.DescricaoTipo))
                        continue;

                    var tipoAtendimentoCampos = new AtendimentoCamposTipo
                    {
                        TipoAtendimento = Convert.ToInt64(item.Id_Tipo_Atendimento),
                        DescricaoTipo = item.DescricaoTipo,
                        AtendimentoCampos = objBanco,
                        UsuarioCriacao = usuarioId,
                        DataHoraCriacao = DateTime.Now
                    };

                    using (var repositoryLogs = new Conexao<LogAtendimentoCampos>(sessionLog))
                    {
                        var logs = tipoAtendimentoCampos.GeraLog(atendimentoCampoTipoAnterior, usuarioId, usuarioNome, Sinapse.ModelLog.TipoOperLog.Insercao);
                        await repositoryLogs.Save(logs);
                    }

                    atendimentosCamposTipos.Add(tipoAtendimentoCampos);
                }
                else
                {
                    var objBancoTipo = await repoAtendimentoTipo.selectFirst(x => x.Id == item.Id);
                    atendimentoCampoTipoAnterior = (AtendimentoCamposTipo)objBancoTipo.Clone();

                    objBancoTipo.TipoAtendimento = Convert.ToInt64(item.Id_Tipo_Atendimento);
                    objBancoTipo.DescricaoTipo = item.DescricaoTipo;
                    objBancoTipo.DataHoraAlteracao = DateTime.Now;
                    objBancoTipo.UsuarioAlteracao = usuarioId;

                    await repoAtendimentoTipo.SaveOrUpdate(objBancoTipo);

                    using (var repositoryLogs = new Conexao<LogAtendimentoCampos>(sessionLog))
                    {
                        var logs = objBancoTipo.GeraLog(atendimentoCampoTipoAnterior, usuarioId, usuarioNome, Sinapse.ModelLog.TipoOperLog.Alteracao);
                        await repositoryLogs.Save(logs);
                    }
                }
            }

            await repository.SaveOrUpdate(objBanco);

            if (atendimentosCamposTipos.Count() > 0)
                await repoAtendimentoTipo.SaveOrUpdate(atendimentosCamposTipos);

            if (atendimentosRespostas.Count() > 0)
                await repoAtendimentoResp.SaveOrUpdate(atendimentosRespostas);

            return true;
        }

        [Route("api/AtendimentoCamposApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Atendimento_Acao>(session))
                    {

                        Atendimento_Acao modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

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
        [HttpPost]
        [Route("api/AtendimentoCamposApi/GravarResposta")] 
        public async Task<PostApiRetorno> GravarResposta(RespostasPersonalizadasUnicas request) 
        {
            var retorno = new PostApiRetorno();
            try 
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSession())
                using (var repository = new Conexao<AtendimentoCamposRespostas>(session)) 
                using (var repositoryLog = new Conexao<LogAtendimentoCampos>(session)) 
                {
                    var resposta = repository.QueryableSelect().Where(p => p.Id == Convert.ToInt64(request.IdResposta)).First();
                    var clone = (AtendimentoCamposRespostas)resposta.Clone();
                    resposta.RespostaPersonalizada = request.Descricao;
                    await repository.SaveOrUpdate(resposta);

                    var logs = resposta.GeraLog(clone,User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                    await repositoryLog.Save(logs);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex) 
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
    }
}
