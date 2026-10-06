using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class WebhookController : BaseController
    {

        public WebhookController() : base("Webhook", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Webhookapi/FindById?id={0}",
                                                   "api/Webhookapi/Gravar",
                                                   "api/Webhookapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Listagem Webhook
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<WebHookCadastroModel, Int64?>(id);
            ViewBag.Areas = Areas;
            return retorno;
        }

        // POST: WebHook/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(WebHookCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<WebHookCadastroModel, Int64>(pModel);
            ViewBag.Areas = Areas;
            return retorno;
        }

       




    }
}