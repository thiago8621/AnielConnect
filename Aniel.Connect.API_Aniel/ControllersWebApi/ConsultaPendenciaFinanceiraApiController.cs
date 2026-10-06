using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.Azza;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.MK;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.MK.PendenciaEndereco;
using Aniel.Connect.Repository.MK.PendenciaFinanceira;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaEnderecoAniel;
using Aniel.Connect.Repository.PendenciaFinanceira;
using Aniel.Connect.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.Repository.Azza.PendenciaEndereco;
using Aniel.Connect.Repository.Repository.Azza.PendenciaFinanceira;
using Aniel.Connect.Repository.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.Repository.T5.ConfigAnielIntegrator;
using Aniel.Connect.Repository.Venda;
using MySqlX.XDevAPI;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ConsultaPendenciaFinanceiraApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/ConsultaPendenciaFinanceiraApi/GetFaturaByCpfCnpj")]
        public async Task<GetApiRetorno<IEnumerable<ConsultaPendenciasListModel>>> GetFaturaByCpfCnpj(PendenciaFinanceiraRequest request)
        {
            var retorno = new GetApiRetorno<IEnumerable<ConsultaPendenciasListModel>>
            {
                Objeto = new List<ConsultaPendenciasListModel>()
            };

            try
            {
                using (var usuarioApi = new UsuarioApiController())
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repositoryVenda = new Conexao<Venda>(sessionNh))
                using (var repositoryCliente = new Conexao<Cliente>(sessionNh))
                {
                    if (request == null)
                    {
                        throw new Exception("Parâmetros da requisição não informados");
                    }

                    if (request.settings != null && !usuarioApi.GetUsuarioValido(request.settings).Result.Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Inválidos");
                    }

                    if (!request.IdVenda.HasValue && String.IsNullOrEmpty(request.Cpf) && String.IsNullOrEmpty(request.Cnpj))
                    {
                        throw new Exception("Documento (CPF/CNPJ) não informado para a consulta financeira");
                    }

                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    IIntegracaoExternaRepository repository = new IntegracaoExternaRepository(session);
                    var business = new IntegracaoExternaBusiness(repository);

                    IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(session);
                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var pendenciaFinanceiraConnectBusiness =
                        new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                    IPendenciaFinanceiraAnielRepository repoPendenciaFinanceira = new PendenciaFinanceiraAnielRepository(sessionNh);
                    IPendenciaEnderecoAnielRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoAnielRepository(sessionNh);
                    var pendenciasConnectBusiness = new ConsultaPendenciasConnectBusiness(repoPendenciaEnderecoConnect, repoPendenciaFinanceira);

                    string documento;
                    Tipo_Pessoa tipoPessoa;


                    if (request.IdVenda.HasValue)
                    {
                        var venda = await repositoryVenda.selectFirst(p => p.Id == request.IdVenda.Value);
                        var cliente = await repositoryCliente.selectFirst(p => p.Id == venda.Id_Cliente);
                        documento = cliente.CpfCnpj;
                        tipoPessoa = cliente.Tipo_Pessoa.Value;
                    }
                    else if (!String.IsNullOrEmpty(request.Cpf))
                    {
                        documento = request.Cpf;
                        tipoPessoa = Tipo_Pessoa.Fisica;
                    }
                    else
                    {
                        documento = request.Cnpj;
                        tipoPessoa = Tipo_Pessoa.Juridica;
                    }
                    

                    PendenciaFinanceira pendenciaFinanceira = null;

                    if (business.UtilizaMkConsultaPendenciaFinanceira())
                    {
                        if (String.IsNullOrEmpty(CONSTANTES.MKConnectionString))
                        {
                            throw new Exception("Banco do MK Solutions não configurado");
                        }

                        using (var connPg = PostgreSqlUtil.GetConnection(CONSTANTES.MKConnectionString))
                        {
                            IPendenciaEnderecoMkRepository repoPendenciaEnderecoMk = new PendenciaEnderecoMkRepository(connPg);
                            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraMk = new PendenciaFinanceiraMkRepository(connPg);
                            var mkBusiness = new MKBusiness(repoPendenciaEnderecoMk, repoPendenciaFinanceiraMk, null);



                            IEnumerable<PendenciaFinanceiraListModel> pendenciasMk =
                                mkBusiness.GetPendenciasFinanceira(documento);

                            // Gravando a execução da consulta e os dados das pendências no Connect
                            pendenciaFinanceiraConnectBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasMk);

                            retorno.Objeto =
                                pendenciasMk
                                    .Select(p => new ConsultaPendenciasListModel
                                    {
                                        CodFatura = p.CodFatura,
                                        Cod_Contrato = p.Cod_Contrato,
                                        Data_Vencimento = p.Data_Vencimento,
                                        Valor_Total = p.Valor_Total
                                    });
                            retorno.Ok = true;
                        }
                    }
                    else if (business.UtilizaIXCConsultaPendenciaFinanceira())
                    {
                        IConfigAnielIntegratorRepository _repoConfigAnielIntegrator = new ConfigAnielIntegratorRepository(sessionT5);
                        var objIntegrator = await _repoConfigAnielIntegrator.ObterConfigAnielIntegrator();
                        if (objIntegrator != null)
                        {
                            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraIXC = new PendenciaFinanceiraIXCRepository(objIntegrator.URL, objIntegrator.Token);
                            ICidadeRepository cidadeRepository = new CidadeRepository(sessionT5);

                            IPendenciaEnderecoIntegracaoRepository repoPendenciaEnderecoIXC = new PendenciaEnderecoIntegracaoRepository(objIntegrator.URL, objIntegrator.Token, cidadeRepository);
                            var azzaBusiness = new IXCBusiness(repoPendenciaFinanceiraIXC, null, repoPendenciaEnderecoIXC);


                            IEnumerable<PendenciaFinanceiraListModel> pendenciasIXC =
                               azzaBusiness.GetPendenciasFinanceira(documento);

                            pendenciaFinanceira =
                                pendenciaFinanceiraConnectBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasIXC);

                            retorno.Objeto =
                              pendenciasIXC
                                  .Select(p => new ConsultaPendenciasListModel
                                  {
                                      CodFatura = p.CodFatura,
                                      Cod_Contrato = p.Cod_Contrato,
                                      Data_Vencimento = p.Data_Vencimento,
                                      Valor_Total = p.Valor_Total
                                  });

                        }
                        else
                        {
                            throw new Exception("Integração IXC não está configurada");
                        }
                    }
                    else if (business.UtilizaAlcansConsultaPendenciaFinanceira())
                    {
                        using var integracoesExternas = new Conexao<IntegracoesExternas>(sessionNh);
                        var configIntegracao = await integracoesExternas.selectFirst(x => x.TipoIntegracao == Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_FINANCEIRA && x.IntegracaoExterna == Model.enumerado.IntegracaoExterna.ALCANS);

                        if (configIntegracao != null || !string.IsNullOrEmpty(configIntegracao.Usuario) || !string.IsNullOrEmpty(configIntegracao.Senha) || !string.IsNullOrEmpty(configIntegracao.UrlApi))
                        {
                            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraAlcans = new PendenciaFinanceiraMkAlcansRepository(configIntegracao.Usuario, configIntegracao.Senha, configIntegracao.UrlApi);

                            IEnumerable<PendenciaFinanceiraListModel> pendenciasAlcans = repoPendenciaFinanceiraAlcans.GetPendenciasFinanceira(documento);

                            pendenciaFinanceira = pendenciaFinanceiraConnectBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasAlcans);

                            retorno.Objeto =
                              pendenciasAlcans.Select(p => new ConsultaPendenciasListModel
                              {
                                  CodFatura = p.CodFatura,
                                  Cod_Contrato = p.Cod_Contrato,
                                  Data_Vencimento = p.Data_Vencimento,
                                  Valor_Total = p.Valor_Total
                              });

                        }
                        else
                            throw new Exception("Middleware da Alcans não configurado");
                    }
                    else
                    {
                        IEnumerable<PendenciaFinanceiraListModel> pendenciasConnect =
                            pendenciasConnectBusiness.GetPendenciasFinanceira(documento);

                        pendenciaFinanceira =
                            pendenciaFinanceiraConnectBusiness.GravarPendenciaFinanceira(documento, tipoPessoa, pendenciasConnect);

                        retorno.Objeto =
                              pendenciasConnect.Select(p => new ConsultaPendenciasListModel
                              {
                                  CodFatura = p.CodFatura,
                                  Cod_Contrato = p.Cod_Contrato,
                                  Data_Vencimento = p.Data_Vencimento,
                                  Valor_Total = p.Valor_Total
                              });
                    }


                    if (request.IdVenda != null && pendenciaFinanceira?.Id != null)
                    {
                        IVendaRepository repoVenda = new VendaRepository(session);
                        var vendaBusiness = new VendaBusiness(repoVenda);
                        Venda venda = vendaBusiness.GetVendaById(request.IdVenda.Value);
                        vendaBusiness.GravarIdConsultaRestricao(venda, EnumEtapaVenda.ConsultaPendenciaFinanceira, pendenciaFinanceira.Id);

                        IEtapaVendaRepository repoEtapa = new EtapaVendaRepository(sessionNh);
                        var businessEtapa = new EtapaVendaBusiness(repoEtapa, repoConfig);
                        bool possuiPendencia = retorno.Objeto.Any();
                        if (possuiPendencia)
                        {
                            var config = repoConfig.GetConfiguracoesGerais();
                            if (config.ConsultaPendenciaFinanceiraRestringeVenda)
                            {
                                businessEtapa.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaPendenciaFinanceira, Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado);
                            }
                            else
                            {
                                businessEtapa.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaPendenciaFinanceira, Sinapse.Model.enumerados.StatusEtapaVenda.AguardandoAprovacaoGestor);
                            }
                        }
                        else
                        {
                            businessEtapa.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaPendenciaFinanceira, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado);
                        }
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.ErrorFormat("Message: {0} \r\n InnerException : {1} StackTrace : {2}",
                                ex.Message,
                                ex?.InnerException?.Message,
                                ex.StackTrace);
            }

            return retorno;
        }
    }
}