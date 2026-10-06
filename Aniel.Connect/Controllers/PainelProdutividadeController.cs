using Aniel.Connect.Core;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class PainelProdutividadeController : BaseController
    {
        public PainelProdutividadeController() : base("PainelProdutividade",
                                                      CONSTANTES.URLPortalInterno,
                                                      "api/PainelProdutividadeApi/GetAll",
                                                      String.Empty,
                                                      String.Empty,
                                                      String.Empty){}

        // GET: PainelProdutividade
        public ActionResult Index()
        {
            return View();
        }
    }
}