using Aniel.Connect.ControllersWebApi.Ativacao.Repositorio;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.mongo;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;

//using Aniel.Connect.Models;
//using Aniel.Connect.Models.Ativacao;
using NHibernate;
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
using System.Globalization;
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
    public class Relatorio_CheckListApi : BaseApiController
    {
        [HttpPost()]
        [Route("api/Relatorio_CheckListApi/Modelo01")]
        public async Task<DataTableResponse<dynamic>> RelatorioCheckList(RelatorioCheckList request)
        {
            DateTime dataInicio = DateTime.ParseExact(request.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dataFim = DateTime.ParseExact(request.DataFim, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            List<dynamic> retorno = new List<dynamic>();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())
            {
                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();

                    parametros.Add("DATA_INI", dataInicio);
                    parametros.Add("DATA_FIM", dataFim);

                    sql.AppendLine(" SELECT A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, A.DATA_MAXIMA, " +
                                     " A.EQUIPE || ' ' || E.NOME AS EQUIPE, A.COD_TIPO_SERV, F.DESCRICAO AS TIPO_SERVICO, " +
                                     " A.COD_SUB_TIPO_SERV, G.DESCRICAO AS SUB_TIPO_SERVICO, A.TITULAR AS CLIENTE, " +
                                     " C.TITULO, C.PROTOCOLO, D.PERGUNTA,D.ORDEM, CAST(COALESCE(RESPOSTA, RESPOSTA_TEXTO, RESPOSTA_VALOR, RESPOSTA_DATA, NULL) AS VARCHAR(300)) AS RESPOSTA,D.ID  ");

                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_QUIZ_ANSWER_DOCPROD B ");
                    sql.AppendLine(" ON(A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_OBRA = B.NUM_OBRA AND A.NUM_DOC = B.NUM_DOC) ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_QUIZ_ANSWER_CAPA C ");
                    sql.AppendLine(" ON(B.ID_ANSWER_CAPA = C.ID) ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_QUIZ_ANSWER_ITEM D");
                    sql.AppendLine(" ON(C.ID = D.ID_ANSWER_CAPA)  ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_EQUIPE E ");
                    sql.AppendLine(" ON(A.EQUIPE = E.EQUIPE) ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_SUB_TIPO_SERVICO G ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) LEFT JOIN TB_QUIZ Q ON (Q.ID = C.ID_QUIZ) ");
                    sql.AppendLine(" WHERE D.TIPO <> 3  ");
                    //sql.AppendLine(" ORDER BY A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, C.PROTOCOLO, D.ORDEM ");


                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        sql.AppendLine("AND (A.CODCT = @CODCT)");
                        parametros.Add("@CODCT", request.Contrato);
                    }

                    if (!String.IsNullOrEmpty(request.Projeto))
                    {
                        sql.AppendLine("AND (A.PROJETO = @PROJETO)");
                        parametros.Add("@PROJETO", Convert.ToInt32(request.Projeto));
                    }

                    if (!String.IsNullOrEmpty(request.Tecnico))
                    {
                        sql.AppendLine("AND (E.EQUIPE = @TECNICO)");
                        parametros.Add("@TECNICO", request.Tecnico);
                    }

                    if (!String.IsNullOrEmpty(request.Questionario))
                    {
                        sql.AppendLine("AND (C.ID_QUIZ = @ID_QUIZ)");
                        parametros.Add("@ID_QUIZ", request.Questionario);
                    }

                    if (!String.IsNullOrEmpty(request.Nos))
                    {
                        sql.AppendLine("AND (A.NUM_DOC = @NUM_DOC)");
                        parametros.Add("@NUM_DOC", request.Nos);
                    }

                    if (!String.IsNullOrEmpty(request.Tipo_Servico))
                    {
                        sql.AppendLine("AND (F.COD_TIPO_SERV = @COD_TIPO_SERV)");
                        parametros.Add("@COD_TIPO_SERV", request.Tipo_Servico);
                    }

                    sql.AppendLine("AND CAST(A.DATA_MAXIMA AS date)  BETWEEN @DATA_INI AND @DATA_FIM");

                    sql.AppendLine(" ORDER BY A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, C.PROTOCOLO, D.ORDEM ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new
                            {
                                CodCt = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"]),
                                NUM_OBRA = Convert.ToString(reader["NUM_OBRA"]),
                                NUM_DOC = Convert.ToString(reader["NUM_DOC"]),
                                DATA = Convert.ToString(reader["DATA_MAXIMA"]),
                                Tecnico = Convert.ToString(reader["EQUIPE"]),
                                Tipo_Servico = Convert.ToString(reader["TIPO_SERVICO"]),
                                Sub_Tipo_Servico = Convert.ToString(reader["SUB_TIPO_SERVICO"]),
                                Cliente = Convert.ToString(reader["CLIENTE"]),
                                Titulo = Convert.ToString(reader["TITULO"]),
                                Protocolo = Convert.ToString(reader["PROTOCOLO"]),
                                Ordem = Convert.ToString(reader["ORDEM"]),
                                Pergunta = Convert.ToString(reader["PERGUNTA"]),
                                Resposta = Convert.ToString(reader["RESPOSTA"]),
                            });
                        }
                    }

                }
            }

            return new DataTableResponse<dynamic>
            {
                data = retorno,
                recordsFiltered = retorno.Count
            };
        }


        [HttpPost()]
        [Route("api/Relatorio_CheckListApi/Modelo02")]
        public async Task<DataTableResponse<dynamic>> RelatorioCheckListModelo02(RelatorioCheckList request)
        {
            DateTime dataInicio = DateTime.ParseExact(request.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dataFim = DateTime.ParseExact(request.DataFim, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            List<dynamic> retorno = new List<dynamic>();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())
            {
                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();

                    parametros.Add("DATA_INI", dataInicio);
                    parametros.Add("DATA_FIM", dataFim);

                    sql.AppendLine(" SELECT A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, A.DATA_MAXIMA, " +
                                     " A.EQUIPE || ' ' || E.NOME AS EQUIPE, A.COD_TIPO_SERV, F.DESCRICAO AS TIPO_SERVICO, " +
                                     " A.COD_SUB_TIPO_SERV, G.DESCRICAO AS SUB_TIPO_SERVICO, A.TITULAR AS CLIENTE, " +
                                     " C.TITULO, C.PROTOCOLO, D.PERGUNTA,D.ORDEM, CAST(COALESCE(RESPOSTA, RESPOSTA_TEXTO, RESPOSTA_VALOR, RESPOSTA_DATA, NULL) AS VARCHAR(300)) AS RESPOSTA,D.ID  ");

                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_QUIZ_ANSWER_DOCPROD B ");
                    sql.AppendLine(" ON(A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_OBRA = B.NUM_OBRA AND A.NUM_DOC = B.NUM_DOC) ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_QUIZ_ANSWER_CAPA C ");
                    sql.AppendLine(" ON(B.ID_ANSWER_CAPA = C.ID) ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_QUIZ_ANSWER_ITEM D");
                    sql.AppendLine(" ON(C.ID = D.ID_ANSWER_CAPA)  ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_EQUIPE E ");
                    sql.AppendLine(" ON(A.EQUIPE = E.EQUIPE) ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_TIPO_SERVICO_EQUIPE F ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV) ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_SUB_TIPO_SERVICO G ");
                    sql.AppendLine(" ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) LEFT JOIN TB_QUIZ Q ON (Q.ID = C.ID_QUIZ) ");
                    sql.AppendLine(" WHERE D.TIPO <> 3  ");
                    //sql.AppendLine(" ORDER BY A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, C.PROTOCOLO, D.ORDEM ");


                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        sql.AppendLine("AND (A.CODCT = @CODCT)");
                        parametros.Add("@CODCT", request.Contrato);
                    }

                    if (!String.IsNullOrEmpty(request.Projeto))
                    {
                        sql.AppendLine("AND (A.PROJETO = @PROJETO)");
                        parametros.Add("@PROJETO", Convert.ToInt32(request.Projeto));
                    }

                    if (!String.IsNullOrEmpty(request.Tecnico))
                    {
                        sql.AppendLine("AND (E.EQUIPE = @TECNICO)");
                        parametros.Add("@TECNICO", request.Tecnico);
                    }

                    if (!String.IsNullOrEmpty(request.Questionario))
                    {
                        sql.AppendLine("AND (C.ID_QUIZ = @ID_QUIZ)");
                        parametros.Add("@ID_QUIZ", request.Questionario);
                    }

                    if (!String.IsNullOrEmpty(request.Nos))
                    {
                        sql.AppendLine("AND (A.NUM_DOC = @NUM_DOC)");
                        parametros.Add("@NUM_DOC", request.Nos);
                    }

                    if (!String.IsNullOrEmpty(request.Tipo_Servico))
                    {
                        sql.AppendLine("AND (F.COD_TIPO_SERV = @COD_TIPO_SERV)");
                        parametros.Add("@COD_TIPO_SERV", request.Tipo_Servico);
                    }

                    sql.AppendLine("AND CAST(A.DATA_MAXIMA AS date)  BETWEEN @DATA_INI AND @DATA_FIM");

                    sql.AppendLine(" ORDER BY A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, C.PROTOCOLO, D.ORDEM ");

                    //1º Buscar os registros de capa
                    //2º Buscar as perguntas adicionando-as cada uma delas em uma 

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new
                            {
                                CodCt = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"]),
                                NUM_OBRA = Convert.ToString(reader["NUM_OBRA"]),
                                NUM_DOC = Convert.ToString(reader["NUM_DOC"]),
                                DATA = Convert.ToString(reader["DATA_MAXIMA"]),
                                Tecnico = Convert.ToString(reader["EQUIPE"]),
                                Tipo_Servico = Convert.ToString(reader["TIPO_SERVICO"]),
                                Sub_Tipo_Servico = Convert.ToString(reader["SUB_TIPO_SERVICO"]),
                                Cliente = Convert.ToString(reader["CLIENTE"]),
                                Titulo = Convert.ToString(reader["TITULO"]),
                                Protocolo = Convert.ToString(reader["PROTOCOLO"]),
                                Ordem = Convert.ToString(reader["ORDEM"]),
                                Pergunta = Convert.ToString(reader["PERGUNTA"]),
                                Resposta = Convert.ToString(reader["RESPOSTA"]),
                            });
                        }

                        retorno = retorno.OrderBy(p => Convert.ToInt32(p.Ordem)).ToList();
                    }

                }
            }

            return new DataTableResponse<dynamic>
            {
                data = retorno,
                recordsFiltered = retorno.Count
            };
        }

    }
}