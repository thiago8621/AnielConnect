using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using NHibernate;
using Resources;
using RestSharp.Extensions;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class DescontoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Descontoapi/GetAll")]
        public async Task<DataTableResponse<DescontoListaModel>> GetAll(Desconto_DataTableRequest request)
        {

            List<DescontoListaModel> retorno = new List<DescontoListaModel>();
            IOrderedEnumerable<DescontoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_DESCONTO A");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_DESCONTO B ");
                    sql.AppendLine(" ON(B.ID = A.ID_TIPO_DESCONTO) ");

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

                    sql.AppendLine(" SELECT A.STATUS,A.ID, A.DESCRICAO,A.VALOR_DESCONTO,A.PERCENTUAL_DESCONTO,REPETICOES_DESCONTO,A.DESCONTO_CAPTACAO, B.DESCRICAO AS TIPO_DESCONTO,B.FORMATO,A.DESCONTO_TEMPO_INDETERMINADO ");
                    sql.AppendLine(" FROM TB_DESCONTO A ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_DESCONTO B ");
                    sql.AppendLine(" ON(B.ID = A.ID_TIPO_DESCONTO) ");
                    //sql.AppendLine(" WHERE A.STATUS = 1 ");

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
                            retorno.Add(new DescontoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Desconto_Captacao = HtmlEnumExtensions.GetEnumDescription<Desconto_Captacao>(Convert.ToString(reader["DESCONTO_CAPTACAO"])),
                                Percentual_Desconto = Convert.ToString(reader["PERCENTUAL_DESCONTO"]),
                                Repeticoes_Desconto = Convert.ToString(reader["REPETICOES_DESCONTO"]),
                                Formato = HtmlEnumExtensions.GetEnumDescription<Formato_Tipo_Desconto>(Convert.ToString(reader["FORMATO"])),
                                Valor_Desconto = reader["VALOR_DESCONTO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_DESCONTO"]).ToString("N2") : String.Empty,
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                Tipo_Desconto = Convert.ToString(reader["TIPO_DESCONTO"]),
                                Desconto_Tempo_Indeterminado = reader["VALOR_DESCONTO"] != DBNull.Value ? (Convert.ToBoolean(reader["VALOR_DESCONTO"]) ? RsBase.SIM : RsBase.NAO) : RsBase.NAO,
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
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }

                    case 2:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                    case 3:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
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
            return new DataTableResponse<DescontoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [Route("api/Descontoapi/DetalhesDesconto")]
        [HttpPost()]
        public async Task<DataTableResponse<DescontoDetalhes>> DetalhesDesconto(Desconto_DataTableRequest request)
        {

            List<DescontoDetalhes> retorno = new List<DescontoDetalhes>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        StringBuilder sqlFrom = new StringBuilder();

                        #region From 
                        sqlFrom.AppendLine("FROM TB_DESCONTO A")
                               .AppendLine("LEFT JOIN TB_TIPO_DESCONTO B")
                               .AppendLine("ON(B.ID = A.ID_TIPO_DESCONTO)")
                               .AppendLine("LEFT JOIN TB_DESCONTO_VALORES C")
                               .AppendLine("ON(C.ID_DESCONTO = A.ID)")
                               .AppendLine("LEFT JOIN TB_CLIENTE_DESCONTO D")
                               .AppendLine("ON(D.ID_DESCONTO = A.ID)");
                        //.AppendLine("LEFT JOIN TB_VENDA E")
                        //.AppendLine("ON(E.ID_CLIENTE = D.ID_CLIENTE)");
                        #endregion

                        sql.AppendLine("SELECT A.ID, C.MESINICIAL, C.MESFINAL, C.VALOR, C.PERCENTUAL, A.VALOR_UNICO, A.USUARIOCRIACAO, A.REPETICOES_DESCONTO AS DURACAO_DESCONTO, A.AUTOMATICO, A.VALOR_DESCONTO, A.PERCENTUAL_DESCONTO");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine($"WHERE D.ID = {request.IdClienteDesconto}");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var valor_unico = Convert.IsDBNull(reader["VALOR_UNICO"]) ? false : Convert.ToBoolean(reader["VALOR_UNICO"]);
                                Int64 valor_desconto = 0, valor_percentual = 0;
                                String mesInicio = string.Empty, mesFinal = string.Empty, repeticoes_desconto = string.Empty;
                                if (!valor_unico)
                                {
                                    valor_desconto = Convert.IsDBNull(reader["VALOR"]) ? 0 : Convert.ToInt64(reader["VALOR"]);
                                    valor_percentual = Convert.IsDBNull(reader["PERCENTUAL"]) ? 0 : Convert.ToInt64(reader["PERCENTUAL"]);
                                    mesInicio = Convert.IsDBNull(reader["MESINICIAL"]) ? String.Empty : Convert.ToString(reader["MESINICIAL"]);
                                    mesFinal = Convert.IsDBNull(reader["MESFINAL"]) ? String.Empty : Convert.ToString(reader["MESFINAL"]);
                                    repeticoes_desconto = Convert.IsDBNull(reader["DURACAO_DESCONTO"]) ? String.Empty : $"{Convert.ToString(reader["DURACAO_DESCONTO"])}";
                                }
                                else
                                {
                                    valor_desconto = Convert.IsDBNull(reader["VALOR_DESCONTO"]) ? 0 : Convert.ToInt64(reader["VALOR_DESCONTO"]);
                                    valor_percentual = Convert.IsDBNull(reader["PERCENTUAL_DESCONTO"]) ? 0 : Convert.ToInt64(reader["PERCENTUAL_DESCONTO"]);
                                    mesInicio = "1";
                                    mesFinal = "1";
                                    repeticoes_desconto = "1";
                                }

                                string usuarioCriacao = reader["USUARIOCRIACAO"] != DBNull.Value ? (Convert.ToString(reader["USUARIOCRIACAO"])) : String.Empty;
                                bool descontoAutomatico = reader["AUTOMATICO"] != DBNull.Value ? Convert.ToBoolean(reader["AUTOMATICO"]) : false;
                                String mes = reader["DURACAO_DESCONTO"] != DBNull.Value ? $"{Convert.ToString(reader["DURACAO_DESCONTO"])}" : String.Empty;

                                retorno.Add(new DescontoDetalhes
                                {
                                    MesInicio = mesInicio,
                                    MesFinal = mesFinal,
                                    Valor = valor_desconto.ToString("N2"),
                                    Percentual = valor_percentual.ToString("P"),
                                    DuracaoDesconto = repeticoes_desconto,
                                    ValorUnico = valor_unico ? RsBase.SIM : RsBase.NAO,
                                    UsuarioCriacao = usuarioCriacao,
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                try
                {
                    //Page e retornando DataTable
                    var responseData = new DataTableResponse<DescontoDetalhes>
                    {
                        draw = request.draw,
                        //recordsTotal = recordsTotal,
                        // recordsFiltered = recordsTotal,
                        data = retorno,
                        error = ""
                    };

                    return responseData;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }


        }

        [Route("api/Descontoapi/DescontoByContrato")]
        [HttpPost()]
        public async Task<ClientePlanoDescontoValores> DescontoByContrato(string numeroContrato)
        {
            ClientePlanoDescontoValores clientePlanoDescontoValores = new ClientePlanoDescontoValores();

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                try
                {
                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        #region sql 
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT ");
                        sql.AppendLine("    COALESCE(ccp.VLR_TOTAL_PLANO_VARIAVEL,ccp.VALOR_REAL) AS VALOR_REAL, ");
                        sql.AppendLine("    ccp.ID_CLIENTE_CONTRATO, ");
                        sql.AppendLine("    ccp.VALOR_FINAL_TOTAL, ");
                        sql.AppendLine("    ccp.VALOR_INSTALACAO_FINAL, ");
                        sql.AppendLine("    COALESCE(SUM(tcd.VALOR_DESCONTO), 0) AS TOTAL_DESCONTO, ");
                        sql.AppendLine("    (ccp.VALOR_FINAL_TOTAL - COALESCE(SUM(tcd.VALOR_DESCONTO), 0) - COALESCE(ccp.VALOR_INSTALACAO_FINAL, 0)) AS VALOR_A_PAGAR ");
                        sql.AppendLine("FROM TB_CLIENTE_PLANO_PRODUTO ccp ");
                        sql.AppendLine("INNER JOIN TB_CLIENTE_CONTRATO cc ON cc.ID = ccp.ID_CLIENTE_CONTRATO ");
                        sql.AppendLine("LEFT JOIN TB_CLIENTE_DESCONTO tcd ON tcd.NUMERO_CONTRATO = cc.NUMERO_CONTRATO ");
                        sql.AppendLine($"WHERE cc.NUMERO_CONTRATO = '{numeroContrato}' ");
                        sql.AppendLine("GROUP BY COALESCE(ccp.VLR_TOTAL_PLANO_VARIAVEL,ccp.VALOR_REAL),ccp.ID_CLIENTE_CONTRATO, ccp.VALOR_FINAL_TOTAL, ccp.VALOR_INSTALACAO_FINAL, cc.NUMERO_CONTRATO;");

                        //.AppendLine("LEFT JOIN TB_VENDA E")
                        //.AppendLine("ON(E.ID_CLIENTE = D.ID_CLIENTE)");
                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                clientePlanoDescontoValores.TotalDesconto = reader["TOTAL_DESCONTO"] != DBNull.Value ? (Convert.ToString(reader["TOTAL_DESCONTO"])) : String.Empty;
                                clientePlanoDescontoValores.ValorAPagarSemInstalacao = reader["VALOR_A_PAGAR"] != DBNull.Value ? (Convert.ToString(reader["VALOR_A_PAGAR"])) : String.Empty;
                                clientePlanoDescontoValores.ValorFinalTotal = reader["VALOR_FINAL_TOTAL"] != DBNull.Value ? (Convert.ToString(reader["VALOR_FINAL_TOTAL"])) : String.Empty;
                                clientePlanoDescontoValores.ValorSubTotal = reader["VALOR_REAL"] != DBNull.Value ? (Convert.ToString(reader["VALOR_REAL"])) : String.Empty;

                                clientePlanoDescontoValores.ValorInstalacaoFinal = reader["VALOR_INSTALACAO_FINAL"] != DBNull.Value ? (Convert.ToString(reader["VALOR_INSTALACAO_FINAL"])) : String.Empty;
                            }
                            //foreach (var x in retorno) 
                            //{

                            //    StringBuilder sqlValores = new StringBuilder();

                            //    x.Meses = 
                            //}
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return clientePlanoDescontoValores;

            }


        }

        #region [Desconto Venda]
        [HttpPost()]
        [Route("api/descontoapi/ConsultaDescontoVenda")]
        public async Task<DataTableResponse<DescontoClientePlanoProdutoList>> ConsultaDescontoVenda(DescontoClientePlanoProdutoRequest request)
        {
            #region [Retorno]
            var retornoDtTable = new DataTableResponse<DescontoClientePlanoProdutoList>()
            {
                data = new List<DescontoClientePlanoProdutoList>(),
                recordsFiltered = 0,
                draw = request.draw
            };
            #endregion

            #region [try/catch]
            try
            {
                #region [ESCOPO]

                #region [Variaveis]
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<String> condicoes = new List<String>();
                StringBuilder sqlFrom = new StringBuilder();
                int recordsTotal = 0;
                #endregion

                #region [Estrutura SQL]
                sqlFrom.AppendLine(" FROM TB_CLIENTE_PLANO_PRODUTO A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE_DESCONTO C ON (C.ID_CLIENTE = A.ID_CLIENTE AND C.NUMERO_CONTRATO = A.ID_CLIENTE_CONTRATO) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_DESCONTO B ON (B.ID = C.ID_DESCONTO) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_TIPO_DESCONTO D ON (D.ID = B.ID_TIPO_DESCONTO) ");
                sqlFrom.AppendLine($@" WHERE B.STATUS = 1 AND C.ID > 0 AND A.IS_PLANO_ADICIONAL = 0");

                if (request?.IdVenda > 0)
                {
                    condicoes.Add($" A.ID_VENDA = @ID_VENDA ");
                    parametros.Add("@ID_VENDA", request.IdVenda);
                }

                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine($" AND {String.Join(" AND ", condicoes)} ");
                }
                #endregion

                using (ISession session = HibernateUtil.GetSession())
                {
                    #region [Total Geral]
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                        }

                        retornoDtTable.recordsFiltered = recordsTotal;
                    }
                    #endregion

                    #region [SELECT]
                    using (var com = session.Connection.CreateCommand())
                    {
                        var lista = new List<DescontoClientePlanoProdutoList>();
                        StringBuilder sql = new StringBuilder($@"SELECT C.ID, B.ID ID_DESCONTO, B.DESCRICAO, D.DESCRICAO TIPO_DESCONTO, C.NUMERO_CONTRATO, D.FORMATO,
                                                            (CASE B.VALOR_UNICO WHEN 0 THEN (SELECT FIRST 1 VALOR FROM TB_DESCONTO_VALORES X WHERE X.ID_DESCONTO = B.ID ORDER BY ID ASC) ELSE B.VALOR_DESCONTO END) VALOR_DESCONTO,
                                                            (CASE B.VALOR_UNICO WHEN 0 THEN (SELECT FIRST 1 PERCENTUAL FROM TB_DESCONTO_VALORES X WHERE X.ID_DESCONTO = B.ID ORDER BY ID ASC) ELSE B.PERCENTUAL_DESCONTO END) PERCENTUAL_DESCONTO "
                                                            );
                        sql.AppendLine(sqlFrom.ToString());
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new DescontoClientePlanoProdutoList();

                                obj.Id = Convert.IsDBNull(reader["ID"]) ? 0 : Convert.ToInt64(reader["ID"]);
                                obj.IdDesconto = Convert.IsDBNull(reader["ID_DESCONTO"]) ? (Int64?)null : Convert.ToInt64(reader["ID_DESCONTO"]);
                                obj.Descricao = Convert.IsDBNull(reader["DESCRICAO"]) ? String.Empty : Convert.ToString(reader["DESCRICAO"]);
                                obj.TipoDesconto = Convert.IsDBNull(reader["TIPO_DESCONTO"]) ? String.Empty : Convert.ToString(reader["TIPO_DESCONTO"]);
                                obj.NumeroContratoVinculado = Convert.IsDBNull(reader["NUMERO_CONTRATO"]) ? String.Empty : Convert.ToString(reader["NUMERO_CONTRATO"]);
                                obj.Formato = Convert.IsDBNull(reader["FORMATO"]) ? String.Empty : HtmlEnumExtensions.GetEnumDescription<Formato_Tipo_Desconto>(Convert.ToString(reader["FORMATO"]));
                                obj.ValorDesconto = Convert.IsDBNull(reader["VALOR_DESCONTO"]) ? String.Empty : String.Format("{0:N2}", Convert.ToDouble(reader["VALOR_DESCONTO"]));
                                obj.PercentualDesconto = Convert.IsDBNull(reader["PERCENTUAL_DESCONTO"]) ? String.Empty : Convert.ToString(reader["PERCENTUAL_DESCONTO"]);
                                lista.Add(obj);
                            }
                        }

                        retornoDtTable.data = lista;
                    }
                    #endregion
                }
                #endregion
            }
            catch (Exception e)
            {
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", e.Message, e?.InnerException?.Message, e.StackTrace);
            }
            #endregion

            #region [Return]
            return retornoDtTable;
            #endregion
        }
        #endregion


        [Route("api/Descontoapi/GetList")]
        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<DescontoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<DescontoListaModel>> retorno = new GetApiRetorno<IEnumerable<DescontoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoDesconto = new Conexao<Desconto>(session))
            {
                try
                {
                    var lista = await repoDesconto.queryover().ListAsync();

                    retorno.Objeto = new List<DescontoListaModel>(lista.Where(p => p.Status == Status_Default.Ativo).Select(p => new DescontoListaModel
                    {
                        Id = p.Id,
                        Descricao = Convert.ToString(p.Descricao),
                        Valor_Desconto = Convert.ToString(p.Valor_Desconto),
                        Percentual_Desconto = Convert.ToString(p.Percentual_Desconto),
                        Tipo_Desconto = Convert.ToString(p.Tipo_Desconto?.Descricao)
                    }));

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
        [Route("api/Descontoapi/GetCombo")]
        public async Task<ObjCombo<Int64?>> GetCombo(RequestObjComboCliente request)
        {
            ObjCombo<Int64?> retorno = new ObjCombo<Int64?>
            {
                results = new List<IObjComboResult<Int64?>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoFatCliente = new Conexao<Faturamento_Cliente>(session))
            using (var repoDescontoRegioes = new Conexao<Desconto_Regioes>(session))
            using (var repoDescontoCondicoes = new Conexao<Desconto_Condicoes>(session))
            using (var repoDesconto = new Conexao<Desconto>(session))
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_DESCONTO A ")
                        .AppendLine("INNER JOIN TB_TIPO_DESCONTO B ")
                        .AppendLine("ON (A.ID_TIPO_DESCONTO = B.ID) ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO,")
                                                    .AppendLine(" (CASE A.VALOR_UNICO WHEN 0 THEN (SELECT FIRST 1 VALOR FROM TB_DESCONTO_VALORES X WHERE X.ID_DESCONTO = A.ID ORDER BY ID ASC) ELSE A.VALOR_DESCONTO END) VALOR_DESCONTO, ")
                                                    .AppendLine(" (CASE A.VALOR_UNICO WHEN 0 THEN (SELECT FIRST 1 PERCENTUAL FROM TB_DESCONTO_VALORES X WHERE X.ID_DESCONTO = A.ID ORDER BY ID ASC) ELSE A.PERCENTUAL_DESCONTO END) PERCENTUAL_DESCONTO, ")
                                                    .AppendLine(" B.DESCRICAO AS TIPO_DESCONTO , B.FORMATO ")
                                                    .AppendLine(sqlFrom.ToString())
                                                    .AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Desconto_ComboModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Valor_Desconto = reader["VALOR_DESCONTO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_DESCONTO"]).ToString("N2") : null,
                                    Percentual_Desconto = reader["PERCENTUAL_DESCONTO"] != DBNull.Value ? Convert.ToDouble(reader["PERCENTUAL_DESCONTO"]).ToString() : null,
                                    Tipo_Desconto = Convert.ToString(reader["TIPO_DESCONTO"]),
                                    Formato = reader["FORMATO"] != DBNull.Value ? Convert.ToInt64(reader["FORMATO"]) : (long?)null
                                };

                                #region Comentario_Filtro

                                var idCliente = request.Config.Id_Cliente;

                                retorno.results.Add(obj);

                                var request_Filtro = new RequestFiltroDesconto<ObjCombo<long?>>
                                {
                                    registros = retorno,
                                    Meio_Captacao = true,
                                };



                                retorno = request_Filtro.registros;
                                #endregion
                            }
                            //Adição de pagamentos
                            if (request.Config.Id_Cliente != null)
                            {
                                //request.Config.Pag_Sem_Atraso_Meses = (await repoFatCliente.select()).Where(p => p.Cliente.Id == request.Config.Id_Cliente.Value);
                            }

                            var descontosValidos = (await Filtragem_Desconto(session, request.Config)).Select(p => p.Id).ToList();
                            if (descontosValidos.Count > 0)
                            {
                                retorno.results = retorno.results.Where(p => descontosValidos.Contains(p.Id.Value)).ToList();
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
        [Route("api/Descontoapi/GetComboCondominiosVinculados")]
        public async Task<ObjCombo<Int64>> GetComboCondominiosVinculados(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoFatCliente = new Conexao<Faturamento_Cliente>(session))
            using (var repoDescontoRegioes = new Conexao<Desconto_Regioes>(session))
            using (var repoDescontoCondicoes = new Conexao<Desconto_Condicoes>(session))
            using (var repoDesconto = new Conexao<Desconto>(session))
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_DESCONTO_CONDOMINIOSPRED A ")
                        .AppendLine("INNER JOIN TB_DESCONTO B ")
                        .AppendLine("ON (B.ID = A.ID_DESCONTO) ");

                    List<String> condicoes = new List<String>();

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($" A.ID_DESCONTO = {request.idpai} ");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($" A.ID = {request.id} ");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ")
                                                    .AppendLine(sqlFrom.ToString())
                                                    .AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
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



        private DescontoCadastroModel GetViewObj(Desconto obj)
        {
            return new DescontoCadastroModel
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                TipoValor = obj.Valor_Desconto.HasValue ? TipoValor_DescontoCadastroModel.Valor : TipoValor_DescontoCadastroModel.Percentual,
                Valor_Desconto = obj.Valor_Desconto.HasValue ? obj.Valor_Desconto.Value.ToString("N2") : string.Empty,

                Percentual_Desconto = obj.Percentual_Desconto,
                Repeticoes_Desconto = obj.Repeticoes_Desconto,
                Desconto_Captacao = obj.Desconto_Captacao,
                Status = obj.Status == Status_Default.Ativo,
                CodigoAuxiliar = obj.CodigoAuxiliar,
                Valor_Unico = obj.Valor_Unico,
                Id_Tipo_Desconto = obj.Tipo_Desconto?.Id,
                Automatico = obj.Automatico,
                Desconto_Tempo_Indeterminado = obj.Desconto_Tempo_Indeterminado,
                CondicoesCombinadas = obj.Condicoes.Where(p => p.CondicaoCombinada).Select(p => new DescontoCondicaoListaModel
                {
                    Id = p.Id,
                    TipoCondicao = p.TipoCondicao,
                    ValorCondicao = p.ValorCondicao,
                    ValorCondicaoMaximo = p.ValorCondicaoMaximo,
                }).ToList(),
                CondicoesIndependentes = obj.Condicoes.Where(p => p.CondicaoCombinada == false).Select(p => new DescontoCondicaoListaModel
                {
                    Id = p.Id,
                    TipoCondicao = p.TipoCondicao,
                    ValorCondicao = p.ValorCondicao,
                    ValorCondicaoMaximo = p.ValorCondicaoMaximo,
                }).ToList(),
                Valores = obj.Valores.Select(p => new DescontoValoresListaModel
                {
                    Valor = p.Valor.HasValue ? p.Valor.Value.ToString("N2") : null,
                    MesInicial = p.MesInicial.HasValue ? p.MesInicial.ToString() : null,
                    MesFinal = p.MesFinal.HasValue ? p.MesFinal.ToString() : null,
                    Percentual = p.Percentual.HasValue ? p.Percentual.ToString() : null,
                    Id = p.Id,
                }).ToList(),
                Regioes = obj.Regioes.Select(p => new DescontoRegiaoListaModel
                {
                    Id = p.Id,
                    Cidade = p.Cidade,
                    Cidade_Desc = p.Cidade,
                    Estado = p.Estado,
                    Estado_Desc = p.Estado,
                    Pais = p.Pais,
                    Pais_Desc = Convert.ToString(p.Pais)
                }).ToList(),

                Condominio_Predios = obj.Condominios_Predios.Select(p => new CondominiosCadastroModel
                {
                    Id = p.Id,
                    Cond_Desc = p.Cond_Descricao,
                    Id_Condominio = $"{p.CondominioPredio.Id}"
                }).ToList()

                //Meios = (obj.Meios.Count() == 0) ? new List<Meio_CaptacaoListaModel>() :
                //                                   obj.Meios.Select(p => new Meio_CaptacaoListaModel
                //                                   {
                //                                       Id = p.Id,
                //                                       Id_Meio = p.Meio_Captacao == null ? 0 : p.Meio_Captacao.Id,
                //                                       Descricao = p.Meio_Captacao == null ? String.Empty : p.Meio_Captacao.Descricao
                //                                   }).ToList(),

            };

        }

        private void BindViewToObj(ISession session, Desconto objBanco, DescontoCadastroModel view)
        {
            using (var repoTipoDesconto = new Conexao<Tipo_Desconto>(session))
            using (var repoMeio_Captacao = new Conexao<Meio_Captacao>(session))
            {
                objBanco.Descricao = view.Descricao;
                objBanco.Desconto_Captacao = view.Desconto_Captacao;
                objBanco.Percentual_Desconto = view.Percentual_Desconto;
                objBanco.Repeticoes_Desconto = view.Repeticoes_Desconto;
                objBanco.CodigoAuxiliar = view.CodigoAuxiliar;
                objBanco.Valor_Desconto = Convert.ToDouble(view.Valor_Desconto);

                objBanco.Desconto_Tempo_Indeterminado = view.Desconto_Tempo_Indeterminado;
                objBanco.Valor_Unico = view.Valor_Unico;
                objBanco.Automatico = view.Automatico;
                objBanco.Tipo_Desconto = repoTipoDesconto.selectFirst(p => p.Id == view.Id_Tipo_Desconto).Result;
                objBanco.Valores = view.Valores.Select(p => new Desconto_Valores
                {
                    Valor = String.IsNullOrEmpty(p.Valor) ? (double?)null : Convert.ToDouble(p.Valor),
                    MesFinal = String.IsNullOrEmpty(p.MesFinal) ? (Int16?)null : Convert.ToInt16(p.MesFinal),
                    MesInicial = String.IsNullOrEmpty(p.MesInicial) ? (Int16?)null : Convert.ToInt16(p.MesInicial),
                    Percentual = String.IsNullOrEmpty(p.Percentual) ? (double?)null : Convert.ToInt16(p.Percentual),
                    Desconto = objBanco,
                    Id = p.Id,
                }).ToList();

                objBanco.Regioes = view.Regioes.Select(p => new Desconto_Regioes
                {
                    Id = p.Id,
                    Cidade = p.Cidade,
                    Pais = p.Pais,
                    Estado = p.Estado,
                    Desconto = objBanco,
                }).ToList();

                objBanco.Condicoes = view.CondicoesCombinadas.Select(p => new Desconto_Condicoes
                {
                    Id = p.Id,
                    CondicaoCombinada = true,
                    TipoCondicao = p.TipoCondicao,
                    ValorCondicao = p.ValorCondicao.HasValue() ? p.ValorCondicao : p.ValorCondicaoMinimo.HasValue() ? p.ValorCondicaoMinimo : p.ValorMeses,
                    ValorCondicaoMaximo = p.ValorCondicaoMaximo,
                    Desconto = objBanco
                }).Concat(view.CondicoesIndependentes.Select(p => new Desconto_Condicoes
                {
                    Id = p.Id,
                    CondicaoCombinada = false,
                    TipoCondicao = p.TipoCondicao,
                    ValorCondicao = p.ValorCondicao.HasValue() ? p.ValorCondicao : p.ValorCondicaoMinimo.HasValue() ? p.ValorCondicaoMinimo : p.ValorMeses,
                    ValorCondicaoMaximo = p.ValorCondicaoMaximo,
                    Desconto = objBanco
                })).ToList();

                objBanco.Condominios_Predios = view.Condominio_Predios.Select(c => new Desconto_CondominiosPredios
                {
                    Id = c.Id,
                    Cond_Descricao = c.Cond_Desc,
                    CondominioPredio = new Condominio_Predio { Id = Convert.ToInt64(c.Id_Condominio) },
                    Desconto = objBanco
                }).ToList();
                //List<Desconto_Meio_Captacao> meiosTmp = objBanco.Meios.ToList();
                //foreach (var meio in view.Meios.FindAll(p => p.Id == 0))
                //{
                //    meiosTmp.Add(new Desconto_Meio_Captacao
                //    {
                //        Meio_Captacao = repoMeio_Captacao.selectFirst(p => p.Id == meio.Id_Meio).Result,
                //        Desconto = objBanco
                //    });
                //}
                //objBanco.Meios = meiosTmp;
            }
        }

        [HttpPost()]
        [Route("api/Descontoapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_DESCONTO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Descontoapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_DESCONTO WHERE ID IN {_ids}");

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


        [HttpGet()]
        [Route("api/Descontoapi/FindById")]
        public async Task<GetApiRetorno<DescontoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<DescontoCadastroModel> retorno = new GetApiRetorno<DescontoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoDesconto = new Conexao<Desconto>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repoDesconto.selectFirst(p => p.Id == Id));

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
        [Route("api/Descontoapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<DescontoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Desconto modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Desconto>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Desconto
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };

                        }

                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
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
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/Descontoapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Desconto>(session))
                    {

                        Desconto modelo = null;
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

        [Route("api/Descontoapi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Cliente_Desconto>(session))
                    {

                        Cliente_Desconto modelo = null;
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

        [Route("api/Descontoapi/ExcluirGridValores")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridValores(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Desconto_Valores>(session))
                    {

                        Desconto_Valores modelo = null;
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



        [Route("api/Descontoapi/ExcluirGridCondominiosPredios")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridCondominiosPredios(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Desconto_CondominiosPredios>(session))
                    {

                        Desconto_CondominiosPredios modelo = null;
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




        [Route("api/Descontoapi/ExcluirGridRegioes")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridRegioes(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Desconto_Regioes>(session))
                    {

                        Desconto_Regioes modelo = null;
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

        [Route("api/Descontoapi/ExcluirGridCondicoes")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridCondicoesCombinadas(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Desconto_Condicoes>(session))
                    {

                        Desconto_Condicoes modelo = null;
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
        [HttpGet()]
        [Route("api/Descontoapi/CheckCondicoesVenda")]
        public async Task<List<DescontoListaModel>> CheckCondicoesDescontoAutomatico(VendaDadosGeraisViewModel request)
        {
            List<DescontoListaModel> retorno = new List<DescontoListaModel>();
            PostApiMensagem Mensagens = new PostApiMensagem();
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionConnect = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
            {
                //request.Planos.Select(p => p.).ToList();
                Configuracoes_Gerais Configuracao = await repoConfig.selectFirst();
                //dentro das configurações, acrescentar o esperado, caso ambos sejam falsos, apenas os descontos Manuais                             
                StringBuilder sql = new StringBuilder("SELECT * "),
                    sqlFrom = new StringBuilder("FROM TB_DESCONTO A INNER JOIN TB_DESCONTO_CONDICOES B INNER JOIN TB_TIPO_DESCONTO C"),
                    sqlWhere = new StringBuilder();
                using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var obj_Tipo_Desconto = new Tipo_Desconto
                        {
                            Formato = (Formato_Tipo_Desconto)reader[""],
                            Descricao = String.Empty,
                        };
                        var obj_Desconto_Condicoes = new Desconto_Condicoes
                        {
                        };
                        var Desconto = new Desconto
                        {
                        };

                    }
                }

                //Trabalhando com as Condições definidas nas Configurações + outras definições
                /*item1:Descontos Diferentes, 
                 *item2: Descontos do mesmo tipo, 
                 *item3: Valor do Desconto maior que valor do plano*/

                ValueTuple<bool, bool, bool> Fatores_Desconto = Tuple.Create(item1: Configuracao.Acumular_Descontos_Diferentes,
                                                                             item2: Configuracao.Acumular_Descontos_Mesmo_Tipo,
                                                                             item3: true/**/).ToValueTuple();

                if (Fatores_Desconto.Item3) { }
                else
                {
                    Mensagens.Mensagem = $"{HtmlEnumExtensions.GetEnumDescription(PostApiMensagemType.Erro)}: " + RsCadastros.DESCONTO_MAIOR_QUE_VALOR;
                    Mensagens.Tipo = PostApiMensagemType.Erro;
                }
            }
            List<(double, bool)> Valores_Descontos = new List<(Double, bool)>();
            Calcula_Valores_Desconto(Valores_Descontos, 0);
            return retorno;
        }

        public Double Calcula_Valores_Desconto(List<(double, bool)> Valores_Desconto, double Valor_Plano)
        {
            Double Plano_Original_Value = Valor_Plano;
            foreach ((double, bool) desconto in Valores_Desconto)
            {
                if (Valor_Plano == 0)
                {
                    PostApiMensagem obj = new PostApiMensagem
                    {
                        Mensagem = "Soma de descontos maior que o valor do desconto",
                        Tipo = PostApiMensagemType.Erro
                    };
                    Valor_Plano = Plano_Original_Value;
                    break;
                }
                else
                {
                    if (!desconto.Item2)
                    {
                        Valor_Plano = Valor_Plano - desconto.Item1;
                    }
                    else if (desconto.Item2)
                    {
                        Valor_Plano = Valor_Plano - (Valor_Plano * (desconto.Item1 / 100));
                    }

                }

            }

            return Valor_Plano;
        }

        public async Task<List<Desconto>> Filtragem_Desconto(ISession session, DescontoParametros parametros)
        {

            var retorno = new List<Desconto>();
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoDescontoRegioes = new Conexao<Desconto_Regioes>(session))
            using (var repoDescontoCondicoes = new Conexao<Desconto_Condicoes>(session))
            using (var repoDesconto = new Conexao<Desconto>(session))
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoFatCliente = new Conexao<Faturamento_Cliente>(session))
            {


                var listdesconto = repoDesconto.select(p => p.Status == Status_Default.Ativo).Result.ToList();



                //request.registros
                //request
                //request.registros;

                foreach (var item in listdesconto)
                {
                    bool atendeRegrasCondicoes = item.Condicoes.Count > 0 ? false : true;
                    bool atendeRegrasRegioes = item.Regioes.Count > 0 ? false : true;


                    foreach (var regiao in item.Regioes)
                    {
                        if (String.IsNullOrEmpty(regiao.Pais) == false && String.IsNullOrEmpty(regiao.Estado) == false && String.IsNullOrEmpty(regiao.Cidade) == false
                            && regiao.Estado != Resources.RsBase.TODOS && regiao.Cidade != Resources.RsBase.TODOS)
                        {
                            var estado = await repoUf.selectFirst(p => p.Descricao == regiao.Estado.ToUpperInvariant());
                            if (regiao?.Pais == Convert.ToString(parametros.Pais) &&
                                estado?.Sigla == Convert.ToString(parametros.Estado) &&
                                regiao?.Cidade == Convert.ToString(parametros.Cidade))
                            {
                                atendeRegrasRegioes = true;
                            }
                        }
                        else if (String.IsNullOrEmpty(regiao.Pais) == false && String.IsNullOrEmpty(regiao.Estado) == false && regiao.Estado != Resources.RsBase.TODOS)
                        {
                            var estado = await repoUf.selectFirst(p => p.Descricao == regiao.Estado.ToUpperInvariant());
                            if (regiao.Pais == Convert.ToString(parametros.Pais) &&
                                estado.Sigla == Convert.ToString(parametros.Estado))
                            {
                                atendeRegrasRegioes = true;
                            }
                        }
                        else if (String.IsNullOrEmpty(regiao.Pais) == false)
                        {
                            if (regiao.Pais == Convert.ToString(parametros.Pais))
                            {
                                atendeRegrasRegioes = true;
                            }
                        }
                    }


                    if (item.Condicoes.Where(p => p.TipoCondicao == "1" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "1" && p.CondicaoCombinada == false))
                        {
                            if (Convert.ToInt32(condicao.ValorCondicao) == Convert.ToInt32(parametros.Tipo_Cliente))
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }

                    if (item.Condicoes.Where(p => p.TipoCondicao == "2" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "2" && p.CondicaoCombinada == false))
                        {
                            if (condicao.ValorCondicao == parametros.Categoria_Cliente)
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }

                    if (item.Condicoes.Where(p => p.TipoCondicao == "3" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "3" && p.CondicaoCombinada == false))
                        {
                            if (condicao.ValorCondicao == parametros.Meio_Captacao)
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }
                    if (item.Condicoes.Where(p => p.TipoCondicao == "4" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "4" && p.CondicaoCombinada == false))
                        {
                            if (parametros.Tipo_Tecnologia.Contains((Tipo_Tecnologia)Convert.ToInt32(condicao.ValorCondicao)))
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }

                    if (item.Condicoes.Where(p => p.TipoCondicao == "5" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "5" && p.CondicaoCombinada == false))
                        {
                            if (parametros.Plano_Produto.Contains(condicao.ValorCondicao))
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }

                    if (item.Condicoes.Where(p => p.TipoCondicao == "6" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "6" && p.CondicaoCombinada == false))
                        {
                            if (parametros.Valor_Plano_Final.Find(p => p >= Convert.ToDouble(condicao.ValorCondicao) && p <= Convert.ToDouble(condicao.ValorCondicaoMaximo)) != null)
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }

                    if (item.Condicoes.Where(p => p.TipoCondicao == "7" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "7" && p.CondicaoCombinada == false))
                        {
                            if (Convert.ToDouble(parametros.Valor_Final_Total) >= Convert.ToDouble(condicao.ValorCondicao) && Convert.ToDouble(parametros.Valor_Final_Total) <= Convert.ToDouble(condicao.ValorCondicaoMaximo))
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }

                    if (item.Condicoes.Where(p => p.TipoCondicao == "8" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "8" && p.CondicaoCombinada == false))
                        {
                            if (parametros.Tipo_Forma_Pagamento.Contains((Tipo_Forma_Pagamento)Convert.ToInt32(condicao.ValorCondicao)))
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }

                    if (item.Condicoes.Where(p => p.TipoCondicao == "9" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "9" && p.CondicaoCombinada == false))
                        {
                            if (parametros.Forma_Pagamento.Contains(condicao.ValorCondicao))
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }

                    if (item.Condicoes.Where(p => p.TipoCondicao == "10" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "10" && p.CondicaoCombinada == false))
                        {
                            if (parametros.Periodo_Fidelidade_Meses.Find(p => p >= Convert.ToInt32(condicao.ValorCondicao) && p <= Convert.ToInt32(condicao.ValorCondicaoMaximo)) != null)
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }

                    var valCondicao = repoDescontoCondicoes.selectFirst(p => p.ValorCondicao != null);

                    if (item.Condicoes.Where(p => p.TipoCondicao == "11" && p.CondicaoCombinada == false).Count() > 0)
                    {
                        parametros.Pag_Sem_Atraso_Meses = Convert.ToInt32(valCondicao.Result.ValorCondicao);
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "11" && p.CondicaoCombinada == false))
                        {

                            if (await checar_NumeroFaturas_PagamentoSequencia(repoFatCliente, Convert.ToInt32(condicao.ValorCondicao)) >= Convert.ToInt32(condicao.ValorCondicao))
                            {
                                atendeRegrasCondicoes = true;
                            }
                        }
                    }


                    if (item.Condicoes.Where(p => p.TipoCondicao == "12" && p.CondicaoCombinada == true).Count() > 0)
                    {
                        foreach (var condicao in item.Condicoes.Where(p => p.TipoCondicao == "12" && p.CondicaoCombinada == true))
                        {
                            if (parametros.TemCondomioPredio != null)
                            {
                                var hasDesconto = item.Condominios_Predios.Any(x => x.Id == parametros.TemCondomioPredio.Id);

                                if (hasDesconto)
                                {
                                    atendeRegrasCondicoes = true;
                                }
                            }
                        }
                    }



                    if ((atendeRegrasCondicoes && atendeRegrasRegioes))
                    {
                        retorno.Add(item);
                    }

                    //********************
                    //Condições Combinadas
                    //********************
                    else if (item.Condicoes.Any(param => param.CondicaoCombinada == true))
                    {
                        if (checharCondicaoCombinada(item, parametros) && atendeRegrasRegioes)
                        {
                            retorno.Add(item);
                        }
                    }

                }


            }

            return retorno;
        }

        public bool checharCondicaoCombinada(Desconto desconto, DescontoParametros parametros)
        {

            var ChecagemCondicoes = new List<Boolean>();
            if (desconto.Condicoes.Where(p => p.TipoCondicao == "1" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "1" && p.CondicaoCombinada == true))
                {
                    if (Convert.ToInt32(condicao.ValorCondicao) == Convert.ToInt32(parametros.Tipo_Cliente))
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "2" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "2" && p.CondicaoCombinada == true))
                {
                    if (condicao.ValorCondicao == parametros.Categoria_Cliente)
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "3" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "3" && p.CondicaoCombinada == true))
                {
                    if (condicao.ValorCondicao == parametros.Meio_Captacao)
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "4" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "4" && p.CondicaoCombinada == true))
                {
                    if (parametros.Tipo_Tecnologia.Contains((Tipo_Tecnologia)Convert.ToInt32(condicao.ValorCondicao)) == true)
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "5" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "5" && p.CondicaoCombinada == true))
                {
                    if (parametros.Plano_Produto.Contains(condicao.ValorCondicao) == true)
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "6" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "6" && p.CondicaoCombinada == true))
                {
                    if (parametros.Valor_Plano_Final.Find(p => p >= Convert.ToDouble(condicao.ValorCondicao) && p <= Convert.ToDouble(condicao.ValorCondicaoMaximo)) != null)
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "7" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "7" && p.CondicaoCombinada == true))
                {
                    if (Convert.ToDouble(parametros.Valor_Final_Total) >= Convert.ToDouble(condicao.ValorCondicao) && Convert.ToDouble(parametros.Valor_Final_Total) <= Convert.ToDouble(condicao.ValorCondicaoMaximo))
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "8" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "8" && p.CondicaoCombinada == true))
                {
                    if (parametros.Tipo_Forma_Pagamento.Contains((Tipo_Forma_Pagamento)Convert.ToInt32(condicao.ValorCondicao)))
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "9" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "9" && p.CondicaoCombinada == true))
                {
                    if (parametros.Forma_Pagamento.Contains(condicao.ValorCondicao))
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "10" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "10" && p.CondicaoCombinada == true))
                {
                    if (parametros.Periodo_Fidelidade_Meses.Find(p => p >= Convert.ToInt32(condicao.ValorCondicao) && p <= Convert.ToInt32(condicao.ValorCondicaoMaximo)) != null)
                    {
                        ChecagemCondicoes.Add(true);
                    }
                }
            }

            if (desconto.Condicoes.Where(p => p.TipoCondicao == "12" && p.CondicaoCombinada == true).Count() > 0)
            {
                foreach (var condicao in desconto.Condicoes.Where(p => p.TipoCondicao == "12" && p.CondicaoCombinada == true))
                {
                    if (parametros.TemCondomioPredio != null)
                    {
                        var hasDesconto = desconto.Condominios_Predios.Any(x => x.Id == parametros.TemCondomioPredio.Id);

                        if (hasDesconto)
                        {
                            ChecagemCondicoes.Add(true);
                        }
                    }
                }
            }

            if (desconto.Condicoes.Count == ChecagemCondicoes.Count)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public async Task<int> checar_NumeroFaturas_PagamentoSequencia(Conexao<Faturamento_Cliente> repository, int qtdMesses)
        {

            DateTime data_amostragem = DateTime.Now.AddMonths(-qtdMesses);

            var faturas_amostragem = (await repository.select(p => p.Vencimento.Value.Month >= data_amostragem.Month)).OrderBy(p => p.Vencimento);
            var checagem_pagas = faturas_amostragem.Select(p => p.Data_Pagamento <= p.Vencimento).ToList();

            return checagem_pagas.Count();
        }


    }
}