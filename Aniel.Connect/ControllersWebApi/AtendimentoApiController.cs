using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Controllers;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Socket.ChatHub;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Criterion;
using Resources;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.MetaBusiness.Integracoes;
using Sinapse.Integracao.WhatsAppBusiness.Repositorio;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Twilio;
using Twilio.Rest.Api.V2010.Account.Message;
using Twilio.Rest.Conversations.V1;
using Twilio.Rest.Conversations.V1.Conversation;
using static Aniel.Connect.Models.MessengerModels;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class AtendimentoApiController : BaseApiController
    {
        private static readonly object _lockDictionary = new object();
        private static readonly Dictionary<string, SemaphoreSlim> _semaforos = new Dictionary<string, SemaphoreSlim>();

        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/atendimentoapi/GetAll")]
        public async Task<DataTableResponse<AtendimentoListaModel>> GetAll(Atendimento_DataTableRequest request)
        {
            List<AtendimentoListaModel> retorno = new List<AtendimentoListaModel>();
            Int32 recordsTotal = 0;

            var SlaView = new Dictionary<String, Boolean>
                           {
                               { "#00FF00", request.Dentro_Prazo },
                               { "#FFFF00", request.Atencao },
                               { "#FF8C00", request.Vencimento_Proximo },
                               { "#FF0000", request.Risco_Perda_Prazo },
                               { "#000000", request.Prazo_Vencido }
                           }
                           .Where(dict => dict.Value == true);


            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO A ");

                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE B ");
                sqlFrom.AppendLine(" ON (A.ID_CLIENTE = B.ID) ");

                /*if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.PesquisaRapidaAtendimento == PesquisaRapidaAtendimento.Organizacao)
                    {
                        sqlFrom.AppendLine(" LEFT JOIN TB_ORGANIZACAO O ");
                        sqlFrom.AppendLine(" ON (B.ID_ORGANIZACAO = O.ID) ");
                    }
                }*/

                sqlFrom.AppendLine(" LEFT JOIN TB_CATEGORIA_ATENDIMENTO C ");
                sqlFrom.AppendLine(" ON (A.ID_CATEGORIA_ATENDIMENTO = C.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_TIPO_ATENDIMENTO D ");
                sqlFrom.AppendLine(" ON (A.ID_TIPO_ATENDIMENTO = D.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_MOTIVO_STATUS_ATENDIMENTO E ");
                sqlFrom.AppendLine(" ON (A.ID_MOTIVO_STATUS = E.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN ASPNETUSERS F ");
                sqlFrom.AppendLine(" ON (A.ID_RESPONSAVEL = F.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_DEPARTAMENTO G");
                sqlFrom.AppendLine(" ON (A.ID_DEPARTAMENTO = G.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE H ");
                sqlFrom.AppendLine(" ON(B.ID_CATEGORIA = H.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_ORIGEM_ATENDIMENTO I ");
                sqlFrom.AppendLine(" ON(A.ID_ORIGEM_ATENDIMENTO = I.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO  J ");
                sqlFrom.AppendLine(" ON(A.ID_PLANOPRODUTO = J.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_PLANO  K ");
                sqlFrom.AppendLine(" ON(J.ID_PLANO = K.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_PRODUTO  L ");
                sqlFrom.AppendLine(" ON(J.ID_PRODUTO = L.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_FLUXO_ATENDIMENTO  M ");
                sqlFrom.AppendLine(" ON(A.ID_ETAPA_FLUXO = M.ID) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_TROCA_TITULARIDADE N ");
                sqlFrom.AppendLine(" ON (N.ID = A.ID_TROCA_TITULAR) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_ORGANIZACAO O ");
                sqlFrom.AppendLine(" ON (O.ID = B.ID_ORGANIZACAO) ");

                sqlFrom.AppendLine(" WHERE A.ID > 0 ");

                //sqlFrom.AppendLine(" WHERE B.STATUS <> 3 ");

                //sqlFrom.AppendLine(" AND A.STATUS_ATENDIMENTO <> 4 AND A.STATUS_ATENDIMENTO <> 3 ");


                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                GetAllFiltros(request, sqlFrom, parametros);

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO  ");
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

                    sql.AppendLine("SELECT A.ID, A.NUMERO , A.PRIORIDADE_PADRAO, A.ASSUNTO,K.DESCRICAO AS PLANO , L.DESCRICAO AS PRODUTO, A.PESSOA_CONTATO,B.TELEFONE_FIXO AS TELEFONE,B.TELEFONE_CELULAR AS CELULAR,B.NOMEFANTASIA,B.EMAIL_PRINCIPAL, I.DESCRICAO AS ORIGEM, A.ID_ORIGEM_ATENDIMENTO,A.DATA_HORA_FECHAMENTO,F.USERNAME, A.ID_TIPO_ATENDIMENTO, D.NOME AS TIPO_ATENDIMENTO, G.DESCRICAO AS DEPARTAMENTO, A.ID_RESPONSAVEL, A.ID_MOTIVO_STATUS, A.PROBLEMA_RELATADO, A.DATAHORACRIACAO , A.STATUS_AUDITORIA, ");
                    sql.AppendLine(" C.DESCRICAO AS CATEGORIA_ATENDIMENTO, D.NOME AS NOME_ATENDIMENTO, E.DESCRICAO AS MOTIVO_STATUS, F.ID AS ID_USUARIO, F.USERNAME AS USUARIO, F.NOME AS NOME_RESPONSAVEL, ");
                    sql.AppendLine(" B.CPF, A.STATUS_ATENDIMENTO,H.DESCRICAO AS CATEGORIA_CLIENTE, B.ID_CATEGORIA AS ID_CATEGORIA_CLIENTE, A.DATAHORA_INDICADOR, B.LOGIN ,M.ID AS ID_ETAPA_ATUAL, O.NOME_FANTASIA AS ORGANIZACAO_CLIENTE, O.ID AS ID_ORGANIZACAO_CLIENTE, ");
                    sql.AppendLine("M.DESCRICAO AS ETAPAATUAL,M.SEQUENCIA,A.DATAINICIO_ETAPAFLUXO,A.DATAINDICADOR_ETAPAFLUXO,M.TEMPO_EXC");
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Status_Atendimento>("A.STATUS_ATENDIMENTO", "STATUS_ORDENAR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Prioridade_Atendimento>("A.PRIORIDADE_PADRAO", "PRIORIDADE_ORDENAR"));
                    sql.AppendLine(", N.ID AS NUMERO_TROCA, (SELECT Count(ID) FROM TB_VINCULAR_ATENDIMENTO P WHERE P.NUN_ATENDIMENTO = A.NUMERO) AS POSSUIOS ");

                    sql.AppendLine(sqlFrom.ToString());

                    //Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;
                        string sortDirection = request.order[0].dir;
                        switch (coluna)
                        {
                            case "numero":     // NUMERO
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.NUMERO");
                                    else
                                        sql.AppendLine("ORDER BY A.NUMERO DESC");
                                    break;
                                }
                            case "slaOrdem":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY (COALESCE(A.DATA_HORA_FECHAMENTO,A.DATAHORA_INDICADOR) - A.DATAHORACRIACAO) ");
                                    else
                                        sql.AppendLine("ORDER BY (COALESCE(A.DATA_HORA_FECHAMENTO,A.DATAHORA_INDICADOR) - A.DATAHORACRIACAO)  DESC");
                                    break;
                                }
                            case "sla":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY (COALESCE(A.DATA_HORA_FECHAMENTO,A.DATAHORA_INDICADOR) - A.DATAHORACRIACAO) ");
                                    else
                                        sql.AppendLine("ORDER BY (COALESCE(A.DATA_HORA_FECHAMENTO,A.DATAHORA_INDICADOR) - A.DATAHORACRIACAO)  DESC");
                                    break;
                                }
                            case "nomeAtendimento":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY D.NOME");
                                    else
                                        sql.AppendLine("ORDER BY D.NOME DESC");
                                    break;
                                }
                            case "dataHoraCriacao":     // DATA_CRIACAO
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.DATAHORACRIACAO ");

                                    else
                                        sql.AppendLine("ORDER BY A.DATAHORACRIACAO DESC");
                                    break;
                                }
                            case "prioridade_Padrao":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY PRIORIDADE_ORDENAR");
                                    else
                                        sql.AppendLine("ORDER BY PRIORIDADE_ORDENAR DESC");
                                    break;
                                }
                            case "status_Atendimento":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY STATUS_ORDENAR");
                                    else
                                        sql.AppendLine("ORDER BY STATUS_ORDENAR DESC");
                                    break;
                                }
                            case "motivo_Status":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY E.DESCRICAO ");
                                    else
                                        sql.AppendLine("ORDER BY E.DESCRICAO DESC");
                                    break;
                                }
                            case "responsavel":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY F.NOME ");
                                    else
                                        sql.AppendLine("ORDER BY F.NOME DESC ");
                                    break;
                                }
                            case "categoria_Atendimento":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY C.DESCRICAO ");
                                    else
                                        sql.AppendLine("ORDER BY C.DESCRICAO DESC ");
                                    break;
                                }
                            case "dataHoraFechamento":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.DATA_HORA_FECHAMENTO ");
                                    else
                                        sql.AppendLine("ORDER BY A.DATA_HORA_FECHAMENTO DESC ");
                                    break;
                                }
                            case "cliente":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY B.NOMEFANTASIA ");
                                    else
                                        sql.AppendLine("ORDER BY B.NOMEFANTASIA DESC ");
                                    break;
                                }
                            case "assunto":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.ASSUNTO ");
                                    else
                                        sql.AppendLine("ORDER BY A.ASSUNTO DESC ");
                                    break;
                                }
                            case "setor":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY G.DESCRICAO ");
                                    else
                                        sql.AppendLine("ORDER BY G.DESCRICAO DESC ");
                                    break;
                                }
                            case "login_Radius":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY B.LOGIN ");
                                    else
                                        sql.AppendLine("ORDER BY B.LOGIN DESC ");
                                    break;
                                }
                            case "origem":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY I.DESCRICAO ");
                                    else
                                        sql.AppendLine("ORDER BY I.DESCRICAO DESC ");
                                    break;
                                }
                            case "categoria_Cliente":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY H.DESCRICAO ");
                                    else
                                        sql.AppendLine("ORDER BY H.DESCRICAO DESC ");
                                    break;
                                }
                            case "email_Cliente":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY B.EMAIL_PRINCIPAL ");
                                    else
                                        sql.AppendLine("ORDER BY B.EMAIL_PRINCIPAL DESC ");
                                    break;
                                }
                            case "telefone":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY B.TELEFONE_FIXO ");
                                    else
                                        sql.AppendLine("ORDER BY B.TELEFONE_FIXO DESC ");
                                    break;
                                }
                            case "celular":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY B.TELEFONE_CELULAR ");
                                    else
                                        sql.AppendLine("ORDER BY B.TELEFONE_CELULAR DESC ");
                                    break;
                                }
                            case "plano":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY K.DESCRICAO ");
                                    else
                                        sql.AppendLine("ORDER BY K.DESCRICAO DESC ");
                                    break;
                                }
                            case "produto":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY L.DESCRICAO ");
                                    else
                                        sql.AppendLine("ORDER BY L.DESCRICAO DESC ");
                                    break;
                                }
                            case "etapa_Atual":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY ETAPAATUAL ");
                                    else
                                        sql.AppendLine("ORDER BY ETAPAATUAL DESC ");
                                    break;
                                }
                            case "prazo_Atual":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY M.TEMPO_EXC ");
                                    else
                                        sql.AppendLine("ORDER BY M.TEMPO_EXC DESC ");
                                    break;
                                }
                            case "tempo_Gasto":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.DATAINICIO_ETAPAFLUXO ");
                                    else
                                        sql.AppendLine("ORDER BY A.DATAINICIO_ETAPAFLUXO DESC ");
                                    break;
                                }
                            case "data_Sla_Etapa":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.DATAINDICADOR_ETAPAFLUXO ");
                                    else
                                        sql.AppendLine("ORDER BY A.DATAINDICADOR_ETAPAFLUXO DESC ");
                                    break;
                                }
                            case "statusAuditoria":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.STATUS_AUDITORIA ");
                                    else
                                        sql.AppendLine("ORDER BY A.STATUS_AUDITORIA DESC ");
                                    break;
                                }

                        }


                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {

                                var inicioEtapa = reader["DATAINICIO_ETAPAFLUXO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAINICIO_ETAPAFLUXO"]) : (DateTime?)null;
                                var slaEtapa = reader["DATAINDICADOR_ETAPAFLUXO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAINDICADOR_ETAPAFLUXO"]) : (DateTime?)null;
                                double? prazoAtual = reader["TEMPO_EXC"] != DBNull.Value ? Convert.ToDouble(reader["TEMPO_EXC"]) : (double?)null;
                                var statusAuditoria = reader["STATUS_AUDITORIA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusAuditoria>(Convert.ToString(reader["STATUS_AUDITORIA"])) : "";
                                var qtdos = reader["POSSUIOS"] != DBNull.Value ? Convert.ToInt32(reader["POSSUIOS"]) : 0;
                                var statusAtendimento = reader["STATUS_ATENDIMENTO"].ToString();
                                var statusAtendimentoEnum = (Status_Atendimento)Convert.ToInt32(reader["STATUS_ATENDIMENTO"]);
                                if (reader["STATUS_ATENDIMENTO"].ToString() == "0" || reader["STATUS_ATENDIMENTO"].ToString() == "5" || reader["STATUS_ATENDIMENTO"].ToString() == "2" || reader["STATUS_ATENDIMENTO"].ToString() == "3")
                                {
                                    statusAuditoria = "";
                                }



                                // Lista de status que NÃO devem preencher StatusAuditoria
                                var statusIgnorados = new[] {
                                    Status_Atendimento.A_Iniciar,
                                    Status_Atendimento.Em_Atendimento,
                                    Status_Atendimento.Aguardando,
                                    Status_Atendimento.Cancelado
                                };

                                if (!statusIgnorados.Contains(statusAtendimentoEnum))
                                {
                                    statusAuditoria = reader["STATUS_AUDITORIA"] != DBNull.Value
                                        ? HtmlEnumExtensions.GetEnumDescription<StatusAuditoria>(Convert.ToString(reader["STATUS_AUDITORIA"]))
                                        : "";
                                }

                                var nome = new List<string>
                                {
                                    !String.IsNullOrEmpty(Convert.ToString(reader["ID_USUARIO"])) ? Convert.ToString(reader["ID_USUARIO"]) : String.Empty,
                                    !String.IsNullOrEmpty(Convert.ToString(reader["NOME_RESPONSAVEL"])) ? Convert.ToString(reader["NOME_RESPONSAVEL"]) : String.Empty
                                };

                                var obj = new AtendimentoListaModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Numero = reader["NUMERO"] != DBNull.Value ? Convert.ToInt64(reader["NUMERO"]) : 0,
                                    Sla = "",
                                    Prioridade_Padrao = HtmlEnumExtensions.GetEnumDescription<Prioridade_Atendimento>(Convert.ToString(reader["PRIORIDADE_PADRAO"])),
                                    Assunto = Convert.ToString(reader["ASSUNTO"].ToString()),
                                    Pessoa_Contato = Convert.ToString(reader["PESSOA_CONTATO"].ToString()),
                                    Categoria_Atendimento = Convert.ToString(reader["CATEGORIA_ATENDIMENTO"].ToString()),
                                    Cliente = Convert.ToString(reader["NOMEFANTASIA"].ToString()),
                                    Id_Categoria_Cliente = reader["ID_CATEGORIA_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CATEGORIA_CLIENTE"]) : (Int64?)null,
                                    Categoria_Cliente = Convert.ToString(reader["CATEGORIA_CLIENTE"]),
                                    Origem_Atendimento = Convert.ToString(reader["ID_ORIGEM_ATENDIMENTO"]),
                                    Origem = Convert.ToString(reader["ORIGEM"]),
                                    Id_Tipo_Atendimento = reader["ID_TIPO_ATENDIMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_TIPO_ATENDIMENTO"]) : (Int64?)null,
                                    Tipo_Atendimento = Convert.ToString(reader["TIPO_ATENDIMENTO"]),
                                    Setor = Convert.ToString(reader["DEPARTAMENTO"]),
                                    Responsavel = String.Join("-", nome.Where(n => !string.IsNullOrEmpty(n))),
                                    Motivo_Status = Convert.ToString(reader["MOTIVO_STATUS"]),
                                    Problema_Relatado = Convert.ToString(reader["PROBLEMA_RELATADO"].ToString()),
                                    DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm:ss") : null,
                                    NomeAtendimento = reader["NOME_ATENDIMENTO"] != DBNull.Value ? Convert.ToString(reader["NOME_ATENDIMENTO"]) : "",
                                    DataHoraFechamento = reader["DATA_HORA_FECHAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_FECHAMENTO"]).ToString("dd/MM/yyyy HH:mm:ss") : String.Empty,
                                    Status_Atendimento = HtmlEnumExtensions.GetEnumDescription<Status_Atendimento>(Convert.ToString(statusAtendimento)),
                                    Login_Radius = reader["LOGIN"] != DBNull.Value ? Convert.ToString(reader["LOGIN"]) : "",
                                    Email_Cliente = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                    StatusAuditoria = statusAuditoria,// reader["STATUS_AUDITORIA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusAuditoria>(Convert.ToString(reader["STATUS_AUDITORIA"])) : "",
                                    Telefone = Convert.ToString(reader["TELEFONE"]),
                                    Celular = Convert.ToString(reader["CELULAR"]),
                                    Plano = Convert.ToString(reader["PLANO"]),
                                    Produto = Convert.ToString(reader["PRODUTO"]),
                                    Etapa_Atual = Convert.ToString(reader["ETAPAATUAL"]),
                                    Tempo_Gasto = inicioEtapa.HasValue ? StringService.HoraParaString(DateTime.Now.Subtract(inicioEtapa.Value).TotalHours) : null,
                                    Prazo_Atual = prazoAtual.HasValue ? StringService.HoraParaString(prazoAtual.Value) : null,
                                    Data_Sla_Etapa = slaEtapa.HasValue ? slaEtapa.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                                    NumeroTrocaTitularidade = reader["NUMERO_TROCA"] != DBNull.Value ? Convert.ToString(reader["NUMERO_TROCA"]) : "-",
                                    //Responsavel = Convert.ToString(reader["ID_RESPONSAVEL"]),
                                    //DataHoraFechamento = reader["DATAHORAFECHAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORAFECHAMENTO"]).ToString("dd/MM/yyyy HH:mm:ss") : null,
                                    //Motivo_Status = Convert.ToString(reader["ID_MOTIVO_STATUS"]),
                                    //Prioridade_Padrao = Convert.ToString(reader["PRIORIDADE_PADRAO"].ToString()),
                                    Organizacao_Cliente = Convert.ToString(reader["ORGANIZACAO_CLIENTE"]),
                                    Id_Organizacao_Cliente = reader["ID_ORGANIZACAO_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_ORGANIZACAO_CLIENTE"]) : (Int64?)null,
                                    PossuiOs = qtdos > 0 ? true : false

                                };

                                try
                                {
                                    DateTime? DataHora_Criacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                                    DateTime? DataHora_Indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                                    if (DataHora_Indicador.HasValue)
                                    {
                                        Int32 _ordem = 0;
                                        obj.Sla = GetCoresSla(CalculaSla(DataHora_Indicador, DataHora_Criacao, obj.DataHoraFechamento, null), ref _ordem);
                                        obj.SlaOrdem = _ordem;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var teste = ex.Message;
                                }


                                ////Filtro por SLA

                                //if (SlaView.Count() != 0)
                                //{
                                //    foreach (var sla in SlaView)
                                //    {
                                //        if (sla.Key == obj.Sla)
                                //        {

                                //            retorno.Add(obj);
                                //            break;
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    retorno.Add(obj);
                                //}

                                retorno.Add(obj);

                            }
                        }


                    }
                }



            }


            //Page e retornando DataTable
            return new DataTableResponse<AtendimentoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""

            };

        }

        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/atendimentoapi/AcompanhamentoAgente")]
        public async Task<DataTableResponse<AcompanhamentoAgenteListaModel>> AcompanhamentoAgenteGetAll(AcompanhamentoAgente_DataTableRequest request)
        {
            List<AcompanhamentoAgenteListaModel> retorno = new List<AcompanhamentoAgenteListaModel>();
            IOrderedEnumerable<AcompanhamentoAgenteListaModel> listaOrdenada = null;

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_AGENTE"); // WHERE STATUS = 1

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


                    sql.AppendLine(" SELECT A.ID,C.NOME AS AGENTE, D.DESCRICAO AS SETOR, B.NUM_MAX_ATEND_SIMULT_AGENT, B.STATUS ");
                    sql.AppendLine(" FROM ASPNETUSERS A ");
                    sql.AppendLine(" INNER JOIN TB_AGENTE B ");
                    sql.AppendLine(" ON(B.ID = A.ID_PERFIL_AGENTE) ");
                    sql.AppendLine(" INNER JOIN TB_FUNCIONARIO C ");
                    sql.AppendLine(" ON(C.ID = A.ID_FUNCIONARIO) ");
                    sql.AppendLine(" LEFT JOIN TB_DEPARTAMENTO D ");
                    sql.AppendLine(" ON(D.ID = A.ID_DEPARTAMENTO) ");

                    sql.AppendLine(" WHERE B.PERFIL_ATENDE_CHAT = 1 ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {

                        while (await reader.ReadAsync())
                        {
                            //Status Suporte
                            //if (SuporteDisponiveis.FindAll(x => x.idSuporte == request.Agente).Count() <= 0)
                            //{
                            //    suporte.status = StatusAtendimento.DISPONIVEL;
                            //}
                            //else
                            //{
                            //    switch (suporte.status)
                            //    {
                            //        case StatusAtendimento.EM_ATENDIMENTO:
                            //            suporte.status = StatusAtendimento.EM_ATENDIMENTO;
                            //            break;
                            //        case StatusAtendimento.AUSENTE:
                            //            suporte.status = StatusAtendimento.AUSENTE;
                            //            break;
                            //    }

                            //}

                            GetStatusResponse statusResponse = ChatHub.getSuporteStatus(Convert.ToString(reader["ID"]));

                            string id = Convert.ToString(reader["ID"]);
                            string agente = Convert.ToString(reader["AGENTE"]);

                            retorno.Add(new AcompanhamentoAgenteListaModel
                            {
                                Agente = $"{id} - {agente}",
                                Setor = Convert.ToString(reader["SETOR"]),
                                Numero_Max_Atendimento = reader["NUM_MAX_ATEND_SIMULT_AGENT"] != DBNull.Value ? Convert.ToInt64(reader["NUM_MAX_ATEND_SIMULT_AGENT"]) : 1,
                                Status_Chat = HtmlEnumExtensions.GetEnumDescription<StatusAtendimento>(statusResponse.status),
                                Motivo = (statusResponse.status == StatusAtendimento.OCUPADO) || (statusResponse.status == StatusAtendimento.EM_ATENDIMENTO) ? Resources.RsBase.EM_ATENDIMENTO : statusResponse.motivoAusencia,
                                Qtd_Atendimento = statusResponse.qtdAtendimento
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

                    case 1:     // DATA
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status_Chat);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status_Chat);
                            break;
                        }
                    case 2:     // DATA
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Agente);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Agente);
                            break;
                        }
                    case 3:     // DATA
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Motivo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Motivo);
                            break;
                        }
                    case 4:     // DATA
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Qtd_Atendimento);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Qtd_Atendimento);
                            break;
                        }
                    case 5:     // DATA
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Setor);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Setor);
                            break;
                        }

                    case 6:     // DESCRICAO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Numero_Max_Atendimento);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Numero_Max_Atendimento);
                            break;
                        }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<AcompanhamentoAgenteListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }


        public void GetAllFiltros(Atendimento_DataTableRequest request, StringBuilder sqlFrom, Dictionary<String, Object> parametros)
        {
            #region Filtros

            if (request.Id_Modelo_PainelAtendimento == 2)
            {
                sqlFrom.AppendLine(" AND (A.ID_DEPARTAMENTO = @DEPARTAMENTO_USUARIO)");
                parametros.Add("@DEPARTAMENTO_USUARIO", request.Id_DepartamentoUsuario_Logado);
            }
            if (request.Id_Modelo_PainelAtendimento == 3)
            {
                sqlFrom.AppendLine($" AND (A.DATAHORACRIACAO BETWEEN @DATA_INI_MODELO AND @DATA_FIM_MODELO) ");
                parametros.Add("@DATA_INI_MODELO", DateTime.Now.Date.AddDays(-90));
                parametros.Add("@DATA_FIM_MODELO", DateTime.Now);
            }
            if (request.Id_Modelo_PainelAtendimento == 4)
            {
                sqlFrom.AppendLine($" AND (A.ID_RESPONSAVEL = @USUARIO_LOGADO) ");
                parametros.Add("@USUARIO_LOGADO", request.Usuario_Logado);
            }
            if (request.Id_Origem_Atendimento.HasValue)
            {
                sqlFrom.AppendLine($" AND (A.ID_ORIGEM_ATENDIMENTO = @ORIGEM) ");
                parametros.Add("@ORIGEM", request.Id_Origem_Atendimento.Value);
            }
            if (request.StatusAuditoria.HasValue)
            {
                sqlFrom.AppendLine($" AND (A.STATUS_AUDITORIA = @AUDITORIA) ");
                parametros.Add("@AUDITORIA", request.StatusAuditoria.Value);
            }

            if (request.Id_Usuario_Responsavel != "" && request.Id_Usuario_Responsavel != null)
            {
                sqlFrom.AppendLine($" AND (A.ID_RESPONSAVEL = @USUARIO_RESPONSAVEL) ");
                parametros.Add("@USUARIO_RESPONSAVEL", request.Id_Usuario_Responsavel);
            }

            if (request.Id_Cliente_Prospecto.HasValue)
            {
                sqlFrom.AppendLine($" AND (A.ID_CLIENTE = @ID_CLIENTE_PROSPECTO) ");
                parametros.Add("@ID_CLIENTE_PROSPECTO", request.Id_Cliente_Prospecto);
            }

            if (request.id_Plano_Produto.HasValue)
            {
                sqlFrom.AppendLine($" AND (K.ID = @ID_PLANO_PRODUTO) ");
                parametros.Add("@ID_PLANO_PRODUTO", request.id_Plano_Produto);
            }

            if (request.id_Etapa_Atual.HasValue)
            {
                sqlFrom.AppendLine($" AND (M.ID = @ID_ETAPA_ATUAL) ");
                parametros.Add("@ID_ETAPA_ATUAL", request.id_Etapa_Atual);
            }

            if (request.id_Organizacao.HasValue)
            {
                sqlFrom.AppendLine($" AND (O.ID = @ID_ORGANIZACAO) ");
                parametros.Add("@ID_ORGANIZACAO", request.id_Organizacao);
            }

            var filtros_modelo = GetModeloFiltros(request.Id_Modelo_PainelAtendimento, request);

            //Filtrando
            if (!String.IsNullOrEmpty(request.search.value))
            {
                switch (request.PesquisaRapidaAtendimento)
                {
                    case PesquisaRapidaAtendimento.Protocolo:
                        sqlFrom.AppendLine(" AND ((UPPER(A.NUMERO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        break;
                    case PesquisaRapidaAtendimento.Categoria_Atendimento:
                        sqlFrom.AppendLine(" AND ((UPPER(C.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        break;
                    case PesquisaRapidaAtendimento.Tipo_Atendimento:
                        sqlFrom.AppendLine(" AND ((UPPER(D.NOME) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        break;
                    case PesquisaRapidaAtendimento.Cliente:
                        sqlFrom.AppendLine(" AND ((UPPER(B.NOMEFANTASIA) LIKE UPPER(@SEARCH)) OR (UPPER(B.RAZAO_SOCIAL) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        break;
                    case PesquisaRapidaAtendimento.Organizacao:
                        sqlFrom.AppendLine(" AND ((UPPER(O.NOME_FANTASIA) LIKE UPPER(@SEARCH)) OR (UPPER(O.RAZAO_SOCIAL) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        break;
                    case PesquisaRapidaAtendimento.Responsavel:
                        sqlFrom.AppendLine(" AND ((UPPER(F.USERNAME) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        break;
                    case PesquisaRapidaAtendimento.Setor:
                        sqlFrom.AppendLine(" AND ((UPPER(G.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        break;
                    case PesquisaRapidaAtendimento.Assunto:
                        sqlFrom.AppendLine(" AND ((UPPER(A.ASSUNTO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        break;
                    default:
                        break;
                }
            }

            //Prioridade

            var prioridades = filtros_modelo.FindAll(p => p.tipo_filtro == Recurso_PainelAtendimentoEnum.Prioridade_Atendimento).
                                                        Select(p => p.valores.Replace("COLUNA", "A.PRIORIDADE_PADRAO")).
                                                        ToList();

            if (prioridades.Count() > 0 && request?.Lista_PrioridadeAtendimento?.Count > 0)
            {
                var expressao = prioridades[0];
                var matches = Regex.Matches(expressao, @"'(\d)'");

                var valoresExtraidos = matches.Cast<Match>()
                                              .Select(m => m.Groups[1].Value)
                                              .ToList();

                if (valoresExtraidos.Contains("0"))
                {
                    request.Lista_PrioridadeAtendimento.Add(0);
                }

                if (valoresExtraidos.Contains("1"))
                {
                    request.Lista_PrioridadeAtendimento.Add(1);
                }

                if (valoresExtraidos.Contains("2"))
                {
                    request.Lista_PrioridadeAtendimento.Add(2);
                }

                sqlFrom.AppendLine($" AND A.PRIORIDADE_PADRAO IN ({String.Join(",", request.Lista_PrioridadeAtendimento.Distinct())}) ");
            }
            else if (prioridades.Count() > 0)
            {
                sqlFrom.AppendLine($" AND ({String.Join(" AND ", prioridades)}) ");
            }
            else if (request?.Lista_PrioridadeAtendimento?.Count > 0)
            {
                sqlFrom.AppendLine($" AND A.PRIORIDADE_PADRAO IN ({String.Join(",", request.Lista_PrioridadeAtendimento)}) ");
            }

            var SlaView = new Dictionary<String, Boolean>
                           {
                               { "#00FF00", request.Dentro_Prazo },
                               { "#FFFF00", request.Atencao },
                               { "#FF8C00", request.Vencimento_Proximo },
                               { "#FF0000", request.Risco_Perda_Prazo },
                               { "#000000", request.Prazo_Vencido }
                           }
                           .Where(dict => dict.Value == true);




            if (SlaView.Where(p => p.Value).Count() > 0)
            {

                string SLA_INDICADOR = "(1 - ROUND(((a.datahora_indicador - coalesce(a.data_hora_fechamento, current_timestamp)) * 24 * 60) / ((a.datahora_indicador - a.datahoracriacao) * 24 * 60), 2))";

                List<string> filtrosSla = new List<string>();
                if (SlaView.Where(p => p.Key == "#00FF00").Count() > 0)
                {
                    filtrosSla.Add($" ({SLA_INDICADOR}  >= 0 AND {SLA_INDICADOR}  <= 0.25) ");
                }
                if (SlaView.Where(p => p.Key == "#FFFF00").Count() > 0)
                {
                    filtrosSla.Add($" ({SLA_INDICADOR}  > 0.25 AND {SLA_INDICADOR}  <= 0.50) ");
                }
                if (SlaView.Where(p => p.Key == "#FF8C00").Count() > 0)
                {
                    filtrosSla.Add($"  ({SLA_INDICADOR}  > 0.50 AND {SLA_INDICADOR}  <= 0.75) ");
                }
                if (SlaView.Where(p => p.Key == "#FF0000").Count() > 0)
                {
                    filtrosSla.Add($" ({SLA_INDICADOR}  > 0.75 AND {SLA_INDICADOR}  <= 1) ");
                }
                if (SlaView.Where(p => p.Key == "#000000").Count() > 0)
                {
                    filtrosSla.Add($" ({SLA_INDICADOR}  > 1) ");
                }

                sqlFrom.AppendLine($"AND {String.Join(" OR ", filtrosSla)}");

            }




            //Data de Fechamento
            if (request.Tipo_Data != null)
            {
                if (request.Tipo_Data == 0)
                {
                    sqlFrom.AppendLine($" AND (A.DATAHORACRIACAO BETWEEN @DATA_INI AND @DATA_FIM) ");
                    parametros.Add("@DATA_INI", String.Format("{0} 00:00", request.Data_Inicial));
                    parametros.Add("@DATA_FIM", String.Format("{0} 23:59", request.Data_Final));
                }
                else if (request.Tipo_Data == 1)
                {
                    sqlFrom.AppendLine($" AND (A.DATA_HORA_FECHAMENTO BETWEEN @DATA_INI AND @DATA_FIM) ");
                    parametros.Add("@DATA_INI", String.Format("{0} 00:00", request.Data_Inicial));
                    parametros.Add("@DATA_FIM", String.Format("{0} 23:59", request.Data_Final));
                }
                else
                {
                    sqlFrom.AppendLine($" AND (A.DATAINDICADOR_ETAPAFLUXO BETWEEN @DATA_INI AND @DATA_FIM) ");
                    parametros.Add("@DATA_INI", String.Format("{0} 00:00", request.Data_Inicial));
                    parametros.Add("@DATA_FIM", String.Format("{0} 23:59", request.Data_Final));
                }
            }

            // Número da OS
            if (request.Num_Os != null)
            {
                sqlFrom.AppendLine(" AND (A.NUMERO = @NUM_OS ) ");
                parametros.Add("@NUM_OS", request.Num_Os);
            }


            //STATUS
            var lista_status = filtros_modelo.FindAll(p => p.tipo_filtro == Recurso_PainelAtendimentoEnum.Status).
                                                    Select(p => p.valores.Replace("COLUNA", "A.STATUS_ATENDIMENTO")).
                                                    ToList();

            if (lista_status.Count() > 0)
            {
                sqlFrom.AppendLine($" AND ({String.Join(" AND ", lista_status)}) ");

            }


            var origens = filtros_modelo.FindAll(p => p.tipo_filtro == Recurso_PainelAtendimentoEnum.Origem).
                                                    Select(p => p.valores.Replace("COLUNA", "I.ID")).
                                                    ToList();

            if (origens.Count() > 0)
            {
                sqlFrom.AppendLine($" AND ({String.Join(" AND ", origens)}) ");
            }

            // Motivo do Status
            var motivos_status = filtros_modelo.FindAll(p => p.tipo_filtro == Recurso_PainelAtendimentoEnum.Motivo_Status).
                                                    Select(p => p.valores.Replace("COLUNA", "E.ID")).
                                                    ToList();

            if (motivos_status.Count() > 0)
            {
                sqlFrom.AppendLine($" AND ({String.Join(" AND ", motivos_status)}) ");

            }



            // Setor

            var setores = filtros_modelo.FindAll(p => p.tipo_filtro == Recurso_PainelAtendimentoEnum.Setor).
                                                        Select(p => p.valores.Replace("COLUNA", "G.ID")).
                                                        ToList();


            if (setores.Count() > 0)
            {
                sqlFrom.AppendLine($" AND ({String.Join(" AND ", setores)}) ");

            }



            // Categoria de Atendimento 

            var categorias_atendimento = filtros_modelo.FindAll(p => p.tipo_filtro == Recurso_PainelAtendimentoEnum.Categoria_Atendimento).
                                                        Select(p => p.valores.Replace("COLUNA", "C.ID")).
                                                        ToList();



            if (categorias_atendimento.Count() > 0)
            {
                sqlFrom.AppendLine($" AND ({String.Join(" AND ", categorias_atendimento)}) ");
            }

            // Categoria do Cliente

            var categorias_cliente = filtros_modelo.FindAll(p => p.tipo_filtro == Recurso_PainelAtendimentoEnum.Categoria_Cliente).
                                                        Select(p => p.valores.Replace("COLUNA", "H.ID")).
                                                        ToList();

            if (categorias_cliente.Count > 0)
            {
                sqlFrom.AppendLine($" AND ({String.Join(" AND ", categorias_cliente)}) ");
            }

            //Tipo Atendimento

            var tipos_atendimento = filtros_modelo.FindAll(p => p.tipo_filtro == Recurso_PainelAtendimentoEnum.Tipo_Atendimento).
                                                        Select(p => p.valores.Replace("COLUNA", "D.ID")).
                                                        ToList();


            if (tipos_atendimento.Count > 0)
            {
                sqlFrom.AppendLine($" AND ({String.Join(" AND ", tipos_atendimento)}) ");
            }


            #endregion
        }

        public List<(Recurso_PainelAtendimentoEnum tipo_filtro, String valores)> GetModeloFiltros(Int64? Id_Modelo_PainelAtendimento, Atendimento_DataTableRequest request)
        {
            List<(Recurso_PainelAtendimentoEnum tipo_filtro, String valores)> retorno = new List<(Recurso_PainelAtendimentoEnum tipo_filtro, String valores)>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_PainelAtendimento>(session))
            {
                Modelo_PainelAtendimento modelo = repository.selectFirst(p => p.Id == Id_Modelo_PainelAtendimento).Result;

                if (modelo != null)
                {
                    var filtros = modelo.Filtros.Select(p => p.Recurso).Distinct();

                    retorno.AddRange(filtros.Select(p =>
                    {
                        return (p, "(" + String.Join(" OR ", modelo.Filtros.Where(y => y.Recurso == p).Select(y => $"COLUNA = '{y.Valor}'").ToList()) + ")");
                    }));
                }

            }

            foreach (var prop in request.GetType().GetProperties())
            {
                var attr = prop.GetCustomAttribute<Atendimento_ModeloAttribute>();

                if (attr != null)
                {
                    if (prop.GetValue(request) != null)
                    {
                        if (prop.GetType() == typeof(string))
                        {
                            retorno.Add((attr.Recurso, $"(COLUNA = '{prop.GetValue(request)}')"));
                        }
                        else
                        {
                            retorno.Add((attr.Recurso, $"(COLUNA = '{Convert.ToInt64(prop.GetValue(request))}')"));
                        }


                    }
                }
            }


            return retorno;
        }



        private Double CalculaSla(DateTime? DataHora_Indicador, DateTime? DataHora_Criacao, String DataHoraFechamento, DateTime? Criacao_acao)
        {

            var totais = (DataHora_Indicador - DataHora_Criacao).Value.TotalMinutes;
            DateTime data_comparar;
            //var agora = (DataHora_Indicador - new DateTime(2019, 7, 10, 15, 0, 0) /*DateTime.Now*/).Value.TotalMinutes;
            var agora = new Double();
            if (String.IsNullOrEmpty(DataHoraFechamento))
            {
                data_comparar = Criacao_acao ?? DateTime.Now;
                agora = (DataHora_Indicador - data_comparar).Value.TotalMinutes;
            }

            else
            {
                data_comparar = Criacao_acao ?? Convert.ToDateTime(DataHoraFechamento);
                agora = (DataHora_Indicador - data_comparar).Value.TotalMinutes;
            }

            //var porcentagem = ((totais * 100) / agora) - 100;

            var porcentagem = ((100 - (agora / totais) * 100));
            var valor_calculado = Math.Round(porcentagem, 2);

            return valor_calculado;

        }


        private async Task<DateTime?> GetDataPromessa(ISession session, Int64? Id_Cliente, Int64? Id_Tipo_Atendimento, Int64? Id_Cliente_Plano_Produto, DateTime DataHora_Criacao, Prioridade_Atendimento? Prioridade)
        {
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoSLA_Atendimento = new Conexao<SLA_Atendimento>(session))

                try
                {
                    SLA_Atendimento _sla = null;
                    List<SLA_Atendimento> _slaPossiveis = new List<SLA_Atendimento>();
                    var cliente = await repoCliente.selectFirst(p => p.Id == Id_Cliente);

                    var ClientePlanoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == Id_Cliente_Plano_Produto);
                    long? idcategoria = cliente?.Categoria_Cliente?.Id;

                    //_slaPossiveis = cliente.Categoria_Cliente != null ? (await repoSLA_Atendimento.select(p => p.Status == Status_Default.Ativo && p.Tipo_Atendimento.Id == Id_Tipo_Atendimento &&
                    //                                                                      (p.Categoria_Cliente.Id == cliente.Categoria_Cliente.Id))).ToList() :
                    //                                                                      (await repoSLA_Atendimento.select(p => p.Status == Status_Default.Ativo && p.Tipo_Atendimento.Id == Id_Tipo_Atendimento &&
                    //                                                                      (p.Categoria_Cliente == null))).ToList();

                    //consulta se o status é ativo, tipo de atendimento for igual e se a categoria cliente for igual
                    _slaPossiveis = (await repoSLA_Atendimento.select(p => p.Status == Status_Default.Ativo && p.Tipo_Atendimento.Id == Id_Tipo_Atendimento && p.Prioridade_Atendimento == Prioridade &&
                                                                                          (p.Categoria_Cliente.Id == idcategoria))).ToList();

                    //consulta se o status é ativo, tipo de atendimento for igual e se a categoria cliente for indiferente
                    _slaPossiveis.AddRange((await repoSLA_Atendimento.select(p => p.Status == Status_Default.Ativo && p.Tipo_Atendimento.Id == Id_Tipo_Atendimento && p.Prioridade_Atendimento == Prioridade.Value &&
                                                                                             (p.Categoria_Cliente.Id == null))).ToList());

                    //consulta o sla pelo plano e produto 
                    foreach (var item in _slaPossiveis)
                    {

                        if ((ClientePlanoProduto == null) || item.Sla_PlanoProduto == Status_Default.Inativo)
                        {
                            _sla = _slaPossiveis.Find(p => p.Sla_PlanoProduto == Status_Default.Inativo);
                        }
                        else
                        {

                            if (ClientePlanoProduto?.Plano != null)
                            {
                                _sla = _slaPossiveis.Find(p => p.PlanosProdutos.Select(y => y.Plano.Id).ToList().Contains(ClientePlanoProduto.Plano.Id));
                            }
                            else
                            {
                                _sla = _slaPossiveis.Find(p => p.PlanosProdutos.Select(y => y.Produto.Id).ToList().Contains(ClientePlanoProduto.Produto.Id));
                            }

                        }

                        if (_sla != null)
                        {
                            break;
                        }
                    }
                    ;



                    Int32? Minutos = _sla?.Minutos.Value;

                    if (Minutos == null)
                    {
                        return DataHora_Criacao.AddDays(1);
                    }

                    Horario_Connect Horario = null;

                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (var repoHorario_Connect = new Conexao<Horario_Connect>(sessionT5))
                    {
                        Horario = await repoHorario_Connect.selectFirst(p => p.Id == _sla.Id_Horario);

                        if (Horario == null)
                            Horario = await repoHorario_Connect.selectFirst(p => p.Padrao == true && p.Status == Status_Default.Ativo && p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico);
                    }

                    if (_sla.Contagem_Prazo == Contagem_Prazo.Horas_Corridas)
                    {
                        return DataHora_Criacao.AddMinutes(Minutos.Value);
                    }

                    Int32 vMinutosRestantes = Minutos.Value;
                    DateTime vData_Corrente = DataHora_Criacao.Date;
                    TimeSpan vHora_Corrente = DataHora_Criacao.TimeOfDay;
                    TimeSpan? vHora_Inicial = DataHora_Criacao.TimeOfDay;

                    DateTime? vInicio = null;
                    DateTime? vTermino = null;
                    DateTime? vInicioIntervalo = null;
                    DateTime? vTerminoIntervalo = null;

                    while (vMinutosRestantes != 0)
                    {
                        DayOfWeek vDia_Da_Semana = vData_Corrente.DayOfWeek;


                        switch (vDia_Da_Semana)
                        {
                            case DayOfWeek.Sunday:
                                vInicio = Horario.Inicio_Domingo;
                                vTermino = Horario.Termino_Domingo;
                                vInicioIntervalo = Horario.Inicio_Intervalo_Domingo;
                                vTerminoIntervalo = Horario.Termino_Intervalo_Domingo;
                                break;
                            case DayOfWeek.Monday:
                                vInicio = Horario.Inicio_Segunda;
                                vTermino = Horario.Termino_Segunda;
                                vInicioIntervalo = Horario.Inicio_Intervalo_Segunda;
                                vTerminoIntervalo = Horario.Termino_Intervalo_Segunda;
                                break;
                            case DayOfWeek.Tuesday:
                                vInicio = Horario.Inicio_Terca;
                                vTermino = Horario.Termino_Terca;
                                vInicioIntervalo = Horario.Inicio_Intervalo_Terca;
                                vTerminoIntervalo = Horario.Termino_Intervalo_Terca;
                                break;
                            case DayOfWeek.Wednesday:
                                vInicio = Horario.Inicio_Quarta;
                                vTermino = Horario.Termino_Quarta;
                                vInicioIntervalo = Horario.Inicio_Intervalo_Quarta;
                                vTerminoIntervalo = Horario.Termino_Intervalo_Quarta;
                                break;
                            case DayOfWeek.Thursday:
                                vInicio = Horario.Inicio_Quinta;
                                vTermino = Horario.Termino_Quinta;
                                vInicioIntervalo = Horario.Inicio_Intervalo_Quinta;
                                vTerminoIntervalo = Horario.Termino_Intervalo_Quinta;
                                break;
                            case DayOfWeek.Friday:
                                vInicio = Horario.Inicio_Sexta;
                                vTermino = Horario.Termino_Sexta;
                                vInicioIntervalo = Horario.Inicio_Intervalo_Sexta;
                                vTerminoIntervalo = Horario.Termino_Intervalo_Sexta;
                                break;
                            case DayOfWeek.Saturday:
                                vInicio = Horario.Inicio_Sabado;
                                vTermino = Horario.Termino_Sabado;
                                vInicioIntervalo = Horario.Inicio_Intervalo_Sabado;
                                vTerminoIntervalo = Horario.Termino_Intervalo_Sabado;
                                break;
                        }

                        if (Horario.Opcao_Trabalha_Feriado)
                        {
                            IList<Feriado> feriados = await new Conexao<Feriado>(session).select(p => p.Data == vData_Corrente);
                            var planoprod = await new Conexao<Cliente_PlanoProduto>(session).selectFirst(p => p.Id == Id_Cliente_Plano_Produto);


                            foreach (var feriado in feriados)
                            {
                                if ((feriado.Abrangencia == Abrangencia.Estadual && planoprod?.Endereco_Instalacao?.UF == feriado.UF) ||
                                    (feriado.Abrangencia == Abrangencia.Municipal && planoprod?.Endereco_Instalacao?.UF == feriado.UF
                                                                                  && planoprod?.Endereco_Instalacao?.Cidade == feriado.Cidade) ||
                                    (feriado.Abrangencia == Abrangencia.Internacional || feriado.Abrangencia == Abrangencia.Internacional))
                                {
                                    vInicio = Horario.Inicio_Feriado;
                                    vTermino = Horario.Termino_Feriado;
                                    vInicioIntervalo = Horario.Inicio_Intervalo_Feriado;
                                    vTerminoIntervalo = Horario.Termino_Intervalo_Feriado;
                                    break;
                                }
                            }
                        }

                        if (!vInicio.HasValue)
                        {
                            return DataHora_Criacao.AddDays(1);
                        }
                        else
                        {
                            if (!vHora_Inicial.HasValue)
                                vHora_Corrente = vInicio.Value.TimeOfDay;
                            else
                            {
                                vHora_Corrente = vHora_Inicial.Value;
                                vHora_Inicial = null;
                            }

                            Int32 vTmpMinutos = Convert.ToInt32((vTermino.Value.TimeOfDay - vInicio.Value.TimeOfDay).TotalMinutes);
                            Int32 vTmpMinutos2 = Convert.ToInt32((vTermino.Value.TimeOfDay - vHora_Corrente).TotalMinutes);

                            if (vTmpMinutos2 < vMinutosRestantes)
                            {
                                if ((vMinutosRestantes - vTmpMinutos2) < 0)
                                {
                                    return vData_Corrente.Add(vHora_Corrente).AddMinutes(vMinutosRestantes);
                                }
                                else
                                    vMinutosRestantes -= vTmpMinutos2;
                                vData_Corrente = vData_Corrente.AddDays(1);
                            }
                            else if (vTmpMinutos <= vTmpMinutos2)
                            {
                                if ((vMinutosRestantes - vTmpMinutos2) <= 0)
                                {
                                    return vData_Corrente.Add(vHora_Corrente).AddMinutes(vMinutosRestantes);
                                }
                                else
                                    vMinutosRestantes -= vTmpMinutos2;
                                vData_Corrente = vData_Corrente.AddDays(1);
                            }
                            else
                            {
                                return vData_Corrente.Add(vHora_Corrente).AddMinutes(vMinutosRestantes);
                            }
                        }
                    }

                    return DataHora_Criacao.AddMinutes(CONSTANTES.SLA_PADRAO_ATENDIMENTO);
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    return DataHora_Criacao.AddDays(1);
                }
        }

        public String GetCoresSla(Double Porcentagem, ref Int32 Ordem)
        {
            if (Porcentagem >= 0 && Porcentagem <= 25)
            {
                Ordem = 1;
                return "#00FF00";
            }
            else if (Porcentagem > 25 && Porcentagem <= 50)
            {
                Ordem = 2;
                return "#FFFF00";
            }
            else if (Porcentagem > 50 && Porcentagem <= 75)
            {
                Ordem = 3;
                return "#FF8C00";
            }
            else if (Porcentagem > 75 && Porcentagem <= 100)
            {
                Ordem = 4;
                return "#FF0000";
            }
            else if (Porcentagem > 100)
            {
                Ordem = 5;
                return "#000000";
            }
            else
            {
                Ordem = 6;
                return "#FF0000";
            }
        }

        private async Task<AtendimentoCadastroModel> GetViewObj(Atendimento obj, ISession session)
        {
            using (var repoONU = new Conexao<ONU>(session))
            using (var repoPOP = new Conexao<POP>(session))
            using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (var repoFluxo = new Conexao<FluxoAtendimento>(session))
            using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
            using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
            using (var repositoryFluxoAtendimento = new Conexao<FluxoAtendimento>(session))
            using (var repositoryConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repositoryContato = new Conexao<Cliente_Contato>(session))
            {
                var retorno = new AtendimentoCadastroModel();

                var lista = obj.PlanoProduto != null ? await repoClientePlano.select(p => p.Id == obj.PlanoProduto.Id) : new List<Cliente_PlanoProduto>();

                var listaRetorno = new List<AtendimentoPlanoContratoModel>();
                var empresa = (await repositoryContato.selectFirst(p => p.Telefone == obj.Cliente.Telefone_Celular || p.Telefone == obj.Cliente.Telefone_Fixo))?.Cliente;

                foreach (var objeto in lista)
                {
                    if (objeto.ClienteContrato != null)
                    {
                        var contrato = await repoClienteContrato.selectFirst(p => p.Id == objeto.ClienteContrato.Id);

                        //Contrato
                        retorno.Status_Contrato = contrato.Status_Contrato;
                        retorno.Status_ContratoStr = contrato.Status_Contrato != null ? HtmlEnumExtensions.GetEnumDescription<Status_Contrato>(contrato.Status_Contrato.ToString()) : HtmlEnumExtensions.GetEnumDescription<Status_Contrato>(Status_Contrato.Sem_Status);
                    }
                    DescontoApiController desconto = new DescontoApiController();
                    ClientePlanoDescontoValores clientePlanoDesconto = new ClientePlanoDescontoValores();

                    if (!String.IsNullOrEmpty(obj.Contrato))
                    {
                        clientePlanoDesconto = await desconto.DescontoByContrato(obj.Contrato);
                        retorno.Desconto = Convert.ToDouble(clientePlanoDesconto.TotalDesconto).ToString("N2");
                        retorno.ValorSubTotal = Convert.ToDouble(clientePlanoDesconto.ValorSubTotal).ToString("N2");
                        retorno.ValorAPagarSemInstalacao = Convert.ToDouble(clientePlanoDesconto.ValorAPagarSemInstalacao).ToString("N2");
                    }

                    AtendimentoPlanoContratoModel tmp = new AtendimentoPlanoContratoModel
                    {
                        id = obj.Id,
                        Nome_Plano_Produto = obj.PlanoProduto?.Plano?.Descricao,
                        Status_PlanoProdutoStr = obj.PlanoProduto != null ? HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(obj.PlanoProduto.Status.ToString()) : HtmlEnumExtensions.GetEnumDescription<Status_Contrato>(Status_Contrato.Sem_Status),
                        //Status_PlanoProduto = obj.PlanoProduto?.Status
                        Numero_Contrato = obj.PlanoProduto?.ClienteContrato?.NumeroContrato,
                        Data_Aceite = Convert.ToString(string.Format("{0:dd/MM/yyyy}", obj.PlanoProduto?.Data_Habilitado)),
                        // Valor_Final_Plano = String.Format("{0:N2}", obj.PlanoProduto?.Valor_Final),
                        Forma_Pagamento_Utilizada = obj.PlanoProduto?.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao,
                        Banco = obj.PlanoProduto?.Forma_Pagamento_Cliente?.Banco?.ToString(),
                        Dia_Vencimento = obj.PlanoProduto?.Forma_Pagamento_Cliente?.Dia_Vencimento,
                        Forma_Envio = obj.PlanoProduto?.Forma_Pagamento_Cliente?.Forma_Envio?.ToString(),
                        Dias_Teste_Gratis = obj.PlanoProduto?.Dias_Teste_Gratis != null ? obj.PlanoProduto?.Dias_Teste_Gratis?.ToString() : "0",
                        Periodo_Carencia = obj.PlanoProduto?.Periodo_Carencia == true ? "SIM" : "NÃO",
                        Meses_Carencia = obj.PlanoProduto?.Meses_Carencia,
                        Multa_Rescisao = obj.PlanoProduto?.Multa_Rescisao,
                    };

                    listaRetorno.Add(tmp);
                }

                ONU onu = obj.PlanoProduto != null ? repoONU.selectFirst(x => x.Cliente_PlanoProduto.Id == obj.PlanoProduto.Id).Result : null;
                Model.nhibernate.LoginRadius radius = obj.PlanoProduto != null ? repoLoginRadius.selectFirst(w => w.Cliente_PlanoProduto.Id == obj.PlanoProduto.Id).Result : null;

                //ClienteContrato clienteContrato = obj.PlanoProduto != null ? repositoryClienteContrato.selectFirst(z => z.Id == obj.PlanoProduto.ClienteContrato.Id).Result : null;

                String _endereco = String.Empty;
                String _endereco_principal = String.Empty;
                String _endereco_instalacao = String.Empty;

                if (obj.PlanoProduto?.Endereco_Instalacao?.Logradouro != null)
                    _endereco = $"{obj.PlanoProduto?.Endereco_Instalacao?.Logradouro} {" , "} {obj.PlanoProduto?.Endereco_Instalacao?.Numero} {" - "} {obj.PlanoProduto?.Endereco_Instalacao?.Bairro} {" - "} {obj.PlanoProduto?.Endereco_Instalacao?.Cidade}";
                if (obj.Cliente?.Endereco_Principal?.Logradouro != null)
                    _endereco_principal = $"{obj.Cliente?.Endereco_Principal.Logradouro} {" , "} {obj.Cliente?.Endereco_Principal.Numero} {" - "} {obj.Cliente?.Endereco_Principal.Bairro} {" - "} {obj.Cliente?.Endereco_Principal.Cidade}";

                var config = await repositoryConfiguracoes_Gerais.selectFirst();
                retorno.CriarOsSemPlanoProduto = config.CriarOsSemPlanoProduto;

                //Cliente/Prospecto - Dados Básicos
                retorno.EtapaAtual = obj.EtapaFluxo?.Descricao;
                retorno.EtapaAtualDecisoria = obj.EtapaFluxo?.Etapa_Decisoria ?? false;
                retorno.ProximaEtapa = obj.EtapaFluxo?.Proxima_Etapa;
                retorno.ProximaEtapaDesc = String.IsNullOrEmpty(obj.EtapaFluxo?.Proxima_Etapa) == false ? repoFluxo.selectFirst(p => p.Status == Status_Default.Ativo && p.Tipo_Atendimento == obj.Tipo_Atendimento && p.Sequencia == Convert.ToInt64(obj.EtapaFluxo.Proxima_Etapa)).Result?.Descricao : null;
                retorno.IdProximaEtapa = String.IsNullOrEmpty(obj.EtapaFluxo?.Proxima_Etapa) == false ? repoFluxo.selectFirst(p => p.Status == Status_Default.Ativo && p.Tipo_Atendimento == obj.Tipo_Atendimento && p.Sequencia == Convert.ToInt64(obj.EtapaFluxo.Proxima_Etapa)).Result?.Id : null;
                retorno.Formato_Protocolo_Atendimento = (Int64?)config?.Formato_Protocolo_Atendimento;
                retorno.Prazo_Etapa_Atual = obj.EtapaFluxo != null ? StringService.HoraParaString(obj.EtapaFluxo.Tempo_Exc) : null;
                retorno.Tempo_Gasto_Etapa_Atual = obj.DataInicio_EtapaFluxo.HasValue ? StringService.HoraParaString(DateTime.Now.Subtract(obj.DataInicio_EtapaFluxo.Value).TotalHours) : null;
                retorno.Sla_Etapa_Atual = obj.DataIndicador_EtapaFluxo.HasValue ? obj.DataIndicador_EtapaFluxo.Value.ToString("dd/MM/yyyy HH:mm") : null;
                retorno.Sequencia = obj.EtapaFluxo?.Sequencia;
                if (obj.Status_Atendimento != Status_Atendimento.A_Iniciar &&
                    obj.Status_Atendimento != Status_Atendimento.Em_Atendimento &&
                    obj.Status_Atendimento != Status_Atendimento.Aguardando &&
                    obj.Status_Atendimento != Status_Atendimento.Cancelado)
                {
                    retorno.StatusAuditoria = obj?.StatusAuditoria;
                }

                retorno.Permite_OS_LEAD = config.Permite_OS_LEAD;

                retorno.Id = obj.Id;
                retorno.TipoPreenchimentoAtendimento = obj.TipoPreenchimento;
                retorno.Id_Cliente_Contrato = obj.Contrato;
                retorno.CPF_CNPJ_Cliente = obj.Cliente?.CpfCnpj;
                retorno.RG_Cliente = obj.Cliente?.Rg;
                retorno.Nome_Cliente = obj.Cliente?.NomeFantasia;
                retorno.Data_Nascimento = string.Format("{0:dd/MM/yyyy}", obj?.Cliente?.Data_Nascimento);
                retorno.Categoria_Cliente = obj.Cliente?.Categoria_Cliente?.Descricao;
                retorno.Id_Cliente = obj.Cliente?.Id;
                //retorno.Id_Organizacao_Cliente = obj.Cliente?.Organizacao?.Id;
                //retorno.Organizacao_Cliente = obj.Cliente?.Organizacao?.Nome_Fantasia ?? "Sem Organização";
                retorno.Id_Organizacao_Cliente = obj?.OrganizacaoCliente?.Id;
                retorno.Organizacao_Cliente = obj?.OrganizacaoCliente?.Nome_Fantasia;
                retorno.Telefone_Fixo = obj.Cliente?.Telefone_Fixo;
                retorno.Telefone_Celular = obj.Cliente?.Telefone_Celular;
                retorno.Telefone_Recado = obj.Cliente?.Telefone_Recado;
                retorno.Nome_Recado = obj.Cliente?.Pessoa_Tel_Recado;
                retorno.Email_Principal = obj.Cliente?.Email_Principal;

                if (obj?.Cliente?.Data_Criacao_Cliente != null)
                {
                    retorno.Cliente_Ha = (DateTime.Now.Date - obj?.Cliente?.Data_Criacao_Cliente.Value.Date).Value.Days.ToString() + "Dias";
                }
                else if (obj?.Cliente?.DataHoraCriacao != null)
                {
                    retorno.Cliente_Ha = (DateTime.Now.Date - obj?.Cliente?.DataHoraCriacao.Value.Date).Value.Days.ToString() + "Dias";
                }
                else
                {
                    retorno.Cliente_Ha = "0 Dias";
                }

                //Cliente/Prospecto - Endereço de Instalação

                if (obj.PlanoProduto?.Endereco_Instalacao != null)
                {
                    var enderecoIntalacao = obj.PlanoProduto?.Endereco_Instalacao;
                    retorno.CepInstalacao = enderecoIntalacao.Cep;
                    retorno.LogradouroInstalacao = enderecoIntalacao.Logradouro;
                    retorno.NumeroInstalacao = enderecoIntalacao.Numero;
                    retorno.UFInstalacao = enderecoIntalacao.UF;
                    retorno.CidadeInstalacao = enderecoIntalacao.Cidade;
                    retorno.BairroInstalacao = enderecoIntalacao.Bairro;
                    retorno.TipoImovelInstalacao = enderecoIntalacao.Tipo_Imovel?.Descricao;
                    retorno.PaisInstalacao = enderecoIntalacao.Pais?.Nome;
                    retorno.ComplementoInstalacao = enderecoIntalacao.Complemento;
                    retorno.PontoReferenciaInstalacao = enderecoIntalacao.Ponto_Referencia;
                    retorno.CepInstalacao = enderecoIntalacao.Cep;
                }

                retorno.Endereco_Instalacao = _endereco_instalacao ?? "";

                //Plano / Produto - Informações do Plano / Produto
                var contratoPlano = obj?.PlanoProduto?.ClienteContrato?.Id != null ? new Conexao<Cliente_PlanoProduto>(session).select(p => p.ClienteContrato.Id == obj.PlanoProduto.ClienteContrato.Id).Result : new List<Cliente_PlanoProduto>();

                for (var i = 0; i < contratoPlano.Count; i++)
                {
                    retorno.Id_Plano_Produto = obj.PlanoProduto?.Plano?.Id == null ? obj.PlanoProduto?.Plano?.Id : obj.PlanoProduto?.Produto?.Id;
                    retorno.Nome_Plano_Produto = obj.PlanoProduto?.Plano?.Descricao != null ? obj.PlanoProduto?.Plano?.Descricao : obj.PlanoProduto?.Produto?.Descricao;
                    retorno.Id_Cliente_Plano_Produto = obj.PlanoProduto?.Id;
                    retorno.Id_Cliente_Contrato = obj.Contrato;
                    retorno.Status = obj.Status == Status_Default.Ativo ? true : false; // OBJ.STATUS,
                                                                                        // retorno.Numero_Contrato = obj.ClienteContrato?.NumeroContrato ?? "-";

                    // retorno.Numero_Contrato = clienteContrato?.NumeroContrato ?? "-";
                    retorno.Numero_Contrato = obj.PlanoProduto?.Id != null ? repoClienteContrato.selectFirst(p => p.Id == obj.PlanoProduto.ClienteContrato.Id).Result?.Descricao : null;

                    retorno.Valor_Final_Plano = String.Format("{0:N2}", obj.PlanoProduto?.Valor_Final);
                    retorno.Valor_Instalacao = obj.PlanoProduto.Valor_Instalacao_Final.HasValue ? String.Format("{0:N2}", obj.PlanoProduto.Valor_Instalacao_Final) : "0,00";
                    retorno.Forma_Pagamento_Utilizada = obj.PlanoProduto?.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao;
                    retorno.Banco = obj.PlanoProduto?.Forma_Pagamento_Cliente?.Banco?.ToString();
                    retorno.Dia_Vencimento = obj.PlanoProduto?.Forma_Pagamento_Cliente?.Dia_Vencimento;
                    retorno.Data_Aceite = Convert.ToString(string.Format("{0:dd/MM/yyyy}", obj.PlanoProduto?.Data_Habilitado));
                    retorno.Forma_Envio = obj.PlanoProduto?.Forma_Pagamento_Cliente?.Forma_Envio?.ToString();
                    retorno.Dias_Teste_Gratis = obj.PlanoProduto?.Dias_Teste_Gratis != null ? obj.PlanoProduto?.Dias_Teste_Gratis?.ToString() : "0";
                    retorno.Periodo_Carencia = obj.PlanoProduto?.Periodo_Carencia == true ? "SIM" : "NÃO";
                    retorno.Meses_Carencia = obj.PlanoProduto?.Meses_Carencia;
                    retorno.Multa_Rescisao = obj.PlanoProduto?.Multa_Rescisao;
                }

                //Plano / Produto - Informações Técnicas
                retorno.Id_Login_Radius = radius?.Id;
                retorno.Login_Radius = radius?.Login ?? obj.PlanoProduto?.Login_Radius;
                retorno.Senha_Radius = radius?.SenhaPPPOE ?? obj.PlanoProduto?.Senha_Radius;
                retorno.ONU = onu?.Serial;
                retorno.Id_ONU = onu?.Id;
                retorno.PON = onu?.PON?.Descricao;
                retorno.Id_PON = onu?.PON?.Id;
                retorno.OLT = onu?.OLT?.Descricao;
                retorno.Id_OLT = onu?.OLT?.Id;
                retorno.Slot = onu?.Slot?.Descricao;
                retorno.POP = onu?.OLT?.POP?.Nome;
                retorno.Caixa_FTTH = Convert.ToString(radius?.Id_Caixa_FTTH);
                retorno.Porta_FTTH = Convert.ToString(radius?.Id_Porta_FTTH);
                retorno.Velocidade_Download = obj.PlanoProduto?.Plano?.VelocidadeDownload;
                retorno.Velocidade_Upload = obj.PlanoProduto?.Plano?.VelocidadeUpload;
                retorno.Velocidade_Nominal = obj.PlanoProduto?.Plano?.VelocidadeNominal;
                retorno.Velocidade_Download_Reduzida = obj.PlanoProduto?.Plano?.VelocidadeDownloadReduzida;
                retorno.Velocidade_Download_Reduzida_Txt = obj.PlanoProduto?.Plano?.VelocidadeDownloadReduzida != null ? obj.PlanoProduto?.Plano?.VelocidadeDownloadReduzida : String.Empty;
                retorno.Velocidade_Upload_Reduzida = obj.PlanoProduto?.Plano?.VelocidadeUploadReduzida;
                retorno.Observacoes = radius?.Observacoes ?? "Sem observações";
                retorno.Velocidade_Reduzida = obj.PlanoProduto?.Velocidade_Reduzida ?? false;
                retorno.Pool_Cliente = obj?.PlanoProduto?.Pool_Ip?.Descricao;
                retorno.Ip_Cliente = radius?.Pool_Ip_Endereco?.Ip;
                //Outros
                retorno.Id_Endereco = obj.PlanoProduto?.Endereco_Instalacao?.Id;
                retorno.Assunto = obj.Assunto;
                retorno.Protocolo = obj.Numero;
                retorno.Id_Categoria_Atendimento = obj.Categoria_Atendimento?.Id;
                retorno.Id_Email_Abertura = obj.Id_Email_Abertura ?? 0;
                retorno.Data_Cadastro_Cliente = obj.Cliente?.DataHoraCriacao?.ToString("dd/MM/yyyy");
                retorno.Id_Origem_Atendimento = obj.Origem_Atendimento?.Id;
                retorno.Id_Responsavel = obj.Responsavel?.Id;
                retorno.Id_Setor = obj.Setor?.Id;
                retorno.Id_Tipo_Atendimento = obj.Tipo_Atendimento?.Id;
                retorno.Numero = obj?.Numero?.ToString();
                retorno.IdPessoaContato = obj.IdPessoaContato;
                retorno.Pessoa_Contato = obj.Pessoa_Contato;
                retorno.ContatosCopia = obj.ContatosCopia;
                retorno.Grau_Ligacao = !String.IsNullOrEmpty(obj.Grau_Ligacao) ? (Grau_Ligacao)Enum.Parse(typeof(Grau_Ligacao), obj.Grau_Ligacao) : (Grau_Ligacao?)null;
                retorno.Status_Atendimento = obj.Status_Atendimento;
                retorno.Tipo_Cliente = obj.Cliente?.Tipo_Cliente == Tipo_Cliente.Cliente ? (int)Tipo_Cliente.Cliente : (int)Tipo_Cliente.Prospecto;
                retorno.Id_Motivo_Status = Convert.ToString(obj.Motivo_Status?.Id);
                retorno.Desc_Motivo_Status = Convert.ToString(obj.Motivo_Status?.Descricao);
                retorno.Prioridade = obj.Prioridade;
                retorno.Problema_Relatado = obj.Problema_Relatado ?? "";
                retorno.Tipo_Pessoa_Contato = obj.Id != 0 ? obj.Tipo_Pessoa_Contato : Tipo_Pessoa_Contato.Outros;
                retorno.Data_Hora_Criacao = obj.DataHoraCriacao.HasValue ? (DateTime.Now.Date.Month - obj.DataHoraCriacao.Value.Date.Month).ToString() + " Meses" : "";
                retorno.Data_Hora_Abertura = obj.Data_Hora_Abertura.HasValue ? Convert.ToString(obj.Data_Hora_Abertura) : null;
                retorno.Data_Hora_Fechamento = obj.Data_Hora_Fechamento.HasValue ? Convert.ToString(obj.Data_Hora_Fechamento) : null;
                retorno.Criar_Os_Encerrar = obj.Tipo_Atendimento?.Criar_Os_Encerrar ?? false;
                retorno.Status_PlanoProduto = obj.PlanoProduto?.Status;
                retorno.Status_PlanoProdutoStr = obj.PlanoProduto != null ? HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(obj.PlanoProduto.Status.ToString()) : String.Empty;
                retorno.ScriptAtendimento = obj?.Tipo_Atendimento?.Script != null;
                retorno.PlanoTelefonia = obj?.PlanoProduto?.Plano?.Tipo_Tecnologia == Tipo_Tecnologia.Telefonia_Fixo;
                //new Conexao<Tipo_Atendimento>(session).selectFirst(p => p.Id == obj.Tipo_Atendimento?.Id).Result
                retorno.Id_TrocaTitularidade = obj?.TrocaTitularidade != null ? obj.TrocaTitularidade.Id : (long?)null;
                if (obj.Tipo_Atendimento?.Id != null)
                {
                    var etapasFluxo = await repositoryFluxoAtendimento.select(p => p.Tipo_Atendimento.Id == obj.Tipo_Atendimento.Id);
                    retorno.Possui_Etapas_Tipo_Atendimento = etapasFluxo.Any();
                }
                else
                {
                    retorno.Possui_Etapas_Tipo_Atendimento = false;
                }


                if (obj.DataHora_Indicador.HasValue)
                {
                    var ordem = 0;
                    retorno.Sla = GetCoresSla(CalculaSla(obj.DataHora_Indicador, obj.DataHoraCriacao, obj.Data_Hora_Fechamento.HasValue ? obj.Data_Hora_Fechamento.Value.ToString("dd/MM/yyyy HH:mm") : null, null), ref ordem);
                    retorno.DataHoraIndicador = obj.DataHora_Indicador.HasValue ? obj.DataHora_Indicador.Value.ToString("dd/MM/yyyy HH:mm:ss") : null;

                }

                using (var repository_cliente = new Conexao<Cliente>(session))
                {
                    if (retorno.Id_Cliente.HasValue)
                    {
                        Cliente cliente = await repository_cliente.selectFirst(p => p.Id == retorno.Id_Cliente);
                        if (cliente != null)
                        {
                            retorno.Cliente_Status = cliente.Status;
                        }
                    }
                }

                return retorno;
            }
        }
        private async void BindViewToObj(ISession session, Atendimento objBanco, AtendimentoCadastroModel view, String Usuario)
        {
            using (var repoOrigem = new Conexao<Origem_Atendimento>(session))
            using (var repoCategoria = new Conexao<Categoria_Atendimento>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
            using (var repoIdentityUser = new Conexao<IdentityUser>(session))
            using (var repoDepartamento = new Conexao<Departamento>(session))
            using (var repoTipo_Atendimento = new Conexao<Tipo_Atendimento>(session))
            using (var repoMotivo_Status_Atendimento = new Conexao<Motivo_Status_Atendimento>(session))
            using (var repoTrocaTitulariade = new Conexao<TrocaTitularidade>(session))
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            using (var repoOrganizacao = new Conexao<Organizacao>(session))
            {
                objBanco.Id = view.Id;
                objBanco.Assunto = view.Assunto;
                objBanco.Protocolo = view.Protocolo;

                if (objBanco.Tipo_Atendimento?.Id != view.Id_Cliente)
                {
                    objBanco.DataHora_Indicador = await GetDataPromessa(session, view.Id_Cliente, view.Id_Tipo_Atendimento, view.Id_Cliente_Plano_Produto, objBanco.DataHoraCriacao ?? DateTime.Now, view.Prioridade);
                }

                objBanco.Origem_Atendimento = view.Id_Origem_Atendimento.HasValue ? repoOrigem.selectFirst(p => p.Id == view.Id_Origem_Atendimento.Value).Result : null;
                objBanco.Categoria_Atendimento = view.Id_Categoria_Atendimento.HasValue ? repoCategoria.selectFirst(p => p.Id == view.Id_Categoria_Atendimento.Value).Result : null;
                objBanco.Cliente = view.Id_Cliente.HasValue ? repoCliente.selectFirst(p => p.Id == view.Id_Cliente.Value).Result : null;
                objBanco.TrocaTitularidade = view.Id_TrocaTitularidade.HasValue ? await repoTrocaTitulariade.selectFirst(x => x.Id == view.Id_TrocaTitularidade.Value) : null;
                objBanco.TipoPreenchimento = view.TipoPreenchimentoAtendimento;
                objBanco.OrganizacaoCliente = view.Id_Organizacao_Cliente.HasValue ? await repoOrganizacao.selectFirst(x => x.Id == view.Id_Organizacao_Cliente.Value) : null;
                if (objBanco.OrganizacaoCliente != null)
                {
                    if (objBanco.Cliente != null)
                    {
                        objBanco.Cliente.Organizacao = objBanco.OrganizacaoCliente;
                    }
                }

                if (objBanco.TipoPreenchimento == TipoPreenchimentoAtendimento.PlanoProduto)
                {
                    objBanco.PlanoProduto = view.Id_Cliente_Plano_Produto.HasValue ? repoClientePlanoProduto.selectFirst(p => p.Id == view.Id_Cliente_Plano_Produto.Value).Result : null;
                    objBanco.Contrato = Convert.ToString(objBanco.PlanoProduto?.ClienteContrato?.Id);
                }
                else
                {
                    objBanco.Contrato = view.Id_Cliente_Contrato;
                    objBanco.ClienteContrato = !String.IsNullOrEmpty(view.Id_Cliente_Contrato) ? repoClienteContrato.selectFirst(p => p.Id == Convert.ToInt64(view.Id_Cliente_Contrato)).Result : null;
                    objBanco.PlanoProduto = view.Id_Cliente_Plano_Produto.HasValue ? repoClientePlanoProduto.selectFirst(p => p.Id == view.Id_Cliente_Plano_Produto.Value).Result : null;
                }



                if (view.NovoResponsavelAceite)
                {
                    var user = await repoIdentityUser.selectFirst(x => x.Id == Usuario);

                    if (user == null)
                        objBanco.Responsavel = null;
                    else
                        objBanco.Responsavel = user;

                    if (user.Departamento != null)
                    {
                        objBanco.Setor = user.Departamento;
                    }
                    else
                    {
                        objBanco.Setor = view.Id_Setor.HasValue ? repoDepartamento.selectFirst(p => p.Id == view.Id_Setor.Value).Result : null;
                    }
                }
                else
                {
                    objBanco.Responsavel = !String.IsNullOrEmpty(view.Id_Responsavel) ? repoIdentityUser.selectFirst(p => p.Id == view.Id_Responsavel).Result : null;
                    objBanco.Setor = view.Id_Setor.HasValue ? repoDepartamento.selectFirst(p => p.Id == view.Id_Setor.Value).Result : null;
                }
                objBanco.Tipo_Atendimento = view.Id_Tipo_Atendimento.HasValue ? repoTipo_Atendimento.selectFirst(p => p.Id == view.Id_Tipo_Atendimento.Value).Result : null;
                objBanco.Tipo_Pessoa_Contato = view.Tipo_Pessoa_Contato;
                objBanco.Motivo_Status = !String.IsNullOrEmpty(view.Id_Motivo_Status) ? repoMotivo_Status_Atendimento.selectFirst(p => p.Id == Convert.ToInt64(view.Id_Motivo_Status)).Result : null;
                //objBanco.Pessoa_Contato = view.Pessoa_Contato;
                if (objBanco.Tipo_Pessoa_Contato == Tipo_Pessoa_Contato.Titular)
                {
                    objBanco.Pessoa_Contato = objBanco.Cliente?.NomeFantasia;
                }
                else
                {
                    objBanco.Pessoa_Contato = view.Pessoa_Contato;
                    objBanco.IdPessoaContato = view.IdPessoaContato;
                    objBanco.Grau_Ligacao = HtmlEnumExtensions.GetEnumDescription<Grau_Ligacao>(view.Grau_Ligacao.ToString());
                }
                objBanco.ContatosCopia = view.ContatosCopia;
                objBanco.Prioridade = view.Prioridade;
                objBanco.Problema_Relatado = view.Problema_Relatado;


                objBanco.Status_Atendimento = view.Status_Atendimento;
                if (view.Status_Atendimento == Status_Atendimento.Concluido)
                {
                    objBanco.Data_Hora_Fechamento = objBanco.Data_Hora_Fechamento == null ? DateTime.Now : objBanco.Data_Hora_Fechamento;

                    var pesquisa_disparo_auto = new Conexao<Pesquisa_Satisfacao>(session).select(p => p.DisparoAuto == PesquisaSatisfacaoDisparoAuto.Envio_Automatico_Atendimento).Result;
                    if (pesquisa_disparo_auto.Count != 0 && objBanco.Cliente != null)
                    {
                        await new Pesquisa_SatisfacaoApiController().EnviarEmMassa(new Pesquisa_Satisfacao_PesquisaEmMassaModel
                        {
                            ids = new List<long> { objBanco.Cliente.Id },
                            ids_pesquisa = pesquisa_disparo_auto.Select(p => p.Id).ToList(),
                            usuario = User.Identity.GetUserId(),
                            idAtendimento = objBanco.Id
                        });
                    }
                }


                if (objBanco.Id == 0)
                {
                    objBanco.Numero = view.Numero;
                }
                else
                {
                    //objBanco.Status = view.Status == true ? Status_Default.Ativo : Status_Default.Inativo;
                }
            }
        }


        [HttpPost()]
        [Route("api/atendimentoapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusAtendimentoViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder();

                        if (value.status == 1)
                        {
                            sql.AppendLine($"UPDATE TB_ATENDIMENTO SET STATUS_ATENDIMENTO =  {value.status} , DATA_HORA_ABERTURA = '{DateTime.Now.ToString().Replace("/", ".")}' WHERE ID IN {_ids}");
                        }
                        else if (value.status == 2)
                        {
                            sql.AppendLine($"UPDATE TB_ATENDIMENTO SET STATUS_ATENDIMENTO =  {value.status} , ID_MOTIVO_STATUS = {value.motivo_status} ,DATA_HORA_FECHAMENTO = '{DateTime.Now.ToString().Replace("/", ".")}' WHERE ID IN {_ids}");
                        }
                        else if (value.status == 3)
                        {
                            sql.AppendLine($"UPDATE TB_ATENDIMENTO SET STATUS_ATENDIMENTO =  {value.status} , ID_MOTIVO_STATUS = {value.motivo_status} ,DATA_HORA_FECHAMENTO = '{DateTime.Now.ToString().Replace("/", ".")}' WHERE ID IN {_ids}");
                        }
                        else if (value.status == 4)
                        {
                            sql.AppendLine($"UPDATE TB_ATENDIMENTO SET STATUS_ATENDIMENTO =  {value.status} , ID_MOTIVO_STATUS = {value.motivo_status} ,DATA_HORA_FECHAMENTO = '{DateTime.Now.ToString().Replace("/", ".")}', STATUS_AUDITORIA={StatusAuditoria.AguardandoAvaliacao}  WHERE ID IN {_ids}");
                        }
                        else if (value.status == 5)
                        {
                            sql.AppendLine($"UPDATE TB_ATENDIMENTO SET STATUS_ATENDIMENTO =  {value.status} , ID_MOTIVO_STATUS = {value.motivo_status} ,DATA_HORA_FECHAMENTO = '{DateTime.Now.ToString().Replace("/", ".")}' WHERE ID IN {_ids}");
                        }

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }

                value.ids.ForEach(p => GerarEmailMudancaStatus(p, (Status_Atendimento)value.status).Wait());

            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/atendimentoapi/CancelarAtendimento")]
        public async Task<PostApiRetorno<Int64>> CancelarAtendimento(Int32 id_atendimento, Int32 id_motivo_status)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Atendimento>(session))
                {


                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        //String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder();

                        if (id_atendimento != 0)
                        {
                            //sql.AppendLine($"UPDATE TB_ATENDIMENTO SET STATUS_ATENDIMENTO =  3 , ID_MOTIVO_STATUS = " + id_motivo_status + " , DATA_HORA_FECHAMENTO = '{DateTime.Now.ToString().Replace("/", ".")}' WHERE ID =" + id_atendimento + "");
                            sql.AppendLine($"UPDATE TB_ATENDIMENTO SET STATUS_ATENDIMENTO =  3 , ID_MOTIVO_STATUS = {id_motivo_status} , DATA_HORA_FECHAMENTO = '{DateTime.Now.ToString().Replace("/", ".")}' WHERE ID =" + id_atendimento + "");

                        }

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);


                        var atendimento = (await repository.selectFirst(p => p.Id == id_atendimento));
                        if (atendimento.Responsavel.Id != User.Identity.GetUserId())
                        {
                            using (var api = new NotificacoesApiController())
                            {
                                var notificacao = api.EnviaNotificacoesAtendimento(7, atendimento, null);
                            }
                        }

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
        [Route("api/atendimentoapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_ATENDIMENTO WHERE ID IN {_ids}");

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
        [Route("api/atendimentoapi/VerificaAtendimentoChatMongo")]
        public async Task<GetApiRetorno<List<AtendimentoChatMongoProtocolo>>> VerificaAtendimentoChatMongo(ExcluirListaViewModel value)
        {
            GetApiRetorno<List<AtendimentoChatMongoProtocolo>> retorno = new GetApiRetorno<List<AtendimentoChatMongoProtocolo>>()
            {
                Objeto = new List<AtendimentoChatMongoProtocolo>
                {
                    new AtendimentoChatMongoProtocolo { }
                }
            };

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                {
                    foreach (var item in value.ids)
                    {
                        var atendimento = await repositoryAtendimento.selectFirst(x => x.Id == item);
                        if (atendimento != null)
                        {
                            var atendimento_mongo = await repository.Find(x => x.protocolo == atendimento.Protocolo);
                            if (atendimento_mongo != null)
                            {
                                if (String.IsNullOrEmpty(atendimento_mongo.dataEncerramento))
                                {
                                    retorno.Objeto = new List<AtendimentoChatMongoProtocolo>()
                                    {
                                        new AtendimentoChatMongoProtocolo
                                        {
                                            id = item,
                                            protocolo = atendimento_mongo.protocolo,
                                            em_atendimento = true
                                        }
                                    };
                                }
                            }
                        }
                    }
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/atendimentoApi/FindById")]
        public async Task<GetApiRetorno<AtendimentoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<AtendimentoCadastroModel> retorno = new GetApiRetorno<AtendimentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new Conexao<Atendimento>(session))
            using (var repositoryLog = new Conexao<LogAtendimento>(sessionLog))
            using (var repositoryAtendimentoChat = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {
                try
                {
                    var atendimento = await repository.selectFirst(p => p.Id == Id);
                    if (atendimento == null)
                        atendimento = await repository.selectFirst(p => p.Protocolo == Convert.ToString(Id));

                    retorno.Objeto = await GetViewObj(atendimento, session);

                    var atendimento_chat = await repositoryAtendimentoChat.Find(x => x.protocolo == retorno.Objeto.Protocolo);
                    retorno.Objeto.DataEncerramento_Chat = atendimento_chat?.dataEncerramento;
                    retorno.Objeto.Chat_Via_WhatsApp = !String.IsNullOrEmpty(atendimento_chat?.whatsAppChatId);
                    retorno.Objeto.Atendimento_Reaberto = !String.IsNullOrEmpty(retorno.Objeto.Data_Hora_Abertura) && String.IsNullOrEmpty(retorno.Objeto.Data_Hora_Fechamento);
                    retorno.Objeto.UsuarioConvidado = atendimento_chat?.usuariosConvidados?.Where(x => x.usuarioAceite == User.Identity.GetUserId()).Count() > 0;

                    var retornoTmp = await repositoryLog.select(p => p.IdLog == Id && p.Campo == RsCadastros.ETAPA_ATUAL);
                    retornoTmp = retornoTmp.Where(p => !string.IsNullOrEmpty(p.Campo_ValorAntes) && !string.IsNullOrEmpty(p.Campo_ValorDepois)).ToList();
                    retorno.Objeto.PossuiAlteracaoEtapa = retornoTmp.FirstOrDefault() != null ? true : false;

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

        [HttpPost(), HttpGet()]
        [Route("api/atendimentoApi/BuscaDados")]
        public async Task<GetApiRetorno<AtendimentoCadastroModel>> BuscaDados(String protocolo)
        {
            GetApiRetorno<AtendimentoCadastroModel> retorno = new GetApiRetorno<AtendimentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Atendimento>(session))
            {
                try
                {
                    retorno.Objeto = await GetViewObj((await repository.selectFirst(p => p.Numero == protocolo)), session);
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

        [HttpGet()]
        [Route("api/atendimentoApi/AtualizarResponsavel_Setor")]
        public async Task<GetApiRetorno<AtendimentoCadastroModel>> AtualizarResponsavel_Setor(String protocolo)
        {
            GetApiRetorno<AtendimentoCadastroModel> retorno = new GetApiRetorno<AtendimentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Atendimento>(session))
            using (var repoIdentityUser = new Conexao<IdentityUser>(session))
            using (var conexaoMongo = new ConexaoMongo())
            using (var repositoryMongo = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            using (var repositoryConfiguracoesGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repositoryCliente = new Conexao<Cliente>(session))
            {
                try
                {
                    var atendimento = await repository.selectFirst(p => p.Numero == protocolo);



                    if (atendimento != null)
                    {
                        var usuario = await repoIdentityUser.selectFirst(p => p.Id == User.Identity.GetUserId());
                        atendimento.Responsavel = usuario;
                        atendimento.Setor = usuario?.Departamento;

                        if (atendimento.Cliente == null)
                        {
                            var configuracao = await repositoryConfiguracoesGerais.selectFirst();

                            int tipo_cliente = Convert.ToInt32(configuracao.Pessoa_Contato_Primeira_Vez);
                            AtendimentoChatMongo chat = await repositoryMongo.Find(p => p.protocolo == protocolo);
                            var cliente = await GetClienteChatBot(session, chat.cliente.telefone, chat.cliente.identificadorTelegram);
                            if (cliente == null)
                            {
                                cliente = new Cliente
                                {
                                    NomeFantasia = chat.cliente.nome,
                                    Telefone_Celular = chat.cliente.telefone,
                                    CpfCnpj = chat.cliente.cpf_cnpj,
                                    Email_Principal = chat.cliente.email,
                                    Tipo_Cliente = tipo_cliente == 0 ? Tipo_Cliente.Prospecto : Tipo_Cliente.Cliente,
                                    Tipo_Pessoa = chat.cliente.cpf_cnpj != null ? (chat.cliente.cpf_cnpj.Length <= 11 ? Tipo_Pessoa.Fisica : Tipo_Pessoa.Juridica) : (Tipo_Pessoa?)null,
                                    Status = Status_Cliente.Ativo,
                                    Status_Prospecto = Status_Prospecto.Lead_Qualificado,
                                    Identificador_Telegram = chat.cliente.identificadorTelegram
                                };
                            }

                            await repositoryCliente.SaveOrUpdate(cliente);
                            atendimento.Cliente = cliente;
                        }

                        await repository.SaveOrUpdate(atendimento);
                    }

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


        public async Task<long?> FindIdByProtocolo(String Protocolo)
        {
            long? retorno = null;

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Atendimento>(session))
            {
                retorno = (await repository.selectFirst(p => p.Numero == Protocolo))?.Id;

            }
            return retorno;
        }




        #region mobile
        [AllowAnonymous]
        [OlaAuthorize]
        [HttpGet()]
        [Route("api/AtendimentoApi/GetListAtendimento")]
        public async Task<GetApiRetorno<List<AtendimentoOla>>> GetListAtendimento(String Id)
        {
            GetApiRetorno<List<AtendimentoOla>> retorno = new GetApiRetorno<List<AtendimentoOla>>();
            using (var session = HibernateUtil.GetSession())
            using (var sessionT5 = HibernateUtil.GetSessionT5())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                try
                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        //Filtros 
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();

                        //Select 
                        var sql = new StringBuilder();

                        sql.AppendLine(" SELECT  ");
                        sql.AppendLine("    OBSERVACAO_AUX, STATUS_EXECUCAO, DATA_MAXIMA, NUM_OBRA, ");
                        sql.AppendLine("    HORA_MAXIMA, EQUIPE, ID_CLIENTE, LATITUDE_STR, LONGITUDE_STR, DIA, HORA ");
                        sql.AppendLine(" FROM  ");
                        sql.AppendLine("    TB_DOCUMENTO_PRODUCAO ");
                        sql.AppendLine(" WHERE  ");
                        sql.AppendLine("    ID_CLIENTE = @ID_CLIENTE ");

                        if (!Exibe_RDOs)
                        {
                            sql.Append(" AND COALESCE(SIGLA_MODULO, '') <> 'MCO' ");
                        }

                        parametros.Add("@ID_CLIENTE", Id);

                        var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null);
                        List<AtendimentoOla> tmpRetorno = new List<AtendimentoOla>();
                        while (await reader.ReadAsync())
                        {
                            tmpRetorno.Add(new AtendimentoOla
                            {
                                Observacao = reader["OBSERVACAO_AUX"].ToString(),
                                Num_Obra = reader["NUM_OBRA"].ToString(),
                                StatusAtendimento = reader["STATUS_EXECUCAO"] != DBNull.Value ? (StatusExecucaoAniel)Enum.Parse(typeof(StatusExecucaoAniel), Convert.ToString(reader["STATUS_EXECUCAO"])) : (StatusExecucaoAniel?)null,
                                DataMaxima = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]).ToString() : (String)null,
                                HoraMaxima = reader["HORA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(((TimeSpan)(reader["HORA_MAXIMA"])).ToString(@"hh\:mm")).ToString() : (String)null,
                                IdCliente = Convert.ToInt64(reader["ID_CLIENTE"]),
                                Equipe = reader["EQUIPE"]?.ToString(),
                                Latitude = reader["LATITUDE_STR"] != DBNull.Value ? Convert.ToString(reader["LATITUDE_STR"]) : null,
                                Longitude = reader["LONGITUDE_STR"] != DBNull.Value ? Convert.ToString(reader["LONGITUDE_STR"]) : null,
                                DiaCriacao = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null,
                                HoraCriacao = reader["HORA"] != DBNull.Value ? Convert.ToDateTime(((TimeSpan)(reader["HORA"])).ToString(@"hh\:mm")) : (DateTime?)null
                            });
                        }

                        retorno.Objeto = tmpRetorno;
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }

            }
            return retorno;
        }

        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/Atendimentoapi/Ordens_ServicoOla")]
        public async Task<List<OrdensServicoResponseMobile>> Ordens_ServicoOla(OrdensServicoRequest request)
        {

            List<OrdensServicoResponseMobile> retorno = new List<OrdensServicoResponseMobile>();

            try
            {
                Documento_ProducaoAniel modelo = new Documento_ProducaoAniel();

                using (var documentoProducaoApi = new Documento_ProducaoApiController())
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var apiDocProd = new Documento_ProducaoApiController())
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoEquipe = new Conexao<EquipeAniel>(sessionT5))
                using (var repoTurno = new Conexao<Turno_ServicoAniel>(sessionT5))
                {
                    using (ITransaction transaction = sessionT5.BeginTransaction())
                    {
                        using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                        using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session))
                        {
                            IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                            IClienteRepository repoCliente = new ClienteRepository(session);
                            var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                            var clienteBusiness = new ClienteBusiness(repoCliente);


                            Cliente _cliente = clienteBusiness.GetById(request.Id_Cliente);

                            List<Documento_ProducaoAniel> _docs;

                            Configuracoes_Gerais config = await repositoryConfig.selectFirst();

                            if (request.Status != null)
                            {
                                _docs = documentoBusiness.GetServicosHistoricoByCpf(_cliente.CpfCnpj, request.Status);
                            }
                            else
                            {
                                _docs = documentoBusiness.GetServicosHistoricoByCpf(_cliente.CpfCnpj);
                            }

                            if (String.IsNullOrEmpty(request.DataInicial) == false && String.IsNullOrEmpty(request.DataFinal) == false)
                            {
                                _docs = _docs.FindAll(p => p.Dia >= Convert.ToDateTime(request.DataInicial) && p.Dia <= Convert.ToDateTime(request.DataFinal)).ToList();
                            }

                            foreach (var item in _docs)
                            {
                                DateTime? dataAgendamento = item.Data_Maxima;
                                DateTime? dataCriacao = item.Dia;
                                DateTime? dataEncerramento = item.Data_Prod;
                                DateTime? dataIndicador = item.Data_Indicador;

                                TimeSpan? horaAgendamento = null;
                                TimeSpan? horaCriacao = null;
                                TimeSpan? horaEncerramento = null;
                                TimeSpan? horaIndicador = null;

                                if (item.Hora_Maxima.HasValue && dataAgendamento.HasValue)
                                {
                                    horaAgendamento = item.Hora_Maxima.Value.TimeOfDay;
                                    dataAgendamento = dataAgendamento.Value.Add(horaAgendamento.Value);
                                }
                                if (item.Hora.HasValue && dataCriacao.HasValue)
                                {
                                    horaCriacao = item.Hora.Value.TimeOfDay;
                                    dataCriacao = dataCriacao.Value.Add(horaCriacao.Value);
                                }
                                if (item.Hora_Prod.HasValue && dataEncerramento.HasValue)
                                {
                                    horaEncerramento = item.Hora_Prod.Value.TimeOfDay;
                                    dataEncerramento = dataEncerramento.Value.Add(horaEncerramento.Value);
                                }
                                if (item.Hora_Indicador.HasValue && dataIndicador.HasValue)
                                {
                                    horaIndicador = item.Hora_Indicador.Value.TimeOfDay;
                                    dataIndicador = dataIndicador.Value.Add(horaIndicador.Value);
                                }

                                Cliente_PlanoProduto cliente_PlanoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == item.Id_Cliente_PlanoProdutoConnect);

                                EquipeAniel equipe = await repoEquipe.selectFirst(d => d.Matricula == item.Equipe);

                                Turno_ServicoAniel turno = await repoTurno.selectFirst(d => d.Id == item.CodTurnoServ);

                                var itemRetorno = new OrdensServicoResponseMobile
                                {

                                    Codct = item.CodCt,
                                    Projeto = item.Projeto,
                                    Num_Doc = Convert.ToString(item.Num_Doc),
                                    Numero_Obra_Original = item.Numero_Obra_Original,
                                    dataAgendamento = dataAgendamento.HasValue ? dataAgendamento.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                    dataHoraCriacao = dataCriacao.HasValue ? dataCriacao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                    dataHoraEncerramento = dataEncerramento.HasValue ? dataEncerramento.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                    numObra = item.Num_Obra,
                                    sla = apiDocProd.GetDescricaoSla(dataIndicador, dataCriacao, dataEncerramento),
                                    statusEnum = item.Status.HasValue ? (int)item.Status : (int?)null,
                                    status = item.Status.HasValue ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(item.Status)) : null,
                                    tecnico = Convert.ToString(item.Equipe),
                                    tipoServico = item.TipoServico != null ? Convert.ToString(item.TipoServico.Descricao) : String.Empty,
                                    codTipoServico = item.TipoServico != null ? item.TipoServico.Cod_Tipo_Serv : (short?)null,
                                    cidade = item.Cidade,
                                    UF = item.UF,
                                    logradouro = item.Logradouro,
                                    bairro = item.Bairro,
                                    numero = item.Numero,
                                    latitude = item.Latitude,
                                    longitude = item.Longitude,
                                    idClientePlanoProduto = item.Id_Cliente_PlanoProdutoConnect,
                                    idAtendimento = item.Id_Atendimento,
                                    cod_turno_serv = item.CodTurnoServ,
                                    nomeTecnico = equipe?.Nome,
                                    descricaoPlano = cliente_PlanoProduto?.Plano?.Descricao,
                                    idPlanoProduto = cliente_PlanoProduto?.Plano?.Id,
                                    descricaoCodTurno = turno?.Descricao,
                                    turno_por_periodo = config.Considerar_Agendamento_Periodo
                                };

                                retorno.Add(itemRetorno);
                            }

                            retorno = retorno.OrderByDescending(x => DateTime.Parse(x.dataHoraCriacao)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception(String.Format(Resources.RsBase.ERROAORETORNARDADOSDOSERVIDOR));
            }

            return retorno;
        }



        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/Atendimentoapi/GetDadosTecnicoOla")]
        public async Task<DadosTecnicoResponse> GetDadosTecnicoOla(DadosTecnicoResponse value)
        {
            DadosTecnicoResponse retorno = new DadosTecnicoResponse();
            try
            {
                retorno.nomeTecnico = await new EquipeApiController().GetNome(value.equipe);

                var fotoBytes = await new T5.FuncionarioAnielApiController().GetFotoFuncionario(value.equipe);
                if (fotoBytes != null)
                {
                    try
                    {
                        string fotoBase64 = null;
                        var bitmap = ImageService.BytesToImage(fotoBytes);

                        if (bitmap.Width > 500 || bitmap.Height > 500)
                        {
                            var bitmap_miniatura = ImageService.ResizeImage(bitmap, new System.Drawing.Size(200, 200));
                            fotoBase64 = ImageService.ImageToBase64(bitmap_miniatura, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            fotoBase64 = ImageService.ImageToBase64(bitmap, System.Drawing.Imaging.ImageFormat.Png);
                        }

                        retorno.fotoTecnico64 = fotoBase64;
                    }
                    catch (Exception ex)
                    {
                        CONSTANTES.log4Net.Error(ex);
                    }
                }

                retorno.equipe = value.equipe;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception(String.Format(Resources.RsBase.ERROAORETORNARDADOSDOSERVIDOR));
            }

            return retorno;
        }
        #endregion
        [HttpGet()]
        [Route("api/AtendimentoApi/FindById_Historico")]
        public async Task<GetApiRetorno<AtendimentoHistoricoCadastroModel>> FindById_Historico(Int64 Id)
        {
            GetApiRetorno<AtendimentoHistoricoCadastroModel> retorno = new GetApiRetorno<AtendimentoHistoricoCadastroModel>();
            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj_Historico(await repoAtendimento.selectFirst(p => p.Id == Id));
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
        [Route("api/AtendimentoApi/CreateUUID")]
        public string CreateUUID()
        {
            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();
            string uuid = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            return uuid;
        }


        [HttpGet()]
        [Route("api/AtendimentoApi/QtdeAtendimentos_Titular")]
        public async Task<GetApiRetorno<List<AcompanhamentoAtendimentoModel>>> QtdeAtendimentos_Titular(Int64 Id_Cliente, Int64 Id_Cliente_Contrato, Int64 Id_Cliente_Plano_Produto, String Protocolo)
        {
            GetApiRetorno<List<AcompanhamentoAtendimentoModel>> retorno = new GetApiRetorno<List<AcompanhamentoAtendimentoModel>>()
            {
                Objeto = new List<AcompanhamentoAtendimentoModel>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            {
                try
                {
                    var protocolo_atendimento = await repoAtendimento.selectFirst(p => p.Protocolo == Protocolo && p.Status_Atendimento != Status_Atendimento.Concluido && p.Status_Atendimento != Status_Atendimento.Cancelado);
                    if (protocolo_atendimento != null)
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = "Atendimento carregado!";
                    }
                    else
                    {
                        var lista = await repoAtendimento.select(p => p.Cliente.Id == Id_Cliente && p.ClienteContrato.Id == Id_Cliente_Contrato && p.PlanoProduto.Id == Id_Cliente_Plano_Produto && p.Status_Atendimento != Status_Atendimento.Concluido && p.Status_Atendimento != Status_Atendimento.Cancelado);
                        foreach (var item in lista)
                        {
                            retorno.Objeto.Add(new AcompanhamentoAtendimentoModel
                            {
                                protocolo = item.Protocolo
                            });
                        }
                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpGet()]
        //[AcceptVerbs("Get", "Post")]
        [Route("api/AtendimentoApi/GerarProtocolo")]
        public async Task<PostApiRetorno<String>> GerarProtocolo(Int64? Id_Cliente)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Configuracoes_Gerais config = await new Conexao<Configuracoes_Gerais>(session).selectFirst();

                    //using (var repository = new Conexao<Atendimento>(session))
                    //{

                    //    modelo.Numero = (await repository.Max(p => p.Numero)) + 1;
                    //    modelo.Status_Atendimento = Status_Atendimento.A_Iniciar;
                    //    modelo.Cliente = Id_Cliente.HasValue ? (await new Conexao<Cliente>(session).selectFirst(p => p.Id == Id_Cliente.Value)) : null;
                    //    modelo.UsuarioCriacao = Id_Usuario;
                    //    modelo.Responsavel = await new Conexao<IdentityUser>(session).selectFirst(p => p.Id == Id_Usuario);

                    //    await repository.SaveOrUpdate(modelo);
                    //}

                    if (config.Formato_Protocolo_Atendimento == Formato_Protocolo_Atendimento.Ano_Mes_Dia_Hora_Sequencial)
                        retorno.IdObjeto = String.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHH"), (await new Conexao<Atendimento>(session).GetSequence("GN_ATENDIMENTO")));
                    else
                        retorno.IdObjeto = $"{await new Conexao<Atendimento>(session).GetSequence("GN_ATENDIMENTO")}";

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/AtendimentoApi/ChatAtivo")]
        public async Task<bool> ChatAtivo(String protocolo)
        {
            var chatAtivo = true;
            using (var conexaoMongo = new ConexaoMongo())
            using (var repositoryChat = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {
                AtendimentoChatMongo atendimento = await repositoryChat.Find(p => p.protocolo == protocolo);

                if (atendimento.dataEncerramento == null)
                {
                    chatAtivo = true;
                }
                else
                {
                    chatAtivo = false;
                }
            }

            return chatAtivo;
        }


        [HttpPost()]
        [Route("api/AtendimentoApi/Gravar")]
        public async Task<PostApiRetornoAtendimento> Gravar([FromBody] ParamWebApi<AtendimentoCadastroModel> value)
        {
            PostApiRetornoAtendimento retorno = new PostApiRetornoAtendimento();

            try
            {
                Atendimento modelo = null;
                Atendimento modeloAnterior = null;
                //Boolean temNovaAcao = false;
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repository_emailAtendimento = new Conexao<Email_Atendimento>(session))
                using (var repositoryCPP = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoVinculoCampos = new Conexao<VinculoAtendimentoCampos>(session))
                using (var repoAtendimentoCampos = new Conexao<AtendimentoCampos>(session))
                using (var repoAtendimentoCamposRespostas = new Conexao<AtendimentoCamposRespostas>(session))
                {
                    var conf = await new Conexao<Configuracoes_Gerais>(session).selectFirst();
                    using (var repository = new Conexao<Atendimento>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        var errosValidacaoCamposObrigatorios = new AtendimentoController().ValidacoesAtendimento(value.Objeto);
                        if (errosValidacaoCamposObrigatorios.Count > 0)
                        {

                            retorno.Mensagens = new List<PostApiMensagem>();
                            foreach (var item in errosValidacaoCamposObrigatorios)
                            {
                                retorno.Mensagens.Add(new PostApiMensagem
                                {
                                    Mensagem = item.Value,
                                    Tipo = PostApiMensagemType.Info
                                });
                            }
                            throw new Exception("errosValidacaoCamposObrigatorios");
                        }

                        if (modelo == null)
                        {
                            modelo = new Atendimento
                            {
                                Numero = value.Objeto.Numero,
                                Protocolo = value.Objeto.Protocolo,
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo,
                                StatusAuditoria = StatusAuditoria.AguardandoAvaliacao,
                                Data_Hora_Abertura = DateTime.Now,
                                DataHora_Indicador = await GetDataPromessa(session, value.Objeto.Id_Cliente, value.Objeto.Id_Tipo_Atendimento, value.Objeto.Id_Cliente_Plano_Produto, DateTime.Now, value.Objeto.Prioridade)
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            modeloAnterior = (Atendimento)modelo.Clone();
                            if (modelo.Status_Atendimento != value.Objeto.Status_Atendimento)
                            {
                                await GerarEmailMudancaStatus(modelo.Id, value.Objeto.Status_Atendimento);
                            }
                        }

                        //if(value.Objeto.Status_Atendimento == Status_Atendimento.Concluido)
                        //{
                        //    await FinalizarAtendimento(value.Objeto.Protocolo, value.Usuario, Convert.ToInt32(value.Objeto.Id_Motivo_Status));
                        //}


                        //if (value.Objeto.Status_Atendimento == Status_Atendimento.Cancelado)
                        //{
                        //    var os = await new Conexao<Documento_ProducaoAniel>(sessionT5).
                        //             selectFirst(p => p.Id_Atendimento.Value == value.Objeto.Id
                        //             && p.Status != StatusExecucaoAniel.Cancelado);

                        //    if (os != null)
                        //        throw new Exception("Alteração de status não permitida. Existe uma ordem de serviço em aberto");
                        //}

                        BindViewToObj(session, modelo, value.Objeto, value.Usuario);

                        var chatAtivo = ChatHub.ChatActive(modelo.Protocolo);
                        using (var repositoryLogs = new Conexao<LogAtendimento>(sessionLog))
                        {
                            var retornoTmp = await repositoryLogs.select(p => p.IdLog == modelo.Id && p.Campo == RsCadastros.ETAPA_ATUAL);
                            retornoTmp = retornoTmp.Where(p => !string.IsNullOrEmpty(p.Campo_ValorAntes) && !string.IsNullOrEmpty(p.Campo_ValorDepois)).ToList();
                            var possuiAlteracaoEtapa = retornoTmp.FirstOrDefault() != null ? true : false;

                            if (modelo?.Tipo_Atendimento?.Id != modeloAnterior?.Tipo_Atendimento?.Id)
                            {
                                if (!possuiAlteracaoEtapa)
                                {
                                    using (var repository_fluxoAtendimento = new Conexao<FluxoAtendimento>(session))
                                    {

                                        var etapasAtendimento = await repository_fluxoAtendimento.select(p => p.Tipo_Atendimento == modelo.Tipo_Atendimento && p.Status == Status_Default.Ativo);

                                        if (etapasAtendimento.Count() > 0)
                                        {
                                            modelo.EtapaFluxo = etapasAtendimento.Where(y => y.Sequencia == etapasAtendimento.Min(p => p.Sequencia)).FirstOrDefault();
                                            var tempoEtapa = TimeSpan.FromHours(modelo.EtapaFluxo.Tempo_Exc);
                                            modelo.DataIndicador_EtapaFluxo = DateTime.Now.Add(tempoEtapa);
                                            modelo.DataInicio_EtapaFluxo = DateTime.Now;
                                        }
                                        else
                                        {
                                            modelo.EtapaFluxo = null;
                                            modelo.DataIndicador_EtapaFluxo = null;
                                            modelo.DataInicio_EtapaFluxo = null;
                                        }
                                        retorno.EtapaAtualizada = true;
                                    }
                                }
                            }
                        }

                        retorno.Concluido_Cancelado = false;
                        if (modelo.Status_Atendimento == Status_Atendimento.Concluido)
                        {
                            if (modelo?.Tipo_Atendimento?.Criar_Os_Encerrar != null && modelo?.Tipo_Atendimento?.Criar_Os_Encerrar == true)
                            {
                                var doc_producao = new Conexao<Documento_ProducaoAniel>(sessionT5).selectFirst(m => m.Id_Atendimento == modelo.Id).Result;
                                if (doc_producao != null)
                                {
                                    var codigo_encerramento = new Conexao<Codigo_EncerramentoAniel>(sessionT5).selectFirst(p => p.Cod_Encerramento == doc_producao.Cod_Encerramento && p.CodCt == doc_producao.CodCt).Result;
                                    Documento_ProducaoApiController doc_prod_api = new Documento_ProducaoApiController();

                                    var tem_improdutivo = doc_prod_api.BuscarPorAtendimentoTrava(modelo.Id, modelo.Tipo_Atendimento.Id);
                                    if (codigo_encerramento != null)
                                    {
                                        if (tem_improdutivo.Result == true && codigo_encerramento.Encerra_OS_Apos_Baixa == "N")
                                        {
                                            // Perguntar se a pessoa quer reabrir a OS ou cancelar o atendimento
                                            retorno.Mensagem = "N";

                                            retorno.Ok = false;
                                            retorno.IdObjeto = modelo.Id;
                                            return retorno;
                                        }
                                        else if (codigo_encerramento.Encerra_OS_Apos_Baixa == "S")
                                        {
                                            //retorno.Mensagem = "S";
                                            retorno.Ok = true;
                                            retorno.IdObjeto = modelo.Id;
                                            //return retorno;
                                        }
                                    }
                                    else
                                    {
                                        retorno.Mensagem = "N";

                                        retorno.Ok = false;
                                        retorno.IdObjeto = modelo.Id;
                                        return retorno;
                                    }

                                }

                            }

                            if (modelo.PlanoProduto != null)
                            {
                                if (modelo.PlanoProduto.Plano != null)
                                {
                                    var plano = await new Conexao<Plano>(session).selectFirst(p => p.Id == modelo.PlanoProduto.Plano.Id);
                                    if (!plano.OSTrocaEndereco)
                                    {
                                        if (modelo.Endereco_Mudanca != null)
                                        {
                                            modelo.PlanoProduto.Endereco_Instalacao = modelo.Endereco_Mudanca;
                                            modelo.Endereco_Mudanca = null;
                                            modelo.Ultima_Acao.Add(new Atendimento_Acao()
                                            {
                                                Atendimento = modelo,
                                                HTML = "Alteração do endereço de instalacao após conclusao do atendimento",
                                                Responsavel = modelo.Responsavel,
                                                Status_Atendimento = modelo.Status_Atendimento,
                                                UsuarioCriacao = value.Usuario,

                                            });
                                            await repositoryCPP.SaveOrUpdate(modelo.PlanoProduto);
                                        }
                                    }
                                }
                            }
                            modelo.StatusAuditoria = StatusAuditoria.AguardandoAvaliacao;
                            retorno.Concluido_Cancelado = true;
                        }
                        else if (modelo.Status_Atendimento == Status_Atendimento.Cancelado)
                        {
                            if (modelo.PlanoProduto != null)
                            {
                                if (modelo.PlanoProduto.Plano != null)
                                {
                                    var plano = await new Conexao<Plano>(session).selectFirst(p => p.Id == modelo.PlanoProduto.Plano.Id);
                                    if (modelo.Endereco_Mudanca != null)
                                    {
                                        modelo.Endereco_Mudanca = null;
                                        modelo.Ultima_Acao.Add(new Atendimento_Acao()
                                        {
                                            Atendimento = modelo,
                                            HTML = "Alteração de endereço de instalação abortada após o cancelamento do atendimento.",
                                            Responsavel = modelo.Responsavel,
                                            Status_Atendimento = modelo.Status_Atendimento,
                                            UsuarioCriacao = value.Usuario,

                                        });
                                    }
                                }
                            }
                            retorno.Concluido_Cancelado = true;
                        }
                        await repository.SaveOrUpdate(modelo);

                        var id_atendimento = conf.Formato_Protocolo_Atendimento == 0 ? Convert.ToInt64(modelo.Protocolo) : modelo.Id;
                        var verifica_ordem_servico = await Ordens_Servico(id_atendimento, "STATUS_EXECUCAO_ORDEM_SERVICO");
                        if (modelo.EtapaFluxo?.Etapa_CriaOS == true && verifica_ordem_servico.recordsFiltered == 0)
                        {
                            using (var apiOrdemServico = new Documento_ProducaoApiController())
                            using (var apiPlanoProduto = new PlanoProdutoApiController())
                            {
                                var retorno_plano_produto = await apiPlanoProduto.GetDadosPlanoProduto(modelo?.PlanoProduto?.Id, null, modelo.Cliente.Id);
                                var objeto_plano_produto = retorno_plano_produto.Objeto;
                                var lista_txt_generico = new List<String>();
                                var criar_ordem_servico = true;

                                if (objeto_plano_produto?.Cod_Localidade == null)
                                {
                                    lista_txt_generico.Add("Campo LOCALIDADE não informado no endereço do cliente.");
                                    criar_ordem_servico = false;
                                }

                                if (String.IsNullOrEmpty(objeto_plano_produto.Cod_Area))
                                {
                                    lista_txt_generico.Add("Campo ÁREA não informado no endereço do cliente.");
                                    criar_ordem_servico = false;
                                }

                                if (String.IsNullOrEmpty(objeto_plano_produto.Cod_Escritorio))
                                {
                                    lista_txt_generico.Add("Campo GRUPO ÁREA não informado no endereço do cliente.");
                                    criar_ordem_servico = false;
                                }

                                if (criar_ordem_servico)
                                {
                                    var resultado = await apiOrdemServico.Gravar(new ParamWebApi<Documento_ProducaoCadastroModel>
                                    {
                                        Usuario = User.Identity.GetUserId(),
                                        Objeto = new Documento_ProducaoCadastroModel
                                        {
                                            Cod_Localidade = objeto_plano_produto?.Cod_Localidade,
                                            Cod_Area = objeto_plano_produto?.Cod_Area,
                                            Cod_Escritorio = objeto_plano_produto?.Cod_Escritorio,
                                            Projeto = modelo?.Tipo_Atendimento?.Projeto,
                                            Contrato = modelo?.Tipo_Atendimento?.CodCt,
                                            Id_Tipo_servico = Convert.ToInt16(modelo.Tipo_Atendimento.Desc_Tipo_Servico),
                                            IdDepartamento = modelo?.Tipo_Atendimento?.Id_Departamento_Padrao,
                                            Tipo = "C",
                                            Id_Cliente_Plano_Produto = modelo?.PlanoProduto?.Id,
                                            Id_Categoria_Cliente = Convert.ToInt64(modelo?.Cliente?.Categoria_Cliente?.Id),
                                            CPF_CNPJ_Cliente = modelo?.Cliente?.CpfCnpj,
                                            Id_Cliente = modelo?.Cliente?.Id,
                                            Nome_Cliente = modelo?.Cliente?.NomeFantasia ?? modelo?.Cliente?.Razao_Social,
                                            Observacao_OS = "Fluxo de atendimento - Etapa Criar OS",
                                            Id_Atendimento = id_atendimento,
                                            Id_Plano_Produto = objeto_plano_produto?.Id_Plano_Produto
                                        }
                                    });

                                    string[] split = resultado.Ok ? resultado.IdObjeto.Num_Obra.Split('/') : new string[0];
                                    var descricao_acao = resultado.Ok ? $"Foi criada a OS de número {split[0]} a partir da entrada do atendimento na etapa {modelo.EtapaFluxo.Descricao}." : resultado.Mensagem;
                                    await GravarAcao_FluxoEtapa(new GravarAcaoAtendimentoModel
                                    {
                                        IdAtendimento = id_atendimento,
                                        Numero = modelo.Numero,
                                        DescricaoAcao = descricao_acao,
                                        Usuario = User.Identity.GetUserId(),
                                        TipoAtividade = 0 //Interno
                                    });
                                }
                                else
                                {
                                    await GravarAcao_FluxoEtapa(new GravarAcaoAtendimentoModel
                                    {
                                        IdAtendimento = id_atendimento,
                                        Numero = modelo.Numero,
                                        DescricaoAcao = $"A etapa - {modelo.EtapaFluxo.Descricao} tentou criar uma OS, porém a operação foi abortada devido ao erro: {String.Join(" - ", lista_txt_generico)}",
                                        Usuario = User.Identity.GetUserId(),
                                        TipoAtividade = 0 //Interno
                                    });
                                }
                            }
                        }
                    }

                    if (value.Objeto.RespostaCamposPersonalizados != null && value.Objeto.RespostaCamposPersonalizados.Count > 0)
                    {
                        foreach (var item in value.Objeto.RespostaCamposPersonalizados)
                        {
                            var atendimentoCampo = await repoAtendimentoCampos.selectFirst(r => r.Id == Convert.ToInt64(item.IdSelecionado));

                            var vinculoAtendimentoCampos = await repoVinculoCampos.selectFirst(x => x.Atendimento == modelo &&
                                                                                   x.AtendimentoCampos == atendimentoCampo);

                            if (vinculoAtendimentoCampos != null)
                            {
                                vinculoAtendimentoCampos.Resposta = item.RespostaCampo;
                                vinculoAtendimentoCampos.UsuarioAlteracao = value.Usuario;
                                vinculoAtendimentoCampos.DataHoraAlteracao = DateTime.Now;

                                await repoVinculoCampos.SaveOrUpdate(vinculoAtendimentoCampos);
                            }
                            else
                            {
                                var novoVinculoCampos = new VinculoAtendimentoCampos
                                {
                                    Atendimento = modelo,
                                    AtendimentoCampos = atendimentoCampo,
                                    Resposta = item.RespostaCampo
                                };

                                await repoVinculoCampos.SaveOrUpdate(novoVinculoCampos);
                            }
                        }
                    }

                    if (value.Objeto.RespostasCamposPersonalizadosUnico != null && value.Objeto.RespostasCamposPersonalizadosUnico.Count > 0)
                    {
                        foreach (var item in value.Objeto.RespostasCamposPersonalizadosUnico)
                        {
                            var atendimentoCampo = await repoAtendimentoCampos.selectFirst(r => r.Id == Convert.ToInt64(item.IdCampoSelecionado));

                            var atendimentoCampoResp = await repoAtendimentoCamposRespostas.selectFirst(r => r.Id == Convert.ToInt64(item.IdSelecionado));

                            var vinculoAtendimentoCampos = await repoVinculoCampos.selectFirst(x => x.Atendimento == modelo &&
                                                                                               x.AtendimentoCampos == atendimentoCampo);

                            if (vinculoAtendimentoCampos != null)
                            {
                                vinculoAtendimentoCampos.UsuarioAlteracao = value.Usuario;
                                vinculoAtendimentoCampos.DataHoraAlteracao = DateTime.Now;
                                vinculoAtendimentoCampos.AtendimentoCamposRespostas = atendimentoCampoResp;

                                await repoVinculoCampos.SaveOrUpdate(vinculoAtendimentoCampos);
                            }
                            else
                            {
                                var novoVinculoCampos = new VinculoAtendimentoCampos
                                {
                                    Atendimento = modelo,
                                    AtendimentoCampos = atendimentoCampo,
                                    AtendimentoCamposRespostas = atendimentoCampoResp,
                                    Resposta = item.RespostaCampo
                                };

                                await repoVinculoCampos.SaveOrUpdate(novoVinculoCampos);
                            }
                        }
                    }



                    if (modelo?.Tipo_Atendimento?.Script != null && modeloAnterior == null)
                    {
                        await GravarScriptAtendimento(value.Objeto.RespostasScript.Select(p => { p.Id_Atendimento = modelo.Id; return p; }).ToList());
                    }

                    if (modeloAnterior != null)
                    {
                        using (var repositoryLogs = new Conexao<LogAtendimento>(sessionLog))
                        {
                            var logs = modelo.GeraLog(modeloAnterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogs.Save(logs);
                        }
                    }
                    else
                    {
                        using (var repositoryLogs = new Conexao<LogAtendimento>(sessionLog))
                        {
                            var logs = modelo.GeraLog(modeloAnterior, value.Usuario, value.Objeto.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Insercao);
                            await repositoryLogs.Save(logs);
                        }
                    }


                    if (value.Objeto.Id_Responsavel != modelo.Responsavel.Id || modelo.Responsavel == null)
                    {
                        using (var api = new NotificacoesApiController())
                        {
                            var notificacao = api.EnviaNotificacoesAtendimento(4, modelo, null);
                        }
                    }

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/AtendimentoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Atendimento>(session))
                    {

                        Atendimento modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

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

        private async Task GerarEmailMudancaStatus(Int64 id_atendimento, Status_Atendimento novo_status)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repository_emailAtendimento = new Conexao<Email_Atendimento>(session))
            using (var repository_atendimento = new Conexao<Atendimento>(session))
            {

                var atendimento = await repository_atendimento.selectFirst(p => p.Id == id_atendimento);

                if (atendimento.Id_Email_Abertura.HasValue && atendimento?.Origem_Atendimento?.Id == -1)
                {
                    await repository_emailAtendimento.Save(new Email_Atendimento
                    {
                        Atendimento = atendimento,
                        Mudanca_Status = novo_status,
                        Enviado = false
                    });
                }
            }
        }

        #region Historico


        private AtendimentoHistoricoCadastroModel GetViewObj_Historico(Atendimento obj)
        {
            String _endereco = String.Empty;
            var retorno = new AtendimentoHistoricoCadastroModel
            {
                id = obj.Id,
                setor = obj.Setor?.Descricao,
                responsavel = obj.Responsavel?.Nome,
                status = obj.Status_Atendimento,
                assunto = obj.Assunto,
                data_Limite = obj.DataHora_Indicador?.ToString("dd/MM/yyyy HH:mm:ss"),
                numero = Convert.ToString(obj.Numero),
                tipo_Atendimento = obj.Tipo_Atendimento?.Nome,
                EtapaAtual = obj.EtapaFluxo?.Descricao,
            };

            List<AtendimentoHistoricoAcoesCadastroModel> historico = new List<AtendimentoHistoricoAcoesCadastroModel>();
            var fechamento = obj.Data_Hora_Fechamento != null ? obj.Data_Hora_Fechamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : String.Empty;
            Int32 _ordem = 0; // apenas criado para preencher a assinatura da função GetCoresSla

            foreach (var acao in obj.Ultima_Acao)
            {
                var porcentagem = obj.DataHora_Indicador.HasValue ? CalculaSla(obj.DataHora_Indicador, obj.DataHoraCriacao, fechamento, acao.DataHoraCriacao) : 100;

                Encoding iso = Encoding.GetEncoding("ISO-8859-1");
                Encoding utf8 = Encoding.UTF8;
                byte[] isoBytes = iso.GetBytes(acao.HTML);
                byte[] utf8Bytes = Encoding.Convert(iso, utf8, isoBytes);

                historico.Add(new AtendimentoHistoricoAcoesCadastroModel
                {
                    dataCriacao = acao?.DataHoraCriacao?.ToString("dd/MM/yyyy HH:mm:ss"),
                    dataCriacaoDateTime = acao?.DataHoraCriacao,
                    id = acao.Id,
                    responsavel = acao.Origem == Origem_AtendimentoAcao.EMAIL ? obj?.Cliente?.NomeFantasia : acao.Responsavel?.UserName,
                    setor = acao.Responsavel?.Departamento?.Descricao,
                    status = HtmlEnumExtensions.GetEnumDescription<Status_Atendimento>(acao.Status_Atendimento),
                    //Usuario = acao.UsuarioCriacao,
                    descricaoAcao = acao.Origem == Origem_AtendimentoAcao.CHAT ? String.Empty : acao.HTML,
                    html = utf8.GetString(utf8Bytes),
                    origem = HtmlEnumExtensions.GetEnumDescription<Origem_AtendimentoAcao>(acao.Origem),
                    mensagens_Chat = acao.Origem == Origem_AtendimentoAcao.CHAT,
                    tipoAtividade = acao.TipoAtividade,

                    arquivos = acao.Arquivos_AtendimentoAcao.Select(p => new ArquivoAcaoModel
                    {
                        arquivo = p.Id + p.Extensao,
                        extensao = p.Extensao,
                        id = p.Id,
                        nome_Arquivo = p.Arquivo
                    }).ToList(),

                    cor_SLA = GetCoresSla(porcentagem, ref _ordem),
                    mensagenschat = acao.Origem == Origem_AtendimentoAcao.CHAT ? JsonConvert.DeserializeObject<List<Mensagem_ChatModels>>(acao.HTML) : new List<Mensagem_ChatModels>(),
                });
            }

            retorno.historico_Atendimento = historico
                .OrderBy(p => p.dataCriacaoDateTime ?? DateTime.MinValue) // Ordenação exclusivamente pela data
                .ToList();

            return retorno;
        }
        #endregion


        [HttpGet()]
        [Route("api/AtendimentoApi/GetViewScriptAtendimento")]
        public async Task<GetApiRetorno<List<Atendimento_Script_AtendimentoModel>>> GetViewScriptAtendimento(Int64 Id_TipoAtendimento, Int64? Id_Atendimento)
        {
            GetApiRetorno<List<Atendimento_Script_AtendimentoModel>> retorno = new GetApiRetorno<List<Atendimento_Script_AtendimentoModel>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var repoTipoAtend = new Conexao<Tipo_Atendimento>(session))
                    using (var repoAtendScriptAtendimento = new Conexao<Atendimento_Script_Atendimento>(session))
                    {
                        var tipo_atendimentos = await repoTipoAtend.select(p => p.Id == Id_TipoAtendimento && p.Script != null);
                        var tipo_atendimentos_filtrados = tipo_atendimentos.Where(p => p != null && p.Script.Status == Status_Default.Ativo).ToList();

                        if (tipo_atendimentos_filtrados.Count() == 0)
                            throw new Exception("Tipo de Atendimento sem script relacionado ou script inativo");

                        Tipo_Atendimento tipo_atendimento = tipo_atendimentos_filtrados[0];
                        Script_Atendimento script_atendimento = tipo_atendimento.Script;

                        var perguntasScriptRespondidas = new List<Atendimento_Script_Atendimento>();

                        bool possuiRespostaScriptAtendimento = false;

                        if (Id_Atendimento.HasValue)
                        {
                            perguntasScriptRespondidas = (await repoAtendScriptAtendimento.select(p => p.Atendimento.Id == Id_Atendimento)).ToList();

                            possuiRespostaScriptAtendimento = perguntasScriptRespondidas != null && perguntasScriptRespondidas.Count() > 0;
                        }

                        List<Atendimento_Script_AtendimentoModel> tmpRetorno = new List<Atendimento_Script_AtendimentoModel>();

                        int row = 0;

                        if (possuiRespostaScriptAtendimento)
                        {
                            foreach (Atendimento_Script_Atendimento perguntaRespondida in perguntasScriptRespondidas)
                            {
                                var respostasMultiplas = new List<bool>();

                                if (perguntaRespondida.ScriptAtendimentoQuestion.Tipo_Resposta == Script_Tipo_Resposta.Multipla_Escolha)
                                {
                                    List<String> _respondidas = perguntaRespondida?.Resposta?.Split(';').ToList();

                                    foreach (string respostaPossivel in perguntaRespondida.ScriptAtendimentoQuestion.Respostas_Possiveis?.Split(';'))
                                    {
                                        if (_respondidas == null)
                                            respostasMultiplas.Add(false);
                                        else
                                            respostasMultiplas.Add(_respondidas.Find(p => p == respostaPossivel) != null);
                                    }
                                }

                                tmpRetorno.Add(new Atendimento_Script_AtendimentoModel
                                {
                                    // Como a pergunta já foi registrada só precisamos informar o ID do registro da resposta
                                    // e deixamos o ID modelo da pergunta zerado
                                    Id_Resposta_Pergunta = perguntaRespondida.ScriptAtendimentoQuestion.Id,
                                    Id_Modelo_Pergunta = 0,

                                    Id_Tipo_Atendimento = tipo_atendimento.Id,
                                    Descricao_Tipo_Atendimento = tipo_atendimento.Nome,
                                    Id = script_atendimento.Id,
                                    Id_Atendimento = Id_Atendimento,
                                    Instrucao_Atendente = perguntaRespondida.ScriptAtendimentoQuestion.Explicacao_Operador,
                                    Instrucao_Cliente = perguntaRespondida.ScriptAtendimentoQuestion.Explicacao_Cliente,
                                    Pergunta = perguntaRespondida.ScriptAtendimentoQuestion.Pergunta,
                                    RespostasPossiveis = perguntaRespondida.ScriptAtendimentoQuestion.Respostas_Possiveis?.Split(';').ToList(),
                                    Tipo_Resposta = perguntaRespondida.ScriptAtendimentoQuestion.Tipo_Resposta,
                                    Row_Pergunta = row,
                                    Resposta = perguntaRespondida?.Resposta,
                                    RespostasMultiplas = respostasMultiplas,
                                    Observacao_Atendimento = perguntaRespondida?.Observacao
                                });

                                row++;
                            }
                        }
                        else
                        {
                            foreach (Script_AtendimentoItem modeloPerguntaScript in script_atendimento.Perguntas)
                            {
                                var respostasMultiplas = new List<bool>();

                                if (modeloPerguntaScript.Tipo_Resposta == Script_Tipo_Resposta.Multipla_Escolha)
                                {
                                    List<String> _respostasPossiveis = modeloPerguntaScript.Respostas_Possiveis.Split(';').ToList();

                                    // Marca as respostas de múltipla escolha como não escolhidas
                                    for (int i = 0; i < _respostasPossiveis.Count(); i++)
                                    {
                                        respostasMultiplas.Add(false);
                                    }
                                }

                                tmpRetorno.Add(new Atendimento_Script_AtendimentoModel
                                {
                                    // Para as novas perguntas retornamos o ID da resposta como 0 e
                                    // informamos o ID do modelo da pergunta do script
                                    Id_Resposta_Pergunta = 0,
                                    Id_Modelo_Pergunta = modeloPerguntaScript.Id,

                                    Id_Tipo_Atendimento = tipo_atendimento.Id,
                                    Descricao_Tipo_Atendimento = tipo_atendimento.Nome,
                                    Id = script_atendimento.Id,
                                    Id_Atendimento = Id_Atendimento,
                                    Instrucao_Atendente = modeloPerguntaScript.Explicacao_Operador,
                                    Instrucao_Cliente = modeloPerguntaScript.Explicacao_Cliente,
                                    Pergunta = modeloPerguntaScript.Pergunta,
                                    RespostasPossiveis = modeloPerguntaScript.Respostas_Possiveis?.Split(';').ToList(),
                                    Tipo_Resposta = modeloPerguntaScript.Tipo_Resposta,
                                    Row_Pergunta = row,
                                    Resposta = null,
                                    RespostasMultiplas = respostasMultiplas,
                                    Observacao_Atendimento = null
                                });

                                row++;
                            }
                        }

                        retorno.Objeto = tmpRetorno;
                    }

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

        [HttpGet()]
        [Route("api/AtendimentoApi/GetScriptAtendimentoAtivo")]
        public async Task<GetApiRetorno> GetScriptAtendimentoAtivo(Int64 idScript)
        {
            GetApiRetorno retorno = new GetApiRetorno();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var repoAtendScriptAtendimento = new Conexao<Script_Atendimento>(session))
                {
                    var tipo_atendimentos = await repoAtendScriptAtendimento.selectFirst(p => p.Id == idScript);


                    if (tipo_atendimentos.Status == Status_Default.Ativo)
                    {
                        retorno.Ok = true;
                    }
                    else
                    {
                        retorno.Ok = false;
                    }
                }
            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/AtendimentoApi/GravarScriptAtendimento")]
        public async Task<PostApiRetorno<Int64>> GravarScriptAtendimento(List<Atendimento_Script_AtendimentoModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    using (var repoRespostaScriptAtendimento = new Conexao<Atendimento_Script_Atendimento>(session))
                    using (var repoScriptAtendQuestion = new Conexao<ScriptAtendimentoQuestion>(session))
                    {
                        if (value.Count() > 0)
                        {
                            Int64 Id_Atendimento = value[0].Id_Atendimento.Value;

                            Tipo_Atendimento tipo_atendimento = await new Conexao<Tipo_Atendimento>(session).selectFirst(p => p.Id == value[0].Id_Tipo_Atendimento);
                            Atendimento atendimento = await new Conexao<Atendimento>(session).selectFirst(p => p.Id == value[0].Id_Atendimento);

                            List<Script_AtendimentoItem> modelosPerguntas = tipo_atendimento.Script.Perguntas.ToList();

                            var repostasScriptAtendimento = new List<Atendimento_Script_Atendimento>();

                            foreach (Atendimento_Script_AtendimentoModel respostaScriptView in value)
                            {
                                Atendimento_Script_Atendimento respostaScript = null;

                                if (respostaScriptView.Id_Resposta_Pergunta != 0)
                                {
                                    respostaScript =
                                        await repoRespostaScriptAtendimento.selectFirst(p => p.Atendimento.Id == atendimento.Id &&
                                                                                             p.ScriptAtendimentoQuestion.Id == respostaScriptView.Id_Resposta_Pergunta);

                                    if (respostaScript != null)
                                    {
                                        respostaScript.DataHoraAlteracao = DateTime.Now;
                                        respostaScript.UsuarioAlteracao = User.Identity.GetUserId();

                                        respostaScript.Observacao = respostaScriptView.Observacao_Atendimento;

                                        if (respostaScript.ScriptAtendimentoQuestion != null &&
                                            respostaScript.ScriptAtendimentoQuestion.Tipo_Resposta == Script_Tipo_Resposta.Multipla_Escolha)
                                        {
                                            List<String> _respostasPossiveis = respostaScript.ScriptAtendimentoQuestion.Respostas_Possiveis.Split(';').ToList();
                                            var _respostas = new List<String>();

                                            for (int i = 0; i < _respostasPossiveis.Count(); i++)
                                            {
                                                if (respostaScriptView.RespostasMultiplas.ElementAtOrDefault(i) != false)
                                                    _respostas.Add(_respostasPossiveis[i]);
                                            }

                                            respostaScript.Resposta = String.Join(";", _respostas);
                                        }
                                        else
                                        {
                                            respostaScript.Resposta = respostaScriptView.Resposta;
                                        }
                                    }
                                }
                                else if (respostaScriptView.Id_Modelo_Pergunta != 0)
                                {
                                    Script_AtendimentoItem modeloPerguntaScript = modelosPerguntas.Find(p => p.Id == respostaScriptView.Id_Modelo_Pergunta);

                                    if (modeloPerguntaScript != null)
                                    {
                                        var scriptAtendimentoQuestion = ScriptAtendimentoQuestion.FromPerguntaModelo(modeloPerguntaScript);

                                        scriptAtendimentoQuestion.DataHoraCriacao = DateTime.Now;
                                        scriptAtendimentoQuestion.UsuarioCriacao = User.Identity.GetUserId();

                                        await repoScriptAtendQuestion.Save(scriptAtendimentoQuestion);

                                        respostaScript = new Atendimento_Script_Atendimento
                                        {
                                            DataHoraCriacao = DateTime.Now,
                                            UsuarioCriacao = User.Identity.GetUserId(),

                                            Atendimento = atendimento,
                                            Tipo_Atendimento = tipo_atendimento,
                                            Observacao = respostaScriptView.Observacao_Atendimento,
                                            Pergunta = modeloPerguntaScript.Pergunta,
                                            Resposta = respostaScriptView.Resposta,
                                            Script_Atendimento = tipo_atendimento.Script,
                                            ScriptAtendimentoQuestion = scriptAtendimentoQuestion
                                        };

                                        if (respostaScript.ScriptAtendimentoQuestion.Tipo_Resposta == Script_Tipo_Resposta.Multipla_Escolha)
                                        {
                                            List<String> _respostasPossiveis = respostaScript.ScriptAtendimentoQuestion?.Respostas_Possiveis?.Split(';').ToList();
                                            var _respostas = new List<String>();

                                            for (int i = 0; i < _respostasPossiveis.Count(); i++)
                                            {
                                                if (respostaScriptView.RespostasMultiplas.ElementAtOrDefault(i) != false)
                                                    _respostas.Add(_respostasPossiveis[i]);
                                            }

                                            respostaScript.Resposta = String.Join(";", _respostas);
                                        }
                                    }
                                }

                                if (respostaScript != null)
                                    repostasScriptAtendimento.Add(respostaScript);
                            }

                            await repoRespostaScriptAtendimento.SaveOrUpdate(repostasScriptAtendimento);

                            transaction.Commit();

                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null && transaction.IsActive)
                    {
                        transaction.Rollback();
                    }

                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        //[HttpGet()]
        [HttpPost()]
        [Route("api/AtendimentoApi/BuscaFiltroConsultaCliAtd")]
        public async Task<DataTableResponse<ConsultaClienteAtendimentoModel>> BuscaFiltroConsultaCliAtd(ConsultaClienteAtendimento_DataTableRequest request)
        {
            List<ConsultaClienteAtendimentoModel> retorno = new List<ConsultaClienteAtendimentoModel>();
            IOrderedEnumerable<ConsultaClienteAtendimentoModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO A ");

                sqlFrom.AppendLine("LEFT JOIN TB_CLIENTE B ");
                sqlFrom.AppendLine("ON (A.ID_CLIENTE = B.ID) ");

                sqlFrom.AppendLine("LEFT JOIN TB_CATEGORIA_ATENDIMENTO C ");
                sqlFrom.AppendLine("ON (A.ID_CATEGORIA_ATENDIMENTO = C.ID) ");

                sqlFrom.AppendLine("LEFT JOIN TB_TIPO_ATENDIMENTO D ");
                sqlFrom.AppendLine("ON (A.ID_TIPO_ATENDIMENTO = D.ID) ");

                sqlFrom.AppendLine("LEFT JOIN TB_MOTIVO_STATUS_ATENDIMENTO E ");
                sqlFrom.AppendLine("ON (A.ID_MOTIVO_STATUS = E.ID) ");

                sqlFrom.AppendLine("LEFT JOIN ASPNETUSERS F ");
                sqlFrom.AppendLine("ON (A.ID_RESPONSAVEL = F.ID) ");

                sqlFrom.AppendLine("WHERE B.STATUS <> 3 ");
                sqlFrom.AppendLine("AND ID_CLIENTE = @VALOR");
                parametros.Add("@VALOR", request.Id_Cliente);

                if ((!String.IsNullOrEmpty(request.Dt_Inicial_Filtro)) && (!String.IsNullOrEmpty(request.Dt_Final_Filtro)))
                {
                    sqlFrom.AppendLine(" AND (A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM) ");
                    parametros.Add("@DATAINI", DateTime.Parse(String.Format("{0} 00:00", request.Dt_Inicial_Filtro)));
                    parametros.Add("@DATAFIM", DateTime.Parse(String.Format("{0} 23:59", request.Dt_Final_Filtro)));
                }

                if (request.Status != null)
                {
                    if (request.Status.HasValue)
                    {
                        sqlFrom.AppendLine(" AND ( A.STATUS_ATENDIMENTO = @STATUS ) ");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status.Value));
                    }
                }


                if (request.Id_Tipo_Atendimento != null)
                {
                    if (request.Id_Tipo_Atendimento.HasValue)
                    {
                        sqlFrom.AppendLine(" AND ( A.ID_TIPO_ATENDIMENTO = @ID_TIPO_ATENDIMENTO  ) ");
                        parametros.Add("@ID_TIPO_ATENDIMENTO", Convert.ToInt32(request.Id_Tipo_Atendimento.Value));
                    }
                }



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

                    sql.AppendLine("SELECT A.ID, A.NUMERO , A.PRIORIDADE_PADRAO, A.ASSUNTO, A.PESSOA_CONTATO, B.NOMEFANTASIA, A.ID_ORIGEM_ATENDIMENTO ");
                    sql.AppendLine(", A.ID_TIPO_ATENDIMENTO, A.ID_DEPARTAMENTO, A.ID_RESPONSAVEL, A.ID_MOTIVO_STATUS, A.STATUS_ATENDIMENTO, A.PROBLEMA_RELATADO, A.DATAHORACRIACAO, A.DATA_HORA_FECHAMENTO, ");
                    sql.AppendLine(" C.DESCRICAO AS CATEGORIA_ATENDIMENTO, D.NOME AS NOME_ATENDIMENTO, E.DESCRICAO AS MOTIVO_STATUS, F.NOME AS NOME_RESPONSAVEL, B.CPF, D.NOME AS TIPO_ATENDIMENTO ");

                    sql.AppendLine(sqlFrom.ToString());

                    //Filtrando
                    //if (!String.IsNullOrEmpty(request.search.value))
                    //{
                    //    sql.AppendLine(" AND ( UPPER(A.ASSUNTO) LIKE UPPER(@SEARCH) ) ");
                    //    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    //}


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ConsultaClienteAtendimentoModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Numero = reader["NUMERO"] != DBNull.Value ? Convert.ToInt64(reader["NUMERO"]) : 0,
                                //Prioridade_Padrao = HtmlEnumExtensions.GetEnumDescription<Prioridade_Atendimento>(Convert.ToString(reader["PRIORIDADE_PADRAO"])),
                                Assunto = reader["ASSUNTO"] != DBNull.Value ? Convert.ToString(reader["ASSUNTO"].ToString()) : "",
                                //Pessoa_Contato = Convert.ToString(reader["PESSOA_CONTATO"].ToString()),
                                //Ultima_Acao = Convert.ToString(reader["ULTIMA_ACAO"].ToString()),
                                //Categoria_Atendimento = Convert.ToString(reader["CATEGORIA_ATENDIMENTO"].ToString()),
                                //Cliente = Convert.ToString(reader["NOMEFANTASIA"].ToString()),
                                //Origem_Atendimento = Convert.ToString(reader["ID_ORIGEM_ATENDIMENTO"]),
                                //Plano = Convert.ToString(reader["ID_PLANO"]),
                                //Produto = Convert.ToString(reader["ID_PRODUTO"]),
                                //Tipo_Atendimento = Convert.ToString(reader["TIPO_ATENDIMENTO"]),
                                //Setor = Convert.ToString(reader["ID_DEPARTAMENTO"]),
                                Responsavel = reader["NOME_RESPONSAVEL"] != DBNull.Value ? Convert.ToString(reader["NOME_RESPONSAVEL"]) : "",
                                Motivo_Status = reader["MOTIVO_STATUS"] != DBNull.Value ? Convert.ToString(reader["MOTIVO_STATUS"]) : "",
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Atendimento>(Convert.ToString(reader["STATUS_ATENDIMENTO"])), // Convert.ToString(reader["STATUS"]),
                                                                                                                                                    //Problema_Relatado = Convert.ToString(reader["PROBLEMA_RELATADO"].ToString()),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : null,
                                NomeAtendimento = reader["NOME_ATENDIMENTO"] != DBNull.Value ? Convert.ToString(reader["NOME_ATENDIMENTO"]) : "",
                                Data_Hora_Fechamento = reader["DATA_HORA_FECHAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_FECHAMENTO"]).ToString("dd/MM/yyyy HH:mm") : null,

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
                    case 1:     // NUMERO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Numero);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Numero);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ConsultaClienteAtendimentoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        //[HttpGet()]
        [HttpPost()]
        [Route("api/atendimentoapi/BuscaFiltroConsultaVendAtd")]
        public async Task<DataTableResponse<ConsultaClienteAtendimentoModel>> BuscaFiltroConsultaVendAtd(ConsultaClienteAtendimento_DataTableRequest request)
        {
            List<ConsultaClienteAtendimentoModel> retorno = new List<ConsultaClienteAtendimentoModel>();
            IOrderedEnumerable<ConsultaClienteAtendimentoModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO A ");

                sqlFrom.AppendLine("LEFT JOIN TB_CLIENTE B ");
                sqlFrom.AppendLine("ON (A.ID_CLIENTE = B.ID) ");

                sqlFrom.AppendLine("LEFT JOIN TB_CATEGORIA_ATENDIMENTO C ");
                sqlFrom.AppendLine("ON (A.ID_CATEGORIA_ATENDIMENTO = C.ID) ");

                sqlFrom.AppendLine("LEFT JOIN TB_TIPO_ATENDIMENTO D ");
                sqlFrom.AppendLine("ON (A.ID_TIPO_ATENDIMENTO = D.ID) ");

                sqlFrom.AppendLine("LEFT JOIN TB_MOTIVO_STATUS_ATENDIMENTO E ");
                sqlFrom.AppendLine("ON (A.ID_MOTIVO_STATUS = E.ID) ");

                sqlFrom.AppendLine("LEFT JOIN ASPNETUSERS F ");
                sqlFrom.AppendLine("ON (A.ID_RESPONSAVEL = F.ID) ");

                sqlFrom.AppendLine("LEFT JOIN TB_VENDA G ");
                sqlFrom.AppendLine("ON (A.ID = G.ID_ATENDIMENTO) ");

                sqlFrom.AppendLine("WHERE B.STATUS <> 3 ");
                sqlFrom.AppendLine("AND A.ID_CLIENTE = @VALOR");
                parametros.Add("@VALOR", request.Id_Cliente);

                if (!String.IsNullOrEmpty(request.search.value))//ok
                {
                    sqlFrom.AppendLine(" AND ((A.ID LIKE @SEARCH) )) ");

                    if (!parametros.ContainsKey("@SEARCH"))
                    {
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));

                    }
                }

                if ((!String.IsNullOrEmpty(request.Dt_Inicial_Filtro)) && (!String.IsNullOrEmpty(request.Dt_Final_Filtro)))
                {
                    sqlFrom.AppendLine(" AND (A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM) ");
                    parametros.Add("@DATAINI", DateTime.Parse(String.Format("{0} 00:00", request.Dt_Inicial_Filtro)));
                    parametros.Add("@DATAFIM", DateTime.Parse(String.Format("{0} 23:59", request.Dt_Final_Filtro)));
                }

                if (request.Status != null)
                {
                    if (request.Status.HasValue)
                    {
                        sqlFrom.AppendLine(" AND ( A.STATUS_ATENDIMENTO = @STATUS ) ");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status.Value));
                    }
                }


                if (request.Id_Tipo_Atendimento != null)
                {
                    if (request.Id_Tipo_Atendimento.HasValue)
                    {
                        sqlFrom.AppendLine(" AND ( A.ID_TIPO_ATENDIMENTO = @ID_TIPO_ATENDIMENTO  ) ");
                        parametros.Add("@ID_TIPO_ATENDIMENTO", Convert.ToInt32(request.Id_Tipo_Atendimento.Value));
                    }
                }



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

                    sql.AppendLine("SELECT A.ID, A.NUMERO , A.PRIORIDADE_PADRAO, A.ASSUNTO, A.PESSOA_CONTATO, B.NOMEFANTASIA, A.ID_ORIGEM_ATENDIMENTO ");
                    sql.AppendLine(", A.ID_TIPO_ATENDIMENTO, A.ID_DEPARTAMENTO, A.ID_RESPONSAVEL, A.ID_MOTIVO_STATUS, A.STATUS_ATENDIMENTO, A.PROBLEMA_RELATADO, A.DATAHORACRIACAO, A.DATA_HORA_FECHAMENTO, ");
                    sql.AppendLine(" C.DESCRICAO AS CATEGORIA_ATENDIMENTO, D.NOME AS NOME_ATENDIMENTO, E.DESCRICAO AS MOTIVO_STATUS, F.NOME AS NOME_RESPONSAVEL, B.CPF, D.NOME AS TIPO_ATENDIMENTO,G.ID AS VENDA ");

                    sql.AppendLine(sqlFrom.ToString());

                    //Filtrando
                    //if (!String.IsNullOrEmpty(request.search.value))
                    //{
                    //    sql.AppendLine(" AND ( UPPER(A.ASSUNTO) LIKE UPPER(@SEARCH) ) ");
                    //    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    //}


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ConsultaClienteAtendimentoModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Numero = reader["NUMERO"] != DBNull.Value ? Convert.ToInt64(reader["NUMERO"]) : 0,
                                //Prioridade_Padrao = HtmlEnumExtensions.GetEnumDescription<Prioridade_Atendimento>(Convert.ToString(reader["PRIORIDADE_PADRAO"])),
                                Assunto = reader["ASSUNTO"] != DBNull.Value ? Convert.ToString(reader["ASSUNTO"].ToString()) : "",
                                //Pessoa_Contato = Convert.ToString(reader["PESSOA_CONTATO"].ToString()),
                                //Ultima_Acao = Convert.ToString(reader["ULTIMA_ACAO"].ToString()),
                                //Categoria_Atendimento = Convert.ToString(reader["CATEGORIA_ATENDIMENTO"].ToString()),
                                //Cliente = Convert.ToString(reader["NOMEFANTASIA"].ToString()),
                                //Origem_Atendimento = Convert.ToString(reader["ID_ORIGEM_ATENDIMENTO"]),
                                //Plano = Convert.ToString(reader["ID_PLANO"]),
                                //Produto = Convert.ToString(reader["ID_PRODUTO"]),
                                //Tipo_Atendimento = Convert.ToString(reader["TIPO_ATENDIMENTO"]),
                                //Setor = Convert.ToString(reader["ID_DEPARTAMENTO"]),
                                Responsavel = reader["NOME_RESPONSAVEL"] != DBNull.Value ? Convert.ToString(reader["NOME_RESPONSAVEL"]) : "",
                                Motivo_Status = reader["MOTIVO_STATUS"] != DBNull.Value ? Convert.ToString(reader["MOTIVO_STATUS"]) : "",
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Atendimento>(Convert.ToString(reader["STATUS_ATENDIMENTO"])), // Convert.ToString(reader["STATUS"]),
                                                                                                                                                    //Problema_Relatado = Convert.ToString(reader["PROBLEMA_RELATADO"].ToString()),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : null,
                                NomeAtendimento = reader["NOME_ATENDIMENTO"] != DBNull.Value ? Convert.ToString(reader["NOME_ATENDIMENTO"]) : "",
                                Data_Hora_Fechamento = reader["DATA_HORA_FECHAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_FECHAMENTO"]).ToString("dd/MM/yyyy HH:mm") : null,
                                Venda = Convert.ToInt64(reader["VENDA"]),
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
                    case 1:     // NUMERO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Numero);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Numero);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ConsultaClienteAtendimentoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/clienteapi/GravarEnderecoAtendimento")]
        public async Task<PostApiRetorno<Int64>> GravarEnderecoAtendimento([FromBody] ParamWebApi<EnderecoClienteViewModel> value, Int64? atendimento)
        {
            var retorno = new PostApiRetorno<Int64>();
            try
            {
                Endereco_Cliente modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    Atendimento modelo_atendimento = await new Conexao<Atendimento>(session).selectFirst(p => p.Id == atendimento.Value);
                    using (var repository = new Conexao<Endereco_Cliente>(session))
                    using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                    using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                    using (var repoEndLogradouro = new Conexao<Endereco_Logradouro>(session))
                    using (var repoAtendimento = new Conexao<Atendimento>(session))
                    using (var repoPais = new Conexao<Pais>(session))
                    using (var repoTipoImovel = new Conexao<Tipo_Imovel>(session))
                    {
                        if (value.Objeto.Busca_Endereco == Busca_Endereco.Endereco_Logradouro || value.Objeto.Busca_Endereco == Busca_Endereco.Condominio_Predio)
                        {
                            Endereco_Logradouro endereco_Logradouro = await repoEndLogradouro.selectFirst(p => p.Id == value.Objeto.Id_Endereco_Logradouro);

                            modelo = await repository.selectFirst(p => p.Id == value.Objeto.Id_Endereco);

                            modelo.Busca_Endereco = value.Objeto.Busca_Endereco;
                            modelo.Endereco_Logradouro = endereco_Logradouro;

                            await repository.SaveOrUpdate(modelo);

                            //Cliente_PlanoProduto ClientePlanoProduto = await repoPlanoProduto.selectFirst(p => p.ClienteContrato.NumeroContrato == modelo_atendimento.Contrato);
                            //ClientePlanoProduto.Endereco_Instalacao = modelo;
                            //await repoPlanoProduto.SaveOrUpdate(ClientePlanoProduto);
                        }
                        else
                        {
                            modelo = new Endereco_Cliente
                            {
                                UsuarioCriacao = value.Usuario,
                                Bairro = value.Objeto.Bairro,
                                Cep = value.Objeto.CEP,
                                Cidade = value.Objeto.Cidade,
                                Complemento = value.Objeto.Complemento,
                                Id_Bairro = value.Objeto.Id_Bairro,
                                Id_Cidade = value.Objeto.Id_Cidade,
                                Id_UF = value.Objeto.Id_UF,
                                Latitude = value.Objeto.Latitude != null ? Convert.ToDouble(value.Objeto.Latitude) : (Double?)null,
                                Longitude = value.Objeto.Longitude != null ? Convert.ToDouble(value.Objeto.Longitude) : (Double?)null,
                                Logradouro = value.Objeto.Logradouro,
                                Numero = value.Objeto.Numero,
                                Pais = await repoPais.selectFirst(p => p.Id == value.Objeto.Id_Pais),
                                Tipo_Imovel = await repoTipoImovel.selectFirst(p => p.Id == value.Objeto.Id_Tipo_Imovel),
                                Ponto_Referencia = value.Objeto.Ponto_Referencia,
                                UF = value.Objeto.UF,

                                Cod_Area = value.Objeto.Cod_Area,
                                Cod_Escritorio = value.Objeto.Cod_Escritorio,
                                Cod_Localidade = value.Objeto.Cod_Localidade != null ? Convert.ToInt32(value.Objeto.Cod_Localidade) : (Int32?)null,
                                Localidade = !String.IsNullOrEmpty(value.Objeto.Localidade) ? value.Objeto.Localidade : "",
                                Busca_Endereco = null,

                            };

                            await repository.SaveOrUpdate(modelo);
                        }

                        //IdInstalacao = Convert.ToString((await repository.Max(p => p.Id)));

                        var repository_atendimento = new Conexao<Atendimento>(session);
                        var atendimento_obj = await repository_atendimento.selectFirst(p => p.Id == atendimento);
                        atendimento_obj.Endereco_Mudanca = modelo;

                        if (atendimento_obj.Status_Atendimento == Status_Atendimento.Concluido)
                        {
                            Atendimento atendimentoBanco = await repoAtendimento.selectFirst(p => p.Id == atendimento);

                            atendimentoBanco.Endereco_Mudanca = modelo;
                            atendimentoBanco.PlanoProduto.Endereco_Instalacao = modelo;
                            atendimento_obj.Endereco_Mudanca = null;
                            //atendimento_obj.PlanoProduto.Endereco_Instalacao.Cod_Area = value.Objeto.Cod_Area;
                            //atendimento_obj.PlanoProduto.Endereco_Instalacao.Cod_Escritorio = value.Objeto.Cod_Escritorio;
                            //atendimento_obj.PlanoProduto.Endereco_Instalacao.Cod_Localidade = !String.IsNullOrEmpty(value.Objeto.Cod_Localidade) ? Convert.ToInt32(value.Objeto.Cod_Localidade) : (Int32?)null;
                            //atendimento_obj.PlanoProduto.Endereco_Instalacao.Localidade = !String.IsNullOrEmpty(value.Objeto.Localidade) ? value.Objeto.Localidade : "";
                            await repoAtendimento.SaveOrUpdate(atendimentoBanco);
                        }

                        retorno.IdObjeto = modelo.Id;
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
        [Route("api/atendimentoapi/ReabrirAtendimento")]
        public async Task<PostApiRetorno<Int64>> ReabrirAtendimento(AlterarStatusAtendimentoViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryLog = new Conexao<LogAtendimento>(sessionLog))
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Atendimento>(session))
                using (var repoFluxoAtendimento = new Conexao<FluxoAtendimento>(session))
                using (var repoDepartamento = new Conexao<Departamento>(session))
                using (var api = new NotificacoesApiController())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        foreach (var id in value.ids)
                        {
                            Atendimento atendimento = await repository.selectFirst(p => p.Id == id);
                            Atendimento modeloAnterior = (Atendimento)atendimento.Clone();
                            if (atendimento.EtapaFluxo != null)
                            {
                                var atendimentoLog = await repositoryLog.select(x => x.IdLog == id && x.Campo.Upper() == "ETAPA ATUAL");
                                if (atendimentoLog.Count > 0)
                                {
                                    var objAtend = atendimentoLog.OrderByDescending(x => x.Id).First();

                                    if (!String.IsNullOrEmpty(objAtend.Campo_ValorAntes))
                                    {
                                        var _idFluxoAtendimento = objAtend.Campo_ValorAntes.Substring(0, 3);
                                        atendimento.EtapaFluxo = await repoFluxoAtendimento.selectFirst(x => x.Id == Convert.ToInt64(_idFluxoAtendimento));
                                    }

                                    atendimentoLog = await repositoryLog.select(x => x.IdLog == id && x.Campo.Upper() == "SETOR");
                                    Int64 _idSetor = modeloAnterior.Setor.Id;
                                    if (atendimentoLog.Count > 0)
                                    {
                                        objAtend = atendimentoLog.OrderByDescending(x => x.Id).First();
                                        if (objAtend.Campo_ValorAntes != null)
                                        {
                                            _idSetor = Convert.ToInt64(objAtend.Campo_ValorAntes.Substring(0, 1));
                                        }
                                    }

                                    atendimento.Setor = await repoDepartamento.selectFirst(x => x.Id == Convert.ToInt64(_idSetor));
                                }
                            }

                            atendimento.Status_Atendimento = Status_Atendimento.Em_Atendimento;
                            atendimento.Motivo_Status = null;
                            atendimento.Data_Hora_Abertura = DateTime.Now;
                            atendimento.Data_Hora_Fechamento = null;
                            atendimento.StatusAuditoria = 0;
                            await repository.SaveOrUpdate(atendimento);

                            var notificacao = api.EnviaNotificacoesAtendimento(8, atendimento, null);

                            var logs = atendimento.GeraLog(modeloAnterior, value.usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLog.SaveOrUpdate(logs);
                        }

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
        [HttpGet()]
        [Route("api/atendimentoapi/CriarAtendimentoChatOla")]
        public async Task<String> CriarAtendimentoChatOla(Int64 Id_Cliente, Int64 Id_Tipo_Atendimento, Int64 Id_Plano, String Assunto, String ProblemaRelatado, String ConversationSid = null)
        {

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var repositoryTipoAtendimento = new Conexao<Tipo_Atendimento>(session))
            using (var repositoryPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repositoryCliente = new Conexao<Cliente>(session))
            using (var repositoryOrigemAtendimento = new Conexao<Origem_Atendimento>(session))
            using (var repositoryAtendimento = new Conexao<Atendimento>(session))
            using (var repositoryLogAtendimento = new Conexao<LogAtendimento>(sessionLog))
            using (var repositoryConfiguracoesGerais = new Conexao<Configuracoes_Gerais>(session))
            {
                var configuracao = await repositoryConfiguracoesGerais.selectFirst();
                var tipo_Atendimento = await repositoryTipoAtendimento.selectFirst(p => p.Id == Id_Tipo_Atendimento);
                var atendimento = new Atendimento
                {
                    Cliente = await repositoryCliente.selectFirst(p => p.Id == Id_Cliente),
                    Assunto = Assunto,
                    Problema_Relatado = ProblemaRelatado,
                    PlanoProduto = await repositoryPlanoProduto.selectFirst(p => p.Id == Id_Plano),
                    Tipo_Atendimento = tipo_Atendimento,
                    Categoria_Atendimento = tipo_Atendimento?.Categoria_Atendimento,
                    Origem_Atendimento = ConversationSid != null ? await repositoryOrigemAtendimento.selectFirst(p => p.Id == -3) : await repositoryOrigemAtendimento.selectFirst(p => p.Id == -2),
                    Status = Status_Default.Ativo,
                    Prioridade = tipo_Atendimento?.Prioridade_Padrao,
                    DataHoraCriacao = DateTime.Now,
                    ConversationSid = ConversationSid
                };

                await repositoryAtendimento.SaveOrUpdate(atendimento);

                if (atendimento.Tipo_Atendimento != null)
                {
                    atendimento.DataHora_Indicador = await GetDataPromessa(session, atendimento.Cliente.Id, atendimento.Tipo_Atendimento.Id, atendimento.PlanoProduto.Id, DateTime.Now, atendimento.Prioridade);
                }

                atendimento.Numero = Gera_NumeroProtocolo(atendimento, configuracao);
                atendimento.Protocolo = atendimento.Numero;
                await repositoryAtendimento.SaveOrUpdate(atendimento);
                var logs = atendimento.GeraLog(null, null, null, Sinapse.ModelLog.TipoOperLog.Insercao);
                await repositoryLogAtendimento.SaveOrUpdate(logs);
                return atendimento.Numero;

            }
        }

        [HttpGet()]
        [Route("api/atendimentoapi/CriarAtendimentoAvulso")]
        public async Task<String> CriarAtendimentoAvulso(AtendimentoChat chat, String ConversationSid = null)
        {

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var repositoryTipoAtendimento = new Conexao<Tipo_Atendimento>(session))
            using (var repositoryPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repositoryCliente = new Conexao<Cliente>(session))
            using (var repositoryClienteAniel = new Conexao<ClienteAniel>(sessionT5))
            using (var repositoryOrigemAtendimento = new Conexao<Origem_Atendimento>(session))
            using (var repositoryAtendimento = new Conexao<Atendimento>(session))
            using (var repositoryLogAtendimento = new Conexao<LogAtendimento>(sessionLog))
            using (var repositoryConfiguracoesGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
            {
                var canal = await repositoryCanal.selectFirst(g => g.Id == chat.idCanal.Value);
                Origem_Atendimento origem = null;

                switch (canal.Tipo_Canal_Comunicacao)
                {
                    case Tipo_Canal_Comunicacao.Whatsapp:
                        {
                            origem = await repositoryOrigemAtendimento.selectFirst(p => p.Id == -3);
                            break;
                        }
                    case Tipo_Canal_Comunicacao.Telegram:
                        {
                            origem = await repositoryOrigemAtendimento.selectFirst(p => p.Id == -4);
                            break;
                        }
                    case Tipo_Canal_Comunicacao.Messenger:
                        {
                            origem = await repositoryOrigemAtendimento.selectFirst(p => p.Id == -5);
                            break;
                        }
                    case Tipo_Canal_Comunicacao.Instagram:
                        {
                            origem = await repositoryOrigemAtendimento.selectFirst(p => p.Id == -6);
                            break;
                        }
                    case Tipo_Canal_Comunicacao.Skype:
                        {
                            origem = await repositoryOrigemAtendimento.selectFirst(p => p.Id == -7);
                            break;
                        }
                    default:
                        break;
                }

                var configuracao = await repositoryConfiguracoesGerais.selectFirst();
                var atendimento = new Atendimento
                {
                    Origem_Atendimento = origem,
                    Status = Status_Default.Ativo,
                    DataHoraCriacao = DateTime.Now,
                    ConversationSid = ConversationSid,
                };

                await repositoryAtendimento.SaveOrUpdate(atendimento);

                atendimento.Numero = Gera_NumeroProtocolo(atendimento, configuracao);
                atendimento.Protocolo = atendimento.Numero;

                if (!chat.bot)
                {
                    int tipo_cliente = Convert.ToInt32(configuracao.Pessoa_Contato_Primeira_Vez);

                    var telefoneVerificado = NormalizarNumero(chat.cliente.telefone);

                    var cliente = await GetClienteChatBot(session, telefoneVerificado, chat.cliente.identificadorTelegram);

                    if (cliente == null)
                    {
                        ClienteAniel cliente_aniel = new ClienteAniel
                        {
                            NomeFantasia = chat.cliente.nome,
                            Telefone_Celular = telefoneVerificado,
                            CPF = chat.cliente.cpf_cnpj,
                            Email_Principal = chat.cliente.email,
                            Tipo_Cliente = tipo_cliente == 0 ? Tipo_ClienteAniel.Cliente : Tipo_ClienteAniel.Prospecto,
                            Tipo_Pessoa = chat.cliente.cpf_cnpj != null ? (chat.cliente.cpf_cnpj.Length <= 11 ? Tipo_Pessoa.Fisica : Tipo_Pessoa.Juridica) : (Tipo_Pessoa?)null,
                            Status = Status_Cliente.Ativo,
                            Status_Prospecto = Status_ProspectoAniel.Lead_Qualificado,
                            Cod_Cliente = Convert.ToInt32(repositoryClienteAniel.Max(p => p.Cod_Cliente).Result) + 1
                        };
                        await repositoryClienteAniel.Save(cliente_aniel);

                        cliente = new Cliente
                        {
                            NomeFantasia = chat.cliente.nome,
                            Telefone_Celular = telefoneVerificado,
                            CpfCnpj = chat.cliente.cpf_cnpj,
                            Email_Principal = chat.cliente.email,
                            Tipo_Cliente = tipo_cliente == 0 ? Tipo_Cliente.Prospecto : Tipo_Cliente.Cliente,
                            Tipo_Pessoa = chat.cliente.cpf_cnpj != null ? (chat.cliente.cpf_cnpj.Length <= 11 ? Tipo_Pessoa.Fisica : Tipo_Pessoa.Juridica) : (Tipo_Pessoa?)null,
                            Status = Status_Cliente.Ativo,
                            Status_Prospecto = Status_Prospecto.Lead_Qualificado,
                            Identificador_Telegram = chat.cliente.identificadorTelegram,
                            Cod_Cliente = cliente_aniel?.Cod_Cliente
                        };
                    }

                    await repositoryCliente.SaveOrUpdate(cliente);
                    atendimento.Cliente = cliente;
                }

                await repositoryAtendimento.SaveOrUpdate(atendimento);
                var logs = atendimento.GeraLog(null, null, null, Sinapse.ModelLog.TipoOperLog.Insercao);
                await repositoryLogAtendimento.SaveOrUpdate(logs);
                return atendimento.Numero;
            }
        }

        private async Task<Cliente> GetClienteChatBot(ISession session, String telefone, String IdentificadorTelegram)
        {
            using (var repositoryCliente = new Conexao<Cliente>(session))
            {
                var obj = new Cliente();
                if (!String.IsNullOrEmpty(telefone))
                {
                    obj = await repositoryCliente.selectFirst(x => x.Telefone_Celular.IsLike(telefone, MatchMode.Anywhere) || x.Telefone_Fixo.IsLike(telefone, MatchMode.Anywhere) ||
                    x.Telefone_Fixo2.IsLike(telefone, MatchMode.Anywhere) || x.Telefone_Recado.IsLike(telefone, MatchMode.Anywhere));

                    /*
                     * Comentado abaixo o trecho de código, caso o Hibernate de algum tipo de conflito no futuro, a consulta utilizando Linq
                     * Está funcional.
                     * No momento, o hibernate continuará, pois está com o código mais enxuto.
                     * *obj = (from cli in session.Query<Cliente>()
                           where cli.Telefone_Celular.Trim().Replace("(", "").Replace(")", "").Replace("-", "").Contains(telefone) ||
                           cli.Telefone_Fixo.Trim().Replace("(", "").Replace(")", "").Replace("-", "").Contains(telefone) ||
                           cli.Telefone_Fixo2.Trim().Replace("(", "").Replace(")", "").Replace("-", "").Contains(telefone) ||
                           cli.Telefone_Recado.Trim().Replace("(", "").Replace(")", "").Replace("-", "").Contains(telefone)
                           select cli).First();*/

                    if (obj != null)
                    {
                        return obj;
                    }
                }

                if (!String.IsNullOrEmpty(IdentificadorTelegram))
                {
                    obj = await repositoryCliente.selectFirst(x => x.Identificador_Telegram == IdentificadorTelegram);
                    if (obj != null)
                    {
                        return obj;
                    }
                }

                return null;
            }
        }

        private String Gera_NumeroProtocolo(Atendimento atendimento, Configuracoes_Gerais configuracao)
        {
            return configuracao.Formato_Protocolo_Atendimento == Formato_Protocolo_Atendimento.Ano_Mes_Dia_Hora_Sequencial ? String.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHH"), atendimento.Id) : atendimento.Id.ToString();
        }

        public async Task StatusEmAtendimento(String Numero_Protocolo, String AtendenteId)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repositoryAtendimento = new Conexao<Atendimento>(session))
            using (var repositoryUser = new Conexao<IdentityUser>(session))
            {
                var atendimento = await repositoryAtendimento.selectFirst(p => p.Numero == Numero_Protocolo);
                atendimento.Status_Atendimento = Status_Atendimento.Em_Atendimento;
                if (!String.IsNullOrEmpty(AtendenteId))
                {
                    var usuario = await repositoryUser.selectFirst(p => p.Id == AtendenteId);
                    atendimento.Responsavel = usuario;
                    atendimento.Setor = usuario.Departamento;
                }

                await repositoryAtendimento.SaveOrUpdate(atendimento);
            }

        }

        public async Task<String> GetHistoricoMensagensChat(String Numero_Protocolo)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repositoryAtendimento = new Conexao<Atendimento>(session))
            {
                var atendimento = await repositoryAtendimento.selectFirst(p => p.Numero == Numero_Protocolo);
                var mensagens = new List<Mensagem_ChatModels>();
                foreach (var item in atendimento.Ultima_Acao.ToList().Where(p => p.Origem == Origem_AtendimentoAcao.CHAT))
                {
                    mensagens.AddRange(JsonService.DeserializeObject<List<Mensagem_ChatModels>>(item.HTML));
                }

                //var conversas = String.Join("", atendimento.Ultima_Acao.ToList().Where(p => p.Origem == Origem_AtendimentoAcao.CHAT).Select(p => p.HTML));
                var conversas = JsonService.SerializeObject(mensagens);
                return conversas;
            }

        }

        public async Task GravarHistoricoMensagensChat(String Numero_Protocolo, List<Mensagem_ChatModels> lista_mensagens)
        {
            using (LogExecucao logExecucao = new LogExecucao("GravarHistoricoMensagensChat", CONSTANTES.LogExecucaoAnielChannel))
            {
                try
                {
                    using (ISession session = HibernateUtil.GetSession())
                    using (var repositoryAtendimento = new Conexao<Atendimento>(session))
                    using (var repository_emailAtendimento = new Conexao<Email_Atendimento>(session))
                    {
                        var atendimento = await repositoryAtendimento.selectFirst(p => p.Numero == Numero_Protocolo);

                        var acao = new Atendimento_Acao
                        {
                            Origem = Origem_AtendimentoAcao.CHAT,
                            HTML = JsonService.SerializeObject(lista_mensagens.OrderBy(p => p.dataHoraServidor).ToList()),
                            Atendimento = atendimento,
                            Responsavel = atendimento.Responsavel,
                            Status_Atendimento = atendimento.Status_Atendimento,
                        };

                        atendimento.Ultima_Acao.Add(acao);
                        await repositoryAtendimento.SaveOrUpdate(atendimento);

                        //await repository_emailAtendimento.Save(new Email_Atendimento { Atendimento_Acao = acao, Enviado = false });
                    }
                }

                catch (Exception ex)
                {
                    logExecucao.RegistraLog("Exception: " + ex.Message);
                    logExecucao.RegistraLog("InnerException: " + ex.InnerException?.Message);
                    logExecucao.RegistraLog("StackTrace: " + ex.StackTrace);
                    throw ex;
                }
            }

        }


        public async Task FinalizarChat(String protocolo, bool StatusAtendimentoFinalizado, String Usuario, Int64? IdMotivoStatus)
        {
            try
            {
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChat>(conexaoMongo))
                using (var repositoryChat = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    AtendimentoChatMongo atendimento = await repositoryChat.Find(p => p.protocolo == protocolo);
                    if (atendimento != null)
                    {
                        var suporte = atendimento.suporte;

                        if (String.IsNullOrEmpty(atendimento?.suporte?.terminoAtendimento) == false)
                        {
                            await ChatHub.GetChatSrategy(atendimento).EnviarMensagem(atendimento.protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                            {
                                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                enviadoPeloSuporte = true,
                                usuario = atendimento?.suporte?.nomeAgente,
                                statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                                mensagem = atendimento?.suporte?.terminoAtendimento,
                                conversasId = atendimento.conversas != null ? atendimento.conversas.FirstOrDefault()?.id : null,
                                matriculaUsuario = atendimento?.suporte?.idSuporte,
                            }));
                        }

                        await ChatHub.GetChatSrategy(atendimento).EnviarMensagem(atendimento.protocolo, JsonConvert.SerializeObject(new Mensagem_ChatModels
                        {
                            dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                            enviadoPeloSuporte = true,
                            statusEnvio = Model.enumerado.StatusEnvioEnum.ENVIADO,
                            mensagem = "Concluiu chat",
                            usuario = atendimento.suporte?.nomeAgente,
                            fluxoChat = true,
                            conversasId = atendimento.conversas != null ? atendimento.conversas.FirstOrDefault()?.id : null,
                            matriculaUsuario = atendimento?.suporte?.idSuporte,
                        }));

                        string conversasId = String.Empty;
                        using (var atendimento_api = new AtendimentoApiController())
                        {
                            conversasId = atendimento.conversas != null ? atendimento.conversas[atendimento.conversas.Count - 1].id : null;
                            await atendimento_api.GravarHistoricoMensagensChat(protocolo, atendimento.mensagens.FindAll(p => (p.statusEnvio == Model.enumerado.StatusEnvioEnum.ENVIADO || p.statusEnvio == Model.enumerado.StatusEnvioEnum.LIDO) && p.conversasId == conversasId));
                        }

                        IMongoQueryable<AtendimentoChatMongo> query = repositoryChat.GetQueryable();
                        var atendimentos = query.ToList().FindAll(p => p.protocolo == atendimento.protocolo).ToList();
                        AtendimentoChatMongo atendimentoChatMongo = atendimentos.FirstOrDefault();


                        if (atendimentoChatMongo != null)
                        {
                            AtendimentoChatMongo chatMongo = await repositoryChat.Find(x => x.protocolo == protocolo);
                            chatMongo.bot = false;
                            chatMongo.dataEncerramento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            if (chatMongo.conversas != null && chatMongo.conversas.Count > 0)
                            {
                                chatMongo.conversas[chatMongo.conversas.Count - 1].data_final = chatMongo.dataEncerramento;
                            }

                            await repositoryChat.Update(x => x.protocolo == atendimento.protocolo, chatMongo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }

        public async Task FinalizarAtendimento(String Protocolo, String Usuario, Int64 IdMotivoStatus)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var repoMotivoStatusAtendimento = new Conexao<Motivo_Status_Atendimento>(session))
            using (var repoAtendimentoLog = new Conexao<LogAtendimento>(sessionLog))
            using (var repoEtapaAtendimento = new Conexao<FluxoAtendimento>(session))
            using (var repoDepatamento = new Conexao<Departamento>(session))
            using (var repoIdentityUser = new Conexao<IdentityUser>(session))
            using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repo = new Conexao<Pesquisa_Satisfacao>(session))
            {

                var atendimento = await repoAtendimento.selectFirst(p => p.Numero == Protocolo);
                var etapaAtendimento = await repoEtapaAtendimento.selectFirst(e => e.Tipo_Atendimento == atendimento.Tipo_Atendimento);
                Atendimento modeloAnterior = (Atendimento)atendimento.Clone();
                //atendimento.Status_Atendimento = Status_Atendimento.Concluido;
                atendimento.Status_Atendimento = (Status_Atendimento?)atendimento?.EtapaFluxo?.StatusAtendimento ?? Status_Atendimento.Concluido;
                atendimento.Data_Hora_Fechamento = DateTime.Now;
                atendimento.Motivo_Status = await repoMotivoStatusAtendimento.selectFirst(p => p.Id == IdMotivoStatus);
                atendimento.StatusAuditoria = StatusAuditoria.Aprovado;
                atendimento.Setor = etapaAtendimento.Setor_Departamento == null ? atendimento.Setor : await repoDepatamento.selectFirst(d => d.Cod_Depto == etapaAtendimento.Setor_Departamento);
                atendimento.Responsavel = await repoIdentityUser.selectFirst(p => p.Id == Usuario);

                if (atendimento.Status_Atendimento == Status_Atendimento.Concluido)
                {
                    var pesquisa_disparo_auto = repo.select(p => p.DisparoAuto == PesquisaSatisfacaoDisparoAuto.Envio_Automatico_Atendimento).Result;
                    if (pesquisa_disparo_auto.Count != 0 && atendimento.Cliente != null)
                    {
                        await new Pesquisa_SatisfacaoApiController().EnviarEmMassa(new Pesquisa_Satisfacao_PesquisaEmMassaModel
                        {
                            ids = new List<long> { atendimento.Cliente.Id },
                            ids_pesquisa = pesquisa_disparo_auto.Select(p => p.Id).ToList(),
                            usuario = User.Identity.GetUserId()
                        });
                    }

                    if (atendimento.PlanoProduto != null)
                    {
                        if (atendimento.PlanoProduto.Plano != null)
                        {
                            var plano = await new Conexao<Plano>(session).selectFirst(p => p.Id == atendimento.PlanoProduto.Plano.Id);
                            if (!plano.OSTrocaEndereco)
                            {
                                if (atendimento.Endereco_Mudanca != null)
                                {
                                    atendimento.PlanoProduto.Endereco_Instalacao = atendimento.Endereco_Mudanca;
                                    atendimento.Endereco_Mudanca = null;
                                    atendimento.Ultima_Acao.Add(new Atendimento_Acao()
                                    {
                                        Atendimento = atendimento,
                                        HTML = "Alteração do endereço de instalacao após conclusao do atendimento.",
                                        Responsavel = atendimento.Responsavel,
                                        Status_Atendimento = atendimento.Status_Atendimento,
                                        UsuarioCriacao = User.Identity.GetUserId(),

                                    });

                                    await repoCliente_PlanoProduto.SaveOrUpdate(atendimento.PlanoProduto);
                                }
                            }
                        }
                    }
                }

                var logs = atendimento.GeraLog(modeloAnterior, Usuario, Usuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                await repoAtendimentoLog.SaveOrUpdate(logs);

                await repoAtendimento.SaveOrUpdate(atendimento);
            }
        }

        public async Task<String> GetEmailCliente(String Protocolo)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            {
                var atendimento = await repoAtendimento.selectFirst(p => p.Numero == Protocolo);
                return atendimento.Cliente.Email_Principal;
            }
        }

        public async Task<AtendimentoCadastroModel> DadosAtendimentoChat(String Protocolo)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            {
                var atendimento = await repoAtendimento.selectFirst(p => p.Numero == Protocolo);
                return new AtendimentoCadastroModel
                {
                    Categoria_Cliente = atendimento?.Cliente?.Categoria_Cliente?.Descricao,
                    CPF_CNPJ_Cliente = atendimento?.Cliente?.CpfCnpj,
                    Id = atendimento.Id,
                    Id_Categoria_Atendimento = atendimento?.Categoria_Atendimento?.Id,
                    Id_Responsavel = atendimento?.Responsavel?.Id,
                    Id_Setor = atendimento?.Setor?.Id,
                    Id_Tipo_Atendimento = atendimento?.Tipo_Atendimento?.Id,
                    Nome_Cliente = atendimento.Tipo_Pessoa_Contato == Tipo_Pessoa_Contato.Titular ? atendimento?.Cliente?.NomeFantasia : atendimento?.Pessoa_Contato,
                    Nome_Plano_Produto = atendimento?.PlanoProduto?.Plano?.Descricao ?? atendimento?.PlanoProduto?.Produto?.Descricao
                };
            }
        }

        [HttpPost()]
        [Route("api/atendimentoapi/GravarDadosAtendimentoChat")]
        public async Task<PostApiRetorno<Int64>> GravarDadosAtendimentoChat(DadosAtendimentoChatModel dados)
        {
            var retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var repoCategoriaAtendimento = new Conexao<Categoria_Atendimento>(session))
            using (var repoTipoAtendimento = new Conexao<Tipo_Atendimento>(session))
            using (var repoIdentityUser = new Conexao<IdentityUser>(session))
            using (var repoDepartamento = new Conexao<Departamento>(session))
            {
                var atendimento = await repoAtendimento.selectFirst(p => p.Id == dados.Id);
                atendimento.Categoria_Atendimento = await repoCategoriaAtendimento.selectFirst(p => p.Id == dados.Id_Categoria_Atendimento);
                atendimento.Tipo_Atendimento = await repoTipoAtendimento.selectFirst(p => p.Id == dados.Id_Tipo_Atendimento);
                atendimento.Responsavel = await repoIdentityUser.selectFirst(p => p.Id == dados.Id_Responsavel);
                atendimento.Setor = await repoDepartamento.selectFirst(p => p.Id == dados.Id_Setor);
                await repoAtendimento.SaveOrUpdate(atendimento);
                retorno.IdObjeto = atendimento.Id;
                retorno.Ok = true;
            }

            List<DadosAtendimentoChatModel> lista_dados = new List<DadosAtendimentoChatModel>();
            lista_dados.Select(p => (p.Id, p.Nome_Cliente, p.Cpf, p)).Distinct().Select(y => y.p);

            return retorno;
        }


        [HttpPost()]
        [Route("api/atendimentoapi/ConvidarAtendente")]
        public async Task<PostApiRetorno<Int64>> ConvidarAtendente(ConvidarAtendenteRequestModel value)
        {
            var retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoIdentityUser = new Conexao<IdentityUser>(session))
                using (var repoDepartamento = new Conexao<Departamento>(session))
                using (var repoNotificacoes = new Conexao<Notificacoes>(session))
                using (NotificacoesApiController notificacoesApiController = new NotificacoesApiController())
                {
                    if (String.IsNullOrEmpty(value.usuarioConvidado))
                    {
                        throw new Exception("Preencha parametros de convite corretamente.");
                    }

                    using (ISession sessionLog = HibernateUtil.GetSessionLog())
                    using (var repositoryLogs = new Conexao<LogAtendimento>(sessionLog))
                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                    {
                        AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == value.protocolo);

                        if (chat?.suporte?.idSuporte != User.Identity.GetUserId())
                        {
                            throw new Exception("{ERROR_NOT_USER}");
                        }

                        List<UsuarioConvidado> lista = chat.usuariosConvidados.FindAll(p => p.usuarioAceite == value.usuarioConvidado && p.aceite == null).ToList();

                        if (lista.Count == 0)
                        {
                            UsuarioConvidado convidado = new UsuarioConvidado
                            {
                                aceite = null,
                                dataConvite = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                dataAceite = null,
                                connectionid = null,
                                usuarioAceite = value.usuarioConvidado,
                                usuarioConvite = User.Identity.GetUserId()
                            };
                            List<UsuarioConvidado> usuario = new List<UsuarioConvidado>();
                            usuario.Add(convidado);
                            chat.usuariosConvidados.AddRange(usuario);

                            await repository.Update(p => p.protocolo == chat.protocolo, chat);

                            var atendimento = await repoAtendimento.selectFirst(p => p.Protocolo == chat.protocolo);
                            if (atendimento != null)
                            {
                                await notificacoesApiController.EnviaNotificacoesAtendimento(38, atendimento);
                            }
                            await ChatHub.AtualizaNotificacoesSino(value.usuarioConvidado);
                            retorno.IdObjeto = Convert.ToInt64(value.protocolo);
                            retorno.Ok = true;
                        }
                        else
                        {
                            retorno.IdObjeto = Convert.ToInt64(value.protocolo);
                            retorno.Ok = false;
                            retorno.Mensagem = "Já existe um convite pendente para este usuário e conversa!";
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/atendimentoapi/AtivarConvite")]
        public async Task<PostApiRetorno<Int64>> AtivarConvite(ConvidarAtendenteRequestModel value)
        {
            var retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoIdentityUser = new Conexao<IdentityUser>(session))
                using (var repoDepartamento = new Conexao<Departamento>(session))
                using (var repoNotificacoes = new Conexao<Notificacoes>(session))
                using (NotificacoesApiController notificacoesApiController = new NotificacoesApiController())
                {

                    if (!String.IsNullOrEmpty(value.usuarioConvidado))
                    {
                        var usuario = await repoIdentityUser.selectFirst(p => p.Id == value.usuarioConvidado);
                    }
                    else
                    {
                        throw new Exception("preencha parametros de convite corretamente");
                    }

                    using (ISession sessionLog = HibernateUtil.GetSessionLog())
                    using (var repositoryLogs = new Conexao<LogAtendimento>(sessionLog))
                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                    {
                        AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == value.protocolo);

                        List<UsuarioConvidado> lista = chat.usuariosConvidados.FindAll(p => p.usuarioAceite == value.usuarioConvidado).ToList();

                        if (lista.Count > 0)
                        {
                            //await ChatHub.EnviarConvite(chat.protocolo, value.usuarioConvidado);
                            retorno.Ok = true;
                        }
                        else
                        {
                            retorno.IdObjeto = Convert.ToInt64(value.protocolo);
                            retorno.Ok = false;
                            retorno.Mensagem = "Já existe um convite aceite para este usuário e conversa!";
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        private static SemaphoreSlim ObterSemaforo(string chave)
        {
            lock (_lockDictionary)
            {
                if (!_semaforos.ContainsKey(chave))
                    _semaforos[chave] = new SemaphoreSlim(1, 1);

                return _semaforos[chave];
            }
        }

        [HttpPost()]
        [Route("api/atendimentoapi/TransferirAtendimento")]
        public async Task<PostApiRetorno<Int64>> TransferirAtendimento(TransferirAtendimentoRequestModel value)
        {
            var semaforo = ObterSemaforo(value.protocolo);
            await semaforo.WaitAsync();
            var retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoIdentityUser = new Conexao<IdentityUser>(session))
                using (var repoDepartamento = new Conexao<Departamento>(session))
                using (var repoNotificacoes = new Conexao<Notificacoes>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoClienteAniel = new Conexao<ClienteAniel>(sessionT5))
                using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                using (var repositoryLogs = new Conexao<LogAtendimento>(sessionLog))
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                using (var repositorySuporte = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                using (NotificacoesApiController notificacoesApiController = new NotificacoesApiController())
                {

                    var atendimento = await repoAtendimento.selectFirst(p => p.Numero == value.protocolo);


                    Atendimento modeloAnterior = (Atendimento)atendimento.Clone();

                    if (String.IsNullOrEmpty(value.novoAtendente) == false)
                    {
                        var usuario = await repoIdentityUser.selectFirst(p => p.Id == value.novoAtendente);
                        atendimento.Responsavel = usuario;
                        atendimento.Setor = usuario.Departamento;

                        var notificacao = await repoNotificacoes.selectFirst(p => p.Id == 4);
                        if (notificacao != null && notificacao.Status == Status_Default.Ativo)
                        {
                            await notificacoesApiController.EnviaNotificacoesAtendimento(4, atendimento);
                        }
                    }
                    else if (value.novoDepartamento != 0)
                    {
                        atendimento.Responsavel = null;
                        atendimento.Setor = await repoDepartamento.selectFirst(p => p.Id == value.novoDepartamento);

                        var notificacao = await repoNotificacoes.selectFirst(p => p.Id == 9);
                        if (notificacao != null && notificacao.Status == Status_Default.Ativo)
                        {
                            await notificacoesApiController.EnviaNotificacoesAtendimento(9, atendimento);
                        }
                    }
                    else
                    {
                        throw new Exception("preencha parametros de transferencia corretamente");
                    }

                    AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == value.protocolo);
                    if (atendimento.Responsavel == null)
                    {
                        chat.suporte = null;
                        await repository.Update(p => p.protocolo == chat.protocolo, chat);
                    }
                    else if (atendimento?.Responsavel?.Id != null && atendimento?.Responsavel?.Id != chat?.suporte?.idSuporte)
                    {
                        chat.suporte = null;
                        var suporte = ChatHub.SuporteDisponiveis.Find(x => x.idSuporte == atendimento?.Responsavel?.Id);

                        if (suporte != null)
                        {
                            suporte.status = StatusAtendimento.EM_ATENDIMENTO;
                            await repositorySuporte.Update(p => p.idSuporte == suporte.idSuporte, suporte);
                        }

                        await repository.Update(p => p.protocolo == chat.protocolo, chat);
                    }


                    if (atendimento.Cliente == null && chat?.cliente != null)
                    {
                        if (chat?.cliente?.cpf_cnpj != null)
                        {
                            String[] caracteres = new String[]
                                       {
                                        ".", "-", "/"
                                       };

                            foreach (var item in caracteres)
                            {
                                chat.cliente.cpf_cnpj = chat.cliente.cpf_cnpj.Replace(item, "");
                            }
                        }

                        var config = await repoConfiguracoes_Gerais.selectFirst();
                        int tipo_cliente = Convert.ToInt32(config.Pessoa_Contato_Primeira_Vez);

                        var telefoneVerificado = NormalizarNumero(chat.cliente.telefone);

                        var cliente = await GetClienteChatBot(session, telefoneVerificado, chat.cliente.identificadorTelegram);

                        if (cliente == null)
                        {
                            ClienteAniel cliente_aniel = new ClienteAniel
                            {
                                NomeFantasia = chat.cliente.nome,
                                Telefone_Celular = telefoneVerificado,
                                CPF = chat.cliente.cpf_cnpj,
                                Email_Principal = chat.cliente.email,
                                Tipo_Cliente = tipo_cliente == 0 ? Tipo_ClienteAniel.Cliente : Tipo_ClienteAniel.Prospecto,
                                Tipo_Pessoa = chat.cliente.cpf_cnpj != null ? (chat.cliente.cpf_cnpj.Length <= 11 ? Tipo_Pessoa.Fisica : Tipo_Pessoa.Juridica) : (Tipo_Pessoa?)null,
                                Status = Status_Cliente.Ativo,
                                Status_Prospecto = Status_ProspectoAniel.Lead_Qualificado,
                                Cod_Cliente = Convert.ToInt32(repoClienteAniel.Max(p => p.Cod_Cliente).Result) + 1
                            };
                            await repoClienteAniel.Save(cliente_aniel);

                            cliente = new Cliente
                            {
                                NomeFantasia = chat.cliente.nome,
                                Telefone_Celular = telefoneVerificado,
                                CpfCnpj = chat.cliente.cpf_cnpj,
                                Email_Principal = chat.cliente.email,
                                Tipo_Cliente = tipo_cliente == 0 ? Tipo_Cliente.Prospecto : Tipo_Cliente.Cliente,
                                Tipo_Pessoa = chat.cliente.cpf_cnpj != null ? (chat.cliente.cpf_cnpj.Length <= 11 ? Tipo_Pessoa.Fisica : Tipo_Pessoa.Juridica) : (Tipo_Pessoa?)null,
                                Status = Status_Cliente.Ativo,
                                Status_Prospecto = Status_Prospecto.Lead_Qualificado,
                                Identificador_Telegram = chat.cliente.identificadorTelegram,
                                Cod_Cliente = cliente_aniel?.Cod_Cliente
                            };
                        }
                        await repoCliente.SaveOrUpdate(cliente);

                        atendimento.Cliente = cliente;
                    }

                    await repoAtendimento.SaveOrUpdate(atendimento);

                    var logs = atendimento.GeraLog(modeloAnterior, value.usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                    await repositoryLogs.Save(logs);

                    if (chat.bot == true)
                    {
                        chat.bot = false;
                        {
                            await repository.Update(p => p.protocolo == chat.protocolo, chat);
                        }
                    }

                    retorno.IdObjeto = atendimento.Id;
                    retorno.Ok = true;

                }
            }

            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }
            finally
            {
                semaforo.Release();
            }

            return retorno;
        }


        public static string NormalizarNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return null;

            // Remove tudo que não for dígito
            string apenasDigitos = Regex.Replace(numero, @"\D", "");

            // Remove o código do país (55) se existir no início
            if (apenasDigitos.StartsWith("55"))
                apenasDigitos = apenasDigitos.Substring(2);

            return apenasDigitos;
        }


        [HttpPost()]
        [Route("api/atendimentoapi/GravarAcao")]
        public async Task<PostApiRetorno<Atendimento_Acao>> GravarAcao(GravarAcaoAtendimentoModel dados)
        {
            var retorno = new PostApiRetorno<Atendimento_Acao>();
            Atendimento_Acao acao = new Atendimento_Acao();
            retorno.IdObjeto = acao;

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            using (var repository_atendimento = new Conexao<Atendimento>(session))
            using (var repository_acao = new Conexao<Atendimento_Acao>(session))
            using (var repository_usuario = new Conexao<IdentityUser>(session))
            using (var repository_emailAtendimento = new Conexao<Email_Atendimento>(session))
            {
                try
                {
                    Atendimento modelo = await repository_atendimento.selectFirst(p => p.Id == dados.IdAtendimento);
                    List<Atendimento_Acao> acoes = modelo.Ultima_Acao.ToList();

                    if (dados.Id == 0)
                    {
                        acao = new Atendimento_Acao
                        {
                            UsuarioCriacao = dados.Usuario,
                            HTML = System.Uri.UnescapeDataString(dados.DescricaoAcao),
                            Status_Atendimento = modelo.Status_Atendimento,
                            Atendimento = modelo,
                            Responsavel = await repository_usuario.selectFirst(p => p.Id == dados.Usuario),
                            Origem = Origem_AtendimentoAcao.MANUAL,
                            TipoAtividade = dados.TipoAtividade
                        };

                        foreach (var arquivo in (dados.Arquivos ?? new List<ArquivoAcaoModel>()))
                        {
                            acao.Arquivos_AtendimentoAcao.Add(new Arquivo_AtendimentoAcao
                            {
                                Atendimento_Acao = acao,
                                Arquivo = arquivo.nome_Arquivo,
                                Extensao = arquivo.extensao
                            });
                        }

                        await repository_acao.Save(acao);
                        acoes.Add(acao);
                    }
                    else
                    {

                        acao = await repository_acao.selectFirst(p => p.Id == dados.Id);
                        acao.HTML = dados.DescricaoAcao;
                        await repository_acao.SaveOrUpdate(acao);

                    }

                    if (dados.TipoAtividade == TipoAtividadeAtendimento.Publico) //EnviarEmail
                    {
                        await repository_emailAtendimento.Save(new Email_Atendimento { Atendimento_Acao = acao, Enviado = false });
                    }

                    //Envia notificação de ação inserida
                    if (dados.Usuario != modelo.Responsavel.Id)
                    {
                        using (var api = new NotificacoesApiController())
                        {
                            var notificacao = api.EnviaNotificacoesAtendimento(10, modelo, null);
                        }
                    }

                    modelo.Ultima_Acao = acoes;
                    await repository_atendimento.SaveOrUpdate(modelo);
                    transaction.Commit();
                    retorno.Ok = true;

                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                }

            }

            retorno.IdObjeto = acao;
            return retorno;
        }

        [HttpPost()]
        [Route("api/atendimentoapi/GravarAcao_FluxoEtapa")]
        public async Task<PostApiRetorno<Int64>> GravarAcao_FluxoEtapa(GravarAcaoAtendimentoModel dados)
        {
            var retorno = new PostApiRetorno<Int64>();
            Atendimento_Acao acao = new Atendimento_Acao();
            retorno.IdObjeto = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            using (var repository_atendimento = new Conexao<Atendimento>(session))
            using (var repository_acao = new Conexao<Atendimento_Acao>(session))
            using (var repository_usuario = new Conexao<IdentityUser>(session))
            using (var repository_emailAtendimento = new Conexao<Email_Atendimento>(session))
            {
                try
                {
                    Atendimento modelo = await repository_atendimento.selectFirst(p => p.Id == dados.IdAtendimento);
                    List<Atendimento_Acao> acoes = modelo.Ultima_Acao.ToList();

                    if (dados.Id == 0)
                    {
                        acao = new Atendimento_Acao
                        {
                            UsuarioCriacao = dados.Usuario,
                            HTML = System.Uri.UnescapeDataString(dados.DescricaoAcao),
                            Status_Atendimento = modelo.Status_Atendimento,
                            Atendimento = modelo,
                            Responsavel = await repository_usuario.selectFirst(p => p.Id == dados.Usuario),
                            Origem = Origem_AtendimentoAcao.MANUAL,
                            TipoAtividade = dados.TipoAtividade
                        };

                        await repository_acao.Save(acao);
                        acoes.Add(acao);
                    }
                    else
                    {
                        acao = await repository_acao.selectFirst(p => p.Id == dados.Id);
                        acao.HTML = dados.DescricaoAcao;
                        await repository_acao.SaveOrUpdate(acao);
                    }

                    modelo.Ultima_Acao = acoes;
                    await repository_atendimento.SaveOrUpdate(modelo);
                    transaction.Commit();

                    retorno.Ok = true;
                    retorno.IdObjeto = modelo.Id;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Atendimentoapi/HistoricoAlteracao")]
        public async Task<DataTableResponse<LogGenericoModel>> HistoricoAlteracao(HistoricoAlteracao_DataTableRequest request)
        {
            List<LogGenericoModel> retorno = new List<LogGenericoModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();


                sqlFrom.AppendLine("FROM TB_ATENDIMENTO A ");


                sqlFrom.AppendLine("WHERE A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM ");

                parametros.Add("@DATAINI", DateTime.Parse(request.DataIni + " 00:00:00"));
                parametros.Add("@DATAFIM", DateTime.Parse(request.DataFim + " 23:59:59"));

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine(" AND ((UPPER(A.OBJETO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                }

                if (request.Id.HasValue)
                {
                    sqlFrom.AppendLine(" AND A.IDLOG = @ID ");
                    parametros.Add("@ID", request.Id);
                }

                if (request.TipoOperacao != null)
                {
                    sqlFrom.AppendLine(" AND A.TIPOOPERACAO = @TIPOOPERACAO ");
                    parametros.Add("@TIPOOPERACAO", Convert.ToInt32(request.TipoOperacao));
                }

                if (!String.IsNullOrEmpty(request.Objeto))
                {
                    sqlFrom.AppendLine(" AND A.OBJETO = @OBJETO ");
                    parametros.Add("@OBJETO", request.Objeto);
                }

                if (!String.IsNullOrEmpty(request.Campo))
                {
                    sqlFrom.AppendLine(" AND A.CAMPO = @CAMPO ");
                    parametros.Add("@CAMPO", request.Campo);
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO ");

                    sql.Append(sqlFrom.ToString());

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

                    sql.AppendLine("SELECT A.ID, A.NOMEUSUARIOCRIACAO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.IDLOG, A.OBJETO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 0:     // CHAVE
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY OBJETO ");
                                    else
                                        sql.Append(" ORDER BY OBJETO DESC ");
                                    break;
                                }
                            case 1:     // CAMPO
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY CAMPO ");
                                    else
                                        sql.Append(" ORDER BY CAMPO DESC ");
                                    break;
                                }
                            case 6: //Data Hora
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY DATAHORACRIACAO ");
                                    else
                                        sql.Append(" ORDER BY DATAHORACRIACAO DESC ");
                                    break;
                                }
                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogGenericoModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Chave = Convert.ToString(reader["OBJETO"]),
                                Campo = Convert.ToString(reader["CAMPO"]),
                                Campo_ValorAntes = Convert.ToString(reader["CAMPO_VALORANTES"]),
                                Campo_ValorDepois = Convert.ToString(reader["CAMPO_VALORDEPOIS"]),
                                TipoOperacao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Nome_usuario = Convert.ToString(reader["NOMEUSUARIOCRIACAO"]),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<LogGenericoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Atendimentoapi/GetOrdensServicoAtendimento/{id}/{status_ordem_servico}")]
        public async Task<DataTableResponse<OrdensServicoAtendimentoResponse>> Ordens_Servico(long id, string status_ordem_servico)
        {
            var retorno = new List<OrdensServicoAtendimentoResponse>();

            var status = status_ordem_servico == "STATUS_EXECUCAO_ORDEM_SERVICO" ? "AND STATUS_EXECUCAO NOT IN (1, 2, 9)" : String.Empty;
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@$"SELECT NUM_OBRA, NUM_DOC, PROJETO, CODCT, STATUS_EXECUCAO, NUM_OBRA_ORIGINAL FROM TB_DOCUMENTO_PRODUCAO 
                             WHERE ID_ATENDIMENTO_CONNECT = @id {status}");

            Dictionary<string, object> parametros = new Dictionary<string, object>()
            { {"id",id} };

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
            {
                while (await reader.ReadAsync())
                {
                    retorno.Add(new OrdensServicoAtendimentoResponse
                    {
                        numObraOriginal = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                        numObra = Convert.ToString(reader["NUM_OBRA"]),
                        numDoc = Convert.ToString(reader["NUM_DOC"]),
                        projeto = Convert.ToString(reader["PROJETO"]),
                        codCt = Convert.ToString(reader["CODCT"]),
                        status = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"]))
                    });
                }
            }

            return new DataTableResponse<OrdensServicoAtendimentoResponse> { draw = 1, recordsFiltered = retorno.Count, data = retorno };
        }

        [HttpPost()]
        [Route("api/Atendimentoapi/Ordens_Servico")]
        public async Task<DataTableResponse<OrdensServicoResponse>> Ordens_Servico(OrdensServico_DataTableRequest request)
        {
            List<OrdensServicoResponse> retorno = new List<OrdensServicoResponse>();

            Int32 recordsTotal = 0;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoClienteAniel = new Conexao<ClienteAniel>(sessionT5))
            using (ISession sessionConnect = HibernateUtil.GetSession())
            using (Documento_ProducaoApiController documentoApi = new Documento_ProducaoApiController())
            using (var repoCliPlaPro = new Conexao<Cliente_PlanoProduto>(sessionConnect))
            {
                try
                {
                    #region [Objetos]
                    if (request.Id_Cliente == null)
                    {
                        throw new Exception("O titular não está informado no Chat");
                    }

                    var objCliente = await repoCliente.selectFirst(x => x.Id == request.Id_Cliente);
                    if (String.IsNullOrEmpty(objCliente.CpfCnpj) && objCliente?.Cod_Cliente == null)
                    {
                        throw new Exception("O titular não possui CPF ou CNPJ cadastrado.");
                    }

                    var objClienteAniel = await repoClienteAniel.selectFirst(x => (x.CPF != null && x.CPF != String.Empty && x.CPF == objCliente.CpfCnpj) || x.Cod_Cliente == objCliente.Cod_Cliente);
                    #endregion

                    #region [Filtros]

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder();

                    sqlFrom.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A ");
                    sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE B ON (A.EQUIPE = B.EQUIPE) ");
                    sqlFrom.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE C ON (A.COD_TIPO_SERV = C.COD_TIPO_SERV)  ");
                    sqlFrom.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO D ON (A.CODCT = D.CODCT AND A.COD_ENCERRAMENTO = D.COD_ENCERRAMENTO) ");
                    sqlFrom.AppendLine("WHERE A.ID_CLIENTE = @ID_CLIENTE AND A.STATUS_EXECUCAO <> 9 ");
                    parametros.Add("@ID_CLIENTE", objClienteAniel.Cod_Cliente);

                    if (request.Id_ClientePlanoProduto.HasValue)
                    {
                        sqlFrom.AppendLine("AND A.ID_CLIENTE_PLANOPRODUTOCONNECT = @ID_CLIENTE_PLANOPRODUTOCONNECT");
                        parametros.Add("@ID_CLIENTE_PLANOPRODUTOCONNECT", request.Id_ClientePlanoProduto);
                    }

                    if (request.Status.HasValue)
                    {
                        sqlFrom.AppendLine("AND A.STATUS_EXECUCAO = @STATUS");
                        parametros.Add("@STATUS", request.Status);
                    }

                    if (String.IsNullOrEmpty(request.DataInicial) == false && String.IsNullOrEmpty(request.DataFinal) == false)
                    {
                        sqlFrom.AppendLine("AND (A.DIA BETWEEN @INICIO AND @FIM)");
                        parametros.Add("@INICIO", request.DataInicial);
                        parametros.Add("@FIM", request.DataFinal);
                    }
                    #endregion

                    #region [Select]
                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(@"SELECT A.NUM_OBRA, A.STATUS_EXECUCAO,A.EQUIPE,A.COD_TIPO_SERV,A.DATA_MAXIMA, A.HORA_MAXIMA, A.DIA , A.HORA , 
                                     A.DATA_PROD,A.HORA_PROD, A.NUM_OBRA, A.DATA_INDICADOR,A.HORA_INDICADOR,C.DESCRICAO AS TIPO_SERVICO,B.NOME AS TECNICO,
                                     
                                     A.NUM_DOC, A.CODCT, A.PROJETO, NUM_OBRA_ORIGINAL, A.COD_ENCERRAMENTO,D.DESCRICAO,A.ENDERECO, A.ENDERECO_CIDADE, A.ENDERECO_BAIRRO, A.ENDERECO_NUMERO, A.ENDERECO_UF, A.ID_CLIENTE_PLANOPRODUTOCONNECT ");
                        sql.AppendLine(sqlFrom.ToString());

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {


                                DateTime? dataAgendamento = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]) : (DateTime?)null;
                                DateTime? dataCriacao = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null;
                                DateTime? dataEncerramento = reader["DATA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_PROD"]) : (DateTime?)null;
                                DateTime? dataIndicador = reader["DATA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INDICADOR"]) : (DateTime?)null;
                                TimeSpan? horaAgendamento = reader["HORA_MAXIMA"] != DBNull.Value ? (TimeSpan?)reader["HORA_MAXIMA"] : (TimeSpan?)null;
                                TimeSpan? horaCriacao = reader["HORA"] != DBNull.Value ? (TimeSpan?)reader["HORA"] : (TimeSpan?)null;
                                TimeSpan? horaEncerramento = reader["HORA_PROD"] != DBNull.Value ? (TimeSpan?)reader["HORA_PROD"] : (TimeSpan?)null;
                                TimeSpan? horaIndicador = reader["HORA_INDICADOR"] != DBNull.Value ? (TimeSpan?)reader["HORA_INDICADOR"] : (TimeSpan?)null;

                                if (horaAgendamento.HasValue && dataAgendamento.HasValue)
                                {
                                    dataAgendamento = dataAgendamento.Value.Add(horaAgendamento.Value);
                                }
                                if (horaCriacao.HasValue && dataCriacao.HasValue)
                                {
                                    dataCriacao = dataCriacao.Value.Add(horaCriacao.Value);
                                }
                                if (horaEncerramento.HasValue && dataEncerramento.HasValue)
                                {
                                    dataEncerramento = dataEncerramento.Value.Add(horaEncerramento.Value);
                                }
                                if (horaIndicador.HasValue && dataIndicador.HasValue)
                                {
                                    dataIndicador = dataIndicador.Value.Add(horaIndicador.Value);
                                }

                                string enderecoInstalacao = "";

                                string endereco = Convert.ToString(reader["ENDERECO"]);
                                string numero = Convert.ToString(reader["ENDERECO_NUMERO"]);
                                string bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]);
                                string cidade = Convert.ToString(reader["ENDERECO_CIDADE"]);
                                string uf = Convert.ToString(reader["ENDERECO_UF"]);
                                string idPlanoProduto = Convert.ToString(reader["ID_CLIENTE_PLANOPRODUTOCONNECT"]);

                                if (!string.IsNullOrEmpty(idPlanoProduto) && string.IsNullOrEmpty(cidade))
                                {
                                    Cliente_PlanoProduto planoProduto = await repoCliPlaPro.selectFirst(p => p.Id == Convert.ToInt32(idPlanoProduto));
                                    cidade = planoProduto.Endereco_Instalacao.Cidade;
                                }

                                var parts = new List<string>();

                                if (!string.IsNullOrEmpty(endereco)) parts.Add(endereco);
                                if (!string.IsNullOrEmpty(numero)) parts.Add(", " + numero + " - ");
                                if (!string.IsNullOrEmpty(bairro)) parts.Add(bairro + ", ");
                                if (!string.IsNullOrEmpty(cidade)) parts.Add(cidade + " - ");
                                if (!string.IsNullOrEmpty(uf)) parts.Add(uf);

                                enderecoInstalacao = string.Join("", parts).TrimEnd(' ', '-').TrimEnd(',', ' ');


                                retorno.Add(new OrdensServicoResponse
                                {
                                    Codct = Convert.ToString(reader["CODCT"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Num_Doc = Convert.ToString(reader["NUM_DOC"]),
                                    Numero_Obra_Original = reader["NUM_OBRA_ORIGINAL"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA_ORIGINAL"]) : Convert.ToString(reader["NUM_OBRA"]),
                                    dataAgendamento = dataAgendamento.HasValue ? dataAgendamento.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                    dataHoraCriacao = dataCriacao.HasValue ? dataCriacao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                    dataHoraEncerramento = dataEncerramento.HasValue ? dataEncerramento.Value.ToString("dd/MM/yyyy HH:mm:ss") : String.Empty,
                                    numObra = Convert.ToString(reader["NUM_OBRA"]),
                                    sla = documentoApi.GetDescricaoSla(dataIndicador, dataCriacao, dataEncerramento),
                                    status = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"])),
                                    tecnico = Convert.ToString(reader["TECNICO"]),
                                    enderecoInstalacao = enderecoInstalacao,
                                    tipoServico = Convert.ToString(reader["TIPO_SERVICO"]),
                                    codEncerramento = Convert.ToString(reader["DESCRICAO"])

                                });


                            }
                        }




                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    return new DataTableResponse<OrdensServicoResponse>
                    {
                        draw = request.draw,
                        recordsFiltered = 0,
                        data = new List<OrdensServicoResponse>(),
                        error = ex.Message
                    };

                }

                //Page e retornando DataTable
                return new DataTableResponse<OrdensServicoResponse>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };


            }


        }

        [HttpGet()]
        [Route("api/Atendimentoapi/GetOrdensServicos_TrocaEndereco")]
        public async Task<List<OrdensServicoResponse>> GetOrdensServicos_TrocaEndereco(Int64 IdCliente, Int64 IdClientePlanoProduto)
        {
            var retorno = new List<OrdensServicoResponse>();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoDocumento_ProducaoAniel = new Conexao<Documento_ProducaoAniel>(sessionT5))
                {
                    var listaAtendimento = await repoAtendimento.select(x => x.PlanoProduto.Id == IdClientePlanoProduto && x.Endereco_Mudanca != null && x.Cliente.Id == IdCliente);
                    var listaCliente_PlanoProduto = await repoCliente_PlanoProduto.select(x => x.Id == IdClientePlanoProduto && x.Endereco_Mudanca_Cliente != null && x.Cliente.Id == IdCliente);

                    if (listaAtendimento.Count > 0)
                    {
                        listaAtendimento.ToList().ForEach(async x =>
                        {
                            var objOS = await repoDocumento_ProducaoAniel.selectFirst(p => p.Id_Atendimento == x.Id);
                            if (objOS != null &&
                                objOS.Status != StatusExecucaoAniel.Cancelado &&
                                objOS.Status != StatusExecucaoAniel.Fechada_Produtiva &&
                                objOS.Status != StatusExecucaoAniel.Fechada_Produtiva)
                            {
                                retorno.Add(new OrdensServicoResponse
                                {
                                    numObra = objOS.Num_Obra,
                                    Codct = objOS.CodCt,
                                    Projeto = objOS.Projeto,
                                    Num_Doc = Convert.ToString(objOS.Num_Doc),
                                    Numero_Obra_Original = objOS.Numero_Obra_Original,
                                    dataAgendamento = Convert.ToString(objOS.DiaHora_Agendamento),
                                    status = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(objOS?.Status)),
                                    dataHoraEncerramento = Convert.ToString(objOS.Data_Encerramento)
                                });
                            }
                        });
                    }
                    else if (listaCliente_PlanoProduto.Count > 0)
                    {
                        listaCliente_PlanoProduto.ToList().ForEach(async x =>
                        {
                            var objOS = await repoDocumento_ProducaoAniel.selectFirst(p => p.Id_Cliente_PlanoProdutoConnect == x.Id);
                            if (objOS != null &&
                                objOS.Status != StatusExecucaoAniel.Cancelado &&
                                objOS.Status != StatusExecucaoAniel.Fechada_Produtiva &&
                                objOS.Status != StatusExecucaoAniel.Fechada_Produtiva)
                            {
                                retorno.Add(new OrdensServicoResponse
                                {
                                    numObra = objOS.Num_Obra,
                                    Codct = objOS.CodCt,
                                    Projeto = objOS.Projeto,
                                    Num_Doc = Convert.ToString(objOS.Num_Doc),
                                    Numero_Obra_Original = objOS.Numero_Obra_Original,
                                    dataAgendamento = Convert.ToString(objOS.DiaHora_Agendamento),
                                    status = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(objOS?.Status)),
                                    dataHoraEncerramento = Convert.ToString(objOS.Data_Encerramento)
                                });
                            }
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Atendimentoapi/GetAvisoImportante_TrocaEndereco")]
        public async Task<List<Atendimento_OrdemServicoResponse>> GetAvisoImportante_TrocaEndereco(Int64 IdCliente, Int64? IdClientePlanoProduto, Int64 IdAtendimento)
        {
            var retorno = new List<Atendimento_OrdemServicoResponse>();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoClienteAniel = new Conexao<ClienteAniel>(sessionT5))
                using (var repoDocumento_ProducaoAniel = new Conexao<Documento_ProducaoAniel>(sessionT5))
                {
                    var objCliente = await repoCliente.selectFirst(x => x.Id == IdCliente);
                    var objClienteAniel = await repoClienteAniel.selectFirst(x => x.CPF == objCliente.CpfCnpj);


                    var listaAtendimentos = new List<Atendimento>();
                    var listaClientePP = new List<Cliente_PlanoProduto>();
                    var objClientePP = new Cliente_PlanoProduto();

                    if (IdClientePlanoProduto != null)
                    {
                        listaAtendimentos = (await repoAtendimento.select(x => x.PlanoProduto.Id == IdClientePlanoProduto && x.Endereco_Mudanca != null)).ToList();
                        listaClientePP = (await repoCliente_PlanoProduto.select(x => IdClientePlanoProduto != null && x.Id == IdClientePlanoProduto && x.Endereco_Mudanca_Cliente != null)).ToList();
                        objClientePP = await repoCliente_PlanoProduto.selectFirst(x => IdClientePlanoProduto != null && x.Id == IdClientePlanoProduto);
                    }

                    var listaOS = new List<OrdensServicoResponse>();
                    if (listaAtendimentos.Count > 0)
                    {
                        foreach (var atendimento in listaAtendimentos)
                        {
                            var listaOrdemServico = await repoDocumento_ProducaoAniel.select(x => x.Id_Atendimento == atendimento.Id);
                            var ordemServico = listaOrdemServico.OrderByDescending(x => x.Dia).ThenBy(x => x.Hora).OrderByDescending(x => x.Hora).FirstOrDefault();
                            if (ordemServico != null && ordemServico.Status != StatusExecucaoAniel.Fechada_Produtiva && ordemServico.Status != StatusExecucaoAniel.Fechada_Improdutiva
                                && ordemServico.Status != StatusExecucaoAniel.Cancelado)
                            {
                                listaOS.Add(new OrdensServicoResponse
                                {
                                    numObra = ordemServico.Num_Obra,
                                    Codct = ordemServico.CodCt,
                                    Projeto = ordemServico.Projeto,
                                    Num_Doc = Convert.ToString(ordemServico.Num_Doc),
                                    Numero_Obra_Original = ordemServico.Numero_Obra_Original,
                                    dataAgendamento = Convert.ToString(ordemServico.DiaHora_Agendamento),
                                    status = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(ordemServico?.Status)),
                                    nrStatus = Convert.ToInt32(ordemServico?.Status),
                                    dataHoraEncerramento = Convert.ToString(ordemServico.Data_Encerramento)
                                });
                            }
                        }
                    }
                    else if (listaClientePP.Count > 0)
                    {
                        foreach (var clientePP in listaClientePP)
                        {
                            var listaOrdemServico = await repoDocumento_ProducaoAniel.select(x => x.Id_Cliente_PlanoProdutoConnect == clientePP.Id);
                            var ordemServico = listaOrdemServico.OrderByDescending(x => x.Dia).ThenBy(x => x.Hora).OrderByDescending(x => x.Hora).FirstOrDefault();
                            if (ordemServico != null && ordemServico.Status != StatusExecucaoAniel.Fechada_Produtiva && ordemServico.Status != StatusExecucaoAniel.Fechada_Improdutiva
                                && ordemServico.Status != StatusExecucaoAniel.Cancelado)
                            {
                                listaOS.Add(new OrdensServicoResponse
                                {
                                    numObra = ordemServico.Num_Obra,
                                    Codct = ordemServico.CodCt,
                                    Projeto = ordemServico.Projeto,
                                    Num_Doc = Convert.ToString(ordemServico.Num_Doc),
                                    Numero_Obra_Original = ordemServico.Numero_Obra_Original,
                                    dataAgendamento = Convert.ToString(ordemServico.DiaHora_Agendamento),
                                    status = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(ordemServico?.Status)),
                                    nrStatus = Convert.ToInt32(ordemServico?.Status),
                                    dataHoraEncerramento = Convert.ToString(ordemServico.Data_Encerramento)
                                });
                            }
                        }
                    }

                    retorno.Add(new Atendimento_OrdemServicoResponse
                    {
                        IdPlano = Convert.ToInt64(objClientePP?.Plano?.Id),
                        PlanoComOs = Convert.ToBoolean(objClientePP?.Plano?.OSTrocaEndereco),
                        ExistePendencia = listaAtendimentos != null && listaAtendimentos.Count > 0 || listaAtendimentos != null && listaClientePP.Count > 0,
                        ListaOs = listaOS
                    });
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Atendimentoapi/HistoricoChamadas")]
        public async Task<DataTableResponse<Relatorio_Historico_ChamadaModel>> HistoricoChamadas(Relatorio_Historico_ChamadaRequest request)
        {

            List<Relatorio_Historico_ChamadaModel> retorno = new List<Relatorio_Historico_ChamadaModel>();
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<CircuitoTelefonia>(session))
            using (Relatorio_Historico_ChamadaApiController historico_ChamadaApiController = new Relatorio_Historico_ChamadaApiController())
            {
                var circuito = await repository.selectFirst(p => p.Clientes_PlanosProdutos.Id == request.idClientePlanoProduto);

                if (circuito != null)
                {
                    foreach (var item in circuito.Did)
                    {
                        var resultado = await historico_ChamadaApiController.Relatorio_Historico_Chamada(new Relatorio_Historico_ChamadaRequest
                        {
                            dataini = request.dataini,
                            datafim = request.datafim,
                            id_did = item.CodigoAuxiliar
                        });

                        retorno.AddRange(resultado.data);
                    }
                }


            }

            //Page e retornando DataTable
            return new DataTableResponse<Relatorio_Historico_ChamadaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count,
                data = retorno,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Atendimentoapi/HistoricoAtendimento")]
        public async Task<DataTableResponse<HistoricoAtendimentoResponse>> HistoricoAtendimento(HistoricoAtendimento_DataTableRequest request)
        {
            List<HistoricoAtendimentoResponse> retorno = new List<HistoricoAtendimentoResponse>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (Documento_ProducaoApiController documentoApi = new Documento_ProducaoApiController())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();


                sqlFrom.AppendLine("FROM TB_ATENDIMENTO A ");
                sqlFrom.AppendLine("LEFT JOIN ASPNETUSERS B ON (A.ID_RESPONSAVEL = B.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_TIPO_ATENDIMENTO D ");
                sqlFrom.AppendLine(" ON (A.ID_TIPO_ATENDIMENTO = D.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_MOTIVO_STATUS_ATENDIMENTO E ");
                sqlFrom.AppendLine(" ON (A.ID_MOTIVO_STATUS = E.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_FLUXO_ATENDIMENTO F ");
                sqlFrom.AppendLine(" ON (F.ID_TIPO_ATENDIMENTO = D.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_VENDA G ");
                sqlFrom.AppendLine(" ON G.ID_CLIENTE =A.ID_CLIENTE ");
                sqlFrom.AppendLine(" LEFT JOIN TB_FUNCIONARIO H ");
                sqlFrom.AppendLine(" ON H.ID =G.ID_VENDEDOR ");
                sqlFrom.AppendLine(" LEFT JOIN TB_FUNCIONARIO I ");
                sqlFrom.AppendLine(" ON I.ID =G.ID_SUPERVISOR  ");
                sqlFrom.AppendLine(" LEFT JOIN TB_ORIGEM_ATENDIMENTO J ");
                sqlFrom.AppendLine(" ON J.ID = A.ID_ORIGEM_ATENDIMENTO  ");

                sqlFrom.AppendLine(" WHERE A.ID_CLIENTE = @ID_CLIENTE AND A.STATUS_ATENDIMENTO <> 3 ");
                parametros.Add("@ID_CLIENTE", request.Id_Cliente);


                try
                {
                    DateTime dataInicial = Convert.ToDateTime(request.DataInicial);
                    DateTime dataFinal = Convert.ToDateTime(request.DataFinal);

                    if ((dataFinal - dataInicial).TotalDays > 90)
                    {
                        throw new Exception("Período maior que 90 dias");
                    }
                    sqlFrom.AppendLine(" AND (A.DATAHORACRIACAO BETWEEN @DATA_INICIAL AND @DATA_FINAL) ");

                    request.DataInicial += " 00:00:00";
                    request.DataFinal += " 23:59:59";
                    parametros.Add("@DATA_INICIAL", request.DataInicial);
                    parametros.Add("@DATA_FINAL", request.DataFinal);

                }
                catch
                {
                    throw new Exception("Periodo Inválido");
                }

                if (request.Id_TipoAtendimento.HasValue)
                {
                    sqlFrom.AppendLine("AND D.ID = @ID_TIPO_ATENDIMENTO");
                    parametros.Add("@ID_TIPO_ATENDIMENTO", request.Id_TipoAtendimento);
                }

                if (string.IsNullOrEmpty(request.StatusAtendimento) == false)
                {
                    sqlFrom.AppendLine("AND A.STATUS_ATENDIMENTO = @STATUS_ATENDIMENTO");
                    parametros.Add("@STATUS_ATENDIMENTO", request.StatusAtendimento);
                }

                if (string.IsNullOrEmpty(request.StatusVenda) == false)
                {
                    sqlFrom.AppendLine("AND G.STATUS = @STATUS");
                    parametros.Add("@STATUS", request.StatusVenda);
                }

                if (string.IsNullOrEmpty(request.TermoAdesao) == false)
                {
                    sqlFrom.AppendLine("AND G.TERMO_ADESAO_ASSINADO = @TERMO_ADESAO_ASSINADO");
                    parametros.Add("@TERMO_ADESAO_ASSINADO", request.TermoAdesao);
                }



                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(@"SELECT A.ID,A.NUMERO,B.USERNAME,B.NOME,A.ASSUNTO,A.DATAHORACRIACAO,A.STATUS_ATENDIMENTO,A.DATA_HORA_FECHAMENTO,D.NOME AS TIPO_ATENDIMENTO,E.DESCRICAO AS MOTIVO_STATUS,G.ID AS VENDA,G.STATUS AS STATUSVENDA, G.TERMO_ADESAO_ASSINADO, H.NOME AS VENDEDOR, I.NOME AS SUPERVISOR, J.DESCRICAO AS ORIGEM_ATT ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine(@"GROUP BY A.ID, A.NUMERO, B.USERNAME, B.NOME, A.ASSUNTO, A.DATAHORACRIACAO, A.STATUS_ATENDIMENTO, A.DATA_HORA_FECHAMENTO, D.NOME, E.DESCRICAO, G.ID , G.STATUS, G.TERMO_ADESAO_ASSINADO, H.NOME, I.NOME, J.DESCRICAO ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime? dataAbertura = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                            DateTime? dataFechamento = reader["DATA_HORA_FECHAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_FECHAMENTO"]) : (DateTime?)null;

                            var desc_user_name = reader["USERNAME"] != DBNull.Value ? Convert.ToString(reader["USERNAME"]) : String.Empty;
                            desc_user_name = !String.IsNullOrEmpty(desc_user_name) ? reader["NOME"] != DBNull.Value ? String.Concat(desc_user_name, " - ", Convert.ToString(reader["NOME"])) : String.Empty : reader["NOME"] != DBNull.Value ? String.Concat(Convert.ToString(reader["NOME"])) : String.Empty;
                            var protocolo_numero = reader["NUMERO"] != DBNull.Value ? Convert.ToString(reader["NUMERO"]) : String.Empty;
                            retorno.Add(new HistoricoAtendimentoResponse
                            {
                                dataHoraEncerramento = dataFechamento.HasValue ? dataFechamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : String.Empty,
                                dataHoraCriacao = dataAbertura.HasValue ? dataAbertura.Value.ToString("dd/MM/yyyy HH:mm:ss") : String.Empty,
                                protocolo = protocolo_numero,
                                tipoAtendimento = reader["TIPO_ATENDIMENTO"] != DBNull.Value ? Convert.ToString(reader["TIPO_ATENDIMENTO"]) : String.Empty,
                                motivoStatus = reader["MOTIVO_STATUS"] != DBNull.Value ? Convert.ToString(reader["MOTIVO_STATUS"]) : String.Empty,
                                assunto = reader["ASSUNTO"] != DBNull.Value ? Convert.ToString(reader["ASSUNTO"]) : String.Empty,
                                numero = protocolo_numero,
                                id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : (Int64?)null,
                                responsavel = desc_user_name,
                                status = reader["STATUS_ATENDIMENTO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Atendimento>(Convert.ToString(reader["STATUS_ATENDIMENTO"])) : String.Empty,
                                venda = reader["VENDA"] != DBNull.Value ? Convert.ToInt64(reader["VENDA"]) : (Int64?)0,
                                vendedor = reader["VENDEDOR"] != DBNull.Value ? Convert.ToString(reader["VENDEDOR"]) : String.Empty,
                                origemAtendimento = reader["ORIGEM_ATT"] != DBNull.Value ? Convert.ToString(reader["ORIGEM_ATT"]) : "-",
                                supervisor = reader["SUPERVISOR"] != DBNull.Value ? Convert.ToString(reader["SUPERVISOR"]) : String.Empty,
                                status_venda = reader["STATUSVENDA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(reader["STATUSVENDA"])) : String.Empty,
                                termo_assinado = reader["TERMO_ADESAO_ASSINADO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Aniel.T5.Model.enumerado.Padrao>(Convert.ToString(reader["TERMO_ADESAO_ASSINADO"])) : String.Empty,
                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<HistoricoAtendimentoResponse>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count(),
                data = retorno,
                error = ""
            };

        }

        public async Task<List<AcompanhamentoAtendimentoModel>> getAcompanhamentoAtendimento(List<String> protocolos, String Usuario, bool verificarSetor)
        {
            List<AcompanhamentoAtendimentoModel> retorno = new List<AcompanhamentoAtendimentoModel>();
            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Tipo_Atendimento> repoTipoAtendimento = new Conexao<Tipo_Atendimento>(session))
            using (Conexao<Origem_Atendimento> repoOrigemAtendimento = new Conexao<Origem_Atendimento>(session))
            using (Conexao<Atendimento> repoAtendimento = new Conexao<Atendimento>(session))
            using (Conexao<IdentityUser> repoUsuario = new Conexao<IdentityUser>(session))
            using (Conexao<Bot> repoBot = new Conexao<Bot>(session))
            using (Conexao<Canal_Comunicacao> repoCanal = new Conexao<Canal_Comunicacao>(session))
            {
                var usuario = await repoUsuario.selectFirst(p => p.Id == Usuario);
                foreach (var item in protocolos)
                {

                    try
                    {
                        var atendimento = await repoAtendimento.selectFirst(p => p.Numero == item);

                        if (atendimento != null)
                        {
                            var resultado = await GetViewObj(atendimento, session);

                            if (verificarSetor)
                            {
                                if (resultado.Id_Setor.HasValue && usuario != null)
                                {
                                    if (resultado.Id_Setor != usuario?.Departamento.Id)
                                        continue;
                                }
                            }

                            bool bot = false;
                            var nomeBot = "";
                            var origem = "";
                            AtendimentoChat chat = ChatHub.Atendimentos[item];

                            origem = atendimento?.Origem_Atendimento?.Descricao;
                            string cliente = atendimento?.Cliente?.NomeFantasia;
                            if (String.IsNullOrEmpty(cliente))
                            {
                                cliente = "Não identificado";
                            }

                            if (chat != null)
                            {
                                if (chat.bot == true && chat.idCanal != null)
                                {
                                    Canal_Comunicacao canal = await repoCanal.selectFirst(p => p.Id == chat.idCanal);
                                    Bot botDados = repoBot.selectFirst(p => p.Id == Convert.ToInt64(canal.Bot.Id)).Result;
                                    bot = true;
                                    nomeBot = botDados.Nome;
                                }
                                else if (chat.bot == true)
                                {
                                    var agenteApi = new AgenteApiController();
                                    Bot botOla = agenteApi.getBotAgenteCliente(chat.cliente, session).Result;
                                    bot = true;
                                    nomeBot = botOla.Nome;
                                }
                            }

                            retorno.Add(new AcompanhamentoAtendimentoModel
                            {
                                protocolo = item,
                                cliente = cliente,
                                categoriaCliente = atendimento?.Categoria_Atendimento?.Descricao,
                                origem = origem,
                                sla = resultado.Sla,
                                tipoAtendimento = atendimento?.Tipo_Atendimento?.Nome,
                                tempoEspera = Math.Truncate((DateTime.Now - atendimento.DataHoraCriacao.Value).TotalMinutes).ToString(),
                                bot = bot,
                                nomeBot = nomeBot,
                                etapa_Atual = atendimento.EtapaFluxo?.Descricao,
                                data_Sla_Etapa = atendimento?.DataIndicador_EtapaFluxo != null ? atendimento?.DataIndicador_EtapaFluxo.Value.ToString("dd/MM/yyyy HH:mm") : null,
                                tempo_Gasto = (bool)(atendimento?.DataInicio_EtapaFluxo.HasValue) ? StringService.HoraParaString(DateTime.Now.Subtract((DateTime)(atendimento?.DataInicio_EtapaFluxo.Value)).TotalHours) : null,
                                organizacaoCliente = atendimento?.Cliente?.Organizacao?.Nome_Fantasia
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }

                }

            }

            return retorno;
        }


        [Route("api/Atendimentoapi/UnificarAtendimento")]
        public async Task<PostApiRetorno> UnificarAtendimento(AtendimentoUnificarRequest request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (ITransaction transaction = session.BeginTransaction())
                using (ITransaction transactionLog = sessionLog.BeginTransaction())
                using (Conexao<Atendimento> atendimentoRepo = new Conexao<Atendimento>(session))
                using (Conexao<Atendimento_Acao> atendimentoAcaoRepo = new Conexao<Atendimento_Acao>(session))
                using (Conexao<Arquivo_AtendimentoAcao> arquivoAcaoRepo = new Conexao<Arquivo_AtendimentoAcao>(session))
                using (Conexao<LogAtendimento> logAtendimentoRepo = new Conexao<LogAtendimento>(sessionLog))
                using (Conexao<Email_Atendimento> emailAtendimentoRepo = new Conexao<Email_Atendimento>(session))
                {

                    List<Atendimento> Atendimentos = (await atendimentoRepo.select(p => p.Id.IsIn(request.ids))).ToList();
                    Atendimento atendimentoMestre = Atendimentos.OrderBy(p => p.DataHoraCriacao).FirstOrDefault();
                    List<Atendimento> atendimentosMesclar = Atendimentos.FindAll(p => p.Id != atendimentoMestre.Id);


                    var acoes = await atendimentoAcaoRepo.select(p => p.Atendimento.Id.IsIn(atendimentosMesclar.Select(y => y.Id).ToList()));
                    var logs = await logAtendimentoRepo.select(p => p.IdLog.IsIn(atendimentosMesclar.Select(y => y.Id).ToList()));
                    var emails = await emailAtendimentoRepo.select(p => p.Atendimento.Id.IsIn(atendimentosMesclar.Select(y => y.Id).ToList()));

                    await atendimentoAcaoRepo.SaveOrUpdate(acoes.Select(p => { p.Atendimento = atendimentoMestre; return p; }).ToList());
                    await logAtendimentoRepo.SaveOrUpdate(logs.Select(p => { p.IdLog = atendimentoMestre.Id; return p; }).ToList());
                    await emailAtendimentoRepo.SaveOrUpdate(emails.Select(p => { p.Atendimento = atendimentoMestre; return p; }).ToList());

                    await atendimentoRepo.Remove(atendimentosMesclar);

                    transaction.Commit();
                    transactionLog.Commit();
                    retorno.Ok = true;

                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Atendimentoapi/CriarConversation")]
        public GetApiRetorno CriarConversation()
        {
            string accountSid = "ACecd1718d1b921ec7526f9266538fc924";
            string authToken = "ceab51d98b24c5bd703459c2d4d3b93a";

            Twilio.TwilioClient.Init(accountSid, authToken);

            var conversation = ConversationResource.Create(
                friendlyName: "SMS-to-WhatsApp Example"
            );

            return new GetApiRetorno
            {
                Mensagem = conversation.Sid,
                Ok = true
            };

        }

        [HttpGet()]
        [Route("api/Atendimentoapi/AdicionarParticipante")]
        public GetApiRetorno AdicionarParticipante(string chaveConversa)
        {
            try
            {
                string accountSid = "ACecd1718d1b921ec7526f9266538fc924";
                string authToken = "ceab51d98b24c5bd703459c2d4d3b93a";

                TwilioClient.Init(accountSid, authToken);

                var participant = ParticipantResource.Create(
                    messagingBindingAddress: "+5521987548668",
                    messagingBindingProxyAddress: "+14053698658",
                    pathConversationSid: chaveConversa
                );

                Console.WriteLine(participant.Sid);

                return new GetApiRetorno
                {
                    Mensagem = participant.Sid,
                    Ok = true
                };
            }
            catch (Exception ex)
            {
                return new GetApiRetorno
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    InnerException = ex?.InnerException?.Message
                };
            }

        }

        [HttpGet()]
        [Route("api/Atendimentoapi/AdicionarMensagem")]
        public GetApiRetorno AdicionarMensagem(string chaveConversa)
        {
            try
            {
                // Find your Account Sid and Token at twilio.com/console
                // and set the environment variables. See http://twil.io/secure
                string accountSid = "ACecd1718d1b921ec7526f9266538fc924";
                string authToken = "ceab51d98b24c5bd703459c2d4d3b93a";

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    author: "whatsapp:+14053698658",
                    //body: "Ahoy there!",
                    pathConversationSid: chaveConversa,
                    mediaSid: "ME18743d95b55e5e89bb1ea7b327d88613"

                );

                Console.WriteLine(message.Sid);



                return new GetApiRetorno
                {
                    Mensagem = message.Sid,
                    Ok = true
                };
            }
            catch (Exception ex)
            {
                return new GetApiRetorno
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    InnerException = ex?.InnerException?.Message
                };
            }

        }


        [HttpGet()]
        [Route("api/Atendimentoapi/AdicionarAnexo")]
        public GetApiRetorno AdicionarAnexo(string chaveConversa, string MediaSID)
        {
            try
            {
                // Find your Account Sid and Token at twilio.com/console
                // and set the environment variables. See http://twil.io/secure
                string accountSid = "ACecd1718d1b921ec7526f9266538fc924";
                string authToken = "ceab51d98b24c5bd703459c2d4d3b93a";

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    author: "whatsapp:+14053698658",
                    pathConversationSid: chaveConversa,
                    mediaSid: MediaSID
                );

                Console.WriteLine(message.Sid);



                return new GetApiRetorno
                {
                    Mensagem = message.Sid,
                    Ok = true
                };
            }
            catch (Exception ex)
            {
                return new GetApiRetorno
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    InnerException = ex?.InnerException?.Message
                };
            }

        }

        [HttpGet()]
        [Route("api/Atendimentoapi/ApagarConversa")]
        public GetApiRetorno ApagarConversa(string chaveConversa, string numeroWhatsapp)
        {
            try
            {
                // Find your Account Sid and Token at twilio.com/console
                // and set the environment variables. See http://twil.io/secure


                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Canal_Comunicacao>(session))
                {

                    Canal_Comunicacao canal = repository.selectFirst(p => p.Numero == numeroWhatsapp).Result;

                    if (canal == null)
                    {
                        throw new Exception("canal nao encontrado");
                    }

                    TwilioClient.Init(canal.TwillioAccountId, canal.TwilioAuthToken);
                    ConversationResource.Delete(pathSid: chaveConversa);

                    return new GetApiRetorno
                    {
                        Mensagem = chaveConversa,
                        Ok = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new GetApiRetorno
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    InnerException = ex?.InnerException?.Message
                };
            }

        }

        [HttpGet()]
        [Route("api/Atendimentoapi/GetMessage")]
        public GetApiRetorno GetMessage(string chaveConversa, string chaveMessage)
        {
            try
            {

                // Find your Account Sid and Token at twilio.com/console
                // and set the environment variables. See http://twil.io/secure
                string accountSid = "ACecd1718d1b921ec7526f9266538fc924";
                string authToken = "ceab51d98b24c5bd703459c2d4d3b93a";
                TwilioClient.Init(accountSid, authToken);

                var messages = MediaResource.Read(pathMessageSid: chaveMessage, limit: 20);

                foreach (var item in messages)
                {
                    Console.WriteLine(item.Uri);
                }


                return new GetApiRetorno
                {
                    Mensagem = chaveConversa,
                    Ok = true
                };
            }
            catch (Exception ex)
            {
                return new GetApiRetorno
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    InnerException = ex?.InnerException?.Message
                };
            }

        }

        [HttpGet()]
        [Route("api/Atendimentoapi/ListarMensagem")]
        public GetApiRetorno<List<Mensagem_ChatModels>> ListarMensagensConversations(string chaveConversa)
        {
            try
            {
                var RETORNO = new List<Mensagem_ChatModels>();
                // Find your Account Sid and Token at twilio.com/console
                // and set the environment variables. See http://twil.io/secure
                string accountSid = "ACecd1718d1b921ec7526f9266538fc924";
                string authToken = "ceab51d98b24c5bd703459c2d4d3b93a";


                string teste;
                TwilioClient.Init(accountSid, authToken);

                var conversa = ConversationResource.Fetch(pathSid: chaveConversa);
                var messages = MessageResource.Read(pathConversationSid: chaveConversa);

                foreach (var item in messages)
                {
                    var media = MediaResource.Read(pathMessageSid: item.Sid);
                    foreach (var item2 in media)
                    {
                        teste = item2.Uri;

                    }
                }

                return new GetApiRetorno<List<Mensagem_ChatModels>>
                {
                    Mensagem = "",
                    Objeto = RETORNO,
                    Ok = true
                };
            }
            catch (Exception ex)
            {
                return new GetApiRetorno<List<Mensagem_ChatModels>>
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    Objeto = new List<Mensagem_ChatModels>(),
                    InnerException = ex?.InnerException?.Message
                };
            }
        }

        //Teste



        [AllowAnonymous]
        [AcceptVerbs("Get", "Post")]
        [Route("api/atendimentopi/WebhookMetaWhatsAppAPI")]
        public async Task<HttpResponseMessage> WebhookMetaWhatsAppAPI([FromBody] MetaWhatsAppRequest request)
        {
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                //Busca o parametro da URL identificando o token
                Uri currurl = HttpContext.Current.Request.Url;
                string hubChallenge = HttpUtility.ParseQueryString(currurl.Query).Get("hub.challenge");
                logExecucao.RegistraLog("currurl: " + currurl);
                logExecucao.RegistraLog("hubChallenge: " + hubChallenge);

                if (!String.IsNullOrEmpty(hubChallenge))
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(hubChallenge, Encoding.UTF8, "text/plain")
                    };
                }
                else
                {

                    try
                    {

                        using (ISession session = HibernateUtil.GetSession())
                        using (var repoCliente = new Conexao<Cliente>(session))
                        using (var repoAtendimento = new Conexao<Atendimento>(session))
                        using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                        using (var conexaoMongo = new ConexaoMongo())
                        using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                        using (var repositoryMensagemPerdida = new RepositoryMongo<MensagemPerdidaAtendimentoMongo>(conexaoMongo))
                        using (var repositoryBot = new Conexao<Bot>(session))
                        {

                            var json = JsonConvert.SerializeObject(request);
                            logExecucao.RegistraLog("JSON: " + json);

                            //Busca o número de telefone da conversa Formato: 553194659859
                            string numeroTelefoneCliente = request.entry[0].changes[0].value.messages[0].from;

                            var clientes = repoCliente.select(p => p.Telefone_Celular != null && p.Telefone_Celular != String.Empty).Result;
                            var celulares = clientes.Select(p => (p, p.Telefone_Celular.TrimStart().TrimEnd().Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""))).ToList();
                            var cliente = celulares.Where(p => ("55" + p.Item2) == numeroTelefoneCliente).Select(y => y.p).FirstOrDefault();

                            //Identificação do número de telefone: 260491770485891
                            var IdNumeroTelefone = request.entry[0].changes[0].value.metadata.phone_number_id;
                            Canal_Comunicacao canal = await repoCanalComunicacao.selectFirst(p => p.IdNumeroWhatsAppBusiness == IdNumeroTelefone);

                            var chat = await repository.Find(p => p.whatsAppChatId == numeroTelefoneCliente && p.dataEncerramento == null);

                            logExecucao.RegistraLog("whatsAppChatId: " + chat?.whatsAppChatId);

                            bool retornoBot = false;
                            if (canal.Bot?.Nome != null)
                            {
                                retornoBot = true;
                            }

                            var evento = "Conversa";
                            if (chat?.whatsAppChatId != null)
                            {
                                evento = "Mensagem";
                            }

                            logExecucao.RegistraLog("Chat Data Encerramento: " + (chat?.dataEncerramento));
                            logExecucao.RegistraLog("WhatsApp Chat Id: " + (chat?.telegramChatId));
                            logExecucao.RegistraLog("Evento: " + (evento));

                            var nomeUsuario = request.entry[0]?.changes[0]?.value?.contacts[0]?.profile;

                            if (evento == "Conversa")
                            {
                                //< !--RECUPERANDO OS ANEXOS RECEBIDOS-- >//
                                ArquivoMensagem_ChatModels anexos = new ArquivoMensagem_ChatModels();
                                List<ArquivoMensagem_ChatModels> arquivos = new List<ArquivoMensagem_ChatModels>();

                                var message = request.entry[0]?.changes[0]?.value?.messages[0];
                                string valorMensagem = null;


                                if (message.video != null)
                                {
                                    var retornoIntegracaoUrl = await new IntegracaoWhatsAppBusinessRepository().RecuperaURLMidia(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                                 message.video.id);

                                    anexos.id = message.video.id;
                                    anexos.name = message.video.sha256;
                                    anexos.url = retornoIntegracaoUrl.url;
                                    anexos.tipo = Tipo_Bot_Evento_Anexo.Video;
                                    arquivos.Add(anexos);
                                    valorMensagem = message.video?.caption;
                                }

                                else if (message.audio != null)
                                {
                                    var retornoIntegracaoUrl = await new IntegracaoWhatsAppBusinessRepository().RecuperaURLMidia(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                                 message.audio.id);

                                    anexos.id = message.audio.id;
                                    anexos.name = message.audio.sha256;
                                    anexos.url = retornoIntegracaoUrl.url;
                                    anexos.tipo = Tipo_Bot_Evento_Anexo.Audio;
                                    arquivos.Add(anexos);
                                    valorMensagem = message.audio?.caption;
                                }

                                else if (message.document != null)
                                {
                                    var retornoIntegracaoUrl = await new IntegracaoWhatsAppBusinessRepository().RecuperaURLMidia(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                                 message.document.id);

                                    anexos.id = message.document.id;
                                    anexos.name = message.document.filename;
                                    anexos.url = retornoIntegracaoUrl.url;
                                    anexos.tipo = Tipo_Bot_Evento_Anexo.Audio;
                                    arquivos.Add(anexos);
                                    valorMensagem = message.document?.caption;
                                }

                                else if (message.image != null)
                                {
                                    var retornoIntegracaoUrl = await new IntegracaoWhatsAppBusinessRepository().RecuperaURLMidia(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                                 message.image.id);

                                    anexos.id = message.image.id;
                                    anexos.name = message.image.sha256;
                                    anexos.url = retornoIntegracaoUrl.url;
                                    anexos.tipo = Tipo_Bot_Evento_Anexo.Imagem;
                                    arquivos.Add(anexos);
                                    valorMensagem = message.image?.caption;
                                }

                                DateTime? dataMensagem = null;

                                if (!String.IsNullOrEmpty(message?.timestamp))
                                {
                                    var timestamp = Convert.ToInt64(message.timestamp);
                                    var dateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
                                    var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");// Windows
                                    dataMensagem = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, brasiliaTimeZone);
                                }
                                else
                                {
                                    dataMensagem = DateTime.Now;
                                }

                                var mensagem = new Mensagem_ChatModels
                                {
                                    id = message.id,
                                    dataHora = dataMensagem.Value.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                    statusEnvio = StatusEnvioEnum.ENVIADO,
                                    mensagem = message.text.body == null ? valorMensagem : message.text.body,
                                    protocolo = null,
                                    enviadoPeloSuporte = false,
                                    fluxoChat = false,
                                    usuario = cliente?.NomeFantasia == null && nomeUsuario != null ? nomeUsuario?.name : cliente?.NomeFantasia,
                                    arquivos = arquivos
                                };

                                List<Mensagem_ChatModels> mensagemList = new List<Mensagem_ChatModels>
                                {
                                    mensagem
                                };

                                ChatBotWhatsApp chatBotWhatsApp = new ChatBotWhatsApp(canal.TokenTelegram, IdNumeroTelefone, null);
                                var crud_telegram = new CRUD_Generico
                                {
                                    Nome = request?.entry?.FirstOrDefault()?.changes?.FirstOrDefault()?.value?.contacts?.FirstOrDefault()?.profile?.name
                                };
                                await chatBotWhatsApp.StartChatAvulso(new AtendimentoChat
                                {
                                    cliente = new ClienteChat_Mobile
                                    {
                                        cpf_cnpj = cliente?.CpfCnpj,
                                        idCliente = cliente?.Id,
                                        nome = crud_telegram?.Nome,
                                        idCategoriaCliente = null,
                                        telefone = numeroTelefoneCliente
                                    },
                                    whatsAppChatId = numeroTelefoneCliente,
                                    whatsAppToken = canal?.TokenWhatsAppBusiness,
                                    idCanal = canal?.Id,
                                    bot = retornoBot,
                                    mensagens = mensagemList,
                                    crud_telegram_whatsapp = crud_telegram
                                });
                            }

                            //ADICIONA MENSAGEM
                            else
                            {

                                ArquivoMensagem_ChatModels anexosMensagem = new ArquivoMensagem_ChatModels();
                                List<ArquivoMensagem_ChatModels> arquivosMensagem = new List<ArquivoMensagem_ChatModels>();


                                //var message = request.entry[0].changes[0].value.messages[0];
                                var message = request?.entry?.FirstOrDefault()?
                                              .changes?.FirstOrDefault()?
                                              .value?.messages?.FirstOrDefault();


                                string valorMensagem = null;

                                if (message.video != null)
                                {
                                    var retornoIntegracaoUrl = await new IntegracaoWhatsAppBusinessRepository().RecuperaURLMidia(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                                 message.video.id);

                                    anexosMensagem.id = message.video.id;
                                    anexosMensagem.name = message.video.id;
                                    anexosMensagem.url = retornoIntegracaoUrl.url;
                                    anexosMensagem.tipo = Tipo_Bot_Evento_Anexo.Video;
                                    arquivosMensagem.Add(anexosMensagem);
                                    valorMensagem = message.video?.caption;
                                }
                                else if (message.audio != null)
                                {
                                    var retornoIntegracaoUrl = await new IntegracaoWhatsAppBusinessRepository().RecuperaURLMidia(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                                 message.audio.id);

                                    anexosMensagem.id = message.audio.id;
                                    anexosMensagem.name = message.audio.id;
                                    anexosMensagem.url = retornoIntegracaoUrl.url;
                                    anexosMensagem.tipo = Tipo_Bot_Evento_Anexo.Audio;
                                    arquivosMensagem.Add(anexosMensagem);
                                }
                                else if (message.document != null)
                                {
                                    var retornoIntegracaoUrl = await new IntegracaoWhatsAppBusinessRepository().RecuperaURLMidia(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                                 message.document.id);

                                    anexosMensagem.id = message.document.id;
                                    anexosMensagem.name = message.document.id;
                                    anexosMensagem.url = retornoIntegracaoUrl.url;
                                    anexosMensagem.tipo = Tipo_Bot_Evento_Anexo.Arquivo;
                                    arquivosMensagem.Add(anexosMensagem);
                                    valorMensagem = message.document?.caption;
                                }
                                else if (message.image != null)
                                {
                                    var retornoIntegracaoUrl = await new IntegracaoWhatsAppBusinessRepository().RecuperaURLMidia(new MetaWhatsAppBusiness(canal.TokenWhatsAppBusiness),
                                                                                                                                 message.image.id);

                                    anexosMensagem.id = message.image.id;
                                    anexosMensagem.name = message.image.id;
                                    anexosMensagem.url = retornoIntegracaoUrl.url;
                                    anexosMensagem.tipo = Tipo_Bot_Evento_Anexo.Imagem;
                                    arquivosMensagem.Add(anexosMensagem);
                                    valorMensagem = message.image?.caption;
                                }

                                var mensagem = new Mensagem_ChatModels
                                {
                                    id = message.id,
                                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                    statusEnvio = StatusEnvioEnum.ENVIADO,
                                    mensagem = message.text == null ? valorMensagem : message.text?.body,
                                    protocolo = null,
                                    enviadoPeloSuporte = false,
                                    fluxoChat = false,
                                    usuario = cliente?.NomeFantasia == null && nomeUsuario != null ? nomeUsuario?.name : cliente?.NomeFantasia,
                                    arquivos = arquivosMensagem
                                };

                                logExecucao.RegistraLog("Atendimento Encontrado no MONGODB");
                                logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);

                                new Thread(async delegate ()
                                {
                                    logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);
                                    await ChatHub.GetChatSrategy(chat).EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));

                                }).Start();
                            }
                        }



                    }
                    catch (Exception ex)
                    {
                        logExecucao.RegistraLog("Erro: " + ex.Message + "INNER EX:" + ex.InnerException?.Message + " STACK:" + ex.StackTrace);
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        [AllowAnonymous]
        [HttpPost()]
        [Route("api/atendimentopi/WebhookConsersationsAPI")]
        public async Task<GetApiRetorno<string>> WebhookConsersationsAPI([FromBody] InboundConversationRequest request)
        {
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                try
                {

                    using (ISession session = HibernateUtil.GetSession())
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoAtendimento = new Conexao<Atendimento>(session))
                    using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                    using (var repositoryMensagemPerdida = new RepositoryMongo<MensagemPerdidaAtendimentoMongo>(conexaoMongo))
                    using (var repositoryBot = new Conexao<Bot>(session))
                    {

                        if (!String.IsNullOrEmpty(request.MessagingBinding?.ProxyAddress))
                        {
                            request.MessagingBinding.ProxyAddress = Regex.Replace(request.MessagingBinding.ProxyAddress, @"[^\d]", "");
                        }

                        var json = JsonConvert.SerializeObject(request);
                        logExecucao.RegistraLog("JSON: " + json);

                        logExecucao.RegistraLog("Tipo Evento: " + request.EventType);

                        var msgDefault = true;
                        if (request.EventType == "onConversationAdded")
                        {
                            logExecucao.RegistraLog("onConversationAdded ativado");
                            logExecucao.RegistraLog("buscando numero: " + request.MessagingBinding.Address);


                            var clientes = repoCliente.select(p => p.Telefone_Celular != null && p.Telefone_Celular != String.Empty).Result;
                            var celulares = clientes.Select(p => (p, p.Telefone_Celular.TrimStart().TrimEnd().Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""))).ToList();
                            var cliente = celulares.Where(p => ("whatsapp:+55" + p.Item2) == request.MessagingBinding.Address).Select(y => y.p).FirstOrDefault();

                            Canal_Comunicacao canal = await repoCanalComunicacao.selectFirst(p => p.Numero == request.MessagingBinding.ProxyAddress);

                            bool retornoBot = false;
                            if (canal.Bot?.Nome != null)
                            {
                                retornoBot = true;
                            }

                            if (canal != null)
                            {
                                logExecucao.RegistraLog("CANAL ENCONTRADO");
                                ChatConversation chatConversation = new ChatConversation(canal.TwillioAccountId, canal.TwilioAuthToken);

                                if (cliente != null)
                                {
                                    logExecucao.RegistraLog("INICIANDO CHAT CLIENTE");
                                    await chatConversation.StartChat(new AtendimentoChat
                                    {
                                        cliente = new ClienteChat_Mobile
                                        {
                                            cpf_cnpj = cliente.CpfCnpj,
                                            idCliente = cliente.Id,
                                            nome = cliente.NomeFantasia,
                                            idCategoriaCliente = cliente.Categoria_Cliente?.Id,
                                        },
                                        conversationSid = request.ConversationSid,
                                        TwilioAccountId = canal?.TwillioAccountId,
                                        TwilioAuthToken = canal?.TwilioAuthToken,
                                        idCanal = canal?.Id,
                                        bot = retornoBot,
                                    });
                                }
                                else
                                {
                                    logExecucao.RegistraLog("INICIANDO CHAT NÃO CLIENTE");
                                    await chatConversation.StartChatAvulso(new AtendimentoChat
                                    {
                                        cliente = new ClienteChat_Mobile
                                        {
                                            cpf_cnpj = null,
                                            idCliente = null,
                                            nome = null,
                                            idCategoriaCliente = null,
                                        },
                                        conversationSid = request.ConversationSid,
                                        TwilioAccountId = canal?.TwillioAccountId,
                                        TwilioAuthToken = canal?.TwilioAuthToken,
                                        idCanal = canal?.Id,
                                        bot = retornoBot,
                                    });
                                }

                                try
                                {
                                    var chat = await repository.Find(p => p.conversationSid == request.ConversationSid);
                                    var mensagensPerdidas = await repositoryMensagemPerdida.FindAll(p => p.conversationid == request.ConversationSid);
                                    mensagensPerdidas = mensagensPerdidas.Where(p => chat.mensagens.Select(y => y.id).Contains(p.id) == false).ToList();

                                    if (mensagensPerdidas.Count > 0 && chat.cliente.cpf_cnpj != null)
                                    {
                                        new Thread(async delegate ()
                                        {
                                            foreach (var mensagemPerdida in mensagensPerdidas)
                                            {
                                                //Busca as imagens
                                                ArquivoMensagem_ChatModels anexos = new ArquivoMensagem_ChatModels();
                                                List<ArquivoMensagem_ChatModels> arquivos = new List<ArquivoMensagem_ChatModels>();
                                                if (request.Media != null)
                                                {
                                                    using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                                                    {
                                                        Canal_Comunicacao canal = await repoCanal.selectFirst(p => p.TwillioAccountId == request.AccountSid);

                                                        List<Media> jsonConverte = JsonConvert.DeserializeObject<List<Media>>(request.Media.ToString());

                                                        foreach (var anexoTwilio in jsonConverte)
                                                        {
                                                            var resposta = new BaseController("", "").BaixarAnexoTwilio(canal.TwilioServiceSID, anexoTwilio.Sid, anexoTwilio.ContentType);
                                                            anexos.id = anexoTwilio.Sid;
                                                            anexos.name = anexoTwilio.Sid;
                                                            anexos.url = $@"{CONSTANTES.URLPortal}/Arquivos/Atendimento/Chat/{resposta}";

                                                            var found = resposta.IndexOf(".");
                                                            var ext = resposta.Substring(found + 1);
                                                            //verificar tipo de anexo
                                                            anexos.tipo = await GetMimeType(ext);

                                                            arquivos.Add(anexos);
                                                        }
                                                    }
                                                }

                                                var mensagem = new Mensagem_ChatModels
                                                {
                                                    id = mensagemPerdida.id,
                                                    dataHora = mensagemPerdida.datahora,
                                                    statusEnvio = StatusEnvioEnum.ENVIADO,
                                                    mensagem = mensagemPerdida.mensagem,
                                                    protocolo = chat.protocolo,
                                                    enviadoPeloSuporte = false,
                                                    fluxoChat = false,
                                                    usuario = null,
                                                    arquivos = arquivos
                                                };
                                                await ChatHub.GetChatSrategy(chat).EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));
                                            }

                                        }).Start();
                                    }

                                }
                                catch (Exception)
                                {
                                    var chat = await repository.Find(p => p.conversationSid == request.ConversationSid);
                                    var mensagensPerdidas = await repositoryMensagemPerdida.FindAll(p => p.conversationid == request.ConversationSid);

                                    if (mensagensPerdidas.Count > 0 && chat.cliente.cpf_cnpj != null)
                                    {
                                        new Thread(async delegate ()
                                        {
                                            foreach (var mensagemPerdida in mensagensPerdidas)
                                            {
                                                //Busca as imagens
                                                ArquivoMensagem_ChatModels anexos = new ArquivoMensagem_ChatModels();
                                                List<ArquivoMensagem_ChatModels> arquivos = new List<ArquivoMensagem_ChatModels>();
                                                if (request.Media != null)
                                                {
                                                    using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                                                    {
                                                        Canal_Comunicacao canal = await repoCanal.selectFirst(p => p.TwillioAccountId == request.AccountSid);

                                                        List<Media> jsonConverte = JsonConvert.DeserializeObject<List<Media>>(request.Media.ToString());

                                                        foreach (var anexoTwilio in jsonConverte)
                                                        {
                                                            var resposta = new BaseController("", "").BaixarAnexoTwilio(canal.TwilioServiceSID, anexoTwilio.Sid, anexoTwilio.ContentType);
                                                            anexos.id = anexoTwilio.Sid;
                                                            anexos.name = anexoTwilio.Sid;
                                                            anexos.url = $@"{CONSTANTES.URLPortal}/Arquivos/Atendimento/Chat/{resposta}";

                                                            var found = resposta.IndexOf(".");
                                                            var ext = resposta.Substring(found + 1);
                                                            //verificar tipo de anexo
                                                            anexos.tipo = await GetMimeType(ext);

                                                            arquivos.Add(anexos);
                                                        }
                                                    }
                                                }

                                                var mensagem = new Mensagem_ChatModels
                                                {
                                                    id = mensagemPerdida.id,
                                                    dataHora = mensagemPerdida.datahora,
                                                    statusEnvio = StatusEnvioEnum.ENVIADO,
                                                    mensagem = mensagemPerdida.mensagem,
                                                    protocolo = chat.protocolo,
                                                    enviadoPeloSuporte = false,
                                                    fluxoChat = false,
                                                    usuario = null,
                                                    arquivos = arquivos
                                                };
                                                await ChatHub.GetChatSrategy(chat).EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));
                                            }

                                        }).Start();
                                    }
                                }
                                msgDefault = false;
                            }

                            if (msgDefault == true)
                            {
                                //logExecucao.RegistraLog("Não identificou mensagem");
                            }
                        }

                        if (request.EventType == "onMessageAdded")
                        {

                            var chat = await repository.Find(p => p.conversationSid == request.ConversationSid);

                            MensagemPerdidaAtendimentoMongo mensagemPerdida = new MensagemPerdidaAtendimentoMongo
                            {
                                id = request.MessageSid,
                                datahora = request.DateCreated.ToString("dd/MM/yyyy HH:mm:ss"),
                                mensagem = request.Body,
                                conversationid = request.ConversationSid
                            };

                            if (mensagemPerdida != null)
                            {
                                await repositoryMensagemPerdida.Save(mensagemPerdida);
                            }

                            if (chat != null)
                            {
                                //Busca as imagens
                                ArquivoMensagem_ChatModels anexos = new ArquivoMensagem_ChatModels();
                                List<ArquivoMensagem_ChatModels> arquivos = new List<ArquivoMensagem_ChatModels>();
                                if (request.Media != null)
                                {
                                    using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                                    {
                                        Canal_Comunicacao canal = await repoCanal.selectFirst(p => p.TwillioAccountId == request.AccountSid);

                                        List<Media> jsonConverte = JsonConvert.DeserializeObject<List<Media>>(request.Media.ToString());

                                        foreach (var anexoTwilio in jsonConverte)
                                        {
                                            var resposta = new BaseController("", "").BaixarAnexoTwilio(canal.TwilioServiceSID, anexoTwilio.Sid, anexoTwilio.ContentType);
                                            anexos.id = anexoTwilio.Sid;
                                            anexos.name = anexoTwilio.Sid;
                                            anexos.url = $@"{CONSTANTES.URLPortal}/Arquivos/Atendimento/Chat/{resposta}";

                                            var found = resposta.IndexOf(".");
                                            var ext = resposta.Substring(found + 1);
                                            //verificar tipo de anexo
                                            anexos.tipo = await GetMimeType(ext);

                                            arquivos.Add(anexos);
                                        }
                                    }
                                }

                                var mensagem = new Mensagem_ChatModels
                                {
                                    id = request.MessageSid,
                                    dataHora = request.DateCreated.ToString("dd/MM/yyyy HH:mm:ss"),
                                    statusEnvio = StatusEnvioEnum.ENVIADO,
                                    mensagem = request.Body,
                                    protocolo = chat.protocolo,
                                    enviadoPeloSuporte = false,
                                    fluxoChat = false,
                                    usuario = null,
                                    arquivos = arquivos
                                };

                                logExecucao.RegistraLog("CV SID:" + chat.conversationSid + " NUMERO:" + chat.protocolo);

                                if (chat != null)
                                {
                                    logExecucao.RegistraLog("Atendimento Encontrado no MONGODB");
                                    logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);

                                    new Thread(async delegate ()
                                    {
                                        logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);
                                        await ChatHub.GetChatSrategy(chat).EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));

                                    }).Start();
                                }

                            }

                            msgDefault = false;
                        }

                        return new GetApiRetorno<string>
                        {
                            Mensagem = "",
                            Objeto = "",
                            Ok = true
                        };
                    }
                }
                catch (Exception ex)
                {
                    logExecucao.RegistraLog("Erro: " + ex.Message + "INNER EX:" + ex.InnerException?.Message + " STACK:" + ex.StackTrace);
                    return new GetApiRetorno<string>
                    {
                        Mensagem = ex.Message,
                        Ok = false,
                        Objeto = "",
                        InnerException = ex?.InnerException?.Message
                    };
                }
            }

        }

        [HttpGet()]
        [Route("api/atendimentopi/TestarBot")]
        public async Task<GetApiRetorno<string>> TestarBot(String mensagem, Int64? IdAtendimento)
        {
            try
            {

                using (ISession session = HibernateUtil.GetSession())
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))

                {

                    if (IdAtendimento.HasValue == false)
                    {
                        CONSTANTES.log4Net.Info("onConversationAdded ativado");
                        var clientes = repoCliente.select(p => p.Telefone_Celular != null && p.Telefone_Celular != String.Empty).Result;
                        var celulares = clientes.Select(p => (p, p.Telefone_Celular.TrimStart().TrimEnd().Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""))).ToList();
                        var cliente = celulares.Where(p => ("whatsapp:+55" + p.Item2) == "whatsapp:+5521981823267").Select(y => y.p).FirstOrDefault();

                        Canal_Comunicacao canal = await repoCanalComunicacao.selectFirst();

                        if (canal != null)
                        {
                            ChatConversation chatConversation = new ChatConversation(canal.TwillioAccountId, canal.TwilioAuthToken);
                            await chatConversation.StartChat(new AtendimentoChat
                            {
                                cliente = new ClienteChat_Mobile
                                {
                                    cpf_cnpj = cliente.CpfCnpj,
                                    idCliente = cliente.Id,
                                    nome = cliente.NomeFantasia,
                                    idCategoriaCliente = cliente.Categoria_Cliente?.Id,
                                },
                                conversationSid = "CH0b1118ee9bcf435894b409dccef34fea",
                                TwilioAccountId = canal?.TwillioAccountId,
                                TwilioAuthToken = canal?.TwilioAuthToken,
                                idCanal = canal?.Id
                            });
                        }
                    }
                    else
                    {

                        var atendimento = await repoAtendimento.selectFirst(p => p.Id == IdAtendimento);
                        var protocolo = atendimento.Numero;
                        var mensagemVar = new Mensagem_ChatModels
                        {
                            dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                            statusEnvio = StatusEnvioEnum.ENVIADO,
                            mensagem = mensagem,
                            protocolo = atendimento.Numero,
                            enviadoPeloSuporte = false,
                            fluxoChat = false,
                            usuario = null
                        };

                        var chat = await repository.Find(p => p.protocolo == protocolo);
                        //AdicionarMediaBot(chat.conversationSid);
                        await ChatHub.GetChatSrategy(chat).EnviarMensagem(atendimento.Numero, JsonConvert.SerializeObject(mensagemVar));
                    }
                }

                return new GetApiRetorno<string>
                {
                    Mensagem = "",
                    Objeto = "",
                    Ok = true
                };
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new GetApiRetorno<string>
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    Objeto = "",
                    InnerException = ex?.InnerException?.Message
                };
            }

        }

        [HttpPost()]
        [Route("api/atendimentopi/TestarBotRecurso")]
        public async Task<RetornoTesteBot> TestarBotRecurso(TestarBotRequestModel request)
        {
            RetornoTesteBot retorno = new RetornoTesteBot();
            retorno.sequenciaMensagens = new List<string>();
            try
            {

                using (ISession session = HibernateUtil.GetSession())
                using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
                using (var repoBot = new Conexao<Bot>(session))
                using (var repoBotEvento = new Conexao<BotEvento>(session))
                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    Bot bot = await repoBot.selectFirst(p => p.Id == request.IdBot);
                    BotEvento ultimoEvento = await repoBotEvento.selectFirst(p => p.Id == request.UltimoEvento);
                    var chatControlador = new ChatBotBusiness(bot, ultimoEvento, logExecucao, request.atendimento);
                    var retornoBot = await chatControlador.RunBot(request.mensagem);


                    var conf = await repoConfCanal.selectFirst();
                    var canal = await repoCanal.selectFirst();

                    foreach (var item in retornoBot.valor.Select(p => p.texto).ToList())
                    {
                        if (conf.MostrarNomeAtendente)
                        {
                            retorno.sequenciaMensagens.Add("<b>" + (canal.Bot.Nome) + ":</b> " + item);
                        }
                        else
                        {
                            retorno.sequenciaMensagens.Add(item);
                        }
                    }

                    if (chatControlador.EventoAtual?.Id != null)
                        retorno.eventoAtual = chatControlador.EventoAtual.Id.ToString();

                    retorno.atendimento = request.atendimento;

                }


            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);

            }

            return retorno;

        }

        //private string GetTwillioMediaUrl(string mediaId, string MessageServiceId)
        //{
        //    var url = $"https://mcs.us1.twilio.com/v1/Services/{MessageServiceId}/Media/${mediaId}";

        //    var client = new RestClient(url);

        //    var request = new RestRequest(Method.GET);
        //    request.AddHeader("postman-token", "d6a5b694-ec96-c2cd-ff0a-5e9efe83f981");
        //    request.AddHeader("cache-control", "no-cache");
        //    request.AddHeader("Authorization", "");
        //    request.AddHeader("Accept", "application/json");

        //    var response = client.Execute<GetApiRetorno<T>>(request);

        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        throw new Exception($"Erro PostByUrl Status: {response.StatusCode} Url: {url} Erro: {response.ErrorMessage} {response.ErrorException}");
        //    }

        // return JsonService.DeserializeObject<GetApiRetorno<MediaContentTwilio>>(response.Content).Objeto.links.content_direct_temporary;
        //}

        [HttpPost()]
        [Route("api/Atendimentoapi/GetMimeType")]
        public async Task<Tipo_Bot_Evento_Anexo> GetMimeType(string extension)
        {
            Tipo_Bot_Evento_Anexo mimeType = 0;

            //(.jpeg|.jpg|.gif|.png|.bmp|.tiff)
            if (extension.EndsWith(".png") || extension.EndsWith(".jpg") || extension.EndsWith(".jpeg") || extension.EndsWith(".gif") || extension.EndsWith(".bmp") || extension.EndsWith(".tiff"))
            {
                mimeType = Tipo_Bot_Evento_Anexo.Imagem;
            }

            //(.mpeg|.mp4|.quicktime|.webm|.3gpp|.3gpp2|.3gpp-tt|.H261|.H263-1998|.H263-2000|.H264)
            else if (extension.EndsWith(".mp4") || extension.EndsWith(".mpeg") || extension.EndsWith(".quicktime") || extension.EndsWith(".webm") || extension.EndsWith(".3gpp") || extension.EndsWith(".3gp2") ||
                extension.EndsWith(".H261") || extension.EndsWith(".H263-1998") || extension.EndsWith(".H263-2000") || extension.EndsWith(".H264") || extension.EndsWith(".3gpp-tt"))
            {
                mimeType = Tipo_Bot_Evento_Anexo.Video;
            }

            //(.L24|.mp4|.mpeg|.ogg|.vnd.rn-realaudio|.vnd.wave|.3gpp|.3gpp2|.ac3|.webm|.amr-nb|.amr|.snd|.au)
            else if (extension.EndsWith(".L24") || extension.EndsWith(".mp4") || extension.EndsWith(".mpeg") || extension.EndsWith(".ogg") ||
                extension.EndsWith(".vnd.rn-realaudio") || extension.EndsWith(".vnd.wave") || extension.EndsWith(".3gpp") ||
                extension.EndsWith(".3gpp2") || extension.EndsWith(".ac3") || extension.EndsWith(".webm") || extension.EndsWith(".amr-nb") ||
                extension.EndsWith(".amr") || extension.EndsWith(".snd") || extension.EndsWith(".au"))
            {
                mimeType = Tipo_Bot_Evento_Anexo.Audio;
            }

            //(.vcard |.pdf |.directory |.calendar |.richtext |.rtf |.x - vcard)
            else if (extension.EndsWith(".vcard") || extension.EndsWith(".pdf") || extension.EndsWith(".directory") || extension.EndsWith(".calendar") ||
                extension.EndsWith(".rtf") || extension.EndsWith(".doc") || extension.EndsWith(".docx") || extension.EndsWith(".x-vcard"))
            {
                mimeType = Tipo_Bot_Evento_Anexo.Arquivo;
            }

            return mimeType;
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/Atendimentoapi/AdicionarArquivoMensagemChat")]
        public async Task<GetApiRetorno> AdicionarArquivoMensagemChat()
        {
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                try
                {
                    var retorno = new GetApiRetorno();
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    var provider = new MultipartMemoryStreamProvider();
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Atendimento\Chat";

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    await Request.Content.ReadAsMultipartAsync(provider);

                    foreach (var file in provider.Contents.Take(1))
                    {
                        var parameters = file.Headers.ContentDisposition.Parameters;
                        string filename = StringService.RemoverAcentos(file.Headers.ContentDisposition.Parameters.Skip(1).ToList().FirstOrDefault().Value.Replace("\"", ""));
                        var buffer = await file.ReadAsByteArrayAsync();
                        File.WriteAllBytes($"{_pasta}/{filename}", buffer);
                    }

                    retorno.Ok = true;
                    return retorno;
                }
                catch (Exception ex)
                {
                    logExecucao.RegistraLog($"Ocorreu um erro ao adicionar arquivo a pasta: {ex.Message}.");

                    return new GetApiRetorno<List<Mensagem_ChatModels>>
                    {
                        Mensagem = ex.Message,
                        Ok = false,
                        Objeto = new List<Mensagem_ChatModels>(),
                        InnerException = ex?.InnerException?.Message
                    };
                }
            }

        }

        [HttpPost()]
        [Route("api/Atendimentoapi/AlterarStatusAgente")]
        public async Task<GetApiRetorno> AlterarStatusAgente(AlterarStatusModels alterarStatusRequest)
        {
            try
            {

                using (var conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                using (var repositorySuporte = new RepositoryMongo<SuporteChatMongo>(conexaoMongo))
                {
                    IMongoQueryable<SuporteChatMongo> query = repository.GetQueryable();
                    var suportes = query.ToList();

                    var suporte = suportes.Find(p => p.idSuporte == alterarStatusRequest.idSuporte);
                    if (suporte != null)
                    {
                        if (alterarStatusRequest.online)
                        {
                            suporte.status = StatusAtendimento.DISPONIVEL;
                            suporte.MotivoAusencia = "";
                        }
                        else
                        {
                            suporte.status = StatusAtendimento.AUSENTE;
                            suporte.MotivoAusencia = alterarStatusRequest.motivoAusencia;
                        }
                    }

                    await repository.Update(p => p.idSuporte == suporte.idSuporte, suporte);
                }

                return new GetApiRetorno<List<Mensagem_ChatModels>>
                {
                    Ok = true
                };

            }
            catch (Exception ex)
            {
                return new GetApiRetorno<List<Mensagem_ChatModels>>
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    Objeto = new List<Mensagem_ChatModels>(),
                    InnerException = ex?.InnerException?.Message
                };
            }

        }


        public class teste
        {
            public HttpPostedFile UploadedImage { get; set; }
            public string ImageName { get; set; }
        }

        //WEBHOOK TELEGRAM BOT API
        [AllowAnonymous]
        [HttpPost()]
        [Route("api/atendimentoapi/WebhookTelegramAPI")]
        public async Task<GetApiRetorno<string>> WebhookTelegramAPI([FromBody] TelegramModels.Root request)
        {
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                try
                {
                    using (ISession session = HibernateUtil.GetSession())
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoAtendimento = new Conexao<Atendimento>(session))
                    using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                    using (var repositoryMensagemPerdida = new RepositoryMongo<MensagemPerdidaAtendimentoMongo>(conexaoMongo))
                    using (var repositoryBot = new Conexao<Bot>(session))
                    {
                        //[FromBody] TelegramModels.Root request

                        //Busca o parametro da URL identificando o token
                        Uri currurl = HttpContext.Current.Request.Url;

                        var token = HttpUtility.ParseQueryString(currurl.Query).Get("token");

                        logExecucao.RegistraLog("Token: " + token);

                        var json = JsonConvert.SerializeObject(request);
                        logExecucao.RegistraLog("JSON: " + json);
                        //Busca o número de telefone da conversa Formato: 553194659859
                        string numeroTelefoneCliente = Convert.ToString(request.message.message_id);

                        var clientes = repoCliente.select(p => p.Telefone_Celular != null && p.Telefone_Celular != String.Empty).Result;
                        var celulares = clientes.Select(p => (p, p.Telefone_Celular.TrimStart().TrimEnd().Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""))).ToList();
                        var cliente = celulares.Where(p => ("55" + p.Item2) == numeroTelefoneCliente).Select(y => y.p).FirstOrDefault();

                        Canal_Comunicacao canal = new Canal_Comunicacao();
                        if (string.IsNullOrEmpty(token))
                        {
                            canal = await repoCanalComunicacao.selectFirst(p => p.Tipo_Canal_Comunicacao == Tipo_Canal_Comunicacao.Telegram);
                        }
                        else
                        {
                            canal = await repoCanalComunicacao.selectFirst(p => p.TokenTelegram == token);
                        }

                        var chat = await repository.Find(p => p.telegramChatId == request.message.chat.id && p.dataEncerramento == null);

                        var msgDefault = true;

                        logExecucao.RegistraLog("telegramChatId: " + chat?.telegramChatId);

                        bool retornoBot = false;
                        if (canal.Bot?.Nome != null)
                        {
                            retornoBot = true;
                        }

                        var evento = "Conversa";
                        if (chat?.telegramChatId != null)
                        {
                            evento = "Mensagem";
                        }

                        logExecucao.RegistraLog("Chat Data Encerramento: " + (chat?.dataEncerramento));
                        logExecucao.RegistraLog("Telegram Chat Id: " + (chat?.telegramChatId));
                        logExecucao.RegistraLog("Evento: " + (evento));

                        if (evento == "Conversa")
                        {

                            //<!--RECUPAERANDO OS ANEXOS RECEBIDOS-->//
                            ArquivoMensagem_ChatModels anexos = new ArquivoMensagem_ChatModels();
                            List<ArquivoMensagem_ChatModels> arquivos = new List<ArquivoMensagem_ChatModels>();

                            if (request.message.document != null)
                            {
                                anexos.id = request.message.document.file_id;
                                anexos.name = request.message.document.file_name;
                                anexos.url = request.message.document.file_name;
                                anexos.tipo = Tipo_Bot_Evento_Anexo.Arquivo;
                                arquivos.Add(anexos);
                            }
                            else if (request.message.video != null)
                            {
                                anexos.id = request.message.video.file_id;
                                anexos.name = request.message.video.file_name;
                                anexos.url = request.message.video.file_name;
                                anexos.tipo = Tipo_Bot_Evento_Anexo.Video;
                                arquivos.Add(anexos);
                            }
                            else if (request.message.photo != null)
                            {
                                foreach (var item in request.message.photo)
                                {
                                    anexos.id = item.file_id;
                                    anexos.name = item.file_id;
                                    anexos.url = item.file_id;
                                    anexos.tipo = Tipo_Bot_Evento_Anexo.Imagem;
                                    arquivos.Add(anexos);
                                }
                            }
                            else if (request.message.audio != null)
                            {
                                anexos.id = request.message.audio.file_id;
                                anexos.name = request.message.audio.file_name;
                                anexos.url = request.message.audio.file_name;
                                anexos.tipo = Tipo_Bot_Evento_Anexo.Audio;
                                arquivos.Add(anexos);
                            }
                            else if (request.message.voice != null)
                            {
                                anexos.id = request.message.voice.file_id;
                                anexos.name = request.message.voice.file_id;
                                anexos.url = request.message.voice.file_id;
                                anexos.tipo = Tipo_Bot_Evento_Anexo.Audio;
                                arquivos.Add(anexos);
                            }

                            DateTime? dataMensagem = null;

                            if (request?.message?.date != null)
                            {
                                var timestamp = request.message.date.Value;
                                var dateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
                                var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");// Windows
                                dataMensagem = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, brasiliaTimeZone);
                            }
                            else
                            {
                                dataMensagem = DateTime.Now;
                            }
                            ChatBotTelegram chatTelegram = new ChatBotTelegram(canal.TokenTelegram, request.message.chat.id, null);
                            var crud_telegram = new CRUD_Generico
                            {
                                Nome = request?.message?.from?.first_name,
                                UltimoNome = !String.IsNullOrEmpty(request.message.from.last_name) ? request.message.from.last_name : String.Empty
                            };

                            var mensagem = new Mensagem_ChatModels
                            {
                                id = request.update_id.ToString(),
                                dataHora = dataMensagem.Value.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                statusEnvio = StatusEnvioEnum.ENVIADO,
                                mensagem = request.message?.text == null ? request.message?.caption : request.message?.text,
                                protocolo = null,
                                enviadoPeloSuporte = false,
                                fluxoChat = false,
                                usuario = $"{crud_telegram.Nome} {crud_telegram.UltimoNome}",
                                arquivos = arquivos
                            };

                            List<Mensagem_ChatModels> mensagemList = new List<Mensagem_ChatModels>();
                            mensagemList.Add(mensagem);

                            await chatTelegram.StartChatAvulso(new AtendimentoChat
                            {
                                cliente = new ClienteChat_Mobile
                                {
                                    cpf_cnpj = null,
                                    idCliente = null,
                                    nome = $"{crud_telegram.Nome} {crud_telegram.UltimoNome}",
                                    idCategoriaCliente = null,
                                    identificadorTelegram = request.message.chat.id
                                },
                                telegramChatId = request.message.chat.id,
                                telegramToken = canal?.TokenTelegram,
                                idCanal = canal?.Id,
                                bot = retornoBot,
                                mensagens = mensagemList,
                                crud_telegram_whatsapp = crud_telegram
                            });
                        }

                        //ADICIONA MENSAGEM
                        else
                        {

                            ArquivoMensagem_ChatModels anexosMensagem = new ArquivoMensagem_ChatModels();
                            List<ArquivoMensagem_ChatModels> arquivosMensagem = new List<ArquivoMensagem_ChatModels>();

                            if (request.message.document != null)
                            {
                                anexosMensagem.id = request.message.document.file_id;
                                anexosMensagem.name = request.message.document.file_name;
                                anexosMensagem.url = request.message.document.file_name;
                                arquivosMensagem.Add(anexosMensagem);
                            }
                            else if (request.message.video != null)
                            {
                                anexosMensagem.id = request.message.video.file_id;
                                anexosMensagem.name = request.message.video.file_name;
                                anexosMensagem.url = request.message.video.file_name;
                                arquivosMensagem.Add(anexosMensagem);
                            }
                            else if (request.message.photo != null)
                            {
                                foreach (var item in request.message.photo)
                                {
                                    anexosMensagem.id = item.file_id;
                                    anexosMensagem.name = item.file_id;
                                    anexosMensagem.url = item.file_id;
                                    arquivosMensagem.Add(anexosMensagem);
                                }
                            }
                            else if (request.message.audio != null)
                            {
                                anexosMensagem.id = request.message.audio.file_id;
                                anexosMensagem.name = request.message.audio.file_name;
                                anexosMensagem.url = request.message.audio.file_name;
                                arquivosMensagem.Add(anexosMensagem);
                            }
                            else if (request.message.voice != null)
                            {
                                anexosMensagem.id = request.message.voice.file_id;
                                anexosMensagem.name = request.message.voice.file_id;
                                anexosMensagem.url = request.message.voice.file_id;
                                arquivosMensagem.Add(anexosMensagem);
                            }

                            ChatBotTelegram chatTelegram = new ChatBotTelegram(canal.TokenTelegram, request.message.chat.id, null);
                            var crud_telegram = new CRUD_Generico
                            {
                                Nome = request?.message?.from?.first_name,
                                UltimoNome = !String.IsNullOrEmpty(request.message.from.last_name) ? request.message.from.last_name : String.Empty
                            };

                            var mensagem = new Mensagem_ChatModels
                            {
                                id = request.update_id.ToString(),
                                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                                statusEnvio = StatusEnvioEnum.ENVIADO,
                                mensagem = request.message?.text == null ? request.message?.caption : request.message?.text,
                                protocolo = chat.protocolo,
                                enviadoPeloSuporte = false,
                                fluxoChat = false,
                                usuario = $"{crud_telegram.Nome} {crud_telegram.UltimoNome}",
                                arquivos = arquivosMensagem
                            };

                            logExecucao.RegistraLog("Atendimento Encontrado no MONGODB");
                            logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);


                            new Thread(async delegate ()
                            {
                                logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);
                                await ChatHub.GetChatSrategy(chat).EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));

                            }).Start();
                        }


                        return new GetApiRetorno<string>
                        {
                            Mensagem = "",
                            Objeto = "",
                            Ok = true
                        };
                    }
                }
                catch (Exception ex)
                {
                    logExecucao.RegistraLog("Erro: " + ex.Message + "INNER EX:" + ex.InnerException?.Message + " STACK:" + ex.StackTrace);
                    return new GetApiRetorno<string>
                    {
                        Mensagem = ex.Message,
                        Ok = false,
                        Objeto = "",
                        InnerException = ex?.InnerException?.Message
                    };
                }
            }

        }

        [HttpPost()]
        [Route("api/atendimentoapi/VerificaFluxoEncerramento")]
        public async Task<PostApiRetorno<Boolean>> VerificaFluxoEncerramento(Int64? Id_Atendimento, Int64? Id_ProximaEtapa)
        {
            var response = new PostApiRetorno<Boolean>() { IdObjeto = true, Ok = true };

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var repoFluxoAtendimento = new Conexao<FluxoAtendimento>(session))
            {
                try
                {
                    if (Id_Atendimento == null || Id_Atendimento == 0)
                        throw new Exception("atendimento não informado");

                    Atendimento atendimento = await repoAtendimento.selectFirst(p => p.Id == Id_Atendimento);

                    if (atendimento == null)
                        throw new Exception("Atendimento não encontrado para validação");

                    var osTravaAtendimento = atendimento.Tipo_Atendimento.Trava_Atendimento_OS;

                    if (!osTravaAtendimento)
                        return response;

                    if (atendimento.EtapaFluxo.Etapa_Decisoria == false)
                    {
                        var proximaEtapa = atendimento.EtapaFluxo.Proxima_Etapa;

                        if (proximaEtapa == null)
                            throw new Exception("Próxima etapa não informada");

                        var idTipoAtendimento = atendimento.Tipo_Atendimento?.Id;
                        atendimento.EtapaFluxo = await repoFluxoAtendimento.selectFirst(p => p.Sequencia == Convert.ToInt64(proximaEtapa) && p.Tipo_Atendimento.Id == idTipoAtendimento && p.Status == Status_Default.Ativo);
                    }
                    else
                    {
                        if (Id_ProximaEtapa.HasValue == false)
                            throw new Exception("Próxima etapa não informada");

                        var proximaEtapa = Id_ProximaEtapa.Value;

                        atendimento.EtapaFluxo = await repoFluxoAtendimento.selectFirst(p => p.Id == Convert.ToInt64(proximaEtapa));
                    }

                    if (atendimento.EtapaFluxo == null)
                        throw new Exception("Etapa não encontrada");


                    bool existeOsAtendimento = false;
                    bool OsEmAberto = false;

                    if (atendimento.EtapaFluxo.Encerra_Atendimento == true)
                    {
                        using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                        using (var docProdRepo = new Conexao<Documento_ProducaoAniel>(sessionT5))
                        {
                            var docProd = await docProdRepo.selectFirst(x => x.Id_Atendimento == Id_Atendimento);

                            if (docProd == null)
                                existeOsAtendimento = false;
                            else
                                existeOsAtendimento = true;

                            if (docProd != null && docProd.Status.HasValue)
                            {
                                if (docProd.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                                    docProd.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                                    docProd.Status == StatusExecucaoAniel.Cancelado)
                                    OsEmAberto = false;
                                else
                                    OsEmAberto = true;
                            }
                        }
                    }

                    if (existeOsAtendimento && OsEmAberto)
                        throw new Exception("Atendimento não pode ser fechado, pois está atrelado a uma OS em Aberto");

                }
                catch (Exception ex)
                {
                    response.Ok = false;
                    response.IdObjeto = false;
                    response.Mensagem = ex.Message;
                }

                return response;
            }
        }

        [HttpPost()]
        [Route("api/atendimentoapi/EtapaFluxoCriarOS")]
        public async Task<PostApiRetorno<RetornoMudarEtapaFluxoModel>> EtapaFluxoCriarOS(MudarEtapaFluxoModel request)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessiont5 = HibernateUtil.GetSessionT5())
                using (var conexaoMongo = new ConexaoMongo())
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoFluxoAtendimento = new Conexao<FluxoAtendimento>(session))
                using (var repoTipoServicoEquipe = new Conexao<Tipo_Servico_EquipeAniel>(sessiont5))
                using (var repositoryAtendimentoChat = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))

                {
                    if (request.IdAtendimento == 0)
                        throw new Exception("atendimento não informado");

                    Atendimento atendimento = await repoAtendimento.selectFirst(p => p.Id == request.IdAtendimento);
                    Atendimento modeloAnterior = (Atendimento)atendimento.Clone();

                    if (atendimento.EtapaFluxo == null)
                        throw new Exception("atendimento não possui etapa");

                    if (request.IdProximaEtapa.HasValue == false)
                        throw new Exception("Próxima etapa não informada");

                    var idTipoAtendimento = atendimento.Tipo_Atendimento?.Id;
                    var proximaEtapa = request.IdProximaEtapa;
                    var etapa_fluxo = await repoFluxoAtendimento.selectFirst(p => p.Id == Convert.ToInt64(proximaEtapa) && p.Tipo_Atendimento.Id == idTipoAtendimento && p.Status == Status_Default.Ativo);

                    var atendimento_chat = await repositoryAtendimentoChat.Find(x => x.protocolo == atendimento.Protocolo);
                    if (atendimento_chat != null)
                    {
                        if (etapa_fluxo.AlterarSetor_Responsavel)
                        {
                            if (String.IsNullOrEmpty(atendimento_chat.dataEncerramento))
                            {
                                throw new Exception("Não é possível avançar etapa de atendimento, existe um chat em aberto.");
                            }
                        }
                    }

                    String codigo_contrato = String.Empty, id_tipo_servico = String.Empty, desc_etapa_atual = String.Empty, projeto = String.Empty;
                    Int16 id_departamento_padrao = 0;
                    if (etapa_fluxo.Etapa_CriaOS)
                    {
                        codigo_contrato = atendimento.Tipo_Atendimento?.CodCt;
                        projeto = atendimento.Tipo_Atendimento?.Projeto;
                        id_tipo_servico = atendimento.Tipo_Atendimento?.Desc_Tipo_Servico;
                        desc_etapa_atual = etapa_fluxo.Descricao;

                        var tipoServico = await repoTipoServicoEquipe.selectFirst(p => p.Cod_Tipo_Serv == Convert.ToInt16(id_tipo_servico) && p.CodCt == codigo_contrato);
                        if (tipoServico?.Natureza != null)
                        {
                            id_departamento_padrao = tipoServico.Natureza.IdDepartamentoPadrao ?? 0;
                        }
                    }

                    return new PostApiRetorno<RetornoMudarEtapaFluxoModel>
                    {
                        Ok = true,
                        IdObjeto = new RetornoMudarEtapaFluxoModel
                        {
                            Etapa_CriaOS = etapa_fluxo.Etapa_CriaOS,
                            Codigo_Contrato = codigo_contrato,
                            Id_Tipo_Servico = id_tipo_servico,
                            Id_Departamento_Padrao = id_departamento_padrao,
                            Desc_Etapa_Atual = desc_etapa_atual,
                            Projeto = projeto
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                return new PostApiRetorno<RetornoMudarEtapaFluxoModel>
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    InnerException = ex?.InnerException?.Message
                };
            }
        }

        [HttpPost()]
        [Route("api/atendimentoapi/MudarEtapaFluxo")]
        public async Task<PostApiRetorno<RetornoMudarEtapaFluxoModel>> MudarEtapaFluxo(MudarEtapaFluxoModel request)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                using (var repoFluxoAtendimento = new Conexao<FluxoAtendimento>(session))
                using (var repoDepartamentoAtendimento = new Conexao<Departamento>(session))
                using (var repoIdentityUser = new Conexao<IdentityUser>(session))
                using (var conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                {
                    if (request.IdAtendimento == 0)
                        throw new Exception("atendimento não informado");

                    Atendimento atendimento = await repoAtendimento.selectFirst(p => p.Id == request.IdAtendimento);
                    Atendimento modeloAnterior = (Atendimento)atendimento.Clone();
                    var motivoStatus = atendimento.EtapaFluxo.Motivo_Status_Atendimento;

                    if (atendimento.EtapaFluxo == null)
                        throw new Exception("atendimento não possui etapa");

                    if (atendimento.EtapaFluxo.Etapa_Decisoria == false)
                    {
                        var proximaEtapa = atendimento.EtapaFluxo.Proxima_Etapa;

                        if (proximaEtapa == null)
                            throw new Exception("Próxima etapa não informada");

                        var idTipoAtendimento = atendimento.Tipo_Atendimento?.Id;
                        atendimento.EtapaFluxo = await repoFluxoAtendimento.selectFirst(p => p.Sequencia == Convert.ToInt64(proximaEtapa) && p.Tipo_Atendimento.Id == idTipoAtendimento && p.Status == Status_Default.Ativo);
                    }
                    else
                    {
                        if (request.IdProximaEtapa.HasValue == false)
                            throw new Exception("Próxima etapa não informada");

                        var proximaEtapa = request.IdProximaEtapa;
                        atendimento.EtapaFluxo = await repoFluxoAtendimento.selectFirst(p => p.Id == Convert.ToInt64(proximaEtapa));
                    }

                    if (atendimento.EtapaFluxo != null)
                    {
                        if (atendimento.EtapaFluxo.AlterarSetor_Responsavel)
                        {
                            atendimento.Setor = await repoDepartamentoAtendimento.selectFirst(p => p.Id == Convert.ToInt64(atendimento.EtapaFluxo.Setor_Departamento));
                            atendimento.Responsavel = !String.IsNullOrEmpty(atendimento?.EtapaFluxo?.Responsavel) ? await repoIdentityUser.selectFirst(x => x.Id == atendimento.EtapaFluxo.Responsavel) : null;
                        }

                        //var setor_Departamento = atendimento.EtapaFluxo.Setor_Departamento;
                        //if (setor_Departamento != Convert.ToString(atendimento.Setor?.Id))
                        //{
                        //    atendimento.Responsavel = null;
                        //}

                        //atendimento.Setor = await repoDepartamentoAtendimento.selectFirst(p => p.Id == Convert.ToInt64(setor_Departamento));

                        var tempoEtapa = TimeSpan.FromHours(atendimento.EtapaFluxo.Tempo_Exc);
                        atendimento.DataIndicador_EtapaFluxo = DateTime.Now.Add(tempoEtapa);
                        atendimento.DataInicio_EtapaFluxo = DateTime.Now;
                    }
                    else
                    {
                        throw new Exception("proxima etapa não encontrada");
                    }

                    await repoAtendimento.SaveOrUpdate(atendimento);

                    if (modeloAnterior != null)
                    {
                        using (var repositoryLogs = new Conexao<LogAtendimento>(sessionLog))
                        using (var repositorioUsuario = new Conexao<IdentityUser>(session))
                        {
                            var usuario = await repositorioUsuario.selectFirst(p => p.Id == User.Identity.GetUserId());
                            var logs = atendimento.GeraLog(modeloAnterior, User.Identity.GetUserId(), usuario.Nome, Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogs.Save(logs);
                        }
                    }

                    if (atendimento.EtapaFluxo.Encerra_Atendimento == true)
                    {
                        var chatAtivo = ChatHub.ChatActive(atendimento.Numero);
                        if (chatAtivo)
                        {
                            await FinalizarChat(atendimento.Numero, true, User.Identity.GetUserId(), Convert.ToInt64(motivoStatus));
                            await FinalizarAtendimento(atendimento.Numero, User.Identity.GetUserId(), Convert.ToInt64(motivoStatus));
                        }
                        else
                        {
                            await FinalizarAtendimento(atendimento.Numero, User.Identity.GetUserId(), Convert.ToInt64(motivoStatus));
                        }
                    }

                    return new PostApiRetorno<RetornoMudarEtapaFluxoModel>
                    {
                        Ok = true,
                        IdObjeto = new RetornoMudarEtapaFluxoModel
                        {
                            EncerraAtendimento = atendimento?.EtapaFluxo?.Encerra_Atendimento ?? false,
                            CriaOs = atendimento?.Tipo_Atendimento?.Criar_Os_Encerrar ?? false
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                return new PostApiRetorno<RetornoMudarEtapaFluxoModel>
                {
                    Mensagem = ex.Message,
                    Ok = false,
                    InnerException = ex?.InnerException?.Message
                };
            }
        }

        [AllowAnonymous]
        [AcceptVerbs("Get", "Post")]
        [Route("api/atendimentoapi/WebhookMessengerAPI")]
        public async Task<HttpResponseMessage> WebhookMessengerAPI([FromBody] MessengerModels.Root request)
        {
            using (LogExecucao logExecucao = new LogExecucao("LogExecucaoBot", CONSTANTES.LogExecucaoAnielChannel))
            {
                try
                {
                    using (ISession session = HibernateUtil.GetSession())
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoAtendimento = new Conexao<Atendimento>(session))
                    using (var repoCanalComunicacao = new Conexao<Canal_Comunicacao>(session))
                    using (var conexaoMongo = new ConexaoMongo())
                    using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
                    using (var repositoryMensagemPerdida = new RepositoryMongo<MensagemPerdidaAtendimentoMongo>(conexaoMongo))
                    using (var repositoryBot = new Conexao<Bot>(session))
                    {
                        //Busca o parametro da URL identificando o token
                        Uri currurl = HttpContext.Current.Request.Url;
                        string hubChallenge = HttpUtility.ParseQueryString(currurl.Query).Get("hub.challenge");

                        if (!String.IsNullOrEmpty(hubChallenge))
                        {
                            return new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent(hubChallenge, Encoding.UTF8, "text/plain")
                            };
                        }
                        else
                        {
                            var json = JsonConvert.SerializeObject(request);
                            JToken convert = JObject.Parse(json);
                            string requisicao = convert.SelectToken("object").ToString();


                            logExecucao.RegistraLog("JSON: " + json);
                            logExecucao.RegistraLog("Tipo Origem: " + requisicao);

                            var count = request.entry.SelectMany(p => p.messaging.Where(y => y.message != null)).ToList();
                            if (count.Count > 0)
                            {
                                //FACEBOOK
                                if (requisicao == "page")
                                {
                                    foreach (var item in request.entry.SelectMany(p => p.messaging).ToList())
                                    {
                                        //Busca o parametro da URL identificando o token
                                        Uri urlMessenger = HttpContext.Current.Request.Url;
                                        string token = HttpUtility.ParseQueryString(urlMessenger.Query).Get("token");

                                        logExecucao.RegistraLog("Token: " + token);

                                        var chat = await repository.Find(p => p.messengerChatId == item.sender.id && p.dataEncerramento == null);

                                        Canal_Comunicacao canal = new Canal_Comunicacao();
                                        if (string.IsNullOrEmpty(token))
                                        {
                                            canal = await repoCanalComunicacao.selectFirst(p => p.Tipo_Canal_Comunicacao == Tipo_Canal_Comunicacao.Messenger);
                                        }
                                        else
                                        {
                                            canal = await repoCanalComunicacao.selectFirst(p => p.TokenMessenger == token);
                                        }

                                        var msgDefault = true;

                                        logExecucao.RegistraLog("messengerChatId: " + item.sender.id);

                                        bool retornoBot = false;
                                        if (canal.Bot?.Nome != null)
                                        {
                                            retornoBot = true;
                                        }

                                        var evento = "Conversa";
                                        if (chat?.messengerChatId != null)
                                        {
                                            evento = "Mensagem";
                                        }


                                        logExecucao.RegistraLog("Chat Data Encerramento: " + (chat?.dataEncerramento));
                                        logExecucao.RegistraLog("Messenger Chat Id: " + (chat?.messengerChatId));
                                        logExecucao.RegistraLog("Evento: " + (evento));


                                        if (!string.IsNullOrEmpty(item.message?.attachments?.ToString()) || !string.IsNullOrEmpty(item.message?.text))
                                        {

                                            if (evento == "Conversa")
                                            {
                                                ArquivoMensagem_ChatModels anexos = new ArquivoMensagem_ChatModels();
                                                List<ArquivoMensagem_ChatModels> arquivos = new List<ArquivoMensagem_ChatModels>();
                                                if (item.message.attachments != null)
                                                {
                                                    List<Attachment> jsonConverte = JsonConvert.DeserializeObject<List<Attachment>>(item.message.attachments.ToString());
                                                    foreach (var it in jsonConverte)
                                                    {
                                                        arquivos.Add(new ArquivoMensagem_ChatModels()
                                                        {
                                                            id = item.message.mid,
                                                            name = it.payload.url,
                                                            url = it.payload.url,
                                                        });
                                                    }
                                                }

                                                var mensagem = new Mensagem_ChatModels
                                                {
                                                    id = item.sender.id,
                                                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                                    statusEnvio = StatusEnvioEnum.ENVIADO,
                                                    mensagem = item.message?.text,
                                                    protocolo = null,
                                                    enviadoPeloSuporte = false,
                                                    fluxoChat = false,
                                                    usuario = null,
                                                    arquivos = arquivos
                                                };

                                                List<Mensagem_ChatModels> mensagemList = new List<Mensagem_ChatModels>();
                                                mensagemList.Add(mensagem);

                                                ChatBotMessenger chatMessenger = new ChatBotMessenger(canal.TokenMessenger, item.sender.id.ToString());
                                                await chatMessenger.StartChatAvulso(new AtendimentoChat
                                                {
                                                    cliente = new ClienteChat_Mobile
                                                    {
                                                        cpf_cnpj = null,
                                                        idCliente = null,
                                                        nome = null,
                                                        idCategoriaCliente = null,
                                                    },
                                                    messengerChatId = item.sender.id,
                                                    messengerToken = canal?.TokenMessenger,
                                                    idCanal = canal?.Id,
                                                    bot = retornoBot,
                                                    mensagens = mensagemList
                                                });
                                            }

                                            //ADICIONA MENSAGEM
                                            else
                                            {

                                                ArquivoMensagem_ChatModels anexosMensagem = new ArquivoMensagem_ChatModels();
                                                List<ArquivoMensagem_ChatModels> arquivosMensagem = new List<ArquivoMensagem_ChatModels>();
                                                if (item.message.attachments != null)
                                                {
                                                    List<Attachment> jsonConverte = JsonConvert.DeserializeObject<List<Attachment>>(item.message.attachments.ToString());
                                                    foreach (var it in jsonConverte)
                                                    {
                                                        arquivosMensagem.Add(new ArquivoMensagem_ChatModels()
                                                        {
                                                            id = item.message.mid,
                                                            name = it.payload.url,
                                                            url = it.payload.url,
                                                        });
                                                    }
                                                }

                                                var mensagem = new Mensagem_ChatModels
                                                {
                                                    id = item.sender.id,
                                                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                                    statusEnvio = StatusEnvioEnum.ENVIADO,
                                                    mensagem = item.message.text,
                                                    protocolo = chat.protocolo,
                                                    enviadoPeloSuporte = false,
                                                    fluxoChat = false,
                                                    usuario = null,
                                                    arquivos = arquivosMensagem
                                                };

                                                logExecucao.RegistraLog("Atendimento Encontrado no MONGODB");
                                                logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);

                                                new Thread(async delegate ()
                                                {
                                                    logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);
                                                    await ChatHub.GetChatSrategy(chat).EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));

                                                }).Start();

                                            }
                                        }
                                    }
                                }

                                //INSTAGRAM
                                else if (requisicao == "instagram")
                                {

                                    foreach (var item in request.entry.SelectMany(p => p.messaging).ToList())
                                    {
                                        if (!string.IsNullOrEmpty(item.message?.text))
                                        {
                                            //Busca o parametro da URL identificando o token
                                            Uri urlMessenger = HttpContext.Current.Request.Url;
                                            string token = HttpUtility.ParseQueryString(urlMessenger.Query).Get("token");

                                            logExecucao.RegistraLog("Token: " + token);

                                            var chat = await repository.Find(p => p.instagramChatId == item.sender.id && p.dataEncerramento == null);

                                            Canal_Comunicacao canal = new Canal_Comunicacao();
                                            if (string.IsNullOrEmpty(token))
                                            {
                                                canal = await repoCanalComunicacao.selectFirst(p => p.Tipo_Canal_Comunicacao == Tipo_Canal_Comunicacao.Instagram);
                                            }
                                            else
                                            {
                                                canal = await repoCanalComunicacao.selectFirst(p => p.TokenInstagram == token);
                                            }

                                            var msgDefault = true;

                                            logExecucao.RegistraLog("InstagramChatId: " + item.sender.id);

                                            bool retornoBot = false;
                                            if (canal.Bot?.Nome != null)
                                            {
                                                retornoBot = true;
                                            }

                                            var evento = "Conversa";
                                            if (chat?.instagramChatId != null)
                                            {
                                                evento = "Mensagem";
                                            }

                                            logExecucao.RegistraLog("Chat Data Encerramento: " + (chat?.dataEncerramento));
                                            logExecucao.RegistraLog("Messenger Chat Id: " + (chat?.instagramChatId));
                                            logExecucao.RegistraLog("Evento: " + (evento));

                                            if (evento == "Conversa")
                                            {
                                                //ChatBotInstagram chatInstagram = new ChatBotInstagram(canal.TokenInstagram, item.sender.id.ToString());
                                                //await chatInstagram.StartChatAvulso(new AtendimentoChat
                                                //{
                                                //    cliente = new ClienteChat_Mobile
                                                //    {
                                                //        cpf_cnpj = null,
                                                //        idCliente = null,
                                                //        nome = null,
                                                //        idCategoriaCliente = null,
                                                //    },
                                                //    instagramChatId = item.sender.id,
                                                //    instagramToken = canal?.TokenInstagram,
                                                //    idCanal = canal?.Id,
                                                //    bot = retornoBot,
                                                //});
                                            }

                                            //ADICIONA MENSAGEM
                                            else
                                            {

                                                var mensagem = new Mensagem_ChatModels
                                                {
                                                    id = item.sender.id,
                                                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                                                    statusEnvio = StatusEnvioEnum.ENVIADO,
                                                    mensagem = item.message.text,
                                                    protocolo = chat.protocolo,
                                                    enviadoPeloSuporte = false,
                                                    fluxoChat = false,
                                                    usuario = null,
                                                };

                                                logExecucao.RegistraLog("Atendimento Encontrado no MONGODB");
                                                logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);

                                                new Thread(async delegate ()
                                                {
                                                    logExecucao.RegistraLog("MENSAGEM: " + mensagem.mensagem);
                                                    await ChatHub.GetChatSrategy(chat).EnviarMensagem(chat.protocolo, JsonConvert.SerializeObject(mensagem));

                                                }).Start();

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logExecucao.RegistraLog("Erro: " + ex.Message + "INNER EX:" + ex.InnerException?.Message + " STACK:" + ex.StackTrace);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        [HttpPost()]
        [Route("api/atendimentoapi/GetComboPrioridade")]
        public async Task<GetApiRetorno<PrioridadeAtendimento>> GetComboPrioridade(PrioridadeAtendimento request)
        {
            List<PrioridadeAtendimento> tmpRetorno = new List<PrioridadeAtendimento>();
            GetApiRetorno<PrioridadeAtendimento> retorno = new GetApiRetorno<PrioridadeAtendimento>();

            SLA_Atendimento _sla = null;
            Tipo_Atendimento _tipoAtendimento = null;

            List<SLA_Atendimento> sla_atendimento = new List<SLA_Atendimento>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoSLA_Atendimento = new Conexao<SLA_Atendimento>(session))
            using (var repoTipo_Atendimento = new Conexao<Tipo_Atendimento>(session))
            {
                try
                {

                    var ClientePlanoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == request.Id_Cliente_Plano_Produto);
                    var cliente = await repoCliente.selectFirst(p => p.Id == request.Id_Cliente);

                    long? idCategoria = cliente?.Categoria_Cliente?.Id;
                    sla_atendimento = (await repoSLA_Atendimento.select(p => p.Status == Status_Default.Ativo && p.Tipo_Atendimento.Id == request.Tipo_AtendimentoSelecionado &&
                                                                                         (p.Categoria_Cliente.Id == idCategoria || p.Categoria_Cliente == null))).ToList();
                    if (sla_atendimento.Count == 0)
                    {
                        _tipoAtendimento = (await repoTipo_Atendimento.selectFirst(x => x.Status == Status_Default.Ativo && x.Id == request.Tipo_AtendimentoSelecionado));
                    }

                    foreach (var item in sla_atendimento)
                    {
                        if (item.Sla_PlanoProduto == Status_Default.Inativo)
                        {
                            _sla = sla_atendimento.Find(p => p.Sla_PlanoProduto == Status_Default.Inativo);
                        }
                        else
                        {

                            if (ClientePlanoProduto?.Plano != null)
                            {
                                _sla = sla_atendimento.Find(p => p.PlanosProdutos.Select(y => y.Plano.Id).Contains(ClientePlanoProduto.Plano.Id));
                            }
                            else
                            {
                                _sla = sla_atendimento.Find(p => p.PlanosProdutos.Select(y => y.Produto.Id).Contains(ClientePlanoProduto.Produto.Id));
                            }

                        }

                        if (_sla != null)
                        {
                            break;
                        }

                    }
                    ;

                    var prioridade_selecionada = new PrioridadeAtendimento
                    {
                        Prioridade_Atendimento = _sla != null ? (long)_sla.Prioridade_Atendimento : (long)_tipoAtendimento.Prioridade_Padrao
                    };


                    retorno.Objeto = prioridade_selecionada;
                }

                ////Filtros 
                //Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //    StringBuilder sqlFrom = new StringBuilder(@"FROM TB_SLA_ATENDIMENTO A 
                //    LEFT JOIN TB_SLA_ATENDIMENTO_PLAN_PROD B
                //    ON B.ID_SLA_ATENDIMENTO = A.ID
                //    WHERE ");

                //    List<String> condicoes = new List<String>();

                //    //if (request.soativos)
                //    //{
                //    //    condicoes.Add($"A.STATUS = 1");
                //    //}

                //    if (request.Tipo_AtendimentoSelecionado.HasValue)
                //    {
                //        condicoes.Add($"A.ID_TIPO_ATENDIMENTO = {request.Tipo_AtendimentoSelecionado}");
                //    }
                //    if (!String.IsNullOrEmpty(request.id_categoria_cliente))
                //    {
                //        condicoes.Add($"A.ID_CATEGORIA_CLIENTE = {request.id_categoria_cliente}");
                //    }

                //    condicoes.Add("A.STATUS = 1");


                //    //if (request != null && !String.IsNullOrEmpty(request.q))
                //    //{
                //    //    condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                //    //}

                //    if (condicoes.Count() > 0)
                //    {
                //        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
                //    }

                //    //using (var com = session.Connection.CreateCommand())
                //    //{
                //    //    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                //    //    sql.AppendLine(sqlFrom.ToString());

                //    //    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                //    //    {
                //    //        if (await reader.ReadAsync())
                //    //            retorno.total_count = Convert.ToInt32(reader["CO"]);
                //    //    }
                //    //}

                //    //Select
                //    using (var com = session.Connection.CreateCommand())
                //    {
                //        StringBuilder sql = new StringBuilder("SELECT A.ID, A.PRIORIDADE_ATENDIMENTO ");
                //        sql.AppendLine(sqlFrom.ToString());
                //        //sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                //        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                //        {
                //            while (await reader.ReadAsync())
                //            {
                //                tmpRetorno.Add(new PrioridadeAtendimento
                //                {
                //                    Id = Convert.ToInt64(reader["PRIORIDADE_ATENDIMENTO"])
                //                    //Text = Convert.ToString(reader["DESCRICAO"])
                //                });
                //                //retorno.Objeto = tmpRetorno;




                catch (Exception ex)
                {
                    return new GetApiRetorno<PrioridadeAtendimento>
                    {
                        Mensagem = ex.Message,
                        InnerException = ex.InnerException?.Message,
                        Ok = false
                    };
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/atendimentoapi/ReprovarAuditoria")]
        public async Task ReprovarAuditoria(AuditoriaRequest request)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtentimento = new Conexao<Atendimento>(session))
            {
                var atendimento = await repoAtentimento.selectFirst(x => x.Id == request.Id);

                atendimento.StatusAuditoria = StatusAuditoria.Reprovado;
                atendimento.JustificativaAuditoria = request.Justificativa;

                await repoAtentimento.Update(atendimento);
            }
        }


        [HttpPost()]
        [Route("api/atendimentoapi/AprovarAuditoria")]
        public async Task AprovarAuditoria(AuditoriaRequest request)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtentimento = new Conexao<Atendimento>(session))
            {
                var atendimento = await repoAtentimento.selectFirst(x => x.Id == request.Id);

                atendimento.StatusAuditoria = StatusAuditoria.Aprovado;
                atendimento.JustificativaAuditoria = request.Justificativa;

                await repoAtentimento.Update(atendimento);
            }
        }


        public async Task<string> RemoveVariaveis(string mensagem, AtendimentoChatMongo chat)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            {
                if (String.IsNullOrEmpty(mensagem))
                    return String.Empty;

                var atendimento = await repoAtendimento.selectFirst(p => p.Numero == chat.protocolo);

                if (atendimento.Cliente != null)
                {
                    mensagem = mensagem.Replace("@NOME", atendimento.Cliente.NomeFantasia);
                    mensagem = mensagem.Replace("@NOME_CONTATO", atendimento.Pessoa_Contato);
                    mensagem = mensagem.Replace("@SEXO", HtmlEnumExtensions.GetEnumDescription<Sexo>(Convert.ToString(atendimento.Cliente.Sexo)));
                    mensagem = mensagem.Replace("@CATEGORIA", HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>(Convert.ToString(atendimento.Cliente.Categoria_Cliente)));
                    mensagem = mensagem.Replace("@ID_CLIENTE", atendimento.Cliente.Id.ToString());
                    mensagem = mensagem.Replace("@CPF_CNPJ", atendimento.Cliente.CpfCnpj);
                    mensagem = mensagem.Replace("@DATA_NASCIMENTO", Convert.ToDateTime(atendimento.Cliente.Data_Nascimento).ToString("dd/MM/yyyy HH:mm"));
                    mensagem = mensagem.Replace("@TELEFONE_FIXO", atendimento.Cliente.Telefone_Fixo);
                    mensagem = mensagem.Replace("@TELEFONE_CELULAR", atendimento.Cliente.Telefone_Celular);
                    mensagem = mensagem.Replace("@ENDERECO", atendimento.Cliente.Endereco_Principal?.Logradouro);
                }

                mensagem = mensagem.Replace("@DATA_ABERTURA", Convert.ToDateTime(chat?.dataAbertura).ToString("dd/MM/yyyy HH:mm"));
                mensagem = mensagem.Replace("@DATA_ENCERRAMENTO", Convert.ToDateTime(chat?.dataEncerramento).ToString("dd/MM/yyyy HH:mm"));
                mensagem = mensagem.Replace("@PROTOCOLO", chat.protocolo);

                if (chat.cliente != null && chat.cliente?.idplanoProduto != null)
                {
                    var planoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == chat.cliente.idplanoProduto);

                    if (planoProduto != null)
                    {
                        if (!String.IsNullOrEmpty(planoProduto.Plano?.Descricao))
                        {
                            mensagem = mensagem.Replace("@PLANO_PRODUTO", planoProduto.Plano?.Descricao);
                        }
                        else
                        {
                            mensagem = mensagem.Replace("@PLANO_PRODUTO", planoProduto.Produto?.Descricao);
                        }
                    }
                }
            }
            return mensagem;
        }

        [HttpPost()]
        [Route("api/AtendimentoApi/GetAllConversas")]
        public async Task<DataTableResponse<AtendimentoChatListModel>> GetAllConversas(AtendimentoChat_DataTableRequest request)
        {

            List<AtendimentoChatListModel> retorno = new List<AtendimentoChatListModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var conexaoMongoSh = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongoSh))
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                {
                    List<AtendimentoChatMongo> allAtendimentosMongo = new List<AtendimentoChatMongo>();
                    List<AtendimentoChatMongo> allAtendimentosMongoPeriodo = new List<AtendimentoChatMongo>();
                    var dt_inicial = Convert.ToDateTime(request.Data_Inicial).AddHours(0).AddMinutes(0).AddSeconds(0);
                    var dt_final = Convert.ToDateTime(request.Data_Final).AddHours(23).AddMinutes(59).AddSeconds(59);

                    var search = !String.IsNullOrEmpty(request.Protocolo) ? request.Protocolo : String.Empty;
                    var somente_numeros = Regex.IsMatch(search, @"^\d+$");

                    if (!String.IsNullOrEmpty(request.Protocolo) && somente_numeros)
                        allAtendimentosMongo = await repositoryMongo.FindAll(a => a.protocolo == search);
                    else
                    {
                        search = request.Protocolo;
                        allAtendimentosMongo = await repositoryMongo.FindAll();
                    }

                    foreach (var att in allAtendimentosMongo)
                    {
                        if (!string.IsNullOrEmpty(att.dataAbertura))
                        {
                            var attAbertura = Convert.ToDateTime(att.dataAbertura);

                            if ((attAbertura >= dt_inicial && attAbertura <= dt_final) == false)
                                continue;

                            if (request.tem_Acesso == "False")
                            {
                                Atendimento attAniel = await repoAtendimento.selectFirst(x => x.Protocolo.IsLike(att.protocolo, MatchMode.Anywhere) && x.Responsavel.Id == User.Identity.GetUserId());

                                if (attAniel == null)
                                    continue;
                            }

                            if (!somente_numeros)
                            {
                                if (att.mensagens != null && att.mensagens.Count() > 0 && !string.IsNullOrEmpty(search))
                                {
                                    var contemTrechoMensagem = att.mensagens.Any(x => x.mensagem.ToUpper().Contains(search.ToUpper()));

                                    if (contemTrechoMensagem)
                                        allAtendimentosMongoPeriodo.Add(att);
                                }
                                else if (string.IsNullOrEmpty(search)) //nao esta pesquisando no momento
                                    allAtendimentosMongoPeriodo.Add(att);
                            }
                            else
                                allAtendimentosMongoPeriodo.Add(att);
                        }
                    }

                    foreach (var item in allAtendimentosMongoPeriodo)
                    {
                        var attAniel = repoAtendimento.selectFirst(x => x.Protocolo.IsLike(item.protocolo, MatchMode.Anywhere)).Result;

                        if (!somente_numeros && !string.IsNullOrEmpty(search))
                        {
                            foreach (var msg in item.mensagens)
                            {
                                var msgContemTrecho = msg.mensagem.ToUpper().Contains(search.ToUpper());

                                if (msgContemTrecho)
                                {
                                    var dadosMensagem = await GetDataAtravesMensagem(item.protocolo, msg.id);

                                    retorno.Add(new AtendimentoChatListModel
                                    {
                                        Id = Convert.ToInt32(attAniel.Id),
                                        Protocolo = item.protocolo,
                                        Hora = dadosMensagem.hora,
                                        UltimaMensagem = dadosMensagem.descricao,
                                        QuantidadeMensagens = Convert.ToInt32(dadosMensagem.totalMensagens),
                                        dataUltimaMensagem = Convert.ToDateTime(dadosMensagem.dataUltimaMensagem),
                                        ordenacao = dadosMensagem.ordenacao,
                                        htmlMensagem = dadosMensagem.htmlMensagem,
                                        dataAbertura = Convert.ToDateTime(dadosMensagem.dataAbertura),
                                    });
                                }
                            }
                        }
                        else
                        {
                            var ultimaMensagem = await GetDataUltimaMensagem(item.protocolo, search);

                            retorno.Add(new AtendimentoChatListModel
                            {
                                Id = Convert.ToInt32(attAniel.Id),
                                Protocolo = item.protocolo,
                                Hora = ultimaMensagem.hora,
                                UltimaMensagem = ultimaMensagem.descricao,
                                QuantidadeMensagens = Convert.ToInt32(ultimaMensagem.totalMensagens),
                                dataUltimaMensagem = Convert.ToDateTime(ultimaMensagem.dataUltimaMensagem),
                                ordenacao = ultimaMensagem.ordenacao,
                                htmlMensagem = ultimaMensagem.htmlMensagem,
                                dataAbertura = Convert.ToDateTime(ultimaMensagem.dataAbertura),
                            });
                        }
                    }

                    var lista_atendimentos_convidados = await repositoryMongo.FindAll(x => x.usuariosConvidados.Any(y => y.usuarioAceite == User.Identity.GetUserId()));
                    if (lista_atendimentos_convidados.Count > 0)
                    {
                        lista_atendimentos_convidados = string.IsNullOrEmpty(search) ? lista_atendimentos_convidados : lista_atendimentos_convidados.Where(x => x.protocolo.Contains(search) ||
                                                                                                 x.mensagens.Any(y => y.mensagem != null &&
                                                                                                                      y.mensagem.ToUpper().Contains(search.ToUpper())
                                                                                                                )
                                                                                        ).ToList();

                        foreach (var item in lista_atendimentos_convidados)
                        {
                            var ultimaMensagem = await GetDataUltimaMensagem(item.protocolo, search);
                            if (ultimaMensagem.descricao != null)
                            {
                                retorno.Add(new AtendimentoChatListModel
                                {
                                    Id = Convert.ToInt32(repoAtendimento.selectFirst(x => x.Protocolo == item.protocolo)?.Result?.Id),
                                    Protocolo = item.protocolo,
                                    Hora = ultimaMensagem.hora,
                                    UltimaMensagem = ultimaMensagem.descricao,
                                    QuantidadeMensagens = Convert.ToInt32(ultimaMensagem.totalMensagens),
                                    dataUltimaMensagem = Convert.ToDateTime(ultimaMensagem.dataUltimaMensagem),
                                    ordenacao = ultimaMensagem.ordenacao,
                                    htmlMensagem = ultimaMensagem.htmlMensagem,
                                    dataAbertura = Convert.ToDateTime(ultimaMensagem.dataAbertura),
                                });
                            }
                        }
                    }
                }

                if (Convert.ToInt32(request.status_Mensagem) == 1)
                {
                    retorno = retorno.Where(p => p.QuantidadeMensagens > 0).ToList();
                }
                else if (Convert.ToInt32(request.status_Mensagem) == 2)
                {
                    retorno = retorno.Where(p => p.QuantidadeMensagens == 0).ToList();
                }

                retorno = retorno.OrderBy(p => p.ordenacao).ThenByDescending(p => p.dataUltimaMensagem).ToList();
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new DataTableResponse<AtendimentoChatListModel>
                {
                    draw = request.draw,
                    recordsFiltered = 0,
                    data = new List<AtendimentoChatListModel>(),
                    error = ex.Message
                };
            }

            //Page e retornando DataTable
            return new DataTableResponse<AtendimentoChatListModel>
            {
                draw = request.draw,
                recordsFiltered = retorno.Count(),
                data = retorno,
                error = ""
            };
        }

        [HttpGet()]
        [Route("api/atendimentoapi/GetNomeUsuario")]
        public async Task<string> GetNomeUsuario(String usuario)
        {
            var retorno = "";
            int recordsTotal = 0;
            using (ISession session = HibernateUtil.GetSession())
            {
                //Filtros 
                if (String.IsNullOrEmpty(usuario))
                {
                    return null;
                }
                else
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sqlFrom = new StringBuilder();
                        sqlFrom.AppendLine("FROM ASPNETUSERS A");
                        sqlFrom.AppendLine("WHERE (UPPER(A.ID) LIKE @VALOR)");

                        parametros.Add("@VALOR", $"%{usuario.ToUpperInvariant()}%");

                        StringBuilder sqlCount = new StringBuilder();
                        sqlCount.AppendLine("SELECT COUNT(A.ID) CO");
                        sqlCount.AppendLine(sqlFrom.ToString());
                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.NOME");
                        sql.AppendLine(sqlFrom.ToString());


                        if (recordsTotal > 0)
                        {
                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    retorno = Convert.ToString(reader["ID"]) + " - " + Convert.ToString(reader["NOME"]);
                                }
                            }
                            return retorno;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public async Task<(String descricao, String totalMensagens, String hora, String dataUltimaMensagem, int ordenacao, String htmlMensagem, String dataAbertura)> GetDataUltimaMensagem(string protocolo, string search)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {
                (String descricao, String totalMensagens, String hora, String dataUltimaMensagem, int ordenacao, String htmlMensagem, String dataAbertura) dataSeparador = (null, null, null, null, 0, "<small></small>", null);
                AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == protocolo);

                if (chat != null)
                {
                    if (chat.mensagens == null)
                    {
                        chat.mensagens = new List<Mensagem_ChatModels>();
                    }

                    if (chat.mensagens.Count > 0)
                    {
                        //Data Abertura
                        dataSeparador.dataAbertura = chat.dataAbertura;

                        //Total mensagens não lidas
                        var total = chat.mensagens.Where(p => p.statusEnvio == StatusEnvioEnum.ENVIADO && p.enviadoPeloSuporte == false).Count();
                        dataSeparador.totalMensagens = Convert.ToString(total);

                        if (string.IsNullOrEmpty(search))
                        {
                            String dataHora = chat.mensagens.Select(up => up.dataHora).Max();
                            Mensagem_ChatModels mensagem = chat.mensagens.First(p => p.dataHora == dataHora);

                            dataSeparador.dataUltimaMensagem = dataHora;
                            dataSeparador.descricao = mensagem.mensagem;
                            DateTime dt = DateTime.Parse(String.Format("{0:HH:mm}", dataHora), new CultureInfo("pt-BR"));
                            dataSeparador.hora = String.Format("{0:HH:mm}", dt);

                        }
                        else
                        {
                            Mensagem_ChatModels mensagem = chat.mensagens.Where(p => p.mensagem != null && p.mensagem.ToUpper().Contains(search.ToUpper())).OrderByDescending(p => Convert.ToDateTime(p.dataHora)).FirstOrDefault();
                            if (mensagem == null)
                            {
                                var dataHoras = chat.mensagens.Select(up =>
                                {
                                    DateTime retorno = DateTime.MinValue;
                                    DateTime.TryParse(up.dataHora, out retorno);
                                    return retorno;
                                });

                                var dataHora = dataHoras.Max();

                                foreach (var item in chat.mensagens)
                                {
                                    DateTime retorno = DateTime.MinValue;
                                    DateTime.TryParse(item.dataHora, out retorno);

                                    if (retorno == DateTime.MinValue)
                                        continue;

                                    if (retorno == dataHora)
                                    {
                                        mensagem = item;
                                        break;
                                    }
                                }

                            }

                            dataSeparador.dataUltimaMensagem = mensagem.dataHora;
                            dataSeparador.descricao = mensagem.mensagem;
                            DateTime dt = DateTime.Parse(String.Format("{0:HH:mm}", mensagem.dataHora), new CultureInfo("pt-BR"));
                            dataSeparador.hora = String.Format("{0:HH:mm}", dt);
                        }

                        //Verifica o dia e hora
                        String dataHoraMax = chat.mensagens.Select(p => p.dataHora).Max();
                        DateTime? thisDay = DateTime.Today;

                        TimeSpan date = Convert.ToDateTime(thisDay).Date - Convert.ToDateTime(dataHoraMax).Date;

                        if (total > 0)
                        {
                            dataSeparador.htmlMensagem = String.Concat($"<small id='naoLidas{protocolo}' style='float: right; background-color:red; color: white; border-radius: 25px; width:13px;text-align:center;'>", total, "</small>");
                        }

                        var parametro = date.Days;
                        if (parametro == 0)
                        {
                            DateTime dte = DateTime.Parse(String.Format("{0:HH:mm}", dataHoraMax), new CultureInfo("pt-BR"));
                            dataSeparador.hora = String.Format("{0:HH:mm}", dte);
                            dataSeparador.ordenacao = 0;
                        }
                        else if (parametro == 1)
                        {
                            dataSeparador.hora = "Ontem";
                            dataSeparador.ordenacao = 1;
                        }
                        else
                        {
                            DateTime dte = DateTime.Parse(String.Format("{0:dd/MM/yyyy}", dataHoraMax), new CultureInfo("pt-BR"));
                            dataSeparador.hora = String.Format("{0:dd/MM/yyyy}", dte);
                            dataSeparador.ordenacao = 2;
                        }


                    }
                    else
                    {
                        dataSeparador.hora = "";
                        dataSeparador.totalMensagens = "";
                        dataSeparador.descricao = "";
                        dataSeparador.dataUltimaMensagem = "";
                        dataSeparador.ordenacao = 0;
                    }
                }

                return dataSeparador;
            }
        }

        public async Task<(String descricao, String totalMensagens, String hora, String dataUltimaMensagem, int ordenacao, String htmlMensagem, String dataAbertura)> GetDataAtravesMensagem(string protocolo, string idMensagem)
        {
            using (var conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            {
                (String descricao, String totalMensagens, String hora, String dataUltimaMensagem, int ordenacao, String htmlMensagem, String dataAbertura) dataSeparador = (null, null, null, null, 0, "<small></small>", null);
                AtendimentoChatMongo chat = await repository.Find(p => p.protocolo == protocolo);

                if (chat != null)
                {
                    if (chat.mensagens == null)
                    {
                        chat.mensagens = new List<Mensagem_ChatModels>();
                    }

                    if (chat.mensagens.Count > 0)
                    {
                        var mensagem = chat.mensagens.FirstOrDefault(x => x.id == idMensagem);

                        //Data Abertura
                        dataSeparador.dataAbertura = chat.dataAbertura;

                        //Total mensagens não lidas
                        var total = chat.mensagens.Where(p => p.statusEnvio == StatusEnvioEnum.ENVIADO && p.enviadoPeloSuporte == false).Count();
                        dataSeparador.totalMensagens = Convert.ToString(total);
                        String dataHora = mensagem.dataHora;
                        dataSeparador.dataUltimaMensagem = dataHora;
                        dataSeparador.descricao = mensagem.mensagem;
                        DateTime dt = DateTime.Parse(String.Format("{0:HH:mm}", dataHora), new CultureInfo("pt-BR"));
                        dataSeparador.hora = String.Format("{0:HH:mm}", dt);

                        //Verifica o dia e hora
                        String dataHoraMax = mensagem.dataHora;
                        DateTime? thisDay = DateTime.Today;

                        TimeSpan date = Convert.ToDateTime(thisDay).Date - Convert.ToDateTime(dataHoraMax).Date;

                        if (total > 0)
                        {
                            dataSeparador.htmlMensagem = String.Concat($"<small id='naoLidas{protocolo}' style='float: right; background-color:red; color: white; border-radius: 25px; width:13px;text-align:center;'>", total, "</small>");
                        }

                        var parametro = date.Days;
                        if (parametro == 0)
                        {
                            DateTime dte = DateTime.Parse(String.Format("{0:HH:mm}", dataHoraMax), new CultureInfo("pt-BR"));
                            dataSeparador.hora = String.Format("{0:HH:mm}", dte);
                            dataSeparador.ordenacao = 0;
                        }
                        else if (parametro == 1)
                        {
                            dataSeparador.hora = "Ontem";
                            dataSeparador.ordenacao = 1;
                        }
                        else
                        {
                            DateTime dte = DateTime.Parse(String.Format("{0:dd/MM/yyyy}", dataHoraMax), new CultureInfo("pt-BR"));
                            dataSeparador.hora = String.Format("{0:dd/MM/yyyy}", dte);
                            dataSeparador.ordenacao = 2;
                        }

                    }
                    else
                    {
                        dataSeparador.hora = "";
                        dataSeparador.totalMensagens = "";
                        dataSeparador.descricao = "";
                        dataSeparador.dataUltimaMensagem = "";
                        dataSeparador.ordenacao = 0;
                    }
                }

                return dataSeparador;
            }
        }

        [HttpGet()]
        [Route("api/AtendimentoApi/ConsultaNumAtendimentos")]
        public async Task<PostApiRetorno<int>> ConsultanumAtendimentos(long Id_Cliente)
        {
            var retorno = new PostApiRetorno<int>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var comm = session.Connection.CreateCommand())
                using (var repoAtendimento = new Conexao<Atendimento>(session))
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine($"SELECT COUNT(A.ID) AS IDS FROM TB_ATENDIMENTO A LEFT JOIN TB_CLIENTE B ON A.ID_CLIENTE = B.ID WHERE B.ID = '{Id_Cliente}' ");
                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, new Dictionary<String, Object>(), session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno.IdObjeto = Convert.ToInt32(reader["IDS"]);
                            retorno.Ok = true;
                        }
                        else
                        {
                            retorno.Ok = false;
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                //           --      Pro Tip         -- 
                //  --    Use Shift + F9 para debuggar o erro     --
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/AtendimentoApi/HistoricoOs")]
        public async Task<DataTableResponse<VincularOsResponse>> HistoricoOs(VincularAtendimento_DataTableRequest request)
        {
            var retorno = new DataTableResponse<VincularOsResponse> { draw = request.draw };
            List<VincularOsResponse> possiveisOsvinculo = new List<VincularOsResponse>();
            Int32 recordsTotal = 0;
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Documento_ProducaoApiController documentoApi = new Documento_ProducaoApiController())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder();

                    sqlFrom.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ON B.COD_TIPO_SERV = A.COD_TIPO_SERV ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO C ON C.COD_SUB_TIPO_SERV = A.COD_SUB_TIPO_SERV AND C.COD_TIPO_SERV = A.COD_TIPO_SERV ");
                    sqlFrom.AppendLine(" INNER JOIN TB_CLIENTE D ON D.COD_CLIENTE = A.ID_CLIENTE ");
                    sqlFrom.AppendLine(" WHERE (A.DIA BETWEEN @DATA_INICIAL AND @DATA_FINAL) ");

                    try
                    {
                        DateTime dataInicial = Convert.ToDateTime(request.DataInicial);
                        DateTime dataFinal = Convert.ToDateTime(request.DataFinal);

                        if ((dataFinal - dataInicial).TotalDays > 30)
                        {
                            throw new Exception("Período maior que 30 dias");
                        }
                        sqlFrom.AppendLine("  ");
                        parametros.Add("@DATA_INICIAL", request.DataInicial);
                        parametros.Add("@DATA_FINAL", request.DataFinal);

                    }
                    catch
                    {
                        throw new Exception("Periodo Inválido");
                    }

                    if (request.Id_Tipo_Atendimento.HasValue)
                    {
                        sqlFrom.AppendLine("AND B.COD_TIPO_SERV = @ID_TIPO_ATENDIMENTO");
                        parametros.Add("@ID_TIPO_ATENDIMENTO", request.Id_Tipo_Atendimento);
                    }

                    if (!string.IsNullOrEmpty(Convert.ToString(request.Status)))
                    {
                        sqlFrom.AppendLine("AND A.STATUS_PROD = @STATUS_OS");
                        parametros.Add("@STATUS_OS", request.Status);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(request.Responsavel)))
                    {
                        sqlFrom.AppendLine("AND A.USUARIO = @ID_RESPONSAVEL");
                        parametros.Add("@ID_RESPONSAVEL", request.Responsavel);
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(request.Id_Cliente)))
                    {
                        sqlFrom.AppendLine("AND A.ID_CLIENTE = @ID_CLIENTE");
                        parametros.Add("@ID_CLIENTE", request.Id_Cliente);
                    }
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(A.NUM_OBRA_ORIGINAL) LIKE UPPER(@SEARCH)) OR (UPPER(A.NUM_OBRA) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(@"SELECT A.NUM_OBRA,B.DESCRICAO AS TIPO_SERVICO,C.DESCRICAO AS SUB_TIPO_SERVICO, CASE WHEN D.FANTASIA IS NULL THEN D.RAZAO_SOCIAL || ' - ' || D.CNPJ ELSE D.FANTASIA  END AS CLIENTE ,CAST(A.DIA || ' ' || A.HORA AS TIMESTAMP) AS DATA_HORA ,A.DATA_SLA,A.USUARIO,A.PROJETO,A.CODCT ,A.NUM_DOC,A.STATUS_PROD ");
                        sql.AppendLine(sqlFrom.ToString());

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                DateTime? dataAbertura = reader["DATA_HORA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA"]) : (DateTime?)null;
                                DateTime? dataSla = reader["DATA_SLA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_SLA"]) : (DateTime?)null;

                                var desc_user_name = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : String.Empty;
                                var doc_numero = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]) : String.Empty;
                                possiveisOsvinculo.Add(new VincularOsResponse
                                {
                                    num_Os = doc_numero,
                                    tipoServico = reader["TIPO_SERVICO"] != DBNull.Value ? Convert.ToString(reader["TIPO_SERVICO"]) : String.Empty,
                                    subTipoServico = reader["SUB_TIPO_SERVICO"] != DBNull.Value ? Convert.ToString(reader["SUB_TIPO_SERVICO"]) : String.Empty,
                                    cliente = reader["CLIENTE"] != DBNull.Value ? Convert.ToString(reader["CLIENTE"]) : String.Empty,
                                    dataHoraCriacao = dataAbertura.HasValue ? dataAbertura.Value.ToString("dd/MM/yyyy HH:mm:ss") : String.Empty,
                                    dataSla = dataSla.HasValue ? dataSla.Value.ToString("dd/MM/yyyy HH:mm:ss") : String.Empty,
                                    responsavel = desc_user_name,
                                    status = reader["STATUS_PROD"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_PROD"])) : String.Empty,
                                    projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : String.Empty,
                                    codCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : String.Empty,
                                    Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToString(reader["NUM_DOC"]) : String.Empty,
                                });
                            }
                        }
                    }
                }
                retorno.recordsFiltered = recordsTotal;
                retorno.data = possiveisOsvinculo;
            }
            catch (Exception ex)
            {
                //retorno.recordsTotal = recordsTotal;
                retorno.recordsFiltered = 0;
                retorno.data = new List<VincularOsResponse>();
                retorno.error = ex.Message;
            }

            //Page e retornando DataTable
            return retorno;
        }

        [HttpPost]
        [Route("api/AtendimentoApi/HistoricoVincular")]
        public async Task<DataTableResponse<HistoricoVincularResponse>> HistoricoVincular([FromBody] HistoricoVincularResponse_DataTableRequest request)
        {
            List<HistoricoVincularResponse> retorno = new List<HistoricoVincularResponse>();
            IOrderedEnumerable<HistoricoVincularResponse> listaOrdenada = null;
            var response = new DataTableResponse<HistoricoVincularResponse>
            {
                draw = request.draw,
                data = new List<HistoricoVincularResponse>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<VincularAtendimento>(session))
            using (var repositoryCliente = new Conexao<Cliente>(session))
            using (var repositoryTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
            using (var repositorySubTipoServico = new Conexao<Sub_Tipo_ServicoAniel>(sessionT5))
            using (var repositoryOS = new Conexao<Documento_ProducaoAniel>(sessionT5))
            {
                try
                {
                    var atendimentos = await repository.select(p => p.Numero_Atendimento == request.NumeroAtendimento);

                    foreach (var atendimento in atendimentos)
                    {
                        var ordensServico = await repositoryOS.select(p =>
                            p.CodCt == atendimento.CodCT &&
                            p.Projeto == atendimento.Projeto &&
                            p.Num_Doc == Convert.ToInt64(atendimento.Num_Doc) &&
                            p.Num_Obra == atendimento.Num_Obra
                        );

                        foreach (var os in ordensServico)
                        {
                            var tipoServico = await repositoryTipoServico.selectFirst(p => p.Cod_Tipo_Serv == os.TipoServico.Cod_Tipo_Serv);
                            var subTipoServico = await repositorySubTipoServico.selectFirst(p => p.Codigo_Sub_Tipo == os.Cod_Sub_Tipo_Serv);
                            var cliente = os.Cliente != null ? await repositoryCliente.selectFirst(p => p.Id == os.Cliente.Cod_Cliente) : null;
                            DateTime? datahoraSla = null;
                            datahoraSla = os.Data_Indicador;
                            if (os.Hora_Indicador.HasValue && datahoraSla != null)
                            {
                                datahoraSla = datahoraSla.Value.Add(os.Hora_Indicador.Value.TimeOfDay);
                            }

                            retorno.Add(new HistoricoVincularResponse
                            {
                                num_Os = os.Num_Obra,
                                Num_Doc = os.Num_Doc.ToString(),
                                projeto = os.Projeto,
                                codCt = os.CodCt,
                                cliente = cliente?.NomeFantasia ?? "N/A",
                                TipoServico = tipoServico?.Descricao ?? "N/A",
                                SubTipoServico = subTipoServico?.Descricao ?? "N/A",
                                DataCriacao = os.Dia?.ToString("dd/MM/yyyy") ?? "N/A",
                                dataSla = datahoraSla?.ToString("dd/MM/yyyy") ?? "N/A",
                                usuario = os.Usuario,
                                status = os.Status.HasValue ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(os.Status)) : null
                            });
                        }
                    }

                    response.recordsFiltered = retorno.Count;
                    response.data = retorno;
                }
                catch (Exception ex)
                {
                    response.error = "Erro interno: " + ex.Message + (ex.InnerException?.Message ?? "");
                }
            }

            return response;
        }

        [HttpPost]
        [Route("api/AtendimentoApi/DesvincularOS")]
        public async Task<PostApiRetorno<Int64>> DesvincularOS([FromBody] AcompanhamentoListaModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<VincularAtendimento>(session))
            {
                try
                {
                    VincularAtendimento modelo = null;

                    if (request != null)
                    {
                        modelo = (await repository.select(p => p.Numero_Atendimento == request.IdAtendimento && p.Projeto == request.Projeto && p.CodCT == request.CodCt && p.Num_Obra == request.Num_OS && p.Num_Doc == Convert.ToString(request.Num_Doc))).FirstOrDefault();

                        if (modelo != null)
                        {
                            await repository.Remove(modelo);
                            retorno.Ok = true;
                            retorno.Mensagem = "OS desvinculada com sucesso.";
                        }
                        else
                        {

                            throw new Exception("Vínculo não encontrado.");

                        }

                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/AtendimentoApi/VincularOs")]
        public async Task<PostApiRetorno<Int64>> VincularOs([FromBody] AcompanhamentoListaModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<VincularAtendimento>(session))
            {
                try
                {
                    VincularAtendimento modelo = null;

                    if (request != null)
                    {
                        modelo = (await repository.select(p => p.Numero_Atendimento == request.IdAtendimento && p.Projeto == request.Projeto && p.CodCT == request.CodCt && p.Num_Obra == request.Num_OS && p.Num_Doc == Convert.ToString(request.Num_Doc))).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new VincularAtendimento
                            {
                                Numero_Atendimento = request.IdAtendimento,
                                CodCT = request.CodCt,
                                Projeto = request.Projeto,
                                Num_Obra = request.Num_OS,
                                Num_Doc = Convert.ToString(request.Num_Doc),
                                UsuarioCriacao = User.Identity.GetUserId(),
                                DataHoraCriacao = DateTime.Now,
                            };
                        }
                        else
                        {
                            throw new Exception("Atendimento já está vinculado a essa ordem de serviço");
                        }

                        await repository.SaveOrUpdate(modelo);


                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        public async Task<bool> ConsultaTransferenciaBot(AtendimentoChatMongo chat)
        {
            bool retorno = false;

            using (var conexaoMongo = new ConexaoMongo())
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new RepositoryMongo<AtendimentoChatMongo>(conexaoMongo))
            using (var repositoryBot = new Conexao<Bot>(session))
            using (var repositoryCanal = new Conexao<Canal_Comunicacao>(session))
            {
                if (chat.bot)
                {
                    Canal_Comunicacao canal = await repositoryCanal.selectFirst(p => p.Id == chat.idCanal);

                    if (canal?.Bot != null)
                    {
                        Bot bot = await repositoryBot.selectFirst(p => p.Id == canal.Bot.Id);

                        if (bot?.Minutos.HasValue == true)
                        {
                            var mensagemMaisAntiga = chat.mensagens.OrderBy(m => m.dataHora).FirstOrDefault();

                            if (mensagemMaisAntiga != null && DateTime.TryParse(mensagemMaisAntiga.dataHora, out DateTime dataHoraMensagem))
                            {
                                var tempoDecorrido = DateTime.UtcNow - dataHoraMensagem;

                                if (tempoDecorrido.TotalMinutes > bot.Minutos.Value)
                                {
                                    retorno = true;
                                }
                            }
                        }
                    }
                }
            }

            return retorno;
        }


        public class AuditoriaRequest
        {
            public long Id { get; set; }
            public string Justificativa { get; set; }
        }

    }

}
