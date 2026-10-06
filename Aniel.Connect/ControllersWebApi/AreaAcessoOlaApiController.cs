using Aniel.Connect.Core;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class AreaAcessoOlaApiController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet()]
        [Route("api/AreaAcessoOlaapi/GetAreasAcessoOla")]
        public async Task<GetApiRetorno<List<AreaAcessoOlaModel>>> GetAreasAcessoOla()
        {
            GetApiRetorno<List<AreaAcessoOlaModel>> retorno = new GetApiRetorno<List<AreaAcessoOlaModel>>();
            List<AreaAcessoOlaModel> listaAreas = new List<AreaAcessoOlaModel>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<string, string> parametros = new Dictionary<string, string>();

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.COD_AREA, COALESCE(A.OCULTAR_ACESSO, 0) AS OCULTAR_ACESSO ");
                    sql.AppendLine("FROM TB_AREA_ACESSO_OLA A ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while(await reader.ReadAsync())
                        {
                            var area = new AreaAcessoOlaModel();
                            area.Id = Convert.ToInt64(reader["ID"]);
                            area.Cod_Area = Convert.ToString(reader["COD_AREA"]);
                            area.Descricao = Convert.ToString(reader["DESCRICAO"]);
                            area.Ocultar_Acesso = reader["OCULTAR_ACESSO"] != DBNull.Value ? Convert.ToBoolean(reader["OCULTAR_ACESSO"]) : false;
                            listaAreas.Add(area);
                        }

                        retorno.Objeto = listaAreas;
                    }
                }

                retorno.Ok = true;
            }
            catch(Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/AreaAcessoOlaapi/GetAll")]
        public async Task<DataTableResponse<AreaAcessoOlaModel>> GetAll(AreaAcessoOlaDataTableRequest request)
        {           
            List<AreaAcessoOlaModel> retorno = new List<AreaAcessoOlaModel>();
            IOrderedEnumerable<AreaAcessoOlaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_AREA_ACESSO_OLA A ");
                    StringBuilder sqlWhere = new StringBuilder(" WHERE A.ID > 0 ");

                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    bool condicoes = false;

                    #region Filtros                    
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        if (request.search.option == "cod_area")
                        {
                            condicoes = true;
                            sqlWhere.AppendLine(" AND ((UPPER(A.COD_AREA) LIKE UPPER(@SEARCH))) ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                        }
                        else if (request.search.option == "descricao")
                        {
                            condicoes = true;
                            sqlWhere.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                        }                        
                    }

                    if(request.Status_Visibilidade.HasValue)
                    {
                        condicoes = true;
                        sqlWhere.AppendLine(" AND ((UPPER(A.OCULTAR_ACESSO) LIKE UPPER(@OCULTAR))) ");
                        parametros.Add("@OCULTAR", request.Status_Visibilidade.Value);
                    }
                    #endregion

                    #region Count
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                        sql.Append(sqlFrom);
                        if (condicoes)
                            sql.Append(sqlWhere);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }
                    #endregion

                    #region Select
                    using (var com = session.Connection.CreateCommand())
                    {                        
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.COD_AREA, COALESCE(A.OCULTAR_ACESSO, 0) AS OCULTAR_ACESSO, A.OBSERVACAO ");
                        sql.Append(sqlFrom);
                        if (condicoes)
                            sql.Append(sqlWhere);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                var area = new AreaAcessoOlaModel();
                                area.Id = Convert.ToInt64(reader["ID"]);
                                area.Cod_Area = Convert.ToString(reader["COD_AREA"]);
                                area.Descricao = Convert.ToString(reader["DESCRICAO"]);
                                area.Ocultar_Acesso = reader["OCULTAR_ACESSO"] != DBNull.Value ? Convert.ToBoolean(reader["OCULTAR_ACESSO"]) : false;
                                area.Observacao = Convert.ToString(reader["OBSERVACAO"]);
                                retorno.Add(area);
                            }                           
                        }
                    }
                    #endregion

                    //Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 2:     // CODIGO
                                {
                                    if (sortDirection == "asc")
                                        listaOrdenada = retorno.OrderBy(p => p.Cod_Area);
                                    else
                                        listaOrdenada = retorno.OrderByDescending(p => p.Cod_Area);
                                    break;
                                }
                            case 3:     // DESCRICAO
                                {
                                    if (sortDirection == "asc")
                                        listaOrdenada = retorno.OrderBy(p => p.Descricao);
                                    else
                                        listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                                    break;
                                }
                            default:
                                {
                                    listaOrdenada = retorno.OrderBy(p => p.Id);
                                    break;
                                }
                                
                        }
                    }
                }

                //Page e retornando DataTable
                return new DataTableResponse<AreaAcessoOlaModel>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = listaOrdenada,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                //Page e retornando DataTable
                return new DataTableResponse<AreaAcessoOlaModel>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = listaOrdenada,
                    error = ex.Message
                };
            }            
        }

        [HttpPost()]
        [Route("api/AreaAcessoOlaapi/AlterarVisibilidadeRecurso")]
        public async Task<GetApiRetorno> AlterarVisibilidadeRecurso(AlterarVisibilidadeModel value)
        {
            GetApiRetorno retorno = new GetApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    String _ids = $"{String.Join(",", value.ids)}";

                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("UPDATE TB_AREA_ACESSO_OLA SET OCULTAR_ACESSO = @OCULTAR ");
                    sql.AppendLine($"WHERE ID IN ({_ids}) ");

                    parametros.Add("@OCULTAR", value.ocultar);

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);                    
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }
    }
}