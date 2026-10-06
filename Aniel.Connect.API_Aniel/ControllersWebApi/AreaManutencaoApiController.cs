using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.API_Aniel.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System.Text;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class AreaApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de aplicações e remoções lançadas em RDOs ou OSs no sistema Aniel.
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/AreaApi/GetAll")]
        public async Task<ApiResponseObj<List<AreaListModel>>> GetAll(AreaManutencao_ApiRequest request)
        {
            List<AreaListModel> retorno = new List<AreaListModel>();


            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    
                    StringBuilder sqlWhere = new StringBuilder();
                    
                    sqlWhere.AppendLine("WHERE A.DATA_APLIC BETWEEN @DATA_INI AND @DATA_FIM AND(A.QUANTIDADE> 0 or A.QUANT_REMOV > 0)");
                    parametros.Add("@DATA_INI", request.Data_Ini);
                    parametros.Add("@DATA_FIM", request.Data_Fim);
                    if (!String.IsNullOrEmpty(request.Projeto)) { 
                        sqlWhere.Append("AND A.PROJETO = @PROJETO ");
                        parametros.Add("@PROJETO", request.Projeto);
                    }
                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        sqlWhere.Append("AND A.CODCT = @CONTRATO ");
                        parametros.Add("@CONTRATO", request.Contrato);
                    }

                    StringBuilder sqlFrom = new StringBuilder();
                        
                    sqlFrom.AppendLine(@" 
                        FROM TB_MATERIAL_TAREFA A
                        INNER JOIN TB_MATERIAL C
                        ON(A.CODMAT = C.CODMAT)");

                    #region //Select

                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(@"
                       SELECT A.CODCT,A.PROJETO, A.NUM_PROTOCOLO, A.NUM_DOC, A.EQUIPE, C.CODCLI,
                        A.CODMAT, C.DESCRICAO, C.UNID, A.DATA_APLIC, C.VALOR, C.CLASSIFICACAO, A.NUM_DOC, A.NUM_OBRA, A.DIA, A.DATA_APLIC,
                        A.QUANTIDADE, A.QUANT_REMOV, A.QUANT_CATAL,C.CODAUX, C.CODMAT_AUX

                       ");

                        sql.AppendLine(sqlFrom.ToString() );
                        sql.AppendLine(sqlWhere.ToString() );

                        #endregion

                        #region lidando com os erros

                        if (String.IsNullOrEmpty(request.Data_Ini))
                        {
                            throw new Exception("Data Inicial é obrigatório");
                        }
                        if (String.IsNullOrEmpty(request.Data_Fim))
                        {
                            throw new Exception("Data Final é obrigatório");
                        }

                        DateTime? Data_Ini = Convert.ToDateTime(request.Data_Ini);
                        DateTime? Data_Fim = Convert.ToDateTime(request.Data_Fim);

                        if ((Data_Fim.Value - Data_Ini.Value).TotalDays >= 90
                         || ((Data_Fim.Value - Data_Ini.Value).TotalDays < 0))
                        {
                            throw new Exception("Informe um período dentro de 90 dias");
                        }
                      
                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new AreaListModel
                                {
                                    CodCpl_Aux = !String.IsNullOrEmpty(Convert.ToString(reader["CODMAT_AUX"])) ? Convert.ToString(reader["CODMAT_AUX"]) : "-" ,
                                    CodCpl = !String.IsNullOrEmpty(Convert.ToString(reader["CODAUX"])) ? Convert.ToString(reader["CODAUX"]) : "-",
                                    Contrato = !String.IsNullOrEmpty(Convert.ToString(reader["CODCT"])) ? Convert.ToString(reader["CODCT"]) : "-",
                                    Cod_Material = !String.IsNullOrEmpty(Convert.ToString(reader["CODMAT"])) ? Convert.ToString(reader["CODMAT"]) : "-",
                                    Contrato_Assinante = !String.IsNullOrEmpty(Convert.ToString(reader["CODCLI"])) ? Convert.ToString(reader["CODCLI"]) : "-",
                                    DataHora_Agendamento = !String.IsNullOrEmpty(Convert.ToString(reader["DIA"])) ? Convert.ToString(reader["DIA"]) : "-",
                                    DataHora_Aplicacao = !String.IsNullOrEmpty(Convert.ToString(reader["DATA_APLIC"])) ? Convert.ToString(reader["DATA_APLIC"]) : "-",
                                    Numero_OS = !String.IsNullOrEmpty(Convert.ToString(reader["NUM_DOC"])) ? Convert.ToString(reader["NUM_DOC"]) : "-",
                                    Equipe = !String.IsNullOrEmpty(Convert.ToString(reader["EQUIPE"])) ? Convert.ToString(reader["EQUIPE"]) : "-",
                                    Número_da_Obra = !String.IsNullOrEmpty(Convert.ToString(reader["NUM_OBRA"])) ? Convert.ToString(reader["NUM_OBRA"]) : "-",
                                    Projeto = !String.IsNullOrEmpty(Convert.ToString(reader["PROJETO"])) ? Convert.ToString(reader["PROJETO"]) : "-",
                                    Quantidade_Aplic = !String.IsNullOrEmpty(Convert.ToString(reader["QUANTIDADE"])) ? Convert.ToString(reader["QUANTIDADE"]) : "-",
                                    Quantidade_Removida = !String.IsNullOrEmpty(Convert.ToString(reader["QUANT_REMOV"])) ? Convert.ToString(reader["QUANT_REMOV"]) : "-",
                                    RDO = !String.IsNullOrEmpty(Convert.ToString(reader["NUM_PROTOCOLO"])) ? Convert.ToString(reader["NUM_PROTOCOLO"]) : "-"

                                });
                            }
                        }

                    }

                    return new ApiResponseObj<List<AreaListModel>>() {
                       
                        data = retorno,
                        error = ""
                    };
                }
                catch (Exception e)
                {
                    return new ApiResponseObj<List<AreaListModel>>()
                    {
                        
                        data = retorno,
                        error = e.Message
                    };
                }
            }
        }
    }
}