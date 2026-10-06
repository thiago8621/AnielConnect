using Aniel.Connect.API_Aniel.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class TipoImovelApiController : ApiController
    {
        /// <summary>
        /// Retorna a Listagem de Tipos de Imóvel
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/TipoImovelapi/GetAll")]
        public async Task<ApiResponseObj<List<TipoImovelListModel>>> GetAll(TipoImovelApiRequest request)
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
                List<TipoImovelListModel> retorno = new List<TipoImovelListModel>();

                using (ISession session = HibernateUtil.GetSessionT5())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFROM = new StringBuilder(" FROM TB_TIPO_IMOVEL A");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.settings.search))
                    {
                        sqlFROM.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.settings.search));
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.COD_TIPO_IMOVEL, A.DESCRICAO, A.DIGITO ");
                        sql.AppendLine(sqlFROM.ToString());

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new TipoImovelListModel
                                {
                                    CodigoTipoImovel = Convert.ToInt64(reader["COD_TIPO_IMOVEL"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Digito = Convert.ToString(reader["DIGITO"]),
                                });
                            }
                        }
                    }
                }
                //Page e retornando DataTable
                return new ApiResponseObj<List<TipoImovelListModel>>
                {
                    ok = true,
                    data = retorno,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                return new ApiResponseObj<List<TipoImovelListModel>>
                {
                    error = ex.Message,
                    ok = false
                };
            }
        }             
    }
}