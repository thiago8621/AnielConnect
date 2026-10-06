using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
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
    public class FuncionarioApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem dos Funcionarios do Aniel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FuncionarioApi/GetAll")]
        public async Task<ApiResponse<Funcionario_ListaModel>> GetAll(Funcionario_ApiRequest request)
        {
            List<Funcionario_ListaModel> retorno = new List<Funcionario_ListaModel>();
            var listagem = new List<Funcionario_ListaModel>();
            int registrosFitlrados = 0;
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
                        var ativo = "ativo"; var inativo = "inativo";
                        
                        #region Sql
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("  SELECT A.NUM_FRE,A.NUM_FRE_AUX,A.NOME,B.COD_SUPERV,(D.NOME)AS NOME_SUPERVISOR,A.COD_FUNCAO,  ");
                        sql.AppendLine("        (C.NOME)AS FUNCAO,A.COD_SECAO,A.CPF,A.APELIDO,  ");
                        sql.AppendLine("         A.DATA_NASC,A.NACIONALIDADE,A.NATURALIDADE,  ");
                        sql.AppendLine("         A.RG,A.ENDERECO,A.MAE,A.PAI,A.BAIRRO,A.TEL_CELULAR,A.SITUACAO  ");
                        sql.AppendLine("  FROM TB_FUNCIONARIO A  ");
                        sql.AppendLine("  LEFT JOIN TB_EQUIPE B ON (A.NUM_FRE = B.NUM_FRE)  ");
                        sql.AppendLine("  LEFT JOIN TB_FUNCAO C ON (A.COD_FUNCAO = C.COD_FUNCAO)  ");
                        sql.AppendLine("  LEFT JOIN TB_FUNCIONARIO D ON (B.COD_SUPERV = D.NUM_FRE)  ");
                        
                        if (!String.IsNullOrEmpty(request.situacao))
                        {
                            if (request.situacao.ToLower() == ativo)
                            {
                                sql.AppendLine("WHERE A.SITUACAO NOT IN (2)");
                            }
                            else if (request.situacao.ToLower() == inativo)
                            {
                                sql.AppendLine("WHERE A.SITUACAO IN (2)");
                            }
                        }

                        if (!String.IsNullOrEmpty(request.settings.search))
                        {
                            sql.AppendLine("WHERE UPPER(A.NOME) LIKE @SEARCH ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search.ToUpperInvariant()));
                        }
                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Funcionario_ListaModel
                                {
                                    numFre = !String.IsNullOrEmpty(Convert.ToString(reader["NUM_FRE"])) ? Convert.ToString(reader["NUM_FRE"]) : "-",
                                    numFreAux = !String.IsNullOrEmpty(Convert.ToString(reader["NUM_FRE_AUX"])) ? Convert.ToString(reader["NUM_FRE_AUX"]) : "-",
                                    codFuncao = !String.IsNullOrEmpty(Convert.ToString(reader["COD_FUNCAO"])) ? Convert.ToString(reader["COD_FUNCAO"]) : "-",
                                    funcaoFuncionario = !String.IsNullOrEmpty(Convert.ToString(reader["FUNCAO"])) ? Convert.ToString(reader["FUNCAO"]) : "-",
                                    codSuperv = !String.IsNullOrEmpty(Convert.ToString(reader["COD_SUPERV"])) ? Convert.ToString(reader["COD_SUPERV"]) : "-",
                                    nomeSuperv = !String.IsNullOrEmpty(Convert.ToString(reader["NOME_SUPERVISOR"])) ? Convert.ToString(reader["NOME_SUPERVISOR"]) : "-",
                                    Cod_Secao = reader["COD_SECAO"] != DBNull.Value ? Convert.ToInt32(reader["COD_SECAO"]) : 0,
                                    CPF = reader["CPF"] != DBNull.Value ? Convert.ToString(reader["CPF"]) : "-",
                                    Apelido = !String.IsNullOrEmpty(Convert.ToString(reader["APELIDO"])) ? Convert.ToString(reader["APELIDO"]) : "-",
                                    Data_Nascimento = !String.IsNullOrEmpty(Convert.ToString(reader["DATA_NASC"])) ? Convert.ToString(reader["DATA_NASC"]) : "-",
                                    Nacionalidade = !String.IsNullOrEmpty(Convert.ToString(reader["NACIONALIDADE"])) ? Convert.ToString(reader["NACIONALIDADE"]) : "-",
                                    Nome = !String.IsNullOrEmpty(Convert.ToString(reader["NOME"])) ? Convert.ToString(reader["NOME"]) : "-",
                                    Naturalidade = !String.IsNullOrEmpty(Convert.ToString(reader["NATURALIDADE"])) ? Convert.ToString(reader["NATURALIDADE"]) : "-",
                                    RG = !String.IsNullOrEmpty(Convert.ToString(reader["RG"])) ? Convert.ToString(reader["RG"]) : "-",
                                    //Salario = !String.IsNullOrEmpty(Convert.ToString(reader["SALARIO_MES"])) ? Convert.ToString(reader["SALARIO_MES"]) : "-",
                                    Endereco = !String.IsNullOrEmpty(Convert.ToString(reader["ENDERECO"])) ? Convert.ToString(reader["ENDERECO"]) : "-",
                                    Mae = !String.IsNullOrEmpty(Convert.ToString(reader["MAE"])) ? Convert.ToString(reader["MAE"]) : "-",
                                    Pai = !String.IsNullOrEmpty(Convert.ToString(reader["PAI"])) ? Convert.ToString(reader["PAI"]) : "-",
                                    Bairro = !String.IsNullOrEmpty(Convert.ToString(reader["BAIRRO"])) ? Convert.ToString(reader["BAIRRO"]) : "-",
                                    Celular = !String.IsNullOrEmpty(Convert.ToString(reader["TEL_CELULAR"])) ? Convert.ToString(reader["TEL_CELULAR"]) : "-",
                                    Situacao = HtmlEnumExtensions.GetEnumDescription<SituacaoFuncionarioAniel>(Convert.ToString(reader["SITUACAO"]))
                                });
                            }
                        }
                        
                        listagem = retorno.Take(request.settings.length).ToList();
                        registrosFitlrados = listagem.Count;
                    }
                    return new ApiResponse<Funcionario_ListaModel>()
                    {
                        recordsFiltered = registrosFitlrados,
                        quantidadeTotalRegistros = listagem.Count,
                        draw = request.settings.length,
                        data = listagem,
                        error = ""
                    };

                }
            }
            catch (Exception e)
            {
                return new ApiResponse<Funcionario_ListaModel>()
                {
                    recordsFiltered = 0,
                    quantidadeTotalRegistros = 0,
                    draw = request.settings.length,
                    data = listagem,
                    error = e.Message
                };
            }
        }

        /// <summary>
        /// Grava um Funcionário
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FuncionarioApi/GravarFuncionario")]
        public async Task<ApiResponseObj<string>> GravarFuncionario(Funcionario_CadastroModel request)
        {
            var retorno = new ApiResponseObj<string>();
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
                using (var repoFuncionario = new Conexao<FuncionarioAniel>(sessionT5))
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    IList<FuncionarioAniel> BancoFuncionarioLista = await repoFuncionario.select(a => a.Num_Fre == request.Num_Fre);

                    if (String.IsNullOrEmpty(request.Num_Fre))
                    {
                        BancoFuncionarioLista = await repoFuncionario.select(b => b.Matricula_Aux == request.Matricula_Aux);
                    }

                    FuncionarioAniel BancoFuncionario;
                    if (BancoFuncionarioLista.Count == 0) 
                    {                         
                        BancoFuncionario = new FuncionarioAniel();
                    }
                    else 
                    {
                        BancoFuncionario = BancoFuncionarioLista.First(); 
                    }
                    if (BancoFuncionarioLista.Count == 0)
                    {
                        if (String.IsNullOrEmpty(request.Num_Fre))
                        {
                            throw new Exception("Num_Fre é obrigatório para o cadastro dos Funcionarios");
                        }
                      
                        BancoFuncionario.Num_Fre = request.Num_Fre;
                        BancoFuncionario.Nome = request.Nome;
                    }
                        
                    #region BindModel                    
                    BancoFuncionario.Cod_Depto = request.Cod_Depto;
                    BancoFuncionario.Cpf = request.Cpf;
                    BancoFuncionario.Email = request.Email;
                    BancoFuncionario.Id_Municipio_Trabalho = request.Id_Municipio_Trabalho;
                    BancoFuncionario.Municipio_Trabalho = request.Municipio_Trabalho;                    
                    BancoFuncionario.Situacao = SituacaoFuncionarioAniel.Ativo;
                    BancoFuncionario.Supervisor = request.Supervisor;
                    BancoFuncionario.Telefone_Celular = request.Telefone_Celular;
                    BancoFuncionario.Uf_Trabalho = request.Uf_Trabalho;
                    #endregion

                    await repoFuncionario.SaveOrUpdate(BancoFuncionario);
                    
                    retorno.data = BancoFuncionario.Num_Fre;
                    retorno.ok = true;
                }
            }

            catch (Exception e)
            {
                retorno.error = e.Message;
                retorno.data = "";
                retorno.ok = false;                
            }
            return retorno;
        }

    }
}