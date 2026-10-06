using Aniel.Connect.Core;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Plano_ProdutoController : BaseController
    {
        public Plano_ProdutoController() : base("Plano_Produto",
                                                 CONSTANTES.URLPortalInterno)
        {

        }
        // GET: Plano_Produto
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
    }
}