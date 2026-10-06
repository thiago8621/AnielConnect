using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model.ImportacaoAreaCobertura;
using Aniel.Connect.Repository.Repository.ImportacaoAreaCobertura;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Aniel.T5.Model.nhibernate.mongo;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Area_CoberturaApicontroller : BaseApiController
    {
        [HttpGet()]
        [Route("api/Area_CoberturaApi/GetAllPoligonosMongo")]
        public async Task<List<Area_CoberturaMongo>> GetAllPoligonosMongo(Int64? Id = null)
        {
            List<Area_CoberturaMongo> retorno = new List<Area_CoberturaMongo>();

            try
            {
                using (var sessionMongo = new ConexaoMongoGeography())
                using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))
                {
                    if (Id.HasValue)
                        retorno = await repoMongo.FindAll(p => p.Id == Id);
                    else
                        retorno = await repoMongo.FindAll();
                }
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Area_CoberturaApi/PoligonosFindById")]
        public async Task<Area_CoberturaMongo> GetPoligonosMongoFindById(Int64? Id)
        {
            Area_CoberturaMongo retorno = new Area_CoberturaMongo();

            try
            {
                using (var sessionMongo = new ConexaoMongoGeography())
                using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))
                {
                    if (Id.HasValue)
                        retorno = await repoMongo.Find(p => p.Id == Id && p.Situacao == Status_Default.Ativo);
                }
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Area_CoberturaApi/GetPoligonosViabilidade")]
        public async Task<List<Area_CoberturaMongo>> GetPoligonosViabilidade([FromBody] AreaCoberturaRequestIds request)
        {
            List<Area_CoberturaMongo> retorno = new List<Area_CoberturaMongo>();

            try
            {
                using (var sessionMongo = new ConexaoMongoGeography())
                using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))
                {
                    if (request.Ids.Count > 0) 
                    {
                        foreach (var Id in request.Ids) 
                        {
                            retorno.Add(await repoMongo.Find(p => p.Id == Id && p.Situacao == Status_Default.Ativo));
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }

            return retorno;
        }

        // <summary>
        // GetAll das Áreas de Cobertura
        // </summary>
        // <param name="request"></param>
        // <returns></returns>
        [HttpPost]
        [Route("api/Area_CoberturaApi/GetAll")]
        #region GetAll
        public async Task<DataTableResponse<Area_CoberturaListModel>> GetAll(Area_CoberturaDataTableRequest request)
        {
            List<Area_CoberturaListModel> retorno = new List<Area_CoberturaListModel>();
            var record = 0;
            Dictionary<String, Object> parametros = new Dictionary<String, Object>();
            List<string> condicoes = new List<string>();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    #region StringBuilders Padrões
                    StringBuilder sqlFrom = new StringBuilder();

                    sqlFrom.AppendLine(" FROM TB_AREA_COBERTURA A ");

                    StringBuilder sqlWhere = new StringBuilder();

                    if (request.Status != null)
                    {
                        condicoes.Add("WHERE A.SITUACAO = @STATUS");
                        parametros.Add("@STATUS", request.Status);
                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        condicoes.Add("WHERE UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                        parametros.Add("@Search", String.Format("%{0}%", request.search.value));                        
                    }

                    if (condicoes.Count > 0) 
                    {
                        sqlWhere.AppendLine(String.Join(" AND ", condicoes));
                    }
                    #endregion

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sqlCount = new StringBuilder();

                        sqlCount.AppendLine("SELECT COUNT(A.ID) CO ");
                        sqlCount.AppendLine(sqlFrom.ToString());
                        sqlCount.AppendLine(sqlWhere.ToString());
                        sqlCount.AppendLine("GROUP BY A.ID");

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                //record = Convert.ToInt32(reader["CO"]);
                                record++;
                            }
                        }
                    }
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID,A.DESCRICAO,A.SITUACAO");
                        sql.AppendLine(sqlFrom.ToString())
                           .AppendLine(sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Area_CoberturaListModel
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "-",
                                    Status = reader["SITUACAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["SITUACAO"])) : "-",
                                });
                            }

                            return new DataTableResponse<Area_CoberturaListModel>
                            {
                                draw = request.draw,
                                error = "",
                                recordsFiltered = record,
                                data = retorno
                            };
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                return new DataTableResponse<Area_CoberturaListModel> { error = erro.Message };
            }
        }
        #endregion


        /// <summary>
        /// Realiza Persistencia de Dados de Cadastro
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Area_CoberturaApi/FindById")]
        #region FindById
        public async Task<GetApiRetorno<Area_CoberturaCadastroModel>> FindById(Int64? Id)
        {
            GetApiRetorno<Area_CoberturaCadastroModel> retorno = new GetApiRetorno<Area_CoberturaCadastroModel>();
            //Area_Cobertura area = null;
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var session = HibernateUtil.GetSession())
                {

                    using (var RepoCobertura = new Conexao<Area_Cobertura>(sessionT5))
                    using (var RepoCoberturaProd = new Conexao<Area_Cobertura_Plano_Produto>(sessionT5))                    
                    using (var RepoCoberturaCombo = new Conexao<Area_Cobertura_Combo>(sessionT5))                    
                    using (var repoPlanoProd = new Conexao<PlanoProdutoAniel>(sessionT5))                    
                    using (var repoCombo = new Conexao<Combo>(session))
                    
                    //using (var sessionMongo = new ConexaoMongoGeography())
                    //using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))

                    {
                        //area = GetMongoObj(await repoMongo.Find(p => p.Id == Id));
                        retorno.Objeto = GetViewObj(await RepoCobertura.selectFirst(p => p.Id == Id));



                        retorno.Objeto.Produtos = RepoCoberturaProd.select(p => p.Area.Id == retorno.Objeto.Id).Result.Select(p =>
                             new Area_Cobertura_Plano_ProdutoModel
                             {
                                 Id = p.Id,
                                 Descricao = p.Descricao,
                                 Plano_produto = p.Codigo_Plano_Produto,

                                 //Plano_produto = repoPlanoProd.selectFirst(y => y.Id == p.Id).Result?.Codigo,

                             }).ToList();

                        retorno.Objeto.Combos = RepoCoberturaCombo.select(p => p.Area.Id == retorno.Objeto.Id).Result.Select(p =>
                             new Area_Cobertura_ComboModel
                             {
                                 Id = p.Id,
                                 Descricao = p.Descricao,
                                 Combo = p.Codigo_Combo,

                             }).ToList();

                    }
                    retorno.Ok = true;

                }
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;

            }
            return retorno;
        }
        #endregion

        /// <summary>
        /// Realiza Persistencia de Dados de Cadastro
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Area_CoberturaApi/FindAreas")]
        #region FindById
        public async Task<GetApiRetorno<List<Area_CoberturaCadastroModel>>> FindAreas()
        {
            GetApiRetorno<List<Area_CoberturaCadastroModel>> retorno = new GetApiRetorno<List<Area_CoberturaCadastroModel>>();
            retorno.Objeto = new List<Area_CoberturaCadastroModel>();
            var areas = new List<Area_CoberturaCadastroModel>();
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var RepoCobertura = new Conexao<Area_Cobertura>(sessionT5))
                using (var RepoCoberturaProd = new Conexao<Area_Cobertura_Plano_Produto>(sessionT5))                 
                {
                    
                    using (var comm = sessionT5.Connection.CreateCommand()) 
                    {
                    var sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.SITUACAO,A.DESCRICAO,A.COR FROM TB_AREA_COBERTURA A");
                        using (var reader = await CommandHelper.ExecuteReader(comm, sql, sessionT5.Connection, null)) 
                        {
                            while (await reader.ReadAsync()) 
                            {
                                areas.Add(new Area_CoberturaCadastroModel {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Cor = Convert.ToString(reader["COR"]),
                                    Status = (Status_Default)(reader["SITUACAO"]),
                                });                                    
                            }
                        }
                    }
                    
                    foreach (var item in areas)
                    {                            

                        item.Produtos = RepoCoberturaProd.select(p => p.Area.Id == item.Id).Result.Select(p =>
                             new Area_Cobertura_Plano_ProdutoModel
                             {
                                 Id = p.Id,
                                 Descricao = p.Descricao,
                                 Plano_produto = p.Codigo_Plano_Produto,

                                 //Plano_produto = repoPlanoProd.selectFirst(y => y.Id == p.Id).Result?.Codigo,

                             }).ToList();

                        retorno.Objeto.Add(item);
                    }                    
                    retorno.Ok = true;

                }
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;

            }
            return retorno;
        }
        #endregion

        #region GetViewObj

        private Area_CoberturaCadastroModel GetViewObj(Area_Cobertura bancoModelos)
        {
            if (bancoModelos == null)
            {
                return new Area_CoberturaCadastroModel();
            }

            return new Area_CoberturaCadastroModel
            {
                Id = bancoModelos.Id,
                Descricao = bancoModelos.Descricao,
                Status = bancoModelos.Situacao,
                Cor = bancoModelos.Cor,


                //Produtos = ,
            };


        }
        #endregion


        /// <summary>
        /// Grava uma nova Área de Cobertura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Area_CoberturaApi/Gravar")]
        #region Gravar
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Area_CoberturaCadastroModel> request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var sessionMongo = new ConexaoMongoGeography())
                using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))
                {
                    Area_Cobertura cobertura = null;

                    using (var repositorio = new Conexao<Area_Cobertura>(sessionT5))
                    using (var repoAreaCoberturaProd = new Conexao<Area_Cobertura_Plano_Produto>(sessionT5))
                    using (var repoAreaCoberturaCombo = new Conexao<Area_Cobertura_Combo>(sessionT5))

                    {

                        if (request.Objeto.Id != 0)
                        {
                            cobertura = await repositorio.selectFirst(p => p.Id == request.Objeto.Id);
                        }

                        if (cobertura == null)
                        {
                            cobertura = new Area_Cobertura();
                            cobertura.DataHoraCriacao = DateTime.Now;
                            cobertura.UsuarioCriacao = request.Usuario;
                            cobertura.Situacao = Status_Default.Ativo;
                            cobertura.Tipo = Tipo_Poligono.Poligono;

                        }
                        else
                        {
                            cobertura.UsuarioAlteracao = request.Usuario;
                            cobertura.DataHoraAlteracao = DateTime.Now;
                            cobertura.Situacao = request.Objeto.Status;
                            if(request.Objeto.Status == Status_Default.Inativo)
                            {
                                cobertura.Tipo = 0;
                            }
                            else
                            {
                                cobertura.Tipo = Tipo_Poligono.Poligono;
                            }
                        }

                        BindViewToOBJ(cobertura, request.Objeto);

                        var planoProduto = new List<Area_Cobertura_Plano_Produto>();
                        foreach (var produto in request.Objeto.Produtos) 
                        {
                            if (produto.Id != 0|| !produto.Id.HasValue)
                            {
                                planoProduto.Add(new Area_Cobertura_Plano_Produto
                                {
                                    Id = produto.Id.Value,
                                    Descricao = produto.Descricao,
                                    Area = cobertura,
                                    Codigo_Plano_Produto = produto.Plano_produto,
                                });
                            }
                            else 
                            {
                                planoProduto.Add(new Area_Cobertura_Plano_Produto
                                {
                                    Id = 0,
                                    Descricao = produto.Descricao,
                                    Area = cobertura,
                                    Codigo_Plano_Produto = produto.Plano_produto,
                                });
                            }
                        }                           
                        var combos = new List<Area_Cobertura_Combo>();
                        foreach (var combo in request.Objeto.Combos) 
                        {
                            if (combo.Id != 0|| !combo.Id.HasValue)
                            {
                                combos.Add(new Area_Cobertura_Combo
                                {
                                    Id = combo.Id.Value,
                                    Descricao = combo.Descricao,
                                    Area = cobertura,
                                    Codigo_Combo = combo.Combo,
                                });
                            }
                            else 
                            {
                                combos.Add(new Area_Cobertura_Combo
                                {
                                    Id = 0,
                                    Descricao = combo.Descricao,
                                    Area = cobertura,
                                    Codigo_Combo = combo.Combo,
                                });
                            }
                        }                           

                        await repositorio.SaveOrUpdate(cobertura);
                        await repoAreaCoberturaProd.SaveOrUpdate(planoProduto);
                        await repoAreaCoberturaCombo.SaveOrUpdate(combos);


                        Area_CoberturaMongo mongo = await repoMongo.Find(p => p.Id == cobertura.Id);
                        if (mongo == null)
                        {
                            mongo = new Area_CoberturaMongo
                            {
                                Id = cobertura.Id,
                                Tipo = cobertura.Tipo,
                                Cor = cobertura.Cor,
                                Situacao = cobertura.Situacao,
                                Descricao = cobertura.Descricao,
                                UsuarioCriacao = request.Usuario,
                                DataHoraCriacao = DateTime.Now
                            };

                            await repoMongo.Save(mongo);
                        }
                        else
                        {
                            mongo.UsuarioAlteracao = request.Usuario;
                            mongo.DataHoraAlteracao = DateTime.Now;
                            mongo.Tipo = cobertura.Tipo;
                            mongo.Cor = cobertura.Cor;
                            mongo.Situacao = cobertura.Situacao;
                            mongo.Descricao = cobertura.Descricao;
                            await repoMongo.Update(p => p.Id == cobertura.Id, mongo);
                        }

                        retorno.IdObjeto = cobertura.Id;
                        retorno.Ok = true;
                    }


                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = erro.Message;
                retorno.InnerException += erro.InnerException.Message;
            }

            return retorno;
        }
        #endregion

        #region BindViewToOBJ

        private void BindViewToOBJ(Area_Cobertura banco, Area_CoberturaCadastroModel view)
        {
            banco.Descricao = view.Descricao;
            //banco.Situacao = view.Status;
            banco.Cor = view.Cor;
        }

        #endregion

        /// <summary>
        /// Deleta registros por Id da Manutenção
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Area_CoberturaApi/Excluir")]
        #region Excluir
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                Area_Cobertura Area = null;
                List<Area_Cobertura_Plano_Produto> Area_Planos = null;

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoArea_PlPr = new Conexao<Area_Cobertura_Plano_Produto>(sessionT5))
                    using (var repositorio = new Conexao<Area_Cobertura>(sessionT5))
                    {
                        Area = await repositorio.selectFirst(p => p.Id == value.Objeto);
                        Area_Planos = (await repoArea_PlPr.select(prop => prop.Area == Area)).ToList();

                        await repositorio.Remove(Area);
                        await repoArea_PlPr.Remove(Area_Planos);

                        retorno.IdObjeto = Area.Id;
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro?.InnerException?.Message;
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
        [Route("api/Area_CoberturaApi/FindCoordenadas")]
        #region FindCoordenadas
        public async Task<GetApiRetorno<Mapa_Area_CoberturaModel>> FindCoordenadas(Int64 Id)
        {
            var retorno = new GetApiRetorno<Mapa_Area_CoberturaModel>();

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

                    retorno.Objeto = new Mapa_Area_CoberturaModel
                    {
                        Id = area.Id,
                        ListCoordenadas = area.Coordenadas,
                        Tipo = area.Tipo,
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


        /// <summary>
        /// Gravar as Coordenadas de uma Área de Cobertura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Area_CoberturaApi/GravarCoordenadas")]
        #region GravarCoordenadas
        public async Task<PostApiRetorno<Int64>> GravarCoordenadas(Mapa_Area_CoberturaModel request)
        {
            var retorno = new PostApiRetorno<Int64>();

            Area_Cobertura area = null;

            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    #region Using
                    using (var repo = new Conexao<Area_Cobertura>(sessionT5))
                    using (var sessionMongo = new ConexaoMongoGeography())
                    using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))
                    #endregion
                    {
                        area = await repo.selectFirst(p => p.Id == request.Id);
                        if (area == null)
                        {
                            area = new Area_Cobertura
                            {
                                Coordenadas = request.Coordenadas,
                                Tipo = request.Tipo
                            };

                        }

                        area.Coordenadas = request.Coordenadas;
                        area.DataHoraAlteracao = DateTime.Now;

                        await repo.SaveOrUpdate(area);

                        #region Mongo
                        Area_CoberturaMongo mongo = await repoMongo.Find(p => p.Id == area.Id);
                        bool existe = true;
                        if (mongo == null)
                        {
                            existe = false;
                            mongo = new Area_CoberturaMongo
                            {
                                Id = area.Id,
                                Coordenadas = request.ListCoordenadas,
                                Tipo = request.Tipo,
                            };
                        }
                        mongo.Coordenadas = request.ListCoordenadas;
                        mongo.Cor = area.Cor;
                        mongo.Tipo = request.Tipo;
                        mongo.Situacao = area.Situacao;
                        mongo.Descricao = area.Descricao;
                        mongo.DataHoraCriacao = area.DataHoraCriacao;
                        mongo.UsuarioCriacao = area.UsuarioCriacao;


                        if (existe)
                        {
                            mongo.UsuarioAlteracao = User.Identity.GetUserName();
                            mongo.DataHoraAlteracao = DateTime.Now;
                            await repoMongo.Update(p => p.Id == area.Id, mongo);
                        }
                        else
                        {
                            await repoMongo.Save(mongo);
                        }
                        #endregion
                    }

                    #region Retorno
                    retorno.Ok = true;
                    retorno.IdObjeto = request.Id;
                    retorno.Mensagem = "OK";

                    #endregion

                }
            }
            catch (Exception erro)
            {
                #region Exception
                retorno.Ok = false;
                retorno.IdObjeto = 0;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
                #endregion
            }
            return retorno;
        }
        #endregion

        [HttpPost]
        [Route("api/Area_CoberturaApi/Importar")]
        public async Task<PostApiRetorno> Importar()
        {
            var retorno = new PostApiRetorno();

            string tmpPath = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Tmp";

            string tmpKmzFilePath = $@"{tmpPath}\{Guid.NewGuid()}.kmz";
            string extractedKmzDirectoryPath = $@"{tmpPath}\extracted-kmz";

            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ITransaction transactionT5Nh = sessionT5Nh.BeginTransaction())
            using (var repoAreaCobertura = new Conexao<Area_Cobertura>(sessionT5Nh))
            using (var connMongo = new ConexaoMongoGeography())
            using (var repoAreaMongo = new RepositoryMongo<Area_CoberturaMongo>(connMongo))
            {
                try
                {
                    var kmzFile = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                    if (kmzFile != null)
                    {
                        if (!Directory.Exists(tmpPath))
                        {
                            Directory.CreateDirectory(tmpPath);
                        }

                        kmzFile.SaveAs(tmpKmzFilePath);

                        ZipFile.ExtractToDirectory(tmpKmzFilePath, extractedKmzDirectoryPath);

                        string kmlFilePath = $@"{extractedKmzDirectoryPath}\doc.kml";
                        string kmlContent = File.ReadAllText(kmlFilePath);

                        if (!kmlContent.Contains("xmlns:xsi"))
                        {
                            kmlContent = kmlContent.Replace(
                                "<kml",
                                "<kml xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\""
                            );
                        }

                        XDocument docKml = XDocument.Parse(kmlContent);

                        IImportacaoAreaCoberturaRepository repoImportacaoAreaCobertura =
                            new ImportacaoAreaCoberturaKmlRepository(docKml);

                        var importacaoAreaBusiness = new ImportacaoAreaCoberturaBusiness(repoImportacaoAreaCobertura);

                        IEnumerable<AreaCoberturaImportacao> areasImportacao = importacaoAreaBusiness.GetDadosAreaCobertura();

                        Directory.Delete(extractedKmzDirectoryPath, true);
                        File.Delete(tmpKmzFilePath);

                        if (areasImportacao != null && areasImportacao.Count() > 0)
                        {
                            foreach (AreaCoberturaImportacao areaImportacao in areasImportacao)
                            {
                                var areaCobertura = new Area_Cobertura();

                                areaCobertura.UsuarioCriacao = User.Identity.GetUserId();
                                areaCobertura.DataHoraCriacao = DateTime.Now;

                                areaCobertura.Descricao = areaImportacao.Name;
                                areaCobertura.Tipo = Tipo_Poligono.Poligono;
                                areaCobertura.Situacao = Status_Default.Ativo;

                                // Tirando os últimos 2 dígitos da cor RGBA para transformar a cor
                                // no formato RGB usado pelo Connect
                                areaCobertura.Cor = StringService.Truncate(areaImportacao.Color, 7);

                                if (areaImportacao.Coordinates != null && areaImportacao.Coordinates.Count() > 0)
                                {
                                    IEnumerable<string> listLatLong = 
                                        areaImportacao
                                            .Coordinates
                                            .Select(c => String.Format(
                                                            "{0},{1}",
                                                            c.Latitude.ToString().Replace(",", "."),
                                                            c.Longitude.ToString().Replace(",", ".")
                                                        )
                                            );

                                    areaCobertura.Coordenadas = "[[" + String.Join(",", listLatLong) + "]]";
                                }

                                await repoAreaCobertura.Save(areaCobertura);

                                List<List<double?>> coordenadasMongoDb = null;

                                if (!String.IsNullOrEmpty(areaCobertura.Coordenadas))
                                {
                                    coordenadasMongoDb = JsonService.DeserializeObject<List<List<double?>>>(areaCobertura.Coordenadas);
                                }

                                var areaCoberturaMongo = new Area_CoberturaMongo
                                {
                                    Id = areaCobertura.Id,
                                    
                                    DataHoraCriacao = DateTime.Now,
                                    UsuarioCriacao = areaCobertura.UsuarioCriacao,

                                    Descricao = areaCobertura.Descricao,
                                    Situacao = areaCobertura.Situacao,
                                    Tipo = areaCobertura.Tipo,
                                    
                                    Cor = areaCobertura.Cor,
                                    Coordenadas = coordenadasMongoDb
                                };

                                await repoAreaMongo.Save(areaCoberturaMongo);
                            }
                        }
                    }

                    transactionT5Nh.Commit();
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;

                    if (File.Exists(tmpKmzFilePath))
                    {
                        File.Delete(tmpKmzFilePath);
                    }

                    if (Directory.Exists(extractedKmzDirectoryPath))
                    {
                        Directory.Delete(extractedKmzDirectoryPath, true);
                    }

                    if (transactionT5Nh.IsActive)
                    {
                        transactionT5Nh.Rollback();
                    }
                } 
            }

            return retorno;
        }


        /// <summary>
        /// Delete Area de Cobertura
        /// </summary>
        /// <param name="Idplano"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("api/Area_CoberturaApi/DeletePlano")]
        #region DeletePlano
        public async Task<DeleteApiRetorno<Int64>> Delete(Int64 Idplano)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            Area_Cobertura_Plano_Produto plano = null;
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repositorio = new Conexao<Area_Cobertura_Plano_Produto>(sessionT5))
                    {

                        if (Idplano != 0)
                        {
                            plano = await repositorio.selectFirst(p => p.Id == Idplano);
                            await repositorio.Remove(plano);
                            retorno.Ok = true;
                            retorno.IdObjeto = plano.Id;

                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }

                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }

            return retorno;
        }
        #endregion

        /// <summary>
        /// Delete Area de Cobertura
        /// </summary>
        /// <param name="IdCombo"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("api/Area_CoberturaApi/DeleteCombo")]
        #region DeleteCombo
        public async Task<DeleteApiRetorno<Int64>> DeleteCombo(Int64 IdCombo)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            Area_Cobertura_Combo combo = null;
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repositorio = new Conexao<Area_Cobertura_Combo>(sessionT5))
                    {

                        if (IdCombo != 0)
                        {
                            combo = await repositorio.selectFirst(p => p.Id == IdCombo);
                            await repositorio.Remove(combo);
                            retorno.Ok = true;
                            retorno.IdObjeto = combo.Id;

                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }

                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }

            return retorno;
        }
        #endregion


        /// <summary>
        /// Alterar o Status de uma Área de Cobertura
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Area_CoberturaApi/AlterarStatus")]
        #region AlterarStatus
        public async Task<PostApiRetorno<Area_CoberturaListModel>> AlterarStatus(AlterarStatusViewModel request)
        {
            PostApiRetorno<Area_CoberturaListModel> retorno = new PostApiRetorno<Area_CoberturaListModel>();
            Tipo_Poligono statusPoligono = 0;

            if (request.status == 1)
            {
                statusPoligono = Tipo_Poligono.Poligono;
            }

            try
            {
                Dictionary<String, object> parametros = new Dictionary<string, Object>();

                string ids = String.Join(",", request.ids);
                using (var session = HibernateUtil.GetSessionT5())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder($"UPDATE TB_AREA_COBERTURA SET SITUACAO=@STATUS, TIPO_POLIGONO=@TIPO_POLIGONO WHERE ID IN ({ids})");
                        parametros.Add("@STATUS", request.status);
                        parametros.Add("@TIPO_POLIGONO", statusPoligono);
                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }
                }

                using (var sessionMongo = new ConexaoMongoGeography())
                using (var repoMongo = new RepositoryMongo<Area_CoberturaMongo>(sessionMongo))
                {
                    foreach(var id in request.ids)
                    {
                        Area_CoberturaMongo mongo = await repoMongo.Find(p => p.Id == id);
                        if(mongo != null)
                        {                          
                            mongo.Tipo = statusPoligono;
                            mongo.Situacao = (Status_Default)request.status;
                            await repoMongo.Update(p => p.Id == id, mongo);
                        }
                    }
                }
                    retorno.Ok = true;
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

        /// <summary>
        /// GetTreeView das Áreas de Cobertura
        /// </summary>
        /// <param name="Id_Area"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("api/Area_CoberturaApi/GetAreaTreeView")]

        #region GetAreaTreeView

        public async Task<GetApiRetorno<List<Area_CoberturaTreeViewModel>>> GetAreaTreeView()
        {
            GetApiRetorno<List<Area_CoberturaTreeViewModel>> retorno = new GetApiRetorno<List<Area_CoberturaTreeViewModel>>
            {
                Objeto = new List<Area_CoberturaTreeViewModel>()
            };

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            try
            {
                StringBuilder sqlFrom = new StringBuilder(@"
                                                    FROM TB_AREA_COBERTURA A ");

                using (var session = HibernateUtil.GetSessionT5())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT  A.ID, A.DESCRICAO");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Area_CoberturaTreeViewModel
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"])

                                };

                                retorno.Objeto.Add(obj);
                            }
                        }
                    }
                }
                retorno.Ok = true;

            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.Message != null ? erro.InnerException.Message : null;
            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Area_Coberturaapi/GetPlanoByArea_Cobertura")]
        #region GetPlanoByArea_Cobertura
        public async Task<PostApiRetorno<Int64>> GetPlanoByArea_cobertura(Double? request)
        {
            PostApiRetorno<long> retorno = new PostApiRetorno<long>();
            try
            {
                using (var SessionMongo = new ConexaoMongo())
                using (var AreaMongo = new RepositoryMongo<Area_CoberturaMongo>(SessionMongo))
                {
                    var areas = await AreaMongo.Find(p => p.Latitude == request);
                    if (areas == null)
                    {
                        throw new Exception("Area de Cobertura Não Encontrada");
                    }
                    retorno.IdObjeto = areas.Id;
                    retorno.Ok = true;

                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.IdObjeto = 0;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Area_Coberturaapi/GetCombo")]
        public async Task<ObjCombo<long>> GetCombo(RequestObjCombo<long?> request)
        {
            ObjCombo<long> retorno = new ObjCombo<long>
            {
                results = new List<IObjComboResult<long>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_AREA_COBERTURA A ");

                    //sqlFrom.AppendLine(" WHERE A.SECAO_CONNECT = 1 AND A.TIPO_USUARIO = 2 ");
                    //.AppendLine(" AND USUARIO_MONITOR IS NOT NULL ");

                    List<String> condicoes = new List<String>();
                    if (request.soativos)
                    {
                        //condicoes.Add($"A.SITUACAO IN (0,4)");
                    }

                    if (request.id != null)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 6)}%'");
                    }

                    if (condicoes.Count() > 0)
                    {
                        //sqlFrom.AppendLine($"AND ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder(" SELECT A.ID, A.DESCRICAO");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.ID");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<long>
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
        [HttpPost]
        [Route("api/Area_CoberturaApi/Get_ElementosInArea")]
        public async Task<IEnumerable<Elemento_RedeMongo>> GetElementosInArea(Area_CoberturaMongo AC)
        {
            List<Elemento_RedeMongo> elements = new List<Elemento_RedeMongo>();
            List<Location> coordenadasCobertura = new List<Location>();

            try 
            {
                using (var GeographyHandler = new GeographyPattern())
                using (var conexao = new ConexaoMongoGeography())
                using (var repoElement = new RepositoryMongo<Elemento_RedeMongo>(conexao))
                using (var repoArea = new RepositoryMongo<Area_CoberturaMongo>(conexao))
                {
                    var listaElementos = await repoElement.FindAll();

                    foreach (var coordenada in AC.Coordenadas)
                    {

                        for (var latlong = 0; latlong < coordenada.Count(); latlong++)
                        {
                            double locLat = coordenada[latlong].HasValue ? coordenada[latlong].Value : 0,
                                    locLong = coordenada[latlong].HasValue ? coordenada[++latlong].Value : 0;

                            coordenadasCobertura.Add(new Location(locLat, locLong));
                        }

                    }

                    foreach (var elemento in listaElementos.Where(param => param.Latitude != null && param.Longitude != null))
                    {
                        var point = new Location(elemento.Latitude.Value, elemento.Longitude.Value);
                        if (GeographyHandler.IsPointInPolygon(coordenadasCobertura, point))
                        {
                            elements.Add(elemento);
                        }
                    }
                }
            }
            catch (Exception erro) 
            {
                Console.WriteLine($"{erro.Message}");
            }
            
            return elements;
        }
    }

}