using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
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
    public class Dashboard_AtendimentoApiController : BaseApiController
    {


        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/GetAtendimentoPendente")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetAtendimentoPendente()
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(A.ID) CO FROM TB_ATENDIMENTO A ");

                    sql.AppendLine(" LEFT JOIN ASPNETUSERS B ");
                    sql.AppendLine(" ON(A.ID_RESPONSAVEL = B.ID) ");

                    //sql.AppendLine(" LEFT JOIN TB_EQUIPE C ");
                    //sql.AppendLine(" ON(C.ID_FUNCIONARIO = B.ID_FUNCIONARIO) ");

                    sql.AppendLine(" WHERE A.STATUS_ATENDIMENTO IN (0,1,2) AND A.ID_RESPONSAVEL = @USUARIO ");

                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto = new Dashboard_Atendimento_Models
                            {
                                Atendimentos_Pendentes = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/GetAtendimentosMesUsuario")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetAtendimentosMesUsuario(RequestDashboardUser request)
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine(" SELECT COUNT(A.ID) CO FROM TB_ATENDIMENTO A ");

                    sql.AppendLine(" LEFT JOIN ASPNETUSERS B ");
                    sql.AppendLine(" ON(A.ID_RESPONSAVEL = B.ID) ");

                    sql.AppendLine(" WHERE A.ID_RESPONSAVEL = @USUARIO ");
                    sql.AppendLine(" AND A.DATAHORACRIACAO BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL ");

                    if (request != null && request.FiltrarEmAberto)
                        sql.AppendLine(" AND A.STATUS_ATENDIMENTO IN (0,5,2) ");

                    if (request != null && request.FiltrarConcluidos)
                        sql.AppendLine(" AND A.STATUS_ATENDIMENTO = 4 ");

                    DateTime data_atual = DateTime.Now;
                    DateTime _data = data_atual;

                    for (int i = data_atual.Day; i > 1; i--)
                    {
                        _data = _data.AddDays(-1);
                    }

                    DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                    parametros.Add("@USUARIO", User.Identity.GetUserId());
                    parametros.Add("@DATA_ATUAL", data_atual);
                    parametros.Add("@DATA_INCIO_MES", data_inicio_mes);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto = new Dashboard_Atendimento_Models
                            {
                                Total_Atendimentos_Mes_Usuario = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/GetAtendimentosMesGeral")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetAtendimentosMesGeral(RequestDashboardUser request)
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(A.ID) CO FROM TB_ATENDIMENTO A");

                    sql.AppendLine(" WHERE A.DATAHORACRIACAO BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL ");

                    if (request != null && request.FiltrarEmAberto)
                        sql.AppendLine(" AND A.STATUS_ATENDIMENTO IN (0,5,2) ");

                    else if (request != null && request.FiltrarConcluidos)
                        sql.AppendLine(" AND A.STATUS_ATENDIMENTO = 4 ");

                    else if (request != null && request.FiltrarAtendimentosFila)
                        sql.AppendLine(" AND A.STATUS_ATENDIMENTO IN (0,2) ");

                    else if (request != null && request.FiltrarAtendimentosEmAndamento)
                        sql.AppendLine(" AND A.STATUS_ATENDIMENTO = 5 ");

                    else if (request != null && request.FiltrarAtendimentosEncerrados)
                        sql.AppendLine(" AND A.STATUS_ATENDIMENTO = 4 ");

                    string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");

                    parametros.Add("@DATA_ATUAL", dataAtual);
                    parametros.Add("@DATA_INCIO_MES", dataInicio);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto = new Dashboard_Atendimento_Models
                            {
                                Total_Atendimentos_Mes_Geral = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/GetTempoMedioAtendimento")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetTempoMedioAtendimento()
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT DATAHORACRIACAO, DATA_HORA_FECHAMENTO FROM TB_ATENDIMENTO ");

                    sql.AppendLine(" WHERE DATAHORACRIACAO IS NOT NULL ");
                    sql.AppendLine(" AND DATA_HORA_FECHAMENTO IS NOT NULL ");
                    sql.AppendLine(" AND DATAHORACRIACAO BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL ");

                    List<TimeSpan> list_tempo_medio = new List<TimeSpan>();

                    DateTime? data_hora_criacao;
                    DateTime? datahora_fechamento;

                    TimeSpan tempo_atendimento;

                    string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");

                    parametros.Add("@DATA_ATUAL", dataAtual);
                    parametros.Add("@DATA_INCIO_MES", dataInicio);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {


                        while (await reader.ReadAsync())
                        {
                            data_hora_criacao = reader["DATAHORACRIACAO"] != null ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                            datahora_fechamento = reader["DATA_HORA_FECHAMENTO"] != null ? Convert.ToDateTime(reader["DATA_HORA_FECHAMENTO"]) : (DateTime?)null;

                            tempo_atendimento = (datahora_fechamento.HasValue && data_hora_criacao.HasValue) ? (datahora_fechamento.Value - data_hora_criacao.Value) : new TimeSpan(0, 0, 0);

                            list_tempo_medio.Add(tempo_atendimento);
                        }
                    }

                    //foreach (var tempo in list_tempo_medio)
                    //{
                    //    tempo_atend_total += tempo;
                    //}

                    //var media = tempo_atend_total.TotalMinutes / list_tempo_medio.Count();

                    retorno.Objeto = new Dashboard_Atendimento_Models
                    {
                        Tempo_Medio_Atendimento = list_tempo_medio.Count != 0
                                                  ?
                                                      (Tempo_Medio(list_tempo_medio).TotalDays >= 1
                                                       ?
                                                       Tempo_Medio(list_tempo_medio).TotalDays >= 1 && Tempo_Medio(list_tempo_medio).TotalDays <= 2
                                                           ?
                                                           Tempo_Medio(list_tempo_medio).ToString("dd") + " Dia " + Tempo_Medio(list_tempo_medio).ToString(@"hh\:mm")
                                                           :
                                                           Tempo_Medio(list_tempo_medio).ToString("dd") + " Dias " + Tempo_Medio(list_tempo_medio).ToString(@"hh\:mm")
                                                       :
                                                       Tempo_Medio(list_tempo_medio).ToString(@"hh\:mm"))
                                                  :
                                                  null
                    };
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/GetTempoMedioFila")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetTempoMedioFila()
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT DATAHORACRIACAO, DATAHORA_VINC_RESP, ID FROM TB_ATENDIMENTO ");

                    sql.AppendLine(" WHERE DATAHORACRIACAO IS NOT NULL ");
                    sql.AppendLine(" AND DATAHORA_VINC_RESP IS NOT NULL ");
                    sql.AppendLine(" AND DATAHORACRIACAO BETWEEN @DATA_INCIO_MES AND @DATA_ATUAL ");

                    List<TimeSpan> list_tempo_medio = new List<TimeSpan>();

                    DateTime? data_hora_criacao;
                    DateTime? dataHoraVincResp;

                    TimeSpan tempo_atendimento;

                    string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");

                    parametros.Add("@DATA_ATUAL", dataAtual);
                    parametros.Add("@DATA_INCIO_MES", dataInicio);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {

                        while (await reader.ReadAsync())
                        {
                            data_hora_criacao = reader["DATAHORACRIACAO"] != null ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                            dataHoraVincResp = reader["DATAHORA_VINC_RESP"] != null ? Convert.ToDateTime(reader["DATAHORA_VINC_RESP"]) : (DateTime?)null;

                            tempo_atendimento = (dataHoraVincResp.HasValue && data_hora_criacao.HasValue) ? (dataHoraVincResp.Value - data_hora_criacao.Value) : new TimeSpan(0, 0, 0);

                            list_tempo_medio.Add(tempo_atendimento);
                        }
                    }


                    retorno.Objeto = new Dashboard_Atendimento_Models
                    {
                        Tempo_Medio_Atendimento = list_tempo_medio.Count != 0
                                                  ?
                                                      (Tempo_Medio(list_tempo_medio).TotalDays >= 1
                                                       ?
                                                       Tempo_Medio(list_tempo_medio).TotalDays >= 1 && Tempo_Medio(list_tempo_medio).TotalDays <= 2
                                                           ?
                                                           Tempo_Medio(list_tempo_medio).ToString("dd") + " Dia " + Tempo_Medio(list_tempo_medio).ToString(@"hh\:mm")
                                                           :
                                                           Tempo_Medio(list_tempo_medio).ToString("dd") + " Dias " + Tempo_Medio(list_tempo_medio).ToString(@"hh\:mm")
                                                       :
                                                       Tempo_Medio(list_tempo_medio).ToString(@"hh\:mm"))
                                                  :
                                                  null
                    };
                    retorno.Ok = true;

                }
            }

            return retorno;
        }



        private TimeSpan Tempo_Medio(IEnumerable<TimeSpan> timeSpans)
        {
            //IEnumerable<Int64> ticksPerTimeSpan = timeSpans.Select(t => t.Ticks);
            //Double averageTicks = ticksPerTimeSpan.Average();
            //Int64 averageTicksLong = Convert.ToInt64(averageTicks);

            //TimeSpan averageTimeSpan = TimeSpan.FromTicks(averageTicksLong);

            TimeSpan tempo_atend_total = new TimeSpan();

            foreach (var tempo in timeSpans)
            {
                tempo_atend_total += tempo;
            }

            var media = tempo_atend_total.TotalMinutes / timeSpans.Count();

            return TimeSpan.FromMinutes(media);
        }

        [HttpPost()]
        //[AcceptVerbs("Get", "Post")]
        [Route("api/Dashboard_Atendimentoapi/GetAtendimentoPendenteEquipe")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetAtendimentoPendenteEquipe()
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(A.ID) CO FROM TB_ATENDIMENTO A ");

                    sql.AppendLine(" LEFT JOIN ASPNETUSERS B ");
                    sql.AppendLine(" ON(A.ID_RESPONSAVEL = B.ID) ");

                    //sql.AppendLine(" LEFT JOIN TB_EQUIPE C ");
                    //sql.AppendLine(" ON(C.ID_FUNCIONARIO = B.ID_FUNCIONARIO) ");

                    sql.AppendLine(" WHERE A.STATUS_ATENDIMENTO IN (0,5,2) AND A.ID_RESPONSAVEL = @USUARIO ");


                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto = new Dashboard_Atendimento_Models
                            {
                                Atendimentos_Pendentes_Equipe = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/GetA_Iniciar")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetA_Iniciar()
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(A.ID) CO FROM TB_ATENDIMENTO A ");

                    //sql.AppendLine(" LEFT JOIN ASPNETUSERS B ");
                    //sql.AppendLine(" ON(A.ID_RESPONSAVEL = B.ID) ");

                    //sql.AppendLine(" LEFT JOIN TB_EQUIPE C ");
                    //sql.AppendLine(" ON(C.ID_FUNCIONARIO = B.ID_FUNCIONARIO) ");

                    sql.AppendLine(" WHERE A.STATUS_ATENDIMENTO = '0' ");



                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto = new Dashboard_Atendimento_Models
                            {
                                A_Iniciar = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/GetEmAtendimento")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetEmAtendimento()
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(A.ID) CO FROM TB_ATENDIMENTO A ");

                    //sql.AppendLine(" LEFT JOIN ASPNETUSERS B ");
                    //sql.AppendLine(" ON(A.ID_RESPONSAVEL = B.ID) ");

                    sql.AppendLine(" WHERE A.STATUS_ATENDIMENTO = '5' ");



                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto = new Dashboard_Atendimento_Models
                            {
                                Em_Atendimento = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/GetAguardando")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetAguardando()
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(A.ID) CO FROM TB_ATENDIMENTO A ");

                    //sql.AppendLine(" LEFT JOIN ASPNETUSERS B ");
                    //sql.AppendLine(" ON(A.ID_RESPONSAVEL = B.ID) ");

                    sql.AppendLine(" WHERE A.STATUS_ATENDIMENTO = '2' ");




                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto = new Dashboard_Atendimento_Models
                            {
                                Aguardando = Convert.ToString(reader["CO"])
                            };

                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/GetSLAGraphic")]
        public async Task<GetApiRetorno<Dashboard_Atendimento_Models>> GetSLAGraphic()
        {

            GetApiRetorno<Dashboard_Atendimento_Models> retorno = new GetApiRetorno<Dashboard_Atendimento_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                List<FlagsAtendimento> _flags = new List<FlagsAtendimento>();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.DATAHORACRIACAO, A.DATAHORA_INDICADOR,A.DATA_HORA_FECHAMENTO ");
                    sql.AppendLine(" FROM TB_ATENDIMENTO A ");

                    sql.AppendLine(" LEFT JOIN TB_CLIENTE B");
                    sql.AppendLine(" ON(A.ID_CLIENTE = B.ID) ");

                    sql.AppendLine(" WHERE  "); /*A.ID_RESPONSAVEL = @USUARIO AND*/

                    sql.AppendLine(" A.DATAHORACRIACAO IS NOT NULL ");
                    sql.AppendLine(" AND A.DATAHORA_INDICADOR IS NOT NULL ");


                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {

                        while (await reader.ReadAsync())
                        {

                            DateTime? DataHora_Criacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                            DateTime? DataHora_Indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                            DateTime? DataHora_Fechamento = reader["DATA_HORA_FECHAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_FECHAMENTO"]) : (DateTime?)null;
                            if (DataHora_Indicador.HasValue)
                            {
                                var totais = (DataHora_Indicador - DataHora_Criacao).Value.TotalMinutes;

                                var agora = new Double();

                                if (!DataHora_Fechamento.HasValue)
                                    agora = (DataHora_Indicador - DateTime.Now).Value.TotalMinutes;
                                else
                                    agora = (DataHora_Indicador - DataHora_Fechamento).Value.TotalMinutes;

                                var porcentagem = ((100 - (agora / totais) * 100));
                                var valor_calculado = Math.Round(porcentagem, 2);

                                var desc_cores = Desc_Cores_SLA(valor_calculado);

                                if (_flags.Count == 0)
                                {
                                    desc_cores.Valor++;
                                    _flags.Add(desc_cores);
                                }

                                else
                                {

                                    var cont = _flags.FirstOrDefault(p => p.Cor == desc_cores.Cor);
                                    if (cont != null)
                                    {
                                        cont.Valor++;
                                        _flags.Remove(_flags.FirstOrDefault(p => p.Cor == desc_cores.Cor));
                                        _flags.Add(cont);
                                    }
                                    else
                                    {
                                        desc_cores.Valor++;
                                        _flags.Add(desc_cores);
                                    }

                                }

                            }



                        }

                    }



                    retorno.Objeto = new Dashboard_Atendimento_Models
                    {
                        Flags_Atendimento = _flags,
                        Dentro_Prazo = Convert.ToString(_flags.Where(p => p.Cor == "#00FF00").FirstOrDefault()?.Valor),
                        Atencao = Convert.ToString(_flags.Where(p => p.Cor == "#ffd700").FirstOrDefault()?.Valor),
                        Vencimento_Proximo = Convert.ToString(_flags.Where(p => p.Cor == "#FFA500").FirstOrDefault()?.Valor),
                        Risco_Perda_Prazo = Convert.ToString(_flags.Where(p => p.Cor == "#FF0000").FirstOrDefault()?.Valor),
                        Prazo_Vencido = Convert.ToString(_flags.Where(p => p.Cor == "#000000").FirstOrDefault()?.Valor)
                    };

                    retorno.Ok = true;

                }
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/ObterEncDentroSLA")]
        public async Task<GetApiRetorno<ResponseInfoSLA>> ObterEncDentroSLA()
        {

            GetApiRetorno<ResponseInfoSLA> retorno = new GetApiRetorno<ResponseInfoSLA>();

            using (ISession session = HibernateUtil.GetSession())
            {

                try
                {
                    List<FlagsAtendimento> _flags = new List<FlagsAtendimento>();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");

                    int totalAtendEncerr = 0;
                    int totalDentroPrazo = 0;

                    StringBuilder sqlFrom = new StringBuilder();

                    sqlFrom.AppendLine(" FROM TB_ATENDIMENTO A ");
                    sqlFrom.AppendLine(" WHERE  ");
                    sqlFrom.AppendLine(" A.DATAHORACRIACAO IS NOT NULL ");
                    sqlFrom.AppendLine(" AND A.DATAHORA_INDICADOR IS NOT NULL ");
                    sqlFrom.AppendLine(" AND A.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAATUAL ");
                    sqlFrom.AppendLine(" AND A.STATUS_ATENDIMENTO = 4 ");

                    parametros.Add("@DATAINICIO", dataInicio);
                    parametros.Add("@DATAATUAL", dataAtual);

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                totalAtendEncerr = reader["CO"] != DBNull.Value ? Convert.ToInt32(reader["CO"]) : 0;
                            }
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.DATAHORACRIACAO, A.DATAHORA_INDICADOR, A.DATA_HORA_FECHAMENTO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                DateTime? DataHora_Criacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                                DateTime? DataHora_Indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                                DateTime? DataHora_Fechamento = reader["DATA_HORA_FECHAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_FECHAMENTO"]) : (DateTime?)null;

                                if (DataHora_Indicador.HasValue)
                                {
                                    var totais = (DataHora_Indicador - DataHora_Criacao).Value.TotalMinutes;

                                    var agora = new Double();

                                    if (!DataHora_Fechamento.HasValue)
                                        agora = (DataHora_Indicador - DateTime.Now).Value.TotalMinutes;
                                    else
                                        agora = (DataHora_Indicador - DataHora_Fechamento).Value.TotalMinutes;

                                    var porcentagem = ((100 - (agora / totais) * 100));
                                    var valor_calculado = Math.Round(porcentagem, 2);

                                    //dentro do prazo
                                    if (valor_calculado > 0 && valor_calculado <= 25)
                                        totalDentroPrazo++;

                                }
                            }
                        }

                        var porcentagemEncerrados = 0.0;

                        if (totalAtendEncerr == totalDentroPrazo)
                        {
                            retorno.Objeto = new ResponseInfoSLA
                            {
                                Porcentagem = "100%",
                                QuantidadeEnc = totalAtendEncerr
                            };
                        }
                        else
                        {
                            porcentagemEncerrados = (totalDentroPrazo * 100) / totalAtendEncerr;

                            var teste = totalAtendEncerr - totalDentroPrazo;
                            retorno.Objeto = new ResponseInfoSLA
                            {
                                Porcentagem = $"{Math.Round(porcentagemEncerrados, 2)}%",
                                QuantidadeEnc = totalDentroPrazo
                            };
                        }

                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    return retorno;
                }
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/ObterEncForaSLA")]
        public async Task<GetApiRetorno<ResponseInfoSLA>> ObterEncForaSLA()
        {

            GetApiRetorno<ResponseInfoSLA> retorno = new GetApiRetorno<ResponseInfoSLA>();

            using (ISession session = HibernateUtil.GetSession())
            {

                List<FlagsAtendimento> _flags = new List<FlagsAtendimento>();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");

                int totalAtendEncerr = 0;
                int totalForaPrazo = 0;

                StringBuilder sqlFrom = new StringBuilder();

                sqlFrom.AppendLine(" FROM TB_ATENDIMENTO A ");
                sqlFrom.AppendLine(" WHERE  ");
                sqlFrom.AppendLine(" A.DATAHORACRIACAO IS NOT NULL ");
                sqlFrom.AppendLine(" AND A.DATAHORA_INDICADOR IS NOT NULL ");
                sqlFrom.AppendLine(" AND A.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAATUAL ");
                sqlFrom.AppendLine(" AND A.STATUS_ATENDIMENTO = 4 ");

                parametros.Add("@DATAINICIO", dataInicio);
                parametros.Add("@DATAATUAL", dataAtual);

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(A.ID) AS CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            totalAtendEncerr = reader["CO"] != DBNull.Value ? Convert.ToInt32(reader["CO"]) : 0;
                        }
                    }
                }

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT A.DATAHORACRIACAO, A.DATAHORA_INDICADOR, A.DATA_HORA_FECHAMENTO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            DateTime? DataHora_Criacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                            DateTime? DataHora_Indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                            DateTime? DataHora_Fechamento = reader["DATA_HORA_FECHAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_FECHAMENTO"]) : (DateTime?)null;

                            if (DataHora_Indicador.HasValue)
                            {
                                var totais = (DataHora_Indicador - DataHora_Criacao).Value.TotalMinutes;

                                var agora = new Double();

                                if (!DataHora_Fechamento.HasValue)
                                    agora = (DataHora_Indicador - DateTime.Now).Value.TotalMinutes;
                                else
                                    agora = (DataHora_Indicador - DataHora_Fechamento).Value.TotalMinutes;

                                var porcentagem = ((100 - (agora / totais) * 100));
                                var valor_calculado = Math.Round(porcentagem, 2);

                                //dentro do prazo
                                if (valor_calculado >= 100)
                                    totalForaPrazo++;

                            }
                        }
                    }

                    var porcentagemEncerrados = 0.0;

                    if (totalAtendEncerr == totalForaPrazo)
                    {
                        retorno.Objeto = new ResponseInfoSLA
                        {
                            Porcentagem = "100%",
                            QuantidadeEnc = totalAtendEncerr
                        };
                    }
                    else
                    {
                        porcentagemEncerrados = (totalForaPrazo * 100) / totalAtendEncerr;

                        retorno.Objeto = new ResponseInfoSLA
                        {
                            Porcentagem = $"{Math.Round(porcentagemEncerrados,2)}%",
                            QuantidadeEnc = totalForaPrazo
                        };
                    }

                    retorno.Ok = true;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/ObterEncContatoHumano")]
        public async Task<GetApiRetorno<ResponseInfoSLA>> ObterEncContatoHumano()
        {

            GetApiRetorno<ResponseInfoSLA> retorno = new GetApiRetorno<ResponseInfoSLA>();

            using (ISession session = HibernateUtil.GetSession())
            {

                List<FlagsAtendimento> _flags = new List<FlagsAtendimento>();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");

                int totalAtendEncerr = 0;
                int totalContatoHum = 0;

                StringBuilder sqlFrom = new StringBuilder();

                sqlFrom.AppendLine(" FROM TB_ATENDIMENTO A ");
                sqlFrom.AppendLine(" WHERE  ");
                sqlFrom.AppendLine(" A.DATAHORACRIACAO IS NOT NULL ");
                sqlFrom.AppendLine(" AND A.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAATUAL ");
                sqlFrom.AppendLine(" AND A.STATUS_ATENDIMENTO = 4 ");

                parametros.Add("@DATAINICIO", dataInicio);
                parametros.Add("@DATAATUAL", dataAtual);

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT SUM (CASE WHEN A.ID_RESPONSAVEL IS NOT NULL THEN 1 ELSE 0 END) AS QUANTIDADE_HUMANO, ");
                    sql.AppendLine(" COUNT(A.ID) AS QTD_ENCERRADO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            totalAtendEncerr = reader["QTD_ENCERRADO"] != DBNull.Value ? Convert.ToInt32(reader["QTD_ENCERRADO"]) : 0;
                            totalContatoHum = reader["QUANTIDADE_HUMANO"] != DBNull.Value ? Convert.ToInt32(reader["QUANTIDADE_HUMANO"]) : 0;
                        }
                    }

                    var porcentagemEncContatos = 0.0;

                    if (totalAtendEncerr == totalContatoHum)
                    {
                        retorno.Objeto = new ResponseInfoSLA
                        {
                            Porcentagem = "100%",
                            QuantidadeEnc = totalAtendEncerr
                        };
                    }
                    else
                    {
                        porcentagemEncContatos = (totalContatoHum * 100) / totalAtendEncerr;

                        retorno.Objeto = new ResponseInfoSLA
                        {
                            Porcentagem = $"{Math.Round(porcentagemEncContatos, 2)}%",
                            QuantidadeEnc = totalAtendEncerr
                        };
                    }

                    retorno.Ok = true;
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/ObterNivelSolucaoPrimContato")]
        public async Task<GetApiRetorno<ResponseInfoSLA>> ObterNivelSolucaoPrimContato()
        {

            GetApiRetorno<ResponseInfoSLA> retorno = new GetApiRetorno<ResponseInfoSLA>();

            using (ISession session = HibernateUtil.GetSession())
            {

                List<FlagsAtendimento> _flags = new List<FlagsAtendimento>();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");

                int totalAtendEncerr = 0;
                int totalReinc = 0;

                StringBuilder sqlFrom = new StringBuilder();

                sqlFrom.AppendLine(" FROM TB_ATENDIMENTO B ");
                sqlFrom.AppendLine(" WHERE  ");
                sqlFrom.AppendLine(" B.DATAHORACRIACAO IS NOT NULL ");
                sqlFrom.AppendLine(" AND B.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAATUAL ");
                sqlFrom.AppendLine(" AND B.STATUS_ATENDIMENTO = 4 ");

                parametros.Add("@DATAINICIO", dataInicio);
                parametros.Add("@DATAATUAL", dataAtual);

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(B.ID) AS CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            totalAtendEncerr = reader["CO"] != DBNull.Value ? Convert.ToInt32(reader["CO"]) : 0;
                        }
                    }
                }

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT SUM(REPETIDOS.CO) AS TOT_REINC ");
                    sql.AppendLine(" FROM (SELECT B.ID_CLIENTE, B.ID_PLANOPRODUTO, B.CONTRATO, B.ID_TIPO_ATENDIMENTO, COUNT(*) - 1 AS CO ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine(" AND B.ID_PLANOPRODUTO IS NOT NULL ");
                    sql.AppendLine(" AND B.CONTRATO IS NOT NULL ");
                    sql.AppendLine(" AND B.ID_CLIENTE IS NOT NULL ");
                    sql.AppendLine(" AND B.ID_TIPO_ATENDIMENTO IS NOT NULL ");
                    sql.AppendLine(" GROUP BY B.ID_CLIENTE, B.ID_PLANOPRODUTO, B.CONTRATO, B.ID_TIPO_ATENDIMENTO ");
                    sql.AppendLine(" HAVING COUNT(*) > 1) AS REPETIDOS; ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            totalReinc = reader["TOT_REINC"] != DBNull.Value ? Convert.ToInt32(reader["TOT_REINC"]) : 0;
                        }
                    }

                    if (totalAtendEncerr == totalReinc)
                    {
                        retorno.Objeto = new ResponseInfoSLA
                        {
                            Porcentagem = "100%",
                            QuantidadeEnc = totalAtendEncerr
                        };
                    }
                    else
                    {
                        
                        double valorFinalPrimeiroContato = 0.0;

                        valorFinalPrimeiroContato = (((totalAtendEncerr - totalReinc) * 100) / totalAtendEncerr);

                        retorno.Objeto = new ResponseInfoSLA
                        {
                            Porcentagem = $"{Math.Round(valorFinalPrimeiroContato, 2)}%",
                            QuantidadeEnc = totalAtendEncerr - totalReinc
                        };
                    }

                    retorno.Ok = true;
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Atendimentoapi/ObterPesquisaSatisfacao")]
        public async Task<GetApiRetorno<ResponseInfoSLA>> ObterPesquisaSatisfacao()
        {

            GetApiRetorno<ResponseInfoSLA> retorno = new GetApiRetorno<ResponseInfoSLA>();

            using (ISession session = HibernateUtil.GetSession())
            {

                List<FlagsAtendimento> _flags = new List<FlagsAtendimento>();
                

                string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");

                Dictionary<String, Object> parametros = new Dictionary<string, object>()
                {
                    { "@DATAINICIO", dataInicio},
                    { "@DATAATUAL", dataAtual},
                };

                int totalAtendEncerr = 0;
                int totalRespondidos = 0;
     
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(DISTINCT A.ID) AS CO ");
                    sql.AppendLine(" FROM TB_ATENDIMENTO A ");
                    sql.AppendLine(" WHERE  ");
                    sql.AppendLine(" A.DATAHORACRIACAO IS NOT NULL ");
                    sql.AppendLine(" AND A.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAATUAL ");
                    sql.AppendLine(" AND A.STATUS_ATENDIMENTO = 4 ");
                
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            totalAtendEncerr = reader["CO"] != DBNull.Value ? Convert.ToInt32(reader["CO"]) : 0;
                        }
                    }
                }

                StringBuilder sqlFrom = new StringBuilder();
             
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(A.ID) AS CO ");
                    sql.AppendLine(" FROM TB_ATENDIMENTO A ");
                    sql.AppendLine(" INNER JOIN TB_PESQUISA_RESPOSTA B ");
                    sql.AppendLine(" ON (B.ID_ATENDIMENTO = A.ID) ");
                    sql.AppendLine(" INNER JOIN TB_PESQUISA_PERGUNTA_RESPOSTA C ");
                    sql.AppendLine(" ON(C.ID_PESQUISA_RESPOSTA = B.ID) ");
                    sql.AppendLine(" WHERE  ");
                    sql.AppendLine(" A.DATAHORACRIACAO IS NOT NULL ");
                    sql.AppendLine(" AND A.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAATUAL ");
                    sql.AppendLine(" AND A.STATUS_ATENDIMENTO = 4 ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            totalRespondidos = reader["CO"] != DBNull.Value ? Convert.ToInt32(reader["CO"]) : 0;
                        }
                    }
                }

                var porcentagemEncerrados = 0.0;

                if (totalAtendEncerr == totalRespondidos)
                {
                    retorno.Objeto = new ResponseInfoSLA
                    {
                        Porcentagem = "100%",
                        QuantidadeEnc = totalRespondidos
                    };
                }
                else
                {
                    porcentagemEncerrados = (totalRespondidos * 100) / totalAtendEncerr;

                    retorno.Objeto = new ResponseInfoSLA
                    {
                        Porcentagem = $"{Math.Round(porcentagemEncerrados, 2)}%",
                        QuantidadeEnc = totalRespondidos
                    };
                }

                retorno.Ok = true;
            }


            return retorno;
        }


        private async Task<SLA_Count_Horas> GetSLAAtendimento(ISession session, Int64? Id_Atendimento, Int64? Id_Categoria_Cliente, DateTime Data_Criacao)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            Int32? Minutos = null;
            Int32 Count = 0;

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder("SELECT A.MINUTOS ")
                                          .AppendLine("FROM TB_SLA_ATENDIMENTO A ")
                                          .AppendLine("WHERE A.ID_TIPO_ATENDIMENTO = @ID_TIPO_ATENDIMENTO ")
                                          .AppendLine("  AND A.ID_CATEGORIA_CLIENTE = @ID_CATEGORIA_CLIENTE");

                parametros.Add("@ID_TIPO_ATENDIMENTO", Id_Atendimento);
                parametros.Add("@ID_CATEGORIA_CLIENTE", Id_Categoria_Cliente);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        Minutos = Convert.ToInt32(reader["MINUTOS"]);
                    }
                }

            }

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT COUNT(A.ID) CO FROM TB_SLA_ATENDIMENTO A ");
                sql.AppendLine(" WHERE A.ID_TIPO_ATENDIMENTO = @ID_TIPO_ATENDIMENTO ");
                sql.AppendLine(" AND A.ID_CATEGORIA_CLIENTE =  @ID_CATEGORIA_CLIENTE");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        Count = Convert.ToInt32(reader["CO"]);
                    }
                }

            }

            return new SLA_Count_Horas
            {
                Minutos = Minutos ?? 60,
                Count = Count
            };
        }

        private FlagsAtendimento Desc_Cores_SLA(Double Porcentagem)
        {
            if (Porcentagem > 0 && Porcentagem <= 25)
                return new FlagsAtendimento
                {
                    Cor = "#00FF00",
                    Descricao = RsCadastros.DENTRO_PRAZO,


                };
            else if (Porcentagem > 25 && Porcentagem <= 50)
                return new FlagsAtendimento
                {
                    Cor = "#ffd700",
                    Descricao = RsCadastros.ATENCAO,

                };
            else if (Porcentagem > 50 && Porcentagem <= 75)
                return new FlagsAtendimento
                {
                    Cor = "#FFA500",
                    Descricao = RsCadastros.VENCIMENTO_PROXIMO,

                };
            else if (Porcentagem > 75 && Porcentagem < 100)
                return new FlagsAtendimento
                {
                    Cor = "#FF0000",
                    Descricao = RsCadastros.RISCO_PERDA_PRAZO,

                };
            else
                return new FlagsAtendimento
                {
                    Cor = "#000000",
                    Descricao = RsCadastros.PRAZO_VENCIDO,

                };
        }

    }


    public class RequestDashboardUser
    {
        public Boolean FiltrarEmAberto { get; set; } = false;
        public Boolean FiltrarConcluidos { get; set; } = false;
        public Boolean FiltrarAtendimentosFila { get; set; } = false;
        public Boolean FiltrarAtendimentosEmAndamento { get; set; } = false;
        public Boolean FiltrarAtendimentosEncerrados { get; set; } = false;
    }


    public class ResponseInfoSLA
    {
        public string Porcentagem { get; set; }
        public int QuantidadeEnc { get; set; }
    }
}
