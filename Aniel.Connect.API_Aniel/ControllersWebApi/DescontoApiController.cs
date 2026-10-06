using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;



namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class DescontoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Clientes
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Descontoapi/GetAll")]
        public async Task<ApiResponseObj<List<DescontoListaModel>>> GetAll(Desconto_ApiRequest request)
        {
            try
            {
                ValidateModel();

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                List<DescontoListaModel> retorno = new List<DescontoListaModel>();
                
                using (ISession session = HibernateUtil.GetSession())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFROM = new StringBuilder(" FROM TB_DESCONTO A");
                    sqlFROM.AppendLine(" LEFT JOIN TB_TIPO_DESCONTO B ");
                    sqlFROM.AppendLine(" ON(B.ID = A.ID_TIPO_DESCONTO) ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.settings.search))
                    {
                        sqlFROM.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search));
                    }

                    

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.STATUS,A.ID, A.DESCRICAO,A.VALOR_DESCONTO,A.PERCENTUAL_DESCONTO,REPETICOES_DESCONTO,A.DESCONTO_CAPTACAO, B.DESCRICAO AS TIPO_DESCONTO ");
                        sql.AppendLine(sqlFROM.ToString());


                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new DescontoListaModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Desconto_Captacao = HtmlEnumExtensions.GetEnumDescription<Desconto_Captacao>(Convert.ToString(reader["DESCONTO_CAPTACAO"])),
                                    Percentual_Desconto = Convert.ToString(reader["PERCENTUAL_DESCONTO"]),
                                    Repeticoes_Desconto = Convert.ToString(reader["REPETICOES_DESCONTO"]),
                                    Valor_Desconto = Convert.ToString(reader["VALOR_DESCONTO"]),
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                    Tipo_Desconto = Convert.ToString(reader["TIPO_DESCONTO"])
                                });
                            }
                        }

                    }
                }


                //Page e retornando DataTable
                return new ApiResponseObj<List<DescontoListaModel>>
                {
                    ok = true,
                    data = retorno,
                    error = ""
                };

            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<DescontoListaModel>>
                {
                    error = ex.Message,
                    ok = false
                };
            }
        }
    }
}