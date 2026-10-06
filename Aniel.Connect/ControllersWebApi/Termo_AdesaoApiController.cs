using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.Venda;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Certificacao_Assinatura.Integracoes;
using Sinapse.Integracao.Certificacao_Assinatura.Repositorio;
using Sinapse.Integracao.MostqContenctExtractionModels.Models;
using Sinapse.Integracao.MostqVioExtractionModels.Models;
using Sinapse.Integracao.ProvaVida.Integracoes;
using Sinapse.Integracao.ProvaVida.Models;
using Sinapse.Integracao.ProvaVida.Repositorio;
using Sinapse.Integracao.ReconhecimentoFacial.Integracoes;
using Sinapse.Integracao.ReconhecimentoFacial.Models;
using Sinapse.Integracao.ReconhecimentoFacial.Repositorio;
using Sinapse.Integracao.ValidacaoDocumento.Integracoes;
using Sinapse.Integracao.ValidacaoDocumento.Repositorio;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using Xceed.Words.NET;
using EnumEtapaVenda = Sinapse.Model.enumerados.EnumEtapaVenda;
using StatusEtapaVenda = Sinapse.Model.enumerados.StatusEtapaVenda;

namespace Aniel.Connect.ControllersWebApi
{
    [AllowAnonymous]
    //[Authorize]
    public class Termo_AdesaoApiController : BaseApiController
    {
        #region Mobile

        [HttpGet()]
        [Route("api/TermoAdesaoApi/getTermoAdesaoMobile")]
        public async Task<GetApiRetorno<List<TermoAdesaoMobile>>> GetTermoAdesaoMobile(Int64? idVenda)
        {
            GetApiRetorno<List<TermoAdesaoMobile>> retorno = new GetApiRetorno<List<TermoAdesaoMobile>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                try
                {
                    Venda venda = null;
                    Cliente cliente = null;
                    if (idVenda.HasValue)
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == idVenda.Value);
                        cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    var listaPlanosProdutos = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id);

                    Configuracoes_Gerais config = await repoConfig.selectFirst();
                    var termo = new List<TermoAdesaoMobile>();

                    var infoEmpresa = GetInfoEmpresa(Convert.ToString(venda.Vendedor.Filial.Empresa.CodEmpresa));

                    foreach (var planoProduto in listaPlanosProdutos)
                    {
                        var t = new TermoAdesaoMobile();

                        t.IdVenda = venda.Id;
                        t.Cliente = cliente.NomeFantasia;
                        t.Plano = planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao;
                        t.ValorPlano = Convert.ToString(planoProduto.Valor_Final);
                        t.ValorInstalacao = Convert.ToString(planoProduto.Valor_Instalacao_Final);
                        t.TesteGratis = planoProduto.Dias_Teste_Gratis ?? 0;
                        t.Vencimento = planoProduto.Forma_Pagamento_Cliente.Dia_Vencimento;
                        t.Fidelidade = planoProduto.Meses_Carencia ?? 0;
                        t.Empresa = infoEmpresa.RazaoSocial;
                        t.Cnpj = FormatarCpfCnpj(infoEmpresa.Cnpj);
                        t.UrlLogo = config.OLA_URL_Logo;
                        t.EnderecoEmpresa = infoEmpresa.Endereco + " - " + infoEmpresa.Cidade + " - " + infoEmpresa.Estado;

                        termo.Add(t);
                    }

                    retorno.Objeto = termo;
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }


        [HttpPost]
        [Route("api/TermoAdesaoApi/sendAssinaturaTermoAdesao")]
        public async Task<GetApiRetorno<Boolean>> SendAssinaturaTermoAdesao(ParamWebApi<AssinaturaTermoAdesaoDados> value)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            {
                try
                {
                    Venda venda = null;
                    var dados = value.Objeto;

                    if (dados != null)
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == dados.IdVenda);
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    if (venda == null)
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    if (String.IsNullOrEmpty(dados.Assinatura))
                    {
                        await SalvarAssinatura(dados.Assinatura, venda);
                    }

                    if (dados.AtualizarStatus)
                    {
                        venda.Termo_Adesao_Assinado = true;
                        await repoVenda.Save(venda);
                    }

                    string caminho = String.Format(@"{0}\Fotos\AssinaturaTermoAdesao\{1}.png", CONSTANTES.CaminhoProjetoPremier ?? String.Empty, venda.Id);
                    if (File.Exists(caminho)) File.Delete(caminho);

                    retorno.Objeto = true;
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }

        private async Task<long> SalvarAssinatura(string assinatura, Venda venda)
        {
            using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
            using (var repo = new Conexao<Assinatura_Termo_Adesao>(sessionFoto))
            {
                using var stream = new MemoryStream();
                using System.Drawing.Image imagem = ImageService.Base64ToImage(assinatura);
                imagem.Save(stream, ImageFormat.Jpeg);
                stream.Position = 0;

                Assinatura_Termo_Adesao assinatura_termo = new Assinatura_Termo_Adesao();
                assinatura_termo.Id_Venda = venda.Id;
                assinatura_termo.Assinatura = stream.ToArray();

                await repo.SaveOrUpdate(assinatura_termo);
                return assinatura_termo.Id_Venda;
            }
        }

        [HttpGet]
        [Route("api/TermoAdesaoApi/CheckTermoAssinado")]
        public GetApiRetorno<bool> CheckTermoAssinado(long idVenda)
        {
            var retorno = new GetApiRetorno<bool>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Venda>(session))
                {
                    retorno.Objeto = repo.selectFirst(p => p.Id == idVenda).Result.Termo_Adesao_Assinado;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/TermoAdesaoApi/CheckTermoAssinadoTroca")]
        public GetApiRetorno<bool> CheckTermoAssinadoTroca(long idTroca)
        {
            var retorno = new GetApiRetorno<bool>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<TrocaTitularidade>(session))
                {
                    retorno.Objeto = repo.selectFirst(p => p.Id == idTroca).Result.TermoAdesaoAssinado;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/TermoAdesaoApi/getAssinaturaTermoAdesao")]
        public async Task<GetApiRetorno<List<AssinaturaTermoAdesaoDados>>> GetAssinaturaTermoAdesao(Int64? idVenda)
        {
            GetApiRetorno<List<AssinaturaTermoAdesaoDados>> retorno = new GetApiRetorno<List<AssinaturaTermoAdesaoDados>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            {
                try
                {
                    Venda venda = null;
                    if (idVenda.HasValue)
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == idVenda.Value);
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    var dados = new List<AssinaturaTermoAdesaoDados>();
                    var assinatura = new AssinaturaTermoAdesaoDados();

                    assinatura.IdVenda = venda.Id;
                    assinatura.Assinatura = GetAssinatura(venda, false);
                    assinatura.AtualizarStatus = venda.Termo_Adesao_Assinado;

                    dados.Add(assinatura);

                    retorno.Objeto = dados;
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }

        private string GetAssinatura(Venda venda, bool isCaminho)
        {
            string retorno;

            string caminhoProjetoWebApi = CONSTANTES.CaminhoProjetoWebApiPremier;
            string caminhoProjeto = CONSTANTES.CaminhoProjetoPremier;

            if (String.IsNullOrEmpty(caminhoProjetoWebApi))
            {
                throw new Exception("CaminhoProjetoWebApiPremier não definido no empresa.config");
            }

            if (String.IsNullOrEmpty(caminhoProjeto))
            {
                throw new Exception("CaminhoProjetoPremier não definido no empresa.config");
            }

            retorno = string.Format(@"{0}/Arquivos/Fotos/AssinaturaTermoAdesao/{1}.png", caminhoProjetoWebApi, venda.Id.ToString());

            string caminho = String.Format(@"{0}\Arquivos\Fotos\AssinaturaTermoAdesao\{1}.png", caminhoProjeto, venda.Id.ToString());

            if (!File.Exists(caminho))
            {
                try
                {
                    string pastaFoto = String.Format(@"{0}\Arquivos\Fotos", caminhoProjeto);
                    if (!Directory.Exists(pastaFoto)) Directory.CreateDirectory(pastaFoto);

                    string pastaAssinatura = String.Format(@"{0}\Arquivos\Fotos\AssinaturaTermoAdesao", caminhoProjeto);
                    if (!Directory.Exists(pastaAssinatura)) Directory.CreateDirectory(pastaAssinatura);

                    byte[] fotoByte = BuscarAssinaturaBanco(venda.Id).Result;
                    using MemoryStream ms = new MemoryStream(fotoByte);
                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                    var foto64 = ImageService.ImageToBase64(img, ImageFormat.Png);
                    img.Save(caminho, ImageFormat.Png);
                }
                catch (Exception)
                {
                    retorno = null;
                    caminho = null;
                }
            }
            return isCaminho ? caminho : retorno;
        }

        private async Task<byte[]> BuscarAssinaturaBanco(long idVenda)
        {
            using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
            using (var repo = new Conexao<Assinatura_Termo_Adesao>(sessionFoto))
            {
                byte[] retorno = null;

                Assinatura_Termo_Adesao assinatura = await repo.selectFirst(p => p.Id_Venda == idVenda);

                if (assinatura != null)
                {
                    retorno = assinatura.Assinatura;
                }

                return retorno;
            }
        }


        [HttpPost]
        [Route("api/TermoAdesaoApi/marcarTermoAdesaoAssinado")]
        public async Task<GetApiRetorno<Boolean>> MarcarTermoAdesaoAssinado(ParamWebApi<AssinaturaTermoAdesaoDados> value)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            {
                try
                {
                    Venda venda = null;
                    var dados = value.Objeto;

                    if (dados != null)
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == dados.IdVenda);
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    if (venda == null)
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    var assinatura = BuscarAssinaturaBanco(venda.Id).Result;

                    if (assinatura != null)
                    {
                        venda.Termo_Adesao_Assinado = true;
                        await repoVenda.SaveOrUpdate(venda);
                    }

                    retorno.Objeto = true;
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }


        [HttpGet()]
        [Route("api/TermoAdesaoApi/enviarTermoAdesao")]
        public async Task<PostApiRetorno<Int64>> EnviarTermoAdesao(Int64? IdVenda)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    Venda venda = null;
                    Cliente cliente = null;

                    using (var repoVenda = new Conexao<Venda>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                    using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    using (var repoEtapaVenda = new Conexao<EtapaVenda>(session))
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == IdVenda);
                        var etapaTermo = await repoEtapaVenda.selectFirst(p => p.Venda == venda && p.Etapa == Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao);

                        if (etapaTermo.Status != StatusEtapaVenda.Aprovado && etapaTermo.Status != StatusEtapaVenda.AprovadoManualmente)
                        {
                            etapaTermo.Status = StatusEtapaVenda.Pendente;
                            await repoEtapaVenda.Update(etapaTermo);
                        }

                        cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);
                        var listaPlanosProdutos = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id);

                        if (String.IsNullOrEmpty(cliente.Email_Principal))
                        {
                            throw new Exception("Cliente não possui email vinculado");
                        }

                        Configuracoes_Gerais config = await repoConfig.selectFirst();

                        if (venda?.Vendedor?.Filial?.Empresa?.CodEmpresa == null)
                        {
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do termo de adesão");
                        }

                        InfoEmpresaTermoAdesao infoEmpresa = GetInfoEmpresa(Convert.ToString(venda.Vendedor.Filial.Empresa.CodEmpresa));

                        if (infoEmpresa == null)
                        {
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do termo de adesão");
                        }

                        EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);


                        var obj_link = await GerarLinkTermoAdesao(venda.Id, true);
                        String urlTermoAdesao = obj_link.IdObjeto;
                        //String urlTermoAdesao = $@"{CONSTANTES.URLPortal}/pt/Termo_Adesao/Formulario/{venda.Id}";

                        var produtos = VendaApiController.GerarTabelaProdutosHTML(listaPlanosProdutos);

                        using var stream = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_termo_adesao.html");
                        String arquivo_email = stream.ReadToEnd();
                        String arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", cliente.NomeFantasia)
                                                                 .Replace("{{URL_TERMO_ADESAO}}", urlTermoAdesao)
                                                                 .Replace("{{URL_LOGO}}", config.OLA_URL_Logo)
                                                                 .Replace("{{PRODUTOS}}", produtos);

                        if (!String.IsNullOrEmpty(cliente.Email_Principal))
                        {
                            emailService.EnviarMensagem(cliente.Email_Principal, $@"Bem-vindo(a) à {infoEmpresa.Nome}! Segue Termo de adesão para assinatura.", arquivo_modificado, CONSTANTES.Email_UsaSsl == "S");
                        }
                        else
                        {
                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.TERMO_ADESAO_CLIENTE_SEM_EMAIL, cliente.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }
                    }

                    retorno.IdObjeto = venda.Id;
                    retorno.Ok = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    transaction.Rollback();
                    retorno.IdObjeto = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/TermoAdesaoApi/enviarTermoAdesaoTroca")]
        public async Task<PostApiRetorno<Int64>> EnviarTermoTrocaTitulardade(Int64? idTrocaTitularidade)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    TrocaTitularidade troca = null;
                    Cliente clienteAtual = null;
                    Cliente clienteNovo = null;

                    using (var repoTrocaTitularidade = new Conexao<TrocaTitularidade>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                    using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    using (var repoEtapa = new Conexao<EtapaVenda>(session))
                    using (var repoVenda = new Conexao<Venda>(session))
                    {
                        troca = await repoTrocaTitularidade.selectFirst(p => p.Id == idTrocaTitularidade);
                        var etapaTermo = await repoEtapa.selectFirst(p => p.TrocaTitularidade == troca && p.Etapa == Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao);

                        if (etapaTermo.Status != StatusEtapaVenda.Aprovado && etapaTermo.Status != StatusEtapaVenda.AprovadoManualmente)
                        {
                            etapaTermo.Status = StatusEtapaVenda.Pendente;
                            await repoEtapa.Update(etapaTermo);
                        }

                        clienteAtual = await repoCliente.selectFirst(p => p.Id == troca.ClienteTitularAtual.Id);
                        clienteNovo = await repoCliente.selectFirst(p => p.Id == troca.ClienteTitularNovo.Id);
                        var listaPlanosProdutos = clienteAtual.PlanosProdutos.Where(p => p.ClienteContrato.Id == troca.NumeroContrato &&
                                                                               p.Cliente.Id == troca.ClienteTitularAtual.Id);

                        var vendaFirst = await repoVenda.selectFirst(x => x.Vendedor != null);

                        if (String.IsNullOrEmpty(clienteNovo.Email_Principal))
                        {
                            throw new Exception("Cliente não possui email vinculado");
                        }

                        Configuracoes_Gerais config = await repoConfig.selectFirst();

                        if (vendaFirst?.Vendedor?.Filial?.Empresa?.CodEmpresa == null)
                        {
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do termo de adesão");
                        }

                        InfoEmpresaTermoAdesao infoEmpresa = GetInfoEmpresa(Convert.ToString(vendaFirst.Vendedor.Filial.Empresa.CodEmpresa));

                        if (infoEmpresa == null)
                        {
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do termo de adesão");
                        }

                        EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);
                        var criptografia = Criptografia_AES256.doEncryptAES(Convert.ToString(troca.Id));
                        String urlTermoAdesao = $@"{CONSTANTES.URLPortal}/pt/Termo_Adesao/Formulario/{criptografia}";

                        var produtos = "";

                        foreach (var planoProduto in listaPlanosProdutos)
                        {
                            if (planoProduto.Combo != null)
                            {
                                var plano_grupo = planoProduto.Combo.PlanosProdutos.GroupBy(x => new { x.Plano }).Select(x => x.First()).ToList();
                                foreach (var planoProdutoCombo in plano_grupo)
                                {
                                    using var stream_produtos = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_termo_adesao_produtos.html");
                                    {
                                        String produto = stream_produtos.ReadToEnd();
                                        var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                                        produto = produto.Replace("{{DESC}}", planoProdutoCombo.Plano != null ? planoProdutoCombo.Plano.Descricao : planoProdutoCombo.Produto.Descricao)
                                             .Replace("{{VENCIMENTO}}", planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "")
                                             .Replace("{{TESTE}}", dias == 1 ? dias + "dia" : dias + " dias")
                                             .Replace("{{VALOR}}", planoProdutoCombo.Plano != null ? planoProdutoCombo?.Valor.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");

                                        produtos += produto;
                                    }
                                }
                            }
                            else
                            {
                                using var stream_produtos = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_termo_adesao_produtos.html");
                                {
                                    String produto = stream_produtos.ReadToEnd();
                                    var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                                    produto = produto.Replace("{{DESC}}", planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao)
                                         .Replace("{{VENCIMENTO}}", planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "")
                                         .Replace("{{TESTE}}", dias == 1 ? dias + "dia" : dias + " dias")
                                         .Replace("{{VALOR}}", planoProduto.Valor_Final != null ? planoProduto.Valor_Final?.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");

                                    produtos += produto;
                                }
                            }
                        }

                        using var stream = File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_termo_adesao.html");
                        String arquivo_email = stream.ReadToEnd();
                        String arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", clienteNovo.NomeFantasia)
                                                                 .Replace("{{URL_TERMO_ADESAO}}", urlTermoAdesao)
                                                                 .Replace("{{URL_LOGO}}", config.OLA_URL_Logo)
                                                                 .Replace("{{PRODUTOS}}", produtos);

                        if (!String.IsNullOrEmpty(clienteNovo.Email_Principal))
                        {
                            emailService.EnviarMensagem(clienteNovo.Email_Principal, $@"Bem-vindo(a) à {infoEmpresa.Nome}! Segue Termo de adesão para assinatura.", arquivo_modificado, CONSTANTES.Email_UsaSsl == "S");
                        }
                        else
                        {
                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Mensagem = String.Format(RsCadastros.TERMO_ADESAO_CLIENTE_SEM_EMAIL, clienteNovo.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }
                    }

                    retorno.IdObjeto = troca.Id;
                    retorno.Ok = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    transaction.Rollback();
                    retorno.IdObjeto = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/TermoAdesaoApi/GerarLinkTermoAdesao")]
        public async Task<PostApiRetorno<String>> GerarLinkTermoAdesao(Int64 IdVenda, Boolean termo)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoVenda = new Conexao<Venda>(session))
                {
                    var venda = await repoVenda.selectFirst(p => p.Id == IdVenda);
                    if (venda != null)
                    {
                        var criptografia = Criptografia_AES256.doEncryptAES(Convert.ToString(venda.Id));
                        retorno.IdObjeto = termo ? $@"{CONSTANTES.URLPortal}/pt/Termo_Adesao/Formulario/{criptografia}" : $@"{CONSTANTES.URLPortal}/pt/Venda/EmailChecklist?Id_Venda={criptografia}&Email=S";
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = "";
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/TermoAdesaoApi/gerarTermoAdesaoIntegracao")]
        public async Task<GetApiRetorno<String>> GerarTermoAdesaoIntegracao(Int64? IdVenda)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<string>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                try
                {
                    var configEmpresa = await repoConfig.selectFirst();
                    var venda = await repoVenda.selectFirst(p => p.Id == IdVenda);

                    if (configEmpresa.Utiliza_Certificacao_Assinatura)
                    {
                        if (String.IsNullOrEmpty(venda.Url_Assinar_Documento))
                        {
                            var retornoApi = await FindById(IdVenda);
                            if (retornoApi.Ok)
                            {

                                switch (configEmpresa.Tipo_Integracao_Assinatura)
                                {
                                    case TipoIntegracaoAssinaturaEnum.Lexio:
                                        retorno.Objeto = (await repoVenda.selectFirst(p => p.Id == IdVenda)).Url_Assinar_Documento;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                throw new Exception(retornoApi.Mensagem, new Exception(retornoApi.InnerException));
                            }
                        }
                        else retorno.Objeto = venda.Url_Assinar_Documento;
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }


        [HttpGet]
        [Route("api/TermoAdesaoApi/validarAssinaturaTermoAdesaoIntegracao")]
        public async Task<GetApiRetorno<bool>> ValidarAssinaturaTermoAdesaoIntegracao(Int64? IdVenda)
        {
            GetApiRetorno<bool> retorno = new GetApiRetorno<bool>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoVenda = new Conexao<Venda>(session))
            {
                try
                {
                    Venda venda = null;
                    venda = await repoVenda.selectFirst(p => p.Id == IdVenda);

                    Configuracoes_Gerais config = await repoConfig.selectFirst();

                    Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                    {
                        Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                        Token_Documento_Assinatura = venda.Token_Documento_Assinatura,
                        Id_Venda = venda.Id,
                        Id_Cliente = venda.Id_Cliente
                    };

                    if (config.Utiliza_Certificacao_Assinatura)
                    {
                        Sinapse.Model.Consultar_Documento_ResponseModel retornoIntegracao = new Sinapse.Model.Consultar_Documento_ResponseModel();
                        switch (config.Tipo_Integracao_Assinatura)
                        {
                            case TipoIntegracaoAssinaturaEnum.Lexio:
                                retornoIntegracao = new Sinapse.Integracao.Certificacao_Assinatura.Repositorio.IntegracaoAssinaturaRepository().Consultar_Documento(new Sinapse.Integracao.Certificacao_Assinatura.Integracoes.Lexio(), session, configCert);
                                break;
                            default:
                                break;
                        }
                        if (!retornoIntegracao.IsAssinado)
                        {
                            retorno.Objeto = false;
                            throw new Exception(Resources.RsCadastros.TERMO_ADESAO_PENDENTE);
                        }

                        venda.Termo_Adesao_Assinado = retornoIntegracao.IsAssinado;
                        venda.Url_Documento_Assinado = retornoIntegracao.Link_Download_Documento;
                    }

                    await repoVenda.SaveOrUpdate(venda);

                    retorno.Objeto = true;
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    //retorno.IdObjeto.IdVenda = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        #endregion

        [AllowAnonymous]
        [HttpGet()]
        [Route("api/TermoAdesaoApi/FindById")]
        public async Task<GetApiRetorno<TermoAdesaoFormularioModel>> FindById(Int64? IdVenda)
        {
            GetApiRetorno<TermoAdesaoFormularioModel> retorno = new GetApiRetorno<TermoAdesaoFormularioModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoAnexosContrato = new Conexao<AnexoContratoAceiteModel>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoAcoes = new Conexao<Acoes_Prova_Vida_Termo>(session))
            using (var apiVenda = new VendaApiController())
            {
                try
                {
                    Venda venda = null;
                    Cliente cliente = null;

                    if (IdVenda.HasValue)
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == IdVenda.Value);
                        cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    Configuracoes_Gerais config = await repoConfig.selectFirst();

                    var acoesTermo = await repoAcoes.select();
                    string acoes = JsonConvert.SerializeObject(acoesTermo);

                    Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                    {
                        Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                        Id_Cliente = cliente.Id,
                        Url_Assinar_Documento = venda.Url_Assinar_Documento,
                        Email_Principal = cliente.Email_Principal,
                        Email_Auxiliar = cliente.Email_Auxiliar,
                        NomeFantasia = cliente.NomeFantasia,
                        Id_Venda = venda.Id,
                        Tipo_Integracao_Assinatura = venda.Tipo_Integracao_Assinatura
                    };

                    var camihoPdf = await GeraTermoAdesaoCliente(cliente, venda);

                    string urlTermoIntegracao = String.Empty;
                    if (config.Utiliza_Certificacao_Assinatura)
                    {
                        if (String.IsNullOrEmpty(venda.Url_Assinar_Documento))
                        {
                            var retornoIntegracao = new Enviar_Documento_ResponseModel();
                            switch (config.Tipo_Integracao_Assinatura)
                            {
                                case TipoIntegracaoAssinaturaEnum.Lexio:

                                    Byte[] bytes = File.ReadAllBytes(camihoPdf);
                                    String file = Convert.ToBase64String(bytes);

                                    retornoIntegracao = new IntegracaoAssinaturaRepository().Enviar_Documento(new Lexio(), session, configCert, file);

                                    venda.Tipo_Integracao_Assinatura = TipoIntegracaoAssinaturaEnum.Lexio;
                                    break;
                                default:
                                    break;

                            }

                            venda.Url_Assinar_Documento = retornoIntegracao.Link_Assinatura;
                            venda.Token_Documento_Assinatura = retornoIntegracao.Id_Documento_Integracao;

                            await repoVenda.SaveOrUpdate(venda);
                        }

                        urlTermoIntegracao = GetHtmlTermoAdesaoIntegracao(cliente, config, venda);
                    }

                    string urlPdfTermoAdesao = $@"{CONSTANTES.URLPortal}/Arquivos/TermoAdesao/termo_adesao_{venda.Id}.pdf";

                    var retorno_contratos = (await apiVenda.GetContratoGerarPdf(session, cliente.Id, venda.Id, null)).Objeto;
                    var lista_contratos = new List<ContratosList>();
                    var listaAnexos = new List<AnexoContrato>();
                    var anexosContrato = await repoAnexosContrato.select(p => p.IdVenda == venda.Id && p.ExigeAceite);
                    var index = 0;
                    foreach (var item in retorno_contratos)
                    {
                        var indexOf = item.Caminho_Contrato.IndexOf("\\Arquivos");
                        var caminho_antigo = item.Caminho_Contrato.Substring(0, indexOf);
                        var caminho = item.Caminho_Contrato.Replace(caminho_antigo, $@"{CONSTANTES.URLPortal}");
                        lista_contratos.Add(new ContratosList
                        {
                            Id = index,
                            Caminho = caminho,
                            Nome_Contrato = item.Nome_Contrato,

                        });
                        index++;
                    }
                    index = 0;
                    foreach (var item in anexosContrato)
                    {
                        listaAnexos.Add(new AnexoContrato
                        {
                            Id = index++,
                            Url = $"{CONSTANTES.URLPortal}/Arquivos/AnexosContrato/{item.IdVenda}/{item.NomeArquivo}.{item.Extensao}",
                            NomeArquivo = item.NomeArquivo,
                            Extensao = item.Extensao,
                        });
                    }

                    var termo = new TermoAdesaoFormularioModel
                    {
                        IdVenda = venda.Id,
                        Cliente = cliente.NomeFantasia,
                        Empresa = config.Nome_Cliente_Sinapse,
                        UrlLogo = config.OLA_URL_Logo,
                        Termo = !String.IsNullOrEmpty(urlTermoIntegracao) ? urlTermoIntegracao : urlPdfTermoAdesao,
                        Contratos = lista_contratos,
                        AnexosContrato = listaAnexos,
                        Aceito = false,
                        isIntegracao = config.Utiliza_Certificacao_Assinatura,
                        ExigeFotoTermo = config.Exige_Foto_Termo,
                        ExigeTermoContrato = config.Exige_Termo_Contrato,
                        Utiliza_Envio_Automatico_Contrato = config.Utiliza_Envio_Automatico_Contrato,
                        TemAnexosDocumento = listaAnexos.Count > 0,
                        TipoComprovanteTermo = config.Tipo_Comprovante_Termo,
                        Acoes_Prova_Vida = acoes,
                        Permite_Anexar_Doc = config.Permite_Anexar_Doc_Termo,
                    };

                    retorno.Objeto = termo;
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }


        [AllowAnonymous]
        [HttpGet()]
        [Route("api/TermoAdesaoApi/FindByIdTroca")]
        public async Task<GetApiRetorno<TermoAdesaoFormularioModel>> FindByIdTrocaTitularidade(Int64? IdTroca)
        {
            GetApiRetorno<TermoAdesaoFormularioModel> retorno = new GetApiRetorno<TermoAdesaoFormularioModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoTrocaTitularidade = new Conexao<TrocaTitularidade>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoAcoes = new Conexao<Acoes_Prova_Vida_Termo>(session))
            using (var apiVenda = new VendaApiController())
            {
                try
                {
                    TrocaTitularidade troca = null;
                    Cliente clienteAtual = null;
                    Cliente clienteNovo = null;

                    if (IdTroca.HasValue)
                    {
                        troca = await repoTrocaTitularidade.selectFirst(p => p.Id == IdTroca.Value);
                        clienteAtual = await repoCliente.selectFirst(p => p.Id == troca.ClienteTitularAtual.Id);
                        clienteNovo = await repoCliente.selectFirst(p => p.Id == troca.ClienteTitularNovo.Id);
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    Configuracoes_Gerais config = await repoConfig.selectFirst();

                    var acoesTermo = await repoAcoes.select();
                    string acoes = JsonConvert.SerializeObject(acoesTermo);

                    var camihoPdf = await TermoAdesaoTrocaTitular(clienteAtual, clienteNovo, troca);

                    string urlPdfTermoAdesao = $@"{CONSTANTES.URLPortal}/Arquivos/TermoAdesao/termo_adesao_{troca.Id}.pdf";

                    var retorno_contratos = (await apiVenda.GetContratoGerarPdf(session, clienteAtual.Id, null, troca.NumeroContrato)).Objeto;
                    var lista_contratos = new List<ContratosList>();
                    var index = 0;
                    foreach (var item in retorno_contratos)
                    {
                        var indexOf = item.Caminho_Contrato.IndexOf("\\Arquivos");
                        var caminho_antigo = item.Caminho_Contrato.Substring(0, indexOf);
                        var caminho = item.Caminho_Contrato.Replace(caminho_antigo, $@"{CONSTANTES.URLPortal}");
                        lista_contratos.Add(new ContratosList
                        {
                            Id = index,
                            Caminho = caminho
                        });
                        index++;
                    }

                    var termo = new TermoAdesaoFormularioModel
                    {
                        IdVenda = troca.Id,
                        Cliente = clienteNovo.NomeFantasia,
                        Empresa = config.Nome_Cliente_Sinapse,
                        UrlLogo = config.OLA_URL_Logo,
                        Termo = urlPdfTermoAdesao,
                        Contratos = lista_contratos,
                        Aceito = false,
                        isIntegracao = false,
                        ExigeFotoTermo = config.Exige_Foto_Termo,
                        Utiliza_Envio_Automatico_Contrato = config.Utiliza_Envio_Automatico_Contrato,
                        ExigeTermoContrato = config.Exige_Termo_Contrato,
                        TipoComprovanteTermo = config.Tipo_Comprovante_Termo,
                        Acoes_Prova_Vida = acoes,
                        Permite_Anexar_Doc = config.Permite_Anexar_Doc_Termo,
                    };

                    retorno.Objeto = termo;
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/TermoAdesaoApi/getPdfTermoAdesao")]
        public async Task<GetApiRetorno<String>> GetPdfTermoAdesao(Int64? idVenda)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<String>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repositoryAnexo = new Conexao<DocTermoAnexo>(session))
            using (var vendaApi = new VendaApiController())
            {
                try
                {
                    Venda venda = await repoVenda.selectFirst(x => x.Id == idVenda);
                    Cliente cliente = await repoCliente.selectFirst(x => x.Id == venda.Id_Cliente);

                    string caminhoProjetoWebApi = CONSTANTES.URLPortal;
                    string caminhoProjeto = CONSTANTES.CaminhoFisico;

                    if (String.IsNullOrEmpty(caminhoProjetoWebApi))
                    {
                        throw new Exception("CaminhoProjetoWebApiPremier não definido no empresa.config");
                    }

                    if (String.IsNullOrEmpty(caminhoProjeto))
                    {
                        throw new Exception("CaminhoProjetoPremier não definido no empresa.config");
                    }

                    if (idVenda.HasValue)
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == idVenda.Value);
                        cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    Configuracoes_Gerais config = await repoConfig.selectFirst();

                    Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                    {
                        Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                        Id_Venda = venda.Id,
                    };

                    if (venda.Termo_Adesao_Assinado)
                    {
                        if (config.Utiliza_Certificacao_Assinatura)
                        {
                            switch (config.Tipo_Integracao_Assinatura)
                            {
                                case TipoIntegracaoAssinaturaEnum.Lexio:
                                    retorno.Objeto = new IntegracaoAssinaturaRepository().Get_Documento_Pdf(new Lexio(), configCert, venda.Url_Documento_Assinado);
                                    retorno.Ok = true;
                                    return retorno;
                                default:
                                    break;
                            }
                        }
                        var nome_pdf = String.Format(@"termo_adesao_{0}.pdf", venda.Id);
                        string caminho_pdf_local = String.Format(@"{0}\Arquivos\TermoAdesao\{1}", caminhoProjeto, nome_pdf);
                        if (File.Exists(caminho_pdf_local))
                        {
                            string caminho_pdf = String.Format(@"{0}/Arquivos/TermoAdesao/{1}", caminhoProjetoWebApi, nome_pdf);
                            string caminho_docx_local = String.Format(@"{0}\Arquivos\TermoAdesao\{1}", caminhoProjeto, $@"termo_adesao_{venda.Id}.docx");
                            string caminho_html_local = String.Format(@"{0}\Arquivos\TermoAdesao\{1}", caminhoProjeto, $@"termo_adesao_{venda.Id}.html");

                            var lista_anexo = await repositoryAnexo.select(x => x.Nome == Convert.ToString(venda.Id));
                            var obj_anexo = lista_anexo?.LastOrDefault();

                            var infosAceite = new InformacoesAceite
                            {
                                EmailDestinatario = cliente.Email_Principal,
                                IpOrigem = venda.IpOrigemAceite,
                                UrlAceite = venda.UrlAceiteDigital,
                                PortaOrigem = venda.PortaOrigemAceite,
                                HashIdentificador = venda.CodigoHashTermoAceite,
                                UserAgent = venda.UserAgentTermoAceite,
                                DataHoraAceite = venda.DataHoraAssinaturaTermo.Value.ToString("dd/MM/yyyy 'às' HH:mm:ss '(UTC-3)'"),
                                AceiteTermoAdesao = venda.Termo_Adesao_Assinado,
                                CaminhoSelfie = obj_anexo != null ? obj_anexo.Url : null
                            };

                            if (!venda.AdicionadoAceiteDigital)
                            {                             
                                var aceiteDigital = await vendaApi.RegistroAceiteDigital(infosAceite);

                                //substituindo arquivos antigos para incluir informações importantes do aceite registrado do cliente/usuario
                                ConvertToPdfService.incluirAceiteDigitalDocx(caminho_docx_local, aceiteDigital);
                                ConvertToPdfService.convertDocxToHtml(caminho_html_local, caminho_docx_local);
                                ConvertToPdfService.convertHtmlToPdf(caminho_pdf_local, caminho_html_local);
                                venda.AdicionadoAceiteDigital = true;
                                await repoVenda.SaveOrUpdate(venda);
                            }
                            else
                            {
                                if (ConvertToPdfService.docContemRegistroDigital(caminho_docx_local))
                                {
                                    retorno.Ok = true;
                                    retorno.Objeto = caminho_pdf;
                                    return retorno;
                                }
                                else
                                {                                
                                    var aceiteDigital = await vendaApi.RegistroAceiteDigital(infosAceite);

                                    ConvertToPdfService.incluirAceiteDigitalDocx(caminho_docx_local, aceiteDigital);
                                    ConvertToPdfService.convertDocxToHtml(caminho_html_local, caminho_docx_local);
                                    ConvertToPdfService.convertHtmlToPdf(caminho_pdf_local, caminho_html_local);

                                    venda.AdicionadoAceiteDigital = true;
                                    await repoVenda.SaveOrUpdate(venda);
                                }
                            }

                            retorno.Ok = true;
                            retorno.Objeto = caminho_pdf;
                            return retorno;
                        }
                    }

                    var caminhoPdf = await GeraTermoAdesaoCliente(cliente, venda);
                    retorno.Ok = true;
                    retorno.Objeto = caminhoPdf;

                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/TermoAdesaoApi/getPdfTrocaTitularidade")]
        public async Task<GetApiRetorno<String>> PdfTrocaTitularidade(Int64? idTroca)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<String>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoTrocaTitularidade = new Conexao<TrocaTitularidade>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                try
                {
                    TrocaTitularidade troca = await repoTrocaTitularidade.selectFirst(x => x.Id == idTroca);

                    if (troca is null)
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    Cliente clienteAtual = await repoCliente.selectFirst(x => x.Id == troca.ClienteTitularAtual.Id);
                    Cliente clienteNovo = await repoCliente.selectFirst(x => x.Id == troca.ClienteTitularNovo.Id);

                    string caminhoProjetoWebApi = CONSTANTES.URLPortal;
                    string caminhoProjeto = CONSTANTES.CaminhoFisico;

                    if (String.IsNullOrEmpty(caminhoProjetoWebApi))
                    {
                        throw new Exception("CaminhoProjetoWebApiPremier não definido no empresa.config");
                    }

                    if (String.IsNullOrEmpty(caminhoProjeto))
                    {
                        throw new Exception("CaminhoProjetoPremier não definido no empresa.config");
                    }

                    Configuracoes_Gerais config = await repoConfig.selectFirst();

                    Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                    {
                        Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                        Id_Venda = troca.Id,
                    };

                    if (troca.TermoAdesaoAssinado)
                    {
                        var nome_pdf = String.Format(@"termo_adesao_{0}.pdf", troca.Id);
                        string caminho_pdf_local = String.Format(@"{0}\Arquivos\TermoAdesao\{1}", caminhoProjeto, nome_pdf);
                        if (File.Exists(caminho_pdf_local))
                        {
                            string caminho_pdf = String.Format(@"{0}/Arquivos/TermoAdesao/{1}", caminhoProjetoWebApi, nome_pdf);

                            retorno.Ok = true;
                            retorno.Objeto = caminho_pdf;

                            return retorno;
                        }
                    }

                    var caminhoPdf = await TermoAdesaoTrocaTitular(clienteAtual, clienteNovo, troca);
                    retorno.Ok = true;
                    retorno.Objeto = caminhoPdf;

                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }



        public async Task<String> GetPdfTermoAdesaoEmail(Int64? idVenda)
        {

            String retorno = null;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<TermoAdesao>(session))
            {
                try
                {
                    var venda = await repoVenda.selectFirst(x => x.Id == idVenda);
                    var cliente = await repoCliente.selectFirst(x => x.Id == venda.Id_Cliente);
                    var termoPadrao = await repoConfig.selectFirst();

                    retorno = await GeraTermoAdesaoCliente(cliente, venda);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message.ToString());
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/TermoAdesaoApi/GravarFormulario")]
        public async Task<PostApiRetorno<TermoAdesaoFormularioResponseModel>> GravarFormulario(ParamWebApi<TermoAdesaoFormularioModel> value)
        {
            PostApiRetorno<TermoAdesaoFormularioResponseModel> retorno = new PostApiRetorno<TermoAdesaoFormularioResponseModel>();
            retorno.IdObjeto = new TermoAdesaoFormularioResponseModel();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var repoEtapaVendaLog = new Conexao<LogEtapaVendas>(sessionLog))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repositoryLogs = new Conexao<LogVendas>(sessionLog))
            using (var repoTrocaTitularidade = new Conexao<TrocaTitularidade>(session))
            using (var api = new VendaApiController())
            using (var repositoryAnexo = new Conexao<DocTermoAnexo>(session))
            using (var repositoryEtapa = new Conexao<EtapaVenda>(session))
            using (var repoAtividadeWorkflow = new Conexao<Workflow_VendasAtividade>(session))
            using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
            using (Conexao<Cliente_PlanoProduto> repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            {
                try
                {
                    Venda venda = null;
                    TrocaTitularidade troca = null;
                    Cliente cliente = null;

                    venda = await repoVenda.selectFirst(p => p.Id == value.Objeto.IdVenda);
                    troca = await repoTrocaTitularidade.selectFirst(p => p.Id == value.Objeto.IdVenda);
                    Int64 IdCliente = venda != null ? venda.Id_Cliente.Value : troca.ClienteTitularNovo.Id;
                    cliente = await repoCliente.selectFirst(x => x.Id == IdCliente);
                    
                    if (venda != null)
                    {
                        EtapaVenda etapa = await repositoryEtapa.selectFirst(p => p.Venda.Id == value.Objeto.IdVenda && p.Etapa == EnumEtapaVenda.TermoAdesao);
                        EtapaVenda etapaClone = (EtapaVenda)etapa.Clone();

                        if (venda.Termo_Adesao_Assinado && !value.Objeto.Aceito)
                        {
                            retorno.IdObjeto.IsAssinado = venda.Termo_Adesao_Assinado;
                            throw new Exception(Resources.RsCadastros.TERMO_ADESAO_JA_ACEITO);
                        }

                        Configuracoes_Gerais config = await repoConfig.selectFirst();

                        if (config.Utiliza_Certificacao_Assinatura)
                        {
                            Consultar_Documento_ResponseModel retornoIntegracao = new Consultar_Documento_ResponseModel();

                            Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                            {
                                Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                                Token_Documento_Assinatura = venda.Token_Documento_Assinatura,
                                Id_Venda = venda.Id,
                                Id_Cliente = venda.Id_Cliente
                            };

                            switch (config.Tipo_Integracao_Assinatura)
                            {
                                case TipoIntegracaoAssinaturaEnum.Lexio:
                                    retornoIntegracao = new IntegracaoAssinaturaRepository().Consultar_Documento(new Lexio(), session, configCert);


                                    if (value.Objeto.Aceito == false && etapa.Status != StatusEtapaVenda.Aprovado && etapa.Status != StatusEtapaVenda.AprovadoManualmente)
                                    {
                                        etapa.Status = StatusEtapaVenda.Reprovado;
                                        await repositoryEtapa.SaveOrUpdate(etapa);
                                        throw new Exception(Resources.RsCadastros.TERMO_ADESAO_RECUSADO);
                                    }
                                    if (!retornoIntegracao.IsAssinado)
                                    {
                                        retorno.IdObjeto.IsAssinado = false;
                                        retorno.IdObjeto.IdVenda = venda.Id;
                                        throw new Exception(Resources.RsCadastros.TERMO_ADESAO_PENDENTE);
                                    }

                                    venda.Termo_Adesao_Assinado = retornoIntegracao.IsAssinado;
                                    venda.DataHoraAssinaturaTermo = retornoIntegracao.Data_Assinatura;
                                    venda.Url_Documento_Assinado = retornoIntegracao.Link_Download_Documento;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            venda.Termo_Adesao_Assinado = value.Objeto.Aceito;
                            venda.DataHoraAssinaturaTermo = DateTime.Now;
                        }

                        if (venda.Termo_Adesao_Assinado)
                        {

                            string ip = HttpContext.Current.Request.Headers["X-Forwarded-For"];

                            if (!string.IsNullOrEmpty(ip))
                            {
                                ip = ip.Split(',')[0].Trim();
                            }
                            else if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]))
                            {
                                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            }
                            else
                            {
                                ip = HttpContext.Current.Request.UserHostAddress;
                            }

                            string porta = HttpContext.Current.Request.ServerVariables["REMOTE_PORT"];

                            if (string.IsNullOrEmpty(porta))
                            {
                                porta = Convert.ToString(HttpContext.Current.Request.Url.Port);
                            }

                            venda.IpOrigemAceite = ip;
                            venda.UserAgentTermoAceite = HttpContext.Current.Request.UserAgent;
                            venda.CodigoHashTermoAceite = HttpContext.Current.Request.Url.Segments.Last().TrimEnd('/');
                            venda.UrlAceiteDigital = HttpContext.Current.Request.Url.AbsoluteUri;
                            venda.PortaOrigemAceite = porta;
                            venda.AdicionadoAceiteDigital = false;

                            var listPlanoProduto = await repoPlanoProduto.select(u => u.Cliente.Id == IdCliente &&
                                                                             u.Id_Venda == venda.Id);
                            foreach (var item in listPlanoProduto)
                            {
                                var clienteContrato = item.ClienteContrato;

                                if (clienteContrato != null)
                                {
                                    clienteContrato.IpOrigemAceite = ip;
                                    clienteContrato.UserAgentContratoAceite = HttpContext.Current.Request.UserAgent;
                                    clienteContrato.CodigoHashContratoAceite = HttpContext.Current.Request.Url.Segments.Last().TrimEnd('/');
                                    clienteContrato.UrlAceiteContrato = HttpContext.Current.Request.Url.AbsoluteUri;
                                    clienteContrato.DataAceiteContrato = venda.DataHoraAssinaturaTermo;
                                    clienteContrato.StatusAceiteContrato = venda.Termo_Adesao_Assinado ? Status_Aceite_Contrato.Aprovado : Status_Aceite_Contrato.Recusado;
                                    clienteContrato.AdicionadoAceiteDigital = false;
                                    clienteContrato.PortaOrigemAceite = porta;

                                    await repoClienteContrato.SaveOrUpdate(clienteContrato);
                                }
                            }
                        }

                        if (venda.Termo_Adesao_Assinado && etapa.Status != StatusEtapaVenda.Aprovado && etapa.Status != StatusEtapaVenda.AprovadoManualmente)
                        {
                            etapa.Status = StatusEtapaVenda.Aprovado;
                            await repositoryEtapa.SaveOrUpdate(etapa);
                            await api.AlterarStatusAnexos(new AlterarStatusAnexoRequest { Venda = venda.Id, Status = StatusAnexoContrato.Aprovado });
                        }

                        if (venda.Termo_Adesao_Assinado == false && etapa.Status != StatusEtapaVenda.Aprovado && etapa.Status != StatusEtapaVenda.AprovadoManualmente)
                        {
                            etapa.Status = StatusEtapaVenda.Reprovado;
                            await repositoryEtapa.SaveOrUpdate(etapa);
                            await api.AlterarStatusAnexos(new AlterarStatusAnexoRequest { Venda = venda.Id, Status = StatusAnexoContrato.Reprovado });
                            throw new Exception(Resources.RsCadastros.TERMO_ADESAO_RECUSADO);
                        }

                        string nome_cliente = cliente.NomeFantasia ?? cliente.Razao_Social;
                        var logs = etapa.GeraLog(etapaClone, Convert.ToString(cliente.Id), nome_cliente, Sinapse.ModelLog.TipoOperLog.Alteracao);
                        await repoEtapaVendaLog.SaveOrUpdate(logs);
                        await repoVenda.SaveOrUpdate(venda);

                        if (config.Tipo_Comprovante_Termo == Tipo_Comprovante_Termo.Selfie)
                        {
                            GravarAnexo(value.Objeto, venda, repositoryAnexo);
                        }

                        if (venda.Termo_Adesao_Assinado && venda.Workflow_VendasAtividade != null)
                        {
                            var primeiraAtividadeWorkflow =
                                venda.Workflow_Vendas.Atividades.Where(p => p.Ativ_Status == Status_Default.Ativo).FirstOrDefault();

                            if (primeiraAtividadeWorkflow != null && venda.Workflow_VendasAtividade.Id == primeiraAtividadeWorkflow.Id)
                            {
                                var proxAtividadeWorkflow =
                                    await repoAtividadeWorkflow.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Ativ_NAtivAprovada);

                                if (proxAtividadeWorkflow != null)
                                {
                                    using (var vendaAPI = new VendaApiController())
                                    {
                                        await vendaAPI.AlterarStatus(new VendaAlterarStatusViewModel
                                        {
                                            id = value.Objeto.IdVenda,
                                            justificativa = "aprovação de termo de adesão",
                                            situacao = 0
                                        });
                                    }

                                    await repoVenda.SaveOrUpdate(venda);
                                }
                            }
                        }

                        retorno.IdObjeto.IsAssinado = true;
                        retorno.IdObjeto.IdVenda = venda.Id;
                        retorno.Ok = true;
                    }
                    else
                    {
                        EtapaVenda etapa = await repositoryEtapa.selectFirst(p => p.TrocaTitularidade.Id == value.Objeto.IdVenda && p.Etapa == EnumEtapaVenda.TermoAdesao);
                        EtapaVenda etapaClone = (EtapaVenda)etapa.Clone();

                        if (troca.TermoAdesaoAssinado && !value.Objeto.Aceito)
                        {
                            retorno.IdObjeto.IsAssinado = venda.Termo_Adesao_Assinado;
                            throw new Exception(Resources.RsCadastros.TERMO_ADESAO_JA_ACEITO);
                        }

                        Configuracoes_Gerais config = await repoConfig.selectFirst();

                        troca.TermoAdesaoAssinado = value.Objeto.Aceito;
                        troca.DataAssinaturaTermoAdesao = DateTime.Now;

                        string ip = HttpContext.Current.Request.Headers["X-Forwarded-For"];

                        if (!string.IsNullOrEmpty(ip))
                        {
                            ip = ip.Split(',')[0].Trim();
                        }
                        else if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]))
                        {
                            ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        }
                        else
                        {
                            ip = HttpContext.Current.Request.UserHostAddress;
                        }

                        string porta = HttpContext.Current.Request.ServerVariables["REMOTE_PORT"];

                        if (string.IsNullOrEmpty(porta))
                        {
                            porta = Convert.ToString(HttpContext.Current.Request.Url.Port);
                        }

                        if (troca.TermoAdesaoAssinado)
                        {
                            troca.IpOrigemAceite = ip;
                            troca.UserAgentTermoAceite = HttpContext.Current.Request.UserAgent;
                            troca.CodigoHashTermoAceite = HttpContext.Current.Request.Url.Segments.Last().TrimEnd('/');
                            troca.UrlAceiteDigital = HttpContext.Current.Request.Url.AbsoluteUri;
                            troca.PortaOrigemAceite = porta;
                            troca.AdicionadoAceiteDigital = false;

                            var listPlanoProduto = await repoPlanoProduto.select(u => u.Cliente.Id == IdCliente &&
                                                                             u.Id_TrocaPlano == troca.Id);
                            foreach (var item in listPlanoProduto)
                            {
                                var clienteContrato = item.ClienteContrato;

                                if (clienteContrato != null)
                                {
                                    clienteContrato.IpOrigemAceite = ip;
                                    clienteContrato.UserAgentContratoAceite = HttpContext.Current.Request.UserAgent;
                                    clienteContrato.CodigoHashContratoAceite = HttpContext.Current.Request.Url.Segments.Last().TrimEnd('/');
                                    clienteContrato.UrlAceiteContrato = HttpContext.Current.Request.Url.AbsoluteUri;
                                    clienteContrato.PortaOrigemAceite = porta;
                                    clienteContrato.DataAceiteContrato = troca.DataAssinaturaTermoAdesao;
                                    clienteContrato.StatusAceiteContrato = troca.TermoAdesaoAssinado ? Status_Aceite_Contrato.Aprovado : Status_Aceite_Contrato.Recusado;
                                    clienteContrato.AdicionadoAceiteDigital = false;

                                    await repoClienteContrato.SaveOrUpdate(clienteContrato);
                                }
                            }
                        }


                        if (troca.TermoAdesaoAssinado && etapa.Status != StatusEtapaVenda.Aprovado && etapa.Status != StatusEtapaVenda.AprovadoManualmente)
                        {
                            etapa.Status = StatusEtapaVenda.Aprovado;
                            await repositoryEtapa.SaveOrUpdate(etapa);
                        }

                        if (troca.TermoAdesaoAssinado == false && etapa.Status != StatusEtapaVenda.Aprovado && etapa.Status != StatusEtapaVenda.AprovadoManualmente)
                        {
                            etapa.Status = StatusEtapaVenda.Reprovado;
                            await repositoryEtapa.SaveOrUpdate(etapa);
                            throw new Exception(Resources.RsCadastros.TERMO_ADESAO_RECUSADO);
                        }
                        await repoTrocaTitularidade.SaveOrUpdate(troca);

                        if (config.Tipo_Comprovante_Termo.Value == Tipo_Comprovante_Termo.Selfie)
                        {
                            GravarAnexo(value.Objeto, venda, repositoryAnexo, troca);
                        }

                        retorno.IdObjeto.IsAssinado = true;
                        retorno.IdObjeto.IdVenda = troca.Id;
                        retorno.Ok = true;
                    }

                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        public string GravarAnexo(TermoAdesaoFormularioModel model, Venda venda, Conexao<DocTermoAnexo> repository, TrocaTitularidade troca = null)
        {
            string retornoURL = null;

            if (model == null || (venda == null && troca == null) || string.IsNullOrEmpty(model.Base64))
            {
                return retornoURL;
            }

            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\";

            try
            {
                string base64 = model.Base64.Replace("data:image/jpeg;base64,", "");

                if (!Directory.Exists(_pasta))
                {
                    Directory.CreateDirectory(_pasta);
                }

                DocTermoAnexo anexo = new DocTermoAnexo();

                anexo.Nome = venda == null ? troca.Id.ToString() : venda.Id.ToString();
                anexo.Extensao = ".jpeg";
                anexo.TipoDocumento = Tipo_Doc_Termo_Anexo.SELFIE;
                anexo.Status = Status_Doc_Aceite.Aceite_Efetuado;
                anexo.Lado = Lado_Doc_Aceite.Unico;
                anexo.Venda = venda == null ? null : venda;
                anexo.TrocaTitularidade = troca == null ? null : troca;
                repository.Save(anexo).Wait();

                var bytes = Convert.FromBase64String(base64);

                string filePath = Path.Combine(_pasta, $"{anexo.Id}.jpeg");

                using (var imageFile = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/DocTermoAdesao/{anexo.Id}{anexo.Extensao}";
                anexo.Url = retornoURL;
                repository.SaveOrUpdate(anexo).Wait();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro: {ex.Message}");
            }

            return retornoURL;
        }


        private async Task<String> TermoAdesaoTrocaTitular(Cliente clienteAtual,
                                                           Cliente clienteNovo,
                                                           TrocaTitularidade trocaTitularidade)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var modeloRepo = new Conexao<TermoAdesao>(session))
            using (var modeloCliente_Anexo = new Conexao<Cliente_Anexo>(session))
            using (var ApiContrato = new Modelo_ContratoApiController())
            using (var clienteProdutoRepo = new Conexao<Cliente_PlanoProduto>(session))
            {
                var valores = new Dictionary<string, object>();
                var termo = await modeloRepo.selectFirst();
                var lsClientePlanoProduto = (await clienteProdutoRepo.select(x => x.ClienteContrato.Id == trocaTitularidade.NumeroContrato &&
                                                                               x.Cliente.Id == clienteAtual.Id)).ToList();

                var clientePlanoProduto = await clienteProdutoRepo.selectFirst(x => x.ClienteContrato.Id == trocaTitularidade.NumeroContrato &&
                                                                              x.Cliente.Id == clienteAtual.Id);

                /*
                 * COMENTADO CÓDIGO ABAIXO PARA EFETUAR TESTES LOCAL 
                 * SUBSTITUIR COM A VARIAVEL ABAIXO
                 * string caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Adesao\MODELO PADRAO TERMO DE ADESÃO.docx";
                 * **/
               
               
                string caminhoArquivo = termo.CaminhoArquivo;
                string caminhoHtml = $@"{CONSTANTES.CaminhoFisico}\Arquivos\TermoAdesao\termo_adesao_{trocaTitularidade.Id}.html";
                string caminhoPDF = $@"{CONSTANTES.CaminhoFisico}\Arquivos\TermoAdesao\termo_adesao_{trocaTitularidade.Id}.pdf";

                foreach (VariaveisModeloContrato item in Enum.GetValues(typeof(VariaveisModeloContrato)))
                {
                    KeyValuePair<string, object> variable = await ApiContrato.SetVariable(new Modelo_ContratoGenerateVariable
                    {
                        item = item,
                        clienteContrato = clientePlanoProduto?.ClienteContrato,
                        planos = lsClientePlanoProduto,
                        TrocaTitularidade = trocaTitularidade,
                        mensagens = new List<PostApiMensagem>(),
                        cpp = clientePlanoProduto
                    });

                    if (!String.IsNullOrEmpty(variable.Key) && variable.Value != null)
                        valores.Add(variable.Key, variable.Value);
                }

                valores.Remove("");

                ConvertToPdfService.modificarDocXContrato(new ConvertToPDFServiceRequestModel
                {
                    caminho = caminhoArquivo,
                    caminhooutput = $@"{CONSTANTES.CaminhoFisico}\Arquivos\TermoAdesao\termo_adesao_{trocaTitularidade.Id}.docx",

                    caminhohtml = caminhoHtml,
                    caminhopdf = caminhoPDF,
                    LogoVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_LOGO),
                    filename = Path.Combine(CONSTANTES.CaminhoFisico, @"Arquivos\LogoCliente.jpg"),//logo do cliente
                    lista_replace = valores.Select(p => new ReplaceDocXModel { Placeholder = p.Key, ReplaceValue = Convert.ToString(p.Value) }).ToList(),
                    FooterVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_FOOTER),
                    FilenameFooter = Path.Combine(CONSTANTES.CaminhoFisico, @"Arquivos\Footer.jpg"),
                    zoomFactorPdf = "1.3"
                });
                
                var objCliente_Anexo = new Cliente_Anexo
                {
                    Nome_Arquivo = $@"termo_adesao_{trocaTitularidade.Id}",
                    Extensao_Arquivo = "pdf",
                    Titulo = $@"termo_adesao_{trocaTitularidade.Id}",
                    Cliente = clienteNovo,
                    ClienteContrato = clientePlanoProduto?.ClienteContrato,
                    Foto_Perfil = false,
                    Contrato = false,
                    Venda = null,
                    Arquivo = File.ReadAllBytes(caminhoPDF),
                    UsuarioCriacao = User.Identity.GetUserId(),
                    DataHoraCriacao = DateTime.Now
                };

                await modeloCliente_Anexo.SaveOrUpdate(objCliente_Anexo);

                String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{clienteNovo.Id}";
                String _arquivo = $@"{_pasta}\{objCliente_Anexo.Titulo}.{objCliente_Anexo.Extensao_Arquivo}";
                if (!Directory.Exists(_pasta))
                {
                    Directory.CreateDirectory(_pasta);
                }

                File.WriteAllBytes(_arquivo, objCliente_Anexo.Arquivo);

                return caminhoPDF;
            }
        }
        private async Task<String> GeraTermoAdesaoCliente(Cliente cliente, Venda venda)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var modeloRepo = new Conexao<TermoAdesao>(session))
            using (var modeloCliente_Anexo = new Conexao<Cliente_Anexo>(session))
            using (var ApiContrato = new Modelo_ContratoApiController())
            using (var clienteProdutoRepo = new Conexao<Cliente_PlanoProduto>(session))
            {
                var valores = new Dictionary<string, object>();
                var termo = await modeloRepo.selectFirst();
                var clientePlanoProduto = await clienteProdutoRepo.selectFirst(x => x.Id_Venda == venda.Id && x.Cliente == cliente);

                /*
                 * COMENTADO CÓDIGO ABAIXO PARA EFETUAR TESTES LOCAL 
                 * SUBSTITUIR COM A VARIAVEL ABAIXO
                 * string caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Adesao\MODELO PADRAO TERMO DE ADESÃO.docx";
                 * **/
                string caminhoArquivo = termo.CaminhoArquivo;               
                string caminhoHtml = $@"{CONSTANTES.CaminhoFisico}\Arquivos\TermoAdesao\termo_adesao_{venda.Id}.html";
                string caminhoPDF = $@"{CONSTANTES.CaminhoFisico}\Arquivos\TermoAdesao\termo_adesao_{venda.Id}.pdf";

                KeyValuePair<string, object> variable;
                List<Cliente_PlanoProduto> listagemPlanos = cliente.PlanosProdutos.Where(p => p.Id_Venda == venda.Id).ToList();
                foreach (VariaveisModeloContrato item in Enum.GetValues(typeof(VariaveisModeloContrato)))
                {
                    variable = await ApiContrato.SetVariable(new Modelo_ContratoGenerateVariable
                    {
                        item = item,
                        clienteContrato = clientePlanoProduto?.ClienteContrato,
                        planos = listagemPlanos,
                        Venda = venda,
                        mensagens = new List<PostApiMensagem>(),
                        cpp = clientePlanoProduto
                    });

                    if (!String.IsNullOrEmpty(variable.Key) && variable.Value != null)
                        valores.Add(variable.Key, variable.Value);
                }

                var variaveisEnumerados = new[] { VariaveisModeloContrato.VAR_VALOR_FINAL_TOTAL, VariaveisModeloContrato.VAR_DESCONTO, VariaveisModeloContrato.VAR_VALOR_DESCONTO_TEMPORARIO };
                var contadorVariaveis = ListarVariaveis(caminhoArquivo);
                for (int index = 0; index < contadorVariaveis; index++)
                {
                    foreach (var variavelItem in variaveisEnumerados)
                    {
                        variable = await ApiContrato.SetVariable(new Modelo_ContratoGenerateVariable
                        {
                            item = variavelItem,
                            clienteContrato = clientePlanoProduto?.ClienteContrato,
                            planos = listagemPlanos,
                            Venda = venda,
                            mensagens = new List<PostApiMensagem>(),
                            cpp = clientePlanoProduto
                        }, index);

                        if (!string.IsNullOrEmpty(variable.Key) && variable.Value != null)
                        {
                            if (valores.ContainsKey(variable.Key))
                                valores.Remove(variable.Key);

                            valores.Add(variable.Key, variable.Value);
                        }
                           

                    }
                }

                valores.Remove("");

                ConvertToPdfService.modificarDocXContrato(new ConvertToPDFServiceRequestModel
                {
                    caminho = caminhoArquivo,
                    caminhooutput = $@"{CONSTANTES.CaminhoFisico}\Arquivos\TermoAdesao\termo_adesao_{venda.Id}.docx",

                    caminhohtml = caminhoHtml,
                    caminhopdf = caminhoPDF,
                    LogoVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_LOGO),
                    filename = Path.Combine(CONSTANTES.CaminhoFisico, @"Arquivos\LogoCliente.jpg"),//logo do cliente
                    lista_replace = valores.Select(p => new ReplaceDocXModel { Placeholder = p.Key, ReplaceValue = Convert.ToString(p.Value) }).ToList(),
                    FooterVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_FOOTER),
                    FilenameFooter = Path.Combine(CONSTANTES.CaminhoFisico, @"Arquivos\Footer.jpg"),
                    zoomFactorPdf = "1.3"
                });

                var objCliente_Anexo = await modeloCliente_Anexo.selectFirst(x => x.Venda.Id == venda.Id && x.ClienteContrato.Id == clientePlanoProduto.ClienteContrato.Id && x.Cliente.Id == cliente.Id);
                if (objCliente_Anexo == null)
                {
                    objCliente_Anexo = new Cliente_Anexo
                    {
                        Nome_Arquivo = $@"termo_adesao_{venda.Id}",
                        Extensao_Arquivo = "pdf",
                        Titulo = $@"termo_adesao_{venda.Id}",
                        Cliente = cliente,
                        ClienteContrato = clientePlanoProduto?.ClienteContrato,
                        Foto_Perfil = false,
                        Contrato = false,
                        Venda = venda,
                        Arquivo = File.ReadAllBytes(caminhoPDF),
                        UsuarioCriacao = User.Identity.GetUserId(),
                        DataHoraCriacao = DateTime.Now
                    };
                }
                String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{venda.Id_Cliente}";
                String _arquivo = $@"{_pasta}\{objCliente_Anexo.Titulo}.{objCliente_Anexo.Extensao_Arquivo}";
                if (!Directory.Exists(_pasta))
                {
                    Directory.CreateDirectory(_pasta);
                }

                await modeloCliente_Anexo.SaveOrUpdate(objCliente_Anexo);

                System.IO.File.WriteAllBytes(_arquivo, objCliente_Anexo.Arquivo);

                return caminhoPDF;
            }
        }

        public static int ListarVariaveis(string caminhoArquivo)
        {
            using (DocX documento = DocX.Load(caminhoArquivo))
            {
                string textoCompleto = documento.Text;

                // Regex para capturar os placeholders
                var regexValorFinal = new Regex(@"VAR_VALOR_FINAL_TOTAL(?:_\d+)?");
                var regexDesconto = new Regex(@"VAR_DESCONTO(?:_\d+)?");
                var regexValorDescontoTemporario = new Regex(@"VAR_VALOR_DESCONTO_TEMPORARIO(?:_\d+)?");

                // Converte MatchCollection em IEnumerable<Match> e extrai os valores
                var matchesValorFinal = regexValorFinal.Matches(textoCompleto).Cast<Match>().Select(m => m.Value).Distinct().ToList();
                var matchesDesconto = regexDesconto.Matches(textoCompleto).Cast<Match>().Select(m => m.Value).Distinct().ToList();
                var matchesValorDescontoTemporario = regexValorDescontoTemporario.Matches(textoCompleto).Cast<Match>().Select(m => m.Value).Distinct().ToList();

                // Calcula o maior número entre as três listas
                int qtd = new[] { matchesValorFinal.Count, matchesDesconto.Count, matchesValorDescontoTemporario.Count }.Max();

                return qtd;
            }
        }


        public String GetHtmlTermoAdesaoIntegracao(Cliente cliente, Configuracoes_Gerais config, Venda venda = null, ClienteContrato clienteContrato = null)
        {
            switch (config.Tipo_Integracao_Assinatura)
            {
                case TipoIntegracaoAssinaturaEnum.Lexio:
                    return clienteContrato != null ? $"{clienteContrato.Url_Assinar_Documento}" : $"{venda.Url_Assinar_Documento}";
                default:
                    return "";
            }
        }

        private void PreencherLinhaServico(Row linha, Cliente_PlanoProduto planoProduto)
        {
            if (planoProduto.Plano != null)
            {
                linha.ReplaceText("(PLANO)", planoProduto.Plano.Descricao);
            }
            else if (planoProduto.Produto != null)
            {
                linha.ReplaceText("(PLANO)", planoProduto.Plano.Descricao);
            }
            else if (planoProduto.Combo != null)
            {
                linha.ReplaceText("(PLANO)", planoProduto.Combo.Descricao);
            }

            linha.ReplaceText("(VALOR_MENSAL)", planoProduto.Valor_Final_Total != null ? planoProduto.Valor_Final_Total?.ToString("C", CultureInfo.CurrentCulture) : 0.ToString("C", CultureInfo.CurrentCulture));
            linha.ReplaceText("(VEL_NOM)", planoProduto.VelocidadeNominal ?? "");
            linha.ReplaceText("(VALOR_INST)", planoProduto.Valor_Instalacao_Final != null ? planoProduto.Valor_Instalacao_Final?.ToString("C", CultureInfo.CurrentCulture) : 0.ToString("C", CultureInfo.CurrentCulture));
            linha.ReplaceText("(PARCELAS)", Convert.ToString(planoProduto.Parcelas_Instalacao ?? 0));

            var mesesCarencia = planoProduto.Meses_Carencia ?? 0;
            linha.ReplaceText("(CARENCIA)", mesesCarencia != 1 ? Convert.ToString(mesesCarencia) + " meses" : Convert.ToString(mesesCarencia) + " mês");

            var testeGratis = planoProduto.Dias_Teste_Gratis ?? 0;
            linha.ReplaceText("(TESTE_GRATIS)", testeGratis != 1 ? Convert.ToString(testeGratis) + " dias" : Convert.ToString(testeGratis) + " dia");
        }

        private void PreencherLinhaPagamento(Row linha, Forma_Pagamento_Cliente pag, Cliente cliente)
        {
            linha.ReplaceText("(DT_VENC)", "dia " + pag.Dia_Vencimento ?? "");
            var tipoFormaPag = pag.Forma_Pagamento.Tipo_Forma_Pagamento;
            linha.ReplaceText("(BOL)", tipoFormaPag == Tipo_Forma_Pagamento.Boleto ? "(X)" : "");
            linha.ReplaceText("(DC)", tipoFormaPag == Tipo_Forma_Pagamento.Debito_Conta ? "(X)" : "");
            linha.ReplaceText("(CC)", tipoFormaPag == Tipo_Forma_Pagamento.Cartao_Credito ? "(X)" : "");
            linha.ReplaceText("(BANCO)", pag.Banco != null ? pag.Banco.Descricao : "");
            linha.ReplaceText("(AGEC)", pag.Agencia ?? "");
            linha.ReplaceText("(CONTA)", pag.Conta_Corrente ?? "");
            linha.ReplaceText("(FATURA_EMAIL)", pag.Forma_Envio == Cliente_FormaEnvio.Email ? "Sim" : "Não");

            Endereco_Cliente endereco;

            if (pag.End_Cobranca_Igual_Principal)
            {
                endereco = cliente.Endereco_Principal;
            }
            else
            {
                endereco = pag.Endereco_Cobranca;
            }

            linha.ReplaceText("(END_COB)", endereco.Logradouro ?? "");
            linha.ReplaceText("(COB_NUM)", endereco.Numero ?? "");
            linha.ReplaceText("(COB_BAIRRO)", endereco.Bairro ?? "");
            linha.ReplaceText("(COB_CEP)", endereco.Cep ?? "");
            linha.ReplaceText("(COB_CID)", endereco.Cidade ?? "");
            linha.ReplaceText("(CO_UF)", endereco.UF ?? "");

        }

        //private void PreencherLogoEmpresa(DocX document, String urlLogo, byte[] logoBd, String tag)
        //{
        //    Int32 linhaFoto = GetLinhaTabela(document, tag);
        //    Int32 colunaFoto = GetCellByTag(document, linhaFoto, tag);

        //    document.ReplaceText(tag, String.Empty);

        //    WebClient webClient = new WebClient();
        //    byte[] data;

        //    if (logoBd == null)
        //    {
        //        data = webClient.DownloadData(urlLogo);
        //    }
        //    else
        //    {
        //        data = logoBd;
        //    }

        //    if (document.Tables[0].RowCount > linhaFoto)
        //    {
        //        var stream = new MemoryStream();

        //        System.Drawing.Image imagem = ImageService.BytesToImage(data);
        //        imagem.Save(stream, ImageFormat.Png);
        //        stream.Position = 0;

        //        Image image = document.AddImage(stream);

        //        Picture picture = image.CreatePicture();
        //        picture.Height = 70;
        //        picture.Width = 70;

        //        document.Tables[0].Rows[linhaFoto].Cells[colunaFoto].Paragraphs[0].AppendPicture(picture);
        //    }
        //}

        private static Int32 GetLinhaTabela(DocX document, String Tag)
        {
            Int32 linha = 0;
            foreach (Xceed.Words.NET.Row row in document.Tables[0].Rows)
            {
                linha++;
                if (row.Xml.Value.Contains(Tag))
                {
                    linha--;
                    break;
                }
            }
            return linha;
        }

        private Int32 GetCellByTag(DocX document, Int32 row, String Tag)
        {
            Int32 intCell = 0;
            foreach (Xceed.Words.NET.Cell cell in document.Tables[0].Rows[row].Cells)
            {
                intCell++;
                if (cell.Xml.Value.Contains(Tag))
                {
                    intCell--;
                    break;
                }
            }
            return intCell;
        }

        public InfoEmpresaTermoAdesao GetInfoEmpresa(String codEmpresa)
        {
            InfoEmpresaTermoAdesao retorno = null;

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT RAZAO_SOCIAL, EMPRESA, CNPJ, ENDERECO, BAIRRO, CIDADE, UF, LOGOTIPO ");
            sql.AppendLine("FROM TB_EMPRESA ");
            sql.AppendLine("WHERE CODEMPRESA = :pCOD_EMPRESA");

            using ISession session = HibernateUtil.GetSessionT5();
            using Query query = new Query(session);
            {
                query.setParametro("pCOD_EMPRESA", codEmpresa);
                var values = query.Values(sql.ToString()).Result;

                if (values.Count > 0) retorno = new InfoEmpresaTermoAdesao
                {
                    Nome = Convert.ToString(values[0]["EMPRESA"]),
                    RazaoSocial = Convert.ToString(values[0]["RAZAO_SOCIAL"]),
                    Cnpj = Convert.ToString(values[0]["CNPJ"]),
                    Endereco = Convert.ToString(values[0]["ENDERECO"]),
                    Bairro = Convert.ToString(values[0]["BAIRRO"]),
                    Cidade = Convert.ToString(values[0]["CIDADE"]),
                    Estado = Convert.ToString(values[0]["UF"]),
                    Logo = (byte[])values[0]["LOGOTIPO"]

                };
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/TermoAdesaoApi/AlterarStatusTermo")]
        public async Task<PostApiRetorno<Int64>> AlterarStatusTermo(Int64 status, Int64 id)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoEtapaVendaLog = new Conexao<LogEtapaVendas>(sessionLog))
                using (var repo = new Conexao<Venda>(session))
                using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                using (var repoEtapaVenda = new Conexao<EtapaVenda>(session))
                using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    var venda = await repo.selectFirst(p => p.Id == id);
                    EtapaVenda etapa = await repoEtapaVenda.selectFirst(p => p.Venda.Id == id && p.Etapa == EnumEtapaVenda.TermoAdesao);
                    EtapaVenda etapaClone = (EtapaVenda)etapa.Clone();

                    var lista_cpp = await repoCliente_PlanoProduto.select(p => p.Id_Venda == id);
                    var config = await repoConfiguracoes_Gerais.selectFirst();

                    if (etapa == null)
                        throw new Exception("Etapa não encontrada");

                    if (venda.Status == Status_Venda.Venda_Cancelada)
                        throw new Exception("Venda já cancelada");

                    if (venda.Status == Status_Venda.Venda_Aprovada)
                        throw new Exception("Venda já aprovada");

                    if (status == 3)
                    {
                        venda.Termo_Adesao_Assinado = false;
                        etapa.Status = StatusEtapaVenda.Pendente;

                        if (config.Exige_Termo_Contrato)
                        {
                            foreach (var item in lista_cpp)
                            {
                                var clienteContrato = item.ClienteContrato;

                                if (clienteContrato != null)
                                {
                                    clienteContrato.StatusAceiteContrato = Status_Aceite_Contrato.Pendente;
                                    clienteContrato.AdicionadoAceiteDigital = false;
                                    clienteContrato.Arquivo = null;
                                    await repoClienteContrato.SaveOrUpdate(clienteContrato);
                                }
                            }
                        }
                    }
                    else if (status == 2)
                    {
                        venda.Termo_Adesao_Assinado = true;
                        if (venda.DataHoraAssinaturaTermo == null)
                            venda.DataHoraAssinaturaTermo = DateTime.Now;

                        etapa.Status = StatusEtapaVenda.AprovadoManualmente;

                        if (config.Exige_Termo_Contrato)
                        {
                            foreach (var item in lista_cpp)
                            {
                                var clienteContrato = item.ClienteContrato;

                                if (clienteContrato != null)
                                {
                                    clienteContrato.DataAceiteContrato = DateTime.Now;
                                    await repoClienteContrato.SaveOrUpdate(clienteContrato);
                                }
                            }
                        }
                    }

                    var logs = etapa.GeraLog(etapaClone, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);

                    await repo.Update(venda);
                    await repoEtapaVenda.Update(etapa);
                    await repoEtapaVendaLog.Save(logs);
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
        [Route("api/TermoAdesaoApi/AlterarStatusDocTermo")]
        public async Task<PostApiRetorno<Int64>> AlterarStatusDocTermoTroca(Int64 status, Int64 idTrocaTitular, Int64 idDocTermo)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<TrocaTitularidade>(session))
                using (var repoDocTermo = new Conexao<DocTermoAnexo>(session))
                {
                    var troca = await repo.selectFirst(p => p.Id == idTrocaTitular);

                    if (troca == null)
                        throw new Exception("Troca de Titularidade não encontrada");

                    var docTermo = await repoDocTermo.selectFirst(x => x.Id == idDocTermo && x.TrocaTitularidade.Id == idTrocaTitular);

                    if (docTermo == null)
                        throw new Exception("Troca de Titularidade não encontrada");

                    if (status == 2)
                    {
                        docTermo.Status = Status_Doc_Aceite.Aprovado;
                    }
                    else if (status == 3)
                    {
                        docTermo.Status = Status_Doc_Aceite.Reprovado;
                    }
                    else
                    {
                        docTermo.Status = Status_Doc_Aceite.Pendente_Aceite;
                    }

                    await repoDocTermo.Update(docTermo);
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



        private String FormatarCpfCnpj(String CpfCnpj)
        {
            if (CpfCnpj.Length == 11)
            {
                return (CpfCnpj != "" && CpfCnpj != null) ? Convert.ToUInt64(CpfCnpj).ToString(@"000\.000\.000\-00") : "";
            }
            else
            {
                return (CpfCnpj != "" && CpfCnpj != null) ? Convert.ToUInt64(CpfCnpj).ToString(@"00\.000\.000\/0000\-00") : "";
            }
        }

        [HttpGet()]
        [Route("api/TermoAdesaoApi/GetPendenciaTermo")]
        public async Task<GetApiRetorno<ConsultaTermoAdesaoModel>> GetPendenciaTermo(long Id)
        {
            var retorno = new GetApiRetorno<ConsultaTermoAdesaoModel>();
            retorno.Objeto = new ConsultaTermoAdesaoModel();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repoTermo = new Conexao<TermoAdesao>(session))
                {
                    IConfiguracoesGeraisRepository repository = new ConfiguracoesGeraisRepository(session);
                    IVendaRepository repoVenda = new VendaRepository(session);

                    var businessVenda = new VendaBusiness(repoVenda);
                    var business = new ConfiguracoesGeraisBusiness(repository);


                    var config = business.GetConfiguracoesGerais();
                    var venda = businessVenda.GetVendaById(Id);

                    if (config.Utiliza_Certificacao_Assinatura)
                    {
                        Consultar_Documento_ResponseModel retornoIntegracao = new Consultar_Documento_ResponseModel();

                        Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                        {
                            Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                            Token_Documento_Assinatura = venda.Token_Documento_Assinatura,
                            Id_Venda = venda.Id,
                            Id_Cliente = venda.Id_Cliente
                        };

                        switch (config.Tipo_Integracao_Assinatura)
                        {
                            case TipoIntegracaoAssinaturaEnum.Lexio:
                                retornoIntegracao = new IntegracaoAssinaturaRepository().Consultar_Documento(new Lexio(), session, configCert);


                                if (!retornoIntegracao.IsAssinado)
                                {
                                    retorno.Objeto.IntegracaoPendente = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        retorno.Objeto.IntegracaoPendente = false;
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                //retorno.IdObjeto.IdVenda = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }
            return retorno;
        }

        #region MOSTQI
        [HttpPost()]
        [Route("api/Termo_Adesao/EnviarVideoLivenessDetect")]
        public async Task<Response> EnviarVideoLivenessDetect([FromBody] TermoAdesaoFormularioModel form)
        {

            Response resposta = new Response();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoIntegracoesExt = new Conexao<IntegracoesExternas>(session))
            {
                if (String.IsNullOrEmpty(form.Base64))
                {
                    throw new Exception("Exceção por vazio");
                }
                else
                {
                    string retornoURL = null;
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\_tempLivenessDetect";

                    string base64 = form.Base64.Replace("data:video/webm;base64,", "");
                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var bytes = Convert.FromBase64String(base64);
                    using (var imageFile = new FileStream(Path.Combine(_pasta, $@"{form.IdVenda}.webm"), FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/DocTermoAdesao/_tempLivenessDetect/{form.IdVenda}.webm";

                    DetectRequest detect = new DetectRequest
                    {
                        fileUrl = retornoURL,
                        fileBase64 = null,
                        movements = new List<object>(),
                        returnFullFace = "true"
                    };

                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas externas =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MOSTQI);


                    var config = await repoConfig.selectFirst();
                    var retornoIntegracao = await new IntegracaoProvaVidaRepository().Detect(new MostqLivenessDetect(externas.TokenAcesso), detect);

                    if (retornoIntegracao.status.code == "200")
                    {
                        if (!String.IsNullOrEmpty(retornoIntegracao.result.frontalImage))
                        {

                            if ((int)(Convert.ToInt32(retornoIntegracao.result.imageScore) * 100) >= config.Porcentagem_Video)
                            {
                                String _pastaDocumento = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\LivenessDetect";

                                if (!Directory.Exists(_pastaDocumento))
                                {
                                    Directory.CreateDirectory(_pastaDocumento);
                                }

                                string fileName = form.IdVenda + ".txt";
                                string filePath = Path.Combine(_pastaDocumento, fileName);
                                System.IO.File.WriteAllText(filePath, retornoIntegracao.result.frontalImage, Encoding.UTF8);

                                //Exclui o arquivo temp
                                string arquivoExcluir = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\_tempLivenessDetect\{form.IdVenda}.webm";
                                File.Delete(arquivoExcluir);


                                resposta.result = true;
                            }
                            else
                            {
                                resposta.result = false;
                                resposta.message = "Não foi possível detectar os movimentos e/ou um rosto no vídeo. Por favor, refaça a gravação do vídeo seguindo as instruções.";
                            }
                        }
                        else
                        {
                            resposta.result = false;
                            resposta.message = "Ocorreu um erro ao processar a imagem, por favor, tente novamente";
                        }
                    }
                    else
                    {
                        resposta.result = false;
                        resposta.message = $@"Ocorreu um erro ao processar o vídeo. Pedimos desculpas pela inconveniência. Por favor, entre em contato com o administrador do sistema e forneça o seguinte código de referência para que possamos ajudá-lo a resolver o problema: {retornoIntegracao.status.code}";
                    }
                }
            }

            return resposta;
        }


        [HttpPost()]
        [Route("api/Termo_Adesao/EnviarDocumentacaoMostQIExtractVio")]
        public async Task<Response> EnviarDocumentacaoMostQIExtractVio([FromBody] TermoAdesaoFormularioModel form)
        {
            Response resposta = new Response();


            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoIntegracoesExt = new Conexao<IntegracoesExternas>(session))
            {
                if (!String.IsNullOrEmpty(form.InputBase64Vio) || !String.IsNullOrEmpty(form.Base64))
                {

                    string retornoURL = null;
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\_tempVio";

                    string base64 = null;


                    //Recebeu um anexo
                    if (!String.IsNullOrEmpty(form.InputBase64Vio))
                    {

                        base64 = form.InputBase64Vio.Replace($"data:{form.InputBase64Type};base64,", "");
                    }

                    //Recebeu uma foto
                    else
                    {
                        base64 = form.Base64.Replace($"data:{form.InputBase64Type};base64,", "");
                    }

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var bytes = Convert.FromBase64String(base64);

                    var ext = form.InputBase64Type == "application/pdf" ? "pdf" : "png";

                    using (var imageFile = new FileStream(Path.Combine(_pasta, $@"{form.IdVenda}.{ext}"), FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/DocTermoAdesao/_tempVio/{form.IdVenda}.{ext}";

                    VioExtractionRequest request = new VioExtractionRequest
                    {
                        fileUrl = retornoURL,
                        fileBase64 = base64,
                        qrCodeField = null
                    };

                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas externas =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MOSTQI);

                    var config = await repoIntegracoesExt.select();
                    var retornoIntegracao = await new IntegracaoValidacaoDocumentoRepository().Extract(new MostqVioExtraction(externas.TokenAcesso), request);


                    if (retornoIntegracao.status.code == "200")
                    {
                        if (retornoIntegracao.result.Count == 0)
                        {
                            resposta.result = false;
                            resposta.message = "Não foi encontrado um documento válido na foto, por favor, tire a foto novamente e certifique-se de que ela esteja visível e enquadrada.";
                        }
                        else
                        {
                            String _pastaDocumento = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\Documentacao";

                            if (!Directory.Exists(_pastaDocumento))
                            {
                                Directory.CreateDirectory(_pastaDocumento);
                            }

                            string fileName = form.IdVenda + ".txt";
                            string filePath = Path.Combine(_pastaDocumento, fileName);
                            System.IO.File.WriteAllText(filePath, base64, Encoding.UTF8);

                            resposta = await EnviarFaceMatch(form);

                        }
                    }
                    else
                    {
                        resposta.result = false;
                        resposta.message = $@"O processamento da sua solicitação encontrou um erro inesperado. Pedimos desculpas pela inconveniência. Por favor, entre em contato com o administrador do sistema e forneça o seguinte código de referência para que possamos ajudá-lo a resolver o problema: {retornoIntegracao.status.code}";
                    }

                }
                else
                {
                    resposta.result = false;
                    resposta.message = "Arquivo de imagem não encontrado, por favor, tente novamente.";
                }
            }

            return resposta;
        }


        [HttpPost()]
        [Route("api/Termo_Adesao/EnviarDocumentacaoMostQIExtractContent")]
        public async Task<Response> EnviarDocumentacaoMostQIExtractContent([FromBody] TermoAdesaoFormularioModel form)
        {
            Response resposta = new Response();


            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoIntegracoesExt = new Conexao<IntegracoesExternas>(session))
            {
                if (!String.IsNullOrEmpty(form.InputBase64Content) || !String.IsNullOrEmpty(form.Base64))
                {

                    string retornoURL = null;
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\_tempContent";

                    string base64 = null;


                    //Recebeu um anexo
                    if (!String.IsNullOrEmpty(form.InputBase64Content))
                    {

                        base64 = form.InputBase64Content.Replace($"data:{form.InputBase64Type};base64,", "");
                    }

                    //Recebeu uma foto
                    else
                    {
                        base64 = form.Base64.Replace($"data:{form.InputBase64Type};base64,", "");
                    }


                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    var bytes = Convert.FromBase64String(base64);

                    var ext = form.InputBase64Type == "application/pdf" ? "pdf" : "png";

                    using (var imageFile = new FileStream(Path.Combine(_pasta, $@"{form.IdVenda}.{ext}"), FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }

                    retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/DocTermoAdesao/_tempContent/{form.IdVenda}.{ext}";

                    ContentExtractionRequest request = new ContentExtractionRequest
                    {
                        fileUrl = retornoURL,
                        fileBase64 = null,
                        qrCodeField = null
                    };

                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas externas =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MOSTQI);

                    var config = await repoIntegracoesExt.select();
                    var retornoIntegracao = await new IntegracaoValidacaoDocumentoRepository().Extract(new MostqContentExtraction(externas.TokenAcesso), request);


                    if (retornoIntegracao.status.code == "200")
                    {
                        if (retornoIntegracao.result.Count == 0)
                        {
                            resposta.result = false;
                            resposta.message = "Não foi encontrado um documento válido na foto, por favor, tire a foto novamente e certifique-se de que ela esteja visível e enquadrada.";
                        }
                        else
                        {
                            String _pastaDocumento = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\Documentacao";

                            if (!Directory.Exists(_pastaDocumento))
                            {
                                Directory.CreateDirectory(_pastaDocumento);
                            }

                            string fileName = form.IdVenda + ".txt";
                            string filePath = Path.Combine(_pastaDocumento, fileName);
                            System.IO.File.WriteAllText(filePath, base64, Encoding.UTF8);

                            resposta = await EnviarFaceMatch(form);

                        }
                    }
                    else
                    {
                        resposta.result = false;
                        resposta.message = $@"O processamento da sua solicitação encontrou um erro inesperado. Pedimos desculpas pela inconveniência. Por favor, entre em contato com o administrador do sistema e forneça o seguinte código de referência para que possamos ajudá-lo a resolver o problema: {retornoIntegracao.status.code}";
                    }
                }
                else
                {
                    resposta.result = false;
                    resposta.message = "Arquivo de imagem não encontrado, por favor, tente novamente.";
                }
            }

            return resposta;
        }


        [HttpPost()]
        [Route("api/Termo_Adesao/EnviarFaceMatch")]
        public async Task<Response> EnviarFaceMatch(TermoAdesaoFormularioModel form)
        {

            Response resposta = new Response();


            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoIntegracoesExt = new Conexao<IntegracoesExternas>(session))
            {

                //LivenessDetect
                String caminhoLiveness = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\LivenessDetect\{form.IdVenda}.txt";
                string base64Liveness = File.ReadAllText(caminhoLiveness, Encoding.UTF8);

                //Doc
                String caminhoDoc = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\Documentacao\{form.IdVenda}.txt";
                string base64Doc = File.ReadAllText(caminhoDoc, Encoding.UTF8);

                var request = new FaceCompareRequest
                {
                    faceFileBase64A = base64Liveness,
                    faceFileBase64B = base64Doc
                };

                IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                IntegracoesExternas externas =
                integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MOSTQI);


                var config = await repoConfig.selectFirst();
                var retornoIntegracao = await new IntegracaoReconhecimentoFacialRepository().FaceCompare(new MostqFaceMatch(externas.TokenAcesso), request);

                if (retornoIntegracao.status.code == "200")
                {
                    if ((int)(Convert.ToInt32(retornoIntegracao.result.similarity) * 100) >= config.Porcentagem_Similaridade)
                    {
                        resposta.result = true;
                    }
                    else
                    {
                        resposta.result = false;
                        resposta.message = "A similaridade detectada não é suficiente. Por favor, refaça a gravação do vídeo.";
                    }
                }
                else
                {
                    resposta.result = false;
                    resposta.message = "Ocorreu um erro ao enviar imagens, por favor, tente novamente.";
                }
            }

            return resposta;
        }

        [HttpPost()]
        [Route("api/Termo_Adesao/ExcluirArquivosTemp")]
        public void ExcluirArquivosTemp(ExcluirTempTermo value)
        {
            //LivenessDetect
            String caminhoLiveness = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\LivenessDetect\{value.idVenda}.txt";
            File.Delete(caminhoLiveness);

            //Doc
            String caminhoDoc = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\Documentacao\{value.idVenda}.txt";
            File.Delete(caminhoDoc);
        }

        #endregion
    }


}
