using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class FranquiaController : BaseController
    {
        public FranquiaController() : base("Franquia",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Franquiaapi/FindById?id={0}",
                                                   "api/Franquiaapi/Gravar",
                                                   String.Empty)
        {

        }

        // GET
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult Cadastro(Int64? id)
        {
            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<FranquiaCadastroModel, Int64?>(id);
            return retorno;
        }

        [HttpPost]
        public ActionResult Cadastro(FranquiaCadastroModel model)
        {
            ActionResult retorno = base.CadastroBase<FranquiaCadastroModel, Int64>(model);
            return retorno;
        }
    }
}