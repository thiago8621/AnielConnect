using Aniel.Connect.Core;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using static Aniel.Connect.Models.TelegramModels;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Historico_NfApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Historico_NfApi/GetAll")]
        public async Task<DataTableResponse<Historico_NfViewModel>> GetAll(Historico_Nf_DataTableRequest request)
        {
            List<Historico_NfViewModel> retorno = new List<Historico_NfViewModel>();
            IOrderedEnumerable<Historico_NfViewModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                //Filtros 
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_NOTA_FISCAL A ");

                sqlFrom.AppendLine("LEFT JOIN TB_CONTA_RECEBER B ");
                sqlFrom.AppendLine("ON A.ID = B.ID_NOTA_FISCAL ");

                sqlFrom.AppendLine("LEFT JOIN TB_CLIENTE C ");
                sqlFrom.AppendLine("ON B.ID_CLIENTE = C.ID ");

                sqlFrom.AppendLine(" WHERE B.ID_CLIENTE = " + request.Id_Cliente);

                if (!String.IsNullOrEmpty(request.Data_Ini) && !String.IsNullOrEmpty(request.Data_Fim))
                {
                    sqlFrom.AppendLine(" AND B.DATA_EMISSAO BETWEEN @DATA_INI AND @DATA_FIM");
                    parametros.Add("@DATA_INI", request.Data_Ini);
                    parametros.Add("@DATA_FIM", request.Data_Fim);
                }
                else
                {
                    sqlFrom.AppendLine(" AND B.DATA_EMISSAO BETWEEN DATEADD(DAY, -90, CURRENT_TIMESTAMP) AND CURRENT_TIMESTAMP ");
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

                    sql.AppendLine("SELECT A.ID, A.CODIGO, C.CPF_CNPJ, C.NOMEFANTASIA, B.DATA_EMISSAO, B.VALOR_RECEBIDO, ");
                    sql.AppendLine(" B.ID_NOTA_FISCAL, B.ID_CLIENTE ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Historico_NfViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Codigo = Convert.ToString(reader["CODIGO"]),
                                Cnpj = Convert.ToString(reader["CPF_CNPJ"]),
                                Nome_Empresa = Convert.ToString(reader["NOMEFANTASIA"]),
                                Data_Emissao = reader["DATA_EMISSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_EMISSAO"]).ToString("dd/MM/yyyy") : "",
                                Valor = reader["VALOR_RECEBIDO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_RECEBIDO"]) : 0
                            });
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
                        case 1:     // TipoPessoa
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Codigo);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Codigo);
                                break;
                            }
                       
                    }
                }

                 //Page e retornando DataTable
            return new DataTableResponse<Historico_NfViewModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
            }

        }


        [HttpPost()]
        [Route("api/Historico_NfApi/ObterNotasFatura")]
        public async Task<DataTableResponse<ResponseNotasRelacionadas>> GetNotasFatura (ObterNotasRelacionadas request)
        {
            List<ResponseNotasRelacionadas> retorno = new List<ResponseNotasRelacionadas>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                //Filtros 
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_FATURAMENTO_CLIENTE A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CONTA_RECEBER B ");
                sqlFrom.AppendLine(" ON B.ID_FATURAMENTO_CLIENTE = A.ID ");
                sqlFrom.AppendLine(" LEFT JOIN TB_NOTA_FISCAL_CONTA_RECEBER C ");
                sqlFrom.AppendLine(" ON C.ID_CONTA_RECEBER = B.ID ");
                sqlFrom.AppendLine(" WHERE A.ID = " + request.IdFatura);

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

                    sql.AppendLine("SELECT C.URL_NOTA_FISCAL AS URL, C.MODELO_NOTA_FISCAL AS MODELO_NF, B.ID AS ID_CR ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine(" GROUP BY C.URL_NOTA_FISCAL, C.MODELO_NOTA_FISCAL, B.ID ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var nota = new ResponseNotasRelacionadas
                            {
                                UrlNota = Convert.ToString(reader["URL"]),
                                ModeloNota = Convert.ToString(reader["MODELO_NF"]),
                                ContaReceberId = reader["ID_CR"] != null ? Convert.ToInt64(reader["ID_CR"]) : (Int64?)null,
                            };

                            if (nota != null && !string.IsNullOrEmpty(nota.ModeloNota))
                            {
                                if (nota.ModeloNota == "ND")
                                    nota.DescricaoModeloNota = "Modelo Débito";
                                else if (nota.ModeloNota == "21")
                                    nota.DescricaoModeloNota = "Modelo 21";
                            }

                            retorno.Add(nota);
                        }
                    }
                }

                //Page e retornando DataTable
                return new DataTableResponse<ResponseNotasRelacionadas>
                {
                    draw = request.draw,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };
            }
        }
    }

    public class ObterNotasRelacionadas : DataTableRequest
    {
        public virtual Int64 IdFatura { get; set; }
    }
    
    public class ResponseNotasRelacionadas : DataTableRequest
    {
        public virtual String UrlNota { get; set; }
        public virtual String ModeloNota { get; set; }
        public virtual String DescricaoModeloNota { get; set; }
        public virtual Int64? ContaReceberId { get; set; }
    }

}