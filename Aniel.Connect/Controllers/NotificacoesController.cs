using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class NotificacoesController : BaseController
    {
        // GET: Bot
        public NotificacoesController() : base("Notificacoes",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Notificacoesapi/FindById?id={0}",
                                                   "api/Notificacoesapi/Gravar",
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
            ViewResult retorno = (ViewResult)base.CadastroBase<Notificacoes_CadastroModel, Int64?>(id);
            return retorno;
        }

        [HttpPost]
        public async Task<ActionResult> Cadastro(Notificacoes_CadastroModel pModel)
        {
            using var notificacoesApi = new NotificacoesApiController();
            var resultado = await notificacoesApi.Gravar(new ParamWebApi<Notificacoes_CadastroModel> { Objeto = pModel, Usuario = User.Identity.GetUserId() });
            return RedirectToAction("Index", "Notificacoes", new { IdNotificacao = resultado.IdObjeto });

        }
    }
}
