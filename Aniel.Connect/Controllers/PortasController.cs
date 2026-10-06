using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Controllers;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class PortasController: BaseController
    {
        public PortasController(): base("Consulta_Portas",CONSTANTES.URLPortalInterno) { }
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
    
    }
}