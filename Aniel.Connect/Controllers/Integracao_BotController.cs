using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Integracao_BotController : BaseController
    {
        public Integracao_BotController() : base("Integracao_Bot",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Botapi/FindById?id={0}",
                                                   "api/Botapi/Gravar",
                                                   String.Empty)
        {

        }

        // GET: Integracao_Bot
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
    }
}