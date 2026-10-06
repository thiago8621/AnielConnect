using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.T5.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class CentroCustoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Centro de Custo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/CentroCustoApi/GetAll")]
        public async Task<ApiResponseObj<List<Centro_Custo_ListaModel>>> GetAll(Centro_Custo_ApiRequest request)
        {


            List<Centro_Custo_ListaModel> retorno = new List<Centro_Custo_ListaModel>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }




                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        #region Sql

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(@"SELECT A.COD_CCUSTO, A.DESCRICAO, A.TIPO, A.CONTA_SINTETICA, A.AGRUPAR_DISPONIVEL, A.CODAUX, A.ATIVO, A.INTEGRACAO, A.COD_GRUPO_CCUSTO,
                                              A.COD_SUBGRUPO_CCUSTO, A.DATA_CRIACAO, A.DATA_ALTERACAO, A.HORA_CRIACAO, A.HORA_ALTERACAO, A.USUARIO_CRIACAO,
                                              A.USUARIO_ALTERACAO, COD_REGIAO_PADRAO, A.TEM_RECEITA_DIRETA, A.PROCESSADO_PORTAL, A.ATUALIZACAO, A.NOVO, A.SITUACAO,
                                              A.FUNC_NOVO, A.COD_REGRA, A.CALCULAR_ABAST, A.TIPO_DEFICIENCIA, A.NUMERO, A.ID_CENTRO_CUSTO, A.ID, A.USUARIOCRIACAO,
                                              A.DATAHORACRIACAO, A.USUARIOALTERACAO, A.DATAHORAALTERACAO, A.TOKEN, A.CODIGO, A.COD_AUX, A.STATUS, A.ID_GRUPO_CENTROCUSTO,
                                              A.ID_SUBGRUPO_CENTROCUSTO
                                       FROM TB_CENTRO_CUSTO A ");


                        if (request.Cod_CentroCusto.HasValue)
                        {
                            sql.AppendLine(" WHERE  A.COD_CCUSTO = @COD_CCUSTO ");
                            parametros.Add("@COD_CCUSTO", request.Cod_CentroCusto);
                        }






                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Centro_Custo_ListaModel
                                {
                                    Cod_CentroCusto = reader["COD_CCUSTO"] != DBNull.Value ? Convert.ToString(reader["COD_CCUSTO"]) : null,
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : null,
                                    Ativo = reader["ATIVO"] != DBNull.Value ? Convert.ToString(reader["ATIVO"]) : null,
                                    Codigo_Auxiliar = reader["CODAUX"] != DBNull.Value ? Convert.ToString(reader["CODAUX"]) : null,

                                });
                            }
                        }

                    }


                    return new ApiResponseObj<List<Centro_Custo_ListaModel>>()
                    {
                        ok = true,
                        data = retorno,
                        error = ""
                    };

                }
            }
            catch (Exception e)
            {
                return new ApiResponseObj<List<Centro_Custo_ListaModel>>()
                {
                    ok = false,
                    data = retorno,
                    error = e.Message
                };
            }
        }

        /// <summary>
        /// Grava registro de Centro de Custo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/CentroCustoApi/Gravar")]
        public async Task<ApiResponseObj<Centro_Custo_CadastroModel>> Gravar(Centro_Custo_CadastroModel request)
        {
            Centro_Custo_CadastroModel retorno = new Centro_Custo_CadastroModel();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }


                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoCentroCusto = new Conexao<Centro_CustoAniel>(sessionT5))
                {

                    List<Centro_CustoAniel> BancoCentroCustoLista = (await repoCentroCusto.select(a => a.Codigo_Auxiliar == request.Codigo_Auxiliar)).ToList();

                    #region If(Condicoes)
                    if (BancoCentroCustoLista.Count != 0)
                    {
                        throw new Exception("Centro de Custo já existente nos registros do T5.");
                    }
                    if (String.IsNullOrEmpty(request.Descricao))
                    {
                        throw new Exception("O Campo Nome é um campo obrigatório no registro de Novas Funções.");
                    }

                    #endregion

                    Centro_CustoAniel centro_Custo = new Centro_CustoAniel();

                    centro_Custo.Descricao = request.Descricao;
                    centro_Custo.Codigo_Auxiliar = request.Codigo_Auxiliar;
                    centro_Custo.Data_Criacao = DateTime.Now;
                    centro_Custo.Hora_Criacao = DateTime.Now;
                    centro_Custo.Ativo = "S";


                    await repoCentroCusto.Save(centro_Custo);

                    return new ApiResponseObj<Centro_Custo_CadastroModel>()
                    {
                        ok = true,
                        data = request,
                        error = ""
                    };

                }
            }
            catch (Exception e)
            {
                return new ApiResponseObj<Centro_Custo_CadastroModel>()
                {
                    ok = false,
                    data = null,
                    error = e.Message
                };
            }
        }

    }
}