using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ConsumoProtheusApiController : BaseApiController
    {
        /// <summary>
        /// Retorna todos os registros das Notas registradas no banco do Aniel 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ConsumoProtheusApi/GetAll")]
        public async Task<ApiResponse<ConsumoProtheusResponse>> GetAll(ConsumoProtheusRequest request)
        {
            List<ConsumoProtheusResponse> retorno = new List<ConsumoProtheusResponse>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.CODCPL, A.CODCT, A.EQUIPE, A.IMPORTADO, A.ESTORNO, A.CODMAT, ");
                    sql.AppendLine(" A.PROJETO, A.SEQRMDM, A.QUANTIDADE, A.DATAHORA_IMPORTACAO, A.NOME_EQUIPE,A.CODMAT,A.QUANTIDADE,A.TIPO_DOC,A.RETORNO ");
                    sql.AppendLine(" FROM TB_CONSUMO_PROTHEUS A ");
                    sql.AppendLine(" WHERE A.TIPO_DOC IN ('RM','DM') ");

                    if (request != null)
                    {
                        if (request.DataInicio > DateTime.MinValue && request.DataFim > DateTime.MinValue && request.DataFim >= request.DataInicio)
                        {
                            sql.AppendLine(" AND CAST(A.DATAHORA_IMPORTACAO AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                            parametros.Add("@DATAINICIO", request.DataInicio.ToString("dd-MM-yyyy"));
                            parametros.Add("@DATAFIM", request.DataFim.ToString("dd-MM-yyyy "));
                        }

                        if (!string.IsNullOrEmpty(request.CodSequencialRMDM))
                        {
                            sql.AppendLine(" AND A.SEQRMDM = @SEQRMDM ");
                            parametros.Add("@SEQRMDM", request.CodSequencialRMDM);
                        }
                    }

                    sql.AppendLine(" GROUP BY A.ID, A.CODCPL, A.CODCT, A.EQUIPE, A.IMPORTADO, A.ESTORNO, A.CODMAT, ");
                    sql.AppendLine(" A.PROJETO, A.SEQRMDM, A.QUANTIDADE, A.DATAHORA_IMPORTACAO, A.NOME_EQUIPE,A.CODMAT,A.QUANTIDADE,A.TIPO_DOC,A.RETORNO ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ConsumoProtheusResponse
                            {
                                Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : (long?)null,
                                CodComplemento = reader["CODCPL"] != DBNull.Value ? Convert.ToString(reader["CODCPL"]) : "",
                                Equipe = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : "",
                                CodContrato = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : "",
                                Importado = reader["IMPORTADO"] != DBNull.Value ? Convert.ToString(reader["IMPORTADO"]) : "",
                                Estorno = reader["ESTORNO"] != DBNull.Value ? Convert.ToString(reader["ESTORNO"]) : "",
                                Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : "",
                                SequenciaRMDM = reader["SEQRMDM"] != DBNull.Value ? Convert.ToString(reader["SEQRMDM"]) : "",
                                Quantidade = reader["QUANTIDADE"] != DBNull.Value ? Convert.ToDouble(reader["QUANTIDADE"]) : (double?)null,
                                DataImportacao = reader["DATAHORA_IMPORTACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_IMPORTACAO"]).ToString() : "",
                                NomeEquipe = reader["NOME_EQUIPE"] != DBNull.Value ? Convert.ToString(reader["NOME_EQUIPE"]) : "",
                                CodMaterial = reader["CODMAT"] != DBNull.Value ? Convert.ToString(reader["CODMAT"]) : "",
                                QTD = reader["QUANTIDADE"] != DBNull.Value ? Convert.ToString(reader["QUANTIDADE"]) : "",
                                Tipo_Doc = reader["TIPO_DOC"] != DBNull.Value ? Convert.ToString(reader["TIPO_DOC"]) : "",
                                Retorno = reader["RETORNO"] != DBNull.Value ? Convert.ToString(reader["RETORNO"]) : "",

                            });
                        }
                    }
                }

                return new ApiResponse<ConsumoProtheusResponse>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<ConsumoProtheusResponse>()
                {
                    error = e.Message,
                };
            }
        }      
    }
}