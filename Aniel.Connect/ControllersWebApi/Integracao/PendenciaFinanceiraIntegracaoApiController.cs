using Aniel.Connect.ConnectBusiness.Azza;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.MK;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
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
using Aniel.Connect.Repository.PendenciaFinanceira;
using Aniel.Connect.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.Repository.Azza.PendenciaEndereco;
using Aniel.Connect.Repository.Repository.Azza.PendenciaFinanceira;
using Aniel.Connect.Repository.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.Repository.T5.ConfigAnielIntegrator;
using Aniel.Connect.Repository.Venda;
using Aniel.Connect.Repository.Voalle.PendenciaFinanceira;
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
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Aniel.Connect.ControllersWebApi
{
    public class PendenciaFinanceiraIntegracaoApiController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/PendenciaFinanceiraIntegracaoApi/ExecutarConsultaPendenciaFinanceira")]
        public async Task<GetApiRetorno<ExecutarConsultaPendenciaFinanceiraResponse>> ExecutarConsultaPendenciaFinanceira(ConsultaPendenciaFinanceiraRequest request)
        {
            var retorno = new GetApiRetorno<ExecutarConsultaPendenciaFinanceiraResponse>
            {
                Objeto = new ExecutarConsultaPendenciaFinanceiraResponse()
            };

            try
            {
                if (String.IsNullOrEmpty(request.Cpf) && String.IsNullOrEmpty(request.Cnpj))
                {
                    throw new Exception("Documento (CPF/CNPJ) não informado para a consulta financeira");
                }

                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryEtapa = new Conexao<EtapaVenda>(sessionNh))
                using (var repositoryLogs = new Conexao<LogEtapaVendas>(sessionLog))
                {
                    Venda venda = null;
                    TrocaTitularidade trocaOld = null;
                    EtapaVenda etapaOld = null;

                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    IPendenciaEnderecoAnielRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoAnielRepository(sessionNh);
                    IPendenciaFinanceiraAnielRepository repoPendenciaFinanceira = new PendenciaFinanceiraAnielRepository(sessionNh);
                    var pendenciasConnectBusiness = new ConsultaPendenciasConnectBusiness(repoPendenciaEnderecoConnect, repoPendenciaFinanceira);

                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(sessionNh);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);

                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);
                    var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);

                    string documento;
                    Tipo_Pessoa tipoPessoa;

                    PendenciaFinanceira pendenciaFinanceira = null;

                    if (!String.IsNullOrEmpty(request.Cpf))
                    {
                        documento = request.Cpf;
                        tipoPessoa = Tipo_Pessoa.Fisica;
                    }
                    else
                    {
                        documento = request.Cnpj;
                        tipoPessoa = Tipo_Pessoa.Juridica;
                    }

                    if (integracaoExternaBusiness.UtilizaMkConsultaPendenciaFinanceira())
                    {
                        if (!String.IsNullOrEmpty(CONSTANTES.MKConnectionString))
                        {
                            using (var connPg = PostgreSqlUtil.GetConnection(CONSTANTES.MKConnectionString))
                            {
                                IPendenciaEnderecoMkRepository repoPendenciaEnderecoMk = new PendenciaEnderecoMkRepository(connPg);
                                IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraMk = new PendenciaFinanceiraMkRepository(connPg);
                                var mkBusiness = new MKBusiness(repoPendenciaEnderecoMk, repoPendenciaFinanceiraMk, null);

                                IEnumerable<PendenciaFinanceiraListModel> pendenciasMk =
                                   mkBusiness.GetPendenciasFinanceira(documento);

                                pendenciaFinanceira =
                                    pendenciaFinanceiraBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasMk);

                                retorno.Objeto.IdConsultaPendenciaFinanceira = pendenciaFinanceira.Id;
                                retorno.Objeto.PossuiPendencia = pendenciasMk != null && pendenciasMk.Count() > 0;
                            }
                        }
                        else
                        {
                            throw new Exception("Banco do MK Solutions não configurado");
                        }
                    }
                    else if (integracaoExternaBusiness.UtilizaAlcansConsultaPendenciaFinanceira())
                    {
                        using var integracoesExternas = new Conexao<IntegracoesExternas>(sessionNh);
                        var configIntegracao = await integracoesExternas.selectFirst(x => x.TipoIntegracao == Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_FINANCEIRA && x.IntegracaoExterna == Model.enumerado.IntegracaoExterna.ALCANS);

                        if (configIntegracao != null || !string.IsNullOrEmpty(configIntegracao.Usuario) || !string.IsNullOrEmpty(configIntegracao.Senha) || !string.IsNullOrEmpty(configIntegracao.UrlApi))
                        {
                            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraAlcans = new PendenciaFinanceiraMkAlcansRepository(configIntegracao.Usuario, configIntegracao.Senha, configIntegracao.UrlApi);

                            IEnumerable<PendenciaFinanceiraListModel> pendenciasAlcans = repoPendenciaFinanceiraAlcans.GetPendenciasFinanceira(documento);

                            pendenciaFinanceira = pendenciaFinanceiraBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasAlcans);

                            retorno.Objeto.IdConsultaPendenciaFinanceira = pendenciaFinanceira.Id;
                            retorno.Objeto.PossuiPendencia = pendenciasAlcans != null && pendenciasAlcans.Count() > 0;
                        }
                        else
                            throw new Exception("Middleware da Alcans não configurado");
                    }
                    else if (integracaoExternaBusiness.UtilizaVoalleConsultaPendenciaFinanceira())
                    {
                        if (!String.IsNullOrEmpty(CONSTANTES.VoalleConnectionString))
                        {
                            using (var connPg = PostgreSqlUtil.GetConnection(CONSTANTES.VoalleConnectionString))
                            {
                                //IPendenciaEnderecoMkRepository repoPendenciaEnderecoMk = new PendenciaEnderecoMkRepository(connPg);
                                IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraVoalle = new PendenciaFinanceiraVoalleRepository(connPg, CONSTANTES.VoalleUtilizaDw == "S");
                                var voalleBusiness = new VoalleBusiness(repoPendenciaFinanceiraVoalle, null);

                                IEnumerable<PendenciaFinanceiraListModel> pendenciasVoalle =
                                   voalleBusiness.GetPendenciasFinanceira(documento);

                                pendenciaFinanceira =
                                    pendenciaFinanceiraBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasVoalle);

                                retorno.Objeto.IdConsultaPendenciaFinanceira = pendenciaFinanceira.Id;
                                retorno.Objeto.PossuiPendencia = pendenciasVoalle != null && pendenciasVoalle.Count() > 0;
                            }
                        }
                        else
                        {
                            throw new Exception("Banco do MK Solutions não configurado");
                        }
                    }
                    else if (integracaoExternaBusiness.UtilizaIXCConsultaPendenciaFinanceira())
                    {

                        IConfigAnielIntegratorRepository _repoConfigAnielIntegrator = new ConfigAnielIntegratorRepository(sessionT5);
                        var objIntegrator = await _repoConfigAnielIntegrator.ObterConfigAnielIntegrator();
                        if (objIntegrator != null)
                        {
                            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraIXC = new PendenciaFinanceiraIXCRepository(objIntegrator.URL, objIntegrator.Token);
                            ICidadeRepository cidadeRepository = new CidadeRepository(sessionT5);

                            IPendenciaEnderecoIntegracaoRepository repoPendenciaEnderecoIXC = new PendenciaEnderecoIntegracaoRepository(objIntegrator.URL, objIntegrator.Token, cidadeRepository);
                            var business = new IXCBusiness(repoPendenciaFinanceiraIXC, null, repoPendenciaEnderecoIXC);

                            IEnumerable<PendenciaFinanceiraListModel> pendenciasIntegracao =
                               business.GetPendenciasFinanceira(documento);

                            pendenciaFinanceira =
                                pendenciaFinanceiraBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasIntegracao);

                            retorno.Objeto.IdConsultaPendenciaFinanceira = pendenciaFinanceira.Id;
                            retorno.Objeto.PossuiPendencia = pendenciasIntegracao != null && pendenciasIntegracao.Count() > 0;
                        }
                        else
                        {
                            throw new Exception("Banco do Azza não está configurado!!");
                        }
                    }
                    else
                    {
                        IEnumerable<PendenciaFinanceiraListModel> pendenciasConnect =
                            pendenciasConnectBusiness.GetPendenciasFinanceira(documento);

                        pendenciaFinanceira =
                            pendenciaFinanceiraBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasConnect);

                        retorno.Objeto.IdConsultaPendenciaFinanceira = pendenciaFinanceira.Id;
                        retorno.Objeto.PossuiPendencia = pendenciasConnect != null && pendenciasConnect.Count() > 0;
                    }

                    if (pendenciaFinanceira == null)
                    {
                        throw new Exception("Erro ao gravar os resultados da consulta de pendência financeira");
                    }

                    if (request.IdVenda.HasValue)
                    {
                        IVendaRepository repoVenda = new VendaRepository(sessionNh);
                        var vendaBusiness = new VendaBusiness(repoVenda);

                        venda = vendaBusiness.GetVendaById(request.IdVenda.Value);
                        var etapa = await repositoryEtapa.selectFirst(p => p.Etapa == EnumEtapaVenda.ConsultaPendenciaFinanceira && p.Venda.Id == request.IdVenda);
                        etapaOld = (EtapaVenda)etapa.Clone();
                        vendaBusiness.GravarIdConsultaRestricao(venda, EnumEtapaVenda.ConsultaPendenciaFinanceira, pendenciaFinanceira.Id);

                        var statusConsulta = pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCpf(documento);
                        etapaVendaBusiness.MudarStatusEtapa(venda.Id, EnumEtapaVenda.ConsultaPendenciaFinanceira, statusConsulta, DateTime.Now);

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
                    
                    if (request.IdTrocaTitularidade.HasValue)
                    {
                       using (var repoTroca = new Conexao<TrocaTitularidade>(sessionNh))
                        {
                            var troca = repoTroca.selectFirst(t => t.Id == request.IdTrocaTitularidade.Value).Result;

                            troca.IdConsultaPendenciaFinanceira = pendenciaFinanceira.Id;

                            await repoTroca.SaveOrUpdate(troca);
                        }
                    }
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
        [OlaAuthorize]
        [HttpPost]
        [Route("api/PendenciaFinanceiraIntegracaoApi/ExecutarConsultaPendenciaFinanceiraMobile")]
        public GetApiRetorno<bool> ExecutarConsultaPendenciaFinanceiraMobile(ConsultaPendenciaFinanceiraRequest request)
        {
            var retorno = new GetApiRetorno<bool>();

            try
            {
                if (String.IsNullOrEmpty(request.Cpf) && String.IsNullOrEmpty(request.Cnpj))
                {
                    throw new Exception("Documento (CPF/CNPJ) não informado para a consulta financeira");
                }

                if (!String.IsNullOrEmpty(CONSTANTES.MKConnectionString))
                {
                    using (ISession sessionNh = HibernateUtil.GetSession())
                    using (var connPg = PostgreSqlUtil.GetConnection(CONSTANTES.MKConnectionString))
                    {
                        IPendenciaEnderecoMkRepository repoPendenciaEnderecoMk = new PendenciaEnderecoMkRepository(connPg);
                        IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraMk = new PendenciaFinanceiraMkRepository(connPg);
                        var mkBusiness = new MKBusiness(repoPendenciaEnderecoMk, repoPendenciaFinanceiraMk, null);

                        IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(sessionNh);
                        IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                        var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                        string documento;
                        Tipo_Pessoa tipoPessoa;

                        if (!String.IsNullOrEmpty(request.Cpf))
                        {
                            documento = request.Cpf;
                            tipoPessoa = Tipo_Pessoa.Fisica;
                        }
                        else
                        {
                            documento = request.Cnpj;
                            tipoPessoa = Tipo_Pessoa.Juridica;
                        }

                        IEnumerable<PendenciaFinanceiraListModel> pendenciasMk =
                            mkBusiness.GetPendenciasFinanceira(documento);

                        pendenciaFinanceiraBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasMk);

                        // O campo "Objeto" do retorno irá indicar se foi identificada alguma pendência financeira
                        retorno.Objeto = pendenciasMk != null && pendenciasMk.Count() > 0;
                    }
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
        [Route("api/PendenciaFinanceiraIntegracaoApi/GetFaturaByVenda")]
        public DataTableResponse<PendenciaFinanceiraListModel> GetFaturaByVenda(GetFaturaByVendaDataTableRequest request)
        {
            IEnumerable<PendenciaFinanceiraListModel> dados = new List<PendenciaFinanceiraListModel>();
            using (ISession session = HibernateUtil.GetSession())            
            {
                IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                
                if (!String.IsNullOrEmpty(CONSTANTES.MKConnectionString) && integracaoExternaBusiness.UtilizaMkConsultaPendenciaFinanceira())
                {                                        
                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    IVendaRepository repoVenda = new VendaRepository(session);
                    var vendaBusiness = new VendaBusiness(repoVenda);

                    Venda venda = vendaBusiness.GetVendaById(request.IdVenda);

                    if (venda?.IdConsultaPendenciaFinanceira != null)
                    {
                        dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasById(venda.IdConsultaPendenciaFinanceira.Value);
                    }
                    
                }
                else if (integracaoExternaBusiness.UtilizaAlcansConsultaPendenciaFinanceira())
                {
                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    IVendaRepository repoVenda = new VendaRepository(session);
                    var vendaBusiness = new VendaBusiness(repoVenda);

                    Venda venda = vendaBusiness.GetVendaById(request.IdVenda);

                    if (venda?.IdConsultaPendenciaFinanceira != null)
                    {
                        dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasById(venda.IdConsultaPendenciaFinanceira.Value);
                    }
                }
                else if (integracaoExternaBusiness.UtilizaIXCConsultaPendenciaFinanceira()) 
                {
                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    IVendaRepository repoVenda = new VendaRepository(session);
                    var vendaBusiness = new VendaBusiness(repoVenda);

                    Venda venda = vendaBusiness.GetVendaById(request.IdVenda);

                    if (venda?.IdConsultaPendenciaFinanceira != null)
                    {
                        dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasById(venda.IdConsultaPendenciaFinanceira.Value);
                    }
                }
                else
                {
                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    IVendaRepository repoVenda = new VendaRepository(session);
                    var vendaBusiness = new VendaBusiness(repoVenda);

                    Venda venda = vendaBusiness.GetVendaById(request.IdVenda);

                    if (venda?.IdConsultaPendenciaFinanceira != null)
                    {
                        dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasById(venda.IdConsultaPendenciaFinanceira.Value);
                    }
                }
            }

            return new DataTableResponse<PendenciaFinanceiraListModel>
            {
                draw = request.draw,
                recordsFiltered = dados.Count(),
                data = dados,
                error = ""
            };
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/PendenciaFinanceiraIntegracaoApi/GetFaturaByTrocaTitularidade")]
        public DataTableResponse<PendenciaFinanceiraListModel> GetFaturaByTrocaTitularidade(GetFaturaByVendaDataTableRequest request)
        {
            IEnumerable<PendenciaFinanceiraListModel> dados = new List<PendenciaFinanceiraListModel>();
            using (ISession session = HibernateUtil.GetSession())
            using (var repoTroca = new Conexao<TrocaTitularidade>(session))
            {
                IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);

                if (!String.IsNullOrEmpty(CONSTANTES.MKConnectionString) && integracaoExternaBusiness.UtilizaMkConsultaPendenciaFinanceira())
                {
                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    TrocaTitularidade troca = repoTroca.selectFirst(x => x.Id == request.IdVenda).Result;

                    if (troca?.IdConsultaPendenciaFinanceira != null)
                    {
                        dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasById(troca.IdConsultaPendenciaFinanceira.Value);
                    }

                }
                else if (integracaoExternaBusiness.UtilizaIXCConsultaPendenciaFinanceira())
                {
                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    TrocaTitularidade troca = repoTroca.selectFirst(x => x.Id == request.IdVenda).Result;

                    if (troca?.IdConsultaPendenciaFinanceira != null)
                    {
                        dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasById(troca.IdConsultaPendenciaFinanceira.Value);
                    }

                    if (troca?.IdConsultaPendenciaFinanceira != null)
                    {
                        dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasById(troca.IdConsultaPendenciaFinanceira.Value);
                    }
                }

            }



            return new DataTableResponse<PendenciaFinanceiraListModel>
            {
                draw = request.draw,
                recordsFiltered = dados.Count(),
                data = dados,
                error = ""
            };
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/PendenciaFinanceiraIntegracaoApi/GetFaturaByCpfCnpj")]
        public DataTableResponse<PendenciaFinanceiraListModel> GetFaturaByCpfCnpj(GetFaturaByCpfCnpjDataTableRequest request)
        {
            IEnumerable<PendenciaFinanceiraListModel> dados = new List<PendenciaFinanceiraListModel>();

            if (!String.IsNullOrEmpty(CONSTANTES.MKConnectionString) || !String.IsNullOrEmpty(CONSTANTES.VoalleConnectionString))
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    if (!String.IsNullOrEmpty(request.Cpf))
                    {
                        dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasByCpf(request.Cpf).pendencias;
                    }
                    else
                    {
                        dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasByCnpj(request.Cnpj).pendencias;
                    }
                }
            }

            dados = dados != null ? dados : new List<PendenciaFinanceiraListModel>();
            return new DataTableResponse<PendenciaFinanceiraListModel>
            {
                draw = request.draw,
                recordsFiltered = dados.Count(),
                data = dados,
                error = ""
            };
        }

        [HttpGet]
        [Route("api/PendenciaFinanceiraIntegracaoApi/ExisteConsultaPendenciaFinanceira")]
        public GetApiRetorno<ExistePendenciaFinanceiraResponse> ExisteConsultaPendenciaFinanceira(string cpf = null, string cnpj = null)
        {
            var retorno = new GetApiRetorno<ExistePendenciaFinanceiraResponse>
            {
                Objeto = new ExistePendenciaFinanceiraResponse()
            };

            try
            {
                if (String.IsNullOrEmpty(cpf) && String.IsNullOrEmpty(cnpj))
                {
                    throw new Exception("CPF/CNPJ não informado");
                }

                using (ISession sessionNh = HibernateUtil.GetSession())
                {

                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    if (!String.IsNullOrEmpty(cpf))
                    {
                        PendenciaFinanceira consultaPendenciaFinanceira = pendenciaFinanceiraBusiness.GetConsultaPendenciaFinanceiraByCpf(cpf);

                        retorno.Objeto.IdConsultaPendenciaFinanceira = consultaPendenciaFinanceira?.Id;
                        retorno.Objeto.ExisteConsulta = consultaPendenciaFinanceira != null;
                        retorno.Objeto.Dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasByCpf(cpf).pendencias;
                    }
                    else
                    {
                        PendenciaFinanceira consultaPendenciaFinanceira = pendenciaFinanceiraBusiness.GetConsultaPendenciaFinanceiraByCnpj(cnpj);

                        retorno.Objeto.IdConsultaPendenciaFinanceira = consultaPendenciaFinanceira?.Id;
                        retorno.Objeto.ExisteConsulta = consultaPendenciaFinanceira != null;
                        retorno.Objeto.Dados = pendenciaFinanceiraBusiness.GetPendenciasFinanceirasByCnpj(cnpj).pendencias;
                    }
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
        [Route("api/PendenciaFinanceiraIntegracaoApi/ExisteConsultaPendenciaFinanceiraVenda")]
        public GetApiRetorno<ExistePendenciaFinanceiraResponse> ExisteConsultaPendenciaFinanceiraVenda(long IdVenda)
        {
            var retorno = new GetApiRetorno<ExistePendenciaFinanceiraResponse>
            {
                Objeto = new ExistePendenciaFinanceiraResponse()
            };

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    IVendaRepository repoVenda = new VendaRepository(sessionNh);
                    var vendaBusiness = new VendaBusiness(repoVenda);

                    IEnumerable<PendenciaFinanceiraListModel> dadosPendenciaFinanceira = null;
                    Venda venda = vendaBusiness.GetVendaById(IdVenda);

                    if (venda?.IdConsultaPendenciaFinanceira != null)
                    {
                        dadosPendenciaFinanceira =
                            pendenciaFinanceiraBusiness.GetPendenciasFinanceirasById(venda.IdConsultaPendenciaFinanceira.Value);
                    }

                    retorno.Objeto.Dados = dadosPendenciaFinanceira;
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

                sql.AppendLine($"SELECT FIRST 1 * ");
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