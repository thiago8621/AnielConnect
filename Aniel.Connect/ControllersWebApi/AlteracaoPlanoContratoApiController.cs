using Aniel.Connect.ConnectBusiness;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.AlteracaoPlanoContrato;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Venda;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    public class AlteracaoPlanoContratoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/AlteracaoPlanoContratoapi/GetAll")]
        public async Task<DataTableResponse<AlteracaoPlanoContratoListModel>> GetAll(AlteracaoPlanoContratoDataTableRequestModel request)
        {
            #region [GetAll]
            DataTableResponse<AlteracaoPlanoContratoListModel> retorno = new DataTableResponse<AlteracaoPlanoContratoListModel>();
            AlteracaoPlanoContratoListResponseModel businessRetorno = new AlteracaoPlanoContratoListResponseModel();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var sessionLog = HibernateUtil.GetSessionLog())
                {
                    IAlteracaoPlanoContratoRepository repository = new AlteracaoPlanoContratoRepository(session, sessionT5, sessionLog);
                    AlteracaoPlanoContratoBusiness business = new AlteracaoPlanoContratoBusiness(repository);

                    businessRetorno = await business.GetAll(request);
                }
            }
            catch (Exception ex)
            {
                retorno.error = String.IsNullOrEmpty(ex.InnerException.Message) ? ex.Message : $"{ex.Message} - {ex.InnerException.Message}";
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            finally
            {
                retorno.draw = request.draw;
                retorno.recordsFiltered = Convert.ToInt32(businessRetorno.RecordsFiltered);
                retorno.data = businessRetorno.Records;
            }
            return retorno;
            #endregion
        }

        [HttpGet()]
        [Route("api/AlteracaoPlanoContratoapi/FindById")]
        public async Task<GetApiRetorno<AlteracaoPlanoContratoCadastroModel>> FindById(long IdVenda)
        {
            #region [FindById]
            var retorno = new GetApiRetorno<AlteracaoPlanoContratoCadastroModel> { Objeto = new AlteracaoPlanoContratoCadastroModel() };
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var sessionLog = HibernateUtil.GetSessionLog())
                using (var api = new VendaApiController())
                using (var repo = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoEtapaVenda = new Conexao<EtapaVenda>(session))
                {
                    IAlteracaoPlanoContratoRepository repository = new AlteracaoPlanoContratoRepository(session, sessionT5, sessionLog);
                    IVendaRepository repositoryVenda = new VendaRepository(session);
                    IClienteRepository repositoryCliente = new ClienteRepository(session);
                    var business = new AlteracaoPlanoContratoBusiness(repository);
                    var businessVenda = new VendaBusiness(repositoryVenda);


                    var venda = businessVenda.GetVendaById(IdVenda);
                    Cliente cliente = repositoryCliente.GetById(venda.Id_Cliente.Value);
                    Cliente_PlanoProduto cpp = businessVenda.GetPlanosProdutosVenda(venda.Id).FirstOrDefault();
                    var etapaTermo = await repoEtapaVenda.selectFirst(p => p.Venda == venda && p.Etapa == Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao);
                    var etapaFinanceira = await repoEtapaVenda.selectFirst(p => p.Venda == venda && p.Etapa == Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaPendenciaFinanceira);
                    var etapaCriacaoOs = await repoEtapaVenda.selectFirst(p => p.Venda == venda && p.Etapa == Sinapse.Model.enumerados.EnumEtapaVenda.OrdemServico);

                    retorno.Objeto.IdVenda = venda.Id;
                    retorno.Objeto.Status = HtmlEnumExtensions.GetEnumDescription(venda.Status);
                    retorno.Objeto.Status_Venda = venda.Status;
                    retorno.Objeto.DataCriacao = venda.DataHoraCriacao.Value.ToString("dd/MM/yyyy");
                    retorno.Objeto.Id_Vendedor = venda.Vendedor.Id;
                    retorno.Objeto.Id_Supervisor = venda.Supervisor.Id;
                    retorno.Objeto.Id_Departamento = venda.Departamento.Id;
                    retorno.Objeto.IdPlanoProduto = cpp?.Plano?.Id;
                    retorno.Objeto.PlanoProduto = business.GetPlanoProdutoListModelByVenda(cpp, venda);
                    retorno.Objeto.Id_Cliente = cliente?.Id;

                    retorno.Objeto.EtapaTermoAdesao = etapaTermo?.Status != null ? etapaTermo?.Status : StatusEtapaVenda.Pendente;
                    retorno.Objeto.EtapaPendenciaFinanceira = etapaFinanceira?.Status != null ? etapaFinanceira?.Status : StatusEtapaVenda.Pendente;
                    retorno.Objeto.NecessitaCriarOS = true;
                    retorno.Objeto.EtapaCriacaoOS = etapaCriacaoOs != null ? etapaCriacaoOs?.Status : StatusEtapaVenda.Pendente;
                    //retorno.Objeto.VendaBase = (await api.FindById(venda.Id, venda.Id_Cliente)).Objeto;
                    //retorno.Objeto.IdPlanoAnterior = retorno.Objeto.VendaBase.Planos.Where(p=>p.Id == cpp.Id_TrocaPlano).FirstOrDefault()?.Id;

                    //Todo
                    if (cpp != null)
                    {
                        var id_troca_plano = venda.Status == Status_Venda.Venda_Cancelada ? cpp?.Id_TrocaPlanoCancelados : cpp?.Id_TrocaPlano;
                        var cppAnterior = await repo.selectFirst(p => p.Id == id_troca_plano);
                        if (cppAnterior != null)
                        {
                            retorno.Objeto.VendaBase = (await business.DuplicarVendaByContratoPlano(cppAnterior.Cliente.Id, cppAnterior.ClienteContrato.Id, cppAnterior.Plano.Id, User.Identity.GetUserId(), venda.Id)).Objeto;
                            retorno.Objeto.IdPlanoAnterior = cppAnterior.Plano.Id;
                        }

                        using (Documento_ProducaoApiController apiDocProd = new Documento_ProducaoApiController())
                        {
                            int idClienteAniel = (await apiDocProd.GetClienteAniel(sessionT5, cliente)).Cod_Cliente;
                            long? idPlanoProdutoAniel = (await apiDocProd.GetPlanoProdutoAniel(sessionT5, cpp)).FirstOrDefault()?.Id;

                            Documento_ProducaoAniel planoProdutoOS = (await repoDocProd.selectFirst(p => p.Id_Cliente_PlanoProdutoConnect == cpp.Id &&
                                                                                                    p.Id_Venda == venda.Id));

                            retorno.Objeto.numeroOS = planoProdutoOS?.Numero_Obra_Original;
                            retorno.Objeto.cod_ct = planoProdutoOS?.CodCt;
                            retorno.Objeto.projeto = planoProdutoOS?.Projeto;
                            retorno.Objeto.num_obra = planoProdutoOS?.Num_Obra;
                            retorno.Objeto.num_doc = planoProdutoOS?.Num_Doc;
                            retorno.Objeto.EtapaCriacaoOS = planoProdutoOS != null ? Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado : Sinapse.Model.enumerados.StatusEtapaVenda.Pendente;
                        }
                    }

                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = null;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
            #endregion
        }
        [HttpPost()]
        [Route("api/AlteracaoPlanoContratoapi/Gravar")]
        public async Task<PostApiRetorno<long>> Gravar([FromBody] ParamWebApi<AlteracaoPlanoContratoCadastroModel> request)
        {
            #region [Gravar]
            var retorno = new PostApiRetorno<long>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var sessionLog = HibernateUtil.GetSessionLog())
                {
                    IAlteracaoPlanoContratoRepository repository = new AlteracaoPlanoContratoRepository(session, sessionT5, sessionLog);
                    AlteracaoPlanoContratoBusiness business = new AlteracaoPlanoContratoBusiness(repository);

                    request.Objeto.NecessitaCriarOS = await business.GetValidacaoCriacaoOS(request.Objeto.IdPlanoAnterior.Value, request.Objeto.IdPlanoProduto.Value);
                    retorno.IdObjeto = await business.Gravar(request.Objeto, User.Identity.GetUserId(), User.Identity.GetUserName());
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = String.IsNullOrEmpty(ex.InnerException?.Message) ? ex.Message : $"{ex.Message} - {ex.InnerException?.Message}";
                retorno.InnerException = String.IsNullOrEmpty(ex.InnerException?.Message) ? "" : ex.InnerException?.Message;

                retorno.IdObjeto = 0;
                retorno.Ok = false;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
            #endregion
        }
        [HttpPost()]
        [Route("api/AlteracaoPlanoContratoapi/GetComboContrato")]
        public async Task<ObjCombo<Int64>> GetComboContrato(RequestObjComboAvo<long?, long?, long?> request)
        {
            #region [GetComboContrato]
            var retorno = new ObjCombo<long> { results = new List<IObjComboResult<long>>() };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionT5())
                {
                    IAlteracaoPlanoContratoRepository repository = new AlteracaoPlanoContratoRepository(session, sessionT5, sessionLog);
                    var business = new AlteracaoPlanoContratoBusiness(repository);

                    retorno.results = await business.GetComboContrato(request);
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                throw ex;
            }

            return retorno;
            #endregion
        }

        [HttpPost()]
        [Route("api/AlteracaoPlanoContratoapi/GetComboPlanoTecnologia")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPaiConfig<long?, long?, Dictionary<string, object>> request)
        {
            #region [GetComboPlanoTecnologia]
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    var idsPlanoArea = new List<long>();

                    StringBuilder sqlFromPlano = new StringBuilder("FROM TB_PLANO A ")
                        .AppendLine(" LEFT JOIN TB_TECNOLOGIA C ON A.ID_TECNOLOGIA = C.ID");

                    #region Filtros

                    List<String> condicoes = new List<String>();

                    if (request.config != null && request.config.ContainsKey("AREACOBERTURA"))
                    {
                        using (var comT5 = sessionT5.Connection.CreateCommand())
                        {
                            StringBuilder sql = new StringBuilder($"SELECT A.COD_PLANO_PRODUTO FROM TB_AREA_COBERTURA_PLANO_PROD A")
                                                      .AppendLine($"WHERE A.ID_AREA_COBERTURA IN({String.Join(", ", request.config["AreaCobertura"])})")
                                                      .AppendLine("ORDER BY UPPER(A.DESCRICAO)");
                            using (var reader = await CommandHelper.ExecuteReader(comT5, sql, sessionT5.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    var planoProduto = Convert.ToString(reader["COD_PLANO_PRODUTO"]);
                                    idsPlanoArea.Add(long.Parse(planoProduto));
                                }
                            }
                        }
                    }
                    if (request.soativos)
                    {
                        condicoes.Add($"A.SITUACAOPLANO = 1 ");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request.config != null)
                    {
                        if (request.config.ContainsKey("TECNOLOGIA"))
                        {
                            var tecnologia = Convert.ToString(request.config["TECNOLOGIA"]);
                            if (!String.IsNullOrEmpty(tecnologia))
                            {
                                condicoes.Add($"A.TIPO_TECNOLOGIA = {tecnologia}");
                            }
                        }
                        if (request.config.ContainsKey("AREACOBERTURA") && idsPlanoArea.Count > 0)
                        {
                            condicoes.Add($"A.ID IN ({String.Join(",", idsPlanoArea)})");
                        }
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.CODIGO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 20)}%' OR " +
                                       $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFromPlano.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    #endregion
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFromPlano.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var comConnect = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.CODIGO, A.DESCRICAO, A.VALORPLANO, A.ID_MODELOCONTRATO, A.AUTENTICACAORADIUS, A.VALORINSTALACAO, ")
                            .AppendLine(" A.VALORINSTALACAOFINAL, A.TESTEGRATIS, A.VALORPLANOFINAL, A.DIASTESTE, A.PERIODO_CARENCIA, A.QTDPARCELAS, A.STATUSINICIAL, ")
                            .AppendLine("A.DETALHESPLANO, A.VELOCIDADEDOWNLOAD, A.VELOCIDADEDOWNLOADREDUZIDA, A.VELOCIDADEUPLOAD, A.TIPO_TECNOLOGIA, ")
                            .AppendLine("A.VELOCIDADEUPLOADREDUZIDA, A.VELOCIDADENOMINAL");

                        sql.AppendLine(sqlFromPlano.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(comConnect, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new PlanoProdutoComboModels
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    ValorPlanoProduto = reader["VALORPLANO"] != DBNull.Value ? Convert.ToDouble(reader["VALORPLANO"]).ToString("N2") : "0.00",
                                    Id_Modelo_Contrato = reader["ID_MODELOCONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["ID_MODELOCONTRATO"]) : (Int64?)null,
                                    AutenticacaoRadius = reader["AUTENTICACAORADIUS"] != DBNull.Value ? Convert.ToBoolean(reader["AUTENTICACAORADIUS"]) : false,
                                    ValorInstalacao = reader["VALORINSTALACAO"] != DBNull.Value ? Convert.ToDouble(reader["VALORINSTALACAO"]).ToString("N2") : "0.00",
                                    ValorInstalacao_Final = reader["VALORINSTALACAOFINAL"] != DBNull.Value ? Convert.ToDouble(reader["VALORINSTALACAOFINAL"]).ToString("N2") : "0.00",
                                    Tipo = PlanoProdutoEnum.Plano,
                                    //Id_Tecnologia = reader["ID_TECNOLOGIA"] != DBNull.Value ? Convert.ToInt64(reader["ID_TECNOLOGIA"]): (long?)null,
                                    Tipo_Tecnologia = Convert.ToInt32(reader["TIPO_TECNOLOGIA"]),
                                    Tipo_TecnologiaStr = HtmlEnumExtensions.GetEnumDescription<Tipo_Tecnologia>(Convert.ToString(reader["TIPO_TECNOLOGIA"])),
                                    Teste_Gratis = reader["TESTEGRATIS"] != DBNull.Value ? Convert.ToBoolean(reader["TESTEGRATIS"]) : false,
                                    ValorPlanoProduto_Final = reader["VALORPLANOFINAL"] != DBNull.Value ? Convert.ToDouble(reader["VALORPLANOFINAL"]).ToString("N2") : "0.00",
                                    Qtde_Dias = reader["DIASTESTE"] != DBNull.Value ? Convert.ToInt64(reader["DIASTESTE"]) : (Int64?)null,
                                    Periodo_Carencia = reader["PERIODO_CARENCIA"] != DBNull.Value ? Convert.ToBoolean(reader["PERIODO_CARENCIA"]) : false,

                                    Numero_Parcelas = reader["QTDPARCELAS"] != DBNull.Value ? Convert.ToInt64(reader["QTDPARCELAS"]) : (Int64?)null,
                                    Status_PlanoProduto = reader["STATUSINICIAL"] != DBNull.Value ? (Status_PlanoProduto)Enum.Parse(typeof(Status_PlanoProduto), reader["STATUSINICIAL"].ToString()) : (Status_PlanoProduto?)null,
                                    Detalhes = Convert.ToString(reader["DETALHESPLANO"]),
                                    VelocidadeDownload = Convert.ToString(reader["VELOCIDADEDOWNLOAD"]),
                                    VelocidadeDownloadReduzida = Convert.ToString(reader["VELOCIDADEDOWNLOADREDUZIDA"]),
                                    VelocidadeUpload = Convert.ToString(reader["VELOCIDADEUPLOAD"]),
                                    VelocidadeUploadReduzida = Convert.ToString(reader["VELOCIDADEUPLOADREDUZIDA"]),
                                    VelocidadeNominal = Convert.ToString(reader["VELOCIDADENOMINAL"]),
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
            #endregion
        }
        [HttpGet]
        [Route("api/AlteracaoPlanoContratoapi/GetVendaByClienteContratoPLano")]
        public async Task<GetApiRetorno<AlteracaoPlanoContratoCadastroModel>> GetVendaByClienteContratoPlano(long IdCliente, long? IdContrato, long? IdPlano, long? IdClientePlanoProduto)
        {
            #region [GetVendaByClienteContrato]
            var retorno = new GetApiRetorno<AlteracaoPlanoContratoCadastroModel> { Objeto = new AlteracaoPlanoContratoCadastroModel() };
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            {
                var usuario = new Conexao<IdentityUser>(session).selectFirst(p => p.Id == User.Identity.GetUserId()).Result;
                IAlteracaoPlanoContratoRepository repository = new AlteracaoPlanoContratoRepository(session, sessionT5, sessionLog);
                var business = new AlteracaoPlanoContratoBusiness(repository);
                var idUsuario = User.Identity.GetUserId();
                if (IdContrato.HasValue && IdPlano.HasValue)
                {
                    /* Todo: Não preencher plano aqui, mas sim no cadastro, já que faremos a pesquisa não por venda, 
                     * que preencheria o plano, mas sim por cliente e contrato */
                    retorno.Objeto.VendaBase = (await business.DuplicarVendaByContratoPlano(IdCliente, IdContrato.Value, IdPlano.Value, idUsuario))?.Objeto;
                }
                else if (IdContrato.HasValue)
                {
                    retorno.Objeto.VendaBase = (await business.DuplicarVendaByContrato(IdCliente, IdContrato.Value, idUsuario))?.Objeto;
                }

                if (IdClientePlanoProduto > 0)
                {
                    retorno.Objeto.VendaBase.Planos = retorno.Objeto.VendaBase.Planos.Where(p => p.Id == IdClientePlanoProduto).ToList();
                }

                retorno.Objeto.IdPlanoAnterior = retorno.Objeto.VendaBase.Planos.Where(p => p.Id == IdClientePlanoProduto).FirstOrDefault()?.Id_Plano_Produto.GetValueOrDefault(0);
                retorno.Objeto.Id_Vendedor = usuario?.Funcionario != null ? (usuario.Funcionario.Vendedor ? usuario.Funcionario.Id : (Int64?)null) : (Int64?)null;
                retorno.Objeto.Id_Supervisor = usuario?.Funcionario?.Supervisor?.Id;
                retorno.Objeto.Id_Cliente = IdCliente;

                retorno.Objeto.Id_Departamento = usuario?.Funcionario != null ? (usuario.Funcionario.Departamento.Id) : (long?)null;
                retorno.Objeto.EtapaTermoAdesao = Sinapse.Model.enumerados.StatusEtapaVenda.Pendente;
                retorno.Objeto.EtapaCriacaoOS = Sinapse.Model.enumerados.StatusEtapaVenda.Pendente;
                retorno.Objeto.NecessitaCriarOS = false;

                retorno.Objeto.numeroOS = "";
                retorno.Objeto.IdPlanoProduto = 0;
                retorno.Objeto.PlanoProduto = null;
            }

            return retorno;
            #endregion
        }
        [HttpPost()]
        [Route("api/AlteracaoPlanoContratoapi/GetComboPlanoByClienteContrato")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboAvo<long?, long?, long?> request)
        {
            #region [GetComboPlanoByClienteContrato]
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

                    StringBuilder sqlFromPlano = new StringBuilder("FROM TB_PLANO A ")
                        .AppendLine("LEFT JOIN TB_CLIENTE_PLANO_PRODUTO B ")
                        .AppendLine("ON(B.ID_PLANO = A.ID)");

                    #region Filtros

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"SITUACAOPLANO = 1 AND 0 = (SELECT COUNT(ID) FROM TB_ONU WHERE ID_CLIENTE_PLANO_PRODUTO = B.ID) AND B.STATUS = 2");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"UPPER(B.ID_CLIENTE_CONTRATO) = {request.idpai}");
                    }
                    if (request.idavo.HasValue)
                    {
                        condicoes.Add($"UPPER(B.ID_CLIENTE) = {request.idavo}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.CODIGO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 20)}%' OR " +
                                       $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFromPlano.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFromPlano.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    #endregion

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID AS PLANO, B.ID, A.CODIGO, A.DESCRICAO, A.VELOCIDADEDOWNLOAD, A.VELOCIDADEUPLOAD,B.ID_CLIENTE_CONTRATO ");
                        sql.AppendLine(sqlFromPlano.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ClientePlanoProdutoComboResult
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Plano = Convert.ToInt64(reader["PLANO"]),
                                    IdClienteContrato = reader["ID_CLIENTE_CONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE_CONTRATO"]) : (long?)null,
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Download = Convert.ToString(reader["VELOCIDADEDOWNLOAD"]),
                                    Upload = Convert.ToString(reader["VELOCIDADEUPLOAD"])
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
            #endregion
        }

        [HttpGet()]
        [Route("api/AlteracaoPlanoContratoapi/GetTecnologiaPlano")]
        public async Task<GetApiRetorno<int>> GetTecnologiaPlano(long? plano)
        {
            #region [GetTecnologiaPlano]

            var retorno = new GetApiRetorno<int>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var comm = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder($"SELECT A.TIPO_TECNOLOGIA FROM TB_PLANO A WHERE A.ID = {plano}");
                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno.Objeto = Convert.ToInt32(reader["TIPO_TECNOLOGIA"]);
                        }
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
            #endregion
        }
        [HttpGet]
        [Route("api/AlteracaoPlanoContratoapi/GetAreas_Consulta")]
        public async Task<List<long>> GetAreas_Consulta(double? latitude, double? longitude)
        {
            #region  [Consulta Areas] 
            var retorno = new List<long>();
            var HandlerArea = new GeographyPattern();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Area_Cobertura> repoArea_Cobertura = new Conexao<Area_Cobertura>(sessionT5))
                {
                    var areas = (await repoArea_Cobertura.select(p => p.Situacao == Status_Default.Ativo)).ToList();

                    foreach (var area in areas)
                    {
                        Dictionary<List<Location>, bool> e_valido = new Dictionary<List<Location>, bool>();
                        var Poligonos = JsonService.DeserializeObject<List<List<Double>>>(area.Coordenadas);
                        List<List<Location>> locations = Poligonos.Select(poligono =>
                        {
                            List<Location> locationsPoly = new List<Location>();
                            for (int iterator = 0; iterator < poligono.Count; iterator++)
                            {
                                locationsPoly.Add(new Location(poligono[iterator], poligono[++iterator]));
                            }
                            return locationsPoly;
                        }).ToList();


                        foreach (var loc in locations)
                        {
                            var validado = HandlerArea.IsPointInPolygon(loc, new Location(latitude.Value, longitude.Value));
                            e_valido.Add(loc, validado);
                        }

                        if (e_valido.Any(p => p.Value))
                        {
                            retorno.Add(area.Id);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return retorno;
            #endregion
        }
        [HttpPost]
        [Route("api/AlteracaoPlanoContratoapi/HistoricoAlteracao")]
        public async Task<DataTableResponse<LogGenericoModel>> HistoricoAlteracao(HistoricoAlteracao_DataTableRequest request)
        {
            #region [HistoricoAlteracao]
            List<LogGenericoModel> retorno = new List<LogGenericoModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();


                sqlFrom.AppendLine("FROM TB_ALTERACAO_PLANO_CON A ");


                sqlFrom.AppendLine("WHERE A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM ");

                parametros.Add("@DATAINI", DateTime.Parse(request.DataIni + " 00:00:00"));
                parametros.Add("@DATAFIM", DateTime.Parse(request.DataFim + " 23:59:59"));

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine(" AND ((UPPER(A.OBJETO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                }

                if (request.Id.HasValue)
                {
                    sqlFrom.AppendLine(" AND A.IDLOG = @ID ");
                    parametros.Add("@ID", request.Id);
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

                    sql.AppendLine("SELECT A.ID, A.NOMEUSUARIOCRIACAO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.IDLOG, A.OBJETO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS ");
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
                            retorno.Add(new LogGenericoModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Chave = Convert.ToString(reader["OBJETO"]),
                                Campo = Convert.ToString(reader["CAMPO"]),
                                Campo_ValorAntes = Convert.ToString(reader["CAMPO_VALORANTES"]),
                                Campo_ValorDepois = Convert.ToString(reader["CAMPO_VALORDEPOIS"]),
                                TipoOperacao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Nome_usuario = Convert.ToString(reader["NOMEUSUARIOCRIACAO"]),
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
            #endregion
        }
    }
}