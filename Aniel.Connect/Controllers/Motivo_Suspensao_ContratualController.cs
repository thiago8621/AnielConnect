using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Motivo_Suspensao_ContratualController : BaseController
    {
        public Motivo_Suspensao_ContratualController() : base("Motivo_Suspensao_Contratual",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Motivo_Suspensao_ContratualApi/FindById?id={0}",
                                                   "api/Motivo_Suspensao_ContratualApi/Gravar",
                                                   "api/Motivo_Suspensao_ContratualApi/Excluir",
                                                   String.Empty)
        { }

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Motivo_Suspensao_ContratualCadastroModel, Int64?>(id);

            return retorno;
        }

        [HttpPost]
        public ActionResult Cadastro(Motivo_Suspensao_ContratualCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Motivo_Suspensao_ContratualCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {

        }

        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Motivo_Suspensao_ContratualCadastroModel, Int64>(id.Value);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Motivo_Suspensao_ContratualCadastroModel pModel)
        {
            return base.DeleteBase<Motivo_Suspensao_ContratualCadastroModel, Int64>(pModel);
        }
    }
}