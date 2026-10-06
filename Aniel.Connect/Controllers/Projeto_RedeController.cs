using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aniel.Connect.ControllersWebApi;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Projeto_RedeController : BaseController
    {

        public Projeto_RedeController() : base("Projeto_Rede",
                                         CONSTANTES.URLPortalInterno,
                                         "api/Projeto_RedeApi/GetAll",
                                         "api/Projeto_RedeApi/FindById?Id={0}",
                                         "api/Projeto_RedeApi/Gravar",
                                         "api/Projeto_RedeApi/Excluir"
                                         )
        {

        }
        // GET: Projeto_Rede
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
        public ActionResult Mapeamento_Rede() 
        { 
            return View();
        }
    }
}