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
    public class HorarioController : BaseController
    {

        public HorarioController() : base("Horario",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Horarioapi/FindById?id={0}",
                                                   "api/Horarioapi/Gravar",
                                                   "api/Horarioapi/Excluir",
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
            ViewResult retorno = (ViewResult)base.CadastroBase<Horario_CadastroModel, Int64?>(id);

            //if (!id.HasValue)
            //    ((CampanhaCadastroModel)retorno.Model).TipoValor = TipoValor_CampanhaCadastroModel.Valor;

            return retorno;
        }

        // POST: Horario/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Horario_CadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Horario_CadastroModel, Int64>(pModel);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
            {
                return RedirectToAction("Index", "Horario");
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

            return base.DeleteBase<Horario_CadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Horario_CadastroModel pModel)
        {
            return base.DeleteBase<Horario_CadastroModel, Int64>(pModel);
        }
    }
}