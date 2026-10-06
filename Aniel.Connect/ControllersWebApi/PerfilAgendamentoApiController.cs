using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Resources;
using RestSharp.Extensions;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.ModelLog;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class PerfilAgendamentoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/PerfilAgendamentoApi/GetAll")]
        public async Task<DataTableResponse<PerfilAgendamentoListaModel>> GetAll(PerfilAgendamentoDatatableRequest request)
        {
            List<PerfilAgendamentoListaModel> retorno = new List<PerfilAgendamentoListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    sql.AppendLine(" SELECT COUNT(A.ID) AS CO");
                    sql.AppendLine(" FROM TB_PERFIL_AGENDAMENTO A ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    sql.AppendLine(" SELECT A.ID,A.STATUS,A.DESCRICAO, ");
                    sql.AppendLine(HtmlEnumExtensions.getEnumDescriptionInSql<Status_Default>("A.STATUS", "STATUS_ORDENAR"));
                    sql.AppendLine(" FROM TB_PERFIL_AGENDAMENTO A ");

                    List<String> filtros = new List<string>();

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
                        sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }




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
                                        sql.AppendLine("ORDER BY STATUS_ORDENAR");
                                    else
                                        sql.AppendLine("ORDER BY STATUS_ORDENAR DESC");
                                    break;
                                }
                            case "descricao":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.DESCRICAO");
                                    else
                                        sql.AppendLine("ORDER BY A.DESCRICAO DESC");
                                    break;
                                }
                            case "id":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.ID");
                                    else
                                        sql.AppendLine("ORDER BY A.ID DESC");
                                    break;
                                }
                        }
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new PerfilAgendamentoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription((Status_Default)Convert.ToInt32(reader["STATUS"])),

                            });
                        }
                    }
                }
            }
            return new DataTableResponse<PerfilAgendamentoListaModel>
            {
                data = retorno,
                recordsFiltered = recordsTotal,
                draw = request.draw
            };
        }

        [HttpPost()]
        [Route("api/PerfilAgendamentoapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PERFIL_AGENDAMENTO A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
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
                                    Text = $"{Convert.ToString(reader["DESCRICAO"])}"
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
        [Route("api/PerfilAgendamentoapi/GetComboParametrosServico")]
        public async Task<ObjCombo<Int64>> GetComboParametrosServico(RequestObjComboConfig<Int64?, Painel_ReteirizacaoDoc_Agrupado> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PERFIL_AGENDAMENTO A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 1");
                    }

                    if (!String.IsNullOrEmpty(request.config.Cod_Escritorio))
                    {
                        parametros.Add("@ESCRITORIO", request.config.Cod_Escritorio);
                        if (!String.IsNullOrEmpty(request.config.Cod_Area))
                        {
                            parametros.Add("@AREA", request.config.Cod_Area);
                            if (!String.IsNullOrEmpty(request.config.LocalidadeStr))
                            {
                                parametros.Add("@LOCALIDADE", request.config.LocalidadeStr);
                                condicoes.Add(@$"EXISTS  (SELECT B.ID FROM TB_PERFIL_AGENDAMENTO_AREA B
                                                      WHERE B.ID_PERFIL_AGENDAMENTO = A.ID AND
                                                      B.COD_ESCRITORIO = @ESCRITORIO AND B.COD_AREA = @AREA AND (B.LOCALIDADE = @LOCALIDADE OR B.TODAS_LOCALIDADES = 1))");
                            }
                            else
                            {
                                condicoes.Add(@$"EXISTS  (SELECT B.ID FROM TB_PERFIL_AGENDAMENTO_AREA B
                                                      WHERE B.ID_PERFIL_AGENDAMENTO = A.ID AND
                                                      B.COD_ESCRITORIO = @ESCRITORIO AND B.COD_AREA = @AREA)");
                            }
                        }
                        else
                        {
                            condicoes.Add(@$"EXISTS  (SELECT B.ID FROM TB_PERFIL_AGENDAMENTO_AREA B
                                                      WHERE B.ID_PERFIL_AGENDAMENTO = A.ID AND
                                                      B.COD_ESCRITORIO = @ESCRITORIO)");
                        }
                    }

                    if (request.config.Cod_Natureza.HasValue)
                    {
                        parametros.Add("@NATUREZA", request.config.Cod_Natureza);
                        condicoes.Add(@$"EXISTS  (SELECT B.ID FROM TB_PERFIL_AGENDAMENTO_NATUREZA B
                                                      WHERE B.ID_PERFIL_AGENDAMENTO = A.ID AND
                                                      B.ID_NATUREZA = @NATUREZA)");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
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
                                    Text = $"{Convert.ToString(reader["DESCRICAO"])}"
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
        [Route("api/PerfilAgendamentoApi/FindById")]
        public async Task<GetApiRetorno<PerfilAgendamentoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<PerfilAgendamentoCadastroModel> retorno = new GetApiRetorno<PerfilAgendamentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<PerfilAgendamento>(session))
            {
                try
                {
                    PerfilAgendamento perfil = await repository.selectFirst(p => p.Id == Id);

                    retorno.Objeto = GetViewObj(session, perfil);
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

        private PerfilAgendamentoCadastroModel GetViewObj(ISession session, PerfilAgendamento obj)
        {
            using (var repoPerfilArea = new Conexao<PerfilAgendamentoArea>(session))
            using (var repoPerfilNatureza = new Conexao<PerfilAgendamentoNatureza>(session))
            using (var repoEscritorio = new Conexao<EscritorioAniel>(session))
            using (var repoArea = new Conexao<AreaAniel>(session))
            using (var repoLocalidade = new Conexao<LocalidadeAniel>(session))
            {
                var retorno = new PerfilAgendamentoCadastroModel();

                retorno.Id = obj.Id;
                retorno.Descricao = obj.Descricao;
                retorno.Status = obj.Status == Status_Default.Ativo ? true : false;
                retorno.Areas = repoPerfilArea.select(p => p.PerfilAgendamento.Id == obj.Id).Result.Select(p => new AreasListaModel
                {
                    Id = (int)p.Id,
                    Cod_Area = p.Area,
                    Cod_Escritorio = p.Escritorio,
                    Nivel = p.Nivel,
                    GrupoAreaStr = repoEscritorio.selectFirst(y => y.Cod_Escritorio == p.Escritorio).Result?.Descricao,
                    AreaStr = repoArea.selectFirst(y => y.Cod_Area == p.Area && y.Cod_Escritorio == p.Escritorio).Result?.Descricao,
                    LocalidadeStr = repoLocalidade.selectFirst(y => y.LocalidadeStr == p.Localidade && y.Cod_Area == p.Area && y.Cod_Escritorio == p.Escritorio).Result?.LocalidadeStr,
                    Todas_Localidades = p.Todas_Localidades,
                    Todas_Areas = p.Todas_Areas,
                }).ToList();
                
                retorno.Naturezas = repoPerfilNatureza.select(p => p.PerfilAgendamento.Id == obj.Id).Result.Select(p => new NaturezasListaModel
                {
                    Id = (int)p.Id,
                    Id_Natureza = p.Natureza?.Id,
                    Descricao = p.Natureza?.Descricao,
                    Departamento = p.Natureza?.Departamento
                }).ToList();


                return retorno;
            }
        }

        [Route("api/PerfilAgendamentoApi/ExcluirGridArea")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridFiltro(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoEscritorio = new Conexao<EscritorioAniel>(session))
                using (var repoArea = new Conexao<AreaAniel>(session))
                using (var repositoryLogs = new Conexao<LogPerfilAgendamentoAniel>(sessionLog))
                {
                    using (var repository = new Conexao<PerfilAgendamentoArea>(session))
                    {


                        PerfilAgendamentoArea modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            var logs = new List<LogPerfilAgendamentoAniel>();
                            logs.Add(new LogPerfilAgendamentoAniel
                            {
                                Id = 0,
                                Campo = RsCadastros.GRUPO_AREA,
                                TipoOperacao = TipoOperLog.Exclusao,
                                IdLog = modelo.PerfilAgendamento.Id,
                                Chave = modelo.PerfilAgendamento.Id.ToString(),
                                UsuarioCriacao = User.Identity.GetUserId(),
                                NomeUsuarioCriacao = User.Identity.GetUserName(),
                                Campo_ValorAntes = modelo.Escritorio,
                                Campo_ValorDepois = modelo.Escritorio,
                                GrupoArea = repoEscritorio.selectFirst(y => y.Cod_Escritorio == modelo.Escritorio).Result?.Descricao,
                                Area = repoArea.selectFirst(y => y.Cod_Area == modelo.Area && y.Cod_Escritorio == modelo.Escritorio).Result?.Descricao,
                                Localidade = modelo.Localidade,
                                DataHoraCriacao = DateTime.Now,
                                Descricao = modelo.PerfilAgendamento?.Descricao,

                            });

                            await repository.Remove(modelo);
                            await repositoryLogs.Save(logs);

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

        [Route("api/PerfilAgendamentoApi/ExcluirGridNatureza")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridNatureza(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                {
                    using (var repository = new Conexao<PerfilAgendamentoNatureza>(session))
                    using (var repositoryLogs = new Conexao<LogPerfilAgendamentoAniel>(sessionLog))
                    {

                        PerfilAgendamentoNatureza modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            var logs = new List<LogPerfilAgendamentoAniel>();
                            logs.Add(new LogPerfilAgendamentoAniel
                            {
                                Id = 0,
                                IdLog = modelo.PerfilAgendamento.Id,
                                Campo = RsCadastros.NATUREZA,
                                TipoOperacao = TipoOperLog.Exclusao,
                                Chave = modelo.PerfilAgendamento.Id.ToString(),
                                UsuarioCriacao = User.Identity.GetUserId(),
                                NomeUsuarioCriacao = User.Identity.GetUserName(),
                                Campo_ValorAntes = modelo.Natureza.Descricao,
                                Campo_ValorDepois = modelo.Natureza.Descricao,
                                DataHoraCriacao = DateTime.Now,
                                Descricao = modelo.PerfilAgendamento?.Descricao,
                                Natureza = modelo.Natureza?.Descricao

                            });

                            await repository.Remove(modelo);
                            await repositoryLogs.Save(logs);


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
        [Route("api/PerfilAgendamentoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<PerfilAgendamentoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            PerfilAgendamento modelo = null;
            PerfilAgendamento modeloAnterior = null;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (ITransaction transaction = sessionT5.BeginTransaction())
            {
                sessionT5.FlushMode = FlushMode.Commit;
                try
                {
                    using (var repoNatureza = new Conexao<Natureza_ProducaoAniel>(sessionT5))
                    using (var repoPerfilAgendamentoArea = new Conexao<PerfilAgendamentoArea>(sessionT5))
                    using (var repoPerfilAgendamentoNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5))
                    using (var repository = new Conexao<PerfilAgendamento>(sessionT5))
                    using (var repoDpto = new Conexao<FolhaDeptoAniel>(sessionT5))
                    {

                        if (value.Objeto.Id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();
                        }


                        if (modelo == null)
                        {
                            modelo = new PerfilAgendamento
                            {
                                UsuarioCriacao = value.Usuario,
                                Descricao = value.Objeto.Descricao,
                                Naturezas = new List<PerfilAgendamentoNatureza>(),
                                Areas = new List<PerfilAgendamentoArea>(),
                                Status = Status_Default.Ativo
                            };
                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                            modeloAnterior = (PerfilAgendamento)modelo.Clone();

                        }

                        List<PerfilAgendamentoNatureza> Naturezas = (await repoPerfilAgendamentoNatureza.select(p => p.PerfilAgendamento.Id == value.Objeto.Id)).ToList();
                        List<PerfilAgendamentoArea> Areas = (await repoPerfilAgendamentoArea.select(p => p.PerfilAgendamento.Id == value.Objeto.Id)).ToList();

                        #region preenchimento Naturezas

                        var naturezasNovasView = value.Objeto.Naturezas.Where(p => p.Id == 0).ToList();
                        foreach (var novo in naturezasNovasView) 
                        {
                            var natureza = new PerfilAgendamentoNatureza();
                            natureza.Id = novo.Id;
                            natureza.PerfilAgendamento = modelo;
                            natureza.Natureza = repoNatureza.selectFirst(y => y.Id == novo.Id_Natureza).Result;
							natureza.Departamento = repoDpto.selectFirst(y=>y.Descricao == novo.Departamento).Result;
                            
                            Naturezas.Add(natureza);
                        }

                        #endregion

                        #region preenchimento Areas/Localidades
                        var areasNovasView = value.Objeto.Areas.Where(p => p.Id == 0).ToList();
                        foreach (var novo in areasNovasView) 
                        {
                            var area = new PerfilAgendamentoArea();
                            
                            area.Id = novo.Id;
                            area.PerfilAgendamento = modelo;
                            area.Escritorio = novo.Cod_Escritorio;
                            area.Area = novo.Cod_Area;
                            area.Localidade = novo.LocalidadeStr;
                            area.Nivel = novo.Nivel.GetValueOrDefault(0);
                            area.Todas_Localidades = string.IsNullOrEmpty(novo.LocalidadeStr) ? true : false;
                            area.Todas_Areas = string.IsNullOrEmpty(novo.AreaStr) ? true : false;

                            Areas.Add(area);
                        }
                        #endregion
                        modelo.Naturezas = Naturezas;
                        modelo.Areas = Areas;

                        BindViewToObj(sessionT5, modelo, value.Objeto);

                        //log
                        try
                        {
                            if (modeloAnterior != null)
                            {
                                using (var repositoryLogs = new Conexao<LogPerfilAgendamentoAniel>(sessionLog))
                                {

                                    var logs = modelo.GeraLog(modeloAnterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao, sessionT5);

                                    await repositoryLogs.Save(logs);
                                }
                            }
                            else
                            {
                                using (var repositoryLogs = new Conexao<LogPerfilAgendamentoAniel>(sessionLog))
                                {
                                    var logs = modelo.GeraLog(modeloAnterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Insercao, sessionT5);
                                    await repositoryLogs.Save(logs);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            retorno.IdObjeto = 0;
                            retorno.Mensagem = ex.Message;
                            if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                        }//fimlog                        


                        modelo.Naturezas = value.Objeto.Naturezas.Where(p => p.Id == 0).Select(p => new PerfilAgendamentoNatureza
                        {
                            Id = p.Id,
                            PerfilAgendamento = modelo,
                            Natureza = repoNatureza.selectFirst(y => y.Id == p.Id_Natureza).Result,
                            Departamento = repoDpto.selectFirst(y => y.Descricao == p.Departamento).Result
                        }).ToList();

                        modelo.Areas = value.Objeto.Areas.Where(p => p.Id == 0).Select(p => new PerfilAgendamentoArea
                        {
                            Id = p.Id,
                            PerfilAgendamento = modelo,
                            Escritorio = p.Cod_Escritorio,
                            Area = p.Cod_Area,
                            Localidade = p.LocalidadeStr,
                            Nivel = p.Nivel.GetValueOrDefault(0),
                            Todas_Localidades = string.IsNullOrEmpty(p.LocalidadeStr) ? true : false,
                            Todas_Areas = string.IsNullOrEmpty(p.AreaStr) ? true : false

                        }).ToList();

                        await repository.SaveOrUpdate(modelo);
                        await repoPerfilAgendamentoArea.SaveOrUpdate(modelo.Areas.ToList());
                        await repoPerfilAgendamentoNatureza.SaveOrUpdate(modelo.Naturezas.ToList());
                    }

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retorno.IdObjeto = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        private void BindViewToObj(ISession session, PerfilAgendamento objBanco, PerfilAgendamentoCadastroModel view)
        {
           objBanco.Descricao = view.Descricao;
           
        }

        [HttpPost()]
        [Route("api/PerfilAgendamentoApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PERFIL_AGENDAMENTO SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        [HttpPost]
        [Route("api/PerfilAgendamentoApi/VincularPerfilGetAll")]
        public async Task<DataTableResponse<VincularPerfilAgendamentoListaModel>> VincularPerfilGetAll(PerfilAgendamentoDatatableRequest request)
        {
            List<VincularPerfilAgendamentoListaModel> retorno = new List<VincularPerfilAgendamentoListaModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                bool filtros = false;
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(@" SELECT B.EQUIPE,B.NOME,SENHA.USUARIO, C.DESCRICAO 
                                 FROM TB_SENHA SENHA
                                 INNER JOIN TB_FUNCIONARIO A ON(SENHA.NUM_FRE = A.NUM_FRE)
                                 INNER JOIN TB_EQUIPE B ON(B.NUM_FRE = A.NUM_FRE)
                                 LEFT JOIN TB_PERFIL_AGENDAMENTO C ON(B.ID_PERFIL_AGENDAMENTO = C.ID)
                                 WHERE B.EQUIPE_CONNECT = 'S'
                                 AND A.SITUACAO <> 2 ");
                #region Filtros

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sql.AppendLine(" AND ((UPPER(B.EQUIPE || B.NOME) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                if (request.Configurados)
                {
                    sql.AppendLine("AND");
                    sql.AppendLine($"C.DESCRICAO IS NOT NULL");
                    filtros = true;
                }

                if (request.NaoConfigurados)
                {
                    sql.AppendLine("AND");
                    sql.AppendLine($"C.DESCRICAO IS NULL");
                    filtros = true;
                }

                if (request.PerfilSearch.HasValue)
                {
                    sql.AppendLine("AND");
                    sql.AppendLine($" C.DESCRICAO LIKE '%{request.Perfil}%' ");
                    filtros = true;
                }

                #endregion

                if (request.order != null && request.order.Any())
                {
                    int sortColumnIndex = request.order[0].column;
                    var coluna = request.columns[sortColumnIndex].data;

                    string sortDirection = request.order[0].dir;
                }

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new VincularPerfilAgendamentoListaModel
                        {
                            Equipe = Convert.ToString(reader["EQUIPE"]),
                            NomeEquipe = Convert.ToString(reader["NOME"]),
                            PerfilAgendamento = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "Nenhum",
                            Usuario = Convert.ToString(reader["USUARIO"]),
                        });
                    }
                }
            }

            return new DataTableResponse<VincularPerfilAgendamentoListaModel>
            {
                data = retorno
            };
        }

        [Route("api/PerfilAgendamentoApi/VincularPerfil")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> VincularPerfil(VincularPerfilRequestModel value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                {
                    using (var repository = new Conexao<EquipeAniel>(session))
                    using (var repositoryLog = new Conexao<LogVincularPerfilAniel>(sessionLog))
                    using (var repositoryPerfil = new Conexao<PerfilAgendamento>(session))
                    {
                        EquipeAniel modeloAnterior = null;

                        PerfilAgendamento perfil = await repositoryPerfil.selectFirst(p => p.Id == value.id_perfil);

                        foreach (var item in value.equipes)
                        {
                            if (String.IsNullOrEmpty(item) == false)
                            {
                                EquipeAniel modelo = (await repository.select(p => p.Matricula == item)).FirstOrDefault();

                                modeloAnterior = (EquipeAniel)modelo.Clone();

                                modelo.PerfilAgendamento = perfil;

                                var tipoOperacao = modeloAnterior.PerfilAgendamento != null ? TipoOperLog.Alteracao : TipoOperLog.Insercao;

                                var logs = modelo.GeraLog(modeloAnterior, User.Identity.GetUserId(), modelo.Nome, tipoOperacao);

                                await repository.Update(modelo);
                                await repositoryLog.Save(logs);

                            }
                            else
                            {
                                throw new Exception(Resources.RsBase.NAOENCONTRADO);
                            }
                        }

                    }
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/PerfilAgendamentoApi/RemoverPerfil")]
        [HttpPost()]
        public async Task<PostApiRetorno<Int64>> RemoverPerfil(VincularPerfilRequestModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                {
                    using (var repository = new Conexao<EquipeAniel>(session))
                    using (var repositoryLog = new Conexao<LogVincularPerfilAniel>(sessionLog))
                    using (var repositoryPerfil = new Conexao<PerfilAgendamento>(session))
                    {

                        foreach (var item in value.equipes)
                        {
                            EquipeAniel modelo = null;
                            EquipeAniel modeloAnterior = null;
                            PerfilAgendamento perfil = await repositoryPerfil.selectFirst(p => p.Id == value.id_perfil);

                            if (String.IsNullOrEmpty(item) == false)
                            {
                                modelo = (await repository.select(p => p.Matricula == item)).FirstOrDefault();
                                modeloAnterior = (EquipeAniel)modelo.Clone();
                                modelo.PerfilAgendamento = null;

                                try
                                {
                                    var usuarioAlteracao = User.Identity.GetUserId();

                                    var logs = modelo.GeraLog(modeloAnterior, usuarioAlteracao, modelo.Nome, Sinapse.ModelLog.TipoOperLog.Exclusao);

                                    await repository.Update(modelo);
                                    await repositoryLog.Save(logs);

                                }
                                catch (Exception ex)
                                {
                                    retorno.Mensagem = ex.Message;
                                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                                }

                            }
                            else
                            {
                                throw new Exception(Resources.RsBase.NAOENCONTRADO);
                            }
                        }


                    }
                }

                retorno.Ok = true;
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
        [Route("api/PerfilAgendamentoApi/GetLogVincularPerfil")]
        public async Task<DataTableResponse<LogVincularPerfilListaModel>> GetLogVincularPerfil(LogVincularPerfilListaModel request)
        {
            List<LogVincularPerfilListaModel> retorno = new List<LogVincularPerfilListaModel>();
            retorno = new List<LogVincularPerfilListaModel>();

            using (ISession session = HibernateUtil.GetSessionLog())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();


                        sql.AppendLine("SELECT A.NOMEUSUARIOCRIACAO AS TECNICO, A.USUARIOCRIACAO, A.TIPOOPERACAO,A.CAMPO_VALORANTES,A.CAMPO_VALORDEPOIS, A.DATAHORACRIACAO");
                        sql.AppendLine("FROM TB_LOG_VINCULAR_PERFIL A");

                        //filtros
                        if (!String.IsNullOrEmpty(request.Tecnico))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" A.EQUIPE = @EQUIPE");
                            parametros.Add("@EQUIPE", request.Tecnico);
                        }

                        if (!String.IsNullOrEmpty(Convert.ToString(request.DataHoraAlteracao)))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" A.COD_ESCRITORIO = @GRUPO_AREA");
                            parametros.Add("@GRUPO_AREA", request.DataHoraAlteracao);
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var log = new LogVincularPerfilListaModel
                                {
                                    Tecnico = reader["TECNICO"] != DBNull.Value ? Convert.ToString(reader["TECNICO"]) : "N/D",
                                    UsuarioAlteracao = reader["USUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCRIACAO"]) : "N/D",
                                    DataHoraAlteracao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : "N/D",
                                    TipoAlteracao = reader["TIPOOPERACAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])) : "N/D",
                                    ValorAnterior = reader["CAMPO_VALORANTES"] != DBNull.Value ? Convert.ToString(reader["CAMPO_VALORANTES"]) : "N/D",
                                    ValorAtual = reader["CAMPO_VALORDEPOIS"] != DBNull.Value ? Convert.ToString(reader["CAMPO_VALORDEPOIS"]) : "N/D"
                                };

                                if (log.TipoAlteracao == "Inserção")
                                {
                                    log.ValorAnterior = "N/D";
                                    log.ValorAtual = log.ValorAtual;
                                }

                                if (log.TipoAlteracao == "Exclusão")
                                {
                                    log.ValorAnterior = log.ValorAnterior;
                                    log.ValorAtual = "N/D";
                                }
                                retorno.Add(log);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new DataTableResponse<LogVincularPerfilListaModel>
            {
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/PerfilAgendamentoApi/GetLogPerfilAgendamento")]
        public async Task<DataTableResponse<PerfilAgendamentoListaModel>> GetLogPerfilAgendamento(PerfilAgendamentoListaModel request)
        {
            List<PerfilAgendamentoListaModel> retorno = new List<PerfilAgendamentoListaModel>();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSessionLog())
            using (var repoPerfilAgendamento = new Conexao<PerfilAgendamento>(sessionT5))
            {
                try
                {
                    var perfis = await repoPerfilAgendamento.select();
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT LOG.DESCRICAO, LOG.GRUPOAREA, LOG.LOCALIDADE, LOG.AREA, LOG.NATUREZA, " +
                           "LOG.USUARIOCRIACAO, LOG.TIPOOPERACAO, LOG.DATAHORACRIACAO, LOG.CAMPO_VALORDEPOIS, LOG.IDLOG");
                        sql.AppendLine(" FROM TB_LOG_PERFIL_AGENDAMENTO LOG ");

                        //filtros
                        if (!String.IsNullOrEmpty(request.Descricao))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" LOG.DESCRICAO = @PERFIL");
                            parametros.Add("@PERFIL", request.Descricao);
                        }

                        if (!String.IsNullOrEmpty(request.GrupoArea))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" LOG.GRUPOAREA = @GRUPOAREA");
                            parametros.Add("@GRUPOAREA", request.GrupoArea);
                        }
                        if (!String.IsNullOrEmpty(request.Area))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" LOG.AREA = @AREA");
                            parametros.Add("@AREA", request.Area);
                        }
                        if (!String.IsNullOrEmpty(request.Localidade))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" LOG.LOCALIDADE = @LOCALIDADE");
                            parametros.Add("@LOCALIDADE", request.Localidade);
                        }
                        if (!String.IsNullOrEmpty(request.Natureza))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" LOG.NATUREZA = @NATUREZA");
                            parametros.Add("@NATUREZA", request.Natureza);
                        }
                        if (!String.IsNullOrEmpty(request.PeriodoInicio) && !String.IsNullOrEmpty(request.PeriodoFim))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" LOG.DATAHORACRIACAO BETWEEN @DATA_INICIO AND @DATA_FIM");
                            parametros.Add("@DATA_INICIO", request.PeriodoInicio);
                            parametros.Add("@DATA_FIM", request.PeriodoFim);
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                //var idLog = reader["IDLOG"] != DBNull.Value ? Convert.ToInt64(reader["IDLOG"]) : 0;
                                //var perfilalteracao = perfis.FirstOrDefault(p => p.Id == idLog)?.Descricao;
                                var log = new PerfilAgendamentoListaModel
                                {
                                    Perfil = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "N/D",
                                    Tipo_Alteracao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                    PeriodoInicio = Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm"),
                                    PeriodoFim = Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm"),
                                    Usuario_Alteracao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                    Natureza = reader["NATUREZA"] != DBNull.Value ? Convert.ToString(reader["NATUREZA"]) : "N/D",
                                    GrupoArea = reader["GRUPOAREA"] != DBNull.Value ? Convert.ToString(reader["GRUPOAREA"]) : "N/D",
                                    Area = reader["AREA"] != DBNull.Value ? Convert.ToString(reader["AREA"]) : "N/D",
                                    Localidade = reader["LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["LOCALIDADE"]) : "N/D"
                                };
                                retorno.Add(log);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new DataTableResponse<PerfilAgendamentoListaModel>
            {
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };

        }

    }
}
