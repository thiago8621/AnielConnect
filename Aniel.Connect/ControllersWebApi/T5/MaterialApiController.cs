using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{

    [Authorize]
    public class MaterialApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/MaterialApi/GetComboGrupo")]
        public async Task<ObjCombo<String>> GetComboGrupo(RequestObjComboConfig<String, MaterialConfigCombo> request)
        {
            ObjCombo<String> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_SUBGRUPO_MATERIAL A ");

                    List<String> condicoes = new List<String>();

                    //if (!String.IsNullOrEmpty(request.id))
                    //{
                    //    condicoes.Add("A.CODMAT = @ID");
                    //    parametros.Add("@ID", request.id);
                    //}

                    //if(!String.IsNullOrEmpty(request.config.complementar))
                    //{
                    //    sqlFrom.Append(@" INNER JOIN TB_CODIGO_COMPLEMENTAR B 
                    //                      ON(B.CODMAT = A.CODMAT)
                    //                      ");

                    //    condicoes.Add($"( B.CODCPL IN ('{request.config.complementar}') )");
                    //}

                    ////informe classficacao com valor com aspas 'I','R','B'
                    //if (!String.IsNullOrEmpty(request.config.classificacao))
                    //{
                    //    condicoes.Add($"( A.CLASSIFICACAO IN ({request.config.classificacao}) )");
                    //}

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        //var sqlCODMAT = request.q.Length <= 12 ? $" OR (UPPER(A.CODMAT) LIKE '{StringService.Truncate(request.q.ToUpperInvariant(), 12)}%')" : "";

                        //condicoes.Add($"((UPPER(A.DESCRICAO) LIKE '{StringService.Truncate(request.q.ToUpperInvariant(), 55)}%') {sqlCODMAT})");

                        sqlFrom.AppendLine(" WHERE UPPER(A.DESCRICAO) LIKE @DESCRICAO ");
                        parametros.Add("@DESCRICAO", '%' + request.q.ToUpper() + '%');
                    }

                    //if (condicoes.Count() > 0)
                    //{
                    //    sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    //}

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_SUBGRUPO_MAT) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.COD_SUBGRUPO_MAT, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new MaterialListaModel
                                {
                                    Id = Convert.ToString(reader["COD_SUBGRUPO_MAT"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),

                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/MaterialApi/GetComboFamilia")]

         public async Task<ObjCombo<String>> GetComboFamilia(RequestObjComboConfig<String, MaterialConfigCombo> request)
         {
            ObjCombo<String> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FAMILIA_MATERIAL A ");

                    List<String> condicoes = new List<String>();

                    //if (!String.IsNullOrEmpty(request.id))
                    //{
                    //    condicoes.Add("A.CODMAT = @ID");
                    //    parametros.Add("@ID", request.id);
                    //}

                    //if(!String.IsNullOrEmpty(request.config.complementar))
                    //{
                    //    sqlFrom.Append(@" INNER JOIN TB_CODIGO_COMPLEMENTAR B 
                    //                      ON(B.CODMAT = A.CODMAT)
                    //                      ");

                    //    condicoes.Add($"( B.CODCPL IN ('{request.config.complementar}') )");
                    //}

                    ////informe classficacao com valor com aspas 'I','R','B'
                    //if (!String.IsNullOrEmpty(request.config.classificacao))
                    //{
                    //    condicoes.Add($"( A.CLASSIFICACAO IN ({request.config.classificacao}) )");
                    //}

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        //var sqlCODMAT = request.q.Length <= 12 ? $" OR (UPPER(A.CODMAT) LIKE '{StringService.Truncate(request.q.ToUpperInvariant(), 12)}%')" : "";

                        //condicoes.Add($"((UPPER(A.DESCRICAO) LIKE '{StringService.Truncate(request.q.ToUpperInvariant(), 55)}%') {sqlCODMAT})");

                        sqlFrom.AppendLine(" WHERE UPPER(A.DESCRICAO) LIKE @DESCRICAO ");
                        parametros.Add("@DESCRICAO", '%' + request.q.ToUpper() + '%');
                    }

                    //if (condicoes.Count() > 0)
                    //{
                    //    sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    //}

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.FAMILIA) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.FAMILIA, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new MaterialListaModel
                                {
                                    Id = Convert.ToString(reader["FAMILIA"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),

                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/MaterialApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboConfig<String, MaterialConfigCombo> request)
        {
            ObjCombo<String> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MATERIAL A ");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add("A.CODMAT = @ID");
                        parametros.Add("@ID", request.id);
                    }

                    if(!String.IsNullOrEmpty(request.config.complementar))
                    {
                        sqlFrom.Append(@" INNER JOIN TB_CODIGO_COMPLEMENTAR B 
                                          ON(B.CODMAT = A.CODMAT)
                                          ");

                        condicoes.Add($"( B.CODCPL IN ('{request.config.complementar}') )");
                    }

                    //informe classficacao com valor com aspas 'I','R','B'
                    if (!String.IsNullOrEmpty(request.config.classificacao))
                    {
                        condicoes.Add($"( A.CLASSIFICACAO IN ({request.config.classificacao}) )");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        var sqlCODMAT = request.q.Length <= 12 ? $" OR (UPPER(A.CODMAT) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 12)}%')" : "";

                        condicoes.Add($"((UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 55)}%') {sqlCODMAT})");
                    }

                    condicoes.Add(" A.TIPO <> 'I' ");

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODMAT) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.CODMAT, A.DESCRICAO, A.UNID, A.VALOR, A.SUBCOD, A.SUBCOD_EXCLUSIVO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new MaterialListaModel
                                {
                                    Id = Convert.ToString(reader["CODMAT"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    SubCod = Convert.ToString(reader["SUBCOD"]),
                                    SubCod_Exclusivo = Convert.ToString(reader["SUBCOD_EXCLUSIVO"]),
                                    Valor = reader["VALOR"] != DBNull.Value ? Convert.ToDouble(reader["VALOR"]): 0,
                                    Unidade = Convert.ToString(reader["UNID"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/MaterialApi/GetComboMaterial")]
        public async Task<ObjCombo<String>> GetComboMaterial(RequestObjCombo request)
        {
            ObjCombo<string> retorno = new ObjCombo<String> { results = new List<IObjComboResult<String>>()};
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    #region incializacao de objetos
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlFrom = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();
                    #endregion
                    //Classificações Possíveis:
                    //Instalação = I, Defeito = D, Def.de Estoque = E, Retirada = R, Reaproveitado = B, Sucata = S
                    sqlWhere.Append("WHERE A. CLASSIFICACAO = 'I' ");

                    if(request != null && !String.IsNullOrEmpty(request.q))
                    {
                        sqlWhere.Append($"AND UPPER(A.DESCRICAO) LIKE '%{request.q.ToUpperInvariant()}%' ");
                    }

                    sqlFrom.Append("FROM TB_MATERIAL A");

                    sql.AppendLine("SELECT A.CODMAT, A.DESCRICAO" + "\r\n" + " " + sqlFrom.ToString() + "\r\n" + " " + sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.results.Add(new ObjComboResult<String>
                            {
                                Id = Convert.ToString(reader["CODMAT"]),
                                Text = Convert.ToString(reader["DESCRICAO"])
                            });
                        }
                    }
                }
            }
            catch (Exception error)
            {
                _ = error.InnerException == null? error.Message + "\r\n" + error.InnerException.Message:error.Message;
            }
            return retorno;
        }
    }
}
