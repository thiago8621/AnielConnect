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
    public class EstornoMaterialApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Estornos de Materiais Aniel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/EstornoMaterialApi/GetAll")]
        public async Task<ApiResponse<EstornoListaModel>> GetAll(Estorno_ApiRequest request)
        {
            List<EstornoListaModel> retorno = new List<EstornoListaModel>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }


                if (String.IsNullOrEmpty(request.DataFinal))
                {
                    throw new Exception("Data Inicial é obrigatório");
                }

                if (String.IsNullOrEmpty(request.DataFinal))
                {
                    throw new Exception("Data Final é obrigatório");
                }

                DateTime? Data_Final = Convert.ToDateTime(request.DataFinal);
                DateTime? Data_Inicial = Convert.ToDateTime(request.DataInicial);

                if ((Data_Final.Value - Data_Inicial.Value).TotalDays >= 90
                    || ((Data_Final.Value - Data_Inicial.Value).TotalDays < 0))
                {
                    throw new Exception("Informe um período dentro de 90 dias");
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    int recordsTotal = 0;

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(CODMAT) CO FROM TB_MOVIMENTO_ELIMINADO");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        #region Sql

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(@"SELECT A.CODCT, A.PROJETO, A.CODMAT, B.DESCRICAO, B.UNID, A.CODCPL, A.COD_CCUSTO,A.COD_FORNECEDOR,D.CNPJ, A.OPERACAO,
                                        A.SEQLIVRE, A.SEQRMDM, A.SEQTRANSF, A.PEDIDO, A.DATA, A.ENTRADA, A.SAIDA, A.OBSERVACAO, A.NOTAFISCAL, A.EQUIPE, C.NOME,
                                        E.TIPO_MOVIMENTO,A.MOTIVO_EXCLUSAO, A.USUARIO, A.DIA, A.HORA,CTORIGEM,PROJORIGEM FROM TB_MOVIMENTO_ELIMINADO A
                                        INNER JOIN TB_MATERIAL B
                                        ON(A.CODMAT = B.CODMAT)
                                        LEFT JOIN TB_EQUIPE C
                                        ON(A.EQUIPE = C.EQUIPE)
                                        LEFT JOIN TB_FORNECEDOR D
                                        ON(A.COD_FORNECEDOR = D.COD_FORNECEDOR)
                                        LEFT JOIN TB_SEQUENCIA_RMDM E
                                        ON(E.CODCT = A.CODCT AND E.PROJETO = A.PROJETO AND E.SEQRMDM = A.SEQRMDM)
                                        WHERE(A.DATA BETWEEN @PDATA1 AND @PDATA2)");

                        parametros.Add("@PDATA1", request.DataInicial);
                        parametros.Add("@PDATA2", request.DataFinal);

                        if (String.IsNullOrEmpty(request.ContratoAniel) == false)
                        {

                            sql.AppendLine(" AND A.CODCT = @CODCT ");
                            parametros.Add("@CODCT", request.ContratoAniel);
                        }
                        if (String.IsNullOrEmpty(request.ProjetoAniel) == false)
                        {
                            sql.AppendLine(" AND A.PROJETO = @PROJETO ");
                            parametros.Add("@PROJETO", request.ProjetoAniel);
                        }


                        #endregion

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new EstornoListaModel
                                {
                                    CodCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : null,
                                    Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : null,
                                    CodCtOrigem = reader["CTORIGEM"] != DBNull.Value ? Convert.ToString(reader["CTORIGEM"]) : null,
                                    ProjetoOrigem = reader["PROJORIGEM"] != DBNull.Value ? Convert.ToString(reader["PROJORIGEM"]) : null,
                                    CodMat = reader["CODMAT"] != DBNull.Value ? Convert.ToString(reader["CODMAT"]) : null,
                                    DescricaoMaterial = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : null,
                                    Pedido = reader["PEDIDO"] != DBNull.Value ? Convert.ToString(reader["PEDIDO"]) : null,
                                    CodigoComplementar = reader["CODCPL"] != DBNull.Value ? Convert.ToString(reader["CODCPL"]) : null,
                                    CodCentroCusto = reader["COD_CCUSTO"] != DBNull.Value ? Convert.ToString(reader["COD_CCUSTO"]) : null,
                                    CnpjFornecedor = reader["CNPJ"] != DBNull.Value ? Convert.ToString(reader["CNPJ"]) : null,
                                    Operacao = reader["OPERACAO"] != DBNull.Value ? Convert.ToString(reader["OPERACAO"]) : null,
                                    SeqLivre = reader["SEQLIVRE"] != DBNull.Value ? Convert.ToString(reader["SEQLIVRE"]) : null,
                                    SeqRmdm = reader["SEQRMDM"] != DBNull.Value ? Convert.ToString(reader["SEQRMDM"]) : null,
                                    SeqTransf = reader["SEQTRANSF"] != DBNull.Value ? Convert.ToString(reader["SEQTRANSF"]) : null,
                                    Data = reader["DATA"] != DBNull.Value ? Convert.ToString(reader["DATA"]) : null,
                                    Entrada = reader["ENTRADA"] != DBNull.Value ? Convert.ToString(reader["ENTRADA"]) : null,
                                    Saida = reader["SAIDA"] != DBNull.Value ? Convert.ToString(reader["SAIDA"]) : null,
                                    Observacao = reader["OBSERVACAO"] != DBNull.Value ? Convert.ToString(reader["OBSERVACAO"]) : null,
                                    NotaFiscal = reader["NOTAFISCAL"] != DBNull.Value ? Convert.ToString(reader["NOTAFISCAL"]) : null,
                                    MatriculaEquipe = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : null,
                                    NomeEquipe = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : null,
                                    MotivoExclusao = reader["MOTIVO_EXCLUSAO"] != DBNull.Value ? Convert.ToString(reader["MOTIVO_EXCLUSAO"]) : null,
                                    Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : null,
                                    Dia = reader["DIA"] != DBNull.Value ? Convert.ToString(reader["DIA"]) : null,
                                    Hora = reader["HORA"] != DBNull.Value ? Convert.ToString(reader["HORA"]) : null,
                                    Unidade = reader["UNID"] != DBNull.Value ? Convert.ToString(reader["UNID"]) : null,
                                    TipoMovimento = reader["TIPO_MOVIMENTO"] != DBNull.Value ? Convert.ToString(reader["TIPO_MOVIMENTO"]) : null,
                                    CodFornecedor = reader["COD_FORNECEDOR"] != DBNull.Value ? Convert.ToString(reader["COD_FORNECEDOR"]) : null
                                });
                            }
                        }
                    }

                    return new ApiResponse<EstornoListaModel>()
                    {
                        data = retorno,
                        draw = request.settings.length,
                        recordsFiltered = retorno.Count,
                        quantidadeTotalRegistros = recordsTotal,
                        error = "",
                    };
                }
            }
            catch (Exception e)
            {
                return new ApiResponse<EstornoListaModel>()
                {
                    error = e.Message
                };
            }
        }
    }
}