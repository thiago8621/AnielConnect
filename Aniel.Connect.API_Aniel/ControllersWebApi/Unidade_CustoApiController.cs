using Aniel.T5.Model.nhibernate;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
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
using Aniel.Connect.API_Aniel.Models;
namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class Unidade_CustoApiController : BaseApiController
    {
        /// <summary>
        /// Listar Unidades de Custo de Cliente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Unidade_CustoApiController/GetAll")]
        public async Task<ApiResponse<Unidade_Custo_ListModel>> GetAll(ApiRequest request)
        {
            List<Unidade_Custo_ListModel> retorno = new List<Unidade_Custo_ListModel>();
            try
            {

                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                Dictionary<String, Object> parametro = new Dictionary<string, object>();
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    var com = sessionT5.Connection.CreateCommand();
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.UNIDADE_CUSTO, DESCRICAO");
                        sql.AppendLine("FROM TB_UNIDADE_CUSTO_CLIENTE A");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametro, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Unidade_Custo_ListModel
                                {
                                    Descricao = Convert.ToString(reader["UNIDADE_CUSTO"]),
                                    Unidade_Custo = Convert.ToString(reader["DESCRICAO"])
                                });
                            }
                        }

                    }

                }
                return new ApiResponse<Unidade_Custo_ListModel>()
                {
                    draw = request.settings.length,
                    data = retorno,
                    error = ""
                };
            }
            catch (Exception erro)
            {
                return new ApiResponse<Unidade_Custo_ListModel>()
                {
                    draw = request.settings.length,
                    data = retorno,
                    error = erro.Message
                };
            }

        }
    }
}