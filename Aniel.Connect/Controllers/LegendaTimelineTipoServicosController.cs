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
    public class LegendaTimelineTipoServicosController : BaseController
    {
        // GET: Legenda
        public LegendaTimelineTipoServicosController() : base("LegendaTimelineTipoServicos",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/LegendaTimelineTipoServicosapi/FindById?id={0}",
                                                   "api/LegendaTimelineTipoServicosapi/Gravar",
                                                   String.Empty)
        {

        }

        // GET: Legenda
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
    }
}
