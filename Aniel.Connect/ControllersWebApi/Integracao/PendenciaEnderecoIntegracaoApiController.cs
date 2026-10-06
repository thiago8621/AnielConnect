using Aniel.Connect.ConnectBusiness.Azza;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.MK;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.Integracao;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.MK.PendenciaEndereco;
using Aniel.Connect.Repository.MK.PendenciaFinanceira;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaEndereco;
using Aniel.Connect.Repository.PendenciaEnderecoAniel;
using Aniel.Connect.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.Repository.Azza.PendenciaEndereco;
using Aniel.Connect.Repository.Repository.Azza.PendenciaFinanceira;
using Aniel.Connect.Repository.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.Repository.T5.ConfigAnielIntegrator;
using Aniel.Connect.Repository.Venda;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Twilio.TwiML.Voice;

namespace Aniel.Connect.ControllersWebApi
{
    public class PendenciaEnderecoIntegracaoApiController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/PendenciaEnderecoIntegracaoApi/ExecutarConsultaPendenciaEndereco")]
        public async Task<GetApiRetorno<ExecutarConsultaPendenciaEnderecoResponse>> ExecutarConsultaPendenciaEndereco(ConsultaPendenciaEnderecoRequest request)
        {
            var retorno = new GetApiRetorno<ExecutarConsultaPendenciaEnderecoResponse>
            {
                Objeto = new ExecutarConsultaPendenciaEnderecoResponse()
            };

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryEtapa = new Conexao<EtapaVenda>(sessionNh))
                using (var repositoryLogs = new Conexao<LogEtapaVendas>(sessionLog))
                {
                    Venda venda = null;
                    TrocaTitularidade trocaOld = null;
                    EtapaVenda etapaOld = null;

                    IPendenciaEnderecoAnielRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoAnielRepository(sessionNh);
                    IPendenciaFinanceiraAnielRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraAnielRepository(sessionNh);
                    var consultaPendenciasConnectBusiness = new ConsultaPendenciasConnectBusiness(repoPendenciaEnderecoConnect, repoPendenciaFinanceiraConnect);

                    IPendenciaEnderecoRepository repoPendenciaEndereco = new PendenciaEnderecoRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEndereco, repoConfig);

                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(sessionNh);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);

                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);
                    var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);

                    PendenciaEndereco pendenciaEndereco = null;
                    string documento = String.Empty;
                    if (!String.IsNullOrEmpty(request.Cpf))
                    {
                        documento = request.Cpf;
                    }
                    else
                    {
                        documento = request.Cnpj;
                    }

                    if (integracaoExternaBusiness.UtilizaMkConsultaPendenciaEndereco())
                    {
                        if (!String.IsNullOrEmpty(CONSTANTES.MKConnectionString))
                        {
                            using (var connPg = PostgreSqlUtil.GetConnection(CONSTANTES.MKConnectionString))
                            {
                                IPendenciaEnderecoMkRepository repoPendenciaEnderecoMk = new PendenciaEnderecoMkRepository(connPg);
                                IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraMk = new PendenciaFinanceiraMkRepository(connPg);
                                var mkBusiness = new MKBusiness(repoPendenciaEnderecoMk, repoPendenciaFinanceiraMk, null);

                                IEnumerable<PendenciaEnderecoListModel> pendenciasMk =
                                    mkBusiness.GetPendenciasEndereco(request.Logradouro, request.Numero, request.Cidade);

                                pendenciaEndereco =
                                    pendenciaEnderecoBusiness.GravarPendenciaEndereco(request.Logradouro, request.Numero, pendenciasMk);

                                retorno.Objeto.IdConsultaPendenciaEndereco = pendenciaEndereco.Id;
                                // O campo "Objeto" do retorno irá indicar se foi identificada alguma pendência no endereço
                                retorno.Objeto.PossuiPendencia = pendenciasMk != null && pendenciasMk.Count() > 0;
                            }
                        }
                        else
                        {
                            throw new Exception("Banco do MK Solutions não configurado");
                        }
                    }
                    else if (integracaoExternaBusiness.UtilizaAlcansConsultaPendenciaEndereco())
                    {
                        using var integracoesExternas = new Conexao<IntegracoesExternas>(sessionNh);
                        using VendaApiController vendaApiController = new VendaApiController();
                        var configIntegracao = await integracoesExternas.selectFirst(x => x.TipoIntegracao == Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_ENDERECO && x.IntegracaoExterna == Model.enumerado.IntegracaoExterna.ALCANS);

                        if (configIntegracao != null || !string.IsNullOrEmpty(configIntegracao.Usuario) || !string.IsNullOrEmpty(configIntegracao.Senha) || !string.IsNullOrEmpty(configIntegracao.UrlApi))
                        {
                            IPendenciaEnderecoMkRepository repoPendenciaEnderecoAlcans = new PendenciaEnderecoMkAlcansRepository(configIntegracao.Usuario, configIntegracao.Senha, configIntegracao.UrlApi);

                            var enderecoConnect = await vendaApiController.GetEnderencoByVenda(request.IdVenda.Value);

                            if (enderecoConnect == null || string.IsNullOrEmpty(enderecoConnect.CodUf) || string.IsNullOrEmpty(enderecoConnect.CodCidade) || string.IsNullOrEmpty(enderecoConnect.CodBairro)
                                || string.IsNullOrEmpty(enderecoConnect.CodRua))
                                throw new Exception(Resources.RsCadastros.MENSAGEM_ENDERECO_SEM_CODAUX);

                            IEnumerable<PendenciaEnderecoListModel> pendenciasAlcans = repoPendenciaEnderecoAlcans.GetPendenciasEnderecoAlcans(
                                "", "", "", enderecoConnect.CodRua, request.Numero);

                            pendenciaEndereco = pendenciaEnderecoBusiness.GravarPendenciaEndereco(request.Logradouro, request.Numero, pendenciasAlcans);

                            retorno.Objeto.IdConsultaPendenciaEndereco = pendenciaEndereco.Id;
                            retorno.Objeto.PossuiPendencia = pendenciasAlcans != null && pendenciasAlcans.Count() > 0;
                        }
                        else
                            throw new Exception("Middleware da Alcans não configurado");
                    }
                    else if (integracaoExternaBusiness.UtilizaIXCConsultaPendenciaEndereco())
                    {
                        IConfigAnielIntegratorRepository _repoConfigAnielIntegrator = new ConfigAnielIntegratorRepository(sessionT5);
                        var objIntegrator = await _repoConfigAnielIntegrator.ObterConfigAnielIntegrator();
                        if (objIntegrator != null)
                        {
                            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraIXC = new PendenciaFinanceiraIXCRepository(objIntegrator.URL, objIntegrator.Token);
                            ICidadeRepository cidadeRepository = new CidadeRepository(sessionNh);
                            IPendenciaEnderecoIntegracaoRepository repoPendenciaEnderecoIXC = new PendenciaEnderecoIntegracaoRepository(objIntegrator.URL, objIntegrator.Token, cidadeRepository);

                            var business = new IXCBusiness(repoPendenciaFinanceiraIXC, null, repoPendenciaEnderecoIXC);

                            IEnumerable<PendenciaEnderecoListModel> pendenciasIntegracao = await business.GetPendenciasEndereco(request.Logradouro, request.Numero, request.Cidade, request.CEP);

                            pendenciaEndereco =
                                    pendenciaEnderecoBusiness.GravarPendenciaEndereco(request.Logradouro, request.Numero, pendenciasIntegracao);

                            retorno.Objeto.IdConsultaPendenciaEndereco = pendenciaEndereco.Id;
                            retorno.Objeto.PossuiPendencia = pendenciasIntegracao != null && pendenciasIntegracao.Count() > 0;
                        }
                    }
                    else
                    {
                        IEnumerable<PendenciaEnderecoListModel> pendenciasConnect =
                           consultaPendenciasConnectBusiness.GetPendenciasEndereco(request.Logradouro, request.Numero);

                        pendenciaEndereco =
                            pendenciaEnderecoBusiness.GravarPendenciaEndereco(request.Logradouro, request.Numero, pendenciasConnect);

                        retorno.Objeto.IdConsultaPendenciaEndereco = pendenciaEndereco.Id;
                        retorno.Objeto.PossuiPendencia = pendenciasConnect != null && pendenciasConnect.Count() > 0;
                    }

                    if (pendenciaEndereco == null)
                    {
                        throw new Exception("Erro ao gravar os resultados da consulta de pendência de endereço");
                    }

                    if (request.IdVenda.HasValue)
                    {
                        IVendaRepository repoVenda = new VendaRepository(sessionNh);
                        var vendaBusiness = new VendaBusiness(repoVenda);

                        venda = vendaBusiness.GetVendaById(request.IdVenda.Value);
                        var etapa = await repositoryEtapa.selectFirst(p => p.Etapa == EnumEtapaVenda.ConsultaPendenciaEndereco && p.Venda.Id == request.IdVenda);
                        etapaOld = (EtapaVenda)etapa.Clone();

                        vendaBusiness.GravarIdConsultaRestricao(venda, EnumEtapaVenda.ConsultaPendenciaEndereco, pendenciaEndereco.Id);

                        var statusConsulta = pendenciaEnderecoBusiness.GetStatusConsultaPendenciaEndereco(request.Logradouro, request.Numero);
                        etapaVendaBusiness.MudarStatusEtapa(venda.Id, EnumEtapaVenda.ConsultaPendenciaEndereco, statusConsulta, DateTime.Now);

                        //Log 
                        var etapa_venda = new EtapaVenda
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = User.Identity.GetUserId(),
                            Descricao = etapa.Descricao,
                            Etapa = etapa.Etapa,
                            Status = statusConsulta,
                            Venda = venda
                        };

                        var logs = etapa_venda.GeraLog(etapaOld, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                        await repositoryLogs.Save(logs);
                    }

                    if (request.IdTroca.HasValue)
                    {
                        using (var repoTroca = new Conexao<TrocaTitularidade>(sessionNh))
                        {
                            var troca = repoTroca.selectFirst(t => t.Id == request.IdTroca.Value).Result;

                            troca.IdConsultaPendenciaFinanceira = pendenciaEndereco.Id;

                            await repoTroca.Save(troca);
                        }
                    }

                    retorno.Objeto.DataHoraCriacao = pendenciaEndereco.DataHoraCriacao;
                }

                retorno.Ok = true;
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

        [AllowAnonymous]
        [HttpPost]
        [Route("api/PendenciaEnderecoIntegracaoApi/GetFaturaByVenda")]
        public DataTableResponse<PendenciaEnderecoListModel> GetFaturaByVenda(GetFaturaEnderecoByVendaDataTableRequest request)
        {
            IEnumerable<PendenciaEnderecoListModel> dados = new List<PendenciaEnderecoListModel>();

            using (ISession sessionNh = HibernateUtil.GetSession())
            {
                IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(sessionNh);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);

                IVendaRepository repoVenda = new VendaRepository(sessionNh);
                var vendaBusiness = new VendaBusiness(repoVenda);

                Venda venda = vendaBusiness.GetVendaById(request.IdVenda);

                if (venda?.IdConsultaPendenciaEndereco != null)
                {
                    dados = pendenciaEnderecoBusiness.GetPendenciasEnderecoById(venda.IdConsultaPendenciaEndereco.Value);
                }
            }

            return new DataTableResponse<PendenciaEnderecoListModel>
            {
                draw = request.draw,
                recordsFiltered = dados.Count(),
                data = dados,
                error = ""
            };
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/PendenciaEnderecoIntegracaoApi/GetFaturaByEndereco")]
        public DataTableResponse<PendenciaEnderecoListModel> GetFaturaByEndereco(GetFaturaByEnderecoDataTableRequest request)
        {
            IEnumerable<PendenciaEnderecoListModel> dados = new List<PendenciaEnderecoListModel>();

            using (ISession sessionNh = HibernateUtil.GetSession())
            {
                IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(sessionNh);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);
                dados = pendenciaEnderecoBusiness.GetPendenciasEndereco(request.Logradouro, request.Numero).pendencias;
            }
            dados = dados != null ? dados : new List<PendenciaEnderecoListModel>();
            return new DataTableResponse<PendenciaEnderecoListModel>
            {
                draw = request.draw,
                recordsFiltered = dados.Count(),
                data = dados,
                error = ""
            };
        }

        [HttpGet]
        [Route("api/PendenciaEnderecoIntegracaoApi/ExisteConsultaPendenciaEndereco")]
        public GetApiRetorno<ExistePendenciaEnderecoResponse> ExisteConsultaPendenciaEndereco(string logradouro, string numero)
        {
            var retorno = new GetApiRetorno<ExistePendenciaEnderecoResponse>
            {
                Objeto = new ExistePendenciaEnderecoResponse()
            };

            try
            {
                if (String.IsNullOrEmpty(logradouro))
                {
                    throw new Exception("Logradouro não informado");
                }

                if (String.IsNullOrEmpty(numero))
                {
                    throw new Exception("Número não informado");
                }

                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                    var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);

                    var consultaPendenciaEndereco = pendenciaEnderecoBusiness.GetConsultaPendenciaEndereco(logradouro, numero);

                    retorno.Objeto.IdConsultaPendenciaEndereco = consultaPendenciaEndereco?.Id;
                    retorno.Objeto.ExisteConsulta = consultaPendenciaEndereco != null;
                    retorno.Objeto.Dados = pendenciaEnderecoBusiness.GetPendenciasEndereco(logradouro, numero).pendencias;
                }

                retorno.Ok = true;
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

        [HttpGet]
        [Route("api/PendenciaEnderecoIntegracaoApi/ExisteConsultaPendenciaEnderecoVenda")]
        public GetApiRetorno<ExistePendenciaEnderecoResponse> ExisteConsultaPendenciaEnderecoVenda(long IdVenda)
        {
            var retorno = new GetApiRetorno<ExistePendenciaEnderecoResponse>
            {
                Objeto = new ExistePendenciaEnderecoResponse()
            };

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                    var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);

                    IVendaRepository repoVenda = new VendaRepository(sessionNh);
                    var vendaBusiness = new VendaBusiness(repoVenda);

                    IEnumerable<PendenciaEnderecoListModel> dadosPendenciaEndereco = null;
                    Venda venda = vendaBusiness.GetVendaById(IdVenda);

                    if (venda?.IdConsultaPendenciaEndereco != null)
                    {
                        dadosPendenciaEndereco =
                            pendenciaEnderecoBusiness.GetPendenciasEnderecoById(venda.IdConsultaPendenciaEndereco.Value);
                    }

                    retorno.Objeto.Dados = dadosPendenciaEndereco;
                }

                retorno.Ok = true;
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

        #region [metodos privados]

        #region [ObterConfigAnielIntegrator]
        private async Task<Config_Aniel_Integrator> ObterConfigAnielIntegrator(ISession sessionT5)
        {
            Config_Aniel_Integrator config = new Config_Aniel_Integrator();
            using (var comT5 = sessionT5.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT * ");
                sql.AppendLine(" FROM TB_CONFIG_ANIEL_INTEGRATOR ");
                //DAO IXC usando o Integrator
                using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sql, parametros, sessionT5.Connection, null, 1, null))
                {
                    if (await reader.ReadAsync())
                    {
                        config.URL = Convert.ToString(reader["URL"]);
                        config.UsuarioAniel = Convert.ToString(reader["USUARIO_ANIEL"]);
                        config.CodCtPadrao = Convert.ToString(reader["CODCT_PADRAO"]);
                        config.ProjetoPadrao = Convert.ToString(reader["PROJETO_PADRAO"]);
                        config.Unidade_Custo = Convert.ToString(reader["UNIDADE_CUSTO"]);
                        config.Token = Convert.ToString(reader["TOKEN"]);
                        config.Setores = Convert.ToString(reader["SETORES"]);
                    }
                }
            }
            return config;
        }
        #endregion

        #endregion
    }
}