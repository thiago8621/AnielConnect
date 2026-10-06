using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class PeriodoController : BaseController
    {

        public PeriodoController() : base("Periodo",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Periodoapi/FindById?id={0}",
                                                   "api/Periodoapi/Gravar",
                                                   "api/Periodoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Horario
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<PeriodoCadastroModel, Int64?>(id);

            //if (!id.HasValue)
            //    ((CampanhaCadastroModel)retorno.Model).TipoValor = TipoValor_CampanhaCadastroModel.Valor;

            return retorno;
        }

        // POST: Horario/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(PeriodoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<PeriodoCadastroModel, Int64>(pModel);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
            {
                return RedirectToAction("Index", "Periodo");
            }

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {

        }

        // GET: Campanha/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<PeriodoCadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PeriodoCadastroModel pModel)
        {
            return base.DeleteBase<PeriodoCadastroModel, Int64>(pModel);
        }
    }
}