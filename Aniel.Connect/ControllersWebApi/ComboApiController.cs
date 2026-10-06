using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Repository.Alcans;
using Aniel.Connect.Repository.T5.AreaCoberturaMongo;
using DocumentFormat.OpenXml.Spreadsheet;
using GeoAPI.Geometries;
using Microsoft.Extensions.Logging.Abstractions;
using NetTopologySuite.Geometries;
using NHibernate;
using NHibernate.Hql.Ast;
using NHibernate.Hql.Ast;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ComboApiController : ApiController
    {
        [HttpPost]
        [Route("api/ComboApi/GetAll")]
        public async Task<DataTableResponse<ComboListModel>> GetAll(DataTableRequest request)
        {
            var combos = new List<ComboListModel>();
            Int32 recordsTotal = 0;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    var sql = new StringBuilder();
                    var param = new Dictionary<string, object>();
                    StringBuilder sqlWhere = new StringBuilder();
                    var parametros = new Dictionary<string, object>();
                    var filtros = new List<string>();

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        filtros.Add($"UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.search.value.ToUpperInvariant(),255)}%'");
                    }

                    if (filtros.Count > 0)
                    {
                        sql.AppendLine($"WHERE {String.Join(" AND ", filtros)}");
                        sqlWhere.AppendLine(sql.ToString());
                    }

                    //Count
                    using (var con = session.Connection.CreateCommand())
                    {
                        var sqlFrom = new StringBuilder();

                        sqlFrom.AppendLine("SELECT COUNT(A.ID) AS CO FROM TB_COMBO A");
                        sqlFrom.AppendLine(sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(con, sqlFrom, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.STATUS");
                        sql.AppendLine("FROM TB_COMBO A");
                        sql.AppendLine(sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, param, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                combos.Add(new ComboListModel
                                {
                                    Codigo = Convert.ToString(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Status =
                                        HtmlEnumExtensions.GetEnumDescription(
                                            (Status_Default)Convert.ToInt16(reader["STATUS"])
                                        )
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Page e retornando DataTable
                return new DataTableResponse<ComboListModel>
                {
                    draw = 0,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = new List<ComboListModel>(),
                    error = ex.Message
                };
            }


            return new DataTableResponse<ComboListModel>
            {
                recordsFiltered = recordsTotal,
                data = combos,
                draw = request.draw,
                error = ""
            };
        }

        [HttpPost]
        [Route("api/ComboApi/AlterarStatus")]
        public async Task<PostApiRetorno> AlterarStatus(AlterarStatusViewModel request)
        {
            var retorno = new PostApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder($"UPDATE TB_COMBO SET STATUS = {request.status} WHERE ID IN ({String.Join(",", request.ids)})");

                    await CommandHelper.ExecuteNonQuery(com, sql, session.Connection, null);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;

                retorno.Mensagem = ex.Message;
                retorno.InnerException =
                    ex.InnerException != null ?
                        ex.InnerException.Message :
                        String.Empty;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/ComboApi/RemoverPlano")]
        public async Task<PostApiRetorno> RemoverPlano(RemoverPlanoComboRequest request)
        {
            var retorno = new PostApiRetorno();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();
                    var param = new Dictionary<string, object>();

                    sql.AppendLine("DELETE FROM TB_COMBO_PLANO_PRODUTO WHERE ID = @ID_COMBO_PLANO_PRODUTO");
                    param.Add("@ID_COMBO_PLANO_PRODUTO", request.IdComboPlanoProduto);

                    await CommandHelper.ExecuteNonQuery(com, sql, param, session.Connection, null);

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        /// <summary>
        ///     Retorna uma lista de combos para ser usada pelo select2
        /// </summary>
        /// <param name="request">
        ///     Dados da requisição com ID do combo e ID pai, sendo o ID pai um filtro do
        ///     tipo de tecnologia incluso nos planos/produtos do combo
        /// </param>
        [HttpPost()]
        [Route("api/ComboApi/GetCombo")]
        public async Task<ObjCombo<long>> GetCombo(RequestObjComboPai<long?, string> request)
        {
            var retorno = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                var param = new Dictionary<string, object>();
                var condicoes = new List<string>();

                var sqlFrom = new StringBuilder();

                sqlFrom.AppendLine("FROM TB_COMBO C");

                sqlFrom.AppendLine("INNER JOIN TB_COMBO_PLANO_PRODUTO CPP");
                sqlFrom.AppendLine("ON (CPP.ID_COMBO = C.ID)");

                sqlFrom.AppendLine("LEFT JOIN TB_MODELO_CONTRATO CT");
                sqlFrom.AppendLine("ON (CT.ID = C.ID_MODELO_CONTRATO)");
                sqlFrom.AppendLine("LEFT JOIN TB_PLANO PL");
                sqlFrom.AppendLine("ON CPP.ID_PLANO = PL.ID");

                #region Filtros

                if (!String.IsNullOrEmpty(request.idpai))
                {
                    string[] tipos = request.idpai.Split(',');
                    var condicao = new List<string>();

                    condicoes.Add($@"EXISTS (
                                        SELECT 1
                                        FROM TB_COMBO_PLANO_PRODUTO CPP2
                                        INNER JOIN TB_PLANO PL2 ON PL2.ID = CPP2.ID_PLANO
                                        WHERE CPP2.ID_COMBO = C.ID
                                          AND PL2.TIPO_TECNOLOGIA IN ({String.Join(",", tipos)})
                                    )
                                ");
                }

                if (request.id.HasValue)
                {
                    condicoes.Add("C.ID = @ID_COMBO");
                    param.Add("@ID_COMBO", request.id);
                }

                if (!String.IsNullOrEmpty(request.q))
                {
                    condicoes.Add($"UPPER (C.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                }

                // Listando somente combos ativos
                condicoes.Add("C.STATUS = 1");

                if (condicoes.Count > 0)
                {
                    sqlFrom.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");
                }

                #endregion

                #region Count

                var sqlCount = new StringBuilder();

                sqlCount.AppendLine("SELECT COUNT(C.ID) AS CO, C.DESCRICAO");
                sqlCount.AppendLine(sqlFrom.ToString());
                sqlCount.AppendLine("GROUP BY C.ID, C.DESCRICAO");
                sqlCount.AppendLine("ORDER BY C.ID");

                using (var readerCount = await CommandHelper.ExecuteReader(com, sqlCount, param, session.Connection, null))
                {
                    while (await readerCount.ReadAsync())
                    {
                        retorno.total_count++;
                    }
                }

                #endregion

                #region Select

                var sql = new StringBuilder();

                sql.AppendLine("SELECT C.ID, C.DESCRICAO, C.ID_MODELO_CONTRATO,");
                sql.AppendLine("       SUM(CPP.VALOR) AS VALOR,");
                sql.AppendLine("       SUM(COALESCE(CPP.VALOR_INSTALACAO, 0)) AS VALOR_INSTALACAO, ");
                sql.AppendLine("       CT.TESTEGRATIS, CT.DIASTESTE, CT.TEM_CARENCIA, CT.PERIODO_CARENCIA, ");
                sql.AppendLine("      (SELECT FIRST 1 'S' from tb_combo_plano_produto ppp left join tb_plano plannn on plannn.id = ppp.id_plano WHERE plannn.tipo_tecnologia = 2 and ppp.id_combo = c.id) as USA_TEC_INTERNET ");
                sql.AppendLine(sqlFrom.ToString());
                sql.AppendLine("GROUP BY C.ID, C.DESCRICAO, C.ID_MODELO_CONTRATO, CT.TESTEGRATIS, CT.DIASTESTE,");
                sql.AppendLine("         CT.TEM_CARENCIA, CT.PERIODO_CARENCIA ");
                sql.AppendLine("ORDER BY C.ID");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var obj = new Modelo_PlanoObjCombo
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Text = Convert.ToString(reader["DESCRICAO"]),

                            Id_Modelo_Contrato =
                                reader["ID_MODELO_CONTRATO"] != DBNull.Value ?
                                    Convert.ToInt64(reader["ID_MODELO_CONTRATO"]) :
                                    (long?)null,
                            Valor =
                                reader["VALOR"] != DBNull.Value ?
                                    Convert.ToDouble(reader["VALOR"]).ToString("N2") :
                                    0.ToString("N2"),
                            ValorInstalacao =
                                Convert.ToDouble(reader["VALOR_INSTALACAO"]) > 0 ?
                                    Convert.ToDouble(reader["VALOR_INSTALACAO"]).ToString("N2") :
                                    null,
                            CheckVigenciaContrato =
                                reader["TESTEGRATIS"] != DBNull.Value ?
                                    Convert.ToBoolean(reader["TESTEGRATIS"]) :
                                    false,
                            Check_DiasTesteGratis =
                                reader["TEM_CARENCIA"] != DBNull.Value ?
                                    Convert.ToBoolean(reader["TEM_CARENCIA"]) :
                                    false,
                            PeriodoFidelidade = Convert.ToString(reader["PERIODO_CARENCIA"]),
                            QtdeDiasGratis = Convert.ToString(reader["DIASTESTE"]),
                            UsaTecnologiaInternet = Convert.ToString(reader["USA_TEC_INTERNET"]),
                        };

                        retorno.results.Add(obj);
                    }
                }

                #endregion
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/ComboApi/GetComboAreaCobertura")]
        public async Task<ObjCombo<long>> GetComboAreaCobertura(RequestObjComboAvo<Int64?, double, double> request)
        {

            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                var idArea = GetIdAreaCobertura(request.idavo, request.idpai).Objeto;
                var ids = new List<long>();
                var AreaCoberturaCombo = new List<AreaCoberturaCombo>();

                try
                {
                    //Filtros 
                    var parametros = new Dictionary<string, object>();

                    var sqlFromCombo = new StringBuilder("FROM TB_AREA_COBERTURA_COMBO A LEFT JOIN  TB_AREA_COBERTURA B ON B.ID = A.ID_AREA_COBERTURA ");
                    var combo = new List<AreaCoberturaCombo>();

                    var condicoesCombo = new List<String>();

                    // Caso a lista de IDs de Área de cobertura venha vazia, não há combos nem produtos para exibir.
                    if ((idArea != null))
                    {
                        string listaIds = string.Join(", ", idArea.Where(p => !string.IsNullOrEmpty(Convert.ToString(p))).Select(p => string.Format("{0}", p)));
                        //throw new Exception("Não existem planos referente a essa geolocalização");

                        if (listaIds.Count() > 0)
                            condicoesCombo.Add($"A.ID_AREA_COBERTURA IN ({listaIds})");
                        else
                            throw new Exception("NENHUMA AREA COBERTURA ENCONTRADA");

                        condicoesCombo.Add($"B.SITUACAO = 1");
                        sqlFromCombo.AppendLine($" WHERE ( {String.Join(" AND ", condicoesCombo)} ) ");


                        //Verificar o id do plano pela area de cobertura
                        using (var comT5 = sessionT5.Connection.CreateCommand())
                        {
                            var sql = new StringBuilder("SELECT A.ID, A.ID_AREA_COBERTURA, A.DESCRICAO,COD_COMBO ");
                            sql.AppendLine(sqlFromCombo.ToString());
                            sql.AppendLine("ORDER BY UPPER(A.DESCRICAO) ");

                            using (var reader = await CommandHelper.ExecuteReader(comT5, sql, parametros, sessionT5.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    long id = 0;
                                    if (reader["COD_COMBO"] != DBNull.Value)
                                    {
                                        id = Convert.ToInt64(reader["COD_COMBO"]);
                                    }
                                    if (!ids.Contains(id)) // Evita duplicados
                                    {
                                        ids.Add(id);
                                    }

                                    var obj = new AreaCoberturaCombo
                                    {
                                        Id = id,
                                        Text = Convert.ToString(reader["DESCRICAO"]),
                                        Id_area_cobertura = Convert.ToString(reader["ID_AREA_COBERTURA"]),
                                    };

                                    AreaCoberturaCombo.Add(obj);

                                }
                            }
                        }

                        //Verificar total de paginas
                        using (var comT5 = sessionT5.Connection.CreateCommand())
                        {
                            var sqlFrom = new StringBuilder();
                            var param = new Dictionary<string, object>();
                            var condicoes = new List<string>();

                            sqlFrom.AppendLine("FROM TB_COMBO C");

                            sqlFrom.AppendLine("INNER JOIN TB_COMBO_PLANO_PRODUTO CPP");
                            sqlFrom.AppendLine("ON (CPP.ID_COMBO = C.ID)");

                            sqlFrom.AppendLine("LEFT JOIN TB_MODELO_CONTRATO CT");
                            sqlFrom.AppendLine("ON (CT.ID = C.ID_MODELO_CONTRATO)");
                            sqlFrom.AppendLine("LEFT JOIN TB_PLANO PL");
                            sqlFrom.AppendLine("ON CPP.ID_PLANO = PL.ID");

                            #region Select 
                            var condicao = new List<string>();

                            string listaIdsCombo = string.Join(", ", ids);
                            condicoes.Add($"C.ID IN ({listaIdsCombo})");

                            // Listando somente combos ativos
                            condicoes.Add("C.STATUS = 1");

                            if (request != null && !String.IsNullOrEmpty(request.q))
                            {
                                if (!String.IsNullOrEmpty(request.q))
                                {
                                    condicoes.Add($"UPPER (C.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                                }
                            }


                            if (condicoes.Count > 0)
                            {
                                sqlFrom.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");
                            }

                            var sqlCount = new StringBuilder("SELECT COUNT(C.ID) AS CO ");
                            sqlCount.AppendLine(sqlFrom.ToString());


                            using (var readerCount = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                            {
                                while (await readerCount.ReadAsync())
                                {
                                    retorno.total_count = Convert.ToInt32(readerCount["CO"]);
                                }
                            }

                            if (ids.Count > 0)
                            {
                                using (var com_union = session.Connection.CreateCommand())
                                {


                                    StringBuilder sql = new StringBuilder();
                                    sql.AppendLine("SELECT C.ID, C.DESCRICAO, C.ID_MODELO_CONTRATO,");
                                    sql.AppendLine("       SUM(CPP.VALOR) AS VALOR,");
                                    sql.AppendLine("       SUM(COALESCE(CPP.VALOR_INSTALACAO, 0)) AS VALOR_INSTALACAO, ");
                                    sql.AppendLine("       CT.TESTEGRATIS, CT.DIASTESTE, CT.TEM_CARENCIA, CT.PERIODO_CARENCIA, ");
                                    sql.AppendLine("      (SELECT FIRST 1 'S' from tb_combo_plano_produto ppp left join tb_plano plannn on plannn.id = ppp.id_plano WHERE plannn.tipo_tecnologia = 2 and ppp.id_combo = c.id) as USA_TEC_INTERNET ");
                                    sql.AppendLine(sqlFrom.ToString());
                                    sql.AppendLine("GROUP BY C.ID, C.DESCRICAO, C.ID_MODELO_CONTRATO, CT.TESTEGRATIS, CT.DIASTESTE,");
                                    sql.AppendLine("         CT.TEM_CARENCIA, CT.PERIODO_CARENCIA ");
                                    sql.AppendLine("ORDER BY C.ID");




                                    #endregion
                                    using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, param, session.Connection, null, request.page))
                                    {
                                        while (await reader.ReadAsync())
                                        {
                                            var obj = new Modelo_PlanoObjCombo
                                            {
                                                Id = Convert.ToInt64(reader["ID"]),
                                                Text = Convert.ToString(reader["DESCRICAO"]),

                                                Id_Modelo_Contrato =
                                                    reader["ID_MODELO_CONTRATO"] != DBNull.Value ?
                                                        Convert.ToInt64(reader["ID_MODELO_CONTRATO"]) :
                                                        (long?)null,
                                                Valor =
                                                    reader["VALOR"] != DBNull.Value ?
                                                        Convert.ToDouble(reader["VALOR"]).ToString("N2") :
                                                        0.ToString("N2"),
                                                ValorInstalacao =
                                                    Convert.ToDouble(reader["VALOR_INSTALACAO"]) > 0 ?
                                                        Convert.ToDouble(reader["VALOR_INSTALACAO"]).ToString("N2") :
                                                        null,
                                                CheckVigenciaContrato =
                                                    reader["TESTEGRATIS"] != DBNull.Value ?
                                                        Convert.ToBoolean(reader["TESTEGRATIS"]) :
                                                        false,
                                                Check_DiasTesteGratis =
                                                    reader["TEM_CARENCIA"] != DBNull.Value ?
                                                        Convert.ToBoolean(reader["TEM_CARENCIA"]) :
                                                        false,
                                                PeriodoFidelidade = Convert.ToString(reader["PERIODO_CARENCIA"]),
                                                QtdeDiasGratis = Convert.ToString(reader["DIASTESTE"]),
                                                UsaTecnologiaInternet = Convert.ToString(reader["USA_TEC_INTERNET"]),
                                            };

                                            retorno.results.Add(obj);
                                        }
                                    }
                                }
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    string Message = ex.Message;
                }
            }

            return retorno;
        }

        private GetApiRetorno<List<Int64>> GetIdAreaCobertura(double Latitude, double Longitude)
        {

            var retorno = new GetApiRetorno<List<Int64>>
            {
                Objeto = new List<Int64>()
            };

            try

            {
                using (var pesquisaAreas = new GeographyPattern())
                using (var sessionMongo = new ConexaoMongoGeography())
                {
                    var location = new Core.Models.Location(Latitude, Longitude);

                    IAreaCoberturaMongoRepository repoAreaCobertura = new AreaCoberturaMongoRepository(sessionMongo);
                    var areas = repoAreaCobertura.GetAll().ToList();

                    GeometryFactory factory = new GeometryFactory();

                    Dictionary<long, Polygon> areasCoberturaDesenhado = new Dictionary<long, Polygon>();

                    Coordinate pointCoordinate = new Coordinate(Latitude, Longitude);

                    Point point = (Point)factory.CreatePoint(pointCoordinate);

                    foreach (var area in areas)

                    {

                        var coordenadas = new List<Coordinate>();

                        if (area != null && area.Coordenadas != null)

                        {

                            for (int i = 0; i < area.Coordenadas.Count; i++)

                            {

                                List<double?> coordenada = area.Coordenadas[i];

                                for (var latlong = 0; latlong < coordenada.Count(); latlong++)

                                {

                                    double locLat = coordenada[latlong].HasValue ? Convert.ToDouble(string.Format("{0:F7}", coordenada[latlong].Value)) : 0,

                                            locLong = coordenada[latlong].HasValue ? Convert.ToDouble(string.Format("{0:F7}", coordenada[++latlong].Value)) : 0;

                                    var recordCoordenada = new Coordinate(locLat, locLong);

                                    if (!coordenadas.Contains(recordCoordenada))

                                    {

                                        coordenadas.Add(recordCoordenada);

                                    }

                                }

                            }

                            if (coordenadas.Count > 0 && coordenadas.LastOrDefault() != coordenadas.FirstOrDefault())

                            {

                                coordenadas.Add(coordenadas.First());

                            }

                        }

                        LinearRing ring = new LinearRing(coordenadas.ToArray());

                        Polygon polygon = new Polygon(ring);

                        areasCoberturaDesenhado.Add(area.Id, polygon);

                    }

                    retorno.Objeto.AddRange(areasCoberturaDesenhado.Where(p => p.Value.Contains(point)).Select(p => p.Key));


                    retorno.Ok = true;

                }

            }

            catch (Exception ex)

            {

                retorno.Ok = false;

                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)

                    retorno.InnerException = ex.InnerException.Message;

            }

            return retorno;

        }

        [HttpGet()]
        [Route("api/ComboApi/ConsultaPlanoEmCombo")]
        public async Task<GetApiRetorno<List<PlanoVinculadoC_ComboModel>>> ConsultaPlanoEmCombo(Int64 Id)
        {
            var retorno = new GetApiRetorno<List<PlanoVinculadoC_ComboModel>>()
            {
                Objeto = new List<PlanoVinculadoC_ComboModel>()
            };
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    var param = new Dictionary<string, object>();
                    var condicoes = new List<string>();

                    var sqlFrom = new StringBuilder();
                    sqlFrom.AppendLine("FROM TB_COMBO A");
                    sqlFrom.AppendLine("INNER JOIN TB_COMBO_PLANO_PRODUTO B ON A.ID = B.ID_COMBO");

                    #region Filtros

                    if (Id > 0)
                    {
                        condicoes.Add("B.ID_PLANO = @ID_PLANO");
                        param.Add("@ID_PLANO", Id);
                    }

                    // Listando somente combos ativos
                    condicoes.Add("A.STATUS = 1");

                    if (condicoes.Count > 0)
                    {
                        sqlFrom.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");
                    }

                    #endregion

                    #region Select

                    var sql = new StringBuilder();

                    sql.AppendLine("A.ID, A.DESCRICAO, A.STATUS, A.ID_MODELO_CONTRATO, A.EMITE_CONTRATO,");
                    sql.AppendLine("B.VALOR, B.COBRAR_INSTALACAO, B.VALOR_INSTALACAO, B.ID_PLANO");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new PlanoVinculadoC_ComboModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = reader["STATUS"] != DBNull.Value ? Convert.ToBoolean(reader["STATUS"]) : false,
                                Id_Modelo_Contrato = reader["ID_MODELO_CONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["ID_MODELO_CONTRATO"]) : (long?)null,
                                Emite_Contrato = reader["EMITE_CONTRATO"] != DBNull.Value ? Convert.ToBoolean(reader["EMITE_CONTRATO"]) : false,
                                Valor = reader["VALOR"] != DBNull.Value ? Convert.ToDouble(reader["VALOR"]) : (Double?)null,
                                Valor_Instalacao = reader["VALOR_INSTALACAO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_INSTALACAO"]) : (Double?)null,
                                Cobrar_Instalacao = reader["COBRAR_INSTALACAO"] != DBNull.Value ? Convert.ToBoolean(reader["COBRAR_INSTALACAO"]) : false,
                                Id_Plano = reader["ID_PLANO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PLANO"]) : (Int64?)null
                            };

                            retorno.Objeto.Add(obj);
                        }
                    }
                    #endregion

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/ComboApi/GetListDetalhes")]
        public async Task<GetApiRetorno<ComboDetalheModel>> GetListDetalhes(long idCombo)
        {
            var retorno = new GetApiRetorno<ComboDetalheModel>
            {
                Objeto = new ComboDetalheModel()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repoCombo = new Conexao<Combo>(session))
            using (var repoComboPlanoProduto = new Conexao<ComboPlanoProduto>(session))
            {
                try
                {
                    Combo combo = await repoCombo.selectFirst(p => p.Id == idCombo);
                    IEnumerable<ComboPlanoProduto> listPlanoProdutoCombo = await repoComboPlanoProduto.select(p => p.Combo.Id == combo.Id);

                    IEnumerable<Plano> listPlanos =
                        listPlanoProdutoCombo
                            .Where(p => p.Plano != null)
                            .Select(p => p.Plano);
                    IEnumerable<Produto> listProdutos =
                        listPlanoProdutoCombo
                            .Where(p => p.Produto != null)
                            .Select(p => p.Produto);

                    if (listPlanoProdutoCombo.Count() > 0)
                    {
                        var detalhesCombo = new ComboDetalheModel
                        {
                            Valor_Combo =
                                listPlanoProdutoCombo
                                    .Sum(u => u.Valor)
                                    .ToString("N2"),
                            Valor_Instalacao =
                                listPlanoProdutoCombo
                                    .Sum(u => u.ValorInstalacao.GetValueOrDefault(0))
                                    .ToString("N2"),
                            Multa_Carencia_Reais =
                                (combo.ModeloContrato?.Multa_Carencia).HasValue ?
                                    Convert.ToString(combo.ModeloContrato?.Multa_Carencia) :
                                    Convert.ToString(0),
                            Cancelar_Contrato_Automatico_Fim_Carencia =
                                (combo.ModeloContrato?.Cancela_Contrato_Automatico_Final_Carencia).GetValueOrDefault(false) ?
                                    RsBase.SIM :
                                    RsBase.NAO,
                            Tem_Carencia =
                                (combo.ModeloContrato?.Tem_Carencia).GetValueOrDefault(false) ?
                                    RsBase.SIM :
                                    RsBase.NAO,
                            Periodo_Carencia = combo.ModeloContrato?.Periodo_Carencia,
                            Numero_Parcelas = 1,
                            ListaModelos = new List<ComboDetalhePlanoProdutoModel>()
                        };

                        bool comboPossuiPlanos = listPlanos.Count() > 0;
                        bool comboPossuiProdutos = listProdutos.Count() > 0;

                        if (comboPossuiPlanos)
                        {
                            detalhesCombo.ListaModelos =
                                listPlanos.Select(p => new ComboDetalhePlanoProdutoModel
                                {
                                    Descricao = p.Descricao,
                                    Detalhe = p.DetalhesPlano,
                                    Formato_Cobranca = HtmlEnumExtensions.GetEnumDescription(p.FormatoCobranca),
                                    Velocidade = p.VelocidadeDownload,
                                    Ponto = String.Empty,
                                    PlanoProduto = HtmlEnumExtensions.GetEnumDescription(PlanoProdutoEnum.Plano),
                                    Frequencia_Cobranca = HtmlEnumExtensions.GetEnumDescription(p.FrequenciaCobranca),
                                    Franquia =
                                        p.PossuiFranquia ?
                                            p.Franquia.Minutos.ToString() :
                                            Convert.ToString(0)
                                });
                        }

                        if (comboPossuiProdutos)
                        {
                            detalhesCombo.ListaModelos =
                                listProdutos.Select(p => new ComboDetalhePlanoProdutoModel
                                {
                                    Descricao = p.Descricao,
                                    Detalhe = p.Detalhes,
                                    PlanoProduto = HtmlEnumExtensions.GetEnumDescription(PlanoProdutoEnum.Produto),
                                    Frequencia_Cobranca = HtmlEnumExtensions.GetEnumDescription(p.FrequenciaCobrancaProduto)
                                });
                        }

                        retorno.Objeto = detalhesCombo;
                    }
                    else
                    {
                        retorno.Objeto = new ComboDetalheModel();
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;

                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [HttpGet()]
        [AllowAnonymous]
        [Route("api/ComboApi/ComboDisponivel/{idCombo}/{cpf}")]
        public async Task<bool> ComboDisponivel(long idCombo, string cpf)
        {
            var retorno = true;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoCombo = new Conexao<Combo>(session))
            using (var repoIntegracoes = new Conexao<IntegracoesExternas>(session))
            {
                var combo = await repoCombo.selectFirst(x => x.Id == idCombo);

                var integracaoHub = await repoIntegracoes.selectFirst(x => x.IntegracaoExterna == IntegracaoExterna.MK_SOLUTION && x.TipoIntegracao == EnumTipoIntegracao.EXCLUSIVIDADE_PLANO);

                var alcans = new AlcansRepository(integracaoHub.Usuario, integracaoHub.Senha, integracaoHub.UrlApi);

                var responseAlcans = await alcans.ContratosDisponiveisAllhub(new ContratosDisponiveisRequest
                {
                    accessPlans = combo.PlanosProdutos.Select(x => Convert.ToInt32(x.Plano.CodigoIntegracao)).ToList(),
                    document = cpf
                });

                if (responseAlcans != null && responseAlcans.Any())
                {
                    foreach (var plano in combo.PlanosProdutos)
                    {
                        var planoResponseAlcans = responseAlcans.FirstOrDefault(x => x.accessPlan.ToString() == plano.Plano.CodigoIntegracao);

                        if (!planoResponseAlcans.permit && plano.Plano.ExclusivoCliente)
                            return false;
                    }
                }
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/ComboApi/FindById")]
        public async Task<GetApiRetorno<ComboCadastroModel>> FindById(long id)
        {
            var retorno = new GetApiRetorno<ComboCadastroModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCombo = new Conexao<Combo>(session))
                {
                    Combo combo = await repoCombo.selectFirst(p => p.Id == id);

                    retorno.Objeto = GetViewObject(combo);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/ComboApi/Gravar")]
        public async Task<PostApiRetorno<long>> Gravar([FromBody] ParamWebApi<ComboCadastroModel> request)
        {
            var retorno = new PostApiRetorno<long>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCombo = new Conexao<Combo>(session))
                {
                    Combo modelo;

                    if (request.Objeto.Id != 0)
                    {
                        modelo = await repoCombo.selectFirst(p => p.Id == request.Objeto.Id);

                        modelo.Status = request.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;

                        modelo.UsuarioAlteracao = request.Usuario;
                        modelo.DataHoraAlteracao = DateTime.Now;
                    }
                    else
                    {
                        modelo = new Combo
                        {
                            UsuarioCriacao = request.Usuario,
                            DataHoraCriacao = DateTime.Now,
                            Status = Status_Default.Ativo
                        };
                    }

                    modelo = await BindViewToObj(modelo, request.Objeto);

                    await repoCombo.SaveOrUpdate(modelo);

                    retorno.Ok = true;
                    retorno.IdObjeto = modelo.Id;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;

                retorno.IdObjeto = request.Objeto.Id;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private ComboCadastroModel GetViewObject(Combo objBanco)
        {
            return new ComboCadastroModel
            {
                Id = objBanco.Id,
                Descricao = objBanco.Descricao,
                DetalhesCombo = objBanco.DetalhesCombo,
                Status = objBanco.Status == Status_Default.Ativo,
                EmitirContrato = objBanco.EmiteContrato,
                IdModeloContrato = objBanco.ModeloContrato?.Id,
                CodigoAuxiliar = objBanco.CodigoIntegracao,
                CodigoIntegracaoAuxiliar = objBanco.CodigoIntegracaoAuxiliar,
                CodigoTipoOS = objBanco.CodigoTipoComboIntegracao,
                ListPlanoProduto =
                    objBanco.PlanosProdutos.Select(p => new ComboPlanoProdutoCadastroModel
                    {
                        Id = p.Id,

                        IdPlano = p.Plano?.Id,
                        IdProduto = p.Produto?.Id,
                        DescricaoPlanoProduto = p.Plano?.Descricao ?? p.Produto?.Descricao,
                        Tipo =
                            p.Plano != null ?
                                (HtmlEnumExtensions.GetEnumDescription(p.Plano.Tipo_Tecnologia) ?? String.Empty) :
                                (HtmlEnumExtensions.GetEnumDescription(p.Produto.Tipo_Produto) ?? String.Empty),

                        ValorReal = String.Format("R$ {0}", p.Plano?.ValorPlano.ToString("N2") ?? p.Produto?.ValorProduto?.ToString("N2")),
                        ValorCombo = p.Valor.ToString("N2"),

                        CobraInstalacao = p.CobrarInstalacao,

                        ValorRealInstalacao = String.Format("R$ {0}", p.Plano?.ValorInstalacao.ToString("N2") ?? p.Produto?.ValorInstalacao?.ToString("N2")),
                        ValorInstalacaoCombo = p.ValorInstalacao?.ToString("N2")
                    }).ToList()
            };
        }

        private async Task<Combo> BindViewToObj(Combo objBanco, ComboCadastroModel view)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoComboPlanoProduto = new Conexao<ComboPlanoProduto>(session))
            using (var repoModeloContrato = new Conexao<Modelo_Contrato>(session))
            using (var repoPlano = new Conexao<Plano>(session))
            using (var repoProduto = new Conexao<Produto>(session))
            {
                objBanco.Descricao = view.Descricao;
                objBanco.DetalhesCombo = view.DetalhesCombo;
                objBanco.EmiteContrato = view.EmitirContrato;
                objBanco.CodigoTipoComboIntegracao = view.CodigoTipoOS;
                objBanco.CodigoIntegracao = view.CodigoAuxiliar;
                objBanco.CodigoIntegracaoAuxiliar = view.CodigoIntegracaoAuxiliar;

                if (objBanco.EmiteContrato && view.IdModeloContrato.HasValue)
                {
                    objBanco.ModeloContrato = await repoModeloContrato.selectFirst(p => p.Id == view.IdModeloContrato);
                }
                else
                {
                    objBanco.ModeloContrato = null;
                }

                foreach (ComboPlanoProdutoCadastroModel planoProduto in view.ListPlanoProduto)
                {
                    if (planoProduto.Id == 0)
                    {
                        var comboPlanoProduto = new ComboPlanoProduto
                        {
                            DataHoraCriacao = objBanco.DataHoraCriacao,
                            UsuarioCriacao = objBanco.UsuarioCriacao,

                            Combo = objBanco
                        };

                        if (planoProduto.IdPlano.HasValue)
                        {
                            comboPlanoProduto.Plano = await repoPlano.selectFirst(p => p.Id == planoProduto.IdPlano);
                        }
                        else if (planoProduto.IdProduto.HasValue)
                        {
                            comboPlanoProduto.Produto = await repoProduto.selectFirst(p => p.Id == planoProduto.IdProduto);
                        }

                        comboPlanoProduto.Valor =
                            !String.IsNullOrEmpty(planoProduto.ValorCombo) ?
                                Convert.ToDouble(planoProduto.ValorCombo) :
                                0;
                        comboPlanoProduto.ValorInstalacao =
                            !String.IsNullOrEmpty(planoProduto.ValorInstalacaoCombo) ?
                                Convert.ToDouble(planoProduto.ValorInstalacaoCombo) :
                                (double?)null;

                        objBanco.PlanosProdutos.Add(comboPlanoProduto);
                    }
                }
            }

            return objBanco;
        }
    }
}
