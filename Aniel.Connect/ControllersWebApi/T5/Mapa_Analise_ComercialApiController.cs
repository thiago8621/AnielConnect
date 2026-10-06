using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using Aniel.Connect.Models.T5;
using NHibernate;
using Sinapse.DAO.repository;
using System.Text;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Util;
using Aniel.T5.Model.nhibernate.mongo;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.Repository;
using Microsoft.AspNet.Identity;
using Aniel.Connect.Models;
using MongoDB.Driver;
using Aniel.Connect.Models.HubDesenvolvedor;
using RestSharp;
using System.Net;
using Aniel.T5.Model.mongo;
using MongoDB.Driver.Linq;
using Resources;
using OpenXmlPowerTools;
using System.Windows.Threading;
using DocumentFormat.OpenXml.Drawing.Charts;
using NHibernate.Mapping;
using DocumentFormat.OpenXml.Office2010.Excel;
using Aniel.Connect.Core.Helpers;
using Twilio.TwiML.Voice;
using static Aniel.Connect.Models.TelegramModels;
using FluentNHibernate.Utils;
using NHibernate.Criterion;
using static Aniel.Connect.Models.MessengerModels;
using NHibernate.Engine;
using DocumentFormat.OpenXml.Bibliography;
using Sinapse.Model;

namespace Aniel.Connect.ControllersWebApi.T5
{
    
    [Authorize]
    public class Mapa_Analise_ComercialApiController : BaseApiController, IMapa_AnaliseComercialApi
    {

        [HttpGet()]
        [Route("api/Mapa_Analise_ComercialApi/ReservarCaixa")]
        #region Teste_de_Api_Cuidado
        public async Task<GetApiRetorno<ReservarCaixa>> ReservarCaixa(Int64? Id_caixa, Int64? Porta, Int64? Cliente_planoProd)
        {

            var retorno = new GetApiRetorno<ReservarCaixa>();
            try
            {


                using (var sessiont5 = HibernateUtil.GetSessionT5())
                using (var com = sessiont5.Connection.CreateCommand())
                {
                    List<ReservarCaixa> objlista = new List<ReservarCaixa>();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //StringBuilder sqlFromReservarPorta = new StringBuilder(" ");                   

                    StringBuilder sql = new StringBuilder("UPDATE TB_ELEMENTO_REDE_PORTA A ");
                    sql.AppendLine($" SET  A.STATUS = '2', A.CLIENTE_PLANO_PRODUTO = {Cliente_planoProd} ");
                    sql.AppendLine(" WHERE A.ID_CAIXA = @CAIXA AND A.PORTA = @PORTA ");

                    parametros.Add("@CAIXA", Id_caixa);
                    parametros.Add("@PORTA", Porta);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessiont5.Connection, null))
                    {
                        retorno.Ok = true;
                    }

                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        [HttpGet()]
        [Route("api/Mapa_Analise_ComercialApi/GetInfo")]
        #region Teste_de_Api_Cuidado
        public async Task<GetApiRetorno<List<PlanoProdutoComboModels>>> GetInfo(String jsonrequest)
        {
            List<Int32> ids = JsonService.DeserializeObject<List<Int32>>(jsonrequest);
            var retorno = new GetApiRetorno<List<PlanoProdutoComboModels>>();
            try
            {

                using (var session = HibernateUtil.GetSession())
                using (var sessionNH = HibernateUtil.GetSession())
                using (var sessiont5 = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                using (var repoPlano = new Conexao<Plano>(session))
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionNH))
                using (var repoAreCob_PlanoProd = new Conexao<Area_Cobertura_Plano_Produto>(sessiont5))
                using (var repoAreCob_Combo = new Conexao<Area_Cobertura_Combo>(sessiont5))

                {
                    Configuracoes_Gerais config = await repoConfig.selectFirst();
                    var objlista = new List<PlanoProdutoComboModels>();

                    var sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.VALIDADE, A.CODIGO, A.DESCRICAO, A.SITUACAOPLANO AS SITUACAO, 1 AS TIPO, ");
                    sql.AppendLine("        A.VALORPLANO AS VALOR, A.VALORINSTALACAO AS VALORINSTALACAO, COALESCE(A.TESTEGRATIS, 0) AS TESTE_GRATIS ");
                    sql.AppendLine(" FROM TB_PLANO A ");
                    sql.AppendLine(" WHERE A.SITUACAOPLANO = 1 ");
                    sql.AppendLine("   AND ((A.ATIVARVALIDADE = 1 AND A.VALIDADE > CURRENT_TIMESTAMP) OR (A.ATIVARVALIDADE = 0)) ");

                    sql.AppendLine(" UNION");

                    sql.AppendLine(" SELECT B.ID, B.DATA_VALIDADE, B.CODIGO, B.DESCRICAO, B.SITUACAO AS SITUACAO, 2 AS TIPO, ");
                    sql.AppendLine("        B.VALORPRODUTO AS VALOR, B.VALORPRODUTO AS VALORINSTALACAO, COALESCE(B.TEM_TESTE_GRATIS, 0) AS TESTE_GRATIS ");
                    sql.AppendLine(" FROM TB_PRODUTO B ");
                    sql.AppendLine(" WHERE B.SITUACAO = 1 ");
                    sql.AppendLine("   AND ((B.ATIVAR_VALIDADE = 1 AND B.DATA_VALIDADE > CURRENT_TIMESTAMP) OR (B.ATIVAR_VALIDADE = 0)) ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new PlanoProdutoComboModels
                            {
                                //FROM TB_PLANO A
                                //Meses_Carencia = Convert.ToInt64(reader["CARENCIACONTRATO"]), FALAR COM YAGO
                                ValorPlanoProduto = String.Format("{0:N2}", Convert.ToDouble(reader["VALOR"])),
                                ValorInstalacao = String.Format("{0:N2}", Convert.ToDouble(reader["VALORINSTALACAO"])),

                                Qtde_Dias = Convert.ToInt64(reader["TESTE_GRATIS"]),

                                Id = Convert.ToInt64(reader["ID"]),
                                Text = Convert.ToString(reader["DESCRICAO"])
                            };

                            objlista.Add(obj);
                        }

                        if (config.Ofertar_Plano_Produto_Cobertura)
                        {
                            List<Area_Cobertura_Plano_Produto> area = new List<Area_Cobertura_Plano_Produto>();
                            List<Area_Cobertura_Combo> areaCombo = new List<Area_Cobertura_Combo>();


                            foreach (int _id in ids)
                            {
                                area.AddRange(await repoAreCob_PlanoProd.select(z => z.Area.Id == _id) as List<Area_Cobertura_Plano_Produto>);
                                areaCombo.AddRange(await repoAreCob_Combo.select(z => z.Area.Id == _id) as List<Area_Cobertura_Combo>);

                            }

                            if (area.Count > 0)
                            {
                                var codigosPlanoProduto = area.Select(p => Convert.ToInt64(p.Codigo_Plano_Produto)).ToList();
                                objlista = objlista.FindAll(p => codigosPlanoProduto.Contains(p.Id));
                            }

                            if (areaCombo.Count > 0)
                            {
                                var codigosCombo = areaCombo.Select(p => Convert.ToInt64(p.Codigo_Combo)).ToList();
                                objlista = objlista.FindAll(p => codigosCombo.Contains(p.Id));
                            }


                        }

                        retorno.Objeto = objlista;
                        retorno.Ok = true;
                    }

                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        [HttpGet()]
        [Route("api/Mapa_Analise_ComercialApi/GetAllPoligonosMongo")]
        #region MyRegion
        public async Task<List<Analise_Area_Cobertura>> GetAllPoligonosMongo(FilterRecordsAnalise filters, Int64? Id = null)
        {
            List<Area_CoberturaMongo> records = new List<Area_CoberturaMongo>();
            List<Analise_Area_Cobertura> retorno = new List<Analise_Area_Cobertura>();


            try
            {
                using (var sessionMongo = new ConexaoMongoGeography())
                using (var session = HibernateUtil.GetSession())
                using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))
                {
                    if (Id.HasValue)
                        records = await repoMongo.FindAll(p => p.Id == Id);
                    else
                        records = await repoMongo.FindAll();

                    if (filters.Campanha.HasValue)
                    {
                        using (var repo_Campanha = new Conexao<Campanha>(session))
                        using (var repo_CAC = new Conexao<Campanha_AreaCobertura>(session))
                        {
                            var Campanha_AreaCover = (await repo_CAC.select(p => p.Campanha.Id == filters.Campanha)).Select(p => p.Area_Cobertura).ToList();
                            records = records.Where(p => p.Id.IsIn(Campanha_AreaCover)).ToList();
                        }
                    }

                    retorno = records.Select(record =>
                    {
                        return new Analise_Area_Cobertura
                        {
                            Id = record.Id,
                            Color = record.Cor,
                            Coordenadas = record.Coordenadas,
                            Descricao = record.Descricao,
                            Latitude = record.Latitude,
                            Longitude = record.Longitude,
                            Situacao = record.Situacao,
                            Tipo = record.Tipo,
                        };
                    }).ToList();
                }
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }

            return retorno;
        }
        #endregion
        //public class value 
        //{
        //    public virtual double Max { get; set; }
        //    public virtual double Min { get; set; }
        //}
        //public value getValues(double campo) 
        //{ value ponto= new value();
        //    ponto.Max =campo+10 ;
        //    ponto.Min =campo-10 ;
        //    return ponto;
        //}

        [HttpGet()]
        [Route("api/Mapa_Analise_ComercialApi/GetComboPoligonosMongo")]
        public async Task<GetApiRetorno<List<Area_CoberturaMongo>>> GetComboPoligonosMongo(double Latitude, double Longitude, Int64? Id = null)
        {
            var retorno = new GetApiRetorno<List<Area_CoberturaMongo>>
            {
                Objeto = new List<Area_CoberturaMongo>()
            };

            try
            {
                var parametros = new Dictionary<String, Object>();
                var operador = new Dictionary<String, Object>();

                if (!((Latitude == 0) || (Longitude == 0)))
                {
                    //parametros.Add("","") 
                    double lat = Latitude;
                    double latMax = lat + 0.4;
                    double latMin = lat - 0.4;

                    double lng = Longitude;
                    double lngMax = lng + 0.4;
                    double lngMin = lng - 0.4;

                    var doc = new MongoDB.Bson.BsonDocument();

                    //"Coordenadas": {$elemMatch: { 0: {$lt: 5000, $gt: -4100},1:{$lt: 5000, $gt: -4100},1:{$lt:5000, $gt:-16.60086}}} } }
                    var coordenadasFilter = new MongoDB.Bson.BsonDocument
                    {
                        {
                            "$elemMatch", new MongoDB.Bson.BsonDocument
                            {
                                {
                                    "0", new MongoDB.Bson.BsonDocument
                                    {
                                        {
                                            "$lte", latMax
                                        },
                                        {
                                            "$gte", latMin
                                        }
                                    }
                                },
                                {
                                    "1", new MongoDB.Bson.BsonDocument
                                    {
                                        {
                                            "$lte", lngMax
                                        },
                                        {
                                            "$gte", lngMin
                                        }
                                    }
                                }
                            }
                        }
                    };

                    doc.Add("Coordenadas", coordenadasFilter);
                    doc.Add("Situacao", 1);

                    using (ConexaoMongoGeography conexaoMongo = new ConexaoMongoGeography())
                    using (var repositoryMongo = new RepositoryMongo<Area_CoberturaMongo>(conexaoMongo))
                    {
                        var collection = repositoryMongo.GetCollection();
                        var query = await collection.FindAsync<Area_CoberturaMongo>(doc);

                        while (await query.MoveNextAsync())
                        {
                            foreach (var documents in query.Current)
                            {
                                var calculo = documents.Coordenadas;
                                retorno.Objeto.Add(documents);
                            }
                        }
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Mapa_Analise_ComercialApi/Get_Objetos_Analise")]
        #region MyRegion
        public async Task<GetApiRetorno<List<IAnalise_RetornoObject>>> Get_Objetos_Analise(FilterRecordsAnalise filters)
        {
            var retorno = new GetApiRetorno<List<IAnalise_RetornoObject>>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessiont5 = HibernateUtil.GetSessionT5())
                {
                    foreach (short? Id_Check in filters.Ids_Check)
                    {
                        switch (Id_Check)
                        {
                            //Areas de Cobertura
                            case 0:
                                {
                                    retorno.Objeto = (await GetAllPoligonosMongo(filters)).Cast<IAnalise_RetornoObject>().ToList();
                                    break;
                                }
                            //Clientes
                            case 1:
                                {
                                    retorno.Objeto = (await Get_Cliente_Analise((session, sessiont5), filters)).Cast<IAnalise_RetornoObject>().ToList();
                                    break;
                                }
                            //Prospects
                            case 2:
                                {
                                    retorno.Objeto = (await Get_Prospecto_Analise((session, sessiont5), filters)).Cast<IAnalise_RetornoObject>().ToList();
                                    break;
                                }

                            //Leads
                            case 3:
                                {
                                    retorno.Objeto = (await Get_Lead_Analise((session, sessiont5), filters)).Cast<IAnalise_RetornoObject>().ToList();

                                    break;
                                }
                            //Elemento_RedeCaixa 
                            case 4:
                                {
                                    retorno.Objeto = (await Get_Elemento_Rede_Analise((session, sessiont5), filters)).Cast<IAnalise_RetornoObject>().ToList();
                                    break;
                                }
                        }
                        retorno.Objeto.ForEach(p => p.Id_Check = Id_Check);

                    }

                    retorno.Ok = true;
                }

            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        #region Methods_Objeto_Analise       
        public async Task<List<Analise_Cliente>> Get_Cliente_Analise((ISession sessionConnect, ISession sessionT5) sessions, FilterRecordsAnalise filter)
        {
            List<Analise_Cliente> retorno = new List<Analise_Cliente>(),
                                  records = new List<Analise_Cliente>();


            IEnumerable<Cliente> cliente;
            try
            {
                using (var com = sessions.sessionConnect.Connection.CreateCommand())
                using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(sessions.sessionConnect))
                {

                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    List<String> condicoes = new List<String>(),
                                 condicoesHaving = new List<String>();

                    StringBuilder sql = new StringBuilder(),
                                  sqlFrom = new StringBuilder(),
                                  sqlWhere = new StringBuilder(),
                                  sqlGroupFields = new StringBuilder(),
                                  sqlHaving = new StringBuilder();

                    /* ---\ Dev Notes \---
                       Campos necessários para essa consulta:
                       CPF_CNPJ, Nome, Tipo_Pessoa, Categoria_Cliente,Sexo
                       Estado_Civil, Grau_Instrucao, Status_prospect, Uf, Cidade,
                       Origem do Cadastro do Lead                                  
                    */
                    #region Filters  

                    condicoes.Add("A.TIPO_CLIENTE = 0");
                    condicoes.Add($"D.STATUS = '{Convert.ToInt16(Status_PlanoProduto.Habilitado)}'");
                    condicoesHaving.Add("(COUNT(D.ID)) > 0 ");

                    if (filter.Uf.HasValue)
                    {
                        parametros.Add("@UF", Convert.ToString(filter.Uf));
                        condicoes.Add(" C.ENDERECO_UF IN (@UF)");
                    }
                    if (filter.Id_Vendedor.HasValue)
                    {
                        parametros.Add("@VENDEDOR", Convert.ToString(filter.Uf));
                        condicoes.Add(" C.ENDERECO_UF = (@VENDEDOR)");
                    }                    
                    if (filter.Status_Caixa.HasValue)
                    {

                    }
                    if (filter.Status_Porta.HasValue)
                    {

                    }
                    if (filter.Meio_Captacao.HasValue)
                    {
                        parametros.Add("@CAPTADO_POR", filter.Meio_Captacao);
                        condicoes.Add("G.ID = @CAPTADO_POR");
                    }
                    if (filter.Campanha.HasValue)
                    {
                        using (var repo_PP = new Conexao<Campanha_PlanoProduto>(sessions.sessionConnect))
                        using (var repo_EV = new Conexao<Campanha_EquipeVendas>(sessions.sessionConnect))
                        using (var repo_R = new Conexao<Campanha_Regioes>(sessions.sessionConnect))
                        {
                            List<Campanha_PlanoProduto> planos = (await repo_PP.select(plan_ => plan_.Campanha.Id == filter.Campanha)).ToList();
                            List<Campanha_EquipeVendas> equipes = (await repo_EV.select(equip_ => equip_.Campanha.Id == filter.Campanha)).ToList();
                            List<Campanha_Regioes> regioes = (await repo_R.select(region => region.Campanha.Id == filter.Campanha)).ToList();

                            condicoes.Add($"E.ID_VENDEDOR IN ({String.Join(",", equipes.Select(p => p.Id_Funcionario))})");
                            condicoes.Add($"D.ID_PLANO IN ({String.Join(",", planos.Select(p => p.Id_Plano_Produto))}) OR " +
                                          $"D.ID_PRODUTO IN ({String.Join(",", planos.Select(p => p.Id_Plano_Produto))})");

                            var estados = regioes.Select(p => p.Estado);

                            if (filter.Uf.HasValue)
                            {
                                parametros["@UF"] = String.Join(",", estados);
                            }
                            else
                            {
                                parametros.Add("@UF", String.Join(",", estados));
                                condicoes.Add(" C.ENDERECO_UF IN (@UF)");
                            }

                        }
                    }
                    if (filter.Filter_Venda != null)
                    {
                        parametros.Add("@COMPARACAO", filter.Filter_Venda.Comparacao == Aniel.T5.Model.enumerado.Comparativo_QuantidadeEnum.Maior_Que ? '>' : '<');
                        parametros.Add("@VALOR", filter.Filter_Venda.Valor);
                        condicoesHaving.Add("SUM(D.VALOR_FINAL_TOTAL) @COMPARACAO @VALOR");

                    }
                    if (!String.IsNullOrEmpty(filter.Nome))
                    {
                        var nome = StringService.Truncate(filter.Nome, 100);

                        parametros.Add("@NOME", nome);
                        condicoes.Add(" A.NOMEFANTASIA LIKE (@NOME) OR A.RAZAO_SOCIAL LIKE(@NOME)");
                    }
                    if (filter.Status_Prospecto.HasValue)
                    {
                        parametros.Add("@STATUS_PROSPECTO", filter.Status_Prospecto);
                        condicoes.Add("A.STATUS_PROSPECTO = @STATUS_PROSPECTO");
                    }
                    if (filter.Cidade.HasValue)
                    {
                        parametros.Add("@CIDADE", filter.Cidade);
                        condicoes.Add(" C.ENDERECO_CIDADE = @CIDADE");
                    }
                    if (filter.Origem_Lead.HasValue)
                    {
                        parametros.Add("@LEAD", filter.Origem_Lead);
                        condicoes.Add("A.ORIGEM_LEAD = @LEAD");
                    }
                    if (filter.Estado_Civil.HasValue)
                    {
                        parametros.Add("@ESTADO_CIVIL", filter.Estado_Civil);
                        condicoes.Add(" A.ESTADO_CIVIL = @ESTADO_CIVIL");
                    }

                    if (filter.Grau_Instrucao.HasValue)
                    {
                        parametros.Add("@GRAU_INSTRUCAO", filter.Grau_Instrucao);
                        condicoes.Add(" A.GRAU_INSTRUCAO = @GRAU_INSTRUCAO");
                    }
                    if (filter.Sexo.HasValue)
                    {
                        parametros.Add("@SEXO", filter.Sexo);
                        condicoes.Add(" A.SEXO = @SEXO");
                    }
                    if (!String.IsNullOrEmpty(filter.Cpf_Cnpj))
                    {
                        var length = filter.Tipo_Pessoa.HasValue && (filter.Tipo_Pessoa == Tipo_Pessoa.Fisica) ? 14 : 18;
                        parametros.Add("@CPF_CNPJ", StringService.Truncate(filter.Cpf_Cnpj, length));
                        condicoes.Add(" A.CPF = @CPF_CNPJ");
                    }
                    if (filter.Categoria_Cliente.HasValue)
                    {
                        parametros.Add("@CATEGORIA_CLIENTE", filter.Categoria_Cliente);
                        condicoes.Add(" A.ID_CATEGORIA = @CATEGORIA_CLIENTE");
                    }
                    #endregion

                    sqlWhere.AppendLine($" WHERE {String.Join(" AND ", condicoes)}");

                    sqlHaving.AppendLine($" HAVING {String.Join(" AND ", condicoesHaving)}");

                    sqlFrom.AppendLine(" FROM TB_CLIENTE A")
                           .AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE B ON B.ID = A.ID_CATEGORIA ")
                           .AppendLine(" LEFT JOIN TB_ENDERECO_CLIENTE C ON C.ID = A.ID_ENDERECO_PRINCIPAL ")
                           .AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO D ON D.ID_CLIENTE = A.ID ")
                           .AppendLine(" LEFT JOIN TB_VENDA E ON E.ID = A.ID ")
                           .AppendLine(" LEFT JOIN TB_MEIO_CAPTACAO G ON G.ID = A.ID_MEIO_CAPTACAO ")
                           .AppendLine(" LEFT JOIN TB_CLIENTE_CONTRATO H ON H.ID_CLIENTE = A.ID ")
                           .AppendLine(" LEFT JOIN TB_CAMPANHA Z ON D.ID_CLIENTE = A.ID ");

                    sqlGroupFields.AppendLine(" A.ID, A.RAZAO_SOCIAL, A.STATUS, A.TIPOPESSOA, A.TIPO_CLIENTE, A.STATUS_PROSPECTO, A.NOMEFANTASIA, ")
                                  .AppendLine(" A.STATUS, C.ENDERECO_LATITUDE, C.ENDERECO_LONGITUDE ");



                    sql.AppendFormat(" SELECT {0}, (COUNT(D.ID)) AS PLANOS_HABILITADOS", sqlGroupFields)
                       .AppendFormat(" {0} \r\n {1} ", sqlFrom, sqlWhere)
                       .AppendFormat(" GROUP BY {0} \r\n {1} \r\n ORDER BY A.ID", sqlGroupFields, sqlHaving);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessions.sessionConnect.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var record = new Analise_Cliente
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = reader["NOMEFANTASIA"] != DBNull.Value ? Convert.ToString(reader["NOMEFANTASIA"]) : null,
                                Latitude = reader["ENDERECO_LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["ENDERECO_LATITUDE"]) : (double?)null,
                                Longitude = reader["ENDERECO_LONGITUDE"] != DBNull.Value ? Convert.ToDouble(reader["ENDERECO_LONGITUDE"]) : (double?)null,
                                Status_Prospecto = reader["STATUS_PROSPECTO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Status_Prospecto)reader["STATUS_PROSPECTO"]) : null,
                                Status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Status_Cliente)reader["STATUS"]) : null,
                                //Status_Conexao = reader["STATUS_CONEXAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Status_Default)reader["STATUS_CONEXAO"]) : null,
                                TipoPessoa = reader["TIPOPESSOA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Tipo_Pessoa)reader["TIPOPESSOA"]) : null,
                                Tipo_Cliente = reader["TIPO_CLIENTE"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Tipo_Cliente)reader["TIPO_CLIENTE"]) : null,
                            };

                            record.Color = getColorHouse(record);
                            records.Add(record);
                        }
                    }
                }

                // SubQuery Conexão
                using (var com = sessions.sessionConnect.Connection.CreateCommand())
                using (var OLT_Api = new OLTApiController())
                {
                    StringBuilder sql = new StringBuilder(),
                                  sqlFrom = new StringBuilder(),
                                  sqlWhere = new StringBuilder();

                    var condicoesSub = new List<String>();

                    var Ids_Cliente = records.Select(obj => { return $"'{Convert.ToString(obj.Id)}'"; }).ToList();

                    condicoesSub.Add($" B.STATUS = '{Convert.ToInt16(Status_PlanoProduto.Habilitado)}'");
                    condicoesSub.Add($"F.ID IN({String.Join(",", Ids_Cliente)})");

                    sqlWhere.AppendLine($" WHERE {String.Join(" AND ", condicoesSub)}");

                    sqlFrom.AppendLine(" FROM TB_ONU A")
                           .AppendLine(" LEFT JOIN TB_PON B ON B.ID = A.ID_PON")
                           .AppendLine(" LEFT JOIN TB_SLOT C ON C.ID = B.ID_SLOT")
                           .AppendLine(" LEFT JOIN TB_OLT D ON D.ID = A.ID_OLT")
                           .AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO E ON E.ID = A.ID_CLIENTE_PLANO_PRODUTO")
                           .AppendLine(" LEFT JOIN TB_CLIENTE F ON F.ID = E.ID_CLIENTE")
                           .AppendLine(" LEFT JOIN TB_LOGIN_RADIUS G ON G.ID = A.ID_LOGIN_RADIUS");

                    sql.AppendLine(" SELECT A.ID, A.LOGIN ,A.SENHA, A.PORTA_TELNET, A.NRO_IP, E.ID AS ID_CLIENTE_PLANO, ")
                       .AppendLine(" F.ID AS ID_CLIENTE, G.ID_CAIXA_FTTH, G.ID_PORTA_FTTH, G.MAC")
                       .AppendFormat("{0} \r\n {1} ORDER BY A.ID", sqlFrom, sqlWhere);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, sessions.sessionConnect.Connection, null))
                    {
                        var RadiusRecord = new List<Analise_Login>();

                        while (await reader.ReadAsync())
                        {

                            var requestTeste = new TestarConexaoRequest
                            {
                                IP = reader["IP"] != DBNull.Value ? Convert.ToString(reader["IP"]) : String.Empty,
                                Login = reader["LOGIN"] != DBNull.Value ? Convert.ToString(reader["LOGIN"]) : String.Empty,
                                PortaSsh = (int?)null,
                                PortaTelnet = reader["PORTA_TELNET"] != DBNull.Value ? Convert.ToInt32(reader["PORTA_TELNET"]) : (int?)null,
                                Senha = reader["SENHA"] != DBNull.Value ? Convert.ToString(reader["SENHA"]) : String.Empty,
                            };

                            var teste = await OLT_Api.TesteConexao(requestTeste);
                            var ApiResults = String.Empty;
                            if (teste.Ok)
                            {
                                ApiResults = teste.Objeto.Any(param => param.Erro) ? RsBase.OFFLINE : RsBase.ONLINE;

                            }
                            else
                            {
                                ApiResults = "SEM_CONEXAO";
                            }

                            RadiusRecord.Add(new Analise_Login
                            {
                                Id_Cliente = Convert.ToInt64(reader["CLIENT_ID"]),
                                Id_PlanoProduto = Convert.ToInt64(reader["ID_PLANO_PRODUTO"]),
                                Mac = reader["MAC"] != DBNull.Value ? Convert.ToString(reader["MAC"]) : null,
                                Caixa_FTTH = reader["CAIXA_FTTH"] != DBNull.Value ? Convert.ToInt64(reader["CAIXA_FTTH"]) : 0,
                                Porta_FTTH = reader["PORTA_FTTH"] != DBNull.Value ? Convert.ToInt64(reader["PORTA_FTTH"]) : 0,
                                Conexao = ApiResults,
                                Color = ApiResults == RsBase.ONLINE ? "green" : ApiResults == RsBase.OFFLINE ? "red" : "gray",
                            });

                        }

                        foreach (var radiusItem in RadiusRecord)
                        {
                            records.Where(p => p.Id == radiusItem.Id_Cliente).ForEach(record =>
                            {
                                record.Radius.Add(radiusItem);
                            });
                        }
                    }

                }

                if (records.Count > 0)
                {
                    using (var comT5 = sessions.sessionT5.Connection.CreateCommand())
                    {

                        StringBuilder sqlTV = new StringBuilder();

                        sqlTV.AppendLine("SELECT A.CAMINHO FROM TB_TIPO_ELEMENTO_ICONE A WHERE A.ID = 52");
                        using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sqlTV, new Dictionary<String, Object>(), sessions.sessionT5.Connection, null, 1, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                // Preenchimento da lista com o  ícone requerido
                                retorno = records.Select(p => { p.Icone_Str = Convert.ToString(reader["CAMINHO"]); return p; }).ToList();
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
                //                              ----\Dev Notes\----
                //      ----\Use Shift + F9 no momento de debug e verá qual o erro gerado.\----
            }

            return retorno;

        }

        public async Task<List<Analise_Cliente>> Get_Prospecto_Analise((ISession sessionConnect, ISession sessionT5) sessions, FilterRecordsAnalise filter)
        {
            var retorno = new List<Analise_Cliente>();
            List<String> condicoes = new List<String>();

            IEnumerable<Cliente> cliente;
            //using (session)
            try
            {
                using (var com = sessions.sessionConnect.Connection.CreateCommand())
                //using (var comT5 = sessions.sessionConnect.Connection.CreateCommand())
                using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(sessions.sessionConnect))
                {

                    Dictionary<String, Object> parameters = new Dictionary<String, Object>();

                    StringBuilder sql = new StringBuilder(),
                                  sqlFrom = new StringBuilder(),
                                  sqlWhere = new StringBuilder(),
                                  sqlTV = new StringBuilder();

                    /* ---\ Dev Notes \---
                       Campos necessários para essa consulta:
                       CPF_CNPJ, Nome, Tipo_Pessoa, Categoria_Cliente,Sexo
                       Estado_Civil, Grau_Instrucao, Status_prospect, Uf, Cidade,
                       Origem do Cadastro do Lead                                  
                    */

                    #region Filters  

                    condicoes.Add($"A.TIPO_CLIENTE = '{Convert.ToInt16(Tipo_Cliente.Prospecto)}'");
                    condicoes.Add($"A.STATUS_PROSPECTO IN('{Convert.ToInt16(Status_Prospecto.Em_Negociacao)}', '{Convert.ToInt16(Status_Prospecto.Inviabilidade_Tecnica)}', '{Convert.ToInt16(Status_Prospecto.Inativo)}')");

                    if (filter.Uf.HasValue)
                    {
                        parameters.Add("@UF", filter.Uf);
                        condicoes.Add(" C.ENDERECO_UF = @UF");
                    }

                    if (filter.Grau_Instrucao.HasValue)
                    {
                        parameters.Add("@GRAU", filter.Grau_Instrucao);
                        condicoes.Add("A.GRAU_INSTRUCAO = @GRAU");

                    }

                    if (!String.IsNullOrEmpty(filter.Nome))
                    {
                        var nome = StringService.Truncate(filter.Nome, 100);

                        parameters.Add("@NOME", nome);
                        condicoes.Add(" A.NOMEFANTASIA LIKE (@NOME) OR A.RAZAO_SOCIAL LIKE(@NOME)");

                    }

                    if (filter.Status_Prospecto != null)
                    {
                        parameters.Add("@STATUS_PROSPECTO", filter.Status_Prospecto);
                        condicoes.Add("A.STATUS_PROSPECTO = @STATUS_PROSPECTO");
                    }

                    if (filter.Cidade != null)
                    {
                        parameters.Add("@CIDADE", filter.Cidade);
                        condicoes.Add(" C.ENDERECO_CIDADE = @CIDADE");
                    }

                    if (filter.Origem_Lead != null)
                    {
                        parameters.Add("@LEAD", filter.Origem_Lead);
                        condicoes.Add("A.ORIGEM_LEAD = @LEAD");
                    }

                    if (filter.Estado_Civil != null)
                    {
                        parameters.Add("@ESTADO_CIVIL", filter.Estado_Civil);
                        condicoes.Add(" A.ESTADO_CIVIL = @ESTADO_CIVIL");
                    }

                    if (filter.Grau_Instrucao != null)
                    {
                        parameters.Add("@GRAU_INSTRUCAO", filter.Grau_Instrucao);
                        condicoes.Add(" A.GRAU_INSTRUCAO = @GRAU_INSTRUCAO");
                    }

                    if (filter.Sexo != null)
                    {
                        parameters.Add("@SEXO", filter.Sexo);
                        condicoes.Add(" A.SEXO = @SEXO");
                    }

                    if (!String.IsNullOrEmpty(filter.Cpf_Cnpj))
                    {
                        var length = filter.Tipo_Pessoa != null && (filter.Tipo_Pessoa == Tipo_Pessoa.Fisica) ? 14 : 18;
                        parameters.Add("@CPF_CNPJ", StringService.Truncate(filter.Cpf_Cnpj, length));
                        condicoes.Add(" A.CPF_CNPJ = @CPF_CNPJ");
                    }

                    if (filter.Categoria_Cliente != null)
                    {
                        parameters.Add("@CATEGORIA_CLIENTE", filter.Categoria_Cliente);
                        condicoes.Add(" A.ID_CATEGORIA_CLIENTE = @CATEGORIA_CLIENTE");
                    }
                    #endregion

                    sqlWhere.AppendLine($" WHERE {String.Join(" AND ", condicoes)}");

                    sqlFrom.AppendLine(" FROM TB_CLIENTE A")
                           .AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE B ON B.ID = A.ID_CATEGORIA")
                           .AppendLine(" LEFT JOIN TB_ENDERECO_CLIENTE C ON C.ID = A.ID_ENDERECO_PRINCIPAL ")
                           .AppendLine(" LEFT JOIN TB_LOGIN_RADIUS D ON D.ID_CLIENTE = A.ID ");

                    sql.AppendLine(" SELECT A.ID, A.RAZAO_SOCIAL, A.STATUS, A.TIPOPESSOA, A.TIPO_CLIENTE, A.STATUS_PROSPECTO, A.NOMEFANTASIA,")
                       .AppendLine(" A.STATUS,C.ENDERECO_LATITUDE, C.ENDERECO_LONGITUDE, D.STATUS AS STATUS_CONEXAO ")
                       .AppendFormat("{0} \r\n {1}", sqlFrom, sqlWhere);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, sessions.sessionConnect.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var record = new Analise_Cliente
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = reader["NOMEFANTASIA"] != DBNull.Value ? Convert.ToString(reader["NOMEFANTASIA"]) : String.Empty,
                                Latitude = reader["ENDERECO_LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["ENDERECO_LATITUDE"]) : (double?)null,
                                Longitude = reader["ENDERECO_LONGITUDE"] != DBNull.Value ? Convert.ToDouble(reader["ENDERECO_LONGITUDE"]) : (double?)null,

                                Status_Prospecto = reader["STATUS_PROSPECTO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Status_Prospecto)reader["STATUS_PROSPECTO"]) : null,
                                Status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Status_Cliente)reader["STATUS"]) : null,
                                //Status_Conexao = reader["STATUS_CONEXAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Status_Default)reader["STATUS_CONEXAO"]) : null,
                                TipoPessoa = reader["TIPOPESSOA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Tipo_Pessoa)reader["TIPOPESSOA"]) : null,
                                Tipo_Cliente = reader["TIPO_CLIENTE"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Tipo_Cliente)reader["TIPO_CLIENTE"]) : null,

                            };
                            retorno.Add(record);
                        }
                    }

                    if (retorno.Count > 0)
                    {
                        using (var comT5 = sessions.sessionT5.Connection.CreateCommand())
                        {
                            sqlTV.AppendLine("SELECT A.CAMINHO FROM TB_TIPO_ELEMENTO_ICONE A WHERE A.ID = 52");
                            using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sqlTV, new Dictionary<String, Object>(), sessions.sessionT5.Connection, null, 1, null))
                            {
                                if (await reader.ReadAsync())
                                {
                                    // Preenchimento da lista com o  ícone requerido
                                    retorno.ForEach(async p =>
                                    {
                                        p.Icone_Str = Convert.ToString(reader["CAMINHO"]);
                                        p.Color = getColorHouse(p);
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //                              ----\Dev Notes\----
                //      ----\Use Shift + F9 no momento de debug e verá qual o erro gerado.\----

            }
            return retorno;
        }

        public async Task<List<Analise_Cliente>> Get_Lead_Analise((ISession sessionConnect, ISession sessionT5) sessions, FilterRecordsAnalise filter)
        {

            List<Analise_Cliente> records = new List<Analise_Cliente>(),
                                  retorno = new List<Analise_Cliente>();


            IEnumerable<Cliente> cliente;
            try
            {
                using (var com = sessions.sessionConnect.Connection.CreateCommand())
                using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(sessions.sessionConnect))
                {

                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    List<String> condicoes = new List<String>();

                    StringBuilder sql = new StringBuilder(),
                                  sqlFrom = new StringBuilder(),
                                  sqlWhere = new StringBuilder(),
                                  sqlGroupFields = new StringBuilder();

                    /* ---\ Dev Notes \---
                       Campos necessários para essa consulta:
                       CPF_CNPJ, Nome, Tipo_Pessoa, Categoria_Cliente,Sexo
                       Estado_Civil, Grau_Instrucao, Status_prospect, Uf, Cidade,
                       Origem do Cadastro do Lead                                  
                    */
                    #region Filters  

                    condicoes.Add("A.TIPO_CLIENTE = '1'");
                    condicoes.Add($"A.STATUS_PROSPECTO IN ('{Convert.ToInt16(Status_Prospecto.Lead_Qualificado)}', '{Convert.ToInt16(Status_Prospecto.Lead_Nao_Qualificado)}')");

                    if (filter.Uf != null)
                    {
                        parametros.Add("@UF", filter.Uf);
                        condicoes.Add(" C.ENDERECO_UF = @UF");
                    }

                    if (!String.IsNullOrEmpty(filter.Nome))
                    {
                        var nome = StringService.Truncate(filter.Nome, 100);

                        parametros.Add("@NOME", nome);
                        condicoes.Add(" A.NOMEFANTASIA LIKE (@NOME) OR A.RAZAO_SOCIAL LIKE(@NOME)");

                    }

                    if (filter.Status_Prospecto != null)
                    {
                        parametros.Add("@STATUS_PROSPECTO", filter.Status_Prospecto);
                        condicoes.Add("A.STATUS_PROSPECTO = @STATUS_PROSPECTO");
                    }

                    if (filter.Cidade != null)
                    {
                        parametros.Add("@CIDADE", filter.Cidade);
                        condicoes.Add(" C.ENDERECO_CIDADE = @CIDADE");
                    }

                    if (filter.Origem_Lead != null)
                    {
                        parametros.Add("@LEAD", filter.Origem_Lead);
                        condicoes.Add("A.ORIGEM_LEAD = @LEAD");
                    }

                    if (filter.Estado_Civil != null)
                    {
                        parametros.Add("@ESTADO_CIVIL", filter.Estado_Civil);
                        condicoes.Add(" A.ESTADO_CIVIL = @ESTADO_CIVIL");
                    }

                    if (filter.Grau_Instrucao != null)
                    {
                        parametros.Add("@GRAU_INSTRUCAO", filter.Grau_Instrucao);
                        condicoes.Add(" A.GRAU_INSTRUCAO = @GRAU_INSTRUCAO");
                    }

                    if (filter.Sexo != null)
                    {
                        parametros.Add("@SEXO", filter.Sexo);
                        condicoes.Add(" A.SEXO = @SEXO");
                    }

                    if (!String.IsNullOrEmpty(filter.Cpf_Cnpj))
                    {
                        var length = filter.Tipo_Pessoa != null && (filter.Tipo_Pessoa == Tipo_Pessoa.Fisica) ? 14 : 18;
                        parametros.Add("@CPF_CNPJ", StringService.Truncate(filter.Cpf_Cnpj, length));
                        condicoes.Add(" A.CPF_CNPJ = @CPF_CNPJ");
                    }

                    if (filter.Categoria_Cliente != null)
                    {
                        parametros.Add("@CATEGORIA_CLIENTE", filter.Categoria_Cliente);
                        condicoes.Add(" A.ID_CATEGORIA_CLIENTE = @CATEGORIA_CLIENTE");
                    }
                    #endregion

                    sqlWhere.AppendLine($" WHERE {String.Join(" AND ", condicoes)}");

                    sqlFrom.AppendLine(" FROM TB_CLIENTE A")
                        .AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE B ON B.ID = A.ID_CATEGORIA ")
                        .AppendLine(" LEFT JOIN TB_ENDERECO_CLIENTE C ON C.ID = A.ID_ENDERECO_PRINCIPAL ")
                        .AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO D ON D.ID_CLIENTE = A.ID ");

                    sqlGroupFields.AppendLine(" A.ID, A.RAZAO_SOCIAL, A.STATUS, A.TIPOPESSOA, A.TIPO_CLIENTE, A.STATUS_PROSPECTO,  A.NOMEFANTASIA, ")
                                  .AppendLine(" A.STATUS, C.ENDERECO_LATITUDE, C.ENDERECO_LONGITUDE ");

                    sql.AppendFormat(" SELECT {0}, (COUNT(D.ID)) AS PLANOS_HABILITADOS ", sqlGroupFields)
                       .AppendFormat(" {0} \r\n {1} ", sqlFrom, sqlWhere)
                       .AppendFormat(" GROUP BY {0} \r\n HAVING (COUNT(D.ID)) > 0 \r\n ORDER BY A.ID", sqlGroupFields);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessions.sessionConnect.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var record = new Analise_Cliente
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = reader["NOMEFANTASIA"] != DBNull.Value ? Convert.ToString(reader["NOMEFANTASIA"]) : null,
                                Latitude = reader["ENDERECO_LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["ENDERECO_LATITUDE"]) : (double?)null,
                                Longitude = reader["ENDERECO_LONGITUDE"] != DBNull.Value ? Convert.ToDouble(reader["ENDERECO_LONGITUDE"]) : (double?)null,
                                Status_Prospecto = reader["STATUS_PROSPECTO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Status_Prospecto)reader["STATUS_PROSPECTO"]) : null,
                                Status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Status_Cliente)reader["STATUS"]) : null,
                                //Status_Conexao = reader["STATUS_CONEXAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Status_Default)reader["STATUS_CONEXAO"]) : null,
                                TipoPessoa = reader["TIPOPESSOA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Tipo_Pessoa)reader["TIPOPESSOA"]) : null,
                                Tipo_Cliente = reader["TIPO_CLIENTE"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription((Tipo_Cliente)reader["TIPO_CLIENTE"]) : null,

                            };

                            records.Add(record);
                        }
                    }
                }

                using (var RadiusApi = new LoginRadiusApiController())
                using (var RadiusRepo = new Conexao<Model.nhibernate.LoginRadius>(HibernateUtil.GetSession()))
                {
                    var Ids_Cliente = records.Select(obj => obj.Id).ToList();
                    List<Model.nhibernate.LoginRadius> ListLogins = (await RadiusRepo.select(p => p.Cliente.Id.IsIn(Ids_Cliente))).OrderBy(p => p.Cliente).ToList();
                }

                if (records.Count > 0)
                {
                    using (var comT5 = sessions.sessionT5.Connection.CreateCommand())
                    {

                        StringBuilder sqlTV = new StringBuilder();

                        sqlTV.AppendLine("SELECT A.CAMINHO FROM TB_TIPO_ELEMENTO_ICONE A WHERE A.ID = 52");
                        using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sqlTV, new Dictionary<String, Object>(), sessions.sessionT5.Connection, null, 1, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                // Preenchimento da lista com o  ícone requerido
                                retorno = records.Select(p =>
                                {
                                    p.Icone_Str = Convert.ToString(reader["CAMINHO"]);
                                    p.Color = getColorHouse(p);
                                    return p;
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //                              ----\Dev Notes\----
                //      ----\Use Shift + F9 no momento de debug e verá qual o erro gerado.\----
            }

            return retorno;
        }

        public async Task<List<Analise_Caixa>> Get_Elemento_Rede_Analise((ISession sessionConnect, ISession sessionT5) sessions, FilterRecordsAnalise filter)
        {
            var retorno = new List<Analise_Caixa>();
            List<String> condicoes = new List<String>();
            IEnumerable<Elemento_Rede> caixas;


            using (var com = sessions.sessionConnect.Connection.CreateCommand())
            using (var comT5 = sessions.sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder(),
                              sqlFrom = new StringBuilder(),
                              sqlWhere = new StringBuilder(),
                              sqlGroupFields = new StringBuilder();

                var parametros = new Dictionary<String, Object>();
                #region Filters

                //Modelo Caixa
                //Status da porta Disponível

                condicoes.Add("D.ID = 7");
                condicoes.Add("E.STATUS = 1");

                //if (filter.Campanha) { }
                //if (filter.Categoria_Cliente) { }
                //if (filter.Cidade) { }
                //if (filter.Cpf_Cnpj) { }
                //if (filter.Estado_Civil) { }
                //if (filter.Grau_Instrucao) { }
                //if (filter.meio_Captacao) { }
                //if (filter.Nome) { }
                //if (filter.Origem_Lead) { }

                //if (filter.Sexo) { }
                //if (filter.) { }


                if (filter.Plano_Produto != null)
                {
                    parametros.Add("@ID_PLANO_PRODUTO", filter.Plano_Produto);
                    condicoes.Add("A.PLANO_PRODUTO = @ID_PLANO_PRODUTO");

                }

                if (filter.Status_Caixa != null)
                {
                    parametros.Add("@STATUS_ELEMENTO", filter.Plano_Produto);
                    condicoes.Add("A.STATUS = @STATUS_ELEMENTO");
                }

                if (filter.Status_Porta != null)
                {
                    parametros.Add("@STATUS_PORTA", filter.Plano_Produto);
                    condicoes.Add("B.SITUACAO_PORTA = @STATUS_PORTA");
                }

                if (filter.Tecnologia != null)
                {
                    parametros.Add("@TECNOLOGIA", filter.Plano_Produto);
                    condicoes.Add("A.TECNOLOGIA = @TECNOLOGIA");
                }

                #endregion

                sqlWhere.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");

                sqlFrom.AppendLine(" FROM TB_ELEMENTO_REDE A ")
                       .AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_REDE B ON B.ID = A.ID_TIPO ")
                       .AppendLine(" LEFT JOIN TB_MODELO_ELEMENTO_REDE D ON D.ID = B.ID_MODELO")
                       .AppendLine(" LEFT JOIN TB_ELEMENTO_REDE_PORTA E ON E.ID_CAIXA = A.ID")
                       .AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_ICONE F ON F.ID = B.ID_ICONE");

                sqlGroupFields.AppendLine(" A.ID, A.LATITUDE, A.LONGITUDE, A.CAPACIDADE, F.CAMINHO, B.DESCRICAO,")
                              .AppendLine(" A.DESCRICAO");

                sql.AppendFormat(" SELECT {0} AS DESCRICAO_ELEMENTO, COUNT (E.ID) AS PORTA_DISPONIVEL {1} \r\n {2} GROUP BY {3} ", sqlGroupFields, sqlFrom, sqlWhere, sqlGroupFields);

                using (var reader = await CommandHelper.ExecuteReader(comT5, sql, sessions.sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        int capacidade = reader["CAPACIDADE"] != DBNull.Value ? Convert.ToInt32(reader["CAPACIDADE"]) : 0,
                        disponiveis = reader["PORTA_DISPONIVEL"] != DBNull.Value ? Convert.ToInt32(reader["PORTA_DISPONIVEL"]) : 0;

                        var record = new Analise_Caixa
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Descricao = reader["DESCRICAO_ELEMENTO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_ELEMENTO"]) : String.Empty,
                            Icone_Str = reader["CAMINHO"] != DBNull.Value ? Convert.ToString(reader["CAMINHO"]) : String.Empty,
                            Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE"]) : (double?)null,
                            Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE"]) : (double?)null,

                            Qtd_PortasTotal = capacidade,
                            Qtd_PortasDisponivel = disponiveis,
                            Color = disponiveis >= 5 ? "green" : disponiveis >= 3 ? "yellow" : disponiveis >= 1 ? "orange" : "red",
                            Tipo = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,

                        };
                        retorno.Add(record);
                    }
                }
            }
            return retorno;
        }

        private string getColorHouse(Analise_Cliente item)
        {
            String retorno = String.Empty;
            var tipos_cliente = Enum.GetNames(typeof(Tipo_Cliente)).ToList();

            if (item?.Tipo_Cliente == tipos_cliente[0]) //Cliente
            {
                retorno = "gray";
                //item.Radius.Conexao = ;
            }
            else if (item?.Tipo_Cliente == RsCadastros.PROSPECTO) //Prospecto
            {
                var tipos_Prospecto = Enum.GetNames(typeof(Status_Prospecto));

                if (item?.Status_Prospecto == RsCadastros.LEAD_NAO_QUALIFICADO) // Lead Não Qualificado
                {
                    retorno = "orange";
                }
                else if (item?.Status_Prospecto == RsCadastros.LEAD_QUALIFICADO) // Lead Qualificado
                {
                    retorno = "green";
                }
                else if (item?.Status_Prospecto == RsCadastros.RESTRICAO_COMERCIAL) // Restricao Comercial
                {
                    retorno = "blue";
                }
                else if (item?.Status_Prospecto == RsCadastros.INVIABILIDADE_TECNICA) // Inviabilidade Técnica
                {
                    retorno = "black";
                }
                else if (item.Status_Prospecto == RsCadastros.INATIVO) // Inativo
                {
                    retorno = "purple";
                }

            }

            return retorno;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Mapa_Analise_ComercialApi/FindCoordenadas")]
        #region FindCoordenadas
        public async Task<GetApiRetorno<Mapa_Analise_ComercialListModel>> FindCoordenadas(Int64 Id)
        {
            var retorno = new GetApiRetorno<Mapa_Analise_ComercialListModel>();

            try
            {

                #region Using

                using (var sessionMongo = new ConexaoMongoGeography())
                using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))
                #endregion
                {
                    Area_CoberturaMongo area = await repoMongo.Find(p => p.Id == Id);

                    if (area == null)
                        throw new Exception("Coordenada não encontrada");

                    retorno.Objeto = new Mapa_Analise_ComercialListModel
                    {

                        ListCoordenadas = area.Coordenadas,
                        //Tipo = area.Tipo,
                        Cor = area.Cor
                    };
                }

                retorno.Ok = true;
                retorno.Mensagem = "OK";

            }
            catch (Exception erro)
            {
                #region Exception
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
                #endregion
            }
            return retorno;
        }
        #endregion


        #region Importação de Leads v1.0
        //[HttpGet()]
        //[Route("api/Mapa_Analise_ComercialApi/Importar_Leads")]
        //public async Task<GetApiRetorno<List<Cliente>>> Importar_Leads(long? Id_Area_Cobertura)
        //{
        //    var retorno = new GetApiRetorno<List<Cliente>>();
        //    List<object> objetos = new List<Object>();
        //    var Coordenadas = new List<List<Double>>();
        //    var Bson = new BsonDocument();

        //    try
        //    {
        //        //trazer todas as áreas de cobertura cadastradas
        //        //trazer todos os endereços dentro das Áreas de Cobertura
        //        //converter todos os endereços em cliente
        //        using (var sessionConnect = HibernateUtil.GetSessionT5())
        //        using (var session = new ConexaoMongoGeography())
        //        using (var repoAreaMongo = new RepositoryMongo<Area_CoberturaMongo>(session))
        //        {
        //            using (var com = sessionConnect.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder();
        //                sql.AppendLine(@" SELECT A.LONGITUDE, A.LATITUDE FROM ENDERECO A ");
        //                using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), sessionConnect.Connection, null))
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var Longitude = Convert.ToDouble(reader["LONGITUDE"]);
        //                        var Latitude = Convert.ToDouble(reader["LATITUDE"]);
        //                        Coordenadas.Add(new List<Double> { Longitude, Latitude });
        //                    }
        //                    dynamic obj = new
        //                    {
        //                        Location = (dynamic)new
        //                        {
        //                            type = "MultiPoint",
        //                            Coordenadas = Coordenadas,
        //                        }
        //                    };
        //                    objetos.Add(obj);
        //                }
        //            }

        //            Area_CoberturaMongo found_Mongo = await repoAreaMongo.Find(p => p.Id == Id_Area_Cobertura);
        //            foreach (Object Objeto in objetos)
        //            {
        //                var collection = session.GetConnection().GetCollection<Area_CoberturaMongo>("Area_CoberturaMongo");
        //                BsonDocument objBson = Objeto.ToBsonDocument();
        //                BsonDocument geometria = new BsonDocument { { "$geometry", objBson } };
        //                BsonDocument geoIntersecao = new BsonDocument { { "$geoIntersects", geometria } };

        //                Bson = new BsonDocument { { "geometry", geoIntersecao } };
        //                var query = await collection.FindAsync<Object>(Bson);
        //                while (await query.MoveNextAsync())
        //                {
        //                    var selected_query = query.Current.Select(prop => prop);
        //                    retorno.Objeto.Add(new Cliente
        //                    {
        //                        Tipo_Cliente = Tipo_Cliente.Prospecto,
        //                        Status_Prospecto = Status_Prospecto.Lead_Não_Qualificado,
        //                    });
        //                }
        //            }
        //        }
        //        retorno.Ok = true;


        //    }
        //    catch (Exception erro)
        //    {
        //        retorno.Ok = false;
        //        retorno.Mensagem = erro.Message;
        //        retorno.InnerException = erro.InnerException.Message;
        //    }
        //    return retorno;
        //}
        #endregion


        
        [HttpPost()]
        [Route("api/Mapa_Analise_ComercialApi/Importar_Leads_Mongo")]
        public async Task<List<PostApiMensagem>> Importar_Leads_Mongo()
        {
            var retorno = new List<PostApiMensagem>();
            var ListaPossiveisLeads = new List<Importacao_LeadMongo>();
            List<Cliente> possibleClients = new List<Cliente>();

            try
            {

                using (var session = HibernateUtil.GetSession())
                using (var conexao = new ConexaoMongo())
                using (var conexaoGepgraphy = new ConexaoMongoGeography())
                using (var repoImport_Leads = new RepositoryMongo<Importacao_LeadMongo>(conexao))
                using (var repositoryStates = new RepositoryMongo<UfMongo>(conexao))
                using (var repositoryArea = new RepositoryMongo<Area_CoberturaMongo>(conexaoGepgraphy))
                using (var repositoryClient = new Conexao<Cliente>(session))
                {

                    //Captura de Areas de Cobertura
                    var Areas = await repositoryArea.FindAll();

                    foreach (var Area in Areas)
                    {

                        try
                        {
                            Dictionary<String, Object> MongoQuery = new Dictionary<String, Object>();
                            MongoQuery.Add("geometry", new
                            {
                                type = Area.Coordenadas.Count == 1 ? "Polygon" : "Multipolygon",
                                coordinates = Area.Coordenadas
                            });

                            var dict = new Dictionary<String, Object>();
                            dict.Add("@POLYPARAM", Area.Coordenadas.Count == 1 ? "'Polygon'" : "'MultiPolygon'");

                            dict.Add("@COORDPARAM", JsonService.SerializeObject(GeoJsonService.ConvertToTridimensionalArray(Area.Coordenadas)));

                            var builderQuery = new StringBuilder();
                            builderQuery.AppendLine("{")
                                        .AppendLine("   geoJson: {")
                                        .AppendLine("       $geoWithin: {")
                                        .AppendLine("           $geometry: {")
                                        .AppendLine("                   type: @POLYPARAM,")
                                        .AppendLine("                   coordinates: @COORDPARAM }")
                                        .AppendLine(@"                  }
                                                           }                                                            
                                              }");

                            var stringQuery = builderQuery.ToString();
                            foreach (var item in dict)
                            {
                                stringQuery = stringQuery.Replace("" + item.Key + "", "" + item.Value + "");

                            }

                            var queryState = repoImport_Leads.GetCollection().Find(stringQuery);
                            if (await queryState.AnyAsync())
                            {
                                await queryState.ForEachAsync(document => ListaPossiveisLeads.Add(document));
                            };

                        }
                        catch (Exception ex)
                        {
                            retorno.Add(new PostApiMensagem { Tipo = PostApiMensagemType.Erro, Mensagem = $"{ex.Message} - {ex?.InnerException?.Message}" });
                        }
                    }

                    possibleClients = AddClientes_Importacao(ListaPossiveisLeads);


                    ListaPossiveisLeads.ForEach(Lead =>
                    {
                        var ClienteinRepo = repositoryClient.selectFirst(cliente => cliente.CpfCnpj == Lead.Numero_Documento).Result;
                        possibleClients.Remove(ClienteinRepo);
                    });


                    await repositoryClient.SaveOrUpdate(possibleClients);

                }
                //if (retorno.Any(message => message.Tipo == PostApiMensagemType.Erro))
                //{
                //    string Erro = retorno.Where(message => message.Tipo == PostApiMensagemType.Erro)
                //                         .Select((message, index) =>
                //                         {
                //                             return $"Erro {index}: {message.Mensagem}";
                //                         }).ToString();

                //    throw new Exception($"{Erro}");
                //}
                //else
                //{
                retorno.Add(new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Info,
                    Mensagem = String.Format(RsCadastros.SUCESSO_IMPORTACAO_LEADS + ", {0} novos leads Importados", possibleClients.Count)
                });

                //}

            }

            catch (Exception ex)
            {
                retorno.Add(new PostApiMensagem { Tipo = PostApiMensagemType.Erro, Mensagem = $"{ex.Message} - {ex?.InnerException?.Message}" });

            }
            return retorno;
        }

        private List<Cliente> AddClientes_Importacao(IEnumerable<Importacao_LeadMongo> listagem)
        {
            var retorno = new List<Cliente>();
            listagem.ForEach(possible =>

                     /*  
             =-=-=-          Dev Notes          =-=-=-
                          Campos Ausentes 
                         Adicionar  Campos               
                      */
                     retorno.Add(new Cliente
                     {
                         CpfCnpj = possible.Numero_Documento,

                         NomeFantasia = possible.Nome,
                         UsuarioCriacao = User.Identity.GetUserId(),
                         Data_Criacao_Cliente = DateTime.Now,
                         Tipo_Cliente = Tipo_Cliente.Prospecto,
                         Status = Status_Cliente.Ativo,
                         Status_Prospecto = Status_Prospecto.Lead_Qualificado,
                         Alterado = false,
                         Tipo_Pessoa = possible.Tipo,



                     }));
            return retorno;
        }
        //Base da importação
        [HttpPost]
        [Route("api/Mapa_Analise_ComercialApi/CPFCNPJ_Service")]
        public void CPFCNPJ_Service()
        {
            List<String> documentos = new List<String>();
            List<Int32> caracteres = new List<Int32>();

            Random random = new Random(),
                   documentGenerator = new Random();

            bool E_cpf = random.NextDouble() >= 0.5;
            int valueDocument = 0;

            if (E_cpf)
            {
                valueDocument = 11;
            }
            else
            {
                valueDocument = 14;
            }

            for (var iterator = 0; iterator < valueDocument; iterator++)
            {
                int valor = documentGenerator.Next(10);
                caracteres.Add(valor);
            }
            //Como eu faço isso??
            while (true)
            {
                documentos.Add(String.Concat(caracteres));
            }

            Importacao_CpfCpnj(documentos);
        }

        private async Task<List<PostApiMensagem>> Importacao_CpfCpnj(IEnumerable<string> documentos)
        {
            var retorno = new List<PostApiMensagem>();
            documentos.ForEach(async documento =>
            {
                try
                {
                    List<char> caracteres = new List<char> { '/', '.', '-' };
                    caracteres.ForEach(caractere => documento = documento.Replace(Convert.ToString(caractere), ""));

                    var url = documento.Length > 11 ? String.Format(CONSTANTES.URLHUBDESENVOLVEDOR_CNPJ, documento) :
                                                      String.Format(CONSTANTES.URLHUBDESENVOLVEDOR_CPF, documento);

                    ResponseHub<ResponseCnpj> resposta = GetByUrl<ResponseHub<ResponseCnpj>>(url);

                    if (resposta.retorno == "NOK")
                    {
                        throw new Exception(RsBase.ERROR);
                    }
                    //Inserção de Registro como possivel lead/prospect/cliente no Mongo
                    using (var addressApi = new EnderecoApiController())
                    using (var conexao = new ConexaoMongo())
                    using (var repositoryImpoLeads = new RepositoryMongo<Importacao_LeadMongo>(conexao))
                    {

                        dynamic objeto = addressApi.GetLatLong(resposta.result.logradouro, resposta.result.numero, "Brasil",
                                                      resposta.result.uf, resposta.result.municipio, resposta.result.bairro);

                        objeto.documento = documento;
                        var record_Impo = this.fillImportacaoObject((Object)objeto);

                        var record = repositoryImpoLeads.Find(p => p.geoJson == record_Impo.geoJson).Result;
                        if (record == null)
                        {
                            await repositoryImpoLeads.Save(record_Impo);
                        }
                        else
                        {
                            await repositoryImpoLeads.Update(null, record);
                        }
                    }
                    retorno.Add(new PostApiMensagem { Tipo = PostApiMensagemType.Info, Mensagem = RsBase.OPERACAOBEMSUCEDIDA });
                }
                catch (Exception ex)
                {
                    retorno.Add(new PostApiMensagem { Tipo = PostApiMensagemType.Erro, Mensagem = ex.Message });
                }

            });
            return retorno;

        }

        private Importacao_LeadMongo fillImportacaoObject(dynamic Objeto)
        {
            Double[,] posicoes = new double[1, 2] { { Objeto.lng, Objeto.lat } };
            var record_Impo = new Importacao_LeadMongo
            {
                Nome = "",
                Tipo = Objeto.documento.Count() > 11 ? Tipo_Pessoa.Juridica : Tipo_Pessoa.Fisica,
                Numero_Documento = Objeto.documento,
                geoJson = new GeoJsonModel { type = "Point", coordinates = posicoes },
            };
            return record_Impo;
        }

        //public async Task<List<PessoaJuridica>> GetListPessoasJuridicas(bool somenteConhecidos)
        //{
        //    //var retorno = new List<PessoaJuridica>();

        //    /*Preciso montar uma forma de capturar os cnpjs para então formular o */
        //    while (true)
        //    {

        //    }

        //}

        private T GetByUrl<T>(String pUrl) where T : new()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, sslPolicyErrors) => true;

            var client = new RestClient(pUrl);
            var request = new RestRequest(RestSharp.Method.POST);
            request.AddHeader("postman-token", "d6a5b694-ec96-c2cd-ff0a-5e9efe83f981");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Accept", "application/json");

            var response = client.Execute<T>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Erro PostByUrl Status: {response.StatusCode} Url: {pUrl} Erro: {response.ErrorMessage} {response.ErrorException}");
            }

            return JsonService.DeserializeObject<T>(response.Content);
        }

    }

    #region Local_Interface
    public interface IMapa_AnaliseComercialApi
    {
        /// <summary>
        /// Api para reserva de Caixa
        /// </summary>
        /// <param name="Id_caixa"></param>
        /// <param name="Porta"></param>
        /// <param name="Cliente_planoProd"></param>
        /// <returns></returns>        
        public Task<GetApiRetorno<ReservarCaixa>> ReservarCaixa(Int64? Id_caixa, Int64? Porta, Int64? Cliente_planoProd);
        /// <summary>
        /// Api para captura de info para Análise
        /// </summary>
        /// <param name="jsonrequest"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<List<PlanoProdutoComboModels>>> GetInfo(String jsonrequest);
        /// <summary>
        /// Api para listagem de polígonos utilizando o Mongo DataBase
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<List<Analise_Area_Cobertura>> GetAllPoligonosMongo(FilterRecordsAnalise filters, Int64? Id);
        /// <summary>
        /// Api Combo para os polígonos utilizando Mongo DataBase
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="Longitude"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<List<Area_CoberturaMongo>>> GetComboPoligonosMongo(double Latitude, double Longitude, Int64? Id);
        /// <summary>
        /// Api para captura de Objetos para o Mapa Análise Comercial
        /// </summary>
        /// <param name="Id_check"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<List<IAnalise_RetornoObject>>> Get_Objetos_Analise(FilterRecordsAnalise filters);
        /// <summary>
        /// Api para encontrar coordenadas
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<Mapa_Analise_ComercialListModel>> FindCoordenadas(Int64 Id);
        /// <summary>
        /// Api apra Importação de Leads pela Area de Cobertura
        /// </summary>        
        /// <returns>Listagem de Mensagens sobre a execução do método</returns>
        [Obsolete("Importação no momento não irá funcionar dessa forma")]
        public Task<List<PostApiMensagem>> Importar_Leads_Mongo();

    }
    #endregion
}