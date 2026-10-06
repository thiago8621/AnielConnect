
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.IntegracaoExterna;
using Microsoft.AspNet.Identity;
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
using Image = Xceed.Words.NET.Image;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{

    public class Termo_AdesaoApiController : BaseApiController
    {


        [HttpGet()]
        [Route("api/TermoAdesaoApi/FindById")]
        public async Task<GetApiRetorno<TermoAdesaoFormularioModel>> FindById(Int64? IdVenda, String CaminhoFisico, String UrlPortal)
        {
            GetApiRetorno<TermoAdesaoFormularioModel> retorno = new GetApiRetorno<TermoAdesaoFormularioModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            //   using (var repoConfigCert = new Conexao<Config_Certificacao_Assinatura_RequestModel>(session))
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

                    //  Config_Certificacao_Assinatura_RequestModel configCert = await repoConfigCert.selectFirst();

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

                    var camihoPdf = await GeraTermoAdesaoCliente(cliente, venda, CaminhoFisico);

                    string urlTermoIntegracao = String.Empty;
                    if (config.Utiliza_Certificacao_Assinatura)
                    {
                        if (String.IsNullOrEmpty(venda.Url_Assinar_Documento))
                        {
                            var retornoIntegracao = new Enviar_Documento_ResponseModel();
                            switch (config.Tipo_Integracao_Assinatura)
                            {
                                case Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum.Lexio:

                                    Byte[] bytes = File.ReadAllBytes(camihoPdf);
                                    String file = Convert.ToBase64String(bytes);

                                    retornoIntegracao = new IntegracaoAssinaturaRepository().Enviar_Documento(new Lexio(), session, configCert, file);

                                    venda.Tipo_Integracao_Assinatura = Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum.Lexio;
                                    break;
                                default:
                                    break;

                            }

                            venda.Url_Assinar_Documento = retornoIntegracao.Link_Assinatura;
                            venda.Token_Documento_Assinatura = retornoIntegracao.Id_Documento_Integracao;

                            await repoVenda.SaveOrUpdate(venda);
                        }

                        urlTermoIntegracao = GetHtmlTermoAdesaoIntegracao(cliente, venda, config);
                    }

                    string urlPdfTermoAdesao = $@"{UrlPortal}/Arquivos/TermoAdesao/termo_adesao_{venda.Id}.pdf";

                    var termo = new TermoAdesaoFormularioModel
                    {
                        IdVenda = venda.Id,
                        Cliente = cliente.NomeFantasia,
                        Empresa = config.Nome_Cliente_Sinapse,
                        UrlLogo = config.OLA_URL_Logo,
                        Termo = !String.IsNullOrEmpty(urlTermoIntegracao) ? urlTermoIntegracao : urlPdfTermoAdesao,
                        Aceito = false,
                        isIntegracao = config.Utiliza_Certificacao_Assinatura,
                        ExigeFotoTermo = config.Exige_Foto_Termo,
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
                    CONSTANTES.log4Net.Error(ex);
                }
            }

            return retorno;
        }



        [HttpPost()]
        [Route("api/TermoAdesaoApi/gerarTermoAdesaoIntegracao")]
        public async Task<GetApiRetorno<String>> GerarTermoAdesaoIntegracao(TermoAdesaoIdVenda request)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<string>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                try
                {

                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    var configEmpresa = await repoConfig.selectFirst();
                    var venda = await repoVenda.selectFirst(p => p.Id == request.IdVenda);

                    if (configEmpresa.Utiliza_Certificacao_Assinatura)
                    {
                        if (String.IsNullOrEmpty(venda.Url_Assinar_Documento))
                        {
                            var retornoApi = await FindById(request.IdVenda, request.CaminhoFisico, request.UrlPortal);
                            if (retornoApi.Ok)
                            {

                                switch (configEmpresa.Tipo_Integracao_Assinatura)
                                {
                                    case Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum.Lexio:
                                        retorno.Objeto = (await repoVenda.selectFirst(p => p.Id == request.IdVenda)).Url_Assinar_Documento;
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
                    CONSTANTES.log4Net.Error(ex);
                }
            }
            return retorno;
        }


        private String GetHtmlTermoAdesaoIntegracao(Cliente cliente, Venda venda, Configuracoes_Gerais config)
        {


            switch (config.Tipo_Integracao_Assinatura)
            {
                case Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum.Lexio:
                    return $"{venda.Url_Assinar_Documento}";
                default:
                    return "";
            }
        }


        private async Task<String> GeraTermoAdesaoCliente(Cliente cliente, Venda venda, String caminhoFisico)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var modeloRepo = new Conexao<TermoAdesao>(session))
            using (var modeloCliente_Anexo = new Conexao<Cliente_Anexo>(session))
            using (var clienteProdutoRepo = new Conexao<Cliente_PlanoProduto>(session))
            {
                var valores = new Dictionary<string, object>();
                var termo = await modeloRepo.selectFirst();
                var clientePlanoProduto = await clienteProdutoRepo.selectFirst(x => x.Id_Venda == venda.Id && x.Cliente == cliente);


                string caminhoArquivo = termo.CaminhoArquivo;
                string caminhoHtml = $@"{caminhoFisico}\Arquivos\TermoAdesao\termo_adesao_{venda.Id}.html";
                string caminhoPDF = $@"{caminhoFisico}\Arquivos\TermoAdesao\termo_adesao_{venda.Id}.pdf";

                KeyValuePair<string, object> variable;
                List<Cliente_PlanoProduto> listagemPlanos = cliente.PlanosProdutos.Where(p => p.Id_Venda == venda.Id).ToList();
                foreach (VariaveisModeloContrato item in Enum.GetValues(typeof(VariaveisModeloContrato)))
                {
                    variable = await SetVariable(new Modelo_ContratoGenerateVariable
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
                        variable = await SetVariable(new Modelo_ContratoGenerateVariable
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
                    caminhooutput = $@"{caminhoFisico}\Arquivos\TermoAdesao\termo_adesao_{venda.Id}.docx",

                    caminhohtml = caminhoHtml,
                    caminhopdf = caminhoPDF,
                    LogoVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_LOGO),
                    filename = Path.Combine(caminhoFisico, @"Arquivos\LogoCliente.jpg"),//logo do cliente
                    lista_replace = valores.Select(p => new ReplaceDocXModel { Placeholder = p.Key, ReplaceValue = Convert.ToString(p.Value) }).ToList(),
                    FooterVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_FOOTER),
                    FilenameFooter = Path.Combine(caminhoFisico, @"Arquivos\Footer.jpg"),
                    zoomFactorPdf = "1.3"
                });

                var objCliente_Anexo = await modeloCliente_Anexo.selectFirst(x => x.Venda.Id == venda.Id && x.ClienteContrato.Id == clientePlanoProduto.ClienteContrato.Id && x.Cliente.Id == cliente.Id);
                if (objCliente_Anexo == null)
                {
                    objCliente_Anexo = new Cliente_Anexo
                    {
                        Nome_Arquivo = $@"termo_adesao_{venda.Id}",
                        Extensao_Arquivo = ".pdf",
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
                await modeloCliente_Anexo.SaveOrUpdate(objCliente_Anexo);

                return caminhoPDF;
            }
        }

        public static int ListarVariaveis(string caminhoArquivo)
        {
            using (DocX documento = DocX.Load(caminhoArquivo))
            {
                string textoCompleto = documento.Text;

                // Regex para capturar os placeholders
                var regexValorFinal = new Regex(@"VAR_VALOR_FINAL_TOTAL_\w+");
                var regexDesconto = new Regex(@"VAR_DESCONTO_\w+");
                var regexValorDescontoTemporario = new Regex(@"VAR_VALOR_DESCONTO_TEMPORARIO_\w+");

                // Converte MatchCollection em IEnumerable<Match> e extrai os valores
                var matchesValorFinal = regexValorFinal.Matches(textoCompleto).Cast<Match>().Select(m => m.Value).Distinct().ToList();
                var matchesDesconto = regexDesconto.Matches(textoCompleto).Cast<Match>().Select(m => m.Value).Distinct().ToList();
                var matchesValorDescontoTemporario = regexValorDescontoTemporario.Matches(textoCompleto).Cast<Match>().Select(m => m.Value).Distinct().ToList();

                // Calcula o maior número entre as três listas
                int qtd = new[] { matchesValorFinal.Count, matchesDesconto.Count, matchesValorDescontoTemporario.Count }.Max();

                return qtd;
            }
        }

        [HttpPost()]
        [Route("api/TermoAdesaoApi/getAssinaturaTermoAdesao")]
        public async Task<GetApiRetorno<List<AssinaturaTermoAdesaoDados>>> GetAssinaturaTermoAdesao(TermoAdesaoIdVenda request)
        {
            GetApiRetorno<List<AssinaturaTermoAdesaoDados>> retorno = new GetApiRetorno<List<AssinaturaTermoAdesaoDados>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            {
                try
                {

                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }


                    Venda venda = null;
                    if (request.IdVenda.HasValue)
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == request.IdVenda.Value);
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                    }

                    var dados = new List<AssinaturaTermoAdesaoDados>();
                    var assinatura = new AssinaturaTermoAdesaoDados();

                    assinatura.IdVenda = venda.Id;
                    assinatura.Assinatura = GetAssinatura(venda, false, request.CaminhoFisico, request.UrlPortal);
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
                    CONSTANTES.log4Net.Error(ex);
                }
            }
            return retorno;
        }


        private string GetAssinatura(Venda venda, bool isCaminho, string CaminhoFisico, string UrlPortal)
        {
            string retorno;

            string caminhoProjetoWebApi = UrlPortal;
            string caminhoProjeto = CaminhoFisico;

            if (String.IsNullOrEmpty(caminhoProjetoWebApi))
            {
                throw new Exception("Url Portal não definida no empresa.config");
            }

            if (String.IsNullOrEmpty(caminhoProjeto))
            {
                throw new Exception("Caminho Fisico não definido no empresa.config");
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
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex);
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


        /*[HttpPost()]
         [Route("api/TermoAdesaoApi/getPdfTermoAdesao")]
         public async Task<GetApiRetorno<String>> GetPdfTermoAdesao(TermoAdesaoIdVenda request)
         {
             GetApiRetorno<String> retorno = new GetApiRetorno<String>();
             using (ISession session = HibernateUtil.GetSession())
             using (var repoVenda = new Conexao<Venda>(session))
             using (var repoCliente = new Conexao<Cliente>(session))
             using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
             using (var repoConfigCert = new Conexao<Config_Certificacao_Assinatura_RequestModel>(session))
             {
                 try
                 {
                     Venda venda = null;
                     Cliente cliente = null;

                     string caminhoProjetoWebApi = request.UrlPortal;
                     string caminhoProjeto = request.CaminhoFisico;

                     using (var api = new UsuarioApiController())
                     {
                         if (!(await api.GetUsuarioValido(request.settings)).Ok)
                         {
                             throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                         }
                     }


                     if (String.IsNullOrEmpty(caminhoProjetoWebApi))
                     {
                         throw new Exception("Url do Connect não definida na TB_CONFIG_URL.");
                     }

                     if (String.IsNullOrEmpty(caminhoProjeto))
                     {
                         throw new Exception("CaminhoProjetoConnect não definido no empresa.config.");
                     }

                     if (request.IdVenda.HasValue)
                     {
                         venda = await repoVenda.selectFirst(p => p.Id == request.IdVenda.Value);
                         cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);
                     }
                     else
                     {
                         throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                     }

                     Configuracoes_Gerais config = await repoConfig.selectFirst();

                     Config_Certificacao_Assinatura_RequestModel config_Certi = await repoConfigCert.selectFirst();

                     if (venda.Termo_Adesao_Assinado)
                     {
                         if (config.Utiliza_Certificacao_Assinatura)
                         {
                             switch (config.Tipo_Integracao_Assinatura)
                             {
                                 case Model.enumerado.TipoIntegracaoAssinaturaEnum.Lexio:
                                     retorno.Objeto = new IntegracaoAssinaturaRepository().Get_Documento_Pdf(new Lexio(), config_Certi, venda.Url_Documento_Assinado);
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

                             retorno.Ok = true;
                             retorno.Objeto = caminho_pdf;

                             return retorno;
                         }
                     }

                     String arquivo_modelo_termo_adesao = $@"{request.CaminhoFisico}\Arquivos\Modelo_Termo_Adesao\Modelo_Termo_Adesao.docx";
                     String _pasta_tmp = $@"{request.CaminhoFisico}\Arquivos\Tmp";

                     if (!Directory.Exists(_pasta_tmp))
                         Directory.CreateDirectory(_pasta_tmp);


                     using (DocX document = DocX.Load(arquivo_modelo_termo_adesao))
                     {
                         var arquivo_modificado = GetHtmlTermoAdesao(document, cliente, venda, config, request.CaminhoFisico, request.UrlPortal);

                         String _tmp_html_modificado = $@"{_pasta_tmp}\termo_adesao_{venda.Id}_modificado.html";

                         File.WriteAllText(_tmp_html_modificado, arquivo_modificado);

                         String nome_pdf = $@"termo_adesao_{venda.Id}.pdf";

                         string pastaTermoAdesao = String.Format(@"{0}\Arquivos\TermoAdesao", caminhoProjeto);
                         if (!Directory.Exists(pastaTermoAdesao)) Directory.CreateDirectory(pastaTermoAdesao);

                         string caminho_pdf_local = String.Format(@"{0}\Arquivos\TermoAdesao\{1}", caminhoProjeto, nome_pdf);

                         ConvertToPdfService.convertHtmlToPdf(caminho_pdf_local, _tmp_html_modificado);

                         File.Delete(_tmp_html_modificado);

                         string caminho_pdf = String.Format(@"{0}/Arquivos/TermoAdesao/{1}", caminhoProjetoWebApi, nome_pdf);

                         retorno.Ok = true;
                         retorno.Objeto = caminho_pdf;
                     }
                 }
                 catch (Exception ex)
                 {
                     retorno.Ok = false;
                     retorno.Objeto = null;
                     retorno.Mensagem = ex.Message;
                     if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                     CONSTANTES.log4Net.Error(ex);
                 }
             }

             return retorno;
         }*/


        [HttpPost()]
        [Route("api/TermoAdesaoApi/getPdfTermoAdesao")]
        public async Task<GetApiRetorno<String>> GetPdfTermoAdesao(TermoAdesaoIdVenda request)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<String>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            //   using (var repoConfigCert = new Conexao<Config_Certificacao_Assinatura_RequestModel>(session))
            {
                try
                {
                    Venda venda = await repoVenda.selectFirst(x => x.Id == request.IdVenda);
                    Cliente cliente = await repoCliente.selectFirst(x => x.Id == venda.Id_Cliente);

                    // Config_Certificacao_Assinatura_RequestModel config_Certi = await repoConfigCert.selectFirst();

                    string caminhoProjetoWebApi = request.UrlPortal;
                    string caminhoProjeto = request.CaminhoFisico;

                    if (String.IsNullOrEmpty(caminhoProjetoWebApi))
                    {
                        throw new Exception("UrlPortal não definida no empresa.config");
                    }

                    if (String.IsNullOrEmpty(caminhoProjeto))
                    {
                        throw new Exception("Caminho Fisico não definido no empresa.config");
                    }

                    if (request.IdVenda.HasValue)
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == request.IdVenda.Value);
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
                                case Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum.Lexio:
                                    retorno.Objeto = new IntegracaoAssinaturaRepository().Get_Documento_Pdf(new Lexio(), configCert, venda.Url_Documento_Assinado);
                                    retorno.Ok = true;
                                    return retorno;
                                default:
                                    break;
                            }
                        }

                    }

                    var nome_pdf = String.Format(@"termo_adesao_{0}.pdf", venda.Id);
                    string caminho_pdf_local = String.Format(@"{0}\Arquivos\TermoAdesao\{1}", caminhoProjeto, nome_pdf);
                    if (File.Exists(caminho_pdf_local))
                    {
                        string caminho_pdf = String.Format(@"{0}/Arquivos/TermoAdesao/{1}", caminhoProjetoWebApi, nome_pdf);

                        retorno.Ok = true;
                        retorno.Objeto = caminho_pdf;

                        return retorno;
                    }

                    else
                    {
                        await GeraTermoAdesaoCliente(cliente, venda, request.CaminhoFisico);
                        var caminhoPdf = String.Format(@"{0}/Arquivos/TermoAdesao/{1}", caminhoProjetoWebApi, nome_pdf);
                        retorno.Ok = true;
                        retorno.Objeto = caminhoPdf;

                        return retorno;
                    }
                    //var caminhoPdf = await GeraTermoAdesaoCliente(cliente, venda, request.CaminhoFisico);
                    // retorno.Ok = true;
                    // retorno.Objeto = caminhoPdf;

                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex);
                }
            }

            return retorno;
        }

        private String GetHtmlTermoAdesao(DocX document, Cliente cliente, Venda venda, Configuracoes_Gerais config, String caminhoFisico, String urlBase)
        {
            String _pasta_tmp = $@"{caminhoFisico}\Arquivos\Tmp";
            var arquivo_modelo_contrato_tmp = $@"{_pasta_tmp}\Modelo_Termo_Adesao.docx";

            if (!Directory.Exists(_pasta_tmp))
                Directory.CreateDirectory(_pasta_tmp);

            // Dados pessoais
            document.ReplaceText("(NOME_CLIENTE)", cliente.NomeFantasia ?? "");
            document.ReplaceText("(CPF_CNPJ)", FormatarCpfCnpj(cliente.CpfCnpj));
            document.ReplaceText("(RG)", cliente.Rg ?? "");
            document.ReplaceText("(DATA_EXP)", cliente.Data_Emissao_Rg != null ? cliente.Data_Emissao_Rg?.ToString("dd/MM/yyyy") : "");
            document.ReplaceText("(ORGAO_ESP)", cliente.Orgao_Emissao_Rg ?? "");
            document.ReplaceText("(SEXO)", HtmlEnumExtensions.GetEnumDescription<Sexo>(Convert.ToString(cliente.Sexo)));
            document.ReplaceText("(EMAIL)", cliente.Email_Principal ?? "");
            document.ReplaceText("(FILIACAO)", cliente.Filiacao_Pai ?? "" + ", " + cliente.Filiacao_Mae ?? "");
            document.ReplaceText("(END_INST)", cliente.Endereco_Principal.Logradouro ?? "");
            document.ReplaceText("(END_NUM)", cliente.Endereco_Principal.Numero ?? "");
            document.ReplaceText("(END_BAIRRO)", cliente.Endereco_Principal.Bairro ?? "");
            document.ReplaceText("(CEP)", cliente.Endereco_Principal.Cep ?? "");
            document.ReplaceText("(CIDADE)", cliente.Endereco_Principal.Cidade ?? "");
            document.ReplaceText("(UF)", cliente.Endereco_Principal.UF ?? "");

            // Dados do Vendedor
            document.ReplaceText("(VENDEDOR)", venda.Vendedor != null ? (venda.Vendedor.Nome != null ? venda.Vendedor.Nome : "") : "");
            document.ReplaceText("(SUPERVISOR)", venda.Supervisor != null ? (venda.Supervisor.Nome != null ? venda.Supervisor.Nome : "") : "");
            document.ReplaceText("(DT_VENDA)", venda.Data_Venda != null ? venda.Data_Venda?.ToString("dd/MM/yyyy") : "");

            //Assinatura no termo
            if (config.Utiliza_Certificacao_Assinatura)
            {
                document.ReplaceText("(ASS_VENDEDOR)", "");
            }
            else
            {
                var url = GetAssinatura(venda, true, caminhoFisico, urlBase);

                if (url != null)
                {
                    var docxImage = document.AddImage(url);
                    string rtag = "(ASS_VENDEDOR)";

                    var paragraphs = document.Paragraphs.Where(x => x.Text.Contains(rtag));
                    foreach (var paragraph in paragraphs)
                    {
                        paragraph.InsertPicture(docxImage.CreatePicture(200, 200), 0);
                        paragraph.ReplaceText(rtag, "");
                    }
                }
            }


            // Empresa
            var infoEmpresa = GetInfoEmpresa(Convert.ToString(venda.Vendedor.Filial.Empresa.CodEmpresa));
            document.ReplaceText("(EMPRESA)", infoEmpresa.Nome ?? "");
            document.ReplaceText("(CNPJ)", FormatarCpfCnpj(infoEmpresa.Cnpj));
            document.ReplaceText("(END_EMPRESA)", infoEmpresa.Endereco + " - " + infoEmpresa.Cidade + " - " + infoEmpresa.Estado);
            try
            {
                PreencherLogoEmpresa(document, config.OLA_URL_Logo, infoEmpresa.Logo, "(LOGO)");
            }
            catch (Exception ex)
            {
                document.ReplaceText("(LOGO)", "");
            }


            // Linhas dos cabeçalhos
            var linhaServicos = GetLinhaTabela(document, "Serviços Contratados");
            var linhaFormasPag = GetLinhaTabela(document, "Formas de Pagamento");
            var linhaDadosVendedor = GetLinhaTabela(document, "Dados do Vendedor");

            // Bloco de linhas com dados dos serviços e formas de pagamento
            List<Row> linhasServico = new List<Row>();
            List<Row> linhasPagamento = new List<Row>();

            for (int i = linhaServicos + 1; i < linhaFormasPag; i++)
            {
                linhasServico.Add(document.Tables[0].Rows[i]);
            }

            for (int i = linhaFormasPag + 1; i < linhaDadosVendedor; i++)
            {
                linhasPagamento.Add(document.Tables[0].Rows[i]);
            }

            // Serviços e Formas de Pagamento
            var listaPlanosProdutos = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id);

            List<Int64> Ids_Forma_Pagamento_Cliente = new List<Int64>();
            foreach (var item in listaPlanosProdutos)
            {
                Ids_Forma_Pagamento_Cliente.Add(item.Forma_Pagamento_Cliente.Id);
            }
            var listaFormasPagamentos = cliente.Formas_Pagamento_Cliente.Where(p => Ids_Forma_Pagamento_Cliente.Contains(p.Id));

            // Preenchimento dos produtos
            int indexProd = linhaFormasPag - 1;
            foreach (var plano in listaPlanosProdutos)
            {
                foreach (var linha in linhasServico)
                {
                    indexProd++;
                    var linhaInserida = document.Tables[0].InsertRow(linha, indexProd, true);
                    PreencherLinhaServico(linhaInserida, plano);
                }
            }

            foreach (var linha in linhasServico)
            {
                linha.Remove();
            }

            // Preenchimento das formas de pagamento
            linhaDadosVendedor = GetLinhaTabela(document, "Dados do Vendedor");

            int indexPag = linhaDadosVendedor - 1;
            foreach (var pag in listaFormasPagamentos)
            {
                foreach (var linha in linhasPagamento)
                {
                    indexPag++;
                    var linhaInserida = document.Tables[0].InsertRow(linha, indexPag, true);
                    PreencherLinhaPagamento(linhaInserida, pag, cliente);
                }
            }

            foreach (var linha in linhasPagamento)
            {
                linha.Remove();
            }

            List<String> contratos = new List<string>();
            foreach (var plano in listaPlanosProdutos)
            {
                string contrato = plano.ClienteContrato?.Descricao;
                if (!contratos.Contains(contrato))
                {
                    contratos.Add(contrato);
                }
            }

            if (contratos.Count > 0)
            {
                document.ReplaceText("(CONTRATO)", string.Join(", ", contratos));
            }
            else
            {
                document.ReplaceText("(CONTRATO)", "Contrato");
            }

            document.SaveAs(arquivo_modelo_contrato_tmp);

            String _tmp_html = $@"{_pasta_tmp}\termo_adesao_{venda.Id}.html";

            ConvertToPdfService.convertDocxToHtml(_tmp_html, arquivo_modelo_contrato_tmp);

            using StreamReader stream = File.OpenText(_tmp_html);
            String arquivo = stream.ReadToEnd();
            stream.Close();
            File.Delete(_tmp_html);
            File.Delete(arquivo_modelo_contrato_tmp);

            return arquivo;
        }

        private void PreencherLogoEmpresa(DocX document, String urlLogo, byte[] logoBd, String tag)
        {
            Int32 linhaFoto = GetLinhaTabela(document, tag);
            Int32 colunaFoto = GetCellByTag(document, linhaFoto, tag);

            document.ReplaceText(tag, String.Empty);

            WebClient webClient = new WebClient();
            byte[] data;

            if (logoBd == null)
            {
                data = webClient.DownloadData(urlLogo);
            }
            else
            {
                data = logoBd;
            }

            if (document.Tables[0].RowCount > linhaFoto)
            {
                var stream = new MemoryStream();

                System.Drawing.Image imagem = ImageService.BytesToImage(data);
                imagem.Save(stream, ImageFormat.Png);
                stream.Position = 0;

                Image image = document.AddImage(stream);

                Picture picture = image.CreatePicture();
                picture.Height = 70;
                picture.Width = 70;

                document.Tables[0].Rows[linhaFoto].Cells[colunaFoto].Paragraphs[0].AppendPicture(picture);
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

        [HttpPost()]
        [Route("api/TermoAdesaoApi/validarAssinaturaTermoAdesaoIntegracao")]
        public async Task<GetApiRetorno<bool>> ValidarAssinaturaTermoAdesaoIntegracao(TermoAdesaoIdVenda request)
        {
            GetApiRetorno<bool> retorno = new GetApiRetorno<bool>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            //  using (var repoConfigCert = new Conexao<Config_Certificacao_Assinatura_RequestModel>(session))
            using (var repoVenda = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            {
                try
                {

                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }


                    Venda venda = null;
                    Cliente cliente = null;
                    venda = await repoVenda.selectFirst(p => p.Id == request.IdVenda);
                    cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);

                    Configuracoes_Gerais config = await repoConfig.selectFirst();

                    //  Config_Certificacao_Assinatura_RequestModel configCert = await repoConfigCert.selectFirst();

                    Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                    {
                        Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                        Token_Documento_Assinatura = venda.Token_Documento_Assinatura,
                        Id_Venda = venda.Id,
                        Id_Cliente = cliente.Id


                    };

                    if (config.Utiliza_Certificacao_Assinatura)
                    {
                        Consultar_Documento_ResponseModel retornoIntegracao = new Consultar_Documento_ResponseModel();
                        switch (config.Tipo_Integracao_Assinatura)
                        {
                            case Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum.Lexio:
                                retornoIntegracao = new Sinapse.Integracao.Certificacao_Assinatura.Repositorio.IntegracaoAssinaturaRepository().Consultar_Documento(new Lexio(), session, configCert);
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
                    retorno.Mensagem = Resources.RsCadastros.TERMO_ADESAO_PENDENTE;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }


        [HttpPost]
        [Route("api/TermoAdesaoApi/sendAssinaturaTermoAdesao")]
        public async Task<GetApiRetorno<Boolean>> SendAssinaturaTermoAdesao(AssinaturaTermoAdesaoDados value)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoVenda = new Conexao<Venda>(session))
            {
                try
                {

                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(value.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    Venda venda = null;
                    var dados = value;

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

                    await SalvarAssinatura(dados.Assinatura, venda);

                    if (dados.AtualizarStatus)
                    {
                        venda.Termo_Adesao_Assinado = true;
                        await repoVenda.Save(venda);
                    }

                    string caminho = String.Format(@"{0}\Fotos\AssinaturaTermoAdesao\{1}.png", value.CaminhoFisico ?? String.Empty, venda.Id);
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
                    CONSTANTES.log4Net.Error(ex);
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



        [HttpPost()]
        [Route("api/TermoAdesaoApi/GerarLinkTermoAdesao")]
        public async Task<ApiResponseObj<String>> GerarLinkTermoAdesao(GerarLinkByVenda request)
        {
            var retorno = new ApiResponseObj<String>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoVenda = new Conexao<Venda>(session))
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }

                    var venda = await repoVenda.selectFirst(p => p.Id == request.Venda);
                    if (venda != null)
                    {
                        var criptografia = Criptografia_AES256.doEncryptAES(Convert.ToString(venda.Id));
                        var criptografiateste = Criptografia_AES256.doEncryptAES(Convert.ToString(venda.Id));
                        retorno.data = $@"{CONSTANTES.GetBaseUrlConnect()}/pt/Termo_Adesao/Formulario/{criptografia}";
                        retorno.ok = true;
                    }
                    else
                    {
                        throw new Exception("Venda não encontrada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.data = "";
                retorno.ok = false;
                retorno.error = ex.Message;
                if (ex.InnerException != null) retorno.error += "\n" + ex.InnerException.Message;
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/TermoAdesaoApi/enviarTermoAdesao")]
        public async Task<PostApiRetorno<Int64>> EnviarTermoAdesao(TermoAdesaoIdVenda request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Inválidos");
                        }
                    }

                    Venda venda = null;
                    Cliente cliente = null;

                    using (var repoVenda = new Conexao<Venda>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                    using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == request.IdVenda);
                        cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);
                        var listaPlanosProdutos = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id);

                        if (string.IsNullOrEmpty(cliente.Email_Principal))
                        {
                            throw new Exception("Cliente não possui e-mail vinculado");
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

                        EmailUtil emailService = new EmailUtil(request.Email_De, request.Email_Senha, request.Email_Smtp, request.Email_Porta, request.Email_Usuario);
                        var obj_link = await GerarLinkTermoAdesao(new GerarLinkByVenda { Venda = venda.Id });
                        String urlTermoAdesao = obj_link.data;
                        // string urlTermoAdesao = $"{request.UrlPortal}/pt/Termo_Adesao/Formulario/{venda.Id}";

                        var produtos = "";

                        foreach (var planoProduto in listaPlanosProdutos)
                        {
                            if (planoProduto.Combo != null)
                            {
                                foreach (var planoProdutoCombo in planoProduto.Combo.PlanosProdutos)
                                {
                                    using var stream_produtos = File.OpenText($@"{request.CaminhoFisico}\Arquivos\email_termo_adesao_produtos.html");
                                    {
                                        string produto = stream_produtos.ReadToEnd();
                                        var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                                        produto = produto.Replace("{{DESC}}", planoProdutoCombo.Plano != null ? planoProdutoCombo.Plano.Descricao : planoProdutoCombo.Produto.Descricao)
                                             .Replace("{{VENCIMENTO}}", planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "")
                                             .Replace("{{TESTE}}", dias == 1 ? dias + " dia" : dias + " dias")
                                             .Replace("{{VALOR}}", planoProduto.Valor_Final != null ? planoProduto.Valor_Final?.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");

                                        produtos += produto;
                                    }
                                }
                            }
                            else
                            {
                                using var stream_produtos = File.OpenText($@"{request.CaminhoFisico}\Arquivos\email_termo_adesao_produtos.html");
                                {
                                    string produto = stream_produtos.ReadToEnd();
                                    var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                                    produto = produto.Replace("{{DESC}}", planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao)
                                         .Replace("{{VENCIMENTO}}", planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "")
                                         .Replace("{{TESTE}}", dias == 1 ? dias + " dia" : dias + " dias")
                                         .Replace("{{VALOR}}", planoProduto.Valor_Final != null ? planoProduto.Valor_Final?.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");

                                    produtos += produto;
                                }
                            }
                        }

                        using var stream = File.OpenText($@"{request.CaminhoFisico}\Arquivos\email_termo_adesao.html");
                        string arquivo_email = stream.ReadToEnd();
                        string arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", cliente.NomeFantasia)
                                                                 .Replace("{{URL_TERMO_ADESAO}}", urlTermoAdesao)
                                                                 .Replace("{{URL_LOGO}}", config.OLA_URL_Logo)
                                                                 .Replace("{{PRODUTOS}}", produtos);

                        if (!string.IsNullOrEmpty(cliente.Email_Principal))
                        {
                            emailService.EnviarMensagem(cliente.Email_Principal, $@"Seja muito bem-vindo(a) à {infoEmpresa.Nome}!", arquivo_modificado, request.CaminhoSsl == "S");
                        }
                        else
                        {
                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Mensagem = string.Format(RsCadastros.TERMO_ADESAO_CLIENTE_SEM_EMAIL, cliente.NomeFantasia),
                                Tipo = PostApiMensagemType.Info
                            });
                        }

                        retorno.IdObjeto = venda.Id;
                        retorno.Ok = true;
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex);
                    transaction.Rollback();
                    retorno.IdObjeto = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }


        private InfoEmpresaTermoAdesao GetInfoEmpresa(String codEmpresa)
        {
            InfoEmpresaTermoAdesao retorno = null;

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT RAZAO_SOCIAL, CNPJ, ENDERECO, BAIRRO, CIDADE, UF, LOGOTIPO ");
            sql.AppendLine("FROM TB_EMPRESA ");
            sql.AppendLine("WHERE CODEMPRESA = :pCOD_EMPRESA");

            using ISession session = HibernateUtil.GetSessionT5();
            using Query query = new Query(session);
            {
                query.setParametro("pCOD_EMPRESA", codEmpresa);
                var values = query.Values(sql.ToString()).Result;

                if (values.Count > 0) retorno = new InfoEmpresaTermoAdesao
                {
                    Nome = Convert.ToString(values[0]["RAZAO_SOCIAL"]),
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

        public async Task<KeyValuePair<string, object>> SetVariable(Modelo_ContratoGenerateVariable request, int? index = null)
        {
            KeyValuePair<String, Object> retorno = new KeyValuePair<string, object>("", "");
            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Configuracoes_Gerais> repositoryConfig = new Conexao<Configuracoes_Gerais>(session))
            using (Conexao<Cliente> repositoryCliente = new Conexao<Cliente>(session))
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
                    multas_carencia = request.planos.Select(p => new { contrato = p?.ClienteContrato?.NumeroContrato, valor = (object)p?.ClienteContrato?.MultaCarencia }).OrderBy(p => p.contrato).ToDictionary(p => p.contrato, p => p.valor);
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
                        multa_carencia = request.cpp?.ClienteContrato?.MultaCarencia;
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
                            multa_carencia = multa_carencia > 0 ? multa_carencia : 0;
                            retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), (multas_carencia.Count == 0 ? "0" : string.Join(", ", multas_carencia.Select(p => $"R$ {p.Value} ({p.Key})").ToList())));
                            break;
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
                                    return p.Combo.DetalhesCombo;
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
                                    TermoAdesaoIdVenda termoAdesaoIdVenda = new TermoAdesaoIdVenda();
                                    termoAdesaoIdVenda.IdVenda = request.Venda?.Id;

                                    GetApiRetorno<List<AssinaturaTermoAdesaoDados>> operacao = await AdesaoApi.GetAssinaturaTermoAdesao(termoAdesaoIdVenda);
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
                            if (request.planos.Any(p => p?.Combo != null))
                            {
                                foreach (var plano in request.planos.Where(p => p.Combo != null))
                                {
                                    composicaoCombo.Add($"{plano.Combo.Descricao} - R${plano.Combo.PlanosProdutos.Sum(p => p.ValorInstalacao) + plano.Combo.PlanosProdutos.Sum(p => p.Valor)}");

                                    composicaoCombo.AddRange(plano.Combo.PlanosProdutos.Select(p =>
                                    {
                                        string Descricao = string.IsNullOrEmpty(p?.Plano?.Descricao) ? p?.Produto?.Descricao : p?.Plano?.Descricao;
                                        return $"\t\u2022{Descricao} - R${p?.Valor ?? 0 + p?.ValorInstalacao ?? 0}";
                                    }));
                                }

                                retorno = new KeyValuePair<string, object>(request.item.GetDisplayName(), string.Join("\n", composicaoCombo));
                            }
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

                    retornoURL = $@"{CONSTANTES.GetBaseUrlConnect()}/Arquivos/DocTermoAdesao/_tempLivenessDetect/{form.IdVenda}.webm";

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
                                resposta.message = "A similaridade detectada não é suficiente. Por favor, refaça a gravação do vídeo.";
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
                        resposta.message = "Ocorreu um erro ao processar a imagem, por favor, tente novamente";
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
                        resposta.message = "Ocorreu um erro ao processar a imagem, por favor, tente novamente";
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
                        fileBase64 = base64,
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
                        resposta.message = "Ocorreu um erro ao processar a imagem, por favor, tente novamente";
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

#if DEBUG
        [HttpPost()]
        [Route("api/TermoAdesaoApi/testEnviarTermoLexio")]
        public async Task<GetApiRetorno<object>> TestEnviarTermoLexio(TermoAdesaoTest request)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                try
                {

                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    if (String.IsNullOrEmpty(request.CaminhoModelo))
                        throw new Exception("Caminho do Arquivo Modelo .Docx não informado");

                    string caminhoPDF = $@"{request.CaminhoFisico}\Arquivos\TermoAdesao\termo_adesao_teste.pdf";

                    using (var repoVenda = new Conexao<Venda>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var modeloRepo = new Conexao<TermoAdesao>(session))
                    using (var clienteProdutoRepo = new Conexao<Cliente_PlanoProduto>(session))
                    {
                        var valores = new Dictionary<string, object>();
                        var venda = await repoVenda.selectFirst(p => p.Id == 837);
                        var cliente = await repoCliente.selectFirst(p => p.Id == 979);
                        var clientePlanoProduto = await clienteProdutoRepo.selectFirst(x => x.Id_Venda == venda.Id && x.Cliente == cliente);


                        string caminhoArquivo = request.CaminhoModelo;
                        string caminhoHtml = $@"{request.CaminhoFisico}\Arquivos\TermoAdesao\termo_adesao_teste.html";

                        foreach (VariaveisModeloContrato item in Enum.GetValues(typeof(VariaveisModeloContrato)))
                        {
                            List<Cliente_PlanoProduto> listagemPlanos = cliente.PlanosProdutos.Where(p => p.Id_Venda == venda.Id).ToList();

                            KeyValuePair<string, object> variable = await SetVariable(new API_Aniel.Models.Modelo_ContratoGenerateVariable
                            {
                                item = item,
                                clienteContrato = clientePlanoProduto.ClienteContrato,
                                planos = listagemPlanos,
                                Venda = venda,
                                mensagens = new List<PostApiMensagem>()
                            });

                            if (!String.IsNullOrEmpty(variable.Key) && variable.Value != null)
                                valores.Add(variable.Key, variable.Value);
                        }

                        valores.Remove("");

                        ConvertToPdfService.modificarDocXContrato(new ConvertToPDFServiceRequestModel
                        {
                            caminho = caminhoArquivo,
                            caminhooutput = $@"{request.CaminhoFisico}\Arquivos\TermoAdesao\termo_adesao_teste.docx",

                            caminhohtml = caminhoHtml,
                            caminhopdf = caminhoPDF,
                            LogoVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_LOGO),
                            filename = Path.Combine(request.CaminhoFisico, @"Arquivos\LogoCliente.jpg"),//logo do cliente
                            lista_replace = valores.Select(p => new ReplaceDocXModel { Placeholder = p.Key, ReplaceValue = Convert.ToString(p.Value) }).ToList(),
                            FooterVar = HtmlEnumExtensions.GetEnumDescription(VariaveisModeloContrato.VAR_FOOTER),
                            FilenameFooter = Path.Combine(request.CaminhoFisico, @"Arquivos\Footer.jpg"),
                            zoomFactorPdf = "1.3"
                        });
                    }

                    var configEmpresa = await repoConfig.selectFirst();

                    if (configEmpresa.Utiliza_Certificacao_Assinatura)
                    {
                        switch (configEmpresa.Tipo_Integracao_Assinatura)
                        {
                            case Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum.Lexio:
                                if (request.EnviarLexio)
                                {
                                    Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                                    {
                                        Token_Integracao_Assinatura = configEmpresa.Token_Integracao_Assinatura,
                                        Id_Cliente = 1,
                                        Email_Principal = "yago@sinapseinformatica.com.br",
                                        Email_Auxiliar = "yago@sinapseinformatica.com.br",
                                        NomeFantasia = "Yago Dev Teste",
                                        Id_Venda = 1,
                                        Tipo_Integracao_Assinatura = TipoIntegracaoAssinaturaEnum.Lexio


                                    };

                                    string urlTermoIntegracao = String.Empty;
                                    var retornoIntegracao = new Enviar_Documento_ResponseModel();

                                    Byte[] bytes = File.ReadAllBytes(caminhoPDF);
                                    String file = Convert.ToBase64String(bytes);

                                    retornoIntegracao = new IntegracaoAssinaturaRepository().Enviar_Documento(new Lexio(), session, configCert, file);

                                    string Url_Assinar_Documento = retornoIntegracao.Link_Assinatura;
                                    string Token_Documento_Assinatura = retornoIntegracao.Id_Documento_Integracao;
                                    string Url_Pdf_Termo_Adesao = $@"{request.UrlPortal}/Arquivos/TermoAdesao/termo_adesao_teste.pdf";

                                    retorno.Objeto = new
                                    {
                                        Url_Assinar_Documento,
                                        Token_Documento_Assinatura,
                                        Url_Pdf_Termo_Adesao
                                    };
                                }
                                else
                                {
                                    string Url_Pdf_Termo_Adesao = $@"{request.UrlPortal}/Arquivos/TermoAdesao/termo_adesao_teste.pdf";

                                    retorno.Objeto = new
                                    {
                                        Url_Pdf_Termo_Adesao
                                    };
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex);
                }
            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/TermoAdesaoApi/testValidarTermoLexio")]
        public async Task<GetApiRetorno<object>> TestValidarTermoLexio(TermoAdesaoTest request)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                try
                {

                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    Configuracoes_Gerais config = await repoConfig.selectFirst();

                    //  Config_Certificacao_Assinatura_RequestModel configCert = await repoConfigCert.selectFirst();

                    Config_Certificacao_Assinatura_RequestModel configCert = new Config_Certificacao_Assinatura_RequestModel
                    {
                        Token_Integracao_Assinatura = config.Token_Integracao_Assinatura,
                        Token_Documento_Assinatura = request.TokenDocumentoAssinatura,

                    };

                    if (config.Utiliza_Certificacao_Assinatura)
                    {
                        Consultar_Documento_ResponseModel retornoIntegracao = new Consultar_Documento_ResponseModel();
                        switch (config.Tipo_Integracao_Assinatura)
                        {
                            case Sinapse.Model.enumerados.TipoIntegracaoAssinaturaEnum.Lexio:
                                retornoIntegracao = new Sinapse.Integracao.Certificacao_Assinatura.Repositorio.IntegracaoAssinaturaRepository().Consultar_Documento(new Lexio(), session, configCert);

                                //if(retornoIntegracao.IsAssinado)
                                //caminhoPdf = new IntegracaoAssinaturaRepository().Get_Documento_Pdf(new Lexio(), configCert, retornoIntegracao.Link_Download_Documento);
                                break;
                            default:
                                break;
                        }
                        if (!retornoIntegracao.IsAssinado)
                        {
                            retorno.Objeto = false;
                            throw new Exception(Resources.RsCadastros.TERMO_ADESAO_PENDENTE);
                        }

                        retorno.Objeto = new
                        {
                            retornoIntegracao.IsAssinado,
                            retornoIntegracao.Link_Download_Documento
                        };
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    //retorno.IdObjeto.IdVenda = 0;
                    retorno.Mensagem = Resources.RsCadastros.TERMO_ADESAO_PENDENTE;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }
#endif
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