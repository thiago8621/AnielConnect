using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [AllowAnonymous]
    //[Authorize]
    public class Termo_AdesaoController : BaseController
    {
        public Termo_AdesaoController() : base("Termo_Adesao",
                                           CONSTANTES.URLPortalInterno,
                                           String.Empty,
                                           "api/TermoAdesaoApi/FindById?id={0}",
                                           "api/TermoAdesaoApi/Gravar",
                                           "api/TermoAdesaoApi/Excluir",
                                           String.Empty)
        {

        }

        public async Task<ActionResult> Cadastro()
        {
            var viewModel = new TermoAdesaoCadastroModel();

            using (ISession session = HibernateUtil.GetSession())
            using (var repo = new Conexao<TermoAdesao>(session))
            {
                var obj = await repo.selectFirst();

                viewModel.MensagemConcluido = obj?.MensagemConcluido;
                viewModel.NomeArquivo = obj?.NomeArquivo;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> SalvarDocumento(TermoAdesaoCadastroModel model)
        {
            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Adesao";
            String _arquivo = $@"{_pasta}\{Request.Files[0].FileName}";

            if (!Directory.Exists(_pasta))
            {
                Directory.CreateDirectory(_pasta);
            }

            if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
            {
                Request.Files[0].SaveAs(_arquivo);
            }
            else
            {
                _arquivo = "";
            }

            using (ISession session = HibernateUtil.GetSession())
            using (var repo = new Conexao<TermoAdesao>(session))
            {
                var obj = await repo.selectFirst();

                if (obj == null) //novo
                {
                    obj = new TermoAdesao
                    {
                        NomeArquivo = Request.Files[0].FileName,
                        CaminhoArquivo = _arquivo,
                        MensagemConcluido = model.MensagemConcluido
                    };
                }
                else
                {
                    if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                    {
                        obj.NomeArquivo = Request.Files[0].FileName;
                        obj.CaminhoArquivo = _arquivo;
                    }

                    obj.MensagemConcluido = model.MensagemConcluido;
                }

                await repo.SaveOrUpdate(obj);
            }

            TempData["sucesso"] = true;

            return RedirectToAction("Cadastro");
        }

        [AllowAnonymous]
        public async Task<ActionResult> Formulario(String Id)
        {
            var retorno = new GetApiRetorno<TermoAdesaoFormularioModel>();

            try
            {
                using (var api = new Termo_AdesaoApiController())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoEtapaVenda = new Conexao<EtapaVenda>(session))
                using (var repoTrocaTitularidade = new Conexao<TrocaTitularidade>(session))
                using (var repoConfiguracao_Gerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    var IdVenda = Criptografia_AES256.doDecryptAES(Id);

                    var venda = await repoVenda.selectFirst(p => p.Id == Convert.ToInt64(IdVenda));
                    var trocaTitularidade = await repoTrocaTitularidade.selectFirst(t => t.Id == Convert.ToInt64(IdVenda));

                    EtapaVenda etapaTermo = null;

                    if (trocaTitularidade != null)
                    {
                        etapaTermo = await repoEtapaVenda.selectFirst(p => p.TrocaTitularidade == trocaTitularidade && p.Etapa == Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao);

                        ViewBag.StatusTroca = trocaTitularidade.StatusTrocaTitularidade;

                        if (trocaTitularidade.TermoAdesaoAssinado)
                        {
                            var cliente = await repoCliente.selectFirst(p => p.Id == trocaTitularidade.ClienteTitularNovo.Id);
                            return RedirectToAction("FormularioConcluido", new { Cliente = cliente.NomeFantasia, IsAssinado = trocaTitularidade.TermoAdesaoAssinado, TrocaId = trocaTitularidade.Id, MensagemErro = String.Empty });
                        }
                        else if (etapaTermo.Status == Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado)
                        {
                            return RedirectToAction("FormularioConcluido", new TermoAdesaoFormularioConcluidoRequestModel { IsAssinado = false, TrocaId = trocaTitularidade.Id, MensagemErro = "Seu termo de adesão foi recusado com sucesso.A empresa será notificada sobre a recusa" });
                        }
                    }
                    else
                    {
                        etapaTermo = await repoEtapaVenda.selectFirst(p => p.Venda == venda && p.Etapa == Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao);

                        ViewBag.VendaStatus = venda.Status;

                        if (venda.Termo_Adesao_Assinado)
                        {
                            var cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente);
                            return RedirectToAction("FormularioConcluido", new { Cliente = cliente.NomeFantasia, IsAssinado = venda.Termo_Adesao_Assinado, IdVenda = venda.Id, MensagemErro = String.Empty });
                        }
                        else if (etapaTermo.Status == Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado)
                        {
                            return RedirectToAction("FormularioConcluido", new TermoAdesaoFormularioConcluidoRequestModel { IsAssinado = false, IdVenda = venda.Id, MensagemErro = "Seu termo de adesão foi recusado com sucesso.A empresa será notificada sobre a recusa" });
                        }
                    }


                    var obj_config = await repoConfiguracao_Gerais.selectFirst();

                    retorno = venda == null ? await api.FindByIdTrocaTitularidade(Convert.ToInt64(IdVenda)) : await api.FindById(Convert.ToInt64(IdVenda));

                    if (!retorno.Ok)
                    {
                        throw new Exception(retorno.Mensagem);
                    }
                }
            }
            catch (Exception ex)
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionNh))
                {
                    var config = await repoConfig.selectFirst();
                    return RedirectToAction("FormularioConcluido", new TermoAdesaoFormularioConcluidoRequestModel { IsAssinado = true, MensagemErro = $@"Não foi possível identificar um contrato vinculado a sua proposta de venda, entre em contato com a {config.Nome_Cliente_Sinapse}" });
                }
            }
            return View(retorno.Objeto);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Formulario_Termo_Contrato(Int64 Id)
        {
            using (var api = new Termo_AdesaoApiController())
            {
                var retorno = await api.FindById(Id);

                if (!retorno.Ok)
                {
                    throw new Exception(retorno.Mensagem);
                }

                return View(retorno.Objeto);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Formulario(TermoAdesaoFormularioModel Model)
        {

            var retorno = await new Termo_AdesaoApiController().GravarFormulario(new ParamWebApi<TermoAdesaoFormularioModel>
            {
                Usuario = null,
                Objeto = Model
            });

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem);
            }

            if (retorno.Mensagem == RsCadastros.TERMO_ADESAO_RECUSADO)
            {
                return RedirectToAction("FormularioConcluido", new TermoAdesaoFormularioConcluidoRequestModel { Cliente = Model.Cliente, IsAssinado = retorno.IdObjeto.IsAssinado, IdVenda = Model.IdVenda, MensagemErro = "Seu termo de adesão foi recusado com sucesso.A empresa será notificada sobre a recusa" });
            }


            return RedirectToAction("FormularioConcluido", new TermoAdesaoFormularioConcluidoRequestModel { Cliente = Model.Cliente, IsAssinado = retorno.IdObjeto.IsAssinado, IdVenda = Model.IdVenda, MensagemErro = retorno.Mensagem });
        }

        [AllowAnonymous]
        public async Task<ActionResult> FormularioConcluido(TermoAdesaoFormularioConcluidoRequestModel Model)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoTermoAdesao = new Conexao<TermoAdesao>(session))
            {
                var config = await repoConfig.selectFirst();
                var termoAdesao = await repoTermoAdesao.selectFirst();

                String mensagem = "";

                if (!String.IsNullOrEmpty(Model.MensagemErro))
                {
                    mensagem = Model.MensagemErro;
                }
                else if (Model.IsAssinado && String.IsNullOrEmpty(Model.MensagemErro))
                {
                    mensagem = termoAdesao?.MensagemConcluido ?? $"Prezado(a) {Model.Cliente}, sua resposta foi gravada com sucesso!";
                }

                return View(new TermoAdesaoFormularioConcluidoModel
                {
                    Cliente = Model.Cliente,
                    UrlLogo = config.OLA_URL_Logo,
                    Empresa = config.Nome_Cliente_Sinapse,
                    Mensagem = mensagem,
                    Sucesso = Model.IsAssinado,
                    IdVenda = Model.IdVenda == 0 ? Model.TrocaId : Model.IdVenda
                });
            }
        }

        [HttpPost]
        public async Task<PostApiRetorno<DocTermoAnexo>> GravarAnexoAvulso(GravarAnexoAvulsoModel model)
        {
            string retornoURL = null;
            PostApiRetorno<DocTermoAnexo> retorno = new PostApiRetorno<DocTermoAnexo>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<DocTermoAnexo>(session))
                using (var repositoryVenda = new Conexao<Venda>(session))
                using (var repoTrocaTitularidade = new Conexao<TrocaTitularidade>(session))
                {
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\DocTermoAdesao\";

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    for (var i = 0; i < Request.Files.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(Request.Files[i].FileName))
                        {
                            string nomeArquivo = Path.GetFileName(Request.Files[i].FileName);
                            string caminhoCompleto = Path.Combine(_pasta, nomeArquivo);
                            var xmltoString = new System.Xml.XmlDocument();
                            byte[] allBytes = new byte[Request.Files[i].InputStream.Length];
                            int bytesRead = Request.Files[i].InputStream.Read(allBytes, 0, (int)Request.Files[i].InputStream.Length);
                            String _arquivo = _arquivo = $@"{_pasta}\{nomeArquivo}";
                            System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);

                            Venda venda = null;
                            TrocaTitularidade troca = null;

                            DocTermoAnexo anexo = new DocTermoAnexo
                            {
                                Nome = nomeArquivo,
                                Extensao = model.ext,
                                TipoDocumento = (Model.enumerado.Tipo_Doc_Termo_Anexo)model.idTipoDoc,
                                Status = (Model.enumerado.Status_Doc_Aceite)model.status,
                                Lado = (Model.enumerado.Lado_Doc_Aceite)model.idLado,                            
                            };

                            if (model.idTroca != null)
                            {
                                troca = await repoTrocaTitularidade.selectFirst(p => p.Id == model.idTroca);
                                anexo.TrocaTitularidade = troca;
                            }
                            else
                            {
                                venda = await repositoryVenda.selectFirst(p => p.Id == model.idVenda);
                                anexo.Venda = venda;
                            }

                            repository.Save(anexo).Wait();

                            retornoURL = $@"{CONSTANTES.URLPortal}/Arquivos/DocTermoAdesao/{nomeArquivo}";
                            anexo.Url = retornoURL;
                            repository.SaveOrUpdate(anexo).Wait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }
    }
}