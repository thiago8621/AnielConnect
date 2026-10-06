using Aniel.Connect.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Aniel_NoSession]
    [Authorize]
    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Login()
        {
            return View();
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Erro404()
        {
            return View();
        }

        public ActionResult Erro500()
        {
            return View();
        }

        public ActionResult ErroSession()
        {
            return View();
        }
        public ActionResult ErroSemAcesso()
        {
            return View();
        }
    }
}