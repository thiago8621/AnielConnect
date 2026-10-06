using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Customizacao_BotController : BaseController
    {
        public Customizacao_BotController() : base("Customizacao_BotApi",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Customizacao_BotApi/FindById?id={0}",
                                                   "api/Customizacao_BotApi/Gravar",
                                                   String.Empty)
        {

        }

        // GET: Customizacao_BotApi
        public async Task<ActionResult> Index(Int64 IdBot)
        {
            using var botapi = new Customizacao_BotApiController();
            var resultado = await botapi.FindById(IdBot);
            ViewBag.Areas = Areas;
            return View(resultado.Objeto);
        }

        // POST: Customizacao_BotApi/Cadastro/5
        [HttpPost]
        public async Task<ActionResult> Cadastro(Bot_CustomizacaoModel pModel)
        {
            using var botapi = new Customizacao_BotApiController();

            var resultado = await botapi.Gravar(new ParamWebApi<Bot_CustomizacaoModel> { Objeto = pModel, Usuario = User.Identity.GetUserId() });

            return RedirectToAction("Index", "Bot");
        }
    }
}