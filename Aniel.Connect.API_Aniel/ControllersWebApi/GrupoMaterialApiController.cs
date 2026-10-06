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
    public class GrupoMaterialApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Sub Grupo Material
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/GrupoMaterialApi/GetAll")]
        public async Task<ApiResponse<SubGrupoMaterialListModel>> GetAll([FromBody]ApiRequest request)
        {
            List<SubGrupoMaterialListModel> retorno = new List<SubGrupoMaterialListModel>();
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(" SELECT * FROM TB_SUBGRUPO_MATERIAL A ");
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(request.settings.search))
                    {
                        parametros.Add("@descricao", $"%{request.settings.search}%");
                        sql.AppendLine(" WHERE A.DESCRICAO LIKE @descricao ");
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new SubGrupoMaterialListModel
                            {
                                Codigo = !String.IsNullOrEmpty(Convert.ToString(reader["COD_SUBGRUPO_MAT"])) ? Convert.ToString(reader["COD_SUBGRUPO_MAT"]) : "",
                                Descricao = !String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])) ? Convert.ToString(reader["DESCRICAO"]) : "",
                            });
                        }
                    }
                }

                return new ApiResponse<SubGrupoMaterialListModel>()
                {
                    draw = request.settings.length,
                    recordsFiltered = retorno.Count,
                    data = retorno,
                    error = ""
                };

            }
            catch (Exception e)
            {
                return new ApiResponse<SubGrupoMaterialListModel>()
                {
                    draw = request.settings.length,
                    recordsFiltered = retorno.Count,
                    data = retorno,
                    error = e.Message
                };
            }
        }
    }

    public class SubGrupoMaterialListModel
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }
}