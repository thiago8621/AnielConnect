using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Microsoft.AspNet.Identity;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class AlteracaoPlanoContratoController : BaseController
    {
        public AlteracaoPlanoContratoController() : base("Alteracao_PlanoContrato", CONSTANTES.URLPortal)
        {

        }
        public ActionResult Index(string msgErro = "")
        {
            ViewBag.Areas = Areas;
            using (var session = HibernateUtil.GetSession())
            {
                var repository = new ConfiguracoesGeraisRepository(session);
                var config = new ConfiguracoesGeraisBusiness(repository).GetConfiguracoesGerais();
                ViewBag.ConsultaPendenciaFinanceiraVendedorVisualiza = config.ConsultaPendenciaFinanceiraVendedorVisualiza;
            }
            if (!string.IsNullOrEmpty(msgErro))
            {
                string[] erro = msgErro.Split('|');
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, erro[0], erro[1]);
            }
            return View();
        }
        public async Task<ActionResult> Cadastro(long? IdCliente, long? IdClienteContrato, long? IdPlano, long? IdClientePlanoProduto, long? IdVenda)
        {
            ViewBag.Areas = Areas;
            bool exigeAssinaturaTermo = false;
            var AreasCobertura = new List<long>();
            AlteracaoPlanoContratoCadastroModel mudancaPlanoContrato = new AlteracaoPlanoContratoCadastroModel();
            try
            {
                using (var api = new AlteracaoPlanoContratoApiController())
                {
                    if (IdVenda.HasValue)
                    {
                        mudancaPlanoContrato = (await api.FindById(IdVenda.Value)).Objeto;
                    }
                    else if (IdCliente.HasValue)
                    {
                        mudancaPlanoContrato = (await api.GetVendaByClienteContratoPlano(IdCliente.Value, IdClienteContrato, IdPlano, IdClientePlanoProduto)).Objeto;
                    }
                    IConfiguracoesGeraisRepository repository = new ConfiguracoesGeraisRepository(HibernateUtil.GetSession());
                    var business = new ConfiguracoesGeraisBusiness(repository);
                    var config = business.GetConfiguracoesGerais();
                    exigeAssinaturaTermo = config.Exige_Termo_Adesao;
                    if (config.Ofertar_Plano_Produto_Cobertura)
                    {
                        
                        AreasCobertura = (await api.GetAreas_Consulta(Convert.ToDouble(mudancaPlanoContrato.VendaBase.EnderecoInstalacao.Latitude), Convert.ToDouble(mudancaPlanoContrato.VendaBase.EnderecoInstalacao.Longitude)));
                    }
                }
                ViewBag.ExigeAssinaturaTermo = exigeAssinaturaTermo;
                ViewBag.AreasCobertura = AreasCobertura;
            }
            catch (Exception ex)
            {

                return RedirectToAction("Index", "AlteracaoPlanoContrato", new { msgErro = $"{ex.Message}|{ex.InnerException?.Message}" });
            }

            return View(mudancaPlanoContrato);
        }
        [HttpPost()]
        public async Task<ActionResult> Cadastro(AlteracaoPlanoContratoCadastroModel view)
        {
            using (var apiMudancaContrato = new AlteracaoPlanoContratoApiController())
            using (var api = new VendaApiController())
            {

                await apiMudancaContrato.Gravar(new ParamWebApi<AlteracaoPlanoContratoCadastroModel>
                {
                    Objeto = view,
                    Usuario = User.Identity.GetUserId(),
                    NomeUsuario = User.Identity.GetUserName()
                });

            }
            return RedirectToAction("Index", "AlteracaoPlanoContrato");
        }
        
        [HttpGet] 
        public ActionResult Historico_Alteracao(long? id, long? venda) 
        {
            ViewBag.Chave = "Venda";
            ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                        new SelectListItem{ Value = "3", Text = Resources.RsBase.EXCLUSAO },
                        new SelectListItem{ Value = "4", Text = Resources.RsBase.BAIXA },
                    };
            return View();
        }
    }
}