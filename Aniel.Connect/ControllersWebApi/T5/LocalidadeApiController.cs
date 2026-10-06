using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.Integracao;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Usuario;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using RestSharp;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class LocalidadeApiController : BaseApiController
    {


        [HttpGet()]
        [Route("api/Localidadeapi/GetList")]
        public async Task<GetApiRetorno<List<ObjBusca<String>>>> GetList(String grupo_area, String area)
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

                        sql.AppendLine("SELECT A.COD_LOCALIDADE,A.LOCALIDADE ");
                        sql.AppendLine("FROM TB_LOCALIDADE A ");
                        sql.AppendLine("WHERE A.COD_ESCRITORIO = @VALOR1 AND A.COD_AREA = @VALOR2 AND A.STATUS = 1");
                        sql.AppendLine("ORDER BY UPPER(A.LOCALIDADE)");
                        parametros.Add("@VALOR1", grupo_area);
                        parametros.Add("@VALOR2", area);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["COD_LOCALIDADE"]),
                                    Label = Convert.ToString(reader["LOCALIDADE"])
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
        [Route("api/LocalidadeApi/GetLogEquipeLocalidade")]
        public async Task<DataTableResponse<LogEquipeLocalidadeListaModel>> GetLogEquipeLocalidade(EquipeLocalidadeDataTableRequest request)
        {
            List<LogEquipeLocalidadeListaModel> retorno = new List<LogEquipeLocalidadeListaModel>();
            retorno = new List<LogEquipeLocalidadeListaModel>();

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


                        sql.AppendLine("SELECT A.DESCRICAO_ESCRITORIO, A.DESCRICAO_AREA, A.DESCRICAO_LOCALIDADE,A.NOME_EQUIPE,A.USUARIOCRIACAO,A.DATAHORACRIACAO,A.TIPOOPERACAO,A.TODAS_LOCALIDADES");
                        sql.AppendLine("FROM TB_LOG_EQUIPELOCALIDADE A");

                        if (!String.IsNullOrEmpty(request.Equipe))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" A.EQUIPE = @EQUIPE");
                            parametros.Add("@EQUIPE", request.Equipe);
                        }

                        if (!String.IsNullOrEmpty(request.Grupo_Area))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" A.COD_ESCRITORIO = @GRUPO_AREA");
                            parametros.Add("@GRUPO_AREA", request.Grupo_Area);
                        }

                        if (!String.IsNullOrEmpty(request.Area))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" A.COD_AREA = @AREA");
                            parametros.Add("@AREA", request.Area);
                        }

                        if (!String.IsNullOrEmpty(request.Localidade))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" A.DESCRICAO_LOCALIDADE = @LOCALIDADE");
                            parametros.Add("@LOCALIDADE", request.Localidade);
                        }


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new LogEquipeLocalidadeListaModel
                                {
                                    Grupo_Area = Convert.ToString(reader["DESCRICAO_ESCRITORIO"]),
                                    Area = Convert.ToString(reader["DESCRICAO_AREA"]),
                                    Localidade = Convert.ToBoolean(reader["TODAS_LOCALIDADES"]) ? Resources.RsBase.TODAS : Convert.ToString(reader["DESCRICAO_LOCALIDADE"]),
                                    Equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                    Data_Alteracao = Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm"),
                                    Tipo_Alteracao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                    Usuario_Alteracao = Convert.ToString(reader["USUARIOCRIACAO"]),
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
            return new DataTableResponse<LogEquipeLocalidadeListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/LocalidadeApi/RelatorioEquipeLocalidade")]
        public async Task<DataTableResponse<RelatorioLocalidadeListaModel>> RelatorioEquipeLocalidade(EquipeLocalidadeDataTableRequest request)
        {
            List<RelatorioLocalidadeListaModel> retorno = new List<RelatorioLocalidadeListaModel>();
            retorno = new List<RelatorioLocalidadeListaModel>();

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
                            sql.AppendLine(@"SELECT A.LOCALIDADE, B.DESCRICAO AS AREA, C.DESCRICAO AS ESCRITORIO, VINCULO.USUARIOCRIACAO,VINCULO.DATAHORACRIACAO,VINCULO.TODAS_LOCALIDADES, E.DESCRICAO AS PERFIL
                                         FROM TB_PERFIL_AGENDAMENTO E
                                         LEFT JOIN TB_PERFIL_AGENDAMENTO_AREA VINCULO
                                         ON(VINCULO.id_perfil_agendamento = E.ID)
                                         LEFT JOIN TB_LOCALIDADE A
                                         ON (A.localidade = VINCULO.localidade AND A.COD_AREA = VINCULO.COD_AREA AND A.COD_ESCRITORIO = VINCULO.COD_ESCRITORIO AND A.COD_CLIENTE = 1)
                                         LEFT JOIN TB_AREA B
                                         ON (B.COD_AREA = VINCULO.COD_AREA AND B.COD_ESCRITORIO = VINCULO.COD_ESCRITORIO AND B.COD_CLIENTE = 1)
                                         LEFT JOIN TB_ESCRITORIO C
                                         ON (C.COD_ESCRITORIO = VINCULO.COD_ESCRITORIO)
                                         WHERE (A.COD_CLIENTE = 1 OR VINCULO.TODAS_LOCALIDADES = 1)");
                        }
                        else
                        {

                            sql.AppendLine(@"SELECT VINCULO.USUARIOCRIACAO,VINCULO.DATAHORACRIACAO,E.DESCRICAO AS PERFIL,N.DESCRICAO AS HABILIDADE
                                         FROM TB_PERFIL_AGENDAMENTO E
                                         LEFT JOIN TB_PERFIL_AGENDAMENTO_NATUREZA VINCULO
                                         ON(VINCULO.id_perfil_agendamento = E.ID)
                                         LEFT JOIN TB_NATUREZA_PRODUCAO N
                                         ON(VINCULO.ID_NATUREZA = N.ID)");
                        }



                        if (!String.IsNullOrEmpty(request.Equipe))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" VINCULO.EQUIPE = @EQUIPE");
                            parametros.Add("@EQUIPE", request.Equipe);
                        }

                        if (!String.IsNullOrEmpty(request.Grupo_Area))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" VINCULO.COD_ESCRITORIO = @GRUPO_AREA");
                            parametros.Add("@GRUPO_AREA", request.Grupo_Area);
                        }

                        if (!String.IsNullOrEmpty(request.Area))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine(" VINCULO.COD_AREA = @AREA");
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
                            sql.AppendLine("PERFIL = @PERFIL");
                            parametros.Add("@PERFIL", request.Perfil);
                        }
                        if (!String.IsNullOrEmpty(request.Habilidades))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine("N.COD_NATUREZA = @HABILIDADES");
                            parametros.Add("@HABILIDADES", request.Habilidades);
                        }

                        sql.AppendLine(" ORDER BY E.DESCRICAO");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (request.TipoRelatorio == "area")
                                {
                                    bool todasLocalidades = reader["TODAS_LOCALIDADES"] != DBNull.Value ? Convert.ToBoolean(reader["TODAS_LOCALIDADES"]) : false;
                                    retorno.Add(new RelatorioLocalidadeListaModel
                                    {
                                        Grupo_Area = reader["ESCRITORIO"] != DBNull.Value ? Convert.ToString(reader["ESCRITORIO"]) : String.Empty,
                                        Area = reader["AREA"] != DBNull.Value ? Convert.ToString(reader["AREA"]) : String.Empty,
                                        Localidade = todasLocalidades ? RsBase.TODAS : reader["LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["LOCALIDADE"]) : String.Empty,
                                        Data_Criacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                        Usuario_Criacao = reader["USUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCRIACAO"]) : String.Empty,
                                        Perfil = reader["PERFIL"] != DBNull.Value ? Convert.ToString(reader["PERFIL"]) : String.Empty
                                    });
                                }
                                else
                                {
                                    retorno.Add(new RelatorioLocalidadeListaModel
                                    {
                                        Data_Criacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                        Usuario_Criacao = reader["USUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCRIACAO"]) : String.Empty,
                                        Perfil = reader["PERFIL"] != DBNull.Value ? Convert.ToString(reader["PERFIL"]) : String.Empty,
                                        Habilidades = reader["HABILIDADE"] != DBNull.Value ? Convert.ToString(reader["HABILIDADE"]) : String.Empty
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


            return new DataTableResponse<RelatorioLocalidadeListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/Localidadeapi/GetListFullLocaliade")]
        public async Task<GetApiRetorno<LocalidadeListaModel>> GetListFullLocaliade(String grupo_area, String area, Int32 localidade)
        {
            GetApiRetorno<LocalidadeListaModel> retorno = new GetApiRetorno<LocalidadeListaModel>();
            retorno.Objeto = new LocalidadeListaModel();

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


                        sql.AppendLine("SELECT C.DESCRICAO AS ESCRITORIO, B.DESCRICAO AS AREA, A.LOCALIDADE AS LOCALIDADE");
                        sql.AppendLine("FROM   TB_LOCALIDADE A");
                        sql.AppendLine("LEFT JOIN TB_AREA B");
                        sql.AppendLine("ON(A.COD_ESCRITORIO = B.COD_ESCRITORIO AND A.COD_AREA = B.COD_AREA)");
                        sql.AppendLine("LEFT JOIN TB_ESCRITORIO C");
                        sql.AppendLine("ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO)");
                        sql.AppendLine("WHERE A.COD_ESCRITORIO = @COD_ESCRITORIO AND A.COD_AREA = @COD_AREA AND A.COD_LOCALIDADE = @COD_LOCALIDADE");


                        parametros.Add("@COD_ESCRITORIO", grupo_area);
                        parametros.Add("@COD_AREA", area);
                        parametros.Add("@COD_LOCALIDADE", localidade);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto = new LocalidadeListaModel
                                {
                                    Grupo_Area = Convert.ToString(reader["ESCRITORIO"]),
                                    Area = Convert.ToString(reader["AREA"]),
                                    Localidade = Convert.ToString(reader["LOCALIDADE"]),

                                };
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
        [Route("api/Localidadeapi/GetCombo")]
        public async Task<ObjCombo<string>> GetCombo(RequestObjComboAvoConfig<string, string, string, PerfilVisualizacaoServicoComboConfig> request)
        {
            var retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                //Filtros 
                var parametros = new Dictionary<string, object>();

                var sqlFrom = new StringBuilder("FROM TB_LOCALIDADE A  ");

                var condicoes = new List<String>();

                if (request.soativos)
                {
                    condicoes.Add($"A.STATUS = 1");
                }

                if (!String.IsNullOrEmpty(request.id))
                {
                    condicoes.Add($"A.COD_LOCALIDADE = @COD_LOCALIDADE ");
                    parametros.Add("@COD_LOCALIDADE", request.id);
                }

                if (!String.IsNullOrEmpty(request.idpai))
                {
                    condicoes.Add($"A.COD_AREA = @COD_AREA ");
                    parametros.Add("@COD_AREA", request.idpai);
                }

                if (!String.IsNullOrEmpty(request.idavo))
                {
                    condicoes.Add($"A.COD_ESCRITORIO = @COD_ESCRITORIO");
                    parametros.Add("@COD_ESCRITORIO", request.idavo);
                }

                if (request != null && !String.IsNullOrEmpty(request.q))
                {
                    condicoes.Add($"(UPPER(A.LOCALIDADE) LIKE @SEARCH_LOCALIDADE)");
                    parametros.Add("@SEARCH_LOCALIDADE", String.Format("%{0}%", StringService.Truncate(request.q.ToUpperInvariant(), 100)));
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
                                .Where(p => p.CodLocalidade.HasValue)
                                .Count() > 0)
                        {
                            var listParamsGrupoArea = new List<string>();

                            foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea.Where(p => p.CodLocalidade.HasValue))
                            {
                                listParamsGrupoArea.Add($"A.COD_LOCALIDADE = '{paramArea.CodLocalidade.Value}'");
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
                    var sql = new StringBuilder("SELECT COUNT(A.LOCALIDADE) AS CO ");
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
                    var sql = new StringBuilder("SELECT A.COD_LOCALIDADE, A.LOCALIDADE, A.CODCT, A.PROJETO ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine("ORDER BY UPPER(A.LOCALIDADE)");

                    using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, sessionT5.Connection, null, request.page))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new ObjComboLocalidade<String>
                            {
                                Id = Convert.ToString(reader["COD_LOCALIDADE"]),
                                Text = Convert.ToString(reader["LOCALIDADE"]),
                                CodCt = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"])
                            };
                            retorno.results.Add(obj);
                        }
                    }
                }
            }

            return retorno;
        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/Localidadeapi/GetAll")]
        public async Task<DataTableResponse<Localidade_ListModel>> GetAll(Localidade_DataTableRequest request)
        {
            List<Localidade_ListModel> retorno = new List<Localidade_ListModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_LOCALIDADE A ")
                .AppendLine("LEFT JOIN TB_AREA B ON (B.COD_AREA = A.COD_AREA AND B.COD_ESCRITORIO = A.COD_ESCRITORIO) ")
                .AppendLine("LEFT JOIN TB_ESCRITORIO C ON C.COD_ESCRITORIO = B.COD_ESCRITORIO ")
                .AppendLine("LEFT JOIN TB_CONTRATO D ON D.CODCT = A.CODCT ");

                List<String> filtros = new List<string>();
                if (!String.IsNullOrEmpty(request.IdArea))
                {
                    filtros.Add("A.COD_AREA = @COD_AREA");
                    parametros.Add("@COD_AREA", request.IdArea);
                }

                if (!String.IsNullOrEmpty(request.IdGrupoArea))
                {
                    filtros.Add("A.COD_ESCRITORIO = @COD_ESCRITORIO");
                    parametros.Add("@COD_ESCRITORIO", request.IdGrupoArea);
                }

                if (!String.IsNullOrEmpty(request.IdContrato))
                {
                    filtros.Add("A.CODCT = @CODCT");
                    parametros.Add("@CODCT", request.IdContrato);
                }

                if (!String.IsNullOrEmpty(request.Projeto))
                {
                    filtros.Add("A.PROJETO = @PROJETO");
                    parametros.Add("@PROJETO", request.Projeto);
                }

                if (!String.IsNullOrEmpty(request.Id))
                {
                    filtros.Add("A.COD_LOCALIDADE = @COD_LOCALIDADE");
                    parametros.Add("@COD_LOCALIDADE", request.Id);
                }

                if (!String.IsNullOrEmpty(request.Status))
                {
                    filtros.Add("A.STATUS = @STATUS");
                    parametros.Add("@STATUS", request.Status);
                }

                if (request.VisualizarSemLocalizacao)
                {
                    filtros.Add("(A.LATITUDE IS NULL OR A.LONGITUDE IS NULL)");
                    parametros.Add("@STATUS", request.Status);
                }

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    filtros.Add("UPPER(A.LOCALIDADE) LIKE UPPER(@SEARCH)");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                if (filtros.Count > 0)
                {

                    sqlFrom.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                }


                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_LOCALIDADE) CO ");
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

                    sql.AppendLine("SELECT A.COD_LOCALIDADE, A.PROJETO, A.CODCT, D.DESCRICAO AS CONTRATO, B.COD_AREA, B.DESCRICAO AS AREADESC, C.COD_ESCRITORIO, C.DESCRICAO AS GRUPOAREADESC, A.STATUS, A.LOCALIDADE, A.PADRAO ");

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
                                        sql.AppendLine(" ORDER BY A.LOCALIDADE");
                                    else
                                        sql.AppendLine(" ORDER BY A.LOCALIDADE DESC");
                                    break;
                                }
                            case "area":     // area
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY B.DESCRICAO");
                                    else
                                        sql.AppendLine(" ORDER BY B.DESCRICAO DESC");
                                    break;
                                }
                            case "grupoArea":     // grupo area
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY C.DESCRICAO");
                                    else
                                        sql.AppendLine(" ORDER BY C.DESCRICAO DESC");
                                    break;
                                }
                            case "contrato":     // contrato
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY D.DESCRICAO");
                                    else
                                        sql.AppendLine(" ORDER BY D.DESCRICAO DESC");
                                    break;
                                }
                            case "projeto":     // projeto
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.PROJETO");
                                    else
                                        sql.AppendLine(" ORDER BY A.PROJETO DESC");
                                    break;
                                }
                            case "id":     // id
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.COD_LOCALIDADE");
                                    else
                                        sql.AppendLine(" ORDER BY A.COD_LOCALIDADE DESC");
                                    break;
                                }
                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Localidade_ListModel
                            {
                                Id = Convert.ToInt64(reader["COD_LOCALIDADE"]),
                                Descricao = Convert.ToString(reader["LOCALIDADE"]),
                                GrupoArea = Convert.ToString(reader["GRUPOAREADESC"]),
                                Status = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.ATIVO : RsCadastros.INATIVO,
                                IdGrupoArea = Convert.ToString(reader["COD_ESCRITORIO"]),
                                Contrato = Convert.ToString(reader["CONTRATO"]),
                                IdContrato = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"]),
                                Area = Convert.ToString(reader["AREADESC"]),
                                IdArea = Convert.ToString(reader["COD_AREA"]),
                                Padrao = reader["PADRAO"] != DBNull.Value ? (Convert.ToBoolean(reader["PADRAO"]) ? "Sim" : "Não") : RsBase.NAO
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Localidade_ListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/Localidadeapi/FindById")]
        public async Task<GetApiRetorno<Localidade_CadastroModel>> FindById(String Id)
        {
            GetApiRetorno<Localidade_CadastroModel> retorno = new GetApiRetorno<Localidade_CadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repositoryArea = new Conexao<AreaAniel>(session))
            using (var repositoryEscritorio = new Conexao<EscritorioAniel>(session))
            using (var repository = new Conexao<LocalidadeAniel>(session))

            {
                try
                {

                    String[] _chaves = Id.Split('|');
                    String IdGrupoArea = _chaves[0];
                    String IdArea = _chaves[1];
                    String Localidade = _chaves[2];

                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Cod_Escritorio == IdGrupoArea && p.Cod_Area == IdArea && p.LocalidadeStr == Localidade));

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

        private Localidade_CadastroModel GetViewObj(LocalidadeAniel obj)
        {
            return new Localidade_CadastroModel
            {
                Id = obj.Cod_Escritorio + "|" + obj.Cod_Area + "|" + obj.LocalidadeStr,
                Codigo = Convert.ToInt64(obj.Cod_Localidade),
                Descricao = Convert.ToString(obj.LocalidadeStr),
                Status = obj.Status == Status_Default.Ativo,
                IdArea = Convert.ToString(obj.Cod_Area),
                IdGrupoArea = Convert.ToString(obj.Cod_Escritorio),
                IdContrato = Convert.ToString(obj.CodCt),
                IdProjeto = Convert.ToString(obj.Projeto),
                IdContrato_Infra = Convert.ToString(obj.CodCt_Infra),
                IdProjeto_Infra = Convert.ToString(obj.Projeto_Infra),
                IdCadastro = Convert.ToString(obj.Cod_Localidade),
                PontoRoteamento = obj.PontoRoteamento,
                Latidude = Convert.ToString(obj.Latitude).Replace(",", "."),
                Longitude = Convert.ToString(obj.Longitude).Replace(",", "."),
                Padrao = obj.Padrao
            };
        }


        [HttpPost()]
        [Route("api/Localidadeapi/HistoricoAlteracao")]
        public async Task<DataTableResponse<LogLocalidade>> HistoricoAlteracao(HistoricoAlteracaoLocalidade_DataTableRequest request)
        {
            List<LogLocalidade> retorno = new List<LogLocalidade>();
            List<LogLocalidade> retornoLog = new List<LogLocalidade>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();


                sqlFrom.AppendLine("FROM TB_LOCALIDADE A ");

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
                    StringBuilder sql = new StringBuilder("SELECT COUNT(COD_LOCALIDADE) CO ");

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

                    sql.AppendLine("SELECT A.COD_LOCALIDADE, A.NOMEUSUARIOCRIACAO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.IDLOG, A.OBJETO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS ");
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
                            retornoLog.Add(new LogLocalidade
                            {
                                Id = Convert.ToInt64(reader["COD_LOCALIDADE"]),
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

                using ISession session = HibernateUtil.GetSessionT5();
                using (var repositoryLocalidade = new Conexao<LocalidadeAniel>(session))              
                {
                    ret.DescricaoLocalidade = (await repositoryLocalidade.selectFirst(p => p.Cod_Localidade == ret.Id))?.LocalidadeStr;
                }

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
                    if (ret.Campo == "Descrição Grupo Área")
                    {
                        using (var repositoryGrupoArea = new Conexao<EscritorioAniel>(session))
                        {
                            var antes = (await repositoryGrupoArea.selectFirst(p => p.Cod_Escritorio == ret.Campo_ValorAntes));
                            var depois = (await repositoryGrupoArea.selectFirst(p => p.Cod_Escritorio == ret.Campo_ValorDepois));

                            ret.Campo_ValorAntes = antes.Descricao;
                            ret.Campo_ValorDepois = depois.Descricao;
                        }
                    }

                    if (ret.Campo == "Descrição da Área")
                    {
                        using (var repositoryArea = new Conexao<AreaAniel>(session))
                        {
                            var antes = (await repositoryArea.selectFirst(p => p.Cod_Area == ret.Campo_ValorAntes));
                            var depois = (await repositoryArea.selectFirst(p => p.Cod_Area == ret.Campo_ValorDepois));

                            ret.Campo_ValorAntes = antes.Descricao;
                            ret.Campo_ValorDepois = depois.Descricao;
                        }
                    }

                    if (ret.Campo == "Contrato")
                    {
                        using (var repositoryContrato = new Conexao<ContratoAniel>(session))
                        {
                            var antes = (await repositoryContrato.selectFirst(p => p.CodCt == ret.Campo_ValorAntes));
                            var depois = (await repositoryContrato.selectFirst(p => p.CodCt == ret.Campo_ValorDepois));

                            ret.Campo_ValorAntes = antes.Descricao;
                            ret.Campo_ValorDepois = depois.Descricao;
                        }
                    }
                }

                retorno.Add(ret);
            }


            //Page e retornando DataTable
            return new DataTableResponse<LogLocalidade>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }


        [HttpPost()]
        [Route("api/Localidadeapi/Gravar")]
        public async Task<PostApiRetorno<string>> Gravar([FromBody] ParamWebApi<Localidade_CadastroModel> value)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();

            try
            {
                LocalidadeAniel modelo = null;
                LocalidadeAniel modeloAnterior = null;

                using ISession session = HibernateUtil.GetSessionT5();
                using ISession sessionUser = HibernateUtil.GetSession();
                using (var repositoryArea = new Conexao<AreaAniel>(session))
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryGrupoArea = new Conexao<EscritorioAniel>(session))
                using (var repository = new Conexao<LocalidadeAniel>(session))
                using (var repoUser = new Conexao<IdentityUser>(sessionUser))
                {
                    IdentityUser usuario = (await repoUser.selectFirst(p => p.Id == value.Usuario));
                    bool erro = false;
                    //verifica se já existe o ID solicitado para cadastro
                    bool num_repetido = (await repository.selectFirst(x => x.Cod_Localidade == value.Objeto.Codigo) != null);
                    bool desc_repetido = (await repository.selectFirst(x => x.LocalidadeStr == (value.Objeto.Descricao).ToString() && x.Cod_Escritorio == value.Objeto.IdGrupoArea
                    && x.Cod_Area == value.Objeto.IdArea && x.Projeto == value.Objeto.Projeto && x.CodCt == value.Objeto.IdContrato) != null);

                    if (num_repetido && String.IsNullOrEmpty(value.Objeto.IdCadastro))
                    {
                        retorno.Mensagem = "Já existe uma Localidade cadastrada no sistema com o código " + value.Objeto.Codigo + "!";
                        retorno.Ok = false;
                        erro = true;
                    }
                    else if (desc_repetido && String.IsNullOrEmpty(value.Objeto.IdCadastro))
                    {
                        retorno.Mensagem = "Já existe uma Localidade cadastrada no sistema com as informações informadas " + value.Objeto.Descricao + "!";
                        retorno.Ok = false;
                        erro = true;
                    }

                    if (value.Objeto.Padrao)
                    {
                        if (value.Objeto.Status == true || value.Objeto.Codigo == 0)
                        {

                            var checarPadrao = value.Objeto.Padrao ? (await repository.select(p => p.Padrao == true &&
                                                                                                   p.Cod_Area == value.Objeto.IdArea &&
                                                                                                   p.Cod_Escritorio == value.Objeto.IdGrupoArea.ToString()
                                                                                                   ))
                                                : null;

                            checarPadrao = checarPadrao.Where(p => p.Cod_Localidade != value.Objeto.Codigo).ToList();

                            if (checarPadrao != null)
                            {
                                if (checarPadrao.Count() > 0)
                                    throw new Exception("Existe localidade padrão já cadastrado para este tipo de horário.");
                            }
                        }
                    }

                    //Exibe erro
                    if (erro)
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                        return retorno;
                    }


                    if (value.Objeto.Codigo != 0)
                        modelo = (await repository.select(p => p.Cod_Localidade == value.Objeto.Codigo)).FirstOrDefault();

                    if (modelo != null)
                    {
                        modeloAnterior = (LocalidadeAniel)modelo.Clone();

                        if (modeloAnterior.Cod_Area != value.Objeto.IdArea || modeloAnterior.Cod_Escritorio != value.Objeto.IdGrupoArea)
                        {
                            await repository.Remove(modeloAnterior);

                            modelo = new LocalidadeAniel
                            {
                                Cod_Localidade = Convert.ToInt32(value.Objeto.Codigo),
                                Cod_Area = value.Objeto.IdArea,
                                Cod_Escritorio = value.Objeto.IdGrupoArea,

                            };
                            value.Objeto.Status = true;
                        }
                    }

                    if (modelo == null)
                    {
                        modelo = new LocalidadeAniel
                        {
                            Cod_Localidade = Convert.ToInt32(value.Objeto.Codigo),   
                        };
                        value.Objeto.Status = true;
                    }

                    modelo.LocalidadeStr = value.Objeto.Descricao;
                    modelo.Status = Status_Default.Ativo;
                    modelo.Projeto = value.Objeto.IdProjeto;
                    modelo.CodCt = value.Objeto.IdContrato;
                    modelo.Projeto_Infra = value.Objeto.IdProjeto_Infra;
                    modelo.CodCt_Infra = value.Objeto.IdContrato_Infra;
                    modelo.PontoRoteamento = value.Objeto.PontoRoteamento;
                    modelo.Padrao = value.Objeto.Padrao;

                    BindViewToObj(session, ref modelo, value.Objeto);

                    //  log
                    try
                    {
                        if (modeloAnterior != null)
                        {
                            using (var repositoryLogs = new Conexao<LogLocalidadeAniel>(sessionLog))
                            {
                                var logs = modelo.GeraLog(modeloAnterior, usuario.Id, usuario.Nome, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLogs.Save(logs);
                            }
                        }
                        else
                        {
                            using (var repositoryLogs = new Conexao<LogLocalidadeAniel>(sessionLog))
                            {
                                var logs = modelo.GeraLog(modeloAnterior, usuario.Id, usuario.Nome, Sinapse.ModelLog.TipoOperLog.Insercao);
                                await repositoryLogs.Save(logs);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        retorno.IdObjeto = value.Objeto.Id;
                        retorno.Mensagem = ex.Message;
                        if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    }

                    await repository.SaveOrUpdate(modelo);
                    retorno.IdObjeto = value.Objeto.Id;
                    retorno.Ok = true;

                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                if (ex.InnerException.Message.Contains("violation of PRIMARY or UNIQUE KEY")) retorno.Mensagem = "Já existe uma localidade cadastrada com os dados inseridos.";
            }
            return retorno;
        }
        private void BindViewToObj(ISession session, ref LocalidadeAniel objBanco, Localidade_CadastroModel view)
        {
            objBanco.Cod_Localidade = Convert.ToInt32(view.Codigo);
            objBanco.Cod_Escritorio = view.IdGrupoArea.ToString();
            objBanco.Cod_Area = view.IdArea;
            objBanco.CodCt = view.IdContrato;
            objBanco.Projeto = view.IdProjeto;    
            objBanco.CodCt_Infra = view.IdContrato_Infra;
            objBanco.Projeto_Infra = view.IdProjeto_Infra;
            objBanco.LocalidadeStr = view.Descricao;
            objBanco.PontoRoteamento = view.PontoRoteamento;
            objBanco.Latitude = String.IsNullOrEmpty(view.Latidude) ? (double?)null : Convert.ToDouble(view.Latidude, CultureInfo.InvariantCulture);
            objBanco.Longitude = String.IsNullOrEmpty(view.Longitude) ? (double?)null : Convert.ToDouble(view.Longitude, CultureInfo.InvariantCulture);
            objBanco.Status = view.Status == true ? Status_Default.Ativo : Status_Default.Inativo;
            objBanco.Padrao = view.Padrao;
        }

        [HttpPost()]
        [Route("api/Localidadeapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarLocalidadeStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                LocalidadeAniel modelo = null;
                LocalidadeAniel modeloAnterior = null;


                using ISession session = HibernateUtil.GetSessionT5();
                using ISession sessionUser = HibernateUtil.GetSession();
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<LocalidadeAniel>(session))
                {
                    IUsuarioRepository repoUsuario = new UsuarioRepository(sessionUser);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);
                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());


                    foreach (var item in value.ids)
                    {

                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        String[] _chaves = item.Split('|');
                        String IdGrupoArea = _chaves[0];
                        String IdArea = _chaves[1];
                        String Localidade = _chaves[2];

                        #region log
                        modelo = (await repository.select(p => p.LocalidadeStr == Localidade && p.Cod_Escritorio == IdGrupoArea && p.Cod_Area == IdArea)).FirstOrDefault();
                        if (modelo != null)
                        {
                            modeloAnterior = (LocalidadeAniel)modelo.Clone();
                            modelo.Status = (Status_Default)value.status;

                            if ((int)modeloAnterior.Status != value.status)
                            {
                                using (var repositoryLogs = new Conexao<LogLocalidadeAniel>(sessionLog))
                                {
                                    var logs = modelo.GeraLog(modeloAnterior, user.Id, user.UserName, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                    await repositoryLogs.Save(logs);
                                }

                            }
                        }
                        #endregion

                        using (var com = session.Connection.CreateCommand())
                        {
                            StringBuilder sql = new StringBuilder($"UPDATE TB_LOCALIDADE SET STATUS = {value.status} WHERE COD_ESCRITORIO = @COD_ESCRITORIO AND COD_AREA = @COD_AREA AND LOCALIDADE = @LOCALIDADE");
                            parametros.Add("@COD_ESCRITORIO", IdGrupoArea);
                            parametros.Add("@COD_AREA", IdArea);
                            parametros.Add("@LOCALIDADE", Localidade);

                            await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                        }
                       
                    }

                    retorno.Ok = true;
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
        [Route("api/Localidadeapi/BuscarGeocalizacao")]
        public async Task<PostApiRetorno<Int64>> BuscarGeocalizacao(BuscarGeolocalizacaoLocalidadeRequestModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repoArea = new Conexao<AreaAniel>(session))
                using (var repoLocalidade = new Conexao<LocalidadeAniel>(session))
                {
                    foreach (var item in value.ids)
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();


                        AreaAniel areaAniel = await repoArea.selectFirst(p => p.Cod_Escritorio == item.CodGrupoArea && p.Cod_Area == item.CodArea);
                        LocalidadeAniel localidadeAniel = await repoLocalidade.selectFirst(p => p.Cod_Escritorio == item.CodGrupoArea && p.Cod_Area == item.CodArea && p.LocalidadeStr == item.Localidade);

                        var retornoLats = GetLatitudeLongitude(areaAniel.Descricao, localidadeAniel.LocalidadeStr);

                        double latitude = 0;
                        double longitude = 0;

                        if (retornoLats.Count > 0)
                        {
                            Double.TryParse(retornoLats.FirstOrDefault()?.lat, NumberStyles.Any, CultureInfo.InvariantCulture, out latitude);
                            Double.TryParse(retornoLats.FirstOrDefault()?.lon, NumberStyles.Any, CultureInfo.InvariantCulture, out longitude);
                        }
                        else
                        {
                            var retornoLatsGoogle = GetEnderecoGoogle(areaAniel.Descricao, localidadeAniel.LocalidadeStr);
                            if (retornoLatsGoogle.Count > 0)
                            {
                                Double.TryParse(retornoLatsGoogle.FirstOrDefault()?.lat, NumberStyles.Any, CultureInfo.InvariantCulture, out latitude);
                                Double.TryParse(retornoLatsGoogle.FirstOrDefault()?.lon, NumberStyles.Any, CultureInfo.InvariantCulture, out longitude);
                            }
                        }

                        if (latitude != 0 && longitude != 0)
                        {
                            localidadeAniel.Latitude = latitude;
                            localidadeAniel.Longitude = longitude;
                            localidadeAniel.UsuarioAlteracao = value.usuario;
                            await repoLocalidade.Update(localidadeAniel);
                        }
                    }


                    retorno.Ok = true;

                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        public List<NominatimLatLong> GetLatitudeLongitude(string cidade, string bairro)
        {
            List<NominatimLatLong> retorno = new List<NominatimLatLong>();
            try
            {
                var client = new RestClient("https://nominatim.openstreetmap.org/search?format=json&polygon=1&addressdetails=1&city=" + cidade + "&q=" + bairro);
                var request = new RestRequest(Method.GET);
                request.AddHeader("postman-token", "d6a5b694-ec96-c2cd-ff0a-5e9efe83f981");
                request.AddHeader("cache-control", "no-cache");
                IRestResponse response = client.Execute(request);
                string responseStr = response.Content;

                List<NominatimLatLong> tmp = JsonService.DeserializeObject<List<NominatimLatLong>>(responseStr);
                tmp = tmp.Where(p => p.address?.city_district != null && StringService.RemoverAcentos(p.address.city_district.ToUpper()) == StringService.RemoverAcentos(cidade.ToUpper())).ToList();
                retorno = tmp;
            }
            catch
            {
                CONSTANTES.log4Net.Error($"Erro ao buscar localizacao na api Nominatim, Parametro usado: {cidade} {bairro}");
            }

            return retorno;
        }

        public static List<NominatimLatLong> GetEnderecoGoogle(string cidade, string bairro)
        {
            String responseStr = String.Empty;
            List<NominatimLatLong> retorno = new List<NominatimLatLong>();
            try
            {
                var client = new RestClient($"https://maps.googleapis.com/maps/api/geocode/json?key={CONSTANTES.TokenGoogleAPI}&address={bairro} {cidade}&region=BR");
                var request = new RestRequest(Method.GET);
                request.AddHeader("postman-token", "77d51ea4-16a5-e998-e1c2-5ad839e68cff");
                request.AddHeader("cache-control", "no-cache");
                IRestResponse response = client.Execute(request);

                responseStr = response.Content;


                RetornoGoogle tmp = JsonService.DeserializeObject<RetornoGoogle>(responseStr);

                if (tmp.status == "OK")
                {
                    foreach (EnderecoGoogle endGoogle in tmp.results)
                    {
                        var elementoCidade = endGoogle.address_components.FirstOrDefault(p => p.types.Contains("administrative_area_level_2"));
                        var elementoBairro = endGoogle.address_components.Select(p => p.long_name).FirstOrDefault(p => StringService.RemoverAcentos(p.ToUpper()).Contains(StringService.RemoverAcentos(bairro.ToUpper())));

                        if (elementoCidade == null || elementoBairro == null || StringService.RemoverAcentos(elementoCidade?.long_name.ToUpper()) != StringService.RemoverAcentos(cidade.ToUpper()))
                            continue;

                        retorno.Add(new NominatimLatLong
                        {
                            lat = endGoogle.geometry.location.lat,
                            lon = endGoogle.geometry.location.lng
                        });
                    }
                }

                return retorno;
            }
            catch
            {
                CONSTANTES.log4Net.Error($"Erro ao buscar localizacao na api google, Parametro usado: {cidade} {bairro}");
            }

            return retorno;
        }



        public async Task<List<ExcelServiceLogItem>> Importacao(Localidade_Importacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            try
            {
                using (NHibernate.ISession session = HibernateUtil.GetSessionT5())
                using (Conexao<LocalidadeAniel> repoLocalidade = new Conexao<LocalidadeAniel>(session))
                using (NHibernate.ISession sessionUser = HibernateUtil.GetSession())
                using (Conexao<AreaAniel> repoArea = new Conexao<AreaAniel>(session))
                using (Conexao<IdentityUser> repoUser = new Conexao<IdentityUser>(sessionUser))
                using (Conexao<EscritorioAniel> repoGrupoArea = new Conexao<EscritorioAniel>(session))
                {

                    IdentityUser usuario = (await repoUser.selectFirst(p => p.Id == User.Identity.GetUserId()));


                    var Localidade = await ImportacaoModeloBase(session, item, retorno);


                    /* if (retorno.Count() > 0)
                      foreach (var logGravar in retorno)
                      {
                          LocalidadeAniel modeloAnterior = new LocalidadeAniel();
                          LocalidadeAniel modelo = new LocalidadeAniel()
                          {
                              LocalidadeStr = logGravar.Chave + " - " + logGravar.Erro,
                              Cod_Localidade = Convert.ToInt32(logGravar.Chave),
                          };

                          using (ISession sessionLog = HibernateUtil.GetSessionLog())
                          using (ISession sessionS = HibernateUtil.GetSession())
                          using (var repositoryLogs = new Conexao<LogLocalidadeAniel>(sessionLog))
                          using (var repository = new Conexao<LocalidadeAniel>(sessionS))
                          {
                              var logs = modelo.GeraLog(modeloAnterior, usuario.Nome, usuario.Id, Sinapse.ModelLog.TipoOperLog.Importacao);

                              await repositoryLogs.Save(logs);
                          }
                      }*/



                    if (retorno.Count() > 0)
                        return retorno;

                    List<LocalidadeAniel> total = (await repoLocalidade.select(p => p.Cod_Localidade == Localidade.Cod_Localidade)).ToList();


                    await repoLocalidade.Save(Localidade);
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo,
                        Erro = "Inserido com sucesso!"
                    });

                    using (ISession sessionLog = HibernateUtil.GetSessionLog())
                    using (var repositoryLogs = new Conexao<LogLocalidadeAniel>(sessionLog))
                    {
                        LocalidadeAniel modeloAnterior = new LocalidadeAniel();
                        LocalidadeAniel modelo = new LocalidadeAniel()
                        {
                            LocalidadeStr = item.Codigo + " - " + "Inserido com sucesso!",
                            Cod_Localidade = Convert.ToInt32(item.Codigo),
                        };

                        var logs = modelo.GeraLog(modeloAnterior, usuario.Nome, usuario.Id, Sinapse.ModelLog.TipoOperLog.Importacao);
                        await repositoryLogs.Save(logs);
                    }

                }
            }
            catch
            {
                // Try-Catch necessário para registrar as mensagens de erros no retorno
            }

            return retorno;
        }


        public async Task<LocalidadeAniel> ImportacaoModeloBase(ISession session, Localidade_Importacao item, List<ExcelServiceLogItem> retorno)
        {

            using (Conexao<AreaAniel> repoArea = new Conexao<AreaAniel>(session))
            using (Conexao<EscritorioAniel> repoEscritorio = new Conexao<EscritorioAniel>(session))
            using (Conexao<LocalidadeAniel> repoLocalidade = new Conexao<LocalidadeAniel>(session))
            using (Conexao<ContratoAniel> repoContrato = new Conexao<ContratoAniel>(session))
            using (Conexao<ProjetoAniel> repoProjeto = new Conexao<ProjetoAniel>(session))
            using (Grupo_AreaAnielApiController clienteApi = new Grupo_AreaAnielApiController())
            {
                //CRIANDO AQUI NO METODO AS VALIDAÇÔES QUE ESTAVAM NO IMPORTACAO PARA EVITAR DUPLICIDADE
                EscritorioAniel grupoArea = null;
                AreaAniel area = null;


                // 1 - Valida Se já esta cadastrado a Localidade;
                if ((await repoLocalidade.selectFirst(p => p.Cod_Localidade == Convert.ToInt32(item.Codigo))) != null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = Convert.ToString(item.Codigo),
                        Erro = "A Identificação Código Localidade pertence a um outra Localidade criada. Localidade não foi importado."
                    });


                // 2 - Valida Grupo Area já esta Cadastrado;

                if (!String.IsNullOrEmpty(item.Grupo_Area))
                {
                    //Valida se é Codigo ou Descrição
                    if(item.Grupo_Area.Count() <= 4)
                    grupoArea = (await repoEscritorio.selectFirst(p => p.Cod_Escritorio == item.Grupo_Area));
                    
                    if(grupoArea == null)
                        grupoArea = (await repoEscritorio.selectFirst(p => p.Descricao == item.Grupo_Area));
                    
                    //Valida se Grupo Area está cadastrado no sistema
                    if (grupoArea == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Codigo, Erro = "Código do Grupo Área não encontrado no sistema" });

                    //Verifica se o Status do Grupo Área é Ativo
                    if (grupoArea != null && grupoArea.Status == Status_Default.Inativo)
                    {
                        retorno.Add(new ExcelServiceLogItem
                        {
                            Chave = item.Codigo,
                            Erro = "Grupo Área informado Inativo!"
                        });
                    }
                }
                else
                {
                    retorno.Add(new ExcelServiceLogItem { Chave = item.Codigo, Erro = "Erro na leitura do campo Grupo Área!" });
                }


                // 3 - Valida se Area já Cadastrada;
                if (!String.IsNullOrEmpty(item.Area))
                {
                    //Valida se é o Codigo ou Descricao;
                    if(item.Area.Count() <= 4)
                        area = await repoArea.selectFirst(p => p.Cod_Area == item.Area);
                    if(area == null)  
                        area = (await repoArea.selectFirst(p => p.Descricao == item.Area));
                                        
                    // Valida se Area esta cadastrado no sistema
                    if (area == null)                    
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Codigo, Erro = "Código de Área não encontrado no sistema" });                    

                    // Valida se Grupo e Area são compativeis;
                    if (area != null && grupoArea != null)
                    {
                        var areaGrupo = await repoArea.selectFirst(p => p.Cod_Area == area.Cod_Area && p.Cod_Escritorio == grupoArea.Cod_Escritorio);
                        if (areaGrupo == null)
                        {
                            retorno.Add(new ExcelServiceLogItem
                            {
                                Chave = item.Codigo,
                                Erro = "Código de Área não é compatível com o Código de Grupo Área. Por favor, verifique as informações."
                            });
                        }
                    }

                    //Verifica se o Status da Área é Ativo

                    if (area != null && area.Status == Status_Default.Inativo)
                    {
                        retorno.Add(new ExcelServiceLogItem
                        {
                            Chave = item.Codigo,
                            Erro = "Área informada Inativo!"
                        });
                    }


                }
                else
                {
                    retorno.Add(new ExcelServiceLogItem { Chave = item.Codigo, Erro = "Erro na leitura do campo Área!" });
                }

                // 4 - Valida Contrato
                if (!String.IsNullOrEmpty(item.Contrato))
                {
                    var contrato = await repoContrato.selectFirst(p => p.CodCt == item.Contrato);
                    if (contrato == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Codigo, Erro = "Código de Contrato não encontrado no sistema" });
                }

                // 5 - Valida Projeto
                if (!String.IsNullOrEmpty(item.Projeto))
                {
                    var projeto = await repoProjeto.selectFirst(p => p.Codigo == item.Projeto);
                    var contratoProjeto = await repoProjeto.selectFirst(p => p.Codigo == item.Projeto && p.CodCt == item.Contrato);
                    if (projeto == null)
                    {
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Codigo, Erro = "Código de Projeto não encontrado no sistema!" });
                    }
                    if (contratoProjeto == null)
                    {
                        retorno.Add(new ExcelServiceLogItem
                        {
                            Chave = item.Codigo,
                            Erro = "Código de Projeto não é compatível com o Código de Contrato. Por favor, verifique as informações."
                        });
                    }
                }

                //Erro na leitura do Campo Codigo
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


                LocalidadeAniel Localidade = new LocalidadeAniel
                {
                    Status = Status_Default.Ativo,

                    Cod_Escritorio = grupoArea?.Cod_Escritorio,
                    Cod_Area = area?.Cod_Area,
                    Cod_Localidade = Convert.ToInt32(item.Codigo),
                    CodCt = item.Contrato,
                    Projeto = item.Projeto,
                    LocalidadeStr = item.Descricao,
                    Cod_Cliente = "1",
                };
                return Localidade;
            }



        }


        [HttpPost()]
        [Route("api/Localidadeapi/GetComboHabilidadesTecnico")]
        //  public async Task<ObjCombo<String>> GetComboHabilidadesTecnico(RequestObjCombo<String> request)
        public async Task<ObjCombo<String>> GetComboHabilidadesTecnico(RequestObjComboAvo<String, String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_NATUREZA_PRODUCAO A  ");


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
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
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

        [HttpPost()]
        [Route("api/Localidadeapi/ConsultaLocalidadePorBairro")]
        public async Task<GetApiRetorno<ObjConsultaLocalidade>> ConsultaLocalidadePorBairro(ObjConsultaLocalidade model)
        {
            var retorno = new GetApiRetorno<ObjConsultaLocalidade>()
            {
                Objeto = new ObjConsultaLocalidade()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sql = new StringBuilder(@$"
                    SELECT COD_ESCRITORIO, COD_AREA, COD_LOCALIDADE 
                    FROM TB_LOCALIDADE
                    WHERE COD_ESCRITORIO = '{model.Cod_Grupo}' AND 
                            COD_AREA = '{model.Cod_Area}' AND 
                            UPPER(LOCALIDADE) LIKE '%{model.Bairro.ToUpperInvariant()}%' AND
                            STATUS = 1
                    ");

                    using (var com = session.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 1))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Cod_Grupo = reader["COD_ESCRITORIO"] != DBNull.Value ? Convert.ToString(reader["COD_ESCRITORIO"]) : String.Empty;
                                retorno.Objeto.Cod_Area = reader["COD_AREA"] != DBNull.Value ? Convert.ToString(reader["COD_AREA"]) : String.Empty;
                                retorno.Objeto.Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["COD_LOCALIDADE"]) : String.Empty;
                            }
                        }
                    }

                    retorno.Ok = true;
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