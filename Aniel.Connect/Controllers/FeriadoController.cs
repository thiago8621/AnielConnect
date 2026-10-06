using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Models;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class FeriadoController : BaseController
    {

        public FeriadoController() : base("Feriado",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Feriadoapi/FindById?id={0}",
                                                   "api/Feriadoapi/Gravar",
                                                   "api/Feriadoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Feriado
        public ActionResult Index()
        {
            ViewBag.ComboTmp = new List<ComboObj>();
            ViewBag.Paises = new PaisApiController().GetList().Result.Objeto;
            ViewBag.Areas = Areas;
            //ViewBag.MesAnoIni = $"01/{DateTime.Now.Year}";
            //ViewBag.MesAnoFim = $"12/{DateTime.Now.Year}";
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            
            return base.CadastroBase<FeriadoCadastroModel, Int64?>(id);
        }

        // POST: Feriado/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(FeriadoCadastroModel pModel)
        {

            ViewBag.ComboTmp = new List<ComboObj>();

            return base.CadastroBase<FeriadoCadastroModel, Int64>(pModel);
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.ComboTmp = new List<ComboObj>();
            ViewBag.Paises = new PaisApiController().GetList().Result.Objeto;
        }

        // GET: Feriado/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<FeriadoCadastroModel, Int64>(id.Value);
        }

        // POST: Feriado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FeriadoCadastroModel pModel)
        {
            return base.DeleteBase<FeriadoCadastroModel, Int64>(pModel);
        }
    }
}