using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class FamiliaMaterialApiController : BaseApiController
    {
        /// <summary>
        /// Retorna Todas as Familias de Materiais
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FamiliaMaterialApiController/GetAll")]
        public async Task<ApiResponseObj<List<FamiliaMaterialModel>>> GetAll(ApiRequest request)
        {
            List<FamiliaMaterialModel> retorno = new List<FamiliaMaterialModel>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (var session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT FAMILIA, DESCRICAO, SINTETICA ");
                    sql.AppendLine(" FROM TB_FAMILIA_MATERIAL ");

                    if (!string.IsNullOrEmpty(request.settings.search))
                    {
                        parametros.Add("DESCRICAO", request.settings.search);
                        sql.AppendLine(" WHERE DESCRICAO = @DESCRICAO");
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new FamiliaMaterialModel { Descricao = reader["DESCRICAO"].ToString(), Familia = reader["FAMILIA"].ToString(), Sintetica = reader["SINTETICA"].ToString() == "N" ? false : true });
                        }
                    }

                    return new ApiResponseObj<List<FamiliaMaterialModel>> { data = retorno, ok = true };
                }
            }
            catch (Exception e)
            {
                return new ApiResponseObj<List<FamiliaMaterialModel>> { error = e.Message, ok = false };
            }
        }

        /// <summary>
        /// Grava ou Atualiza Uma Familia Material
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FamiliaMaterialApiController/Gravar")]
        public async Task<ApiResponseObj<string>> Gravar(FamiliaMaterialRequest request)
        {
            List<FamiliaMaterialModel> retorno = new List<FamiliaMaterialModel>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            bool update = false;
            string sintetica = request.Sintetica ? "S" : "N";

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                using (var session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {

                    parametros.Add("FAMILIA", request.Familia);
                    parametros.Add("DESCRICAO", request.Descricao);
                    parametros.Add("SINTETICA", sintetica);

                    StringBuilder sql = new StringBuilder("SELECT FAMILIA, DESCRICAO, SINTETICA ");
                    sql.AppendLine(" FROM TB_FAMILIA_MATERIAL WHERE FAMILIA = @FAMILIA");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            update = true;
                        }
                    }

                    if (!update)
                    {
                        sql = new StringBuilder(" INSERT INTO TB_FAMILIA_MATERIAL (FAMILIA,DESCRICAO,SINTETICA) ");
                        sql.AppendLine(" VALUES (@FAMILIA,@DESCRICAO,@SINTETICA) ");

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }
                    else
                    {
                        sql = new StringBuilder(" UPDATE TB_FAMILIA_MATERIAL SET DESCRICAO = @DESCRICAO , SINTETICA = @SINTETICA ");
                        sql.AppendLine(" WHERE FAMILIA = @FAMILIA ");

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }

                    return new ApiResponseObj<string> { data = "Gravado com sucesso", ok = true };
                }
            }
            catch (Exception e)
            {
                return new ApiResponseObj<string> { error = "Houve um erro : " + e.Message, ok = false };
            }
        }
    }
}