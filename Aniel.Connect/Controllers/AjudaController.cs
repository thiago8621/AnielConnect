using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class AjudaController : Controller
    {
        // GET: Ajuda
        public ActionResult Index()
        {
            return View();
        }

        // GET: Desenvolvimento
        public ActionResult Desenvolvimento()
        {
            return View();
        }
    }
}