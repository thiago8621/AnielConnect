using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Socket.ChatHub;
using Newtonsoft.Json;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Modelo_ContratoApiController : BaseApiController
    {


        [HttpPost()]
        [Route("api/Modelo_ContratoApi/GetAll")]
        public async Task<DataTableResponse<Modelo_ContratoListaModel>> GetAll(Modelo_Contrato_DataTableRequest request)
        {

            List<Modelo_ContratoListaModel> retorno = new List<Modelo_ContratoListaModel>();
            IOrderedEnumerable<Modelo_ContratoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MODELO_CONTRATO"); //WHERE STATUS = 1  

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

                    sql.AppendLine("SELECT A.ID,A.DESCRICAO,A.ARQUIVO_DOC,A.EXTENSAO_ARQUIVO,A.STATUS ");
                    sql.AppendLine("FROM TB_MODELO_CONTRATO A");


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
                            retorno.Add(new Modelo_ContratoListaModel
                            {

                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Arquivo_Doc = Convert.ToString(reader["ARQUIVO_DOC"]),
                                Extensao_Arquivo = Convert.ToString(reader["EXTENSAO_ARQUIVO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))


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
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
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
            return new DataTableResponse<Modelo_ContratoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Modelo_ContratoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Modelo_ContratoListaModel>> retorno = new GetApiRetorno<IEnumerable<Modelo_ContratoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoModelo = new Conexao<Modelo_Contrato>(session))
            {
                try
                {
                    var lista = await repoModelo.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Modelo_ContratoListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        Status_Default = p.Status,
                        Contrato_Automatico = p.Contrato_Automatico
                    });

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
        [Route("api/Modelo_ContratoApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboConfig<Int64?, bool?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MODELO_CONTRATO A WHERE ");

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
                    if (request.config.HasValue && request.config.Value)
                    {
                        condicoes.Add("A.PLANO_ESPECIFICO_COMBO = 1");
                    }
                    else if (request.config.HasValue && !request.config.Value)
                    {
                        condicoes.Add("COALESCE(A.PLANO_ESPECIFICO_COMBO, 0) = 0");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO, A.CONTRATO_AUTOMATICO, A.TEM_CARENCIA, A.PERIODO_CARENCIA,A.TESTEGRATIS,A.DIASTESTE ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ModeloContratoComboModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Contrato_Automatico = Convert.ToBoolean(reader["CONTRATO_AUTOMATICO"]),
                                    PossuiCarencia = Convert.ToBoolean(reader["TEM_CARENCIA"]),
                                    PeriodoCarencia =
                                        reader["PERIODO_CARENCIA"] != DBNull.Value ?
                                            Convert.ToInt32(reader["PERIODO_CARENCIA"]) :
                                            (int?)null,
                                    PossuiTesteGratis = Convert.ToBoolean(reader["TESTEGRATIS"]),
                                    DiasTesteGratis = reader["DIASTESTE"] != DBNull.Value ?
                                            Convert.ToInt32(reader["DIASTESTE"]) :
                                            (int?)null,
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


        [HttpPost()]
        [Route("api/Modelo_ContratoApi/GetComboAtivos")]
        public async Task<ObjCombo<Int64>> GetComboAtivos(RequestObjCombo<String> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MODELO_CONTRATO A WHERE A.STATUS = '1' ");

                    List<String> condicoes = new List<String>();

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" AND ({String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ModeloContratoComboModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
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

        [HttpPost()]
        [Route("api/Modelo_ContratoApi/GetCombo_CombosModelo")]
        public async Task<ObjCombo<long>> GetCombo_CombosModelo(RequestObjComboPai<long?, string> request)
        {
            long recordsFiltered = 0;
            var retorno = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> Condicoes = new List<string>();

                    #region SqlDefs
                    StringBuilder sql = new StringBuilder(),
                        sqlFrom = new StringBuilder(),
                        sqlOrder = new StringBuilder(),
                        sqlWhere = new StringBuilder();
                    #endregion
                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        var tipos = request.idpai.Split(',');
                        var condicao = new List<string>();

                        foreach (var item in tipos)
                        {
                            condicao.Add($"(C.TIPO_TECNOLOGIA = {item})");

                        }

                        Condicoes.Add($" ({String.Join(" OR ", condicao)})");

                    }
                    if (request.id.HasValue)
                    {
                        Condicoes.Add("A.ID=@ID_MODELO");
                        parametros.Add("@ID_MODELO", request.id);
                    }
                    if (!String.IsNullOrEmpty(request.q))
                    {
                        Condicoes.Add($" UPPER (A.NOME_COMBO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                    }



                    sqlFrom.AppendLine(" FROM TB_MODELO_CONTRATO A ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_MODELO_CONTRATO_PLANOPROD B ON(A.ID = B.ID_CONTRATO)");
                    sqlFrom.AppendLine(" LEFT JOIN TB_PLANO C ON(B.ID_PLANO = C.ID)");
                    sqlFrom.AppendLine(" LEFT JOIN TB_PRODUTO D ON(B.ID_PRODUTO = D.ID)");

                    Condicoes.Add(" A.PLANO_ESPECIFICO_COMBO = 1 ");
                    if (Condicoes.Count > 0)
                    {
                        sqlWhere.AppendLine($"WHERE {String.Join(" AND ", Condicoes)}");
                    }

                    //Count
                    //sql.AppendLine("SELECT COUNT(A.ID) AS CO" + sqlFrom.ToString() + "\r\n" + sqlWhere.ToString() + "\r\n" + sqlOrder.ToString());
                    sql.AppendFormat("SELECT COUNT(A.ID)  AS CO,A.DESCRICAO,A.NOME_COMBO  \r\n {0}  {1} GROUP BY  A.ID,A.DESCRICAO,A.NOME_COMBO \r\n ORDER BY A.ID", sqlFrom, sqlWhere);
                    using (var readerCount = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await readerCount.ReadAsync())
                        {
                            recordsFiltered = Convert.ToInt64(readerCount["CO"]);
                        }
                    }
                    sql.Clear();
                    //Select

                    sql.AppendFormat("SELECT A.ID ID_MODELO,A.DESCRICAO,A.NOME_COMBO,SUM(COALESCE(C.valorplano,D.valorproduto)) AS VALOR,SUM(COALESCE(C.valorinstalacao,0)) AS VALOR_INSTALACAO, " +
                        "A.TESTEGRATIS,A.DIASTESTE,A.TEM_CARENCIA,A.PERIODO_CARENCIA \r\n {0}  \r\n {1} \r\n  " +
                        "GROUP BY  A.ID, A.DESCRICAO, A.NOME_COMBO, A.TESTEGRATIS, A.DIASTESTE, A.TEM_CARENCIA,A.PERIODO_CARENCIA \r\n ORDER BY A.ID ", sqlFrom, sqlWhere);
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Modelo_PlanoObjCombo
                            {
                                Id = Convert.ToInt64(reader["ID_MODELO"]),
                                Text = Convert.ToString(reader["NOME_COMBO"]),
                                Id_Modelo_Contrato = Convert.ToInt64(reader["ID_MODELO"]),
                                Valor = reader["VALOR"] != DBNull.Value ? Convert.ToDouble(reader["VALOR"]).ToString("N2") : 0.ToString("N2"),
                                ValorInstalacao = reader["VALOR_INSTALACAO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_INSTALACAO"]).ToString("N2") : 0.ToString("N2"),
                                CheckVigenciaContrato = Convert.ToBoolean(reader["TESTEGRATIS"]),
                                Check_DiasTesteGratis = Convert.ToBoolean(reader["TEM_CARENCIA"]),
                                PeriodoFidelidade = Convert.ToString(reader["PERIODO_CARENCIA"]),
                                QtdeDiasGratis = Convert.ToString(reader["DIASTESTE"]),
                            };
                            retorno.results.Add(obj);
                        }
                    }
                }
            }
            catch (Exception erro)
            { _ = erro.Message; }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Modelo_ContratoApi/GetCombo_CombosModeloPlano")]
        public async Task<ObjCombo<long>> GetCombo_CombosModeloPlano(RequestObjComboPai<long?, string> request)
        {
            long recordsFiltered = 0;
            var retorno = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> Condicoes = new List<string>();

                    #region SqlDefs
                    StringBuilder sql = new StringBuilder(),
                        sqlFrom = new StringBuilder(),
                        sqlOrder = new StringBuilder(),
                        sqlWhere = new StringBuilder();
                    #endregion
                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        var tipos = request.idpai.Split(',');
                        var condicao = new List<string>();

                        foreach (var item in tipos)
                        {
                            condicao.Add($"(C.TIPO_TECNOLOGIA = {item})");

                        }

                        Condicoes.Add($" ({String.Join(" OR ", condicao)})");

                    }
                    if (request.id.HasValue)
                    {
                        Condicoes.Add("A.ID=@ID_MODELO");
                        parametros.Add("@ID_MODELO", request.id);
                    }
                    if (!String.IsNullOrEmpty(request.q))
                    {
                        Condicoes.Add($" UPPER (A.NOME_COMBO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                    }



                    sqlFrom.AppendLine(" FROM TB_MODELO_CONTRATO A ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_MODELO_CONTRATO_PLANOPROD B ON(A.ID = B.ID_CONTRATO)");
                    sqlFrom.AppendLine(" LEFT JOIN TB_PLANO C ON(B.ID_PLANO = C.ID)");
                    sqlFrom.AppendLine(" LEFT JOIN TB_PRODUTO D ON(B.ID_PRODUTO = D.ID)");

                    Condicoes.Add(" A.PLANO_ESPECIFICO_COMBO = 1 ");
                    if (Condicoes.Count > 0)
                    {
                        sqlWhere.AppendLine($"WHERE {String.Join(" AND ", Condicoes)}");
                    }

                    //Count
                    //sql.AppendLine("SELECT COUNT(A.ID) AS CO" + sqlFrom.ToString() + "\r\n" + sqlWhere.ToString() + "\r\n" + sqlOrder.ToString());
                    sql.AppendFormat("SELECT COUNT(A.ID)  AS CO,A.DESCRICAO,A.NOME_COMBO  \r\n {0}  {1} GROUP BY  A.ID,A.DESCRICAO,A.NOME_COMBO \r\n ORDER BY A.ID", sqlFrom, sqlWhere);
                    using (var readerCount = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await readerCount.ReadAsync())
                        {
                            recordsFiltered = Convert.ToInt64(readerCount["CO"]);
                        }
                    }
                    sql.Clear();
                    //Select

                    sql.AppendFormat("SELECT A.ID ID_MODELO,CASE WHEN B.DESCRICAO IS NULL THEN D.DESCRICAO ELSE B.DESCRICAO END AS PLANOPRODUTO,A.DESCRICAO,A.NOME_COMBO,SUM(COALESCE(C.valorplano,D.valorproduto)) AS VALOR,SUM(COALESCE(C.valorinstalacao,0)) AS VALOR_INSTALACAO, " +
                        "A.TESTEGRATIS,A.DIASTESTE,A.TEM_CARENCIA,A.PERIODO_CARENCIA \r\n {0}  \r\n {1} \r\n  " +
                        "GROUP BY  A.ID, A.DESCRICAO,CASE WHEN B.DESCRICAO IS NULL THEN D.DESCRICAO ELSE B.DESCRICAO END, A.NOME_COMBO, A.TESTEGRATIS, A.DIASTESTE, A.TEM_CARENCIA,A.PERIODO_CARENCIA \r\n ORDER BY A.ID ", sqlFrom, sqlWhere);
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Modelo_PlanoObjCombo
                            {
                                Id = Convert.ToInt64(reader["ID_MODELO"]),
                                Text = Convert.ToString(reader["PLANOPRODUTO"]),
                                Id_Modelo_Contrato = Convert.ToInt64(reader["ID_MODELO"]),
                                Valor = reader["VALOR"] != DBNull.Value ? Convert.ToDouble(reader["VALOR"]).ToString("N2") : 0.ToString("N2"),
                                ValorInstalacao = reader["VALOR_INSTALACAO"] != DBNull.Value ? Convert.ToDouble(reader["VALOR_INSTALACAO"]).ToString("N2") : 0.ToString("N2"),
                                CheckVigenciaContrato = Convert.ToBoolean(reader["TESTEGRATIS"]),
                                Check_DiasTesteGratis = Convert.ToBoolean(reader["TEM_CARENCIA"]),
                                PeriodoFidelidade = Convert.ToString(reader["PERIODO_CARENCIA"]),
                                QtdeDiasGratis = Convert.ToString(reader["DIASTESTE"]),
                            };
                            retorno.results.Add(obj);
                        }
                    }
                }
            }
            catch (Exception erro)
            { _ = erro.Message; }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Modelo_Contratoapi/GetItem_Contrato")]
        public async Task<ObjCombo<long?>> GetItem_Contrato(long? Id_Referencia = null)
        {
            var retorno = new ObjCombo<long?> { results = new List<IObjComboResult<long?>>() };
            Monte_seu_Combo obj = new Monte_seu_Combo { Contratos = new List<String>() };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MODELO_CONTRATO A LEFT JOIN TB_MODELO_CONTRATO_PLANOPROD B  ON A.ID = B.ID_CONTRATO"),
                        sqlWhere = new StringBuilder(),
                        sql = new StringBuilder();
                    if (Id_Referencia != null)
                    {
                        parametros.Add("@PARAM", Id_Referencia);
                        sqlWhere.AppendLine(" WHERE B.ID_PLANO = @PARAM OR B.ID_PRODUTO = @PARAM");
                    }

                    sql.AppendFormat(" SELECT A.ID, A.DESCRICAO,(SELECT COUNT (ID) FROM TB_MODELO_CONTRATO_PLANOPROD AS FILHOS)AS NUM_ITENS_CONTRATO {0} {1}", sqlFrom, sqlWhere);

                    using (System.Data.Common.DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            //Passado um plano específico, passar o número de ocorrencias em cada contrato
                            obj.Num_Planos = Convert.ToInt32(reader["NUM_ITENS_CONTRATO"]);
                            obj.Contratos.Add(Convert.ToString(reader["ID"]));
                        }
                    }
                    sql.Clear();
                    sql.AppendLine($"SELECT DISTINCT A.ID, A.DESCRICAO, A.TIPO_TECNOLOGIA FROM TB_MODELO_CONTRATO_PLANOPROD A WHERE A.ID_CONTRATO IN ({String.Join(",", obj.Contratos)})");
                    using (System.Data.Common.DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var combo = new Get_Item_ContratoObjCombo
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Text = Convert.ToString(reader["DESCRICAO"]),
                                Tipo_Tecnologia = (Tipo_Tecnologia)Convert.ToInt64(reader["TIPO_TECNOLOGIA"])

                            };
                            retorno.results.Add(combo);
                        }
                    }
                    retorno.total_count = obj.Num_Planos.HasValue ? obj.Num_Planos.Value : 0;
                }
            }
            catch (Exception erro)
            {
                String Mensagem = erro.Message;
                String InnerException = erro.InnerException.Message;
            }
            return retorno;
        }


        [HttpGet()]
        [Route("api/Modelo_Contratoapi/FindById")]
        public async Task<GetApiRetorno<Modelo_ContratoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Modelo_ContratoCadastroModel> retorno = new GetApiRetorno<Modelo_ContratoCadastroModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoModelo = new Conexao<Modelo_Contrato>(session))
                using (var repoAviso = new Conexao<AvisoModeloContrato>(session))
                {
                    retorno.Objeto = GetViewObj(await repoModelo.selectFirst(p => p.Id == Id));

                    retorno.Objeto.ListaCobrancasPre = await repoAviso.select(x => x.IdModeloContrato == Id);


                    retorno.Ok = true;

                }

            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }


            return retorno;
        }

        private Modelo_ContratoCadastroModel GetViewObj(Modelo_Contrato obj)
        {
            return new Modelo_ContratoCadastroModel
            {
                //saperX
                Cancela_Contrato_Automatico_Final_Carencia = obj.Cancela_Contrato_Automatico_Final_Carencia,
                Tem_Reajuste = obj.Tem_Reajuste,
                //
                Id = obj.Id,
                Descricao = obj.Descricao,
                Arquivo_Doc = obj.Arquivo_Doc,
                Extensao_Arquivo = obj.Extensao_Arquivo,
                Contrato_Automatico = obj.Contrato_Automatico,
                Tem_Carencia = obj.Tem_Carencia,
                Periodo_Carencia = obj.Periodo_Carencia,
                Multa_Carencia = obj.Multa_Carencia.HasValue ? obj.Multa_Carencia.Value.ToString("N2") : null,
                Tem_Pro_Rata = obj.Tem_Pro_Rata,

                MultaFidelidade = obj.MultaFidelidade,
                RenovarAutomaticamente = obj.RenovarAutomaticamente,
                DisponibilizaTelefonia = obj.DisponibilizaTelefonia,
                Status = obj.Status.Value == Status_Default.Ativo ? true : false,
                IntegracaoTelefonia = obj.IntegracaoTelefonia,
                FormatoCobrancaTelefonia = obj.FormatoCobrancaTelefonia,
                QuantidadeDid = obj.QuantidadeDid,
                DidFormatoCobrar = obj.DidFormatoCobrar,

                Renova_Carencia_Automaticamente = obj.Renova_Carencia_Automaticamente,
                Percentual_Reajuste_Contratual = Convert.ToString(obj.Percentual_Reajuste_Contratual),
                Valor_Reajuste_Contratual = Convert.ToString(obj.Valor_Reajuste_Contratual),
                Frequencia_Reajuste = obj.Frequencia_Reajuste,
                Tipo_Reajuste = obj.Tipo_Reajuste,

                ImpedirBloqueio_Automatico = obj.ImpedirBloqueio_Automatico == true ? true : false,
                ImpedirBloqueio_Segunda = obj.ImpedirBloqueio_Segunda,
                ImpedirBloqueio_Terca = obj.ImpedirBloqueio_Terca,
                ImpedirBloqueio_Quarta = obj.ImpedirBloqueio_Quarta,
                ImpedirBloqueio_Quinta = obj.ImpedirBloqueio_Quinta,
                ImpedirBloqueio_Sexta = obj.ImpedirBloqueio_Sexta,
                ImpedirBloqueio_Sabado = obj.ImpedirBloqueio_Sabado,
                QuantidadeDiasBloqueio = obj.QuantidadeDiasBloqueio,

                Utiliza_Desconto = obj.Utiliza_Desconto,
                Desconto = obj?.Desconto?.Id,
                ImpedirBloqueio_Domingo = obj.ImpedirBloqueio_Domingo,
                ImpedirBloqueio_Feriados = obj.ImpedirBloqueio_Feriados,
                Desbloqueio_Conf = obj.UtilizaDesbloqueioConfianca,
                BloqueioAutomaticoDebito = obj.BloqueioAutomaticoDebito,
                Frequencia_Desbloqueio = obj.FrequenciaDesbloqueio,
                Dias_desbloq = obj.QtdDiasdesbloqueados,
                TesteGratis = obj.TesteGratis,
                DiasTeste = obj.DiasTeste,
                CodigoIntegracao = obj.CodigoIntegracao,
                Observacao_ModeloContrato = obj.Observacao_ModeloContrato,
                BloquearNumeroDias = obj.BloquearNumeroDias,
                BloquearAtrasarRenegociadas = obj.BloquearAtrasarRenegociadas,
                MaximoTitulosAberto = obj.MaximoTitulosAberto
            };
        }

        private void BindViewToObj(ISession session, Modelo_Contrato objBanco, Modelo_ContratoCadastroModel view)
        {
            using (var repoModeloContrato = new Conexao<Modelo_Contrato>(session))
            {
                objBanco.Id = view.Id;
                objBanco.Descricao = view.Descricao;

                objBanco.Contrato_Automatico = view.Contrato_Automatico;
                objBanco.Tem_Carencia = view.Tem_Carencia;
                objBanco.Periodo_Carencia = view.Periodo_Carencia;
                objBanco.Multa_Carencia = Convert.ToDouble(view.Multa_Carencia);
                objBanco.MultaFidelidade = view.MultaFidelidade;
                objBanco.Tem_Pro_Rata = view.Tem_Pro_Rata;
                objBanco.RenovarAutomaticamente = view.RenovarAutomaticamente;
                objBanco.IntegracaoTelefonia = view.IntegracaoTelefonia;

                objBanco.FormatoCobrancaTelefonia = view.FormatoCobrancaTelefonia;
                objBanco.QuantidadeDid = view.QuantidadeDid;
                objBanco.DidFormatoCobrar = view.DidFormatoCobrar;
                objBanco.Renova_Carencia_Automaticamente = view.Renova_Carencia_Automaticamente;
                objBanco.Percentual_Reajuste_Contratual = Convert.ToDouble(view.Percentual_Reajuste_Contratual);
                objBanco.Valor_Reajuste_Contratual = Convert.ToDouble(view.Valor_Reajuste_Contratual);
                objBanco.Frequencia_Reajuste = view.Frequencia_Reajuste;
                objBanco.Tipo_Reajuste = view.Tipo_Reajuste;

                objBanco.QuantidadeDiasBloqueio = view.QuantidadeDiasBloqueio;
                objBanco.BloquearNumeroDias = view.BloquearNumeroDias;
                objBanco.BloquearAtrasarRenegociadas = view.BloquearAtrasarRenegociadas;
                objBanco.MaximoTitulosAberto = view.MaximoTitulosAberto;
                objBanco.FrequenciaPermitida = view.FrequenciaPermitida;

                objBanco.ImpedirBloqueio_Automatico = view.ImpedirBloqueio_Automatico == true ? true : false;
                objBanco.ImpedirBloqueio_Segunda = view.ImpedirBloqueio_Segunda;
                objBanco.ImpedirBloqueio_Terca = view.ImpedirBloqueio_Terca;
                objBanco.ImpedirBloqueio_Quarta = view.ImpedirBloqueio_Quarta;
                objBanco.ImpedirBloqueio_Quinta = view.ImpedirBloqueio_Quinta;
                objBanco.ImpedirBloqueio_Sexta = view.ImpedirBloqueio_Sexta;
                objBanco.ImpedirBloqueio_Sabado = view.ImpedirBloqueio_Sabado;
                objBanco.ImpedirBloqueio_Domingo = view.ImpedirBloqueio_Domingo;
                objBanco.ImpedirBloqueio_Feriados = view.ImpedirBloqueio_Feriados;

                objBanco.Utiliza_Desconto = view.Utiliza_Desconto;
                using (var repoDesconto = new Conexao<Desconto>(session))
                {
                    objBanco.Desconto = view?.Desconto > 0 ? repoDesconto.selectFirst(x => x.Id == view.Desconto).Result : null;
                }
                objBanco.UtilizaDesbloqueioConfianca = view.Desbloqueio_Conf;
                objBanco.FrequenciaDesbloqueio = view.Frequencia_Desbloqueio;
                objBanco.QtdDiasdesbloqueados = view.Dias_desbloq;
                objBanco.BloqueioAutomaticoDebito = view.BloqueioAutomaticoDebito;
                objBanco.TesteGratis = view.TesteGratis;
                objBanco.DiasTeste = view.DiasTeste;
                objBanco.CodigoIntegracao = view.CodigoIntegracao;
                objBanco.Observacao_ModeloContrato = view.Observacao_ModeloContrato;

                #region saperX

                objBanco.Cancela_Contrato_Automatico_Final_Carencia = view.Cancela_Contrato_Automatico_Final_Carencia;
                objBanco.Tem_Reajuste = view.Tem_Reajuste;

                #endregion

                if (view.ArquivoAlterado)
                {
                    objBanco.Arquivo_Doc = view.Arquivo_Doc;
                    objBanco.Extensao_Arquivo = view.Extensao_Arquivo;
                }
            }
        }

        [HttpPost()]
        [Route("api/Modelo_ContratoApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MODELO_CONTRATO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Modelo_ContratoApi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_MODELO_CONTRATO WHERE ID IN {_ids}");

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
        [Route("api/Modelo_ContratoApi/GetArquivoContrato")]
        public async Task<GetApiRetorno<Modelo_ContratoCadastroModel>> GetArquivoContrato(Int64 Id)
        {
            GetApiRetorno<Modelo_ContratoCadastroModel> retorno = new GetApiRetorno<Modelo_ContratoCadastroModel>();


            using (ISession session = HibernateUtil.GetSession())
            using (var repoModelo = new Conexao<Modelo_Contrato>(session))
            using (var repoAviso = new Conexao<AvisoModeloContrato>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repoModelo.selectFirst(p => p.Id == Id));

                    var avisos = await repoAviso.select(x => x.IdModeloContrato == Id);

                    if (avisos != null)
                    {
                        retorno.Objeto.ListaCobrancasPre = avisos.Where(x => x.TipoAviso == TipoAvisoModeloContratoEnum.PreCobranca).ToList();
                        retorno.Objeto.ListaBloqueios = avisos.Where(x => x.TipoAviso == TipoAvisoModeloContratoEnum.Bloqueio).ToList();
                        retorno.Objeto.ListaCobrancas = avisos.Where(x => x.TipoAviso == TipoAvisoModeloContratoEnum.Cobranca).ToList();
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
        [Route("api/modelo_contratoapi/GetPdfContrato")]
        public async Task<GetApiRetorno<PdfModeloContrato>> GetPdfContrato(Int64 Id_Modelo_Contrato)
        {
            GetApiRetorno<PdfModeloContrato> retorno = new GetApiRetorno<PdfModeloContrato>();
            retorno.Objeto = new PdfModeloContrato();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoModeloContrato = new Conexao<Modelo_Contrato>(session))
                {
                    Modelo_Contrato modelo_contrato = await repoModeloContrato.selectFirst(p => p.Id == Id_Modelo_Contrato);

                    String arquivo_modelo_contrato = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\{modelo_contrato.Id}.{modelo_contrato.Extensao_Arquivo}";
                    String desc_modelo_contrato = modelo_contrato.Descricao;

                    String _pasta_tmp = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Tmp";

                    if (!Directory.Exists(_pasta_tmp))
                        Directory.CreateDirectory(_pasta_tmp);

                    string nome_pdf = "";

                    if (modelo_contrato.Extensao_Arquivo.Equals("docx") || modelo_contrato.Extensao_Arquivo.Equals("doc"))
                    {
                        String _tmp_html = $@"{_pasta_tmp}\{Directory.GetFiles(_pasta_tmp).Length}.html";

                        ConvertToPdfService.convertDocxToHtml(_tmp_html, arquivo_modelo_contrato);

                        nome_pdf = $@"{Directory.GetFiles(_pasta_tmp).Length}.pdf";
                        String caminho_pdf = $@"{_pasta_tmp}\{nome_pdf}";

                        ConvertToPdfService.convertHtmlToPdf(caminho_pdf, _tmp_html);

                        File.Delete(_tmp_html);
                    }
                    else
                    {
                        nome_pdf = $"{Id_Modelo_Contrato}.{modelo_contrato.Extensao_Arquivo}";
                        File.Copy(arquivo_modelo_contrato, @$"{_pasta_tmp}\{nome_pdf}", true);
                    }

                    retorno.Ok = true;
                    retorno.Objeto = new PdfModeloContrato
                    {
                        Descricao_Modelo_Contrato = desc_modelo_contrato,
                        Nome_Pdf = nome_pdf
                    };
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
        [Route("api/Modelo_ContratoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Modelo_ContratoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Modelo_Contrato modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_Contrato>(session))
                    using (var repoPlanos = new Conexao<Plano>(session))
                    using (var repoProdutos = new Conexao<Produto>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Modelo_Contrato
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Status = Convert.ToBoolean(value.Objeto.Status) ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        List<Avisos> avisoCobrancas = JsonConvert.DeserializeObject<List<Avisos>>(value.Objeto.cobrancas).Where(x => x.Dias != null || x.Dias != 0).ToList();
                        List<Avisos> avisoPreCobrancas = JsonConvert.DeserializeObject<List<Avisos>>(value.Objeto.cobrancasPre).Where(x => x.Dias != null || x.Dias != 0).ToList();
                        List<Avisos> avisoBloqueios = JsonConvert.DeserializeObject<List<Avisos>>(value.Objeto.bloqueios).Where(x => x.Dias != null || x.Dias != 0).ToList();
                        List<Avisos> avisoNegativacoes = JsonConvert.DeserializeObject<List<Avisos>>(value.Objeto.negativacao).Where(x => x.Dias != null || x.Dias != 0).ToList();

                        await SalvarAvisosModelo(session, avisoCobrancas, avisoPreCobrancas, avisoBloqueios, avisoNegativacoes, modelo.Id);

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

        async Task SalvarAvisosModelo(ISession session, List<Avisos> avisosCobranca, List<Avisos> avisosPreCobranca, List<Avisos> avisosBloqueio, List<Avisos> avisosNegativacao, long idModeloContrato)
        {
            using (var repo = new Conexao<AvisoModeloContrato>(session))
            {
                var avisosExistentes = await repo.select(x => x.IdModeloContrato == idModeloContrato);

                if (avisosExistentes != null)
                    await repo.Remove(avisosExistentes);

                var listaBanco = new List<AvisoModeloContrato>();

                foreach (var aviso in avisosCobranca)
                {
                    listaBanco.Add(new AvisoModeloContrato
                    {
                        IdEmail = aviso.Email,
                        IdSMS = aviso.Sms,
                        IdNotificacao = aviso.Notificacao,
                        IdModeloContrato = idModeloContrato,
                        TipoAviso = TipoAvisoModeloContratoEnum.Cobranca
                    });
                }

                foreach (var aviso in avisosPreCobranca)
                {
                    listaBanco.Add(new AvisoModeloContrato
                    {
                        IdEmail = aviso.Email,
                        IdSMS = aviso.Sms,
                        IdNotificacao = aviso.Notificacao,
                        IdModeloContrato = idModeloContrato,
                        TipoAviso = TipoAvisoModeloContratoEnum.PreCobranca
                    });
                }

                foreach (var aviso in avisosBloqueio)
                {
                    listaBanco.Add(new AvisoModeloContrato
                    {
                        IdEmail = aviso.Email,
                        IdSMS = aviso.Sms,
                        IdNotificacao = aviso.Notificacao,
                        IdModeloContrato = idModeloContrato,
                        TipoAviso = TipoAvisoModeloContratoEnum.Bloqueio
                    });
                }

                foreach (var aviso in avisosNegativacao)
                {
                    listaBanco.Add(new AvisoModeloContrato
                    {
                        IdEmail = aviso.Email,
                        IdSMS = aviso.Sms,
                        IdNotificacao = aviso.Notificacao,
                        IdModeloContrato = idModeloContrato,
                        TipoAviso = TipoAvisoModeloContratoEnum.Negativacao
                    });
                }

                await repo.Save(listaBanco);
            }
        }

        [Route("api/Modelo_ContratoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_Contrato>(session))
                    {

                        Modelo_Contrato modelo = null;
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

        [HttpGet()]
        [Route("api/Modelo_ContratoApi/ExcluirContrato")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirContrato(Int64 Id)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_Contrato>(session))
                    {
                        Modelo_Contrato modelo = (await repository.select(p => p.Id == Id)).FirstOrDefault();

                        try
                        {
                            String caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\{modelo.Id}.{modelo.Extensao_Arquivo}";

                            if (File.Exists(caminhoArquivo))
                                File.Delete(caminhoArquivo);
                        }
                        catch { }

                        await repository.Remove(modelo);

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



        //teste para recuperar OS abertas de acordo com o cliente
        [HttpPost()]
        [Route("api/Modelo_ContratoApi/GetContratoCliente")]
        public async Task<RetornoSequenciaBot> GetContratoCliente(Int64? IdCliente)
        {
            RetornoSequenciaBot retorno = new RetornoSequenciaBot();
            List<BotEventoAnexo> anexo = new List<BotEventoAnexo>();
            List<string> mensagemRetorno = new List<string>();


            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT B.ID, B.ARQUIVO_DOC ")
                        .AppendLine(" FROM TB_CLIENTE_PLANO_PRODUTO A  ")
                        .AppendLine(" INNER JOIN TB_MODELO_CONTRATO B ")
                        .AppendLine(" ON (A.ID_MODELO_CONTRATO = B.ID) ")
                        .AppendLine("WHERE A.ID_CLIENTE = @ID_CLIENTE");

                        parametros.Add("@ID_CLIENTE", IdCliente);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {

                            using (var repoModeloContrato = new Conexao<Modelo_Contrato>(session))
                            {
                                while (await reader.ReadAsync())
                                {
                                    var descricao = Convert.ToString(reader["ARQUIVO_DOC"]);
                                    var idContrato = Convert.ToString(reader["ID"]);
                                    var idContratoBusca = Convert.ToInt32(reader["ID"]);

                                    Modelo_Contrato modelo_contrato = await repoModeloContrato.selectFirst(p => p.Id == idContratoBusca);

                                    String arquivo_modelo_contrato = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Contrato\{modelo_contrato.Id}.{modelo_contrato.Extensao_Arquivo}";
                                    String desc_modelo_contrato = modelo_contrato.Descricao;

                                    //String _pasta_tmp = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Tmp";
                                    String _pasta_tmp = $@"{CONSTANTES.CaminhoFisico}/Arquivos/BotEvento/";

                                    if (!Directory.Exists(_pasta_tmp))
                                        Directory.CreateDirectory(_pasta_tmp);

                                    String _tmp_html = $@"{_pasta_tmp}\{Directory.GetFiles(_pasta_tmp).Length}.html";

                                    ConvertToPdfService.convertDocxToHtml(_tmp_html, arquivo_modelo_contrato);

                                    int idNome = Directory.GetFiles(_pasta_tmp).Length;
                                    String nome_pdf = $@"{Directory.GetFiles(_pasta_tmp).Length}.pdf";
                                    String caminho_pdf = $@"{_pasta_tmp}\{nome_pdf}";

                                    ConvertToPdfService.convertHtmlToPdf(caminho_pdf, _tmp_html);

                                    File.Delete(_tmp_html);

                                    //Monta a mensagem de retorno
                                    if (descricao != "")
                                    {
                                        mensagemRetorno.Add("\r\n" + idContrato + " - " + descricao + "\n");
                                    }
                                    else
                                    {
                                        mensagemRetorno.Add("Não foram encontrados contratos para o cliente");
                                    }

                                    anexo.Add(new BotEventoAnexo
                                    {
                                        Id = idNome,
                                        Nome = descricao,
                                        Extensao = ".pdf",
                                        Tipo = Model.enumerado.Tipo_Bot_Evento_Anexo.Arquivo,
                                        Url = caminho_pdf
                                    });
                                }
                            }

                            retorno.sequenciaMensagens = mensagemRetorno;
                            retorno.anexos = anexo;
                        }
                    }
                }
                catch (Exception)
                {
                    //mensagemRetorno.Add("\r\n" + $"Erro Encontrado: {ex.Message}.");
                    retorno.sequenciaMensagens = mensagemRetorno;
                    retorno.anexos = anexo;
                }
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Modelo_ContratoApi/GetAllCombo")]
        public async Task<DataTableResponse<ComboDetalhePlanoProdutoModel>> GetAllCombo(Modelo_Contrato_DataTableRequest request)
        {

            List<ComboDetalhePlanoProdutoModel> retorno = new List<ComboDetalhePlanoProdutoModel>();
            //IOrderedEnumerable<Modelo_ContratoComboListModel> listaOrdenada = null;
            Int32 recordsTotal = 0;
            List<String> Condicoes = new List<String>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MODELO_CONTRATO"); //WHERE STATUS = 1  

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


                    StringBuilder sql = new StringBuilder(), sqlFrom = new StringBuilder(), sqlWhere = new StringBuilder();
                    sqlFrom.AppendLine(@"FROM TB_MODELO_CONTRATO_PLANOPROD A
                                         LEFT JOIN TB_PLANO B ON A.ID_PLANO = B.ID
                                         LEFT JOIN TB_PRODUTO C ON A.ID_PRODUTO = C.ID
                                         LEFT JOIN TB_FRANQUIA D ON B.ID_FRANQUIA=D.ID");

                    sqlWhere.AppendLine("WHERE A.ID_CONTRATO = @ID ");

                    sql.AppendLine(@"SELECT A.ID,A.DESCRICAO,A.TIPO_TECNOLOGIA,B.VELOCIDADEDOWNLOAD,B.FREQUENCIACOBRANCA,
                                     COALESCE(B.formatocobranca,C.formato_cobranca)AS FORMATO_COBRANCA, B.POSSUI_FRANQUIA,B.ID_FRANQUIA,D.MINUTOS,COALESCE(B.DETALHESPLANO, C.DETALHES) AS DETALHES_PLANOPRODUTO
                                     ");

                    sql.AppendFormat("{0} \r\n {1}", sqlFrom, sqlWhere);


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        //sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        Condicoes.Add("(UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }
                    if (request.Id.HasValue)
                    {
                        Condicoes.Add("(UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                    }



                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ComboDetalhePlanoProdutoModel
                            {

                                Detalhe = reader["DETALHES_PLANOPRODUTO"] != DBNull.Value ? Convert.ToString(reader["DETALHES_PLANOPRODUTO"]) : String.Empty,
                                Formato_Cobranca = reader["FORMATO_COBRANCA"] != DBNull.Value ? Convert.ToString(reader["FORMATO_COBRANCA"]) : String.Empty,
                                Franquia = reader[""] != DBNull.Value ? Convert.ToString(reader[""]) : String.Empty,
                                Frequencia_Cobranca = reader[""] != DBNull.Value ? Convert.ToString(reader[""]) : String.Empty,
                                PlanoProduto = reader[""] != DBNull.Value ? Convert.ToString(reader[""]) : String.Empty,
                                Ponto = reader[""] != DBNull.Value ? Convert.ToString(reader[""]) : String.Empty,
                                Velocidade = reader[""] != DBNull.Value ? Convert.ToString(reader[""]) : String.Empty
                                //Id = Convert.ToInt64(reader["ID"]),
                                //Descricao = Convert.ToString(reader["DESCRICAO"]),
                                //Arquivo_Doc = Convert.ToString(reader["ARQUIVO_DOC"]),
                                //Extensao_Arquivo = Convert.ToString(reader["EXTENSAO_ARQUIVO"]),
                                //Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))


                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ComboDetalhePlanoProdutoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }
        [HttpGet()]
        [Route("api/Modelo_ContratoApi/Get_ModeloVinculadoPlano")]
        public async Task<GetApiRetorno<long?>> Get_ModeloVinculadoPlano(string idPlanos)
        {
            var retorno = new GetApiRetorno<long?>
            {
                Objeto = null
            };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    long[] ids = JsonService.DeserializeObject<List<long>>(idPlanos).ToArray();
                    var sql = new StringBuilder($"SELECT FIRST 1 A.ID_CONTRATO FROM TB_MODELO_CONTRATO_PLANOPROD A WHERE A.ID IN({String.Join(",", ids)})");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno.Objeto = Convert.ToInt64(reader["ID_CONTRATO"]);
                        }
                    }
                }
                retorno.Ok = true;
            }

            catch (Exception error)
            {
                _ = $"Erro encontrado: {error.Message}";
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/Modelo_ContratoApi/SubstituirVariaveisContrato")]
        public IEnumerable<PostApiMensagem> SubstituirVariaveisContrato(ClienteContrato contrato, List<Cliente_PlanoProduto> planos, Venda venda, string doc)
        {
            var mensagens = new List<PostApiMensagem>();
            var valores = new Dictionary<String, Object>();

            foreach (VariaveisModeloContrato item in Enum.GetValues(typeof(VariaveisModeloContrato)))
            {
                try
                {
                    KeyValuePair<string, object> variable = this.SetVariable(new Modelo_ContratoGenerateVariable
                    {
                        item = item,
                        clienteContrato = contrato,
                        planos = planos,
                        Venda = venda,
                        mensagens = mensagens
                    }).Result;

                    valores.Add(variable.Key, variable.Value);
                }
                catch (Exception erro)
                {
                    mensagens.Add(new PostApiMensagem { Tipo = PostApiMensagemType.Erro, Mensagem = $"Erro ao capturar valores das variaveis: {erro.Message}" });
                }

            }

            try
            {

                doc = doc.Split().Select(sentence =>
                {
                    if (valores.ContainsKey(sentence))
                    {
                        sentence = Convert.ToString(valores.Where(p => p.Key == sentence).First().Value);
                    }
                    return sentence;
                }).ToString();

                mensagens.Add(new PostApiMensagem { Tipo = PostApiMensagemType.Info, Mensagem = Resources.RsBase.OPERACAOBEMSUCEDIDA });
            }

            catch (Exception erro)
            {
                mensagens.Add(new PostApiMensagem { Tipo = PostApiMensagemType.Erro, Mensagem = $"Erro ao substituir as variáveis: {erro.Message}" });
            }

            return mensagens;
        }

        public async Task<KeyValuePair<string, object>> SetVariable(Modelo_ContratoGenerateVariable request, int? index = null)
        {
            KeyValuePair<String, Object> retorno = new KeyValuePair<string, object>("", "");
            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Configuracoes_Gerais> repositoryConfig = new Conexao<Configuracoes_Gerais>(session))
            using (Conexao<Cliente> repositoryCliente = new Conexao<Cliente>(session))
            using (Conexao<Funcionario> repositoryFuncionario = new Conexao<Funcionario>(session))
            using (Conexao<Filial> repositoryFilial = new Conexao<Filial>(session))
            using (Conexao<Venda> repositoryVenda = new Conexao<Venda>(session))
            using (var repoClienteDesconto = new Conexao<Cliente_Desconto>(session))
            using (var repoTipo_Desconto = new Conexao<Tipo_Desconto>(session))
            using (var repoDesconto = new Conexao<Desconto>(session))
            using (var repoPlano = new Conexao<Plano>(session))
            {
                try
                {
                    #region [Validação - Cliente Contrato]

                    #region [Variáveis]
                    String nome_fantasia = String.Empty, cargo_responsavel = String.Empty, cpf_cnpj = String.Empty, cpf_responsavel = String.Empty,
                        email_principal = String.Empty, endereco_principal_logradouro = String.Empty, endereco_principal_descricao = String.Empty,
                        endereco_principal_numero = String.Empty, endereco_principal_bairro = String.Empty, endereco_principal_cidade = String.Empty,
                        endereco_principal_uf = String.Empty, endereco_principal_cep = String.Empty, filiacao_mae = String.Empty, filiacao_pai = String.Empty,
                        cliente_numero_contrato = String.Empty, inscricao_estadual = String.Empty, inscricao_municipal = String.Empty, pessoa_tel_contato = String.Empty,
                        razao_social = String.Empty, nome_resp_financeiro = String.Empty, rg = String.Empty, telefone_celular = String.Empty, telefone_recado = String.Empty,
                        telefone_fixo = String.Empty, numero_contrato = String.Empty;
                    DateTime? dt_nascimento = new DateTime();
                    Double? periodo_carencia = 0, multa_carencia = 0;
                    var multas_carencia = new Dictionary<string, object>();
                    multas_carencia = request.planos.Where(p => p?.ClienteContrato?.NumeroContrato != null).GroupBy(p => p.ClienteContrato.NumeroContrato).OrderBy(g => g.Key).ToDictionary(g => g.Key, g => (object)g.First().ClienteContrato.MultaCarencia);
                    Porte? enum_cliente_porte;
                    #endregion

                    if (request.clienteContrato != null)
                    {
                        #region [Cliente Contrato]
                        nome_fantasia = request.clienteContrato?.Cliente?.NomeFantasia;
                        cargo_responsavel = request.clienteContrato?.Cliente?.Cargo_Responsavel;
                        cpf_cnpj = request.clienteContrato?.Cliente?.CpfCnpj;
                        cpf_responsavel = request.clienteContrato?.Cliente?.Cpf_Responsavel;
                        dt_nascimento = request.clienteContrato?.Cliente?.Data_Nascimento;
                        email_principal = request.clienteContrato?.Cliente?.Email_Principal;
                        endereco_principal_logradouro = request.clienteContrato?.Cliente?.Endereco_Principal?.Logradouro;
                        endereco_principal_descricao = request.clienteContrato?.Cliente?.Endereco_Principal?.Endereco_Logradouro?.Descricao;
                        endereco_principal_numero = request.clienteContrato?.Cliente?.Endereco_Principal?.Numero;
                        endereco_principal_bairro = request.clienteContrato?.Cliente?.Endereco_Principal?.Bairro;
                        endereco_principal_cidade = request.clienteContrato?.Cliente?.Endereco_Principal?.Cidade;
                        endereco_principal_uf = request.clienteContrato?.Cliente?.Endereco_Principal?.UF;
                        endereco_principal_cep = request.clienteContrato?.Cliente?.Endereco_Principal?.Cep;
                        filiacao_mae = request.clienteContrato?.Cliente?.Filiacao_Mae;
                        filiacao_pai = request.clienteContrato?.Cliente?.Filiacao_Pai;
                        cliente_numero_contrato = request.clienteContrato?.Cliente?.Numero_Contrato;
                        inscricao_estadual = request.clienteContrato?.Cliente?.Inscricao_Estadual;
                        inscricao_municipal = request.clienteContrato?.Cliente?.Inscricao_Municipal;
                        pessoa_tel_contato = request.clienteContrato?.Cliente?.Pessoa_Tel_Recado;
                        enum_cliente_porte = request.clienteContrato?.Cliente?.Porte;
                        razao_social = request.clienteContrato?.Cliente?.Razao_Social;
                        nome_resp_financeiro = request.clienteContrato?.Cliente?.Nome_Resp_Financeiro;
                        rg = request.clienteContrato?.Cliente?.Rg;
                        telefone_celular = request.clienteContrato?.Cliente?.Telefone_Celular;
                        telefone_recado = request.clienteContrato?.Cliente?.Telefone_Recado;
                        telefone_fixo = request.clienteContrato?.Cliente?.Telefone_Fixo;
                        periodo_carencia = request.clienteContrato?.PeriodoCarencia;
                        numero_contrato = string.Join("-", request.planos.Where(p => p.ClienteContrato != null).Select(p => p?.ClienteContrato?.NumeroContrato).Distinct().ToList());                        
                        #endregion
                    }
                    else
                    {
                        #region [Objeto - Cliente]
                        var cliente = await repositoryCliente.selectFirst(x => x.Id == request.Venda.Id_Cliente);
                        nome_fantasia = cliente?.NomeFantasia;
                        cargo_responsavel = cliente?.Cargo_Responsavel;
                        cpf_cnpj = cliente?.CpfCnpj;
                        cpf_responsavel = cliente?.Cpf_Responsavel;
                        dt_nascimento = cliente?.Data_Nascimento;
                        email_principal = cliente?.Email_Principal;
                        endereco_principal_logradouro = cliente?.Endereco_Principal?.Logradouro;
                        endereco_principal_descricao = cliente?.Endereco_Principal?.Endereco_Logradouro?.Descricao;
                        endereco_principal_numero = cliente?.Endereco_Principal?.Numero;
                        endereco_principal_bairro = cliente?.Endereco_Principal?.Bairro;
                        endereco_principal_cidade = cliente?.Endereco_Principal?.Cidade;
                        endereco_principal_uf = cliente?.Endereco_Principal?.UF;
                        endereco_principal_cep = cliente?.Endereco_Principal?.Cep;
                        filiacao_mae = cliente?.Filiacao_Mae;
                        filiacao_pai = cliente?.Filiacao_Pai;
                        cliente_numero_contrato = cliente?.Numero_Contrato;
                        inscricao_estadual = cliente?.Inscricao_Estadual;
                        inscricao_municipal = cliente?.Inscricao_Municipal;
                        pessoa_tel_contato = cliente?.Pessoa_Tel_Recado;
                        enum_cliente_porte = cliente?.Porte;
                        razao_social = cliente?.Razao_Social;
                        nome_resp_financeiro = cliente?.Nome_Resp_Financeiro;
                        rg = cliente?.Rg;
                        telefone_celular = cliente?.Telefone_Celular;
                        telefone_recado = cliente?.Telefone_Recado;
                        telefone_fixo = cliente?.Telefone_Fixo;

                        var plano = await repoPlano.selectFirst(x => x.Id == request.cpp.Plano.Id);
                        periodo_carencia = plano?.Modelo_Contrato?.Periodo_Carencia;

                        numero_contrato = string.Join("-", request.planos.Where(p => p.ClienteContrato != null).Select(p => p?.ClienteContrato?.NumeroContrato).Distinct().ToList());
                        
                        #endregion
                    }
                    #endregion

                    switch (request.item)
                    {
                        case VariaveisModeloContrato.VAR_AGENCIA:
                            var agencia = !String.IsNullOrEmpty(request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Agencia) ? request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Agencia : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), agencia);
                            break;
                        case VariaveisModeloContrato.VAR_BANCO:
                            var banco = !String.IsNullOrEmpty(request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Banco?.Descricao) ? request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Banco?.Descricao : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), banco);
                            break;
                        case VariaveisModeloContrato.VAR_CARGO_RESPONSAVEL:
                            cargo_responsavel = !String.IsNullOrEmpty(cargo_responsavel) ? cargo_responsavel : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), cargo_responsavel);
                            break;
                        case VariaveisModeloContrato.VAR_CNPJ_DA_EMPRESA:
                            var cnpj_empresa = !String.IsNullOrEmpty(cpf_cnpj) ? cpf_cnpj : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), cnpj_empresa);
                            break;
                        case VariaveisModeloContrato.VAR_CONTA_CORRENTE:
                            var conta_corrente = !String.IsNullOrEmpty(request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Conta_Corrente) ? request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Conta_Corrente : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), conta_corrente);
                            break;
                        case VariaveisModeloContrato.VAR_CPF:
                            var cpf = !String.IsNullOrEmpty(cpf_cnpj) ? cpf_cnpj : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), cpf);
                            break;
                        case VariaveisModeloContrato.VAR_CPF_RESPONSAVEL:
                            cpf_responsavel = !String.IsNullOrEmpty(cpf_responsavel) ? cpf_responsavel : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), cpf_responsavel);
                            break;
                        case VariaveisModeloContrato.VAR_DATA_APROVACAO_VENDA:
                            var data_aprovacao = !String.IsNullOrEmpty(Convert.ToString(request.Venda?.Data_Venda)) ? request.Venda?.Data_Venda?.ToString("dd/MM/yyyy") : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), data_aprovacao);
                            break;
                        case VariaveisModeloContrato.VAR_DATA_DE_ABERTURA:
                            var data_abertura = !String.IsNullOrEmpty(Convert.ToString(request.Venda?.DataHoraCriacao)) ? request.Venda?.DataHoraCriacao?.ToString("dd/MM/yyyy") : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), data_abertura);
                            break;
                        case VariaveisModeloContrato.VAR_DATA_DE_CRIACAO_VENDA:
                            var data_criacao = !String.IsNullOrEmpty(Convert.ToString(request.Venda?.DataHoraCriacao)) ? request.Venda?.DataHoraCriacao?.ToString("dd/MM/yyyy") : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), data_criacao);
                            break;
                        case VariaveisModeloContrato.VAR_DATA_DE_ENCERRAMENTO:
                            var data_encerramento = !String.IsNullOrEmpty(Convert.ToString(request.Venda?.Data_Encerramento)) ? request.Venda?.Data_Encerramento?.ToString("dd/MM/yyyy") : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), data_encerramento);
                            break;
                        case VariaveisModeloContrato.VAR_DATA_EMISSAO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), DateTime.Now.ToString("dd/MM/yyyy"));
                            break;
                        case VariaveisModeloContrato.VAR_DATA_NASCIMENTO:
                            var data_nascimento = !String.IsNullOrEmpty(Convert.ToString(dt_nascimento)) ? dt_nascimento?.ToString("dd/MM/yyyy") : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), data_nascimento);
                            break;
                        case VariaveisModeloContrato.VAR_DESCONTO:
                            {
                            Double desconto = 0;
                                var desc_desconto = request.item.GetDisplayName();
                            if (request.clienteContrato != null)
                            {
                                var agroupamentoClienteContrato = request.planos.GroupBy(x => x.ClienteContrato).Select(x => x.First()).ToList();
                                foreach (var client_contract in agroupamentoClienteContrato)
                                {
                                    var lista_cliente_plano_produto = request.planos.Where(x => x.ClienteContrato.Id == client_contract.ClienteContrato.Id).ToList();
                                    var lista_cliente_desconto = await repoClienteDesconto.select(x => x.Cliente.Id == request.Venda.Id_Cliente && x.Numero_ContratoVinculado == Convert.ToString(client_contract.ClienteContrato.Id));
                                    var valor_final_plano = lista_cliente_plano_produto.Sum(x => x.Valor_Final);

                                    #region [variaveis desconto]
                                    Double valor_desconto_permanente = 0;
                                    Double percentual_desconto_permanente = 0;

                                    Double valor_desconto_promocional = 0;
                                    Double percentual_desconto_promocional = 0;
                                    #endregion

                                    if (lista_cliente_desconto.Count > 0)
                                    {
                                        foreach (var cliente_desconto in lista_cliente_desconto)
                                        {
                                                if (!cliente_desconto.Desconto.Valor_Unico && index != null)
                                                {
                                                    var lista_desconto_valores = cliente_desconto.Desconto.Valores.ToArray();
                                                    desc_desconto += $"_{index + 1}";
                                                    if (index < lista_desconto_valores.Length)
                                                    {
                                                        valor_desconto_permanente = Convert.ToDouble(lista_desconto_valores[Convert.ToInt32(index)].Valor);
                                                        percentual_desconto_permanente = Convert.ToDouble(lista_desconto_valores[Convert.ToInt32(index)].Percentual);
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                            var obj_desconto = await repoDesconto.selectFirst(x => x.Id == cliente_desconto.Desconto.Id);
                                            valor_desconto_permanente += obj_desconto.Desconto_Tempo_Indeterminado ? cliente_desconto.Valor_Desconto : 0;
                                            percentual_desconto_permanente += obj_desconto.Desconto_Tempo_Indeterminado ? cliente_desconto.Percentual_Desconto : 0;

                                            valor_desconto_promocional += !obj_desconto.Desconto_Tempo_Indeterminado ? cliente_desconto.Valor_Desconto : 0;
                                            percentual_desconto_promocional += !obj_desconto.Desconto_Tempo_Indeterminado ? cliente_desconto.Percentual_Desconto : 0;
                                        }
                                            }

                                        var _soma_valor_plano_contrato = lista_cliente_plano_produto.Sum(x => x.Valor_Final);

                                        var resultado_percentual_permanente = (_soma_valor_plano_contrato * percentual_desconto_permanente) / 100;
                                        resultado_percentual_permanente += valor_desconto_permanente;

                                        var resultado_percentual_promocional = (_soma_valor_plano_contrato * percentual_desconto_promocional) / 100;
                                        resultado_percentual_promocional += valor_desconto_promocional;

                                        var resultado_percentual = resultado_percentual_permanente + resultado_percentual_promocional;
                                        desconto += Convert.ToDouble(resultado_percentual);
                                    }
                                }
                            }

                                retorno = new KeyValuePair<string, object>(desc_desconto, String.Format("R$ {0:N2}", desconto));
                            break;
                            }                            
                        case VariaveisModeloContrato.VAR_DIA_DE_VENCIMENTO:
                            var dia_vencimento = request.planos.FirstOrDefault()?.Forma_Pagamento_Cliente?.Vencimento?.DiaVencimento > 0 ? request.planos.FirstOrDefault()?.Forma_Pagamento_Cliente?.Vencimento?.DiaVencimento : 0;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), dia_vencimento);
                            break;
                        //*******************
                        //Verificar Telefonia
                        //*******************
                        case VariaveisModeloContrato.VAR_DIDS_VINCULADOS_A_VENDA:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), null/*.Where(p=>p.JsonTelefonia)*/);
                            break;
                        //*******************
                        //Verificar Telefonia
                        //*******************
                        case VariaveisModeloContrato.VAR_DID_VINCULADO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), null);
                            break;
                        case VariaveisModeloContrato.VAR_DIGITO:
                            var digito = !String.IsNullOrEmpty(request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Digito) ? request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Digito : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), digito);
                            break;
                        case VariaveisModeloContrato.VAR_EMAIL:
                            var email = !String.IsNullOrEmpty(email_principal) ? email_principal : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), email);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_COBRANCA:
                            {
                                string logradouro = "";
                                string numero = "";
                                string bairro = "";
                                string cidade = "";
                                string uf = "";
                                string cep = "";

                                logradouro = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Logradouro;

                                if (String.IsNullOrEmpty(logradouro))
                                {
                                    logradouro = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Endereco_Logradouro?.Descricao;
                                }

                                numero = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Numero;
                                bairro = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Bairro;
                                cidade = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Cidade;
                                uf = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.UF;

                                cep = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Cep;

                                if (!String.IsNullOrEmpty(cep) && !cep.Contains("-"))
                                {
                                    cep = cep.Substring(0, 5) + "-" + cep.Substring(5, 3);
                                }

                                string endereco = String.Format("{0} nº {1} - {2} - {3}/{4} - {5}", logradouro, numero, bairro, cidade, uf, cep);

                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), endereco);
                                break;
                            }
                        case VariaveisModeloContrato.VAR_ENDERECO_DA_EMPRESA:
                            {
                                string logradouro = "";
                                string numero = "";
                                string bairro = "";
                                string cidade = "";
                                string uf = "";
                                string cep = "";

                                if (request.Venda != null)
                                {
                                    var obj_func = await repositoryFuncionario.selectFirst(x => x.Num_Fre == request.Venda.Vendedor.Num_Fre);
                                    if (obj_func != null)
                                    {
                                        var obj_filial = await repositoryFilial.selectFirst(x => x.Id == obj_func.Filial.Id);

                                        logradouro = Convert.ToString(obj_filial?.Endereco);
                                        numero = Convert.ToString(obj_filial?.Numero);
                                        bairro = Convert.ToString(obj_filial?.Bairro);
                                        cidade = Convert.ToString(obj_filial?.Cidade);
                                        uf = Convert.ToString(obj_filial?.Uf);

                                        cep = Convert.ToString(obj_filial?.Cep);

                                        if (!String.IsNullOrEmpty(cep) && !cep.Contains("-"))
                                {
                                            cep = cep.Substring(0, 5) + "-" + cep.Substring(5, 3);
                                }
                                    }
                                }
                                else
                                {

                                    var obj_cpp = request.planos.Where(x => x.ClienteContrato.Id == request.clienteContrato.Id).First();
                                    var obj_venda = await repositoryVenda.selectFirst(x => x.Id == obj_cpp.Id_Venda);
                                    var obj_func = await repositoryFuncionario.selectFirst(x => x.Num_Fre == obj_venda.Vendedor.Num_Fre);

                                    if (obj_func != null)
                                    {
                                        var obj_filial = await repositoryFilial.selectFirst(x => x.Id == obj_func.Filial.Id);

                                        logradouro = Convert.ToString(obj_filial?.Endereco);
                                        numero = Convert.ToString(obj_filial?.Numero);
                                        bairro = Convert.ToString(obj_filial?.Bairro);
                                        cidade = Convert.ToString(obj_filial?.Cidade);
                                        uf = Convert.ToString(obj_filial?.Uf);

                                        cep = Convert.ToString(obj_filial?.Cep);

                                if (!String.IsNullOrEmpty(cep) && !cep.Contains("-"))
                                {
                                    cep = cep.Substring(0, 5) + "-" + cep.Substring(5, 3);
                                }
                                    }
                                }
                                string endereco = String.Format("{0} - {1} nº {2} - {3} - {4}/{5}", cep, logradouro, numero, bairro, cidade, uf);

                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), endereco);
                                break;
                            }
                        case VariaveisModeloContrato.VAR_ENDERECO_DE_INSTALACAO:
                            {
                                string logradouro = "";
                                string numero = "";
                                string bairro = "";
                                string cidade = "";
                                string uf = "";
                                string cep = "";

                                logradouro = request.planos?.FirstOrDefault()?.Endereco_Instalacao?.Logradouro;

                                if (String.IsNullOrEmpty(logradouro))
                                {
                                    logradouro = request.planos?.FirstOrDefault()?.Endereco_Instalacao?.Endereco_Logradouro?.Descricao;
                                }

                                numero = request.planos?.FirstOrDefault()?.Endereco_Instalacao?.Numero;
                                bairro = request.planos?.FirstOrDefault()?.Endereco_Instalacao?.Bairro;
                                cidade = request.planos?.FirstOrDefault()?.Endereco_Instalacao?.Cidade;
                                uf = request.planos?.FirstOrDefault()?.Endereco_Instalacao?.UF;

                                cep = request.planos?.FirstOrDefault()?.Endereco_Instalacao?.Cep;

                                if (!String.IsNullOrEmpty(cep) && !cep.Contains("-"))
                                {
                                    cep = cep.Substring(0, 5) + "-" + cep.Substring(5, 3);
                                }

                                string endereco = String.Format("{0} nº {1} - {2} - {3}/{4} - {5}", logradouro, numero, bairro, cidade, uf, cep);

                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), endereco);
                                break;
                            }
                        case VariaveisModeloContrato.VAR_ENDERECO_PRINCIPAL:
                            {
                                string logradouro = "";
                                string numero = "";
                                string bairro = "";
                                string cidade = "";
                                string uf = "";
                                string cep = "";

                                logradouro = endereco_principal_logradouro;

                                if (String.IsNullOrEmpty(logradouro))
                                {
                                    logradouro = endereco_principal_descricao;
                                }

                                numero = endereco_principal_numero;
                                bairro = endereco_principal_bairro;
                                cidade = endereco_principal_cidade;
                                uf = endereco_principal_uf;

                                cep = endereco_principal_cep;

                                if (!String.IsNullOrEmpty(cep) && !cep.Contains("-"))
                                {
                                    cep = cep.Substring(0, 5) + "-" + cep.Substring(5, 3);
                                }

                                string endereco = String.Format("{0} nº {1} - {2} - {3}/{4} - {5}", logradouro, numero, bairro, cidade, uf, cep);

                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), endereco);
                                break;
                            }
                        case VariaveisModeloContrato.VAR_FILIACAO:
                            var filiacao = !String.IsNullOrEmpty(filiacao_mae) ? filiacao_mae : String.Empty;
                            if (!String.IsNullOrEmpty(filiacao))
                            {
                                if (!String.IsNullOrEmpty(filiacao_pai))
                                {
                                    filiacao += String.Concat(" - ", filiacao_pai);
                                }
                                else
                                {
                                    filiacao += String.Empty;
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(filiacao_pai))
                                {
                                    filiacao = String.Concat(filiacao_pai);
                                }
                                else
                                {
                                    filiacao += String.Empty;
                                }
                            }
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), filiacao);
                            break;
                        case VariaveisModeloContrato.VAR_FORMA_COBRANCA:
                            var desc_forma_cobranca_enum = HtmlEnumExtensions.GetEnumDescription(request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Forma_Pagamento?.Tipo_Forma_Pagamento);
                            var forma_cobranca = !String.IsNullOrEmpty(desc_forma_cobranca_enum) ? desc_forma_cobranca_enum : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), forma_cobranca);
                            break;
                        case VariaveisModeloContrato.VAR_FORMA_ENVIO_FATURA:
                            var desc_forma_envio_fatura_enum = HtmlEnumExtensions.GetEnumDescription(request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Forma_Envio);
                            var forma_envio_fatura = !String.IsNullOrEmpty(desc_forma_envio_fatura_enum) ? desc_forma_envio_fatura_enum : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), forma_envio_fatura);
                            break;
                        case VariaveisModeloContrato.VAR_FORMA_PAGAMENTO:
                            var forma_pagamento = String.Join(",", request.planos?.Select(p => p.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao).ToList());
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), !String.IsNullOrEmpty(forma_pagamento) ? forma_pagamento : String.Empty);
                            break;
                        case VariaveisModeloContrato.VAR_FRANQUIA:
                            var franquia = !String.IsNullOrEmpty(request.planos?.FirstOrDefault()?.Plano?.Franquia?.CodigoAuxiliar) ? request.planos?.FirstOrDefault()?.Plano?.Franquia?.CodigoAuxiliar : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), franquia);
                            break;
                        case VariaveisModeloContrato.VAR_IDENTIFICACAO_CLIENTE:
                            var identificacao_cliente = !String.IsNullOrEmpty(cliente_numero_contrato) ? cliente_numero_contrato : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), identificacao_cliente);
                            break;
                        case VariaveisModeloContrato.VAR_INSCRICAO_ESTADUAL:
                            inscricao_estadual = !String.IsNullOrEmpty(inscricao_estadual) ? inscricao_estadual : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), inscricao_estadual);
                            break;
                        case VariaveisModeloContrato.VAR_INSCRICAO_MUNICIPAL:
                            inscricao_municipal = !String.IsNullOrEmpty(inscricao_municipal) ? inscricao_municipal : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), inscricao_municipal);
                            break;
                        //*******************
                        //Verificar Telefonia
                        //*******************
                        case VariaveisModeloContrato.VAR_JUSTIFICATIVA_DA_APROVCAO_OU_CANCELAMENTO_DA_VENDA:
                            var justificativa_aprovacao_cancelamento_venda = !String.IsNullOrEmpty(request.Venda?.Justificativa_Avaliacao_Comercial) ? request.Venda?.Justificativa_Avaliacao_Comercial : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), justificativa_aprovacao_cancelamento_venda);
                            break;
                        case VariaveisModeloContrato.VAR_MENSALIDADE_DE_CADA_DID:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), null);
                            break;
                        case VariaveisModeloContrato.VAR_MULTA_CARENCIA:
                            {
                                string valorMultaCarencia = "";
                                if (multa_carencia != null)
                                    valorMultaCarencia = String.Format("R$ {0:N2}", request.planos.ToList()?.Sum(x => x.Multa_Carencia_Reais));
                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), (multas_carencia.Count == 0 ? "0" : string.Join(", ", multas_carencia.Select(p => $"R$ {p.Value} ({p.Key})").ToList())));
                                break;
                            }
                        case VariaveisModeloContrato.VAR_NOME_CLIENTE:
                            var nome_cliente = !String.IsNullOrEmpty(nome_fantasia) ? nome_fantasia : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), nome_cliente);
                            break;
                        case VariaveisModeloContrato.VAR_NOME_DA_EMPRESA:
                            var nome_empresa = !String.IsNullOrEmpty(nome_fantasia) ? nome_fantasia : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), nome_empresa);
                            break;
                        case VariaveisModeloContrato.VAR_NOME_FANTASIA_EMPRESA:
                            var nome_fantasia_empresa = !String.IsNullOrEmpty(nome_fantasia) ? nome_fantasia : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), nome_fantasia_empresa);
                            break;
                        case VariaveisModeloContrato.VAR_NOME_PARA_RECADO:
                            var nome_recado = !String.IsNullOrEmpty(pessoa_tel_contato) ? pessoa_tel_contato : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), nome_recado);
                            break;
                        case VariaveisModeloContrato.VAR_NUMERO_CONTRATO:
                            numero_contrato = !String.IsNullOrEmpty(numero_contrato) ? numero_contrato : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), numero_contrato.Replace("-", "\n"));
                            break;
                        case VariaveisModeloContrato.VAR_NUMERO_DA_VENDA:
                            var numero_venda = request.Venda?.Id > 0 ? request.Venda?.Id : 0;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), numero_venda);
                            break;
                        case VariaveisModeloContrato.VAR_PERIODO_DE_CARENCIA:
                            periodo_carencia = periodo_carencia > 0 ? periodo_carencia : 0;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), periodo_carencia);
                            break;
                        case VariaveisModeloContrato.VAR_PLANO:
                            string descricao = "";

                            if (!request.planos.Any(p => p.Combo != null) && request.planos.Any() != false)
                            {
                                var primeiroPlanoProduto = request.planos.FirstOrDefault();

                                if (primeiroPlanoProduto.Plano != null)
                                {
                                    descricao = primeiroPlanoProduto.Plano.Descricao;
                                }
                                else if (primeiroPlanoProduto.Produto != null)
                                {
                                    descricao = primeiroPlanoProduto.Produto.Descricao;
                                }
                                else if (primeiroPlanoProduto.Combo != null)
                                {
                                    descricao = primeiroPlanoProduto.Combo.Descricao;
                                }
                            }

                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), descricao);
                            break;
                        case VariaveisModeloContrato.VAR_PLANOS_PRODUTOS:
                            List<string> descricoesPlanoProduto = request.planos.SelectMany(p =>
                            {
                                List<String> strLista = new List<String>();
                                if (p.Combo != null)
                                    strLista.Add(p.Combo.Descricao);
                                else if (p.Plano != null)
                                    strLista.Add(p.Plano.Descricao);
                                else if (p.Produto != null)
                                    strLista.Add(p.Produto.Descricao);

                                return strLista;
                            }).ToList();

                            var agrupamento_planos_produtos = descricoesPlanoProduto.Where(x => !String.IsNullOrEmpty(x)).GroupBy(c => c).Select(c => new { c.Key });
                            var lista_planos_produtos = new List<String>();
                            foreach (var item in agrupamento_planos_produtos)
                            {
                                lista_planos_produtos.Add(item.Key);
                            }

                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), $"{String.Join("\n", lista_planos_produtos)}");
                            break;
                        case VariaveisModeloContrato.VAR_DESCRICAO_CONTRATO:
                            List<string> descricaoModeloContratoPlanoProduto = request.planos.SelectMany(p =>
                            {
                                List<String> strLista = new List<String>();
                                if (p.Plano != null)
                                {
                                    var descricao = p.Plano.Modelo_Contrato != null ? p.Plano.Modelo_Contrato.Descricao : String.Empty;
                                    strLista.Add(descricao);
                                }
                                else if (p.Produto != null)
                                {
                                    var descricao = p.Produto.ModeloContratoProduto != null ? p.Produto.ModeloContratoProduto.Descricao : String.Empty;
                                    strLista.Add(descricao);
                                }
                                else if (p.Combo != null)
                                {
                                    var _tmp = p.Combo.PlanosProdutos.Select(y => y.Plano.Modelo_Contrato.Descricao).Distinct().ToList();
                                    strLista.AddRange(_tmp);
                                }

                                return strLista;
                            }).ToList();

                            var agrupamento_descricoes_contratos = descricaoModeloContratoPlanoProduto.Where(x => !String.IsNullOrEmpty(x)).GroupBy(c => c).Select(c => new { c.Key });
                            var lista_descricoes_contratos = new List<String>();
                            foreach (var item in agrupamento_descricoes_contratos)
                            {
                                lista_descricoes_contratos.Add(item.Key);
                            }

                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), $"{String.Join("\n", lista_descricoes_contratos)}");
                            break;
                        case VariaveisModeloContrato.VAR_OBSERVACAO_CONTRATO:
                            List<string> observacaoModeloContratoPlanoProduto = request.planos.SelectMany(p =>
                            {
                                List<String> strLista = new List<String>();
                                if (p.Plano != null)
                                {
                                    var observacao = p.Plano.Modelo_Contrato != null ? p.Plano.Modelo_Contrato.Observacao_ModeloContrato : String.Empty;
                                    strLista.Add(observacao);
                                }
                                else if (p.Produto != null)
                                {
                                    var observacao = p.Produto.ModeloContratoProduto != null ? p.Produto.ModeloContratoProduto.Observacao_ModeloContrato : String.Empty;
                                    strLista.Add(observacao);
                                }
                                else if (p.Combo != null)
                                {
                                    var _tmp = p.Combo.PlanosProdutos.Select(y => y.Plano.Modelo_Contrato.Observacao_ModeloContrato).Distinct().ToList();
                                    strLista.AddRange(_tmp);
                                }
                                return strLista;
                            }).ToList();


                            var agrupamento_descricoes_observacao_contrato = observacaoModeloContratoPlanoProduto.Where(x => !String.IsNullOrEmpty(x)).GroupBy(c => c).Select(c => new { c.Key });
                            var lista_descricao_observacao_contrato = new List<String>();
                            foreach (var item in agrupamento_descricoes_observacao_contrato)
                            {
                                lista_descricao_observacao_contrato.Add(item.Key);
                            }

                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), $"{String.Join("\n", lista_descricao_observacao_contrato)}");
                            break;
                        case VariaveisModeloContrato.VAR_DETALHES_PLANO:
                            var idsDet = new List<long>();
                            List<string> detalhesPlanoProduto = request.planos.SelectMany(p =>
                            {
                                List<String> strLista = new List<String>();
                                if (p.Plano != null && !idsDet.Contains(p.Plano.Id))
                                {
                                    idsDet.Add(p.Plano.Id);
                                    strLista.Add(p.Plano.DetalhesPlano);
                                }
                                else if (p.Produto != null && !idsDet.Contains(p.Produto.Id))
                                {
                                    idsDet.Add(p.Produto.Id);
                                    strLista.Add(p.Produto.Detalhes);
                                }
                                else if (p.Combo != null && !idsDet.Contains(p.Combo.Id))
                                {
                                    idsDet.Add(p.Combo.Id);
                                    var _tmp = p.Combo.PlanosProdutos.Select(y => y.Plano.DetalhesPlano).Distinct().ToList();
                                    strLista.AddRange(_tmp);
                                }

                                return strLista;
                            }).ToList();


                            detalhesPlanoProduto = detalhesPlanoProduto.Where(x => !String.IsNullOrEmpty(x)).ToList();

                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), $"{String.Join("\n", detalhesPlanoProduto)}");
                            break;
                        case VariaveisModeloContrato.VAR_DETALHES_COMBO:
                            var _idsDet = new List<long>();
                            List<string> detalhesCombo = request.planos.Select(p =>
                            {
                                if (p.Combo != null && !_idsDet.Contains(p.Combo.Id))
                                {
                                    _idsDet.Add(p.Combo.Id);
                                    return p.Combo?.DetalhesCombo ?? "";
                                }
                                return "";
                            }).ToList();

                            detalhesCombo = detalhesCombo.Where(x => !x.Equals("")).ToList();
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), $"{String.Join("\n", detalhesCombo)}");
                            break;
                        case VariaveisModeloContrato.VAR_PORTE:
                            var desc_porte_enum = HtmlEnumExtensions.GetEnumDescription(enum_cliente_porte);
                            var porte = !String.IsNullOrEmpty(desc_porte_enum) ? desc_porte_enum : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), porte);
                            break;
                        case VariaveisModeloContrato.VAR_RAZAO_SOCIAL:
                            razao_social = !String.IsNullOrEmpty(razao_social) ? razao_social : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), razao_social);
                            break;
                        case VariaveisModeloContrato.VAR_RESPONSAVEL:
                            var responsavel = !String.IsNullOrEmpty(nome_resp_financeiro) ? nome_resp_financeiro : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), responsavel);
                            break;
                        case VariaveisModeloContrato.VAR_RG:
                            rg = !String.IsNullOrEmpty(rg) ? rg : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), rg);
                            break;
                        case VariaveisModeloContrato.VAR_SERVICOS_AGREGADOS:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), null);
                            break;
                        case VariaveisModeloContrato.VAR_STATUS:
                            var desc_status_enum = HtmlEnumExtensions.GetEnumDescription(request.Venda?.Status);
                            var status = !String.IsNullOrEmpty(desc_status_enum) ? desc_status_enum : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), status);
                            break;
                        case VariaveisModeloContrato.VAR_SUPERVISOR_DO_VENDEDOR:
                            var supervisor_vendedor = !String.IsNullOrEmpty(request.Venda?.Supervisor.Nome) ? request.Venda?.Supervisor.Nome : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), supervisor_vendedor);
                            break;
                        case VariaveisModeloContrato.VAR_TELEFONE_CELULAR:
                            telefone_celular = !String.IsNullOrEmpty(telefone_celular) ? telefone_celular : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), telefone_celular);
                            break;
                        case VariaveisModeloContrato.VAR_TELEFONE_DE_RECADO:
                            telefone_recado = !String.IsNullOrEmpty(telefone_recado) ? telefone_recado : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), telefone_recado);
                            break;
                        case VariaveisModeloContrato.VAR_TELEFONE_FIXO_1:
                            telefone_fixo = !String.IsNullOrEmpty(telefone_fixo) ? telefone_fixo : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), telefone_fixo);
                            break;
                        case VariaveisModeloContrato.VAR_TESTE_GRATIS:
                            var teste_gratis = request.planos?.FirstOrDefault()?.Dias_Teste_Gratis > 0 ? request.planos?.FirstOrDefault()?.Dias_Teste_Gratis : 0;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), teste_gratis);
                            break;

                        case VariaveisModeloContrato.VAR_VALOR_FINAL_TOTAL:
                            {
                                Double? valor_final = Convert.ToDouble(request.planos.ToList().Sum(x => x.Valor_Final));
                                var desc_valor_final_total = request.item.GetDisplayName();
                                if (request.clienteContrato != null)
                                {
                                    var agroupamentoClienteContrato = request.planos.GroupBy(x => x.ClienteContrato).Select(x => x.First()).ToList();
                                    foreach (var client_contract in agroupamentoClienteContrato)
                                    {
                                        var lista_cliente_plano_produto = request.planos.Where(x => x.ClienteContrato.Id == client_contract.ClienteContrato.Id).ToList();
                                        var lista_cliente_desconto = await repoClienteDesconto.select(x => x.Cliente.Id == request.Venda.Id_Cliente && x.Numero_ContratoVinculado == Convert.ToString(client_contract.ClienteContrato.Id));
                                        Double valor_desconto = 0;
                                        Double percentual_desconto = 0;
                                        Double valor_acrescimento = 0;
                                        Double percentual_acrescimento = 0;

                                        if (lista_cliente_desconto.Count > 0)
                                        {
                                            foreach (var cliente_desconto in lista_cliente_desconto)
                                            {
                                                if (!cliente_desconto.Desconto.Valor_Unico && index != null)
                                                {
                                                    var lista_desconto_valores = cliente_desconto.Desconto.Valores.ToArray();
                                                    desc_valor_final_total += $"_{index + 1}";
                                                    if (index < lista_desconto_valores.Length)
                                                    {
                                                        valor_desconto = Convert.ToDouble(lista_desconto_valores[Convert.ToInt32(index)].Valor);
                                                        percentual_desconto = Convert.ToDouble(lista_desconto_valores[Convert.ToInt32(index)].Percentual);
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                var obj_tipo_desconto = await repoTipo_Desconto.selectFirst(x => x.Id == cliente_desconto.Desconto.Tipo_Desconto.Id);
                                                valor_desconto += obj_tipo_desconto.Formato == Formato_Tipo_Desconto.Desconto ? cliente_desconto.Valor_Desconto : 0;
                                                percentual_desconto += obj_tipo_desconto.Formato == Formato_Tipo_Desconto.Desconto ? cliente_desconto.Percentual_Desconto : 0;

                                                valor_acrescimento += obj_tipo_desconto.Formato == Formato_Tipo_Desconto.Acrescimo ? cliente_desconto.Valor_Desconto : 0;
                                                percentual_acrescimento += obj_tipo_desconto.Formato == Formato_Tipo_Desconto.Acrescimo ? cliente_desconto.Percentual_Desconto : 0;
                                            }
                                            }

                                            var _soma_valor_plano_contrato = lista_cliente_plano_produto.Sum(x => x.Valor_Final);
                                            var resultado_percentual = (_soma_valor_plano_contrato * percentual_desconto) / 100;
                                            resultado_percentual += valor_desconto;

                                            var resultado_acrescimo = (_soma_valor_plano_contrato * percentual_acrescimento) / 100;
                                            resultado_acrescimo += valor_acrescimento;

                                            valor_final = ((valor_final + resultado_acrescimo) - resultado_percentual);
                                        }
                                    }
                                }

                                retorno = new KeyValuePair<string, object>(desc_valor_final_total, String.Format("R$ {0:N2}", valor_final));
                                break;
                            }
                        case VariaveisModeloContrato.VAR_VALOR_DESCONTO_VITALICIO:
                            {
                                Double valor_desconto_vitalicio = 0;
                                if (request.clienteContrato != null)
                                {
                                    var agroupamentoClienteContrato = request.planos.GroupBy(x => x.ClienteContrato).Select(x => x.First()).ToList();
                                    Double valor_final_plano = Convert.ToDouble(request.planos.ToList().Sum(x => x.Valor_Final));
                                    foreach (var client_contract in agroupamentoClienteContrato)
                                    {
                                        var lista_cliente_plano_produto = request.planos.Where(x => x.ClienteContrato.Id == client_contract.ClienteContrato.Id).ToList();
                                        var lista_cliente_desconto = await repoClienteDesconto.select(x => x.Cliente.Id == request.Venda.Id_Cliente && x.Numero_ContratoVinculado == Convert.ToString(client_contract.ClienteContrato.Id));
                                        if (lista_cliente_desconto.Count > 0)
                                        {
                                            Double valor_desconto = 0;
                                            Double percentual_desconto = 0;
                                            foreach (var obj in lista_cliente_desconto)
                                            {
                                                var obj_desconto = await repoDesconto.selectFirst(x => x.Id == obj.Desconto.Id);
                                                valor_desconto += obj_desconto.Desconto_Tempo_Indeterminado ? obj.Valor_Desconto : 0;
                                                percentual_desconto += obj_desconto.Desconto_Tempo_Indeterminado ? obj.Percentual_Desconto : 0;
                                            }

                                            var _soma_valor_plano_contrato = lista_cliente_plano_produto.Sum(x => x.Valor_Final);
                                            var resultado_percentual = (_soma_valor_plano_contrato * percentual_desconto) / 100;
                                            resultado_percentual += valor_desconto;

                                            valor_desconto_vitalicio += Convert.ToDouble(resultado_percentual);
                                        }
                                    }
                                }

                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), String.Format("R$ {0:N2}", valor_desconto_vitalicio));
                                break;
                            }
                        case VariaveisModeloContrato.VAR_VALOR_DESCONTO_TEMPORARIO:
                            {
                                Double valor_desconto_vitalicio = 0;
                                var desc_valor_desconto_temporario = request.item.GetDisplayName();
                                if (request.clienteContrato != null)
                                {
                                    var agroupamentoClienteContrato = request.planos.GroupBy(x => x.ClienteContrato).Select(x => x.First()).ToList();
                                    Double valor_final_plano = Convert.ToDouble(request.planos.ToList().Sum(x => x.Valor_Final));
                                    foreach (var client_contract in agroupamentoClienteContrato)
                                    {
                                        var lista_cliente_plano_produto = request.planos.Where(x => x.ClienteContrato.Id == client_contract.ClienteContrato.Id).ToList();
                                        var lista_cliente_desconto = await repoClienteDesconto.select(x => x.Cliente.Id == request.Venda.Id_Cliente && x.Numero_ContratoVinculado == Convert.ToString(client_contract.ClienteContrato.Id));
                                        if (lista_cliente_desconto.Count > 0)
                                        {
                                            Double valor_desconto = 0;
                                            Double percentual_desconto = 0;
                                            foreach (var obj in lista_cliente_desconto)
                                            {
                                                if (!obj.Desconto.Valor_Unico && index != null)
                                                {
                                                    var lista_desconto_valores = obj.Desconto.Valores.ToArray();
                                                    desc_valor_desconto_temporario += $"_{index + 1}";
                                                    if (index < lista_desconto_valores.Length)
                                                    {
                                                        valor_desconto = Convert.ToDouble(lista_desconto_valores[Convert.ToInt32(index)].Valor);
                                                        percentual_desconto = Convert.ToDouble(lista_desconto_valores[Convert.ToInt32(index)].Percentual);
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                var obj_desconto = await repoDesconto.selectFirst(x => x.Id == obj.Desconto.Id);
                                                valor_desconto += !obj_desconto.Desconto_Tempo_Indeterminado ? obj.Valor_Desconto : 0;
                                                percentual_desconto += !obj_desconto.Desconto_Tempo_Indeterminado ? obj.Percentual_Desconto : 0;
                                            }
                                            }

                                            var _soma_valor_plano_contrato = lista_cliente_plano_produto.Sum(x => x.Valor_Final);
                                            var resultado_percentual = (_soma_valor_plano_contrato * percentual_desconto) / 100;
                                            resultado_percentual += valor_desconto;

                                            valor_desconto_vitalicio += Convert.ToDouble(resultado_percentual);
                                        }
                                    }
                                }

                                retorno = new KeyValuePair<string, object>(desc_valor_desconto_temporario, String.Format("R$ {0:N2}", valor_desconto_vitalicio));
                                break;
                            }
                        case VariaveisModeloContrato.VAR_DURACAO_MAX_DESCONTO:
                            {
                                Double valor_repeticoes_desconto = 0;
                                if (request.clienteContrato != null)
                                {
                                    var agroupamentoClienteContrato = request.planos.GroupBy(x => x.ClienteContrato).Select(x => x.First()).ToList();
                                    Double valor_final_plano = Convert.ToDouble(request.planos.ToList().Sum(x => x.Valor_Final));
                                    List<Double> valor_repeticoes = new List<Double>();
                                    foreach (var client_contract in agroupamentoClienteContrato)
                                    {
                                        var lista_cliente_desconto = await repoClienteDesconto.select(x => x.Cliente.Id == request.Venda.Id_Cliente && x.Numero_ContratoVinculado == Convert.ToString(client_contract.ClienteContrato.Id));
                                        if (lista_cliente_desconto.Count > 0)
                                        {
                                            foreach (var obj in lista_cliente_desconto)
                                            {
                                                var obj_desconto = await repoDesconto.selectFirst(x => x.Id == obj.Desconto.Id);
                                                if (obj_desconto?.Repeticoes_Desconto > valor_repeticoes_desconto)
                                                {
                                                    valor_repeticoes_desconto = Convert.ToDouble(obj_desconto?.Repeticoes_Desconto);
                                                }
                                            }
                                        }
                                    }
                                }

                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), Convert.ToString(valor_repeticoes_desconto));
                                break;
                            }
                        case VariaveisModeloContrato.VAR_VALOR_INSTALACAO:
                            {
                                string valorInstalacao = "";

                                if (request.planos.FirstOrDefault()?.Valor_Instalacao_Final != null)
                                    valorInstalacao = String.Format("R$ {0:N2}", request.planos.FirstOrDefault()?.Valor_Instalacao_Final);

                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), valorInstalacao);
                                break;
                            }
                        case VariaveisModeloContrato.VAR_VALOR_MENSALIDADE:
                            {
                                string valorMensalidade = "";

                                if (request.planos.FirstOrDefault()?.Valor_Final != null)
                                    valorMensalidade = String.Format("R$ {0:N2}", request.planos.ToList()?.Sum(x => x.Valor_Final));

                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), valorMensalidade);
                                break;
                            }
                        case VariaveisModeloContrato.VAR_VELOCIDADE_DOWNLOAD:
                            var velocidade_download = !String.IsNullOrEmpty(request.planos.FirstOrDefault()?.Plano?.VelocidadeDownload) ? request.planos.FirstOrDefault()?.Plano?.VelocidadeDownload : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), velocidade_download);
                            break;
                        case VariaveisModeloContrato.VAR_VELOCIDADE_UPLOAD:
                            var velocidade_upload = !String.IsNullOrEmpty(request.planos.FirstOrDefault()?.Plano?.VelocidadeUpload) ? request.planos.FirstOrDefault()?.Plano?.VelocidadeUpload : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), velocidade_upload);
                            break;
                        case VariaveisModeloContrato.VAR_VENDEDOR:
                            var vendedor = !String.IsNullOrEmpty(request.Venda?.Vendedor.Nome) ? request.Venda?.Vendedor.Nome : String.Empty;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), vendedor);
                            break;
                        case VariaveisModeloContrato.VAR_TERMO_ASSINATURA:
                            var configuracao = await repositoryConfig.selectFirst();
                            string doc = null;

                            if (configuracao.Utiliza_Certificacao_Assinatura)
                            {
                                using (var AdesaoApi = new Termo_AdesaoApiController())
                                {
                                    GetApiRetorno<List<AssinaturaTermoAdesaoDados>> operacao = await AdesaoApi.GetAssinaturaTermoAdesao(request.Venda?.Id);
                                    if (operacao.Ok)
                                    {
                                        doc = operacao.Objeto.Select(parameter => parameter.Assinatura).FirstOrDefault();

                                    }
                                }
                            }

                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), doc);
                            break;
                        case VariaveisModeloContrato.VAR_COMPOSICAO_COMBO:
                            var composicaoCombo = new List<string>();
                            string retorno_descricao_composicao_combo = String.Empty;
                            if (request.planos.Any(p => p?.Combo != null))
                            {
                                foreach (var plano in request.planos.Where(p => p.Combo != null))
                                {
                                    composicaoCombo.Add($"{plano.Combo.Descricao} - R${plano.Combo.PlanosProdutos.Sum(p => p.ValorInstalacao) + plano.Combo.PlanosProdutos.Sum(p => p.Valor)}");

                                    composicaoCombo.AddRange(plano.Combo.PlanosProdutos.Select(p =>
                                {
                                    string descricao_composicao_combo = string.IsNullOrEmpty(p?.Plano?.Descricao) ? p?.Produto?.Descricao : p?.Plano?.Descricao;
                                        return $"\t\u2022{descricao_composicao_combo} - R${p?.Valor ?? 0 + p?.ValorInstalacao ?? 0}";
                                    }));
                                }
                            }

                            retorno_descricao_composicao_combo = String.Join("\n", composicaoCombo);
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), retorno_descricao_composicao_combo);
                            break;
                        //CARD 104 APOIO IMPLANTACAO
                        case VariaveisModeloContrato.VAR_LOGIN_RADIUS:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.cpp?.Login_Radius);
                            break;
                        case VariaveisModeloContrato.VAR_SENHA_RADIUS:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.cpp?.Senha_Radius);
                            break;
                        /*case VariaveisModeloContrato.VAR_DETALHES_PLANO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.planos.FirstOrDefault()?.Plano?.DetalhesPlano);
                            break;*/

                        case VariaveisModeloContrato.VAR_MODELO_CONTRATO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.clienteContrato?.ModeloContrato);
                            break;
                        case VariaveisModeloContrato.VAR_TIPO_PLANO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.planos.FirstOrDefault()?.Plano?.Tipo_Tecnologia);
                            break;


                        //ENDERECO PRINCIPAL
                        case VariaveisModeloContrato.VAR_ENDERECO_PRINCIPAL_UF:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.clienteContrato?.Cliente?.Endereco_Principal?.UF);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_PRINCIPAL_CIDADE:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.clienteContrato?.Cliente?.Endereco_Principal?.Cidade);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_PRINCIPAL_BAIRRO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.clienteContrato?.Cliente?.Endereco_Principal?.Bairro);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_PRINCIPAL_RUA:
                            string logradouroprincipal = "";

                            logradouroprincipal = request.clienteContrato?.Cliente?.Endereco_Principal?.Logradouro;

                            if (String.IsNullOrEmpty(logradouroprincipal))
                            {
                                logradouroprincipal = request.clienteContrato?.Cliente?.Endereco_Principal?.Endereco_Logradouro?.Descricao;
                            }
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), logradouroprincipal);
                            break;

                        case VariaveisModeloContrato.VAR_ENDERECO_PRINCIPAL_NUMERO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.clienteContrato?.Cliente?.Endereco_Principal?.Numero);
                            break;

                        case VariaveisModeloContrato.VAR_ENDERECO_PRINCIPAL_CEP:
                            string cepprincipal = "";

                            cepprincipal = request.clienteContrato?.Cliente?.Endereco_Principal?.Cep;

                            if (!String.IsNullOrEmpty(cepprincipal) && !cepprincipal.Contains("-"))
                            {
                                cepprincipal = cepprincipal.Substring(0, 5) + "-" + cepprincipal.Substring(5, 3);
                            }

                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), cepprincipal);
                            break;



                        //ENDERECO COBRANCA
                        case VariaveisModeloContrato.VAR_ENDERECO_COBRANCA_UF:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.UF);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_COBRANCA_CIDADE:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Cidade);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_COBRANCA_BAIRRO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Bairro);
                            break;


                        case VariaveisModeloContrato.VAR_ENDERECO_COBRANCA_RUA:
                            string logradourocobranca = "";
                            string cepcobranca = "";

                            logradourocobranca = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Logradouro;

                            if (String.IsNullOrEmpty(logradourocobranca))
                            {
                                logradourocobranca = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Endereco_Logradouro?.Descricao;
                            }
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), logradourocobranca);
                            break;


                        case VariaveisModeloContrato.VAR_ENDERECO_COBRANCA_NUMERO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Numero);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_COBRANCA_CEP:
                            string cepcobrancaseparado = "";

                            cepcobrancaseparado = request.planos?.FirstOrDefault()?.Forma_Pagamento_Cliente?.Endereco_Cobranca?.Cep;

                            if (!String.IsNullOrEmpty(cepcobrancaseparado) && !cepcobrancaseparado.Contains("-"))
                            {
                                cepcobranca = cepcobrancaseparado.Substring(0, 5) + "-" + cepcobrancaseparado.Substring(5, 3);
                            }

                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), cepcobrancaseparado);
                            break;

                        //ENDERECO INSTALACAO
                        case VariaveisModeloContrato.VAR_ENDERECO_INSTALACAO_UF:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.clienteContrato?.Cliente?.Endereco_Instalacao?.UF);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_INSTALACAO_CIDADE:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.clienteContrato?.Cliente?.Endereco_Instalacao?.Cidade);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_INSTALACAO_BAIRRO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.clienteContrato?.Cliente?.Endereco_Instalacao?.Bairro);
                            break;
                        case VariaveisModeloContrato.VAR_ENDERECO_INSTALACAO_RUA:
                            string logradouroinstalacao = "";

                            logradouroinstalacao = request.planos?.FirstOrDefault()?.ClienteContrato?.Cliente?.Endereco_Instalacao?.Logradouro;

                            if (String.IsNullOrEmpty(logradouroinstalacao))
                            {
                                logradouroinstalacao = request.planos?.FirstOrDefault()?.ClienteContrato?.Cliente?.Endereco_Instalacao?.Endereco_Logradouro?.Descricao;
                            }
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), logradouroinstalacao);
                            break;

                        case VariaveisModeloContrato.VAR_ENDERECO_INSTALACAO_CEP:
                            string cepinstalacao = "";

                            cepinstalacao = request.clienteContrato?.Cliente?.Endereco_Instalacao?.Numero;

                            if (!String.IsNullOrEmpty(cepinstalacao) && !cepinstalacao.Contains("-"))
                            {
                                cepinstalacao = cepinstalacao.Substring(0, 5) + "-" + cepinstalacao.Substring(5, 3);
                            }

                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), cepinstalacao);
                            break;


                        case VariaveisModeloContrato.VAR_ENDERECO_INSTALACAO_NUMERO:
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), request.clienteContrato?.Cliente?.Endereco_Instalacao?.Numero);
                            break;

                    }
                }
                catch (Exception exception)
                {
                    String Field = $"Campo atual da Requisição:{request.item}";
                    request.mensagens.Add(new PostApiMensagem
                    {
                        Mensagem = exception.InnerException != null ? $" {Field}\r\n Message:{exception.Message}\r\n{exception.InnerException.Message}" : $" {Field}\r\n Message:{exception.Message}",
                        Tipo = PostApiMensagemType.Erro,
                    });
                }

            }

            return retorno;
        }

        #region Garbage
        //[HttpPost()]
        //[Route("api/Modelo_ContratoApi/GetComboModeloAvulso_Combo")]
        //public async Task<TesteOptGroup<String, String>> GetComboModeloAvulso_Combo(RequestObjComboConfig<long?, Modelo_ContratoOptGroupAvulso_Combo> request)
        //{
        //    List<ObjComboOptGroupResult<String, String>> data = null;
        //    try
        //    {
        //        #region try

        //        using (ISession session = HibernateUtil.GetSession())
        //        using (ISession sessionNH = HibernateUtil.GetSession())
        //        using (var com = session.Connection.CreateCommand())
        //        using (var repoPlanos = new Conexao<Modelo_ContratoPlanosAdicionados>(sessionNH))
        //        {
        //            Dictionary<long, string> Dict_ModeloContrato = new Dictionary<long, string>();
        //            List<String> Condicoes = new List<string>();
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //            StringBuilder sql = new StringBuilder(),
        //                          sqlFrom = new StringBuilder(" FROM TB_MODELO_CONTRATO A"),
        //                          sqlWhere = new StringBuilder();

        //            if (request.config.Combo)
        //                Condicoes.Add($"A.PLANO_ESPECIFICO_COMBO=1");

        //            else if (!request.config.Combo)
        //                Condicoes.Add($"A.PLANO_ESPECIFICO_COMBO=0");

        //            sqlWhere.AppendLine($"WHERE {String.Join("AND", Condicoes)}");

        //            sql.AppendFormat("Select A.ID, A.DESCRICAO {0} \r\n {1} ", sqlFrom.ToString(), sqlWhere.ToString());
        //            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //            {
        //                while (await reader.ReadAsync())
        //                {

        //                    Dict_ModeloContrato.Add(Convert.ToInt64(reader["ID"]), Convert.ToString(reader["DESCRICAO"]));

        //                }
        //            }
        //            data = Dict_ModeloContrato.Select(p =>
        //             new ObjComboOptGroupResult<string, String>
        //             {
        //                 Id_Grupo = Convert.ToString(p.Key),
        //                 Text = p.Value,
        //                 Children = new List<IObjComboResult<string>>()

        //             }).ToList();

        //            data.ForEach(p => p.Children.AddRange(repoPlanos.select(obj => obj.Modelo.Id == Convert.ToInt64(p.Id_Grupo)).Result.Select(p => new Contrato_Combo_AvulsoObjComboResult<string>
        //            {
        //                Id = Convert.ToString(p.Id),
        //                Text = p.Descricao,
        //                Id_Pai = p.Modelo.Id
        //            }).ToList()));
        //            data.RemoveAll(p => p.Children.Count() == 0);

        //        }
        //        #endregion

        //    }
        //    catch (Exception ex) { _ = ex.Message; }
        //    return new TesteOptGroup<String, string> { results = data, };
        //}
        #endregion
    }

    public class Avisos
    {
        public long? Dias { get; set; }
        public long? Email { get; set; }
        public long? Sms { get; set; }
        public long? Notificacao { get; set; }
    }
}
