using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Tipo_Servico_EquipeApiController : BaseApiController
    {
        [HttpGet()]
        [Route("api/Tipo_Servico_EquipeApi/GetList")]
        public async Task<GetApiRetorno<List<Tipo_ServicoListaModel>>> GetList(String CodCt = null)
        {
            GetApiRetorno<List<Tipo_ServicoListaModel>> retorno = new GetApiRetorno<List<Tipo_ServicoListaModel>>();
            retorno.Objeto = new List<Tipo_ServicoListaModel>();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        // P/ Fazer:
                        // Qualidade do Serviço

                        sql.AppendLine("SELECT A.COD_TIPO_SERV ,A.DESCRICAO,A.CODCT,B.DESCRICAO AS NATUREZA FROM TB_TIPO_SERVICO_EQUIPE A");
                        sql.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO B");
                        sql.AppendLine("ON A.COD_NATUREZA = B.ID");
                        if (!String.IsNullOrEmpty(CodCt))
                        {
                            sql.AppendLine($" WHERE A.CODCT = '{CodCt}' ");
                        }

                        sql.AppendLine(" ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new Tipo_ServicoListaModel
                                {
                                    Id = Convert.ToInt64(reader["COD_TIPO_SERV"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Natureza = Convert.ToString(reader["NATUREZA"]),
                                    CodCt = Convert.ToString(reader["CODCT"]),
                                });
                            }

                        }

                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }


        public async Task<Response<TimeSpan?>> TempoServicoMinimo(ISession sessionT5)
        {
            TimeSpan? tempoMinimo = (TimeSpan?)null;

            try
            {
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT MIN(T.HORAS) AS HORAS FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL T ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            //    tempoMinimo = reader["HORAS"] != DBNull.Value ? (TimeSpan?)reader["HORAS"] : (TimeSpan?)null;
                            tempoMinimo = Convert.ToDateTime(reader["HORAS"]).TimeOfDay;
                        }
                    }
                }

                return new Response<TimeSpan?>
                {
                    valor = tempoMinimo,
                    result = true,
                };

            }

            catch (Exception ex)
            {
                return new Response<TimeSpan?>
                {
                    valor = tempoMinimo,
                    message = ex.Message,
                    stacktrace = ex.StackTrace,
                    result = false,
                    exception = ex.Message

                };
            }
        }

        public async Task<Response<TimeSpan?>> TempoServico(ISession sessionT5, int CodTipoServ,long? IdTipoImovel, Configuracoes_Gerais config)
        {
            TimeSpan? tempoMinimo = (TimeSpan?)null;

            try
            {
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT FIRST 1 M.HORAS ")
                    .AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ")
                    .AppendLine("WHERE (M.ID_TIPO_SERVICO = @COD_TIPO_SERV) ");
                    parametros.Add("@COD_TIPO_SERV", CodTipoServ);

                    if (IdTipoImovel.HasValue)
                    {
                        sql.AppendLine(" AND M.ID_TIPO_IMOVEL = @ID_TIPO_IMOVEL");
                        parametros.Add("@ID_TIPO_IMOVEL", IdTipoImovel);
                    }

                    sql.AppendLine(" ORDER BY M.ID_TIPO_IMOVEL DESC ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            tempoMinimo = reader["HORAS"] != DBNull.Value ? (TimeSpan)(reader["HORAS"]) : config.Tempo_Execucao_Padrao.Value.TimeOfDay;
                        }
                        else
                        {
                            tempoMinimo = config.Tempo_Execucao_Padrao.Value.TimeOfDay;
                        }
                    }
                }

                return new Response<TimeSpan?>
                {
                    valor = tempoMinimo,
                    result = true,
                };

            }

            catch (Exception ex)
            {
                return new Response<TimeSpan?>
                {
                    valor = tempoMinimo,
                    message = ex.Message,
                    stacktrace = ex.StackTrace,
                    result = false,
                    exception = ex.Message

                };
            }
        }

    }
}
