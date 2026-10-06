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
    public class AtividadeNFApiController : BaseApiController
    {
        /// <summary>
        /// Retorna todas as Origens De Mercadoria cadastradas
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/AtividadeNFApi/GetAll")]
        public async Task<ApiResponse<AtividadeNFListModel>> GetAll(ApiRequest request)
        {
            List<AtividadeNFListModel> retorno = new List<AtividadeNFListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO");
                    sql.AppendLine(" FROM TB_ATIVIDADE_NF A ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new AtividadeNFListModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                            });
                        }
                    }
                }

                return new ApiResponse<AtividadeNFListModel>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<AtividadeNFListModel>()
                {
                    error = e.Message,
                };
            }
        }
    }
}