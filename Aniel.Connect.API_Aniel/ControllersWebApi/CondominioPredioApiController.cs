using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core.Helpers;
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
    public class CondominioPredioApiController : ApiController
    {
        /// <summary>
        /// Retorna a Listagem de Condomínios
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/CondominioPredioapi/GetAll")]
        public async Task<ApiResponseObj<List<CondominioPredioListModel>>> GetAllCondominios(CondominioPredioApiRequest request)
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
                List<CondominioPredioListModel> retorno = new List<CondominioPredioListModel>();

                using (ISession session = HibernateUtil.GetSession())
                {

                    var parametros = new Dictionary<string, object>();
                    var condicoes = new List<string>();

                    StringBuilder sqlFROM = new StringBuilder(" FROM TB_CONDOMINIO_PREDIO A");

                    //Filtrando
                    if(!String.IsNullOrEmpty(request.CEP)) 
                    {
                        var cepSemHifen = request.CEP.Replace("-", "").Trim();

                        var cepComHifen = cepSemHifen.Length == 8
                            ? cepSemHifen.Insert(5, "-")
                            : cepSemHifen;

                        condicoes.Add("(A.CEP = @CEP1 OR A.CEP = @CEP2)");
                        parametros.Add("@CEP1", cepSemHifen);
                        parametros.Add("@CEP2", cepComHifen);
                    }
                    if (!String.IsNullOrEmpty(request.Numero)) 
                    {
                        condicoes.Add("A.NUMERO = @NUMERO");
                        parametros.Add("@NUMERO",request.Numero);
                    }
                    if (!String.IsNullOrEmpty(request.settings.search))
                    {
                        condicoes.Add("(UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search));
                    }

                    if (condicoes.Count > 0) 
                    {
                        sqlFROM.AppendLine($" WHERE {String.Join(" AND ", condicoes)}");
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID, A.ID_PAIS, A.ID_CIDADE, A.DESCRICAO, A.NUMERO, A.BAIRRO, A.LOGRADOURO,")
                           .AppendLine(" A.UF, A.CEP, A.ID_ENDERECO_LOGRADOURO, A.STATUS")
                           .AppendLine(sqlFROM.ToString());

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new CondominioPredioListModel
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                    IdPais = reader["ID_PAIS"] != DBNull.Value ? Convert.ToInt64(reader["ID_PAIS"]) : 0,
                                    IdCidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : 0,
                                    IdEnderecoLogradouro = reader["ID_ENDERECO_LOGRADOURO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ENDERECO_LOGRADOURO"]) : 0,
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                    Numero = reader["NUMERO"] != DBNull.Value ? Convert.ToString(reader["NUMERO"]) : "",
                                    Bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : "",
                                    Logradouro = reader["LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["LOGRADOURO"]) : "",
                                    Uf = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : "",
                                    Cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : "",
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>((Status_Default)reader["STATUS"]),
                                }); ;
                            }
                        }
                    }
                }
                //Page e retornando DataTable
                return new ApiResponseObj<List<CondominioPredioListModel>>
                {
                    ok = true,
                    data = retorno,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                return new ApiResponseObj<List<CondominioPredioListModel>>
                {
                    error = ex.Message,
                    ok = false
                };
            }
        }
    }
}