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
    public class RotaTelefoniaController : BaseController
    {
        public RotaTelefoniaController() : base("RotaTelefonia",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/RotaTelefoniaapi/FindById?id={0}",
                                                   "api/RotaTelefoniaapi/Gravar",
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
            ViewResult retorno = (ViewResult)base.CadastroBase<RotaTelefoniaCadastroModel, Int64?>(id);
            return retorno;
        }

        [HttpPost]
        public ActionResult Cadastro(RotaTelefoniaCadastroModel model)
        {
            ActionResult retorno = base.CadastroBase<RotaTelefoniaCadastroModel, Int64>(model);
            return retorno;
        }
    }
}