using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class PonController : BaseController
    {

        public PonController() : base("Pon", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Ponapi/FindById?id={0}",
                                                   "api/Ponapi/Gravar",
                                                   "api/Ponapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Pon
        public ActionResult Index()
        {
            //FillListasView();

            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<PonCadastroModel, Int64?>(id);

            return retorno;
        }

        // POST: Pon/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(PonCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<PonCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            
        }
        protected void FillListasView()
        {

            var _retornoPon = new PonApiController().GetList().Result;

            if (!_retornoPon.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPon.Mensagem, _retornoPon.InnerException);

            ViewBag.Pon = _retornoPon.Objeto;

            var _retornoSlot = new SlotApiController().GetList().Result;

            if (!_retornoSlot.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoSlot.Mensagem, _retornoSlot.InnerException);

            ViewBag.Slot = _retornoSlot.Objeto;

            var _retornoOLT = new OLTApiController().GetListOLT().Result;

            if (!_retornoOLT.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoOLT.Mensagem, _retornoOLT.InnerException);

            ViewBag.OLT = _retornoOLT.Objeto;

        }

        // GET: Pon/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<PonCadastroModel, Int64>(id.Value);
        }

        // POST: Pon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PonCadastroModel pModel)
        {
            return base.DeleteBase<PonCadastroModel, Int64>(pModel);
        }
    }
}