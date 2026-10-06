using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class CampanhaController : BaseController
    {

        public CampanhaController() : base("Campanha",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Campanhaapi/FindById?id={0}",
                                                   "api/Campanhaapi/Gravar",
                                                   "api/Campanhaapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Campanha
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<CampanhaCadastroModel, Int64?>(id);

            //if (!id.HasValue)
            //    ((CampanhaCadastroModel)retorno.Model).TipoValor = TipoValor_CampanhaCadastroModel.Valor;

            return retorno;
        }

        // POST: Campanha/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(CampanhaCadastroModel pModel)
        {
            return base.CadastroBase<CampanhaCadastroModel, Int64>(pModel);
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Planos = new List<PlanoProdutoListModel>();

            ViewBag.Filiais = new List<FilialListaModel>();

            ViewBag.Vendedor = new List<FuncionarioListaModel>();

            ViewBag.Supervisor = new List<FuncionarioListaModel>();

            var _retornoPlanos = new PlanoProdutoApiController().GetList().Result;

            var _retornofilial = new FilialApiController().GetList().Result;

            var _retornoVendedor = new FuncionarioApiController().GetListVendedor().Result;

            var _retornoSupervisor = new FuncionarioApiController().GetListSupervisor().Result;

            if (!_retornoPlanos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPlanos.Mensagem, _retornoPlanos.InnerException);

            if (!_retornofilial.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornofilial.Mensagem, _retornofilial.InnerException);

            if (!_retornoVendedor.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoVendedor.Mensagem, _retornoVendedor.InnerException);

            if (!_retornoSupervisor.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoVendedor.Mensagem, _retornoVendedor.InnerException);

            ViewBag.Planos = _retornoPlanos.Objeto;
            ViewBag.Filiais = _retornofilial.Objeto;
            ViewBag.Vendedor = _retornoVendedor.Objeto;
            ViewBag.Supervisor = _retornoSupervisor.Objeto;

            ViewBag.ComboTmp = new List<ComboObj>();
        }

        // GET: Campanha/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<CampanhaCadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(CampanhaCadastroModel pModel)
        {
            return base.DeleteBase<CampanhaCadastroModel, Int64>(pModel);
        }
    }
}