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
    public class OrigemMercadoriaApiController : BaseApiController
    {
        /// <summary>
        /// Retorna todas as Origens De Mercadoria cadastradas
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/OrigemMercadoriaApi/GetAll")]
        public async Task<ApiResponse<OrigemMercadoriaListModel>> GetAll(ApiRequest request)
        {
            List<OrigemMercadoriaListModel> retorno = new List<OrigemMercadoriaListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO ");
                    sql.AppendLine(" FROM TB_ORIGEM_MERCADORIA A ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new OrigemMercadoriaListModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                            });
                        }
                    }
                }

                return new ApiResponse<OrigemMercadoriaListModel>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<OrigemMercadoriaListModel>()
                {
                    error = e.Message,
                };
            }
        }
    }
}