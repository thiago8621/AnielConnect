using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.MK;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.MK.OrdemServico;
using Aniel.Connect.Repository.Repository.HubSoft;
using Aniel.Connect.Repository.Repository.Log;
using Aniel.Connect.Repository.T5.Area;
using Aniel.Connect.Repository.T5.Cliente;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Localidade;
using Aniel.Connect.Repository.TipoImovel;
using Aniel.Connect.Repository.Usuario;
using Aniel.Connect.Repository.Voalle;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Vml;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using Resources;
using Sinapse.Core;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.IntegracoesERP.Ixc;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Twilio.TwiML.Voice;

namespace Aniel.Connect.ControllersWebApi.T5
{

    [Authorize]
    public class Painel_ServicosApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Painel_ServicosApi/GetOSManutencao")]
        public DataTableResponse<Painel_ServicosListaModel> GetOSManutencao(Painel_Servicos_DataTableRequest request)
        {
            var lst = new List<Documento_ProducaoAniel>();
            var cc = new DataTableResponse<Painel_ServicosListaModel>();

            List<Painel_ServicosListaModel> retorno = new List<Painel_ServicosListaModel>();
            Int32 recordsTotal = 0;
            try
            {

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDocumentoProducaoAnielRepository repo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);

                    lst = new DocumentoProducaoAnielBusiness(repo).GetOSManutencao(Convert.ToDateTime(request.DataInicial), Convert.ToDateTime(request.DataFinal)).ToList();

                }
                foreach (var item in lst)
                {
                    var obj = new Painel_ServicosListaModel();
                    obj.CodCt = item.CodCt;
                    obj.Numero_Obra_Original = item.Numero_Obra_Original;
                    obj.Num_Doc = item.Num_Doc;
                    obj.Projeto = item.Projeto;
                    retorno.Add(obj);
                }

                // Retornando o JSON
                //Page e retornando DataTable
                return new DataTableResponse<Painel_ServicosListaModel>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };

            }
            catch (Exception ex)
            {
                return new DataTableResponse<Painel_ServicosListaModel>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = null,

                    error = ex.Message
                };
            }
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/GetOSIntegracoes")]
        public async Task<PostApiRetorno<List<ResultOSIntegracao>>> GetOSIntegracoes(ObterOSIntegracoes request)
        {
            PostApiRetorno<List<ResultOSIntegracao>> response = new PostApiRetorno<List<ResultOSIntegracao>>();
            List<ResultOSIntegracao> osResults = new List<ResultOSIntegracao>();
            var connectionString = CONSTANTES.VoalleConnectionString;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoDocConexao = new Conexao<Documento_ProducaoAniel>(sessionT5))
                {
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    IConfiguracoesGeraisRepository repoConfigGerais = new ConfiguracoesGeraisRepository(session);
                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    ILocalidadeAnielRepository repoLocalidade = new LocalidadeAnielRepository(sessionT5);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5);
                    ILogRepository repoLog = new LogRepository(sessionLog);
                    List<Config_Aniel_Integrator> configs = await ObterConfigAnielIntegrator(sessionT5);
                    var configEmpresa = repoConfigGerais.GetConfiguracoesGerais();
                    DocumentoProducaoAnielBusiness docBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas dadosIntegracaoVoalle = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.VOALLE);
                    IntegracoesExternas dadosIntegracaoIxc = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.IXC);
                    IntegracoesExternas dadosIntegracaoHubsoft = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.HUBSOFT);
                    IntegracoesExternas dadosIntegracaoMK = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.MK_SOLUTION);
                    LoginHubsoftModel loginHubsoft = new LoginHubsoftModel();

                    if (dadosIntegracaoVoalle is null && dadosIntegracaoIxc is null && dadosIntegracaoHubsoft is null && dadosIntegracaoMK is null)
                    {
                        string message = "Integração não foi configurada corretamente";
                        throw new Exception(message);
                    }
                    else
                    {
                        if (dadosIntegracaoVoalle != null)
                        {
                            if (String.IsNullOrEmpty(dadosIntegracaoVoalle.UrlApi) ||
                                String.IsNullOrEmpty(dadosIntegracaoVoalle.TokenAcesso) ||
                                String.IsNullOrEmpty(dadosIntegracaoVoalle.Senha))
                            {
                                string message = "Integração com a API da Voalle não foi configurada corretamente, confira os acessos.";
                                await repoLog.InserirLogIntegracaoOS("-", message, MomentoImportacao.Integracao_Manual);
                                throw new Exception(message);
                            }
                        }
                        else if (dadosIntegracaoIxc != null)
                        {
                            if (String.IsNullOrEmpty(dadosIntegracaoIxc.UrlApi) ||
                                String.IsNullOrEmpty(dadosIntegracaoIxc.TokenAcesso))
                            {
                                string message = "Integração com a API do IXC não foi configurada corretamente, confira os acessos.";
                                await repoLog.InserirLogIntegracaoOS("-", message, MomentoImportacao.Integracao_Manual);
                                throw new Exception(message);
                            }
                        }
                        else if (dadosIntegracaoHubsoft != null)
                        {
                            if (String.IsNullOrEmpty(dadosIntegracaoHubsoft.UrlApi) ||
                                String.IsNullOrEmpty(dadosIntegracaoHubsoft.Usuario) ||
                                String.IsNullOrEmpty(dadosIntegracaoHubsoft.Senha) ||
                                String.IsNullOrEmpty(dadosIntegracaoHubsoft.ApiSecret) ||
                                String.IsNullOrEmpty(dadosIntegracaoHubsoft.TokenAcesso))
                            {
                                string message = "Integração com a API do HubSoft não foi configurada corretamente, confira os acessos.";
                                await repoLog.InserirLogIntegracaoOS("-", message, MomentoImportacao.Integracao_Manual);
                                throw new Exception(message);
                            }

                            loginHubsoft = new LoginHubsoftModel
                            {
                                client_id = dadosIntegracaoHubsoft.TokenAcesso,
                                client_secret = dadosIntegracaoHubsoft.ApiSecret,
                                username = dadosIntegracaoHubsoft.Usuario,
                                password = dadosIntegracaoHubsoft.Senha
                            };
                        }
                        else if (dadosIntegracaoMK != null)
                        {
                            if (String.IsNullOrEmpty(CONSTANTES.MKConnectionString))
                            {
                                string message = "Integração com o banco do MK Solutions não foi configurada corretamente, confira a connection string.";
                                await repoLog.InserirLogIntegracaoOS("-", message, MomentoImportacao.Integracao_Manual);
                                throw new Exception(message);
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(request.OrdensServicos))
                    {
                        string message = "Não há Ordens de Serviços a serem importadas.";
                        CONSTANTES.log4Net.Info(message);
                        throw new Exception(message);
                    }

                    var numerosOrdensServicos = request.OrdensServicos.Split(',');
                    foreach (var num in numerosOrdensServicos)
                    {
                        var hasDocGravado = await repoDocConexao.selectFirst(d => d.Numero_Obra_Original == num.Trim());

                        if (hasDocGravado != null)
                        {
                            string message = $"A OS Protocolo/NumObra: {num} já consta gravado no sistema.";
                            osResults.Add(new ResultOSIntegracao { NumOrdemServico = num, Mensagem = message, Situacao = false });
                            CONSTANTES.log4Net.Info(message);
                            continue;
                        }

                        if (dadosIntegracaoVoalle != null)
                        {
                            IVoalleRepository voalleRepository = new VoalleApiRepository(dadosIntegracaoVoalle);
                            VoalleBusiness voalleBusiness = new VoalleBusiness(null, voalleRepository);
                            var voalleOs = voalleBusiness.ObterOrdemServico(connectionString, num);
                            if (voalleOs == null)
                            {
                                string message = $"A OS Protocolo/NumObra: {num} não foi encontrado.";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = num, Mensagem = message, Situacao = false });
                                CONSTANTES.log4Net.Info(message);
                                continue;
                            }

                            if (voalleOs.incident_status_id == 3 || voalleOs.incident_status_id == 4 || voalleOs.incident_status_id == 8)
                            {
                                string message = $"A OS Protocolo/NumObra: {num} já se encontra fechada no Voalle.";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = num, Mensagem = message, Situacao = false });
                                CONSTANTES.log4Net.Info(message);
                                continue;
                            }

                            var configVoalle = configs.FirstOrDefault(p => p.UF == voalleOs.UF);

                            var dadosLocalidade = repoLocalidade.GetByLocalidade(voalleOs.Cidade, voalleOs.Bairro, configVoalle);

                            string descricaoEscritorio = dadosLocalidade?.DescricaoEscritorio;
                            string descricaoArea = dadosLocalidade?.DescricaoArea;

                            if (descricaoEscritorio == null)
                            {
                                var area = repoArea.GetByCidade(voalleOs.Cidade);
                                descricaoEscritorio = area?.descricaoEscritorio;
                                descricaoArea = area?.descricaoArea;
                            }

                            var servico = voalleBusiness.BindObject(voalleOs, dadosLocalidade, configEmpresa, configVoalle, descricaoEscritorio, descricaoArea);
                            Response gravarOsResponse = new Response();
                            try
                            {

                                gravarOsResponse = await docBusiness.CriarOsIntegracao(servico);
                            }
                            catch (Exception erro)
                            {

                                gravarOsResponse.message = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                                gravarOsResponse.result = false;
                            }

                            if (String.IsNullOrEmpty(gravarOsResponse.message) == false)
                            {
                                string message = $"{gravarOsResponse.message}";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = servico.NumOS, Mensagem = message, Situacao = true });
                                CONSTANTES.log4Net.Info(message);
                            }
                            else if (!gravarOsResponse.result)
                            {
                                string message = $"Ocorreu um erro durante criacao de servico : OS {servico.NumOS}";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = servico.NumOS, Mensagem = message, Situacao = false });
                                await repoLog.InserirLogIntegracaoOS(servico.NumOS, message, MomentoImportacao.Integracao_Manual);
                                CONSTANTES.log4Net.Error(message);
                            }
                            else
                            {
                                string message = $"OS {servico.NumOS} cadastrada com sucesso";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = servico.NumOS, Mensagem = message, Situacao = true });
                                CONSTANTES.log4Net.Info(message);
                            }

                        }
                        else if (dadosIntegracaoIxc != null)
                        {
                            var config = configs.FirstOrDefault();
                            ITurnoRepository repoTurno = new TurnoRepository(sessionT5);
                            IClienteAnielRepository repoClienteAniel = new ClienteAnielRepository(sessionT5);
                            IIxcRepository ixcRepository = new IxcRepository(config.URL, config.Token, configEmpresa, repoDocProd);
                            IxcHelper ixcHelper = new IxcHelper(config.URL, config.Token);
                            IxcBusiness ixcBusiness = new IxcBusiness(ixcRepository, repoTurno, repoClienteAniel, config, ixcHelper);
                            var ixcOs = ixcBusiness.ObterOrdemServico(num);
                            if (ixcOs == null)
                            {
                                string message = $"A OS Protocolo/NumObra: {num} não foi encontrado.";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = num, Mensagem = message, Situacao = false });
                                CONSTANTES.log4Net.Info(message);
                                continue;
                            }
                            ixcBusiness.ImportarOs(ixcOs);
                        }
                        else if (dadosIntegracaoHubsoft != null)
                        {
                            var config = configs.FirstOrDefault();
                            var ordemServicoHubSoft = OrdemServicoHubSoftDao.GetOrdemServico(dadosIntegracaoHubsoft.UrlApi, num, loginHubsoft);

                            if (ordemServicoHubSoft == null)
                            {
                                string message = $"A OS {num} não foi encontrada no HubSoft";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = num, Mensagem = message, Situacao = false });
                                CONSTANTES.log4Net.Info(message);
                                continue;
                            }

                            if (ordemServicoHubSoft.cliente_servico == null)
                            {
                                throw new Exception($"OS {ordemServicoHubSoft.numero_ordem_servico} não possui cliente vinculado");
                            }

                            var dadosLocalidade = repoLocalidade.GetByCidadeBairro(ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.cidade.nome, ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.cidade.nome);

                            string descricaoEscritorio = dadosLocalidade?.DescricaoEscritorio;
                            string descricaoArea = dadosLocalidade?.DescricaoArea;

                            if (descricaoEscritorio == null)
                            {
                                var area = repoArea.GetByCidade(ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.cidade.nome);
                                descricaoEscritorio = area?.descricaoEscritorio;
                                descricaoArea = area?.descricaoArea;
                            }

                            var clienteHubSoft = ClienteDao.GetCliente(dadosIntegracaoHubsoft.UrlApi, loginHubsoft, ordemServicoHubSoft.cliente_servico.cliente.codigo_cliente).clientes[0];

                            var enderecoPreenchido = ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero != null;

                            var dadosLoginRadius = OrdemServicoHubSoftDao.GetLoginRadiusByOS(dadosIntegracaoHubsoft.UrlApi, num, loginHubsoft);

                            string loginRadius = null;
                            string senhaRadius = null;

                            if (dadosLoginRadius != null && !String.IsNullOrEmpty(dadosLoginRadius.autenticacao_servico.login))
                            {
                                loginRadius = dadosLoginRadius.autenticacao_servico.login;
                                senhaRadius = dadosLoginRadius.autenticacao_servico.password;

                            }

                            string bairro = ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.bairro;
                            var usuario = Regex.Unescape(ordemServicoHubSoft.usuario_abertura.name);

                            //commit feito direto na branch hotfixBreakingCanges

                            var servico = new OsApiModel
                            {
                                NumOS = ordemServicoHubSoft.numero_ordem_servico.ToString(),
                                Endereco = !string.IsNullOrEmpty(ordemServicoHubSoft.cliente_servico.endereco_numero_completo) ? ordemServicoHubSoft.cliente_servico.endereco_numero_completo.Replace("\"", "") : "",
                                NumeroEndereco = enderecoPreenchido ? ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.numero : "",
                                DescricaoPlanoProduto = !string.IsNullOrEmpty(ordemServicoHubSoft.cliente_servico.servico.descricao) ? ordemServicoHubSoft.cliente_servico.servico.descricao : "",
                                Complemento = enderecoPreenchido ? ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.complemento : "",
                                Bairro = enderecoPreenchido ? bairro.Length > 30 ? bairro.Substring(0, 30) : bairro : "",
                                Cidade = enderecoPreenchido ? ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.cidade.nome : "",
                                Cep = enderecoPreenchido ? ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.cep : "",
                                UF = enderecoPreenchido ? ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.cidade.estado.sigla : "",
                                NomeCliente = !string.IsNullOrEmpty(ordemServicoHubSoft.cliente_servico.cliente.nome_razaosocial) ? ordemServicoHubSoft.cliente_servico.cliente.nome_razaosocial : "",
                                TipoServico = ordemServicoHubSoft.tipo_ordem_servico.descricao,
                                Cpf = clienteHubSoft.cpf_cnpj,
                                Observacao = ordemServicoHubSoft.descricao_abertura,
                                Adicional1 = ordemServicoHubSoft.id_ordem_servico.ToString(),
                                CodCt = config.CodCtPadrao,
                                Projeto = config.ProjetoPadrao,
                                Uni = config.Unidade_Custo,
                                ContratoCliente = Convert.ToString(ordemServicoHubSoft.cliente_servico.id_cliente_servico),
                                Num_Doc = Convert.ToString(ordemServicoHubSoft.cliente_servico.id_cliente),
                                TelefoneCelularCliente = clienteHubSoft.telefone_primario,
                                TelefoneFixoCliente = clienteHubSoft.telefone_secundario,
                                GrupoArea = descricaoEscritorio,
                                Area = descricaoArea,
                                Localidade = dadosLocalidade?.LocalidadeStr,
                                Latitude = Convert.ToString(ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.coordenadas?.coordinates[1]),
                                Longitude = Convert.ToString(ordemServicoHubSoft.cliente_servico.endereco_instalacao.endereco_numero.coordenadas?.coordinates[0]),
                                CodigoAuxiliarLogin = Convert.ToString(ordemServicoHubSoft.cliente_servico.id_cliente_servico),
                                LoginRadius = loginRadius,
                                SenhaRadius = senhaRadius,
                                UsuarioIntegracao = usuario.Length > 10 ? usuario.Substring(0, 10) : usuario,
                                Plano = new PlanoRequest
                                {
                                    Descricao = !string.IsNullOrEmpty(ordemServicoHubSoft.cliente_servico.servico.descricao) ? ordemServicoHubSoft.cliente_servico.servico.descricao : "",
                                    Login = loginRadius,
                                    Senha = senhaRadius,
                                },
                            };

                            if (clienteHubSoft.servicos.Count > 0)
                            {
                                var tecnologia = clienteHubSoft.servicos.FirstOrDefault(p => p.id_cliente_servico == ordemServicoHubSoft.id_cliente_servico || p.id_cliente_servico == ordemServicoHubSoft.cliente_servico?.id_servico)?.tecnologia;
                                if (!string.IsNullOrEmpty(tecnologia))
                                    servico.Tecnologia = tecnologia.ToUpperInvariant();
                            }

                            Response gravarOsResponse = new Response();
                            try
                            {
                                gravarOsResponse = await docBusiness.CriarOsIntegracao(servico);
                            }
                            catch (Exception erro)
                            {
                                gravarOsResponse.message = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                                gravarOsResponse.result = false;
                            }

                            if (String.IsNullOrEmpty(gravarOsResponse.message) == false)
                            {
                                string message = $"{gravarOsResponse.message}";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = servico.NumOS, Mensagem = message, Situacao = true });
                                CONSTANTES.log4Net.Info(message);
                            }
                            else if (!gravarOsResponse.result)
                            {
                                string message = $"Ocorreu um erro durante criacao de servico : OS {servico.NumOS}";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = servico.NumOS, Mensagem = message, Situacao = false });
                                await repoLog.InserirLogIntegracaoOS(servico.NumOS, message, MomentoImportacao.Integracao_Manual);
                                CONSTANTES.log4Net.Error(message);
                            }
                            else
                            {
                                string message = $"OS {servico.NumOS} cadastrada com sucesso";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = servico.NumOS, Mensagem = message, Situacao = true });
                                CONSTANTES.log4Net.Info(message);
                            }
                        }
                        else if (dadosIntegracaoMK != null)
                        {
                            var config = configs.FirstOrDefault();
                            IOrdemServicoMkRepository repoOrdemServicoMk = new OrdemServicoMkRepository();
                            MKBusiness mkBusiness = new MKBusiness(null, null, null, repoOrdemServicoMk);
                            var mkOs = mkBusiness.ObterOrdemServico(CONSTANTES.MKConnectionString, num);

                            if (mkOs == null)
                            {
                                string message = $"A OS Protocolo/NumObra: {num} não foi encontrado.";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = num, Mensagem = message, Situacao = false });
                                CONSTANTES.log4Net.Info(message);
                                continue;
                            }

                            if (mkOs.Encerrado == "S")
                            {
                                string message = $"A OS Protocolo/NumObra: {num} já se encontra fechada no MK.";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = num, Mensagem = message, Situacao = false });
                                CONSTANTES.log4Net.Info(message);
                                continue;
                            }

                            var dadosLocalidade = repoLocalidade.GetByLocalidade(mkOs.Cidade, mkOs.Bairro, config);

                            string descricaoEscritorio = dadosLocalidade?.DescricaoEscritorio;
                            string descricaoArea = dadosLocalidade?.DescricaoArea;

                            if (descricaoEscritorio == null)
                            {
                                var area = repoArea.GetByCidade(mkOs.Cidade);
                                descricaoEscritorio = area?.descricaoEscritorio;
                                descricaoArea = area?.descricaoArea;
                            }

                            var servico = mkBusiness.BindObject(mkOs, dadosLocalidade, configEmpresa, config, descricaoEscritorio, descricaoArea);
                            Response gravarOsResponse = new Response();
                            try
                            {
                                gravarOsResponse = await docBusiness.CriarOsIntegracao(servico);
                            }
                            catch (Exception erro)
                            {
                                gravarOsResponse.message = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                                gravarOsResponse.result = false;
                            }

                            if (String.IsNullOrEmpty(gravarOsResponse.message) == false)
                            {
                                string message = $"{gravarOsResponse.message}";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = servico.NumOS, Mensagem = message, Situacao = true });
                                CONSTANTES.log4Net.Info(message);
                            }
                            else if (!gravarOsResponse.result)
                            {
                                string message = $"Ocorreu um erro durante criacao de servico : OS {servico.NumOS}";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = servico.NumOS, Mensagem = message, Situacao = false });
                                await repoLog.InserirLogIntegracaoOS(servico.NumOS, message, MomentoImportacao.Integracao_Manual);
                                CONSTANTES.log4Net.Error(message);
                            }
                            else
                            {
                                string message = $"OS {servico.NumOS} cadastrada com sucesso";
                                osResults.Add(new ResultOSIntegracao { NumOrdemServico = servico.NumOS, Mensagem = message, Situacao = true });
                                CONSTANTES.log4Net.Info(message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message);
                response.Mensagem = ex.Message;
                response.Ok = false;
                return response;
            }

            response.IdObjeto = osResults.OrderByDescending(x => x.Situacao).ToList();
            response.Ok = true;
            return response;
        }

        public class requestTeste
        {
            public string nodeId { get; set; }
        }


        [HttpPost()]
        [Route("api/Painel_ServicosApi/CheckOSManutencao")]
        public async Task<PostApiRetorno<List<string>>> CheckOSManutencao(CheckOsManutencao listOsNumeros)
        {
            PostApiRetorno<List<string>> response = new PostApiRetorno<List<string>>();
            List<string> isOsManutencao = new List<string>();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDocumentoProducaoAnielRepository docProdRepo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);

                    foreach (var item in listOsNumeros.osNumeros)
                    {
                        var docProd = await docProdRepo.isOsManutencao(item);

                        if (docProd)
                            isOsManutencao.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = ex.Message;
                return response;
            }

            response.Ok = true;
            response.IdObjeto = isOsManutencao;
            return response;
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/GetAll")]
        public async Task<DataTableResponse<Painel_ServicosListaModel>> GetAll(Painel_Servicos_DataTableRequest request)
        {

            List<Painel_ServicosListaModel> retorno = new List<Painel_ServicosListaModel>();
            Int32 recordsTotal = 0;
            try
            {
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var documentoApi = new Documento_ProducaoApiController())
                using (var api = new AcessoApiController())
                {
                    IUsuarioRepository repoUsuario = new UsuarioRepository(sessionConnect);
                    var usuarioBusiness = new UsuarioBusiness(repoUsuario);

                    IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                    var config = await new Conexao<Configuracoes_Gerais>(sessionConnect).selectFirst();
                    var configSla = await new Conexao<Master>(session).selectFirst();

                    Boolean Exibe_RDOs = config.ExibeRDOs;
                    Enum_Tipo_Sla_ SLA_Data_Abertura = configSla.SLA_Data_Abertura;

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    GetApiRetorno<List<Models.T5.AreaModuloModel>> areasAcesso = await api.GetArea_ModuloUsuario(User.Identity.GetUserId());
                    bool temAcesso360 = Core.Helpers.AcessoHelper.TemAcesso(areasAcesso.Objeto, "360");

                    string where = "WHERE A.ULTIMO_SEQ = 'S'";
                    if (request.UtilizaHistoricoReabertura)
                    {
                        where = " WHERE 1=1 ";
                    }

                    StringBuilder sqlJoin = new StringBuilder(@$"FROM TB_DOCUMENTO_PRODUCAO A                  
                    LEFT JOIN TB_EQUIPE B 
                    ON(A.EQUIPE = B.EQUIPE) 
                    LEFT JOIN TB_ESCRITORIO C 
                    ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO) 
                    LEFT JOIN TB_AREA D 
                    ON(A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) 
                    LEFT JOIN TB_LOCALIDADE E 
                    ON(A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND 
                       A.COD_LOCALIDADE = E.COD_LOCALIDADE) 
                    LEFT JOIN TB_TIPO_SERVICO_EQUIPE F 
                    ON(F.CODCT = A.CODCT AND A.COD_TIPO_SERV = F.COD_TIPO_SERV) 
                    LEFT JOIN TB_SUB_TIPO_SERVICO G 
                    ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV) 
                    LEFT JOIN TB_CLIENTE H 
                    ON(A.ID_CLIENTE = H.COD_CLIENTE) 
                    LEFT JOIN TB_CODIGO_ENCERRAMENTO I 
                    ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO) 
                    LEFT JOIN TB_TIPO_IMOVEL J 
                    ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) 
                    LEFT JOIN TB_FUNCIONARIO K 
                    ON(K.NUM_FRE = B.COD_SUPERV) 
                    LEFT JOIN TB_NATUREZA_PRODUCAO L 
                    ON(L.ID = F.COD_NATUREZA) 
                    LEFT JOIN TB_SEGMENTO_SERVICO S 
                    ON(S.CODIGO = A.SEGMENTO)
                    LEFT JOIN TB_CATEGORIA_CLIENTE_CONNECT M 
                    ON(M.ID = H.ID_CATEGORIA) 
                    LEFT JOIN TB_PLANO_PRODUTO N
                    ON(N.ID = A.ID_PLANOPRODUTO)
                    LEFT JOIN TB_CONTRATO O
                    ON(O.CODCT = A.CODCT)
                    LEFT JOIN SP_CONNECT_TOTAL_PONTOS(A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC) P
                    ON(A.CODCT = P.RCODCT AND A.PROJETO = P.RPROJETO AND A.NUM_DOC = P.RNUM_DOC AND A.NUM_OBRA = P.RNUM_OBRA)
                    LEFT JOIN TB_TURNO_SERVICO Q
                    ON(Q.ID = A.COD_TURNO_SERV)
                    LEFT JOIN TB_FOLHA_DEPTO R
                    ON(R.COD_DEPTO = A.COD_DEPTO) " + where);

                    StringBuilder sqlWhere = new StringBuilder(@"");
                    if (!Exibe_RDOs)
                        sqlWhere.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                    if (!String.IsNullOrEmpty(request.Cidade))
                        request.Cidade = request.Cidade.Replace("'", "''"); //escape com aspas simples para pegar as cidades

                    var filtros_modelo = GetModeloFiltros(request.Id_Modelo, request);

                    if (request.ExibirSomenteOSComVenda)
                    {
                        sqlWhere.AppendLine(" AND A.ID_VENDA IS NOT NULL ");
                    }

                    if (request.Id_Venda.HasValue)
                    {
                        sqlWhere.AppendLine(" AND A.ID_VENDA = @ID_VENDA ");
                        parametros.Add("@ID_VENDA", request.Id_Venda.Value);
                    }
                    if (!String.IsNullOrEmpty(request.Tipo))
                    {
                        sqlWhere.AppendLine(" AND A.TIPO = @TIPO ");
                        parametros.Add("@TIPO", request.Tipo);
                    }

                    #region Filtro Perfil Visualização de Serviço

                    if (user.PerfilVisualizacaoServico != null)
                    {
                        if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Count() > 0)
                        {
                            var filtrosContratoProj = new List<string>();

                            foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto)
                            {
                                if (!String.IsNullOrEmpty(paramContratoProj.CodCt) && !String.IsNullOrEmpty(paramContratoProj.Projeto))
                                {
                                    filtrosContratoProj.Add($"(A.CODCT = '{paramContratoProj.CodCt}' AND A.PROJETO = '{paramContratoProj.Projeto}')");
                                }
                                else
                                {
                                    filtrosContratoProj.Add($"(A.CODCT = '{paramContratoProj.CodCt}')");
                                }
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosContratoProj)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                        {
                            var filtrosArea = new List<string>();

                            foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea)
                            {
                                if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                   !String.IsNullOrEmpty(paramArea.CodArea) &&
                                   paramArea.CodLocalidade.HasValue)
                                {
                                    filtrosArea.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}' AND A.COD_LOCALIDADE = {paramArea.CodLocalidade.Value})");
                                }
                                else if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                         !String.IsNullOrEmpty(paramArea.CodArea))
                                {
                                    filtrosArea.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}')");
                                }
                                else
                                {
                                    filtrosArea.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}')");
                                }
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosArea)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                        {
                            var filtrosSetor = new List<string>();

                            foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                            {
                                filtrosSetor.Add($"(A.COD_DEPTO = {paramSetor.CodDepto})");
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosSetor)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                        {
                            var filtrosSupervisor = new List<string>();

                            foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                            {
                                filtrosSupervisor.Add($"(K.NUM_FRE = '{paramSupervisor.CodSuperv}')");
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosSupervisor)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                        {
                            var filtrosMonitor = new List<string>();

                            foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                            {
                                filtrosMonitor.Add($"(B.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosMonitor)})");
                        }
                    }

                    #endregion

                    //CPF OU NOME
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sqlWhere.AppendLine("AND (");



                        sqlWhere.AppendLine($" (A.NUM_OBRA_ORIGINAL LIKE @SEARCHNUM_OBRA_ORIGINAL)  ");

                        int numDocSearch = 0;
                        if (int.TryParse(request.search.value, out numDocSearch))
                        {
                            sqlWhere.AppendLine(" OR (A.NUM_DOC LIKE @SEARCH)  ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpperInvariant()));
                        }

                        sqlWhere.AppendLine(" OR (A.OXBDPEE LIKE @SEARCHOXBDPEE) ");
                        sqlWhere.AppendLine(" OR (UPPER(A.TITULAR) LIKE @SEARCHCLIENTE) ");

                        sqlWhere.AppendLine(")");


                        parametros.Add("@SEARCHNUM_OBRA_ORIGINAL", String.Format("%{0}%", StringService.Truncate(request.search.value.ToUpperInvariant(), 48)));
                        parametros.Add("@SEARCHOXBDPEE", String.Format("%{0}%", StringService.Truncate(request.search.value.ToUpperInvariant(), 48)));
                        parametros.Add("@SEARCHCLIENTE", String.Format("%{0}%", StringService.Truncate(request.search.value.ToUpperInvariant(), 98)));
                    }

                    var contratos = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Contrato).
                                                            Select(p => p.valores.Replace("COLUNA", "A.CODCT")).
                                                            ToList();


                    if (request.Id_Modelo == 5)
                    {
                        sqlWhere.AppendLine(" AND (A.EQUIPE IS NULL) ");
                        sqlWhere.AppendLine(" AND (A.DATA_MAXIMA BETWEEN @PDATAINI AND @PDATAEND) ");
                        parametros.Add("@PDATAINI", DateTime.Now.AddDays(-30));
                        parametros.Add("@PDATAEND", DateTime.Now);

                    }
                    if (request.Id_Modelo == 4)
                    {
                        sqlWhere.AppendLine($"AND A.DATA_MAXIMA IS NULL");
                        sqlWhere.AppendLine($"AND (A.DIA BETWEEN '{DateTime.Now.AddDays(-31).ToString("yyyy-MM-dd")}' AND '{DateTime.Now.ToString("yyyy-MM-dd")}')");
                    }
                    if (request.Id_Modelo == 10)
                    {
                        sqlWhere.AppendLine("AND (A.DATA_INDICADOR + A.HORA_INDICADOR < COALESCE(A.DATA_PROD + A.HORA_PROD,current_timestamp))");
                    }
                    if (request.Id_Modelo == 9)
                    {
                        sqlWhere.AppendLine("AND (A.DATA_INDICADOR + A.HORA_INDICADOR > COALESCE(A.DATA_PROD + A.HORA_PROD,current_timestamp))");
                    }
                    if (request.Id_Modelo == 8)
                    {
                        sqlWhere.AppendLine(" AND (A.DATA_MAXIMA BETWEEN @PDATAINI AND @PDATAEND) ");
                        parametros.Add("@PDATAINI", DateTime.Now.AddDays(-30));
                        parametros.Add("@PDATAEND", DateTime.Now);
                    }
                    if (request.Id_Modelo == 6)
                    {
                        sqlWhere.AppendLine(" AND (A.DATA_MAXIMA BETWEEN @PDATAINI AND @PDATAEND) ");
                        parametros.Add("@PDATAINI", DateTime.Now);
                        parametros.Add("@PDATAEND", DateTime.Now);
                    }
                    if (request.Id_Modelo == 100)
                    {
                        sqlWhere.AppendLine("AND ((A.STATUS_EXECUCAO = 4) AND (A.SEQ <= 1 OR A.SEQ IS NULL))");
                    }
                    if (request.Id_Modelo == 101)
                    {
                        sqlWhere.AppendLine("AND (CURRENT_DATE > A.DATA_MAXIMA AND (A.STATUS_EXECUCAO <> 1 AND A.STATUS_EXECUCAO <> 2 AND A.STATUS_EXECUCAO <> 9) OR (A.STATUS_EXECUCAO = 4) AND (A.SEQ > 1))");
                    }



                    if (!String.IsNullOrEmpty(request.HabilidadeNatureza))
                    {
                        if (request.HabilidadeNatureza == "2")
                        {
                            sqlWhere.AppendLine(" AND L.TIPO_HABILIDADE IN (0,1)");
                        }
                        else
                        {
                            sqlWhere.AppendLine(" AND L.TIPO_HABILIDADE = @TIPO_HABILIDADE");
                            parametros.Add("@TIPO_HABILIDADE", request.HabilidadeNatureza);
                        }

                    }

                    if (!String.IsNullOrEmpty(request.Projeto))
                    {
                        sqlWhere.AppendLine(" AND A.PROJETO = @PROJETO");
                        parametros.Add("@PROJETO", request.Projeto);
                    }
                    if (!String.IsNullOrEmpty(request.TipoPrioridade))
                    {
                        sqlWhere.AppendLine(" AND A.TIPO_PRIORIDADE = @TIPOPRIORIDADE");
                        parametros.Add("@TIPOPRIORIDADE", request.TipoPrioridade);
                    }

                    if (request.Id_Modelo == 8)
                    {
                        sqlWhere.AppendLine($" AND (K.NUM_FRE = '{User.Identity.GetUserId()}' OR B.USUARIO_MONITOR = '{User.Identity.GetUserId()}')");
                    }

                    if (contratos.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", contratos)}) ");

                    }

                    var projetos = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Projeto).
                                                            Select(p => p.valores.Replace("COLUNA_PAI", "A.CODCT").Replace("COLUNA", "A.PROJETO")).
                                                            ToList();
                    if (projetos.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", projetos)}) ");

                    }

                    var encerramentos = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Encerramento).
                                                            Select(p => p.valores.Replace("COLUNA_PAI", "A.CODCT").Replace("COLUNA", "A.COD_ENCERRAMENTO")).
                                                            ToList();
                    if (encerramentos.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", encerramentos)}) ");

                    }

                    if (!String.IsNullOrEmpty(request.Id_Monitor))
                    {
                        sqlWhere.AppendLine("AND (COALESCE(B.USUARIO_MONITOR,A.USUARIO_MONITOR) = @MONITOR) ");
                        parametros.Add("@MONITOR", request.Id_Monitor);
                    }

                    if (!String.IsNullOrEmpty(request.Responsabilidade))
                    {
                        sqlWhere.AppendLine(" AND A.RESPONSABILIDADE = @RESPONSABILIDADE ");
                        parametros.Add("@RESPONSABILIDADE", request.Responsabilidade);
                    }

                    var naturezas = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Natureza_Producao).
                                                            Select(p => p.valores.Replace("COLUNA", "L.ID")).
                                                            ToList();
                    if (naturezas.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", naturezas)}) ");

                    }

                    var tipos_servico = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Tipo_Servico).
                                                            Select(p => p.valores.Replace("COLUNA", "F.COD_TIPO_SERV")).
                                                            ToList();
                    if (tipos_servico.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", tipos_servico)}) ");
                    }

                    var periodos = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Periodo).
                                                            Select(p => p.valores.Replace("COLUNA", "A.COD_TURNO_SERV")).
                                                            ToList();

                    if (!String.IsNullOrEmpty(request.SituacaoOS))
                    {

                        if (request.SituacaoOS == "abertas")
                        {
                            sqlWhere.AppendLine("AND (A.STATUS_EXECUCAO NOT IN (8,9,1,2)) ");

                        }
                        else if (request.SituacaoOS == "fechadas")
                        {
                            sqlWhere.AppendLine("AND (A.STATUS_EXECUCAO IN (8,1,2)) ");
                        }
                    }

                    if (!String.IsNullOrEmpty(request.Status_Auditoria))
                    {

                        if (request.Status_Auditoria != "2")
                        {
                            sqlWhere.AppendLine("AND (A.STATUS_AUDITORIA = @STATUS_AUDITORIA) ");
                        }
                        else // AGUARDANDO_AVALIACAO
                        {
                            sqlWhere.AppendLine(" AND A.STATUS_EXECUCAO = 2 AND (A.STATUS_AUDITORIA = @STATUS_AUDITORIA OR (A.STATUS_AUDITORIA IS NULL)) ");
                        }

                        parametros.Add("@STATUS_AUDITORIA", request.Status_Auditoria);
                    }

                    var subtipos_servico = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Sub_Tipo_Servico).
                                                               Select(p => p.valores.Replace("COLUNA_PAI", "F.COD_TIPO_SERV").Replace("COLUNA", "G.COD_SUB_TIPO_SERV")).
                                                               ToList();
                    if (subtipos_servico.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", subtipos_servico)}) ");
                    }

                    if (periodos.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", periodos)}) ");
                    }

                    var categorias_cliente = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Categoria_Cliente).
                                                               Select(p => p.valores.Replace("COLUNA", "M.ID")).
                                                               ToList();

                    if (categorias_cliente.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", categorias_cliente)}) ");
                    }

                    var supervisores = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Supervisor_Tecnico).
                                                               Select(p => p.valores.Replace("COLUNA", "K.NUM_FRE")).
                                                               ToList();
                    if (supervisores.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", supervisores)}) ");
                    }


                    var monitores = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Monitor_Tecnico).
                                                               Select(p => p.valores.Replace("COLUNA", "B.USUARIO_MONITOR")).
                                                               ToList();

                    if (monitores.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", monitores)}) ");
                    }


                    var responsaveis = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Responsavel).
                                                               Select(p => p.valores.Replace("COLUNA", "A.EQUIPE")).
                                                               ToList();

                    if (responsaveis.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", responsaveis)}) ");
                    }

                    var datas_agendamento = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.DataAgendamento).
                                                            Select(p => p.valores.Replace("COLUNA", "A.DATA_MAXIMA")).
                                                            ToList();
                    if (datas_agendamento.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", datas_agendamento)}) ");

                    }

                    var datas_criacao = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.DataCriacao).
                                                        Select(p => p.valores.Replace("COLUNA", "A.DIA")).
                                                        ToList();
                    if (datas_criacao.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", datas_criacao)}) ");

                    }

                    var datas_encerramento = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.DataEncerramento).
                                                        Select(p => p.valores.Replace("COLUNA", "A.DATA_PROD")).
                                                        ToList();
                    if (datas_encerramento.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", datas_encerramento)}) ");

                    }

                    if (String.IsNullOrEmpty(request.TemOsDigital) == false)
                    {
                        sqlWhere.AppendLine($" AND (A.TEM_OSDIGITAL = '{request.TemOsDigital}') ");
                    }

                    if (String.IsNullOrEmpty(request.CodDepto) == false)
                    {
                        sqlWhere.AppendLine($" AND (A.COD_DEPTO = {request.CodDepto}) ");
                    }

                    if (request.Sla == "0")
                    {
                        sqlWhere.AppendLine($" AND datediff(HOUR ,(CURRENT_DATE + CURRENT_TIME),(A.DATA_INDICADOR + A.HORA_INDICADOR) )  >4  and  A.DATA_PROD IS Null ");
                    }
                    if (request.Sla == "1")
                    {

                        sqlWhere.AppendLine($" AND datediff(HOUR, (CURRENT_DATE + CURRENT_TIME), (A.DATA_INDICADOR + A.HORA_INDICADOR)) >= 3 AND datediff(HOUR, (CURRENT_DATE + CURRENT_TIME), (A.DATA_INDICADOR + A.HORA_INDICADOR)) < 4 and  A.DATA_PROD IS Null");
                    }

                    if (request.Sla == "2")
                    {

                        sqlWhere.AppendLine($" AND datediff(HOUR, (CURRENT_DATE + CURRENT_TIME), (A.DATA_INDICADOR + A.HORA_INDICADOR)) >= 2 AND datediff(HOUR, (CURRENT_DATE + CURRENT_TIME), (A.DATA_INDICADOR + A.HORA_INDICADOR)) < 3 and  A.DATA_PROD IS Null ");
                    }

                    if (request.Sla == "3")
                    {

                        sqlWhere.AppendLine($" AND datediff(MINUTE, (CURRENT_DATE + CURRENT_TIME), (A.DATA_INDICADOR + A.HORA_INDICADOR)) >= 0 AND datediff(MINUTE, (CURRENT_DATE + CURRENT_TIME), (A.DATA_INDICADOR + A.HORA_INDICADOR)) <=59 and  A.DATA_PROD IS Null ");
                    }
                    if (request.Sla == "4")
                    {
                        sqlWhere.AppendLine($" AND (A.DATA_INDICADOR + A.HORA_INDICADOR) <CURRENT_DATE + CURRENT_TIME  and  A.DATA_PROD IS Null");
                    }
                    if (request.Sla == "5")
                    {
                        sqlWhere.AppendLine($" AND (A.DATA_PROD + A.HORA_PROD) <(A.DATA_INDICADOR + A.HORA_INDICADOR)  and  A.DATA_PROD IS NOT Null");
                    }
                    if (request.Sla == "6")
                    {
                        sqlWhere.AppendLine($" AND (A.DATA_PROD + A.HORA_PROD) >(A.DATA_INDICADOR + A.HORA_INDICADOR) and  A.DATA_PROD IS NOT Null");
                    }
                    if (request.Sla == "7")
                    {
                        sqlWhere.AppendLine($"   and  A.DATA_INDICADOR IS  Null");
                    }
                    if ((!String.IsNullOrEmpty(request.DataInicial) || request.TipoData.HasValue == false || request.TipoData == 4))
                    {
                        if (!String.IsNullOrEmpty(request.DataInicial))
                        {
                            if (request.ConfigRoteirizados == "S")
                            {
                                sqlWhere.AppendLine("AND ( (A.DATA_MAXIMA BETWEEN @DATAINICIAL AND @DATAFINAL) ");
                                sqlWhere.AppendLine("AND (A.EQUIPE IS NOT NULL)");
                                sqlWhere.AppendLine("AND ((A.STATUS_EXECUCAO <> 1 AND A.STATUS_EXECUCAO <> 2 AND A.STATUS_EXECUCAO <> 9) OR (A.DATA_PROD = @DATAINICIAL AND A.STATUS_EXECUCAO <> 9)))");

                                parametros.Add("@DATAINICIAL", String.Format("{0}", request.DataInicial));
                                parametros.Add("@DATAFINAL", String.Format("{0}", request.DataFinal));
                            }
                            else if (request.ConfigDemanda == "S")
                            {
                                sqlWhere.AppendLine("AND ((A.STATUS_EXECUCAO <> 1 AND A.STATUS_EXECUCAO <> 2 AND A.STATUS_EXECUCAO <> 9) OR (A.DATA_PROD = @DATAINICIAL AND A.STATUS_EXECUCAO <> 9))");
                                sqlWhere.AppendLine("AND ( (A.DATA_MAXIMA BETWEEN @DATA_INICIO_DEMANDA AND @DATAFINALANTERIOR) ");
                                sqlWhere.AppendLine("OR (A.DATA_MAXIMA IS NULL)");
                                sqlWhere.AppendLine(")");
                                //sqlFrom.AppendLine("OR ((A.DATA_MAXIMA BETWEEN @DATAINICIAL AND @DATAFINAL ) AND A.EQUIPE IS NULL))");

                                parametros.Add("@DATAINICIAL", String.Format("{0}", request.DataInicial));
                                parametros.Add("@DATAFINALANTERIOR", String.Format("{0}", Convert.ToDateTime(request.DataFinal).AddDays(-1).ToString("dd/MM/yyyy")));
                                parametros.Add("@DATAFINAL", String.Format("{0}", request.DataFinal));
                                parametros.Add("@DATA_INICIO_DEMANDA", request.DataInicioDemanda ?? "01/01/2000");
                            }
                            else if (!request.TipoData.HasValue)
                            {
                                sqlWhere.AppendLine("AND ( (A.DATA_MAXIMA BETWEEN @DATAINICIAL AND @DATAFINAL) ");
                                sqlWhere.AppendLine("OR (A.DIA BETWEEN @DATAINICIAL AND @DATAFINAL))");

                                parametros.Add("@DATAINICIAL", String.Format("{0}", request.DataInicial));
                                parametros.Add("@DATAFINAL", String.Format("{0}", request.DataFinal));
                            }
                            else if (request.TipoData == 0)
                            {
                                sqlWhere.AppendLine("AND A.DATA_MAXIMA BETWEEN @DATAINICIAL_AGENDAMENTO_OS AND @DATAFINAL_AGENDAMENTO_OS");
                                parametros.Add("@DATAINICIAL_AGENDAMENTO_OS", String.Format("{0}", request.DataInicial));
                                parametros.Add("@DATAFINAL_AGENDAMENTO_OS", String.Format("{0}", request.DataFinal));


                            }
                            else if (request.TipoData == 1)
                            {
                                sqlWhere.AppendLine("AND A.DIA BETWEEN @DATAINICIAL_ABERTURA_OS AND @DATAFINAL_ABERTURA_OS");
                                parametros.Add("@DATAINICIAL_ABERTURA_OS", String.Format("{0}", request.DataInicial));
                                parametros.Add("@DATAFINAL_ABERTURA_OS", String.Format("{0}", request.DataFinal));
                            }
                            else if (request.TipoData == 2)
                            {
                                sqlWhere.AppendLine("AND A.DATA_PROD BETWEEN @DATAINICIAL_ENCERRAMENTO_OS AND @DATAFINAL_ENCERRAMENTO_OS");
                                parametros.Add("@DATAINICIAL_ENCERRAMENTO_OS", String.Format("{0}", request.DataInicial));
                                parametros.Add("@DATAFINAL_ENCERRAMENTO_OS", String.Format("{0}", request.DataFinal));


                                if (!String.IsNullOrEmpty(request.DataInicial))
                                {

                                }
                                else if (request.TipoData == 3)
                                {
                                    if (!String.IsNullOrEmpty(request.DataInicial) && (!String.IsNullOrEmpty(request.DataFinal)))
                                    {

                                        sqlWhere.AppendLine(" AND A.DATA_INDICADOR BETWEEN @DATA_INICIO AND @DATA_FIM");
                                        parametros.Add("@DATA_INICIO", string.Format("{0}", request.DataInicial));
                                        parametros.Add("@DATA_FIM", string.Format("{0}", request.DataFinal));
                                    }
                                }
                            }
                        }

                        if (request.TipoData == 4)
                        {
                            sqlWhere.AppendLine("AND A.DATA_MAXIMA IS NULL");

                            if (!temAcesso360)
                            {
                                sqlWhere.AppendLine($"AND (A.DIA BETWEEN '{DateTime.Now.AddDays(-31).ToString("dd.MM.yyyy")}' AND '{DateTime.Now.ToString("dd.MM.yyyy")}')");
                            }

                            if (!temAcesso360)
                            {
                                sqlWhere.AppendLine($"AND (A.DIA BETWEEN '{DateTime.Now.AddDays(-31).ToString("dd.MM.yyyy")}' AND '{DateTime.Now.ToString("dd.MM.yyyy")}')");
                            }

                        }
                        if (request.TipoData != 4 && (String.IsNullOrEmpty(request.DataInicial) || String.IsNullOrEmpty(request.DataFinal)) && request.Id_Modelo == null)
                        {
                            throw new Exception("Preencha corretamente o filtro de período");
                        }
                    }


                    //Filtro por UF
                    var UFs = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.UF).
                                                            Select(p => p.valores.Replace("COLUNA", "UPPER(A.ENDERECO_UF)")).
                                                            ToList();
                    if (UFs.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", UFs)}) ");
                    }

                    var status_cobranca = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Status_Cobranca).
                                                      Select(p => p.valores.Replace("COLUNA", "A.STATUS_COBRANCA")).ToList();
                    if (status_cobranca.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", status_cobranca)}) ");

                    }

                    //Filtro por Cidade
                    var cidades = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Cidade).
                                                            Select(p => p.valores.Replace("COLUNA", "UPPER(A.ENDERECO_TIPO)")).
                                                            ToList();

                    if (cidades.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", cidades)}) ");
                    }

                    //Filtro por Bairro
                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        sqlWhere.AppendLine("AND (UPPER(A.ENDERECO_BAIRRO) = @BAIRRO)");
                        parametros.Add("@BAIRRO", request.Bairro);
                    }

                    var grupos_area = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.GrupoArea).
                                                            Select(p => p.valores.Replace("COLUNA", "A.COD_ESCRITORIO")).
                                                            ToList();
                    if (grupos_area.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", grupos_area)}) ");

                    }


                    var areas = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Area).
                                                            Select(p => p.valores.Replace("COLUNA_PAI", "A.COD_ESCRITORIO").Replace("COLUNA", "A.COD_AREA")).
                                                            ToList();
                    if (areas.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", areas)}) ");

                    }

                    var localidades = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Localidade).
                                                            Select(p => p.valores.Replace("COLUNA_AVO", "A.COD_ESCRITORIO").Replace("COLUNA_PAI", "A.COD_AREA").Replace("COLUNA", "A.COD_LOCALIDADE")).
                                                            ToList();
                    if (localidades.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", localidades)}) ");

                    }

                    var status = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Status).
                                                            Select(p => p.valores.Replace("COLUNA", "A.STATUS_EXECUCAO")).
                                                            ToList();
                    if (status.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", status)}) ");

                    }

                    var planos_produtos = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.PlanoProduto).
                                                        Select(p => p.valores.Replace("COLUNA", "N.ID")).
                                                        ToList();
                    if (planos_produtos.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", planos_produtos)}) ");

                    }

                    var tipos_imovel = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Tipo_Imovel).
                                                            Select(p => p.valores.Replace("COLUNA", "A.PARPRIM_PARA")).
                                                            ToList();
                    if (tipos_imovel.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", tipos_imovel)}) ");

                    }

                    var origens = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Origem).
                                                            Select(p => p.valores.Replace("COLUNA", "A.SIGLA_MODULO")).
                                                            ToList();
                    if (origens.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", origens)}) ");

                    }

                    var empresas = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Empresa).
                                                            Select(p => p.valores.Replace("COLUNA", "O.CODEMPRESA")).
                                                            ToList();
                    if (empresas.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", empresas)}) ");

                    }

                    var filiais = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.Filial).
                                                            Select(p => p.valores.Replace("COLUNA_PAI", "o.CODEMPRESA").Replace("COLUNA", "O.CODFILIAL")).
                                                            ToList();
                    if (filiais.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", filiais)}) ");

                    }

                    var usuarios_abertura = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.UsuarioAbertura).
                                                            Select(p => p.valores.Replace("COLUNA", "A.USUARIO")).
                                                            ToList();
                    if (usuarios_abertura.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", usuarios_abertura)}) ");

                    }

                    var usuarios_agendamento = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.UsuarioAgendamento).
                                                            Select(p => p.valores.Replace("COLUNA", "A.USUARIO_AGENDAMENTO")).
                                                            ToList();
                    if (usuarios_agendamento.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", usuarios_agendamento)}) ");

                    }

                    if (!String.IsNullOrEmpty(request.StatusEquipe))
                    {
                        sqlWhere.AppendLine(" AND A.EQUIPE IN (SELECT Y.EQUIPE FROM TB_EQUIPE_STATUS Y WHERE Y.TIPO = @STATUS_EQUIPE)");
                        parametros.Add("@STATUS_EQUIPE", Convert.ToInt32(request.StatusEquipe));
                    }

                    if (!String.IsNullOrEmpty(request.CategoriaCliente))
                    {
                        sqlWhere.AppendLine(" AND H.ID_CATEGORIA  = @CATEGORIA_CLIENTE");
                        parametros.Add("@CATEGORIA_CLIENTE", request.CategoriaCliente);
                    }

                    if (!String.IsNullOrEmpty(request.Estado))
                    {
                        sqlWhere.AppendLine(" AND A.ENDERECO_UF = @ENDERECO_UF_ ");
                        parametros.Add("@ENDERECO_UF_", request.Estado);
                    }

                    //Filtro Elementos de Rede
                    if (request.Id_Pop.HasValue)
                    {
                        sqlWhere.AppendLine(" AND A.ID_POP = @ID_POP ");
                        parametros.Add("@ID_POP", request.Id_Pop);
                    }

                    if (request.Id_Olt.HasValue)
                    {
                        sqlWhere.AppendLine(" AND A.ID_OLT = @ID_OLT ");
                        parametros.Add("@ID_OLT", request.Id_Olt);
                    }

                    if (request.Id_ElementoRede.HasValue)
                    {
                        sqlWhere.AppendLine(" AND A.ID_ELEMENTO_REDE = @ID_ELEMENTO_REDE ");
                        parametros.Add("@ID_ELEMENTO_REDE", request.Id_ElementoRede);
                    }

                    if (request.Id_Organizacao.HasValue)
                    {
                        sqlWhere.AppendLine(" AND H.ID_ORGANIZACAO  = @ID_ORGANIZACAO");
                        parametros.Add("@ID_ORGANIZACAO", request.Id_Organizacao);
                    }
                    if (request.OperadorFiltro.HasValue)
                    {
                        var operadores = new List<string> { ">", "<", ">=", "<=", "=" };
                        var operador = Convert.ToInt32(request.OperadorFiltro) >= 5 ? "" : operadores[Convert.ToInt32(request.OperadorFiltro)];
                        sqlWhere.AppendLine(request.OperadorFiltro == Comparativo_QuantidadeEnum.Todos ? string.Empty : $" AND A.SEQ {(request.OperadorFiltro == Comparativo_QuantidadeEnum.Entre ? "BETWEEN" : operador)} {request.ValorInicialReabertura} {(request.OperadorFiltro == Comparativo_QuantidadeEnum.Entre ? "AND " + request.ValorFinalReabertura : string.Empty)}");
                    }

                    //Count
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder(@"SELECT COUNT(A.NUM_OBRA) CO ");
                        IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(session, session);
                        var sqlJoinCount = repoDocProd.GetJoinCountPainelServico(sqlWhere.ToString());
                        sql.AppendLine(sqlJoinCount);
                        sql.AppendLine(where);

                        //if (request.UtilizaHistoricoReabertura)
                        //{
                        //    string and = " AND ";
                        //    if (sqlWhere.ToString().Substring(0, 5) == and)
                        //    {
                        //        int index = sqlWhere.ToString().Substring(0, 5).Length;

                        //        string subStringSQLWhere = sqlWhere.ToString();
                        //        subStringSQLWhere = subStringSQLWhere.Substring(index);

                        //        sqlWhere = new StringBuilder(@"");
                        //        sqlWhere.AppendLine(subStringSQLWhere);
                        //        sql.AppendLine(subStringSQLWhere);
                        //    }
                        //}
                        //else
                        //{
                        //    sql.AppendLine(sqlWhere.ToString());
                        //}
                        sql.AppendLine(sqlWhere.ToString());
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

                        sql.AppendLine(" SELECT A.OXBDPEE, A.CPF_CLIENTE,A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_OBRA_ORIGINAL, A.NUM_DOC, A.ID_VENDA, A.WO, A.ID_ATENDIMENTO_CONNECT, A.TEM_OSDIGITAL, ");
                        sql.AppendLine(@"COALESCE((A.DATA_ABERTURA + COALESCE(A.HORA_ABERTURA, CAST('00:00' AS TIME))),(CASE WHEN
                                                        COALESCE(A.SEQ,1) > 1 THEN
                                                            (SELECT FIRST 1 M.DIA + COALESCE(M.HORA, CAST('00:00' AS TIME))
                                                            FROM TB_DOCUMENTO_PRODUCAO M
                                                            WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO
                                                            AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL
                                                            AND A.NUM_DOC = M.NUM_DOC ORDER BY M.SEQ)
                                                   WHEN
                                                        COALESCE(A.SEQ,1) <= 1 THEN
                                                            A.DIA + COALESCE(A.HORA, CAST('00:00' AS TIME))

                                            END)) AS DATAHORA_ABERTURA_OS_ORIGINAL, ");

                        sql.AppendLine("        A.DIA + COALESCE(A.HORA, CAST('00:00' AS TIME)) AS DATAHORA_ABERTURA_OS ,A.DATA_INDICADOR + A.HORA_INDICADOR AS DATAHORA_VENCIMENTO_OS, ");
                        sql.AppendLine("        A.DATA_MAXIMA + COALESCE(A.HORA_MAXIMA, CAST('00:00' AS TIME)) AS DATAHORA_AGENDAMENTO_OS,A.DATAHORA_DESLOCAMENTO,A.DATAHORA_INI_ATEND,A.DATAHORA_FIM_ATEND, A.DATA_PROD + A.HORA_PROD AS DATAHORA_ENCERRAMENTO_OS, ");
                        sql.AppendLine("        A.DATAHORA_DESLOCAMENTO, A.DATAHORA_INI_ATEND, A.DATAHORA_PAUSOU, A.DATAHORA_REINICIO, ");
                        sql.AppendLine("        A.USUARIO AS USUARIO_ABERTURA_OS, A.USUARIO_AGENDAMENTO AS USUARIO_AGENDAMENTO_OS, A.RESPONSABILIDADE, A.ROTERIZAR, ");
                        sql.AppendLine("        B.NOME AS NOME_EQUIPE, B.NUM_FRE AS MATRICULA_EQUIPE, C.DESCRICAO AS DESCRICAO_ESCRITORIO, C.COD_ESCRITORIO, A.DATAHORA_FIM_ATEND, ");
                        sql.AppendLine("        D.DESCRICAO AS DESCRICAO_AREA, D.COD_AREA, E.LOCALIDADE, E.COD_LOCALIDADE, F.DESCRICAO AS DESCRICAO_TIPO_SERV, F.COD_TIPO_SERV, N.DESCRICAO AS DESCRICAO_PLANOPRODUTO,B.USUARIO_MONITOR, ");
                        sql.AppendLine("        G.DESCRICAO AS DESCRICAO_SUBTIPO_SERV,A.DATA_INDICADOR + A.HORA_INDICADOR AS DATAHORA_INDICADOR, ");
                        sql.AppendLine("        COALESCE(H.RAZAO_SOCIAL,A.TITULAR) AS NOME_CLIENTE,M.DESCRICAO AS CATEGORIA_CLIENTE, ");
                        sql.AppendLine("        A.ENDERECO AS ENDERECO_CLIENTE, COALESCE(H.TELEFONE2,A.TEL_CIENTE) AS CELULAR_CLIENTE, ");
                        sql.AppendLine("        A.ENDERECO_COMPL AS ENDERECO_COMPL, COALESCE(H.TELEFONE,A.TAREFA) AS TELEFONE_PRINCIPAL_CLIENTE, ");
                        sql.AppendLine("        COALESCE(A.ENDERECO_TIPO, A.ENDERECO_CIDADE) AS ENDERECO_CIDADE, A.ENDERECO_BAIRRO AS ENDERECO_BAIRRO, A.ENDERECO_UF AS ENDERECO_UF, ");
                        sql.AppendLine("        A.ENDERECO_CPL AS ENDERECO_CEP, A.ENDERECO_NUMERO, 'Brasil' AS ENDERECO_PAIS, ");
                        sql.AppendLine("        I.DESCRICAO AS DESCRICAO_ENCERRAMENTO, J.DESCRICAO AS TIPO_IMOVEL, K.NOME AS SUPERVISOR, L.DESCRICAO AS NATUREZA_OS, ");
                        sql.AppendLine("        A.STATUS_EXECUCAO, A.USUARIO_BAIXA, ");
                        sql.AppendLine("        P.RPONTOS_PREVISTOS AS QTDE_PREV, P.RPONTOS_PRODUZIDOS AS QTDE_PROD, Q.DESCRICAO AS TURNO,A.OBSERVACAO_AUX,OBSERVACAO_TESTE,");
                        sql.AppendLine("        A.OBSERVACAO_PAUSA,A.LATITUDE_STR,A.LONGITUDE_STR, A.STATUS_AUDITORIA,A.STATUS_COBRANCA, A.SEQ,A.TIPO_PRIORIDADE,R.DESCRICAO AS DEPARTAMENTO,S.DESCRICAO AS SEGMENTO,  ");
                        sql.AppendLine("        (SELECT FIRST 1 X.TIPO FROM TB_EQUIPE_STATUS X WHERE X.EQUIPE = A.EQUIPE) AS STATUS_RESPONSAVEL, ");
                        sql.AppendLine("        (SELECT LIST(EA.EQUIPE || ' - ' || TE.NOME, ', ') FROM TB_MOBILE_PARTICIPANTES_RDO EA LEFT JOIN TB_EQUIPE TE ON TE.EQUIPE = EA.EQUIPE ")
                           .AppendLine("        WHERE (A.CODCT = EA.CODCT AND A.PROJETO = EA.PROJETO AND A.NUM_DOC = EA.NUM_DOC AND A.NUM_OBRA = EA.NUM_OBRA)) AS EQUIPES_AUXILIARES");
                        sql.AppendLine(sqlJoin.ToString());
                        sql.AppendLine(sqlWhere.ToString());
                        //sql.AppendLine(" ORDER BY A.DATA_MAXIMA DESC, A.HORA_MAXIMA DESC");

                        #region OrdenacaoSql(Teste)
                        //Ordenacao
                        if (request.order != null && request.order.Any())
                        {

                            int sortColumnIndex = request.order[0].column;
                            var coluna = request.columns[sortColumnIndex].data;
                            string sortDirection = request.order[0].dir;
                            switch (coluna)
                            {
                                case "numero_Obra_Original":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY NUM_OBRA_ORIGINAL ");
                                        else
                                            sql.AppendLine("ORDER BY NUM_OBRA_ORIGINAL DESC");
                                        break;
                                    }
                                case "Matricula_Tecnico":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY MATRICULA_EQUIPE ");

                                        else
                                            sql.AppendLine("ORDER BY MATRICULA_EQUIPE DESC");
                                        break;
                                    }
                                case "num_Doc":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY NUM_DOC");
                                        else
                                            sql.AppendLine("ORDER BY NUM_DOC DESC");
                                        break;
                                    }
                                case "responsavel":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY NOME_EQUIPE");
                                        else
                                            sql.AppendLine("ORDER BY NOME_EQUIPE DESC");
                                        break;
                                    }
                                case "status_Responsavel":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY STATUS_RESPONSAVEL");
                                        else
                                            sql.AppendLine("ORDER BY STATUS_RESPONSAVEL DESC");
                                        break;
                                    }
                                case "tipo_Servico":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY DESCRICAO_TIPO_SERV");
                                        else
                                            sql.AppendLine("ORDER BY DESCRICAO_TIPO_SERV DESC");
                                        break;
                                    }

                                case "subTipo_Servico":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY DESCRICAO_SUBTIPO_SERV");
                                        else
                                            sql.AppendLine("ORDER BY DESCRICAO_SUBTIPO_SERV DESC");
                                        break;
                                    }
                                case "tipo_Imovel":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY TIPO_IMOVEL");
                                        else
                                            sql.AppendLine("ORDER BY TIPO_IMOVEL DESC");
                                        break;
                                    }
                                case "supervisor":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY SUPERVISOR");
                                        else
                                            sql.AppendLine("ORDER BY SUPERVISOR DESC");
                                        break;
                                    }
                                case "grupo_Area ":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY DESCRICAO_ESCRITORIO");
                                        else
                                            sql.AppendLine("ORDER BY DESCRICAO_ESCRITORIO DESC");
                                        break;
                                    }
                                case "area":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY DESCRICAO_AREA");
                                        else
                                            sql.AppendLine("ORDER BY DESCRICAO_AREA DESC");
                                        break;
                                    }
                                case "localidade":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY E.LOCALIDADE");
                                        else
                                            sql.AppendLine("ORDER BY E.LOCALIDADE DESC");
                                        break;
                                    }
                                case "nome_Cliente":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY RAZAO_SOCIAL");
                                        else
                                            sql.AppendLine("ORDER BY RAZAO_SOCIAL DESC");
                                        break;
                                    }
                                case "data_Hora_Abertura_OS":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY DATAHORA_ABERTURA");
                                        else
                                            sql.AppendLine("ORDER BY DATAHORA_ABERTURA DESC");
                                        break;
                                    }
                                case "data_Hora_Agendamento_OS":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY A.DATA_MAXIMA,A.HORA_MAXIMA ");
                                        else
                                            sql.AppendLine("ORDER BY A.DATA_MAXIMA DESC,A.HORA_MAXIMA DESC ");
                                        break;
                                    }
                                case "numero_Cliente":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY OXBDPEE ");
                                        else
                                            sql.AppendLine("ORDER BY OXBDPEE DESC");
                                        break;
                                    }
                                case "cpf_Cnpj_Cliente":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY CPF_CLIENTE");
                                        else
                                            sql.AppendLine("ORDER BY CPF_CLIENTE DESC");
                                        break;
                                    }

                                case "natureza":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY NATUREZA_OS");
                                        else
                                            sql.AppendLine("ORDER BY NATUREZA_OS DESC");
                                        break;
                                    }
                                case "endereco_Cliente":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY ENDERECO_CLIENTE");
                                        else
                                            sql.AppendLine("ORDER BY ENDERECO_CLIENTE DESC");
                                        break;
                                    }
                                case "endereco_UF":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY ENDERECO_UF");
                                        else
                                            sql.AppendLine("ORDER BY ENDERECO_UF DESC");
                                        break;
                                    }
                                case "endereco_Cidade":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY  UPPER (A.ENDERECO_TIPO)");
                                        else
                                            sql.AppendLine("ORDER BY UPPER( A.ENDERECO_TIPO DESC)");
                                        break;
                                    }
                                case "endereco_Bairro":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY UPPER (ENDERECO_BAIRRO)");
                                        else
                                            sql.AppendLine("ORDER BY UPPER (ENDERECO_BAIRRO) DESC");
                                        break;
                                    }

                                case "codCt":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY CODCT");
                                        else
                                            sql.AppendLine("ORDER BY CODCT DESC");
                                        break;
                                    }
                                case "projeto":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY PROJETO");
                                        else
                                            sql.AppendLine("ORDER BY PROJETO DESC");
                                        break;
                                    }
                                case "dataHora_Abertura_OS":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY A.DIA,A.HORA ");
                                        else
                                            sql.AppendLine("ORDER BY A.DIA DESC,A.HORA DESC ");
                                        break;
                                    }
                                case "dataHora_Encerramento_OS":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY A.DATA_PROD, A.HORA_PROD ");
                                        else
                                            sql.AppendLine("ORDER BY A.DATA_PROD DESC,A.HORA_PROD DESC ");
                                        break;
                                    }
                                case "dataHora_Vencimento_OS":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY DATAHORA_VENCIMENTO_OS");
                                        else
                                            sql.AppendLine("ORDER BY DATAHORA_VENCIMENTO_OS DESC");
                                        break;
                                    }
                                case "periodo":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY TURNO");
                                        else
                                            sql.AppendLine("ORDER BY TURNO DESC");
                                        break;
                                    }

                                case "categoriaCliente":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY CATEGORIA_CLIENTE");
                                        else
                                            sql.AppendLine("ORDER BY CATEGORIA_CLIENTE DESC");
                                        break;
                                    }
                                case "status_Execucao":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine("ORDER BY STATUS_EXECUCAO");
                                        else
                                            sql.AppendLine("ORDER BY STATUS_EXECUCAO DESC");
                                        break;
                                    }

                            }
                        }
                        #endregion


                        bool buscaPaginada = request.length < int.MaxValue;


                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string usuarioAberturaOS = reader["USUARIO_ABERTURA_OS"].ToString() == CONSTANTES.USUARIOOLA ? String.Concat(Convert.ToString(reader["NOME_CLIENTE"]), "(OLA)") : Convert.ToString(reader["USUARIO_ABERTURA_OS"]);

                                var obj = new Painel_ServicosListaModel();
                                obj.Equipe_Matricula = reader["MATRICULA_EQUIPE"].ToString();
                                obj.Numero_Cliente = reader["OXBDPEE"].ToString();
                                obj.Cpf_Cnpj_Cliente = reader["CPF_CLIENTE"].ToString();
                                obj.CodCt = reader["CODCT"].ToString();
                                obj.Projeto = reader["PROJETO"].ToString();
                                obj.Numero_Obra = reader["NUM_OBRA"].ToString();                                
                                if (request.UtilizaHistoricoReabertura)
                                {
                                    obj.Numero_Obra_Original = reader["NUM_OBRA"] != DBNull.Value ? reader["NUM_OBRA"].ToString() : string.Empty;
                                }
                                else
                                {
                                    obj.Numero_Obra_Original = reader["NUM_OBRA_ORIGINAL"] != DBNull.Value ? reader["NUM_OBRA_ORIGINAL"].ToString() : reader["NUM_OBRA"].ToString();
                                }
                                obj.Num_Doc = Convert.ToInt32(reader["NUM_DOC"]);
                                obj.Id_Venda = Convert.ToString(reader["ID_VENDA"]);
                                obj.Id_Atendimento = Convert.ToString(reader["ID_ATENDIMENTO_CONNECT"]);
                                obj.DataHora_Abertura_OS = Convert.ToString(reader["DATAHORA_ABERTURA_OS"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_ABERTURA_OS"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.DataHora_Abertura_OS_Original = Convert.ToString(reader["DATAHORA_ABERTURA_OS_ORIGINAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_ABERTURA_OS_ORIGINAL"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.Data_Hora_Agendamento_OS = Convert.ToString(reader["DATAHORA_AGENDAMENTO_OS"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_AGENDAMENTO_OS"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.DataHora_Encerramento_OS = Convert.ToString(reader["DATAHORA_ENCERRAMENTO_OS"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_ENCERRAMENTO_OS"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.DataHora_Vencimento_OS = Convert.ToString(reader["DATAHORA_VENCIMENTO_OS"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_VENCIMENTO_OS"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.DataHora_Deslocamento_OS = Convert.ToString(reader["DATAHORA_DESLOCAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_DESLOCAMENTO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.DataHora_IniAtendimento_OS = Convert.ToString(reader["DATAHORA_INI_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.DataHora_Pausou_OS = Convert.ToString(reader["DATAHORA_PAUSOU"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_PAUSOU"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.DataHora_Reinicio_OS = Convert.ToString(reader["DATAHORA_REINICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_REINICIO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.DataHora_FimAtendimento_OS = Convert.ToString(reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.Status_Execucao = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"]));
                                obj.Tipo_Prioridade = HtmlEnumExtensions.GetEnumDescription<Tipo_PrioridadeT5>(Convert.ToString(reader["TIPO_PRIORIDADE"]));
                                obj.Status_Auditoria = reader["STATUS_AUDITORIA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_AuditoriaAniel>(Convert.ToString(reader["STATUS_AUDITORIA"])) : String.Empty;
                                obj.Roterizar = reader["ROTERIZAR"].ToString();
                                obj.Nome_Cliente = reader["NOME_CLIENTE"].ToString();
                                obj.Endereco_Cliente = reader["ENDERECO_CLIENTE"].ToString();
                                obj.Endereco_UF = reader["ENDERECO_UF"].ToString();
                                obj.Endereco_Cidade = reader["ENDERECO_CIDADE"].ToString();
                                obj.Endereco_Bairro = reader["ENDERECO_BAIRRO"].ToString();
                                obj.Endereco_Complemento = reader["ENDERECO_COMPL"].ToString();
                                obj.Endereco_Numero = reader["ENDERECO_NUMERO"].ToString();
                                obj.Endereco_Pais = reader["ENDERECO_PAIS"].ToString();
                                obj.Atendimento_Finalizado = reader["DATAHORA_FIM_ATEND"] == DBNull.Value ? Resources.RsBase.NAO : Resources.RsBase.SIM;
                                obj.Status_Responsavel = reader["STATUS_RESPONSAVEL"] == DBNull.Value ? String.Empty : HtmlEnumExtensions.GetEnumDescription<EquipeStatusTipo>(Convert.ToString(reader["STATUS_RESPONSAVEL"]));
                                obj.Telefone_Principal_Cliente = reader["TELEFONE_PRINCIPAL_CLIENTE"].ToString();
                                obj.Celular_Cliente = reader["CELULAR_CLIENTE"].ToString();
                                obj.Tipo_Servico = reader["DESCRICAO_TIPO_SERV"].ToString();
                                obj.Cod_Tipo_Servico = reader["COD_TIPO_SERV"].ToString();
                                obj.SubTipo_Servico = reader["DESCRICAO_SUBTIPO_SERV"].ToString();
                                obj.Responsavel = reader["NOME_EQUIPE"].ToString();
                                obj.Tipo_Imovel = reader["TIPO_IMOVEL"].ToString();
                                obj.Supervisor = reader["SUPERVISOR"].ToString();
                                obj.Grupo_Area = reader["DESCRICAO_ESCRITORIO"].ToString();
                                obj.Cod_Grupo_Area = reader["COD_ESCRITORIO"].ToString();
                                obj.Area = reader["DESCRICAO_AREA"].ToString();
                                obj.Cod_Area = reader["COD_AREA"].ToString();
                                obj.Localidade = reader["LOCALIDADE"].ToString();
                                obj.Cod_Localidade = Convert.ToString(reader["COD_LOCALIDADE"]);
                                obj.Descricao_Encerramento = reader["DESCRICAO_ENCERRAMENTO"].ToString();
                                obj.Usuario_Abertura_OS = usuarioAberturaOS;
                                obj.Usuario_Agendamento_OS = reader["USUARIO_AGENDAMENTO_OS"].ToString() == CONSTANTES.USUARIOOLA ? String.Concat(Convert.ToString(reader["NOME_CLIENTE"]), "(OLA)") : Convert.ToString(reader["USUARIO_AGENDAMENTO_OS"]);
                                obj.Usuario_Encerramento = reader["USUARIO_BAIXA"].ToString();
                                obj.CategoriaCliente = reader["CATEGORIA_CLIENTE"].ToString();
                                obj.Natureza = reader["NATUREZA_OS"].ToString();
                                obj.Monitor = reader["USUARIO_MONITOR"].ToString();
                                obj.PlanoProduto = reader["DESCRICAO_PLANOPRODUTO"].ToString();
                                obj.Pontos_Previstos = reader["QTDE_PREV"] != DBNull.Value ? Convert.ToDouble(reader["QTDE_PREV"]).ToString("N2") : "0,00";
                                obj.Pontos_Realizados = reader["QTDE_PROD"] != DBNull.Value ? Convert.ToDouble(reader["QTDE_PROD"]).ToString("N2") : "0,00";
                                obj.Periodo = reader["TURNO"].ToString();
                                if (obj.Responsabilidade == "2" && obj.Roterizar == "S")
                                {
                                    obj.Responsabilidade = "Sim";
                                }
                                else
                                {
                                    obj.Responsabilidade = "NÆo";
                                }
                                obj.LatitudeLongitude = $"{reader["LATITUDE_STR"].ToString()},{reader["LONGITUDE_STR"].ToString()}";
                                obj.Seq = reader["SEQ"] != DBNull.Value ? Convert.ToString(reader["SEQ"]) : String.Empty;
                                obj.Wo = reader["WO"].ToString();
                                obj.TemOsDigital = String.IsNullOrEmpty(reader["TEM_OSDIGITAL"].ToString()) ? "N" : reader["TEM_OSDIGITAL"].ToString();
                                obj.Responsabilidade = HtmlEnumExtensions.GetEnumDescription<ResponsabilidadeOS>(Convert.ToString(reader["RESPONSABILIDADE"]));
                                obj.Status_Cobranca = HtmlEnumExtensions.GetEnumDescription<Status_Cobranca>(Convert.ToString(reader["STATUS_COBRANCA"]));

                                obj.Observacao = reader["OBSERVACAO_AUX"] != DBNull.Value ? reader["OBSERVACAO_AUX"].ToString() : "-";
                                obj.ObservacaoTeste = reader["OBSERVACAO_TESTE"] != DBNull.Value ? reader["OBSERVACAO_TESTE"].ToString() : "-";
                                obj.Observacao_Pausa = reader["OBSERVACAO_PAUSA"] != DBNull.Value ? reader["OBSERVACAO_PAUSA"].ToString() : "-";
                                obj.Departamento = Convert.ToString(reader["DEPARTAMENTO"]);
                                obj.Segmento = Convert.ToString(reader["SEGMENTO"]);
                                obj.Data_Hora_Deslocamento = Convert.ToString(reader["DATAHORA_DESLOCAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_DESLOCAMENTO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.Data_Hora_Inicio = Convert.ToString(reader["DATAHORA_INI_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);
                                obj.Data_Hora_Final = Convert.ToString(reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]).ToString("dd/MM/yyyy HH:mm") : String.Empty);

                                obj.Equipe_Auxiliares = reader["EQUIPES_AUXILIARES"].ToString();
                                if (reader["DATAHORA_INDICADOR"] != DBNull.Value)
                                {
                                    DateTime? DataHora_Indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                                    DateTime? DataHora_Criacao = reader["DATAHORA_ABERTURA_OS"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_ABERTURA_OS"]) : (DateTime?)null;
                                    DateTime? DataHora_Prod = null;

                                    if (reader["DATAHORA_ENCERRAMENTO_OS"] != DBNull.Value)
                                        DataHora_Prod = Convert.ToDateTime(reader["DATAHORA_ENCERRAMENTO_OS"]);

                                    obj.Sla = documentoApi.GetDescricaoHorasSla(DataHora_Indicador, DataHora_Criacao, DataHora_Prod);

                                }
                                else
                                    obj.Sla = "Sem Indicador de Sla";

                                ////GEISSIANE: verifica se está entre o horário de trabalho
                                //if (obj.Status_Responsavel == HtmlEnumExtensions.GetEnumDescription<EquipeStatusTipo>(Convert.ToString(EquipeStatusTipo.Em_Atendimento)) && request.StatusEquipe == "7")
                                //{
                                //    using (ISession sessionNH = HibernateUtil.GetSessionT5())
                                //    using (ISession session_connectNH = HibernateUtil.GetSession())
                                //    using (var repository = new Conexao<Horario_Connect>(sessionNH))
                                //    using (var repositoryPerfilAgendamento = new Conexao<PerfilAgendamento>(sessionNH))
                                //    using (var feriado_repository = new Conexao<Feriado>(session_connectNH))
                                //    using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionNH))
                                //    using (var horarioapi = new HorarioApiController())
                                //    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                                //    {
                                //        DateTime dataAtual = DateTime.Now;
                                //        var horario_modelo = await repository.selectFirst(p => p.Padrao && p.Tipo == Tipo_Hora_Connect.Horario_Servico);
                                //        bool feriado = feriado_repository.selectFirst(p => p.Data == dataAtual).Result != null;
                                //        var horariosEquipes = await repositoryHorarioEquipe.select(p => p.Data == dataAtual);
                                //        var horarioEquipeTecnico = horariosEquipes.FirstOrDefault(p => p.Equipe.Matricula == obj.Equipe_Matricula);

                                //        var atend_fora_horario = await new Gestao_EquipeApiController().GetAtendimentoForaHorario(dataAtual, horario_modelo, feriado, horarioEquipeTecnico);
                                //        if (atend_fora_horario)
                                //        {
                                //            obj.Status_Responsavel = Convert.ToString(EquipeStatusTipo.Atendimento_Fora_Horario);
                                //        }
                                //    }
                                //}

                                if (String.IsNullOrEmpty(obj.Status_Auditoria))
                                {
                                    if (obj.Status_Execucao == HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(StatusExecucaoAniel.Fechada_Produtiva)))
                                    {
                                        obj.Status_Auditoria = HtmlEnumExtensions.GetEnumDescription<Status_AuditoriaAniel>(Convert.ToString(Status_AuditoriaAniel.AguardandoAvaliacao));
                                    }
                                }

                                retorno.Add(obj);

                            }

                            var status_sla = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelServicoEnum.SLA);
                            if (status_sla.Count > 0)
                            {
                                var lista_status = status_sla.Select(p =>
                                {
                                    var inicio = p.valores.IndexOf("'") + 1;
                                    var fim = p.valores.IndexOf("')");
                                    var sla = p.valores.Substring(inicio, fim - inicio);
                                    return HtmlEnumExtensions.GetEnumDescription<Status_SLA_Horas_Servico>(sla);


                                }).ToList();

                                retorno = retorno.FindAll(p => lista_status.Contains(p.Sla));

                            }

                        }

                    }


                    //Ordenação SLA em OBJETO
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;

                        string sortDirection = request.order[0].dir;
                        if (coluna == "sla")

                        {

                            if (sortDirection == "asc")
                                retorno = retorno.OrderBy(p => p.Sla).ToList();
                            else
                                retorno = retorno.OrderByDescending(p => p.Sla).ToList();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new DataTableResponse<Painel_ServicosListaModel>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ex.Message
                };
            }


            //Page e retornando DataTable
            return new DataTableResponse<Painel_ServicosListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        public List<(Filtro_PainelServicoEnum tipo_filtro, String valores)> GetModeloFiltros(Int64? Id_Modelo_PainelServico, Painel_Servicos_DataTableRequest request)
        {
            List<(Filtro_PainelServicoEnum tipo_filtro, String valores)> retorno = new List<(Filtro_PainelServicoEnum tipo_filtro, String valores)>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_PainelServico>(session))
            {
                Modelo_PainelServico modelo = repository.selectFirst(p => p.Id == Id_Modelo_PainelServico).Result;

                if (modelo != null)
                {
                    var filtros = modelo.Filtros.Select(p => p.Filtro).Distinct();

                    retorno.AddRange(filtros.Select(p =>
                    {
                        return (p, "(" + String.Join(" OR ", modelo.Filtros.Where(y => y.Filtro == p).Select(y =>
                        {
                            if (y.Data.HasValue)
                            {
                                return $"(COLUNA {SqlModeloPainelData(y.Data.Value)})";
                            }
                            else if (string.IsNullOrEmpty(y.ValorPai) == false && string.IsNullOrEmpty(y.ValorAvo) == false)
                            {
                                return $"(COLUNA = '{y.Valor}' AND COLUNA_PAI = '{y.ValorPai}' AND COLUNA_AVO = '{y.ValorAvo}')";
                            }
                            else if (string.IsNullOrEmpty(y.ValorPai) == false)
                            {
                                return $"(COLUNA = '{y.Valor}' AND COLUNA_PAI = '{y.ValorPai}')";
                            }
                            else
                                return $"(COLUNA = '{y.Valor}')";
                        }).ToList()) + ")");
                    }));
                }

            }

            foreach (var prop in request.GetType().GetProperties())
            {
                var attr = prop.GetCustomAttribute<Servico_ModeloAttribute>();

                if (attr != null)
                {
                    if (prop.GetValue(request) != null)
                    {
                        retorno.Add((attr.Recurso, $"(COLUNA = '{Convert.ToString(prop.GetValue(request))}')"));
                    }
                }
            }


            return retorno;
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/GetAllLogAlteracaoOS")]
        public async Task<DataTableResponse<LogAlteracaoOSListaModel>> GetAllLogAlteracaoOS(LogAlteracaoOS_DataTableRequest request)
        {

            List<LogAlteracaoOSListaModel> retorno = new List<LogAlteracaoOSListaModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;



                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.CODCT, A.DIA, A.HORA, A.NUM_DOC, A.NUM_OBRA, A.PROJETO, A.USUARIO, A.ID, A.VALOR_ANT, A.ALTERACAO,  ")
                       .AppendLine(" A.DATA_MAXIMA, B.DATA_MAXIMA + HORA_MAXIMA AS AGENDAMENTO ,A.COD_TIPO_SERV, A.MOTIVO_EXCLUSAO, B.COD_ENCERRAMENTO, C.EQUIPE, C.NOME, D.DESCRICAO AS TIPO_SERVICO, E.DESCRICAO AS SUBTIPO_SERVICO ")

                       .AppendLine(" FROM TB_LOG_LANC_SERVICO A ")

                       .AppendLine(" LEFT JOIN TB_DOCUMENTO_PRODUCAO B ")
                       .AppendLine(" ON(B.CODCT = A.CODCT AND B.PROJETO = A.PROJETO AND B.NUM_DOC = A.NUM_DOC AND B.NUM_OBRA = A.NUM_OBRA");

                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(B.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine(") ")

                       .AppendLine(" LEFT JOIN TB_EQUIPE C ")
                       .AppendLine(" ON(C.EQUIPE = B.EQUIPE) ")

                       .AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE D ")
                       .AppendLine(" ON(D.COD_TIPO_SERV = B.COD_TIPO_SERV) ")

                       .AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO E ")
                       .AppendLine(" ON(E.COD_TIPO_SERV = B.COD_TIPO_SERV AND E.COD_SUB_TIPO_SERV = B.COD_SUB_TIPO_SERV) ")

                       .AppendLine(" WHERE  A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_DOC = @NUM_DOC AND A.NUM_OBRA LIKE @NUM_OBRA "); //

                    parametros.Add("@CODCT", request.CodCt);
                    parametros.Add("@PROJETO", request.Projeto);
                    parametros.Add("@NUM_DOC", request.Num_Doc);
                    parametros.Add("@NUM_OBRA", '%' + request.Numero_Obra.Split('/')[0] + '%');

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new LogAlteracaoOSListaModel
                            {
                                Num_Obra = Convert.ToString(reader["NUM_OBRA"]).Split('/')[0],
                                Usuario = Convert.ToString(reader["USUARIO"]),
                                Dia = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]).ToString("dd/MM/yyyy") : String.Empty,
                                Hora = reader["HORA"] != DBNull.Value ? Convert.ToString(reader["HORA"]) : String.Empty,
                                Tipo = Convert.ToString(reader["ALTERACAO"]),
                                Valor_Ant = Convert.ToString(reader["VALOR_ANT"])
                            };


                            if (reader["DATA_MAXIMA"] != DBNull.Value)
                            {
                                obj.Valor_Atual = Convert.ToDateTime(reader["DATA_MAXIMA"]).ToString("dd/MM/yyyy");
                                if (String.IsNullOrEmpty(obj.Valor_Ant))
                                    obj.Valor_Ant = "-";
                            }

                            else if (obj.Tipo == "Cancelamento")
                            {
                                obj.Valor_Ant = obj.Valor_Ant;
                                obj.Valor_Atual = obj.Tipo;
                            }

                            else if (obj.Tipo == "Latitude/Longitude")
                            {
                                obj.Valor_Ant = obj.Valor_Ant;
                                obj.Valor_Atual = obj.Tipo;
                            }

                            else if (obj.Tipo == "Baixou OS" || obj.Tipo == "Reabriu OS")
                            {
                                obj.Valor_Ant = obj.Valor_Ant.Split('/')[0];
                                obj.Valor_Atual = obj.Valor_Ant;
                            }
                            else if (obj.Tipo == "Agendamento")
                            {
                                obj.Valor_Ant = obj.Valor_Ant;
                                obj.Valor_Atual = Convert.ToDateTime(reader["AGENDAMENTO"]).ToString("dd/MM/yyyy HH:mm");
                            }
                            else if (obj.Tipo == "Cria?Æo do Servi?o")
                            {
                                obj.Valor_Ant = "-";
                                obj.Valor_Atual = "-";
                            }
                            else if (obj.Tipo == "Incluiu Material" || obj.Tipo == "Incluiu Produ?Æo")
                            {
                                obj.Valor_Atual = obj.Valor_Ant;
                                obj.Valor_Ant = "-";
                            }
                            else if (obj.Tipo == "C¢digo de Encerramento")
                            {
                                obj.Valor_Atual = Convert.ToString(reader["COD_ENCERRAMENTO"]);
                            }
                            else if (obj.Tipo == "Tipo de Servi?o")
                            {
                                obj.Valor_Atual = Convert.ToString(reader["TIPO_SERVICO"]);
                            }
                            else if (obj.Tipo == "Subtipo de Servi?o")
                            {
                                obj.Valor_Atual = Convert.ToString(reader["SUBTIPO_SERVICO"]);
                            }
                            else if (obj.Tipo == "Avalia?Æo Auditor")
                            {
                                obj.Valor_Ant = obj.Valor_Ant;
                                obj.Valor_Atual = HtmlEnumExtensions.GetEnumDescription<Status_AuditoriaAniel>(Convert.ToString(reader["STATUS_AUDITORIA"]));
                            }
                            else if (obj.Tipo.Contains("Equipe"))
                            {
                                var equipe = Convert.ToString(reader["EQUIPE"]);
                                var nome = Convert.ToString(reader["NOME"]);

                                obj.Valor_Atual = $"{equipe} - {nome}";
                            }

                            retorno.Add(obj);
                        }
                    }

                }
            }

            recordsTotal = retorno.Count();

            //Page e retornando DataTable
            return new DataTableResponse<LogAlteracaoOSListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/AtualizaVinculoManutencaoOSs")]
        public async void AtualizaVinculoManutencaoOSs(VincularOsModel request)
        {

            using (var sessionT5 = HibernateUtil.GetSessionT5())
            {
                IDocumentoProducaoAnielRepository repo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);

                new DocumentoProducaoAnielBusiness(repo).AtualizaVinculoManutencaoOSs(request);

            }

        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/AtribuirResponsavel")]
        public async Task<PostApiRetorno<List<DocumentoProducao_AtribuirEquipe>>> AtribuirResponsavel(AlterarResponsavelViewModel value)
        {
            PostApiRetorno<List<DocumentoProducao_AtribuirEquipe>> retorno = new PostApiRetorno<List<DocumentoProducao_AtribuirEquipe>>();

            try
            {
                if (String.IsNullOrEmpty(value.equipe))
                    throw new Exception("nenhuma equipe selecionada");
                if (value.ids == null || value.ids.Count == 0)
                    throw new Exception("nenhuma ordem de servi?o selecionada");

                retorno.Ok = true;

                List<DocumentoProducao_AtribuirEquipe> documentosNaoAtribuidos = new List<DocumentoProducao_AtribuirEquipe>();
                List<DocumentoProducao_AtribuirEquipe> chaves_documento = value.ids.Select(chave =>
                {

                    String[] _chaves = chave.Split('|');
                    String CodCt = _chaves[0];
                    String Projeto = _chaves[1];
                    String Num_Obra = _chaves[2];
                    Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                    return new DocumentoProducao_AtribuirEquipe
                    {
                        CodCt = CodCt,
                        Num_Doc = Num_Doc,
                        Num_Obra = Num_Obra,
                        Projeto = Projeto
                    };

                }).ToList();

                foreach (var listaQuebrada in ListService.GetListaQuebrada(chaves_documento, 200))
                {
                    var documentos = listaQuebrada.Value;
                    using (ISession session = HibernateUtil.GetSessionT5())
                    using (ISession sessionNH = HibernateUtil.GetSessionT5())
                    using (var repository = new Conexao<EquipeAniel>(sessionNH))
                    using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(sessionNH))
                    {

                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        using (var com = session.Connection.CreateCommand())
                        {
                            StringBuilder sql = new StringBuilder($"SELECT A.NUM_OBRA,A.NUM_DOC,A.PROJETO,A.CODCT,A.EQUIPE,B.NOME,A.STATUS_EXECUCAO,A.DATA_MAXIMA FROM TB_DOCUMENTO_PRODUCAO A LEFT JOIN TB_EQUIPE B ON (A.EQUIPE = B.EQUIPE )WHERE ");
                            for (int i = 0; i < documentos.Count; i++)
                            {
                                sql.AppendLine($"(A.NUM_OBRA = '{documentos[i].Num_Obra}' AND A.NUM_DOC = {documentos[i].Num_Doc} AND A.PROJETO = '{documentos[i].Projeto}' AND A.CODCT = '{documentos[i].CodCt}')");
                                if ((i + 1) < documentos.Count)
                                    sql.AppendLine(" OR ");
                            }



                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {

                                //List<DocumentoProducao_AtribuirEquipe> Lista_Tecnico_Nao_Alterado = new List<DocumentoProducao_AtribuirEquipe>();

                                while (await reader.ReadAsync())
                                {
                                    Int32? num_doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : (Int32?)null;
                                    var documento = documentos.Find(p => p.CodCt == Convert.ToString(reader["CODCT"]) &&
                                                    p.Num_Obra == Convert.ToString(reader["NUM_OBRA"]) &&
                                                    p.Projeto == Convert.ToString(reader["PROJETO"]) &&
                                                    p.Num_Doc == num_doc);

                                    documento.Equipe = Convert.ToString(reader["EQUIPE"]);
                                    documento.Nome_Equipe = Convert.ToString(reader["NOME"]);
                                    documento.DataAgendamento = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]) : (DateTime?)null;
                                    documento.statusExecucaoAniel = (StatusExecucaoAniel)Enum.Parse(typeof(StatusExecucaoAniel), Convert.ToString(reader["STATUS_EXECUCAO"]));

                                    if (documento.DataAgendamento.HasValue == false)
                                    {
                                        documentos.Remove(documento);
                                        documentosNaoAtribuidos.Add(documento);
                                        retorno.Ok = false;
                                    }

                                    if (new List<StatusExecucaoAniel> { StatusExecucaoAniel.Fechada_Produtiva,
                                                                        StatusExecucaoAniel.Fechada_Improdutiva,
                                                                        StatusExecucaoAniel.OS_Em_deslocamento,
                                                                        StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado
                                                                       }.Contains(documento.statusExecucaoAniel))
                                    {
                                        //Lista_Tecnico_Nao_Alterado.Add(documento);

                                        documentos.Remove(documento);
                                        documentosNaoAtribuidos.Add(documento);
                                        retorno.Ok = false;
                                        // retorno.Mensagem = "Devido a restri?Æo do status da OS, nÆo foi poss¡vel atribuir t?cnico nas seguintes OS's."
                                        //    + "<br><br>" + "<b>N§ Ordem de Servi?o: </b>" + "<br>" + String.Join("<br>",Lista_Tecnico_Nao_Alterado);

                                        //retorno.Mensagem = "" + documentosNaoAtribuidos.ToList();

                                    }
                                }
                            }
                        }

                        if (documentos.Count > 0)
                        {
                            using (var com = session.Connection.CreateCommand())
                            {
                                StringBuilder sql = new StringBuilder($"UPDATE TB_DOCUMENTO_PRODUCAO SET EQUIPE = '{value.equipe}', STATUS_EXECUCAO = 0 WHERE ");


                                for (int i = 0; i < documentos.Count; i++)
                                {

                                    sql.AppendLine($"(NUM_OBRA = '{documentos[i].Num_Obra}' AND NUM_DOC = {documentos[i].Num_Doc} AND PROJETO = '{documentos[i].Projeto}' AND CODCT = '{documentos[i].CodCt}')");
                                    if ((i + 1) < documentos.Count)
                                        sql.AppendLine(" OR ");

                                }

                                await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);
                            }
                        }


                        List<String> tecnicos = new List<String>();
                        foreach (var documento in documentos)
                        {

                            if (value.equipe != documento.Equipe)
                            {
                                Agendamento_EquipeAniel agendamento = await repositoryAgendamento.selectFirst(p => p.CodCt == documento.CodCt && p.Projeto == documento.Projeto && p.Num_Doc == documento.Num_Doc && p.Num_Obra == documento.Num_Obra);

                                if (agendamento != null)
                                {
                                    agendamento.Equipe = await repository.selectFirst(p => p.Matricula == value.equipe);
                                    await repositoryAgendamento.SaveOrUpdate(agendamento);
                                }

                                tecnicos.Add(documento.Equipe);
                                tecnicos.Add(value.equipe);


                                await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc.GetValueOrDefault(0),
                                "Equipe", $"{documento?.Equipe} - {documento?.Nome_Equipe}", value.usuario, null, null, value.equipe, null);

                            }

                            if (documento.statusExecucaoAniel != StatusExecucaoAniel.Aberta_Aguard_Atendimento)
                            {

                                await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session,
                                                                                        documento.CodCt,
                                                                                        documento.Projeto,
                                                                                        documento.Num_Obra,
                                                                                        documento.Num_Doc.GetValueOrDefault(0),
                                                                                        "Mudança de Status",
                                                                                        HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(documento.statusExecucaoAniel),
                                                                                        value.usuario,
                                                                                        null,
                                                                                        null,
                                                                                        HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Aberta_Aguard_Atendimento), null);
                            }

                        }

                        var lista_equipes = await repository.select(p => p.Matricula.IsIn(tecnicos.Distinct().ToList()));

                        foreach (var item in lista_equipes)
                        {
                            if (!String.IsNullOrEmpty(item.Device_Id))
                                MobileService.NotificarSmartphone(item.Device_Id, String.Empty, "Houve mudan?as em sua rotina.",
                                                                    "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS",
                                                                    "", "", "", "");
                        }



                        var equipe = await repository.select(p => p.Matricula == value.equipe);

                    }
                }

                retorno.IdObjeto = documentosNaoAtribuidos;


            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }


            return retorno;
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/AlterarResponsabilidade")]
        public async Task<PostApiRetorno> AlterarResponsabilidade(AlterarResponsabilidadeModel value)
        {
            PostApiRetorno<Documento_ProducaoChaveModel> retorno = new PostApiRetorno<Documento_ProducaoChaveModel>();

            try
            {

                if (value.ids == null || value.ids.Count == 0)
                {
                    throw new Exception("nenhuma ordem de servi?o selecionada");
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (ITransaction transaction = sessionT5.BeginTransaction())
                {

                    List<Documento_ProducaoChaveModel> chaves_documento = value.ids.Select(chave =>
                    {

                        String[] _chaves = chave.Split('|');
                        String CodCt = _chaves[0];
                        String Projeto = _chaves[1];
                        String Num_Obra = _chaves[2];
                        Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                        return new Documento_ProducaoChaveModel
                        {
                            CodCt = CodCt,
                            Num_Doc = Num_Doc,
                            Num_Obra = Num_Obra,
                            Projeto = Projeto
                        };

                    }).ToList();

                    foreach (var item in chaves_documento)
                    {
                        Documento_ProducaoAniel documento = await repository.selectFirst(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt);
                        if (documento.Status == StatusExecucaoAniel.Fechada_Produtiva || documento.Status == StatusExecucaoAniel.Fechada_Improdutiva)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_BAIXADA, documento.Numero_Obra_Original));
                        }

                        if (documento.Status == StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado || documento.Status == StatusExecucaoAniel.Reiniciado_Atendimento)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_INICIADO, documento.Numero_Obra_Original));
                        }

                        if (documento.Status == StatusExecucaoAniel.OS_Em_deslocamento)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_DESLOCAMENTO, documento.Numero_Obra_Original));
                        }

                        if (documento == null)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_NAO_ENCONTRADA, documento.Numero_Obra_Original));
                        }

                        await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(sessionT5, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, "Alteração de Responsabilidade",
                          documento.Responsabilidade.ToString(), User.Identity.GetUserId(), documento.Data_Maxima, documento.Cod_Tipo_Serv, value.responsabilidade.ToString(), null, transaction);

                        documento.Responsabilidade = value.responsabilidade;

                        if (documento.Responsabilidade == ResponsabilidadeOS.COP || documento.Responsabilidade == ResponsabilidadeOS.LIVRE)
                        {
                            documento.Roterizar = "N";
                        }

                        await repository.SaveOrUpdate(documento);

                        if (documento.EquipeAniel != null)
                        {
                            MobileService.NotificarSmartphone(documento.EquipeAniel.Device_Id, "TransfCop", "Transfêrencia de Responsabilidade",
                                                              $"Transfêrencia de Responsabilidade - OS:{documento.Num_Obra}", "",
                                                              documento.CodCt, documento.Projeto, Convert.ToString(documento.Num_Doc), documento.Num_Obra);
                        }


                        var notificacao = new NotificacoesApiController().EnviaNotificacoesServico(22, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, null);
                    }

                    transaction.Commit();
                }

                retorno.Ok = true;

            }

            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/RemoverTecnico")]
        public async Task<PostApiRetorno> RemoverTecnico(AlterarResponsabilidadeModel value)
        {
            PostApiRetorno<Documento_ProducaoChaveModel> retorno = new PostApiRetorno<Documento_ProducaoChaveModel>();

            try
            {

                if (value.ids == null || value.ids.Count == 0)
                {
                    throw new Exception("nenhuma ordem de servi?o selecionada");
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repositoryEquipe = new Conexao<EquipeAniel>(sessionT5))
                using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(sessionT5))
                using (ITransaction transaction = sessionT5.BeginTransaction())
                {

                    List<Documento_ProducaoChaveModel> chaves_documento = value.ids.Select(chave =>
                    {

                        String[] _chaves = chave.Split('|');
                        String CodCt = _chaves[0];
                        String Projeto = _chaves[1];
                        String Num_Obra = _chaves[2];
                        Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                        return new Documento_ProducaoChaveModel
                        {
                            CodCt = CodCt,
                            Num_Doc = Num_Doc,
                            Num_Obra = Num_Obra,
                            Projeto = Projeto
                        };

                    }).ToList();

                    List<Documento_ProducaoAniel> documentos = new List<Documento_ProducaoAniel>();
                    foreach (var item in chaves_documento)
                    {
                        Documento_ProducaoAniel documento = await repository.selectFirst(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt);
                        if (documento.Responsabilidade == ResponsabilidadeOS.TECNICO)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_REP_TEC, documento.Numero_Obra_Original));
                        }

                        if (documento.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                            documento.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                            documento.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_REP_TEC, documento.Numero_Obra_Original));
                        }

                        if (documento == null)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_NAO_ENCONTRADA, documento.Numero_Obra_Original));
                        }


                        documentos.Add(documento);
                    }

                    foreach (var item in documentos)
                    {
                        Documento_ProducaoAniel documento = item;
                        EquipeAniel equipe = await repositoryEquipe.selectFirst(p => p.Matricula == documento.Equipe);


                        documento.Equipe = null;

                        if (documento.Data_Maxima.HasValue)
                        {
                            documento.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                        }
                        else
                        {
                            documento.Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento;
                        }

                        await repository.Update(documento);
                        await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(sessionT5,
                                                                                           documento.CodCt,
                                                                                           documento.Projeto,
                                                                                           documento.Num_Obra,
                                                                                           documento.Num_Doc,
                                                                                           "Remo?Æo de Equipe",
                                                                                           equipe == null ? String.Empty : $"{equipe?.Matricula} - {equipe?.Nome}",
                                                                                           User.Identity.GetUserId(),
                                                                                           null,
                                                                                           null,
                                                                                           String.Empty,
                                                                                           null,
                                                                                           transaction);


                        Agendamento_EquipeAniel agendamento = await repositoryAgendamento.selectFirst(p => p.CodCt == documento.CodCt && p.Projeto == documento.Projeto && p.Num_Doc == documento.Num_Doc && p.Num_Obra == documento.Num_Obra);
                        if (agendamento != null)
                        {
                            agendamento.Equipe = null;
                            await repositoryAgendamento.Update(agendamento);
                        }
                    }

                    transaction.Commit();
                }

                retorno.Ok = true;

            }

            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/RoteirizarEncaixe")]
        public async Task<PostApiRetorno> RoteirizarEncaixe(EncaixarRoteirizacaoOsModel value)
        {
            PostApiRetorno<Documento_ProducaoChaveModel> retorno = new PostApiRetorno<Documento_ProducaoChaveModel>();

            try
            {

                if (value.ids == null || value.ids.Count == 0)
                {
                    throw new Exception("nenhuma ordem de servio selecionada");
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repositoryEquipe = new Conexao<EquipeAniel>(sessionT5))
                using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(sessionT5))
                using (ITransaction transaction = sessionT5.BeginTransaction())
                {

                    List<Documento_ProducaoChaveModel> chaves_documento = value.ids.Select(chave =>
                    {

                        String[] _chaves = chave.Split('|');
                        String CodCt = _chaves[0];
                        String Projeto = _chaves[1];
                        String Num_Obra = _chaves[2];
                        Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                        return new Documento_ProducaoChaveModel
                        {
                            CodCt = CodCt,
                            Num_Doc = Num_Doc,
                            Num_Obra = Num_Obra,
                            Projeto = Projeto
                        };

                    }).ToList();

                    List<Documento_ProducaoAniel> documentos = new List<Documento_ProducaoAniel>();
                    foreach (var item in chaves_documento)
                    {
                        Documento_ProducaoAniel documento = await repository.selectFirst(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt);
                        if (documento.Responsabilidade == ResponsabilidadeOS.TECNICO)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_REP_TEC, documento.Numero_Obra_Original));
                        }

                        if (documento.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                            documento.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                            documento.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_DESLOCAMENTO2, documento.Numero_Obra_Original));
                        }

                        if (documento == null)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_NAO_ENCONTRADA, documento.Numero_Obra_Original));
                        }


                        documentos.Add(documento);
                    }

                    retorno.Ok = true;

                    Painel_RoteirizacaoApiController painelRoteirizacaoApiController = new Painel_RoteirizacaoApiController();
                    var resultadoEncaixe = await painelRoteirizacaoApiController.Roteirizar(new ParamWebApi<RoteirizarServicosModel>
                    {
                        Usuario = User.Identity.GetUserId(),
                        Objeto = new RoteirizarServicosModel
                        {
                            ids = documentos.Select(p => String.Join("|", new List<String> {
                                            p.CodCt, p.Projeto,
                                            p.Num_Obra,
                                            p.Num_Doc.ToString(),
                                        })).ToList(),
                            data = Convert.ToDateTime(value.data).ToString("dd/MM/yyyy"),
                            numero_max_atendimento = 1000,
                            encaixe = true
                        },
                    });

                    retorno.Ok = resultadoEncaixe.Ok;
                    retorno.Mensagem = resultadoEncaixe.Mensagem;


                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/RecalcularAgendamento")]
        public async Task<PostApiRetorno> RecalcularAgendamento(AlterarResponsabilidadeModel value)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Documento_ProducaoApiController documento_ProducaoApiController = new Documento_ProducaoApiController())
                using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                using (var repoAgendamento = new Conexao<Agendamento_EquipeAniel>(sessionT5))
                using (var repositoryDocumentoProducao = new Conexao<Documento_ProducaoAniel>(sessionT5))
                {
                    Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();

                    if (config.Considerar_Agendamento_Periodo == false)
                    {
                        throw new Exception($"Recurso só pode ser usado se a empresa usar agendamento por período");
                    }

                    TimeSpan tempoDeslocamento = config.Tempo_Medio_Deslocamento.HasValue ? config.Tempo_Medio_Deslocamento.Value.TimeOfDay : TimeSpan.Zero;

                    List<Documento_ProducaoChaveModel> chaves_documento = value.ids.Select(chave =>
                    {

                        String[] _chaves = chave.Split('|');
                        String CodCt = _chaves[0];
                        String Projeto = _chaves[1];
                        String Num_Obra = _chaves[2];
                        Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                        return new Documento_ProducaoChaveModel
                        {
                            CodCt = CodCt,
                            Num_Doc = Num_Doc,
                            Num_Obra = Num_Obra,
                            Projeto = Projeto
                        };

                    }).ToList();

                    List<Documento_ProducaoAniel> documentos = new List<Documento_ProducaoAniel>();
                    List<Agendamento_EquipeAniel> agendamentos = new List<Agendamento_EquipeAniel>();
                    foreach (var item in chaves_documento)
                    {
                        Documento_ProducaoAniel documento = await repositoryDocumentoProducao.selectFirst(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt);
                        Agendamento_EquipeAniel agendamento = await repoAgendamento.selectFirst(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt);
                        if (documento == null)
                        {
                            throw new Exception($"{item.Num_Obra} - Serviço não encontrado");
                        }
                        if (documento.Status != StatusExecucaoAniel.Aberta_Aguard_Responsavel)
                        {
                            throw new Exception($"{item.Num_Obra} - Serviço precisa estar no status Abert Aguardando Responsável");
                        }
                        if (agendamento == null)
                        {
                            throw new Exception($"{item.Num_Obra} - Agendamento não encontrado");
                        }


                        documentos.Add(documento);
                        agendamentos.Add(agendamento);
                    }

                    foreach (var documento in documentos)
                    {
                        Agendamento_EquipeAniel agendamento = agendamentos.Find(p => p.Num_Obra == documento.Num_Obra && p.Num_Doc == documento.Num_Doc && p.Projeto == documento.Projeto && p.CodCt == documento.CodCt);
                        var tempoExecucao = await documento_ProducaoApiController.GetTempoExecucao(config, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc);
                        if (tempoExecucao.HasValue)
                        {
                            DateTime novaDataHoraFim = agendamento.DataHora_Inicio.Add(tempoExecucao.Value + tempoDeslocamento);
                            if (agendamento.DataHora_Fim != novaDataHoraFim || agendamento.Deslocamento != tempoDeslocamento.TotalMinutes)
                            {
                                DateTime antigaDataHoraFim = agendamento.DataHora_Fim;
                                agendamento.DataHora_Fim = novaDataHoraFim;
                                agendamento.Deslocamento = tempoDeslocamento.TotalMinutes;
                                await repoAgendamento.Update(agendamento);
                                await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(sessionT5, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc,
                                    "Recálculo de Agendamento", $"{agendamento.DataHora_Inicio.ToString("dd/MM/yyyy HH:mm")} - {antigaDataHoraFim.ToString("dd/MM/yyyy HH:mm")}", User.Identity.GetUserId(), null, null, $"{agendamento.DataHora_Inicio.ToString("dd/MM/yyyy HH:mm")} - {novaDataHoraFim.ToString("dd/MM/yyyy HH:mm")}", null);
                            }
                        }
                    }

                    return new PostApiRetorno
                    {
                        Ok = true
                    };

                }
            }
            catch (Exception ex)
            {
                return new PostApiRetorno
                {
                    Ok = false,
                    Mensagem = ex.Message,
                    InnerException = ex.InnerException?.Message
                };
            }
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/ParalisarAtendimento")]
        public async Task<PostApiRetorno> ParalisarAtendimento(AlterarStatusModel value)
        {
            PostApiRetorno<Documento_ProducaoChaveModel> retorno = new PostApiRetorno<Documento_ProducaoChaveModel>();

            try
            {

                if (value.ids == null || value.ids.Count == 0)
                {
                    throw new Exception("nenhuma ordem de servi?o selecionada");
                }

                if (String.IsNullOrEmpty(value.justificativa))
                {
                    throw new Exception("Nenhuma justificativa informada");
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repositoryEquipe = new Conexao<EquipeAniel>(sessionT5))
                using (var repositoryUsuario = new Conexao<SenhaAniel>(sessionT5))
                using (var repositoryFuncionario = new Conexao<FuncionarioAniel>(sessionT5))
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
                {



                    List<Documento_ProducaoChaveModel> chaves_documento = value.ids.Select(chave =>
                    {

                        String[] _chaves = chave.Split('|');
                        String CodCt = _chaves[0];
                        String Projeto = _chaves[1];
                        String Num_Obra = _chaves[2];
                        Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                        return new Documento_ProducaoChaveModel
                        {
                            CodCt = CodCt,
                            Num_Doc = Num_Doc,
                            Num_Obra = Num_Obra,
                            Projeto = Projeto
                        };

                    }).ToList();

                    foreach (var item in chaves_documento)
                    {
                        Documento_ProducaoAniel documento = await repository.selectFirst(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt);
                        if (documento.Status != StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado && documento.Status != StatusExecucaoAniel.Reiniciado_Atendimento)
                        {
                            throw new Exception($"Ordem de Serviço {documento.Numero_Obra_Original} não se encontra com atendimento iniciado. Não é possível paralisar atendimento.");
                        }
                        IDocumentoProducaoAnielRepository repo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                        await new DocumentoProducaoAnielBusiness(repo).ParalisarAtendimento(item.CodCt,
                                                                                            item.Projeto,
                                                                                            item.Num_Doc ?? 0,
                                                                                            item.Num_Obra,
                                                                                            value.justificativa,
                                                                                            User.Identity.GetUserId());


                        var notificacao = new NotificacoesApiController().EnviaNotificacoesServico(27, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, null);

                    }
                }

                retorno.Ok = true;
            }

            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/InterromperDeslocamento")]
        public async Task<PostApiRetorno> InterromperDeslocamento(AlterarStatusModel value)
        {
            PostApiRetorno<Documento_ProducaoChaveModel> retorno = new PostApiRetorno<Documento_ProducaoChaveModel>();

            try
            {

                if (value.ids == null || value.ids.Count == 0)
                {
                    throw new Exception("nenhuma ordem de servi?o selecionada");
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repositoryEquipe = new Conexao<EquipeAniel>(sessionT5))
                using (var repositoryFuncionario = new Conexao<FuncionarioAniel>(sessionT5))
                using (var repositoryUsuario = new Conexao<SenhaAniel>(sessionT5))
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
                {



                    List<Documento_ProducaoChaveModel> chaves_documento = value.ids.Select(chave =>
                    {

                        String[] _chaves = chave.Split('|');
                        String CodCt = _chaves[0];
                        String Projeto = _chaves[1];
                        String Num_Obra = _chaves[2];
                        Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                        return new Documento_ProducaoChaveModel
                        {
                            CodCt = CodCt,
                            Num_Doc = Num_Doc,
                            Num_Obra = Num_Obra,
                            Projeto = Projeto
                        };

                    }).ToList();

                    foreach (var item in chaves_documento)
                    {
                        Documento_ProducaoAniel documento = await repository.selectFirst(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt);
                        if (documento.Status != StatusExecucaoAniel.OS_Em_deslocamento)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_DESLOCAMENTO3, documento.Numero_Obra_Original));
                        }

                        documento.Status = StatusExecucaoAniel.Interromper_Deslocamento;
                        await repository.SaveOrUpdate(documento);

                        MongoDB.Driver.Linq.IMongoQueryable<DocumentoProducaoMongo> query = (MongoDB.Driver.Linq.IMongoQueryable<DocumentoProducaoMongo>)repositoryMongo.GetQueryable().Where(x =>
                                                                                x.codCt == documento.CodCt &&
                                                                                x.projeto == documento.Projeto &&
                                                                                x.contrato == documento.Num_Doc &&
                                                                                x.numObra == documento.Num_Obra);
                        var ordemServicoLocalizada = query.SingleOrDefault();
                        var status = new List<StatusDocumentoProducao>();
                        SenhaAniel usuarioAniel = await repositoryUsuario.selectFirst(p => p.Usuario == User.Identity.GetUserId());
                        FuncionarioAniel funcionario = String.IsNullOrEmpty(usuarioAniel?.NumFre) ? null : await repositoryFuncionario.selectFirst(p => p.Num_Fre == usuarioAniel.NumFre);

                        status.Add(new StatusDocumentoProducao()
                        {

                            statusExecucao = (int)StatusExecucaoAniel.Interromper_Deslocamento,
                            equipe = User.Identity.GetUserId(),
                            nome = funcionario?.Nome,
                            numFre = funcionario?.Num_Fre,
                            data = DateTime.Now,
                            dataHoraServidor = DateTime.Now,
                            hora = DateTime.Now,
                            latitude = 0,
                            longitude = 0,
                            //justificativaPausaMongo = value.justificativa

                        });


                        if (ordemServicoLocalizada != null)
                        {
                            ordemServicoLocalizada.status.AddRange(status);
                            await repositoryMongo.Update(x => x.codCt == documento.CodCt &&
                                                                                x.projeto == documento.Projeto &&
                                                                                x.contrato == documento.Num_Doc &&
                                                                                x.numObra == documento.Num_Obra, ordemServicoLocalizada);
                        }
                        else
                        {
                            var documentoMongo = new DocumentoProducaoMongo()
                            {
                                codCt = documento.CodCt,
                                projeto = documento.Projeto,
                                contrato = documento.Num_Doc,
                                numObra = documento.Num_Obra,
                                dataAgendamento = documento.Data_Maxima.HasValue ? documento.Data_Maxima.Value : new DateTime(),
                                status = status

                            };
                            await repositoryMongo.Save(documentoMongo);
                        }

                        if (documento.EquipeAniel != null)
                        {
                            MobileService.NotificarSmartphone(documento.EquipeAniel.Device_Id, String.Empty, "Houve mudan?as em sua rotina.",
                                                                        "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS",
                                                                        "", "", "", "");
                        }

                        var notificacao = new NotificacoesApiController().EnviaNotificacoesServico(28, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, null);
                    }
                }

                retorno.Ok = true;


            }

            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Painel_ServicosApi/AlterarCaixa")]
        public async Task<PostApiRetorno<Int64>> AlterarCaixa(AlterarCaixaModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ITransaction transaction = sessionT5.BeginTransaction())
                using (var repositoryDocumentoProducao = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repositoryElementoRede = new Conexao<Elemento_Rede>(sessionT5))
                using (var repositoryElementoRedePorta = new Conexao<Elemento_Rede_Porta>(sessionT5))

                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    Documento_ProducaoAniel documento = await repositoryDocumentoProducao.selectFirst(p => p.Num_Obra == value.num_obra && p.Num_Doc == value.num_doc && p.Projeto == value.projeto && p.CodCt == value.codct);

                    if (documento.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                        documento.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                        documento.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                    {
                        throw new Exception(String.Format(RsBase.ORDEM_SERVICO_BAIXA, documento.Numero_Obra_Original));
                    }

                    Elemento_Rede caixaAntiga = await repositoryElementoRede.selectFirst(p => p.Id == Convert.ToInt32(documento.CaixaFTTH));
                    Elemento_Rede caixaAtual = await repositoryElementoRede.selectFirst(p => p.Id == Convert.ToInt32(value.caixa));
                    Elemento_Rede_Porta portaAtual = await repositoryElementoRedePorta.selectFirst(p => p.Porta == Convert.ToInt32(value.porta) && p.Caixa.Id == caixaAtual.Id);
                    Elemento_Rede_Porta portaAntiga = caixaAntiga != null ? await repositoryElementoRedePorta.selectFirst(p => p.Porta == Convert.ToInt32(documento.PortaFTTH) && p.Caixa.Id == caixaAntiga.Id) : null;

                    if (caixaAtual == null || portaAtual == null)
                    {
                        throw new Exception($"Caixa ou porta não encontrada.");
                    }


                    documento.CaixaFTTH = value.caixa;
                    documento.PortaFTTH = value.porta;

                    if (portaAntiga?.Id != portaAtual?.Id)
                    {
                        portaAtual.Status = Situacao_Porta.Reservado;
                        await repositoryElementoRedePorta.Update(portaAtual);

                        if (portaAntiga?.Id != null)
                        {
                            portaAntiga.Status = Situacao_Porta.Disponivel;
                            await repositoryElementoRedePorta.Update(portaAntiga);
                        }
                    }

                    await repositoryDocumentoProducao.Update(documento);

                    await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(sessionT5,
                                                                                          documento.CodCt,
                                                                                          documento.Projeto,
                                                                                          documento.Num_Obra,
                                                                                          documento.Num_Doc,
                                                                                          "Alteração Caixa e Porta",
                                                                                          "Caixa: " + caixaAntiga?.Descricao + " Porta: " + portaAntiga?.Porta,
                                                                                          User.Identity.GetUserId(),
                                                                                          null,
                                                                                          null,
                                                                                          "Caixa: " + caixaAtual.Descricao + " Porta: " + value.porta,
                                                                                          null,
                                                                                          transaction);


                    transaction.Commit();
                    retorno.Ok = true;

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
        [Route("api/Painel_ServicosApi/ExcluirServico")]
        public async Task<PostApiRetorno> ExcluirServico(AlterarStatusModel value)
        {
            PostApiRetorno<Documento_ProducaoChaveModel> retorno = new PostApiRetorno<Documento_ProducaoChaveModel>();

            try
            {

                if (value.ids == null || value.ids.Count == 0)
                {
                    throw new Exception("nenhuma ordem de servi?o selecionada");
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repositoryEquipe = new Conexao<EquipeAniel>(sessionT5))
                using (var repositoryFuncionario = new Conexao<FuncionarioAniel>(sessionT5))
                using (var repositoryUsuario = new Conexao<SenhaAniel>(sessionT5))
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
                using (var ExclusaoOsApi = new ExclusaoOsApiController())
                {



                    List<Documento_ProducaoChaveModel> chaves_documento = value.ids.Select(chave =>
                    {

                        String[] _chaves = chave.Split('|');
                        String CodCt = _chaves[0];
                        String Projeto = _chaves[1];
                        String Num_Obra = _chaves[2];
                        Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                        return new Documento_ProducaoChaveModel
                        {
                            CodCt = CodCt,
                            Num_Doc = Num_Doc,
                            Num_Obra = Num_Obra,
                            Projeto = Projeto
                        };

                    }).ToList();

                    var listaExcluir = new List<ExclusaoServicoPlanilha>();
                    foreach (var item in chaves_documento)
                    {
                        Documento_ProducaoAniel documento = await repository.selectFirst(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt);

                        if (documento.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                            documento.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                            documento.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_BAIXADA_EX, documento.Numero_Obra_Original));
                        }

                        if (documento.Responsabilidade == ResponsabilidadeOS.TECNICO)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_TECNICO_EX, documento.Numero_Obra_Original));
                        }



                        if (documento == null)
                        {
                            throw new Exception(String.Format(RsBase.ORDEM_SERVICO_NAO_ENCONTRADA, documento.Numero_Obra_Original));
                        }

                        listaExcluir.Add(new ExclusaoServicoPlanilha
                        {
                            Contrato = item.CodCt,
                            Projeto = item.Projeto,
                            IdentificacaoCliente = Convert.ToString(item.Num_Doc),
                            OrdemServico = documento.Numero_Obra_Original,
                            DataCriacao = documento.Dia.Value.ToString("dd/MM/yyyy")
                        });
                    }

                    var erros = await ExclusaoOsApi.ExcluiOrdemServico(listaExcluir);
                    if (erros.Count > 0)
                    {
                        throw new Exception("Uma ou mais OS's nÆo puderam ser exclu¡das. Para mais informa?äes visualize o log de exclusÆo de OS's.");
                    }
                }

                retorno.Ok = true;
            }

            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
        public static String SqlModeloPainelData(ModeloPainel_Data data_enum)
        {

            switch (data_enum)
            {
                case ModeloPainel_Data.DIA_ATUAL:
                    return $" = '{DateTime.Now.ToString("dd.MM.yyyy")}'";
                case ModeloPainel_Data.MES_ATUAL:
                    return $" BETWEEN '{DateTime.Now.ToString("01.MM.yyyy")}' AND '{DateTime.Now.ToString("dd.MM.yyyy")}'";
                case ModeloPainel_Data.SEMANA_ATUAL:
                    return $" BETWEEN '{DateTimeUtil.GetLastDayofTheWeek(DayOfWeek.Monday).ToString("dd.MM.yyyy")}' AND '{DateTime.Now.ToString("dd.MM.yyyy")}'";
                case ModeloPainel_Data.ULTIMOS_30_DIAS:
                    return $" BETWEEN '{DateTime.Now.AddDays(-30).ToString("dd.MM.yyyy")}' AND '{DateTime.Now.ToString("dd.MM.yyyy")}'";
                case ModeloPainel_Data.ULTIMOS_3_MESES:
                    return $" BETWEEN '{DateTime.Now.AddDays(-90).ToString("dd.MM.yyyy")}' AND '{DateTime.Now.ToString("dd.MM.yyyy")}'";
                case ModeloPainel_Data.ULTIMOS_7_DIAS:
                    return $" BETWEEN '{DateTime.Now.AddDays(-7).ToString("dd.MM.yyyy")}' AND '{DateTime.Now.ToString("dd.MM.yyyy")}'";
            }
            return null;
        }


        private async Task<List<Config_Aniel_Integrator>> ObterConfigAnielIntegrator(ISession sessionT5)
        {
            List<Config_Aniel_Integrator> retorno = new List<Config_Aniel_Integrator>();
            using (DbCommand comT5 = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine($"SELECT * ");
                sql.AppendLine(" FROM TB_CONFIG_ANIEL_INTEGRATOR ");
                //DAO IXC usando o Integrator

                using (var reader = await CommandHelper.ExecuteReader(comT5, sql, parametros, sessionT5.Connection, null))
                {
                    while (reader.Read())
                    {
                        Config_Aniel_Integrator config = new Config_Aniel_Integrator()
                        {
                            URL = Convert.ToString(reader["URL"]),
                            UsuarioAniel = Convert.ToString(reader["USUARIO_ANIEL"]),
                            CodCtPadrao = Convert.ToString(reader["CODCT_PADRAO"]),
                            ProjetoPadrao = Convert.ToString(reader["PROJETO_PADRAO"]),
                            Unidade_Custo = Convert.ToString(reader["UNIDADE_CUSTO"]),
                            Token = Convert.ToString(reader["TOKEN"]),
                            Setores = Convert.ToString(reader["SETORES"]),
                            UF = Convert.ToString(reader["UF"]),
                        };

                        retorno.Add(config);
                    }

                }

            }

            return retorno;
        }
    }

    public class CheckOsManutencao
    {
        public List<string> osNumeros { get; set; } = new List<string>();
    }
}