using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Mapa_Analise_ComercialController : BaseController

    {
        public Mapa_Analise_ComercialController() : base("Mapa_Analise_Comercial",
                                                         CONSTANTES.URLPortalInterno,
                                                         "api/Mapa_Analise_ComercialApi/GetAll",
                                                         "api/Mapa_Analise_ComercialApi/FindById?Id={0}",
                                                          String.Empty,
                                                          String.Empty)
        {


        }

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
          
            
            return View();
        }

       
    }
}
