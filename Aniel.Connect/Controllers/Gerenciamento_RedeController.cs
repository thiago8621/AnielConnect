using Aniel.Connect.Core;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Gerenciamento_RedeController : BaseController
    {
        public Gerenciamento_RedeController() : base("Gerenciamento_Rede", CONSTANTES.URLPortalInterno)
        {

        }

        // GET: Gerenciamento_Rede
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

    }
}