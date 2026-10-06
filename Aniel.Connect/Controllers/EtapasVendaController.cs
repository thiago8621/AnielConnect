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
    public class EtapasVendaController : BaseController
    {
        // GET: EtapasVenda
        public EtapasVendaController() : base("EtapasVenda",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/EtapasVendaapi/FindById?id={0}",
                                                   "api/EtapasVendaapi/Gravar",
                                                   String.Empty)
        {

        }

        // GET: EtapasVenda
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
    }
}
