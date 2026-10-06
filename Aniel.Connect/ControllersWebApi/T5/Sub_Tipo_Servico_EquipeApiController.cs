using Aniel.Connect.Core;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    
    [Authorize]
    public class Sub_Tipo_Servico_EquipeApiController : BaseApiController
    {
        [HttpGet()]
        [Route("api/Sub_Tipo_Servico_EquipeApi/GetList")]
        public async Task<GetApiRetorno<IEnumerable<ObjBusca<String>>>> GetList(Int32 Cod_Tipo_Serv)
        {
            GetApiRetorno<IEnumerable<ObjBusca<String>>> retorno = new GetApiRetorno<IEnumerable<ObjBusca<String>>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    var lista = await new Conexao<Sub_Tipo_ServicoAniel>(session).select(p => p.Codigo_Tipo == Cod_Tipo_Serv);

                    retorno.Objeto = lista.Select(p => new ObjBusca<String>
                    {
                        Value = p.Codigo_Sub_Tipo,
                        Label = p.Descricao
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

        [HttpPost()]
        [Route("api/SubTipo_Servicoapi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<Int32?, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_SUB_TIPO_SERVICO A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"SITUACAO = 'S'");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"A.COD_TIPO_SERV in ('{request.idpai.ToUpperInvariant()}')");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.COD_SUB_TIPO_SERV = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_SUB_TIPO_SERV) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.COD_SUB_TIPO_SERV, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["COD_SUB_TIPO_SERV"]),
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