using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Checklist;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class TrocaTitularidadeController : BaseController
    {

        public TrocaTitularidadeController() : base("TrocaTitularidade",
                                            CONSTANTES.URLPortal,
                                            String.Empty,
                                            "api/TrocaTitularidadeApi/FindById?id={0}",
                                            "api/TrocaTitularidadeApi/Gravar",
                                            "api/TrocaTitularidadeApi/Cancelar",
                                            String.Empty)
        {
        }



        // GET: TrocaTitularidade
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> Cadastro(string idCliente, string numeroContrato, long idTrocaTitularidade)
        {
            var campos = new CamposObrigatoriosVendaApiController().GetCampos().Result.ToList<ICamposObrigatoriosModel>();
            ViewBag.CamposObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campos);
            ViewBag.Areas = Areas;
            var response = new PostApiRetorno<CadastroTrocaTitularidadeModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var trocaTitularidadeApi = new TrocaTitularidadeApiController())
            {
                if (idTrocaTitularidade == 0)
                {
                    response = await trocaTitularidadeApi.FindById(idTrocaTitularidade,
                                                                       Convert.ToInt64(idCliente),
                                                                       Convert.ToInt64(numeroContrato));
                }
                else
                {
                    response = await trocaTitularidadeApi.FindById(idTrocaTitularidade, null, null);
                }
                ViewBag.Utiliza_Cadastro_Endereco = Convert.ToBoolean(repoConfigGerais.selectFirst().Result.Utiliza_Cadastro_Endereco);
                ViewBag.Utiliza_Id_Cliente_Sequencial = repoConfigGerais.selectFirst().Result.Identificacao_Cliente == Model.enumerado.Identificacao_Cliente.Sequencial;
            }

            return View(response.IdObjeto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cadastro(CadastroTrocaTitularidadeModel model)
        {
            ViewBag.Areas = Areas;
            try
            {
                if (ModelState.IsValid)
                {
                    //PostApiRetorno<Int64> retorno = PostByUrl<VendaDadosGeraisViewModel, Int64>(valor, UrlPortal, UrlPost);
                    ParamWebApi<CadastroTrocaTitularidadeModel> modelWebPai = new ParamWebApi<CadastroTrocaTitularidadeModel>
                    {
                        Usuario = User.Identity.GetUserId(),
                        Objeto = model
                    };

                    PostApiRetorno<Int64> retorno = await new TrocaTitularidadeApiController().Gravar(modelWebPai);

                    if (retorno.Ok) //Edição
                    {
                        using (ISession session = HibernateUtil.GetSession())
                        using (var repoTroca = new Conexao<TrocaTitularidade>(session))
                        {
                            TrocaTitularidade trocaTitularidade = repoTroca.selectFirst(p => p.Id == retorno.IdObjeto).Result;

                            IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
                            IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);

                            var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);

                            //bool trocaConcluida = !etapaVendaBusiness.GetDadosEtapasTrocaTitularidade(trocaTitularidade.Id)
                            //                                         .Any(e => e.Status != (int)StatusEtapaVenda.Aprovado && e.Status != (int)StatusEtapaVenda.AprovadoManualmente);

                            return RedirectToAction("Cadastro", new { idTrocaTitularidade = retorno.IdObjeto });
                        }
                    }
                    else
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                    }

                }

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, ex.Message, ex.InnerException?.Message);
            }

            return View(model);
        }


        [AllowAnonymous]
        public async Task<ActionResult> VisualizarChecklist(long troca)
        {
            ViewBag.Areas = Areas;

            using (var apiQuiz = new QuizApiController())
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionFotoSql = HibernateUtil.GetSessionFoto())
            using (var repoDocumentoProducao = new Conexao<Documento_ProducaoAniel>(sessionT5Nh))
            using (var repoTroca = new Conexao<TrocaTitularidade>(sessionNh))
            {
                var model = new GetApiRetorno<List<QuizAnswer>>
                {
                    Objeto = new List<QuizAnswer>()
                };

                GetApiRetorno<List<QuizAnswer>> resultado = await apiQuiz.GetQuizAnswerVenda(0, troca);

                model.Objeto = resultado.Objeto;

                var obj = await repoTroca.selectFirst(i => i.Id == troca);
                ViewBag.TrocaId = troca;
                ViewBag.Cliente = obj?.ClienteTitularNovo?.NomeFantasia ?? String.Empty;

                return View(model.Objeto);
            }
        }


        [AllowAnonymous]
        public async Task<ActionResult> EmailChecklistTroca(long idTroca)
        {
            ViewBag.Areas = Areas;
            var model = new EmailChecklistTrocaModel();
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (var repoTroca = new Conexao<TrocaTitularidade>(session))
            {
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

                IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);

                var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();
                Quiz checklist = checklistBusiness.GetChecklistTrocaTitularidade(idTroca);

                model.IdTroca = idTroca;

                model.NomeEmpresa = config.Nome_Cliente_Sinapse;
                model.Cnpj = config.CNPJ;
                model.LogoClienteBase64 = config.OLA_URL_Logo;

                model.MensagemConclusao = checklist?.MensagemConcluido ?? CONSTANTES.MensagemPadraoConclusaoChecklist;
                model.ChecklistPreenchido = checklistBusiness.ChecklistTrocaPreenchido(idTroca);

                var troca = await repoTroca.selectFirst(i => i.Id == idTroca);
                model.Status = troca.StatusTrocaTitularidade;

                return View(model);
            }
        }
    }
}