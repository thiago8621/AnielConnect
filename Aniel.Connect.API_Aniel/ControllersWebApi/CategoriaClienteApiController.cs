using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class CategoriaClienteApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Categoria de Cliente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/CategoriaClienteApi/GetAll")]
        public async Task<ApiResponseObj<List<Categoria_ClienteListaModel>>> GetAll(ApiRequest request)
        {
            List<Categoria_ClienteListaModel> retorno = new List<Categoria_ClienteListaModel>();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession session = HibernateUtil.GetSession())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.STATUS ");
                        sql.AppendLine(" FROM TB_CATEGORIA_CLIENTE A ");


                        //Filtrando
                        if (!String.IsNullOrEmpty(request.settings.search))
                        {
                            sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search));
                        }


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Categoria_ClienteListaModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                                });
                            }
                        }

                    }
                }

                return new ApiResponseObj<List<Categoria_ClienteListaModel>>() { ok = true, data = retorno };
            
            }
            catch (Exception e)
            {
                return new ApiResponseObj<List<Categoria_ClienteListaModel>>() { ok = true, error = e.Message };
            }
        }
    }
}