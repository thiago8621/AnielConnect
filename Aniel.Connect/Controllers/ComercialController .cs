using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
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
    public class ComercialController : BaseController
    {
        public ComercialController() : base("Comercial", CONSTANTES.URLPortalInterno)
        {
        }

        // GET: Comercial Index
        public ActionResult Index()
        {
            if (Areas.Find(p => p.Cod_Area == "121") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }
            ViewBag.Areas = Areas;
            return View();
        }

        
    }
}