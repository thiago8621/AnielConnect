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
    public class TecnologiaController : BaseController
    {

        public TecnologiaController() : base("Tecnologia",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Tecnologiaapi/FindById?id={0}",
                                                   "api/Tecnologiaapi/Gravar",
                                                   "api/Tecnologiapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Tecnologia
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Tecnologia_CadastroModel, Int64?>(id);

            return retorno;
        }

        // POST: Tecnologia/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Tecnologia_CadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Tecnologia_CadastroModel, Int64>(pModel);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
            {
                return RedirectToAction("Index", "Tecnologia");
            }

            return retorno;
        }

        // GET: Tecnologia/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Tecnologia_CadastroModel, Int64>(id.Value);
        }

        // POST: Tecnologia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Tecnologia_CadastroModel pModel)
        {
            return base.DeleteBase<Tecnologia_CadastroModel, Int64>(pModel);
        }
    }
}