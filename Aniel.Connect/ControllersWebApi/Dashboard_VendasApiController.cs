using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.DashBoardVendas;
using Aniel.Connect.Repository.Model;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Dashboard_VendasApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasPendentesMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> VendasPendentesMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                retorno.Objeto = new Dashboard_Vendas_Models();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    //var mes = DateTime.Now.ToString("MMMM").ToUpper();
                    var now = DateTime.Now;
                    var inicio_mes = new DateTime(now.Year, now.Month, 1);
                    var fim_mes = DateTimeUtil.GetLastDayOfMonth(now);

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($" SELECT COUNT(C.ID) CO FROM ASPNETUSERS A");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                    sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                    sql.AppendLine($" LEFT JOIN TB_VENDA C");
                    sql.AppendLine($" ON(C.ID_VENDEDOR = D.ID)");
                    sql.AppendLine($" WHERE A.id = @USUARIO AND ");
                    sql.AppendLine($" C.STATUS IN ('0','5') AND ");
                    sql.AppendLine($" C.DATAHORACRIACAO BETWEEN @DATA_INICIO AND @DATA_FIM");

                    parametros.Add("@DATA_INICIO", inicio_mes);
                    parametros.Add("@DATA_FIM", fim_mes);
                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Vendas_Pendentes_Mes = Convert.ToDouble(reader["CO"]);
                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasPendentesEquipeMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> VendasPendentesEquipeMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                retorno.Objeto = new Dashboard_Vendas_Models();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    //var mes = DateTime.Now.ToString("MMMM").ToUpper();
                    var now = DateTime.Now;
                    var inicio_mes = new DateTime(now.Year, now.Month, 1);
                    var fim_mes = DateTimeUtil.GetLastDayOfMonth(now);

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($" SELECT COUNT(C.ID) CO FROM ASPNETUSERS A ");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D ");
                    sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO) ");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO E ");
                    sql.AppendLine($" ON(D.ID = E.ID_SUPERVISOR) ");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO F ");
                    sql.AppendLine($" ON(F.ID_SUPERVISOR = E.ID) ");
                    sql.AppendLine($" LEFT JOIN TB_VENDA C ");
                    sql.AppendLine($" ON(C.ID_VENDEDOR = F.ID) OR (C.ID_VENDEDOR = E.ID) ");
                    sql.AppendLine($" WHERE A.id = @USUARIO AND ");
                    sql.AppendLine($" C.STATUS IN ('0','5') AND ");
                    sql.AppendLine($" C.DATAHORACRIACAO BETWEEN @DATA_INICIO AND @DATA_FIM");

                    parametros.Add("@DATA_INICIO", inicio_mes);
                    parametros.Add("@DATA_FIM", fim_mes);
                    parametros.Add("@USUARIO", User.Identity.GetUserId());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Vendas_Pendentes_Equipe_Mes = Convert.ToDouble(reader["CO"]);
                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasAprovadasMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> VendasAprovadasMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                retorno.Objeto = new Dashboard_Vendas_Models();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    //var mes = DateTime.Now.ToString("MMMM").ToUpper();
                    var now = DateTime.Now;
                    var inicio_mes = new DateTime(now.Year, now.Month, 1);
                    var fim_mes = DateTimeUtil.GetLastDayOfMonth(now);

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($" SELECT COUNT(C.ID) CO FROM ASPNETUSERS A");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                    sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                    sql.AppendLine($" LEFT JOIN TB_VENDA C");
                    sql.AppendLine($" ON(C.ID_VENDEDOR = D.ID)");
                    sql.AppendLine($" WHERE A.id = @USUARIO AND ");
                    sql.AppendLine($" C.STATUS = '6' AND ");
                    sql.AppendLine($" C.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM");

                    parametros.Add("@DATA_INICIO", inicio_mes);
                    parametros.Add("@DATA_FIM", fim_mes);
                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Vendas_Aprovadas_Mes = Convert.ToDouble(reader["CO"]);
                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasAprovadas")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> VendasAprovadas()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                retorno.Objeto = new Dashboard_Vendas_Models();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    //var mes = DateTime.Now.ToString("MMMM").ToUpper();
                    var now = DateTime.Now;


                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($" SELECT COUNT(C.ID) CO FROM ASPNETUSERS A");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                    sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                    sql.AppendLine($" LEFT JOIN TB_VENDA C");
                    sql.AppendLine($" ON(C.ID_VENDEDOR = D.ID)");
                    sql.AppendLine($" WHERE A.id = @USUARIO AND ");
                    sql.AppendLine($" C.STATUS = '6' AND ");
                    sql.AppendLine($" C.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM ");

                    parametros.Add("@DATA_INICIO", now.Date);
                    parametros.Add("@DATA_FIM", now.Date.AddHours(23).AddMinutes(59).AddSeconds(59));
                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Vendas_Aprovadas = Convert.ToDouble(reader["CO"]);
                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasAprovadasEquipeMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> VendasAprovadasEquipeMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                retorno.Objeto = new Dashboard_Vendas_Models();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    //var mes = DateTime.Now.ToString("MMMM").ToUpper();
                    var now = DateTime.Now;
                    var inicio_mes = new DateTime(now.Year, now.Month, 1);
                    var fim_mes = DateTimeUtil.GetLastDayOfMonth(now);

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($" SELECT COUNT(C.ID) CO FROM ASPNETUSERS A");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                    sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO E");
                    sql.AppendLine($" ON(D.ID = E.ID_SUPERVISOR)");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO F");
                    sql.AppendLine($" ON(F.ID_SUPERVISOR = E.ID)");
                    sql.AppendLine($" LEFT JOIN TB_VENDA C");
                    sql.AppendLine($" ON(C.ID_VENDEDOR = F.ID) OR (C.ID_VENDEDOR = E.ID) ");
                    sql.AppendLine($" WHERE A.id = @USUARIO AND");
                    sql.AppendLine($" C.STATUS = '6' AND ");
                    sql.AppendLine($" C.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM");

                    parametros.Add("@DATA_INICIO", inicio_mes);
                    parametros.Add("@DATA_FIM", fim_mes);
                    parametros.Add("@USUARIO", User.Identity.GetUserId());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Vendas_Aprovadas_Equipe_Mes = Convert.ToDouble(reader["CO"]);
                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasAprovadasEquipe")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> VendasAprovadasEquipe()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                retorno.Objeto = new Dashboard_Vendas_Models();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    //var mes = DateTime.Now.ToString("MMMM").ToUpper();
                    var now = DateTime.Now;
                    var inicio_mes = new DateTime(now.Year, now.Month, 1);
                    var fim_mes = DateTimeUtil.GetLastDayOfMonth(now);

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($" SELECT COUNT(C.ID) CO FROM ASPNETUSERS A");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                    sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO E");
                    sql.AppendLine($" ON(D.ID = E.ID_SUPERVISOR)");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO F");
                    sql.AppendLine($" ON(E.ID_SUPERVISOR = F.ID)");
                    sql.AppendLine($" LEFT JOIN TB_VENDA C");
                    sql.AppendLine($" ON(C.ID_VENDEDOR = F.ID) OR (C.ID_VENDEDOR = E.ID) ");
                    sql.AppendLine($" WHERE A.id = @USUARIO AND");
                    sql.AppendLine($" C.STATUS = '6' AND ");
                    sql.AppendLine($" C.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM ");

                    parametros.Add("@DATA_INICIO", now.Date);
                    parametros.Add("@DATA_FIM", now.Date.AddHours(23).AddMinutes(59).AddSeconds(59));
                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Vendas_Aprovadas_Equipe = Convert.ToDouble(reader["CO"]);
                        }
                    }
                    retorno.Ok = true;

                }
            }

            return retorno;
        }




        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetMinhaMeta")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetMinhaMeta()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {
                var ano = DateTime.Now.Year;
                var diaConfig = new Configuracoes_GeraisApiController().FindById(1).Result.Objeto.Dia_Mes_Finaliza_Medicao_Comissao;

                string dataInicio;
                string dataFim;

                if (diaConfig <= 30)
                    dataInicio = (Convert.ToInt32(ano) - 1).ToString() + "-12-" + (Convert.ToInt32(diaConfig) + 1).ToString();
                else
                    dataInicio = ano + "-01-" + "01";

                dataFim = ano + "-12-" + diaConfig;


                DateTime dateTimeInicio = Convert.ToDateTime(dataInicio);
                DateTime dateTimeFim = Convert.ToDateTime(dataFim);
                Int64 Id_Funcionario = 0;
                Double vendas;
                Double meta = 0;

                retorno.Objeto = new Dashboard_Vendas_Models();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    var mes = DateTime.Now.ToString("MMMM").ToUpper();

                    if (mes == "MARÇO")
                    {
                        string mesTratado = mes.Replace(mes, "MARCO");
                        mes = mesTratado;
                    }

                    StringBuilder sql = new StringBuilder(@$" SELECT C.ID AS FUNCIONARIO_ID, COALESCE(B.{mes}, F.{mes}) AS META FROM ASPNETUSERS A
                                                                LEFT JOIN TB_FUNCIONARIO C
                                                                  ON (C.ID = A.ID_FUNCIONARIO)                    
                                                                LEFT JOIN TB_FUNCIONARIO E
                                                                  ON (C.ID_SUPERVISOR = E.ID)
                                                                LEFT JOIN TB_META B
                                                                  ON (C.id_meta = B.ID)
                                                                LEFT JOIN TB_META F
                                                                  ON (E.ID_META = F.ID)
                                                                WHERE A.id = @USUARIO AND COALESCE(B.ANO,F.ANO) = '{DateTime.Now.Year}'
                                                                GROUP BY B.{mes} ,F.{mes} , FUNCIONARIO_ID
                                                                ORDER BY B.{mes}, F.{mes}");



                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            meta = reader["META"] != DBNull.Value ? Convert.ToDouble(reader["META"]) : 0;
                            Id_Funcionario = Convert.ToInt64(reader["FUNCIONARIO_ID"]);
                        }
                    }
                    retorno.Ok = true;

                }

                using (var com2 = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros2 = new Dictionary<string, object>();

                    StringBuilder sql2 = new StringBuilder();

                    sql2.AppendLine(" SELECT SUM(D.QUANTIDADE) AS PONTOS ");

                    sql2.AppendLine(" FROM ASPNETUSERS A ");

                    sql2.AppendLine(" LEFT JOIN TB_VENDA B ");
                    sql2.AppendLine(" ON(B.ID_VENDEDOR = A.ID_FUNCIONARIO) ");

                    sql2.AppendLine(" INNER JOIN TB_CLIENTE_PLANO_PRODUTO C ");
                    sql2.AppendLine(" ON(C.ID_VENDA = B.ID) ");

                    sql2.AppendLine(" INNER JOIN TB_COMISSAO_PLANO_PRODUTO D ");
                    sql2.AppendLine(" ON(D.ID_COMISSIONAMENTO = B.ID_COMISSIONAMENTO AND(D.ID_PLANO = C.ID_PLANO OR D.ID_PRODUTO = C.ID_PRODUTO)) ");

                    sql2.AppendLine(" WHERE A.ID = @ID_USUARIO AND B.DATA_VENDA BETWEEN @DATA_INI AND @DATA_FIM");


                    var now = DateTime.Now;
                    var data_ini = new DateTime(now.Year, now.Month, 1);
                    var data_fim = DateTimeUtil.GetLastDayOfMonth(now);


                    parametros2.Add("@ID_USUARIO", User.Identity.GetUserId());
                    parametros2.Add("@DATA_INI", data_ini);
                    parametros2.Add("@DATA_FIM", data_fim);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com2, sql2, parametros2, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            vendas = reader["PONTOS"] != DBNull.Value ? Convert.ToDouble(reader["PONTOS"]) : 0;
                            retorno.Objeto.Minhas_Vendas = meta > 0 ? Math.Round(((vendas / meta) * 100), 2) : 0;
                            retorno.Objeto.Minhas_Vendas = retorno.Objeto.Minhas_Vendas > 100 ? 100 : retorno.Objeto.Minhas_Vendas;
                        }
                    }
                    retorno.Ok = true;

                }

            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasPerdidas")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetVendasPerdidas()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($" SELECT COUNT(ID) CO FROM TB_VENDA ");

                    sql.AppendLine(" WHERE USUARIOCRIACAO = @USUARIO ");
                    sql.AppendLine($" AND STATUS IN (0, 5) ");



                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Vendas_Models
                            {
                                Vendas_Perdidas = Convert.ToString(reader["CO"])
                            };

                        }
                    }

                    retorno.Ok = true;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasPerdidasMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetVendasPerdidasMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                DateTime data_atual = DateTime.Now;
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($" SELECT COUNT(ID) CO FROM TB_VENDA ");

                    sql.AppendLine(" WHERE USUARIOCRIACAO = @USUARIO ");
                    sql.AppendLine(" AND DATA_VENDA BETWEEN @DATA_INICIO_MES AND @DATA_ATUAL ");
                    sql.AppendLine($" AND STATUS IN (3, 4) ");



                    parametros.Add("@USUARIO", User.Identity.GetUserId());
                    parametros.Add("@DATA_INICIO_MES", data_inicio_mes);
                    parametros.Add("@DATA_ATUAL", data_atual);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Vendas_Models
                            {
                                Vendas_Perdidas_Mes = Convert.ToString(reader["CO"])
                            };

                        }
                    }

                    retorno.Ok = true;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasReprovadasMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetVendasReprovadasMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                DateTime data_atual = DateTime.Now;
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($" SELECT COUNT(ID) CO FROM TB_VENDA ");

                    sql.AppendLine(" WHERE USUARIOCRIACAO = @USUARIO ");
                    sql.AppendLine(" AND DATA_VENDA BETWEEN @DATA_INICIO_MES AND @DATA_ATUAL ");
                    sql.AppendLine($" AND STATUS = '3' ");



                    parametros.Add("@USUARIO", User.Identity.GetUserId());
                    parametros.Add("@DATA_INICIO_MES", data_inicio_mes);
                    parametros.Add("@DATA_ATUAL", data_atual);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Vendas_Models
                            {
                                Vendas_Reprovadas_Mes = Convert.ToString(reader["CO"])
                            };

                        }
                    }

                    retorno.Ok = true;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasCanceladasMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetVendasCanceladasMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                DateTime data_atual = DateTime.Now;
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($"  SELECT COUNT(B.ID) CO FROM ASPNETUSERS A ");
                    sql.AppendLine($"  INNER JOIN TB_VENDA B ");
                    sql.AppendLine($"  ON B.ID_VENDEDOR =A.ID_FUNCIONARIO ");

                    sql.AppendLine(" WHERE A.ID = @USUARIO ");
                    sql.AppendLine(" AND B.DATA_VENDA BETWEEN @DATA_INICIO_MES AND @DATA_ATUAL ");
                    sql.AppendLine($" AND B.STATUS = '4' ");



                    parametros.Add("@USUARIO", User.Identity.GetUserId());
                    parametros.Add("@DATA_INICIO_MES", data_inicio_mes);
                    parametros.Add("@DATA_ATUAL", data_atual);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Vendas_Models
                            {
                                Vendas_Canceladas_Mes = Convert.ToString(reader["CO"])
                            };

                        }
                    }

                    retorno.Ok = true;
                }
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetMetaEquipe")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetMetaEquipe()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();
            retorno.Objeto = new Dashboard_Vendas_Models();

            using (ISession session = HibernateUtil.GetSession())
            {
                var ano = DateTime.Now.Year;
                var diaConfig = new Configuracoes_GeraisApiController().FindById(1).Result.Objeto.Dia_Mes_Finaliza_Medicao_Comissao;

                string dataInicio;
                string dataFim;

                if (diaConfig <= 30)
                    dataInicio = (Convert.ToInt32(ano) - 1).ToString() + "-12-" + (Convert.ToInt32(diaConfig) + 1).ToString();
                else
                    dataInicio = ano + "-01-" + "01";

                dataFim = ano + "-12-" + diaConfig;


                DateTime dateTimeInicio = Convert.ToDateTime(dataInicio);
                DateTime dateTimeFim = Convert.ToDateTime(dataFim);

                List<Int64> lista_func = new List<long>();
                Double vendas = 0;
                Double meta = 0;



                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var mes = DateTime.Now.ToString("MMMM").ToUpper();

                if (mes == "MARÇO")
                {
                    string mesTratado = mes.Replace(mes, "MARCO");
                    mes = mesTratado;
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($"SELECT B.{mes} AS META, E.NOME , F.ID AS ID_FUNCIONARIO FROM ASPNETUSERS A");
                    sql.AppendLine($"LEFT JOIN TB_FUNCIONARIO D");
                    sql.AppendLine($"ON(D.ID = A.ID_FUNCIONARIO)");
                    sql.AppendLine($"LEFT JOIN TB_FUNCIONARIO E");
                    sql.AppendLine($"ON(E.ID = D.ID_SUPERVISOR)");

                    sql.AppendLine($"LEFT JOIN TB_FUNCIONARIO F");
                    sql.AppendLine($"ON(F.ID_SUPERVISOR = E.ID)");
                    sql.AppendLine($"LEFT JOIN TB_META B");
                    sql.AppendLine($"ON(E.ID_META = B.ID)");

                    sql.AppendLine($"WHERE A.ID = @USUARIO AND D.ID_SUPERVISOR = E.ID ");
                    sql.AppendLine($"AND B.ANO = '{DateTime.Now.Year}'");
                    sql.AppendLine($"GROUP BY B.{mes} ,E.NOME,ID_FUNCIONARIO");
                    sql.AppendLine($"ORDER BY B.{mes} , UPPER(E.NOME)");

                    parametros.Add("@USUARIO", User.Identity.GetUserId());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            meta = reader["META"] != DBNull.Value ? Convert.ToDouble(reader["META"]) : 0;
                            lista_func.Add(Convert.ToInt64(reader["ID_FUNCIONARIO"]));
                        }
                    }

                    retorno.Ok = true;

                }

                foreach (var Id_Funcionario in lista_func)
                {

                    using (var com2 = session.Connection.CreateCommand())
                    {

                        Dictionary<String, Object> parametros2 = new Dictionary<string, object>();

                        StringBuilder sql2 = new StringBuilder();

                        //sql2.AppendLine("SELECT SUM(PONTOS) PONTOS FROM sp_comissionamento_vendedor(@ID_VENDEDOR,@INICIO,@ANO_INI,@FIM,@ANO_FIM)");

                        sql2.AppendLine(" SELECT SUM(F.QUANTIDADE) AS PONTOS ");

                        sql2.AppendLine(" FROM ASPNETUSERS A ");

                        sql2.AppendLine(" LEFT JOIN TB_FUNCIONARIO B "); // VENDEDOR DO USUÁRIO LOGADO
                        sql2.AppendLine(" ON(B.ID = A.ID_FUNCIONARIO) ");

                        sql2.AppendLine(" LEFT JOIN TB_FUNCIONARIO C  "); //SUPERVISOR
                        sql2.AppendLine(" ON(C.ID = B.ID_SUPERVISOR) ");

                        sql2.AppendLine(" LEFT JOIN TB_VENDA D ");
                        sql2.AppendLine(" ON(D.ID_SUPERVISOR = C.ID) ");

                        sql2.AppendLine(" INNER JOIN TB_CLIENTE_PLANO_PRODUTO E ");
                        sql2.AppendLine(" ON(E.ID_VENDA = D.ID) ");

                        sql2.AppendLine(" INNER JOIN TB_COMISSAO_PLANO_PRODUTO F ");
                        sql2.AppendLine(" ON(F.ID_COMISSIONAMENTO = D.ID_COMISSIONAMENTO AND(F.ID_PLANO = E.ID_PLANO OR F.ID_PRODUTO = E.ID_PRODUTO)) ");

                        sql2.AppendLine(" WHERE A.ID = @ID_USUARIO AND D.DATA_VENDA BETWEEN @DATA_INI AND @DATA_FIM");


                        var now = DateTime.Now;
                        var data_ini = new DateTime(now.Year, now.Month, 1);
                        var data_fim = DateTimeUtil.GetLastDayOfMonth(now);


                        parametros2.Add("@ID_USUARIO", User.Identity.GetUserId());
                        parametros2.Add("@DATA_INI", data_ini);
                        parametros2.Add("@DATA_FIM", data_fim);

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com2, sql2, parametros2, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                vendas += reader["PONTOS"] != DBNull.Value ? Convert.ToDouble(reader["PONTOS"]) : 0;
                            }
                        }

                        retorno.Objeto.Vendas_Equipe = meta > 0 ? Math.Round(((vendas / meta) * 100), 2) : 0;
                        retorno.Objeto.Vendas_Equipe = retorno.Objeto.Vendas_Equipe > 100 ? 100 : retorno.Objeto.Vendas_Equipe;
                        retorno.Ok = true;

                    }
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasPerdidasEquipe")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetVendasPerdidasEquipe()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT COUNT(C.ID) CO");
                    sql.AppendLine("FROM ASPNETUSERS A");
                    sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B");
                    sql.AppendLine("ON(B.ID_SUPERVISOR = A.ID_FUNCIONARIO)");
                    sql.AppendLine("LEFT JOIN TB_VENDA C");
                    sql.AppendLine("ON(C.ID_VENDEDOR = B.ID)");
                    sql.AppendLine("WHERE A.ID_FUNCIONARIO = B.ID_SUPERVISOR AND A.ID = @USUARIO AND C.STATUS IN(3,4)");
                    parametros.Add("@USUARIO", User.Identity.GetUserId());



                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Vendas_Models
                            {
                                Vendas_Perdidas_Equipe = Convert.ToString(reader["CO"])
                            };

                        }
                    }

                    retorno.Ok = true;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasPerdidasEquipeMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetVendasPerdidasEquipeMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                DateTime data_atual = DateTime.Now;
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT COUNT(C.ID) CO");
                    sql.AppendLine("FROM ASPNETUSERS A");
                    sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B");
                    sql.AppendLine("ON(B.ID_SUPERVISOR = A.ID_FUNCIONARIO)");
                    sql.AppendLine("LEFT JOIN TB_VENDA C");
                    sql.AppendLine("ON(C.ID_VENDEDOR = B.ID)");
                    sql.AppendLine("WHERE A.ID_FUNCIONARIO = B.ID_SUPERVISOR AND A.ID = @USUARIO AND C.STATUS IN(0,5)");
                    sql.AppendLine(" AND C.DATA_VENDA BETWEEN @DATA_INICIO_MES AND @DATA_ATUAL ");

                    parametros.Add("@USUARIO", User.Identity.GetUserId());
                    parametros.Add("@DATA_INICIO_MES", data_inicio_mes);
                    parametros.Add("@DATA_ATUAL", data_atual);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Vendas_Models
                            {
                                Vendas_Perdidas_Equipe_Mes = Convert.ToString(reader["CO"])
                            };

                        }
                    }

                    retorno.Ok = true;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasReprovadasEquipeMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetVendasReprovadasEquipeMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                DateTime data_atual = DateTime.Now;
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT COUNT(C.ID) CO");
                    sql.AppendLine("FROM ASPNETUSERS A");
                    sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B");
                    sql.AppendLine("ON(B.ID_SUPERVISOR = A.ID_FUNCIONARIO)");
                    sql.AppendLine("LEFT JOIN TB_VENDA C");
                    sql.AppendLine("ON(C.ID_VENDEDOR = B.ID)");
                    sql.AppendLine("WHERE A.ID_FUNCIONARIO = B.ID_SUPERVISOR AND A.ID = @USUARIO AND C.STATUS = '3'");
                    sql.AppendLine("AND C.DATA_VENDA BETWEEN @DATA_INICIO_MES AND @DATA_ATUAL ");

                    parametros.Add("@USUARIO", User.Identity.GetUserId());
                    parametros.Add("@DATA_INICIO_MES", data_inicio_mes);
                    parametros.Add("@DATA_ATUAL", data_atual);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Vendas_Models
                            {
                                Vendas_Reprovadas_Equipe_Mes = Convert.ToString(reader["CO"])
                            };

                        }
                    }

                    retorno.Ok = true;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasCanceladasEquipeMes")]
        public async Task<GetApiRetorno<Dashboard_Vendas_Models>> GetVendasCanceladasEquipeMes()
        {

            GetApiRetorno<Dashboard_Vendas_Models> retorno = new GetApiRetorno<Dashboard_Vendas_Models>();

            using (ISession session = HibernateUtil.GetSession())
            {

                DateTime data_atual = DateTime.Now;
                DateTime _data = data_atual;

                for (int i = data_atual.Day; i > 1; i--)
                {
                    _data = _data.AddDays(-1);
                }

                DateTime data_inicio_mes = new DateTime(_data.Year, _data.Month, _data.Day, 0, 0, 0);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT COUNT(C.ID) CO");
                    sql.AppendLine("FROM ASPNETUSERS A");
                    sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B");
                    sql.AppendLine("ON(B.ID = A.ID_FUNCIONARIO)");
                    sql.AppendLine("LEFT JOIN TB_FUNCIONARIO D");
                    sql.AppendLine("ON(D.ID = B.ID_SUPERVISOR)");
                    sql.AppendLine("INNER JOIN TB_FUNCIONARIO E");
                    sql.AppendLine("ON(E.id_supervisor = D.ID)");
                    sql.AppendLine("LEFT JOIN TB_VENDA C");
                    sql.AppendLine("ON(C.ID_VENDEDOR = E.ID) OR (C.ID_VENDEDOR = D.ID)");
                    sql.AppendLine("WHERE E.ID_SUPERVISOR = D.ID AND A.ID = @USUARIO AND C.STATUS = '4'");
                    sql.AppendLine("AND C.DATA_VENDA BETWEEN @DATA_INICIO_MES AND @DATA_ATUAL");

                    parametros.Add("@USUARIO", User.Identity.GetUserId());
                    parametros.Add("@DATA_INICIO_MES", data_inicio_mes);
                    parametros.Add("@DATA_ATUAL", data_atual);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Objeto = new Dashboard_Vendas_Models
                            {
                                Vendas_Canceladas_Equipe_Mes = Convert.ToString(reader["CO"])
                            };

                        }
                    }

                    retorno.Ok = true;
                }
            }

            return retorno;
        }
        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetMetaUser")]
        public async Task<PostApiRetorno<DashBoardMetaResults>> GetMetaUser([FromBody] DashBoardVendasFiltro request)
        {
            var retorno = new PostApiRetorno<DashBoardMetaResults>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    var diaConfig = new Configuracoes_GeraisApiController().FindById(1).Result.Objeto.Dia_Mes_Finaliza_Medicao_Comissao;
                    IDashBoardVendasRepository repo = new DashBoardVendasRepository(session, sessionT5);

                    var business = new DashboardVendasBusiness(repo);
                    retorno = (await business.GetMeta(new DashBoardVendasPeriodoFiltro
                    {
                        Usuario = User.Identity.GetUserId(),
                        Filtro = request.DataFiltrada,
                        FormatoVisualizacao = request.FormatoVisualizacao
                    }, diaConfig));
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = null;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetFormatoVisualizacao")]
        public async Task<PostApiRetorno<long>> GetFormatoVisualizacao()
        {
            var retorno = new PostApiRetorno<long>();
            try 
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var comm = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder("SELECT A.VISUALIZACAO_DASHBOARD FROM TB_PERFIL_DASHBOARD_CONNECT A ")
                    .AppendLine("LEFT JOIN ASPNETUSERS B ")
                    .AppendLine("ON B.ID_PERFIL_DASH_CONNECT = A.ID ")
                    .AppendLine($"WHERE B.ID = '{User.Identity.GetUserId()}'");

                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno.IdObjeto = Convert.ToInt64(reader["VISUALIZACAO_DASHBOARD"]);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            
            return retorno;
        }
        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasAprovadasUser")]
        public async Task<PostApiRetorno<long>> GetVendasAprovadasUser([FromBody] DashBoardVendasFiltro request)
        {
            var retorno = new PostApiRetorno<long>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDashBoardVendasRepository repo = new DashBoardVendasRepository(session, sessionT5);
                    var business = new DashboardVendasBusiness(repo);
                    retorno = (await business.GetVendasAprovadas(new DashBoardVendasPeriodoFiltro
                    {
                        Usuario = User.Identity.GetUserId(),
                        Filtro = request.DataFiltrada,
                        StatusFiltrados = new List<Status_Venda> { Status_Venda.Venda_Aprovada },
                        FormatoVisualizacao = request.FormatoVisualizacao
                    }));
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasPendentesUser")]
        public async Task<PostApiRetorno<long>> GetVendasPendentesUser([FromBody] DashBoardVendasFiltro request)
        {
            var retorno = new PostApiRetorno<long>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDashBoardVendasRepository repo = new DashBoardVendasRepository(session, sessionT5);
                    var business = new DashboardVendasBusiness(repo);
                    retorno = (await business.GetVendasPendentes(new DashBoardVendasPeriodoFiltro
                    {
                        Usuario = User.Identity.GetUserId(),
                        Filtro = request.DataFiltrada,
                        FormatoVisualizacao = request.FormatoVisualizacao,
                        StatusFiltrados = new List<Status_Venda> { Status_Venda.Analise_Cliente,
                                                                   Status_Venda.Aguardando_Avaliacao_Comercial,
                                                                   Status_Venda.aguardando_Pagamento}
                    }));
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetDesistenciasUser")]
        public async Task<PostApiRetorno<long>> GetDesistenciasUser([FromBody] DashBoardVendasFiltro request)
        {
            var retorno = new PostApiRetorno<long>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDashBoardVendasRepository repo = new DashBoardVendasRepository(session, sessionT5);
                    var business = new DashboardVendasBusiness(repo);
                    retorno = (await business.GetDesistencias(new DashBoardVendasPeriodoFiltro
                    {
                        Usuario = User.Identity.GetUserId(),
                        Filtro = request.DataFiltrada,
                        StatusFiltrados = new List<Status_Venda> { Status_Venda.Venda_Cancelada },
                        FormatoVisualizacao = request.FormatoVisualizacao
                    }));
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasInstaladasUser")]
        public async Task<PostApiRetorno<DashboardConversaoVendasInstaladas>> GetVendasInstaladasUser([FromBody] DashBoardVendasFiltro request)
        {
            var retorno = new PostApiRetorno<DashboardConversaoVendasInstaladas>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDashBoardVendasRepository repo = new DashBoardVendasRepository(session, sessionT5);
                    var business = new DashboardVendasBusiness(repo);
                    retorno = (await business.GetVendasInstaladas(new DashBoardVendasPeriodoFiltro
                    {
                        Usuario = User.Identity.GetUserId(),
                        StatusFiltrados = new List<Status_Venda> { Status_Venda.Venda_Aprovada },
                        Filtro = request.DataFiltrada,
                        FormatoVisualizacao = request.FormatoVisualizacao
                    }));
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = null;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;

        }
        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetVendasByStatusUser")]
        public async Task<PostApiRetorno<DashBoardVendasByStatus>> GetVendasByStatusUser([FromBody] DashBoardVendasFiltro request)
        {
            var retorno = new PostApiRetorno<DashBoardVendasByStatus>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDashBoardVendasRepository repo = new DashBoardVendasRepository(session, sessionT5);
                    var business = new DashboardVendasBusiness(repo);
                    retorno = (await business.GetVendasByStatus(new DashBoardVendasPeriodoFiltro
                    {
                        Usuario = User.Identity.GetUserId(),
                        Filtro = request.DataFiltrada,
                        FormatoVisualizacao = request.FormatoVisualizacao
                    }));
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = null;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/Dashboard_Vendasapi/GetFunilVendasUser")]
        public async Task<PostApiRetorno<DashBoardFunilVendas>> GetFunilVendas([FromBody] DashBoardVendasFiltro request)
        {
            var retorno = new PostApiRetorno<DashBoardFunilVendas>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDashBoardVendasRepository repo = new DashBoardVendasRepository(session, sessionT5);
                    var business = new DashboardVendasBusiness(repo);
                    retorno = (await business.GetFunilVendas(new DashBoardVendasPeriodoFiltro
                    {
                        Usuario = User.Identity.GetUserId(),
                        Filtro = request.DataFiltrada,
                        FormatoVisualizacao = request.FormatoVisualizacao
                    }));
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = null;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
    }
}
