using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Office2010.Excel;
using NHibernate;
using NHibernate.Criterion;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
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
    public class PeriodoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/PeriodoApi/GetAll")]
        public async Task<DataTableResponse<PeriodoListaModel>> GetAll(Horario_DataTableRequest request)
        {

            List<PeriodoListaModel> retorno = new List<PeriodoListaModel>();
            IOrderedEnumerable<PeriodoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_TURNO_SERVICO"); //WHERE STATUS = 1  

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

                    sql.AppendLine("SELECT A.ID, A.HORA_INI, A.DESCRICAO, A.HORA_FIM,A.STATUS");
                    sql.AppendLine(" FROM TB_TURNO_SERVICO A");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new PeriodoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Hora_Ini = reader["HORA_INI"] != DBNull.Value ? ((TimeSpan)reader["HORA_INI"]).ToString(@"hh\:mm") : "",
                                Hora_Fim = reader["HORA_FIM"] != DBNull.Value ? ((TimeSpan)reader["HORA_FIM"]).ToString(@"hh\:mm") : "",
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<PeriodoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private PeriodoCadastroModel GetViewObj(Turno_ServicoAniel obj)
        {
            return new PeriodoCadastroModel
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                Hora_Ini = obj.Hora_Ini.HasValue ? obj.Hora_Ini.Value.TimeOfDay.ToString(@"hh\:mm") : "",
                Hora_Fim = obj.Hora_Fim.HasValue ? obj.Hora_Fim.Value.TimeOfDay.ToString(@"hh\:mm") : "",
                Status = obj.Status == Status_Default.Ativo ? true : false,
            };

        }

        private void BindViewToObj(ISession session, Turno_ServicoAniel objBanco, PeriodoCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Descricao = view.Descricao;
            objBanco.Hora_Ini = Convert.ToDateTime(view.Hora_Ini);
            objBanco.Hora_Fim = Convert.ToDateTime(view.Hora_Fim);

            if (view.Id == 0)
                objBanco.Status = Status_Default.Ativo;
            else
                objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;

        }


        [HttpGet()]
        [Route("api/PeriodoApi/FindById")]
        public async Task<GetApiRetorno<PeriodoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<PeriodoCadastroModel> retorno = new GetApiRetorno<PeriodoCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Turno_ServicoAniel>(session))
            {
                try
                {
                    var Turno = await repository.selectFirst(p => p.Id == Id);
                    retorno.Objeto = Turno == null ? null : GetViewObj(Turno);

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
        [Route("api/PeriodoApi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_HORARIO_CONNECT WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/PeriodoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<PeriodoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Turno_ServicoAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Turno_ServicoAniel>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            modelo = new Turno_ServicoAniel
                            {
                            };
                        }



                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/PeriodoApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_TURNO_SERVICO A ");

                    List<String> condicoes = new List<String>();

                    //if (request.soativos)
                    //{
                    //    condicoes.Add($"A.STATUS = 1");
                    //}

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 1");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE  ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
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

        public static Turno_ServicoAniel BuscarTurno(ISession session, DateTime dataAgendamento, TimeSpan? TempoExecucao)
        {
            using (var repository = new Conexao<Turno_ServicoAniel>(session))
            {

                var turnos = repository.select().Result;
                var dataFimAgendamento = dataAgendamento.Add(TempoExecucao.Value).TimeOfDay;
                var turno = turnos.FirstOrDefault(p => dataAgendamento.TimeOfDay >= p.Hora_Ini?.TimeOfDay && dataAgendamento.TimeOfDay <= p.Hora_Fim?.TimeOfDay);
                
                return turno;
            }
        }

        public static Turno_ServicoAniel BuscarTurno(List<Turno_ServicoAniel> turnos, DateTime dataAgendamento, TimeSpan? TempoExecucao)
        {
            var turno = turnos.FirstOrDefault(p => dataAgendamento.TimeOfDay >= p.Hora_Ini?.TimeOfDay && dataAgendamento.TimeOfDay <= p.Hora_Fim?.TimeOfDay);
            return turno;

        }

        public static bool Between(DateTime? input, DateTime? date1, DateTime? date2)
        {
            return (input.Value.TimeOfDay >= date1.Value.TimeOfDay && input.Value.TimeOfDay <= date2.Value.TimeOfDay);
        }

    }
}