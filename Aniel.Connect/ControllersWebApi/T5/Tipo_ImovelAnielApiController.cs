using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Tipo_ImovelAnielApiController : BaseApiController
    {


        [HttpGet()]
        [Route("api/Tipo_ImovelAnielapi/getlist")]
        public async Task<GetApiRetorno<IEnumerable<ObjBusca<Int32>>>> GetList()
        {
            GetApiRetorno<IEnumerable<ObjBusca<Int32>>> retorno = new GetApiRetorno<IEnumerable<ObjBusca<Int32>>>();


            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    var lista = await new Conexao<Tipo_Imovel>(session).queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new ObjBusca<Int32>
                    {
                        Label = p.Descricao,
                        Value = p.Cod_Tipo_Imovel
                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }

        //[HttpPost()]
        //[Route("api/Tipo_ImovelAnielapi/GetCombo")]
        //public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<int> request)
        //{
          
        //    ObjCombo<Int64> retorno = new ObjCombo<Int64>
        //    {
        //        results = new List<IObjComboResult<Int64>>()
        //    };

        //    using (ISession session = HibernateUtil.GetSessionT5())
        //    {
        //        try
        //        {
        //            //Filtros 
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();
        //            StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_IMOVEL A ");


        //            List<String> condicoes = new List<String>();

        //            if (!String.IsNullOrEmpty(Convert.ToString(request.id)))
        //            {
        //                condicoes.Add($"A.COD_TIPO_IMOVEL = '{request.id}'");
        //            }

        //            if (condicoes.Count() > 0)
        //            {
        //                sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
        //            }


        //            //Select
        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder();
        //                sql.AppendLine("SELECT A.COD_TIPO_IMOVEL, A.DESCRICAO ");
        //                sql.AppendLine(sqlFrom.ToString());
        //                sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");





        //                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        retorno.results.Add(new ObjComboResult<long>
        //                        {
        //                            Id = Convert.ToInt64(reader["COD_TIPO_IMOVEL"]),
        //                            Text = Convert.ToString(reader["DESCRICAO"]),
        //                        });
        //                    }

        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return retorno;

        //}

        [HttpPost()]
        [Route("api/Tipo_ImovelAnielapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_IMOVEL A ");

                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.COD_TIPO_IMOVEL = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_TIPO_IMOVEL) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.COD_TIPO_IMOVEL, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["COD_TIPO_IMOVEL"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
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



    }
}