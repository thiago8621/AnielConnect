using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class TransferenciaApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Transferencias entre Projetos
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/TransferenciaApi/GetAllProjeto")]
        public async Task<ApiResponse<TransferenciaListaModel>> GetAllProjeto(Transferencia_ApiRequest request)
        {


            List<TransferenciaListaModel> retorno = new List<TransferenciaListaModel>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                DateTime? Data_Inicial = Convert.ToDateTime(request.DataInicial);
                DateTime? Data_Final = Convert.ToDateTime(request.DataFinal);

                if (!Data_Inicial.HasValue)
                {
                    throw new Exception("Data Inicial é obrigatório");
                }

                if (!Data_Final.HasValue)
                {
                    throw new Exception("Data Final é obrigatório");
                }

                if ((Data_Final.Value - Data_Inicial.Value).TotalDays >= 90
                    || ((Data_Final.Value - Data_Inicial.Value).TotalDays < 0))
                {
                    throw new Exception("Informe um período dentro de 90 dias");
                }

                int recordsTotal = 0;

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(CODCT) CO FROM TB_SEQUENCIAL_TRANSFERE");

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
                        sql.AppendLine(@" SELECT A.CODCT CODCT_DESTINO,A.PROJETO PROJETO_DESTINO,C.CODCT_ORIGEM,C.PROJETO_ORIGEM, A.SEQTRANSF, A.DATA, A.NUM_DOC_EMPRESA, A.EMPRESA,
                                           A.CFOP, A.ID_CONFIG_NFE, C.CODMAT, D.DESCRICAO AS MATERIAL, D.UNID, C.CODCPL, C.COD_CCUSTO, G.DESCRICAO AS DESC_CCUSTO, C.QUANTIDADE,C.USUARIO_CRIACAO,C.DIA_CRIACAO,C.HORA_CRIACAO,F.LOCALIZACAO,
                                G.DESCRICAO AS DESC_CCUSTO, C.QUANTIDADE,C.USUARIO_CRIACAO,C.DIA_CRIACAO,C.HORA_CRIACAO, G.CODAUX
                                FROM TB_SEQUENCIAL_TRANSFERE A
                                INNER JOIN TB_TRANSFERENCIA C ON (A.CODCT= C.CODCT AND A.PROJETO= C.PROJETO AND A.SEQTRANSF= C.SEQTRANSF)
                                LEFT JOIN TB_MATERIAL D ON (C.CODMAT=D.CODMAT)
                                LEFT JOIN TB_CODIGO_COMPLEMENTAR F ON (C.CODCT_ORIGEM=F.CODCT AND C.PROJETO_ORIGEM=F.PROJETO AND C.CODMAT=F.CODMAT AND C.CODCPL=F.CODCPL)
                                LEFT JOIN TB_CENTRO_CUSTO G ON (A.COD_CCUSTO=G.COD_CCUSTO)

                                WHERE A.RECEB_EMPRESA='N'  AND A.BAIXADA = 'S' AND(A.DATA BETWEEN @PDATA1 AND @PDATA2) ");

                        parametros.Add("@PDATA1", Data_Inicial);
                        parametros.Add("@PDATA2", Data_Final);


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

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new TransferenciaListaModel
                                {

                                    Contrato_Destino = DBNull.Value == reader["CODCT_DESTINO"] ? null : Convert.ToString(reader["CODCT_DESTINO"]),
                                    Projeto_Destino = DBNull.Value == reader["PROJETO_DESTINO"] ? null : Convert.ToString(reader["PROJETO_DESTINO"]),
                                    Codigo_Complementar = DBNull.Value == reader["CODCPL"] ? null : Convert.ToString(reader["PROJETO_DESTINO"]),
                                    Codigo_Material = DBNull.Value == reader["CODMAT"] ? null : Convert.ToString(reader["CODMAT"]),
                                    QuantidadeTransferida = DBNull.Value == reader["QUANTIDADE"] ? null : Convert.ToString(reader["QUANTIDADE"]),
                                    Seq_Transf = DBNull.Value == reader["SEQTRANSF"] ? null : Convert.ToString(reader["SEQTRANSF"]),
                                    Contrato_Origem = DBNull.Value == reader["CODCT_ORIGEM"] ? null : Convert.ToString(reader["CODCT_ORIGEM"]),
                                    Projeto_Origem = DBNull.Value == reader["PROJETO_ORIGEM"] ? null : Convert.ToString(reader["PROJETO_ORIGEM"]),
                                    Data_Documento = DBNull.Value == reader["DATA"] ? null : Convert.ToString(reader["DATA"]),
                                    Data_Criacao = DBNull.Value == reader["DIA_CRIACAO"] ? null : Convert.ToString(reader["DIA_CRIACAO"]),
                                    Hora_Criacao = DBNull.Value == reader["HORA_CRIACAO"] ? null : Convert.ToString(reader["HORA_CRIACAO"]),
                                    CodCentroCusto = DBNull.Value == reader["COD_CCUSTO"] ? null : Convert.ToString(reader["COD_CCUSTO"]),
                                    DescricaoCCentroCusto = DBNull.Value == reader["DESC_CCUSTO"] ? null : Convert.ToString(reader["DESC_CCUSTO"]),
                                    UsuarioCriacao = DBNull.Value == reader["USUARIO_CRIACAO"] ? null : Convert.ToString(reader["USUARIO_CRIACAO"]),
                                    Codigo_ComplementarAux = reader["LOCALIZACAO"] != DBNull.Value ? Convert.ToString(reader["LOCALIZACAO"]) : null,
                                });
                            }
                        }
                    }

                    return new ApiResponse<TransferenciaListaModel>()
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
                return new ApiResponse<TransferenciaListaModel>()
                {
                    error = e.Message,
                };
            }
        }

        /// <summary>
        /// Retorna a Listagem de Transferencias entre Fornecedores
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/TransferenciaApi/GetAllFornecedor")]
        public async Task<ApiResponse<TransferenciaFornecedorListaModel>> GetAllFornecedor(Transferencia_ApiRequest request)
        {
            List<TransferenciaFornecedorListaModel> retorno = new List<TransferenciaFornecedorListaModel>();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                DateTime? Data_Inicial = Convert.ToDateTime(request.DataInicial);
                DateTime? Data_Final = Convert.ToDateTime(request.DataFinal);

                if (!Data_Final.HasValue)
                {
                    throw new Exception("Data Inicial é obrigatório");
                }

                if (!Data_Final.HasValue)
                {
                    throw new Exception("Data Final é obrigatório");
                }

                if ((Data_Final.Value - Data_Inicial.Value).TotalDays >= 90
                    || ((Data_Final.Value - Data_Inicial.Value).TotalDays < 0))
                {
                    throw new Exception("Informe um período dentro de 90 dias");
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    int recordsTotal = 0;

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(CODMAT) CO FROM TB_TRANSF_FORNEC");

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
                        sql.AppendLine(@"SELECT A.SEQUENCIAL, A.COD_FORNECEDOR,b.fornecedor, A.DATA, A.NUM_DOC_FORNEC, A.ID_CONFIG_NFE,
                                         A.OBSERVACAO, A.PRAZO_GARANTIA, A.TIPO, A.BAIXADA, A.USUARIO, COALESCE(A.TIPO_TRANSFERENCIA,0) AS TIPO_TRANSFERENCIA,
                                         A.DIA,A.HORA, A.ID_REGIME_ESPECIAL, A.CFOP,B.CNPJ,

                                         D.CODMAT, D.DESCRICAO, D.UNID, D.SUBCOD, D.CLASSIFICACAO AS CLASSIF_MAT, D.HISTORICO_CODCPL,
                                         D.LIBERA_CODCPL, C.CODCPL, C.QUANTIDADE, C.VALOR,
                                         C.CODCT,C.PROJETO,C.DIA,E.LOCALIZACAO

                                        FROM TB_TRANSF_FORNEC A
                                        INNER JOIN TB_FORNECEDOR B
                                        ON(A.COD_FORNECEDOR=B.COD_FORNECEDOR)
                                        INNER JOIN TB_TRANSF_FORNEC_MAT C
                                        ON(A.SEQUENCIAL=C.SEQUENCIAL)
                                        LEFT OUTER JOIN TB_MATERIAL D
                                        ON(C.CODMAT=D.CODMAT)
                                        LEFT JOIN TB_CODIGO_COMPLEMENTAR E ON (C.CODCT=E.CODCT AND C.PROJETO=E.PROJETO AND D.CODMAT=E.CODMAT AND C.CODCPL=E.CODCPL)

                                        WHERE   A.BAIXADA = 'S' AND (A.DATA BETWEEN @PDATA1 AND @PDATA2) "); //A.COD_FORNECEDOR=:pCOD_FORNECEDOR AND A.TIPO=1 AND

                        parametros.Add("@PDATA1", Data_Inicial);
                        parametros.Add("@PDATA2", Data_Final);

                        if (String.IsNullOrEmpty(request.ContratoAniel) == false)
                        {
                            sql.AppendLine(" AND C.CODCT = @CODCT ");
                            parametros.Add("@CODCT", request.ContratoAniel);
                        }
                        if (String.IsNullOrEmpty(request.ProjetoAniel) == false)
                        {
                            sql.AppendLine(" AND C.PROJETO = @PROJETO ");
                            parametros.Add("@PROJETO", request.ProjetoAniel);
                        }
                        if (request.Alterado)
                        {
                            sql.AppendLine(" AND A.ALTERADO = 1 ");
                        }



                        #endregion
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                        {
                            while (await reader.ReadAsync())
                            {

                                retorno.Add(new TransferenciaFornecedorListaModel
                                {
                                    Tipo = reader["TIPO"] != DBNull.Value ? Convert.ToString(reader["TIPO"]) : null,
                                    Codigo_Complementar = reader["CODCPL"] != DBNull.Value ? Convert.ToString(reader["CODCPL"]) : null,
                                    Codigo_Material = reader["CODMAT"] != DBNull.Value ? Convert.ToString(reader["CODMAT"]) : null,
                                    QuantidadeTransferida = reader["QUANTIDADE"] != DBNull.Value ? Convert.ToString(reader["QUANTIDADE"]) : null,
                                    Sequencial = reader["SEQUENCIAL"] != DBNull.Value ? Convert.ToString(reader["SEQUENCIAL"]) : null,
                                    Data_Documento = reader["DATA"] != DBNull.Value ? Convert.ToString(reader["DATA"]) : null,
                                    Cnpj_Fornecedor = reader["CNPJ"] != DBNull.Value ? Convert.ToString(reader["CNPJ"]) : null,
                                    Contrato = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : null,
                                    Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : null,
                                    Data_Criacao = reader["DIA"] != DBNull.Value ? Convert.ToString(reader["DIA"]) : null,
                                    Hora_Criacao = reader["HORA"] != DBNull.Value ? Convert.ToString(reader["HORA"]) : null,
                                    UsuarioCriacao = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : null,
                                    Codigo_ComplementarAux = reader["LOCALIZACAO"] != DBNull.Value ? Convert.ToString(reader["LOCALIZACAO"]) : null,
                                });
                            }
                        }
                    }

                    return new ApiResponse<TransferenciaFornecedorListaModel>()
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
                return new ApiResponse<TransferenciaFornecedorListaModel>()
                {
                    error = e.Message,
                };
            }
        }

        /// <summary>
        ///  Retorna a Listagem de Material Tarefa Alterados e Atualiza o Campo ALTERADO para 0
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/TransferenciaApi/GetIntegracao")]
        public async Task<ApiResponseObj<List<object>>> GetIntegracao(ApiRequest request)
        {
            using (var api = new UsuarioApiController())
            {
                if (!(await api.GetUsuarioValido(request.settings)).Ok)
                {
                    throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                }
            }

            List<object> retorno = new List<object>();

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repo = new Conexao<MaterialTarefa>(sessionT5))
                {
                    var listaBanco = repo.select(x => x.Alterado == true).Result.ToList();

                    foreach (var item in listaBanco)
                    {
                        item.Alterado = false;
                    }

                    await repo.SaveOrUpdate(listaBanco);
                }

                return new ApiResponseObj<List<object>> { ok = true, data = retorno };
            }
            catch (Exception e)
            {
                return new ApiResponseObj<List<object>> { ok = true, error = e.Message };
            }
        }

    }
}