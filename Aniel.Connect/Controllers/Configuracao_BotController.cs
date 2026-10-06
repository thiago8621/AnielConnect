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
    public class Configuracao_BotController : BaseController
    {
        public Configuracao_BotController() : base("Configuracao_Bot", CONSTANTES.URLPortalInterno)
        {

        }

        // GET: Configuracao_Bot
        public ActionResult Index(Int64 IdBot)
        {
            ViewBag.Areas = Areas;
            return View(new Bot_ConfiguracaoGeralModel{
                Id = IdBot
            });
        }
    }
}