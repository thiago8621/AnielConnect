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
    public class CST_ICMSApiController : BaseApiController
    {
        /// <summary>
        /// Retorna todos CST ICMS
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/CST_ICMSApi/GetAll")]
        public async Task<ApiResponse<CST_ICMSListModel>> GetAll(ApiRequest request)
        {
            List<CST_ICMSListModel> retorno = new List<CST_ICMSListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.CODIGO ");
                    sql.AppendLine(" FROM TB_CST_ICMS A ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new CST_ICMSListModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Codigo = Convert.ToString(reader["CODIGO"]),
                            });
                        }
                    }
                }

                return new ApiResponse<CST_ICMSListModel>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<CST_ICMSListModel>()
                {
                    error = e.Message,
                };
            }
        }
    }
}