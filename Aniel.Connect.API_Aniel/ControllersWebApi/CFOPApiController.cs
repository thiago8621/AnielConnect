using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class CFOPApiController : BaseApiController
    {
        /// <summary>
        /// Retorna todos os CFOPs cadastrados
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/CFOPApi/GetAll")]
        public async Task<ApiResponse<CFOPListModel>> GetAll(ApiRequest request)
        {
            List<CFOPListModel> retorno = new List<CFOPListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.CODIGO ");
                    sql.AppendLine(" FROM TB_CFOP A ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new CFOPListModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Codigo = Convert.ToString(reader["CODIGO"])
                            });
                        }
                    }
                }

                return new ApiResponse<CFOPListModel>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<CFOPListModel>()
                {
                    error = e.Message,
                };
            }
        }
    }
}