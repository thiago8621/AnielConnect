using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class LogGenericoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/LogGenericoapi/GetAll")]
        public async Task<DataTableResponse<LogGenericoModel>> GetAll(LogGenerico_DataTableRequest request)
        {
            List<LogGenericoModel> retorno = new List<LogGenericoModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();

                switch (request.Modelo)
                {
                    case LogGenericoEnum.Atendimento:
                        sqlFrom.AppendLine("FROM TB_Atendimento A ");
                        break;

                    case LogGenericoEnum.RotaTelefonia:
                        sqlFrom.AppendLine("FROM TB_ROTA_TELEFONIA A ");
                        break;

                    case LogGenericoEnum.Did:
                        sqlFrom.AppendLine("FROM TB_DID A ");
                        break;

                    case LogGenericoEnum.Uf:
                        sqlFrom.AppendLine("FROM TB_UF A ");
                        break;

                    case LogGenericoEnum.Cidade:
                        sqlFrom.AppendLine("FROM TB_CIDADE A ");
                        break;

                    case LogGenericoEnum.Bairro:
                        sqlFrom.AppendLine("FROM TB_BAIRRO A ");
                        break;

                    case LogGenericoEnum.Endereco_Logradouro:
                        sqlFrom.AppendLine("FROM TB_ENDERECO_LOGRADOURO A ");
                        break;

                    case LogGenericoEnum.Condominio_Predio:
                        sqlFrom.AppendLine("FROM TB_CONDOMINIO_PREDIO A ");
                        break;   
                    case LogGenericoEnum.Vendas:
                        sqlFrom.AppendLine("FROM TB_VENDAS A ");
                        break;

                    default:
                        sqlFrom.AppendLine("FROM TB_FRANQUIA A ");
                        break;
                }

                sqlFrom.AppendLine("WHERE A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM ");

                parametros.Add("@DATAINI", DateTime.Parse(request.DataIni + " 00:00:00"));
                parametros.Add("@DATAFIM", DateTime.Parse(request.DataFim + " 23:59:59"));

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine(" AND ((UPPER(A.OBJETO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
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
                if (!String.IsNullOrEmpty(request.Usuario))
                {
                    sqlFrom.AppendLine(" AND A.USUARIOCRIACAO LIKE UPPER (@USUARIO)");
                    parametros.Add("@USUARIO", request.Usuario);
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

                    sql.AppendLine("SELECT A.ID, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.IDLOG, A.OBJETO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS ");
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
                            (string valorAnterior, string valorAtual) valores = (Convert.ToString(reader["CAMPO_VALORANTES"]), Convert.ToString(reader["CAMPO_VALORDEPOIS"]));

                            retorno.Add(new LogGenericoModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Chave = Convert.ToString(reader["OBJETO"]),
                                Campo = Convert.ToString(reader["CAMPO"]),
                                Campo_ValorAntes = String.IsNullOrEmpty(valores.valorAnterior) ? Resources.RsBase.N_A : valores.valorAnterior,
                                Campo_ValorDepois = String.IsNullOrEmpty(valores.valorAtual) ? Resources.RsBase.N_A : valores.valorAtual,
                                TipoOperacao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
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

    }
}
