using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Ajax.Utilities;
using MongoDB.Driver;
using NHibernate;
using NHibernate.Criterion;
using RestSharp.Extensions;
using NHibernate.Hql.Ast;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.MetaBusiness.Models;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls.WebParts;
using System.Globalization;
using Twilio.TwiML.Voice;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class RelatorioApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/relatorioapi/GetAll_Supervisores")]
        public async Task<DataTableResponse<RelatorioProducaoModel>> GetAll_Supervisores(RelatorioProducao_DataTableRequest request)
        {
            List<RelatorioProducaoModel> retorno = new List<RelatorioProducaoModel>();
            IOrderedEnumerable<RelatorioProducaoModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                DateTime fimPeriodo = Convert.ToDateTime(request.Fim_Periodo);
                DateTime iniPeriodo = Convert.ToDateTime(request.Ini_Periodo);

                Int32 diferencaMes = fimPeriodo.Month - iniPeriodo.Month;
                List<Int64> meses = new List<Int64>();

                for (var i = 0; i <= diferencaMes; i++)
                    meses.Add(iniPeriodo.AddMonths(i).Month);

                Int64 meta_supervisor = 0;

                List<Faixa_Comissionamento> faixas_comissionamento = (await session.QueryOver<Faixa_Comissionamento>().Fetch(SelectMode.Fetch, u => u.Itens).ListAsync()).ToList();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder(" SELECT SUM(QUANTIDADE) AS QUANTIDADE, MAX(UNIDADE) AS UNIDADE, ID_SUPERV, MATRICULA_SUPERV, NOME_SUPERV, ")
                    .AppendLine(" FILIAL, ID_FAIXA_SUPERV, FAIXA_SUPERV, ID_FAIXA_FILIAL ")
                    .AppendLine(" FROM ")
                    .AppendLine(" ( ")

                    .AppendLine("    SELECT A.NUM_FRE AS MATRICULA, A.NOME AS NOME_FUNCIONARIO, B.ID AS ID_SUPERV, B.NUM_FRE AS MATRICULA_SUPERV, B.NOME AS NOME_SUPERV, ")
                    .AppendLine("    C.NOME AS FILIAL, B.ID_FAIXA AS ID_FAIXA_SUPERV, K.DESCRICAO AS FAIXA_SUPERV, ")
                    .AppendLine("    C.ID_FAIXA_COMISSIONAMENTO AS ID_FAIXA_FILIAL, E.ID_PLANO, E.ID_PRODUTO, ")
                    .AppendLine("    ( ")
                    .AppendLine("        SELECT SUM(I.QUANTIDADE) AS QUANTIDADE FROM TB_COMISSAO_PLANO_PRODUTO I ")
                    .AppendLine("        WHERE(I.ID_PLANO = E.ID_PLANO OR I.ID_PRODUTO = E.ID_PRODUTO) ")
                    .AppendLine("    ), ")
                    .AppendLine("    ( ")
                    .AppendLine("        SELECT MAX(J.UNIDADE) AS UNIDADE FROM TB_COMISSAO_PLANO_PRODUTO J ")
                    .AppendLine("        WHERE(J.ID_PLANO = E.ID_PLANO OR J.ID_PRODUTO = E.ID_PRODUTO) ")
                    .AppendLine("    ) ")

                    .AppendLine("    FROM TB_FUNCIONARIO A ")
                    .AppendLine("    INNER JOIN TB_FUNCIONARIO B ")
                    .AppendLine("    ON(A.ID_SUPERVISOR = B.ID) ")
                    .AppendLine("    INNER JOIN TB_FILIAL C ")
                    .AppendLine("    ON(A.ID_FILIAL = C.ID) ")
                    .AppendLine("    INNER JOIN TB_VENDA D ")
                    .AppendLine("    ON(A.ID = D.ID_VENDEDOR) ")
                    .AppendLine("    INNER JOIN TB_CLIENTE_PLANO_PRODUTO E ")
                    .AppendLine("    ON(E.ID_VENDA = D.ID) ")
                    .AppendLine("    INNER JOIN TB_COMISSIONAMENTO G ")
                    .AppendLine("    ON(G.ID = A.ID_COMISSIONAMENTO) ")
                    //.AppendLine("    ON(G.ID_FILIAL = A.ID_FILIAL) ")
                    .AppendLine("    INNER JOIN TB_CAMPANHA H ")
                    .AppendLine("    ON(H.ID = E.ID_CAMPANHA) ")
                    .AppendLine("    INNER JOIN TB_FAIXA_COMISSIONAMENTO K ")
                    .AppendLine("    ON(B.ID_FAIXA = K.ID) ")

                    .AppendLine("    WHERE D.STATUS = 6 ");  /*VENDA APROVADA*/

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (request.Id_Filial.HasValue)
                    {
                        sql.AppendLine(" AND (C.ID = @ID_FILIAL) ");
                        parametros.Add("@ID_FILIAL", request.Id_Filial);
                    }

                    if (request.Id_Supervisor.HasValue)
                    {
                        sql.AppendLine(" AND (B.ID = @ID_SUPERV) ");
                        parametros.Add("@ID_SUPERV", request.Id_Supervisor);
                    }

                    if (request.Id_Vendedor.HasValue)
                    {
                        sql.AppendLine(" AND (A.ID = @ID_VENDEDOR) ");
                        parametros.Add("@ID_VENDEDOR", request.Id_Vendedor);
                    }

                    if (request.Id_Campanha.HasValue)
                    {
                        sql.AppendLine(" AND (H.ID = @ID_CAMPANHA) ");
                        parametros.Add("@ID_CAMPANHA", request.Id_Campanha);
                    }

                    if (!String.IsNullOrEmpty(request.Ini_Periodo) && !String.IsNullOrEmpty(request.Fim_Periodo))
                    {
                        sql.AppendLine(" AND (D.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM) ");
                        parametros.Add("@DATA_INICIO", request.Ini_Periodo);
                        parametros.Add("@DATA_FIM", request.Fim_Periodo);
                    }

                    if (request.Ids_Plano_Produto != null)
                    {
                        String ids = $"({String.Join(",", request.Ids_Plano_Produto)})";
                        sql.AppendLine($" AND ((E.ID_PLANO IN {ids}) OR (E.ID_PLANO IN {ids}))");
                    }

                    sql.AppendLine("    GROUP BY A.NUM_FRE, A.NOME, B.ID, B.NUM_FRE, B.NOME, C.NOME, B.ID_FAIXA, K.DESCRICAO, ");
                    sql.AppendLine("    C.ID_FAIXA_COMISSIONAMENTO, E.ID_PLANO, E.ID_PRODUTO ");
                    sql.AppendLine(" ) ");
                    sql.AppendLine(" GROUP BY ID_SUPERV, MATRICULA_SUPERV, NOME_SUPERV, ID_FAIXA_SUPERV, ");
                    sql.AppendLine(" FILIAL, FAIXA_SUPERV, ID_FAIXA_FILIAL ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                    {
                        while (await reader.ReadAsync())
                        {
                            var objMeta = new MetaApiController().exibirMetaPeriodoBySupervisor(Convert.ToInt64(reader["ID_SUPERV"]), request.Ini_Periodo, request.Fim_Periodo).Result.Objeto;
                            var Metas = objMeta.Where(x => meses.Contains(x.Mes));

                            foreach (var meta in Metas)
                                meta_supervisor += meta.Valor_meta;

                            var _obj = new RelatorioProducaoModel
                            {
                                Id_Superv = Convert.ToInt16(reader["ID_SUPERV"]),
                                Matricula_Superv = Convert.ToString(reader["MATRICULA_SUPERV"]),
                                Nome_Superv = Convert.ToString(reader["NOME_SUPERV"]),
                                Filial = Convert.ToString(reader["FILIAL"]),
                                Meta = Convert.ToString(meta_supervisor),
                                Faixa_Superv = Convert.ToString(reader["FAIXA_SUPERV"]),
                                Id_Faixa_Filial = reader["ID_FAIXA_FILIAL"] != DBNull.Value ? Convert.ToInt64(reader["ID_FAIXA_FILIAL"]) : (Int64?)null,
                                Id_Faixa_Superv = reader["ID_FAIXA_SUPERV"] != DBNull.Value ? Convert.ToInt64(reader["ID_FAIXA_SUPERV"]) : (Int64?)null,
                                Unidade = Convert.ToString(reader["UNIDADE"]),
                                Quantidade = reader["QUANTIDADE"] != DBNull.Value ? Convert.ToInt64(reader["QUANTIDADE"]) : 0
                            };

                            _obj.Coeficiente = GetCoeficiente(faixas_comissionamento, null, _obj.Id_Faixa_Superv, _obj.Id_Faixa_Filial, _obj.Quantidade);
                            _obj.Total = _obj.Coeficiente * _obj.Quantidade;

                            retorno.Add(_obj);
                        }
                    }

                }
            }

            listaOrdenada = retorno.OrderBy(p => p.Nome_Funcionario);

            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<RelatorioProducaoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/relatorioapi/GetAll_Vendedores")]
        public async Task<DataTableResponse<RelatorioProducaoModel>> GetAll_Vendedores(RelatorioProducao_DataTableRequest request)
        {
            List<RelatorioProducaoModel> retorno = new List<RelatorioProducaoModel>();
            IOrderedEnumerable<RelatorioProducaoModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                DateTime fimPeriodo = Convert.ToDateTime(request.Fim_Periodo);
                DateTime iniPeriodo = Convert.ToDateTime(request.Ini_Periodo);

                Int32 diferencaMes = fimPeriodo.Month - iniPeriodo.Month;
                List<Int64> meses = new List<Int64>();

                for (var i = 0; i <= diferencaMes; i++)
                    meses.Add(iniPeriodo.AddMonths(i).Month);

                Int64 meta_vendedor = 0;

                List<Faixa_Comissionamento> faixas_comissionamento = (await session.QueryOver<Faixa_Comissionamento>().Fetch(SelectMode.Fetch, u => u.Itens).ListAsync()).ToList();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine(" SELECT SUM(QUANTIDADE) AS QUANTIDADE, MAX (UNIDADE) AS UNIDADE, ID_VENDEDOR, ID_SUPERVISOR, NOME_FUNCIONARIO, MATRICULA, FILIAL, ID_FAIXA_VENDEDOR, FAIXA_VENDEDOR ");

                    sql.AppendLine(" FROM ");
                    sql.AppendLine(" ( ");
                    sql.AppendLine("     SELECT A.ID AS ID_VENDEDOR, A.NUM_FRE AS MATRICULA, A.NOME AS NOME_FUNCIONARIO, B.ID AS ID_SUPERVISOR, ");
                    sql.AppendLine("     K.DESCRICAO AS FAIXA_VENDEDOR, C.NOME AS FILIAL, A.ID_FAIXA AS ID_FAIXA_VENDEDOR, ");
                    sql.AppendLine("     E.ID_PLANO, E.ID_PRODUTO, ");
                    sql.AppendLine("     ( ");
                    sql.AppendLine("         SELECT SUM(I.QUANTIDADE) AS QUANTIDADE FROM TB_COMISSAO_PLANO_PRODUTO I ");
                    sql.AppendLine("        WHERE(I.ID_PLANO = E.ID_PLANO OR I.ID_PRODUTO = E.ID_PRODUTO) ");
                    sql.AppendLine("     ), ");
                    sql.AppendLine("     ( ");
                    sql.AppendLine("         SELECT MAX(J.UNIDADE) AS UNIDADE FROM TB_COMISSAO_PLANO_PRODUTO J ");
                    sql.AppendLine("         WHERE(J.ID_PLANO = E.ID_PLANO OR J.ID_PRODUTO = E.ID_PRODUTO) ");
                    sql.AppendLine("     ) ");


                    sql.AppendLine("     FROM TB_FUNCIONARIO A ");
                    sql.AppendLine("     INNER JOIN TB_FUNCIONARIO B ");
                    sql.AppendLine("     ON(A.ID_SUPERVISOR = B.ID) ");
                    sql.AppendLine("     INNER JOIN TB_FILIAL C ");
                    sql.AppendLine("     ON(A.ID_FILIAL = C.ID) ");
                    sql.AppendLine("     INNER JOIN TB_VENDA D ");
                    sql.AppendLine("     ON(A.ID = D.ID_VENDEDOR) ");
                    sql.AppendLine("     INNER JOIN TB_CLIENTE_PLANO_PRODUTO E ");
                    sql.AppendLine("     ON(E.ID_VENDA = D.ID) ");
                    sql.AppendLine("     INNER JOIN TB_COMISSIONAMENTO G ");
                    sql.AppendLine("     ON(G.ID = A.ID_COMISSIONAMENTO) ");
                    //sql.AppendLine("     ON(G.ID_FILIAL = A.ID_FILIAL) ");
                    sql.AppendLine("     INNER JOIN TB_CAMPANHA H ");
                    sql.AppendLine("     ON(H.ID = E.ID_CAMPANHA) ");
                    sql.AppendLine("     INNER JOIN TB_COMISSAO_PLANO_PRODUTO I ");
                    sql.AppendLine("     ON(I.ID_PLANO = E.ID_PLANO OR I.ID_PRODUTO = E.ID_PRODUTO) ");
                    sql.AppendLine("     INNER JOIN TB_COMISSAO_PLANO_PRODUTO J ");
                    sql.AppendLine("     ON(J.ID_PLANO = E.ID_PLANO OR J.ID_PRODUTO = E.ID_PRODUTO) ");
                    sql.AppendLine("     INNER JOIN TB_FAIXA_COMISSIONAMENTO K ");
                    sql.AppendLine("     ON(A.ID_FAIXA = K.ID) ");


                    sql.AppendLine("     WHERE D.STATUS = 6 ");   /*VENDA APROVADA*/

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (request.Id_Filial.HasValue)
                    {
                        sql.AppendLine(" AND (C.ID = @ID_FILIAL) ");
                        parametros.Add("@ID_FILIAL", request.Id_Filial);
                    }

                    if (request.Id_Supervisor.HasValue)
                    {
                        sql.AppendLine(" AND (B.ID = @ID_SUPERV) ");
                        parametros.Add("@ID_SUPERV", request.Id_Supervisor);
                    }

                    if (request.Id_Vendedor.HasValue)
                    {
                        sql.AppendLine(" AND (A.ID = @ID_VENDEDOR) ");
                        parametros.Add("@ID_VENDEDOR", request.Id_Vendedor);
                    }

                    if (request.Id_Campanha.HasValue)
                    {
                        sql.AppendLine(" AND (H.ID = @ID_CAMPANHA) ");
                        parametros.Add("@ID_CAMPANHA", request.Id_Campanha);
                    }

                    if (!String.IsNullOrEmpty(request.Ini_Periodo) && !String.IsNullOrEmpty(request.Fim_Periodo))
                    {
                        sql.AppendLine(" AND (D.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM) ");
                        parametros.Add("@DATA_INICIO", request.Ini_Periodo);
                        parametros.Add("@DATA_FIM", request.Fim_Periodo);
                    }

                    if (request.Ids_Plano_Produto != null)
                    {
                        String ids = $"({String.Join(",", request.Ids_Plano_Produto)})";
                        sql.AppendLine($" AND ((E.ID_PLANO IN {ids}) OR (E.ID_PLANO IN {ids}))");
                    }

                    sql.AppendLine("     GROUP BY A.ID, A.NUM_FRE, A.NOME, B.ID, ");
                    sql.AppendLine("     K.DESCRICAO, C.NOME, A.ID_FAIXA, ");
                    sql.AppendLine("     E.ID_PLANO, E.ID_PRODUTO ");
                    sql.AppendLine(" ) ");

                    sql.AppendLine(" GROUP BY ID_VENDEDOR, ID_SUPERVISOR, NOME_FUNCIONARIO, MATRICULA, FILIAL, ID_FAIXA_VENDEDOR, FAIXA_VENDEDOR  ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                    {
                        while (await reader.ReadAsync())
                        {
                            var objMeta = new MetaApiController().exibirMetaPeriodo(Convert.ToInt64(reader["ID_VENDEDOR"]), request.Ini_Periodo, request.Fim_Periodo).Result.Objeto;
                            var Metas = objMeta.Where(x => meses.Contains(x.Mes));

                            foreach (var meta in Metas)
                                meta_vendedor += meta.Valor_meta;

                            var _obj = new RelatorioProducaoModel
                            {
                                Id_Superv = Convert.ToInt64(reader["ID_SUPERVISOR"]),
                                Id_Vendedor = Convert.ToInt64(reader["ID_VENDEDOR"]),
                                Matricula = Convert.ToString(reader["MATRICULA"]),
                                Nome_Funcionario = Convert.ToString(reader["NOME_FUNCIONARIO"]),
                                Filial = Convert.ToString(reader["FILIAL"]),
                                Meta = Convert.ToString(meta_vendedor),
                                Id_Faixa_Vendedor = reader["ID_FAIXA_VENDEDOR"] != DBNull.Value ? Convert.ToInt64(reader["ID_FAIXA_VENDEDOR"]) : (Int64?)null,
                                Faixa_Vendedor = Convert.ToString(reader["FAIXA_VENDEDOR"]),
                                Unidade = Convert.ToString(reader["UNIDADE"]),
                                Quantidade = reader["QUANTIDADE"] != DBNull.Value ? Convert.ToInt64(reader["QUANTIDADE"]) : 0
                            };

                            _obj.Coeficiente = GetCoeficiente(faixas_comissionamento, _obj.Id_Faixa_Vendedor, _obj.Id_Faixa_Superv, _obj.Id_Faixa_Filial, _obj.Quantidade);
                            _obj.Total = _obj.Coeficiente * _obj.Quantidade;

                            retorno.Add(_obj);
                        }
                    }

                }
            }

            listaOrdenada = retorno.OrderBy(p => p.Nome_Funcionario);

            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<RelatorioProducaoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/relatorioapi/GetInfoPlanoProduto")]
        public async Task<DataTableResponse<InfoPlanoProdutoModel>> GetInfoPlanoProduto(RelatorioProducao_DataTableRequest request)
        {
            List<InfoPlanoProdutoModel> retorno = new List<InfoPlanoProdutoModel>();
            IOrderedEnumerable<InfoPlanoProdutoModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();


                    sql.AppendLine(" SELECT A.ID, A.DATA_VENDA, C.TITULO AS CAMPANHA, D.DESCRICAO AS PLANO, E.DESCRICAO AS PRODUTO, D.VALORPLANO, E.VALORPRODUTO ");


                    sql.AppendLine(" FROM TB_VENDA A ");


                    sql.AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO B ");
                    sql.AppendLine(" ON(B.ID_VENDA = A.ID) ");

                    sql.AppendLine(" LEFT JOIN TB_CAMPANHA C ");
                    sql.AppendLine(" ON(C.ID = B.ID_CAMPANHA) ");
                    sql.AppendLine(" AND ");
                    sql.AppendLine("    CASE ");
                    sql.AppendLine("        WHEN B.ID_CAMPANHA IS NULL ");
                    sql.AppendLine("            THEN 1 ");
                    sql.AppendLine("        ELSE 1 ");
                    sql.AppendLine("    END = 1 ");

                    sql.AppendLine(" LEFT JOIN TB_PLANO D ");
                    sql.AppendLine(" ON(D.ID = B.ID_PLANO) ");

                    sql.AppendLine(" LEFT JOIN TB_PRODUTO E ");
                    sql.AppendLine(" ON(E.ID = B.ID_PRODUTO) ");

                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO F ");
                    sql.AppendLine(" ON(F.ID = A.ID_VENDEDOR) ");

                    sql.AppendLine(" LEFT JOIN TB_FILIAL G ");
                    sql.AppendLine(" ON(G.ID = F.ID_FILIAL) ");

                    sql.AppendLine(" WHERE A.STATUS = '6'");

                    if (request.Id_Filial.HasValue)
                    {
                        sql.AppendLine(" AND (G.ID = @ID_FILIAL) ");
                        parametros.Add("@ID_FILIAL", request.Id_Filial);
                    }

                    if (request.Id_Supervisor.HasValue)
                    {
                        sql.AppendLine(" AND (A.ID_SUPERVISOR = @ID_SUPERV) ");
                        parametros.Add("@ID_SUPERV", request.Id_Supervisor);
                    }

                    if (request.Id_Vendedor.HasValue)
                    {
                        sql.AppendLine(" AND (A.ID_VENDEDOR = @ID_VENDEDOR) ");
                        parametros.Add("@ID_VENDEDOR", request.Id_Vendedor);
                    }

                    if (request.Id_Campanha.HasValue)
                    {
                        sql.AppendLine(" AND (C.ID = @ID_CAMPANHA) ");
                        parametros.Add("@ID_CAMPANHA", request.Id_Campanha);
                    }

                    if (!String.IsNullOrEmpty(request.Ini_Periodo) && !String.IsNullOrEmpty(request.Fim_Periodo))
                    {
                        sql.AppendLine(" AND (A.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM) ");
                        parametros.Add("@DATA_INICIO", request.Ini_Periodo);
                        parametros.Add("@DATA_FIM", request.Fim_Periodo);
                    }

                    if (request.Ids_Plano_Produto != null)
                    {
                        String ids = $"({String.Join(",", request.Ids_Plano_Produto)})";
                        sql.AppendLine($" AND ((B.ID_PLANO IN {ids}) OR (B.ID_PLANO IN {ids}))");
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                    {
                        while (await reader.ReadAsync())
                        {

                            retorno.Add(new InfoPlanoProdutoModel
                            {
                                Data_Venda = Convert.ToString(reader["DATA_VENDA"]),
                                Campanha = Convert.ToString(reader["CAMPANHA"]),
                                Plano_Produto = reader["PLANO"] != DBNull.Value ? Convert.ToString(reader["PLANO"]) : Convert.ToString(reader["PRODUTO"]),
                                Valor_Plano_Produto = reader["VALORPLANO"] != DBNull.Value ? Convert.ToString(reader["VALORPLANO"]) : Convert.ToString(reader["VALORPRODUTO"])
                            });

                        }
                    }
                }
            }

            listaOrdenada = retorno.OrderBy(p => Convert.ToDateTime(p.Data_Venda));

            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<InfoPlanoProdutoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private Double GetCoeficiente(List<Faixa_Comissionamento> faixas, Int64? Id_Funcionario, Int64? Id_Supervisor, Int64? Id_Filial, Int64 Quantidade)
        {
            if (Quantidade == 0)
                return 0;

            Int64? _id = null;
            if (Id_Funcionario.HasValue)
                _id = Id_Funcionario;
            else if (Id_Supervisor.HasValue)
                _id = Id_Supervisor;
            else if (Id_Filial.HasValue)
                _id = Id_Filial;

            if (_id.HasValue)
            {
                Faixa_Comissionamento faixa = faixas.Find(p => p.Id == _id);

                if (faixa == null)
                    return 0;

                Faixa_Comissionamento_Item item = faixa.Itens.ToList().Find(p => Quantidade >= p.Faixa_Inicial && Quantidade <= p.Faixa_Final);

                if (item == null)
                    return 0;
                else
                    return Convert.ToDouble(item.Coeficiente);
            }

            return 0;
        }

        [HttpPost()]
        [Route("api/relatorioapi/RelatorioProjecaoVendas")]
        public async Task<DataTableResponse<RelatorioProjecaoVendasModel>> RelatorioProjecaoVendas(RelatorioProjecao_Vendas request)
        {
            List<RelatorioProjecaoVendasModel> retorno = new List<RelatorioProjecaoVendasModel>();
            IOrderedEnumerable<RelatorioProjecaoVendasModel> listaOrdenada = null;
            Int32 recordsTotal = 0;
            var inicio_mes = new DateTime(Convert.ToInt32(request.Ano), Convert.ToInt32(request.Mes + 1), 1);
            var fim_mes = DateTimeUtil.GetLastDayOfMonth(inicio_mes).AddDays(1).AddSeconds(-1);

            //Select
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessiont5 = HibernateUtil.GetSessionT5())
            {
                var parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                using (var com1 = session.Connection.CreateCommand())
                using (var com2 = session.Connection.CreateCommand())
                using (var com3 = sessiont5.Connection.CreateCommand())
                using (var com4 = session.Connection.CreateCommand())
                using (var com5 = session.Connection.CreateCommand())
                using (var com6 = session.Connection.CreateCommand())
                using (var com7 = session.Connection.CreateCommand())
                using (var com8 = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    //sql.AppendLine(" SELECT DISTINCT B.NUM_FRE, A.ID_VENDEDOR, B.NOME AS VENDEDOR, D.NUM_FRE AS SUPERVISOR, D.NOME AS NOME_SUPERV, C.DESCRICAO AS DESCR_META,  ");
                    //sql.AppendLine(" C.JANEIRO, C.FEVEREIRO, C.MARCO, C.ABRIL, C.MAIO, C.JUNHO, C.JULHO, C.AGOSTO, C.SETEMBRO, C.OUTUBRO, C.NOVEMBRO, C.DEZEMBRO ");

                    //sql.AppendLine(" FROM TB_VENDA A ");

                    //sql.AppendLine("  INNER JOIN TB_FUNCIONARIO B ON B.ID = A.ID_VENDEDOR ");
                    //sql.AppendLine("  INNER JOIN TB_FUNCIONARIO D ON D.ID = B.ID_SUPERVISOR ");
                    //sql.AppendLine("  LEFT JOIN TB_META C ON C.ID = B.ID_META");
                    //sql.AppendLine(" LEFT JOIN TB_FILIAL E ON (B.ID_FILIAL = E.ID) ");
                    //sql.AppendLine(" LEFT JOIN TB_EMPRESA F ON (E.ID_EMPRESA = F.ID) ");


                    sql.AppendLine(" SELECT DISTINCT A.NUM_FRE, B.ID_VENDEDOR, A.NOME AS VENDEDOR, C.NUM_FRE AS SUPERVISOR, C.NOME AS NOME_SUPERV, D.DESCRICAO AS DESCR_META, ");
                    sql.AppendLine(" D.JANEIRO, D.FEVEREIRO, D.MARCO, D.ABRIL, D.MAIO, D.JUNHO, D.JULHO, D.AGOSTO, D.SETEMBRO, D.OUTUBRO, D.NOVEMBRO, D.DEZEMBRO ");

                    sql.AppendLine(" FROM TB_FUNCIONARIO A ");

                    sql.AppendLine(" LEFT JOIN TB_VENDA B ON B.ID_VENDEDOR = A.ID ");

                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO C ON C.ID = B.ID_SUPERVISOR ");
                    sql.AppendLine(" LEFT JOIN TB_META D ON D.ID = A.ID_META ");
                    sql.AppendLine(" LEFT JOIN TB_FILIAL E ON (A.ID_FILIAL = E.ID) ");
                    sql.AppendLine(" LEFT JOIN TB_EMPRESA F ON (E.ID_EMPRESA = F.ID) ");
                    sql.AppendLine(" WHERE  (A.VENDEDOR = 1) AND A.SITUACAO IN (0,4)");

                    if (request.Ano.HasValue)
                    {
                        sql.Append(" AND (B.DATA_VENDA BETWEEN @INICIO AND @FIM) ");
                        parametros.Add("@INICIO", inicio_mes);
                        parametros.Add("@FIM", fim_mes);
                    }

                    if (!String.IsNullOrEmpty(request.Id_Supervisor))
                    {
                        sql.AppendLine(" AND (C.NUM_FRE = @ID_SUPERV)");
                        parametros.Add("@ID_SUPERV", request.Id_Supervisor);
                    }

                    if (request.Id_Vendedor.HasValue)
                    {
                        sql.AppendLine(" AND (A.ID = @ID_VENDEDOR)");
                        parametros.Add("@ID_VENDEDOR", request.Id_Vendedor);
                    }
                    //else { sql.AppendLine(" OR(B.ID_VENDEDOR IS NOT NULL) "); }

                    if (request.Id_Empresa.HasValue)
                    {
                        sql.AppendLine(" AND (F.ID = @ID_EMPRESA) ");
                        parametros.Add("@ID_EMPRESA", request.Id_Empresa);
                    }

                    if (request.Id_Filial.HasValue)
                    {
                        sql.AppendLine(" AND (E.ID = @ID_FILIAL) ");
                        parametros.Add("@ID_FILIAL", request.Id_Filial);
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                    {
                        while (await reader.ReadAsync())
                        {
                            //######## META ATUAL VINCULADA E META MÊS ########
                            //O banco de dados possui colunas com o nome dos meses, infelizmente não consegui otimizar o código
                            var meta_mes = "";
                            if (request.Mes.HasValue)
                            {
                                switch (request.Mes)
                                {
                                    case 0:
                                        meta_mes = Convert.ToString(reader["JANEIRO"]);
                                        break;

                                    case 1:
                                        meta_mes = Convert.ToString(reader["FEVEREIRO"]);
                                        break;

                                    case 2:
                                        meta_mes = Convert.ToString(reader["MARCO"]);
                                        break;

                                    case 3:
                                        meta_mes = Convert.ToString(reader["ABRIL"]);
                                        break;

                                    case 4:
                                        meta_mes = Convert.ToString(reader["MAIO"]);
                                        break;

                                    case 5:
                                        meta_mes = Convert.ToString(reader["JUNHO"]);
                                        break;

                                    case 6:
                                        meta_mes = Convert.ToString(reader["JULHO"]);
                                        break;

                                    case 7:
                                        meta_mes = Convert.ToString(reader["AGOSTO"]);
                                        break;

                                    case 8:
                                        meta_mes = Convert.ToString(reader["SETEMBRO"]);
                                        break;

                                    case 9:
                                        meta_mes = Convert.ToString(reader["OUTUBRO"]);
                                        break;

                                    case 10:
                                        meta_mes = Convert.ToString(reader["NOVEMBRO"]);
                                        break;

                                    case 11:
                                        meta_mes = Convert.ToString(reader["DEZEMBRO"]);
                                        break;
                                }

                                if (meta_mes == "")
                                {
                                    meta_mes = "0";
                                }
                            }

                            //######## VENDEDOR ########
                            var vendedor = reader["ID_VENDEDOR"] != DBNull.Value ? Convert.ToInt32(reader["ID_VENDEDOR"]) : (Int32?)null;

                            //######## DIAS ÚTEIS ########
                            var DiasUteis = 0;
                            TimeSpan date = Convert.ToDateTime(fim_mes) - Convert.ToDateTime(inicio_mes);
                            int totalDias = date.Days;
                            Dictionary<String, Object> parametrosFeriados = new Dictionary<string, object>();
                            StringBuilder sqlFeriados = new StringBuilder();
                            sqlFeriados.AppendLine($" SELECT COUNT(A.ID) CO FROM TB_FERIADO A");
                            sqlFeriados.AppendLine($" WHERE A.DATA BETWEEN @DATA_INICIO AND @DATA_FIM ");
                            parametrosFeriados.Add("@DATA_INICIO", inicio_mes);
                            parametrosFeriados.Add("@DATA_FIM", fim_mes);

                            int days = 0;
                            var inicioWhile = inicio_mes;
                            while (inicioWhile <= fim_mes)
                            {
                                if (inicioWhile.DayOfWeek != DayOfWeek.Saturday
                                   && inicioWhile.DayOfWeek != DayOfWeek.Sunday)
                                    days++;

                                inicioWhile = inicioWhile.AddDays(1);
                            }

                            using (var readerFeriados = await CommandHelper.ExecuteReaderFirst(com4, sqlFeriados, parametrosFeriados, session.Connection, null, 10, 0))
                            {
                                while (await readerFeriados.ReadAsync())
                                {
                                    DiasUteis = Convert.ToInt32(readerFeriados["CO"]);
                                }
                            }
                            DiasUteis = days - DiasUteis;


                            //######## META DIÁRIA ########
                            //meta_diaria = meta mês / dias_uteis
                            double MetaDiaria = 0;
                            if (meta_mes != "0")
                            {
                                MetaDiaria = Convert.ToInt32(meta_mes) / DiasUteis;
                            }

                            var metricavendedor = new RelatorioProjecaoVendasModel();
                            metricavendedor.vendedor = reader["NUM_FRE"] != DBNull.Value ? Convert.ToString(reader["NUM_FRE"]) + " - " + Convert.ToString(reader["VENDEDOR"]) : null;
                            metricavendedor.supervisor = reader["SUPERVISOR"] != DBNull.Value ? Convert.ToString(reader["SUPERVISOR"]) + " - " + Convert.ToString(reader["NOME_SUPERV"]) : null;
                            metricavendedor.meta_atual_vinculada = reader["DESCR_META"] == DBNull.Value ? "-" : Convert.ToString(reader["DESCR_META"]);
                            metricavendedor.meta_mes = meta_mes;
                            metricavendedor.dias_uteis = Convert.ToString(DiasUteis);
                            metricavendedor.meta_diaria = Convert.ToString(Math.Ceiling(MetaDiaria));


                            if (vendedor != null)
                            {
                                //######## REALIZADO MÊS ######## Vendas aprovadas no mês
                                //Select
                                var Vendas_Aprovadas_Mes = 0;

                                Dictionary<String, Object> parametrosVendasAprovadas = new Dictionary<string, object>();

                                StringBuilder sqlAprovadasMes = new StringBuilder();
                                sqlAprovadasMes.AppendLine($" SELECT COUNT(C.ID) CO FROM ASPNETUSERS A");
                                sqlAprovadasMes.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                                sqlAprovadasMes.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                                sqlAprovadasMes.AppendLine($" LEFT JOIN TB_VENDA C");
                                sqlAprovadasMes.AppendLine($" ON(C.ID_VENDEDOR = D.ID)");
                                sqlAprovadasMes.AppendLine($" WHERE C.ID_VENDEDOR = @ID_VENDEDOR AND ");
                                sqlAprovadasMes.AppendLine($" C.STATUS = '6' AND ");
                                sqlAprovadasMes.AppendLine($" C.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM");

                                parametrosVendasAprovadas.Add("@DATA_INICIO", inicio_mes);
                                parametrosVendasAprovadas.Add("@DATA_FIM", fim_mes);
                                parametrosVendasAprovadas.Add("@ID_VENDEDOR", vendedor);

                                using (var readerAprovadasMes = await CommandHelper.ExecuteReaderFirst(com1, sqlAprovadasMes, parametrosVendasAprovadas, session.Connection, null, 10, 0))
                                {
                                    while (await readerAprovadasMes.ReadAsync())
                                    {
                                        Vendas_Aprovadas_Mes = readerAprovadasMes["CO"] == DBNull.Value ? 0 : Convert.ToInt32(readerAprovadasMes["CO"]);
                                    }
                                }

                                //######## PONTOS PLANO PRODUTO ######## Soma dos pontos plano produto
                                //Select
                                var pontosPlanoProduto = 0;
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
                                sql2.AppendLine(" WHERE B.ID_VENDEDOR = @ID_VENDEDOR AND B.DATA_VENDA BETWEEN @DATA_INI AND @DATA_FIM");

                                parametros2.Add("@ID_VENDEDOR", vendedor);
                                parametros2.Add("@DATA_INI", inicio_mes);
                                parametros2.Add("@DATA_FIM", fim_mes);

                                using (var readerpontos = await CommandHelper.ExecuteReaderFirst(com2, sql2, parametros2, session.Connection, null, 10, 0))
                                {
                                    while (await readerpontos.ReadAsync())
                                    {
                                        pontosPlanoProduto = readerpontos["PONTOS"] == DBNull.Value ? 0 : Convert.ToInt32(readerpontos["PONTOS"]);
                                    }
                                }

                                //######## VENDAS OS FINALIZADAS ######## Vendas OS finalizadas, com situação de baixado produtivo
                                //Select
                                List<int> idsVenda = new List<int>();
                                Dictionary<String, Object> parametrosVenda = new Dictionary<String, Object>();
                                StringBuilder sqlVenda = new StringBuilder();

                                sqlVenda.AppendLine(" SELECT  A.ID ");
                                sqlVenda.AppendLine(" FROM TB_VENDA A ");
                                sqlVenda.AppendLine(" WHERE A.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM ");
                                sqlVenda.AppendLine(" AND A.ID_VENDEDOR = @ID_VENDEDOR ");

                                parametrosVenda.Add("@DATA_INICIO", inicio_mes);
                                parametrosVenda.Add("@DATA_FIM", fim_mes);
                                parametrosVenda.Add("@ID_VENDEDOR", vendedor);

                                using (var readerVenda = await CommandHelper.ExecuteReader(com8, sqlVenda, parametrosVenda, session.Connection, null))
                                {
                                    while (await readerVenda.ReadAsync())
                                    {
                                        idsVenda.Add(Convert.ToInt32(readerVenda["ID"]));
                                    }
                                }

                                var VendasOSFinalizadas = 0;
                                foreach (var idvenda in idsVenda)
                                {
                                    Dictionary<String, Object> parametrosOsFinalizadas = new Dictionary<string, object>();
                                    StringBuilder sqlOSFinalizadas = new StringBuilder();

                                    sqlOSFinalizadas.AppendLine($" SELECT COUNT(A.NUM_OBRA) CO FROM TB_DOCUMENTO_PRODUCAO A ");
                                    sqlOSFinalizadas.AppendLine($" WHERE A.STATUS_EXECUCAO IN (2) ");
                                    sqlOSFinalizadas.AppendLine($" AND A.ID_VENDA = @ID_VENDA");

                                    parametrosOsFinalizadas.Add("@ID_VENDA", idvenda);

                                    using (var readerOSFinalizadas = await CommandHelper.ExecuteReaderFirst(com3, sqlOSFinalizadas, parametrosOsFinalizadas, sessiont5.Connection, null, 10, 0))
                                    {
                                        while (await readerOSFinalizadas.ReadAsync())
                                        {
                                            VendasOSFinalizadas += Convert.ToInt32(readerOSFinalizadas["CO"]);
                                        }
                                    }
                                }

                                //######## VENDAS PARA ATINGIR META DO MÊS ######## meta do mês e subtraindo pelas vendas realizadas no período filtrado
                                var VendasAtingirMeta = 0;
                                if (Convert.ToInt32(meta_mes) > 0)
                                {
                                    VendasAtingirMeta = Convert.ToInt32(meta_mes) - Vendas_Aprovadas_Mes;
                                }


                                //######## META DO DIA ########  meta do mês e subtrair pelo realizado. Com o resultado dividi-lo pelos dias úteis restantes
                                //######## DIAS ÚTEIS RESTANTES ########
                                var inicio_mes_atual = new DateTime();
                                var DiasUteisRest = 0;
                                if (inicio_mes.Month == inicio_mes_atual.Month && inicio_mes.Month == inicio_mes_atual.Year)
                                {
                                    var fim_mes_atual = DateTimeUtil.GetLastDayOfMonth(inicio_mes_atual);

                                    TimeSpan dateRest = Convert.ToDateTime(fim_mes_atual) - Convert.ToDateTime(inicio_mes_atual);
                                    int totalDiasRestantes = date.Days;

                                    Dictionary<String, Object> parametrosFeriadosRestantes = new Dictionary<string, object>();
                                    StringBuilder sqlFeriadosRestantes = new StringBuilder();
                                    sqlFeriados.AppendLine($" SELECT COUNT(C.ID) CO FROM TB_FERIADO A");
                                    sqlFeriados.AppendLine($" WHERE A.data BETWEEN @DATA_INICIO AND @DATA_FIM ");
                                    parametrosFeriados.Add("@DATA_INICIO", inicio_mes_atual);
                                    parametrosFeriados.Add("@DATA_FIM", fim_mes_atual);

                                    using (var readerFeriadosRestantes = await CommandHelper.ExecuteReaderFirst(com5, sqlFeriadosRestantes, parametrosFeriadosRestantes, session.Connection, null, 10, 0))
                                    {
                                        while (await readerFeriadosRestantes.ReadAsync())
                                        {
                                            DiasUteisRest = readerFeriadosRestantes["CO"] == DBNull.Value ? 0 : Convert.ToInt32(readerFeriadosRestantes["CO"]);
                                        }
                                    }

                                    DiasUteisRest = totalDiasRestantes - DiasUteisRest;
                                }

                                int meta_do_dia = 0;
                                if (VendasAtingirMeta > 0 && DiasUteisRest > 0)
                                {
                                    meta_do_dia = VendasAtingirMeta / DiasUteisRest;
                                }

                                //######## VENDAS PERDIDAS ######## Vendas com status de cancelado
                                //Select
                                var VendasPerdidas = 0;
                                Dictionary<String, Object> parametrosTickets = new Dictionary<string, object>();

                                StringBuilder sqlVendasPerdidas = new StringBuilder();

                                sqlVendasPerdidas.AppendLine("SELECT COUNT(C.ID) CO");
                                sqlVendasPerdidas.AppendLine("FROM ASPNETUSERS A");
                                sqlVendasPerdidas.AppendLine("LEFT JOIN TB_FUNCIONARIO B");
                                sqlVendasPerdidas.AppendLine("ON(B.ID_SUPERVISOR = A.ID_FUNCIONARIO)");
                                sqlVendasPerdidas.AppendLine("LEFT JOIN TB_VENDA C");
                                sqlVendasPerdidas.AppendLine("ON(C.ID_VENDEDOR = B.ID)");
                                sqlVendasPerdidas.AppendLine("WHERE A.ID_FUNCIONARIO = B.ID_SUPERVISOR AND C.ID_VENDEDOR = @ID_VENDEDOR AND C.STATUS IN(0,5)");
                                sqlVendasPerdidas.AppendLine(" AND C.DATA_VENDA BETWEEN @DATA_INICIO_MES AND @DATA_ATUAL ");

                                parametrosTickets.Add("@ID_VENDEDOR", vendedor);
                                parametrosTickets.Add("@DATA_INICIO_MES", inicio_mes);
                                parametrosTickets.Add("@DATA_ATUAL", fim_mes);

                                //Busca com paginacao
                                using (var readerVendasPerdidas = await CommandHelper.ExecuteReaderFirst(com6, sqlVendasPerdidas, parametrosTickets, session.Connection, null, 10, 0))
                                {
                                    while (await readerVendasPerdidas.ReadAsync())
                                    {
                                        VendasPerdidas = readerVendasPerdidas["CO"] == DBNull.Value ? 0 : Convert.ToInt32(readerVendasPerdidas["CO"]);
                                    }
                                }

                                //######## TICKET MEDIO ######## Soma do valor das vendas / numero de vendas
                                var TicketMedio = 0;
                                //Select
                                Dictionary<String, Object> parametrosTicket = new Dictionary<string, object>();

                                StringBuilder sqlTicket = new StringBuilder();

                                sqlTicket.AppendLine($" SELECT SUM(B.VALOR_FINAL_TOTAL) AS VALOR_TOTAL FROM ASPNETUSERS A ");
                                sqlTicket.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                                sqlTicket.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                                sqlTicket.AppendLine($" LEFT JOIN TB_VENDA C");
                                sqlTicket.AppendLine($" ON(C.ID_VENDEDOR = D.ID)");
                                sqlTicket.AppendLine($" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO B ON B.ID_VENDA = C.ID ");
                                sqlTicket.AppendLine($" WHERE C.ID_VENDEDOR = @ID_VENDEDOR AND ");
                                sqlTicket.AppendLine($" C.STATUS = '6' AND ");
                                sqlTicket.AppendLine($" C.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM ");

                                parametrosTicket.Add("@DATA_INICIO", inicio_mes);
                                parametrosTicket.Add("@DATA_FIM", fim_mes);
                                parametrosTicket.Add("@ID_VENDEDOR", vendedor);

                                //Busca com paginacao
                                using (var readerTicket = await CommandHelper.ExecuteReaderFirst(com7, sqlTicket, parametrosTicket, session.Connection, null, 10, 0))
                                {
                                    while (await readerTicket.ReadAsync())
                                    {
                                        TicketMedio = readerTicket["VALOR_TOTAL"] != DBNull.Value ? Convert.ToInt32(readerTicket["VALOR_TOTAL"]) : 0;
                                    }
                                }
                                var valorTicket = 0;
                                if (TicketMedio > 0 && Vendas_Aprovadas_Mes > 0)
                                {
                                    valorTicket = TicketMedio / Vendas_Aprovadas_Mes;
                                }


                                metricavendedor.realizado_mes = Convert.ToString(Vendas_Aprovadas_Mes);
                                metricavendedor.pontos_pl_pr_vendidos = Convert.ToString(pontosPlanoProduto);
                                metricavendedor.vendas_os_finalizadas = Convert.ToString(VendasOSFinalizadas);
                                metricavendedor.vendas_atingir_meta = Convert.ToString(VendasAtingirMeta);
                                metricavendedor.meta_dia = Convert.ToString(meta_do_dia);
                                metricavendedor.vendas_perdidas = Convert.ToString(VendasPerdidas);
                                metricavendedor.ticket_medio = Convert.ToDouble(valorTicket);


                            }
                            retorno.Add(metricavendedor);
                        }
                    }
                }
            }

            listaOrdenada = retorno.OrderBy(p => p.vendedor);

            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<RelatorioProjecaoVendasModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/relatorioapi/RelatorioVendasNaoEfetivadas")]
        public async Task<DataTableResponse<RelatorioVendasNaoEfetivadasModel>> RelatorioVendasNaoEfetivadas(RelatorioVendas_Nao_Efetivadas request)
        {
            List<RelatorioVendasNaoEfetivadasModel> retorno = new List<RelatorioVendasNaoEfetivadasModel>();
            IOrderedEnumerable<RelatorioVendasNaoEfetivadasModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                Dictionary<String, Object> parametrosContato = new Dictionary<string, object>();
                Dictionary<String, Object> parametrosPlanoProduto = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT DISTINCT B.ID, F.DESCRICAO AS CATEGORIA, B.DATA_VENDA, D.TIPO_CLIENTE, B.DATAHORACRIACAO, B.DATA_ENCERRAMENTO, D.NOMEFANTASIA, D.CPF, ");
                    sql.AppendLine(" A.NUM_FRE, A.NOME AS VENDEDOR, C.NOME AS SUPERVISOR, ");
                    sql.AppendLine(" E.ENDERECO_BAIRRO, E.ENDERECO_CIDADE,  E.ENDERECO_UF, ");
                    sql.AppendLine(" G.DESCRICAO AS MOTIVO_CANCELAMENTO ");

                    sql.AppendLine(" FROM TB_VENDA B ");
                    sql.AppendLine(" INNER JOIN TB_FUNCIONARIO A ON B.ID_VENDEDOR = A.ID ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO C ON C.ID = B.ID_SUPERVISOR ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE D ON D.id = B.ID_CLIENTE ");
                    sql.AppendLine(" LEFT JOIN TB_ENDERECO_CLIENTE E ON E.ID = D.ID_ENDERECO_PRINCIPAL ");
                    sql.AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE F ON F.ID = D.ID_CATEGORIA ");
                    sql.AppendLine(" LEFT JOIN TB_MOTIVO_DESISTENCIA_VENDA G ON G.ID = B.ID_MOTIVO_DESISTENCIA_VENDA ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO H ON H.ID_VENDA = B.ID ");
                    sql.AppendLine(" LEFT JOIN TB_CIDADE I ON I.DESCRICAO = E.ENDERECO_CIDADE ");
                    sql.AppendLine("  LEFT JOIN TB_UF J ON I.ID_UF = J.ID");


                    sql.AppendLine(" WHERE B.STATUS = '4'");


                    if (!String.IsNullOrEmpty(request.Data_Inicio) && !String.IsNullOrEmpty(request.Data_Fim))
                    {
                        if (!String.IsNullOrEmpty(request.Tipo_Filtro))
                        {
                            if (request.Tipo_Filtro == "criacao")
                            {
                                sql.AppendLine(" AND (B.DATAHORACRIACAO BETWEEN @DATA_INICIO AND @DATA_FIM) ");
                                parametros.Add("DATA_INICIO", DateTime.Parse(request.Data_Inicio).ToString("yyyy-MM-dd"));
                                parametros.Add("DATA_FIM", DateTime.Parse(request.Data_Fim).ToString("yyyy-MM-dd"));

                            }
                            else if (request.Tipo_Filtro == "cancelamento")
                            {
                                sql.AppendLine(" AND (B.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_FIM) ");
                                parametros.Add("DATA_INICIO", DateTime.Parse(request.Data_Inicio).ToString("yyyy-MM-dd"));
                                parametros.Add("DATA_FIM", DateTime.Parse(request.Data_Fim).ToString("yyyy-MM-dd"));
                            }

                        }
                    }

                    if (request.Id_Supervisor.HasValue)
                    {
                        sql.AppendLine(" AND (B.ID_SUPERVISOR = @ID_SUPERV) ");
                        parametros.Add("@ID_SUPERV", request.Id_Supervisor);
                    }

                    if (request.Id_Vendedor.HasValue)
                    {
                        sql.AppendLine(" AND (B.ID_VENDEDOR = @ID_VENDEDOR) ");
                        parametros.Add("@ID_VENDEDOR", request.Id_Vendedor);
                    }

                    if (request.Tipo_Cadastro.HasValue)
                    {
                        if (request.Tipo_Cadastro != null)
                        {
                            sql.AppendLine(" AND (D.TIPO_CLIENTE = @TIPO_CLIENTE) ");
                            parametros.Add("@TIPO_CLIENTE", request.Id_Vendedor);
                        }
                    }

                    if (request.UF.HasValue)
                    {
                        sql.AppendLine(" AND (J.ID = @ENDERECO_UF) ");
                        parametros.Add("@ENDERECO_UF", request.UF);
                    }

                    if (request.Cidade.HasValue)
                    {
                        sql.AppendLine(" AND (I.ID = @ENDERECO_CIDADE) ");
                        parametros.Add("@ENDERECO_CIDADE", request.Cidade);
                    }

                    //Plano
                    if (request.Plano != null)
                    {
                        String ids = $"({String.Join(",", request.Plano)})";
                        sql.AppendLine($" AND ((H.ID_PLANO IN {ids}) OR (H.ID_PLANO IN {ids}))");
                    }

                    //Produto
                    if (request.Produto != null)
                    {
                        String ids = $"({String.Join(",", request.Produto)})";
                        sql.AppendLine($" AND ((H.ID_PRODUTO IN {ids}) OR (H.ID_PRODUTO IN {ids}))");
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                    {
                        while (await reader.ReadAsync())
                        {

                            var idVenda = Convert.ToString(reader["ID"]);


                            int qtdContato = 0;

                            using (var comContato = session.Connection.CreateCommand())
                            {
                                var sqlContato = new StringBuilder();
                                sqlContato.AppendLine("SELECT COUNT(ID) AS CO FROM TB_VENDA_HISTORICO_CONTATO HC");
                                sqlContato.AppendLine("WHERE HC.ID_VENDA = @ID_VENDA");

                                parametrosContato = new Dictionary<string, object>
        {
            { "@ID_VENDA", idVenda }
        };

                                using (var readerContato = await CommandHelper.ExecuteReader(
                                    comContato, sqlContato, parametrosContato, session.Connection, null))
                                {
                                    if (await readerContato.ReadAsync())
                                    {
                                        qtdContato = readerContato["CO"] != DBNull.Value
                                            ? Convert.ToInt32(readerContato["CO"])
                                            : 0;
                                    }
                                }
                            }


                            int qtdPlanoProduto = 0;
                            decimal valorTotal = 0;

                            using (var comPlanoProduto = session.Connection.CreateCommand())
                            {
                                var sqlPlanoProduto = new StringBuilder();
                                sqlPlanoProduto.AppendLine("SELECT COUNT(ID) AS CO, SUM(CPP.VALOR_FINAL_TOTAL) AS VALOR_FINAL_TOTAL");
                                sqlPlanoProduto.AppendLine("FROM TB_CLIENTE_PLANO_PRODUTO CPP");
                                sqlPlanoProduto.AppendLine("WHERE CPP.ID_VENDA = @ID_VENDA");

                                parametrosPlanoProduto = new Dictionary<string, object>
        {
            { "@ID_VENDA", idVenda }
        };

                                using (var readerPlanoProduto = await CommandHelper.ExecuteReader(
                                    comPlanoProduto, sqlPlanoProduto, parametrosPlanoProduto, session.Connection, null))
                                {
                                    if (await readerPlanoProduto.ReadAsync())
                                    {
                                        qtdPlanoProduto = readerPlanoProduto["CO"] != DBNull.Value
                                            ? Convert.ToInt32(readerPlanoProduto["CO"])
                                            : 0;

                                        valorTotal = readerPlanoProduto["VALOR_FINAL_TOTAL"] != DBNull.Value
                                            ? Convert.ToDecimal(readerPlanoProduto["VALOR_FINAL_TOTAL"])
                                            : 0;
                                    }
                                }
                            }


                            retorno.Add(new RelatorioVendasNaoEfetivadasModel
                            {
                                num_venda = idVenda,
                                tipo_cadastro = reader["TIPO_CLIENTE"] != DBNull.Value
                                    ? HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>(Convert.ToString(reader["TIPO_CLIENTE"]))
                                    : "-",
                                data_criacao = Convert.ToString(reader["DATAHORACRIACAO"]),
                                data_cancelamento = Convert.ToString(reader["DATA_VENDA"]),
                                motivo_cancelamento = Convert.ToString(reader["MOTIVO_CANCELAMENTO"]),
                                cliente_prospect = $"{reader["CPF"]} - {reader["NOMEFANTASIA"]}",
                                categoria_cliente = Convert.ToString(reader["CATEGORIA"]),
                                valor_venda = valorTotal.ToString("F2"), // formato 2 casas decimais
                                uf_cidade = $"{reader["ENDERECO_UF"]} - {reader["ENDERECO_CIDADE"]}",
                                bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                vendedor = $"{reader["NUM_FRE"]} - {reader["VENDEDOR"]}",
                                contatos_realizados = qtdContato.ToString(),
                                planos_produtos = qtdPlanoProduto.ToString()
                            });
                        }

                    }
                }
            }

            listaOrdenada = retorno.OrderBy(p => p.num_venda);

            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<RelatorioVendasNaoEfetivadasModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }


        [HttpPost()]
        [Route("api/relatorioapi/RelatorioReagendamentoOS")]
        public async Task<DataTableResponse<RelatorioReagendamentoOSModel>> RelatorioReagendamentoOS(RelatorioReagendamentoOS request)
        {
            #region [RelatorioReagendamentoOS]

            #region [retornoDtTable]
            var retornoDtTable = new DataTableResponse<RelatorioReagendamentoOSModel>()
            {
                data = new List<RelatorioReagendamentoOSModel>(),
                recordsFiltered = 0,
                draw = request.draw
            };
            #endregion

            #region [TRY / CATCH]
            try
            {
                #region [Variaveis]
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<String> condicoes = new List<String>();
                StringBuilder sqlFrom = new StringBuilder();
                int recordsTotal = 0;
                var lista = new List<RelatorioReagendamentoOSModel>();
                #endregion
                
                #region [ESCOPO]
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    #region [SELECT]
                    sqlFrom.AppendLine("FROM ");
                    sqlFrom.AppendLine("    TB_DOCUMENTO_PRODUCAO A ");
                    sqlFrom.AppendLine("    LEFT JOIN TB_EQUIPE B ON A.EQUIPE = B.EQUIPE");
                    sqlFrom.AppendLine("    LEFT JOIN TB_ESCRITORIO C ON A.COD_ESCRITORIO = C.COD_ESCRITORIO");
                    sqlFrom.AppendLine("    LEFT JOIN TB_AREA D ON D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA");
                    sqlFrom.AppendLine("    LEFT JOIN TB_LOCALIDADE E ON E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND A.COD_LOCALIDADE = E.COD_LOCALIDADE");
                    sqlFrom.AppendLine("    LEFT JOIN TB_TIPO_SERVICO_EQUIPE F ON A.COD_TIPO_SERV = F.COD_TIPO_SERV");
                    sqlFrom.AppendLine("    LEFT JOIN TB_SUB_TIPO_SERVICO G ON A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV");
                    sqlFrom.AppendLine("    LEFT JOIN TB_CLIENTE H ON A.ID_CLIENTE = H.COD_CLIENTE");
                    sqlFrom.AppendLine("    LEFT JOIN TB_CODIGO_ENCERRAMENTO I ON I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO");
                    sqlFrom.AppendLine("    LEFT JOIN TB_TIPO_IMOVEL J ON A.PARPRIM_PARA = J.COD_TIPO_IMOVEL");
                    sqlFrom.AppendLine("    LEFT JOIN (");
                    sqlFrom.AppendLine("        SELECT NUM_OBRA, COUNT(*) AS QUANTIDADE_REAGENDAMENTOS");
                    sqlFrom.AppendLine("        FROM TB_LOG_LANC_SERVICO");
                    sqlFrom.AppendLine("        WHERE COD_MOTIVO_REAGENDAMENTO IS NOT NULL");
                    sqlFrom.AppendLine("        GROUP BY NUM_OBRA");
                    sqlFrom.AppendLine("    ) LLS ON LLS.NUM_OBRA = A.NUM_OBRA");
                    sqlFrom.AppendLine("WHERE ");
                    sqlFrom.AppendLine("    COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");
                    sqlFrom.AppendLine("    AND LLS.QUANTIDADE_REAGENDAMENTOS > 0");
                    #endregion

                    #region [FILTROS]
                    if (!string.IsNullOrEmpty(request.TipoData))
                    {
                        if (request.TipoData == "CRIACAO")
                        {
                            sqlFrom.AppendLine(" AND CAST(A.DIA AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else if (request.TipoData == "AGENDAMENTO")
                        {
                            sqlFrom.AppendLine(" AND CAST(A.DATAHORA_AGENDAMENTO AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else
                        {
                            sqlFrom.AppendLine(" AND CAST(A.DATA_PROD AS DATE) BETWEEN @DATAINICIO AND @DATAFIM ");
                        }

                        parametros.Add("@DATAINICIO", request.Data_Inicio);
                        parametros.Add("@DATAFIM", request.Data_Fim);
                    }

                    if (!string.IsNullOrEmpty(request.NumOs))
                    {
                        sqlFrom.AppendLine(" AND A.NUM_OBRA_ORIGINAL = @NUMOS ");
                        parametros.Add("@NUMOS", request.NumOs);
                    }

                    if (!string.IsNullOrEmpty(request.Equipe))
                    {
                        sqlFrom.AppendLine(" AND A.EQUIPE = @EQUIPE ");
                        parametros.Add("@EQUIPE", request.Equipe);
                    }

                    if (!string.IsNullOrEmpty(request.Contrato))
                    {
                        sqlFrom.AppendLine(" AND A.CODCT = @CODCT ");
                        parametros.Add("@CODCT", request.Contrato);
                    }

                    if (!string.IsNullOrEmpty(request.Projeto))
                    {
                        sqlFrom.AppendLine(" AND A.PROJETO = @PROJETO ");
                        parametros.Add("@PROJETO", request.Projeto);
                    }

                    if (!string.IsNullOrEmpty(request.TipoServico))
                    {
                        sqlFrom.AppendLine(" AND A.COD_TIPO_SERV = @TIPOSERV ");
                        parametros.Add("@TIPOSERV", request.TipoServico);
                    }

                    if (!string.IsNullOrEmpty(request.Area))
                    {
                        sqlFrom.AppendLine(" AND A.COD_AREA = @COD_AREA ");
                        parametros.Add("@COD_AREA", request.Area);
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        sqlFrom.AppendLine(" AND A.COD_ESCRITORIO = @COD_ESCRITORIO ");
                        parametros.Add("@COD_ESCRITORIO", request.GrupoArea);
                    }

                    if (!string.IsNullOrEmpty(request.Localidade))
                    {
                        sqlFrom.AppendLine(" AND A.COD_LOCALIDADE= @COD_LOCALIDADE ");
                        parametros.Add("@COD_LOCALIDADE", request.Localidade);
                    }

                    #region [Filtros de Quantidade de Reabertura]
                    if (!String.IsNullOrEmpty(request.QtdeReabertura) && request.QtdeReabertura != "todas")
                    {
                        bool temPrimeiroValor = int.TryParse(request.primeiroValorReabertura, out int primeiroValor);
                        bool temSegundoValor = int.TryParse(request.segundoValorReabertura, out int segundoValor);

                        switch (request.QtdeReabertura)
                        {
                            case "entre":
                                if (temPrimeiroValor && temSegundoValor)
                                {
                                    sqlFrom.AppendLine(" AND A.SEQ BETWEEN @PRIMEIRO_VALOR AND @SEGUNDO_VALOR ");
                                    parametros.Add("@PRIMEIRO_VALOR", primeiroValor);
                                    parametros.Add("@SEGUNDO_VALOR", segundoValor);
                                }
                                break;

                            case "maior":
                                if (temPrimeiroValor)
                                {
                                    sqlFrom.AppendLine(" AND A.SEQ > @PRIMEIRO_VALOR ");
                                    parametros.Add("@PRIMEIRO_VALOR", primeiroValor);
                                }
                                break;

                            case "menor":
                                if (temPrimeiroValor)
                                {
                                    sqlFrom.AppendLine(" AND A.SEQ < @PRIMEIRO_VALOR ");
                                    parametros.Add("@PRIMEIRO_VALOR", primeiroValor);
                                }
                                break;

                            case "maiorIgual":
                                if (temPrimeiroValor)
                                {
                                    sqlFrom.AppendLine(" AND A.SEQ >= @PRIMEIRO_VALOR ");
                                    parametros.Add("@PRIMEIRO_VALOR", primeiroValor);
                                }
                                break;

                            case "menorIgual":
                                if (temPrimeiroValor)
                                {
                                    sqlFrom.AppendLine(" AND A.SEQ <= @PRIMEIRO_VALOR ");
                                    parametros.Add("@PRIMEIRO_VALOR", primeiroValor);
                                }
                                break;

                            case "igual":
                                if (temPrimeiroValor)
                                {
                                    sqlFrom.AppendLine(" AND A.SEQ = @PRIMEIRO_VALOR ");
                                    parametros.Add("@PRIMEIRO_VALOR", primeiroValor);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    #endregion

                    #region [Filtros de Quantidade de Reagendamento]
                    if (!String.IsNullOrEmpty(request.QtdeReagendamento))
                    {
                        switch (request.QtdeReagendamento)
                        {
                            case "entre":
                                sqlFrom.AppendLine(" AND LLS.QUANTIDADE_REAGENDAMENTOS BETWEEN @PRIMEIRO_VALOR AND @SEGUNDO_VALOR");
                                parametros.Add("@PRIMEIRO_VALOR", request.primeiroValorReagendamento);
                                parametros.Add("@SEGUNDO_VALOR", request.segundoValorReagendamento);
                                break;

                            case "igual":
                                sqlFrom.AppendLine(" AND LLS.QUANTIDADE_REAGENDAMENTOS = @PRIMEIRO_VALOR");
                                parametros.Add("@PRIMEIRO_VALOR", request.primeiroValorReagendamento);
                                break;

                            case "maior":
                                sqlFrom.AppendLine(" AND LLS.QUANTIDADE_REAGENDAMENTOS > @PRIMEIRO_VALOR");
                                parametros.Add("@PRIMEIRO_VALOR", request.primeiroValorReagendamento);
                                break;

                            case "menor":
                                sqlFrom.AppendLine(" AND LLS.QUANTIDADE_REAGENDAMENTOS < @PRIMEIRO_VALOR");
                                parametros.Add("@PRIMEIRO_VALOR", request.primeiroValorReagendamento);
                                break;

                            case "maiorIgual":
                                sqlFrom.AppendLine(" AND LLS.QUANTIDADE_REAGENDAMENTOS >= @PRIMEIRO_VALOR");
                                parametros.Add("@PRIMEIRO_VALOR", request.primeiroValorReagendamento);
                                break;

                            case "menorIgual":
                                sqlFrom.AppendLine(" AND LLS.QUANTIDADE_REAGENDAMENTOS <= @PRIMEIRO_VALOR");
                                parametros.Add("@PRIMEIRO_VALOR", request.primeiroValorReagendamento);
                                break;
                        }
                    }
                    #endregion

                    #endregion

                    #region [Total Geral]
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sqlCount = new StringBuilder("SELECT COUNT(A.NUM_OBRA_ORIGINAL) CO ");
                        sqlCount.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                        }

                        retornoDtTable.recordsFiltered = recordsTotal;
                    }
                    #endregion

                    #region [SQL - RETORNO]
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT ");
                        sql.AppendLine("    A.NUM_OBRA_ORIGINAL, ");
                        sql.AppendLine("    A.NUM_DOC, ");
                        sql.AppendLine("    A.OBSERVACAO_MOTIVO_REAGENDAMENTO, ");
                        sql.AppendLine("    A.SEQ AS QTDEREABERTURA,");
                        sql.AppendLine("    A.DIA + COALESCE(A.HORA, CAST('00:00' AS TIME)) AS DATAHORA_ABERTURA_OS,");
                        sql.AppendLine("    A.DATA_HORA_REABERTURA AS DATAHORA_REABERTURA,");
                        sql.AppendLine("    A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)) AS DATAHORA_AGENDAMENTO_OS,");
                        sql.AppendLine("    B.NOME AS NOME_EQUIPE, ");
                        sql.AppendLine("    B.NUM_FRE AS MATRICULA_EQUIPE,");
                        sql.AppendLine("    C.DESCRICAO AS DESCRICAO_GRUPO_AREA, ");
                        sql.AppendLine("    F.DESCRICAO AS DESCRICAO_TIPO_SERV, ");
                        sql.AppendLine("    F.COD_TIPO_SERV,");
                        sql.AppendLine("    COALESCE(H.RAZAO_SOCIAL,A.TITULAR) AS NOME_CLIENTE,");
                        sql.AppendLine("    A.ENDERECO AS ENDERECO_CLIENTE,");
                        sql.AppendLine("    A.ENDERECO_COMPL AS ENDERECO_COMPL,");
                        sql.AppendLine("    A.ENDERECO_TIPO AS ENDERECO_CIDADE, ");
                        sql.AppendLine("    A.ENDERECO_BAIRRO AS ENDERECO_BAIRRO, ");
                        sql.AppendLine("    A.ENDERECO_UF AS ENDERECO_UF, ");
                        sql.AppendLine("    A.ENDERECO_CPL AS ENDERECO_CEP, ");
                        sql.AppendLine("    A.ENDERECO_NUMERO, ");
                        sql.AppendLine("    A.USUARIO_BAIXA, ");
                        sql.AppendLine("    'Brasil' AS ENDERECO_PAIS, ");
                        sql.AppendLine("    I.DESCRICAO AS DESCRICAO_ENCERRAMENTO, ");
                        sql.AppendLine("    J.DESCRICAO AS TIPO_IMOVEL,");
                        sql.AppendLine("    A.STATUS_AUDITORIA,");
                        sql.AppendLine("    A.DATA_PROD,");
                        sql.AppendLine("    A.USUARIO_REABERTURA,");
                        sql.AppendLine("    LLS.QUANTIDADE_REAGENDAMENTOS ");
                        sql.AppendLine(sqlFrom.ToString());

                        sql.AppendLine("ORDER BY ");
                        sql.AppendLine("    NUM_OBRA_ORIGINAL DESC;");

                        using (var reader = CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start).Result)
                        {
                            while (reader.Read())
                            {
                                lista.Add(new RelatorioReagendamentoOSModel
                                {
                                    NumOS = reader["NUM_OBRA_ORIGINAL"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA_ORIGINAL"]) : null,
                                    QtdReabertura = reader["QTDEREABERTURA"] != DBNull.Value ? Convert.ToInt64(reader["QTDEREABERTURA"]) : 0,
                                    TecnicoResponsavel = reader["NOME_EQUIPE"] != DBNull.Value ? Convert.ToString(reader["NOME_EQUIPE"]) + " - " + Convert.ToString(reader["MATRICULA_EQUIPE"]) : null,
                                    DataAgendamento = reader["DATAHORA_AGENDAMENTO_OS"] != DBNull.Value ? Convert.ToString(reader["DATAHORA_AGENDAMENTO_OS"]) : null,
                                    QtdReagendamento = reader["QUANTIDADE_REAGENDAMENTOS"] != DBNull.Value ? Convert.ToInt64(reader["QUANTIDADE_REAGENDAMENTOS"]) : 0,
                                    TipoServico = reader["DESCRICAO_TIPO_SERV"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_TIPO_SERV"]) : null,
                                    DataCriacao = reader["DATAHORA_ABERTURA_OS"] != DBNull.Value ? Convert.ToString(reader["DATAHORA_ABERTURA_OS"]) : null,
                                    DataEncerramento = reader["DATA_PROD"] != DBNull.Value ? Convert.ToString(reader["DATA_PROD"]) : null,
                                    UsuarioEncerramento = reader["USUARIO_BAIXA"] != DBNull.Value ? Convert.ToString(reader["USUARIO_BAIXA"]) : null,
                                    Cliente = reader["NOME_CLIENTE"] != DBNull.Value ? Convert.ToString(reader["NOME_CLIENTE"]) : null,
                                    Endereco = reader["ENDERECO_CLIENTE"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_CLIENTE"]) : null,
                                    Complemento = reader["ENDERECO_COMPL"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_COMPL"]) : null,
                                    Bairro = reader["ENDERECO_BAIRRO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_BAIRRO"]) : null,
                                    Cidade = reader["ENDERECO_CIDADE"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_CIDADE"]) : null,
                                    GrupoArea = reader["DESCRICAO_GRUPO_AREA"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_GRUPO_AREA"]) : null,
                                    DescricaoEncerramento = reader["DESCRICAO_ENCERRAMENTO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_ENCERRAMENTO"]) : null,
                                    ObservacaoTecnico = reader["OBSERVACAO_MOTIVO_REAGENDAMENTO"] != DBNull.Value ? Convert.ToString(reader["OBSERVACAO_MOTIVO_REAGENDAMENTO"]) : null,
                                    StatusAuditoria = reader["STATUS_AUDITORIA"] != DBNull.Value ? Convert.ToString(reader["STATUS_AUDITORIA"]) : null
                                });
                            }

                        }
                    }
                    #endregion
                }

                retornoDtTable.data = lista;
                #endregion

            }
            catch (Exception e)
            {
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", e.Message, e?.InnerException?.Message, e.StackTrace);
            }

            #endregion

            return retornoDtTable;
            #endregion
        }

        [HttpPost()]
        [Route("api/relatorioapi/RelatorioContratoCliente")]
        public async Task<DataTableResponse<Relatorio_Contrato_Cliente>> RelatorioContratoCliente(Relatorio_Contrato_ClienteDataTableRequest request)
        {
            int recordsTotal = 0;
            List<Relatorio_Contrato_Cliente> retorno = new List<Relatorio_Contrato_Cliente>();
            using (ISession session = HibernateUtil.GetSession())
            {
                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();

                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        sqlWhere.AppendLine(" AND (A.NUMERO_CONTRATO  = @CODCT)");
                        parametros.Add("@CODCT", request.Contrato);
                    }

                    if (!String.IsNullOrEmpty(request.CodCliente))
                    {
                        sqlWhere.AppendLine("AND (A.ID_CLIENTE  = @COD_CLIENTE)");
                        parametros.Add("@COD_CLIENTE", request.CodCliente);
                    }

                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        sqlWhere.AppendLine("AND (G.ENDERECO_BAIRRO = @BAIRRO)");
                        parametros.Add("@BAIRRO", request.Bairro);
                    }

                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        sqlWhere.AppendLine("AND G.ENDERECO_CIDADE = @CIDADE");
                        parametros.Add("@CIDADE", request.Cidade);
                    }

                    if (!String.IsNullOrEmpty(request.Data_Inicial))
                    {
                        request.Data_Inicial = request.Data_Inicial + " 00:00";
                        request.Data_Final = request.Data_Final + " 23:59";
                        if (!request.TipoData.HasValue || request.TipoData == 0)
                        {
                            sqlWhere.AppendLine("AND A.DATAHORACRIACAO BETWEEN @DATAINICIAL_ABERTURA_VENDA AND @DATAFINAL_ABERTURA_VENDA");
                            parametros.Add("@DATAINICIAL_ABERTURA_VENDA", String.Format("{0}", request.Data_Inicial));
                            parametros.Add("@DATAFINAL_ABERTURA_VENDA", String.Format("{0}", request.Data_Final));

                        }
                        else if (request.TipoData == 1)
                        {
                            sqlWhere.AppendLine("AND A.DATA_ACEITE_CONTRATO BETWEEN @DATAINICIAL_ACEITE AND @DATAFINAL_ACEITE");
                            parametros.Add("@DATAINICIAL_ACEITE", String.Format("{0}", request.Data_Inicial));
                            parametros.Add("@DATAFINAL_ACEITE", String.Format("{0}", request.Data_Final));
                        }

                        else if (request.TipoData == 2)
                        {
                            sqlWhere.AppendLine("AND (A.DATAHORACRIACAO BETWEEN @DATAINICIAL AND @DATAFINAL");
                            sqlWhere.AppendLine("OR A.DATA_VENDA BETWEEN @DATAINICIAL AND @DATAFINAL)");
                            parametros.Add("@DATAINICIAL", String.Format("{0}", request.Data_Inicial));
                            parametros.Add("@DATAFINAL", String.Format("{0}", request.Data_Final));
                        }
                        else if (request.TipoData == 3)
                        {
                            sqlWhere.AppendLine("AND E.DATAHORACRIACAO BETWEEN @DATAINICIAL_ENCERRAMENTO AND @DATAFINAL_ENCERRAMENTO");
                            parametros.Add("@DATAINICIAL_ENCERRAMENTO", String.Format("{0}", request.Data_Inicial));
                            parametros.Add("@DATAFINAL_ENCERRAMENTO", String.Format("{0}", request.Data_Final));
                        }
                        else if (request.TipoData == 4)
                        {
                            sqlWhere.AppendLine("AND (D.DATAHORACRIACAO BETWEEN @DATAINICIALSUSP AND @DATAFINALSUSP)");
                            parametros.Add("@DATAINICIALSUSP", String.Format("{0}", request.Data_Inicial));
                            parametros.Add("@DATAFINALSUSP", String.Format("{0}", request.Data_Final));
                        }
                    }
                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        // utiliza o filtro de status caso não esteja presente no modelo de exibição mas o usuário filtre pelo status manualmente
                        sqlWhere.AppendLine($" AND A.STATUS IN (@STATUS)");
                        parametros.Add("@STATUS", request.Status);
                    }


                    var groupby = String.Empty;

                    sql.AppendLine($@" SELECT COUNT(*) OVER() AS TOTAL_REG,A.ID_CLIENTE,B.NOMEFANTASIA ,A.NUMERO_CONTRATO,CAST(A.DATAHORACRIACAO AS DATE) AS DATAHORACRIACAO ,CAST(A.DATA_ACEITE_CONTRATO AS DATE) AS DATA_ACEITE_CONTRATO , 
                                             CAST(DATEADD(A.PERIODO_CARENCIA MONTH  TO A.DATA_ACEITE_CONTRATO) AS DATE) AS DataFinal,
                                             A.STATUS,MAX(D.DATAHORACRIACAO) AS DATASUPENSAO,MAX(E.DATAHORACRIACAO) AS DATACANCELAMENTO,COALESCE(SUM(C.VALOR_FINAL), 0) AS VALORSEMDES, 
                                             COALESCE(SUM(M.VALOR_DESCONTO), 0) AS DESCONTO,
                                             COALESCE(SUM(L.VALOR_DESCONTO), 0) AS ACRESSIMO,  
                                             COALESCE(SUM(C.VALOR_FINAL), 0) + COALESCE(SUM(L.VALOR_DESCONTO), 0) + COALESCE(SUM(M.VALOR_DESCONTO), 0) AS VALORCOMDESC,
                                             G.ENDERECO_BAIRRO ,G.ENDERECO_CIDADE  
                                       FROM TB_CLIENTE_CONTRATO A 
                                       INNER JOIN TB_CLIENTE B ON B.ID =A.ID_CLIENTE 
                                       LEFT JOIN TB_CLIENTE_PLANO_PRODUTO C ON C.ID_CLIENTE_CONTRATO =A.ID 
                                       LEFT JOIN TB_MOTIVO_SUSPENSAO_CONTRATUAL D ON D.ID =C.ID_MOTIVO_SUSPENSAO_CONTRATUAL 
                                       LEFT JOIN TB_MOTIVO_CANC_PLANOPRODUTO E ON E.ID =C.ID_MOTIVO_CANCELAMENTO
                                       LEFT JOIN TB_PLANO F ON F.ID =C.ID_PLANO
                                       LEFT JOIN TB_ENDERECO_CLIENTE G ON G.ID =A.ID_ENDERECO 
                                       LEFT JOIN TB_CLIENTE_DESCONTO H ON H.ID =A.ID_CLIENTE 
                                       LEFT JOIN TB_DESCONTO I ON I.ID =H.ID_DESCONTO 
                                       LEFT JOIN TB_TIPO_DESCONTO J ON J.ID =I.ID_TIPO_DESCONTO 
                                       LEFT JOIN TB_DESCONTO l ON l.ID_TIPO_DESCONTO =J.ID  AND J.FORMATO =0
                                       LEFT JOIN TB_DESCONTO M ON M.ID_TIPO_DESCONTO =J.ID  AND J.FORMATO =1
                                       WHERE A.ID_CLIENTE IS NOT NULL
                                       {sqlWhere.ToString()}
                                       GROUP BY A.ID_CLIENTE,B.NOMEFANTASIA ,A.NUMERO_CONTRATO,CAST(A.DATAHORACRIACAO AS DATE),CAST(A.DATA_ACEITE_CONTRATO AS DATE) ,A.PERIODO_CARENCIA , 
                                              CAST(DATEADD(A.PERIODO_CARENCIA MONTH  TO A.DATA_ACEITE_CONTRATO) AS DATE) ,
                                              A.STATUS,G.ENDERECO_BAIRRO ,G.ENDERECO_CIDADE                               		
                                         ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Relatorio_Contrato_Cliente
                            {
                                CodCliente = Convert.ToString(reader["ID_CLIENTE"]),
                                NomeCliente = Convert.ToString(reader["NOMEFANTASIA"]),
                                Contrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                                DTCriacao = Convert.ToString(reader["DATAHORACRIACAO"]),
                                DTAceite = Convert.ToString(reader["DATA_ACEITE_CONTRATO"]),
                                DTFinal = Convert.ToString(reader["DataFinal"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Contrato>(Convert.ToString(reader["STATUS"])),
                                DTSuspensao = Convert.ToString(reader["DATASUPENSAO"]),
                                DTCancelado = Convert.ToString(reader["DATACANCELAMENTO"]),
                                Valor = Convert.ToDouble(reader["VALORSEMDES"]),
                                Desconto = Convert.ToDouble(reader["DESCONTO"]),
                                Acressimo = Convert.ToDouble(reader["DESCONTO"]),
                                ValorTotal = Convert.ToDouble(reader["VALORCOMDESC"]),
                                Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),

                            };

                            recordsTotal = Convert.ToInt32(reader["TOTAL_REG"]);
                            retorno.Add(obj);

                        }



                    }

                }
            }

            return new DataTableResponse<Relatorio_Contrato_Cliente>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/relatorioapi/RelatorioVendasMesmoEndereco")]
        public async Task<DataTableResponse<RelatorioMesmaVendaEnderecoResponseViewModel>> RelatorioVendasMesmoEndereco(RelatorioMesmaVendaEnderecoViewModel request)
        {
            int recordsTotal = 0;
            List<RelatorioMesmaVendaEnderecoResponseViewModel> response = new List<RelatorioMesmaVendaEnderecoResponseViewModel>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();

                    sql.AppendLine(" SELECT DISTINCT A.ID AS ID_VENDA, A.DATAHORACRIACAO AS DATACRIACAO, A.DATA_VENDA, A.STATUSVENDA, B.NOMEFANTASIA, C.TITULO AS CAMPANHA, D.DESCRICAO AS MOTIVO_DESISTENCIA,");
                    sql.AppendLine("	CASE ");
                    sql.AppendLine("        WHEN E.ID_ENDERECO_LOGRADOURO IS NOT NULL THEN 'Endereço/Logradouro' ");
                    sql.AppendLine("        WHEN E.ID_CONDOMINIO_PREDIO IS NOT NULL THEN 'Condomínio/Prédio' ");
                    sql.AppendLine("        ELSE 'Endereço/Logradouro' ");
                    sql.AppendLine("    END AS ENDERECO_POR, ");
                    sql.AppendLine("	CASE ");
                    sql.AppendLine("        WHEN E.ID_ENDERECO_LOGRADOURO IS NOT NULL ");
                    sql.AppendLine("            THEN COALESCE(E.ENDERECO_LOGRADOURO, G.DESCRICAO) || ', ' || COALESCE(E.ENDERECO_NUMERO, '-') || ', ' || ");
                    sql.AppendLine("                 COALESCE(I.DESCRICAO, E.ENDERECO_BAIRRO) || ', ' || COALESCE(H.DESCRICAO, E.ENDERECO_CIDADE) || ', ' || ");
                    sql.AppendLine("                 COALESCE(J.DESCRICAO, E.ENDERECO_UF) || ', ' || K.NOME ");
                    sql.AppendLine("        WHEN E.ID_CONDOMINIO_PREDIO IS NOT NULL ");
                    sql.AppendLine("            THEN COALESCE(F.DESCRICAO, '-') || ', ' || COALESCE(F.LOGRADOURO, '-') || ', ' || ");
                    sql.AppendLine("                 COALESCE(F.NUMERO, '-') || ', Bloco: ' || COALESCE(E.BLOCO, '-') || ', Apartamento: ' || ");
                    sql.AppendLine("                 COALESCE(E.APARTAMENTO, '-') || ', ' || COALESCE(I.DESCRICAO, F.BAIRRO) || ', ' || ");
                    sql.AppendLine("                 COALESCE(H.DESCRICAO, '-') || ', ' || COALESCE(J.DESCRICAO, F.UF) || ', ' || K.NOME ");
                    sql.AppendLine("        ELSE ");
                    sql.AppendLine("            COALESCE(E.ENDERECO_LOGRADOURO, '-') || ', ' || COALESCE(E.ENDERECO_NUMERO, '-') || ', ' || ");
                    sql.AppendLine("            COALESCE(I.DESCRICAO, E.ENDERECO_BAIRRO, '-') || ', ' || COALESCE(E.ENDERECO_CIDADE, '-') || ', ' || ");
                    sql.AppendLine("            COALESCE(J.DESCRICAO, E.ENDERECO_UF, '-') || ', ' || K.NOME");
                    sql.AppendLine("    END AS ENDERECO, A.JUSTIFICATIVA_AVALIACAO ");
                    sql.AppendLine(" FROM TB_VENDA A ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE B ON(B.ID = A.ID_CLIENTE) ");
                    sql.AppendLine(" LEFT JOIN TB_CAMPANHA C ON(C.ID = A.ID_CAMPANHA) ");
                    sql.AppendLine(" LEFT JOIN TB_MOTIVO_DESISTENCIA_VENDA D ON(D.ID = A.ID_MOTIVO_DESISTENCIA_VENDA) ");
                    sql.AppendLine(" LEFT JOIN TB_ENDERECO_CLIENTE E ON(E.ID = A.ID_ENDERECO_INSTALACAO) ");
                    sql.AppendLine(" LEFT JOIN TB_CONDOMINIO_PREDIO F ON(F.ID = E.ID_CONDOMINIO_PREDIO) ");
                    sql.AppendLine(" LEFT JOIN TB_ENDERECO_LOGRADOURO G ON(G.ID = E.ID_ENDERECO_LOGRADOURO OR G.ID = F.ID_ENDERECO_LOGRADOURO) ");
                    sql.AppendLine(" LEFT JOIN TB_CIDADE H ON(H.ID = G.ID_CIDADE) ");
                    sql.AppendLine(" LEFT JOIN TB_BAIRRO I ON(I.ID = G.ID_BAIRRO) ");
                    sql.AppendLine(" LEFT JOIN TB_UF J ON(J.ID = H.ID_UF) ");
                    sql.AppendLine(" LEFT JOIN TB_PAIS K ON(K.ID = J.ID_PAIS) ");
                    sql.AppendLine(" LEFT JOIN TB_WORKFLOW_ATIVIDADE L ON(L.ID = A.ID_WORKFLOW_VENDAS_ATIVIDADE) ");
                    sql.AppendLine(" WHERE A.ID IS NOT NULL AND B.STATUS = '1' ");

                    if (!string.IsNullOrEmpty(request.DataInicio) && !string.IsNullOrEmpty(request.DataFim))
                    {
                        DateTime dataInicio = Convert.ToDateTime(request.DataInicio).Date;
                        DateTime dataFim = Convert.ToDateTime(request.DataFim).Date;

                        if (dataInicio > dataFim)
                            throw new Exception("Data fim não pode ser maior que Data de ínicio");

                        if (request.TipoData == "0")
                            sql.AppendLine($" AND A.DATA_VENDA BETWEEN  @DATAINICIO AND @DATAFIM ");
                        else
                            sql.AppendLine($" AND A.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAFIM ");

                        parametros.Add("@DATAINICIO", dataInicio.ToString("yyyy-MM-dd 00:00:00"));
                        parametros.Add("@DATAFIM", dataFim.ToString("yyyy-MM-dd 23:59:59"));
                    }

                    if (!string.IsNullOrEmpty(request.Status))
                    {
                        if (request.TipoStatus == "0")
                            sql.AppendLine($" AND A.STATUS = @STATUS ");
                        else
                            sql.AppendLine($" AND L.ATIV_STATUSVENDA = @STATUS ");

                        parametros.Add("@STATUS", request.Status);
                    }

                    if (!string.IsNullOrEmpty(request.EnderecoPor))
                    {
                        if (request.EnderecoPor == "0")
                            sql.AppendLine($" AND E.ID_ENDERECO_LOGRADOURO IS NOT NULL");
                        else
                            sql.AppendLine($" AND E.ID_CONDOMINIO_PREDIO IS NOT NULL");
                    }


                    if (!string.IsNullOrEmpty(request.IdCondominioPredio))
                    {
                        sql.AppendLine($" AND F.ID = @CONDPRED ");
                        parametros.Add("@CONDPRED", Convert.ToInt64(request.IdCondominioPredio));
                    }

                    if (!string.IsNullOrEmpty(request.CEP))
                    {
                        sql.AppendLine($@" AND (REPLACE(REPLACE(E.ENDERECO_CEP, '.', ''), '-', '') = @CEP OR
                                                REPLACE(REPLACE(G.CEP, '.', ''), '-', '') = @CEP OR
                                                REPLACE(REPLACE(F.CEP, '.', ''), '-', '') = @CEP) ");
                        parametros.Add("@CEP", request.CEP.Replace(".", "").Replace("-", ""));
                    }

                    if (!string.IsNullOrEmpty(request.Pais))
                    {
                        sql.AppendLine($" AND K.ID = @PAIS ");
                        parametros.Add("@PAIS", Convert.ToInt64(request.Pais));
                    }

                    if (!string.IsNullOrEmpty(request.UF))
                    {
                        sql.AppendLine($" AND J.ID = @UF ");
                        parametros.Add("@UF", Convert.ToInt64(request.UF));
                    }

                    if (!string.IsNullOrEmpty(request.Cidade))
                    {
                        sql.AppendLine($" AND H.ID = @Cidade ");
                        parametros.Add("@Cidade", Convert.ToInt64(request.Cidade));
                    }

                    if (!string.IsNullOrEmpty(request.Bairro))
                    {
                        sql.AppendLine($" AND I.ID = @BAIRRO ");
                        parametros.Add("@BAIRRO", Convert.ToInt64(request.Bairro));
                    }

                    if (!string.IsNullOrEmpty(request.Campanha))
                    {
                        sql.AppendLine($" AND C.ID = @CAMPANHA ");
                        parametros.Add("@CAMPANHA", Convert.ToInt64(request.Campanha));
                    }

                    if (request.IdsMotivos != null && request.IdsMotivos.Count > 0)
                    {
                        var listaIds = request.IdsMotivos.Where(id => id != 0).ToList();
                        bool isIdOutro = request.IdsMotivos.Contains(0);

                        if (isIdOutro && listaIds.Count > 0)
                            sql.AppendLine($" AND (A.ID_MOTIVO_DESISTENCIA_VENDA IN ({String.Join(", ", listaIds)}) OR A.ID_MOTIVO_DESISTENCIA_VENDA IS NULL AND A.JUSTIFICATIVA_AVALIACAO IS NOT NULL) ");
                        else if (isIdOutro)
                            sql.AppendLine($" AND A.ID_MOTIVO_DESISTENCIA_VENDA IS NULL AND A.JUSTIFICATIVA_AVALIACAO IS NOT NULL ");
                        else
                            sql.AppendLine($" AND A.ID_MOTIVO_DESISTENCIA_VENDA IN ({String.Join(", ", listaIds)}) ");
                    }

                    sql.AppendLine(@"GROUP BY A.ID, A.DATAHORACRIACAO, A.DATA_VENDA, A.STATUSVENDA, B.NOMEFANTASIA, C.TITULO, D.DESCRICAO, 
                                    E.ID_ENDERECO_LOGRADOURO,  E.ID_CONDOMINIO_PREDIO, E.ENDERECO_LOGRADOURO, E.ENDERECO_BAIRRO, E.ENDERECO_UF, E.ENDERECO_NUMERO, 
                                    J.DESCRICAO, H.DESCRICAO, K.NOME, F.DESCRICAO, F.NUMERO, F.LOGRADOURO, E.BLOCO, E.ENDERECO_CIDADE, G.DESCRICAO, I.DESCRICAO,E.APARTAMENTO, F.BAIRRO,F.UF, 
                                    A.JUSTIFICATIVA_AVALIACAO");

                    //if (!request.columns.Any(x => x.orderable == true))
                    //    sql.AppendLine("ORDER BY A.DATA_VENDA DESC");

                    ////Ordenacao
                    //if (request.order != null && request.order.Any())
                    //{
                    //    int sortColumnIndex = request.order[0].column;
                    //    var coluna = request.columns[sortColumnIndex].data;
                    //    string sortDirection = request.order[0].dir;
                    //    switch (coluna)
                    //    {
                    //        case "nome":
                    //            {
                    //                if (sortDirection == "asc")
                    //                    sql.AppendLine(" ORDER BY A.NOME ");

                    //                else
                    //                    sql.AppendLine(" ORDER BY A.NOME DESC ");
                    //                break;
                    //            }

                    //    }
                    //}


                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sqlCount = new StringBuilder();
                        sqlCount.AppendLine($"SELECT COUNT (*) AS CO FROM ({sql.ToString()}) ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new RelatorioMesmaVendaEnderecoResponseViewModel
                                {
                                    EnderecoPor = reader["ENDERECO_POR"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_POR"]) : "-",
                                    Endereco = reader["ENDERECO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO"]) : "-",
                                    Cliente = reader["NOMEFANTASIA"] != DBNull.Value ? Convert.ToString(reader["NOMEFANTASIA"]) : "-",
                                    DataCriacaoString = reader["DATACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATACRIACAO"]).ToString("dd/MM/yyyy") : "-",
                                    DataCriacao = reader["DATACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATACRIACAO"]) : (DateTime?)null,
                                    DataVendaString = reader["DATA_VENDA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VENDA"]).ToString("dd/MM/yyyy") : "-",
                                    DataVenda = reader["DATA_VENDA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VENDA"]) : (DateTime?)null,
                                    StatusVenda = reader["STATUSVENDA"] != DBNull.Value ? Convert.ToString(reader["STATUSVENDA"]) : "-",
                                    MotivoJustificativaVenda = Convert.IsDBNull(reader["MOTIVO_DESISTENCIA"]) ? Convert.IsDBNull(reader["JUSTIFICATIVA_AVALIACAO"]) ? "-" : Convert.ToString(reader["JUSTIFICATIVA_AVALIACAO"]) : Convert.ToString(reader["MOTIVO_DESISTENCIA"])
                                };

                                response.Add(obj);
                            }
                        }
                    }

                    if (response != null && response.Count > 0)
                    {
                        List<string> enderecos = response.Where(x => !string.IsNullOrEmpty(x.Endereco)).Select(x => x.Endereco).Distinct().ToList();

                        foreach (var end in enderecos)
                        {
                            var count = response.Count(x => x.Endereco == end);

                            foreach (var cli in response)
                            {
                                if (cli.Endereco == end)
                                    cli.QtdeVendasMesmoEndCliente = count;
                            }
                        }
                    }

                    if (request.QtdeVendasMesmoEndereco != null && !string.IsNullOrEmpty(request.Valor))
                    {
                        long valorComparacao = Convert.ToInt64(request.Valor);

                        switch (request.QtdeVendasMesmoEndereco)
                        {
                            case RelatorioQtdeVendas.MaiorQue:
                                response = response.Where(x => x.QtdeVendasMesmoEndCliente > valorComparacao).ToList();
                                break;
                            case RelatorioQtdeVendas.MenorQue:
                                response = response.Where(x => x.QtdeVendasMesmoEndCliente < valorComparacao).ToList();
                                break;
                            case RelatorioQtdeVendas.MaiorOuIgual:
                                response = response.Where(x => x.QtdeVendasMesmoEndCliente >= valorComparacao).ToList();
                                break;
                            case RelatorioQtdeVendas.MenorOuIgual:
                                response = response.Where(x => x.QtdeVendasMesmoEndCliente <= valorComparacao).ToList();
                                break;
                            case RelatorioQtdeVendas.Igual:
                                response = response.Where(x => x.QtdeVendasMesmoEndCliente == valorComparacao).ToList();
                                break;
                            case RelatorioQtdeVendas.Entre:
                                if (!string.IsNullOrEmpty(request.ValorFinal))
                                {
                                    response = response.Where(x => x.QtdeVendasMesmoEndCliente >= valorComparacao && x.QtdeVendasMesmoEndCliente <= valorComparacao).ToList();
                                }
                                break;
                        }
                    }
                }

                return new DataTableResponse<RelatorioMesmaVendaEnderecoResponseViewModel>
                {
                    draw = request.draw,
                    recordsFiltered = request.QtdeVendasMesmoEndereco != null && !string.IsNullOrEmpty(request.Valor) ? response.Count() : recordsTotal,
                    data = response,
                    error = ""
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponse<RelatorioMesmaVendaEnderecoResponseViewModel>
                {
                    draw = request.draw,
                    recordsFiltered = recordsTotal,
                    data = response,
                    error = ""
                };
            }
        }

        [HttpPost()]
        [Route("api/relatorioapi/RelatorioSOS")]
        public async Task<DataTableResponse<RelatorioSOSViewModel>> RelatorioSOS(RelatorioSOSRequestModel request)
        {
            try
            {
                List<RelatorioSOSViewModel> retorno = new List<RelatorioSOSViewModel>();
                int recordsTotal = 0;
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<AtividadeMongo>(conexaoMongo))
                using (ISession session = HibernateUtil.GetSession())
                using (var repositoryUser = new Conexao<IdentityUser>(session))
                {

                    var filtroBuilder = Builders<AtividadeMongo>.Filter;
                    var filtros = new List<FilterDefinition<AtividadeMongo>>();

                    if (request.ConfirmadoCiencia != null)
                        filtros.Add(filtroBuilder.Eq(u => u.ConfirmadoCiencia, request.ConfirmadoCiencia));
                    //          *-- Verificar se consta apenas uma data para filtragem --*
                    //if (!string.IsNullOrEmpty(request.DataHoraEnvio)) 
                    //{
                    //    DateTime dataEnvio = DateTime.Parse(request.DataHoraConfirmacao).ToUniversalTime();
                    //    filtros.Add(filtroBuilder.Gte(u => u.DataHoraEnvioNotificacao, dataEnvio));
                    //    filtros.Add(filtroBuilder.Lte(u => u.DataHoraEnvioNotificacao, dataEnvio));
                    //}

                    if (!String.IsNullOrEmpty(request.Tecnico))
                    {
                        filtros.Add(filtroBuilder.Regex(u => u.Equipe, new MongoDB.Bson.BsonRegularExpression(request.Tecnico)));
                    }

                    if (!String.IsNullOrEmpty(request.Supervisor))
                    {
                        filtros.Add(filtroBuilder.Regex(u => u.EquipeSupervisor, new MongoDB.Bson.BsonRegularExpression(request.Supervisor)));
                    }

                    if (!String.IsNullOrEmpty(request.Monitor))
                    {
                        filtros.Add(filtroBuilder.Regex(u => u.EquipeMonitor, new MongoDB.Bson.BsonRegularExpression(request.Monitor)));
                    }

                    if (!String.IsNullOrEmpty(request.Resposta))
                    {
                        var _resposta = (EnumRespostaSOS)Convert.ToInt32(request.Resposta);
                        filtros.Add(filtroBuilder.Eq(u => u.Resposta, _resposta));
                    }

                    if (!String.IsNullOrEmpty(request.Gerou_Notificacao) && request.Gerou_Notificacao == "SIM")
                        filtros.Add(filtroBuilder.Ne(u => u.DataHoraEnvioNotificacao, null));
                    else if (!String.IsNullOrEmpty(request.Gerou_Notificacao) && request.Gerou_Notificacao == "NAO")
                        filtros.Add(filtroBuilder.Eq(u => u.DataHoraEnvioNotificacao, null));

                    if (!string.IsNullOrEmpty(request.DataHoraIni) && !string.IsNullOrEmpty(request.DataHoraFim))
                    {
                        DateTime dataInicial = DateTime.Parse(request.DataHoraIni).ToUniversalTime();
                        DateTime dataFinal = DateTime.Parse(request.DataHoraFim).AddHours(23).AddMinutes(59).AddSeconds(59).ToUniversalTime();
                        filtros.Add(filtroBuilder.Gte(u => u.DataHoraCriacao, dataInicial));
                        filtros.Add(filtroBuilder.Lte(u => u.DataHoraCriacao, dataFinal));
                    }

                    var filtroFinal = filtros.Count > 0 ? filtroBuilder.And(filtros) : filtroBuilder.Empty;
                    recordsTotal = (await repositoryMongo.FindAllWFilter(filtroFinal)).Count();

                    var queryRelatorio = (await repositoryMongo.FindPageAsync(filtroFinal, request.start, request.length));

                    retorno = queryRelatorio.OrderBy(p => p.DataHoraCriacao).ToList().Select(p => new RelatorioSOSViewModel
                    {
                        //Id = p._id,
                        Tecnico = !String.IsNullOrEmpty(p.Nome) ? p.Nome : "-",
                        Monitor = !String.IsNullOrEmpty(p.NomeMonitor) ? p.NomeMonitor : "-",
                        Supervisor = !String.IsNullOrEmpty(p.NomeSupervisor) ? p.NomeSupervisor : "-",
                        //confirmar com Renato se datahoracriacao ou envio notificacao
                        DataHoraEnvio = p.Data,
                        ConfirmadoCiencia = p.ConfirmadoCiencia ? RsBase.SIM : RsBase.NAO,
                        DataHoraConfirmacao = p.DataHoraConfirmacao.HasValue ? p.DataHoraConfirmacao.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") : "-",
                        Resposta = p.Resposta == EnumRespostaSOS.SOS ? "SOS" : p.Resposta == EnumRespostaSOS.Ok ? "Respondido" : "Não Respondido",
                        GerouNotificacao = p.DataHoraEnvioNotificacao.HasValue ? RsBase.SIM : RsBase.NAO,
                        DthGerouNotificacao = p.DataHoraEnvioNotificacao.HasValue ? p.DataHoraEnvioNotificacao.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") : "-",
                        UsuarioConfirmacao = !String.IsNullOrEmpty(p.UsuarioConfirmacao) ? p.UsuarioConfirmacao + " - " + repositoryUser.selectFirst(x => x.Id == p.UsuarioConfirmacao).Result.Nome : "-",
                        EquipeMonitor = p.EquipeMonitor ?? "-",
                    }).ToList();
                }

                //Page e retornando DataTable
                return new DataTableResponse<RelatorioSOSViewModel>
                {
                    draw = request.draw,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };
            }
            catch (Exception e)
            {
                CONSTANTES.log4Net.Error(e.Message, e);
                throw e;
            }
        }

        #region [RelatorioVendasReprovadasScore]
        [HttpPost()]
        [Route("api/relatorioapi/RelatorioVendasReprovadasScore")]
        public async Task<DataTableResponse<RelatorioVendasReprovadasScoreViewModel>> RelatorioVendasReprovadasScore(RelatorioVendasReprovadasScoreRequestModel request)
        {
            #region [Retorno]
            var retornoDtTable = new DataTableResponse<RelatorioVendasReprovadasScoreViewModel>()
            {
                data = new List<RelatorioVendasReprovadasScoreViewModel>(),
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
                sqlFrom.AppendLine(" FROM TB_VENDA A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE B ON (A.ID_CLIENTE = B.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_FUNCIONARIO C ON (A.ID_VENDEDOR = C.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_FUNCIONARIO D ON (A.ID_SUPERVISOR = D.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN ASPNETUSERS ASP ON (C.ID = ASP.ID_FUNCIONARIO) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_MOTIVO_DESISTENCIA_VENDA E ON (E.ID = A.ID_MOTIVO_DESISTENCIA_VENDA) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_WORKFLOW_ATIVIDADE F ON(F.ID_WORKFLOW_VENDAS = A.ID_WORKFLOW_VENDAS AND F.ID_STATUS_PADRAO = A.STATUS) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_ATIVSTATUS_VENDA G ON(G.ID = F.ATIV_STATUSVENDA AND G.STATUS_PADRAO = A.STATUS) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_DEPARTAMENTO H ON(H.ID = A.ID_DEPARTAMENTO) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_ETAPA_VENDA I ON (I.ID = A.ID_ETAPA_VENDA) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CAMPANHA J ON (J.ID = A.ID_CAMPANHA) ");
                sqlFrom.AppendLine(" WHERE A.SCORE_CONSULTA_CREDITO IS NOT NULL AND ");
                #endregion

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {

                    #region [Filtros]
                    DateTime dtIni = DateTime.ParseExact(request.DataHoraIni, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime dtFim = DateTime.ParseExact(request.DataHoraFim, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (request.TipoData == 1)
                    {
                        condicoes.Add($" CAST(A.DATAHORACRIACAO AS DATE) BETWEEN @DATAHORACRIACAO_INI AND @DATAHORACRIACAO_FIM ");
                        parametros.Add("@DATAHORACRIACAO_INI", dtIni.ToString("yyyy-MM-dd"));
                        parametros.Add("@DATAHORACRIACAO_FIM", dtFim.ToString("yyyy-MM-dd"));
                    }
                    else if (request.TipoData == 2)
                    {
                        condicoes.Add($" CAST(A.DATA_VENDA AS DATE) BETWEEN @DATAVENDA_INI AND @DATAVENDA_FIM ");
                        parametros.Add("@DATAVENDA_INI", dtIni.ToString("yyyy-MM-dd"));
                        parametros.Add("@DATAVENDA_FIM", dtFim.ToString("yyyy-MM-dd"));
                    }

                    if (request.TipoStatusVenda == 1 && !String.IsNullOrEmpty(request.StatusVenda))
                    {
                        condicoes.Add($" A.STATUS = @STATUS ");
                        parametros.Add("@STATUS", Convert.ToInt32(request.StatusVenda));
                    }
                    else if (request.TipoStatusVenda == 2 && !String.IsNullOrEmpty(request.StatusVenda))
                    {
                        condicoes.Add($" A.STATUSVENDA = @STATUS_VENDA ");
                        parametros.Add("@STATUS_VENDA", request.StatusVenda);
                    }

                    if (!String.IsNullOrEmpty(request.Vendedor))
                    {
                        condicoes.Add($" A.ID_VENDEDOR = @ID_VENDEDOR ");
                        parametros.Add("@ID_VENDEDOR", Convert.ToInt64(request.Vendedor));
                    }

                    if (!String.IsNullOrEmpty(request.Supervisor))
                    {
                        condicoes.Add($" A.ID_SUPERVISOR = @ID_SUPERVISOR ");
                        parametros.Add("@ID_SUPERVISOR", Convert.ToInt64(request.Supervisor));
                    }

                    if (!String.IsNullOrEmpty(request.Departamento))
                    {
                        condicoes.Add($" A.ID_DEPARTAMENTO = @ID_DEPARTAMENTO ");
                        parametros.Add("@ID_DEPARTAMENTO", Convert.ToInt64(request.Departamento));
                    }

                    if (request.Motivo_JustificativaVenda != null && request?.Motivo_JustificativaVenda?.Count > 0)
                    {
                        var listaIds = request.Motivo_JustificativaVenda.Where(id => id != 0).ToList();
                        bool isIdOutro = request.Motivo_JustificativaVenda.Contains(0);

                        if (isIdOutro && listaIds.Count > 0)
                            condicoes.Add($" (A.ID_MOTIVO_DESISTENCIA_VENDA IN ({String.Join(", ", listaIds)}) OR A.ID_MOTIVO_DESISTENCIA_VENDA IS NULL AND A.JUSTIFICATIVA_AVALIACAO IS NOT NULL) ");
                        else if (isIdOutro)
                            condicoes.Add($" A.ID_MOTIVO_DESISTENCIA_VENDA IS NULL AND A.JUSTIFICATIVA_AVALIACAO IS NOT NULL ");
                        else
                            condicoes.Add($" A.ID_MOTIVO_DESISTENCIA_VENDA IN ({String.Join(", ", listaIds)}) ");
                    }

                    if (!String.IsNullOrEmpty(request.EtapaVenda))
                    {
                        condicoes.Add($" I.ETAPA = @ETAPA ");
                        parametros.Add("@ETAPA", Convert.ToInt64(request.EtapaVenda));
                    }

                    if (!String.IsNullOrEmpty(request.Campanha))
                    {
                        condicoes.Add($" A.ID_CAMPANHA = @ID_CAMPANHA ");
                        parametros.Add("@ID_CAMPANHA", Convert.ToInt64(request.Campanha));
                    }

                    switch (request.TipoScoreCredito)
                    {
                        case "entre":
                            condicoes.Add($" A.SCORE_CONSULTA_CREDITO BETWEEN @VALOR_INICIAL AND @VALOR_FINAL ");
                            parametros.Add("@VALOR_INICIAL", request.Valor_Inicial);
                            parametros.Add("@VALOR_FINAL", request.Valor_Final);
                            break;

                        case "maior":
                            condicoes.Add($" A.SCORE_CONSULTA_CREDITO > @VALOR_UNICO ");
                            parametros.Add("@VALOR_UNICO", request.Valor_Unico);
                            break;

                        case "menor":
                            condicoes.Add($" A.SCORE_CONSULTA_CREDITO < @VALOR_UNICO ");
                            parametros.Add("@VALOR_UNICO", request.Valor_Unico);
                            break;

                        case "maior_igual":
                            condicoes.Add($" A.SCORE_CONSULTA_CREDITO >= @VALOR_UNICO ");
                            parametros.Add("@VALOR_UNICO", request.Valor_Unico);
                            break;

                        case "menor_igual":
                            condicoes.Add($" A.SCORE_CONSULTA_CREDITO <= @VALOR_UNICO ");
                            parametros.Add("@VALOR_UNICO", request.Valor_Unico);
                            break;

                        case "igual":
                            condicoes.Add($" A.SCORE_CONSULTA_CREDITO = @VALOR_UNICO ");
                            parametros.Add("@VALOR_UNICO", request.Valor_Unico);
                            break;
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    #endregion

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
                        var lista = new List<RelatorioVendasReprovadasScoreViewModel>();
                        StringBuilder sql = new StringBuilder($@"SELECT A.ID, A.DATA_VENDA, A.DATAHORACRIACAO, B.ID ID_CLIENTE, COALESCE(B.NOMEFANTASIA, B.RAZAO_SOCIAL) CLIENTE,
                                                                  C.ID ID_VENDEDOR, C.NOME VENDEDOR, D.ID ID_SUPERVISOR, D.NOME SUPERVISOR, A.STATUS, A.SCORE_CONSULTA_CREDITO, I.ETAPA, 
                                                                  E.DESCRICAO, A.JUSTIFICATIVA_AVALIACAO, A.STATUS_CONSULTA_CREDITO, J.TITULO DESCRICAO_CAMPANHA ");
                        sql.AppendLine(sqlFrom.ToString());

                        #region [Ordenação]
                        if (request.order != null && request.order.Any())
                        {
                            int sortColumnIndex = request.order[0].column;
                            string coluna = request.columns[sortColumnIndex].data;
                            string sortDirection = request.order[0].dir;
                            switch (coluna)
                            {

                                case "id":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.ID");
                                        else
                                            sql.AppendLine(" ORDER BY A.ID DESC");
                                        break;
                                    }
                                case "descCliente":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.NOMEFANTASIA");
                                        else
                                            sql.AppendLine(" ORDER BY B.NOMEFANTASIA DESC");
                                        break;
                                    }
                                case "descVendedor":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY C.NOME");
                                        else
                                            sql.AppendLine(" ORDER BY C.NOME DESC");
                                        break;
                                    }
                                case "descSupervisor":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY D.NOME");
                                        else
                                            sql.AppendLine(" ORDER BY D.NOME DESC");
                                        break;
                                    }
                                case "dthCriacao":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.DATAHORACRIACAO");
                                        else
                                            sql.AppendLine(" ORDER BY A.DATAHORACRIACAO DESC");
                                        break;
                                    }
                                case "dthVenda":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.DATA_VENDA");
                                        else
                                            sql.AppendLine(" ORDER BY A.DATA_VENDA DESC");
                                        break;
                                    }
                                case "score":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.SCORE_CONSULTA_CREDITO");
                                        else
                                            sql.AppendLine(" ORDER BY A.SCORE_CONSULTA_CREDITO DESC");
                                        break;
                                    }
                                case "status":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.STATUS");
                                        else
                                            sql.AppendLine(" ORDER BY A.STATUS DESC");
                                        break;
                                    }
                                case "etapaVenda":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY I.ETAPA");
                                        else
                                            sql.AppendLine(" ORDER BY I.ETAPA DESC");
                                        break;
                                    }
                                case "statusConsultaCredito":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.STATUS_CONSULTA_CREDITO");
                                        else
                                            sql.AppendLine(" ORDER BY A.STATUS_CONSULTA_CREDITO DESC");
                                        break;
                                    }
                                case "descCampanha":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY J.TITULO");
                                        else
                                            sql.AppendLine(" ORDER BY J.TITULO DESC");
                                        break;
                                    }
                            }
                        }
                        #endregion

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new RelatorioVendasReprovadasScoreViewModel();

                                obj.Id = Convert.IsDBNull(reader["ID"]) ? 0 : Convert.ToInt64(reader["ID"]);
                                obj.DthVenda = Convert.IsDBNull(reader["DATA_VENDA"]) ? "-" : Convert.ToDateTime(reader["DATA_VENDA"]).ToString("dd/MM/yyyy");
                                obj.DthCriacao = Convert.IsDBNull(reader["DATAHORACRIACAO"]) ? String.Empty : Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy");
                                obj.IdCliente = Convert.IsDBNull(reader["ID_CLIENTE"]) ? (long?)null : Convert.ToInt64(reader["ID_CLIENTE"]);
                                obj.DescCliente = Convert.IsDBNull(reader["CLIENTE"]) ? String.Empty : Convert.ToString(reader["CLIENTE"]);
                                obj.IdVendedor = Convert.IsDBNull(reader["ID_VENDEDOR"]) ? (long?)null : Convert.ToInt64(reader["ID_VENDEDOR"]);
                                obj.DescVendedor = Convert.IsDBNull(reader["VENDEDOR"]) ? String.Empty : Convert.ToString(reader["VENDEDOR"]);
                                obj.IdSupervisor = Convert.IsDBNull(reader["ID_SUPERVISOR"]) ? (long?)null : Convert.ToInt64(reader["ID_SUPERVISOR"]);
                                obj.DescSupervisor = Convert.IsDBNull(reader["SUPERVISOR"]) ? String.Empty : Convert.ToString(reader["SUPERVISOR"]);
                                obj.Score = Convert.IsDBNull(reader["SCORE_CONSULTA_CREDITO"]) ? String.Empty : Convert.ToString(reader["SCORE_CONSULTA_CREDITO"]);
                                obj.Status = Convert.IsDBNull(reader["STATUS"]) ? String.Empty : HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(reader["STATUS"]));
                                obj.EtapaVenda = Convert.IsDBNull(reader["ETAPA"]) ? "-" : HtmlEnumExtensions.GetEnumDescription<EnumEtapaVenda>(Convert.ToString(reader["ETAPA"]));
                                obj.MotivoJustificativaVenda = Convert.IsDBNull(reader["DESCRICAO"]) ? Convert.IsDBNull(reader["JUSTIFICATIVA_AVALIACAO"]) ? "-" : Convert.ToString(reader["JUSTIFICATIVA_AVALIACAO"]) : Convert.ToString(reader["DESCRICAO"]);
                                obj.StatusConsultaCredito = Convert.IsDBNull(reader["STATUS_CONSULTA_CREDITO"]) ? String.Empty : HtmlEnumExtensions.GetEnumDescription<AprovacaoCriterioParametroCredito>(Convert.ToString(reader["STATUS_CONSULTA_CREDITO"]));
                                obj.DescCampanha = Convert.IsDBNull(reader["DESCRICAO_CAMPANHA"]) ? "-" : Convert.ToString(reader["DESCRICAO_CAMPANHA"]);
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

        #region [Painel de Acompanhamento de Alertas SOS]
        [HttpPost()]
        [Route("api/relatorioapi/PainelAcompanhamentoAlertaSOS")]
        public async Task<DataTableResponse<RelatorioSOSViewModel>> PainelAcompanhamentoAlertaSOS(RelatorioSOSRequestModel request)
        {
            try
            {
                List<RelatorioSOSViewModel> retorno = new List<RelatorioSOSViewModel>();
                int recordsTotal = 0;
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repositoryMongo = new RepositoryMongo<AtividadeMongo>(conexaoMongo))
                using (var repositoryUser = new Conexao<IdentityUser>(session))
                using (var repositoryEquipeStatus = new Conexao<EquipeStatusAniel>(sessionT5))
                {

                    var filtroBuilder = Builders<AtividadeMongo>.Filter;
                    var filtros = new List<FilterDefinition<AtividadeMongo>>();

                    if (!string.IsNullOrEmpty(request.DataHoraIni))
                    {
                        DateTime dataInicio = DateTime.Parse(request.DataHoraIni).ToUniversalTime();
                        DateTime dataFinal = DateTime.Parse(request.DataHoraFim).AddHours(23).AddMinutes(59).AddSeconds(59).ToUniversalTime();

                        filtros.Add(filtroBuilder.Gte(u => u.DataHoraEnvioNotificacao, dataInicio));
                        filtros.Add(filtroBuilder.Lte(u => u.DataHoraEnvioNotificacao, dataFinal));
                    }

                    if (!String.IsNullOrEmpty(request.Supervisor))
                    {
                        filtros.Add(filtroBuilder.Regex(u => u.EquipeSupervisor, new MongoDB.Bson.BsonRegularExpression(request.Supervisor)));
                    }

                    if (!String.IsNullOrEmpty(request.Monitor))
                    {
                        filtros.Add(filtroBuilder.Regex(u => u.EquipeMonitor, new MongoDB.Bson.BsonRegularExpression(request.Monitor)));
                    }

                    //if (!string.IsNullOrEmpty(request.DataHoraIni) && !string.IsNullOrEmpty(request.DataHoraFim))
                    //{
                    //    DateTime dataInicial = DateTime.Parse(request.DataHoraIni).ToUniversalTime();
                    //    DateTime dataFinal = DateTime.Parse(request.DataHoraFim).AddHours(23).AddMinutes(59).AddSeconds(59).ToUniversalTime();
                    //    filtros.Add(filtroBuilder.Gte(u => u.DataHoraCriacao, dataInicial));
                    //    filtros.Add(filtroBuilder.Lte(u => u.DataHoraCriacao, dataFinal));
                    //}

                    var filtroFinal = filtros.Count > 0 ? filtroBuilder.And(filtros) : filtroBuilder.Empty;
                    recordsTotal = (await repositoryMongo.FindAllWFilter(filtroFinal)).Count();

                    var queryRelatorio = (await repositoryMongo.FindPageAsync(filtroFinal, request.start, request.length));

                    retorno = queryRelatorio.OrderByDescending(p => p.DataHoraCriacao).ToList().Select(p => new RelatorioSOSViewModel
                    {
                        //Id = p._id,
                        Tecnico = !String.IsNullOrEmpty(p.Nome) ? p.Nome : "-",
                        Monitor = !String.IsNullOrEmpty(p.NomeMonitor) ? p.NomeMonitor : "-",
                        Supervisor = !String.IsNullOrEmpty(p.NomeSupervisor) ? p.NomeSupervisor : "-",
                        //confirmar com Renato se datahoracriacao ou envio notificacao
                        DataHoraEnvio = p.Data,
                        ConfirmadoCiencia = p.ConfirmadoCiencia ? RsBase.SIM : RsBase.NAO,
                        DataHoraConfirmacao = p.DataHoraConfirmacao.HasValue ? p.DataHoraConfirmacao.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") : "-",
                        Resposta = p.Resposta == EnumRespostaSOS.SOS ? "SOS" : p.Resposta == EnumRespostaSOS.Ok ? "Respondido" : "Não Respondido",
                        GerouNotificacao = p.DataHoraEnvioNotificacao.HasValue ? RsBase.SIM : RsBase.NAO,
                        DthGerouNotificacao = p.DataHoraEnvioNotificacao.HasValue ? p.DataHoraEnvioNotificacao.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") : "-",
                        UsuarioConfirmacao = !String.IsNullOrEmpty(p.UsuarioConfirmacao) ? p.UsuarioConfirmacao + " - " + repositoryUser.selectFirst(x => x.Id == p.UsuarioConfirmacao).Result.Nome : "-",
                        EquipeMonitor = p.EquipeMonitor ?? "-",
                        Latitude = Convert.ToString(repositoryEquipeStatus.selectFirst(x => x.Equipe.Matricula == p.Equipe)?.Result?.Latitude) ?? "",
                        Longitude = Convert.ToString(repositoryEquipeStatus.selectFirst(x => x.Equipe.Matricula == p.Equipe)?.Result?.Longitude) ?? "",
                    }).ToList();
                }

                //Page e retornando DataTable
                return new DataTableResponse<RelatorioSOSViewModel>
                {
                    draw = request.draw,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };
            }
            catch (Exception e)
            {
                CONSTANTES.log4Net.Error(e.Message, e);
                return new DataTableResponse<RelatorioSOSViewModel>
                {
                    draw = 0,
                    recordsFiltered = 0,
                    data = null,
                    error = $"{e.Message}"
                };
            }
        }
        #endregion

        [HttpPost()]
        [Route("api/relatorioapi/RelatorioCustoImpServico")]
        public async Task<DataTableResponse<RelatorioCustoImpServicoModel>> RelatorioCustoImpServico(RelatorioCusto_Imp_Servico request)
        {
            List<RelatorioCustoImpServicoModel> retorno = new List<RelatorioCustoImpServicoModel>();
            IOrderedEnumerable<RelatorioCustoImpServicoModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession sessiont5 = HibernateUtil.GetSessionT5())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                Dictionary<String, Object> parametrosContato = new Dictionary<string, object>();
                Dictionary<String, Object> parametrosPlanoProduto = new Dictionary<string, object>();

                //Select
                using (var com = sessiont5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.CODCT,A.PROJETO,A.NUM_OBRA AS OS,A.NUM_DOC,A.DATAHORA_AGENDAMENTO AS AGENDAMENTO,A.DATAHORA_FIM_ATEND AS ENCERRAMENTO, ");
                    sql.AppendLine(" E.DESCRICAO AS NATUREZA  ,A.CODCT AS CONTRATO ,B.NOME AS TECNICO,D.NOME AS SUPERVISOR ");
                    sql.AppendLine(" ,C.RAZAO_SOCIAL AS CLIENTE ,CASE WHEN  A.STATUS_EXECUCAO = 1 THEN 'Improdutivo' ELSE 'Produtivo' END AS STATUSENCERRAMENTO,A.STATUS_EXECUCAO  ,F.DESCRICAO AS  DESENCERRAMENTO, ");
                    sql.AppendLine(" (SELECT FIRST 1 CAST((EXTRACT(HOUR FROM M.HORAS) * 60 + EXTRACT(MINUTE FROM M.HORAS)) AS NUMERIC(8,2))/60 FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) * H.HXH AS CMAOOBRA,");
                    sql.AppendLine(" (A.QUANTIDADE*G.VALOR) AS CMATERIAL,COUNT(*) OVER() AS TOTAL_REG");

                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE  B ON B.NUM_FRE =A.EQUIPE  ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE C ON C.COD_CLIENTE =A.ID_CLIENTE  ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO D ON D.NUM_FRE = B.COD_SUPERV  ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE E ON E.COD_TIPO_SERV  =A.COD_TIPO_SERV  ");
                    sql.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO F ON F.COD_ENCERRAMENTO =A.COD_ENCERRAMENTO  ");
                    sql.AppendLine(" LEFT JOIN TB_MATERIAL G ON G.CODMAT =A.CODMAT   ");
                    sql.AppendLine(" LEFT JOIN TB_HXH_FUNCAO H ON H.COD_FUNCAO =D.COD_FUNCAO  ");
                    sql.AppendLine(" LEFT JOIN TB_PROJETO I ON I.PROJETO =A.PROJETO AND I.CODCT =A.CODCT   ");
                    sql.AppendLine(" LEFT JOIN TB_EMPRESA J ON J.CODEMPRESA =I.CODEMPRESA ");
                    sql.AppendLine(" LEFT JOIN TB_FILIAL L ON L.CODEMPRESA =I.CODEMPRESA  ");
                    sql.AppendLine(" LEFT JOIN TB_CENTRO_CUSTO M ON M.COD_CCUSTO =I.COD_CCUSTO ");


                    sql.AppendLine(" WHERE A.STATUS_EXECUCAO IS NOT NULL  ");


                    if (!String.IsNullOrEmpty(request.Data_Inicio) && !String.IsNullOrEmpty(request.Data_Fim))
                    {
                        if (!String.IsNullOrEmpty(request.Tipo_Filtro))
                        {
                            if (request.Tipo_Filtro == "AGENDAMENTO")
                            {
                                sql.AppendLine(" AND (A.DATAHORA_AGENDAMENTO  BETWEEN @DATA_INICIO AND @DATA_FIM) ");
                                parametros.Add("@DATA_INICIO", request.Data_Inicio);
                                parametros.Add("@DATA_FIM", request.Data_Fim);
                            }



                            else if (request.Tipo_Filtro == "EXECUCAO")
                            {
                                sql.AppendLine(" AND (A.DATA_ABERTURA BETWEEN @DATA_INICIO AND @DATA_FIM) ");
                                parametros.Add("@DATA_INICIO", request.Data_Inicio);
                                parametros.Add("@DATA_FIM", request.Data_Fim);
                            }



                            else if (request.Tipo_Filtro == "CRIACAO")
                            {
                                sql.AppendLine(" AND (A.DATAHORA_INI_ATEND  BETWEEN @DATA_INICIO AND @DATA_FIM) ");
                                parametros.Add("@DATA_INICIO", request.Data_Inicio);
                                parametros.Add("@DATA_FIM", request.Data_Fim);
                            }

                        }
                    }

                    if (!String.IsNullOrEmpty(request.Tipo_Prod))
                    {
                        if (request.Tipo_Prod == "IMPRODUTIVAS")
                        {
                            sql.AppendLine(" AND (A.STATUS_EXECUCAO = '1') ");
                        }
                        else if (request.Tipo_Prod == "PRODUTIVAS")
                        {
                            sql.AppendLine(" AND (A.STATUS_EXECUCAO = '2') ");
                        }
                        else if (request.Tipo_Prod == "Ambos")
                        {
                            sql.AppendLine(" AND (A.STATUS_EXECUCAO  = '1' OR A.STATUS_EXECUCAO  = '2') ");

                        }
                    }

                    if (request.Supervisor != null)
                    {
                        sql.AppendLine(" AND (D.NUM_FRE = @SUPERV) ");
                        parametros.Add("@SUPERV", request.Supervisor);
                    }

                    if (request.Tecnico != null)
                    {
                        sql.AppendLine(" AND (B.EQUIPE = @EQUIPE) ");
                        parametros.Add("@EQUIPE", request.Tecnico);
                    }

                    if (request.Contrato != null)
                    {
                        sql.AppendLine(" AND (A.CODCT = @CODCT) ");
                        parametros.Add("@CODCT", request.Contrato);
                    }

                    if (request.Projeto != null)
                    {
                        sql.AppendLine(" AND (A.PROJETO = @PROJETO) ");
                        parametros.Add("@PROJETO", request.Projeto);
                    }

                    if (request.Empresa.HasValue)
                    {
                        sql.AppendLine(" AND (J.CODEMPRESA = @CODEMPRESA) ");
                        parametros.Add("@CODEMPRESA", request.Empresa);
                    }
                    if (request.Filial.HasValue)
                    {
                        sql.AppendLine(" AND (L.CODFILIAL = @CODFILIAL) ");
                        parametros.Add("@CODFILIAL", request.Filial);
                    }
                    if (request.CentroCusto.HasValue)
                    {
                        sql.AppendLine(" AND (M.COD_CCUSTO = @COD_CCUSTO) ");
                        parametros.Add("@COD_CCUSTO", request.CentroCusto);
                    }
                    if (request.NaturezaServico.HasValue)
                    {
                        sql.AppendLine(" AND (A.NATUREZA = @NATUREZA) ");
                        parametros.Add("@NATUREZA", request.NaturezaServico);
                    }
                    if (request.Tipo_Servico.HasValue)
                    {
                        sql.AppendLine(" AND (A.COD_TIPO_SERV = @COD_TIPO_SERV) ");
                        parametros.Add("@COD_TIPO_SERV", request.Tipo_Servico);
                    }
                    switch (request.Agrupamento)
                    {
                        case "naturezaServico":
                            sql.AppendLine("  ORDER BY E.DESCRICAO,A.NUM_OBRA DESC ");
                            break;
                    }

                    //if (request.Agrupamento != null)
                    //{
                    //    sql.AppendLine("  ORDER BY E.DESCRICAO,A.NUM_OBRA DESC ");

                    //}

                    Acompanhamento_ServicoApiController apiServico = new Acompanhamento_ServicoApiController();
                    //Busca com paginacao
                    using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessiont5.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            String custoProducao = null;
                            String codCt = Convert.ToString(reader["CODCT"]);
                            String projeto = Convert.ToString(reader["PROJETO"]);
                            String OS = Convert.ToString(reader["OS"]);
                            Int32 num_Doc = Convert.ToInt32(reader["NUM_DOC"]);

                            var custoServicoResponse = await apiServico.GetCustoServico(codCt, projeto, OS, num_Doc);
                            custoProducao = custoServicoResponse.Objeto;


                            retorno.Add(new RelatorioCustoImpServicoModel
                            {
                                Nos = Convert.ToString(reader["OS"]),
                                DataAgendamento = Convert.ToString(reader["AGENDAMENTO"]),
                                Databaixa = Convert.ToString(reader["ENCERRAMENTO"]),
                                NaturezaServico = Convert.ToString(reader["NATUREZA"]),
                                Contrato = Convert.ToString(reader["CONTRATO"]),
                                Tecnico = Convert.ToString(reader["TECNICO"]),
                                Supervisor = Convert.ToString(reader["SUPERVISOR"]),
                                CategoriaCliente = Convert.ToString(reader["CLIENTE"]),
                                StatusEncerramento = Convert.ToString(reader["STATUSENCERRAMENTO"]),
                                DescricaoEncerramento = Convert.ToString(reader["DESENCERRAMENTO"]),
                                CustoMaoObra = Convert.ToString(reader["CMAOOBRA"]),
                                CustoMaterial = Convert.ToString(reader["CMATERIAL"]),
                                CustoProducao = custoProducao,
                            });
                            recordsTotal = Convert.ToInt32(reader["TOTAL_REG"]);
                        }
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<RelatorioCustoImpServicoModel>
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
