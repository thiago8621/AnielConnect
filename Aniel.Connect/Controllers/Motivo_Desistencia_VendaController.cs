using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Motivo_Desistencia_VendaController : BaseController
    {

        public Motivo_Desistencia_VendaController() : base("Motivo_Desistencia_Venda",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Motivo_Desistencia_VendaApi/FindById?id={0}",
                                                   "api/Motivo_Desistencia_VendaApi/Gravar",
                                                   "api/Motivo_Desistencia_VendaApi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Desconto
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
      
        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewBag.Areas = AcessoHelper.TemAcesso(Areas, "159");

            if (Areas.Find(p => p.Cod_Area == "159") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewResult retorno = (ViewResult)base.CadastroBase<Motivo_Desistencia_VendaCadastroModel, Int64?>(id);
            return retorno;

        }

        // POST: Desconto/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Motivo_Desistencia_VendaCadastroModel pModel)
        { 
            ViewBag.Areas = AcessoHelper.TemAcesso(Areas, "159");
            ActionResult retorno = base.CadastroBase<Motivo_Desistencia_VendaCadastroModel, Int64?>(pModel);
            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            View(pModel);
        }
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Motivo_Desistencia_VendaCadastroModel, Int64>(id.Value);
        }

        // POST: Desconto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Motivo_Desistencia_VendaCadastroModel pModel)
        {
            return base.DeleteBase<Motivo_Desistencia_VendaCadastroModel, Int64>(pModel);
        }
    }
}