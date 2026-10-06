using Aniel.Connect.ControllersWebApi.Ativacao.Repositorio;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.mongo;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;

//using Aniel.Connect.Models;
//using Aniel.Connect.Models.Ativacao;
using NHibernate;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.Ativacao.Interfaces;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Integracao.Ativacao.Radius;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Relatorio_ConexaoApiController : BaseApiController
    {        
        [HttpPost()]
        [Route("api/Relatorio_Conexaoapi/GetAll")]
        public async Task<DataTableResponse<RelatorioConexaoModel>> GetAll(RelatorioConexao_DataTableRequest request)
        {
            List<RelatorioConexaoModel> retorno = new List<RelatorioConexaoModel>();
            IOrderedEnumerable<RelatorioConexaoModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();

                sqlFrom.AppendLine(" FROM TB_LOGIN_RADIUS A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE B ON (A.ID_CLIENTE = B.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_POOL_IP_ENDERECO C ON (A.ID_POOL_IP_ENDERECO = C.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_ONU D ON (D.ID_LOGIN_RADIUS = A.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_OLT E ON (E.ID = D.ID_OLT) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO F ON (F.ID = A.ID_PLANO_PRODUTO) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_PLANO G ON (G.ID = F.ID_PLANO) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_POOL_IP H ON (H.ID = F.ID_POOL_IP) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_POP I ON (I.ID = A.ID_POP OR I.ID = E.ID_POP) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_SLOT J ON (J.ID = D.ID_SLOT) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_PON K ON (K.ID = D.ID_PON) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_ENDERECO_CLIENTE L ON (L.ID = B.ID_ENDERECO_PRINCIPAL) ");

                if (request.Status.HasValue)
                    sqlFrom.AppendLine($" WHERE A.STATUS = {request.Status} ");
                else
                    sqlFrom.AppendLine($" WHERE A.STATUS IN (0,1) ");

                #region Filtros                
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "ip")
                    {
                        sqlFrom.AppendLine(" AND (C.IP LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "login")
                    {
                        sqlFrom.AppendLine(" AND (UPPER(A.LOGIN) LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "cliente")
                    {
                        sqlFrom.AppendLine(" AND (UPPER(B.NOMEFANTASIA) LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "mac")
                    {
                        sqlFrom.AppendLine(" AND (UPPER(A.MAC) LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                }

                if(request.IdCliente.HasValue)
                {
                    sqlFrom.AppendLine(" AND B.ID = @IDCLIENTE ");
                    parametros.Add("@IDCLIENTE", request.IdCliente.Value);
                }

                if (request.IdPlano.HasValue)
                {
                    sqlFrom.AppendLine(" AND G.ID = @IDPLANO ");
                    parametros.Add("@IDPLANO", request.IdPlano.Value);
                }

                if(request.StatusOnu.HasValue)
                {
                    sqlFrom.AppendLine(" AND D.STATUS_ONU = @STATUS_ONU ");
                    parametros.Add("@STATUS_ONU", request.StatusOnu.Value);
                }

                if (request.IdOLT.HasValue)
                {
                    sqlFrom.AppendLine(" AND E.ID = @IDOLT ");
                    parametros.Add("@IDOLT", request.IdOLT.Value);
                }

                if (request.IdSlot.HasValue)
                {
                    sqlFrom.AppendLine(" AND J.ID = @IDSLOT ");
                    parametros.Add("@IDSLOT", request.IdSlot.Value);
                }

                if (request.IdPon.HasValue)
                {
                    sqlFrom.AppendLine(" AND K.ID = @IDPON ");
                    parametros.Add("@IDPON", request.IdPon.Value);
                }

                if (!String.IsNullOrEmpty(request.Estado) && request.Estado != RsBase.TODOS)
                {
                    sqlFrom.AppendLine(" AND UPPER(L.ENDERECO_UF) = UPPER(@ESTADO) ");
                    parametros.Add("@ESTADO", request.Estado);
                }

                if (!String.IsNullOrEmpty(request.Cidade) && request.Cidade != RsBase.TODOS)
                {
                    sqlFrom.AppendLine(" AND UPPER(L.ENDERECO_CIDADE) = UPPER(@CIDADE) ");
                    parametros.Add("@CIDADE", request.Cidade);
                }

                if (!String.IsNullOrEmpty(request.Bairro) && request.Bairro != RsBase.TODOS)
                {
                    sqlFrom.AppendLine(" AND UPPER(L.ENDERECO_BAIRRO) = UPPER(@BAIRRO) ");
                    parametros.Add("@BAIRRO", request.Bairro);
                }

                if (request.IdConcentradora.HasValue)
                {
                    sqlFrom.AppendLine(" AND A.ID_CONCENTRADORA = @IDCONCENTRADORA ");
                    parametros.Add("@IDCONCENTRADORA", request.IdConcentradora.Value);
                }

                if (request.IdPoolIp.HasValue)
                {
                    sqlFrom.AppendLine(" AND H.ID = @IDPOOL_IP ");
                    parametros.Add("@IDPOOL_IP", request.IdPoolIp.Value);
                }

                if(request.IdCaixaFtth.HasValue)
                {
                    sqlFrom.AppendLine(" AND A.ID_CAIXA_FTTH = @IDCAIXA_FTTH ");
                    parametros.Add("@IDCAIXA_FTTH", request.IdCaixaFtth.Value);
                }
                
                if(request.StatusAutenticacaoRadius.HasValue)
                {
                    sqlFrom.AppendLine(" AND A.STATUS_ULTIMA_AUTH_RADIUS = @STATUS_ULTIMA_AUTH_RADIUS ");
                    parametros.Add("@STATUS_ULTIMA_AUTH_RADIUS", request.StatusAutenticacaoRadius.Value);
                }

                #endregion

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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

                    sql.AppendLine(" SELECT A.ID, A.ID_PLANO_PRODUTO, A.LOGIN, A.ID AS ID_LOGIN_RADIUS, COALESCE(A.MAC, '00:00:00:00:00:00') AS MAC, A.STATUS AS BLOQUEADO, B.NOMEFANTASIA AS DESC_CLIENTE, COALESCE(C.IP, COALESCE(A.ENDERECO_IPV4, A.ENDERECO_IPV6)) AS IP, ");
                    sql.AppendLine(" D.STATUS_ONU, I.DESCRICAO AS DESC_POP, H.DESCRICAO AS DESC_POOL_IP, E.DESCRICAO  AS DESC_OLT, G.DESCRICAO AS DESC_PLANO, A.ID_PORTA_FTTH, ");
                    sql.AppendLine(" A.STATUS_ULTIMA_AUTH_RADIUS, A.DATAHORA_ULTIMA_AUTH_RADIUS ");
                    sql.AppendLine(sqlFrom.ToString());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var ultimo_status_autenticacao = reader["STATUS_ULTIMA_AUTH_RADIUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Sinapse.Model.enumerados.Status_Autenticacao_Radius>(Convert.ToString(reader["STATUS_ULTIMA_AUTH_RADIUS"])) : null;
                            var datahora_ultima_autenticacao = reader["DATAHORA_ULTIMA_AUTH_RADIUS"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_ULTIMA_AUTH_RADIUS"]).ToString("dd/MM/yyyy HH:mm:ss") : null;
                            var ultima_autenticacao = $"{ultimo_status_autenticacao} - {datahora_ultima_autenticacao}";


                            retorno.Add(new RelatorioConexaoModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Endereco_Ip = Convert.ToString(reader["IP"]),
                                Login_Radius = Convert.ToString(reader["LOGIN"]),
                                Id_Login_Radius = Convert.ToInt64(reader["ID_LOGIN_RADIUS"]),
                                Bloqueado = Convert.ToBoolean(reader["BLOQUEADO"]) == true ? Convert.ToString(RsCadastros.NAO) : Convert.ToString(RsCadastros.SIM),
                                //Ponto_Acesso = Convert.ToString(reader["PONTO_ACESSO"]),
                                IdClientePlanoProduto = reader["ID_PLANO_PRODUTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PLANO_PRODUTO"]) : (long?)null,
                                Desc_Cliente = Convert.ToString(reader["DESC_CLIENTE"]),
                                Mac = Convert.ToString(reader["MAC"]),
                                StatusOnu = reader["STATUS_ONU"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Sinapse.Model.enumerados.StatusONU>(Convert.ToString(reader["STATUS_ONU"])) : HtmlEnumExtensions.GetEnumDescription(Sinapse.Model.enumerados.StatusONU.Indisponivel),
                                Desc_POP = Convert.ToString(reader["DESC_POP"]),
                                Desc_Pool_Ip = Convert.ToString(reader["DESC_POOL_IP"]),
                                Desc_OLT = Convert.ToString(reader["DESC_OLT"]),
                                Desc_Plano = Convert.ToString(reader["DESC_PLANO"]),
                                Id_Porta_FTTH = reader["ID_PORTA_FTTH"] != DBNull.Value ? Convert.ToInt64(reader["ID_PORTA_FTTH"]) : (long?)null,
                                Ultima_Autenticacao = ultima_autenticacao
                            });
                        }
                    }                   
                }
            }

            #region Consulta Caixa/Porta FTTH
            List<long> listaIdPortas = retorno.Where(w => w.Id_Porta_FTTH != null).Select(p => p.Id_Porta_FTTH.Value).ToList();
            if(listaIdPortas.Count > 0)
            {
                List<Models.T5.Elemento_Rede_PortaGetListModel> retornoFtth = new List<Models.T5.Elemento_Rede_PortaGetListModel>();

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID AS ID_PORTA, A.PORTA AS NUM_PORTA, B.DESCRICAO AS DESC_CAIXA ");
                        sql.AppendLine("FROM TB_ELEMENTO_REDE_PORTA A ");
                        sql.AppendLine("INNER JOIN TB_ELEMENTO_REDE B ON (A.ID_CAIXA = B.ID)  ");
                        sql.AppendLine($"WHERE A.ID IN ({string.Join(",", listaIdPortas)}) ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while(await reader.ReadAsync())
                            {
                                retornoFtth.Add(new Models.T5.Elemento_Rede_PortaGetListModel {
                                    Id = reader["ID_PORTA"] != DBNull.Value ? Convert.ToInt64(reader["ID_PORTA"]) : (long?)null,     
                                    Porta = reader["NUM_PORTA"] != DBNull.Value ? Convert.ToInt32(reader["NUM_PORTA"]) : (int?)null,
                                    Caixa = Convert.ToString(reader["DESC_CAIXA"])
                                });
                            }
                        }
                    }
                }

                if(retornoFtth.Count > 0)
                {
                    foreach (var item in retorno)
                    {
                        if(item.Id_Porta_FTTH.HasValue && retornoFtth.Any(p => p.Id == item.Id_Porta_FTTH))
                        {
                            var modelFtth = retornoFtth.FirstOrDefault(p => p.Id == item.Id_Porta_FTTH);
                            item.Num_Porta_FTHH = Convert.ToString(modelFtth.Porta) ?? string.Empty;
                            item.Desc_Caixa_FTTH = modelFtth.Caixa ?? string.Empty;
                        }
                    }
                }                
            }
            #endregion

            #region Consulta dados Radius
            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoRadius = new Conexao<Sinapse.Model.Radius>(session))
                {
                    var radius = await repoRadius.selectFirst();

                    foreach (var item in retorno)
                    {
                        var retornoRadius = await new AnielRadius(radius).GetDadosLoginPainelDiagnostico(item.Login_Radius);
                        //item.Ultima_Autenticacao = $"{retornoRadius.replyAuth ?? string.Empty} - {retornoRadius.dataUltimoAuth ?? string.Empty}";
                        item.Data_Ini_Ultimo_Consumo = !String.IsNullOrEmpty(retornoRadius.dataInicioUltimoAcct) ? retornoRadius.dataInicioUltimoAcct : "-";
                        item.Data_Fim_Ultimo_Consumo = !String.IsNullOrEmpty(retornoRadius.dataFimUltimoAcct) ? retornoRadius.dataFimUltimoAcct : "-";
                    }
                }
            }
            catch(Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
                            
            #endregion

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Descrição Cliente
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Desc_Cliente);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Desc_Cliente);
                            break;
                        }
                    case 2:     // Login Radius
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Login_Radius);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Login_Radius);
                            break;
                        }
                    case 4:     // Ip
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }                    
                    default:
                        listaOrdenada = retorno.OrderBy(p => p.Desc_Cliente);
                        break;
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<RelatorioConexaoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/Relatorio_Conexaoapi/GetRelatorioByLogin")]
        public async Task<DataTableResponse<RelatorioHistoricoConexaoModel>> GetRelatorioConexaoByLogin(GetHistoricoConexaoRadius_DataTableRequest request)
        {
            List<RelatorioHistoricoConexaoModel> retorno = new List<RelatorioHistoricoConexaoModel>();
            GetHistoricoConexaoRadius_Response retornoApi = new GetHistoricoConexaoRadius_Response();
            IOrderedEnumerable<RelatorioHistoricoConexaoModel> listaOrdenada = null;            

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            {
                var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);

                request.Login_Radius = loginRadius.Login;

                retornoApi = await RadiusRepository.GetHistoricoConexao(anielRadius, sessionRadius, request);
            }

            //TODO Criar Ordenação
            listaOrdenada = retornoApi.lista.OrderBy(p => p.Hora_Login);

            //Page e retornando DataTable
            return new DataTableResponse<RelatorioHistoricoConexaoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retornoApi.recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpGet]
        public async Task<GetApiRetorno<List<RelatorioHistoricoConexaoModel>>> GetRelatorioConexaoByLogin(long Id_Login_Radius)
        {
            GetApiRetorno<List<RelatorioHistoricoConexaoModel>> retorno = new GetApiRetorno<List<RelatorioHistoricoConexaoModel>>();

            try 
            {
                GetHistoricoConexaoRadius_Response retornoApi = new GetHistoricoConexaoRadius_Response();
                List<RelatorioHistoricoConexaoModel> listaOrdenada = null;

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == Id_Login_Radius);
                    var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);

                    var request = new GetHistoricoConexaoRadius_DataTableRequest { Id_Login_Radius = loginRadius.Id, Login_Radius = loginRadius.Login };

                    retornoApi = await RadiusRepository.GetHistoricoConexao(anielRadius, sessionRadius, request);
                }
                
                //TODO Criar Ordenação
                listaOrdenada = retornoApi.lista.Count() > 0 ? retornoApi.lista.OrderBy(p => p.Hora_Login).ToList() : retornoApi.lista;

                retorno.Objeto = listaOrdenada;
                retorno.Ok = true;
            }
            catch (Exception ex) 
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                if (ex.InnerException is FluentNHibernate.Cfg.FluentConfigurationException)
                    retorno.Mensagem = "Configuração de conexão ao Radius inválida ou incompleta.";
                else
                    retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }
            
            return retorno;
        }

        [Obsolete("Método substituído pela requisição WebSocket GraficoConexaoTempoReal().")]
        [HttpPost()]
        [Route("api/Relatorio_Conexaoapi/GetGraficoConexaoTempoReal")]
        public async Task<GetApiRetorno<ConsumoPeriodoModel>> GetGraficoConexaoTempoReal(ConexaoTempoReal_Request request)
        {
            GetApiRetorno<ConsumoPeriodoModel> retorno = new GetApiRetorno<ConsumoPeriodoModel>();           
            /*SELECT                
                @HORA_INICIO,
                acctstoptime, //Confirmar qual campo deve ser considerado como data_hora final (e se é necessário)
                SUM(acctinputoctets) AS acctinputoctets,
                SUM(acctoutputoctets) AS acctoutputoctets
            FROM
                radacct
            WHERE
                username = @USERNAME
                acctstoptime > @HORA_INICIO OR
                acctstoptime IS NULL
            GROUP BY
                username
              
             */
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoLoginRadius = new Conexao<LoginRadius>(session)) 
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                    var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);

                    //var retornoApi = await RadiusRepository.GetConexaoTempoReal(anielRadius, sessionRadius, loginRadius.Login, request.Unidade_Medida_Velocidade); //Requisição original

                    //retorno.Objeto = retornoApi;
                    /*retorno.Objeto.Consumo_Periodo = retornoApi;

                    retorno.Objeto.DataHora_Inicio = request.Data_Inicial_String;
                    retorno.Objeto.DataHora_Fim = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                    retorno.Objeto.Download_Atual = retornoApi.Download;
                    retorno.Objeto.Download_Atual_Str = retornoApi.DownloadString;
                    retorno.Objeto.Download_Total += retornoApi.Download;
                    retorno.Objeto.Download_Total_Str = OctetsToKbMbGb(retorno.Objeto.Download_Total); //TODO fazer conversão de acordo com Medida_Velocidade_ConexaoEnum
                    retorno.Objeto.Download_Maximo = retornoApi.Download > retorno.Objeto.Download_Maximo ? retornoApi.Download : retorno.Objeto.Download_Maximo;
                    retorno.Objeto.Download_Maximo_Str = retornoApi.Download > retorno.Objeto.Download_Maximo ? retornoApi.DownloadString : retorno.Objeto.Download_Maximo_Str;

                    retorno.Objeto.Upload_Atual = retornoApi.Upload;
                    retorno.Objeto.Upload_Atual_Str = retornoApi.UploadString;
                    retorno.Objeto.Upload_Total += retornoApi.Upload;
                    retorno.Objeto.Upload_Total_Str = OctetsToKbMbGb(retorno.Objeto.Upload_Total); //TODO fazer conversão de acordo com Medida_Velocidade_ConexaoEnum
                    retorno.Objeto.Upload_Maximo = retornoApi.Upload > retorno.Objeto.Upload_Maximo ? retornoApi.Upload : retorno.Objeto.Upload_Maximo;
                    retorno.Objeto.Upload_Maximo_Str = retornoApi.Upload > retorno.Objeto.Upload_Maximo ? retornoApi.UploadString : retorno.Objeto.Upload_Maximo_Str;*/
                }

                retorno.Ok = true;
            }
            catch(Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Relatorio_Conexaoapi/GetGraficoConsumoPeriodo")]
        public async Task<GetApiRetorno<GraficoConsumoPeriodoModel>> GetGraficoConsumoPeriodo(GraficoConsumoPeriodo_Request request)
        {
            GetApiRetorno<GraficoConsumoPeriodoModel> retorno = new GetApiRetorno<GraficoConsumoPeriodoModel>();
            List<ConsumoPeriodoModel> retornoApi = new List<ConsumoPeriodoModel>();

            request.Data_Inicial = !String.IsNullOrEmpty(request.Data_Inicial_String) ? Convert.ToDateTime(request.Data_Inicial_String) : (DateTime?)null;
            var dataInicio = request.Data_Inicial?.ToString("yyyy-MM-ddTHH:mm:ssZ");
            try
            {
                
                request.Data_Fim = !String.IsNullOrEmpty(request.Data_Fim_String) ? Convert.ToDateTime(request.Data_Fim_String) : (DateTime?)null;

                if (String.IsNullOrEmpty(dataInicio))
                {
                    switch (request.Periodo_Consumo)
                    {
                        case HistoricoConsumoPeriodoEnum.Dia:
                            dataInicio = request.Data_Fim.Value.AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy-MM-ddTHH:mm:ssZ");
                            break;
                        case HistoricoConsumoPeriodoEnum.Semana:
                            dataInicio = request.Data_Fim.Value.AddDays(-6).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy-MM-ddTHH:mm:ssZ");
                            break;
                        case HistoricoConsumoPeriodoEnum.Mes:
                            dataInicio = request.Data_Fim.Value.AddDays(-29).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy-MM-ddTHH:mm:ssZ");
                            break;
                        default:
                            request.Data_Fim.Value.AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy-MM-ddTHH:mm:ssZ");
                            break;
                    }
                };

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                    var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);
                   
                    retornoApi = await RadiusRepository.GetConsumoPeriodo(anielRadius, sessionRadius, loginRadius.Login, request.Periodo_Consumo, DateTime.Parse(dataInicio), request.Data_Fim);                                                           

                    if (retornoApi.Count > 0)
                    {
                        var consumoAtual = retornoApi.OrderByDescending(p => p.DataHoraConsumo).First();
                        var downloadMax = OctetsToKbMbGb(retornoApi.Max(p => p.Download));
                        var uploadMax = OctetsToKbMbGb(retornoApi.Max(p => p.Upload));
                        var downloadMedio = OctetsToKbMbGb(retornoApi.Sum(p => p.Download) / retornoApi.Count);
                        var uploadMedio = OctetsToKbMbGb(retornoApi.Sum(p => p.Upload) / retornoApi.Count);

                        retorno.Objeto = new GraficoConsumoPeriodoModel
                        {
                            Download_Atual = consumoAtual.DownloadString,
                            Download_Medio = downloadMedio,
                            Download_Maximo = downloadMax,
                            Upload_Atual = consumoAtual.UploadString,
                            Upload_Medio = uploadMedio,
                            Upload_Maximo = uploadMax,
                            ListaConsumoPeriodo = retornoApi,//.OrderBy(p => p.DataHoraConsumo).ToList(),
                            DataHora_Inicio = dataInicio,
                            DataHora_Fim = request.Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo ? 
                            request.Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy-MM-ddTHH:mm:ssZ") : 
                            request.Data_Fim.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")
                        };
                    }
                    else
                    {
                        retorno.Objeto = new GraficoConsumoPeriodoModel
                        {
                            Download_Atual = "0Kb",
                            Download_Medio = "0Kb",
                            Download_Maximo = "0Kb",
                            Upload_Atual = "0Kb",
                            Upload_Medio = "0Kb",
                            Upload_Maximo = "0Kb",
                            ListaConsumoPeriodo = new List<ConsumoPeriodoModel>(),
                            DataHora_Inicio = dataInicio,
                            DataHora_Fim = request.Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo ?
                            request.Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy-MM-ddTHH:mm:ssZ") :
                            request.Data_Fim.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")
                        };
                    }
                }
                
                retorno.Ok = true;
            }
            catch (Exception ex)
            {//{"An invalid or incomplete configuration was used while creating a SessionFactory. Check PotentialReasons collection, and InnerException for more detail.\r\n\r\n  * Database was not configured through Database method.\r\n"}
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;

                retorno.Objeto = new GraficoConsumoPeriodoModel
                {
                    Download_Atual = "0Kb",
                    Download_Medio = "0Kb",
                    Download_Maximo = "0Kb",
                    Upload_Atual = "0Kb",
                    Upload_Medio = "0Kb",
                    Upload_Maximo = "0Kb",
                    ListaConsumoPeriodo = new List<ConsumoPeriodoModel>(),
                    DataHora_Inicio = dataInicio,
                    DataHora_Fim = request.Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo ?
                            request.Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy-MM-ddTHH:mm:ssZ") :
                            request.Data_Fim.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                if (ex.InnerException is FluentNHibernate.Cfg.FluentConfigurationException)
                    retorno.Mensagem = "Configuração de conexão ao Radius inválida ou incompleta.";
                else
                    retorno.Mensagem = ex.Message;
            }
            
            return retorno;
        }

        [HttpPost()]
        [Route("api/Relatorio_Conexaoapi/GetGraficoTempoOnline")]
        public async Task<GetApiRetorno<GraficoTempoOnlineOfflineModel>> GetGraficoTempoOnline(GraficoTempoOnlineOffline_Request request)
        {
            GetApiRetorno<GraficoTempoOnlineOfflineModel> retorno = new GetApiRetorno<GraficoTempoOnlineOfflineModel>();
            List<TempoOnlineOfflineModel> retornoApi = new List<TempoOnlineOfflineModel>();

            request.Data_Inicial = !String.IsNullOrEmpty(request.Data_Inicial_String) ? Convert.ToDateTime(request.Data_Inicial_String) : (DateTime?)null;
            var dataInicio = request.Data_Inicial?.ToString("yyyy/MM/dd HH:mm:ss");
            try
            {
                
                request.Data_Fim = !String.IsNullOrEmpty(request.Data_Fim_String) ? Convert.ToDateTime(request.Data_Fim_String) : (DateTime?)null;

                if (String.IsNullOrEmpty(dataInicio))
                {
                    switch (request.Periodo_Consumo)
                    {
                        case HistoricoConsumoPeriodoEnum.Dia:
                            dataInicio = request.Data_Fim.Value.AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        case HistoricoConsumoPeriodoEnum.Semana:
                            dataInicio = request.Data_Fim.Value.AddDays(-6).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        case HistoricoConsumoPeriodoEnum.Mes:
                            dataInicio = request.Data_Fim.Value.AddDays(-29).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        default:
                            request.Data_Fim.Value.AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                    }
                };

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                    var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);

                    retornoApi = await RadiusRepository.GetTempoOnline(anielRadius, sessionRadius, loginRadius.Login, DateTime.Parse(dataInicio), request.Data_Fim.Value);

                    retorno.Objeto = new GraficoTempoOnlineOfflineModel 
                    { 
                        ListaTempoOnlineOffline = retornoApi,
                        DataHora_Inicio = dataInicio,
                        DataHora_Fim = request.Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo ?
                            request.Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy/MM/dd HH:mm:ss") :
                            request.Data_Fim.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;

                retorno.Objeto = new GraficoTempoOnlineOfflineModel
                {
                    ListaTempoOnlineOffline = new List<TempoOnlineOfflineModel>(),
                    DataHora_Inicio = dataInicio,
                    DataHora_Fim = request.Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo ?
                            request.Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy/MM/dd HH:mm:ss") :
                            request.Data_Fim.Value.ToString("yyyy-MM-dd HH:mm:ss")
                };

                if (ex.InnerException is FluentNHibernate.Cfg.FluentConfigurationException)
                    retorno.Mensagem = "Configuração de conexão ao Radius inválida ou incompleta.";
                else
                    retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Relatorio_Conexaoapi/GetGraficoTempoOffline")]
        public async Task<GetApiRetorno<GraficoTempoOnlineOfflineModel>> GetGraficoTempoOffline(GraficoTempoOnlineOffline_Request request)
        {
            GetApiRetorno<GraficoTempoOnlineOfflineModel> retorno = new GetApiRetorno<GraficoTempoOnlineOfflineModel>();
            List<TempoOnlineOfflineModel> retornoApi = new List<TempoOnlineOfflineModel>();

            request.Data_Inicial = !String.IsNullOrEmpty(request.Data_Inicial_String) ? Convert.ToDateTime(request.Data_Inicial_String) : (DateTime?)null;
            var dataInicio = request.Data_Inicial?.ToString("yyyy/MM/dd HH:mm:ss");
            try
            {
                
                request.Data_Fim = !String.IsNullOrEmpty(request.Data_Fim_String) ? Convert.ToDateTime(request.Data_Fim_String) : (DateTime?)null;

                if (String.IsNullOrEmpty(dataInicio))
                {
                    switch (request.Periodo_Consumo)
                    {
                        case HistoricoConsumoPeriodoEnum.Dia:
                            dataInicio = request.Data_Fim.Value.AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        case HistoricoConsumoPeriodoEnum.Semana:
                            dataInicio = request.Data_Fim.Value.AddDays(-6).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        case HistoricoConsumoPeriodoEnum.Mes:
                            dataInicio = request.Data_Fim.Value.AddDays(-29).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        default:
                            request.Data_Fim.Value.AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                    }
                };

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                    var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);                    

                    retornoApi = await RadiusRepository.GetTempoOffline(anielRadius, sessionRadius, loginRadius.Login, DateTime.Parse(dataInicio), request.Data_Fim.Value);

                    retorno.Objeto = new GraficoTempoOnlineOfflineModel
                    {
                        ListaTempoOnlineOffline = retornoApi,
                        DataHora_Inicio = dataInicio,
                        DataHora_Fim = request.Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo ?
                            request.Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy/MM/dd HH:mm:ss") :
                            request.Data_Fim.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;

                retorno.Objeto = new GraficoTempoOnlineOfflineModel
                {
                    ListaTempoOnlineOffline = new List<TempoOnlineOfflineModel>(),
                    DataHora_Inicio = dataInicio,
                    DataHora_Fim = request.Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo ?
                            request.Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy/MM/dd HH:mm:ss") :
                            request.Data_Fim.Value.ToString("yyyy-MM-dd HH:mm:ss")
                };

                if (ex.InnerException is FluentNHibernate.Cfg.FluentConfigurationException)
                    retorno.Mensagem = "Configuração de conexão ao Radius inválida ou incompleta.";
                else
                    retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        [Obsolete("Método substituído pela requisição WebSocket GraficoPingClienteConcentradora().")]
        [HttpPost]
        [Route("api/Relatorio_Conexaoapi/GetGraficoPing")]
        public async Task<GetApiRetorno<List<GraficoPingModel>>> GetGraficoPing(GraficoPing_Request request)
        {
            GetApiRetorno<List<GraficoPingModel>> retorno = new GetApiRetorno<List<GraficoPingModel>>();
            List<GraficoPingModel> lista_ping = new List<GraficoPingModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                    var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);

                    string ip = String.Empty;
                    if (request.pingConcentradora)
                    {
                        ip = loginRadius.Concentradora.Ip;
                    }
                    else
                    {
                        ip = !String.IsNullOrEmpty(loginRadius.EnderecoIPV4) ? loginRadius.EnderecoIPV4 : loginRadius.EnderecoIPV6;
                    }
                    var dataHora = DateTime.Now;
                    Ping ping = new Ping();
                    PingReply reply = await ping.SendPingAsync(ip);

                    retorno.Objeto.Add(new GraficoPingModel
                    {
                        latencia = reply.RoundtripTime,
                        DataHoraRequisicao = dataHora.ToString("HH:mm:ss")
                    });
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
           
            return retorno;
        }


        [HttpPost]
        [Route("api/Relatorio_Conexaoapi/GetPing")]
        public async Task<Response> GetPing(GraficoPing_Request request)
        {
            var retorno = new Response();
            List<GraficoPingModel> lista_ping = new List<GraficoPingModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                    var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);

                    string ip = !String.IsNullOrEmpty(loginRadius.EnderecoIPV4) ? loginRadius.EnderecoIPV4 : loginRadius.EnderecoIPV6;
                    
                    Ping ping = new Ping();
                    PingReply reply = await ping.SendPingAsync(ip);

                    if(reply.Status != IPStatus.Success)
                    {
                        retorno.result = false;
                        retorno.message = "Não foi possível se comunicar com o equipamento. Tempo limite excedido." ;
                    }
                    else
                    {
                        retorno.result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.result = false;
                retorno.message = ex.Message;
            }

            return retorno;
        }


        [HttpGet]
        public HttpResponseMessage GraficoWebSocket(int Tipo_grafico)
        {
            var httpContext = Request.Properties["MS_HttpContext"] as HttpContextBase;           
                       
            if (httpContext.IsWebSocketRequest)
            {
                switch ((WebSocketGraficosEnum)Tipo_grafico)
                {
                    case WebSocketGraficosEnum.Grafico_Ping:
                        httpContext.AcceptWebSocketRequest(GraficoPingClienteConcentradora);
                        break;
                    case WebSocketGraficosEnum.Grafico_Conexao_Tempo_Real:
                        httpContext.AcceptWebSocketRequest(GraficoConexaoTempoReal);
                        break;
                }
            }
                
            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        private async Task GraficoPingClienteConcentradora(System.Web.WebSockets.AspNetWebSocketContext context)
        {
            var socket = context.WebSocket;
            var retorno = new GraficoPingModel();             
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                //using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                {                   
                    string ip = String.Empty;

                    ArraySegment<byte> bufferReceive = new ArraySegment<byte>(new byte[1024]);
                    WebSocketReceiveResult result = await socket.ReceiveAsync(
                        bufferReceive, CancellationToken.None);

                    var request = JsonService.DeserializeObject<GraficoPing_Request>(Encoding.UTF8.GetString(bufferReceive.Array).Replace("\0", String.Empty));

                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                    var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);

                    if (request.pingConcentradora)
                    {
                        ip = loginRadius.Concentradora.Ip;
                    }
                    else
                    {
                        ip = !String.IsNullOrEmpty(loginRadius.EnderecoIPV4) ? loginRadius.EnderecoIPV4 : loginRadius.EnderecoIPV6;
                    }                                      

                    while (socket.State == System.Net.WebSockets.WebSocketState.Open)
                    {
                        var dataHora = DateTime.Now;
                        Ping ping = new Ping();
                        PingReply reply = ping.Send(ip);

                        retorno = new GraficoPingModel
                        {
                            latencia = reply.RoundtripTime,
                            DataHoraRequisicao = dataHora.ToString("HH:mm:ss")
                        };

                        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonService.SerializeObject(retorno)));
                        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);

                        await Task.Delay(1000);
                    }

                        
                }

                    //retorno.Ok = true;
                }
                catch (Exception ex)
                {
                //retorno.Ok = false;
                //retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
                var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonService.SerializeObject(ex)));
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                    socket.Abort();
                }                                                   
        }
      
        private async Task GraficoConexaoTempoReal(System.Web.WebSockets.AspNetWebSocketContext context)
        {
            var socket = context.WebSocket;
            GetApiRetorno<ConsumoPeriodoModel> retorno = new GetApiRetorno<ConsumoPeriodoModel>();           
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                {
                    ArraySegment<byte> bufferReceive = new ArraySegment<byte>(new byte[1024]);
                    WebSocketReceiveResult result = await socket.ReceiveAsync(
                        bufferReceive, CancellationToken.None);

                    var request = JsonService.DeserializeObject<ConexaoTempoReal_Request>(Encoding.UTF8.GetString(bufferReceive.Array).Replace("\0", String.Empty));

                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                    var anielRadius = new AnielRadius(loginRadius.Concentradora.Radius);

                    double LastTotalUploadOctects = 0.0;
                    double LastTotalDownloadOctects = 0.0;
                    DateTime DataUltimaAtualizacao = DateTime.MinValue;

                    bool FirstRun = true;
                    double LastUpdateDownloadOctets = 0.0;
                    double LastUpdateUploadOctets = 0.0;

                    while (socket.State == System.Net.WebSockets.WebSocketState.Open)
                    {                        
                        var retornoApi = await RadiusRepository.GetConexaoTempoReal(anielRadius, sessionRadius, loginRadius.Login, request.Unidade_Medida_Velocidade, LastTotalDownloadOctects, LastTotalUploadOctects);                        

                        //Quando a data for igual, significa que o valor de consumo não foi alterado entre o intervalo das requisições,
                        //então atualiza a data apenas para manter a constante no gráfico evitando inconsistências.
                        //Nesse caso também não deve atualizar as informações de Download e Upload Total,
                        //pois é apenas a mesma informação repetida.
                        if(retornoApi.DataHoraConsumo == DataUltimaAtualizacao)
                        {
                            retornoApi.DataHoraConsumo = DateTime.Now;
                            retornoApi.DataHoraConsumoFormatada = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            retornoApi.Download = LastUpdateDownloadOctets;
                            retornoApi.Upload = LastUpdateUploadOctets;
                            retornoApi.AtualizarConsumoTotal = false;
                        }                        
                        else
                        {                            
                            DataUltimaAtualizacao = retornoApi.DataHoraConsumo;
                            LastUpdateDownloadOctets = retornoApi.Download;
                            LastUpdateUploadOctets = retornoApi.Upload;
                            retornoApi.AtualizarConsumoTotal = true;
                        }

                        retorno.Objeto = retornoApi;                       

                        LastTotalDownloadOctects = retornoApi.LastTotalDownloadOctects;
                        LastTotalUploadOctects = retornoApi.LastTotalUploadOctects;

                        if (FirstRun || retornoApi.LastTotalDownloadOctects == LastUpdateDownloadOctets) 
                        {
                            retornoApi.Download = 0.0;
                            retornoApi.DownloadString = "0Kb";
                        }                            
                        
                        if (FirstRun || retornoApi.LastTotalUploadOctects == LastUpdateUploadOctets) 
                        {
                            retornoApi.Upload = 0.0;
                            retornoApi.UploadString = "0Kb";
                        }

                        if (FirstRun)
                        {                            
                            FirstRun = false;
                        }

                        retorno.Ok = true;

                        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonService.SerializeObject(retorno)));
                        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);

                        await Task.Delay(3000);
                    }
                }                
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonService.SerializeObject(retorno)));
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                socket.Abort();                
            }           
        }

        [HttpPost()]
        [Route("api/Relatorio_Conexaoapi/GetGraficoPotenciaRxTx")]
        public async Task<GetApiRetorno<GraficoPotenciaRxTxModel_Response>> GetGraficoPotenciaRxTx(GraficoPotenciaRxTxModel request)
        {
            GetApiRetorno<GraficoPotenciaRxTxModel_Response> retorno = new GetApiRetorno<GraficoPotenciaRxTxModel_Response>();
            List<PotenciaRxTxModel_Response> listaRetorno = new List<PotenciaRxTxModel_Response>();

            request.Data_Inicial = !String.IsNullOrEmpty(request.Data_Inicial_String) ? Convert.ToDateTime(request.Data_Inicial_String) : (DateTime?)null;
            var dataInicio = request.Data_Inicial?.ToString("yyyy/MM/dd HH:mm:ss");
            try
            {
                
                request.Data_Fim = !String.IsNullOrEmpty(request.Data_Fim_String) ? Convert.ToDateTime(request.Data_Fim_String) : (DateTime?)null;

                if (String.IsNullOrEmpty(dataInicio))
                {
                    switch (request.Periodo_Consumo)
                    {
                        case HistoricoConsumoPeriodoEnum.Dia:
                            dataInicio = request.Data_Fim.Value.AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        case HistoricoConsumoPeriodoEnum.Semana:
                            dataInicio = request.Data_Fim.Value.AddDays(-6).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        case HistoricoConsumoPeriodoEnum.Mes:
                            dataInicio = request.Data_Fim.Value.AddDays(-29).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                        default:
                            request.Data_Fim.Value.AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss");
                            break;
                    }
                };

                using (ISession session = HibernateUtil.GetSession())
                //using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repoOnuMongo = new RepositoryMongo<OnuMongo>(conexaoMongo))
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                using (var repoOnu = new Conexao<ONU>(session))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == request.Id_Login_Radius);
                    //var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);
                    var onu = await repoOnu.selectFirst(p => p.LoginRadius.Id == loginRadius.Id);

                    //var data_atual = DateTime.Now;
                    List<OnuMongo> listaOnuMongo = new List<OnuMongo>();
                    listaOnuMongo = repoOnuMongo.GetQueryable().Where(x => x.IdOnu == onu.Id &&
                                                                                x.DataDocumento >= DateTime.Parse(dataInicio) &&
                                                                                x.DataDocumento < request.Data_Fim.Value).ToList();
                    if(listaOnuMongo != null &&  listaOnuMongo.Count > 0)
                    {
                        List<DiagnosticoOnuMongo> listaDiagnosticos = new List<DiagnosticoOnuMongo>();

                        foreach (var onuMongo in listaOnuMongo)
                            listaDiagnosticos.AddRange(onuMongo.Diagnosticos);

                        foreach (var diagnostico in listaDiagnosticos)
                            listaRetorno.Add(ConvertDiagnosticoOnuMongoToView(diagnostico));
                    }

                    //listaRetorno = MockPotenciaRxTx(); //RETIRAR POIS RETORNA DADOS MOCKADOS
                    retorno.Objeto = new GraficoPotenciaRxTxModel_Response
                    {
                        ListaPotenciaRxTx = listaRetorno,
                        DataHora_Inicio = dataInicio,
                        DataHora_Fim = request.Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo ?
                            request.Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy/MM/dd HH:mm:ss") :
                            request.Data_Fim.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    }; 
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);

                retorno.Objeto = new GraficoPotenciaRxTxModel_Response
                {
                    ListaPotenciaRxTx = new List<PotenciaRxTxModel_Response>(),
                    DataHora_Inicio = dataInicio,
                    DataHora_Fim = request.Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo ?
                            request.Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy/MM/dd HH:mm:ss") :
                            request.Data_Fim.Value.ToString("yyyy-MM-dd HH:mm:ss")
                };

                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        private PotenciaRxTxModel_Response ConvertDiagnosticoOnuMongoToView(DiagnosticoOnuMongo objBanco)
        {
            PotenciaRxTxModel_Response retorno = new PotenciaRxTxModel_Response();

            retorno.DataHoraConsulta = objBanco.DataHoraConsulta.ToString("yyyy/MM/dd HH:mm:ss");
            retorno.Potencia_Rx = objBanco.Potencia_Rx;
            retorno.Potencia_Tx = objBanco.Potencia_Tx;

            return retorno;
        }

        private List<PotenciaRxTxModel_Response> MockPotenciaRxTx()
        {
            List<PotenciaRxTxModel_Response> retorno = new List<PotenciaRxTxModel_Response>();

            Random rnd = new Random();

            var tipo_mock = rnd.Next(3);

            switch(tipo_mock)
            {
                case 0: //MOCK POTÊNCIA RANDÔMICO
                    var quant_repeticoes = rnd.Next(40);
                    for (var i = 0; i < quant_repeticoes; i++)
                    {
                        retorno.Add(new PotenciaRxTxModel_Response
                        {
                            Id_Log_Potencia_Rx_Tx = i,
                            Potencia_Rx = (rnd.NextDouble() * Math.Abs((-1) - (-25)) + (-25)), //Gerar valores negativos aleatórios 
                            Potencia_Tx = (rnd.NextDouble() * Math.Abs((1) + (25))), //Gerar valores positivos aleatórios 
                            DataHoraConsulta = DateTime.Now.AddDays(-rnd.Next(7)).AddHours(rnd.Next(24)).ToString("yyyy-MM-dd HH:mm:ss") //Gerar datas aleatórias dentre os últimos 7 dias
                        });
                    }
                    break;
                case 1: //MOCK POTÊNCIA ESTÁVEL
                    for(var i = 7; i >= 0; i--)
                    {
                        retorno.Add(new PotenciaRxTxModel_Response
                        {
                            Id_Log_Potencia_Rx_Tx = i,
                            Potencia_Rx = -20.0,  
                            Potencia_Tx = 1.1,  
                            DataHoraConsulta = DateTime.Now.AddDays(-i).AddHours(-6).ToString("yyyy-MM-dd HH:mm:ss") 
                        });

                        retorno.Add(new PotenciaRxTxModel_Response
                        {
                            Id_Log_Potencia_Rx_Tx = i,
                            Potencia_Rx = -20.2, 
                            Potencia_Tx = 1.3, 
                            DataHoraConsulta = DateTime.Now.AddDays(-i).AddHours(-3).ToString("yyyy-MM-dd HH:mm:ss")
                        });

                        retorno.Add(new PotenciaRxTxModel_Response
                        {
                            Id_Log_Potencia_Rx_Tx = i,
                            Potencia_Rx = -20.1,  
                            Potencia_Tx = 1.2,
                            DataHoraConsulta = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd HH:mm:ss")
                        });

                        retorno.Add(new PotenciaRxTxModel_Response
                        {
                            Id_Log_Potencia_Rx_Tx = i,
                            Potencia_Rx = -20.0, 
                            Potencia_Tx = 1.0,
                            DataHoraConsulta = DateTime.Now.AddDays(-i).AddHours(3).ToString("yyyy-MM-dd HH:mm:ss")
                        });

                        retorno.Add(new PotenciaRxTxModel_Response
                        {
                            Id_Log_Potencia_Rx_Tx = i,
                            Potencia_Rx = -20.2,
                            Potencia_Tx = 1.1,
                            DataHoraConsulta = DateTime.Now.AddDays(-i).AddHours(6).ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }
                    break;
                case 2: //MOCK RETORNO VAZIO

                    break;
            }
                      

            return retorno;
        }

        public enum WebSocketGraficosEnum
        {
            Grafico_Ping = 0,
            Grafico_Conexao_Tempo_Real = 1
        }

        private String OctetsToKbMbGb(Double octets_velocidade)
        {
            String velocidade_convertida = String.Empty;
            try
            {
                Double velocidade_Kb = (octets_velocidade / 1000);
                Double velocidade_Mb = velocidade_Kb / 1000;
                Double velocidade_Gb = velocidade_Mb / 1000;

                if (velocidade_Mb > 1024d) //Manter em Gb 
                {
                    velocidade_convertida = String.Format("{0} Gb", velocidade_Gb.ToString("0.##"));
                }
                else if (velocidade_Kb > 1024d) //Manter em Mb
                {
                    velocidade_convertida = String.Format("{0} Mb", velocidade_Mb.ToString("0.##"));
                }
                else //Manter em Kb
                {
                    velocidade_convertida = String.Format("{0} Kb", velocidade_Kb.ToString("0.##"));
                }

                return velocidade_convertida;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return "-";
            }
        }

        [HttpPost()]
        [Route("api/Relatorio_Conexaoapi/GetAllPainelHistorico")]
        public async Task<DataTableResponse<RelatorioPainelHistoricoAutenticacao>> GetAllPainelHistorico(RelatorioPainelHistoricoAutenticacao_DataTableRequest request)
        {
            List<Sinapse.Integracao.Ativacao.Models.Pool_IpModel> listaNomesPoolIP = new List<Sinapse.Integracao.Ativacao.Models.Pool_IpModel>();
            DataTableResponse<RelatorioPainelHistoricoAutenticacao> retornoRadius = null;

           
               

                if (CONSTANTES.UsaRadius != "S")
                {
                    throw new Exception("A aplicação não está configurada para uso do Radius! Por favor, contate o suporte para mais detalhes.");
                }

                try
                {
                    using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                    using (ISession session = HibernateUtil.GetSession())
                    using (var repoRadius = new Conexao<Sinapse.Model.Radius>(session))
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();

                        StringBuilder sql = new StringBuilder();

                        //O radius atualmente exibe o nome da pool como está cadastrada no Connect
                        //exceto as pools que são controladas pela concentradora. Estas pools possuem
                        //um nome diferente da descrição do cadastro. Por isso, este select irá
                        //auxiliar a mostrar os nomes da pool como estão no Connect
                        using (var com = session.Connection.CreateCommand())
                        {
                            sql.AppendLine("SELECT DESCRICAO, NOME_POOL_CONCENTRADORA FROM TB_POOL_IP ");

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {

                                    listaNomesPoolIP.Add(new Sinapse.Integracao.Ativacao.Models.Pool_IpModel
                                    {
                                        Descricao = Convert.ToString(reader["DESCRICAO"]),
                                        NomePoolConcentradora = Convert.ToString(reader["NOME_POOL_CONCENTRADORA"])
                                    });
                                }
                            }
                        }

                        var radius = await repoRadius.selectFirst();

                        retornoRadius = await new AnielRadius(radius).GetDadosPainelHistoricoAutenticacoes(request, listaNomesPoolIP);

                        //Count
                        //recordsTotal = retornoRadius.Count;
                        /* listaOrdenada = retornoRadius
                         .OrderBy(x => DateTime.TryParse(x.Data_Autenticacao, out var date) ? date : DateTime.MinValue);*/



                    }

                }
                catch(Exception ex)
                {
                      if(ex.InnerException.ToString().ToUpperInvariant().Contains("Database was not configured".ToUpperInvariant()))
                      {
                        throw new Exception("O banco de dados do servidor RADIUS não está configurado corretamente ou não está acessível. Verifique a configuração e o status do banco de dados e tente novamente.");
                      }
                      else
                      {
                        throw new Exception(ex.Message);

                      }
                   

                }

               


                //Page e retornando DataTable
                return new DataTableResponse<RelatorioPainelHistoricoAutenticacao>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = retornoRadius.recordsFiltered,
                    data = retornoRadius.data,
                    error = ""
                };
      
          
        }
    }
}