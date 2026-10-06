using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using NHibernate.Hql.Ast;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class CidadeApiController : BaseApiController
    {
        /// <summary>
        /// Retorna todas cidades
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/CidadeApi/GetAll")]
        public async Task<ApiResponse<CidadeListModel>> GetAll(ApiRequest request)
        {
            List<CidadeListModel> retorno = new List<CidadeListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {

                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO,A.CODIGO_IBGE,A.UF");
                    sql.AppendLine(" FROM TB_CIDADE A ");

                    if (!String.IsNullOrEmpty(request.settings.search))
                    {
                        sql.AppendLine("WHERE UPPER(A.DESCRICAO) LIKE @SEARCH");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search.ToUpperInvariant()));
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new CidadeListModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                CodigoIbge = Convert.ToString(reader["CODIGO_IBGE"]),
                                Uf = Convert.ToString(reader["UF"]),
                            });
                        }
                    }
                }

                return new ApiResponse<CidadeListModel>()
                {
                    data = retorno,
                    draw = request.settings.length,
                    error = "",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<CidadeListModel>()
                {
                    error = e.Message,
                };
            }
        }
    }
}