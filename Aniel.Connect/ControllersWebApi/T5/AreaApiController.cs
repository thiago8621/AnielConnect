using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
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
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class AreaApiController : BaseApiController
    {

        [HttpGet()]
        [Route("api/Areaapi/GetList")]
        public async Task<GetApiRetorno<List<ObjBusca<String>>>> GetList(String grupo_area)
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

                        sql.AppendLine("SELECT A.COD_AREA,A.DESCRICAO");
                        sql.AppendLine("FROM TB_AREA A");
                        sql.AppendLine("WHERE A.COD_ESCRITORIO = @VALOR AND A.STATUS = 1 ");
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                        parametros.Add("@VALOR", grupo_area);


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["COD_AREA"]),
                                    Label = Convert.ToString(reader["DESCRICAO"])
                                });
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
        [Route("api/Areaapi/GetCombo")]
        public async Task<ObjCombo<string>> GetCombo(RequestObjComboPaiConfig<string, string, PerfilVisualizacaoServicoComboConfig> request)
        {
            var retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                //Filtros
                var parametros = new Dictionary<string, object>();

                var sqlFrom = new StringBuilder("FROM TB_AREA A  ");

                var condicoes = new List<string>();

                if (request.soativos)
                {
                    condicoes.Add($"A.STATUS = 1");
                }

                if (!String.IsNullOrEmpty(request.id))
                {
                    condicoes.Add($" A.COD_AREA = @COD_AREA ");
                    parametros.Add("@COD_AREA", request.id);
                }
                if (!String.IsNullOrEmpty(request.idpai))
                {
                    condicoes.Add($" A.COD_ESCRITORIO = @COD_ESCRITORIO ");
                    parametros.Add("@COD_ESCRITORIO", request.idpai);
                }

                if (request != null && !String.IsNullOrEmpty(request.q))
                {
                    condicoes.Add($"(UPPER(A.DESCRICAO) LIKE @SEARCH_AREA)");
                    parametros.Add("@SEARCH_AREA", String.Format("%{0}%", StringService.Truncate(request.q.ToUpperInvariant(), 100)));

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
                            user.PerfilVisualizacaoServico
                                .ParamsArea
                                .Where(p => !String.IsNullOrEmpty(p.CodArea))
                                .Count() > 0)
                        {
                            var listParamsGrupoArea = new List<string>();

                            foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea.Where(p => !String.IsNullOrEmpty(p.CodArea)))
                            {
                                listParamsGrupoArea.Add($"A.COD_AREA = '{paramArea.CodArea}'");
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
                    var sql = new StringBuilder("SELECT COUNT(A.COD_AREA) AS CO ");
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
                    var sql = new StringBuilder("SELECT A.COD_AREA, A.DESCRICAO ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                    using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, sessionT5.Connection, null, request.page))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new ObjComboResult<string>
                            {
                                Id = Convert.ToString(reader["COD_AREA"]),
                                Text = Convert.ToString(reader["DESCRICAO"])
                            };

                            retorno.results.Add(obj);
                        }
                    }
                }
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Areaapi/GetComboRegiao")]
        public async Task<ObjCombo<Int64>> GetComboRegiao(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_REGIAO A WHERE A.COD_REGIAO IS NOT NULL ");

                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($" AND A.COD_REGIAO = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder(" SELECT COUNT(A.COD_REGIAO) AS CO ");
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
                        StringBuilder sql = new StringBuilder(" SELECT A.COD_REGIAO, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.NOME) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["COD_REGIAO"]),
                                    Text = Convert.ToString(reader["NOME"])
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
        [Route("api/Areaapi/GetAll")]
        public async Task<DataTableResponse<Area_ListModel>> GetAll(Area_DataTableRequest request)
        {
            List<Area_ListModel> retorno = new List<Area_ListModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_AREA A ")
                                        .AppendLine("INNER JOIN TB_ESCRITORIO B ON B.COD_ESCRITORIO = A.COD_ESCRITORIO ");

                List<String> filtros = new List<string>();
                if (!String.IsNullOrEmpty(request.Id))
                {
                    filtros.Add("A.COD_AREA = @COD_AREA");
                    parametros.Add("@COD_AREA", request.Id);
                }

                if (!String.IsNullOrEmpty(request.IdGrupoArea))
                {
                    filtros.Add("A.COD_ESCRITORIO = @COD_ESCRITORIO");
                    parametros.Add("@COD_ESCRITORIO", request.IdGrupoArea);
                }

                if (!String.IsNullOrEmpty(request.Status))
                {
                    filtros.Add("A.STATUS = @STATUS");
                    parametros.Add("@STATUS", request.Status);
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
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_AREA) CO ");
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

                    sql.AppendLine("SELECT A.DESCRICAO, B.DESCRICAO AS GRUPOAREADESCR, A.STATUS, A.COD_AREA, B.COD_ESCRITORIO ");


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
                                        sql.AppendLine(" ORDER BY A.STATUS ");

                                    else
                                        sql.AppendLine(" ORDER BY A.STATUS DESC ");
                                    break;
                                }
                            case "descricao":     // descricao
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.DESCRICAO");
                                    else
                                        sql.AppendLine(" ORDER BY A.DESCRICAO DESC");
                                    break;
                                }
                            case "grupoAreaDescricao":     // descricaoGrupoArea
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY B.DESCRICAO");
                                    else
                                        sql.AppendLine(" ORDER BY B.DESCRICAO DESC");
                                    break;
                                }
                            case "id":     // id
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.COD_AREA");
                                    else
                                        sql.AppendLine(" ORDER BY A.COD_AREA DESC");
                                    break;
                                }
                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Area_ListModel
                            {
                                Id = Convert.ToString(reader["COD_AREA"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                GrupoAreaDescricao = Convert.ToString(reader["GRUPOAREADESCR"]),
                                Status = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.ATIVO : RsCadastros.INATIVO,
                                IdGrupoArea = Convert.ToString(reader["COD_ESCRITORIO"]),
                                IdCadastro = Convert.ToString(reader["COD_AREA"]),
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Area_ListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/Areaapi/FindById")]
        public async Task<GetApiRetorno<Area_CadastroModel>> FindById(String Id, String Cod_Escritorio)
        {
            GetApiRetorno<Area_CadastroModel> retorno = new GetApiRetorno<Area_CadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<AreaAniel>(session))
            using (var repositoryEscritorio = new Conexao<EscritorioAniel>(session))

            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Cod_Area == Id && p.Cod_Escritorio == Cod_Escritorio));

                    var GrupoAreaDescricao = await repositoryEscritorio.selectFirst(x => x.Cod_Escritorio == retorno.Objeto.IdGrupoArea);
                    retorno.Objeto.GrupoAreaDescricao = GrupoAreaDescricao.Descricao;

                    retorno.Objeto.CompositeId = new Area_CadastroModel.compositeId()
                    {
                        IdGrupoArea = retorno.Objeto?.IdGrupoArea,
                        IdArea = retorno.Objeto?.IdCadastro
                    };

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

        private Area_CadastroModel GetViewObj(AreaAniel obj)
        {
            return new Area_CadastroModel
            {
                Id = obj?.Cod_Area,
                Descricao = obj?.Descricao,
                Status = obj?.Status == Status_Default.Ativo,
                IdGrupoArea = obj?.Cod_Escritorio,

                GrupoAreaDescricao = obj?.Grupo_Area?.Descricao,
                IdCadastro = obj?.Cod_Area,

                Latitude = Convert.ToString(obj?.Latitude),
                Longitude = Convert.ToString(obj?.Longitude),
                Distancia = obj?.Distancia,

                Uf = obj?.UF,
            };
        }


        [HttpPost()]
        [Route("api/Areaapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Area_CadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                AreaAniel modelo = null;
                AreaAniel modeloAnterior = null;

                using ISession session = HibernateUtil.GetSessionT5();
                using ISession sessionUser = HibernateUtil.GetSession();
                using (var repository = new Conexao<AreaAniel>(session))
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryGrupoArea = new Conexao<EscritorioAniel>(session))
                using (var repoUser = new Conexao<IdentityUser>(sessionUser))
                {

                    IdentityUser usuario = (await repoUser.selectFirst(p => p.Id == value.Usuario));

                    if (value.Objeto.CompositeId.IdGrupoArea != null && value.Objeto.CompositeId.IdArea != null)
                        modelo = await repository.selectFirst(x => x.Cod_Escritorio == value.Objeto.CompositeId.IdGrupoArea && x.Cod_Area == (value.Objeto.CompositeId.IdArea).ToString());

                    modeloAnterior = (AreaAniel)modelo?.Clone();



                    BindViewToObj(session, ref modelo, value.Objeto);



                    if (modeloAnterior != null)
                    {

                        using (var com = session.Connection.CreateCommand())
                        {
                            Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                            StringBuilder sql = new StringBuilder();

                            sql.AppendLine(" UPDATE TB_AREA ")
                               .AppendLine(" SET COD_ESCRITORIO = @COD_ESCRITORIO, COD_AREA = @COD_AREA, ")
                               .AppendLine(" DESCRICAO = @DESCRICAO, STATUS = @STATUS, UF = @UF, LATITUDE = @LATITUDE, LONGITUDE = @LONGITUDE, DISTANCIA = @DISTANCIA ")
                               .AppendLine(" WHERE COD_ESCRITORIO = @COD_ESCRITORIO_ANTERIOR AND COD_AREA = @COD_AREA_ANTERIOR ");

                            parametros.Add("@COD_ESCRITORIO", value.Objeto.IdGrupoArea);
                            parametros.Add("@COD_AREA", value.Objeto.Id);
                            parametros.Add("@DESCRICAO", value.Objeto.Descricao);
                            parametros.Add("@STATUS", value.Objeto.Status);
                            //parametros.Add("CODCT", modelo.Contrato);
                            //parametros.Add("PROJETO", modelo.Projeto);
                            parametros.Add("@UF", value.Objeto.Uf);
                            parametros.Add("@LATITUDE", value.Objeto.Latitude);
                            parametros.Add("@LONGITUDE", value.Objeto.Longitude);
                            parametros.Add("@DISTANCIA", value.Objeto.Distancia);

                            parametros.Add("@COD_ESCRITORIO_ANTERIOR", value.Objeto.CompositeId.IdGrupoArea);
                            parametros.Add("@COD_AREA_ANTERIOR", value.Objeto.CompositeId.IdArea);

                            await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                        }
                    }
                    else
                    {
                        await repository.Save(modelo);
                    }


                    //  log
                    try
                    {
                        if (modeloAnterior != null)
                        {
                            using (var repositoryLogs = new Conexao<LogAreaAniel>(sessionLog))
                            {
                                var logs = modelo.GeraLog(modeloAnterior, usuario.Id, usuario.Nome, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLogs.Save(logs);
                            }
                        }
                        else
                        {
                            using (var repositoryLogs = new Conexao<LogAreaAniel>(sessionLog))
                            {
                                var logs = modelo.GeraLog(modelo, usuario.Id, usuario.Nome, Sinapse.ModelLog.TipoOperLog.Insercao);
                                await repositoryLogs.Save(logs);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        retorno.IdObjeto = 0;
                        retorno.Mensagem = ex.Message;
                        if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    }

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = Convert.ToInt64(value.Objeto.Id);

                if (ex.Message.Contains("violation of PRIMARY or UNIQUE KEY"))
                    retorno.Mensagem = "Já Existe uma Área cadastrada no sistema ";
                else
                    retorno.Mensagem = ex.Message;

                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Areaapi/HistoricoAlteracao")]
        public async Task<DataTableResponse<LogArea>> HistoricoAlteracao(HistoricoAlteracaoArea_DataTableRequest request)
        {
            List<LogArea> retornoLog = new List<LogArea>();
            List<LogArea> retorno = new List<LogArea>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();


                sqlFrom.AppendLine("FROM TB_AREA A ");

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
                    StringBuilder sql = new StringBuilder("SELECT COUNT(COD_AREA) CO ");

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

                    sql.AppendLine("SELECT A.COD_AREA, A.NOMEUSUARIOCRIACAO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.IDLOG, A.OBJETO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS, A.OBSERVACAO ");
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
                            retornoLog.Add(new LogArea
                            {
                                Id = Convert.ToInt64(reader["COD_AREA"]),
                                Chave = Convert.ToString(reader["OBJETO"]),
                                Campo = Convert.ToString(reader["CAMPO"]),
                                Campo_ValorAntes = Convert.ToString(reader["CAMPO_VALORANTES"]),
                                Campo_ValorDepois = Convert.ToString(reader["CAMPO_VALORDEPOIS"]),
                                TipoOperacao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Nome_usuario = Convert.ToString(reader["NOMEUSUARIOCRIACAO"]),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Observacao = Convert.ToString(reader["OBSERVACAO"]),
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
                    using ISession session = HibernateUtil.GetSessionT5();
                    if (ret.Campo == "Grupo Área")
                    {
                        using (var repositoryGrupoArea = new Conexao<EscritorioAniel>(session))
                        {
                            var antes = (await repositoryGrupoArea.selectFirst(p => p.Cod_Escritorio == ret.Campo_ValorAntes));
                            var depois = (await repositoryGrupoArea.selectFirst(p => p.Cod_Escritorio == ret.Campo_ValorDepois));

                            ret.Campo_ValorAntes = antes.Descricao;
                            ret.Campo_ValorDepois = depois.Descricao;
                        }
                    }
                }

                retorno.Add(ret);
            }

            //Page e retornando DataTable
            return new DataTableResponse<LogArea>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        private void BindViewToObj(ISession session, ref AreaAniel objBanco, Area_CadastroModel view)
        {

            if (objBanco == null)
            {
                objBanco = new AreaAniel
                {
                    Descricao = view.Descricao,
                    Cod_Area = view.Id,

                    Cod_Escritorio = view.IdGrupoArea,
                    Status = Status_Default.Ativo,
                    UF = view.Uf,

                    Cod_Cliente = "1",
                };
                view.Status = true;
            }


            objBanco.Cod_Escritorio = view.IdGrupoArea.ToString();
            objBanco.Cod_Area = view.Id.ToString();
            objBanco.Descricao = view.Descricao;
            objBanco.UF = view.Uf;
            objBanco.Status = view.Status == true ? Status_Default.Ativo : Status_Default.Inativo;
            objBanco.Latitude = string.IsNullOrEmpty(view.Latitude) ? (double?)null : Convert.ToDouble(view.Latitude.Replace(".", ","));
            objBanco.Longitude = string.IsNullOrEmpty(view.Longitude) ? (double?)null : Convert.ToDouble(view.Longitude.Replace(".", ","));
            objBanco.Distancia = Convert.ToInt32(view.Distancia);
            using (var repoEsc = new Conexao<EscritorioAniel>(session))
            {
                if (view.Id == null)
                {
                    objBanco.Grupo_Area = repoEsc.selectFirst(p => p.Cod_Escritorio == view.IdGrupoArea).Result;
                    view.GrupoAreaDescricao = objBanco.Grupo_Area.Descricao;
                }
            }
        }

        [HttpPost()]
        [Route("api/Areaapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarLocalidadeStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                AreaAniel modelo = null;
                AreaAniel modeloAnterior = null;

                using ISession session = HibernateUtil.GetSessionT5();
                using ISession sessionUser = HibernateUtil.GetSession();
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<AreaAniel>(session))
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    #region log
                    IUsuarioRepository repoUsuario = new UsuarioRepository(sessionUser);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());

                    foreach (String item in value.ids)
                    {
                        modelo = (await repository.select(p => p.Cod_Area == (item).ToString())).FirstOrDefault();

                        modeloAnterior = (AreaAniel)modelo.Clone();
                        modelo.Status = (Status_Default)value.status;

                        if ((int)modeloAnterior.Status != value.status)
                        {
                            using (var repositoryLogs = new Conexao<LogAreaAniel>(sessionLog))
                            {
                                var logs = modelo.GeraLog(modeloAnterior, user.Id, user.UserName, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLogs.Save(logs);
                            }
                        }

                    }
                    #endregion

                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"('{String.Join("','", value.ids)}')";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_AREA SET STATUS = '{value.status}' WHERE COD_AREA IN {_ids}");

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

        public async Task<List<ExcelServiceLogItem>> Importacao(Area_Importacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSessionT5())
            using (NHibernate.ISession sessionUser = HibernateUtil.GetSession())
            using (Conexao<AreaAniel> repoArea = new Conexao<AreaAniel>(session))
            using (Conexao<IdentityUser> repoUser = new Conexao<IdentityUser>(sessionUser))
            using (Conexao<EscritorioAniel> repoGrupoArea = new Conexao<EscritorioAniel>(session))
            {
                EscritorioAniel codValido = null;
                AreaAniel descricaoValida = null;
                AreaAniel area = null;

                IdentityUser usuario = (await repoUser.selectFirst(p => p.Id == User.Identity.GetUserId()));

                //<!-- VALIDAÇÕES -->
                if (!String.IsNullOrEmpty(item.Grupo_Area))
                {
                    //1) Busca se p GRUPO ÁREA existe (Descrição ou Código)
                    if ((item.Grupo_Area).All(char.IsDigit))
                    {
                        codValido = (await repoGrupoArea.selectFirst(p => p.Cod_Escritorio == item.Grupo_Area));
                    }
                    else
                    {
                        codValido = (await repoGrupoArea.selectFirst(p => p.Descricao == item.Grupo_Area));
                    }

                    //2) Verifica se já existe uma ÁREA cadastrada com a mesma Descrição e mesmo Grupo Área
                    if (!String.IsNullOrEmpty(codValido.Cod_Escritorio))
                    {
                        descricaoValida = (await repoArea.selectFirst(p => p.Cod_Escritorio == codValido.Cod_Escritorio && p.Descricao == codValido.Descricao));

                        if (!String.IsNullOrEmpty(descricaoValida?.Cod_Escritorio))
                        {
                            retorno.Add(new ExcelServiceLogItem
                            {
                                Chave = item.Codigo,
                                Erro = "Já existe uma Área Cadastrada com a mesma Descrição e Grupo Área!"
                            });
                        }
                    }

                    //Verifica se o Status do Grupo Área é Ativo
                    else if (codValido.Status == Status_Default.Inativo)
                    {
                        retorno.Add(new ExcelServiceLogItem
                        {
                            Chave = item.Codigo,
                            Erro = "Grupo Área informado Inativo!"
                        });
                    }

                    //Verifica se existe Grupo Área cadastrado com a Descrição
                    else
                    {
                        retorno.Add(new ExcelServiceLogItem
                        {
                            Chave = item.Codigo,
                            Erro = "Não foram encontrados um Grupo Área com a Descrição ou Código informados!"
                        });
                    }
                }

                //Verifica erro na leitura do Grupo Área
                else
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo,
                        Erro = "Erro na leitura do campo Grupo Área!"
                    });
                }

                //Erro na leitura do campo Código
                if (String.IsNullOrEmpty(item.Codigo))
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo,
                        Erro = "Erro na leitura do campo Código"
                    });
                }

                //Erro na leitura do campo Descrição
                if (String.IsNullOrEmpty(item.Descricao))
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo,
                        Erro = "Erro na leitura do campo Descrição"
                    });
                }

                AreaAniel Area = new AreaAniel
                {
                    Status = Status_Default.Ativo,
                    Cod_Escritorio = codValido.Cod_Escritorio,
                    Cod_Area = item.Codigo,
                    Descricao = item.Descricao,
                    Cod_Cliente = "1",
                };

                retorno = await ImportacaoModeloBase(session, item, Area);


                if (retorno.Count() > 0)
                    foreach (var logGravar in retorno)
                    {
                        AreaAniel modeloAnterior = new AreaAniel();
                        AreaAniel modelo = new AreaAniel()
                        {
                            Observacao = logGravar.Chave + " - " + logGravar.Erro,
                            Cod_Area = logGravar.Chave,
                        };

                        using (ISession sessionLog = HibernateUtil.GetSessionLog())
                        using (ISession sessionS = HibernateUtil.GetSession())
                        using (var repositoryLogs = new Conexao<LogAreaAniel>(sessionLog))
                        using (var repository = new Conexao<AreaAniel>(sessionS))
                        {
                            var logs = modelo.GeraLog(modeloAnterior, usuario.Id, usuario.Nome, Sinapse.ModelLog.TipoOperLog.Importacao); ;
                            await repositoryLogs.Save(logs);
                        }
                    }


                if (retorno.Count() > 0)
                    return retorno;


                await repoArea.SaveOrUpdate(Area);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Codigo,
                    Erro = "Inserido com sucesso!"
                });


                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryLogs = new Conexao<LogAreaAniel>(sessionLog))
                {
                    AreaAniel modeloAnterior = new AreaAniel();
                    AreaAniel modelo = new AreaAniel()
                    {
                        Observacao = item.Codigo + " - " + "Inserido com sucesso!",
                        Cod_Area = item.Codigo,
                    };
                    var logs = modelo.GeraLog(modeloAnterior, usuario.Id, usuario.Nome, Sinapse.ModelLog.TipoOperLog.Importacao);
                    await repositoryLogs.Save(logs);
                }

            }

            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, Area_Importacao item, AreaAniel Area)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (Conexao<AreaAniel> repoArea = new Conexao<AreaAniel>(session))
            using (Conexao<EscritorioAniel> repoEscritorio = new Conexao<EscritorioAniel>(session))
            using (Grupo_AreaAnielApiController clienteApi = new Grupo_AreaAnielApiController())
            {
                if ((await repoArea.selectFirst(p => p.Cod_Area == item.Codigo)) != null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo,
                        Erro = "A Identificação Código Área pertence a um outra Área criado. Área não foi importada."
                    });


                //if (String.IsNullOrEmpty(item.Grupo_Area))
                //{

                //    var escritorio = await repoEscritorio.selectFirst(p => p.Cod_Escritorio == item.Grupo_Area);
                //    if (escritorio == null)
                //        retorno.Add(new ExcelServiceLogItem { Chave = item.Grupo_Area, Erro = "Código do Grupo Área não encontrado no sistema" });
                //}
            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/Areaapi/RelatorioAreaVinculadaTecnico")]
        public async Task<DataTableResponse<RelatorioAreaListModel>> RelatorioAreaVinculadaTecnico(EquipeAreaDataTableRequest request)
        {
            List<RelatorioAreaListModel> retorno = new List<RelatorioAreaListModel>();
            retorno = new List<RelatorioAreaListModel>();

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

                        if (request.TipoRelatorio == "area")
                        {
                            sql.AppendLine(@"SELECT A.LOCALIDADE, B.DESCRICAO AS AREA, C.DESCRICAO AS ESCRITORIO, D.NUM_FRE AS MATRICULA, D.NOME AS NOME_EQUIPE,VINCULO.USUARIOCRIACAO,VINCULO.DATAHORACRIACAO,VINCULO.TODAS_LOCALIDADES, E.DESCRICAO AS PERFIL, I.NOME AS SUPERVISOR, H.NOME AS MONITOR
                                         FROM TB_EQUIPE D
                                         LEFT JOIN TB_PERFIL_AGENDAMENTO E
                                         ON(D.ID_PERFIL_AGENDAMENTO = E.ID)
                                         LEFT JOIN TB_PERFIL_AGENDAMENTO_AREA VINCULO
                                         ON(VINCULO.ID_PERFIL_AGENDAMENTO = E.ID)
                                         LEFT JOIN TB_LOCALIDADE A
                                         ON (A.LOCALIDADE = VINCULO.LOCALIDADE AND A.COD_AREA = VINCULO.COD_AREA AND A.COD_ESCRITORIO = VINCULO.COD_ESCRITORIO AND A.COD_CLIENTE = 1)
                                         LEFT JOIN TB_AREA B
                                         ON (B.COD_AREA = VINCULO.COD_AREA AND B.COD_ESCRITORIO = VINCULO.COD_ESCRITORIO AND B.COD_CLIENTE = 1)
                                         LEFT JOIN TB_ESCRITORIO C
                                         ON (C.COD_ESCRITORIO = VINCULO.COD_ESCRITORIO)
                                         LEFT JOIN TB_FUNCIONARIO I
                                         ON(I.NUM_FRE = D.COD_SUPERV)
                                         LEFT JOIN TB_SENHA H
                                         ON(D.USUARIO_MONITOR = H.USUARIO)
                                         WHERE (A.COD_CLIENTE = 1 OR VINCULO.TODAS_LOCALIDADES = 1) ");
                        }

                        if (request.TipoRelatorio == "habilidade")
                        {
                            sql.AppendLine(@"SELECT A.DESCRICAO AS HABILIDADE,D.NUM_FRE AS MATRICULA, D.NOME AS NOME_EQUIPE,VINCULO.USUARIOCRIACAO,VINCULO.DATAHORACRIACAO,E.DESCRICAO AS PERFIL, I.NOME AS SUPERVISOR, H.NOME AS MONITOR
                                         FROM TB_EQUIPE D
                                         LEFT JOIN TB_PERFIL_AGENDAMENTO E
                                         ON(D.ID_PERFIL_AGENDAMENTO = E.ID)
                                         LEFT JOIN TB_PERFIL_AGENDAMENTO_NATUREZA VINCULO
                                         ON(VINCULO.ID_PERFIL_AGENDAMENTO = E.ID)
                                         LEFT JOIN TB_NATUREZA_PRODUCAO A
                                         ON(VINCULO.ID_NATUREZA = A.ID)
                                         LEFT JOIN TB_FUNCIONARIO I
                                         ON(I.NUM_FRE = D.COD_SUPERV)
                                         LEFT JOIN TB_SENHA H
                                         ON(D.USUARIO_MONITOR = H.USUARIO) ");
                        }



                        if (!String.IsNullOrEmpty(request.Equipe))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" D.EQUIPE = @EQUIPE");
                            parametros.Add("@EQUIPE", request.Equipe);
                        }

                        if (!String.IsNullOrEmpty(request.Grupo_Area))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" C.COD_ESCRITORIO = @GRUPO_AREA");
                            parametros.Add("@GRUPO_AREA", request.Grupo_Area);
                        }

                        if (!String.IsNullOrEmpty(request.Area))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" B.COD_AREA = @AREA");
                            parametros.Add("@AREA", request.Area);
                        }

                        if (!String.IsNullOrEmpty(request.Localidade))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" A.LOCALIDADE = @LOCALIDADE");
                            parametros.Add("@LOCALIDADE", request.Localidade);
                        }
                        if (!String.IsNullOrEmpty(request.Perfil))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine("E.ID = @PERFIL");
                            parametros.Add("@PERFIL", request.Perfil);
                        }
                        if (!String.IsNullOrEmpty(request.Habilidades))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine("A.ID = @HABILIDADES");
                            parametros.Add("@HABILIDADES", request.Habilidades);
                        }
                        if (!String.IsNullOrEmpty(request.Supervisor))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine("I.NUM_FRE = @SUPERVISOR");
                            parametros.Add("@SUPERVISOR", request.Supervisor);
                        }
                        if (!String.IsNullOrEmpty(request.Monitor))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine("MONITOR = @MONITOR");
                            parametros.Add("@MONITOR", request.Monitor);
                        }

                        switch (request.Agrupamento)
                        {
                            case "supervisor":
                                sql.Append($"ORDER BY SUPERVISOR, MATRICULA DESC");
                                break;

                            case "monitor":
                                sql.Append($"ORDER BY MONITOR, MATRICULA DESC");
                                break;

                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {


                                if (request.TipoRelatorio == "area")
                                {
                                    bool todasLocalidades = reader["TODAS_LOCALIDADES"] != DBNull.Value ? Convert.ToBoolean(reader["TODAS_LOCALIDADES"]) : false;
                                    retorno.Add(new RelatorioAreaListModel
                                    {
                                        Grupo_Area = reader["ESCRITORIO"] != DBNull.Value ? Convert.ToString(reader["ESCRITORIO"]) : String.Empty,
                                        Area = reader["AREA"] != DBNull.Value ? Convert.ToString(reader["AREA"]) : String.Empty,
                                        Localidade = todasLocalidades ? RsBase.TODAS : reader["LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["LOCALIDADE"]) : String.Empty,
                                        Equipe = reader["NOME_EQUIPE"] != DBNull.Value ? Convert.ToString(reader["NOME_EQUIPE"]) : String.Empty,
                                        Data_Criacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                        Usuario_Criacao = reader["USUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCRIACAO"]) : String.Empty,
                                        Matricula = reader["MATRICULA"] != DBNull.Value ? Convert.ToString(reader["MATRICULA"]) : String.Empty,
                                        Perfil = reader["PERFIL"] != DBNull.Value ? Convert.ToString(reader["PERFIL"]) : String.Empty,
                                        Supervisor = reader["SUPERVISOR"] != DBNull.Value ? Convert.ToString(reader["SUPERVISOR"]) : String.Empty,
                                        Monitor = reader["MONITOR"] != DBNull.Value ? Convert.ToString(reader["MONITOR"]) : String.Empty
                                    });
                                }
                                else
                                {
                                    retorno.Add(new RelatorioAreaListModel
                                    {
                                        Habilidades = reader["HABILIDADE"] != DBNull.Value ? Convert.ToString(reader["HABILIDADE"]) : String.Empty,
                                        Equipe = reader["NOME_EQUIPE"] != DBNull.Value ? Convert.ToString(reader["NOME_EQUIPE"]) : String.Empty,
                                        Data_Criacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                        Usuario_Criacao = reader["USUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCRIACAO"]) : String.Empty,
                                        Matricula = reader["MATRICULA"] != DBNull.Value ? Convert.ToString(reader["MATRICULA"]) : String.Empty,
                                        Perfil = reader["PERFIL"] != DBNull.Value ? Convert.ToString(reader["PERFIL"]) : String.Empty,
                                        Supervisor = reader["SUPERVISOR"] != DBNull.Value ? Convert.ToString(reader["SUPERVISOR"]) : String.Empty,
                                        Monitor = reader["MONITOR"] != DBNull.Value ? Convert.ToString(reader["MONITOR"]) : String.Empty
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


            return new DataTableResponse<RelatorioAreaListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };

        }


        [HttpPost()]
        [Route("api/Areaapi/ConsultaAreaPorDescricao_Uf")]
        public async Task<GetApiRetorno<RelatorioAreaListModel>> ConsultaAreaPorDescricao_Uf([FromBody] ObjConsultaArea model)
        {
            var retorno = new GetApiRetorno<RelatorioAreaListModel>()
            {
                Objeto = new RelatorioAreaListModel()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(@$"
                        SELECT COD_AREA, COD_ESCRITORIO
                        FROM TB_AREA
                        WHERE UPPER(DESCRICAO) LIKE '%{model.Descricao.ToUpperInvariant()}%' AND
                                UF = '{model.UF}' AND 
                                STATUS = 1
                        ");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 1))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Area = reader["COD_AREA"] != DBNull.Value ? Convert.ToString(reader["COD_AREA"]) : String.Empty;
                                retorno.Objeto.Grupo_Area = reader["COD_ESCRITORIO"] != DBNull.Value ? Convert.ToString(reader["COD_ESCRITORIO"]) : String.Empty;
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(retorno.Objeto.Area) && !String.IsNullOrEmpty(retorno.Objeto.Grupo_Area))
                    {
                        using (var api = new LocalidadeApiController())
                        {
                            var retornoAPI = await api.ConsultaLocalidadePorBairro(new ObjConsultaLocalidade { Cod_Area = retorno.Objeto.Area, Cod_Grupo = retorno.Objeto.Grupo_Area, Bairro = model.Bairro });
                            retorno.Ok = retornoAPI.Ok;
                            if (retornoAPI.Ok)
                            {
                                retorno.Objeto.Cod_Localidade = !String.IsNullOrEmpty(retornoAPI.Objeto.Cod_Localidade) ? retornoAPI.Objeto.Cod_Localidade : String.Empty;
                            }
                        }
                    }
                    else
                    {
                        retorno.Ok = false;
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                }
            }

            return retorno;
        }
    }
}