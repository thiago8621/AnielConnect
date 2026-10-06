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
    public class FuncaoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Funções dos Funcionarios do Aniel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FuncaoApi/GetAll")]
        public async Task<ApiResponse<Funcao_ListaModel>> GetAll(ApiRequest request)
        {


            List<Funcao_ListaModel> retorno = new List<Funcao_ListaModel>();
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
                        sql.AppendLine("  SELECT * FROM TB_FUNCAO");


                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Funcao_ListaModel
                                {
                                    Cod_Funcao = reader["COD_FUNCAO"] != DBNull.Value ? Convert.ToInt32(reader["COD_FUNCAO"]) : 0,

                                    Adic_Insalubre = !String.IsNullOrEmpty(Convert.ToString(reader["ADIC_INSALUBRE"])) ? Convert.ToString(reader["ADIC_INSALUBRE"]) : "-",
                                    Adic_Periculosidade = !String.IsNullOrEmpty(Convert.ToString(reader["ADIC_PERICULOSIDADE"])) ? Convert.ToString(reader["ADIC_PERICULOSIDADE"]) : "-",
                                    Adic_Transferencia = !String.IsNullOrEmpty(Convert.ToString(reader["ADIC_TRANSFERENCIA"])) ? Convert.ToString(reader["ADIC_TRANSFERENCIA"]) : "-",
                                    Classe = !String.IsNullOrEmpty(Convert.ToString(reader["CLASSE"])) ? Convert.ToString(reader["CLASSE"]) : "-",
                                    
                                    Cod_Funcao_Aux = !String.IsNullOrEmpty(Convert.ToString(reader["COD_FUNCAO_AUX"])) ? Convert.ToString(reader["COD_FUNCAO_AUX"]) : "-",
                                    Cod_Perfil_Equipe = !String.IsNullOrEmpty(Convert.ToString(reader["COD_PERFIL_EQUIPE"])) ? Convert.ToString(reader["COD_PERFIL_EQUIPE"]) : "-",
                                    Cod_Sindicato = !String.IsNullOrEmpty(Convert.ToString(reader["COD_SINDICATO"])) ? Convert.ToString(reader["COD_SINDICATO"]) : "-",
                                    Coluna = !String.IsNullOrEmpty(Convert.ToString(reader["COLUNA"])) ? Convert.ToString(reader["COLUNA"]) : "-",
                                    Desc_At_Ppp = !String.IsNullOrEmpty(Convert.ToString(reader["DESC_AT_PPP"])) ? Convert.ToString(reader["DESC_AT_PPP"]) : "-",
                                    Funcao_Tributavel = !String.IsNullOrEmpty(Convert.ToString(reader["FUNCAO_TRIBUTAVEL"])) ? Convert.ToString(reader["FUNCAO_TRIBUTAVEL"]) : "-",
                                    Grupo_Funcao = !String.IsNullOrEmpty(Convert.ToString(reader["GRUPO_FUNCAO"])) ? Convert.ToString(reader["GRUPO_FUNCAO"]) : "-",
                                    HXH = !String.IsNullOrEmpty(Convert.ToString(reader["HXH"])) ? Convert.ToString(reader["HXH"]) : "-",
                                    Integracao = !String.IsNullOrEmpty(Convert.ToString(reader["INTEGRACAO"])) ? Convert.ToString(reader["INTEGRACAO"]) : "-",
                                    Nome = !String.IsNullOrEmpty(Convert.ToString(reader["NOME"])) ? Convert.ToString(reader["NOME"]) : "-",
                                    Num_Cbo = !String.IsNullOrEmpty(Convert.ToString(reader["NUM_CBO"])) ? Convert.ToString(reader["NUM_CBO"]) : "-",
                                    Objetivo = !String.IsNullOrEmpty(Convert.ToString(reader["OBJETIVO"])) ? Convert.ToString(reader["OBJETIVO"]) : "-",
                                    Permite_Gerar_Equipe = !String.IsNullOrEmpty(Convert.ToString(reader["PERMITE_GERAR_EQUIPE"])) ? Convert.ToString(reader["PERMITE_GERAR_EQUIPE"]) : "-",
                                    Piso_Minimo = !String.IsNullOrEmpty(Convert.ToString(reader["PISO_MINIMO"])) ? Convert.ToString(reader["PISO_MINIMO"]) : "-",
                                    Plano = !String.IsNullOrEmpty(Convert.ToString(reader["PLANO"])) ? Convert.ToString(reader["PLANO"]) : "-",
                                    Requisito = !String.IsNullOrEmpty(Convert.ToString(reader["REQUISITO"])) ? Convert.ToString(reader["REQUISITO"]) : "-",
                                    Responsabilidade = !String.IsNullOrEmpty(Convert.ToString(reader["RESPONSABILIDADE"])) ? Convert.ToString(reader["RESPONSABILIDADE"]) : "-",
                                    Revisao = !String.IsNullOrEmpty(Convert.ToString(reader["REVISAO"])) ? Convert.ToString(reader["REVISAO"]) : "-",
                                    Salario = !String.IsNullOrEmpty(Convert.ToString(reader["SALARIO"])) ? Convert.ToString(reader["SALARIO"]) : "-",
                                    Status = !String.IsNullOrEmpty(Convert.ToString(reader["STATUS"])) ? Convert.ToString(reader["STATUS"]) : "-",
                                });
                            }
                        }

                    }


                    return new ApiResponse<Funcao_ListaModel>()
                    {
                        draw = request.settings.length,
                        data = retorno,
                        error = ""
                    };

                }
            }
            catch (Exception e)
            {
                return new ApiResponse<Funcao_ListaModel>()
                {
                    draw = request.settings.length,
                    data = retorno,
                    error = e.Message
                };
            }
        }

        /// <summary>
        /// Grava uma nova função
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FuncaoApi/GravarFuncao")]
        public async Task<ApiResponse<Funcao_ListaModel>> GravarFuncao(Funcao_CadastroModel request)
        {
            List<Funcao_ListaModel> retorno = new List<Funcao_ListaModel>();

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
                using (var repoFuncao = new Conexao<Funcao>(sessionT5))
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();


                    IList<Funcao> BancoFuncaoLista = await repoFuncao.select(a => a.Nome == request.Nome);

                    #region If(Condicoes)
                    if (BancoFuncaoLista.Count != 0)
                    {
                        throw new Exception("Função já existente nos registros do T5.");
                    }
                    if (String.IsNullOrEmpty(request.Nome))
                    {
                        throw new Exception("O Campo Nome é um campo obrigatório no registro de Novas Funções.");
                    } 
                    #endregion

                    Funcao BancoFuncao  = new Funcao();


                    #region BindModel

                    BancoFuncao.Adic_Insalubre = request.Adic_Insalubre;
                    BancoFuncao.Adic_Periculosidade = request.Adic_Periculosidade;
                    BancoFuncao.Adic_Transferencia = request.Adic_Transferencia;
                    BancoFuncao.Classe = request.Classe;
                    BancoFuncao.Cod_Funcao = request.Cod_Funcao;
                    BancoFuncao.Cod_Funcao_Aux = request.Cod_Funcao_Aux;
                    BancoFuncao.Cod_Perfil_Equipe = request.Cod_Perfil_Equipe;
                    BancoFuncao.Cod_Sindicato = request.Cod_Sindicato;
                    BancoFuncao.Coluna = request.Coluna;
                    BancoFuncao.Desc_At_Ppp = request.Desc_At_Ppp;
                    BancoFuncao.Funcao_Tributavel = request.Funcao_Tributavel;
                    BancoFuncao.Grupo_Funcao = request.Grupo_Funcao;
                    BancoFuncao.HXH = request.HXH;
                    BancoFuncao.Integracao = request.Integracao;
                    BancoFuncao.Nome = request.Nome;
                    BancoFuncao.Num_Cbo = request.Num_Cbo;
                    BancoFuncao.Objetivo = request.Objetivo;
                    BancoFuncao.Permite_Gerar_Equipe = request.Permite_Gerar_Equipe;
                    BancoFuncao.Piso_Minimo = request.Piso_Minimo;
                    BancoFuncao.Plano = request.Plano;
                    BancoFuncao.Requisito = request.Requisito;
                    BancoFuncao.Responsabilidade = request.Responsabilidade;
                    BancoFuncao.Revisao = request.Revisao;
                    BancoFuncao.Salario = request.Salario;


                    #endregion

                    await repoFuncao.Save(BancoFuncao);



                    return new ApiResponse<Funcao_ListaModel>()
                    {
                        draw = request.settings.length,
                        data = retorno,
                        error = ""
                    };
                }
            }


            catch (Exception e)
            {

                return new ApiResponse<Funcao_ListaModel>()
                {
                    draw = request.settings.length,
                    data = retorno,
                    error = e.Message
                };
            }
        }

    }
}       