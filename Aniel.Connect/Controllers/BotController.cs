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
    public class BotController : BaseController
    {
        // GET: Bot
        public BotController() : base("Bot",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Botapi/FindById?id={0}",
                                                   "api/Botapi/Gravar",
                                                   String.Empty)
        {

        }

        // GET: Bot
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Bot_CadastroModel, Int64?>(id);
        }

        // POST: Bot/Cadastro/5
        [HttpPost]
        public async Task<ActionResult> Cadastro(Bot_CadastroModel pModel)
        {
            using var botapi = new BotApiController();
            var resultado = await botapi.Gravar(new ParamWebApi<Bot_CadastroModel> { Objeto = pModel, Usuario = User.Identity.GetUserId() });
            return RedirectToAction("Index", "Configuracao_Bot", new { IdBot = resultado.IdObjeto });

        }
    }
}
