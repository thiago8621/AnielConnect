using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class CampanhaApiController : BaseApiController
    {
        /// <summary>
        /// Listagem API Campanha exibe as informações cadastradas e ativas.
        /// Incluindo os blocos relacionados a Equipe de Vendas, Regiões, Área de Cobertura e Planos e Produtos que estejam vinculadas em cada campanha.
        /// </summary>
        [HttpPost]
        [Route("api/campanhaApi/GetAll")]
        public async Task<ApiResponseObj<List<CampanhaListModel>>> GetAllAsync(CampanhaApiRequest request)
        {
            List<CampanhaListModel> retorno = new List<CampanhaListModel>();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<string> clausulasWhere = new List<string>();

                    var com = session.Connection.CreateCommand();
                    {
                        #region Sql

                        StringBuilder sql = new StringBuilder();

                        clausulasWhere.Add("A.ID <> @ID");
                        clausulasWhere.Add("A.STATUS = @STATUS");

                        parametros.Add("@ID", 0);
                        parametros.Add("@STATUS", Status_Default.Ativo);

                        if (!String.IsNullOrEmpty(request.PrefixoCupom))
                        {
                            clausulasWhere.Add("UPPER(A.PREFIXO_CUPOM) LIKE UPPER(@CUPOM)");
                            parametros.Add("@CUPOM", String.Format("%{0}%", StringService.Truncate(request.PrefixoCupom, 50)));
                        }

                        if (!String.IsNullOrEmpty(request.settings.search))
                        {
                            clausulasWhere.Add("(UPPER(A.TITULO) LIKE UPPER(@SEARCH))");
                            parametros.Add("@SEARCH", String.Format("%{0}%", StringService.Truncate(request.settings.search, 100)));
                        }

                        sql.AppendLine(" SELECT A.ID, A.TITULO, A.CAMP_TEMPO_DETERMINADO,A.DATA_INICIO,A.DATA_FINAL,A.STATUS");
                        sql.AppendLine(" FROM TB_CAMPANHA A ");
                        sql.AppendLine($" WHERE {String.Join(" \r\n AND ", clausulasWhere)}");

                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                string IdCampanha = Convert.ToString(reader["ID"]);

                                retorno.Add(new CampanhaListModel
                                {
                                    Codigo = Convert.ToInt64(reader["ID"]),
                                    Titulo = Convert.ToString(reader["TITULO"]),
                                    CampanhaTempoDeterminado = Convert.ToBoolean(reader["CAMP_TEMPO_DETERMINADO"]),
                                    Status = Convert.ToBoolean(reader["STATUS"]),
                                    DataInicio = Convert.ToString(reader["DATA_INICIO"]),
                                    DataFinal = Convert.ToString(reader["DATA_FINAL"]),
                                });   
                            }

                            
                        }

                        
                        
                    }


                    foreach (var dados in retorno)
                    {
                        List<EquipeVendaCampanhaList> equipes = await GetEquipeVendaCampanha(session, dados.Codigo);
                        List<RegioesCampanhaList> regioes = await GetRegioesCampanha(session, dados.Codigo);
                        List<AreaCoberturaCampanhaList> areaCobertura = await GetAreaCoberturaCampanha(session, dados.Codigo);
                        List<PlanoProdutoCampanhaList> planoProduto = await GetPlanoProdutoCampanha(session, dados.Codigo);

                        dados.EquipeVendaCampanha = equipes;
                        dados.RegioesCampanha = regioes;
                        dados.AreaCoberturaCampanha = areaCobertura;
                        dados.PlanoProdutoCampanha = planoProduto;
                    }

                    return new ApiResponseObj<List<CampanhaListModel>>
                    {
                        data = retorno,
                        ok = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<CampanhaListModel>>
                {
                    error = ex.Message,
                    ok = false
                };
            }
        }

        private async Task<List<EquipeVendaCampanhaList>> GetEquipeVendaCampanha(ISession session, Int64 IdCampanha)
        {
            List<EquipeVendaCampanhaList> retorno = new List<EquipeVendaCampanhaList>();
            try
            {

                Dictionary<String, Object> param = new Dictionary<String, Object>();

                using (var com = session.Connection.CreateCommand())
                {
                    #region Sql
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT A.ID,A.ID_CAMPANHA,A.SETOR_DEPARTAMENTO, A.META, B.NOME AS VENDEDOR, (SELECT FUNC.NOME FROM TB_FUNCIONARIO FUNC WHERE FUNC.Id = B.ID_SUPERVISOR) AS NOME_SUPERVISOR ");
                    sql.AppendLine(" FROM TB_CAMPANHA_EQUIPE_VENDAS A ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO B ");
                    sql.AppendLine(" ON(B.ID = A.ID_FUNCIONARIO) ");
                    sql.AppendLine(" WHERE A.ID_CAMPANHA = @ID ");

                    param.Add("@ID", IdCampanha);

                    #endregion

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new EquipeVendaCampanhaList
                            {
                                Id = Convert.ToInt32(reader["ID"]),
                                Setor = Convert.ToString(reader["SETOR_DEPARTAMENTO"]),
                                Vendedor = Convert.ToString(reader["VENDEDOR"]),
                                Supervisor = Convert.ToString(reader["NOME_SUPERVISOR"]),
                                CadastroMeta = Convert.ToString(reader["META"])
                            });
                        }
                    }
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<RegioesCampanhaList>> GetRegioesCampanha(ISession session, Int64 IdCampanha)
        {
            List<RegioesCampanhaList> retorno = new List<RegioesCampanhaList>();
            try
            {
                using (session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> param = new Dictionary<String, Object>();

                    var com = session.Connection.CreateCommand();

                    #region Sql

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT A.ID,A.PAIS,A.ID_CAMPANHA,A.CIDADE,A.UF ");
                    sql.AppendLine(" FROM TB_CAMPANHA_REGIOES A ");
                    sql.AppendLine(" WHERE A.ID_CAMPANHA = @ID ");

                    param.Add("@ID", IdCampanha);

                    #endregion

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new RegioesCampanhaList
                            {
                                Id = Convert.ToInt32(reader["ID"]),
                                Pais = Convert.ToString(reader["PAIS"]),
                                UF = Convert.ToString(reader["UF"]),
                                Cidade = Convert.ToString(reader["CIDADE"])
                            });
                        }
                    }
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<AreaCoberturaCampanhaList>> GetAreaCoberturaCampanha(ISession session, Int64 IdCampanha)
        {
            List<AreaCoberturaCampanhaList> retorno = new List<AreaCoberturaCampanhaList>();
            try
            {

                Dictionary<String, Object> param = new Dictionary<String, Object>();

                using (var com = session.Connection.CreateCommand())
                {
                    #region Sql
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT A.ID, A.AREA_COBERTURA, A.ID_CAMPANHA ");
                    sql.AppendLine(" FROM TB_CAMPANHA_AREA_COBERTURA A ");

                    sql.AppendLine(" WHERE A.ID_CAMPANHA = @ID ");

                    param.Add("@ID", IdCampanha);

                    #endregion

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new AreaCoberturaCampanhaList
                            {
                                Id = Convert.ToInt32(reader["ID"]),
                                AreaCobertura = Convert.ToString(reader["AREA_COBERTURA"])
                            });
                        }
                    }
                    return retorno;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<PlanoProdutoCampanhaList>> GetPlanoProdutoCampanha(ISession session, Int64 IdCampanha)
        {
            List<PlanoProdutoCampanhaList> retorno = new List<PlanoProdutoCampanhaList>();
            try
            {

                Dictionary<String, Object> param = new Dictionary<String, Object>();

                using (var com = session.Connection.CreateCommand())
                {

                    #region Sql

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID,A.ID_CAMPANHA ,A.N_NOME_PLANO_PRODUTO,A.VALOR_PLANO_PRODUTO,A.VALOR_INSTALACAO, ");
                    sql.AppendLine(" A.VALIDADE_PLANO_PRODUTO ,A.PARCELAS_INSTALACAO ,A.E_PLANO_PRODUTO ,A.DATA_VALIDADE,");
                    sql.AppendLine(" (SELECT D.DESCRICAO ");
                    sql.AppendLine(" FROM TB_PLANO D ");
                    sql.AppendLine(" LEFT JOIN TB_PRODUTO E ");
                    sql.AppendLine(" ON E.ID = A.ID_PLANO_PRODUTO ");
                    sql.AppendLine(" WHERE D.Id = A.ID_PLANO_PRODUTO OR E.ID = A.ID_PLANO_PRODUTO) AS PLANOPRODUTO ");

                    sql.AppendLine(" FROM TB_CAMPANHA_PLANO_PRODUTO A ");

                    sql.AppendLine(" WHERE A.ID_CAMPANHA = @ID ");

                    param.Add("@ID", IdCampanha);

                    #endregion

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new PlanoProdutoCampanhaList
                            {
                                Id = Convert.ToInt32(reader["ID"]),
                                NomeExibicao = Convert.ToString(reader["N_NOME_PLANO_PRODUTO"]),
                                PlanoProduto = Convert.ToString(reader["PLANOPRODUTO"]),
                                ValorPlanoProduto = Convert.ToString(reader["VALOR_PLANO_PRODUTO"]),
                                ValorInstalacao = Convert.ToString(reader["VALOR_INSTALACAO"]),
                                TemValidade = Convert.ToBoolean(reader["VALIDADE_PLANO_PRODUTO"]),
                                DataValidade = Convert.ToString(reader["DATA_VALIDADE"]),
                                NumeroParcelasInstalacao = Convert.ToInt32(reader["PARCELAS_INSTALACAO"])
                            });
                        }
                    }
                    return retorno;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}