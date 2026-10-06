using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using FirebirdSql.Data.FirebirdClient;
using log4net;
using NHibernate;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.repository;
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
    public class RelatorioLogProvisionamentoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/RelatorioLogProvisionamentoapi/GetAll")]
        public async Task<DataTableResponseDynamicColumns<RelatorioLogProvisionamento_Model>> GetAll(RelatorioLogProvisionamento_DataTableRequest request)
        {           
            List<RelatorioLogProvisionamento_Model> retorno = new List<RelatorioLogProvisionamento_Model>();
            Int32 recordsTotal = 0;
            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                #region sqlWhere
                StringBuilder sqlWhere = new StringBuilder(" WHERE (SERVICO = 'ATIVAÇÃO DE SINAL COM TOUCH' OR SERVICO = 'ATIVAÇÃO DE SINAL PELO CONNECT') ");
                if (!String.IsNullOrEmpty(request.Data_Inicio))
                {
                    DateTime date = DateTime.ParseExact(request.Data_Inicio, "dd/MM/yyyy", null);
                    string dataFormatada = date.ToString("yyyy-MM-dd");
                    sqlWhere.AppendLine(" AND DATA_HORA LIKE @DATA_INI ");
                    parametros.Add("@DATA_INI", dataFormatada + "%");
                }

                /* if (!String.IsNullOrEmpty(request.Data_Fim))
                 {
                     sqlWhere.AppendLine(" AND DATA_HORA <= CAST(@DATA_FIM AS TIMESTAMP) ");
                     parametros.Add("@DATA_FIM", Convert.ToDateTime(request.Data_Fim).AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy-MM-dd HH:mm:ss"));
                 } */

                if (!String.IsNullOrEmpty(request.Num_OS))
                {
                    sqlWhere.AppendLine($" AND DETALHE LIKE '%{request.Num_OS}%' ");
                }

                if (!String.IsNullOrEmpty(request.Usuario))
                {
                    sqlWhere.AppendLine($" AND USUARIO LIKE '%{request.Usuario}%' ");
                }

                if(!String.IsNullOrEmpty(request.Id_OLT) && String.IsNullOrEmpty(request.Serial))                    
                {
                    var descricao = await GetDescricaoOLTByID(request.Id_OLT);
                    if(!String.IsNullOrEmpty(descricao))
                    {
                        sqlWhere.AppendLine($" AND XML_REQUEST LIKE '%{descricao}%'");
                    }                  
                }

                if (String.IsNullOrEmpty(request.Id_OLT) && !String.IsNullOrEmpty(request.Serial))
                {
                    sqlWhere.AppendLine($" AND UPPER(XML_REQUEST) LIKE UPPER('%{request.Serial}%')");

                }

                if (!String.IsNullOrEmpty(request.Id_OLT) && !String.IsNullOrEmpty(request.Serial))
                {
                    var descricao = await GetDescricaoOLTByID(request.Id_OLT);
                    if (!String.IsNullOrEmpty(descricao))
                    {
                        sqlWhere.AppendLine($" AND XML_REQUEST LIKE '%{descricao}%' AND UPPER(XML_REQUEST) LIKE UPPER('%{request.Serial}%')");
                    }                      

                }
                #endregion

                StringBuilder sqlOrderBy = new StringBuilder(" ORDER BY DETALHE, DATA_HORA ");

                #region Count
                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_LOG_COMUNICACAO ");
                    sql.Append(sqlWhere);
                    //sql.Append(sqlOrderBy);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                #endregion

                #region Select
                //Select
                using (var com = session.Connection.CreateCommand())
                {                    

                    StringBuilder sql = new StringBuilder(@" SELECT ID, SERVICO, DETALHE, DATA_HORA, USUARIO, XML_REQUEST, XML_RESPONSE, TIPO
                                                                    FROM TB_LOG_COMUNICACAO ");
                    sql.Append(sqlWhere);
                    sql.Append(sqlOrderBy);

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new RelatorioLogProvisionamento_Model();

                            obj.Id = Convert.ToInt64(reader["ID"]);
                            obj.Servico = reader["SERVICO"] != DBNull.Value ? Convert.ToString(reader["SERVICO"]) : String.Empty;
                            obj.Detalhe = reader["DETALHE"] != DBNull.Value ? Convert.ToString(reader["DETALHE"]) : String.Empty;                           
                            obj.Data_Hora_Str = reader["DATA_HORA"] != DBNull.Value ? Convert.ToString(reader["DATA_HORA"]) : String.Empty;
                          
                            if (!String.IsNullOrEmpty(obj.Data_Hora_Str)) 
                                obj.Data_Hora = DateTime.Parse(obj.Data_Hora_Str);

                            obj.Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : String.Empty;
                            obj.Xml_Request = reader["XML_REQUEST"] != DBNull.Value ? Convert.ToString(reader["XML_REQUEST"]) : String.Empty;
                            obj.Xml_Response = reader["XML_RESPONSE"] != DBNull.Value ? Convert.ToString(reader["XML_RESPONSE"]) : String.Empty;

                            //Formata o inicio do request para exibir como Etapa no log
                            if(!String.IsNullOrEmpty(obj.Xml_Request))
                            {
                                var index = obj.Xml_Request.IndexOf("->");
                                if (index <= 0)
                                    index = obj.Xml_Request.Length;

                                obj.Etapa = index > 0 ? obj.Xml_Request.Substring(0, index) : string.Empty;
                            }
                            

                            retorno.Add(obj);
                        }
                    }
                    #endregion
                }
            }

            if(retorno.Count > 0)
            {
                var listanumObra = retorno.Select(p => new { p.Detalhe, p.Usuario}).Distinct().ToList();
                foreach(var item in listanumObra)
                {
                    RegistroAtivacao registroAtivacao = null;
                    using (ISession session = HibernateUtil.GetSessionT5())
                    {
                        using (var com = session.Connection.CreateCommand())
                        {
                            Dictionary<String, Object> parametros = new Dictionary<string, object>();
                            StringBuilder sql = new StringBuilder("SELECT FIRST 1 ERRO, EXCEPTION FROM TB_REGISTRO_ATIVACAO ");
                            sql.AppendLine($" WHERE NUM_OBRA LIKE '%{item.Detalhe}%' AND EQUIPE = @EQUIPE ");
                            sql.AppendLine($" ORDER BY DATA_HORA DESC ");

                            parametros.Add("@EQUIPE", item.Usuario);

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                if (await reader.ReadAsync())
                                {
                                    registroAtivacao = new RegistroAtivacao();
                                    registroAtivacao.Erro = reader["ERRO"] != DBNull.Value ? Convert.ToString(reader["ERRO"]) == "S" : false;
                                    registroAtivacao.Exception = reader["EXCEPTION"] != DBNull.Value ? Convert.ToString(reader["EXCEPTION"]) : null;                                   
                                }
                            }
                        }
                    }
                   
                    if(registroAtivacao != null)
                    {
                        retorno.ForEach(p => {
                            if(p.Detalhe.Equals(item.Detalhe))
                            {
                                p.Erro_Provisionamento = registroAtivacao.Erro;
                                p.Msg_Erro = registroAtivacao.Exception;
                            }                            
                        });
                    }                       
                }
            }
            

            var results = new List<Connect.Core.Column>();

            results.Add(new Connect.Core.Column { data = "id", title = RsCadastros.ID, className = "text-center", visible = true, orderable = false });
            results.Add(new Connect.Core.Column { data = "servico", title = RsCadastros.SERVICO, className = "text-center", visible = true, orderable = false });
            results.Add(new Connect.Core.Column { data = "detalhe", title = RsCadastros.OS, className = "text-center", visible = true, orderable = false });
            results.Add(new Connect.Core.Column { data = "data_Hora_Str", title = RsCadastros.DATAHORA, className = "text-center", visible = true, orderable = false });
            results.Add(new Connect.Core.Column { data = "usuario", title = RsCadastros.EQUIPE, className = "text-center", visible = true, orderable = false });
            results.Add(new Connect.Core.Column { data = "etapa", title = RsCadastros.ETAPA, className = "text-center", visible = true, orderable = false });
            results.Add(new Connect.Core.Column { data = "xml_Request_Response", title = "Requisição/Resposta", className = "text-left", visible = true, orderable = false });
            results.Add(new Connect.Core.Column { data = "xml_Request", title = "Requisição", className = "text-left", visible = false, orderable = false });
            results.Add(new Connect.Core.Column { data = "xml_Response", title = "Resposta", className = "text-left", visible = false, orderable = false });

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                results = results
            });

            //Page e retornando DataTable
            return new DataTableResponseDynamicColumns<RelatorioLogProvisionamento_Model>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                columns = json,
                error = ""
            };
        }

        public async Task<string> GetDescricaoOLTByID(string Id_OLT)
        {
            string retorno = "";

            using (ISession session = HibernateUtil.GetSession())
            using (var repoOLT = new Conexao<Sinapse.Model.OLT>(session))
            {
                if(!String.IsNullOrEmpty(Id_OLT))
                {
                    var IdConvertido = Convert.ToInt64(Id_OLT);

                    var descricaoOLT = await repoOLT.selectFirst(p => p.Id == IdConvertido);

                    if(descricaoOLT != null)
                    {
                        retorno = descricaoOLT.Descricao;
                    }
                   
                }
            }

            return retorno;

        }

       
    }
}