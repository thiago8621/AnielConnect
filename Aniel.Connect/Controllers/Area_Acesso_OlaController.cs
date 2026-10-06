using Aniel.Connect.Core;
using System;
using System.Web.Mvc;


namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Area_Acesso_OlaController : BaseController
    {
        public Area_Acesso_OlaController() : base("Area_Acesso_Ola",
                                                CONSTANTES.URLPortalInterno)

        {

        }
       
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }            

    }
}