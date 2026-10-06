using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class IndicadorController : Controller
    {
        // GET: Indicador
        public ActionResult Index()
        {
            return View();
        }
    }
}