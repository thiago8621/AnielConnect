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
    public class CSOSN_ICMSApiController : BaseApiController
    {
        /// <summary>
        /// Retorna todas as Origens De Mercadoria cadastradas
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/CSOSN_ICMSApi/GetAll")]
        public async Task<ApiResponse<CSOSN_ICMSListModel>> GetAll(ApiRequest request)
        {
            List<CSOSN_ICMSListModel> retorno = new List<CSOSN_ICMSListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.CODIGO ");
                    sql.AppendLine(" FROM TB_CSOSN_ICMS A ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new CSOSN_ICMSListModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Codigo = Convert.ToString(reader["CODIGO"]),
                            });
                        }
                    }
                }

                return new ApiResponse<CSOSN_ICMSListModel>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<CSOSN_ICMSListModel>()
                {
                    error = e.Message,
                };
            }
        }
    }
}