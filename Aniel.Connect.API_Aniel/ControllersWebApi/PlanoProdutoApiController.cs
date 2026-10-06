using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using NHibernate;
using Sinapse.DAO.repository;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class PlanoProdutoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Planos/Produtos
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PlanoProdutoApi/GetAll")]
        public async Task<ApiResponseObj<List<PlanoProdutoIndexModel>>> GetAll(ApiRequest request)
        {
            List<PlanoProdutoIndexModel> retorno = new List<PlanoProdutoIndexModel>();

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

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT * FROM ( ");
                        sql.AppendLine("SELECT A.ID, A.CODIGO, A.DESCRICAO, A.SITUACAOPLANO AS SITUACAO,1 TIPO,");
                        sql.AppendLine("C.MODELONOTAFISCAL, C.VALOR_DISCRIMINADO, C.VALORDISCRIMINADONF, C.PCT_IMPOSTO_VALOR, C.PCT_ICMS, C.PCT_PIS, C.PCT_COFINS, C.PCT_ISS, ");
                        sql.AppendLine(HtmlEnumExtensions.getEnumDescriptionInSql<Situacao_plano>("A.SITUACAOPLANO", "SITUACAO_ORDENAR"));
                        sql.AppendLine(", '" + HtmlEnumExtensions.GetEnumDescription<Model.enumerado.PlanoProdutoEnum>("1") + "' AS TIPO_ORDENAR");
                        sql.AppendLine(" FROM TB_PLANO A ");
                        sql.AppendLine(" LEFT JOIN TB_CONFIG_NF_PLANOPRODUTO C ON(A.ID = C.ID_PRODUTO) ");

                        sql.AppendLine(" UNION ");

                        sql.AppendLine("SELECT B.ID, B.CODIGO, B.DESCRICAO, B.SITUACAO AS SITUACAO,2 TIPO,");
                        sql.AppendLine("C.MODELONOTAFISCAL, C.VALOR_DISCRIMINADO, C.VALORDISCRIMINADONF, C.PCT_IMPOSTO_VALOR, C.PCT_ICMS, C.PCT_PIS, C.PCT_COFINS, C.PCT_ISS, ");
                        sql.AppendLine(HtmlEnumExtensions.getEnumDescriptionInSql<Situacao_plano>("B.SITUACAO", "SITUACAO_ORDENAR"));
                        sql.AppendLine(", '" + HtmlEnumExtensions.GetEnumDescription<Model.enumerado.PlanoProdutoEnum>("2") + "' AS TIPO_ORDENAR");
                        sql.AppendLine(" FROM TB_PRODUTO B ");
                        sql.AppendLine(" LEFT JOIN TB_CONFIG_NF_PLANOPRODUTO C ON(B.ID = C.ID_PRODUTO) ) ");
                        sql.AppendLine(" WHERE ID <> 0 ");

                       

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.start, request.settings.length))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new PlanoProdutoIndexModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Codigo = Convert.ToString(reader["CODIGO"]),
                                    Tipo = HtmlEnumExtensions.GetEnumDescription<Model.enumerado.PlanoProdutoEnum>(Convert.ToString(reader["TIPO"])),
                                    Situacao = HtmlEnumExtensions.GetEnumDescription<Situacao_plano>(Convert.ToString(reader["SITUACAO"])),
                                    ModeloNF = Convert.ToString(reader["MODELONOTAFISCAL"]),
                                    ValorDiscriminadoNF = Convert.ToString(reader["VALORDISCRIMINADONF"]),
                                    ValorDiscriminado = Convert.ToString(reader["VALOR_DISCRIMINADO"]),
                                    PctImpostoValor = Convert.ToString(reader["PCT_IMPOSTO_VALOR"]),
                                    Icms = Convert.ToString(reader["PCT_ICMS"]),
                                    Pis = Convert.ToString(reader["PCT_PIS"]),
                                    Confins = Convert.ToString(reader["PCT_COFINS"]),
                                    Iss = Convert.ToString(reader["PCT_ISS"]),
                                });
                            }
                        }

                    }
                }

                return new ApiResponseObj<List<PlanoProdutoIndexModel>> { data = retorno, ok = true };
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<PlanoProdutoIndexModel>> { error = ex.Message };
            }

        }
        /// <summary>
        /// Retorna Plano/Produto mais detalhado
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PlanoProdutoApi/FindById")]
        public async Task<ApiResponseObj<List<PlanoProdutoIndexModel>>> FindById(FindByIdApiRequest request)
        {
            List<PlanoProdutoIndexModel> retorno = new List<PlanoProdutoIndexModel>();

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

                        sql.AppendLine("SELECT * FROM ( ");
                        sql.AppendLine("SELECT A.ID, A.CODIGO, A.DESCRICAO, A.SITUACAOPLANO AS SITUACAO,1 TIPO,");
                        sql.AppendLine("C.MODELONOTAFISCAL, C.VALOR_DISCRIMINADO, C.VALORDISCRIMINADONF, C.PCT_IMPOSTO_VALOR, C.PCT_ICMS, C.PCT_PIS, C.PCT_COFINS, C.PCT_ISS, ");
                        sql.AppendLine(HtmlEnumExtensions.getEnumDescriptionInSql<Situacao_plano>("A.SITUACAOPLANO", "SITUACAO_ORDENAR"));
                        sql.AppendLine(", '" + HtmlEnumExtensions.GetEnumDescription<Model.enumerado.PlanoProdutoEnum>("1") + "' AS TIPO_ORDENAR");
                        sql.AppendLine(" FROM TB_PLANO A ");
                        sql.AppendLine(" LEFT JOIN TB_CONFIG_NF_PLANOPRODUTO C ON(A.ID = C.ID_PRODUTO) ");

                        sql.AppendLine(" UNION ");

                        sql.AppendLine("SELECT B.ID, B.CODIGO, B.DESCRICAO, B.SITUACAO AS SITUACAO,2 TIPO,");
                        sql.AppendLine("C.MODELONOTAFISCAL, C.VALOR_DISCRIMINADO, C.VALORDISCRIMINADONF, C.PCT_IMPOSTO_VALOR, C.PCT_ICMS, C.PCT_PIS, C.PCT_COFINS, C.PCT_ISS, ");
                        sql.AppendLine(HtmlEnumExtensions.getEnumDescriptionInSql<Situacao_plano>("B.SITUACAO", "SITUACAO_ORDENAR"));
                        sql.AppendLine(", '" + HtmlEnumExtensions.GetEnumDescription<Model.enumerado.PlanoProdutoEnum>("2") + "' AS TIPO_ORDENAR");
                        sql.AppendLine(" FROM TB_PRODUTO B ");
                        sql.AppendLine(" LEFT JOIN TB_CONFIG_NF_PLANOPRODUTO C ON(B.ID = C.ID_PRODUTO) ) ");
                        sql.AppendLine(" WHERE ID <> 0 ");

                        parametros.Add("ID", request.id);

                        sql.AppendLine(" WHERE ID = @ID ");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new PlanoProdutoIndexModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Codigo = Convert.ToString(reader["CODIGO"]),
                                    Tipo = HtmlEnumExtensions.GetEnumDescription<Model.enumerado.PlanoProdutoEnum>(Convert.ToString(reader["TIPO"])),
                                    Situacao = HtmlEnumExtensions.GetEnumDescription<Situacao_plano>(Convert.ToString(reader["SITUACAO"])),
                                    ModeloNF = Convert.ToString(reader["MODELONOTAFISCAL"]),
                                    ValorDiscriminadoNF = Convert.ToString(reader["VALORDISCRIMINADONF"]),
                                    ValorDiscriminado = Convert.ToString(reader["VALOR_DISCRIMINADO"]),
                                    PctImpostoValor = Convert.ToString(reader["PCT_IMPOSTO_VALOR"]),
                                    Icms = Convert.ToString(reader["PCT_ICMS"]),
                                    Pis = Convert.ToString(reader["PCT_PIS"]),
                                    Confins = Convert.ToString(reader["PCT_COFINS"]),
                                    Iss = Convert.ToString(reader["PCT_ISS"]),
                                });
                            }
                        }

                    }
                }

                return new ApiResponseObj<List<PlanoProdutoIndexModel>> { data = retorno, ok = true };
            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<PlanoProdutoIndexModel>> { error = ex.Message };
            }

        }


        public class CodigoIntegracaoRequest
        {
            public int Id { get; set; }
            public getCodigoIntegracao Request { get; set; }
        }

        /// <summary>
        /// Retorna codigo integracao de determinado plano produto
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PlanoProdutoApi/getCodigoIntegracaoById")]
        public async Task<GetApiRetorno<getCodigoIntegracao>> getCodigoIntegracaoById([FromBody] getCodigoIntegracaoRequest request)
        {
            GetApiRetorno<getCodigoIntegracao> response = new GetApiRetorno<getCodigoIntegracao>();

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
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    // Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT CODIGO_INTEGRACAO FROM TB_CLIENTE_PLANO_PRODUTO");
                        sql.AppendLine("WHERE ID = @ID");
                        parametros.Add("ID", request.Id);

                        var data = new getCodigoIntegracao();
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                data.CodigoIntegracao = Convert.ToString(reader["CODIGO_INTEGRACAO"]);
                            }
                        }

                        response.Ok = true;
                        response.Objeto = data;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"Exception: {ex.Message}";
            }

            return response;
        }


        /// <summary>
        /// Atualiza codigo de integracao de um determinado plano produto
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PlanoProdutoApi/UpdateCodigoIntegracaoById")]
        public async Task<PostApiRetorno> UpdateCodigoIntegracaoById([FromBody] UpdateCodigoIntegracaoRequest request)
        {
            PostApiRetorno response = new PostApiRetorno();

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

                   
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("UPDATE TB_CLIENTE_PLANO_PRODUTO");
                        sql.AppendLine("SET CODIGO_INTEGRACAO = @CodigoIntegracao");
                        sql.AppendLine("WHERE ID = @ID");
                        parametros.Add("CodigoIntegracao", request.CodigoIntegracao);
                        parametros.Add("ID", request.Id);



                      
                            com.CommandText = sql.ToString();

                            foreach (var param in parametros)
                            {
                                var dbParam = com.CreateParameter();
                                dbParam.ParameterName = param.Key;
                                dbParam.Value = param.Value ?? DBNull.Value;
                                com.Parameters.Add(dbParam);
                            }

                            
                            int rowsAffected = await com.ExecuteNonQueryAsync();

                            if (rowsAffected > 0)
                            {
                               
                                response.Ok = true;
                                response.Mensagem = "Atualização bem sucedida.";
                            }
                            else
                            {
                               
                                response.Ok = false;
                                response.Mensagem = "Nenhum dado foi atualizado.";
                            }
                        

                    }
                }

            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"Exception: {ex.Message}";
            }

            return response;
        }

        /////<summary>
        /////Realiza a Função de Desloqueio em Confiança
        /////</summary>
        /////<param name="request">Objeto de Request</param>
        /////<returns></returns>
        //[HttpPost]
        //[Route("PlanoProdutoapi/GetDesbloqueioConfianca")]
        //public async Task<Response> GetDesbloqueioConfianca(PlanoProdutoApiRequest request)
        //{
        //    var response = new Response();

        //    try
        //    {
        //        using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
        //        using (ISession sessionConnect = HibernateUtil.GetSession())
        //        using (var repoLoginRadius = new Conexao<LoginRadius>(sessionConnect))
        //        using (var repoConcentradora = new Conexao<Concentradora>(sessionConnect))
        //        using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionConnect))
        //        {
        //            var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);

        //            if (loginRadius == null)
        //                throw new EAnielException("Login não informado para o plano.");

        //            if (loginRadius.Concentradora?.Tipo == null)
        //                throw new EAnielException("Marca não informado da concentradora.");

        //            switch (loginRadius.Concentradora.Tipo)
        //            {
        //                case Model.enumerado.Marca_Modelo_Concentradora.Mikrotik:
        //                    RadiusRepository.DesbloqueioConfianca(new Mikrotik(), sessionRadius, loginRadius.Cliente_PlanoProduto, request.Usuario);
        //                    break;
        //            }

        //            loginRadius.Cliente_PlanoProduto.Status = Sinapse.Model.enumerados.Status_PlanoProduto.Habilitado;
        //            loginRadius.Cliente_PlanoProduto.Data_Desbloqueio_Confianca = DateTime.Now.Date;
        //            await repoClientePlanoProduto.SaveOrUpdate(loginRadius.Cliente_PlanoProduto);
        //            response.result = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.result = false;
        //        response.message = ex.Message;
        //    }

        //    return response;
        //}



        /////<summary>
        /////Realiza a Função de Desloqueio em Confiança
        /////</summary>
        /////<param name="request">Objeto de Request</param>
        /////<returns></returns>
        //[HttpPost]
        //[Route("PlanoProdutoapi/GetPlano")]
        //public async Task<ApiResponse<PlanoProdutoListModel>> GetPlano(PlanoApiRequest request)
        //{

        //}
    }
}
