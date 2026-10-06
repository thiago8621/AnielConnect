using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
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
    public class FluxoAtendimentoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/FluxoAtendimentoApi/GetAll")]
        public async Task<DataTableResponse<Tipo_AtendimentoListaModel>> GetAll(Tipo_Atendimento_DataTableRequest request)
        {
            List<Tipo_AtendimentoListaModel> retorno = new List<Tipo_AtendimentoListaModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_FLUXO A");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }
                // Consulta
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"SELECT A.CODIGO, A.SEQUENCIA, A.DESCRICAO, A.SETOR_DEPARTAMENTO, 
                                                                       A.TEMPO_EXC, 
                                                                       A.ENCERRA_ATENDIMENTO, A.ETAPA_DECISORIA, 
                                                                       A.PROXIMA_ETAPA, A.STATUS FROM FLUXO A");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Tipo_AtendimentoListaModel
                            {
                                Codigo = Convert.ToString(reader["CODIGO"]),
                                Sequencia = Convert.ToInt64(reader["SEQUENCIA"]),
                                Descricao = Convert.ToString(reader["DESCROICAO"]),
                                Setor = Convert.ToString(reader["SETOR_DEPARTAMENTO"]),
                                //Contrato = Convert.ToString(reader["CONTRATO"]),
                                //Cod_Encerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                                Tempo_Exc = Convert.ToString(reader["TEMPO_EXC"]),
                                Encerrar_Atendimento = Convert.ToBoolean(reader["ENCERRAR_ATENDIMENTO"]),
                                Etapa_Decisoria = Convert.ToBoolean(reader["ETAPA_DECISORIA"]),
                                Proxima_Etapa = Convert.ToString(reader["PROXIMA_ETAPA"]),




                            });
                        }
                    }
                }
            }




            return new DataTableResponse<Tipo_AtendimentoListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno

            };
        }



        [HttpPost()]
        [Route("api/FluxoAtendimentoApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FLUXO_ATENDIMENTO A WHERE  ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1 ");
                    }

                    

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($" UPPER(A.ID) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($" UPPER(A.ID_TIPO_ATENDIMENTO) = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(  UPPER(A.SEQUENCIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 4)}%' OR " +
                                      $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    sqlFrom.AppendLine($" GROUP BY a.sequencia ");

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.SEQUENCIA) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.SEQUENCIA ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.SEQUENCIA)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["SEQUENCIA"]),
                                    Text = $"{Convert.ToString(reader["SEQUENCIA"])} "
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
        [Route("api/FluxoAtendimentoApi/GetComboEtapa")]
        public async Task<ObjCombo<String>> GetComboEtapa(RequestObjComboAvo<String, String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FLUXO_ATENDIMENTO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1 ");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.ID) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"UPPER(A.SEQUENCIA) = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.idavo))
                    {
                        condicoes.Add($"UPPER(A.ID_TIPO_ATENDIMENTO) = '{request.idavo.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.SEQUENCIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 4)}%' OR " +
                                      $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID,A.SEQUENCIA,A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.SEQUENCIA)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["SEQUENCIA"])} - {Convert.ToString(reader["DESCRICAO"])}"
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

        [HttpGet()]
        [Route("api/FluxoApi/FindById")]
        public async Task<GetApiRetorno<Tipo_AtendimentoCadastroModel>> FindById(String Codigo)

        {
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<FluxoAtendimento>(session))
            {
                FluxoAtendimento Tipo_AtendimentoModel = (FluxoAtendimento)await repository.select(p => p.Codigo == Codigo);
                Tipo_AtendimentoCadastroModel retorno = new Tipo_AtendimentoCadastroModel();
                return new GetApiRetorno<Tipo_AtendimentoCadastroModel>
                {

                    Ok = true,
                    Mensagem = "",
                    InnerException = "",
                    Objeto = retorno,

                };
            }




        }


        [Route("api/FluxoAtendimentoapi/ExcluirGridEncerramento")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridEncerramento(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<EncerramentoAtendimento>(session))
                    {

                        EncerramentoAtendimento modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

    }
}