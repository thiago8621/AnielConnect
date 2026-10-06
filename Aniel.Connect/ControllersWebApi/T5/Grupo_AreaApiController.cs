using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Usuario;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Grupo_AreaAnielApiController : BaseApiController
    {

        [HttpGet()]
        [Route("api/GrupoAreaapi/GetList")]
        public async Task<GetApiRetorno<List<ObjBusca<String>>>> GetList()
        {
            GetApiRetorno<List<ObjBusca<String>>> retorno = new GetApiRetorno<List<ObjBusca<String>>>();

            retorno.Objeto = new List<ObjBusca<String>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.COD_ESCRITORIO,A.DESCRICAO ");
                        sql.AppendLine("FROM TB_ESCRITORIO A ");
                        sql.AppendLine("WHERE A.STATUS = 1");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["COD_ESCRITORIO"]),
                                    Label = Convert.ToString(reader["DESCRICAO"])
                                };
                                retorno.Objeto.Add(obj);
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

        [HttpPost]
        [Route("api/GrupoAreaapi/GetCombo")]
        public async Task<ObjCombo<string>> GetCombo(RequestObjComboConfig<string, PerfilVisualizacaoServicoComboConfig> request)
        {
            var retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    var parametros = new Dictionary<string, object>();

                    var sqlFrom = new StringBuilder("FROM TB_ESCRITORIO A ");

                    var condicoes = new List<string>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.COD_ESCRITORIO = @COD_ESCRITORIO");
                        parametros.Add("@COD_ESCRITORIO", request.id);
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE @SEARCH_ESCRITORIO)");
                        parametros.Add("@SEARCH_ESCRITORIO", String.Format("%{0}%", StringService.Truncate(request.q.ToUpperInvariant(), 100)));
                    }

                    #region Filtro do Perfil de Visualização de Serviço

                    if (request.config != null && request.config.UsaPerfilVisualizacaoServico)
                    {
                        using (ISession session = HibernateUtil.GetSession())
                        {
                            IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                            var businessUsuario = new UsuarioBusiness(repoUsuario);

                            IdentityUser user = businessUsuario.GetById(User.Identity.GetUserId());

                            if (user.PerfilVisualizacaoServico != null &&
                                user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                            {
                                var listParamsGrupoArea = new List<string>();

                                foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea)
                                {
                                    listParamsGrupoArea.Add($"A.COD_ESCRITORIO = '{paramArea.CodEscritorio}'");
                                }

                                condicoes.Add($"({String.Join(" OR ", listParamsGrupoArea)})");
                            }
                        }
                    }

                    #endregion

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_ESCRITORIO) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.COD_ESCRITORIO, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, sessionT5.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["COD_ESCRITORIO"]),
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

        [AcceptVerbs("Get", "Post")]
        [Route("api/GrupoAreaapi/GetAll")]
        public async Task<DataTableResponse<GrupoArea_ListModel>> GetAll(GrupoArea_DataTableRequest request)
        {
            List<GrupoArea_ListModel> retorno = new List<GrupoArea_ListModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_ESCRITORIO A ");

                List<String> filtros = new List<string>();
                if (!String.IsNullOrEmpty(request.Id))
                {
                    filtros.Add("A.COD_ESCRITORIO = @COD_ESCRITORIO");
                    parametros.Add("@COD_ESCRITORIO", request.Id);
                }

                if (!String.IsNullOrEmpty(request.Status))
                {
                    filtros.Add(" (A.STATUS = @STATUS)");
                    parametros.Add("@STATUS", Convert.ToInt32(request.Status));
                }

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    filtros.Add("UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                if (filtros.Count > 0)
                {

                    sqlFrom.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                }


                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_ESCRITORIO) CO ");
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

                    sql.AppendLine("SELECT A.COD_ESCRITORIO, A.DESCRICAO, A.STATUS ");

                    sql.AppendLine(sqlFrom.ToString());



                    ////Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;
                        string sortDirection = request.order[0].dir;
                        switch (coluna)
                        {
                            case "status":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY STATUS ");

                                    else
                                        sql.AppendLine(" ORDER BY STATUS DESC ");
                                    break;
                                }
                            case "descricao":     // descricao
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY DESCRICAO");
                                    else
                                        sql.AppendLine(" ORDER BY DESCRICAO DESC");
                                    break;
                                }
                            case "id":     // id
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.COD_ESCRITORIO");
                                    else
                                        sql.AppendLine(" ORDER BY A.COD_ESCRITORIO DESC");
                                    break;
                                }
                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new GrupoArea_ListModel
                            {
                                Id = Convert.ToString(reader["COD_ESCRITORIO"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.ATIVO : RsCadastros.INATIVO,
                                IdCadastro = Convert.ToString(reader["COD_ESCRITORIO"]),
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<GrupoArea_ListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }


        [HttpGet()]
        [Route("api/GrupoAreaapi/FindById")]
        public async Task<GetApiRetorno<GrupoArea_CadastroModel>> FindById(String Id)
        {
            GetApiRetorno<GrupoArea_CadastroModel> retorno = new GetApiRetorno<GrupoArea_CadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<EscritorioAniel>(session))

            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Cod_Escritorio == Id));

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

        private GrupoArea_CadastroModel GetViewObj(EscritorioAniel obj)
        {
            return new GrupoArea_CadastroModel
            {
                Id = obj.Cod_Escritorio,
                Descricao = obj.Descricao,
                Status = obj.Status == Status_Default.Ativo,
                IdCadastro = obj.Cod_Escritorio,
            };
        }


        [HttpPost()]
        [Route("api/GrupoAreaapi/Gravar")]
        public async Task<PostApiRetorno<String>> Gravar([FromBody] ParamWebApi<GrupoArea_CadastroModel> value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                EscritorioAniel modelo = null;
                EscritorioAniel modeloAnterior = null;

                using ISession session = HibernateUtil.GetSessionT5();
                using ISession sessionUser = HibernateUtil.GetSession();
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<EscritorioAniel>(session))
                using (var repoUser = new Conexao<IdentityUser>(sessionUser))
                {

                    IdentityUser usuario = (await repoUser.selectFirst(p => p.Id == value.Usuario));

                    //verifica se já existe o ID solicitado para cadastro
                    bool num_repetido = (await repository.selectFirst(x => x.Cod_Escritorio == (value.Objeto.Id).ToString()) != null);
                    bool desc_repetido = (await repository.selectFirst(x => x.Descricao == (value.Objeto.Descricao).ToString()) != null);

                    if (num_repetido && String.IsNullOrEmpty(value.Objeto.IdCadastro))
                    {
                        retorno.Mensagem = "Já existe um Grupo Área cadastrado no sistema com o Código " + value.Objeto.Id + "!";
                        retorno.Ok = false;
                    }
                    if (desc_repetido && String.IsNullOrEmpty(value.Objeto.IdCadastro))
                    {
                        retorno.Mensagem = "Já existe um Grupo Área cadastrado no sistema com a Descrição " + value.Objeto.Descricao + "!";
                        retorno.Ok = false;
                    }
                    else
                    {

                        if (value.Objeto.Id != null)
                            modelo = (await repository.select(p => p.Cod_Escritorio == (value.Objeto.Id).ToString())).FirstOrDefault();
                        if (modelo != null)
                            modeloAnterior = (EscritorioAniel)modelo.Clone();

                        if (modelo == null)
                        {
                            modelo = new EscritorioAniel
                            {
                                Status = Status_Default.Ativo
                            };
                            value.Objeto.Status = true;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        //  log
                        try
                        {
                            if (modeloAnterior != null)
                            {
                                using (var repositoryLogs = new Conexao<LogEscritorioAniel>(sessionLog))
                                {
                                    var logs = modelo.GeraLog(modeloAnterior, usuario.Id, usuario.UserName, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                    await repositoryLogs.Save(logs);
                                }
                            }
                            else
                            {
                                using (var repositoryLogs = new Conexao<LogEscritorioAniel>(sessionLog))
                                {
                                    var logs = modelo.GeraLog(modeloAnterior, usuario.Id, usuario.UserName, Sinapse.ModelLog.TipoOperLog.Insercao);
                                    await repositoryLogs.Save(logs);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            retorno.IdObjeto = modelo.Cod_Escritorio;
                            retorno.Mensagem = ex.Message;
                            if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                        }

                        await repository.SaveOrUpdate(modelo);
                        retorno.IdObjeto = modelo.Cod_Escritorio;
                        retorno.Ok = true;
                    }

                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }




        private void BindViewToObj(ISession session, ref EscritorioAniel objBanco, GrupoArea_CadastroModel view)
        {
            objBanco.Cod_Escritorio = view.Id.ToString();
            objBanco.Descricao = view.Descricao;
            objBanco.Status = view.Status == true ? Status_Default.Ativo : Status_Default.Inativo;

        }

        [HttpPost()]
        [Route("api/GrupoAreaapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarLocalidadeStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                EscritorioAniel modelo = null;
                EscritorioAniel modeloAnterior = null;

                using ISession session = HibernateUtil.GetSessionT5();
                using ISession sessionUser = HibernateUtil.GetSession();
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<EscritorioAniel>(session))
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    #region log
                    IUsuarioRepository repoUsuario = new UsuarioRepository(sessionUser);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());

                    foreach (String item in value.ids)
                    {
                        modelo = (await repository.select(p => p.Cod_Escritorio == (item).ToString())).FirstOrDefault();

                        modeloAnterior = (EscritorioAniel)modelo.Clone();
                        modelo.Status = (Status_Default)value.status;

                        if ((int)modeloAnterior.Status != value.status)
                        {
                            using (var repositoryLogs = new Conexao<LogEscritorioAniel>(sessionLog))
                            {
                                var logs = modelo.GeraLog(modeloAnterior, user.Id, user.UserName, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLogs.Save(logs);
                            }
                        }

                    }
                    #endregion

                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids.Select(id => $"'{id}'"))})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_ESCRITORIO SET STATUS = {value.status} WHERE COD_ESCRITORIO IN {_ids}");

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
        [Route("api/GrupoAreaapi/HistoricoAlteracao")]
        public async Task<DataTableResponse<LogGrupoArea>> HistoricoAlteracao(HistoricoAlteracaoGrupoArea_DataTableRequest request)
        {
            List<LogGrupoArea> retorno = new List<LogGrupoArea>();
            List<LogGrupoArea> retornoLog = new List<LogGrupoArea>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();


                sqlFrom.AppendLine("FROM TB_ESCRITORIO A ");

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine(" AND ((UPPER(A.OBJETO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                }

                if (request.Id.HasValue)
                {
                    sqlFrom.AppendLine(" AND A.IDLOG = @ID ");
                    parametros.Add("@ID", request.Id);
                }

                if (request.TipoOperacao != null)
                {
                    sqlFrom.AppendLine(" AND A.TIPOOPERACAO = @TIPOOPERACAO ");
                    parametros.Add("@TIPOOPERACAO", Convert.ToInt32(request.TipoOperacao));
                }

                if (!String.IsNullOrEmpty(request.Objeto))
                {
                    sqlFrom.AppendLine(" AND A.OBJETO = @OBJETO ");
                    parametros.Add("@OBJETO", request.Objeto);
                }

                if (!String.IsNullOrEmpty(request.Campo))
                {
                    sqlFrom.AppendLine(" AND A.CAMPO = @CAMPO ");
                    parametros.Add("@CAMPO", request.Campo);
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(COD_ESCRITORIO) CO ");

                    sql.Append(sqlFrom.ToString());

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

                    sql.AppendLine("SELECT A.COD_ESCRITORIO, A.NOMEUSUARIOCRIACAO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.IDLOG, A.OBJETO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 0:     // CHAVE
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY OBJETO ");
                                    else
                                        sql.Append(" ORDER BY OBJETO DESC ");
                                    break;
                                }
                            case 1:     // CAMPO
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY CAMPO ");
                                    else
                                        sql.Append(" ORDER BY CAMPO DESC ");
                                    break;
                                }
                            case 6: //Data Hora
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY DATAHORACRIACAO ");
                                    else
                                        sql.Append(" ORDER BY DATAHORACRIACAO DESC ");
                                    break;
                                }
                        }
                    }

                    sql.Append(" ORDER BY DATAHORACRIACAO DESC ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retornoLog.Add(new LogGrupoArea
                            {
                                Id = Convert.ToInt64(reader["COD_ESCRITORIO"]),
                                Chave = Convert.ToString(reader["OBJETO"]),
                                Campo = Convert.ToString(reader["CAMPO"]),
                                Campo_ValorAntes = Convert.ToString(reader["CAMPO_VALORANTES"]),
                                Campo_ValorDepois = Convert.ToString(reader["CAMPO_VALORDEPOIS"]),
                                TipoOperacao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Nome_usuario = Convert.ToString(reader["NOMEUSUARIOCRIACAO"]),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                            });
                        }
                    }

                }
            }


            foreach (var ret in retornoLog)
            {
                if (ret.TipoOperacao == HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(TipoOperacao.Importacao)))
                {
                    ret.Observacao = ret.Campo_ValorDepois;
                    ret.Campo_ValorDepois = "-";
                    ret.Campo_ValorAntes = "-";
                }

                if (ret.TipoOperacao == HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(TipoOperacao.Insercao)))
                {
                    ret.Observacao = ret.Campo_ValorDepois;
                    ret.Campo_ValorDepois = "-";
                    ret.Campo_ValorAntes = "-";
                }

                if (ret.TipoOperacao == HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(TipoOperacao.Alteracao)))
                {
                    ret.Observacao = "-";
                }

                retorno.Add(ret);
            }

            //Page e retornando DataTable
            return new DataTableResponse<LogGrupoArea>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        public async Task<List<ExcelServiceLogItem>> Importacao(Grupo_Area_Importacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSessionT5())
            using (Conexao<EscritorioAniel> repoEscritorio = new Conexao<EscritorioAniel>(session))
            {
                EscritorioAniel grupoArea = new EscritorioAniel
                {
                    Status = Status_Default.Ativo,
                    Cod_Escritorio = item.Codigo,
                    Descricao = item.Descricao
                };

                retorno = await ImportacaoModeloBase(session, item, grupoArea);

                List<EscritorioAniel> total = (await repoEscritorio.select(p => p.Cod_Escritorio == grupoArea.Cod_Escritorio)).ToList();
                List<EscritorioAniel> totalDesc = (await repoEscritorio.select(p => p.Descricao == grupoArea.Descricao)).ToList();
                if (total.Count > 0)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo,
                        Erro = "O Código já é utilizado por outro Grupo Área!"
                    });

                if (totalDesc.Count > 0)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo,
                        Erro = "A Descrição já é utilizada por outro Grupo Área!"
                    });

                if (retorno.Count() > 0)
                    foreach (var logGravar in retorno)
                    {
                        EscritorioAniel modeloAnterior = new EscritorioAniel();
                        EscritorioAniel modelo = new EscritorioAniel()
                        {
                            Observacao = logGravar.Chave + " - " + logGravar.Erro,
                            Cod_Escritorio = logGravar.Chave,
                        };

                        using (ISession sessionLog = HibernateUtil.GetSessionLog())
                        using (ISession sessionS = HibernateUtil.GetSession())
                        using (var repositoryLogs = new Conexao<LogEscritorioAniel>(sessionLog))
                        using (var repository = new Conexao<EscritorioAniel>(sessionS))
                        {
                            var usuario = User.Identity.GetUserId();
                            var logs = modelo.GeraLog(modeloAnterior, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Importacao);

                            await repositoryLogs.Save(logs);
                        }
                    }

                if (retorno.Count() > 0)
                    return retorno;


                await repoEscritorio.SaveOrUpdate(grupoArea);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Codigo,
                    Erro = "Inserido com sucesso!"
                });

                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryLogs = new Conexao<LogEscritorioAniel>(sessionLog))
                {
                    EscritorioAniel modeloAnterior = new EscritorioAniel();
                    EscritorioAniel modelo = new EscritorioAniel()
                    {
                        Observacao = item.Codigo + " - " + "Inserido com sucesso!",
                        Cod_Escritorio = item.Codigo,
                    };

                    var logs = modelo.GeraLog(modeloAnterior, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Importacao);
                    await repositoryLogs.Save(logs);
                }

            }


            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, Grupo_Area_Importacao item, EscritorioAniel grupoArea)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (Conexao<EscritorioAniel> repoCliente = new Conexao<EscritorioAniel>(session))
            using (Grupo_AreaAnielApiController clienteApi = new Grupo_AreaAnielApiController())
            {
                if ((await repoCliente.selectFirst(p => p.Cod_Escritorio == item.Codigo)) != null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo,
                        Erro = "A Identificação Código Grupo Área pertence a um outro Grupo Área criado. Grupo Área não foi importado."
                    });
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/GrupoAreaapi/GetGrupoAreaLocalidadeByBairro")]
        public async Task<GetApiRetorno<GrupoAreaByBairroResponse>> GetGrupoAreaByBairro(GrupoAreaByBairroRequest request)
        {

            GetApiRetorno<GrupoAreaByBairroResponse> retorno = new GetApiRetorno<GrupoAreaByBairroResponse>();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())

            using (var repoCidade = new Conexao<Cidade>(session))
            using (var repoBairro = new Conexao<Bairro>(session))
            {
                try
                {
                    //Cidade cidade = await repoCidade.selectFirst(p => p.Descricao == request.DescricaoCidade);
                    //Bairro bairro = await repoBairro.selectFirst(p => p.Descricao == request.DescricaoBairro && p.Cidade == cidade);

                    //if (cidade == null || bairro == null)
                    //    throw new Exception("Cidade ou bairro não encontrados.");

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        //Filtros 
                        Dictionary<String, Object> parametros = new Dictionary<string, object>
                        {
                            { "@DESC_CIDADE", request.DescricaoCidade },
                            { "@DESC_BAIRRO", request.DescricaoBairro },
                        };

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT FIRST 1 A.COD_ESCRITORIO, A.DESCRICAO AS DESC_ESCRITORIO, B.COD_AREA, B.DESCRICAO AS DESC_AREA, C.COD_LOCALIDADE, C.LOCALIDADE AS DESC_LOCALIDADE ");
                        sql.AppendLine(" FROM TB_ESCRITORIO A ");
                        sql.AppendLine(" INNER JOIN TB_AREA B ON(A.COD_ESCRITORIO = B.COD_ESCRITORIO) ");
                        sql.AppendLine(" INNER JOIN TB_LOCALIDADE C ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO AND B.COD_AREA = C.COD_AREA) ");
                        sql.AppendLine($" WHERE (A.STATUS = 1 AND B.STATUS = 1 AND C.STATUS = 1) AND (UPPER(B.DESCRICAO) LIKE @DESC_CIDADE) AND (UPPER(C.LOCALIDADE) LIKE @DESC_BAIRRO)");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto = new GrupoAreaByBairroResponse
                                {
                                    IdGrupoArea = Convert.ToString(reader["COD_ESCRITORIO"]),
                                    DescGrupoArea = Convert.ToString(reader["DESC_ESCRITORIO"]),

                                    IdArea = Convert.ToString(reader["COD_AREA"]),
                                    DescArea = Convert.ToString(reader["DESC_AREA"]),

                                    IdLocalidade = Convert.ToString(reader["COD_LOCALIDADE"]),
                                    DescLocalidade = Convert.ToString(reader["DESC_LOCALIDADE"]),
                                };

                                retorno.Ok = true;
                            }
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    retorno.Ok = false;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }

                return retorno;
            }
        }
    }
}