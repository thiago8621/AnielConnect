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
    public class SlotController : BaseController
    {

        public SlotController() : base("Slot", 
                                       CONSTANTES.URLPortalInterno,
                                       String.Empty,
                                       "api/SlotApi/FindById?id={0}",
                                       "api/SlotApi/Gravar",
                                       "api/SlotApi/Excluir",
                                       String.Empty)
        {

        }

        // GET: Slot
        public ActionResult Index()
        {
            //FillListasView();

            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<SlotCadastroModel, Int64?>(id);

            return retorno;
        }

        // POST: Slot/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(SlotCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<SlotCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {

        }

        protected void FillListasView()
        {


            var _retornoSlot = new SlotApiController().GetList().Result;

            if (!_retornoSlot.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoSlot.Mensagem, _retornoSlot.InnerException);

            ViewBag.Slot = _retornoSlot.Objeto;

            var _retornoOLT = new OLTApiController().GetListOLT().Result;

            if (!_retornoOLT.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoOLT.Mensagem, _retornoOLT.InnerException);

            ViewBag.OLT = _retornoOLT.Objeto;

        }

        // GET: Slot/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<SlotCadastroModel, Int64>(id.Value);
        }

        // POST: Slot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(SlotCadastroModel pModel)
        {
            return base.DeleteBase<SlotCadastroModel, Int64>(pModel);
        }
    }
}