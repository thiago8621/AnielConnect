using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.ConsultaCredito;
using Aniel.Connect.Repository.CriteriosParamCredito;
using Aniel.Connect.Repository.EnderecoCliente;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.ParametrosConsultaCredito;
using Aniel.Connect.Repository.ParametrosConsultaPreQual;
using Aniel.Connect.Repository.Venda;
using Microsoft.AspNet.Identity;
using NHibernate;
using OpenXmlPowerTools;
using RestSharp.Extensions;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ConsultaCreditoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/ConsultaCreditoApi/ConsultaCreditoByCpf")]
        public async Task<GetApiRetorno<ConsultaCreditoApiResponse>> ConsultaCreditoByCpf(ConsultaCreditoByCpfApiRequest request)
        {
            var retorno = new GetApiRetorno<ConsultaCreditoApiResponse>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repoIntegracoes = new Conexao<IntegracoesExternas>(sessionNh))
                using (var repositoryCliente = new Conexao<Cliente>(sessionNh))
                using (var repoIdentity = new Conexao<IdentityUser>(sessionNh))
                using (var repoVenda = new Conexao<Venda>(sessionNh))
                using (var usuarioApi = new UsuarioApiController())
                {
                    if (request == null)
                    {
                        throw new Exception("Parâmetros da requisição não informados");
                    }

                    if (request.settings != null && !usuarioApi.GetUsuarioValido(request.settings).Result.Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Inválidos");
                    }

                    if (String.IsNullOrEmpty(request.Cpf))
                    {
                        throw new Exception("CPF não informado para a consulta de crédito");
                    }
                    
                    IntegracoesExternas configIntegracaoSerasa = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SERASA || p.IntegracaoExterna == IntegracaoExterna.PREQUAL || p.IntegracaoExterna == IntegracaoExterna.SERASA_EXPERIAN).Result;                    

                    if (configIntegracaoSerasa != null)
                    {
                        string cpf = request.Cpf.Replace(".", String.Empty)
                                                .Replace("-", String.Empty);

                        Sinapse.Model.ConsultaSerasaDados consultaSerasa = null;
                        Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusConsulta;
                        string usuario = configIntegracaoSerasa.Usuario;
                        string senha = configIntegracaoSerasa.Senha;
                        string url = configIntegracaoSerasa.UrlApi;

                        IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                        ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                        IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                        IParametrosConsultaPreQualRepository repoPrequalParams = new ParametrosConsultaPreQualRepository(sessionNh);
                        IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                        IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                        IVendaRepository repoVendaNH = new VendaRepository(sessionNh);
                        Repository.IntegracaoExterna.IIntegracaoExternaRepository repoIntegracao = new Repository.IntegracaoExterna.IntegracaoExternaRepository(sessionNh);

                        switch (configIntegracaoSerasa.IntegracaoExterna)
                        {
                            case IntegracaoExterna.SERASA:
                                {
                                    var consultaCreditoBusiness =
                                        new ConsultaCreditoBusiness(
                                            repoConsulta,
                                            repoCriterio,
                                            repoParams,
                                            repoConfig,
                                            usuario,
                                            senha,
                                            url
                                        );

                                    consultaSerasa = await consultaCreditoBusiness.ExecutarConsultaCreditoCpf(cpf, User.Identity.GetUserName());
                                    int pontos = consultaCreditoBusiness.GetTotalPontosConsultaCpf(consultaSerasa.Id, cpf);
                                    statusConsulta = consultaCreditoBusiness.GetStatusConsultaCreditoCpf(cpf).valor;                                    

                                    retorno.Objeto = new ConsultaCreditoApiResponse
                                    {
                                        Pontos = pontos,
                                        Status = HtmlEnumExtensions.GetEnumDescription(statusConsulta),
                                        Retorno_Consulta = consultaSerasa.consulta,
                                        Id = consultaSerasa.Id,
                                        Tipo_Integracao = IntegracaoExterna.SERASA
                                    };

                                    break;
                                }
                            case IntegracaoExterna.SERASA_EXPERIAN:
                                {
                                    var consultaCreditoBusiness =
                                        new ConsultaCreditoBusiness(
                                            repoConsulta,
                                            repoCriterio,
                                            repoParams,
                                            repoConfig,
                                            usuario,
                                            senha,
                                            url
                                        );

                                    var urlBaseToken = CONSTANTES.UrlBaseSerasaAuth();
                                    var urlBase = CONSTANTES.UrlBaseSerasaExperian();

                                    var authToken = await consultaCreditoBusiness.ObterTokenAuth(urlBaseToken);

                                    if (string.IsNullOrEmpty(authToken))
                                        throw new Exception("Não foi possível obter o token de autenticação da Serasa Experian.");

                                    consultaSerasa = await consultaCreditoBusiness.ConsultarSerasaPF(cpf, authToken, urlBase);
                                    statusConsulta = (consultaCreditoBusiness.GetStatusPFSerasaExperian(consultaSerasa.consulta, consultaSerasa.Id, cpf)).valor;

                                    retorno.Objeto = new ConsultaCreditoApiResponse
                                    {
                                        Pontos = 0, //precisa verificar
                                        Status = HtmlEnumExtensions.GetEnumDescription(statusConsulta), // precisa verificar
                                        Retorno_Consulta = consultaSerasa.consulta,
                                        Id = consultaSerasa.Id,
                                        Tipo_Integracao = IntegracaoExterna.SERASA_EXPERIAN
                                    };

                                    break;
                                }
                                
                            case IntegracaoExterna.PREQUAL:                            
                                {

                                    string token = configIntegracaoSerasa.Token;
                                    string apiKey = configIntegracaoSerasa.ApiKey;
                                    string apiSecret = configIntegracaoSerasa.ApiSecret;
                                    string domain = configIntegracaoSerasa.TokenAcesso;
                                    string correlationId = configIntegracaoSerasa.HeaderIntegracaoExterna1;

                                    var ConsultaCreditoPreQualBusiness =
                                            new ConsultaCreditoPreQualBusiness(
                                                repoConsulta,
                                                repoCriterio,
                                                repoParams,
                                                repoPrequalParams,
                                                repoConfig,
                                                repoIntegracao,
                                                repoCliente,
                                                repoVendaNH,
                                                usuario,
                                                senha,
                                                url,
                                                apiKey,
                                                apiSecret,
                                                domain,
                                                correlationId
                                            );

                                    var identityUser = new IdentityUser();
                                    var cliente = await repositoryCliente.selectFirst(p => p.CpfCnpj == cpf);

                                    if (request.IdVendedor.HasValue())
                                    {
                                        identityUser = await repoIdentity.selectFirst(p => p.Funcionario.Id == Convert.ToInt64(request.IdVendedor));

                                    }
                                    else
                                    {
                                        identityUser = await repoIdentity.selectFirst(p => p.Id == cliente.UsuarioCriacao);
                                    }

                                    consultaSerasa = await ConsultaCreditoPreQualBusiness.ExecutarConsultaCreditoPF(cpf, identityUser, Convert.ToInt64(request.IdVendedor));

                                    var consultaCreditoBusinessSerasaPrequal =
                                        new ConsultaCreditoBusiness(
                                            repoConsulta,
                                            repoCriterio,
                                            repoParams,
                                            repoConfig
                                        );
                                
                                    statusConsulta = consultaCreditoBusinessSerasaPrequal.GetStatusConsultaCreditoCpfByConsulta(consultaSerasa.Id, cpf).valor;

                                    retorno.Objeto = new ConsultaCreditoApiResponse
                                    {
                                        Pontos = 0,
                                        Status = HtmlEnumExtensions.GetEnumDescription(statusConsulta),
                                        Retorno_Consulta = consultaSerasa.consulta,
                                        Id = consultaSerasa.Id,
                                        Tipo_Integracao = IntegracaoExterna.PREQUAL,
                                        ParamPreQual = consultaSerasa.ParamLoja
                                    };

                                    break;
                            
                                }                                                                                                                             
                            default:
                                throw new Exception("Integração com o Serasa não configurada");
                        }

                        if (request.IdVenda != null && consultaSerasa != null) 
                        {
                            IEnderecoClienteRepository repoEnderecoCliente = new EnderecoClienteRepository(sessionNh);
                            IVendaRepository repositoryVenda = new VendaRepository(sessionNh);
                            var vendabusiness = new VendaBusiness(repositoryVenda, repoEnderecoCliente);
                            IEtapaVendaRepository repoEtapa = new EtapaVendaRepository(sessionNh);
                            var business = new EtapaVendaBusiness(repoEtapa, repoConfig);
                            var venda = vendabusiness.GetVendaById(request.IdVenda.Value);

                            vendabusiness.GravarIdConsultaRestricao(venda, EnumEtapaVenda.ConsultaCredito, consultaSerasa.Id);

                            if (statusConsulta == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVADO ||
                               statusConsulta == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_SEM_INSTALACAO ||
                               statusConsulta == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_SEM_INSTALACAO)
                            {
                                business.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaCredito, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado);
                            }
                            else if (statusConsulta == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR)
                            {
                                business.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaCredito, Sinapse.Model.enumerados.StatusEtapaVenda.AguardandoAprovacaoGestor);
                            }
                            else 
                            {
                                business.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaCredito, Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado);
                            }
                        }

                        retorno.Ok = true;
                    }
                    else
                    {
                        throw new Exception("Integração com o Serasa não configurada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        } 
        
        [HttpPost]
        [Route("api/ConsultaCreditoApi/ConsultaCreditoByCnpj")]
        public async Task<GetApiRetorno<ConsultaCreditoApiResponse>> ConsultaCreditoByCnpj(ConsultaCreditoByCnpjApiRequest request)
        {
            var retorno = new GetApiRetorno<ConsultaCreditoApiResponse>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repoIntegracoes = new Conexao<IntegracoesExternas>(sessionNh))
                using (var repositoryCliente = new Conexao<Cliente>(sessionNh))
                using (var repoIdentity = new Conexao<IdentityUser>(sessionNh))
                using (var repoVenda = new Conexao<Venda>(sessionNh))
                using (var usuarioApi = new UsuarioApiController())
                {
                    if (request == null)
                    {
                        throw new Exception("Parâmetros da requisição não informados");
                    }

                    if (request.settings != null && !usuarioApi.GetUsuarioValido(request.settings).Result.Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Inválidos");
                    }

                    if (String.IsNullOrEmpty(request.Cnpj))
                    {
                        throw new Exception("CNPJ não informado para a consulta de crédito");
                    }

                    IntegracoesExternas configIntegracaoSerasa = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SERASA || p.IntegracaoExterna == IntegracaoExterna.PREQUAL || p.IntegracaoExterna == IntegracaoExterna.SERASA_EXPERIAN).Result;

                    if (configIntegracaoSerasa != null)
                    {
                        string cnpj = request.Cnpj.Replace(".", String.Empty)
                                                  .Replace("-", String.Empty)
                                                  .Replace("/", String.Empty);

                        Sinapse.Model.ConsultaSerasaDados consultaSerasa = null;
                        Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusConsulta;

                        string usuario = configIntegracaoSerasa.Usuario;
                        string senha = configIntegracaoSerasa.Senha;
                        string url = configIntegracaoSerasa.UrlApi;

                        IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                        ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                        IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                        IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                        IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                        IVendaRepository repoVendaNH = new VendaRepository(sessionNh);
                        IParametrosConsultaPreQualRepository repoPrequalParams = new ParametrosConsultaPreQualRepository(sessionNh);
                        Repository.IntegracaoExterna.IIntegracaoExternaRepository repoIntegracao = new Repository.IntegracaoExterna.IntegracaoExternaRepository(sessionNh);

                        switch (configIntegracaoSerasa.IntegracaoExterna)
                        {
                            case IntegracaoExterna.SERASA:
                                {
                                    var consultaCreditoBusiness =
                                new ConsultaCreditoBusiness(
                                    repoConsulta,
                                    repoCriterio,
                                    repoParams,
                                    repoConfig,
                                    usuario,
                                    senha,
                                    url
                                );

                                    consultaSerasa = await consultaCreditoBusiness.ExecutarConsultaCreditoCnpj(cnpj, User.Identity.GetUserName());
                                    int pontos = consultaCreditoBusiness.GetTotalPontosConsultaCnpj(consultaSerasa.Id, cnpj);
                                    statusConsulta = consultaCreditoBusiness.GetStatusConsultaCreditoCnpj(cnpj);

                                    retorno.Objeto = new ConsultaCreditoApiResponse
                                    {
                                        Pontos = pontos,
                                        Status = HtmlEnumExtensions.GetEnumDescription(statusConsulta),
                                        Retorno_Consulta = consultaSerasa.consulta,
                                        Id = consultaSerasa.Id
                                    };
                                    break;
                                }
                            case IntegracaoExterna.SERASA_EXPERIAN:
                                {
                                    var consultaCreditoBusiness =
                                        new ConsultaCreditoBusiness(
                                            repoConsulta,
                                            repoCriterio,
                                            repoParams,
                                            repoConfig,
                                            usuario,
                                            senha,
                                            url
                                        );

                                    var urlBaseToken = CONSTANTES.UrlBaseSerasaAuth();
                                    var urlBase = CONSTANTES.UrlBaseSerasaExperian();

                                    var authToken = await consultaCreditoBusiness.ObterTokenAuth(urlBaseToken);

                                    if (string.IsNullOrEmpty(authToken))
                                        throw new Exception("Não foi possível obter o token de autenticação da Serasa Experian.");

                                    consultaSerasa = await consultaCreditoBusiness.ConsultarSerasaPJ(cnpj, authToken, urlBase);

                                    statusConsulta = (consultaCreditoBusiness.GetStatusPJSerasaExperian(consultaSerasa.consulta, consultaSerasa.Id, cnpj)).valor;

                                    retorno.Objeto = new ConsultaCreditoApiResponse
                                    {
                                        Pontos = 0, //precisa verificar
                                        Status = HtmlEnumExtensions.GetEnumDescription(statusConsulta),
                                        Retorno_Consulta = consultaSerasa.consulta,
                                        Id = consultaSerasa.Id,
                                        Tipo_Integracao = IntegracaoExterna.SERASA_EXPERIAN
                                    };

                                    break;
                                }
                            case IntegracaoExterna.PREQUAL:
                                {
                                    string token = configIntegracaoSerasa.Token;
                                    string apiKey = configIntegracaoSerasa.ApiKey;
                                    string apiSecret = configIntegracaoSerasa.ApiSecret;
                                    string domain = configIntegracaoSerasa.TokenAcesso;
                                    string correlationId = configIntegracaoSerasa.HeaderIntegracaoExterna1;

                                    var ConsultaCreditoPreQualBusiness =
                                            new ConsultaCreditoPreQualBusiness(
                                                repoConsulta,
                                                repoCriterio,
                                                repoParams,
                                                repoPrequalParams,
                                                repoConfig,
                                                repoIntegracao,
                                                repoCliente,
                                                repoVendaNH,
                                                usuario,
                                                senha,
                                                url,
                                                apiKey,
                                                apiSecret,
                                                domain,
                                                correlationId
                                            );

                                    var identityUser = new IdentityUser();
                                    var cliente = await repositoryCliente.selectFirst(p => p.CpfCnpj == cnpj);
                                    if (cliente?.Tipo_Cliente == Tipo_Cliente.Prospecto)
                                    {
                                        identityUser = await repoIdentity.selectFirst(p => p.Funcionario.Num_Fre == cliente.UsuarioCriacao);
                                    }
                                    else if (request.IdVendedor.HasValue())
                                    {
                                        identityUser = await repoIdentity.selectFirst(p => p.Funcionario.Id == Convert.ToInt64(request.IdVendedor));
                                    }

                                    consultaSerasa = await ConsultaCreditoPreQualBusiness.ExecutarConsultaCreditoPJ(cnpj, identityUser, null);

                                    var consultaCreditoBusinessSerasaPrequal =
                                        new ConsultaCreditoBusiness(
                                            repoConsulta,
                                            repoCriterio,
                                            repoParams,
                                            repoConfig
                                        );

                                    statusConsulta = consultaCreditoBusinessSerasaPrequal.GetStatusConsultaCreditoCnpjByConsulta(consultaSerasa.Id, cnpj).valor;

                                    retorno.Objeto = new ConsultaCreditoApiResponse
                                    {
                                        Pontos = 0,
                                        Status = HtmlEnumExtensions.GetEnumDescription(statusConsulta),
                                        Retorno_Consulta = consultaSerasa.consulta,
                                        Id = consultaSerasa.Id,
                                        Tipo_Integracao = IntegracaoExterna.PREQUAL

                                    };

                                    break;
                                }
                            default:
                                throw new Exception("Integração com o Serasa não configurada");
                        }
                        if (request.IdVenda != null && consultaSerasa != null)
                        {
                            IEtapaVendaRepository repoEtapa = new EtapaVendaRepository(sessionNh);
                            var business = new EtapaVendaBusiness(repoEtapa, repoConfig);
                            if (statusConsulta == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVADO ||
                               statusConsulta == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_SEM_INSTALACAO ||
                               statusConsulta == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_SEM_INSTALACAO)
                            {
                                business.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaCredito, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado);
                            }
                            else if (statusConsulta == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR)
                            {
                                business.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaCredito, Sinapse.Model.enumerados.StatusEtapaVenda.AguardandoAprovacaoGestor);
                            }
                            else
                            {
                                business.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaCredito, Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado);
                            }
                        }

                        retorno.Ok = true;
                    }
                    else
                    {
                        throw new Exception("Integração com o Serasa não configurada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }     
    }
}