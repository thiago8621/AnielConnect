using Aniel.Connect.API_Aniel.Models;
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


namespace Aniel.Connect.API_Aniel.ControllersWebApi
{

    public class Meio_CaptacaoApiController : BaseApiController
    { 
        /// <summary>
      /// Retorna a Listagem do Meio de Captação
      /// </summary>
      /// <param name="request"></param>
      /// <returns></returns>

        [HttpPost()]
        [Route("api/Meio_Captacaoapi/GetAll")]
        public async Task<ApiResponse<Meio_Captacao_List_Model>> GetAll(Meio_Captacao_ApiRequest request)
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

                List<Meio_Captacao_List_Model> retorno = new List<Meio_Captacao_List_Model>();
                IOrderedEnumerable<Meio_Captacao_List_Model> listaOrdenada = null;
                Int32 recordsTotal = 0;

                using (ISession session = HibernateUtil.GetSession())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Count
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MEIO_CAPTACAO WHERE STATUS = 1");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.STATUS ");
                        sql.AppendLine(" FROM TB_MEIO_CAPTACAO A ");

                        //  sql.AppendLine(" WHERE A.STATUS = 1 ");


                        //Filtrando
                        if (!String.IsNullOrEmpty(request.settings.search))
                        {
                            sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search));
                        }

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Meio_Captacao_List_Model
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                                });
                            }
                        }

                    }
                }

                //Ordenacao
                if (request.settings.order != null && request.settings.order.column.HasValue)
                {
                    int sortColumnIndex = request.settings.order.column.Value;
                    string sortDirection = request.settings.order.dir;
                    switch (sortColumnIndex)
                    {
                        case 1:     // Nome
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Id);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Id);
                                break;
                            }
                        default:
                            {
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                                break;
                            }
                    }
                }



                //Page e retornando DataTable
                return new ApiResponse<Meio_Captacao_List_Model>
                {
                    draw = request.settings.length,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = listaOrdenada,
                    error = ""
                };

            }


            catch (Exception ex)
            {

                return new ApiResponse<Meio_Captacao_List_Model>
                {
                    error = ex.Message
                };
            }
        }
    }
}