using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Models;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ProdutosController : BaseController
    {
        public ProdutosController() : base("Produto",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Produtoapi/FindById?id={0}",
                                                   "api/Produtoapi/Gravar",
                                                   "api/Produtoapi/Excluir",
                                                   String.Empty)

        {
        }
        // GET: Produto
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            using (var configuracoes_api = new Configuracoes_GeraisApiController())
            {
                ViewBag.UtilizaEva = configuracoes_api.FindById(1).Result.Objeto?.UtilizaEva;
            }
            return base.CadastroBase<ProdutoCadastroModel, Int64?>(id);
        }

        // POST: Produto/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(ProdutoCadastroModel pModel)
        {
            var retorno = base.CadastroBase<ProdutoCadastroModel, Int64>(pModel);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
                return RedirectToAction("Index", "Plano_Produto");

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.ModContrato = new List<Modelo_ContratoListaModel>();
            ViewBag.Atividades = new List<Atividade_NfListModel>();

            var _retornoModContrato = new Modelo_ContratoApiController().GetList().Result;
            var _retornoAtividades = new Atividade_NfApiController().GetList(Tipo_Atividade_Nf.Plano_Produto).Result;

            if (!_retornoModContrato.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoModContrato.Mensagem, _retornoModContrato.InnerException);

            if (!_retornoAtividades.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoAtividades.Mensagem, _retornoAtividades.InnerException);

            ViewBag.ModContrato = _retornoModContrato.Objeto;
            ViewBag.Atividades = _retornoAtividades.Objeto;
        }

        public ActionResult Descontos()
        {
            //var planoViewModel = new ConfiguracaoPlanoModel();
            return PartialView("DescontosViewModel.cshtml", null);
        }

        public ActionResult FaixaMensalidade()
        {
            //var planoViewModel = new ConfiguracaoPlanoModel();
            return PartialView("FaixaMensalidadeViewModel.cshtml", null);
        }

        public ActionResult Detalhes(Int64 Id)
        {
            var retorno = new PlanoApiController().GetListDetalhes(Id);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);
        }

    }
}