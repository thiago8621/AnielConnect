using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ComboApiController : BaseApiController
    {
        /// <summary>
        /// Listagem de Combos do sistema incluindo planos e produtos vinculados ao combo.
        /// </summary>
        /// <param name="request">Objeto de request</param>

        [HttpPost]
        [Route("api/ComboApi/GetAll")]
        public async Task<ApiResponseObj<List<ComboListModel>>> GetAll(RequestCombo request)
        {
            List<ComboListModel> retorno = new List<ComboListModel>();

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
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID,A.DESCRICAO,A.STATUS,A.EMITE_CONTRATO,B.DESCRICAO AS MODELO_CONTRATO,B.TEM_CARENCIA,B.PERIODO_CARENCIA ");
                    sql.AppendLine(" FROM TB_COMBO A ");
                    sql.AppendLine(" LEFT JOIN TB_MODELO_CONTRATO B ");
                    sql.AppendLine(" ON(A.ID_MODELO_CONTRATO = B.ID) ");
                    sql.AppendLine(" WHERE 1 = 1 ");

                    if (request.Id > 0)
                    {
                        sql.AppendLine(" AND A.ID = @ID ");
                        parametros.Add("ID", request.Id);
                    }

                    if (!string.IsNullOrEmpty(request.CodigoIntegracao))
                    {
                        sql.AppendLine(" AND A.CODIGO_INTEGRACAO = @CODIGO_INTEGRACAO ");
                        parametros.Add("CODIGO_INTEGRACAO", request.CodigoIntegracao);
                    }


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            string IdCombo = Convert.ToString(reader["ID"]);

                            retorno.Add(new ComboListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                Status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])) : "",
                                EmiteContrato = reader["EMITE_CONTRATO"] != DBNull.Value ? Convert.ToBoolean(reader["EMITE_CONTRATO"]) : false,
                                ModeloContrato = reader["MODELO_CONTRATO"] != DBNull.Value ? Convert.ToString(reader["MODELO_CONTRATO"]) : "",
                                ModeloTemCarencia = reader["TEM_CARENCIA"] != DBNull.Value ? Convert.ToBoolean(reader["TEM_CARENCIA"]) : false,
                                PeriodoCarencia = reader["PERIODO_CARENCIA"] != DBNull.Value ? Convert.ToString(reader["PERIODO_CARENCIA"]) : ""
                            });

                            foreach (var dados in retorno)
                            {
                                List<PlanoProdutoComboList> planoProdutoCombo = await GetPlanoProdutoCombo(session, IdCombo);

                                dados.ComboPlanoProduto = planoProdutoCombo;
                            }
                        }
                    }
                }

                return new ApiResponseObj<List<ComboListModel>>
                {
                    data = retorno,
                    ok = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<ComboListModel>>
                {
                    error = ex.Message
                };
            }
        }


        private async Task<List<PlanoProdutoComboList>> GetPlanoProdutoCombo(ISession session, string IdCombo)
        {
            List<PlanoProdutoComboList> retorno = new List<PlanoProdutoComboList>();
           
            using (session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> param = new Dictionary<String, Object>();

                var com = session.Connection.CreateCommand();

                #region Sql

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT A.ID,A.ID_COMBO,A.VALOR,A.VALOR_INSTALACAO,A.COBRAR_INSTALACAO,B.TIPO_TECNOLOGIA,B.VALORINSTALACAO,B.VALORPLANO, ");

                sql.AppendLine(" (SELECT D.DESCRICAO FROM TB_PLANO D ");
                sql.AppendLine(" LEFT JOIN TB_PRODUTO E ");
                sql.AppendLine(" ON E.ID = A.ID_PLANO ");
                sql.AppendLine(" WHERE D.ID = A.ID_PLANO) AS PLANOPRODUTO ");

                sql.AppendLine(" FROM TB_COMBO_PLANO_PRODUTO A ");
                sql.AppendLine(" LEFT JOIN TB_PLANO B ");
                sql.AppendLine(" ON(B.ID = A.ID_PLANO) ");
                sql.AppendLine(" LEFT JOIN TB_PRODUTO C ");
                sql.AppendLine(" ON(C.ID = A.ID_PRODUTO) ");

                sql.AppendLine(" WHERE A.ID_COMBO = @ID ");

                param.Add("@ID", IdCombo);

                #endregion

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new PlanoProdutoComboList
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            PlanoProduto = Convert.ToString(reader["PLANOPRODUTO"]),
                            Tipo = HtmlEnumExtensions.GetEnumDescription<Tipo_Tecnologia>(Convert.ToString(reader["TIPO_TECNOLOGIA"])),
                            CobrarInstalacao = Convert.ToBoolean(reader["COBRAR_INSTALACAO"]),
                            ValorReal = Convert.ToString(reader["VALORPLANO"]),
                            ValorIntalacaoReal = Convert.ToString(reader["VALORINSTALACAO"]),
                            ValorNoCombo = Convert.ToString(reader["VALOR"]),
                            ValorInstalacaoCombo = Convert.ToString(reader["VALOR_INSTALACAO"])
                        });
                    }
                }
                return retorno;
            }           
        }



        /// <summary>
        /// Listagem de combos por id incluindo planos e produtos vinculados ao combo
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ComboApi/FindById")]
        public async Task<ApiResponseObj<List<ComboListModel>>> FindById(FindByIdComboApiRequest request)
        {
            List<ComboListModel> retorno = new List<ComboListModel>();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession session = HibernateUtil.GetSession())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID,A.DESCRICAO,A.STATUS,A.EMITE_CONTRATO,B.DESCRICAO AS MODELO_CONTRATO,B.TEM_CARENCIA,B.PERIODO_CARENCIA ");
                        sql.AppendLine(" FROM TB_COMBO A ");
                        sql.AppendLine(" LEFT JOIN TB_MODELO_CONTRATO B ");
                        sql.AppendLine(" ON(A.ID_MODELO_CONTRATO = B.ID) ");

                        parametros.Add("@ID", request.id);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                string IdCombo = Convert.ToString(reader["ID"]);

                                retorno.Add(new ComboListModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                    Status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])) : "",
                                    EmiteContrato = reader["EMITE_CONTRATO"] != DBNull.Value ? Convert.ToBoolean(reader["EMITE_CONTRATO"]) : false,
                                    ModeloContrato = reader["MODELO_CONTRATO"] != DBNull.Value ? Convert.ToString(reader["MODELO_CONTRATO"]) : "",
                                    ModeloTemCarencia = reader["TEM_CARENCIA"] != DBNull.Value ? Convert.ToBoolean(reader["TEM_CARENCIA"]) : false,
                                    PeriodoCarencia = reader["PERIODO_CARENCIA"] != DBNull.Value ? Convert.ToString(reader["PERIODO_CARENCIA"]) : ""
                                });

                                foreach (var dados in retorno)
                                {
                                    List<PlanoProdutoComboList> planoProdutoCombo = await GetPlanoProdutoCombo(session, IdCombo);

                                    dados.ComboPlanoProduto = planoProdutoCombo;
                                }
                            }
                        }                        
                    }
                }

                return new ApiResponseObj<List<ComboListModel>>
                {
                    data = retorno,
                    ok = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<ComboListModel>> { error = ex.Message };
            }
        }
    }
}